using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace FirstStudioTournamentScheduler
{
	public class Heat
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public List<DancingPair> Pairs = new List<DancingPair>();

		public bool CanAddPair(DancingPair pair)
		{
			List<string> existing = Pairs.ConvertAll(a => a.Dancer1);
			existing.AddRange(Pairs.ConvertAll(a => a.Dancer2));

			return !existing.Any(a =>
				String.Equals(a, pair.Dancer1, StringComparison.InvariantCultureIgnoreCase) ||
				String.Equals(a, pair.Dancer2, StringComparison.InvariantCultureIgnoreCase));
		}

		public void DumpToLog(string title)
		{
			log.InfoFormat("Printing Heat <{0}>, total {1} pairs.", title, Pairs.Count);
			for (int i = 0; i < Pairs.Count; i++)
			{
				DancingPair pair = Pairs[i];
				log.InfoFormat("Team: <{0}> Pair {1}: <{2}>-<{3}> Ins: <{4}> Rank: <{5}>.",
					pair.Team, i + 1, pair.Dancer1, pair.Dancer2, pair.Instructor, pair.Rank);
			}
		}
	}
}
