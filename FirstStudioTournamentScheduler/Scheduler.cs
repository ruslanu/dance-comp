using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using log4net;
using log4net.Config;
using System.Text;

namespace FirstStudioTournamentScheduler
{

	public class Scheduler
	{
		private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public string SourceFileName = "entryforms.csv";
		public string DestFileName = "matchschedule.txt";

		public static Random glbRandom = new Random();

		List<DancingPair> DancingPairs = new List<DancingPair>();
		enum FormFields
		{
			Team = 0,
			Dancer1 = 1,
			Dancer2 = 2,
			Instructor = 3,
			Rank = 4,
			NumWaltz = 5,
			NumTango = 6,
			NumFoxtrot = 7,
			NumChacha = 8,
			NumRumba = 9,
			NumSwing = 10,
			NumHustle = 11,
			NumBolero = 12,
			NumBachata = 13,
		}
		const int MIN_REQ_FIELDS = 6;

		Dance Waltz = new Dance { Name = "Waltz", isRythm = false };
		Dance Tango = new Dance { Name = "Tango", isRythm = false };
		Dance Foxtrot = new Dance { Name = "Foxtrot", isRythm = false };
		Dance Chacha = new Dance { Name = "Cha-Cha", isRythm = true };
		Dance Rumba = new Dance { Name = "Rumba", isRythm = true };
		Dance Swing = new Dance { Name = "Swing", isRythm = true };
		Dance Hustle = new Dance { Name = "Hustle", isRythm = true };
		Dance Bolero = new Dance { Name = "Bolero", isRythm = true };
		Dance Bachata = new Dance { Name = "Bachata", isRythm = true };

		MatchBlock SmoothDances = new MatchBlock("Smooth and Standard Dances");
		MatchBlock RythmDances = new MatchBlock("Rythm and Latin Dances");

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
					log.ErrorFormat("Unable to parse num of dances {0} in {1}", NumDances, FullLine);
				}
			}
			return ret;
		}
		public void PrintFullMatchSchedule()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Blue, Red and White Team Match 03/12/2017");
			sb.AppendLine("Please report all sorting issues to Ruslan Usmanov to improve our algorithm. Thank you.");
			sb.AppendLine();

			SmoothDances.DumpSchedule(sb, 0);
			RythmDances.DumpSchedule(sb, SmoothDances.BlockHeats.Count);

			sb.AppendLine();
			sb.AppendLine("End of competition.");

			log.InfoFormat("Writing to file <{0}>", DestFileName);
			File.WriteAllText(DestFileName, sb.ToString());
		}

		public void ReadParticipants()
		{
			// Browse for file
			if (!File.Exists(SourceFileName))
			{
				log.ErrorFormat("Input file {0} not found!", SourceFileName);
				return;
			}

			log.InfoFormat("Reading file <{0}>", SourceFileName);
			string[] EntryForm = File.ReadAllLines(SourceFileName);

			// Store in data structure
			// Content of each line in .csv file
			// Team, Dancer1, Dancer2, Instructor, Rank/Comment, dances...

			int NumFields = Enum.GetValues(typeof(FormFields)).Length;

			foreach (string CurrLine in EntryForm)
			{
				string[] Line = CurrLine.Split(','); // Read file content line by line;

				if (Line.Count() > 0)
				{
					try
					{
						Teams Team = (Teams)Enum.Parse(typeof(Teams), Line[(int)FormFields.Team], true);

						if (Line.Count() >= MIN_REQ_FIELDS)
						{
							DancingPair pair = new DancingPair()
							{
								Team = Team,
								Dancer1 = Line[(int)FormFields.Dancer1],
								Dancer2 = Line[(int)FormFields.Dancer2],
								Instructor = Line[(int)FormFields.Instructor],
								Rank = Line[(int)FormFields.Rank],
							};

							if (Line.Count() > (int)FormFields.NumWaltz)
							{
								Waltz.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumWaltz], CurrLine));
							}

							if (Line.Count() > (int)FormFields.NumTango)
							{
								Tango.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumTango], CurrLine));
							}

							if (Line.Count() > (int)FormFields.NumFoxtrot)
							{
								Foxtrot.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumFoxtrot], CurrLine));
							}

							if (Line.Count() > (int)FormFields.NumChacha)
							{
								Chacha.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumChacha], CurrLine));
							}

							if (Line.Count() > (int)FormFields.NumRumba)
							{
								Rumba.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumRumba], CurrLine));
							}

							if (Line.Count() > (int)FormFields.NumSwing)
							{
								Swing.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumSwing], CurrLine));
							}

							if (Line.Count() > (int)FormFields.NumHustle)
							{
								Hustle.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumHustle], CurrLine));
							}

							if (Line.Count() > (int)FormFields.NumBolero)
							{
								Bolero.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumBolero], CurrLine));
							}

							if (Line.Count() > (int)FormFields.NumBachata)
							{
								Bachata.AddPair(pair, ParseNumDances(Line[(int)FormFields.NumBachata], CurrLine));
							}
						}
					}
					catch (ArgumentException)
					{
						log.ErrorFormat("Unable to parse team {0}", Line[0]);
					}
				}
			}

		}

		public void GenerateMatchSchedule()
		{
			log.Info("Begin generating team match schedule.");
			ReadParticipants();

			SmoothDances.Dances = new List<Dance> { Waltz, Tango, Foxtrot };
			SmoothDances.GenerateSchedule();

			RythmDances.Dances = new List<Dance> { Chacha, Rumba, Swing, Hustle, Bolero, Bachata };
			RythmDances.GenerateSchedule();

			PrintFullMatchSchedule();
			log.Info("Finished generating team match schedule.");
			log.Info("-----------------------------------------------------------");
		}


		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			XmlConfigurator.Configure();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());

		}

	}
}
