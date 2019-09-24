
/****** Object:  StoredProcedure [dbo].[Create_New_User]    Script Date: 12-03-2019 15:58:43 ******/
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
	SELECT TOP 1 * from sec.[User] where UserID=1
			

		
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

