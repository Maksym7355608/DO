using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DO.Algorithm
{
    public class GreedyAlgorithm
    {
        public int ProductionSize { get; }
        public List<float> Ecologies { get; }
        public List<int> Costs { get; }
        public List<int> Profits { get; }
        public int A { get; }

        public GreedyAlgorithm(int productionsize, List<float> ecologies, List<int> costs, List<int> profits, int A, int populationsize = 7)
        {
            ProductionSize = productionsize;
            Ecologies = ecologies;
            Costs = costs;
            Profits = profits;
            this.A = A;
        }

        public List<int> Execute()
        {
            List<float> cf = new List<float>();
            for (int i = 0; i < ProductionSize; i++)
            {
                cf.Add(CalculateCf(Ecologies[i], Costs[i], Profits[i]));
            }
            var ordered = cf.OrderByDescending(x => x).ToList();
            int sum = 0;
            List<int> rez = new List<int>();
            for (int i = 0; sum <= A;i++)
            {
                int index = cf.IndexOf(ordered[i]);
                rez.Add(index);
                sum += Costs[index];
            }
            if (sum <= A)
                return rez;
            else
            {
                rez.Remove(rez.Last());
                return rez;
            }

        }

        public float CalculateCf(float ecology, int cost, int profit)
        {
            return ecology * profit / cost;
        }
    }
}