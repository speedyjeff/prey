using engine.Common;
using engine.Common.Entities;
using System;

namespace simulation.engine
{
    public class Camera : Player
    {
        public Camera()
        {
            // hover around
            CanMove = false; // odd, but this allows the camera to move without hitting the ai
            TakesDamage = false;
            IsSolid = false;
            IsTransparent = true;
            ShowDefaultDrawing = false;
            Z = Constants.Ground + 1;

            Speed = Constants.Speed * 4;
        }

        public long Lifetime { get { return LifetimeCount; } }

        public override void Update()
        {
            System.Threading.Interlocked.Increment(ref LifetimeCount);
        }

        #region private
        private long LifetimeCount;
        #endregion
    }
}
