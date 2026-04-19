CREATE DATABASE robotdb;

CREATE TABLE "user" (
    id SERIAL PRIMARY KEY,
    email TEXT,
    firstname TEXT,
    lastname TEXT,
    passwordhash TEXT,
    role TEXT,
    createddate TIMESTAMP,
    modifieddate TIMESTAMP
);

SELECT * FROM "user";