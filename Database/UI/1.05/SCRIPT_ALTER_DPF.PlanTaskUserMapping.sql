


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Duration'
          AND Object_ID = Object_ID(N'DPF.PlanTaskUserMapping'))
BEGIN   
	ALTER TABLE [DPF].[PlanTaskUserMapping]
		ADD Duration INT 
END

UPDATE [DPF].[PlanTaskUserMapping]
SET Duration = 0
where Duration  is null
