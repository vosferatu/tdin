# TDIN

## Setting Up

 - [Tomcat9](https://tomcat.apache.org/download-90.cgi)
   - Follow guide at [Install Tomcat9](https://www.osradar.com/how-to-install-tomcat-on-fedora-29/) up to step 3
   - User: 'admin', Password: 'tdin'
 - [Maven](http://maven.apache.org/)
 - [Java-Gnome](http://java-gnome.sourceforge.net/)
   - I have already compiled and put the files in the <i>lib</i> folder. I am unsure whether the files will work on other computers :/
 - [RabbitMQ](https://www.rabbitmq.com/)


## Running

Before starting any of the servers you need to start the rabbitmq service with

    sudo rabbitmq-server

### Bookstore Printer
Printer should be the first program to be started since Bookstore Server will try to connect to it. Bookstore Server will still work if it cannot connect, but it only tries to connect at startup. Run with command:

    mvn exec:java@Printer

### Bookstore Server
Bookstore server is the server that should always be running, it should always be started first and is where the database will be accessed. 
Start it by running:

    mvn exec:java@BookstoreServer

### Bookstore Client
In the root of the project run:

    mvn exec:java@BookstoreClient

### Warehouse Server
Warehouse server should not need to be running to accept orders from Bookstore Server
Start it by running:

    mvn exec:java@WarehouseServer

### Warehouse Client
In the root of the project run:

    mvn exec:java@WarehouseClient

### WebApp
Webapp is probably the most unsafe web application I have ever seen, but give me a break!

In the root of the project run:

    mvn tomcat7:run
    
To start hosting a local webapp service. Then access webapp at:
   
    localhost:8000/bookstore

## Possible Improvements

 - Make Servers communicate directly with GUI when new information arrives through RMI.
   - This will probably cause some race conditions so it will be left for the latest stage of the project since it is not absolutely necessary.
