using Microsoft.Azure.EventHubs;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Threading.Tasks;
using System.Linq;

namespace CoffeeMachine.EventHub.Receiver.Direct
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Connecting to the Event Hub...");

            IConfiguration configuration = new ConfigurationBuilder()
                                                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                                .AddJsonFile("AppSettings.json", true, true)
                                                .Build();

            var eventHubConnectionString = configuration.GetConnectionString("EventHubConnectionString");
            var eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString);
            var runtimeInfo = await eventHubClient.GetRuntimeInformationAsync();
            var partitions = runtimeInfo.PartitionIds;

            var partitionReceivers = partitions.Select(x=> eventHubClient.CreateReceiver("$Default", x, EventPosition.FromEnqueuedTime(DateTime.Now)));

            Console.WriteLine("Waiting for incoming events....");

            foreach (var partitionReceiver in partitionReceivers)
            {
                partitionReceiver.SetReceiveHandler(new PartitionReceiverHandler(partitionReceiver.PartitionId));
            }

            Console.WriteLine("Press any key to exit....");
            Console.ReadLine();
            await eventHubClient.CloseAsync();
        }
    }
}
