using engine.Common;
using engine.Common.Entities;
using engine.Common.Entities.AI;
using Learning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// inspiration from https://www.youtube.com/watch?v=qwrp3lB-jkQ

/*
Game parameters:

Map 128 cells x 128 cells (6,400 x 6,400 pixels)

Preadators
 energy depletes over time
   movement consumes energy
   gain energy by eating prey
 eating requires digestion
   reproduce after eating 2.1 prey
 if no energy, then death
 field of view is long and narrow (80 cells) (850 pixels)
 max 170


Prey
 energy depletes over time
   movement consumes energy
   gain energy by staying still
 reproduce based on time alive (6 * 10) (~6 sec)
 if no energy, cannot move
 field of view is wide and short (40 cells) (350 pixels)
 max 800
*/

/*
Map 512 x 512 cells (25,600 x 25,600 pixels)

Predator 
 kill prey in 2 hits
 max 3,000

Prey
 kill predator in 4 hits
 max 13,000

*/

namespace simulation.engine
{
    public abstract class AIBase : AI
    {
        // attributes for all ais
        public AIType Type;
        public float Lifetime { get; private set; }
        public int Generation { get; private set; }
        public float Damage { get; protected set; }

        // view port angles
        public float[] ViewportAngles { get; protected set; }  // degrees from angle (eg. -15, 15)
        public float ViewportDistance { get; protected set; }
        public bool ShowViewPort { get; set; }

        // per update cost to energy (proportional with effort)
        public int EnergyCost { get; protected set; }
        // per update cost to digestion
        public int DigestionCost { get; protected set; }
        // per update cost to reproduce
        public int ReproduceCost { get; protected set; }
        // per update cost to attack
        public int AttackCost { get; protected set; }

        // starting/max energy meter
        public const int EnergyMax = 1000;
        // starting/max digestion meter
        public const int DigestionMax = 1000;
        // starting/max reproduce meter
        public const int ReproduceMax = 1000;
        // starting/max attack meter
        public const int AttackMax = 1000;
        // starting/max health
        public const float HealthMax = 1000;

        // core meters
        public int EnergyMeter { get; protected set; }
        public int DigestionMeter { get; protected set; }
        public int ReproduceMeter { get; protected set; }
        public int AttackMeter { get; protected set; }

        // customization
        public bool IsRandomMovement { get; set; }

        public AIBase(AIBase parent = null)
        {
            // base class updates
            Width = Height = 50;
            Health = HealthMax;
            Shield = 0;
            Speed = Constants.Speed;
            Generation = (parent != null) ? parent.Generation + 1 : 0;
            ShowViewPort = false;

            // todo take in parameters to due the cost of each of the metrics

            // init
            PreviousEnergyCost = 0;

            // starting values for core attributes (have different starting set - helps to overset every ai updating at once)
            EnergyMeter = EnergyMax;  // always start full
            DigestionMeter = DigestionMax - (GetRandom() % (DigestionMax/2));
            ReproduceMeter = ReproduceMax - (GetRandom() % (ReproduceMax / 2));
            AttackMeter = AttackMax - (GetRandom() % (AttackMax / 2));
        }

        public static Action<AIBase> OnDeath { get; set; }
        public static Action<AIBase> OnBirth { get; set; }

        public override void Draw(IGraphics g)
        {
            if (ShowViewPort)
            {
                // draw the view port
                for (int i = 0; i < ViewportAngles.Length; i++)
                {
                    Collision.CalculateLineByAngle(X, Y, Angle + ViewportAngles[i], distance: ViewportDistance, out float x1, out float y1, out float x2, out float y2);
                    g.Line(RGBA.White, x1, y1, x2, y2, thickness: 2);
                }
            }

            // draw body
            g.Ellipse(Color, X - Width / 2, Y - Height / 2, Width, Height, fill: true, border: true, thickness: 1);

            // only show the additional detail if close enough
            if (g.LevelOfDetail < 0.25)
            {
                // draw eyeballs
                for (int d = -20; d <= 20; d += 40)
                {
                    // calculate location for eyeball
                    Collision.CalculateLineByAngle(X, Y, Angle + d, (Width / 2) - 10, out float x1, out float y1, out float x2, out float y2);

                    // draw eyeball
                    g.Ellipse(RGBA.White, x2 - (Width / 6), y2 - (Width / 6), Width / 3, Height / 3, fill: true, border: true, thickness: 3);
                }
            }
        }

