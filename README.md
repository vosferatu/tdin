# TDIN

## Setting Up

 - Tomcat9
   - Follow guide at [Install Tomcat9](https://www.osradar.com/how-to-install-tomcat-on-fedora-29/) up to step 3
   - User: 'admin', Password: 'tdin'
 - Maven
 - MySQL
   - Setup user: 'root', password: 'Pass123!'
   - Create a database named 'bookstore'
   - Use file database/bookstore.sql to initialize database

## Running

Recommended run command at project root:

    mvn tomcat7:run