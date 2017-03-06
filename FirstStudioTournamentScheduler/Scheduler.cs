using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace FirstStudioTournamentScheduler
{

    public class Scheduler
    {
        List<DancingPair> DancingPairs = new List<DancingPair>();
        enum FormFields
        {
            Team = 0,
            Dancer1 = 1,
            Dancer2 = 2,
            Instructor = 3,
            Rank = 4,
            NumWaltz = 5,
            NumTango = 6

        }
        const int MIN_REQ_FIELDS = 6;

        Dance Waltz = new Dance { Name = "Waltz", isRythm = false};
        Dance Tango = new Dance { Name = "Tango", isRythm = false};

        public int ParseNumDances(string NumDances, string FullLine)
        {
            int ret = 0;
            if (!String.IsNullOrEmpty(NumDances))
            {
                try
                {
                    ret = int.Parse(NumDances);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Unable to parse num of dances {0} in {1}", NumDances, FullLine);
                }
            }
            return ret;
        }

        public void ReadParticipants()
        {
            // Browse for file
            string filename = "C:\\Test\\foo.csv";

            if (!File.Exists(filename))
            {
                Console.WriteLine("Input file {0} not found!", filename);
                return;
            }

            // Open file
            StreamReader sr = new StreamReader(filename);

            // Store in data structure
            // Content of each line in .csv file
            // Team, Dancer1, Dancer2, Instructor, Rank/Comment, (W)altz, (T)ango, (F)oxtrot, (R)umba,
            // (S)wing, (B)olero, (C)hacha, (A)Bachata, (H)ustle

            int NumFields = Enum.GetValues(typeof(FormFields)).Length;

            while (!sr.EndOfStream)
            {
                string CurrLine = sr.ReadLine();
                string[] Line = CurrLine.Split(','); // Read file content line by line;

                if (Line.Count() > 0)
                {
                    try
                    {
                        Teams Team = (Teams)Enum.Parse(typeof(Teams), Line[(int)FormFields.Team], true);

                        if (Line.Count() > MIN_REQ_FIELDS)
                        {
                            DancingPair pair = new DancingPair();
                            pair.Team = Team;
                            pair.Dancer1 = Line[(int)FormFields.Dancer1];
                            pair.Dancer2 = Line[(int)FormFields.Dancer2];
                            pair.Instructor = Line[(int)FormFields.Instructor];
                            pair.Rank = Line[(int)FormFields.Rank];

                            if (Line.Count() > (int)FormFields.NumWaltz)
                            {
                                int NumWaltz = ParseNumDances(Line[(int)FormFields.NumWaltz], CurrLine);
                                if (NumWaltz > 0)
                                {
                                    pair.AppliedDances[Waltz] = NumWaltz;
                                }
                            }

                            if (Line.Count() > (int)FormFields.NumTango)
                            {
                                int NumTango = ParseNumDances(Line[(int)FormFields.NumTango], CurrLine);
                                if (NumTango > 0)
                                {
                                    pair.AppliedDances[Tango] = NumTango;
                                }
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Unable to parse team {0}", Line[0]);
                    }
                }
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }

    }
}
