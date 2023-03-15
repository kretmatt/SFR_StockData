using Confluent.Kafka;
using System;
using System.Net;
using System.Threading;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;


//Based on: https://developer.confluent.io/get-started/dotnet/#build-producer 

public class Producer
{
    static void Main(string[] args)
    {
    	// Get the configuration from the docker-compose.yaml through the environment
    	var bootstrapServers = Environment.GetEnvironmentVariable("bootstrapservers");
    	var topic = Environment.GetEnvironmentVariable("topic");
    	var schemaRegistry = Environment.GetEnvironmentVariable("schemaRegistry");
        Console.WriteLine(bootstrapServers);
    
    	// Create config for the producer
    
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            EnableDeliveryReports = true,
            ClientId = Dns.GetHostName(),
            Acks = Acks.All,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000,
            EnableIdempotence = true,
        };
        
        // Create config for the schema registry
        
        var schemaRegistryConfig = new SchemaRegistryConfig
        {
            Url = schemaRegistry
        };
        
        // Create config for Avro
        
        var avroSerializerConfig = new AvroSerializerConfig
        {
            BufferBytes = 100
        };


        string[] bonds = { "Google", "Amazon", "Apple", "Microsoft", "Squer", "CSS" };
        int price = 120;

        using (var registry = new CachedSchemaRegistryClient(schemaRegistryConfig))
        {
            using (var producer = new ProducerBuilder<string, Demo.Bond>(producerConfig)
                       .SetValueSerializer(new AvroSerializer<Demo.Bond>(registry, avroSerializerConfig))
                       .Build())
            {
                string text;
                Random rnd = new Random();
                while ((text = Console.ReadLine()) != "q")
                {
                    var name = bonds[rnd.Next(bonds.Length)];
                    price = price + rnd.Next(0, 2);
                    var time = DateTime.Now;
                    Demo.Bond bondy = new Demo.Bond()
                    {
                        bondname = name,
                        price = price,
                        timestamp = time
                    };

                    String key = name;

                    producer.ProduceAsync(topic, new Message<string, Demo.Bond> { Key = key, Value = bondy })
                        .ContinueWith(task =>
                            {
                                if (!task.IsFaulted)
                                {
                                    Console.WriteLine($"produced to {task.Result.TopicPartitionOffset}");
                                    return;
                                }

                                Console.WriteLine(
                                    $"An error occured whilst producing a message: {task.Exception.InnerExceptions}");
                            }
                        );

                    producer.Flush(TimeSpan.FromSeconds(10));
                    Thread.Sleep(10000);
                }
            }
        }
    }
}