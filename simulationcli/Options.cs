using simulation.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulationcli
{
    class Options
    {
        public bool Help { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int InitialPredators { get; set; }
        public int MaxPredators { get; set; }
        public int InitialPrey { get; set; }
        public int MaxPrey { get; set; }
        public AIType Combat { get; set; }
        public AIType Random { get; set; }
        public int Iterations { get; set; }
        public long MaxLifetime { get; set; }

        public static void ShowHelp()
        {
            Console.WriteLine("./simulationcli");
            Console.WriteLine("  -(help)             : this help");
            Console.WriteLine("  -(w)idth            : the 2d width of the environment (default: 6400)");
            Console.WriteLine("  -(hei)ght           : the 2d height of the enviorment (default: 6400)");
            Console.WriteLine("  -(initialPred)ators : the initial number of predators at start (default: 50)");
            Console.WriteLine("  -(maxPred)ators     : the max number of predators (default: 170)");
            Console.WriteLine("  -(initialPrey)      : the initial number of prey at start (default: 300)");
            Console.WriteLine("  -(maxPrey)          : the max numbre of prey (default: 800)");
            Console.WriteLine("  -(c)ombat           : allowed combat (0: none, 1:Predators, 2:Prey, 3:Predators+Prey) (default: 1)");
            Console.WriteLine("  -(r)andom           : random movement (0: none, 1:Predators, 2:Prey, 3:Predators+Prey) (default: 0)");
            Console.WriteLine("  -(it)erations       : number of training runs (a run stops when either Predators or Prey are extinct) (default: 10)");
            Console.WriteLine("  -(maxl)ifetime      : maximum number of updates per iteration (default: 5000");
        }

        public static Options Parse(string[] args)
        {
            var options = new Options()
            {
                Width = 6400,
                Height = 6400,
                InitialPredators = 50,
                InitialPrey = 300,
                MaxPredators = 170,
                MaxPrey = 800,
                Combat = AIType.Predator,
                Random = 0,
                Iterations = 10,
                MaxLifetime = 5000
            };

            for(int i=0; i<args.Length; i++)
            {
                if (args[i].StartsWith("-help",StringComparison.OrdinalIgnoreCase))
                {
                    options.Help = true;
                }
                else if (args[i].StartsWith("-w", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.Width = Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-hei", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.Height = Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-initialpred", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.InitialPredators = Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-initialprey", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.InitialPrey = Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-maxpred", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.MaxPredators = Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-maxprey", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.MaxPrey = Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-c", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.Combat = (AIType)Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-r", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.Random = (AIType)Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-it", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.Iterations = Convert.ToInt32(args[++i]);
                }
                else if (args[i].StartsWith("-maxl", StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length) options.MaxLifetime = Convert.ToInt64(args[++i]);
                }
                else
                {
                    Console.WriteLine($"unknown option: {args[i]}");
                }
            }

            return options;
        }
    }
}
