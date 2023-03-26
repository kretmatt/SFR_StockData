using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Demo;

namespace MS
{
    public class KafkaConsumerHandler : IHostedService
    {
        //BackgroundService
        private readonly string topic = Environment.GetEnvironmentVariable("topic");
        private StocksContext dbContext;
        public KafkaConsumerHandler(StocksContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var conf = new ConsumerConfig
            {
                GroupId = "st_consumer_group",
                BootstrapServers = Environment.GetEnvironmentVariable("bootstrapServers"),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                Acks = Acks.All,
            };

            var schemaRegistrConfig = new SchemaRegistryConfig
            {
                Url = Environment.GetEnvironmentVariable("schemaRegistryUrl"),
            };

            var avroSerializerConfig = new AvroSerializerConfig
            {
                BufferBytes = 100
            };
            
            var cancelToken = new CancellationTokenSource();
            var consumeTask = Task.Run(async () =>
            {
                
                
                using(var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistrConfig))
                using (var consumer = new ConsumerBuilder<Null, Bond>(conf)
                           .SetValueDeserializer(new AvroDeserializer<Bond>(schemaRegistry).AsSyncOverAsync())
                           .SetErrorHandler((_,e)=>Console.WriteLine($"Error: {e.Reason}"))                           
                           .Build())
                {
                    consumer.Subscribe(topic);
                    
                    try
                    {
                        while (true)
                        {
                            var consumerResult = consumer.Consume(cancelToken.Token);
                            Bond bond = consumerResult.Message.Value;

                            dbContext.Bonds.Add(bond);
                            await dbContext.SaveChangesAsync(cancelToken.Token);
                        }
                    }
                    catch (Exception)
                    {
                        consumer.Close();
                    }
                }
            });
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}