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
		public Dictionary<string, int> Dancers = new Dictionary<string, int>();

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

	}
}
