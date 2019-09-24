BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
--delete all workflows except General

Update fldr.Journal set AddedWFStateID = 40
update fldr.FolderVersionWorkFlowState set WFStateID = 40 where FolderVersionID in (select FolderVersionID from Fldr.FolderVersion where CategoryID > 0 and CategoryID <> 7)
--update fldr.FolderVersion set CategoryID = 7, WFStateID = 40
update fldr.FolderVersion set CategoryID = 7, WFStateID = 40 where FolderVersionID in (select FolderVersionID from Fldr.FolderVersion where CategoryID > 0 and CategoryID <> 7)
--select FolderVersionCategoryID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID <> 7
--delete from WF.WFVersionStatesApprovalType where WorkFlowVersionStateID in (select WorkFlowVersionStateID from WF.WorkFlowVersionState where WorkFlowVersionID in (select WorkFlowVersionID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID in (select FolderVersionCategoryID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID <> 7)))

delete from WF.WFStatesApprovalTypeAction where WFVersionStatesApprovalTypeID in (select WFVersionStatesApprovalTypeID from WF.WFVersionStatesApprovalType where WorkFlowVersionStateID in (select WorkFlowVersionStateID from WF.WorkFlowVersionState where WorkFlowVersionID in (select WorkFlowVersionID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID in (select FolderVersionCategoryID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID <> 7))))
delete from WF.WFVersionStatesApprovalType where WorkFlowVersionStateID in (select WorkFlowVersionStateID from WF.WorkFlowVersionState where WorkFlowVersionID in (select WorkFlowVersionID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID in (select FolderVersionCategoryID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID <> 7)))
delete from WF.WorkFlowVersionStatesAccess where WorkFlowVersionStateID in (select WorkFlowVersionStateID from WF.WorkFlowVersionState where WorkFlowVersionID in (select WorkFlowVersionID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID in (select FolderVersionCategoryID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID <> 7)))
delete from WF.WorkFlowVersionState where WorkFlowVersionID in (select WorkFlowVersionID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID in (select FolderVersionCategoryID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID <> 7))
delete from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID in (select FolderVersionCategoryID from [WF].[WorkFlowCategoryMapping] where FolderVersionCategoryID <> 7)

delete from fldr.FolderVersionCategory where FolderVersionCategoryName <> 'General'
delete from [WF].[State] where WFStateID > 27
--delete from [DPF].[WFStateTaskMapping]
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