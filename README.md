# TDIN

## Setting Up

 - [Tomcat9](https://tomcat.apache.org/download-90.cgi)
   - Follow guide at [Install Tomcat9](https://www.osradar.com/how-to-install-tomcat-on-fedora-29/) up to step 3
   - User: 'admin', Password: 'tdin'
 - [Maven](http://maven.apache.org/)
 - [MySQL](https://www.mysql.com/downloads/)
   - Setup user: 'root', password: 'Pass123!'
   - Create a database named 'bookstore'
   - Use file database/bookstore.sql to initialize database
 - [Java-Gnome](http://java-gnome.sourceforge.net/)
   - I have already compiled and put the files in the <i>lib</i> folder. I am unsure whether the files will work on other computers :/


## Running

### Bookstore Server
Bookstore server is the server that should always be running, it should always be started first and is where the database will be accessed. 
Start it by running:

    mvn exec:java@BookstoreServer

### Bookstore Client
Only first draft is made. Still need to add more rules to the other GUI, however the basis is also here.
In the root of the project run:

    mvn exec:java@BookstoreClient

### WebApp

Currently the webapp is still very undeveloped and needs further work. However the basis is there.
In the root of the project run:

    mvn tomcat7:run
    
To start hosting a local webapp service. Then access webapp at:
   
    localhost:8000/bookstore

