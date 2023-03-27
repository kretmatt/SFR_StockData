using Avro.Generic;
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
        private readonly string bondTrendTopic = Environment.GetEnvironmentVariable("bondTrendTopic");
        private StocksContext dbContext;
        public KafkaConsumerHandler(StocksContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var bondConsumer = new ConsumerConfig
            {
                GroupId = "st_consumer_group",
                BootstrapServers = Environment.GetEnvironmentVariable("bootstrapServers"),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                Acks = Acks.All,
                EnableAutoCommit = true,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 10000
            };
            
            var bondTrendConsumer = new ConsumerConfig
            {
                GroupId = "st_consumer_group_2",
                BootstrapServers = Environment.GetEnvironmentVariable("bootstrapServers"),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                Acks = Acks.All,
                EnableAutoCommit = true,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 10000
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
                using (var consumer = new ConsumerBuilder<string, Bond>(bondConsumer)
                           .SetValueDeserializer(new AvroDeserializer<Bond>(schemaRegistry).AsSyncOverAsync())
                           .SetErrorHandler((_,e)=>Console.WriteLine($"Error: {e.Reason}"))                           
                           .Build())
                {
                    consumer.Subscribe(topic);

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var consumerResult = consumer.Consume(cancelToken.Token);
                                Bond bond = consumerResult.Message.Value;
                                BondEntity bondEntity = new BondEntity()
                                {
                                    BondName = bond.bondname,
                                    Price = bond.price,
                                    TimeStamp = bond.timestamp
                                };
                                dbContext.Bonds.Add(bondEntity);
                                await dbContext.SaveChangesAsync(cancelToken.Token);
                            }
                            catch (ConsumeException ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine(ex);
                        consumer.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
            
            var consumeBondTrendTask = Task.Run(async () =>
            {
                using(var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistrConfig))
                using (var consumer = new ConsumerBuilder<string, BondTrend>(bondConsumer)
                           .SetValueDeserializer(new AvroDeserializer<BondTrend>(schemaRegistry).AsSyncOverAsync())
                           .SetErrorHandler((_,e)=>Console.WriteLine($"Error: {e.Reason}"))                           
                           .Build())
                {
                    consumer.Subscribe(bondTrendTopic);

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var consumerResult = consumer.Consume(cancelToken.Token);
                                BondTrend bondTrend = consumerResult.Message.Value;
                                string key = consumerResult.Message.Key;

                                if (!string.IsNullOrEmpty(key) && dbContext.BondTrends.Any(x => x.BondName.Equals(key)))
                                {
                                    BondTrendEntity bondTrendEntity = dbContext.BondTrends.FirstOrDefault(x => x.BondName.Equals(key));
                                    bondTrendEntity.LastHourChange = bondTrend.lasthourchange;
                                    bondTrendEntity.LastMinuteChange = bondTrend.lastminutechange;
                                    bondTrendEntity.OverallChange = bondTrend.overallchange;
                                }
                                else
                                {
                                    BondTrendEntity bondTrendEntity = new BondTrendEntity()
                                    {
                                        BondName = key,
                                        LastHourChange = bondTrend.lasthourchange,
                                        LastMinuteChange = bondTrend.lastminutechange,
                                        OverallChange = bondTrend.overallchange
                                    };
                                    dbContext.BondTrends.Add(bondTrendEntity);
                                }
                                await dbContext.SaveChangesAsync(cancelToken.Token);
                                
                            }
                            catch (ConsumeException ex)
                            {
                                Console.WriteLine(ex);
                            }
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        Console.WriteLine(ex);
                        consumer.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
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