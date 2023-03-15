# SFR_StockData

This is our SFR Repository.

## How to run

**Prerequisites:** Docker Desktop / Docker CLI, Docker Compose

In order to run the SFR_StockData application, you first need to build the image for the kafka producer by executing the command `docker build -t stockproducer .`. As a next step you also have to build the image of the processor which aggregates the bond stream and creates a new stream with the growth rate of the bond. Navigate to the `SFR_StockData/processor/StocksProcessor` directory and execute the command `docker build -t stockproc .`. After a successful build, you are able to start the application by using the `docker compose up -d` command (shutdown is possible with `docker compose down`). Here are some additional commands to check out the automatically created topic and the messages that are published by the producer (for service kafka-1, the same is possible for the other kafka services by inserting the respective names+ports):

```Shell
# Command for checking the topic with 3 partitions, 3 replicas, and min.insync.replicas=2
docker exec --interactive --tty <INSERT CONTAINER NAME FOR kafka-1 SERVICE> kafka-topics --bootstrap-server <INSERT CONTAINER NAME FOR kafka-1 SERVICE>:19092 --describe --topic <INSERT TOPIC>

# Command for checking out the messages sent to the topic
docker exec --interactive --tty <INSERT CONTAINER NAME FOR kafka-1 SERVICE> kafka-console-consumer --bootstrap-server <INSERT CONTAINER NAME FOR kafka-1 SERVICE>:19092 --topic <INSERT TOPIC> --from-beginning
```

Our used topics are:
* **stocks:** Our producer is sending a stock object that contains a string companyname, int price and a DateTime timestamp. The key for the message is the name of the company
* **stocks-table:** This topic includes the overall change rate of the stock price as well as the change rate of the last hour and last minute. It is saved as a table and then turned back into a stream to send it to the next topic. This shows the **Table Stream Duality**.
* **bonds-change:** Only the overall change rate is sent to this topic

## Analyze how brokers, partitions, replicas & in.sync.replica configuration are related

Brokers: Kafka can be "distributed" across multiple nodes, also called brokers, to allow for easier scaling and to increase reliability of Kafka itself. Multiple brokers working together are often called a Kafka cluster.

Partitioning: A Kafka topic itself is a collection of messages with a unique name. Those topics can be split into smaller subsets, called partitions, to allow for multiple producers & consumers (due to concurrent access to the subsets on different brokers). 

Replication: Process of having multiple copies of the data spread across multiple brokers for the sole purpose of availability in case one of the brokers goes down and is unable to handle incoming requests. In Kafka, replication happens at the "partition level". This means, that copies of partitions are maintained at multiple brokers. Therefore, the replica factor determines how many copies of a partition need to be kept at a time (on different brokers). The original partition is normally called leader, whereas the copies on other brokers are called followers.

in.sync.replica Configuration: This configuration (most of the time provided as a minimum value) determines, how many replicas are needed to be available / in-sync with the leader in order for producers to successfully send records to a partition / in order for the kafka to continue running and accept new messages. The higher the configuration value is, the more
safety at the cost of latency you get. 

There can be an arbitrary amount of partitions for a topic. The maximum (reasonable) replica factor is dictated by the amount of brokers. If you only have 3 brokers and would like to have a replica factor of 4, it would not add to availability if one broker has 2 replicas of the same partition. The minimum value for insync replicas is capped by the replica factor. If the minimum value exceeded the replication factor, producers would never be able to send messages to partitions. 

## Schema and Compatibility Mode
To create our schema, we used Avro. In a first step, we had to download the avrogen tool by executing this command `dotnet tool install -g Confluent.Apache.Avro.AvroGen`. With this we can execute the `avrogen -s filename.avsc .` command to auto-generate a C# class from an .avsc file. The schema will also 

There are three different compatibility modes which are the following:
* Backward: All fields can be deleted but only optional ones can be added 
  * Consumer has to be updated first
  * Requests
* Forward: Any field can be added but only optional ones can be deleted 
  * Producer has to be updated first
  * Answers
* Full: Only optional fields can be added or deleted
All of them refer only to compatibility with the version before (so for v5 it would be v4). By adding the keyword transitive at the end, it states that they are compatible with all previous versions.
The default setting for the Avro Schema that we went with is the backwards compatibility. This can be defined in our docker compose 
```Shell
SCHEMA_REGISTRY_AVRO_COMPATIBILITY_LEVEL: 'backward'
```
