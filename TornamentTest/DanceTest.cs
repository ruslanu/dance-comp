using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FirstStudioTournamentScheduler;

namespace TournamentTest
{
	[TestClass]
	public class DanceTest
	{
		[TestMethod]
		public void AddPair_PairsAdded()
		{
			Dance Waltz = new Dance { Name = "Waltz", isRythm = false };

			DancingPair pair1 = new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "Maria", Instructor = "Vasya", Rank = "American Beginner" };
			DancingPair pair2 = new DancingPair { Team = Teams.Blue, Dancer1 = "Maria", Dancer2 = "Nikolai", Instructor = "Vasya", Rank = "American Bronze" };
			DancingPair pair3 = new DancingPair { Team = Teams.Blue, Dancer1 = "Kostya", Dancer2 = "Anton", Instructor = "Vasya", Rank = "American Beginner" };

			Waltz.AddPair(pair1, 1);
			Waltz.AddPair(pair2, 2);
			Waltz.AddPair(pair3, 2);

			Assert.AreEqual(5, Waltz.Dancers.Count, "Number of dancers should match.");

			Assert.AreEqual(1, Waltz.Dancers["Petya"], "Number of heats for dancer should match.");
			Assert.AreEqual(3, Waltz.Dancers["Maria"], "Number of heats for dancer should match.");
			Assert.AreEqual(2, Waltz.Dancers["Nikolai"], "Number of heats for dancer should match.");
			Assert.AreEqual(2, Waltz.Dancers["Kostya"], "Number of heats for dancer should match.");
			Assert.AreEqual(2, Waltz.Dancers["Anton"], "Number of heats for dancer should match.");
		}
	}
}
