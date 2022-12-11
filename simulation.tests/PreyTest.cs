using engine.Common.Entities;
using engine.Common;
using simulation.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulation.tests
{
    internal class PreyTest
    {
        // basic stats
        public static void Stats()
        {
            var prey = new Prey();

            // check stats
            if (prey.EnergyMeter != AIBase.EnergyMax) throw new Exception("incorrect energy");
            if (prey.Health != AIBase.HealthMax) throw new Exception("incorrect health");
            if (prey.DigestionCost != 0) throw new Exception("incorrect digestion");
            var startingReproduce = prey.ReproduceMeter;
            var startingDigestion = prey.DigestionMeter;

            // update a few times and check the meters
            var updates = 0;
            var energy = 0;
            ActionEnum action;
            do
            {
                // take action
                var xdelta = 0f;
                var ydelta = 0f;
                var zdelta = 0f;
                var angle = 0f;
                action = prey.Action(
                   elements: new List<Element>(),
                   angleToCenter: 0,
                   inZone: false,
                   ref xdelta,
                   ref ydelta,
                   ref zdelta,
                   ref angle);

                // track updates
                updates++;

                // update
                prey.Update();

                // predict energy usage
                if (prey.Speed == 0) { }
                else if (prey.Speed == Constants.Speed) energy += prey.EnergyCost;
                else if (prey.Speed == Constants.Speed * 2) energy += (2 * prey.EnergyCost);
                else throw new Exception("unknown speed");
            }
            while (action != ActionEnum.Move);

            // stay the same
            if (prey.Health != AIBase.HealthMax) throw new Exception("invalid health");

            // changes
            if (prey.EnergyMeter != (AIBase.EnergyMax + energy)) throw new Exception("invalid energy");
            if (prey.DigestionMeter != startingDigestion) throw new Exception("invalid digestion");
            if (prey.ReproduceMeter != startingReproduce + (updates * prey.ReproduceCost)) throw new Exception("invalid reproduce");
        }

        // reproduction
        public static void Reproduce()
        {
            var prey = new Prey();

            // reset
            OnBirthCount = OnDeathCount = 0;
            AIBase.OnBirth += PreyOnBirth;
            AIBase.OnDeath += PreyOnDeath;

            // reproduce over time
            if (prey.Health != AIBase.HealthMax) throw new Exception("incorrect health");
            var startingReproduce = prey.ReproduceMeter;
            var startingDigestion = prey.DigestionMeter;

            // eat until they reproduce
            for (int i = Math.Abs(startingReproduce / prey.ReproduceCost); i >= 0; i--)
            {
                prey.Update();

                // check
                if (OnDeathCount != 0) throw new Exception("invalid ondeath");
                if (i == 0)
                {
                    if (OnBirthCount != 1) throw new Exception("invalid onbirth");
                }
                else if (OnBirthCount != 0) throw new Exception("invalid onbirth");
            }

            if (prey.ReproduceMeter != AIBase.ReproduceMax) throw new Exception("invalid reproduce");

            // reset
            OnBirthCount = OnDeathCount = 0;
            AIBase.OnBirth -= PreyOnBirth;
            AIBase.OnDeath -= PreyOnDeath;
        }

        // attack
        public static void Attack()
        {
            var prey = new Prey();
            var predator = new Predator();

            // reset
            OnBirthCount = OnDeathCount = 0;
            AIBase.OnBirth += PreyOnBirth;
            AIBase.OnDeath += PreyOnDeath;

            // attack
            if (prey.Health != AIBase.HealthMax) throw new Exception("incorrect health");
            if (predator.Health != AIBase.HealthMax) throw new Exception("incorrect health");
            var startingReproduce = predator.ReproduceMeter;
            var startingDigestion = predator.DigestionMeter;

            // eat until they reproduce
            for (int i = (int)Math.Abs(AIBase.HealthMax / prey.Damage); i > 0; i--)
            {
                // wait until can attack
                while (prey.AttackMeter > 0) prey.Update();

                var before = predator.Health;

                // attack
                prey.Attack(predator);

                // check
                if (prey.Health != AIBase.HealthMax) throw new Exception("invalid health");
                if (predator.Health != (before + prey.Damage)) throw new Exception("invalid health");
                if (OnBirthCount != 0) throw new Exception("invalid onbirth");
                if (i == 1)
                {
                    if (OnDeathCount != 1) throw new Exception("invalid ondeath");

                }
                else
                {
                    if (OnDeathCount != 0) throw new Exception("invalid ondeath");
                }
            }

            // reset
            OnBirthCount = OnDeathCount = 0;
            AIBase.OnBirth -= PreyOnBirth;
            AIBase.OnDeath -= PreyOnDeath;
        }

        #region private
        // utility
        private static int OnBirthCount = 0;
        private static int OnDeathCount = 0;
        private static void PreyOnBirth(AIBase ai) { OnBirthCount++; }
        private static void PreyOnDeath(AIBase ai) { OnDeathCount++; }
        #endregion
    }
}