        public override void Update()
        {
            // increase lifetime counter
            Lifetime++;

            // adjust Energy
            EnergyMeter += PreviousEnergyCost;
            PreviousEnergyCost = 0;
            if (EnergyMeter < 0) EnergyMeter = 0;
            if (EnergyMeter > EnergyMax) EnergyMeter = EnergyMax;

            // adjust attack
            AttackMeter += AttackCost;
            if (AttackMeter < 0) AttackMeter = 0;
            if (AttackMeter > AttackMax) AttackMeter = AttackMax;

            // health
            if (Health <= 0)
            {
                // die
                if (OnDeath != null) OnDeath(this);
                return;
            }

            // reproduction
            if (ReproduceMeter > ReproduceMax) ReproduceMeter = ReproduceMax;
            if (ReproduceMeter <= 0)
            {
                // reproduce
                if (OnBirth != null) OnBirth(this);

                // reset
                ReproduceMeter = ReproduceMax;
            }
        }

        public override ActionEnum Action(List<Element> elements, float angleToCenter, bool inZone, ref float xdelta, ref float ydelta, ref float zdelta, ref float angle)
        {
            // if there is energy, determine the next move
            PreviousEnergyCost = 0;
            if (EnergyMeter > 0 && Health > 0)
            {
                // check if completely random
                var angleIndex = 0;
                var speedIndex = 0;
                if (IsRandomMovement)
                {
                    angleIndex = GetRandom() % AngleDeltas.Length;
                    speedIndex = GetRandom() % SpeedMultipliers.Length;
                }
                else
                {
                    // generate information for use by the model
                    var viewport = Viewport(elements);

                    //
                    // Angle
                    //
                    var angleOutput = NetworkAngle.Evaluate(viewport);

                    // choose an angle
                    angleIndex = !UseRandomSample() ? angleOutput.Result : GetRandom() % AngleDeltas.Length;

                    // reinforce this action
                    NetworkAngle.Learn(angleOutput, angleIndex);

                    //
                    // Speed
                    //
                    var speedOutput = NetworkSpeed.Evaluate(viewport);

                    // choose speed
                    speedIndex = !UseRandomSample() ? speedOutput.Result : GetRandom() % SpeedMultipliers.Length;

                    // reinforce this action
                    NetworkSpeed.Learn(speedOutput, speedIndex);
                }

                // determine an angle
                angle = Angle + AngleDeltas[angleIndex];

                // set speed
                Speed = Constants.Speed * SpeedMultipliers[speedIndex];

                if (Speed > 0)
                {
                    //  determine the direction
                    Collision.CalculateLineByAngle(X, Y, angle, distance: 0.5f, out float x1, out float y1, out float x2, out float y2);

                    // convert to deltas
                    xdelta = x2 - X;
                    ydelta = y2 - Y;
                    zdelta = 0;
                }

                // calculate the energy cost
                PreviousEnergyCost = EnergyCost * SpeedMultipliers[speedIndex];
            }

            // return action
            if (PreviousEnergyCost == 0)
            {
                // no movement
                // when out of energy it is handled in the derived class
                xdelta = ydelta = zdelta = 0;
                angle = Angle;
                return ActionEnum.None;
            }
            else return ActionEnum.Move;
        }

        // result
        //  float array of 2 values X number of angles
        //    0 - [0..1] percent distance to object (1 being close)
        //    1 - 0:nothing, 1:opposite type, 0:same type, -1:obstacle
        public float[] Viewport(List<Element> elements)
        {
            // create the cache which is reused for these calls
            if (ViewportResultCache == null) ViewportResultCache = new float[ViewportAngles.Length * 2];

            // trace rays to identify closest elements
            for (int i = 0; i < ViewportAngles.Length; i++)
            {
                // clear
                ViewportResultCache[(i * 2)] = ViewportResultCache[(i * 2) + 1] = 0f;

                // calcualte the line that represents the trajectory
                Collision.CalculateLineByAngle(X, Y, Angle + ViewportAngles[i], ViewportDistance, out float x1, out float y1, out float x2, out float y2);

                // check all elements
                var min = Single.MaxValue;
                foreach (var elem in elements)
                {
                    if (elem.Id == Id || elem.IsDead) continue;

                    // check if this element is closest
                    var tdistance = Collision.LineIntersectingRectangle(
                        x1, y1, x2, y2, // line
                        elem.X, elem.Y, elem.Width, elem.Height // element
                        );

                    if (tdistance > ViewportDistance + 1f) throw new Exception("invalid distance");

                    // check if this is the closest collision
                    if (tdistance > 0f && tdistance < min)
                    {
                        // capture how close (closer is larger)
                        ViewportResultCache[(i * 2)] = 1 - (tdistance / ViewportDistance);

                        // capture type
                        if (elem is AIBase ai) ViewportResultCache[(i * 2) + 1] = (ai.Type == Type) ? 0f : 1f;
                        else if (elem is Wall) ViewportResultCache[(i * 2) + 1] = -1;

                        // set min
                        min = tdistance;
                    }
                } // foreach element
            } // for viewport angles

            return ViewportResultCache;
        }

