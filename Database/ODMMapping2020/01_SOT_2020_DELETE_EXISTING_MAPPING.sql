----Get Form Design ID for 2020
DECLARE @FormDesignID AS INT
DECLARE @FormDesignVersionID AS INT
DECLARE @MappingID AS INT

SELECT @FormDesignID = FD.FormID
	,@FormDesignVersionID = FDV.FormDesignVersionID
FROM UI.FormDesign FD
INNER JOIN UI.FormDesignVersion FDV ON FDV.FormDesignID = FD.FormID
WHERE FD.FormName = 'Medicare'
	AND YEAR(FDV.EffectiveDate) = 2020 ----Delete Existing Records for PBP view - 2020

DELETE BD
FROM [ODM].[BenefitsDictionary] BD
INNER JOIN [ODM].[BenefitMapping] BM ON BM.MappingID = BD.MappingID
WHERE BM.ViewType = 'SOT'
	AND BM.IsActive = 1
	AND BM.FormDesignID = @FormDesignID
	AND BM.FormDesignVersionID = @FormDesignVersionID

DELETE
FROM [ODM].[BenefitMapping]
WHERE ViewType = 'SOT'
	AND IsActive = 1
	AND FormDesignID = @FormDesignID
	AND FormDesignVersionID = @FormDesignVersionID