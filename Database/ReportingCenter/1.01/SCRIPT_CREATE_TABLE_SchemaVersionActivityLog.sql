

/****** Object:  Table [RPT].[SchemaVersionActivityLog]    Script Date: 4/23/2018 2:02:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
IF NOT OBJECT_ID(N'RPT.SchemaVersionActivityLog', N'U') IS NOT NULL
BEGIN
 
	CREATE TABLE [RPT].[SchemaVersionActivityLog](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[DesignVersionID] [int] NULL,
		[DesignVersion] [varchar](20) NULL,
		[ObjectType] [varchar](5000) NULL,
		[Operation] [nvarchar](50) NULL,
		[Value] [nvarchar](max) NULL,
		[DesignType] [nvarchar](1000) NULL,
		[Label] [nvarchar](2000) NULL,
		[CustomType] [nvarchar](200) NULL,
		[valuePath][nvarchar](2000) NULL,
		[CreationDate] [datetime] NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	print 'SchemaVersionActivityLog Successfully Created'
END
ELSE
BEGIN 

	print 'SchemaVersionActivityLog Already exists'
END

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'DesignVersionNumber'
          AND Object_ID = Object_ID(N'rpt.ReportingTableInfo'))
BEGIN
    -- Co
ALTER TABLE rpt.ReportingTableInfo
		ADD  [DesignVersionNumber] [varchar](20) NULL
	print 'Successfully DesignVersionNumber column Created'
END
ELSE
BEGIN 

	print 'DesignVersionNumber Column Already exists'
END

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Visible'
          AND Object_ID = Object_ID(N'rpt.ReportingTableColumnInfo'))
BEGIN
   
ALTER TABLE rpt.ReportingTableColumnInfo
		ADD  [Visible] bit NULL
	print 'Successfully Visible column Created'
END
ELSE
BEGIN 

	print 'Visible Column Already exists'
END


IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'IsValidationPassed'
          AND Object_ID = Object_ID(N'rpt.ReportQueue'))
BEGIN
   
ALTER TABLE rpt.ReportQueue
		ADD  [IsValidationPassed] bit NULL
	print 'Successfully IsValidationPassed column Created'
END	
ELSE
BEGIN 
	print 'IsValidationPassed Column Already exists'
END
	
	
IF NOT OBJECT_ID(N'RPT.ValidationLog', N'U') IS NOT NULL
BEGIN
	CREATE TABLE rpt.ValidationLog
	(
		ID int identity(1,1),
		ReportingTableColumnInfoID INT,
		ErrorMessage  nvarchar(max), 
		creationDate date,
		DesignVersionId INT
	)
	print 'ValidationLog Successfully Created'
END
ELSE
BEGIN 

	print 'ValidationLog Already exists'
END



IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'FormInstanceId'
          AND Object_ID = Object_ID(N'rpt.ValidationLog'))
BEGIN
   
ALTER TABLE rpt.ValidationLog
		ADD  [FormInstanceId] INT NULL
	print 'Successfully FormInstanceId column Created'
END	
ELSE
BEGIN 
	print 'FormInstanceId Column Already exists'
END
	