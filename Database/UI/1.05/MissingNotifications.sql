INSERT [Setup].[NoticationMessage] ( [MessageType], [MessageKey], [MessageText]) VALUES ( N'Section', N'NOTIFY_RELEASE', N'Document: {Document}, View: {View}, Section: {Section} has been released by {user}.')
INSERT [Setup].[NoticationMessage] ( [MessageType], [MessageKey], [MessageText]) VALUES ( N'Section', N'NOTIFY_RELEASE_DOC', N'Document: {Document} has been released by {user}.')
INSERT [Setup].[NoticationMessage] ( [MessageType], [MessageKey], [MessageText]) VALUES ( N'Task', N'TASK_EV_COMPLETED', N'{message}')
select 'NoticationMessage Record inserted Successfully Created' as [record]
