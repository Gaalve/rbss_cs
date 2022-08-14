
# C\# RBSS Implementation 
The C\# implementation of the range-based set synchronization protocol as proposed by A. Meyer.

The persistence layer contains one of two auxiliary data structures and one database:
+ the sorted set
+ a custom red-black tree implementation adjusted to better fit the requirements of the protocol
+ SQLite database implementation using the EntityFramework

# Deployment
The peer implementation can be deployed via docker by executing
```
docker build . -t rbss_cs --force-rm -f ./RBSS_CS/Dockerfile
```

or starting the ´dockerBuild.bat´ script.

# Configuration

The peer can be configured by providing a settings.yml to the docker container
and by providing environment variables.

## Environment Variables

RBSS_CONNECTION - the internet address of another peer
RBSS_SELF - the internet address of this peer

## Settings

A more detailed description of each variable can be found in ServerSettings.cs.

+ testingMode: bool - enables debugging features if set to true
+ testingModeInitiator: bool - enables integration test routine if set to true requires testingMode
+ auxiliaryDS: string - assembly name of the auxiliary data structure
+ dbKind: string - assembly name of the database persistence or none if persistence should be disabled
+ hashFunc: string - assembly name of the hash function used for the fingerprints
+ bifunctor: string - assembly name of the bifunctor used for the fingerprint
+ useIntervalForSync: bool - true: syncs via an interval, false: syncs when the set is modified through the modify api
+ syncIntervalMs: int - the interval to initiate a synchronization requires useIntervalForSync, a value less than 0 disables the automatic synchronization
+ branchingFactor: int - the branching factor of the rbss protocol, has to be bigger than 2
+ itemSize: int - the item size parameter of the rbss protocol, has to be bigger than 1
+ p2PStructure: string - the name of the p2p structure used, currently only "RING" is supported

# Integration Tests

The implementation allows for self contained integration tests.
Multiple configurations are provided to execute the integration tests.

Integration test cases by modified in IntegrationTest.cs. Some test cases are disable via a comment.
By uncommenting the attribute IntegrationTestMethod, these tests can be enabled.

Starts the integration tests between two C\# clients with the C\# integration test routine:
```
docker-compose build
docker-compose up --abort-on-container-exit
```
The integration test results can be read from the stdin, the logs provided by docker or by
copying the logs from the container.
To copy the logs exectute:
```
docker cp rbbs_cs_rbss_cs_initiator_1:/app/memResults.log memResults.log
docker cp rbbs_cs_rbss_cs_initiator_1:/app/testResults.log testResults.log
```
The data can be visualized by on of the python scripts in ./TestResults/

Starts the integration tests between a C\# client and Java Client with the C\# integration test routine:
```
docker-compose -f docker-composeCStoJ.yml build
docker-compose -f docker-composeCStoJ.yml pull
docker-compose -f docker-composeCStoJ.yml up
```

Starts the integration tests between a C\# client and Java Client with the Java integration test routine:
```
docker-compose -f docker-composeJtoCS.yml build
docker-compose -f docker-composeJtoCS.yml pull
docker-compose -f docker-composeJtoCS.yml up
```
