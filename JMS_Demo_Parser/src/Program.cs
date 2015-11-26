﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JMS_Demo_Parser.src;
using DemoInfo;

namespace JMS_Demo_Parser
{
    class Program
    {
        private int roundEndedCount = 0;
        private DemoParser parser;
        private int i = 0;

        public Program(DemoParser parser)
        {
            this.parser = parser;

            parser.RoundEnd += (object sender, RoundEndedEventArgs e) => {
                // The reason I'm doing this after tick_done is that
                // entity-updates only come in the same tick as round_end
                // comes, meaning the score-update might not be transmitted yet
                // which would be sad - we always want the current score!
                // so we wait 1 second. 

                roundEndedCount = 1;
            };

            parser.TickDone += (object sender, TickDoneEventArgs e) => {
                if (roundEndedCount == 0)
                    return;

                roundEndedCount++;

                // Wait twice the tickrate of the demo (~2 seconds) to make sure the 
                // screen has been updated. I *LOVE* demo files :)
                if (roundEndedCount < parser.TickRate * 2)
                {
                    return;
                }

                roundEndedCount = 0;

                if(i == 0)
                {
                    Console.WriteLine("------------------------------------------------------------");

                    Console.WriteLine("Round {0}, CT: {1}, T: {2}", ++i, parser.CTScore, parser.TScore);
                    Console.WriteLine("Ts\t" + parser.TClanName);
                    Console.WriteLine("CTs\t" + parser.CTClanName);

                    Console.WriteLine();

                    //DatabaseConnection connection = new DatabaseConnection();
                    //connection.addTeam(parser.TClanName);
                    //connection.addTeam(parser.CTClanName);
                }
            };
        }

        static void Main(string[] args)
        {
            int demosParsed = 0;
            int errors = 0;
            String[] directories = Directory.GetDirectories(@"D:\demos\extracted\");

            foreach (String directory in directories)
            {
                if(directory.Length != 28 || !directory.StartsWith(@"D:\demos\extracted\demo190"))
                {
                    continue;
                }

                foreach(String file in Directory.GetFiles(directory))
                {
                    DemoParser parser = new DemoParser(File.OpenRead(file));

                    try
                    {
                        parser.ParseHeader();

                        Program program = new Program(parser);
                        parser.ParseToEnd();

                        demosParsed++;
                    } catch (Exception e)
                    {
                        parser = null;
                        errors++;
                        break;
                    }
                }

                if(demosParsed > 100)
                {
                    break;
                }
            }

            Console.WriteLine("Errors: " + errors);
            Console.WriteLine("Demos Parsed: " + demosParsed);

            Console.Read();
        }
    }
}
