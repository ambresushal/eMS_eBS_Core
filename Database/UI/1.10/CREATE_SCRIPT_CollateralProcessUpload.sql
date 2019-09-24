SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'CollateralProcessUpload' AND
			TABLE_SCHEMA = N'Setup')
BEGIN
	Drop table [Setup].[CollateralProcessUpload]
END


CREATE TABLE [Setup].[CollateralProcessUpload](
[ID] [int] IDENTITY(1,1) NOT NULL,
[ProductID] [varchar](300) NULL,
[ProductName] [varchar](300) NULL,
[AccountID] [int] NULL,
[AccountName] [varchar](200) NULL,
[FolderID] [int] NULL,
[FolderName] [varchar](200) NULL,
[FolderVersionID] [int] NULL,
[FolderVersionNumber] [varchar](50) NULL,
[FormInstanceID] [int] NULL,
[FormInstanceName] [varchar](200) NULL,
[FormDesignID] [int] NULL,
[FormDesignVersionID] [int] NULL,
[WordFile] [varbinary](max) NULL,
[PrintXFile] [varbinary](max) NULL,
[File508] [varbinary](max) NULL,
[HasError] [bit] NULL,
[CreatedBy] [varchar](50) NULL,
[CreatedDate] [datetime] NULL,
[ErrorDescription] [varchar](4000) NULL,
[CollateralName] varchar(500),
CONSTRAINT [PK_CollateralProcessUpload] PRIMARY KEY CLUSTERED 
(
[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

print '[Setup].[CollateralProcessUpload] Successfully Updated'

