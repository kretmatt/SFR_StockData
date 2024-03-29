# SFR_StockData

This is our SFR Repository.

## How to run

**Prerequisites:** Docker Desktop / Docker CLI, Docker Compose

In order to run the SFR_StockData application, you need to execute the command `docker compose up -d --build`. After a successful build, the containers should automatically start with the previouse command (shutdown is possible with `docker compose down`). Here are some additional commands to check out the automatically created topic and the messages that are published by the producer (for service kafka-1, the same is possible for the other kafka services by inserting the respective names+ports):

```Shell
# Command for checking the topic with 3 partitions, 3 replicas, and min.insync.replicas=2
docker exec --interactive --tty <INSERT CONTAINER NAME FOR kafka-1 SERVICE> kafka-topics --bootstrap-server <INSERT CONTAINER NAME FOR kafka-1 SERVICE>:19092 --describe --topic <INSERT TOPIC>

# Command for checking out the messages sent to the topic
docker exec --interactive --tty <INSERT CONTAINER NAME FOR kafka-1 SERVICE> kafka-console-consumer --bootstrap-server <INSERT CONTAINER NAME FOR kafka-1 SERVICE>:19092 --topic <INSERT TOPIC> --from-beginning
```

Our used topics are:
* **stocks:** Our producer is sending a stock object that contains a string companyname, int price and a DateTime timestamp. The key for the message is the name of the company
* **stocks-table:** This topic includes the overall change rate of the stock price as well as the change rate of the last hour and last minute. It is saved as a table and then turned back into a stream to send it to the next topic. This shows the **Table Stream Duality**.
* **bond-changes:** Only the overall change rate is sent to this topic

## Database
We included a MS SQL Server image in our docker compose and use the .NET Entity Framework to automatically create the database. This includes a table that saves the bond objects of our normal producers and a table that saves the bondchanges that are published by our processor To ensure persistency of the saved data we also added a volume in the container. Therefore, after restarting the docker container, the data in the database is not lost. 

## Microservice/Backend
Through our backend, we can send querys to the database and provide the frontend with the needed data with the help of endpoints.
<br/>This is the endpoint to request all entries in the database: http://localhost:8087/Stock
<br/>This is the endpoint to request all entries for a specific company in the database: http://localhost:8087/Stock/{company}
<br/>This is the endpoint to request all entries for the recorded changes (%) of the stock prices in the database: http://localhost:8087/Stock/trends

Available companies to check:
* Amazon http://localhost:8087/Stock/Amazon
* Apple http://localhost:8087/Stock/Apple
* CSS http://localhost:8087/Stock/CSS
* Google http://localhost:8087/Stock/Google
* Microsoft http://localhost:8087/Stock/Microsoft
* Squer http://localhost:8087/Stock/Squer

## Frontend
A requirement for the frontend was that it is based on progressive enhancement and server-side rendering. For the implementation, we used the Next.js Framework.
Once the docker container is built, the index page of our frontend can be accessed with the following URL: http://localhost:3000. There you can see all companies and information regarding the price changes in %. The data is fetched with the method getServerSideProps from our microservice endpoints mentioned above.
Clicking on one of the companies directs to a detailed page that shows all recored stock prices for this company. This is done through dynamic routing.

Available companies to check in our frontend:
* Amazon http://localhost:3000/stock/Amazon
* Apple http://localhost:3000/stock/Apple
* CSS http://localhost:3000/stock/CSS
* Google http://localhost:3000/stock/Google
* Microsoft http://localhost:3000/stock/Microsoft
* Squer http://localhost:3000/stock/Squer


Furthermore, since we are working with real time data, buildtime rendering or static site generation are unsuitable for our needs.

## Analyze how brokers, partitions, replicas & in.sync.replica configuration are related

Brokers: Kafka can be "distributed" across multiple nodes, also called brokers, to allow for easier scaling and to increase reliability of Kafka itself. Multiple brokers working together are often called a Kafka cluster.

Partitioning: A Kafka topic itself is a collection of messages with a unique name. Those topics can be split into smaller subsets, called partitions, to allow for multiple producers & consumers (due to concurrent access to the subsets on different brokers). 

Replication: Process of having multiple copies of the data spread across multiple brokers for the sole purpose of availability in case one of the brokers goes down and is unable to handle incoming requests. In Kafka, replication happens at the "partition level". This means, that copies of partitions are maintained at multiple brokers. Therefore, the replica factor determines how many copies of a partition need to be kept at a time (on different brokers). The original partition is normally called leader, whereas the copies on other brokers are called followers.

in.sync.replica Configuration: This configuration (most of the time provided as a minimum value) determines, how many replicas are needed to be available / in-sync with the leader in order for producers to successfully send records to a partition / in order for the kafka to continue running and accept new messages. The higher the configuration value is, the more
safety at the cost of latency you get. 

There can be an arbitrary amount of partitions for a topic. The maximum (reasonable) replica factor is dictated by the amount of brokers. If you only have 3 brokers and would like to have a replica factor of 4, it would not add to availability if one broker has 2 replicas of the same partition. The minimum value for insync replicas is capped by the replica factor. If the minimum value exceeded the replication factor, producers would never be able to send messages to partitions. 

## Schema and Compatibility Mode
To create our schema, we used Avro. In a first step, we had to download the avrogen tool by executing this command `dotnet tool install -g Confluent.Apache.Avro.AvroGen`. With this we can execute the `avrogen -s filename.avsc .` command to auto-generate a C# class from an .avsc file.  

There are three different compatibility modes which are the following:
* Backward: 
  * All fields can be deleted but only optional ones can be added 
  * Consumer has to be updated first
  * Requests
* Forward: 
  * Any field can be added but only optional ones can be deleted 
  * Producer has to be updated first
  * Answers
* Full: 
  * Only optional fields can be added or deleted

All of them refer only to compatibility with the version before (so for v5 it would be v4). By adding the keyword transitive at the end, it states that they are compatible with all previous versions.
The default setting for the Avro Schema is the backwards compatibility. We changed this in our docker compose to backward_transitive.
```Shell
SCHEMA_REGISTRY_AVRO_COMPATIBILITY_LEVEL: 'backward_transitive'
```
The registered schema can be checked under this URL: '[http://localhost:8085/subjects](http://localhost:8085/subjects)'
