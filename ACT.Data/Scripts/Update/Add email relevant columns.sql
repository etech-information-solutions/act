USE [DAPRS]
GO

ALTER TaBLE [dbo].[Configs]
Add
EmailStart time(7),
EmailEnd time(7),
EmailPoll decimal,
LastEmailRun datetime,
EmailImportPath varchar(200),
EmailExportPath varchar(200)