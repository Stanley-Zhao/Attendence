USE [master]
GO

IF EXISTS(SELECT * FROM MASTER..SYSDATABASES WHERE NAME = N'CARS')
BEGIN
	DROP DATABASE  [CARS]
END
GO

-- Get the SQL Server data path
DECLARE @data_path nvarchar(256);
SET @data_path = (SELECT SUBSTRING(physical_name, 1, CHARINDEX(N'master.mdf', LOWER(physical_name)) - 1)
                  FROM master.sys.master_files
                  WHERE database_id = 1 AND file_id = 1);
-- execute the CREATE DATABASE statement 
EXECUTE ('CREATE DATABASE [CARS]
ON PRIMARY
( NAME = N''CARS'',
    FILENAME = N'''+ @data_path + 'CARS.mdf'',
    SIZE = 102400KB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 1024KB )
LOG ON
( NAME = N''CARS_log'',
    FILENAME = N'''+ @data_path + 'CARS_log.ldf'',
    SIZE = 102400KB,
    MAXSIZE = 2048GB,
    FILEGROWTH = 10% )'
);
GO
