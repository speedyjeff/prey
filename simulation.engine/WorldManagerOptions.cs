using System;
using System.Collections.Generic;
using System.Text;

namespace simulation.engine
{
    public struct WorldManagerOptions
    {
        public int Width;
        public int Height;
        public int InitialPredatorCount;
        public int MaxPredators;
        public int InitialPreyCount;
        public int MaxPrey;
        public AIType Combat;
        public AIType FullRandom;
        public AIBase StartingPredator;
        public AIBase StartingPrey;
        public bool StartPaused;
        public bool EnableFastPlayerUpdate;
    }
}
