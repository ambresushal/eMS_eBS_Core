ALTER PROCEDURE [dbo].[uspSaveFormInstanceSectionsData]   
@FormInstanceSectionList FormInstanceSectionType readonly,  
@TenantID INT,  
@CurrentUserName NVARCHAR(50)  
AS  
 BEGIN TRY  
  BEGIN TRANSACTION  
  DECLARE @FormData NVARCHAR(MAX)  
  DECLARE @FormInstanceID INT   
  DECLARE @FolderVersionID INT   
  DECLARE @FolderID INT   
  DECLARE @SectionName NVARCHAR(MAX)  
  DECLARE @SectionData NVARCHAR(MAX)  
    
  SELECT * INTO #temp FROM @FormInstanceSectionList  
  SET @FormInstanceID = (SELECT TOP 1 FormInstanceID FROM #temp)  
  SET @FormData =(SELECT FormData FROM Fldr.FormInstanceDataMap WHERE FormInstanceID=@formInstanceID)  
    
  WHILE EXISTS(select top 1  *FROM  #temp)  
  BEGIN  
   SELECT TOP 1 @SectionName=SectionName , @SectionData=dbo.UnZip(SectionData) FROM #temp  
   SET @FormData = (SELECT [dbo].[ReplacePart](@FormData,@SectionName,@SectionData,1))  
   DELETE TOP(1) FROM #temp  
  END  
  -- Update into table  
  UPDATE Fldr.FormInstanceDataMap SET FormData= @formData WHERE FormInstanceID=@FormInstanceID  
  UPDATE Fldr.FormInstance SET UpdatedBy= @CurrentUserName, UpdatedDate =  GETDATE() WHERE FormInstanceID=@FormInstanceID   
  -- Save FormInstanceHistory  
  INSERT INTO Arc.FormInstanceHistory VALUES(@FormInstanceID,@CurrentUserName ,GETDATE(),@TenantID ,'Update',CAST(dbo.GZip(@FormData) as varbinary(max)))  
    
  -- Update FolderChange   
  SET @FolderVersionID=(SELECT FolderVersionID FROM Fldr.FormInstance WHERE FormInstanceID= @FormInstanceID)  
  SET @FolderID=(SELECT FolderID FROM Fldr.FolderVersion WHERE FolderVersionID= @FolderVersionID)  
  UPDATE Fldr.Folder SET UpdatedBy=@CurrentUserName, UpdatedDate=GETDATE() WHERE FolderID=@FolderID  
  UPDATE Fldr.FolderVersion SET UpdatedBy=@CurrentUserName, UpdatedDate=GETDATE() WHERE FolderID=@FolderID and FolderVersionID = @FolderVersionID  
    
  -- Return forminstancedatamap for @FormInstanceID  
  SELECT FormInstanceDataMapID, FormInstanceID,ObjectInstanceID, '' as FormData, '' as CompressJsonData FROM Fldr.FormInstanceDataMap WHERE FormInstanceID=@FormInstanceID  
    
  COMMIT TRANSACTION  
 END TRY  
 BEGIN CATCH  
  ROLLBACK TRANSACTION  
 END CATCH  
  