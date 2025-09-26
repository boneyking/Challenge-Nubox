USE [master]
GO

IF DB_ID('IntegracionNubox') IS NOT NULL
  set noexec on 

CREATE DATABASE IntegracionNubox;
GO

USE IntegracionNubox
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE LOGIN [integracionNuboxlogin] WITH PASSWORD = 'Fatel4707!@'
GO


CREATE USER [integracionNuboxlogin] FOR LOGIN [integracionNuboxlogin] WITH DEFAULT_SCHEMA=[dbo]
GO

EXEC sp_addrolemember N'db_owner', N'integracionNuboxlogin'
GO



