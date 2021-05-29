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
        public int ForagingCount { get; private set; }
        public int FlowersAreas { get; }

        public List<List<int>> CurrentPopulation { get; private set; }
        public List<List<int>> TheBestScouts { get; private set; }
        public List<int> ConcreteForaging { get; private set; }
        public Dictionary<List<int>, List<List<int>>> Grouping { get; private set; }


        public BeesAlgorithm(int productionsize, List<float> ecologies, List<int> costs, List<int> profits, int A, int scout_count = 7, int foraging_count = 10, int flowers_areas = 3)
        {
            ProductionSize = productionsize;
            Ecologies = ecologies;
            Costs = costs;
            Profits = profits;
            this.A = A;
            ScoutCount = scout_count;
            ForagingCount = foraging_count;
            FlowersAreas = flowers_areas;
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

        public void GetTheBestScouts(int n)
        {
            TheBestScouts = CurrentPopulation.OrderByDescending(x => CalculateCost(x)).Take(n).ToList();
        }

        public void RunForagingBees()
        {
            ConcreteForaging = new List<int>();
            Grouping = new Dictionary<List<int>, List<List<int>>>();
            var cfs = TheBestScouts.Select(x => CalculateCF(x)).ToList();
            var sum_cfs = TheBestScouts.Sum(x => CalculateCF(x));
            for (int i = 0; i < TheBestScouts.Count; i++)
            {
                ConcreteForaging.Add(Convert.ToInt32(cfs[i] / sum_cfs * ForagingCount));
            }

            List<List<int>> best = new List<List<int>>();
            for (int i = 0; i < ConcreteForaging.Count; i++)
            {
                for (int j = 0; j < ConcreteForaging[i]; j++)
                {
                    best.Add(CreateForaging(TheBestScouts[i]));
                }
                Grouping.Add(TheBestScouts[i], best);
                best.Clear();
            }

        }

        private List<int> CreateForaging(List<int> scout)
        {
            while (true)
            {
                Random random = new Random();
                int index = random.Next(0, scout.Count);
                scout[index] = Reverse(scout[index]);

                if (CalculateCost(scout) <= A)
                    return scout;
                else
                    scout[index] = Reverse(scout[index]);
            }
        }

        public void LocalUpdate()
        {
            List<List<int>> best = new List<List<int>>();
            foreach (var item in Grouping)
            {
                List<List<int>> temp = item.Value;
                temp.Add(item.Key);
                best.Add(temp.First(x => CalculateCF(x) == temp.Max(y => CalculateCF(y))));
            }
            TheBestScouts = best;
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

        private int Reverse(int boolean)
        {
            if (boolean == 1)
                return 0;
            else
                return 1;
        }

        public (List<int>, float) GetTheBestBee()
        {
            var best = TheBestScouts.First(y => CalculateCF(y) == TheBestScouts.Max(x => CalculateCF(x)));
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
