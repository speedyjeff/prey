namespace simulation.tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //
            // AIBase
            //
            AIBaseTest.All();
            AIBaseTest.Shadow();
            AIBaseTest.Closest();

            //
            // Predator
            //
            PredatorTest.Stats();
            PredatorTest.Digestion();
            PredatorTest.Reproduce();

            //
            // Prey
            //
            PreyTest.Stats();
            PreyTest.Reproduce();
            PreyTest.Attack();
        }
    }
}