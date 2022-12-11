using engine.Common;
using engine.Common.Entities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace simulation.engine
{
    public class Hud : Menu
    {
        public Hud()
        {
            // track ai that are alive
            AlivePrey = AlivePredators = 0;

            // frame rate
            FpsTimer = new Stopwatch();
            Fps = new float[20];
            FpsIndex = 0;
        }

        public int AlivePredators { get; set; }
        public int AlivePrey { get; set; }

        public override void Draw(IGraphics g)
        {
            // display counts
            g.Text(RGBA.Black, x: 10, y: 10, $"Predator\t: {AlivePredators}");
            g.Text(RGBA.Black, x: 10, y: 50, $"Prey\t: {AlivePrey}");
            var avg = Fps.Average();
            g.Text(RGBA.Black, x: 10, y: 90, $"FPS\t: {1000 / (avg > 0 ? avg : 1000):f1}");

            // draw highlighted AI
            var startx = g.Width - 250;
            var starty = g.Height - 300;
            g.Text(RGBA.Black, startx, starty + 0, $"Type\t: {(HighlightedAI != null ? HighlightedAI.Type.ToString() : "")}", fontsize: 8);
            g.Text(RGBA.Black, startx, starty + 25, $"Health\t: {(HighlightedAI != null ? HighlightedAI.Health : 0)}", fontsize: 8);
            g.Text(RGBA.Black, startx, starty + 50, $"Lifetime\t: {(HighlightedAI != null ? HighlightedAI.Lifetime : 0)}", fontsize: 8);
            g.Text(RGBA.Black, startx, starty + 75, $"Gen\t: {(HighlightedAI != null ? HighlightedAI.Generation : 0)}", fontsize: 8);
            g.Text(RGBA.Black, startx, starty + 100, $"Energy\t: {(HighlightedAI != null ? HighlightedAI.EnergyMeter : 0)}", fontsize: 8);
            g.Text(RGBA.Black, startx, starty + 125, $"Digest\t: {(HighlightedAI != null ? HighlightedAI.DigestionMeter : 0)}", fontsize: 8);
            g.Text(RGBA.Black, startx, starty + 150, $"Reproduce: {(HighlightedAI != null ? HighlightedAI.ReproduceMeter : 0)}", fontsize: 8);
            g.Text(RGBA.Black, startx, starty + 175, $"Attack\t: {(HighlightedAI != null ? HighlightedAI.AttackMeter : 0)}", fontsize: 8);

            // track the time between calls to draw (use as an appromixation of fps)
            if (FpsTimer.IsRunning)
            {
                FpsTimer.Stop();
                Fps[FpsIndex] = FpsTimer.ElapsedMilliseconds;
                FpsIndex = (FpsIndex + 1) % Fps.Length;
            }

            // restart timer
            FpsTimer.Restart();
        }

        public void Highlight(AIBase ai)
        {
            if (HighlightedAI != null && HighlightedAI.Id != ai.Id) UnHighlight();
            HighlightedAI = ai;
            HighlightedAI.ShowViewPort = true;
        }

        public void UnHighlight()
        {
            if (HighlightedAI != null) HighlightedAI.ShowViewPort = false;
            HighlightedAI = null;
        }

        #region private
        private Stopwatch FpsTimer;
        private float[] Fps;
        private int FpsIndex;

        private AIBase HighlightedAI;
        #endregion
    }
}
