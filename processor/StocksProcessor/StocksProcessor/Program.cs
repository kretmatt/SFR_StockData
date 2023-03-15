﻿using Confluent.Kafka;
using System;
using System.Net;
using System.Threading;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Demo;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.Crosscutting;
using Streamiz.Kafka.Net.SchemaRegistry.SerDes.Avro;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.State;
using Streamiz.Kafka.Net.Stream;
using Streamiz.Kafka.Net.Table;


//Based on: https://developer.confluent.io/get-started/dotnet/#build-producer 

public class StocksProcessor
{
    async static Task Main(string[] args)
    {
	    // Currently there is a bug where the producer / aggregator does not function correctly within docker without a sleep period in the beginning.
	    Thread.Sleep(30000);
	    // Get the configuration from the docker-compose.yaml through the environment
	    var bootstrapServers = Environment.GetEnvironmentVariable("bootstrapservers");
	    var topic = Environment.GetEnvironmentVariable("rawdatatopic");
	    var schemaRegistry = Environment.GetEnvironmentVariable("schemaRegistry");
	    var topic2 = Environment.GetEnvironmentVariable("tabletopic");
	    var topic3 = Environment.GetEnvironmentVariable("newdatatopic");
	    
	    Console.WriteLine(bootstrapServers);
	    
	    var config = new StreamConfig();
	    config.SchemaRegistryUrl = schemaRegistry;
	    config.BootstrapServers = bootstrapServers;
	    config.EnableIdempotence = true;
	    config.RetryBackoffMs = 1000;
	    config.MessageSendMaxRetries = 3;
	    config.Acks = Acks.All;
	    config.EnableDeliveryReports = true;
	    config.BufferBytes = 100;
	    config.ReplicationFactor = 3;
	    config.ApplicationId = "stocks-processor";

	    var stream = new StreamBuilder();
	    stream.Stream<string, Demo.Bond>(topic, new StringSerDes(), new SchemaAvroSerDes<Bond>())
		    .GroupByKey()
		    .Aggregate(
			    () => new BondChange()
				    { lasthourchange = 0, overallchange = 0, lastminutechange = 0, BondEntries = new List<Bond>() },
			    (s, bond, aggregator) =>
			    {
				    aggregator.BondEntries.Add(bond);

				    if (aggregator.BondEntries.Count > 1)
				    {
					    var oldestEntry = aggregator.BondEntries.MaxBy(x => x.timestamp);
					    var latestEntry = aggregator.BondEntries.MinBy(x => x.timestamp);

					    if (oldestEntry == null || latestEntry == null)
						    aggregator.overallchange = 0;
					    else
							aggregator.overallchange = ((latestEntry.price-oldestEntry.price) / oldestEntry.price)*100;
						Console.WriteLine(oldestEntry.price);
						Console.WriteLine(latestEntry.price);
					    var currentTimestamp = DateTime.Now;

					    var oldestEntryLastHour = aggregator.BondEntries
						    .Where(x => currentTimestamp > x.timestamp.AddMinutes(60)).MaxBy(x => x.timestamp);
					    var latestEntryLastHour = aggregator.BondEntries
						    .Where(x => currentTimestamp > x.timestamp.AddMinutes(60)).MinBy(x => x.timestamp);

					    if (oldestEntryLastHour == null || latestEntryLastHour == null)
						    aggregator.lasthourchange = 0;
					    else
						    aggregator.lasthourchange = ((latestEntryLastHour.price - oldestEntryLastHour.price) / oldestEntryLastHour.price)*100;

					    var oldestEntryLastMinute = aggregator.BondEntries
						    .Where(x => currentTimestamp > x.timestamp.AddMinutes(1)).MaxBy(x => x.timestamp);
					    var latestEntryLastMinute = aggregator.BondEntries
						    .Where(x => currentTimestamp > x.timestamp.AddMinutes(1)).MinBy(x => x.timestamp);

					    if (oldestEntryLastMinute == null || latestEntryLastMinute == null)
						    aggregator.lastminutechange = 0;
					    else
						    aggregator.lastminutechange = ((latestEntryLastMinute.price-oldestEntryLastMinute.price) /
						                                  oldestEntryLastMinute.price)* 100;
				    }
				    
				    Console.WriteLine($"{s} - {bond.price}: {aggregator.overallchange}% Overall | {aggregator.lasthourchange}% Last Hour | {aggregator.lastminutechange}% Last Minute");
				    return aggregator;
			    },
			    InMemory.As<string,BondChange>(topic2)
				    .WithValueSerdes(new SchemaAvroSerDes<BondChange>()).WithKeySerdes(new StringSerDes()))
		    .ToStream()
		    .MapValues((k, v) => v.overallchange)
		    .To<StringSerDes, DoubleSerDes>(topic3);


	    Topology t = stream.Build();
	    KafkaStream kstream = new KafkaStream(t, config);
	    Console.CancelKeyPress += (o, e) => kstream.Dispose();
	    await kstream.StartAsync();
    }
}