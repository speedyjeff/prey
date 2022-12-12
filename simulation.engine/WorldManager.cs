using engine.Common;
using engine.Common.Entities;
using engine.Common.Entities.AI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace simulation.engine
{
    public class WorldManager
    {
        // setup options
        public static WorldManagerOptions CreateStartupOptions(bool isSmall = true)
        {
            // small options
            if (isSmall)
                return new WorldManagerOptions()
                {
                    Width = 6400,
                    Height = 6400,

                    // predators
                    InitialPredatorCount = 50,
                    MaxPredators = 170,

                    // prey
                    InitialPreyCount = 300,
                    MaxPrey = 800,

                    // config
                    Combat = AIType.Predator
                };

            // large options
            return new WorldManagerOptions()
            {
                Width = 16000,
                Height = 16000,

                // predators
                InitialPredatorCount = 800,
                MaxPredators = 3000,

                // prey
                InitialPreyCount = 4800,
                MaxPrey = 13000,

                // config
                Combat = (AIType.Predator | AIType.Prey),
            };
        }

        public static WorldManager Create(WorldManagerOptions options)
        {
            // create the world manager
            var manager = new WorldManager()
            {
                MaxPrey = options.MaxPrey,
                MaxPredators = options.MaxPredators,
                Combat = options.Combat,
                FullRandom = options.FullRandom,
                StartingPredator = options.StartingPredator,
                StartingPrey = options.StartingPrey
            };

            // put the oberver in the middle (birds eye view, and controller of all ai
            manager.Camera = new Camera() { X = options.Width / 2, Y = options.Height / 2 };

            // create the HUD
            manager.HUD = new Hud();

            // create world
            manager.World = new World(
              new WorldConfiguration()
              {
                  Width = options.Width,
                  Height = options.Height,
                  EnableZoom = true,
                  ShowCoordinates = false,
                  ForcesApplied = (int)Forces.Z,
                  HUD = manager.HUD,
                  StartMenu = (options.StartPaused) ? new Menu() : null,
                  EnableFastPlayerUpdate = options.EnableFastPlayerUpdate,
                  ServerUrl = null // not compatible with remote execution
              },
              new Player[] { manager.Camera },
              MakeBorders(options.Width, options.Height, WallThickness),
              new Background(options.Width, options.Height) { GroundColor = new RGBA { R = 125, G = 125, B = 125, A = 255 } }
            );

            // add the initial populations
            for (int i = 0; i < options.InitialPredatorCount; i++) manager.Add(AIType.Predator);
            for (int i = 0; i < options.InitialPreyCount; i++) manager.Add(AIType.Prey);

            // setup callbacks
            manager.World.OnBeforeKeyPressed += manager.WorldOnBeforeKeyPressed;
            manager.World.OnBeforeMousedown += manager.WorldOnBeforeMousedown;
            manager.World.OnContact += manager.WorldOnTouch;
            manager.World.OnDeath += manager.WorldOnDeath;
            AIBase.OnBirth += manager.AIBaseOnBirth;
            AIBase.OnDeath += manager.AIBaseOnDeath;

            return manager;
        }

        public World World { get; private set; }

        public int MaxPredators { get; private set; }
        public int MaxPrey { get; private set; }

        public int AlivePredators { get { return AlivePredatorCount; } }
        public int AlivePrey { get { return AlivePreyCount; } }

        public Predator AncestorPredator { get; set; }
        public Prey AncestorPrey { get; set; }

        public long AveragePredatorLifetime { get { return (PredatorLifetimeCount > 0 ? PredatorLifetimeTotal / PredatorLifetimeCount : 0); } }
        public long AveragePreyLifetime { get { return (PreyLifetimeCount > 0 ? PreyLifetimeTotal / PreyLifetimeCount : 0); } }

        public long Lifetime { get { return Camera.Lifetime; } }

        public void Add(AIType type)
        {
            // choose a random place to put the ai
            var x = WallThickness + (GetRandom() % (World.Width - (2 * WallThickness) - PlayerWidth));
            var y = WallThickness + (GetRandom() % (World.Height - (2 * WallThickness) - PlayerWidth));

            // choose parent (small chance to not use the ancestor or starting ai)
            var parent = (type == AIType.Predator) ? (AIBase)AncestorPredator : (AIBase)AncestorPrey;
            if (parent == null || GetRandom() % 100 < 25) parent = (type == AIType.Predator) ? StartingPredator : StartingPrey;
            if (GetRandom() % 100 < 10) parent = null;

            // add ai
            Reproduce(type, x, y, parent);
        }

        #region private
        private static RandomNumberGenerator Random;
        private Hud HUD;
        private Camera Camera;
        private AIType Combat;
        private AIType FullRandom;
        private volatile int AlivePredatorCount;
        private volatile int AlivePreyCount;
        private long PredatorLifetimeTotal;
        private long PredatorLifetimeCount;
        private long PreyLifetimeTotal;
        private long PreyLifetimeCount;
        private AIBase StartingPredator;
        private AIBase StartingPrey;

        private const int WallThickness = 20;
        private const int PlayerWidth = 50;

        static WorldManager()
        {
            Random = RandomNumberGenerator.Create();
        }

        //
        // Utility
        //
        private static int GetRandom()
        {
            var int32buffer = new byte[4];
            Random.GetNonZeroBytes(int32buffer);
            // ensure positive
            int32buffer[3] &= 0x7f;
            return BitConverter.ToInt32(int32buffer);
        }

        private static Element[] MakeBorders(float width, float height, float thickness)
        {
            // make the boundaries just out of the area
            return new Element[]
            {
                // top
                new Wall(WallDirection.Horiztonal, width, thickness) { X = width / 2, Y = -1 * thickness/2, Z=Constants.Sky, Depth = Constants.Sky*2 },
                // left
                new Wall(WallDirection.Vertical, height, thickness) { X = -1 * thickness/2, Y = height / 2, Z=Constants.Sky, Depth = Constants.Sky*2 },
                // bottom
                new Wall(WallDirection.Horiztonal, width, thickness) { X = width / 2, Y = height + thickness/2, Z=Constants.Sky, Depth = Constants.Sky*2 },
                // right
                new Wall(WallDirection.Vertical, height, thickness) { X = width + thickness/2, Y = height / 2, Z=Constants.Sky, Depth = Constants.Sky*2 }
            };
        }

        //
        // logic
        //
        private void Reproduce(AIType type, float x, float y, AIBase parent = null)
        {
            // check if we are at our maximums
            if (type == AIType.Predator && AlivePredatorCount >= MaxPredators) return;
            else if (type == AIType.Prey && AlivePreyCount >= MaxPrey) return;

            // duplicate the ai (inherit details from the parents)
            AIBase child;
            if (type == AIType.Predator)
            {
                child = new Predator(parent as Predator)
                { 
                    X = x, Y = y, Z = Constants.Ground + 1,
                    IsLethal = Combat == AIType.Predator, // only predators, so lethal damage
                    IsRandomMovement = (FullRandom & AIType.Predator) == AIType.Predator
                };
                System.Threading.Interlocked.Increment(ref AlivePredatorCount);
            }
            else if (type == AIType.Prey)
            {
                child = new Prey(parent as Prey) 
                { 
                    X = x, Y = y, Z = Constants.Ground + 1,
                    IsRandomMovement = (FullRandom & AIType.Prey) == AIType.Prey
                };
                System.Threading.Interlocked.Increment(ref AlivePreyCount);
            }
            else throw new Exception($"unknown type {parent.Type}");

            // update HUD
            HUD.AlivePredators = AlivePredatorCount;
            HUD.AlivePrey = AlivePreyCount;

            // add to world
            World.AddItem(child);
        }

        private void Dead(AIBase ai)
        {
            // update counts
            if (ai is Predator predator)
            {
                System.Threading.Interlocked.Decrement(ref AlivePredatorCount);

                // track lifetime
                PredatorLifetimeCount++;
                PredatorLifetimeTotal += (long)predator.Lifetime;

                // keep as the strongest
                if (AncestorPredator == null) AncestorPredator = predator;
                else if (predator.Lifetime >= AncestorPredator.Lifetime) AncestorPredator = predator;
            }
            else if (ai is Prey prey)
            {
                System.Threading.Interlocked.Decrement(ref AlivePreyCount);

                // track lifetime
                PreyLifetimeCount++;
                PreyLifetimeTotal += (long)prey.Lifetime;

                // keep as the strongest
                if (AncestorPrey == null) AncestorPrey = prey;
                else if (prey.Lifetime >= AncestorPrey.Lifetime) AncestorPrey = prey;
            }
            else throw new Exception($"unknown type {ai.Type}");

            // update HUD
            HUD.AlivePredators = AlivePredatorCount;
            HUD.AlivePrey = AlivePreyCount;

            // debug
            World.AddItem(new OnScreenText() { Text = $"{ai.Type} dead gen:{ai.Generation} lifetime:{ai.Lifetime}" });

            // remove from the world
            World.RemoveItem(ai);
        }

        private void Eating(Predator predator, Prey prey)
        {
            // check config
            if ((Combat & AIType.Predator) != AIType.Predator) return;

            // eat the prey
            predator.Eat(prey);
        }

        private void Attacking(Prey prey, Predator predator)
        {
            // check config
            if ((Combat & AIType.Prey) != AIType.Prey) return;

            // attack the predator
            prey.Attack(predator);
        }

        // 
        // callbacks
        //
        private bool WorldOnBeforeMousedown(Element elem, MouseButton btn, float sx, float sy, float wx, float wy, float wz, ref char key)
        {
            if (btn == MouseButton.Left)
            {
                // highlight/unhighlight
                if (elem != null && elem is AIBase ai) HUD.Highlight(ai);
                else HUD.UnHighlight();

                // handled
                return true;
            }

            // continue
            return false;
        }
    
        private void WorldOnTouch(Element elem1, Element elem2)
        {
            // determine valid attacks
            if (elem1 is Predator predator1 && elem2 is Prey prey1) Eating(predator1, prey1);
            else if (elem1 is Prey prey2 && elem2 is Predator predator2) Attacking(prey2, predator2);
        }

        private void WorldOnDeath(Element elem)
        {
            if (elem is AIBase ai) Dead(ai);
        }

        private void AIBaseOnBirth(AIBase ai)
        {
            Reproduce(ai.Type, ai.X, ai.Y, parent: ai);
        }

        private void AIBaseOnDeath(AIBase ai)
        {
            Dead(ai);
        }

        private bool WorldOnBeforeKeyPressed(Player player, ref char key)
        {
            // debug
            // add predators and prey
            if (key == '1' || key == '2')
            {
                Add(key == '1' ? AIType.Predator : AIType.Prey);

                // handled
                return true;
            }

            // discard the attack action (avoid the sound playing)
            if (key == Constants.LeftMouse) return true;

            // not handled
            return false;
        }
        #endregion
    }
}
