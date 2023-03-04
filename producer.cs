using Confluent.Kafka;
using System;
using System.Net;
using System.Text.Json;
using producer;
using System.Threading;

//Based on: https://developer.confluent.io/get-started/dotnet/#build-producer 

public class Producer
{
    static void Main(string[] args)
    {
        var bootstrapservers = Environment.GetEnvironmentVariable("bootstrapservers");
        var topic = Environment.GetEnvironmentVariable("topic");
        
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapservers,
            EnableDeliveryReports = true,
            ClientId = Dns.GetHostName(),
            // Emit debug logs for message writer process, remove this setting in production
            Debug = "msg",
            // retry settings:
            // Receive acknowledgement from all sync replicas
            Acks = Acks.All,
            // Number of times to retry before giving up
            MessageSendMaxRetries = 3,
            // Duration to retry before next attempt
            RetryBackoffMs = 1000,
            // Set to true if you don't want to reorder messages on retry
            EnableIdempotence = true
        };
        
        string[] bonds = { "Google", "Amazon", "Apple", "Microsoft", "Squer", "CSS" };
        int price = 120;

        while (true)
        {
            using (var producer = new ProducerBuilder<string, string>(
                producerConfig).Build())
            {
                var numProduced = 0;
                Random rnd = new Random();
                var name = bonds[rnd.Next(bonds.Length)];
                price = price + rnd.Next(0,2);
                var time = new DateTime(DateTime.Now.TimeOfDay.Ticks).ToString();
                string json = JsonSerializer.Serialize(new Bond(name, price, time));

                producer.Produce(topic, new Message<string, string> {Value = json },
                    (deliveryReport) =>
                    {
                        if (deliveryReport.Error.Code != ErrorCode.NoError)
                        {
                            Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                        }
                        else
                        {
                            Console.WriteLine($"Produced event to topic {topic}: value = {json}");
                            numProduced += 1;
                        }
                    });
                producer.Flush(TimeSpan.FromSeconds(10));
            }
            Thread.Sleep(20000);
        }
    }
}