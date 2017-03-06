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

		public List<DancingPair> Pairs = new List<DancingPair>();
		public Dictionary<string, int> Dancers = new Dictionary<string, int>();

		public void AddPair(DancingPair Pair, int NumDances)
		{
			for (int i = 0; i < NumDances; i++)
			{
				Pairs.Add(Pair);

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
	}
}
