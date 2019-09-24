----Get Form Design ID for 2020
DECLARE @FormDesignID AS INT
DECLARE @FormDesignVersionID AS INT

SELECT @FormDesignID = FD.FormID, @FormDesignVersionID = FDV.FormDesignVersionID
FROM UI.FormDesign FD
INNER JOIN UI.FormDesignVersion FDV ON FDV.FormDesignID = FD.FormID
WHERE FD.FormName = 'Medicare'
	AND YEAR(FDV.EffectiveDate) = 2020

DELETE
FROM [ML].[ElementDocumentRule]
WHERE RunOnMigration = 1
	AND FormDesignID = @FormDesignID
	AND FormDesignVersionID = @FormDesignVersionID