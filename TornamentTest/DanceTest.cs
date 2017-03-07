using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net.Config;

using FirstStudioTournamentScheduler;

namespace TournamentTest
{
	[TestClass]
	public class DanceTest
	{
		private TestContext testContext;
		public TestContext TestContext
		{
			get { return testContext; }
			set { testContext = value; }
		}

		[AssemblyInitialize()]
		public static void InitializeAssembly(TestContext ctx)
		{
			// Configure log4net based on the App.config
			XmlConfigurator.Configure();
		}

		[TestMethod]
		public void Dance_AddPair_PairsAdded()
		{
			Dance Waltz = new Dance { Name = "Waltz", isRythm = false };

			DancingPair pair1 = new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "Maria", Instructor = "Vasya", Rank = "American Beginner" };
			DancingPair pair2 = new DancingPair { Team = Teams.Blue, Dancer1 = "Maria", Dancer2 = "Nikolai", Instructor = "Vasya", Rank = "American Bronze" };
			DancingPair pair3 = new DancingPair { Team = Teams.Blue, Dancer1 = "Kostya", Dancer2 = "Anton", Instructor = "Vasya", Rank = "American Beginner" };

			bool actual = Waltz.AddPair(pair1, 1);
			Assert.IsTrue(actual, "Function should return true.");

			actual = Waltz.AddPair(pair2, 2);
			Assert.IsTrue(actual, "Function should return true.");

			actual = Waltz.AddPair(pair3, 3);
			Assert.IsTrue(actual, "Function should return true.");

			Assert.AreEqual(6, Waltz.InitialPool.Pairs.Count, "Number of pairs added should match.");

			Assert.AreEqual(5, Waltz.Dancers.Count, "Number of dancers should match.");

			Assert.AreEqual(1, Waltz.Dancers["Petya"], "Number of heats for dancer should match.");
			Assert.AreEqual(3, Waltz.Dancers["Maria"], "Number of heats for dancer should match.");
			Assert.AreEqual(2, Waltz.Dancers["Nikolai"], "Number of heats for dancer should match.");
			Assert.AreEqual(3, Waltz.Dancers["Kostya"], "Number of heats for dancer should match.");
			Assert.AreEqual(3, Waltz.Dancers["Anton"], "Number of heats for dancer should match.");
		}

		[TestMethod]
		public void Dance_AddPair_BadDancers_PairNotAdded()
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

			Assert.AreEqual(0, Waltz.InitialPool.Pairs.Count, "No pairs should be added during test.");

			Assert.AreEqual(0, Waltz.Dancers.Count, "No dancers should be added during test.");
		}

		[TestMethod]
		public void Dance_CreateInitialHeats_TopDancerLessThan15_15HeatsCreated()
		{
			Dance Waltz = new Dance { Name = "Waltz", isRythm = false };

			DancingPair pair1 = new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "Maria", Instructor = "Vasya", Rank = "American Beginner" };
			DancingPair pair2 = new DancingPair { Team = Teams.Blue, Dancer1 = "Maria", Dancer2 = "Nikolai", Instructor = "Vasya", Rank = "American Bronze" };
			DancingPair pair3 = new DancingPair { Team = Teams.Blue, Dancer1 = "Kostya", Dancer2 = "Anton", Instructor = "Vasya", Rank = "American Beginner" };

			Waltz.AddPair(pair1, 1);
			Waltz.AddPair(pair2, 2);
			Waltz.AddPair(pair3, 3);

			bool actual = Waltz.CreateInitialHeats();
			Assert.IsTrue(actual, "CreateInitialHeats() should succeed");

			// Maria is top dancer applied to 3 dances
			Assert.AreEqual(15, Waltz.Heats.Count, "Number of heats added should match.");
			Assert.AreEqual(3, Waltz.MinHeats, "Minimal number of heats should match.");
		}

		[TestMethod]
		public void Dance_CreateInitialHeats_TopDancerMoreThan15_MoreHeatsCreated()
		{
			Dance Waltz = new Dance { Name = "Waltz", isRythm = false };

			DancingPair pair1 = new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "Maria", Instructor = "Vasya", Rank = "American Beginner" };
			DancingPair pair2 = new DancingPair { Team = Teams.Blue, Dancer1 = "Maria", Dancer2 = "Nikolai", Instructor = "Vasya", Rank = "American Bronze" };
			DancingPair pair3 = new DancingPair { Team = Teams.Blue, Dancer1 = "Kostya", Dancer2 = "Anton", Instructor = "Vasya", Rank = "American Beginner" };

			Waltz.AddPair(pair1, 1);
			Waltz.AddPair(pair2, 20); 
			Waltz.AddPair(pair3, 3);

			bool actual = Waltz.CreateInitialHeats();
			Assert.IsTrue(actual, "CreateInitialHeats() should succeed");

			// Maria is top dancer applied to 21 dances
			Assert.AreEqual(21, Waltz.Heats.Count, "Number of heats added should match.");
			Assert.AreEqual(21, Waltz.MinHeats, "Minimal number of heats should match.");
		}

		[TestMethod]
		public void Dance_FindHeatForPair_EmptyHeats_RandomHeatSelected()
		{
			Dance Waltz = new Dance { Name = "Waltz", isRythm = false };

			DancingPair pair1 = new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "Maria", Instructor = "Vasya", Rank = "American Beginner" };
			Waltz.AddPair(pair1, 1);
			Waltz.CreateInitialHeats();

			DancingPair pair2 = new DancingPair { Team = Teams.Blue, Dancer1 = "Tanya", Dancer2 = "Nikolai", Instructor = "Vasya", Rank = "American Bronze" };

			// Make 5 attempts to get heat, at least once it should be not first
			bool bRandomHeatFound = false;
			for (int i = 0; !bRandomHeatFound && i < 5; i++)
			{
				Heat curr = Waltz.FindHeatForPair(pair2);
				bRandomHeatFound = !Object.ReferenceEquals(curr, Waltz.Heats[0]);
			}

			Assert.IsTrue(bRandomHeatFound, "Pairs should be populated to heats in random order.");
		}

		[TestMethod]
		public void Dance_PopulateHeats_HeatsPopulated()
		{
			Dance Waltz = new Dance { Name = "Waltz", isRythm = false };

			DancingPair pair1 = new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "Maria", Instructor = "Vasya", Rank = "American Beginner" };
			DancingPair pair2 = new DancingPair { Team = Teams.Blue, Dancer1 = "Maria", Dancer2 = "Nikolai", Instructor = "Vasya", Rank = "American Bronze" };
			DancingPair pair3 = new DancingPair { Team = Teams.Blue, Dancer1 = "Kostya", Dancer2 = "Anton", Instructor = "Vasya", Rank = "American Beginner" };

			Waltz.AddPair(new DancingPair { Team = Teams.Blue, Dancer1 = "Eileen" }, 1);
			Waltz.AddPair(pair2, 9);
			Waltz.AddPair(pair3, 3);

			bool actual = Waltz.PopulateHeats();
			Assert.IsTrue(actual, "PopulateHeats() should succeed");
		}
	}
}
