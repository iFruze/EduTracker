-- Создание базы данных (если ещё не создана)
IF DB_ID(N'TeachHours') IS NULL
    CREATE DATABASE [TeachHours];
GO

-- Переключение на базу
USE [TeachHours];
GO

-- Таблица Teachers
CREATE TABLE dbo.Teachers (
    id INT IDENTITY(1,1) PRIMARY KEY,
    login NVARCHAR(50) NOT NULL,
    password NVARCHAR(255) NOT NULL,
    url NVARCHAR(MAX)
);
GO

-- Таблица Subjects
CREATE TABLE dbo.Subjects (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(50) NOT NULL,
    teacherId INT NOT NULL,
    CONSTRAINT FK_Subjects_Teachers FOREIGN KEY (teacherId)
        REFERENCES dbo.Teachers(id)
);
GO

-- Таблица Dates
CREATE TABLE dbo.Dates (
    id INT IDENTITY(1,1) PRIMARY KEY,
    date DATE NOT NULL
);
GO

-- Таблица AllHours
CREATE TABLE dbo.AllHours (
    id INT IDENTITY(1,1) PRIMARY KEY,
    subjectName NVARCHAR(50) NOT NULL,
    teacherId INT NOT NULL,
    countHours INT NOT NULL,
    CONSTRAINT FK_AllHours_Teachers FOREIGN KEY (teacherId)
        REFERENCES dbo.Teachers(id)
);
GO

-- Таблица Hours
CREATE TABLE dbo.Hours (
    id INT IDENTITY(1,1) PRIMARY KEY,
    subjectId INT NOT NULL,
    dateId INT NOT NULL,
    teacherId INT NOT NULL,
    CONSTRAINT FK_Hours_Subjects FOREIGN KEY (subjectId)
        REFERENCES dbo.Subjects(id),
    CONSTRAINT FK_Hours_Dates FOREIGN KEY (dateId)
        REFERENCES dbo.Dates(id),
    CONSTRAINT FK_Hours_Teachers FOREIGN KEY (teacherId)
        REFERENCES dbo.Teachers(id)
);
GO
