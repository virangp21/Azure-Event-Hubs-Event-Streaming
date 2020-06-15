using CoffeeMachine.EventHub.Sender.Model;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CoffeeMachine.EventHub.Sender
{
    public interface ICoffeeMachineDataSender
    {
        Task SendDataAsync(CoffeeMachineData data);
        Task SendDataAsync(IEnumerable<CoffeeMachineData> lstData);


    }
    public class CoffeeMachineDataSender : ICoffeeMachineDataSender
    {
        private  EventHubClient _eventHubClient;
        public CoffeeMachineDataSender(string eventHubConnectionString)
        {
             _eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString);
        }
        public async Task SendDataAsync(CoffeeMachineData data)
        {
            var eventData = PrepareEventData(data);
            await _eventHubClient.SendAsync(eventData);
        }

        public async Task SendDataAsync(IEnumerable<CoffeeMachineData> lstData)
        {
            var lstEventData = lstData.Select(data => PrepareEventData(data));
            var eventDataBatch = _eventHubClient.CreateBatch();

            foreach (var eventData in lstEventData)
            {
                // If Event data cannot to be added to batch then batch is full
                if (!eventDataBatch.TryAdd(eventData))
                { 
                    await _eventHubClient.SendAsync(eventDataBatch.ToEnumerable()); // Send whatever is there in batch to event hub
                    eventDataBatch = _eventHubClient.CreateBatch(); // Create a new batch
                    eventDataBatch.TryAdd(eventData); // Add last event data that cannot be added to new batch
                }
            }

            if (eventDataBatch.Count > 0) // If there is any data in event batch 
            {
                await _eventHubClient.SendAsync(eventDataBatch.ToEnumerable()); // Send to event hub
            }
        }

        private EventData PrepareEventData(CoffeeMachineData data)
        {
            var dataAsJson = JsonConvert.SerializeObject(data);
            var eventData = new EventData(Encoding.UTF8.GetBytes(dataAsJson));
            return eventData;
        }
    }
}
