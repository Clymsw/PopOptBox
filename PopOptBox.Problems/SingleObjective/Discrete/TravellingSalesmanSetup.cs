using System;
using System.Collections.Generic;
using System.IO;

namespace PopOptBox.Problems.SingleObjective.Discrete
{
    public class TravellingSalesmanSetup
    {
        #region Constructor

        public TravellingSalesmanSetup(string problemFilePath)
        {
            Locations = new List<double[]>();
            ParseProblemFile(problemFilePath);

            string solutionFilePath = problemFilePath.Replace(".tsp", ".opt.tour");
            ParseSolutionFile(solutionFilePath);
        }

        #endregion

        #region Fields

        public string ShortName { get; private set; }
        public string LongName { get; private set; }
        public List<double[]> Locations { get; private set; }
        public int[] OptimumRoute { get; private set; }

        #endregion

        private void ParseProblemFile(string filePath)
        {
            using (var reader = new StreamReader(File.OpenRead(@filePath)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    // See if it's a header line
                    var testForHeader = line.Split(':');

                    if (testForHeader[0] == "NAME")
                    {
                        ShortName = testForHeader[1];
                        continue;
                    }

                    if (testForHeader[0] == "COMMENT")
                    {
                        LongName = testForHeader[1];
                        continue;
                    }

                    // See if there's useful data
                    var testForData = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    if (testForData[0] == "EOF")
                        break;

                    int.TryParse(testForData[0], out int cityIndex);

                    if (testForData.Length == 3)
                    {
                        //We've reached the coordinates
                        double.TryParse(testForData[1], out double x);
                        double.TryParse(testForData[2], out double y);
                        Locations.Add(new double[2] { x, y });
                    }
                }
            }
        }

        private void ParseSolutionFile(string filePath)
        {
            using (var reader = new StreamReader(File.OpenRead(@filePath)))
            {
                List<int> optimumTour = new List<int>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    var test = line.Split(':');

                    if (test[0] == "EOF")
                        break;

                    int.TryParse(test[0], out int cityIndex);

                    if (cityIndex > 0)
                    {
                        optimumTour.Add(cityIndex - 1);
                    }
                }

                // Routes return to origin
                optimumTour.Add(optimumTour[0]);

                OptimumRoute = optimumTour.ToArray();
            }
        }
    }
}
