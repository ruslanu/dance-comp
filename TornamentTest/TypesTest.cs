using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FirstStudioTournamentScheduler;

namespace TornamentTest
{
	[TestClass]
	public class TypesTest
	{
		[TestMethod]
		public void Heat_CanAddPair_NewDancers_ExpectedTrue()
		{
			Heat heat = new Heat();
			heat.Pairs.Add(new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "Maria", Instructor = "Vasya", Rank = "American Beginner" });
			heat.Pairs.Add(new DancingPair { Team = Teams.Blue, Dancer1 = "Kostya", Dancer2 = "Anton", Instructor = "Vasya", Rank = "American Beginner" });

			bool actual = heat.CanAddPair(new DancingPair { Team = Teams.Blue, Dancer1 = "Jahon", Dancer2 = "Shukhrat", Instructor = "Vasya", Rank = "American Beginner" });
			Assert.IsTrue(actual, "Pair should be allowed to add.");
		}

		[TestMethod]
		public void Heat_CanAddPair_ExistingDancer_ExpectedFalse()
		{
			Heat heat = new Heat();
			heat.Pairs.Add(new DancingPair { Team = Teams.Blue, Dancer1 = "Petya", Dancer2 = "Maria", Instructor = "Vasya", Rank = "American Beginner" });
			heat.Pairs.Add(new DancingPair { Team = Teams.Blue, Dancer1 = "Kostya", Dancer2 = "Anton", Instructor = "Vasya", Rank = "American Beginner" });

			bool actual = heat.CanAddPair(new DancingPair { Team = Teams.Blue, Dancer1 = "Jahon", Dancer2 = "Kostya", Instructor = "Vasya", Rank = "American Beginner" });
			Assert.IsFalse(actual, "Pair shouldn't be allowed to add.");
		}
	}
}
