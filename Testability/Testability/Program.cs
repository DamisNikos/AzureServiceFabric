using System;
using System.Collections.Generic;
using System.Fabric;
using System.Fabric.Testability.Scenario;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testability
{
    class Program
    {
        static int Main(string[] args)
        {
            string clusterConnection = "localhost:19080";
            Console.WriteLine("Starting Chaos Test Scenario...");
            try
            {
                RunChaosTestScenarioAsync(clusterConnection).Wait();
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("Chaos Test Scenario did not complete: ");
                foreach (Exception ex in ae.InnerExceptions)
                {
                    if (ex is FabricException)
                    {
                        Console.WriteLine("HResult: {0} Message: {1}", ex.HResult, ex.Message);
                    }
                }
                return -1;
            }
            Console.WriteLine("Chaos Test Scenario completed.");
            return 0;
        }

        static async Task RunChaosTestScenarioAsync(string clusterConnection)
        {
            TimeSpan maxClusterStabilizationTimeout = TimeSpan.FromSeconds(180);
            uint maxConcurrentFaults = 3;
            bool enableMoveReplicaFaults = true;

            FabricClient fabricClient = new FabricClient(clusterConnection);

            TimeSpan timeToRun = TimeSpan.FromMinutes(60);
            ChaosTestScenarioParameters scenarioParameters = new ChaosTestScenarioParameters(
                maxClusterStabilizationTimeout,
                maxConcurrentFaults,
                enableMoveReplicaFaults,
                timeToRun);

            ChaosTestScenario chaosScenario = new ChaosTestScenario(fabricClient,
        scenarioParameters);

            try
            {
                await chaosScenario.ExecuteAsync(CancellationToken.None);
            }
            catch (AggregateException ae)
            {
                throw ae.InnerException;
            }
        }
    }
}
