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
				dance.PopulateHeats();
				foreach(Heat heat in dance.Heats)
				{
					BlockHeats.Add(new BlockHeat {
						SortIndex = Scheduler.glbRandom.Next(65536),
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

		//public void PrintSchedule(StreamWriter writer, int InitialNumber)
		//{
		//
		//}
	}
}
