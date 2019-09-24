Execute [dbo].[Create_New_User] 'Viewer','Viewer','test@simplifyhealthcare.com','Viewer','Viewer','Superuser'
Go
	Execute [dbo].[Create_New_User] 'Reviewer','Reviewer','test@simplifyhealthcare.com','Reviewer','','Superuser'
	Go
	Execute [dbo].[Create_New_User] 'Poweruser','Power User','test@simplifyhealthcare.com','Poweruser','','Superuser'
	Go
	Execute [dbo].[Create_New_User] 'ClientSuperuser','Client SuperUser','test@simplifyhealthcare.com','ClientSuperuser','','Superuser'
	Go
	Execute [dbo].[Create_New_User] 'ProductDesigner','Product Core Admin Designer','test@simplifyhealthcare.com','ProductDesigner','','Superuser'