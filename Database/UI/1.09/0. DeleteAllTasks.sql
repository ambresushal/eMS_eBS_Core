--select * from DPF.Tasks
--select * from DPF.PlanTaskUserMapping
--select * from DPF.TaskComments
--select * from DPF.WFStateTaskMapping
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
DELETE FROM DPF.TaskComments
DELETE FROM DPF.WFStateTaskMapping
DELETE FROM DPF.PlanTaskUserMapping
DELETE FROM DPF.Tasks
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