using JobTruckAssigner.Models;

namespace JobTruckAssigner.Services
{
    public static class InputProcessor
    {
        private static readonly string delimiter = " ";

        public static (IEnumerable<Truck> trucks, IEnumerable<Job> jobs) ProcessInput(string inputFilePath)
        {
            var lines = File.ReadAllLines(inputFilePath);

            int numberOfTrucksStated;
            if (!int.TryParse(lines[0], out numberOfTrucksStated))
                Utilities.RaiseException("Number of trucks not provided in the input data file .");

            var truckLines = lines[0..(numberOfTrucksStated + 1)];
            var jobLines = lines[(numberOfTrucksStated + 1)..lines.Length];

            var jobs = ProcessJobData(jobLines);
            var trucks = ProcessTruckData(truckLines, jobs);

            return (trucks, jobs);
        }

        private static IEnumerable<Job> ProcessJobData(IEnumerable<string> jobLines)
        {
            var jobs = new List<Job>();

            if (jobLines == null || jobLines.Count() < 2)
                Utilities.RaiseException("Job data not correctly provided . Please check the input data .");

            for (var i = 0; i < jobLines.Count(); i++)
            {
                var job = new Job();
                int jobID;

                // FIRST DETAIL SHOULD BE THE NUMBER OF JOBS STATED
                if (i == 0)
                {
                    continue;
                }
                else
                {
                    var jobLine = jobLines.ElementAt(i);
                    var jobDetails = jobLine.Split(delimiter);

                    if (jobDetails.Length != 2)
                        Utilities.RaiseException($"Some job data was not provided in the correct format . The data provided as job data was: {jobLine} .");

                    for (var j = 0; j < jobDetails.Length; j++)
                    {
                        // CHECKING FOR JOB ID
                        if (j == 0)
                        {
                            var isJobIDProvided = int.TryParse(jobDetails[j], out jobID);

                            if (!isJobIDProvided)
                                Utilities.RaiseException($"Job ID not correctly provided at some job data . Data provided was: {jobDetails[j]} .");
                            else
                                job.ID = jobID;
                        }
                        else
                        {
                            var jobType = jobDetails[j].ToCharArray();

                            var actualJobTypeLetter = jobType[0];

                            if (jobType.Length > 1 || !char.IsLetter(actualJobTypeLetter))
                                Utilities.RaiseException($"Job type not correctly provided at some job data . Job type provided was: {jobDetails[j]} .");

                            job.Type = actualJobTypeLetter;
                        }
                    }
                }

                jobs.Add(job);
            }

            return jobs;
        }
        private static IEnumerable<Truck> ProcessTruckData(IEnumerable<string> truckLines, IEnumerable<Job> jobs)
        {
            var trucks = new List<Truck>();

            if (truckLines == null || truckLines.Count() < 2)
                Utilities.RaiseException("Truck data not correctly provided . Please check the input data .");

            for (var i = 0; i < truckLines.Count(); i++)
            {
                var truck = new Truck();
                int truckID;

                // FIRST DETAIL SHOULD BE THE NUMBER OF TRUCKS STATED
                if (i == 0)
                {
                    continue;
                }
                else
                {
                    var truckLine = truckLines.ElementAt(i);
                    var truckDetails = truckLine.Split(delimiter);

                    if (truckDetails.Length < 2)
                        Utilities.RaiseException($"Some truck data was not provided in the correct format . The data provided as truck data was: {truckLine} .");

                    for (var j = 0; j < truckDetails.Length; j++)
                    {
                        // CHECKING FOR TRUCK ID
                        if (j == 0)
                        {
                            var isTruckIDProvided = int.TryParse(truckDetails[j], out truckID);

                            if (!isTruckIDProvided)
                                Utilities.RaiseException($"Truck ID not correctly provided at some truck data . Data provided was: {truckDetails[i]} .");
                            else
                                truck.ID = truckID;
                        }
                        else
                        {
                            var jobType = truckDetails[j].ToCharArray();

                            var actualJobTypeLetter = jobType[0];
                            if (jobType.Length > 1 || !char.IsLetter(actualJobTypeLetter))
                                Utilities.RaiseException($"Job type not correctly provided at some truck data . Job type provided was: {truckDetails[j]} .");

                            truck.JobTypeLetters.Add(actualJobTypeLetter);
                        }
                    }
                }

                trucks.Add(truck);
            }

            return trucks;
        }

    }
}