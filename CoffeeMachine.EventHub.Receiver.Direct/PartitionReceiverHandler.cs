using Microsoft.Azure.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMachine.EventHub.Receiver.Direct
{
    public class PartitionReceiverHandler : IPartitionReceiveHandler
    {
        private readonly string partitionId;
        public int MaxBatchSize { get; set; }

        public PartitionReceiverHandler(string partitionId)
        {
            this.partitionId = partitionId;
            this.MaxBatchSize = 5;
        }

        public Task ProcessErrorAsync(Exception error)
        {
            Console.WriteLine($"Exception: {error.Message}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(IEnumerable<EventData> events)
        {
            if (events != null)
            {
                foreach (var eventData in events)
                {
                    var dataAsJson = Encoding.UTF8.GetString(eventData.Body.Array);
                   
                    Console.WriteLine($"{dataAsJson} | PartitionId: {partitionId}" +
                      $" | Offset: {eventData.SystemProperties.Offset}");
                }
            }
            return Task.CompletedTask;
        }
    }
}
