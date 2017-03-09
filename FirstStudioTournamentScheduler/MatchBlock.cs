using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace FirstStudioTournamentScheduler
{
	public class MatchBlock
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public struct BlockHeat
		{
			public int SortIndex;
			public string DanceName;
			public Heat Heat;
		}

		public MatchBlock(string name)
		{
			Name = name;
		}

		public string Name;
		public List<Dance> Dances = new List<Dance>();
		public List<BlockHeat> BlockHeats = new List<BlockHeat>();

		public void GenerateSchedule()
		{
			foreach(Dance dance in Dances)
			{
				dance.PopulateHeatsNewWay();
				foreach(Heat heat in dance.Heats)
				{
					BlockHeats.Add(new BlockHeat {
						SortIndex = Scheduler.glbRandom.Next(256),
						DanceName = dance.Name,
						Heat = heat,
					});
				}
			}
			BlockHeats.Sort((h1, h2) => h1.SortIndex.CompareTo(h2.SortIndex));
			DumpBlockHeatsToLog();
		}

		public void DumpBlockHeatsToLog()
		{
			log.InfoFormat("**** MatchBlock <{0}> ****", Name);
			for(int i = 0; i < BlockHeats.Count; i++)
			{
				log.InfoFormat("Heat #{0} - {1} - {2} dancers", i + 1, BlockHeats[i].DanceName, BlockHeats[i].Heat.Pairs.Count);
			}
			log.InfoFormat("End of MatchBlock.");
		}

		public void DumpSchedule(StringBuilder ScheduleContent, int InitialNumber)
		{
			ScheduleContent.AppendLine(Name);
			for (int i = 0; i < BlockHeats.Count; i++)
			{
				BlockHeat heat = BlockHeats[i];
				ScheduleContent.AppendLine();
				ScheduleContent.AppendLine(String.Format("Heat {0} - {1}", InitialNumber + i + 1, heat.DanceName));
				foreach (DancingPair pair in heat.Heat.Pairs)
				{
					ScheduleContent.AppendLine(String.Format("{0}\t{1}\t{2}\t{3}",
						pair.Team, pair.Dancer1, pair.Dancer2, pair.Rank));
				}
			}
		}
	}
}
