using System;
using System.Collections.Generic;
using System.Linq;
using DO.Algorithm;

namespace DO
{
    static class UI
    {
        public static void LaunchMainMenu()
        {
            Console.WriteLine("Choose algorithm:" +
                "1) Genetic algorithm;" +
                "2) Beef algorithm;" +
                "3) Greedy algorithm;" +
                "4) Exit" +
                "Your choose: ");
            int n = int.Parse(Console.ReadKey().KeyChar.ToString());
            switch (n)
            {
                case 1:
                    LaunchGenetic();
                    break;
                case 2:
                    LaunchBeef();
                    break;
                case 3:
                    LaunchGreedy();
                    break;
                case 4:
                    Console.WriteLine("Bye");
                    return;
                default:
                    Console.WriteLine("Error input! Try again");
                    break;
            }
        }

        #region Genetic Algorithm UI
        private static void LaunchGenetic()
        {
            int production_size = Convert.ToInt32(Console.ReadLine());
            string[] production_names = new string[production_size];
            float[] ecologies = new float[production_size];
            int[] costs = new int[production_size];
            int[] profits = new int[production_size];
            for (int i = 0; i < production_size; i++)
            {
                Console.WriteLine($"Write info about product {i}:");
                Console.Write($"Enter name for product {i}:");
                production_names[i] = Console.ReadLine();
                Console.Write($"Enter ecology for {production_names[i]}:");
                ecologies[i] = (float)Convert.ToDouble(Console.ReadLine());
                Console.Write($"Enter cost for {production_names[i]}:");
                costs[i] = Convert.ToInt32(Console.ReadLine());
                Console.Write($"Enter profit for {production_names[i]}:");
                profits[i] = Convert.ToInt32(Console.ReadLine());
            }
            Console.Write("Enter A value: ");
            int a = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter population size: ");
            int population_size = Convert.ToInt32(Console.ReadLine());

            GeneticAlgorithm algorithm = new GeneticAlgorithm(production_size, ecologies.ToList(), costs.ToList(), profits.ToList(), a, population_size);
            int i_not = 0;
            while (i_not < 30)
            {
                WriteInfoAboutProduction(production_names, algorithm);

                algorithm.GetStartPopulation();
                WritePopulationMatrix(algorithm);

                WriteBest(algorithm);

                algorithm.GetParents();
                WriteParents(algorithm);

                algorithm.CreateMask();
                WriteMask(algorithm);

                algorithm.GetChildren();
                WriteChildren(algorithm);

                var best = algorithm.GetTheBestChromosome();
                
                algorithm.UpdatePopulation();

                if (best == algorithm.GetTheBestChromosome())
                    i_not++;
                else
                    i_not = 0;
            }
        }

        private static void WriteInfoAboutProduction(string[] production_names, GeneticAlgorithm algorithm)
        {
            Console.Write("Names");
            Array.ForEach(production_names, x => Console.WriteLine(x + "\t|\t"));
            Console.WriteLine();

            Console.Write("Ecologies");
            Array.ForEach(algorithm.Ecologies.ToArray(), x => Console.WriteLine(x.ToString() + "\t|\t"));
            Console.WriteLine();

            Console.Write("Costs");
            Array.ForEach(algorithm.Costs.ToArray(), x => Console.WriteLine(x.ToString() + "\t|\t"));
            Console.WriteLine();

            Console.Write("Profits");
            Array.ForEach(algorithm.Profits.ToArray(), x => Console.WriteLine(x.ToString() + "\t|\t"));
            Console.WriteLine();
        }

        private static void WritePopulationMatrix(GeneticAlgorithm algorithm)
        {
            for (int i = 0; i < algorithm.PopulationSize; i++)
            {
                Console.Write($"Population {i}\t");
                for (int j = 0; j < algorithm.ProductionSize; j++)
                {
                    Console.Write(algorithm.CurrentPopulation[i][j] + '\t');
                }
                Console.WriteLine();
            }
        }

        private static void WriteParents(GeneticAlgorithm algorithm)
        {
            Console.Write($"The best blue Population \t");
            for (int j = 0; j < algorithm.ProductionSize; j++)
            {
                Console.Write(algorithm.CurrentBlueParent[j] + '\t');
            }
            Console.WriteLine();
            Console.Write($"The best yellow Population \t");
            for (int j = 0; j < algorithm.ProductionSize; j++)
            {
                Console.Write(algorithm.CurrentYellowParent[j] + '\t');
            }
            Console.WriteLine();
        }

        private static void WriteMask(GeneticAlgorithm algorithm)
        {
            Console.Write($"Population Z\t");
            for (int j = 0; j < algorithm.ProductionSize; j++)
            {
                Console.Write(algorithm.CurrentMask[j] + '\t');
            }
        }

        private static void WriteChildren(GeneticAlgorithm algorithm)
        {
            Console.Write($"Population X\t");
            for (int j = 0; j < algorithm.ProductionSize; j++)
            {
                Console.Write(algorithm.CurrentBlueChild[j] + '\t');
            }
            Console.WriteLine();
            Console.Write($"Population Y\t");
            for (int j = 0; j < algorithm.ProductionSize; j++)
            {
                Console.Write(algorithm.CurrentYellowChild[j] + '\t');
            }
            Console.WriteLine();
        }

        private static void WriteBest(GeneticAlgorithm algorithm)
        {
            Console.Write("Best pop:\t");
            var rez = algorithm.GetTheBestChromosome();
            for (int i = 0; i < rez.Item1.Count; i++)
            {
                Console.Write(rez.Item1[i] + '\t');
            }
            Console.WriteLine(rez.Item2);
        }
        #endregion
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<float> ecology = new List<float>() { 0.7f, 0.8f, 0.9f, 1.2f, 1.3f };
            List<int> costs = new List<int>() { 15, 20, 30, 40, 50 };
            List<int> profits = new List<int>() { 28, 30, 40, 45, 55 };
            GeneticAlgorithm genetic = new GeneticAlgorithm(5, ecology, costs, profits, 105);
            genetic.GetStartPopulation();
            genetic.GetParents();
            genetic.CreateMask();
            genetic.GetChildren();
        }
    }
}
