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
                        LaunchGenetic();
                        break;
                    case 2:
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
            using StreamWriter fs = new StreamWriter("iteration.txt");
            fs.WriteLine($"Iteration: {counter}\tCF: {cf}");
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
            while (i_not < 30)
            {
                i++;
                //WriteInfoAboutProduction(production_names, algorithm);

                algorithm.GetStartPopulation();
                //WritePopulationMatrix(algorithm);

                WriteBest(algorithm);

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

                if (rez.Item2 <= algorithm.GetTheBestChromosome().Item2)
                    i_not++;
                else
                    i_not = 0;
            }
            WriteResultToFile(rez_names, rez_cf);
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
                Console.Write(rez.Item1[i].ToString() + ' ');
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

        private static List<string> ConvertNames(List<int> rez, string[] production_names)
        {
            return rez.Select(x => production_names[rez.IndexOf(x)]).ToList();
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
