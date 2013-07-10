RavenDB-demo
============

An MVC application created during an internship in July 2013 in [Billennium](http://www.billennium.pl/) by [Pawel Michna](http://pawelmichna.com) and Jan Grzybowski.
Its purpose is to demonstrate the functionality and ease of use of [RavenDB](http://ravendb.net/).

# Running #
Download RavenDB build from [here](http://ravendb.net/download#builds) and run two servers, one port 8080, the other on 8081.
To do this, just copy the downloaded build and in the copy edit `$RAVENHOME/Server/local.xml` and set appropriate ports.

Open the project in Visual Studio (tested in VS 2012) and run the ShardingMvcDemo.
