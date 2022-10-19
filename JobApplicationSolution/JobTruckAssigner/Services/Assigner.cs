using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using JobTruckAssigner.Models;

namespace JobTruckAssigner.Services
{
    public class Assigner
    {
        private Dictionary<Job, Truck> assignments = new();
        private Dictionary<Job, List<Truck>> jobTruckCapabilities = new();

        private int noOfAssignmentProcesses = 0;

        public void ProcessAssignment(IEnumerable<Truck> trucks, IEnumerable<Job> jobs)
        {
            foreach (var job in jobs)
            {
                assignments.Add(job, null);
                jobTruckCapabilities.Add(job, new List<Truck>());
            }

            foreach (var truck in trucks)
            {
                foreach (var jobTypeLetter in truck.JobTypeLetters)
                {
                    foreach (var jobTruckCapability in jobTruckCapabilities)
                    {
                        var currentJobTypeLetter = jobTruckCapability.Key.Type;

                        if (currentJobTypeLetter == jobTypeLetter)
                        {
                            jobTruckCapability.Value.Add(truck);
                        }
                    }
                }
            }

            var isSolutionFound = false;

            noOfAssignmentProcesses++;
            AssignTruckToJobs(ref jobTruckCapabilities, ref assignments, ref isSolutionFound);

            DisplayStatistics();

            var limitOfAssignmentFiningTrials = 5;

            while (GetJobsWithNoTrucksAssigned().Count > 0)
            {
                if (limitOfAssignmentFiningTrials == 0)
                    break;

                noOfAssignmentProcesses++;
                TryAssignmentFining(GetJobsWithNoTrucksAssigned(), jobTruckCapabilities, assignments);

                DisplayStatistics();

                limitOfAssignmentFiningTrials--;
            }

            Logger.Log($"The Job-Truck assignment process has been carried out . The final statistics of the assignment is listed below . {Environment.NewLine}");

            DisplayStatistics();
        }

        private List<Job> GetJobsWithNoTrucksAssigned()
        {
            var jobsWithNoTruckAssigned = new List<Job>();
            foreach (var assignment in assignments)
            {
                var job = assignment.Key;
                var truck = assignment.Value;

                if (truck == null)
                    jobsWithNoTruckAssigned.Add(job);
            }

            return jobsWithNoTruckAssigned;
        }

        private int i = -1;
        private void AssignTruckToJobs(ref Dictionary<Job, List<Truck>> jobTruckCapabilities, ref Dictionary<Job, Truck> assignments, ref bool isSolutionFound)
        {
            i++;

            if (i > jobTruckCapabilities.Count - 1) return;

            var j = 0;

            var currentJobTrucksAssignment = jobTruckCapabilities.ElementAt(i);
            var currentJob = jobTruckCapabilities.ElementAt(i).Key;

            while (!isSolutionFound && j < currentJobTrucksAssignment.Value.Count)
            {
                j++;

                var capableTrucks = currentJobTrucksAssignment.Value;

                var isTruckFoundForJob = false;
                foreach (var truck in capableTrucks)
                {
                    if (!truck.HasBeenAssigned)
                    {
                        truck.HasBeenAssigned = true;
                        isTruckFoundForJob = true;

                        if (isTruckFoundForJob)
                        {
                            assignments[currentJob] = truck;

                            break;
                        }
                    }
                }

                var nextJobTrucksAssignment = assignments.ElementAtOrDefault(i);

                // A FULL SCAN HAS BEEN CARRIED OUT
                if (nextJobTrucksAssignment.Key == null)
                {
                    isSolutionFound = true;
                }
                else
                {
                    AssignTruckToJobs(ref jobTruckCapabilities, ref assignments, ref isSolutionFound);
                }
            }
        }

        private void TryAssignmentFining(List<Job> jobsWithNoTruckAssigned, Dictionary<Job, List<Truck>> jobTruckCapabilities, Dictionary<Job, Truck> assignments)
        {
            foreach (var assignment in assignments)
            {
                var truck = assignment.Value;

                if (truck != null)
                    truck.HasBeenAssigned = false;
            }

            foreach (var job in jobsWithNoTruckAssigned)
            {
                var capableTrucks = jobTruckCapabilities[job];

                var isTruckFoundForJob = false;

                foreach (var truck in capableTrucks)
                {
                    if (!truck.HasBeenAssigned)
                    {
                        truck.HasBeenAssigned = true;
                        isTruckFoundForJob = true;

                        if (isTruckFoundForJob)
                        {
                            assignments[job] = truck;
                            break;
                        }
                    }
                }
            }
        }

        private void DisplayStatistics()
        {
            var truckAssignments = new Dictionary<Truck, int>();
            foreach (var assignment in assignments)
            {
                var truck = assignment.Value;

                if (truck != null)
                {
                    if (!truckAssignments.ContainsKey(truck))
                    {
                        truckAssignments.Add(truck, 1);
                    }
                    else
                    {
                        truckAssignments[truck] += 1;
                    }
                }
            }

            var noOfTrucksAssignedToJobs = truckAssignments.Where(x => x.Value > 0).Count();
            Logger.Log($"{noOfTrucksAssignedToJobs} truck(s) was/were assigned at least one job .");

            var noOfTrucksAssignedOneJob = truckAssignments.Where(x => x.Value == 1).Count();
            Logger.Log($"{noOfTrucksAssignedOneJob} truck(s) was/were assigned one job .");

            var noOfTrucksAssignedWithMoreThanOneJob = truckAssignments.Where(x => x.Value > 1).Count();
            Logger.Log($"{noOfTrucksAssignedWithMoreThanOneJob} truck(s) was/were assigned more than one job .");

            Logger.Log($"Number of jobs with no trucks assigned after the assignment process #{noOfAssignmentProcesses}: {GetJobsWithNoTrucksAssigned().Count} .");

            Logger.Log(Environment.NewLine);
        }
 
    }
}