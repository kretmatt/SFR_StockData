# SFR_StockData

This is our SFR Repository.

## How to run

In order to run the kafka cluster, simply use the `docker compose up` (-d is optional) command. To shut down the cluster, simply run `docker compose down`. To run the producer C# script / project, you simply have to start it (maybe adjust ports inside the code if needed). You may also need to add `kafka-1, kafka-2, kafka-3` to your hosts file (127.0.0.1 for the IP address).

## Analyze how brokers, partitions, replicas & in.sync.replica configuration are related

Brokers: Kafka can be "distributed" across multiple nodes, also called brokers, to allow for easier scaling and to increase reliability of Kafka itself. Multiple brokers working together are often called a Kafka cluster.

Partitioning: A Kafka topic itself is a collection of messages with a unique name. Those topics can be split into smaller subsets, called partitions, to allow for multiple producers & consumers (due to concurrent access to the subsets on different brokers). 

Replication: Process of having multiple copies of the data spread across multiple brokers for the sole purpose of availability in case one of the brokers goes down and is unable to handle incoming requests. In Kafka, replication happens at the "partition level". This means, that copies of partitions are maintained at multiple brokers. Therefore, the replica factor determines how many copies of a partition need to be kept at a time (on different brokers). The original partition is normally called leader, whereas the copies on other brokers are called followers.

in.sync.replica Configuration: This configuration (most of the time provided as a minimum value) determines, how many replicas are needed to be available / in-sync with the leader in order for producers to successfully send records to a partition / in order for the kafka to continue running and accept new messages. The higher the configuration value is, the more
safety at the cost of latency you get. 

There can be an arbitrary amount of partitions for a topic. The maximum (reasonable) replica factor is dictated by the amount of brokers. If you only have 3 brokers and would like to have a replica factor of 4, it would not add to availability if one broker has 2 replicas of the same partition. The minimum value for insync replicas is capped by the replica factor. If the minimum value exceeded the replication factor, producers would never be able to send messages to partitions. 
