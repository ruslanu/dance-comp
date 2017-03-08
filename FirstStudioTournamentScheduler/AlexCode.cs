using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FirstStudioTournamentScheduler
{
    class AlexCode
    {
        class TeamData
        {
            string[] name1 = null;
            string[] name1_type = null;
            string[] name2 = null;
            string[] name2_type = null;
            List<string>[] dances = new List<string>[20];
            int totalParticipants = 0;

            void ReadParticipants()
            {
                //Browse for file
                string filename = "foo.txt";

                // Open file
                StreamReader sr = new StreamReader(filename);

                // index to use when copying lines from file
                int Row = 0;

                // Store in data structure
                // Content of each line in .csv file
                // Name1, (I)nstructor/(A)mateur, Name2, I/A, (W)altz, (T)ango, (F)oxtrot, (R)umba,
                // (S)wing, (B)olero, (C)hacha, (A)Bachata, (H)ustle

                while (!sr.EndOfStream)
                {
                    string[] Line = sr.ReadLine().Split(','); // Read file content line by line;
                    name1[Row] = Line[0];
                    name1_type[Row] = Line[1];
                    name2[Row] = Line[2];
                    name2_type[Row] = Line[3];
                    dances[Row] = new List<string>();
                    for (int i = 4; i < Line.Length; i++)
                    {
                        dances[Row].Add(Line[i]);
                    }
                    Row++;
                }
                totalParticipants = Row;
            }

            void ScheduleRounds()
            {
                // used to index through loops
                int i = 0;

                // Rules for scheduling
                // 1. No more than two heats in a row for the same person.
                // 2. 5-7 dances per heat
                // 3. Heat 69 should be a swing w/all amateur couples

                // Find person with the most amount of dances and base schedule around that person to determine
                // max rounds in each Smooth/Latin round.  For example, if a teacher has 20 dances in Latin and
                // want to go around 100 heats, then that teacher could be schedules every 5 heats - provided
                // there are enough dances to extend it that far.
                int maxDances = 0;
                string maxDancer1 = null;
                string maxDancer2 = null;

                for (i = 0; i < totalParticipants; i++)
                {
                    if (dances[i].Count > maxDances)
                    {
                        maxDances = dances[i].Count;
                        maxDancer1 = name1[i];
                        maxDancer2 = name2[i];
                    }

                }

                if (maxDances == 0)
                {
                    //Error message: Let user know .csv file is bad
                }

                // Also, find the total number of dances needed to determine if we should lean toward the 5 per
                // heat, if the amount of participants is light, or the 7 per heat, if it is heavy in participants.
                int accumulatedMaxDances = 0;
                int waltzDances = 0;
                //int tangoDances = 0;
                //int foxtrotDances = 0;
                //int rumbaDances = 0;
                //int chachaDances = 0;
                //int swingDances = 0;
                //int hustleDances = 0;
                //int bachataDances = 0;


                for (i = 0; i < totalParticipants; i++)
                {
                    foreach (string item in dances[i])
                    {
                        switch (item)
                        {
                            case "W":
                                waltzDances++;
                                break;
                            default: //errormsg
                                break;
                        }
                        accumulatedMaxDances++;
                    }

                }
            }
        }
    }
}
