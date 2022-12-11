using engine.Common.Entities;
using engine.Common;
using engine.Winforms;
using simulation.engine;

namespace simulation
{
    public partial class simulation : Form
    {
        public simulation()
        {
            Width = 1024;
            Height = 800;
            // setting a double buffer eliminates the flicker
            DoubleBuffered = true;

            // create world
            Manager = WorldManager.Create(WorldManager.CreateStartupOptions(isSmall: true));
            
            // start the UI painting
            UI = new UIHookup(this, Manager.World);
        }

        #region private
        private UIHookup UI;
        private WorldManager Manager;
        #endregion
    }
}