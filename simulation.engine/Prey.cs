using engine.Common;
using engine.Common.Entities;
using Learning;
using System;
using System.Collections.Generic;
using System.Text;

namespace simulation.engine
{
    public class Prey : AIBase
    {
        public Prey(Prey parent = null) : base(parent)
        {
            Type = AIType.Prey;
            Color = Yellow;
            Damage = -1 * (HealthMax / 4f); // 4 hits

            // prey can see all around, but not that far
            ViewportAngles = CachedViewportAngles;
            ViewportDistance = Width * 7;

            // energy (shrinks)
            EnergyCost = -1 * (EnergyMax/500); // every 500 updates

            // digestion (not used)
            DigestionCost = 0;

            // reproduction (shrinks)
            ReproduceCost = -1 * ((ReproduceMax/300) + 1); // every 300 updates

            // attack (shrinks)
            AttackCost = -1 * ((AttackMax / 5)); // every 5 updates

            // initialize models
            InitializeModels(parent, new int[] { 16 });
        }

        public override void Update()
        {
            // gain energy if standing still
            if (PreviousEnergyCost == 0)
            {
                EnergyMeter += Math.Abs(EnergyCost/2);
            }

            // adjust reproduction (reproduce overtime)
            ReproduceMeter += ReproduceCost;

            base.Update();
        }

        public void Attack(Predator predator)
        {
            // attack the predator
            if (AttackMeter <= 0)
            {
                // apply damage
                predator.Health += Damage;

                // reset
                AttackMeter = AttackMax;
            }
        }

        #region private
        private static readonly float[] CachedViewportAngles = new float[] { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };
        private static readonly RGBA Yellow = new RGBA() { R = 233, G = 235, B = 39, A = 255 };
        #endregion
    }
}
