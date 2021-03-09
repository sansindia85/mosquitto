﻿using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Susbcriber
{
    class Program
    {
        private static IMqttClient _client;
        private static IMqttClientOptions _options;
        static async Task Main(string[] args)
        {
            //create subscriber client
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();
            
            //configure options
            _options = new MqttClientOptionsBuilder()
                .WithClientId("Subscriber")
                .WithTcpServer("localhost")
                .WithCleanSession()
                .Build();

            //Handlers
            _client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Connected successfully with MQTT Broker.");

                //Subscribe to topic
                //Subscribe to topic
                await _client.SubscribeAsync(new MqttTopicFilterBuilder()
                        .WithTopic("test")
                        .Build());
            });
            _client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from MQTT Brokers.");
            });
            _client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });

            //actually connect
            await _client.ConnectAsync(_options);

            Console.WriteLine("Press key to exit");
            Console.ReadLine();


            _client.DisconnectAsync().Wait();
        }
    }
}
