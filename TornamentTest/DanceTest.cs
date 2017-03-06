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

			bool actual = Waltz.AddPair(pair1, 1);
			Assert.IsTrue(actual, "Function should return true.");

			actual = Waltz.AddPair(pair2, 2);
			Assert.IsTrue(actual, "Function should return true.");

			actual = Waltz.AddPair(pair3, 2);
			Assert.IsTrue(actual, "Function should return true.");

			Assert.AreEqual(5, Waltz.Dancers.Count, "Number of dancers should match.");

			Assert.AreEqual(1, Waltz.Dancers["Petya"], "Number of heats for dancer should match.");
			Assert.AreEqual(3, Waltz.Dancers["Maria"], "Number of heats for dancer should match.");
			Assert.AreEqual(2, Waltz.Dancers["Nikolai"], "Number of heats for dancer should match.");
			Assert.AreEqual(2, Waltz.Dancers["Kostya"], "Number of heats for dancer should match.");
			Assert.AreEqual(2, Waltz.Dancers["Anton"], "Number of heats for dancer should match.");
		}

		[TestMethod]
		public void AddPair_BadDancers_PairNotAdded()
		{
			Dance Waltz = new Dance { Name = "Waltz", isRythm = false };

			DancingPair pair1 = new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "", Instructor = "Vasya", Rank = "American Beginner" };
			DancingPair pair2 = new DancingPair { Team = Teams.Blue, Dancer1 = "", Dancer2 = "Nikolai", Instructor = "Vasya", Rank = "American Bronze" };
			DancingPair pair3 = new DancingPair { Team = Teams.Blue, Dancer1 = "Kostya", Dancer2 = "Kostya", Instructor = "Vasya", Rank = "American Beginner" };

			bool actual = Waltz.AddPair(pair1, 1);
			Assert.IsFalse(actual, "Function should return false with empty dancer.");

			actual = Waltz.AddPair(pair2, 2);
			Assert.IsFalse(actual, "Function should return false with empty dancer.");

			actual = Waltz.AddPair(pair3, 2);
			Assert.IsFalse(actual, "Function should return false with same dancer.");

			Assert.AreEqual(0, Waltz.Dancers.Count, "No pairs should be added during test.");
		}
	}
}
