RavenDB-demo
============

An MVC 4 application created during the first week of an internship in July 2013 in [Billennium](http://www.billennium.pl/) by [Pawel Michna](http://pawelmichna.com) and Jan Grzybowski.
Its purpose is to demonstrate the functionality and ease of use of [RavenDB](http://ravendb.net/).

## Running ##
Download RavenDB build from [here](http://ravendb.net/download#builds) and run two servers, one on port 8080, the other on 8081.
To do this, just copy the downloaded build and in the copy edit `$RAVENHOME/Server/local.xml` and set appropriate ports.

On the server on port 8080 create a database called "EU_Clients" and "Orders". During the creating of the latter one check the "Replication Bundle" option. When created go to "Orders" db settings -> replication.
In the "Url" field write `http://localhost:8081`, in "Database" write `Orders`. Click "Save Changes".

On the server on port 8081 create two databases: "Rest_Clients" and "USA_Clients".
Create also an "Orders" db and similarly as before, check the "Replication Bundle" option. In the replication settings as the Url put `http://localhost:8080` and `Orders` as database name. Remember to save changes.

Open the project in Visual Studio (tested in VS 2012) and run the ShardingMvcDemo.

## Functionality ##
The application uses the RavenDB as a database for storing clients' and orders' information. It allows to do simple CRUD operation on both.

### Sharding ###
In this demo application we use the [sharding feature](http://ravendb.net/docs/2.0/server/scaling-out/sharding). Depending from which country the client is, he is stored in one of the databases: `EU_Clients`, `USA_Clients` or `Rest_Clients` (notice that they are on different servers).
RavenDB makes that feature very easy to use and it works almost out of the box.

### Replication ###
RavenDB allows also to use [replication](http://ravendb.net/docs/2.0/server/scaling-out/replication) easily. Two `Orders` databases are on different servers. They work in a Master - Master configuration (whatever is changed in any of them - it is replicated in the other).
Our application works in the following fashion: whatever operations concerning orders are done, one of the two servers is randomly selected. This is done to lower down the load on the server.
