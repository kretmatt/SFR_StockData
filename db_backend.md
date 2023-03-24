# Database and Backend

After we built a producer, set up the schema registry, transformed the information of a Kafka topic and posted it to a different topic, we are now looking at building a microservice/backend that consumes this new topic and stores the information in a database.

## Selection of the Database

**Task:** Describe in a markdown file your decision with your selected database

SQL vs NoSQL

## Backend

**Task::** Describe in a markdown file in which case you would choose an AOT approach and in which case JIT


| AOT         | Jit         |
| ----------- | ----------- |
| Compilations happens once during the built     | Compiled in the browser.      |
| Browser doesn't need to compile it during runtime <br /> allowing for quicker component rendering  | Only compiles the code that is called <br /> |
