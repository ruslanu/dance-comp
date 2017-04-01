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

		public void SeedPairsToHeats()
		{
			while(InitialPool.Pairs.Count > 0)
			{
				DancingPair pair = InitialPool.Pairs[0];
				Heat heat = FindHeatForPair(pair);
				if (heat == null)
				{
					log.ErrorFormat("Error: cannot find a heat for pair {0}: <{1}> - <{2}>! New heat will be created",
						pair.Team, pair.Dancer1, pair.Dancer2);
					heat = new Heat();
					Heats.Add(heat);
				}
				heat.Pairs.Add(pair);
				InitialPool.Pairs.RemoveAt(0);
			}
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

		public void DumpHeatsToLog(string title)
		{
			log.InfoFormat("Printing Heats for dance <{0}>, stage <{1}> total {2} heats.", Name, title, Heats.Count);
			for (int i = 0; i < Heats.Count; i++)
			{
				Heats[i].DumpToLog(String.Format("Heat #{0}", i + 1));
			}
		}

		#region new way of sorting

		public bool BalanceHeatsNewWay()
		{
			bool changed = false;

			int averagePairs = (int)Heats.Average(h => h.Pairs.Count);

			// Create list of heats larger than max desired and candidates to get pairs
			List<Heat> crowded = Heats.FindAll(h => h.Pairs.Count > averagePairs + 1);
			List<Heat> candidates = Heats.FindAll(h => h.Pairs.Count < averagePairs);
			log.InfoFormat("BalanceHeatsNewWay begin: {0} average pairs in heat, {1} overcrowded heats {2} candidates to get pairs.",
				averagePairs, crowded.Count, candidates.Count);

			// Sort crowded heats in descending order
			crowded.Sort((h1, h2) => h2.Pairs.Count.CompareTo(h1.Pairs.Count));

			for (int i = 0; i < crowded.Count; i++)
			{
				Heat currcrowded = crowded[i];
				// Sort candidates in ascending order
				candidates.Sort((h1, h2) => h1.Pairs.Count.CompareTo(h2.Pairs.Count));
				for (int j = 0; j < candidates.Count; j++)
				{
					Heat currcandidate = candidates[j];
					if (currcrowded.Pairs.Count <= currcandidate.Pairs.Count + 1)
					{
						// No need to continue, crowded heat downsized enough
						break;
					}
					while (currcrowded.Pairs.Count > currcandidate.Pairs.Count + 1)
					{
						foreach (DancingPair pair in currcrowded.Pairs)
						{
							if (currcandidate.CanAddPair(pair))
							{
								log.InfoFormat("Moving pair <{0}>-<{1}> from heat with {2} pairs to heat with {3} pairs",
									pair.Dancer1, pair.Dancer2, currcrowded.Pairs.Count, currcandidate.Pairs.Count);
								currcandidate.Pairs.Add(pair);
								currcrowded.Pairs.Remove(pair);
								changed = true;
								break;
							}
						}
					}
				}
			}

			log.InfoFormat("BalanceHeatsNewWay end: {0} overcrowded heats {1} underfilled heats.",
				Heats.Count(h => h.Pairs.Count > averagePairs + 1), Heats.Count(h => h.Pairs.Count < averagePairs));

			return changed;
		}

		public bool CreateInitialHeatsNewWay()
		{
			// Start with at least (n/6)+1 heats but no less than top dancer
			if (InitialPool.Pairs.Count > 0)
			{
				MinHeats = Dancers.Values.Max();
				int InitialHeats = Math.Max(MinHeats, (InitialPool.Pairs.Count / 6) + 1);

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

		public bool PopulateHeatsNewWay()
		{
			log.InfoFormat("Populate heats (new way) for <{0}>...", Name);
			bool ret = CreateInitialHeatsNewWay();
			if (ret)
			{
				InitialPool.DumpToLog("Before sorting");
				SeedPairsToHeats();
				DumpHeatsToLog("After first seeding");

				if (BalanceHeatsNewWay())
				{
					DumpHeatsToLog("After leveling down heats");
				}
				else
				{
					log.Info("No changes were made during attempt to rebalance heats.");
				}
			}
			else
			{
				log.ErrorFormat("Cannot populate dance {0} - no heats created for the dance!", Name);
			}
			log.InfoFormat("Finished to populate heats (new way) for <{0}>...", Name);

			return ret;
		}

		#endregion
	}
}
