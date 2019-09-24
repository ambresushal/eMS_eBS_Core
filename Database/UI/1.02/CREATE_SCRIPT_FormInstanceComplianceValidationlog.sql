

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID(N'Fldr.FormInstanceComplianceValidationlog', N'U') IS NOT NULL
BEGIN
	Drop table [Fldr].[FormInstanceComplianceValidationlog]

	CREATE TABLE [Fldr].[FormInstanceComplianceValidationlog](
		[LOGID] [int] IDENTITY(1,1) NOT NULL,
		[FormInstanceID] [int] NULL,
		[CollateralProcessQueue1Up] [int] NULL,
		[ComplianceType] [varchar](50) NULL,
		[No] [int] NULL,
		[AddedBy] [nvarchar](20) NULL,
		[Error] [nvarchar](max) NULL,
		[ValidationType] [varchar](20) NULL,
		[AddedDate] [datetime] NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	print 'FormInstanceComplianceValidationlog Successfully Updated'
END
ELSE
BEGIN
	CREATE TABLE [Fldr].[FormInstanceComplianceValidationlog](
		[LOGID] [int] IDENTITY(1,1) NOT NULL,
		[FormInstanceID] [int] NULL,
		[CollateralProcessQueue1Up] [int] NULL,
		[ComplianceType] [varchar](50) NULL,
		[No] [int] NULL,
		[AddedBy] [nvarchar](20) NULL,
		[Error] [nvarchar](max) NULL,
		[ValidationType] [varchar](20) NULL,
		[AddedDate] [datetime] NULL
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

	print 'FormInstanceComplianceValidationlog Successfully Created'
END
