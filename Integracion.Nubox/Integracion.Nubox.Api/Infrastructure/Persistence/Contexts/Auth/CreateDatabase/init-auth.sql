USE [master]
GO

IF DB_ID('Auth') IS NOT NULL
  set noexec on 

CREATE DATABASE Auth;
GO

USE Auth
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE LOGIN [authlogin] WITH PASSWORD = 'Fatel4707!@'
GO


CREATE USER [authlogin] FOR LOGIN [authlogin] WITH DEFAULT_SCHEMA=[dbo]
GO

EXEC sp_addrolemember N'db_owner', N'authlogin'
GO



