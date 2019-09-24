ALTER TABLE [WF].[State] ADD [IsValidationRequired] [bit] NULL
Go
Update [WF].[State] set [IsValidationRequired] = 0
GO