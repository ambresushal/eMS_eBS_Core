/****** Script for SelectTopNRows command from SSMS  ******/
--SELECT *   FROM [eMedicareSync_HAMP_UAT].[Frmk].[EmailTemplate] where EmailContent like '%Wellcare%'
--UPDATE EMAIL NOTIFICATION CONTENT
UPDATE [Frmk].[EmailTemplate] SET EmailContent  = REPLACE(EmailContent, 'Wellcare Support Team.', 'Simplify Healthcare Support Team.') where EmailContent like '%wellcare%'