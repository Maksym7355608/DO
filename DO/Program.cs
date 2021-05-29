using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DO.Algorithm;

namespace DO
{
    static class UI
    {
        public static void LaunchMainMenu()
        {
            while (true)
            {
                Console.WriteLine("Choose algorithm:\n" +
                                "1) Genetic algorithm;\n" +
                                "2) Bees algorithm;\n" +
                                "3) Greedy algorithm;\n" +
                                "4) Exit\n" +
                                "Your choose: ");
                int n = int.Parse(Console.ReadLine());
                switch (n)
                {
                    case 1:
                        File.WriteAllText("iteration.txt", string.Empty);
                        LaunchGenetic();
                        break;
                    case 2:
                        File.WriteAllText("iteration.txt", string.Empty);
                        LaunchBees();
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

        }

        private static void ReadStartInfoFromFile(ref int production_size, ref string[] production_names, ref float[] ecologies, ref int[] costs, ref int[] profits, ref int a)
        {
            using StreamReader fs = new StreamReader("data1.txt", Encoding.Default);
            string line;
            line = fs.ReadLine();
            production_size = int.Parse(line);
            line = fs.ReadLine();
            production_names = line.Split(';');
            line = fs.ReadLine();
            ecologies = line.Split(';').Select(x => float.Parse(x)).ToArray();
            line = fs.ReadLine();
            costs = line.Split(';').Select(x => int.Parse(x)).ToArray();
            line = fs.ReadLine();
            profits = line.Split(';').Select(x => int.Parse(x)).ToArray();
            line = fs.ReadLine();
            a = int.Parse(line);

            fs.Close();
        }
        private static void ReadStartInfoFromConsole(ref int production_size, ref string[] production_names, ref float[] ecologies, ref int[] costs, ref int[] profits, ref int a)
        {
            Console.WriteLine("Enter production count: ");
            production_size = Convert.ToInt32(Console.ReadLine());

            production_names = new string[production_size];
            ecologies = new float[production_size];
            costs = new int[production_size];
            profits = new int[production_size];
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
            a = Convert.ToInt32(Console.ReadLine());
        }
        private static void WriteIterationBestResultToFile(int counter, float cf)
        {
            using StreamWriter fs = new StreamWriter("iteration.txt", true);

            fs.Write($"Iteration: {counter}\tCF: {cf}\n");
        }
        private static void WriteResultToFile(string[] names, float cf)
        {
            using StreamWriter fs = new StreamWriter("result.txt");
            fs.Flush();
            string r = default;
            Array.ForEach(names, x => r += x + ' ');
            fs.WriteLine($"Optimal set: {r}");
            fs.WriteLine($"CF: {cf}");
        }

        private static List<string> ConvertNames(List<int> rez, string[] production_names)
        {
            return rez.Select(x => production_names[rez.IndexOf(x)]).ToList();
        }

        #region Genetic Algorithm UI
        private static void LaunchGenetic()
        {
            int production_size = default;
            string[] production_names = default;
            float[] ecologies = default;
            int[] costs = default;
            int[] profits = default;
            int a = default;
            Console.WriteLine("Choose input data:\n" +
                "1 - from console;\n" +
                "2 - from file;\n");
            int choose = int.Parse(Console.ReadLine());
            if (choose == 1)
                ReadStartInfoFromConsole(ref production_size, ref production_names, ref ecologies, ref costs, ref profits, ref a);
            else if (choose == 2)
                ReadStartInfoFromFile(ref production_size, ref production_names, ref ecologies, ref costs, ref profits, ref a);
            Console.Write("Enter population size: ");
            int population_size = Convert.ToInt32(Console.ReadLine());

            GeneticAlgorithm algorithm = new GeneticAlgorithm(production_size, ecologies.ToList(), costs.ToList(), profits.ToList(), a, population_size);
            int i_not = 0;
            int i = 0;
            string[] rez_names = default;
            float rez_cf = default;
            Dictionary<float, List<string>> best = new Dictionary<float, List<string>>();
            best.Add(0, null);
            while (i_not < 30)
            {
                i++;
                //WriteInfoAboutProduction(production_names, algorithm);

                algorithm.GetStartPopulation();
                //WritePopulationMatrix(algorithm);

                WriteBest(algorithm, production_names);

                algorithm.GetParents();
                //WriteParents(algorithm);

                algorithm.CreateMask();
                //WriteMask(algorithm);

                algorithm.GetChildren();
                //WriteChildren(algorithm);

                var rez = algorithm.GetTheBestChromosome();
                rez_names = ConvertNames(rez.Item1, production_names).ToArray();
                rez_cf = rez.Item2;

                algorithm.UpdatePopulation();

                WriteIterationBestResultToFile(i, rez_cf);

                if (rez.Item2 <= best.Keys.Max())
                    i_not++;
                else
                    i_not = 0;

                if (!best.ContainsKey(rez_cf))
                    best.Add(rez_cf, rez_names.ToList());
            }
            var item = best.First(x => x.Key == best.Keys.Max());
            WriteResultToFile(item.Value.ToArray(), item.Key);
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

        private static void WriteBest(GeneticAlgorithm algorithm, string[] production_names)
        {
            Console.Write("Best pop:\t");
            var rez = algorithm.GetTheBestChromosome();
            var names = ConvertNames(rez.Item1, production_names);
            for (int i = 0; i < names.Count; i++)
            {
                Console.Write(names[i] + '\t');
            }
            Console.WriteLine('\n' + rez.Item2.ToString());
        }
        #endregion

        #region Greedy Algorithm UI
        private static void LaunchGreedy()
        {
            int production_size = default;
            string[] production_names = default;
            float[] ecologies = default;
            int[] costs = default;
            int[] profits = default;
            int a = default;
            Console.WriteLine("Choose input data:\n" +
                "1 - from console;\n" +
                "2 - from file;\n");
            int choose = int.Parse(Console.ReadLine());
            if (choose == 1)
                ReadStartInfoFromConsole(ref production_size, ref production_names, ref ecologies, ref costs, ref profits, ref a);
            else if (choose == 2)
                ReadStartInfoFromFile(ref production_size, ref production_names, ref ecologies, ref costs, ref profits, ref a);

            GreedyAlgorithm algorithm = new GreedyAlgorithm(production_size, ecologies.ToList(), costs.ToList(), profits.ToList(), a);

            var rez = algorithm.Execute();
            var rez_prod = ConvertNames(rez.Item1, production_names);
            var rez_cf = rez.Item2;

            Console.WriteLine("Rezult:");
            rez_prod.ForEach(x => Console.WriteLine(x));
            Console.WriteLine($"CF: {rez_cf}");

            WriteResultToFile(rez_prod.ToArray(), rez_cf);
        }
        #endregion

        #region Bees Alorithm UI
        private static void LaunchBees()
        {
            int production_size = default;
            string[] production_names = default;
            float[] ecologies = default;
            int[] costs = default;
            int[] profits = default;
            int a = default;
            Console.WriteLine("Choose input data:\n" +
                "1 - from console;\n" +
                "2 - from file;\n");
            int choose = int.Parse(Console.ReadLine());
            if (choose == 1)
                ReadStartInfoFromConsole(ref production_size, ref production_names, ref ecologies, ref costs, ref profits, ref a);
            else if (choose == 2)
                ReadStartInfoFromFile(ref production_size, ref production_names, ref ecologies, ref costs, ref profits, ref a);

            Console.Write("Enter scouts count: ");
            int scout_count = Convert.ToInt32(Console.ReadLine());

            BeesAlgorithm algorithm = new BeesAlgorithm(production_size, ecologies.ToList(), costs.ToList(), profits.ToList(), a);
            int i_not = 0;
            int i = 0;
            string[] rez_names = default;
            float rez_cf = default;
            Dictionary<float, List<string>> best = new Dictionary<float, List<string>>();
            best.Add(0, null);
            while (i_not < 30)
            {
                i++;
                algorithm.RunScouts();

                algorithm.GetTheBestScouts(3);

                algorithm.RunForagingBees();

                algorithm.LocalUpdate();
                WriteBest(algorithm, production_names);
                var rez = algorithm.GetTheBestBee();
                rez_names = ConvertNames(rez.Item1, production_names).ToArray();
                rez_cf = rez.Item2;

                WriteIterationBestResultToFile(i, rez_cf);

                if (rez.Item2 <= best.Keys.Max())
                    i_not++;
                else
                    i_not = 0;

                if (!best.ContainsKey(rez_cf))
                    best.Add(rez_cf, rez_names.ToList());
            }
            var item = best.First(x => x.Key == best.Keys.Max());
            WriteResultToFile(item.Value.ToArray(), item.Key);
        }

        private static void WriteBest(BeesAlgorithm algorithm, string[] production_names)
        {
            Console.Write("Best pop:\t");
            var rez = algorithm.GetTheBestBee();
            var names = ConvertNames(rez.Item1, production_names);
            for (int i = 0; i < names.Count; i++)
            {
                Console.Write(names[i] + '\t');
            }
            Console.WriteLine('\n' + rez.Item2.ToString());
        }
        #endregion
    }
    class Program
    {
        static void Main(string[] args)
        {
            //List<float> ecology = new List<float>() { 0.7f, 0.8f, 0.9f, 1.2f, 1.3f };
            //List<int> costs = new List<int>() { 15, 20, 30, 40, 50 };
            //List<int> profits = new List<int>() { 28, 30, 40, 45, 55 };
            //GeneticAlgorithm genetic = new GeneticAlgorithm(5, ecology, costs, profits, 105);
            //genetic.GetStartPopulation();
            //genetic.GetParents();
            //genetic.CreateMask();
            //genetic.GetChildren();
            //genetic.UpdatePopulation();
            //var best = genetic.GetTheBestChromosome();
            //genetic.GetStartPopulation();
            //genetic.GetParents();
            //genetic.CreateMask();
            //genetic.GetChildren();
            //genetic.Mutation();
            //genetic.UpdatePopulation();
            //var best1 = genetic.GetTheBestChromosome();
            UI.LaunchMainMenu();
        }
    }
}
