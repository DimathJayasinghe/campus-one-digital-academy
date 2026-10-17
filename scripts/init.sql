-- ============================================================
-- Campus One Digital Academy – Database Initialisation Script
-- Runs automatically when the Docker container first starts.
-- ============================================================

-- 1. Create the database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Student')
BEGIN
    CREATE DATABASE Student;
END
GO

USE Student;
GO

-- 2. Logins table (optional multi-user support)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Logins' AND xtype='U')
BEGIN
    CREATE TABLE Logins (
        id        INT          IDENTITY(1,1) PRIMARY KEY,
        username  VARCHAR(50)  NOT NULL UNIQUE,
        password  VARCHAR(100) NOT NULL   -- store hash in production
    );

    -- Default admin account (as required by the brief)
    INSERT INTO Logins (username, password) VALUES ('Admin', 'Campusone@123');
END
GO

-- 3. Registration table (primary entity)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Registration' AND xtype='U')
BEGIN
    CREATE TABLE Registration (
        regNo       INT          PRIMARY KEY,
        firstName   VARCHAR(50)  NOT NULL,
        lastName    VARCHAR(50)  NOT NULL,
        dateOfBirth DATETIME     NOT NULL,
        gender      VARCHAR(50)  NOT NULL,   -- 'Male' | 'Female'
        address     VARCHAR(50)  NOT NULL,
        email       VARCHAR(50)  NOT NULL,
        mobilePhone BIGINT       NOT NULL,
        homePhone   BIGINT,
        parentName  VARCHAR(50),
        nic         VARCHAR(50),
        contactNo   BIGINT
    );
END
GO
