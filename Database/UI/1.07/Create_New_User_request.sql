

Execute [dbo].[Create_New_User] 'UserIDViewer','Viewer','test@simplifyhealthcare.com','Viewer','Viewer','Superuser'
Go
	Execute [dbo].[Create_New_User] 'UserIDReviewer','Reviewer','test@simplifyhealthcare.com','Reviewer','','Superuser'
	Go
	Execute [dbo].[Create_New_User] 'UserIDPoweruser','Power User','test@simplifyhealthcare.com','Poweruser','','Superuser'
	Go
	Execute [dbo].[Create_New_User] 'UserIDClientSuperuser','Client SuperUser','test@simplifyhealthcare.com','ClientSuperuser','','Superuser'
	Go
	Execute [dbo].[Create_New_User] 'UserIDProductDesigner','Product Level Designer','test@simplifyhealthcare.com','ProductDesigner','','Superuser'

