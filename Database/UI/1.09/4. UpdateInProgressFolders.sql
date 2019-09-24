BEGIN
	BEGIN TRY
		BEGIN TRANSACTION

		DECLARE @startCnt INT
		DECLARE @totalCnt INT
		DECLARE @folderVersionId INT
		
		If Object_ID ('[dbo].[FolderVersionTableIDs]') Is Not Null
			BEGIN
			DROP TABLE FolderVersionTableIDs
			END
			SET @startCnt = 1
		--Insert into temp FolderVersionTableIDs tbl For all folder IDs
		
		CREATE TABLE FolderVersionTableIDs(
		FolderVersionID INT,
		ID  INT IDENTITY(1,1)
		);
			
		INSERT INTO FolderVersionTableIDs (FolderVersionID) 
		SELECT FolderVersionID FROM fldr.FolderVersion where categoryId > 0 and FolderVersionStateID in (1)

	    SELECT  * FROM FolderVersionTableIDs 
		SELECT  @totalCnt= ID FROM FolderVersionTableIDs 

		
		WHILE @startCnt <= @totalCnt
		BEGIN
			SELECT @folderVersionId = FolderVersionID FROM FolderVersionTableIDs WHERE ID=@startCnt
----------------------------------------------------------------------------
--DELETE--
---------------------------------------------------------------------------- 
				PRINT 'Start..'
				PRINT @startCnt
				PRINT @folderVersionId
				DELETE FROM FLDR.FolderVersionWorkFlowState WHERE FolderVersionID = @folderVersionId
				INSERT [Fldr].[FolderVersionWorkFlowState] ([TenantID], [IsActive], [AddedDate], [AddedBy], [UpdatedDate], [UpdatedBy], [FolderVersionID], [WFStateID], [ApprovalStatusID], [Comments], [UserID]) VALUES (1, 1, CAST(N'2019-04-04T13:44:15.940' AS DateTime), N'superuser', CAST(N'2019-04-04T13:47:09.967' AS DateTime), N'superuser', @folderVersionId, 55, 2, N'', 1228)
				Update fldr.FolderVersion set WFStateID = 55, CategoryID = 10 where FolderVersionID = @folderVersionId	
					           
---------------------------------------------------------------------------
			SET @startCnt=@startCnt + 1
			
		END
	

	

	Update Fldr.Journal set AddedWFStateID = 55
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