        public void Evolve(float pcntEnergyCost, float pcntDigestCost, float pcntReproduceCost, float pcntAttackCost)
        {
            // evolve each of the costs per the percentages
            if (pcntEnergyCost >= 0 && pcntEnergyCost < MaxPcntEvolution)
            {
                EnergyCost = (int)Math.Floor(EnergyCost * pcntEnergyCost);
            }
            if (pcntDigestCost >= 0 && pcntDigestCost < MaxPcntEvolution)
            {
                DigestionCost = (int)Math.Floor(DigestionCost * pcntDigestCost);
            }
            if (pcntReproduceCost >= 0 && pcntReproduceCost < MaxPcntEvolution)
            {
                ReproduceCost = (int)Math.Floor(ReproduceCost * pcntReproduceCost);
            }
            if (pcntAttackCost >= 0 && pcntAttackCost < MaxPcntEvolution)
            {
                AttackCost = (int)Math.Floor(AttackCost * pcntAttackCost);
            }
        }

        #region private
        private float[] ViewportResultCache;
        private static RandomNumberGenerator Random;

        private const float MaxPcntEvolution = 2f;

        static AIBase()
        {
            Random = RandomNumberGenerator.Create();
        }

        private unsafe static int GetRandom()
        {
            var array = stackalloc byte[4];
            var int32buffer = new Span<byte>(array, 4);
            Random.GetNonZeroBytes(int32buffer);
            // ensure positive
            int32buffer[3] &= 0x7f;
            return BitConverter.ToInt32(int32buffer);
        }

        private bool UseRandomSample()
        {
            // start off more random and reduce over time
            if (Lifetime < 100) return GetRandom() % 100 < 50;
            else if (Lifetime < 500) return GetRandom() % 100 < 30;
            else return GetRandom() % 100 < 10;

        }
        #endregion

        #region protected
        protected NeuralNetwork NetworkAngle;
        protected NeuralNetwork NetworkSpeed;
        protected int PreviousEnergyCost;

        protected readonly float[] AngleDeltas = new float[] { -22f, -11f, 0f, 11f, 22f };
        protected readonly int[] SpeedMultipliers = new int[] { 0, 1, 2 };

        protected void InitializeModels(AIBase parent, int[] hiddenLayers)
        {
            if (parent != null)
            {
                // inherit the network from the parent
                NetworkAngle = NeuralNetwork.Load(parent.NetworkAngle);
                NetworkSpeed = NeuralNetwork.Load(parent.NetworkSpeed);

                // inherit costs from parent
                EnergyCost = parent.EnergyCost;
                DigestionCost = parent.DigestionCost;
                AttackCost = parent.AttackCost;
                ReproduceCost = parent.ReproduceCost;
            }
            else
            {
                // network for determining speed
                NetworkAngle = new NeuralNetwork(
                    new NeuralOptions()
                    {
                        InputNumber = ViewportAngles.Length * 2,
                        OutputNumber = AngleDeltas.Length,
                        HiddenLayerNumber = hiddenLayers,
                        LearningRate = 0.0015f,
                        MinibatchCount = 1,
                        ParallizeExecution = false
                    });

                // network for speed
                NetworkSpeed = new NeuralNetwork(
                    new NeuralOptions()
                    {
                        InputNumber = ViewportAngles.Length * 2,
                        OutputNumber = SpeedMultipliers.Length,
                        HiddenLayerNumber = hiddenLayers,
                        LearningRate = 0.0015f,
                        MinibatchCount = 1,
                        ParallizeExecution = false
                    });
            }
        }
        #endregion
    }
}
