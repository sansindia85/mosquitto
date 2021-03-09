using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace Publisher
{
    class Program
    {
        private static IMqttClient _client;
        private static IMqttClientOptions _options;

        static async Task Main()
        {
            Console.WriteLine("Starting Publisher....");

            
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            //configure options
            _options = new MqttClientOptionsBuilder()
                .WithClientId("PublisherId")
                .WithTcpServer("localhost")
                .WithCleanSession()
                .Build();

            //handlers
            _client.UseConnectedHandler(e =>
            {
                Console.WriteLine("Connected successfully with MQTT Broker.");
            });

            _client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected from MQTT Brokers.");
            });

            
            //connect
            await _client.ConnectAsync(_options);
            await SimulatePublishAsync();

            Console.ReadLine();

        }

        private static async Task SimulatePublishAsync()
        {
            for (var counter = 0; counter < 1000; ++counter)
            {
                var testMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("test")
                    .WithPayload($"Payload: {counter}")
                    .WithExactlyOnceQoS()
                    .WithRetainFlag()
                    .Build();

                if (_client.IsConnected)
                {
                    await _client.PublishAsync(testMessage);
                }
                Console.WriteLine($"Sending mesage : {counter}");
                await Task.Delay(1000);
            }
        }
    }
}
