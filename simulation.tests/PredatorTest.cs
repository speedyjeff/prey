using engine.Common;
using engine.Common.Entities;
using simulation.engine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulation.tests
{
    internal static class PredatorTest
    {
        // basic stats
        public static void Stats()
        {
            var predator = new Predator();

            // check stats
            if (predator.EnergyMeter != AIBase.EnergyMax) throw new Exception("incorrect energy");
            if (predator.Health != AIBase.HealthMax) throw new Exception("incorrect health");
            var startingDigestion = predator.DigestionMeter;
            var startingReproduce = predator.ReproduceMeter;

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
                action = predator.Action(
                   elements: new List<Element>(),
                   angleToCenter: 0,
                   inZone: false,
                   ref xdelta,
                   ref ydelta,
                   ref zdelta,
                   ref angle);

                // track updats
                updates++;

                // update
                predator.Update();

                // predict energy usage
                energy += Predator.EnergyFixedCost;
                if (predator.Speed == 0) { }
                else if (predator.Speed == Constants.Speed) energy += predator.EnergyCost;
                else if (predator.Speed == Constants.Speed * 2) energy += (2 * predator.EnergyCost);
                else throw new Exception("unknown speed");
            }
            while (action != ActionEnum.Move);

            // stay the same
            if (predator.Health != AIBase.HealthMax) throw new Exception("invalid health");

            // changes
            if (predator.EnergyMeter != (AIBase.EnergyMax + energy)) throw new Exception("invalid energy");
            if (predator.DigestionMeter > (startingDigestion + (predator.DigestionCost * updates)) && predator.DigestionMeter != 0) throw new Exception("invalid digestion");
            if (predator.ReproduceMeter != startingReproduce) throw new Exception("invalid reproduce");
        }

        // digestion
        public static void Digestion()
        {
            var predator = new Predator();

            var startingDigestion = predator.DigestionMeter;

            // update until digestion resets
            for(int i = startingDigestion/predator.DigestionCost; i >= 0; i--)
            {
                var before = predator.DigestionMeter;

                // update
                predator.Update();

                // check
                if (i == 0)
                {
                    if (predator.DigestionMeter != AIBase.DigestionMax) throw new Exception("invalid digestion");
                }
                else
                {
                    if (predator.DigestionMeter != (before + predator.DigestionCost)) throw new Exception("invalid digest");
                }
            }
        }

        // eat / reproduction
        public static void Reproduce()
        {
            var predator = new Predator();
            var prey = new Prey();

            // reset
            OnBirthCount = OnDeathCount = 0;
            AIBase.OnBirth += PredatorOnBirth;
            AIBase.OnDeath += PredatorOnDeath;

            // check IsLethal = true
            predator.IsLethal = true;
            if (predator.Health != AIBase.HealthMax) throw new Exception("incorrect health");
            var startingReproduce = predator.ReproduceMeter;
            var startingDigestion = predator.DigestionMeter;

            // eat until they reproduce
            for (int i = startingReproduce / predator.ReproduceCost; i >= 0; i--)
            {
                // update until ready to eat
                while (predator.DigestionMeter > 0) predator.Update();

                // eat
                predator.Eat(prey);

                // check
                if (prey.Health != 0) throw new Exception("invalid health");
                if (OnDeathCount != 1) throw new Exception("invalid ondeath");
                if (i == 0)
                {
                    if (OnBirthCount != 1) throw new Exception("invalid onbirth");
                }
                else if (OnBirthCount != 0) throw new Exception("invalid onbirth");

                // recreate the prey
                prey = new Prey();
            }

            // reset
            OnBirthCount = OnDeathCount = 0;

            // check not letal
            predator = new Predator();
            if (predator.Health != AIBase.HealthMax) throw new Exception("incorrect health");
            startingReproduce = predator.ReproduceMeter;
            startingDigestion = predator.DigestionMeter;

            for (int i = 1; i >= 0; i--)
            {
                var before = prey.Health;

                // update until ready to eat
                while (predator.DigestionMeter > 0 || predator.AttackMeter > 0) predator.Update();

                // eat
                predator.Eat(prey);

                // check
                if (prey.Health != (before + predator.Damage)) throw new Exception("invalid health");
                if (i == 0)
                {
                    if (OnDeathCount != 1) throw new Exception("invalid ondeath");
                }
                else if (OnDeathCount != 0) throw new Exception("invalid ondeath");
                if (OnBirthCount != 0) throw new Exception("invalid onbirth");
            }

            // reset
            OnBirthCount = OnDeathCount = 0;
            AIBase.OnBirth -= PredatorOnBirth;
            AIBase.OnDeath -= PredatorOnDeath;
        }

        #region private
        // utility
        private static int OnBirthCount = 0;
        private static int OnDeathCount = 0;
        private static void PredatorOnBirth(AIBase ai) { OnBirthCount++; }
        private static void PredatorOnDeath(AIBase ai) { OnDeathCount++; }
        #endregion
    }
}
