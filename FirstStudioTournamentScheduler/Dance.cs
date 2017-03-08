using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace FirstStudioTournamentScheduler
{
	public class Dance
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string Name;
		public bool isRythm;

		public Heat InitialPool = new Heat();
		public List<Heat> Heats = new List<Heat>();
		public Dictionary<string, int> Dancers = new Dictionary<string, int>();
		public int MinHeats = 0; // Will be equal to number of heats top dancer applied to

		private const int MIN_DESIRED_CAPACITY = 4;
		private const int MAX_DESIRED_CAPACITY = 6;

		public void IncludeDancerInStats(string Dancer, int NumDances)
		{
			string upperdancer = Dancer.ToUpperInvariant();
			if (!Dancers.ContainsKey(upperdancer))
			{
				Dancers.Add(upperdancer, NumDances);
			}
			else
			{
				Dancers[upperdancer] += NumDances;
			}
		}

		public bool AddPair(DancingPair Pair, int NumDances)
		{
			log.InfoFormat("{0}: Adding pair <{1}>-<{2}> num {3}", Name, Pair.Dancer1, Pair.Dancer2, NumDances);
			if (Pair.IsValidDancers)
			{
				InitialPool.Pairs.AddRange(Enumerable.Repeat(Pair, NumDances));

				// Accumulate statistics
				IncludeDancerInStats(Pair.Dancer1, NumDances);
				IncludeDancerInStats(Pair.Dancer2, NumDances);
			}
			return Pair.IsValidDancers;
		}

		public bool CreateInitialHeats()
		{
			// Start with at least 15 heats but no less than top dancer
			if (InitialPool.Pairs.Count > 0)
			{
				MinHeats = Dancers.Values.Max();
				int InitialHeats = Math.Max(MinHeats, 15);

				string TopDancer = Dancers.First(d => d.Value == MinHeats).Key;
				log.InfoFormat("For dance <{0}> top dancer is <{1}> with {2} heats participation. {3} heats will be initially created.",
					Name, TopDancer, MinHeats, InitialHeats);

				// Each heat should be new reference
				for (int i = 0; i < InitialHeats; i++)
				{
					Heats.Add(new Heat());
				}
			}
			return Heats.Count > 0;
		}

		public Heat FindHeatForPair(DancingPair pair)
		{
			Heat result = null;

			if (Heats.Count > 0)
			{
				// Try random heat 50 times having max desired capacity in mind
				for (int i = 0; result == null && i < 50; i++)
				{
					int index = Scheduler.glbRandom.Next(Heats.Count);
					Heat curr = Heats[index];
					if (curr.Pairs.Count < MAX_DESIRED_CAPACITY && curr.CanAddPair(pair))
					{
						result = curr;
					}
				}

				// If not found - try to find any heat despite max desired capacity
				for (int i = 0; result == null && i < Heats.Count; i++)
				{
					Heat curr = Heats[i];
					if (curr.CanAddPair(pair))
					{
						result = curr;
					}
				}
			}

			return result;
		}

		public bool SeedPairsToHeats()
		{
			while(InitialPool.Pairs.Count > 0)
			{
				DancingPair pair = InitialPool.Pairs[0];
				Heat heat = FindHeatForPair(pair);
				if (heat != null)
				{
					heat.Pairs.Add(pair);
					InitialPool.Pairs.RemoveAt(0);
				}
				else
				{
					log.ErrorFormat("Fatal error: cannot find a heat for pair {0}: <{1}> - <{2}>!", pair.Team, pair.Dancer1, pair.Dancer2);
					break;
				}
			}
			
			// Succeeded if all pairs transferred to heats
			return InitialPool.Pairs.Count == 0;
		}

		public void DispatchHighAttendeeHeats()
		{
			List<Heat> tmpList = Heats.FindAll(h => h.Pairs.Count > MAX_DESIRED_CAPACITY);
			Heats.RemoveAll(h => tmpList.Contains(h));
			log.InfoFormat("High atendees heats num = {0} moved from main list num = {1}", tmpList.Count, Heats.Count);

			while (tmpList.Count > 0)
			{
				Heat currHeat = tmpList[0];
				log.InfoFormat("Attempting to lower attendees in temporary heat ***, num = {0}", currHeat.Pairs.Count);
				while (currHeat.Pairs.Count > MAX_DESIRED_CAPACITY)
				{
					bool anyfound = false;
					foreach(DancingPair pair in currHeat.Pairs)
					{
						Heat allowedHeat = FindHeatForPair(pair);
						if (allowedHeat != null)
						{
							anyfound = true;
							log.InfoFormat("Moving out pair <{0}>-<{1}>", pair.Dancer1, pair.Dancer2);
							allowedHeat.Pairs.Add(pair);
							currHeat.Pairs.Remove(pair);
							break;
						}
					}
					if (!anyfound)
					{
						break;
					}
				}
				log.InfoFormat("Moving back temporary heat ***, num = {0}", currHeat.Pairs.Count);
				Heats.Add(currHeat);
				tmpList.RemoveAt(0);
			}
		}

		public void DispatchLowAttendeesHeats()
		{
			// Remove empty heats
			log.InfoFormat("Number of heats in <{0}> before removal of empty: {1}", Name, Heats.Count);
			Heats.RemoveAll(h => h.Pairs.Count == 0);
			log.InfoFormat("Number of heats in <{0}> after removal of empty: {1}", Name, Heats.Count);

			if (Heats.Count > MinHeats)
			{
				List<Heat> tmpList = Heats.FindAll(h => h.Pairs.Count < MIN_DESIRED_CAPACITY);
				Heats.RemoveAll(h => tmpList.Contains(h));
				log.InfoFormat("Low atendees heats num = {0} moved from main list num = {1}", tmpList.Count, Heats.Count);

				while (tmpList.Count > 0)
				{
					Heat currHeat = tmpList[0];
					Dictionary<DancingPair, Heat> dispatchList = new Dictionary<DancingPair, Heat>();
					foreach (DancingPair pair in currHeat.Pairs)
					{
						Heat allowedHeat = FindHeatForPair(pair);
						if (allowedHeat != null)
						{
							dispatchList.Add(pair, allowedHeat);
						}
						else
						{
							break;
						}
					}
					if (dispatchList.Count != currHeat.Pairs.Count)
					{
						log.InfoFormat("Cannot dispatch temporary heat ***, will move to main list.");
						Heats.Add(currHeat);
					}
					else
					{
						log.InfoFormat("Temporary heat *** will be dispatched to remaining heats.");
						foreach (var p in dispatchList)
						{
							p.Value.Pairs.Add(p.Key);
						}
					}
					currHeat.DumpToLog("Temporary heat ***");
					tmpList.RemoveAt(0);

					// TODO: If total number of heats qual to minimal, return all tmp heats to main list and stop iterations.
				}
			}
			else
			{
				log.Info("Low attendee heats not dispatched because number of heats minimal already.");
			}
		}

		public bool PopulateHeats()
		{
			log.InfoFormat("Populate heats for <{0}>...", Name);
			bool ret = CreateInitialHeats();
			if (ret)
			{
				InitialPool.DumpToLog("Before sorting");
				ret = SeedPairsToHeats();
				if (ret)
				{
					DumpHeatsToLog("After first seeding");

					// Try to free up some space in overfilled heats (num > 6)
					DispatchHighAttendeeHeats();
					DumpHeatsToLog("After dispatching high attendee heats");

					// Remove empty and try to dispatch low capacity heats (num < 4)
					DispatchLowAttendeesHeats();
					DumpHeatsToLog("After dispatching low attendee heats");

					// Second attempt to level down overfilled heats (num > 6)
					DispatchHighAttendeeHeats();
					DumpHeatsToLog("After second dispatching (leveling down) high attendee heats");
				}
			}
			else
			{
				log.ErrorFormat("Cannot populate dance {0} - no heats created for the dance!", Name);
			}

			return ret;
		}

		public void DumpHeatsToLog(string title)
		{
			log.InfoFormat("Printing Heats for dance <{0}>, stage <{1}> total {2} heats.", Name, title, Heats.Count);
			for (int i = 0; i < Heats.Count; i++)
			{
				Heats[i].DumpToLog(String.Format("Heat #{0}", i + 1));
			}
		}
	}
}
