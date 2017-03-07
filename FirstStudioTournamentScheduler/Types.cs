using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstStudioTournamentScheduler
{
    class Types
    {
    }

    public enum Teams { Blue, Red, White }

	public class Heat
	{
		public List<DancingPair> Pairs = new List<DancingPair>();

		public bool CanAddPair(DancingPair pair)
		{
			List<string> existing = Pairs.ConvertAll(a => a.Dancer1);
			existing.AddRange(Pairs.ConvertAll(a => a.Dancer2));

			return !existing.Any(a => String.Equals(a, pair.Dancer1) || String.Equals(a, pair.Dancer2));
		}
	}


}
