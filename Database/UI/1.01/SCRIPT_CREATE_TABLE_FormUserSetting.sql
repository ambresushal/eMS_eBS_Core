
/*this script should be executed in UI */
IF NOT OBJECT_ID(N'RPT.FormDesignUserSetting', N'U') IS NOT NULL
BEGIN

CREATE TABLE RPT.FormDesignUserSetting
(
	[FormDesignUserSettingID] [int] IDENTITY(1,1) NOT NULL,
	[UserId]  [int] NOT NULL,
	[FormDesignVersionId] [int] NOT NULL,
	[LevelAt] [varchar](10) NOT NULL,
	[Key] [varchar](200) NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
	[AddedDate] [datetime] NOT NULL,
	[AddedBy] [varchar](50) NOT NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](50) NULL
CONSTRAINT [Pk_FormDesignUserSetting] PRIMARY KEY CLUSTERED 
(
	[FormDesignUserSettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
	print 'Successfully Created'
END
ELSE
BEGIN 

	print 'Already exists'
END

