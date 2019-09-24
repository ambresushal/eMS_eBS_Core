ALTER TABLE [Setup].[ExitValidateQueue] ADD [IsNotificationSent] [bit] NULL
Go
ALTER TABLE [Setup].[ExitValidateQueue] ADD [IsQueuedForWFStateUpdate] [bit] NULL
Go
Update [Setup].[ExitValidateQueue] set [IsNotificationSent] = 0
GO
Update [Setup].[ExitValidateQueue] set [IsQueuedForWFStateUpdate] = 0
Go
insert into Setup.NoticationMessage(MessageType, MessageKey, MessageText) values ('Task','TASK_EV_COMPLETED', '{message}')