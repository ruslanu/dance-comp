using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstStudioTournamentScheduler
{
	public class Dance
	{
		public string Name;
		public bool isRythm;

		public Heat InitialPool = new Heat();
		public List<Heat> Heats = new List<Heat>();
		public Dictionary<string, int> Dancers = new Dictionary<string, int>();
		private int MinHeats = 0; // Will be equal to number of heats top dancer applied to

		private const int MIN_DESIRED_CAPACITY = 4;
		private const int MAX_DESIRED_CAPACITY = 6;

		public bool AddPair(DancingPair Pair, int NumDances)
		{
			if (Pair.IsValidDancers)
			{
				for (int i = 0; i < NumDances; i++)
				{
					InitialPool.Pairs.Add(Pair);

					// Accumulate statistics
					if (!Dancers.ContainsKey(Pair.Dancer1))
					{
						Dancers.Add(Pair.Dancer1, 1);
					}
					else
					{
						Dancers[Pair.Dancer1]++;
					}

					if (!Dancers.ContainsKey(Pair.Dancer2))
					{
						Dancers.Add(Pair.Dancer2, 1);
					}
					else
					{
						Dancers[Pair.Dancer2]++;
					}
				}
			}
			return Pair.IsValidDancers;
		}

		public bool CreateInitialHeats()
		{
			// Start with at least 15 heats but no less than top dancer
			MinHeats = Dancers.Values.Max();
			int InitialHeats = Math.Max(MinHeats, 15);

			string TopDancer = Dancers.First(d => d.Value == MinHeats).Key;
			Console.WriteLine("For dance <{0}> top dancer is <{1}> with {2} heats participation. {3} heats will be initially created.",
				Name, TopDancer, MinHeats, InitialHeats);

			Heats = new List<Heat>(InitialHeats);

			return Heats.Count > 0;
		}

		private bool CanAddToHeat(Heat heat, DancingPair pair)
		{
			List<string> existing = heat.Pairs.ConvertAll(a => a.Dancer1);
			existing.AddRange(heat.Pairs.ConvertAll(a => a.Dancer2));

			return !existing.Any(a => String.Equals(a, pair.Dancer1) || String.Equals(a, pair.Dancer2));
		}

		private Heat FindHeatForPair(DancingPair pair)
		{
			Heat result = null;

			Random rnd = new Random();

			// Try random heat 50 times having max desired capacity in mind
			for (int i = 0; result == null && i < 50; i++)
			{
				int index = rnd.Next(Heats.Count);
				Heat curr = Heats[index];
				if (curr.Pairs.Count < MAX_DESIRED_CAPACITY && CanAddToHeat(curr, pair))
				{
					result = curr;
				}
			}

			// If not found - try to find any heat despite max desired capacity
			for (int i = 0; result == null && i < Heats.Count; i++)
			{
				Heat curr = Heats[i];
				if (CanAddToHeat(curr, pair))
				{
					result = curr;
				}
			}

			return result;
		}

		public bool SeedPairsToHeats()
		{
			foreach (DancingPair pair in InitialPool.Pairs)
			{
				Heat heat = FindHeatForPair(pair);
				if (heat != null)
				{
					heat.Pairs.Add(pair);
					InitialPool.Pairs.Remove(pair);
				}
				else
				{
					Console.WriteLine("Fatal error: cannot find a heat for pair {0}: <{1}> - <{2}>!", pair.Team, pair.Dancer1, pair.Dancer2);
					break;
				}
			}
			
			// Succeeded if all pairs transferred to heats
			return InitialPool.Pairs.Count == 0;
		}

		public bool PopulateHeats()
		{
			bool ret = CreateInitialHeats();
			if (ret)
			{
				ret = SeedPairsToHeats();
				if (ret)
				{
					// TODO: Try to free up some space in overfilled heats (num > 6)

					// TODO: Remove empty and try to dispatch low capacity heats (num < 4)
				}
			}
			else
			{
				Console.WriteLine("Cannot populate dance {0} - no heats created for the dance!", Name);
			}

			return ret;
		}
	}
}
