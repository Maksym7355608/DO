using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DO.Algorithm
{
    public class BeesAlgorithm
    {
        public int ProductionSize { get; }
        public List<float> Ecologies { get; }
        public List<int> Costs { get; }
        public List<int> Profits { get; }
        public int A { get; }
        public int ScoutCount { get; }

        public List<List<int>> CurrentPopulation { get; private set; }
        public List<List<int>> TheBestScouts { get; private set; }
        public int ForagingCount { get; private set; }

        public BeesAlgorithm(int productionsize, List<float> ecologies, List<int> costs, List<int> profits, int A, int scout_count = 7)
        {
            ProductionSize = productionsize;
            Ecologies = ecologies;
            Costs = costs;
            Profits = profits;
            this.A = A;
            ScoutCount = scout_count;
        }

        public void RunScouts()
        {
            List<List<int>> start = new List<List<int>>();
            for (int i = 0; i < ScoutCount; i++)
            {
                var temp = GetScout();
                if (!start.Contains(temp))
                    start.Add(temp);
                else
                    i--;
            }
            CurrentPopulation = start;
        }

        public void GetTheBestScouts()
        {
            TheBestScouts = CurrentPopulation.OrderByDescending(x => CalculateCost(x)).Take(3).ToList();
        }

        public void RunForagingBees()
        {
            ForagingCount = (int)Math.Round(TheBestScouts.Sum(x => CalculateCF(x)));


        }

        private List<int> GetScout()
        {
            Random random = new Random();
            List<int> bee = new List<int>();
            while (true)
            {
                for (int i = 0; i < ProductionSize; i++)
                {
                    bee.Add(random.Next(0, 2));
                }
                if (CalculateCost(bee) <= A)
                {
                    return bee;
                }
                else bee.Clear();
            }
        }

        private int CalculateCost(List<int> bee)
        {
            int sum = 0;
            for (int i = 0; i < bee.Count; i++)
            {
                if (bee[i] == 1)
                {
                    sum += Costs[i];
                }
            }
            return sum;
        }

        public float CalculateCF(List<int> bee)
        {
            float cf = 0;
            for (int i = 0; i < bee.Count; i++)
            {
                if (bee[i] == 1)
                {
                    cf += Profits[i] * Ecologies[i] / Costs[i];
                }
            }
            return cf;
        }
    }
}
