
/****** Object:  StoredProcedure [dbo].[Create_New_User]    Script Date: 3/1/2019 3:30:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[Create_New_User]
	-- Add the parameters for the stored procedure here
		@username VARCHAR(20),
		@role VARCHAR(100),
		@email VARCHAR(50),
		@firstname VARCHAR(100),
		@lastname VARCHAR(100),
		@createdBy VARCHAR(50)
AS
	BEGIN
		BEGIN TRY
		BEGIN TRANSACTION
			DECLARE @RoleID_Assoc INT
			DECLARE @USERID_Assoc INT
	
			BEGIN
	     	INSERT [Sec].[User] ([UserName], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [TenantID],[FirstName],[LastName],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate]) 
			
			VALUES (@username, @email, 1, N'ABuRmZu+LVr0X4mR5r9gYLPRW+5On3Ae026hTiCiQUzHtXzZG4EYj4kq3SDDJgJQ9Q==', N'8882847c-85b2-499c-8ad7-3c9b0e0bceb0', N'12@UserID456789', 1, 1, CAST(0x0000A2A600000000 AS DateTime), 1, 0, 1,@firstname,@lastname,@createdBy,GETDATE(),@createdBy,GETDATE())
			SELECT @RoleID_Assoc = [RoleID] FROM [Sec].[UserRole] where Name = @role
			SELECT @USERID_Assoc = [UserID] FROM [Sec].[User] where UserName = @username

			INSERT [Sec].[UserRoleAssoc] ([UserID], [RoleID]) VALUES (@USERID_Assoc, @RoleID_Assoc)
	
	END
	
		/* Fetch UserID whose Role is EBA Analyst  */

		DECLARE @StartCount INT
		DECLARE @USERID INT
		DECLARE @NEWUSERID INT
		DECLARE @RoleID INT
		DECLARE @tempUserCount INT
		DECLARE @tempUserId TABLE
		(
		  ID INT Identity, 
		  UserID INT
		)
		/*INSERT UserID list into temp Table   */
		SELECT @RoleID= [RoleID] FROM [Sec].[UserRole] where[Name] =@role

		INSERT INTO @tempUserId (UserID) 
		SELECT [UserID] FROM [Sec].[User] where UserName = @username

		/* Delete */
		SELECT @NEWUSERID = [UserID] FROM [Sec].[User] where UserName = @username
		DELETE FROM [Sec].[UserClaim] WHERE RoleID=@RoleID AND UserId = @NEWUSERID AND ResourceType = 'Section' AND ResourceID IS NULL
		--PRINT @NEWUSERID
		/* END Delete */

		SELECT @tempUserCount=COUNT(1) FROM @tempUserId
		SET @StartCount=1
			WHILE (@StartCount <= @tempUserCount)
			BEGIN
				SELECT @UserID=UserID FROM @tempUserId WHERE ID=@StartCount
			

			--If role is EBA Analystr...
			IF @role = 'EBA Analyst'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
						/**Menu Permissions**/
						('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
						('Menu', 'Account', @RoleId, @UserID, 'Section'),
						('Menu', 'COMPAREANDSYNC', @RoleId, @UserID, 'Section'),	
						('Menu', 'GLOBALUPDATE', @RoleId, @UserID, 'Section'),
								

						 /**Home/Dashboard Permissions**/
						('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'OverrideLock', @RoleId, @UserID, 'Section'),						
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
																	
						
						/**Portfolio Search Permissions**/
						('ConsumerAccount/GetPortfolioList ', 'Retro', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'OverrideLock', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchNew', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchEdit', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchCopy', @RoleId, @UserID, 'Section'),	
						
						 
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Retro', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'OverrideLock', @RoleId, @UserID, 'Section'),


						/**Account Search Permissions**/ 
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Retro', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'OverrideLock', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioadd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioedit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfoliocopy', @RoleId, @UserID, 'Section'),
						
						/**Account Manage & Consortium Permissions( same as account search) **/
						('ConsumerAccount/GetAccountList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetAccountList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetAccountList', 'Delete', @RoleId, @UserID, 'Section'),
						('Consortium/GetConsortiumList', 'New', @RoleId, @UserID, 'Section'),
						('Consortium/GetConsortiumList', 'Edit', @RoleId, @UserID, 'Section'),	
						
						/**Consumer Account ManageAccount Permissions**/
						('ConsumerAccount/ManageAccount', 'btnManageAccountAdd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/ManageAccount', 'btnManageAccountEdit', @RoleId, @UserID, 'Section'),

						/**Account Folders Permissions**/
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'DeleteInstance', 22, @UserID, 'Section'), 
						/* Account Folders - Version History Permissions*/				
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'UnlockFolder', @RoleId, @UserID, 'Section'), --for lock/unlock icon
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						
						 /**Folder Creation PopUp Permissions**/
						('ConsumerAccount/GetAccountList', 'Display portfolio folder question', @RoleId, @UserID, 'Section'),

						 /**Portfolio Folders/ Non Porfolio Permissions**/				
						('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Baseline', @RoleId, @UserID, 'Section')

					
						 			
																	
				--PRINT @UserID
				SET @StartCount =@StartCount +1	
			END
			
			--If role is Product Designer Level 1...Shree
			IF @role = 'Power User'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
				
				('Menu','Dashboard',@RoleId, @UserID, 'Section'),
				('Menu','Masterlist',@RoleId, @UserID, 'Section'),
				('Menu','Portfolio',@RoleId, @UserID, 'Section'),
				('Menu','PBPIntegration',@RoleId, @UserID, 'Section'),
				('Menu','Report',@RoleId, @UserID, 'Section'),
				('Dashboard/GetWatchList','Edit',@RoleId, @UserID, 'Section'),
				('Dashboard/GetWatchList','View',@RoleId, @UserID, 'Section'),
				('Dashboard/GetWorkQueueListNotReleasedAndApproved','Edit',@RoleId, @UserID, 'Section'),
				('Dashboard/GetWorkQueueListNotReleasedAndApproved','View',@RoleId, @UserID, 'Section'),
				('Dashboard/GetWorkQueueListNotReleasedAndApproved','OverrideLock',@RoleId, @UserID, 'Section'),
				('ConsumerAccount/GetPortfolioList','View',@RoleId, @UserID, 'Section'),
				('ConsumerAccount/GetPortfolioList','Copy',@RoleId, @UserID, 'Section'),
				('ConsumerAccount/GetPortfolioList','btnPortfolioSearchView',@RoleId, @UserID, 'Section'),
				('ConsumerAccount/GetPortfolioList','btnPortfolioSearchCopy',@RoleId, @UserID, 'Section'),
				('ConsumerAccount/GetPortfolioList','btnPortfolioSearchEdit',@RoleId, @UserID, 'Section'),
				('ConsumerAccount/GetPortfolioList','btnPortfolioSearchNew',@RoleId, @UserID, 'Section'),
				('ConsumerAccount/GetPortfolioList','OverrideLock',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Baseline',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','StatusUpdate',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','VersionHistory',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','New',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Validate',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Save',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','DeleteInstance',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Edit',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Newversion',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Retro',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Edit',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','View',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Delete',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','Rollback',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','UnlockFolder',@RoleId, @UserID, 'Section'),
				('FolderVersion/Index','OverrideLock',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Baseline',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','StatusUpdate',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','VersionHistory',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','New',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Validate',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Save',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Newversion',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Retro',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Edit',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','View',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Delete',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Rollback',@RoleId, @UserID, 'Section'),
				('FolderVersion/IndexML','Release',@RoleId, @UserID, 'Section'),
				('Settings/Index','AutoSave',@RoleId, @UserID, 'Section'),
				('ReportingCenter/Index','btngeneratereport',@RoleId, @UserID, 'Section'),
				('PBPImport/PBPDataBase','new',@RoleId, @UserID, 'Section'),
				('PBPImport/PBPDataBase','edit',@RoleId, @UserID, 'Section'),
				('PBPImport/Index','new',@RoleId, @UserID, 'Section'),
				('PBPImport/Index','edit',@RoleId, @UserID, 'Section')
				

				--PRINT @UserID
				SET @StartCount =@StartCount +1
			END			
						
			--If role is TPA Analyst...
			IF @role = 'TPA Analyst'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
							/**Menu Permissions**/
						('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
						--('Menu', 'Design', @RoleId, @UserID, 'Section'),
						--('Menu', 'Masterlist', @RoleId, @UserID, 'Section'),
						('Menu', 'Account', @RoleId, @UserID, 'Section'),
						--('Menu', 'Report', @RoleId, @UserID, 'Section'),
						--('Menu', 'AUDIT', @RoleId, @UserID, 'Section'),	
						--('Menu', 'SETTING', @RoleId, @UserID, 'Section'),
						--('Menu', 'PDFCONFIGURATION', @RoleId, @UserID, 'Section'),
						--('Menu', 'GENERALSETTING', @RoleId, @UserID, 'Section'),
						--('Menu', 'WORKFLOWSETTING', @RoleId, @UserID, 'Section'),
						--('Menu', 'UserManagement', @RoleId, @UserID, 'Section'),
						('Menu', 'COMPAREANDSYNC', @RoleId, @UserID, 'Section'),	
						--('Menu', 'GLOBALUPDATE', @RoleId, @UserID, 'Section'),	
								

						 /**Home/Dashboard Permissions**/
						('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'OverrideLock', @RoleId, @UserID, 'Section'),						
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
																	
						
						/**Account Search Permissions**/ 
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Retro', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'OverrideLock', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Delete', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioadd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioedit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfoliocopy', @RoleId, @UserID, 'Section'),

						/**Consumer Account ManageAccount Permissions**/
						('ConsumerAccount/ManageAccount', 'btnManageAccountAdd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/ManageAccount', 'btnManageAccountEdit', @RoleId, @UserID, 'Section'),

						/**Account Manage & Consortium Permissions( same as account search) **/
						('ConsumerAccount/GetAccountList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetAccountList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetAccountList', 'Delete', @RoleId, @UserID, 'Section'),
						('Consortium/GetConsortiumList', 'New', @RoleId, @UserID, 'Section'),
						('Consortium/GetConsortiumList', 'Edit', @RoleId, @UserID, 'Section'),	
						
						
						/**Account Folders Permissions**/
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'DeleteInstance', 22, @UserID, 'Section'),
						/* Account Folders - Version History Permissions*/				
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'UnlockFolder', @RoleId, @UserID, 'Section'), --for lock/unlock icon
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'OverrideLock', @RoleId, @UserID, 'Section'),

					     /**Portfolio Folders / Non Porfolio Permissions**/					
						('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Retro', @RoleId, @UserID, 'Section'),
					    ('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'StatusUpdate', @RoleId, @UserID, 'Section'),
					    ('FolderVersion/Index', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Baseline', @RoleId, @UserID, 'Section'),
						
						 /**Folder Creation PopUp Permissions**/
						('ConsumerAccount/GetAccountList', 'Display portfolio folder question', @RoleId, @UserID, 'Section')
																	
				--PRINT @UserID
				SET @StartCount =@StartCount +1	
			END
			
			--If role is Viewer...
			IF @role = 'Viewer'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
           ([Resource]
			,[Action]
			,[RoleID]
			,[UserId]
		    ,[ResourceType])
		VALUES 
				  /**Menu Permissions**/		
					
				('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),	
				('Menu', 'Account', @RoleId, @UserID, 'Section'),
				('Menu', 'Report', @RoleId, @UserID, 'Section'),
				('Menu', 'Integration', @RoleId, @UserID, 'Section'),
				('Menu', 'GENERALSETTING', @RoleId, @UserID, 'Section'),
				('Menu', 'WORKFLOWSETTING', @RoleId, @UserID, 'Section'),
					
				 /**Form Design Permissions**/		
					--NA				
				
				 /**Home/Dashboard Permissions**/				
				('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),				
				('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
				
				/**Portfolio Search Permissions*					
				('ConsumerAccount/GetPortfolioList', 'View', @RoleId, @UserID, 'Section'),	
				 				
				('ConsumerAccount/GetPortfolioBasedAccountList', 'View', @RoleId, @UserID, 'Section'),*/			
	            
	            /**Account Search Permissions**/
				('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),					
				
				/**Account Folders Permissions**/
				--Only View Permission to be given to this user.			
				('FolderVersion/GetNonPortfolioBasedAccountFolders', 'VersionHistory', @RoleId, @UserID, 'Section'),
				('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
				
				 /**Portfolio Folders Permissions*				
				('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),			
				('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),*/
				
				 /**Folder Creation PopUp Permissions**/
				('ConsumerAccount/GetAccountList', 'NA', @RoleId, @UserID, 'Section')	
				


				--PRINT @UserID
				SET @StartCount =@StartCount +1
			END

				--If role is Reviewer...
			IF @role = 'Reviewer'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
           ([Resource]
			,[Action]
			,[RoleID]
			,[UserId]
		    ,[ResourceType])
		VALUES 
				  /**Menu Permissions**/		
					
				('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),	
				('Menu', 'Account', @RoleId, @UserID, 'Section'),
				('Menu', 'Report', @RoleId, @UserID, 'Section'),
				('Menu', 'Integration', @RoleId, @UserID, 'Section'),
				('Menu', 'GENERALSETTING', @RoleId, @UserID, 'Section'),
				('Menu', 'WORKFLOWSETTING', @RoleId, @UserID, 'Section'),
					
				 /**Form Design Permissions**/		
					--NA				
				
				 /**Home/Dashboard Permissions**/				
				('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),				
				('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
				
				/**Portfolio Search Permissions*					
				('ConsumerAccount/GetPortfolioList', 'View', @RoleId, @UserID, 'Section'),	
				 				
				('ConsumerAccount/GetPortfolioBasedAccountList', 'View', @RoleId, @UserID, 'Section'),*/			
	            
	            /**Account Search Permissions**/
				('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),					
				
				/**Account Folders Permissions**/
				--Only View Permission to be given to this user.			
				('FolderVersion/GetNonPortfolioBasedAccountFolders', 'VersionHistory', @RoleId, @UserID, 'Section'),
				('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section')
				
				 /**Portfolio Folders Permissions*				
				('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),			
				('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),*/
				
							


				--PRINT @UserID
				SET @StartCount =@StartCount +1
			END

			--If role is Product SME (collateral)...
			IF @role = 'Product SME (collateral)'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
				/**Menu Permissions**/
						('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
						('Menu', 'Portfolio', @RoleId, @UserID, 'Section'),
						('Menu', 'Design', @RoleId, @UserID, 'Section'),
						('Menu', 'Masterlist', @RoleId, @UserID, 'Section'),
						('Menu', 'Account', @RoleId, @UserID, 'Section'),
						('Menu', 'Report', @RoleId, @UserID, 'Section'),
					
						 /**Home/Dashboard Permissions**/
						('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'OverrideLock', @RoleId, @UserID, 'Section'),						
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						--/**Portfolio Search Permissions**/
						
						('ConsumerAccount/GetPortfolioList','btnPortfolioSearchView',@RoleId, @UserID, 'Section'),
						
						/**Account Search Permissions**/ 
						
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),
												
						/* Account Folders - Version History Permissions*/				
					
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
					
						/* Portfolio Folders - Version History Permissions*/				
						('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
										
						/* MasterList - Version History Permissions*/				
					
						('FolderVersion/Index/ML', 'View', @RoleId, @UserID, 'Section'),
						
						 /**Collateral Permissions**/
						('DocumentCollateral/ViewReportTemplate', 'CreateCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'CreateCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'DeleteCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'DeleteCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'createCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'createCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'editCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'editCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'deleteCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'deleteCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'finalizedCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'finalizedCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'QueueCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GenerateReports', 'QueueCollateralTemplate', @RoleId, @UserID, 'Section'),
							/**Report Permission**/
						('ReportingCenter/Index', 'btngeneratereport', @RoleId, @UserID, 'Section')


						
				--PRINT @UserID
				SET @StartCount =@StartCount +1	
			END	

			--If role is Product SME (core admin)...
			IF @role = 'Product SME (core admin)'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
				/**Menu Permissions**/
						('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
						('Menu', 'Portfolio', @RoleId, @UserID, 'Section'),
						('Menu', 'Design', @RoleId, @UserID, 'Section'),
						('Menu', 'Masterlist', @RoleId, @UserID, 'Section'),
						('Menu', 'Account', @RoleId, @UserID, 'Section'),
						('Menu', 'Report', @RoleId, @UserID, 'Section'),
					
						 /**Home/Dashboard Permissions**/
						('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'OverrideLock', @RoleId, @UserID, 'Section'),						
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						--/**Portfolio Search Permissions**/
						
						('ConsumerAccount/GetPortfolioList','btnPortfolioSearchView',@RoleId, @UserID, 'Section'),
						
						/**Account Search Permissions**/ 
						
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),
												
						/* Account Folders - Version History Permissions*/				
					
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
					
						/* Portfolio Folders - Version History Permissions*/				
						('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
										
						/* MasterList - Version History Permissions*/				
					
						('FolderVersion/Index/ML', 'View', @RoleId, @UserID, 'Section'),
						
						-- /**Collateral Permissions**/
						--('DocumentCollateral/ViewReportTemplate', 'CreateCollateralTemplate', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/GetReportNames', 'CreateCollateralTemplate', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/ViewReportTemplate', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/ViewReportTemplate', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/GetReportNames', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/ViewReportTemplate', 'DeleteCollateralTemplate', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/GetReportNames', 'DeleteCollateralTemplate', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/ViewReportTemplate', 'createCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/GetReportNames', 'createCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/ViewReportTemplate', 'editCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/GetReportNames', 'editCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/ViewReportTemplate', 'deleteCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/GetReportNames', 'deleteCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/ViewReportTemplate', 'finalizedCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/GetReportNames', 'finalizedCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/ViewReportTemplate', 'QueueCollateralTemplate', @RoleId, @UserID, 'Section'),
						--('DocumentCollateral/GenerateReports', 'QueueCollateralTemplate', @RoleId, @UserID, 'Section'),

							/**Report Permission**/
						('ReportingCenter/Index', 'btngeneratereport', @RoleId, @UserID, 'Section')

						
				--PRINT @UserID
				SET @StartCount =@StartCount +1	
			END	

			
			--If role is Product Designer Level 2...
			IF @role = 'Product Level Designer'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
						/**Menu Permissions**/
						('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
						('Menu', 'Portfolio', @RoleId, @UserID, 'Section'),
						('Menu', 'Design', @RoleId, @UserID, 'Section'),
						('Menu', 'Masterlist', @RoleId, @UserID, 'Section'),
						('Menu', 'Account', @RoleId, @UserID, 'Section'),
						('Menu', 'Report', @RoleId, @UserID, 'Section'),
						('Menu', 'SETTING', @RoleId, @UserID, 'Section'),
						('Menu', 'WORKFLOWSETTING', @RoleId, @UserID, 'Section'),
						
						
					

						 /**Home/Dashboard Permissions**/
						('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'OverrideLock', @RoleId, @UserID, 'Section'),						
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						
						/**Portfolio Search Permissions**/
						('ConsumerAccount/GetPortfolioList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'OverrideLock', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchNew', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchEdit', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchCopy', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/GetPortfolioBasedAccountList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						/**Account Search Permissions**/ 
					
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'OverrideLock', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'DeleteInstance', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Delete', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioadd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioedit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfoliocopy', @RoleId, @UserID, 'Section'),
						
						('ConsumerAccount/AccountSearch', 'btnNonPortfolioRemoveFolder', @RoleId, @UserID, 'Section'),
						
						/**Account Folders Permissions**/
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						/* Account Folders - Version History Permissions*/				
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'DeleteInstance', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'UnlockFolder', @RoleId, @UserID, 'Section'), --for lock/unlock icon
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						 /**Portfolio Folders Permissions**/				
						('FolderVersion/Index', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'DeleteInstance', @RoleId, @UserID, 'Section'),
												
						/* Portfolio Folders - Version History Permissions*/				
						('FolderVersion/Index', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'UnlockFolder', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'OverrideLock', @RoleId, @UserID, 'Section'),
											
						/* MasterList - Version History Permissions*/				
						
						('FolderVersion/Index/ML', 'View', @RoleId, @UserID, 'Section'),
												
						 /**Folder Creation PopUp Permissions**/
						('ConsumerAccount/GetAccountList', 'Display portfolio folder question', @RoleId, @UserID, 'Section'),

						/**Report Permission**/
						('ReportingCenter/Index', 'btngeneratereport', @RoleId, @UserID, 'Section')
						 
						
				--PRINT @UserID
				SET @StartCount =@StartCount +1	
			END	

				--If role is Product Core Admin Designer Level 2...
			IF @role = 'Product Core Admin Designer'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
						/**Menu Permissions**/
						('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
						('Menu', 'Portfolio', @RoleId, @UserID, 'Section'),
						('Menu', 'Design', @RoleId, @UserID, 'Section'),
						('Menu', 'Masterlist', @RoleId, @UserID, 'Section'),
						('Menu', 'Account', @RoleId, @UserID, 'Section'),
						('Menu', 'Report', @RoleId, @UserID, 'Section'),
						('Menu', 'SETTING', @RoleId, @UserID, 'Section'),
						('Menu', 'WORKFLOWSETTING', @RoleId, @UserID, 'Section'),
						
						 /**Home/Dashboard Permissions**/
						('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'OverrideLock', @RoleId, @UserID, 'Section'),						
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						
						/**Portfolio Search Permissions**/
						
						('ConsumerAccount/GetPortfolioList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'OverrideLock', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchNew', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchEdit', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchCopy', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/GetPortfolioBasedAccountList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						/**Account Search Permissions**/ 
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Retro', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'OverrideLock', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'DeleteInstance', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Delete', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioadd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioedit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfoliocopy', @RoleId, @UserID, 'Section'),
						
						('ConsumerAccount/AccountSearch', 'btnNonPortfolioRemoveFolder', @RoleId, @UserID, 'Section'),

						/**Account Folders Permissions**/
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						/* Account Folders - Version History Permissions*/				
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'DeleteInstance', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'UnlockFolder', @RoleId, @UserID, 'Section'), --for lock/unlock icon
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						 /**Portfolio Folders Permissions**/				
						('FolderVersion/Index', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'DeleteInstance', @RoleId, @UserID, 'Section'),
												
						/* Portfolio Folders - Version History Permissions*/				
						('FolderVersion/Index', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'UnlockFolder', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'OverrideLock', @RoleId, @UserID, 'Section'),
										
										
						/* MasterList - Version History Permissions*/				
						('FolderVersion/Index/ML', 'View', @RoleId, @UserID, 'Section'),
						
						
						 /**Folder Creation PopUp Permissions**/
						('ConsumerAccount/GetAccountList', 'Display portfolio folder question', @RoleId, @UserID, 'Section')
						 
						

						
																	
				--PRINT @UserID
				SET @StartCount =@StartCount +1	
			END	

			--If role is Product Core Admin Designer Level 1...
			IF @role = 'Core Admin Power User'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
						/**Menu Permissions**/
						('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
						('Menu', 'Portfolio', @RoleId, @UserID, 'Section'),
						('Menu', 'Design', @RoleId, @UserID, 'Section'),
						('Menu', 'Masterlist', @RoleId, @UserID, 'Section'),
						('Menu', 'Account', @RoleId, @UserID, 'Section'),
						('Menu', 'Report', @RoleId, @UserID, 'Section'),
						('Menu', 'SETTING', @RoleId, @UserID, 'Section'),
						('Menu', 'WORKFLOWSETTING', @RoleId, @UserID, 'Section'),
																

						 /**Home/Dashboard Permissions**/
						('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'OverrideLock', @RoleId, @UserID, 'Section'),						
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						
						/**Portfolio Search Permissions**/
						
						('ConsumerAccount/GetPortfolioList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'OverrideLock', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchNew', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchEdit', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchCopy', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/GetPortfolioBasedAccountList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'OverrideLock', @RoleId, @UserID, 'Section'),
				
						/**Account Search Permissions**/ 
						
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'OverrideLock', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'DeleteInstance', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Delete', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioadd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioedit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfoliocopy', @RoleId, @UserID, 'Section'),
						
						('ConsumerAccount/AccountSearch', 'btnNonPortfolioRemoveFolder', @RoleId, @UserID, 'Section'),
					
						
						/**Account Folders Permissions**/
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						/* Account Folders - Version History Permissions*/				
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'DeleteInstance', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'UnlockFolder', @RoleId, @UserID, 'Section'), --for lock/unlock icon
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						 /**Portfolio Folders Permissions**/				
						('FolderVersion/Index', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'DeleteInstance', @RoleId, @UserID, 'Section'),
												
						/* Portfolio Folders - Version History Permissions*/				
						('FolderVersion/Index', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'UnlockFolder', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'OverrideLock', @RoleId, @UserID, 'Section'),
										
						/**MasterList Permissions.**/
						('FolderVersion/Index/ML', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/IndexML', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/IndexML', 'Save', @RoleId, @UserID, 'Section'),							
						/* MasterList - Version History Permissions*/				
						('FolderVersion/Index/ML', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Rollback', @RoleId, @UserID, 'Section'),
						
						 /**Folder Creation PopUp Permissions**/
						('ConsumerAccount/GetAccountList', 'Display portfolio folder question', @RoleId, @UserID, 'Section')
						 
						
						

						
																	
				--PRINT @UserID
				SET @StartCount =@StartCount +1	
			END	


			--If role is Simplify SuperUser...
			IF @role = 'Simplify SuperUser'
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
						/**Menu Permissions**/
						('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
						('Menu', 'Portfolio', @RoleId, @UserID, 'Section'),
						('Menu', 'Design', @RoleId, @UserID, 'Section'),
						('Menu', 'Masterlist', @RoleId, @UserID, 'Section'),
						('Menu', 'Account', @RoleId, @UserID, 'Section'),
						('Menu', 'Report', @RoleId, @UserID, 'Section'),
						--('Menu', 'AUDIT', @RoleId, @UserID, 'Section'),	
						('Menu', 'SETTING', @RoleId, @UserID, 'Section'),
						('Menu', 'PDFCONFIGURATION', @RoleId, @UserID, 'Section'),
						--('Menu', 'GENERALSETTING', @RoleId, @UserID, 'Section'),
						('Menu', 'WORKFLOWSETTING', @RoleId, @UserID, 'Section'),
						('Menu', 'UserManagement', @RoleId, @UserID, 'Section'),
						('Menu', 'COMPAREANDSYNC', @RoleId, @UserID, 'Section'),	
						('Menu', 'GLOBALUPDATE', @RoleId, @UserID, 'Section'),	

				
						 /**Form Design Permissions && Data Source Permissions.**/		
						('FormDesign/FormDesignList',   'Edit', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignList',   'View', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignList',   'New', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignList',   'Delete', @RoleId, @UserID, 'Section'),
						
						('FormDesign/FormDesignVersionList', 'Edit', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignVersionList', 'View', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignVersionList', 'New', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignVersionList', 'Delete', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignVersionList', 'Finalized', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignVersionList', 'Preview', @RoleId, @UserID, 'Section'),
						('FormDesign/FormDesignVersionList', 'Compile', @RoleId, @UserID, 'Section'),
										
						('FormDesign/FormDesignVersionList', 'DataSource', @RoleId, @UserID, 'Section'),						
						
						/**Design Folder Permissions**/		
						('FormDesignGroup/FormDesignGroupList',   'New', @RoleId, @UserID, 'Section'),
						('FormDesignGroup/FormDesignGroupList',   'Edit', @RoleId, @UserID, 'Section'),				
						('FormDesignGroup/FormGroupMappingList',   'Save', @RoleId, @UserID, 'Section'),			
										

						 /**Home/Dashboard Permissions**/
						('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWatchList', 'OverrideLock', @RoleId, @UserID, 'Section'),						
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
						('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						
						/**Portfolio Search Permissions**/
						('ConsumerAccount/GetPortfolioList ', 'Retro', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioList', 'OverrideLock', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchNew', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchEdit', @RoleId, @UserID, 'Section'),	
						('ConsumerAccount/PortfolioSearch', 'btnPortfolioSearchCopy', @RoleId, @UserID, 'Section'),	
						
						 
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Retro', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetPortfolioBasedAccountList', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						/**Consumer Account ManageAccount Permissions**/
						('ConsumerAccount/ManageAccount', 'btnManageAccountAdd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/ManageAccount', 'btnManageAccountEdit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/ManageAccount', 'btnManageAccountDelete', @RoleId, @UserID, 'Section'),

						/**Account Search Permissions**/ 
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Retro', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'View', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Copy', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'OverrideLock', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'DeleteInstance', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'Delete', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioadd', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfolioedit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetNonPortfolioBasedAccountist', 'btnnonportfoliocopy', @RoleId, @UserID, 'Section'),
						
						('ConsumerAccount/AccountSearch', 'btnNonPortfolioRemoveFolder', @RoleId, @UserID, 'Section'),

						/**Account Manage & Consortium Permissions( same as account search) **/
						('ConsumerAccount/GetAccountList', 'New', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetAccountList', 'Edit', @RoleId, @UserID, 'Section'),
						('ConsumerAccount/GetAccountList', 'Delete', @RoleId, @UserID, 'Section'),
						('Consortium/GetConsortiumList', 'New', @RoleId, @UserID, 'Section'),
						('Consortium/GetConsortiumList', 'Edit', @RoleId, @UserID, 'Section'),	
						
						
						/**Account Folders Permissions**/
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						/* Account Folders - Version History Permissions*/				
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'DeleteInstance', @RoleId, @UserID, 'Section'),
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'UnlockFolder', @RoleId, @UserID, 'Section'), --for lock/unlock icon
						('FolderVersion/GetNonPortfolioBasedAccountFolders', 'OverrideLock', @RoleId, @UserID, 'Section'),
						
						 /**Portfolio Folders Permissions**/				
						('FolderVersion/Index', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'DeleteInstance', @RoleId, @UserID, 'Section'),
												
						/* Portfolio Folders - Version History Permissions*/				
						('FolderVersion/Index', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'Rollback', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'UnlockFolder', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index', 'OverrideLock', @RoleId, @UserID, 'Section'),
										
						/**MasterList Permissions.**/
						('FolderVersion/Index/ML', 'Baseline', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'StatusUpdate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'VersionHistory', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'New', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Save', @RoleId, @UserID, 'Section'),
						('FolderVersion/IndexML', 'Validate', @RoleId, @UserID, 'Section'),
						('FolderVersion/IndexML', 'Save', @RoleId, @UserID, 'Section'),							
						/* MasterList - Version History Permissions*/				
						('FolderVersion/Index/ML', 'Newversion', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Retro', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Edit', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'View', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Delete', @RoleId, @UserID, 'Section'),
						('FolderVersion/Index/ML', 'Rollback', @RoleId, @UserID, 'Section'),
						
						 /**Folder Creation PopUp Permissions**/
						('ConsumerAccount/GetAccountList', 'Display portfolio folder question', @RoleId, @UserID, 'Section'),
						 
						 /**Settings Menu Permissions**/
						('Settings/Index', 'AutoSave', @RoleId, @UserID, 'Section'),
						
						/**WorkFlowSettings Permissions**/
						('Settings/Index', 'WorkFlowSettings', @RoleId, @UserID, 'Section'),

						 /**Collateral Permissions**/
						('DocumentCollateral/ViewReportTemplate', 'CreateCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'CreateCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'EditCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'DeleteCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'DeleteCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'createCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'createCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'editCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'editCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'deleteCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'deleteCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'finalizedCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GetReportNames', 'finalizedCollateralTemplateVersion', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/ViewReportTemplate', 'QueueCollateralTemplate', @RoleId, @UserID, 'Section'),
						('DocumentCollateral/GenerateReports', 'QueueCollateralTemplate', @RoleId, @UserID, 'Section'),

						/**GlobalUpdate Permissions**/
						('GlobalUpdate/BatchExecutionStatus', 'downloadAuditReport', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/BatchExecutionStatus', 'rollbackBatch', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/BatchExecution', 'batchAdd', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/BatchExecution', 'batchEdit', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/BatchExecution', 'batchDelete', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/BatchExecution', 'approveBatch', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/BatchExecution', 'realtimeBatchExecution', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/ExistsingGUGrid', 'globalUpdateAdd', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/ExistsingGUGrid', 'globalUpdateEdit', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/ExistsingGUGrid', 'globalUpdateUploadIAS', @RoleId, @UserID, 'Section'),
						('GlobalUpdate/ExistsingGUGrid', 'globalUpdateCopy', @RoleId, @UserID, 'Section'),
						('PrintTemplate/PDFConfiguration', 'PDFConfigurationAdd', @RoleId, @UserID, 'Section'),
						('PrintTemplate/PDFConfiguration', 'PDFConfigurationDelete', @RoleId, @UserID, 'Section')
																	
				--PRINT @UserID
				SET @StartCount =@StartCount +1	
			END	


--If role is Client SuperUser ...
			IF @role = 'Client SuperUser '
			BEGIN
				INSERT INTO [Sec].[UserClaim]
				   ([Resource]
					,[Action]
					,[RoleID]
					,[UserId]
					,[ResourceType]) 
					
				VALUES 
		('Menu', 'Dashboard', @RoleId, @UserID, 'Section'),
		('Menu', 'Design', @RoleId, @UserID, 'Section'),
		('Menu', 'Masterlist', @RoleId, @UserID, 'Section'),
		('Menu', 'Portfolio', @RoleId, @UserID, 'Section'),
		('Menu', 'PBPIntegration', @RoleId, @UserID, 'Section'),
		('Menu', 'Report', @RoleId, @UserID, 'Section'),
		('Menu', 'WORKFLOWSETTING', @RoleId, @UserID, 'Section'),
		('Menu', 'SETTING', @RoleId, @UserID, 'Section'),
		('FormDesign/FormDesignList', 'Edit', @RoleId, @UserID, 'Section'),	
('FormDesign/FormDesignList', 'View', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignList', 'New', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignList', 'Delete', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignVersionList', 'Edit', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignVersionList', 'View', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignVersionList', 'New', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignVersionList', 'Delete', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignVersionList', 'Finalized', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignVersionList', 'Preview', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignVersionList', 'Compile', @RoleId, @UserID, 'Section'),
('FormDesign/FormDesignVersionList', 'DataSource', @RoleId, @UserID, 'Section'),
('FormDesignGroup/FormDesignGroupList', 'New', @RoleId, @UserID, 'Section'),
('FormDesignGroup/FormDesignGroupList', 'Edit', @RoleId, @UserID, 'Section'),
('FormDesignGroup/FormGroupMappingList', 'Save', @RoleId, @UserID, 'Section'),
('Dashboard/GetWatchList', 'Edit', @RoleId, @UserID, 'Section'),
('Dashboard/GetWatchList', 'View', @RoleId, @UserID, 'Section'),
('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'Edit', @RoleId, @UserID, 'Section'),
('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'View', @RoleId, @UserID, 'Section'),
('Dashboard/GetWorkQueueListNotReleasedAndApproved', 'OverrideLock', @RoleId, @UserID, 'Section'),
('ConsumerAccount/GetPortfolioList', 'View', @RoleId, @UserID, 'Section'),
('ConsumerAccount/GetPortfolioList', 'Copy', @RoleId, @UserID, 'Section'),
('ConsumerAccount/GetPortfolioList', 'btnPortfolioSearchView', @RoleId, @UserID, 'Section'),
('ConsumerAccount/GetPortfolioList', 'btnPortfolioSearchCopy', @RoleId, @UserID, 'Section'),
('ConsumerAccount/GetPortfolioList', 'btnPortfolioSearchEdit', @RoleId, @UserID, 'Section'),
('ConsumerAccount/GetPortfolioList', 'btnPortfolioSearchNew', @RoleId, @UserID, 'Section'),
('ConsumerAccount/GetPortfolioList', 'OverrideLock', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Baseline', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'StatusUpdate', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'VersionHistory', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'New', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Validate', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Save', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'DeleteInstance', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Edit', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Newversion', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Retro', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Edit', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'View', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Delete', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'Rollback', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'UnlockFolder', @RoleId, @UserID, 'Section'),
('FolderVersion/Index', 'OverrideLock', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'Baseline', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'StatusUpdate', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'VersionHistory', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'New', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'Validate', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'Save', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'Newversion', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'Retro', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'Edit', @RoleId, @UserID, 'Section'),
('FolderVersion/IndexML', 'View', @RoleId, @UserID, 'Section'),
		('FolderVersion/IndexML', 'Delete', @RoleId, @UserID, 'Section'),
		('FolderVersion/IndexML', 'Rollback', @RoleId, @UserID, 'Section'),
		('FolderVersion/IndexML', 'Release', @RoleId, @UserID, 'Section'),
		('Settings/Index', 'AutoSave', @RoleId, @UserID, 'Section'),
		('Settings/Index', 'WorkFlowSettings', @RoleId, @UserID, 'Section'),
		('ReportingCenter/Index', 'btngeneratereport', @RoleId, @UserID, 'Section'),
		('PBPImport/PBPDataBase', 'new', @RoleId, @UserID, 'Section'),
		('PBPImport/PBPDataBase', 'edit', @RoleId, @UserID, 'Section'),
		('PBPImport/Index', 'new', @RoleId, @UserID, 'Section'),
		('PBPImport/Index', 'edit', @RoleId, @UserID, 'Section')
																
				--PRINT @UserID
			SET @StartCount =@StartCount +1	

			END		
			SELECT TOP 1 * from sec.[User] where UserID=1
			
		 END 
		
		 COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		SELECT 
			ERROR_NUMBER() AS ErrorNumber,
			ERROR_SEVERITY() AS ErrorSeverity,
			ERROR_STATE() as ErrorState,
			ERROR_PROCEDURE() as ErrorProcedure,
			ERROR_LINE() as ErrorLine,
			ERROR_MESSAGE() as ErrorMessage;    
		
	END CATCH
END

