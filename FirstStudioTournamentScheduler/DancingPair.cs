using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstStudioTournamentScheduler
{
	public class DancingPair
	{
		public Teams Team;
		public string Dancer1;
		public string Dancer2;
		public string Instructor;
		public string Rank;

		public bool IsValidDancers
		{
			get
			{
				return !String.IsNullOrEmpty(Dancer1) && !String.IsNullOrEmpty(Dancer2) && !String.Equals(Dancer1, Dancer2);
			}
		}
	}
}
