Introduction
============
This project is created to showcase how to use Azure Event Hubs for Event Streaming. Application is built using C#, .NET, WPF for sender which produces Events and sent it to Event Hub in Azure and consumer built using .NET Core which consume those events from Azure Event Hub.

Sender is a Coffee Machine Simulator which sends different sensor events to Azure Event Hub. Sensor events includes type and number of coffee made, boiler temperature and bean level.

Coffee Machine simulator produces cappuccino or espresso coffee events when buttons corresponding to those events are pressed in WPF application. 

There is also a periodic events sent every two seconds which includes sensor events for boiler temperature and bean level which is sent when checkbox is selected in WPF application. You can adjust the temperature and bean level using slider. 

Consumer is a .NET Core console application which uses EventHubClient to consume those events from Azure Event Hub and display them in console.  
  
Important
============

You will need to provide your own Azure Event Hub connection string to use this project. 
Connection Strings are specified in 

Azure-Event-Hubs-Event-Streaming\CoffeeMachine.Simulator.UI\App.config
Azure-Event-Hubs-Event-Streaming\CoffeeMachine.EventHub.Receiver.Direct\AppSettings.json