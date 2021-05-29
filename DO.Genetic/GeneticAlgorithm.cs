using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DO.Algorithm
{
    public class GeneticAlgorithm
    {
        public int ProductionSize { get; }
        public List<float> Ecologies { get; }
        public List<int> Costs { get; }
        public List<int> Profits { get; }
        public int A { get; }
        public int PopulationSize { get; }

        public List<List<int>> CurrentPopulation { get; private set; }
        public List<int> CurrentBlueParent { get; private set; }
        public List<int> CurrentYellowParent { get; private set; }
        public List<int> CurrentMask { get; private set; }
        public List<int> CurrentBlueChild { get; private set; }
        public List<int> CurrentYellowChild { get; private set; }

        public GeneticAlgorithm(int productionsize, List<float> ecologies, List<int> costs, List<int> profits, int A, int populationsize = 7)
        {
            ProductionSize = productionsize;
            Ecologies = ecologies;
            Costs = costs;
            Profits = profits;
            this.A = A;
            PopulationSize = populationsize;
        }

        public void GetStartPopulation()
        {
            List<List<int>> start = new List<List<int>>();
            for (int i = 0; i < PopulationSize; i++)
            {
                var temp = CreateChromosome();
                if (!start.Contains(temp))
                    start.Add(temp);
                else
                    i--;
            }
            CurrentPopulation = start;
        }

        private List<int> CreateChromosome()
        {
            Random random = new Random();
            List<int> chromosome = new List<int>();
            while (true)
            {
                for (int i = 0; i < ProductionSize; i++)
                {
                    chromosome.Add(random.Next(0, 2));
                }
                if (CalculateCost(chromosome) <= A)
                {
                    return chromosome;
                }
                else chromosome.Clear();
            }
        }

        public void GetParents()
        {
            List<List<int>> BluePopulation = new List<List<int>>();
            List<List<int>> YellowPopulation = new List<List<int>>();
            Random random = new Random();
            while (BluePopulation.Count == 0 || YellowPopulation.Count == 0)
            {
                BluePopulation.Clear();
                YellowPopulation.Clear();
                for (int i = 0; i < PopulationSize; i++)
                {
                    if (random.Next(0, 2) == 0)
                        BluePopulation.Add(CurrentPopulation[i]);
                    else
                        YellowPopulation.Add(CurrentPopulation[i]);
                }
            }
            CurrentBlueParent = BluePopulation.First(y => CalculateCF(y) == BluePopulation.Max(x => CalculateCF(x)));
            CurrentYellowParent = YellowPopulation.First(y => CalculateCF(y) == YellowPopulation.Max(x => CalculateCF(x)));
        }

        public List<int> CreateMask()
        {
            CurrentMask = CreateChromosome();
            return CurrentMask;
        }

        public void GetChildren()
        {
            CurrentBlueChild = CalculateChild(CurrentBlueParent, CurrentMask).ToList();
            CurrentYellowChild = CalculateChild(CurrentYellowParent, CurrentMask).ToList();
        }

        public void UpdatePopulation()
        {
            var min = CurrentPopulation.First(y => CalculateCF(y) == CurrentPopulation.Min(x => CalculateCF(x)));

            if (CalculateCF(CurrentBlueChild) > CalculateCF(min))
            {
                CurrentPopulation[CurrentPopulation.IndexOf(min)] = CurrentBlueChild;
                min = CurrentPopulation.First(y => CalculateCF(y) == CurrentPopulation.Min(x => CalculateCF(x)));
            }

            if (CalculateCF(CurrentYellowChild) > CalculateCF(min))
            {
                CurrentPopulation[CurrentPopulation.IndexOf(min)] = CurrentYellowChild;
            }

        }

        public void Mutation()
        {
            Random random = new Random();
            bool reversed = false;
            if (CalculateCF(CurrentBlueChild) < CalculateCF(CurrentYellowChild))
            {
                CurrentBlueChild[random.Next(0, CurrentBlueChild.Count)] = Reverse(CurrentBlueChild[random.Next(0, CurrentBlueChild.Count)]);
                reversed = true;
            }
            else
            {
                CurrentYellowChild[random.Next(0, CurrentYellowChild.Count)] = Reverse(CurrentYellowChild[random.Next(0, CurrentYellowChild.Count)]);
                reversed = false;
            }
            if (reversed)
                while (!CheckAdmissibility(CurrentBlueChild))
                {
                    CurrentBlueChild[random.Next(0, CurrentBlueChild.Count)] = Reverse(CurrentBlueChild[random.Next(0, CurrentBlueChild.Count)]);
                }
            else
                while (!CheckAdmissibility(CurrentYellowChild))
                {
                    CurrentYellowChild[random.Next(0, CurrentYellowChild.Count)] = Reverse(CurrentYellowChild[random.Next(0, CurrentYellowChild.Count)]);
                }
        }

        private bool CheckAdmissibility(List<int> chromosome)
        {
            if (CalculateCost(chromosome) >= A)
            {
                return false;
            }
            else return true;
        }

        private int Reverse(int boolean)
        {
            if (boolean == 1)
                return 0;
            else
                return 1;
        }

        private IEnumerable<int> CalculateChild(List<int> parent, List<int> mask)
        {
            List<int> child = new List<int>();
            while (true)
            {
                for (int i = 0; i < ProductionSize; i++)
                {
                    child.Add(parent[i] ^ mask[i]);
                }
                if (CalculateCost(child) <= A)
                {
                    return child;
                }
                else
                {
                    mask = CreateMask();
                    child.Clear();
                }
            }
        }

        private int CalculateCost(List<int> chromosome)
        {
            int sum = 0;
            for (int i = 0; i < chromosome.Count; i++)
            {
                if (chromosome[i] == 1)
                {
                    sum += Costs[i];
                }
            }
            return sum;
        }

        private float CalculateCF(List<int> chromosome)
        {
            float cf = 0;
            for (int i = 0; i < chromosome.Count; i++)
            {
                if (chromosome[i] == 1)
                {
                    cf += Profits[i] * Ecologies[i] / Costs[i];
                }
            }
            return cf;
        }

        public (List<int>, float) GetTheBestChromosome()
        {
            var best = CurrentPopulation.First(y => CalculateCF(y) == CurrentPopulation.Max(x => CalculateCF(x)));
            List<int> cf_names = new List<int>();

            for (int i = 0; i < best.Count; i++)
            {
                if (best[i] == 1)
                    cf_names.Add(i);
            }

            var cf = CalculateCF(best);
            return (cf_names, cf);
        }
    }
}
