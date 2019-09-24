ALTER TABLE [Setup].[ExitValidateQueue] ADD [UsersInterestedInStatus] varchar(500) NULL
Go
Update [Setup].[ExitValidateQueue] set [UsersInterestedInStatus] = ''
GO