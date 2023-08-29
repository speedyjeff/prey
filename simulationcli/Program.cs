using engine.Common;
using simulation.engine;

namespace simulationcli
{
    internal class Program
    {
        static int Main(string[] args)
        {
            var options = Options.Parse(args);

            if (options.Help)
            {
                Options.ShowHelp();
                return 1;
            }

            Console.WriteLine($"# W,H {options.Width},{options.Height} MaxLifetime {options.MaxLifetime} Predators {options.InitialPredators}/{options.MaxPredators} Prey {options.InitialPrey}/{options.MaxPrey} Combat {options.Combat} Random {options.Random}");

            // create the world manager options
            var wmoptions = new WorldManagerOptions()
            {
                Width = options.Width,
                Height = options.Height,
                InitialPredatorCount = options.InitialPredators,
                InitialPreyCount = options.InitialPrey,
                MaxPredators = options.MaxPredators,
                MaxPrey = options.MaxPrey,
                Combat = options.Combat,
                FullRandom = options.Random,
                StartPaused = true,
                EnableFastPlayerUpdate = true // required
            };

            // todo add evolution mode

            // todo load models

            // run iterations
            Predator bestPredator = null;
            Prey bestPrey = null;
            Console.WriteLine("Iteration\tLifetime\tAlivePredators\tAlivePrey\tBestPredatorLifetime\tBestPredatorGeneration\tBestPreyLifetime\tBestPreyGenertation\tAvgPredatorLifetime\tAvgPreyLifetime");
            for (int i = 0; i < options.Iterations; i++)
            {
                // use the best from the previous round
                wmoptions.StartingPredator = bestPredator;
                wmoptions.StartingPrey = bestPrey;

                // create
                var manager = WorldManager.Create(wmoptions);

                // start world
                manager.World.KeyPress(Constants.Esc);

                // poll until one of the populations becomes extinct
                while (manager.AlivePredators > 0 && manager.AlivePrey > 0 && manager.Lifetime < options.MaxLifetime)
                {
                    System.Threading.Thread.Sleep(50);
                }

                // stop world
                manager.World.KeyPress(Constants.Esc);

                // show stats
                Console.WriteLine($"{i}\t{manager.Lifetime}\t{manager.AlivePredators}\t{manager.AlivePrey}\t{(manager.AncestorPredator != null ? manager.AncestorPredator.Lifetime : 0)}\t{(manager.AncestorPredator != null ? manager.AncestorPredator.Generation : 0)}\t{(manager.AncestorPrey != null ? manager.AncestorPrey.Lifetime : 0)}\t{(manager.AncestorPrey != null ? manager.AncestorPrey.Generation : 0)}\t{manager.AveragePredatorLifetime}\t{manager.AveragePreyLifetime}");

                // store the best models from this round
                bestPredator = manager.AncestorPredator;
                bestPrey = manager.AncestorPrey;

                // todo save models
            }

            return 0;
        }
    }
}
