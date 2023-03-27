# Database and Backend

After we built a producer, set up the schema registry, transformed the information of a Kafka topic and posted it to a different topic, we are now looking at building a microservice/backend that consumes this new topic and stores the information in a database.

## Selection of the Database

**Task:** Describe in a markdown file your decision with your selected database

As dicsussed in the lecture, the first decsion reagrding the selection of a database is to decide between SQl and NoSQL. The following table highlights some of the differences.
|        | SQL       | NoSQL        |
| ----------- | ----------- |----------- |
| Language  | Structured Query Language    | Depends on Database      |
| Data Format  | Structured Data    | Unstructured Data      |
| Scalability  | Scales vertically  => increase physical hardware |High Scalability, built in sharding |
| Structure  | Relational Tabular Database    | Column-Oriented , Graph Databases, Document Stores, Key-Value Stores |
| Properties  | ACID    | CAP-Theorem |
| When to use  | <ul><li>Structured or structure doesn't change frequently</li><li>Routinely perform complex queries</li><li>Require high degree of data security and integrity</li></ul>    | <ul><li>Require flexibility regarding data model</li><li>Require scalability</li><li>Working with data that doesn't fit in the relational model</li></ul> |

In the case of our project, we decided to use Microsoft SQL as a database since we don't have to think about scalability and a table works well for the data that we want to store. Furthermore, SQL is suitable for simple aggregations. In our case calculating the average stock price for a specific company.

We included a MS SQL Server image in our docker compose and use the .NET entity framework to automatically create a database. This includes a table that saves the bond objects of our normal producers and a table that saves the bondchanges that are published by our processor To ensure persistency of the saved data we also added a volume in the container. Therefore, after restarting the docker container, the data in the database is not lost.

## Backend

**Task:** Describe in a markdown file in which case you would choose an AOT approach and in which case JIT

The following table compares the Ahead-of-Time (AOT) with the Just-in-Time (JIT) compilation.

|            | AOT         | JIT         |
| -----------| ----------- | ----------- |
| Compiling  | Build time  | Runtime     |
| Advantages | <ul><li>Useful in Cloud services because of faster startup</li><li>Smaller packaging</li><li>Suitable for microservices that are running in a cloud docker container</li></ul>   | <ul><li>Sophisticated otimization with runtime information</li><li>Only compiles the code that is called</li></ul>|
|  Disadvantages   | <ul><li>If the code has a lot of if conditions where some branches are never reached, AOT can performe worse than JIT</li></ul>  | <ul><li>Slower startup because compilation happens at runtime</li><li>Bigger packaging</li></ul>     |

We used .NET for the development of our Kafka Service. AOT was only introduced not to long ago with .NET 7 and focusses primarily on console applications. Therefore, we are more or less forced to go with the JIT compilation. However, if we had the choice we think that we would go with the AOT compilation since we prioritise a fast startup and our code doesn't include a lot of passages which are never called where the JIT compilation would shine with a better performance.

Through our backend, we can send querys to the database and provide the frontend with the needed data with the help of endpoints. 
This is the endpoint to request all entries in the database: http://localhost:8087/Stock 
This is the endpoint to request all entries for a specific company in the database: http://localhost:8087/Stock/{company}

Available companies to check:

    Amazon
    Apple
    CSS
    Google
    Microsoft
    Squer

