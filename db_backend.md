# Database and Backend

After we built a producer, set up the schema registry, transformed the information of a Kafka topic and posted it to a different topic, we are now looking at building a microservice/backend that consumes this new topic and stores the information in a database.

## Selection of the Database

**Task:** Describe in a markdown file your decision with your selected database

As dicsussed in the lecture, the first decsion reagrding the selection of a database is to decide between SQl and NoSQL. The following table highlights some of the differences.
|        | SQL       | NoSQL        |
| ----------- | ----------- |----------- |
| Data Format  | Structured Data    | Unstructured Data      |
| Scalability  | Scales vertically  => increase physical hardware |High Scalability, built in sharding |
| Structure  | Relational Tabular Database    | Column-Oriented , Graph Databases, Document Stores, Key-Value Stores |
| Properties  | ACID    | CAP-Theorem |
| When to use  | ACID    | CAP-Theorem |

In the case of our project, we decided to use Microsoft SQL as a database since we don't have to think about scalability and a table works well for the data that we want to store. Furthermore, SQL is suitable for simple aggregations. In our case calculating the average stock price for a specific company.


## Backend

**Task:** Describe in a markdown file in which case you would choose an AOT approach and in which case JIT

https://www.c-sharpcorner.com/article/ahead-of-time-aot-compilation-to-native-code-in-net-7/

|            | AOT         | JIT         |
| -----------| ----------- | ----------- |
| Size  | Smaller    | Bigger      |
|       | Compilations happens once during the built     | Compiled in the browser.      |
| Size  | Smaller    | Sopisticated otimization with runtime information      |
| Compiling  | Build time    | Runtime      |
|  Advantages   | <ul><li>item1</li><li>item2</li></ul>   | <ul><li>item1</li><li>item2</li></ul>     |
| Browser doesn't need to compile it during runtime <br /> allowing for quicker component rendering  | Only compiles the code that is called <br /> |
| Platform-independent  | Platform-dependent |

We used .NET for the development of our Kafka Service. AOT was only introduced not to long ago with .NET 7 and focusses primarily on console applications. Therefore, we are more or less forced to go with the JIT compilation.


