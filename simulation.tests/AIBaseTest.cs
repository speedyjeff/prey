using engine.Common;
using simulation.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace simulation.tests
{
    internal class AIBaseTestObject : AIBase
    {
        public AIBaseTestObject(float[] angles, float dist)
        {
            ViewportAngles = angles;
            ViewportDistance = dist;
        }
    }

    internal static class AIBaseTest
    {
        // basic close
        public static void Closest()
        {
            var ai = new AIBaseTestObject(angles: new float[] { -45f, 0f, 45f }, dist: 500)
            {
                Type = AIType.Predator,
                X = 0,
                Y = 0,
                Z = 0,
                Width = 50,
                Height = 50
            };

            // all of these values should be getting closer (eg. approaching 1f)
            var prvdist = 0f;
            foreach (var y in new float[] { -499, -100, -51, -551})
            {
                var elements = new List<Element>()
                {
                    new Prey() { X = 0, Y = y}
                };
                var results = ai.Viewport(elements);

                // always true
                if (results.Length != 6) throw new Exception("invalid result");
                if (results[0] != 0 || results[1] != 0) throw new Exception("invalid first angle view");
                if (results[4] != 0 || results[5] != 0) throw new Exception("invalid third angle view");
                if (y != -551 && results[3] != 1f) throw new Exception("invalid element type");
                if (results[2] > 1f) throw new Exception("invalid distance");

                // specific
                if (y == -551)
                {
                    if (results[2] != 0f) throw new Exception("incorrect distance");
                }
                else if (prvdist > 0f)
                {
                    // check that this object is now closer
                    if (prvdist >= results[2]) throw new Exception("too close");
                }

                // set previous distance
                prvdist = results[2];
            }
        }

        // two objects in a line
        public static void Shadow()
        {
            var ai = new AIBaseTestObject(angles: new float[] { -45f, 0f, 45f }, dist: 1000)
            {
                Type = AIType.Predator,
                X = 10,
                Y = 10,
                Z = 0,
                Width = 50,
                Height = 50
            };

            // two objects at -45 degrees from ai
            var elements = new List<Element>()
                {
                    new Predator() { X = -90, Y = -90},
                    new Prey() { X = -190, Y = -190}
                };
            var results = ai.Viewport(elements);

            // always true
            if (results.Length != 6) throw new Exception("invalid result");
            if (results[2] != 0 || results[3] != 0) throw new Exception("invalid second angle view");
            if (results[4] != 0 || results[5] != 0) throw new Exception("invalid third angle view");

            // one object is close
            if (results[1] != 0f) throw new Exception("invalid element type");
            if (results[0]< 0.85f || results[0] > 0.95f) throw new Exception("invalid distance");
        }

        // all rays connecting with an object
        public static void All()
        {
            var ai = new AIBaseTestObject(angles: new float[] { -90f, 180f, 45f }, dist: 1000)
            {
                Type = AIType.Predator,
                X = 0,
                Y = 0,
                Z = 0,
                Width = 50,
                Height = 50
            };

            // two objects at -45 degrees from ai
            var elements = new List<Element>()
                {
                    new Predator() { X = -900, Y = 0},
                    new Prey() { X = 0, Y = 500},
                    new Wall() { X = 100, Y = -100, Width = 50, Height = 50}
                };
            var results = ai.Viewport(elements);

            // always true
            if (results.Length != 6) throw new Exception("invalid result");

            var expected = new float[]
            {
                0.125f, 0f,
                0.525f, 1f,
                0.893f, -1f
            };

            for(int i=0; i<expected.Length; i++)
            {
                if (Math.Abs(results[i] - expected[i]) > 0.01f) throw new Exception($"invalid : {results[i]} and {expected[i]}");
            }
        }
    }
}
