using engine.Common;
using engine.Common.Entities;
using Learning;
using System;
using System.Collections.Generic;
using System.Text;

namespace simulation.engine
{
    public class Predator : AIBase
    {
        public Predator(Predator parent = null) : base(parent)
        {
            Type = AIType.Predator;
            Color = Blue;
            Damage = -1 * (AttackMax / 2f); // 2 hits (unless IsLethal)

            // energy (shrinks) (max 1000)
            EnergyCost = -1 * ((EnergyMax - Math.Abs(EnergyFixedCost * 300)) / 300); // (including energy fixed cost) every 300 updates

            // digestion (shrinks)
            DigestionCost = -1* (DigestionMax/10); // every 10 updates

            // reproduction (shrinks) (max 1000)
            ReproduceCost = -1 * ((ReproduceMax / 3) + 1); // after 3 prey eaten  (eg. takes ~30 updates with digestion)

            // viewport - can only see out infront, but further
            ViewportAngles = CachedViewportAngles;
            ViewportDistance = Width * 17;

            // attack (shrinks)
            AttackCost = -1 * ((AttackMax / 5)); // every 5 updates

            // initialize models
            InitializeModels(parent, new int[] { 12 });
        }

        // fixed cost to energy
        public const int EnergyFixedCost = -1;
        public const int EnergyMultiplyWhenFull = 3;

        public bool IsLethal { get; set; }

        public override void Update()
        {
            // reduce energy by fixed energy cost
            EnergyMeter += EnergyFixedCost;

            // predators die when they run out of energy
            if (EnergyMeter <= 0)
            {
                if (OnDeath != null) OnDeath(this);
            }

            // digestion
            DigestionMeter += DigestionCost;
            if (DigestionMeter < 0) DigestionMeter = 0;
            if (DigestionMeter > DigestionMax) DigestionMeter = DigestionMax;

            base.Update();
        }

        public void Eat(AIBase ai)
        {
            if (AttackMeter <= 0)
            {
                // apply damage
                ai.Health += Damage;

                // check if dead
                if (IsLethal || ai.Health <= 0)
                {
                    // set health to 0
                    ai.Health = 0;

                    // notify
                    if (OnDeath != null) OnDeath(ai);

                    // add energy (equal to n rounds of digesting)
                    var digestionDuration = Math.Abs(DigestionMax / DigestionCost) * EnergyMultiplyWhenFull;
                    EnergyMeter += Math.Abs((EnergyCost + EnergyFixedCost) * digestionDuration);
                    if (EnergyMeter > EnergyMax) EnergyMeter = EnergyMax;

                    // if not digesting, then add to reproduction
                    if (DigestionMeter <= 0)
                    {
                        // start potential reproduction
                        ReproduceMeter += ReproduceCost;

                        // restart digesting
                        DigestionMeter = DigestionMax;
                    }
                }

                // reset
                AttackMeter = AttackMax;
            }
        }

        #region private
        private static readonly float[] CachedViewportAngles = new float[] { -22f, -11f, 0f, 11f, 22f };
        private static readonly RGBA Blue = new RGBA() { R = 39, G = 23, B = 219, A = 255 };
        #endregion
    }
}
