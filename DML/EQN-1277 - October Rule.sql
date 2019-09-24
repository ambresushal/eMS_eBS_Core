-- ===============================================================
-- Author:		Bhushan Kahandal
-- Create date: 10/04/2016
-- Description:	To Add rule for October Business Rule.
-- Run: 
-- ================================================================
GO
BEGIN TRANSACTION
	BEGIN TRY
	
	DECLARE @RuleID INT
	DECLARE @ParentExpressionRuleID INT
	DECLARE @UIElementID INT=481
	

	-----------------------Rule 1------------

	PRINT('Add rows to [UI].[Rule]')
	
	-- 1.INSERT INTO RULE
	INSERT INTO [UI].[Rule] 
([RuleName]
,[RuleDescription]
,[RuleTargetTypeID]
,[ResultSuccess]
,[ResultFailure]
,[AddedBy]
,[AddedDate]
,[UpdatedBy]
,[UpdatedDate]
,[IsResultSuccessElement]
,[IsResultFailureElement]
,[Message])
 VALUES ('RULE', NULL, 1, NULL, NULL, 'tmgsuperuser', GETDATE(), 'tmgsuperuser', GETDATE(), 0, 0, NULL)

	SET @RuleID =SCOPE_IDENTITY()

	PRINT('Add row 1 to [UI].[Expression]')
	-- 2.INSERT INTO Expression
	INSERT INTO [UI].[Expression]
 ([LeftOperand],
  [RightOperand],
  [OperatorTypeID], 
  [LogicalOperatorTypeID], 
  [AddedBy], 
  [AddedDate], 
  [UpdatedBy], 
  [UpdatedDate], 
  [RuleID], 
  [ParentExpressionID], 
  [ExpressionTypeID], 
  [IsRightOperandElement]) 
  VALUES (NULL, NULL, 1, 1, 'tmgsuperuser', GETDATE(), 'tmgsuperuser', GETDATE(), @RuleID, NULL, 1, 0)

	SET @ParentExpressionRuleID = SCOPE_IDENTITY()

	PRINT('Add row 2 to [UI].[Expression]')
	INSERT INTO [UI].[Expression]
 ([LeftOperand],
  [RightOperand],
  [OperatorTypeID], 
  [LogicalOperatorTypeID], 
  [AddedBy], 
  [AddedDate], 
  [UpdatedBy], 
  [UpdatedDate], 
  [RuleID], 
  [ParentExpressionID], 
  [ExpressionTypeID], 
  [IsRightOperandElement]) 
 VALUES ('PRO3Repeater471_1_PRO3DropDown480', 'A-Include All', 5, 1, 'tmgsuperuser', GETDATE(), 'tmgsuperuser', GETDATE(), @RuleID, @ParentExpressionRuleID, 2, 0)

	

	PRINT('Add rows to [UI].[PropertyRuleMap]')
	-- 5.INSERT INTO PropertyRuleMap
	INSERT INTO [UI].[PropertyRuleMap] 
  ([RuleID], 
  [UIElementID], 
  [TargetPropertyID], 
  [AddedBy], 
  [AddedDate], 
  [UpdatedBy], 
  [UpdatedDate], 
  [IsCustomRule]) 
  VALUES (@RuleID, @UIElementID, 1, 'tmgsuperuser', GETDATE(), 'tmgsuperuser', GETDATE(), 0)

	
-----------------------Rule 2------------
	
	PRINT('Add rows to [UI].[Rule] for Rule 2')
	
	-- 1.INSERT INTO RULE
	INSERT INTO [UI].[Rule] 
([RuleName]
,[RuleDescription]
,[RuleTargetTypeID]
,[ResultSuccess]
,[ResultFailure]
,[AddedBy]
,[AddedDate]
,[UpdatedBy]
,[UpdatedDate]
,[IsResultSuccessElement]
,[IsResultFailureElement]
,[Message])
 VALUES ('RULE', NULL, 1, ' ', 'PRO3Repeater471_1_PRO3DropDown481', 'tmgsuperuser', GETDATE(), 'tmgsuperuser', GETDATE(), 0, 1, NULL)

	SET @RuleID =SCOPE_IDENTITY()

	PRINT('Add row 1 to [UI].[Expression] for Rule 2')
	-- 2.INSERT INTO Expression
	INSERT INTO [UI].[Expression]
 ([LeftOperand],
  [RightOperand],
  [OperatorTypeID], 
  [LogicalOperatorTypeID], 
  [AddedBy], 
  [AddedDate], 
  [UpdatedBy], 
  [UpdatedDate], 
  [RuleID], 
  [ParentExpressionID], 
  [ExpressionTypeID], 
  [IsRightOperandElement]) 
  VALUES (NULL, NULL, 1, 1, 'tmgsuperuser', GETDATE(), 'tmgsuperuser', GETDATE(), @RuleID, NULL, 1, 0)

	SET @ParentExpressionRuleID = SCOPE_IDENTITY()

	PRINT('Add row 2 to [UI].[Expression] for Rule 2')
	INSERT INTO [UI].[Expression]
 ([LeftOperand],
  [RightOperand],
  [OperatorTypeID], 
  [LogicalOperatorTypeID], 
  [AddedBy], 
  [AddedDate], 
  [UpdatedBy], 
  [UpdatedDate], 
  [RuleID], 
  [ParentExpressionID], 
  [ExpressionTypeID], 
  [IsRightOperandElement]) 
 VALUES ('PRO3Repeater471_1_PRO3DropDown480', 'A-Include All', 1, 1, 'tmgsuperuser', GETDATE(), 'tmgsuperuser', GETDATE(), @RuleID, @ParentExpressionRuleID, 2, 0)

	

	PRINT('Add rows to [UI].[PropertyRuleMap] for Rule 2')
	-- 5.INSERT INTO PropertyRuleMap
	INSERT INTO [UI].[PropertyRuleMap] 
  ([RuleID], 
  [UIElementID], 
  [TargetPropertyID], 
  [AddedBy], 
  [AddedDate], 
  [UpdatedBy], 
  [UpdatedDate], 
  [IsCustomRule]) 
  VALUES (@RuleID, @UIElementID, 3, 'tmgsuperuser', GETDATE(), 'tmgsuperuser', GETDATE(), 0)

	
	
	SELECT 'COMMIT TRANSACTION'
	COMMIT TRANSACTION
	
END TRY
BEGIN CATCH
	SELECT 'ROLLBACK TRANSACTION'
	ROLLBACK TRANSACTION
END CATCH
GO