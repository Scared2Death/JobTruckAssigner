using JobTruckAssigner.Services;

namespace JobTruckAssigner
{
    public class Program
    {
        private static readonly string inputFilePath = "./Data/jobs_tasks_assignment.txt";
        private static void Main(string[] args)
        {
            try
            {
                var trucksJobs = InputProcessor.ProcessInput(inputFilePath);

                var trucks = trucksJobs.trucks;
                Logger.Log($"Number of trucks: {trucks.Count()} .");

                var jobs = trucksJobs.jobs;
                Logger.Log($"Number of jobs: {jobs.Count()} .");

                Logger.Log(Environment.NewLine);

                var assigner = new Assigner();
                assigner.ProcessAssignment(trucks, jobs);
            }
            catch (Exception ex)
            {
                Logger.Log($"Some error occurred while trying to process the Job-Truck assignment task with the following details: {ex.Message}");
            }

            Console.ReadLine();
        }

    }
}