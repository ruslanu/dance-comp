using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using FirstStudioTournamentScheduler;

namespace TournamentTest
{
    [TestClass]
    public class DanceTest
    {
        [TestMethod]
        public void ParseNumDances_TestCases()
        {
            Scheduler scheduler = new Scheduler();

            int actual = scheduler.ParseNumDances("5", "Blue,Dancer1,Dancer2,Instructor,NoCare,5");
            Assert.AreEqual(5, actual, "Field should be parsed correctly!");
        }
    }
}
