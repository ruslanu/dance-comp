using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FirstStudioTournamentScheduler
{
    class DancingPair
    {
        public Teams Team;
        public string Dancer1;
        public string Dancer2;
        public string Instructor;
        public string Rank;

        public Dictionary<Dance, int> AppliedDances = new Dictionary<Dance, int>();
    }
}
