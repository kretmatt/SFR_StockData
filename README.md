# SFR_StockData

This is our SFR Repository.

## How to run

**Prerequisites:** Docker Desktop / Docker CLI, Docker Compose

In order to run the SFR_StockData application, you first need to build the image for the kafka producer by executing the command `docker build -t stockproducer .`. After a successful build, you are able to start the application by using the `docker compose up -d` command (shutdown is possible with `docker compose down`). Here are some additional commands to check out the automatically created topic and the messages that are published by the producer (for service kafka-1, the same is possible for the other kafka services by inserting the respective names+ports):

```Shell
# Command for checking the topic with 3 partitions, 3 replicas, and min.insync.replicas=2
docker exec --interactive --tty <INSERT CONTAINER NAME FOR kafka-1 SERVICE> kafka-topics --bootstrap-server <INSERT CONTAINER NAME FOR kafka-1 SERVICE>:19092 --describe --topic stocks

# Command for checking out the messages sent to the topic
docker exec --interactive --tty <INSERT CONTAINER NAME FOR kafka-1 SERVICE> kafka-console-consumer --bootstrap-server <INSERT CONTAINER NAME FOR kafka-1 SERVICE>:19092 --topic stocks --from-beginning
```

## Analyze how brokers, partitions, replicas & in.sync.replica configuration are related

Brokers: Kafka can be "distributed" across multiple nodes, also called brokers, to allow for easier scaling and to increase reliability of Kafka itself. Multiple brokers working together are often called a Kafka cluster.

Partitioning: A Kafka topic itself is a collection of messages with a unique name. Those topics can be split into smaller subsets, called partitions, to allow for multiple producers & consumers (due to concurrent access to the subsets on different brokers). 

Replication: Process of having multiple copies of the data spread across multiple brokers for the sole purpose of availability in case one of the brokers goes down and is unable to handle incoming requests. In Kafka, replication happens at the "partition level". This means, that copies of partitions are maintained at multiple brokers. Therefore, the replica factor determines how many copies of a partition need to be kept at a time (on different brokers). The original partition is normally called leader, whereas the copies on other brokers are called followers.

in.sync.replica Configuration: This configuration (most of the time provided as a minimum value) determines, how many replicas are needed to be available / in-sync with the leader in order for producers to successfully send records to a partition / in order for the kafka to continue running and accept new messages. The higher the configuration value is, the more
safety at the cost of latency you get. 

There can be an arbitrary amount of partitions for a topic. The maximum (reasonable) replica factor is dictated by the amount of brokers. If you only have 3 brokers and would like to have a replica factor of 4, it would not add to availability if one broker has 2 replicas of the same partition. The minimum value for insync replicas is capped by the replica factor. If the minimum value exceeded the replication factor, producers would never be able to send messages to partitions. 
