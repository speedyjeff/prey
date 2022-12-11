using engine.Common;
using engine.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace simulation.engine
{
    public enum WallDirection { Horiztonal, Vertical };

    public class Wall : Obstacle
    {
        public Wall() : base()
        {
        }

        public Wall(WallDirection dir, float length, float thickness) : base()
        {
            IsSolid = true;
            if (dir == WallDirection.Horiztonal)
            {
                Width = length;
                Height = thickness;
            }
            else if (dir == WallDirection.Vertical)
            {
                Width = thickness;
                Height = length;
            }
            else throw new Exception("Unknown wall direction : " + dir);
        }

        public override void Draw(IGraphics g)
        {
            g.Rectangle(RGBA.Black, X-(Width/2), Y-(Height/2), Width, Height);
            base.Draw(g);
        }
    }
}
