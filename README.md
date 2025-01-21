Programm has appsetings.json file in which there are important setting,
that has to be updated in order for programm to work




Querries used for data base


CREATE DATABASE TripsBD;

USE TripsBD;

CREATE TABLE TripRecords (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PickupDateTime DATETIME NOT NULL,
    DropoffDateTime DATETIME NOT NULL,
    PassengerCount INT NULL,
    TripDistance FLOAT NOT NULL,
    StoreAndFwdFlag NVARCHAR(3) NOT NULL,
    PULocationID INT NOT NULL,
    DOLocationID INT NOT NULL,
    FareAmount DECIMAL(10, 2) NOT NULL,
    TipAmount DECIMAL(10, 2) NOT NULL
);

CREATE INDEX IX_TripRecords_PULocationID ON TripRecords (PULocationID);

CREATE INDEX IX_TripRecords_TripDistance ON TripRecords (TripDistance DESC);

CREATE INDEX IX_TripRecords_Pickup_Dropoff ON TripRecords (PickupDateTime, DropoffDateTime);




29889 rows  was inserted after running of program




Assumptions about working with large data

1)Working with large data i would change  how records file is precessed,  
reading it in small parts rather than all at once.Same logic applies to writing it in to data base

2)Possibly changed type of "Id" field, to be able to hold more unique variants( but it can 
slow down overall speed of program)

3)using multiple threads for processing file, in order to each thread work with separate part of file independently
(but it requires very precise controll over threads)




