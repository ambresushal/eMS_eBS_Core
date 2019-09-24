USE [PBPImportTest] /******Give DataBase Name Here******/
GO

GO

/****** Object:  Schema [PBP]    Script Date: 5/3/2017 12:49:43 PM ******/
CREATE SCHEMA [PBP]
GO

/****** Object:  Table [PBP].[PBP]    Script Date: 5/18/2017 4:16:55 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF EXISTS ( SELECT * FROM sys.tables WHERE name LIKE 'PBP%' AND SCHEMA_NAME(schema_id) = 'PBP') DROP TABLE [PBP].[PBP]

CREATE TABLE [PBP].[PBP](
	[QID] [varchar](max) NULL,
	[REABSTRA] [varchar](max) NULL,
	[VERSION] [varchar](max) NULL,
	[VERSIONI] [varchar](max) NULL,
	[ISINUSE] [varchar](max) NULL,
	[REVTIME] [varchar](max) NULL,
	[ABSTDATE] [varchar](max) NULL,
	[CMPLTN_DT] [varchar](max) NULL,
	[BPT_MA_DT] [varchar](max) NULL,
	[BPT_PD_DT] [varchar](max) NULL,
	[BPT_MSA_DT] [varchar](max) NULL,
	[BPT_ESRDSNP_DT] [varchar](max) NULL,
	[UPLOAD_DT] [varchar](max) NULL,
	[ABSTRAID] [varchar](max) NULL,
	[NOTES] [varchar](max) NULL,
	[ACTIONRS] [varchar](max) NULL,
	[SCREENS] [varchar](max) NULL,
	[CATID1] [varchar](max) NULL,
	[SUBCAT1ID] [varchar](max) NULL,
	[STATUS_A] [varchar](max) NULL,
	[STATUS_B1] [varchar](max) NULL,
	[STATUS_B2] [varchar](max) NULL,
	[STATUS_B3] [varchar](max) NULL,
	[STATUS_B4] [varchar](max) NULL,
	[STATUS_B5] [varchar](max) NULL,
	[STATUS_B6] [varchar](max) NULL,
	[STATUS_B7] [varchar](max) NULL,
	[STATUS_B8] [varchar](max) NULL,
	[STATUS_B9] [varchar](max) NULL,
	[STATUS_B10] [varchar](max) NULL,
	[STATUS_B11] [varchar](max) NULL,
	[STATUS_B12] [varchar](max) NULL,
	[STATUS_B13] [varchar](max) NULL,
	[STATUS_B14] [varchar](max) NULL,
	[STATUS_B15] [varchar](max) NULL,
	[STATUS_B16] [varchar](max) NULL,
	[STATUS_B17] [varchar](max) NULL,
	[STATUS_B18] [varchar](max) NULL,
	[STATUS_B19] [varchar](max) NULL,
	[STATUS_B20] [varchar](max) NULL,
	[STATUS_C] [varchar](max) NULL,
	[STATUS_D] [varchar](max) NULL,
	[STATUS_RX] [varchar](max) NULL,
	[PBPVER] [varchar](max) NULL,
	[ACR_VALIDATE] [varchar](max) NULL,
	[DICTVER] [varchar](max) NULL,
	[UPLOADED] [varchar](max) NULL,
	[PBPCATS] [varchar](max) NULL,
	[PBP_A_SQUISH_ID] [varchar](max) NULL,
	[PBP_A_ORG_NAME] [varchar](max) NULL,
	[PBP_A_ORG_MARKETING_NAME] [varchar](max) NULL,
	[PBP_A_ORG_WEBSITE] [varchar](max) NULL,
	[PBP_A_ORG_TYPE] [varchar](max) NULL,
	[PBP_A_PLAN_TYPE] [varchar](max) NULL,
	[PBP_A_NETWORK_FLAG] [varchar](max) NULL,
	[PBP_A_BEN_COV] [varchar](max) NULL,
	[PBP_A_HOSPICE_CARE_YN] [varchar](max) NULL,
	[PBP_A_SERVICE_AREA] [varchar](max) NULL,
	[PBP_A_CONTRACT_NUMBER] [varchar](max) NULL,
	[PBP_A_PLAN_IDENTIFIER] [varchar](max) NULL,
	[PBP_A_SEGMENT_ID] [varchar](max) NULL,
	[PBP_A_CONTRACT_PERIOD] [varchar](max) NULL,
	[PBP_A_PLAN_NAME] [varchar](max) NULL,
	[PBP_A_PLAN_GEOG_NAME] [varchar](max) NULL,
	[PBP_A_SEGMENT_NAME] [varchar](max) NULL,
	[PBP_A_EGHP_YN] [varchar](max) NULL,
	[PBP_A_SPECIAL_NEED_FLAG] [varchar](max) NULL,
	[PBP_A_SPECIAL_NEED_PLAN_TYPE] [varchar](max) NULL,
	[PBP_A_SNP_INSTITUTIONAL_TYPE] [varchar](max) NULL,
	[PBP_A_EST_MEMB] [varchar](max) NULL,
	[PBP_A_CONTINUE_YN] [varchar](max) NULL,
	[PBP_A_CONTINUE_COSTSHARE_YN] [varchar](max) NULL,
	[PBP_A_CONTINUE_COSTSHARE_DESC] [varchar](max) NULL,
	[PBP_A_SNP_PCT] [varchar](max) NULL,
	[PBP_A_DSNP_ZERODOLLAR] [varchar](max) NULL,
	[PBP_A_SNP_COND] [varchar](max) NULL,
	[PBP_A_CURMBR_PHONE] [varchar](max) NULL,
	[PBP_A_TTYTDD_CURMBR_PHONE] [varchar](max) NULL,
	[PBP_A_CURMBR_LOC_PHONE] [varchar](max) NULL,
	[PBP_A_TTYTDD_CUR_LOC_PHONE] [varchar](max) NULL,
	[PBP_A_PROMBR_PHONE] [varchar](max) NULL,
	[PBP_A_TTYTDD_PROMBR_PHONE] [varchar](max) NULL,
	[PBP_A_PROMBR_LOC_PHONE] [varchar](max) NULL,
	[PBP_A_TTYTDD_PRO_LOC_PHONE] [varchar](max) NULL,
	[PBP_A_PD_CURMBR_PHONE] [varchar](max) NULL,
	[PBP_A_PD_TTYTDD_CURMBR_PHONE] [varchar](max) NULL,
	[PBP_A_PD_CURMBR_LOC_PHONE] [varchar](max) NULL,
	[PBP_A_PD_TTYTDD_CUR_LOC_PHONE] [varchar](max) NULL,
	[PBP_A_PD_PROMBR_PHONE] [varchar](max) NULL,
	[PBP_A_PD_TTYTDD_PROMBR_PHONE] [varchar](max) NULL,
	[PBP_A_PD_PROMBR_LOC_PHONE] [varchar](max) NULL,
	[PBP_A_PD_TTYTDD_PRO_LOC_PHONE] [varchar](max) NULL,
	[PBP_A_NOTES] [varchar](max) NULL,
	[PBP_A_OPD_WEB_ADDR] [varchar](max) NULL,
	[PBP_A_FORMULARY_WEB_ADDR] [varchar](max) NULL,
	[PBP_A_TTYTDD_PROMBR_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_PROMBR_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_PD_PROMBR_LOC_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_PD_TTYTDD_CUR_LOC_PHN_EX] [varchar](max) NULL,
	[PBP_A_PROMBR_LOC_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_CURMBR_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_TTYTDD_CURMBR_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_PD_TTYTDD_CURMBR_PHN_EXT] [varchar](max) NULL,
	[PBP_A_PD_CURMBR_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_PD_TTYTDD_PRO_LOC_PHN_EX] [varchar](max) NULL,
	[PBP_A_CURMBR_LOC_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_TTYTDD_CUR_LOC_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_TTYTDD_PRO_LOC_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_PD_CURMBR_LOC_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_PD_TTYTDD_PROMBR_PHN_EXT] [varchar](max) NULL,
	[PBP_A_PD_PROMBR_PHONE_EXT] [varchar](max) NULL,
	[PBP_A_CONTRACT_PARTD_FLAG] [varchar](max) NULL,
	[PBP_A_PLATINO_FLAG] [varchar](max) NULL,
	[PBP_A_PLATINO_YN] [varchar](max) NULL,
	[PBP_A_PHARMACY_WEBSITE] [varchar](max) NULL,
	[PBP_A_FFS_BID_B_YN] [varchar](max) NULL,
	[PBP_A_FFS_BID_C_YN] [varchar](max) NULL,
	[PBP_A_FFS_BID_D_YN] [varchar](max) NULL,
	[PBP_A_FFS_BID_B_AUTH_YN] [varchar](max) NULL,
	[PBP_A_FFS_BID_B_REF_YN] [varchar](max) NULL,
	[PBP_A_FFS_BID_C_AUTH_YN] [varchar](max) NULL,
	[PBP_A_FFS_BID_C_REF_YN] [varchar](max) NULL,
	[PBP_A_FFS_BID_B_AUTH_CATS] [varchar](max) NULL,
	[PBP_A_FFS_BID_B_REF_CATS] [varchar](max) NULL,
	[PBP_A_FFS_BID_C_AUTH_CATS] [varchar](max) NULL,
	[PBP_A_FFS_BID_C_REF_CATS] [varchar](max) NULL,
	[PBP_A_SNP_STATE_CVG_YN] [varchar](max) NULL,
	[PBP_A_TIER_YN] [varchar](max) NULL,
	[PBP_A_TIER_BENDESC_BENS] [varchar](max) NULL,
	[PBP_A_TIER_MC_BENDESC_CATS] [varchar](max) NULL,
	[PBP_A_TIER_NMC_BENDESC_CATS] [varchar](max) NULL,
	[PBP_A_VBID_INDICATOR] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[PBPB1]    Script Date: 5/18/2017 4:23:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF EXISTS ( SELECT * FROM sys.tables WHERE name LIKE 'PBPB1%' AND SCHEMA_NAME(schema_id) = 'PBP') DROP TABLE [PBP].[PBPB1]

CREATE TABLE [PBP].[PBPB1](
	[QID] [varchar](max) NULL,
	[PBP_B1A_BENDESC_YN] [varchar](max) NULL,
	[PBP_B1A_BENDESC_AD_UP_NMCS] [varchar](max) NULL,
	[PBP_B1A_BENDESC_AMO_AD] [varchar](max) NULL,
	[PBP_B1A_BENDESC_LIM_AD] [varchar](max) NULL,
	[PBP_B1A_BENDESC_AMT_AD] [varchar](max) NULL,
	[PBP_B1A_BENDESC_AMO_NMCS] [varchar](max) NULL,
	[PBP_B1A_BENDESC_AMO_UP] [varchar](max) NULL,
	[PBP_B1A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B1A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B1A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B1A_COST_VARY_TIERS_YN] [varchar](max) NULL,
	[PBP_B1A_COST_VARY_TIER_NUM] [varchar](max) NULL,
	[PBP_B1A_COST_VARY_LOW_TIER] [varchar](max) NULL,
	[PBP_B1A_COINS_YN] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_MC_COINS_CSTSHR_YN_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_STRUC_YN] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_PCT] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_INTRVL_NUM] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_PCT_INTRVL1] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_BGND_NTRVL1] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_ENDD_NTRVL1] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_PCT_INTRVL2] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_BGND_NTRVL2] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_ENDD_NTRVL2] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_PCT_INTRVL3] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_BGND_NTRVL3] [varchar](max) NULL,
	[PBP_B1A_COINS_NMCS_ENDD_NTRVL3] [varchar](max) NULL,
	[PBP_B1A_COINS_PCT_UP] [varchar](max) NULL,
	[PBP_B1A_DED_YN] [varchar](max) NULL,
	[PBP_B1A_DED_AMT_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_YN] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_MC_COPAY_CSTSHR_YN_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_STRUC_YN] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_AMT] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_INTRVL_NUM] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_AMT_INTRVL1] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_BGND_NTRVL1] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_ENDD_NTRVL1] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_AMT_INTRVL2] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_BGND_NTRVL2] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_ENDD_NTRVL2] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_AMT_INTRVL3] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_BGND_NTRVL3] [varchar](max) NULL,
	[PBP_B1A_COPAY_NMCS_ENDD_NTRVL3] [varchar](max) NULL,
	[PBP_B1A_COPAY_UP_AMT_STAY] [varchar](max) NULL,
	[PBP_B1A_COPAY_UP_AMT_DAY] [varchar](max) NULL,
	[PBP_B1A_AUTH] [varchar](max) NULL,
	[PBP_B1A_REFER_YN] [varchar](max) NULL,
	[PBP_B1A_NOTES] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B1A_DED_AMT_T2] [varchar](max) NULL,
	[PBP_B1A_MC_COINS_CSTSHR_YN_T2] [varchar](max) NULL,
	[PBP_B1A_MC_COPAY_CSTSHR_YN_T2] [varchar](max) NULL,
	[PBP_B1A_DED_AMT_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_LRD_PCT_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_MCS_PCT_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_AMT_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_LRD_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_AMT_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_MCS_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B1A_MC_COINS_CSTSHR_YN_T3] [varchar](max) NULL,
	[PBP_B1A_MC_COPAY_CSTSHR_YN_T3] [varchar](max) NULL,
	[PBP_B1A_HOSP_BEN_PERIOD] [varchar](max) NULL,
	[PBP_B1A_HOSP_BEN_PERIOD_OTH] [varchar](max) NULL,
	[PBP_B1A_COST_DISCHARGE_YN] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_INTRVL_NUM_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_AD_COST_VARY_TIERS_YN] [varchar](max) NULL,
	[PBP_B1A_AD_COST_VARY_TIER_NUM] [varchar](max) NULL,
	[PBP_B1A_AD_COST_VARY_LOW_TIER] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_INTRVL_NUM_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_INTRVL_NUM_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_PCT_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COINS_AD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_INTRVL_NUM_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_INTRVL_NUM_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_INTRVL_NUM_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_AMT_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1A_COPAY_AD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB1_2]    Script Date: 5/18/2017 4:25:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB1_2](
	[QID] [varchar](max) NULL,
	[PBP_B1B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B1B_BENDESC_AD_NMCS] [varchar](max) NULL,
	[PBP_B1B_BENDESC_AMO_AD] [varchar](max) NULL,
	[PBP_B1B_BENDESC_LIM_AD] [varchar](max) NULL,
	[PBP_B1B_BENDESC_AMT_AD] [varchar](max) NULL,
	[PBP_B1B_BENDESC_AMO_NMCS] [varchar](max) NULL,
	[PBP_B1B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B1B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B1B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B1B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B1B_COST_VARY_TIERS_YN] [varchar](max) NULL,
	[PBP_B1B_COST_VARY_TIER_NUM] [varchar](max) NULL,
	[PBP_B1B_COST_VARY_LOW_TIER] [varchar](max) NULL,
	[PBP_B1B_COINS_YN] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_STRUC_YN] [varchar](max) NULL,
	[PBP_B1B_COINS_PCT_NMCS] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_INTRVL_NUM] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_PCT_INTRVL1] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_BGND_NTRVL1] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_ENDD_NTRVL1] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_PCT_INTRVL2] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_BGND_NTRVL2] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_ENDD_NTRVL2] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_PCT_INTRVL3] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_BGND_NTRVL3] [varchar](max) NULL,
	[PBP_B1B_COINS_NMCS_ENDD_NTRVL3] [varchar](max) NULL,
	[PBP_B1B_DED_YN] [varchar](max) NULL,
	[PBP_B1B_COPAY_YN] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_STRUC_YN] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_AMT] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_INTRVL_NUM] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_AMT_INTRVL1] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_BGND_NTRVL1] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_ENDD_NTRVL1] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_AMT_INTRVL2] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_BGND_NTRVL2] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_ENDD_NTRVL2] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_AMT_INTRVL3] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_BGND_NTRVL3] [varchar](max) NULL,
	[PBP_B1B_COPAY_NMCS_ENDD_NTRVL3] [varchar](max) NULL,
	[PBP_B1B_AUTH] [varchar](max) NULL,
	[PBP_B1B_REFER_YN] [varchar](max) NULL,
	[PBP_B1B_NOTES] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_LRD_PCT_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_MCS_PCT_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_AMT_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_LRD_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_AMT_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_MCS_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B1B_DED_AMT_T1] [varchar](max) NULL,
	[PBP_B1B_DED_AMT_T2] [varchar](max) NULL,
	[PBP_B1B_DED_AMT_T3] [varchar](max) NULL,
	[PBP_B1B_MC_COINS_CSTSHR_YN_T1] [varchar](max) NULL,
	[PBP_B1B_MC_COINS_CSTSHR_YN_T2] [varchar](max) NULL,
	[PBP_B1B_MC_COINS_CSTSHR_YN_T3] [varchar](max) NULL,
	[PBP_B1B_MC_COPAY_CSTSHR_YN_T1] [varchar](max) NULL,
	[PBP_B1B_MC_COPAY_CSTSHR_YN_T2] [varchar](max) NULL,
	[PBP_B1B_MC_COPAY_CSTSHR_YN_T3] [varchar](max) NULL,
	[PBP_B1B_HOSP_BEN_PERIOD] [varchar](max) NULL,
	[PBP_B1B_HOSP_BEN_PERIOD_OTH] [varchar](max) NULL,
	[PBP_B1B_COST_DISCHARGE_YN] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_INTRVL_NUM_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_AD_COST_VARY_TIERS_YN] [varchar](max) NULL,
	[PBP_B1B_AD_COST_VARY_TIER_NUM] [varchar](max) NULL,
	[PBP_B1B_AD_COST_VARY_LOW_TIER] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_INTRVL_NUM_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_INTRVL_NUM_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_PCT_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COINS_AD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_INTRVL_NUM_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_INTRVL_NUM_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_INTRVL_NUM_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_AMT_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B1B_COPAY_AD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB1_B]    Script Date: 5/18/2017 4:26:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB1_B](
	[QID] [varchar](max) NULL,
	[PBP_B1A_BENDESC_YN_BONLY] [varchar](max) NULL,
	[PBP_B1A_BENDESC_AMO_IACT_BONLY] [varchar](max) NULL,
	[PBP_B1A_BENDESC_ULIM_BONLY] [varchar](max) NULL,
	[PBP_B1A_BENDESC_AMT_DAYS_BONLY] [varchar](max) NULL,
	[PBP_B1A_BENDESC_PER_BONLY] [varchar](max) NULL,
	[PBP_B1A_MAXPLAN_YN_BONLY] [varchar](max) NULL,
	[PBP_B1A_MAXPLAN_AMT_BONLY] [varchar](max) NULL,
	[PBP_B1A_MAXPLAN_PER_BONLY] [varchar](max) NULL,
	[PBP_B1A_MAXENR_YN_BONLY] [varchar](max) NULL,
	[PBP_B1A_MAXENR_AMT_BONLY] [varchar](max) NULL,
	[PBP_B1A_MAXENR_PER_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_YN_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PCT_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_NUM_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_PCT1_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_BGND1_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_ENDD1_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_PCT2_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_BGND2_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_ENDD2_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_PCT3_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_BGND3_BONLY] [varchar](max) NULL,
	[PBP_B1A_COINS_PS_ENDD3_BONLY] [varchar](max) NULL,
	[PBP_B1A_DED_YN_BONLY] [varchar](max) NULL,
	[PBP_B1A_DED_AMT_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_YN_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_AMT_PS_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_NUM_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_AMT1_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_BGND1_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_ENDD1_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_AMT2_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_BGND2_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_ENDD2_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_AMT3_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_BGND3_BONLY] [varchar](max) NULL,
	[PBP_B1A_COPAY_PS_ENDD3_BONLY] [varchar](max) NULL,
	[PBP_B1A_AUTH_BONLY] [varchar](max) NULL,
	[PBP_B1A_REFER_YN_BONLY] [varchar](max) NULL,
	[PBP_B1A_NOTES_BONLY] [varchar](max) NULL,
	[PBP_B1B_BENDESC_YN_BONLY] [varchar](max) NULL,
	[PBP_B1B_BENDESC_AMO_BONLY] [varchar](max) NULL,
	[PBP_B1B_BENDESC_LIM_BONLY] [varchar](max) NULL,
	[PBP_B1B_BENDESC_AMT_BONLY] [varchar](max) NULL,
	[PBP_B1B_BENDESC_PER_BONLY] [varchar](max) NULL,
	[PBP_B1B_MAXPLAN_YN_BONLY] [varchar](max) NULL,
	[PBP_B1B_MAXPLAN_TYPE_BONLY] [varchar](max) NULL,
	[PBP_B1B_MAXPLAN_AMT_BONLY] [varchar](max) NULL,
	[PBP_B1B_MAXPLAN_PER_BONLY] [varchar](max) NULL,
	[PBP_B1B_MAXENR_YN_BONLY] [varchar](max) NULL,
	[PBP_B1B_MAXENR_TYPE_BONLY] [varchar](max) NULL,
	[PBP_B1B_MAXENR_AMT_BONLY] [varchar](max) NULL,
	[PBP_B1B_MAXENR_PER_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_YN_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PCT_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_NUM_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_PCT1_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_BGND1_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_ENDD1_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_PCT2_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_BGND2_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_ENDD2_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_PCT3_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_BGND3_BONLY] [varchar](max) NULL,
	[PBP_B1B_COINS_PS_ENDD3_BONLY] [varchar](max) NULL,
	[PBP_B1B_DED_YN_BONLY] [varchar](max) NULL,
	[PBP_B1B_DED_AMT_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_YN_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_AMT_PS_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_NUM_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_AMT1_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_BGND1_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_ENDD1_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_AMT2_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_BGND2_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_ENDD2_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_AMT3_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_BGND3_BONLY] [varchar](max) NULL,
	[PBP_B1B_COPAY_PS_ENDD3_BONLY] [varchar](max) NULL,
	[PBP_B1B_AUTH_BONLY] [varchar](max) NULL,
	[PBP_B1B_REFER_YN_BONLY] [varchar](max) NULL,
	[PBP_B1B_NOTES_BONLY] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB10]    Script Date: 5/18/2017 4:26:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB10](
	[QID] [varchar](max) NULL,
	[PBP_B10A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B10A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B10A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B10A_COINS_YN] [varchar](max) NULL,
	[PBP_B10A_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B10A_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B10A_COINS_WAV_YN] [varchar](max) NULL,
	[PBP_B10A_DED_YN] [varchar](max) NULL,
	[PBP_B10A_DED_AMT] [varchar](max) NULL,
	[PBP_B10A_COPAY_YN] [varchar](max) NULL,
	[PBP_B10A_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B10A_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B10A_COPAY_WAV_YN] [varchar](max) NULL,
	[PBP_B10A_AUTH] [varchar](max) NULL,
	[PBP_B10A_NOTES] [varchar](max) NULL,
	[PBP_B10B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B10B_BENDESC_TRN] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMO_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_LIM_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMT_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_PER_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_TT_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMT_PAL_DAYS] [varchar](max) NULL,
	[PBP_B10B_BENDESC_MT_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMO_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_LIM_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMT_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_PER_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_TT_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMT_AL_DAYS] [varchar](max) NULL,
	[PBP_B10B_BENDESC_MT_AL] [varchar](max) NULL,
	[PBP_B10B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B10B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B10B_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B10B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B10B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B10B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B10B_COINS_YN] [varchar](max) NULL,
	[PBP_B10B_DED_YN] [varchar](max) NULL,
	[PBP_B10B_DED_AMT] [varchar](max) NULL,
	[PBP_B10B_COPAY_YN] [varchar](max) NULL,
	[PBP_B10B_AUTH] [varchar](max) NULL,
	[PBP_B10B_REFER_YN] [varchar](max) NULL,
	[PBP_B10B_NOTES] [varchar](max) NULL,
	[PBP_B10B_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B10B_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B10B_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B10B_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB11]    Script Date: 5/18/2017 4:26:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB11](
	[QID] [varchar](max) NULL,
	[PBP_B11A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B11A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B11A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B11A_COINS_YN] [varchar](max) NULL,
	[PBP_B11A_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B11A_COINS_PCT_MCMAX] [varchar](max) NULL,
	[PBP_B11A_DED_YN] [varchar](max) NULL,
	[PBP_B11A_DED_AMT] [varchar](max) NULL,
	[PBP_B11A_COPAY_YN] [varchar](max) NULL,
	[PBP_B11A_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B11A_COPAY_MCMAX_AMT] [varchar](max) NULL,
	[PBP_B11A_AUTH] [varchar](max) NULL,
	[PBP_B11A_NOTES] [varchar](max) NULL,
	[PBP_B11B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B11B_MAXENR_COST_TYPE] [varchar](max) NULL,
	[PBP_B11B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B11B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B11B_COINS_YN] [varchar](max) NULL,
	[PBP_B11B_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B11B_COINS_PCT_MCMAX] [varchar](max) NULL,
	[PBP_B11B_COINS_PCT_MCMS_MIN] [varchar](max) NULL,
	[PBP_B11B_COINS_PCT_MCMS_MAX] [varchar](max) NULL,
	[PBP_B11B_DED_YN] [varchar](max) NULL,
	[PBP_B11B_DED_AMT] [varchar](max) NULL,
	[PBP_B11B_COPAY_YN] [varchar](max) NULL,
	[PBP_B11B_COPAY_MCMIN_AMT] [varchar](max) NULL,
	[PBP_B11B_COPAY_MCMAX_AMT] [varchar](max) NULL,
	[PBP_B11B_COPAY_MCMS_MIN_AMT] [varchar](max) NULL,
	[PBP_B11B_COPAY_MCMS_MAX_AMT] [varchar](max) NULL,
	[PBP_B11B_AUTH] [varchar](max) NULL,
	[PBP_B11B_NOTES] [varchar](max) NULL,
	[PBP_B11C_MAXENR_YN] [varchar](max) NULL,
	[PBP_B11C_MAXENR_COST_TYPE] [varchar](max) NULL,
	[PBP_B11C_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B11C_MAXENR_PER] [varchar](max) NULL,
	[PBP_B11C_COINS_YN] [varchar](max) NULL,
	[PBP_B11C_COINS_PCT_MCMIN] [varchar](max) NULL,
	[PBP_B11C_COINS_PCT_MCMAX] [varchar](max) NULL,
	[PBP_B11C_DED_YN] [varchar](max) NULL,
	[PBP_B11C_DED_AMT] [varchar](max) NULL,
	[PBP_B11C_COPAY_YN] [varchar](max) NULL,
	[PBP_B11C_COPAY_MCMIN_AMT] [varchar](max) NULL,
	[PBP_B11C_COPAY_MCMAX_AMT] [varchar](max) NULL,
	[PBP_B11C_AUTH] [varchar](max) NULL,
	[PBP_B11C_NOTES] [varchar](max) NULL,
	[PBP_B11C_COINS_DTSI_MCMIN_PCT] [varchar](max) NULL,
	[PBP_B11C_COINS_DTSI_MCMAX_PCT] [varchar](max) NULL,
	[PBP_B11C_COPAY_DTSI_MCMIN_AMT] [varchar](max) NULL,
	[PBP_B11C_COPAY_DTSI_MCMAX_AMT] [varchar](max) NULL,
	[PBP_B11B_COINS_EHC] [varchar](max) NULL,
	[PBP_B11B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B11C_COINS_EHC] [varchar](max) NULL,
	[PBP_B11C_COPAY_EHC] [varchar](max) NULL,
	[PBP_B11A_DME_PREF_VEND_MAN_YN] [varchar](max) NULL,
	[PBP_B11C_DME_LIMIT_MANUFACT_YN] [varchar](max) NULL,
	[PBP_B11A_MM_ADD_SERVICE_OTHER1] [varchar](max) NULL,
	[PBP_B11A_MM_ADD_SERVICE_OTHER2] [varchar](max) NULL,
	[PBP_B11A_MM_AUTH] [varchar](max) NULL,
	[PBP_B11A_MM_COINS_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B11A_MM_COINS_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B11A_MM_COINS_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B11A_MM_COINS_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B11A_MM_COINS_DME_MAX] [varchar](max) NULL,
	[PBP_B11A_MM_COINS_DME_MIN] [varchar](max) NULL,
	[PBP_B11A_MM_COINS_SERVICES] [varchar](max) NULL,
	[PBP_B11A_MM_COINS_YN] [varchar](max) NULL,
	[PBP_B11A_MM_COPAY_DME_MAX] [varchar](max) NULL,
	[PBP_B11A_MM_COPAY_DME_MIN] [varchar](max) NULL,
	[PBP_B11A_MM_COPAY_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B11A_MM_COPAY_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B11A_MM_COPAY_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B11A_MM_COPAY_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B11A_MM_COPAY_SERVICES] [varchar](max) NULL,
	[PBP_B11A_MM_COPAY_YN] [varchar](max) NULL,
	[PBP_B11A_MM_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B11A_MM_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B11A_MM_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B11A_MM_NMC_YN] [varchar](max) NULL,
	[PBP_B11A_MM_REFER_YN] [varchar](max) NULL,
	[PBP_B11A_MM_SERVICES] [varchar](max) NULL,
	[PBP_B11B_MM_ADD_SERVICE_OTHER1] [varchar](max) NULL,
	[PBP_B11B_MM_AUTH] [varchar](max) NULL,
	[PBP_B11B_MM_COINS_YN] [varchar](max) NULL,
	[PBP_B11B_MM_COPAY_YN] [varchar](max) NULL,
	[PBP_B11B_MM_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B11B_MM_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B11B_MM_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B11B_MM_NMC_YN] [varchar](max) NULL,
	[PBP_B11B_MM_REFER_YN] [varchar](max) NULL,
	[PBP_B11B_MM_COPAY_AMT] [varchar](max) NULL,
	[PBP_B11B_MM_COINS_PCT] [varchar](max) NULL,
	[PBP_B11A_MM_NOTES] [varchar](max) NULL,
	[PBP_B11B_MM_NOTES] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB12]    Script Date: 5/18/2017 4:27:17 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB12](
	[QID] [varchar](max) NULL,
	[PBP_B12_MAXENR_YN] [varchar](max) NULL,
	[PBP_B12_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B12_MAXENR_PER] [varchar](max) NULL,
	[PBP_B12_COINS_YN] [varchar](max) NULL,
	[PBP_B12_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B12_DED_YN] [varchar](max) NULL,
	[PBP_B12_DED_AMT] [varchar](max) NULL,
	[PBP_B12_COPAY_YN] [varchar](max) NULL,
	[PBP_B12_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B12_AUTH] [varchar](max) NULL,
	[PBP_B12_REFER_YN] [varchar](max) NULL,
	[PBP_B12_NOTES] [varchar](max) NULL,
	[PBP_B12_COINS_MAX_PCT_MC] [varchar](max) NULL,
	[PBP_B12_COPAY_MC_MAX_AMT] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB13]    Script Date: 5/18/2017 4:27:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB13](
	[QID] [varchar](max) NULL,
	[PBP_B13A_BENDESC_YN] [varchar](max) NULL,
	[PBP_B13A_BENDESC_ENHAN] [varchar](max) NULL,
	[PBP_B13A_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B13A_BENDESC_LIM] [varchar](max) NULL,
	[PBP_B13A_BENDESC_NUMV] [varchar](max) NULL,
	[PBP_B13A_BENDESC_PER] [varchar](max) NULL,
	[PBP_B13A_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B13A_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B13A_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B13A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B13A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B13A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B13A_COMBINED_BEN] [varchar](max) NULL,
	[PBP_B13A_COINS_YN] [varchar](max) NULL,
	[PBP_B13A_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B13A_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B13A_DED_YN] [varchar](max) NULL,
	[PBP_B13A_DED_AMT] [varchar](max) NULL,
	[PBP_B13A_COPAY_YN] [varchar](max) NULL,
	[PBP_B13A_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B13A_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B13A_AUTH] [varchar](max) NULL,
	[PBP_B13A_REFER_YN] [varchar](max) NULL,
	[PBP_B13A_NOTES] [varchar](max) NULL,
	[PBP_B13B_BENDESC_OTC] [varchar](max) NULL,
	[PBP_B13B_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B13B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B13B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B13B_OTC_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B13B_MAXPLAN_PRD_YN] [varchar](max) NULL,
	[PBP_B13B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B13B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B13B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B13B_COINS_YN] [varchar](max) NULL,
	[PBP_B13B_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B13B_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B13B_DED_YN] [varchar](max) NULL,
	[PBP_B13B_DED_AMT] [varchar](max) NULL,
	[PBP_B13B_COPAY_YN] [varchar](max) NULL,
	[PBP_B13B_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B13B_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B13B_CMS_OTC_LIST_YN] [varchar](max) NULL,
	[PBP_B13B_NOTES] [varchar](max) NULL,
	[PBP_B13C_BENDESC_SERVICE] [varchar](max) NULL,
	[PBP_B13C_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B13C_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B13C_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B13C_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B13C_MAXENR_YN] [varchar](max) NULL,
	[PBP_B13C_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B13C_MAXENR_PER] [varchar](max) NULL,
	[PBP_B13C_WEEKS] [varchar](max) NULL,
	[PBP_B13C_MAX_MEALS] [varchar](max) NULL,
	[PBP_B13C_COINS_YN] [varchar](max) NULL,
	[PBP_B13C_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B13C_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B13C_DED_YN] [varchar](max) NULL,
	[PBP_B13C_DED_AMT] [varchar](max) NULL,
	[PBP_B13C_COPAY_YN] [varchar](max) NULL,
	[PBP_B13C_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B13C_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B13C_AUTH] [varchar](max) NULL,
	[PBP_B13C_REFER_YN] [varchar](max) NULL,
	[PBP_B13C_NOTES] [varchar](max) NULL,
	[PBP_B13D_BENDESC_SERVICE] [varchar](max) NULL,
	[PBP_B13D_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B13D_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B13D_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B13D_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B13D_MAXENR_YN] [varchar](max) NULL,
	[PBP_B13D_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B13D_MAXENR_PER] [varchar](max) NULL,
	[PBP_B13D_COINS_YN] [varchar](max) NULL,
	[PBP_B13D_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B13D_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B13D_DED_YN] [varchar](max) NULL,
	[PBP_B13D_DED_AMT] [varchar](max) NULL,
	[PBP_B13D_COPAY_YN] [varchar](max) NULL,
	[PBP_B13D_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B13D_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B13D_AUTH] [varchar](max) NULL,
	[PBP_B13D_REFER_YN] [varchar](max) NULL,
	[PBP_B13D_NOTES] [varchar](max) NULL,
	[PBP_B13E_BENDESC_SERVICE] [varchar](max) NULL,
	[PBP_B13E_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B13E_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B13E_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B13E_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B13E_MAXENR_YN] [varchar](max) NULL,
	[PBP_B13E_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B13E_MAXENR_PER] [varchar](max) NULL,
	[PBP_B13E_COINS_YN] [varchar](max) NULL,
	[PBP_B13E_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B13E_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B13E_DED_YN] [varchar](max) NULL,
	[PBP_B13E_DED_AMT] [varchar](max) NULL,
	[PBP_B13E_COPAY_YN] [varchar](max) NULL,
	[PBP_B13E_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B13E_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B13E_AUTH] [varchar](max) NULL,
	[PBP_B13E_REFER_YN] [varchar](max) NULL,
	[PBP_B13E_NOTES] [varchar](max) NULL,
	[PBP_B13F_BENDESC_SERVICE] [varchar](max) NULL,
	[PBP_B13F_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B13F_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B13F_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B13F_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B13F_MAXENR_YN] [varchar](max) NULL,
	[PBP_B13F_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B13F_MAXENR_PER] [varchar](max) NULL,
	[PBP_B13F_COINS_YN] [varchar](max) NULL,
	[PBP_B13F_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B13F_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B13F_DED_YN] [varchar](max) NULL,
	[PBP_B13F_DED_AMT] [varchar](max) NULL,
	[PBP_B13F_COPAY_YN] [varchar](max) NULL,
	[PBP_B13F_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B13F_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B13F_AUTH] [varchar](max) NULL,
	[PBP_B13F_REFER_YN] [varchar](max) NULL,
	[PBP_B13F_NOTES] [varchar](max) NULL,
	[PBP_B13G_ATTESTATION] [varchar](max) NULL,
	[PBP_B13G_BENDESC_SERVICE] [varchar](max) NULL,
	[PBP_B13G_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B13G_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B13G_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B13G_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B13G_MAXENR_YN] [varchar](max) NULL,
	[PBP_B13G_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B13G_MAXENR_PER] [varchar](max) NULL,
	[PBP_B13G_COINS_YN] [varchar](max) NULL,
	[PBP_B13G_COINS_MIN_PCT] [varchar](max) NULL,
	[PBP_B13G_COINS_MAX_PCT] [varchar](max) NULL,
	[PBP_B13G_DED_YN] [varchar](max) NULL,
	[PBP_B13G_DED_AMT] [varchar](max) NULL,
	[PBP_B13G_COPAY_YN] [varchar](max) NULL,
	[PBP_B13G_COPAY_MIN_AMT] [varchar](max) NULL,
	[PBP_B13G_COPAY_MAX_AMT] [varchar](max) NULL,
	[PBP_B13G_AUTH] [varchar](max) NULL,
	[PBP_B13G_REFER_YN] [varchar](max) NULL,
	[PBP_B13G_NOTES] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB13_2]    Script Date: 5/18/2017 4:28:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB13_2](
	[QID] [varchar](max) NULL,
	[PBP_B13H_ADD_SRVCS_YN] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICES] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER1] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER2] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER3] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER4] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER5] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER6] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER7] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER8] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER9] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER10] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER11] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER12] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER13] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER14] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER15] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER16] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER17] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER18] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER19] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER20] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER21] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER22] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER23] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER24] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER25] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER26] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER27] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER28] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER29] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER30] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER31] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER32] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER33] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER34] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER35] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER36] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER37] [varchar](max) NULL,
	[PBP_B13H_ADDL_SERVICE_OTHER38] [varchar](max) NULL,
	[PBP_B13H_LIMIT_YN] [varchar](max) NULL,
	[PBP_B13H_LIMIT_SERVICES] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_EPSDT] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_EPSDT] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_EPSDT] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_TCCPW] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_TCCPW] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_TCCPW] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_FBCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_FBCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_FBCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_RCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_RCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_RCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_FPS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_FPS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_FPS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_NHS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_NHS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_NHS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_HCBS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_HCBS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_HCBS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_PCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_PCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_PCS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_SDPAS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_SDPAS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_SDPAS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_PDNS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_PDNS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_PDNS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_CM_LT] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_CM_LT] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_CM_LT] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_IMDS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_IMDS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_IMDS] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_ICFMR] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_ICFMR] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_ICFMR] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_CM] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_CM] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_CM] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH1] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH1] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH1] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH2] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH2] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH2] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH3] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH3] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH3] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH4] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH4] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH4] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH5] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH5] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH5] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH6] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH6] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH6] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH7] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH7] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH7] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH8] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH8] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH8] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH9] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH9] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH9] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH10] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH10] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH10] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH11] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH11] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH11] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH12] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH12] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH12] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH13] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH13] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH13] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH14] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH14] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH14] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH15] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH15] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH15] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH16] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH16] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH16] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH17] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH17] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH17] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH18] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH18] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH18] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH19] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH19] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH19] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH20] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH20] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH20] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH21] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH21] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH21] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH22] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH22] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH22] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH23] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH23] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH23] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH24] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH24] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH24] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH25] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH25] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH25] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH26] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH26] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH26] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH27] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH27] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH27] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH28] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH28] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH28] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH29] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH29] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH29] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH30] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH30] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH30] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH31] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH31] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH31] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH32] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH32] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH32] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH33] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH33] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH33] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH34] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH34] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH34] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH35] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH35] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH35] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH36] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH36] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH36] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH37] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH37] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH37] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_TYPE_OTH38] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_NUM_OTH38] [varchar](max) NULL,
	[PBP_B13H_LIMIT_UNIT_PER_OTH38] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_SERVICES] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_EPSDT_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_EPSDT_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_TCCPW_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_TCCPW_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_FBCS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_FBCS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_RCS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_RCS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_FPS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_FPS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_NHS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_NHS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_HCBS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_HCBS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_PCS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_PCS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_SDPAS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_SDPAS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_PDNS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_PDNS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_CM_LTC_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_CM_LTC_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_IMDS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_IMDS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_ICFMRS_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_ICFMRS_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_CM_AMT] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB13_3]    Script Date: 5/18/2017 4:28:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB13_3](
	[QID] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_CM_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER1_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER1_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER2_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER2_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER3_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER3_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER4_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER4_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER5_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER5_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER6_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER6_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER7_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER7_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER8_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER8_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER9_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER9_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER10_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER10_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER11_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER11_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER12_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER12_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER13_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER13_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER14_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER14_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER15_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER15_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER16_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER16_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER17_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER17_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER18_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER18_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER19_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER19_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER20_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER20_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER21_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER21_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER22_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER22_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER23_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER23_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER24_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER24_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER25_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER25_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER26_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER26_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER27_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER27_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER28_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER28_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER29_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER29_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER30_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER30_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER31_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER31_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER32_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER32_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER33_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER33_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER34_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER34_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER35_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER35_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER36_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER36_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER37_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER37_PER] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER38_AMT] [varchar](max) NULL,
	[PBP_B13H_MAXPLAN_OTHER38_PER] [varchar](max) NULL,
	[PBP_B13H_WAIVER_YN] [varchar](max) NULL,
	[PBP_B13H_WAIVER] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_YN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_EPSDT_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_EPSDT_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_TCCPW_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_TCCPW_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_FBCS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_FBCS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_RCS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_RCS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_FPS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_FPS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_NHS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_NHS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_HCBS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_HCBS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_PCS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_PCS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_SDPAS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_SDPAS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_PDNS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_PDNS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_CM_LTC_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_CM_LTC_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_IMDS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_IMDS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_ICFMRS_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_ICFMRS_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_CM_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_CM_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER3_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER3_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER4_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER4_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER5_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER5_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER6_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER6_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER7_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER7_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER8_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER8_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER9_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER9_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER10_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER10_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER11_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER11_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER12_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER12_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER13_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER13_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER14_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER14_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER15_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER15_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER16_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER16_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER17_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER17_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER18_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER18_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER19_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER19_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER20_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER20_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER21_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER21_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER22_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER22_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER23_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER23_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER24_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER24_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER25_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER25_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER26_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER26_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER27_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER27_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER28_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER28_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER29_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER29_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER30_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER30_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER31_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER31_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER32_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER32_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER33_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER33_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER34_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER34_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER35_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER35_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER36_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER36_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER37_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER37_MAX] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER38_MIN] [varchar](max) NULL,
	[PBP_B13H_PAYAMT_OTHER38_MAX] [varchar](max) NULL,
	[PBP_B13H_ADD_SRVCS_COINS_YN] [varchar](max) NULL,
	[PBP_B13H_ADD_SRVCS_COINS_ITEMS] [varchar](max) NULL,
	[PBP_B13H_COINS_EPSDT_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_EPSDT_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_TCCPW_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_TCCPW_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_FBCS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_FBCS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_RCS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_RCS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_FPS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_FPS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_NHS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_NHS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_HCBS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_HCBS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_PCS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_PCS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_SDPAS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_SDPAS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_PDNS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_PDNS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_CM_LTC_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_CM_LTC_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_IMDS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_IMDS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_ICFMRS_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_ICFMRS_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_CM_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_CM_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER3_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER3_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER4_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER4_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER5_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER5_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER6_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER6_MAX] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB13_4]    Script Date: 5/18/2017 4:28:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB13_4](
	[QID] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER7_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER7_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER8_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER8_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER9_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER9_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER10_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER10_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER11_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER11_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER12_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER12_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER13_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER13_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER14_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER14_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER15_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER15_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER16_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER16_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER17_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER17_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER18_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER18_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER19_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER19_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER20_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER20_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER21_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER21_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER22_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER22_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER23_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER23_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER24_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER24_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER25_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER25_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER26_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER26_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER27_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER27_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER28_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER28_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER29_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER29_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER30_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER30_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER31_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER31_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER32_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER32_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER33_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER33_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER34_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER34_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER35_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER35_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER36_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER36_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER37_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER37_MAX] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER38_MIN] [varchar](max) NULL,
	[PBP_B13H_COINS_OTHER38_MAX] [varchar](max) NULL,
	[PBP_B13H_ADD_SRVCS_COPAY_YN] [varchar](max) NULL,
	[PBP_B13H_ADD_SRVCS_COPAY_ITEMS] [varchar](max) NULL,
	[PBP_B13H_COPAY_EPSDT_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_EPSDT_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_TCCPW_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_TCCPW_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_FBCS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_FBCS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_RCS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_RCS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_FPS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_FPS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_NHS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_NHS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_HCBS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_HCBS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_PCS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_PCS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_SDPAS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_SDPAS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_PDNS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_PDNS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_CM_LTC_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_CM_LTC_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_IMDS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_IMDS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_ICFMRS_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_ICFMRS_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_CM_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_CM_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER3_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER3_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER4_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER4_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER5_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER5_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER6_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER6_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER7_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER7_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER8_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER8_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER9_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER9_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER10_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER10_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER11_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER11_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER12_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER12_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER13_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER13_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER14_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER14_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER15_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER15_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER16_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER16_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER17_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER17_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER18_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER18_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER19_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER19_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER20_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER20_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER21_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER21_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER22_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER22_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER23_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER23_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER24_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER24_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER25_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER25_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER26_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER26_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER27_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER27_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER28_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER28_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER29_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER29_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER30_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER30_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER31_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER31_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER32_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER32_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER33_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER33_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER34_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER34_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER35_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER35_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER36_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER36_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER37_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER37_MAX] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER38_MIN] [varchar](max) NULL,
	[PBP_B13H_COPAY_OTHER38_MAX] [varchar](max) NULL,
	[PBP_B13H_AUTH_YN] [varchar](max) NULL,
	[PBP_B13H_AUTH_TCCPW] [varchar](max) NULL,
	[PBP_B13H_AUTH_EPSDT] [varchar](max) NULL,
	[PBP_B13H_AUTH_FBCS] [varchar](max) NULL,
	[PBP_B13H_AUTH_RCS] [varchar](max) NULL,
	[PBP_B13H_AUTH_FPS] [varchar](max) NULL,
	[PBP_B13H_AUTH_NHS] [varchar](max) NULL,
	[PBP_B13H_AUTH_HCBS] [varchar](max) NULL,
	[PBP_B13H_AUTH_PCS] [varchar](max) NULL,
	[PBP_B13H_AUTH_SDPAS] [varchar](max) NULL,
	[PBP_B13H_AUTH_PDNS] [varchar](max) NULL,
	[PBP_B13H_AUTH_CM_LTC] [varchar](max) NULL,
	[PBP_B13H_AUTH_IMDS] [varchar](max) NULL,
	[PBP_B13H_AUTH_ICFMRS] [varchar](max) NULL,
	[PBP_B13H_AUTH_CM] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER1] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER2] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER3] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER4] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER5] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER6] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER7] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER8] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER9] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER10] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER11] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER12] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER13] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER14] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER15] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER16] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER17] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER18] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER19] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER20] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER21] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER22] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER23] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER24] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER25] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER26] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER27] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER28] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER29] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER30] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER31] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER32] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER33] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER34] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER35] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER36] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER37] [varchar](max) NULL,
	[PBP_B13H_AUTH_OTHER38] [varchar](max) NULL,
	[PBP_B13H_REF_YN] [varchar](max) NULL,
	[PBP_B13H_REF_SERVICES] [varchar](max) NULL,
	[PBP_B13H_NOTES] [varchar](max) NULL,
	[PBP_B13H_NOTES2] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB14]    Script Date: 5/18/2017 4:29:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB14](
	[QID] [varchar](max) NULL,
	[PBP_B14A_MC_PREVENT_ATTEST] [varchar](max) NULL,
	[PBP_B14A_AUTH] [varchar](max) NULL,
	[PBP_B14A_REFER_YN] [varchar](max) NULL,
	[PBP_B14A_NOTES] [varchar](max) NULL,
	[PBP_B14B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B14B_BENDESC_RPE_AMO] [varchar](max) NULL,
	[PBP_B14B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B14B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B14B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B14B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B14B_COINS_YN] [varchar](max) NULL,
	[PBP_B14B_COINS_PCT_RPE_MIN] [varchar](max) NULL,
	[PBP_B14B_COINS_PCT_RPE_MAX] [varchar](max) NULL,
	[PBP_B14B_DED_YN] [varchar](max) NULL,
	[PBP_B14B_DED_AMT] [varchar](max) NULL,
	[PBP_B14B_COPAY_YN] [varchar](max) NULL,
	[PBP_B14B_COPAY_AMT_RPE_MIN] [varchar](max) NULL,
	[PBP_B14B_COPAY_AMT_RPE_MAX] [varchar](max) NULL,
	[PBP_B14B_AUTH] [varchar](max) NULL,
	[PBP_B14B_REFER_YN] [varchar](max) NULL,
	[PBP_B14B_NOTES] [varchar](max) NULL,
	[PBP_B14C_BENDESC_YN] [varchar](max) NULL,
	[PBP_B14C_BENDESC_EHC] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_HEC] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_NTB] [varchar](max) NULL,
	[PBP_B14C_BENDESC_LIM_NTB] [varchar](max) NULL,
	[PBP_B14C_BENDESC_NUM_NTB] [varchar](max) NULL,
	[PBP_B14C_BENDESC_DUR_NTB] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_SC] [varchar](max) NULL,
	[PBP_B14C_BENDESC_VISIT_NUM_SC] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_MHC] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_EDM] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_TM] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_RAT] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_BSD] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_CS] [varchar](max) NULL,
	[PBP_B14C_BENDESC_LIM_CS] [varchar](max) NULL,
	[PBP_B14C_BENDESC_NUM_CS] [varchar](max) NULL,
	[PBP_B14C_BENDESC_DUR_CS] [varchar](max) NULL,
	[PBP_B14C_BENDESC_DUR_NUM_CS] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_ISA] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_PRS] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_MNT] [varchar](max) NULL,
	[PBP_B14C_MNT_MCD_YN] [varchar](max) NULL,
	[PBP_B14C_MNT_MCD_UNIT] [varchar](max) NULL,
	[PBP_B14C_MNT_MCD_NUM] [varchar](max) NULL,
	[PBP_B14C_MNT_NMCD_YN] [varchar](max) NULL,
	[PBP_B14C_MNT_NMCD_UNIT] [varchar](max) NULL,
	[PBP_B14C_MNT_NMCD_NUM] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_IMR] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_RP] [varchar](max) NULL,
	[PBP_B14C_RP_BENDESC_EHC] [varchar](max) NULL,
	[PBP_B14C_RP_BENDESC_OTHER] [varchar](max) NULL,
	[PBP_B14C_RP_DAYS] [varchar](max) NULL,
	[PBP_B14C_RP_MAX_MEALS] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_WIG] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_WM] [varchar](max) NULL,
	[PBP_B14C_BENDESC_AMO_AT] [varchar](max) NULL,
	[PBP_B14C_AT_VISITS] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_EHC] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_HEC] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_HEC] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_NTB] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_NTB] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_SC] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_SC] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_MHC] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_MHC] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_EDM] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_EDM] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_TM] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_TM] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_RAT] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_RAT] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_BSD] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_BSD] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_CS] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_CS] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_ISA] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_ISA] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_PRS] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_PRS] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_MNT] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_MNT] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_IMR] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_IMR] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_RP] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_RP] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_WIG] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_WIG] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_WM] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_WM] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_AMT_AT] [varchar](max) NULL,
	[PBP_B14C_MAXPLAN_PER_AT] [varchar](max) NULL,
	[PBP_B14C_MAXENR_YN] [varchar](max) NULL,
	[PBP_B14C_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B14C_MAXENR_PER] [varchar](max) NULL,
	[PBP_B14C_COINS_YN] [varchar](max) NULL,
	[PBP_B14C_COINS_EHC] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_HEC] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_HEC] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_NTB] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_NTB] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_SC] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_SC] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_MHC] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_MHC] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_EDM] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_EDM] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_TM] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_TM] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_RAT] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_RAT] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_BSD] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_BSD] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_CS] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_CS] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_ISA] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_ISA] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_PRS] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_PRS] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_MNT] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_MNT] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_IMR] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_IMR] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_RP] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_RP] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_WIG] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_WIG] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_WM] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_WM] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MIN_AT] [varchar](max) NULL,
	[PBP_B14C_COINS_PCT_MAX_AT] [varchar](max) NULL,
	[PBP_B14C_DED_YN] [varchar](max) NULL,
	[PBP_B14C_DED_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_YN] [varchar](max) NULL,
	[PBP_B14C_COPAY_EHC] [varchar](max) NULL,
	[PBP_B14C_COPAY_HEC_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_HEC_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_NTB_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_NTB_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_SC_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_SC_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_MHC_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_MHC_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_EDM_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_EDM_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_TM_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_TM_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_RAT_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_RAT_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_BSD_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_BSD_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_CS_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_CS_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_ISA_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_ISA_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_PRS_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_PRS_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_MNT_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_MNT_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_IMR_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_IMR_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_RP_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_RP_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_WIG_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_WIG_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_WM_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_WM_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_AT_MIN_AMT] [varchar](max) NULL,
	[PBP_B14C_COPAY_AT_MAX_AMT] [varchar](max) NULL,
	[PBP_B14C_AUTH] [varchar](max) NULL,
	[PBP_B14C_REFER_YN] [varchar](max) NULL,
	[PBP_B14C_NOTES_HEC] [varchar](max) NULL,
	[PBP_B14C_NOTES_NTB] [varchar](max) NULL,
	[PBP_B14C_NOTES_SC] [varchar](max) NULL,
	[PBP_B14C_NOTES_MHC] [varchar](max) NULL,
	[PBP_B14C_NOTES_EDM] [varchar](max) NULL,
	[PBP_B14C_NOTES_TM] [varchar](max) NULL,
	[PBP_B14C_NOTES_RAT] [varchar](max) NULL,
	[PBP_B14C_NOTES_BSD] [varchar](max) NULL,
	[PBP_B14C_NOTES_CS] [varchar](max) NULL,
	[PBP_B14C_NOTES_ISA] [varchar](max) NULL,
	[PBP_B14C_NOTES_PRS] [varchar](max) NULL,
	[PBP_B14C_NOTES_MNT] [varchar](max) NULL,
	[PBP_B14C_NOTES_IMR] [varchar](max) NULL,
	[PBP_B14C_NOTES_RP] [varchar](max) NULL,
	[PBP_B14C_NOTES_WIG] [varchar](max) NULL,
	[PBP_B14C_NOTES_WM] [varchar](max) NULL,
	[PBP_B14C_NOTES_AT] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB14_2]    Script Date: 5/18/2017 4:29:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB14_2](
	[QID] [varchar](max) NULL,
	[PBP_B14D_MAXENR_YN] [varchar](max) NULL,
	[PBP_B14D_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B14D_MAXENR_PER] [varchar](max) NULL,
	[PBP_B14D_COINS_YN] [varchar](max) NULL,
	[PBP_B14D_COINS_MC_MIN_PCT] [varchar](max) NULL,
	[PBP_B14D_COINS_MC_MAX_PCT] [varchar](max) NULL,
	[PBP_B14D_DED_YN] [varchar](max) NULL,
	[PBP_B14D_DED_AMT] [varchar](max) NULL,
	[PBP_B14D_COPAY_YN] [varchar](max) NULL,
	[PBP_B14D_COPAY_MC_MIN_AMT] [varchar](max) NULL,
	[PBP_B14D_COPAY_MC_MAX_AMT] [varchar](max) NULL,
	[PBP_B14D_AUTH] [varchar](max) NULL,
	[PBP_B14D_REFER_YN] [varchar](max) NULL,
	[PBP_B14D_NOTES] [varchar](max) NULL,
	[PBP_B14E_OTHER_YN] [varchar](max) NULL,
	[PBP_B14E_OTHERS_EHC] [varchar](max) NULL,
	[PBP_B14E_OTHER1_NAME] [varchar](max) NULL,
	[PBP_B14E_OTHER2_NAME] [varchar](max) NULL,
	[PBP_B14E_OTHER3_NAME] [varchar](max) NULL,
	[PBP_B14E_OTHER4_NAME] [varchar](max) NULL,
	[PBP_B14E_OTHER5_NAME] [varchar](max) NULL,
	[PBP_B14E_MAXENR_YN] [varchar](max) NULL,
	[PBP_B14E_MAXENR_EHC] [varchar](max) NULL,
	[PBP_B14E_MAXENR_AMT_GLAUC] [varchar](max) NULL,
	[PBP_B14E_MAXENR_PER_GLAUC] [varchar](max) NULL,
	[PBP_B14E_MAXENR_AMT_DIAB] [varchar](max) NULL,
	[PBP_B14E_MAXENR_PER_DIAB] [varchar](max) NULL,
	[PBP_B14E_MAXENR_AMT_OTHER1] [varchar](max) NULL,
	[PBP_B14E_MAXENR_PER_OTHER1] [varchar](max) NULL,
	[PBP_B14E_MAXENR_AMT_OTHER2] [varchar](max) NULL,
	[PBP_B14E_MAXENR_PER_OTHER2] [varchar](max) NULL,
	[PBP_B14E_MAXENR_AMT_OTHER3] [varchar](max) NULL,
	[PBP_B14E_MAXENR_PER_OTHER3] [varchar](max) NULL,
	[PBP_B14E_MAXENR_AMT_OTHER4] [varchar](max) NULL,
	[PBP_B14E_MAXENR_PER_OTHER4] [varchar](max) NULL,
	[PBP_B14E_MAXENR_AMT_OTHER5] [varchar](max) NULL,
	[PBP_B14E_MAXENR_PER_OTHER5] [varchar](max) NULL,
	[PBP_B14E_COINS_YN] [varchar](max) NULL,
	[PBP_B14E_COINS_EHC] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MIN_GLAUC] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MAX_GLAUC] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MIN_DIAB] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MAX_DIAB] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MIN_OTHER1] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MAX_OTHER1] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MIN_OTHER2] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MAX_OTHER2] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MIN_OTHER3] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MAX_OTHER3] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MIN_OTHER4] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MAX_OTHER4] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MIN_OTHER5] [varchar](max) NULL,
	[PBP_B14E_COINS_PCT_MAX_OTHER5] [varchar](max) NULL,
	[PBP_B14E_DED_YN] [varchar](max) NULL,
	[PBP_B14E_DED_EHC] [varchar](max) NULL,
	[PBP_B14E_DED_AMT_GLAUC] [varchar](max) NULL,
	[PBP_B14E_DED_AMT_DIAB] [varchar](max) NULL,
	[PBP_B14E_DED_AMT_OTHER1] [varchar](max) NULL,
	[PBP_B14E_DED_AMT_OTHER2] [varchar](max) NULL,
	[PBP_B14E_DED_AMT_OTHER3] [varchar](max) NULL,
	[PBP_B14E_DED_AMT_OTHER4] [varchar](max) NULL,
	[PBP_B14E_DED_AMT_OTHER5] [varchar](max) NULL,
	[PBP_B14E_COPAY_YN] [varchar](max) NULL,
	[PBP_B14E_COPAY_EHC] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MIN_GLAUC] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MAX_GLAUC] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MIN_DIAB] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MAX_DIAB] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MIN_OTHER1] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MAX_OTHER1] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MIN_OTHER2] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MAX_OTHER2] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MIN_OTHER3] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MAX_OTHER3] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MIN_OTHER4] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MAX_OTHER4] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MIN_OTHER5] [varchar](max) NULL,
	[PBP_B14E_COPAY_AMT_MAX_OTHER5] [varchar](max) NULL,
	[PBP_B14E_AUTH_GLAUC] [varchar](max) NULL,
	[PBP_B14E_AUTH_DIAB] [varchar](max) NULL,
	[PBP_B14E_AUTH_OTHER1] [varchar](max) NULL,
	[PBP_B14E_AUTH_OTHER2] [varchar](max) NULL,
	[PBP_B14E_AUTH_OTHER3] [varchar](max) NULL,
	[PBP_B14E_AUTH_OTHER4] [varchar](max) NULL,
	[PBP_B14E_AUTH_OTHER5] [varchar](max) NULL,
	[PBP_B14E_REFER_YN] [varchar](max) NULL,
	[PBP_B14E_REFER_EHC] [varchar](max) NULL,
	[PBP_B14E_NOTES_GLAUC] [varchar](max) NULL,
	[PBP_B14E_NOTES_DIAB] [varchar](max) NULL,
	[PBP_B14E_NOTES_OTHER1] [varchar](max) NULL,
	[PBP_B14E_NOTES_OTHER2] [varchar](max) NULL,
	[PBP_B14E_NOTES_OTHER3] [varchar](max) NULL,
	[PBP_B14E_NOTES_OTHER4] [varchar](max) NULL,
	[PBP_B14E_NOTES_OTHER5] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB15]    Script Date: 5/18/2017 4:29:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB15](
	[QID] [varchar](max) NULL,
	[MRX_B_MAX_OOP_YN] [varchar](max) NULL,
	[MRX_B_MAX_OOP_AMT] [varchar](max) NULL,
	[MRX_B_MAX_OOP_PER] [varchar](max) NULL,
	[MRX_B_COINS_YN] [varchar](max) NULL,
	[MRX_B_COINS_PCT] [varchar](max) NULL,
	[MRX_B_DED_YN] [varchar](max) NULL,
	[MRX_B_DED_AMT] [varchar](max) NULL,
	[MRX_B_COPAY_YN] [varchar](max) NULL,
	[MRX_B_COPAY_MIN_AMT] [varchar](max) NULL,
	[MRX_B_COPAY_MAX_AMT] [varchar](max) NULL,
	[MRX_B_AUTH_YN] [varchar](max) NULL,
	[MRX_B_CHEMO_COINS_PCT] [varchar](max) NULL,
	[MRX_B_CHEMO_COPAY_AMT] [varchar](max) NULL,
	[MRX_B_CHEMO_COPAY_AMT_MAX] [varchar](max) NULL,
	[MRX_B_CHEMO_COINS_MAX_PCT] [varchar](max) NULL,
	[MRX_B_COINS_MAX_PCT] [varchar](max) NULL,
	[MRX_B_HI_YN] [varchar](max) NULL,
	[MRX_B_HI_MCAID_YN] [varchar](max) NULL,
	[MRX_B_NOTES] [varchar](max) NULL,
	[MRX_B_COINS_EHC] [varchar](max) NULL,
	[MRX_B_COPAY_EHC] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[PBPB16]    Script Date: 5/18/2017 4:30:13 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB16](
	[QID] [varchar](max) NULL,
	[PBP_B16A_BENDESC_YN] [varchar](max) NULL,
	[PBP_B16A_BENDESC_EHC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_AMO_OE] [varchar](max) NULL,
	[PBP_B16A_BENDESC_LIM_OE] [varchar](max) NULL,
	[PBP_B16A_BENDESC_NUMV_OE] [varchar](max) NULL,
	[PBP_B16A_BENDESC_PER_OE] [varchar](max) NULL,
	[PBP_B16A_BENDESC_AMO_PC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_LIM_PC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_NUMV_PC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_PER_PC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_AMO_FT] [varchar](max) NULL,
	[PBP_B16A_BENDESC_LIM_FT] [varchar](max) NULL,
	[PBP_B16A_BENDESC_NUMV_FT] [varchar](max) NULL,
	[PBP_B16A_BENDESC_PER_FT] [varchar](max) NULL,
	[PBP_B16A_BENDESC_AMO_DX] [varchar](max) NULL,
	[PBP_B16A_BENDESC_LIM_DX] [varchar](max) NULL,
	[PBP_B16A_BENDESC_NUMV_DX] [varchar](max) NULL,
	[PBP_B16A_BENDESC_PER_DX] [varchar](max) NULL,
	[PBP_B16A_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B16A_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B16A_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B16A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B16A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B16A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B16A_COINS_YN] [varchar](max) NULL,
	[PBP_B16A_COINS_CSERV_SC_POV_YN] [varchar](max) NULL,
	[PBP_B16A_COINS_CSERV_SC_POV] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_OV] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_OE] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_MAXOE] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_PC] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_MAXPC] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_FT] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_MAXFT] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_DX] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_MAXDX] [varchar](max) NULL,
	[PBP_B16A_DED_YN] [varchar](max) NULL,
	[PBP_B16A_DED_AMT] [varchar](max) NULL,
	[PBP_B16A_COPAY_YN] [varchar](max) NULL,
	[PBP_B16A_COPAY_CSERV_SC_POV_YN] [varchar](max) NULL,
	[PBP_B16A_COPAY_CSERV_SC_POV] [varchar](max) NULL,
	[PBP_B16A_COPAY_OV_AMT] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_OEMIN] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_OEMAX] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_PCMIN] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_PCMAX] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_FTMIN] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_FTMAX] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_DXMIN] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_DXMAX] [varchar](max) NULL,
	[PBP_B16A_NOTES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B16B_BENDESC_EHC] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_ES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_ES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_ES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_ES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_DS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_DS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_DS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_DS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_RS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_RS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_RS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_RS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_EPE] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_EPE] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_EPE] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_EPE] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_POO] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_POO] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_POO] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_POO] [varchar](max) NULL,
	[PBP_B16B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B16B_MAXBENE_TYPE] [varchar](max) NULL,
	[PBP_B16B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B16B_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B16B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B16B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B16B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B16B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B16B_COINS_YN] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAX_MC] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_ES] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXES] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_DS] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXDS] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_RS] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXRS] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_EPE] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXEPE] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_POO] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXPOO] [varchar](max) NULL,
	[PBP_B16B_DED_YN] [varchar](max) NULL,
	[PBP_B16B_DED_AMT] [varchar](max) NULL,
	[PBP_B16B_COPAY_YN] [varchar](max) NULL,
	[PBP_B16B_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B16B_COPAY_MCMAX_AMT] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_ESMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_ESMAX] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_DSMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_DSMAX] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_RSMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_RSMAX] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_EPEMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_EPEMAX] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_POOMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_POOMAX] [varchar](max) NULL,
	[PBP_B16B_AUTH] [varchar](max) NULL,
	[PBP_B16B_REFER_YN] [varchar](max) NULL,
	[PBP_B16B_NOTES] [varchar](max) NULL,
	[PBP_B16A_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B16B_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B16A_COINS_EHC] [varchar](max) NULL,
	[PBP_B16A_COPAY_EHC] [varchar](max) NULL,
	[PBP_B16B_COINS_EHC] [varchar](max) NULL,
	[PBP_B16B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B16A_AUTH] [varchar](max) NULL,
	[PBP_B16A_REFER_YN] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB17]    Script Date: 5/18/2017 4:30:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB17](
	[QID] [varchar](max) NULL,
	[PBP_B17A_BENDESC_YN] [varchar](max) NULL,
	[PBP_B17A_BENDESC_ENH] [varchar](max) NULL,
	[PBP_B17A_BENDESC_AMO_REX] [varchar](max) NULL,
	[PBP_B17A_BENDESC_LIM] [varchar](max) NULL,
	[PBP_B17A_BENDESC_NUMV] [varchar](max) NULL,
	[PBP_B17A_BENDESC_PER] [varchar](max) NULL,
	[PBP_B17A_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B17A_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B17A_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B17A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B17A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B17A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B17A_COINS_YN] [varchar](max) NULL,
	[PBP_B17A_COINS_MCMIN_PCT] [varchar](max) NULL,
	[PBP_B17A_COINS_MCMAX_PCT] [varchar](max) NULL,
	[PBP_B17A_COINS_REXMIN_PCT] [varchar](max) NULL,
	[PBP_B17A_COINS_REXMAX_PCT] [varchar](max) NULL,
	[PBP_B17A_DED_YN] [varchar](max) NULL,
	[PBP_B17A_DED_AMT] [varchar](max) NULL,
	[PBP_B17A_COPAY_YN] [varchar](max) NULL,
	[PBP_B17A_COPAY_MCMIN_AMT] [varchar](max) NULL,
	[PBP_B17A_COPAY_MCMAX_AMT] [varchar](max) NULL,
	[PBP_B17A_COPAY_REXMIN_AMT] [varchar](max) NULL,
	[PBP_B17A_COPAY_REXMAX_AMT] [varchar](max) NULL,
	[PBP_B17A_NOTES] [varchar](max) NULL,
	[PBP_B17B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B17B_BENDESC_ENH] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_CL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_LIM_CL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_NUMV_CL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_PER_CL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_EGS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_LIM_EGS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_NUMV_EGS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_PER_EGS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_EGI] [varchar](max) NULL,
	[PBP_B17B_BENDESC_LIM_EGL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_NUMV_EGL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_PER_EGL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_EGF] [varchar](max) NULL,
	[PBP_B17B_BENDESC_LIM_EGF] [varchar](max) NULL,
	[PBP_B17B_BENDESC_NUMV_EGF] [varchar](max) NULL,
	[PBP_B17B_BENDESC_PER_EGF] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_UPG] [varchar](max) NULL,
	[PBP_B17B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B17B_MAXPLAN_TYPE] [varchar](max) NULL,
	[PBP_B17B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B17B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B17B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B17B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B17B_COINS_YN] [varchar](max) NULL,
	[PBP_B17B_DED_YN] [varchar](max) NULL,
	[PBP_B17B_DED_AMT] [varchar](max) NULL,
	[PBP_B17B_COPAY_YN] [varchar](max) NULL,
	[PBP_B17B_NOTES] [varchar](max) NULL,
	[PBP_B17B_COMB_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_EGL] [varchar](max) NULL,
	[PBP_B17B_COMB_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_UPG] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_EGS] [varchar](max) NULL,
	[PBP_B17B_COMB_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_EGF] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_EGL] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_EGS] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_CL] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_CL] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_UPG] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_EGF] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_BENDESC] [varchar](max) NULL,
	[PBP_B17A_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B17B_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B17A_COINS_EHC] [varchar](max) NULL,
	[PBP_B17A_COPAY_EHC] [varchar](max) NULL,
	[PBP_B17B_COINS_EHC] [varchar](max) NULL,
	[PBP_B17B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B17A_AUTH] [varchar](max) NULL,
	[PBP_B17B_AUTH] [varchar](max) NULL,
	[PBP_B17A_REFER_YN] [varchar](max) NULL,
	[PBP_B17B_REFER_YN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_MC_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_MC_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_CL_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_CL_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_CL_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_CL_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGF_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGF_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGF_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGF_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGL_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGL_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGL_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGL_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGS_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGS_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGS_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGS_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_UPG_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_UPG_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_UPG_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_UPG_MAX] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB18]    Script Date: 5/18/2017 4:30:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB18](
	[QID] [varchar](max) NULL,
	[PBP_B18A_BENDESC_YN] [varchar](max) NULL,
	[PBP_B18A_BENDESC_ENH] [varchar](max) NULL,
	[PBP_B18A_BENDESC_AMO_RHT] [varchar](max) NULL,
	[PBP_B18A_BENDESC_LIM_RHT] [varchar](max) NULL,
	[PBP_B18A_BENDESC_NUMV_CL] [varchar](max) NULL,
	[PBP_B18A_BENDESC_PER_RHT] [varchar](max) NULL,
	[PBP_B18A_BENDESC_AMO_FHA] [varchar](max) NULL,
	[PBP_B18A_BENDESC_LIM_FHA] [varchar](max) NULL,
	[PBP_B18A_BENDESC_NUMV_FHA] [varchar](max) NULL,
	[PBP_B18A_BENDESC_PER_FHA] [varchar](max) NULL,
	[PBP_B18A_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B18A_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B18A_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B18A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B18A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B18A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B18A_COINS_YN] [varchar](max) NULL,
	[PBP_B18A_MED_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B18A_MED_COINS_PCT] [varchar](max) NULL,
	[PBP_B18A_COINS_PCT_RHT] [varchar](max) NULL,
	[PBP_B18A_COINS_PCT_MAX_RHT] [varchar](max) NULL,
	[PBP_B18A_COINS_PCT_FHA] [varchar](max) NULL,
	[PBP_B18A_COINS_PCT_MAX_FHA] [varchar](max) NULL,
	[PBP_B18A_DED_YN] [varchar](max) NULL,
	[PBP_B18A_DED_AMT] [varchar](max) NULL,
	[PBP_B18A_COPAY_YN] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT] [varchar](max) NULL,
	[PBP_B18A_MED_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT_RHT] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT_MAX_RHT] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT_FHA] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT_MAX_FHA] [varchar](max) NULL,
	[PBP_B18A_AUTH] [varchar](max) NULL,
	[PBP_B18A_REFER_YN] [varchar](max) NULL,
	[PBP_B18A_NOTES] [varchar](max) NULL,
	[PBP_B18B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B18B_BENDESC_ENH] [varchar](max) NULL,
	[PBP_B18B_BENDESC_AMO_AT] [varchar](max) NULL,
	[PBP_B18B_BENDESC_LIM_AT] [varchar](max) NULL,
	[PBP_B18B_BENDESC_NUMV_AT] [varchar](max) NULL,
	[PBP_B18B_BENDESC_PER_AT] [varchar](max) NULL,
	[PBP_B18B_BENDESC_AMO_IE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_LIM_IE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_NUMV_IE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_PER_IE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_AMO_OE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_LIM_OE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_NUMV_OE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_PER_OE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_AMO_OTE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_LIM_OTE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_NUMV_OTE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_PER_OTE] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_PEREAR_YN] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_TYPE] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B18B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B18B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B18B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B18B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B18B_COINS_YN] [varchar](max) NULL,
	[PBP_B18B_DED_YN] [varchar](max) NULL,
	[PBP_B18B_DED_AMT] [varchar](max) NULL,
	[PBP_B18B_COPAY_YN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AT_MIN_AMT] [varchar](max) NULL,
	[PBP_B18B_COPAY_AT_MAX_AMT] [varchar](max) NULL,
	[PBP_B18B_AUTH] [varchar](max) NULL,
	[PBP_B18B_REFER_YN] [varchar](max) NULL,
	[PBP_B18B_NOTES] [varchar](max) NULL,
	[PBP_B18A_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B18A_COINS_EHC] [varchar](max) NULL,
	[PBP_B18A_COPAY_EHC] [varchar](max) NULL,
	[PBP_B18B_COINS_EHC] [varchar](max) NULL,
	[PBP_B18B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_IE_MIN] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_IE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_IE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_IE_MAX] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_OE_MIN] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_OE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_OE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_OE_MAX] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_OTE_MIN] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_OTE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_OTE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_OTE_MAX] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_AT_MIN] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_AT_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_IE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_IE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_OE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_OE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_OTE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_OTE_MAX] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB19]    Script Date: 5/18/2017 4:31:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB19](
	[QID] [varchar](max) NULL,
	[PBP_B19A_ATTESTATION] [varchar](max) NULL,
	[PBP_B19A_REDUCT_COST_ADD_YN] [varchar](max) NULL,
	[PBP_B19A_REDUCT_COST_YN] [varchar](max) NULL,
	[PBP_B19A_REDUCT_PACKAGE_NUM] [varchar](max) NULL,
	[PBP_B19B_ADD_COST_YN] [varchar](max) NULL,
	[PBP_B19B_ADD_PACKAGE_NUM] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB2]    Script Date: 5/18/2017 4:31:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB2](
	[QID] [varchar](max) NULL,
	[PBP_B2_BENDESC_YN] [varchar](max) NULL,
	[PBP_B2_BENDESC_AD_NMCS] [varchar](max) NULL,
	[PBP_B2_BENDESC_AMO_AD] [varchar](max) NULL,
	[PBP_B2_BENEDESC_LIM_AD] [varchar](max) NULL,
	[PBP_B2_BENDESC_AD] [varchar](max) NULL,
	[PBP_B2_BENDESC_AMO_NMCS] [varchar](max) NULL,
	[PBP_B2_BENDESC_PR_HOSP_YN] [varchar](max) NULL,
	[PBP_B2_BENDESC_PR_HOSP_NUM] [varchar](max) NULL,
	[PBP_B2_MAXENR_YN] [varchar](max) NULL,
	[PBP_B2_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B2_MAXENR_PER] [varchar](max) NULL,
	[PBP_B2_COINS_YN] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_STRUC_YN] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_PCT] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_INTRVL_NUM] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_PCT_INTRVL1] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_BGND_INTRVL1] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_ENDD_INTRVL1] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_PCT_INTRVL2] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_BGND_INTRVL2] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_ENDD_INTRVL2] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_PCT_INTRVL3] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_BGND_INTRVL3] [varchar](max) NULL,
	[PBP_B2_COINS_NMCS_ENDD_INTRVL3] [varchar](max) NULL,
	[PBP_B2_DED_YN] [varchar](max) NULL,
	[PBP_B2_COPAY_YN] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_STRUC_YN] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_AMT] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_INTRVL_NUM] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_AMT_INTRVL1] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_BGND_INTRVL1] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_ENDD_INTRVL1] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_AMT_INTRVL2] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_BGND_INTRVL2] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_ENDD_INTRVL2] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_AMT_INTRVL3] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_BGND_INTRVL3] [varchar](max) NULL,
	[PBP_B2_COPAY_NMCS_ENDD_INTRVL3] [varchar](max) NULL,
	[PBP_B2_AUTH] [varchar](max) NULL,
	[PBP_B2_REFER_YN] [varchar](max) NULL,
	[PBP_B2_NOTES] [varchar](max) NULL,
	[PBP_B2_HOSP_BEN_PERIOD] [varchar](max) NULL,
	[PBP_B2_HOSP_BEN_PERIOD_OTH] [varchar](max) NULL,
	[PBP_B2_COST_DISCHARGE_YN] [varchar](max) NULL,
	[PBP_B2_COST_VARY_TIERS_YN] [varchar](max) NULL,
	[PBP_B2_COST_VARY_TIER_NUM] [varchar](max) NULL,
	[PBP_B2_COST_VARY_LOW_TIER] [varchar](max) NULL,
	[PBP_B2_MC_COINS_CSTSHR_YN_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B2_MC_COINS_CSTSHR_YN_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B2_MC_COINS_CSTSHR_YN_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_PCT_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COINS_MCS_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B2_DED_AMT_T1] [varchar](max) NULL,
	[PBP_B2_DED_AMT_T2] [varchar](max) NULL,
	[PBP_B2_DED_AMT_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_INT_NUM_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_T1] [varchar](max) NULL,
	[PBP_B2_MC_COPAY_CSTSHR_YN_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_INT_NUM_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_T2] [varchar](max) NULL,
	[PBP_B2_MC_COPAY_CSTSHR_YN_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_INT_NUM_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_MCS_AMT_T3] [varchar](max) NULL,
	[PBP_B2_MC_COPAY_CSTSHR_YN_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_INTRVL_NUM_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B2_AD_COST_VARY_TIERS_YN] [varchar](max) NULL,
	[PBP_B2_AD_COST_VARY_TIER_NUM] [varchar](max) NULL,
	[PBP_B2_AD_COST_VARY_LOW_TIER] [varchar](max) NULL,
	[PBP_B2_COINS_AD_INTRVL_NUM_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COINS_AD_INTRVL_NUM_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_PCT_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COINS_AD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_INTRVL_NUM_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT1_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT2_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT3_T1] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_INTRVL_NUM_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT1_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT2_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT3_T2] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_INTRVL_NUM_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT1_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT2_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_AMT_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_BGND_INT3_T3] [varchar](max) NULL,
	[PBP_B2_COPAY_AD_ENDD_INT3_T3] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB2_B]    Script Date: 5/18/2017 4:31:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB2_B](
	[QID] [varchar](max) NULL,
	[PBP_B2_BENDESC_YN_BONLY] [varchar](max) NULL,
	[PBP_B2_BENDESC_AMO_BONLY] [varchar](max) NULL,
	[PBP_B2_BENDESC_SNF_UNLM_BONLY] [varchar](max) NULL,
	[PBP_B2_BENDE_AL_AD_PRIOR_BONLY] [varchar](max) NULL,
	[PBP_B2_BENDESC_SNF_PER_BONLY] [varchar](max) NULL,
	[PBP_B2_BENDESC_HOSTAY_BONLY_YN] [varchar](max) NULL,
	[PBP_B2_BENDE_NMDAY_PRIOR_BONLY] [varchar](max) NULL,
	[PBP_B2_MAXPLAN_YN_BONLY] [varchar](max) NULL,
	[PBP_B2_MAXPLAN_AMT_BONLY] [varchar](max) NULL,
	[PBP_B2_MAXPLAN_PER_BONLY] [varchar](max) NULL,
	[PBP_B2_MAXENR_YN_BONLY] [varchar](max) NULL,
	[PBP_B2_MAXENR_AMT_BONLY] [varchar](max) NULL,
	[PBP_B2_MAXENR_PER_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_YN_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PCT_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_NUM_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_PCT1_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_BGND1_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_ENDD1_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_PCT2_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_BGND2_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_ENDD2_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_PCT3_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_BGND3_BONLY] [varchar](max) NULL,
	[PBP_B2_COINS_PS_ENDD3_BONLY] [varchar](max) NULL,
	[PBP_B2_DED_YN_BONLY] [varchar](max) NULL,
	[PBP_B2_DED_AMT_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_YN_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_AMT_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_NUM_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_AMT1_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_BGND1_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_ENDD1_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_AMT2_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_BGND2_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_ENDD2_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_AMT3_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_BGND3_BONLY] [varchar](max) NULL,
	[PBP_B2_COPAY_PS_ENDD3_BONLY] [varchar](max) NULL,
	[PBP_B2_AUTH_BONLY] [varchar](max) NULL,
	[PBP_B2_REFER_YN_BONLY] [varchar](max) NULL,
	[PBP_B2_NOTES_BONLY] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB20]    Script Date: 5/18/2017 4:32:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB20](
	[QID] [varchar](max) NULL,
	[PBP_B20_BENDESC_YN] [varchar](max) NULL,
	[PBP_B20_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B20_BENDESC_GRP_NUM] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_TYPES] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_NET_COPAY_YN] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_1YR] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_6MTH] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_3MTH] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_1MTH] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_OTH] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_UNUSED_YN] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_CDT] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_PER_CDT] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_CDT_1YR] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_CDT_6MTH] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_CDT_3MTH] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_CDT_1MTH] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_AMT_CDT_OTH] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_GRP_UNLM_YN] [varchar](max) NULL,
	[PBP_B20_MAXPLAN_GRP_WAIVED] [varchar](max) NULL,
	[PBP_B20_ADDL_COST] [varchar](max) NULL,
	[PBP_B20_MAXENR_YN] [varchar](max) NULL,
	[PBP_B20_MAXENR_CDT] [varchar](max) NULL,
	[PBP_B20_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B20_MAXENR_PER] [varchar](max) NULL,
	[PBP_B20_COINS_MC_YN] [varchar](max) NULL,
	[PBP_B20_COINS_MC_PCT] [varchar](max) NULL,
	[PBP_B20_DED_YN] [varchar](max) NULL,
	[PBP_B20_DED_CDT] [varchar](max) NULL,
	[PBP_B20_DED_AMT] [varchar](max) NULL,
	[PBP_B20_COPAY_MC_YN] [varchar](max) NULL,
	[PBP_B20_COPAY_MIN_MC_AMT] [varchar](max) NULL,
	[PBP_B20_COPAY_MAX_MC_AMT] [varchar](max) NULL,
	[PBP_B20_AUTH] [varchar](max) NULL,
	[PBP_B20_HI_YN] [varchar](max) NULL,
	[PBP_B20_NOTES] [varchar](max) NULL,
	[PBP_B20_GRP1_LABEL] [varchar](max) NULL,
	[PBP_B20_GRP1_DRUG_TYPES] [varchar](max) NULL,
	[PBP_B20_GRP1_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B20_GRP1_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B20_GRP1_MAXPLAN_AMT_1YR] [varchar](max) NULL,
	[PBP_B20_GRP1_MAXPLAN_AMT_6MTH] [varchar](max) NULL,
	[PBP_B20_GRP1_MAXPLAN_AMT_3MTH] [varchar](max) NULL,
	[PBP_B20_GRP1_MAXPLAN_AMT_1MTH] [varchar](max) NULL,
	[PBP_B20_GRP1_MAXPLAN_AMT_PRES] [varchar](max) NULL,
	[PBP_B20_GRP1_MAXPLAN_AMT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP1_PLACE] [varchar](max) NULL,
	[PBP_B20_GRP1_COINS_YN] [varchar](max) NULL,
	[PBP_B20_GRP1_COINS_PCT_DRP] [varchar](max) NULL,
	[PBP_B20_GRP1_COINS_PCT_HMO] [varchar](max) NULL,
	[PBP_B20_GRP1_COINS_PCT_MO] [varchar](max) NULL,
	[PBP_B20_GRP1_COINS_PCT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP1_COPAY_YN] [varchar](max) NULL,
	[PBP_B20_GRP1_COPAY_DRP_AMT] [varchar](max) NULL,
	[PBP_B20_GRP1_COPAY_HMO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP1_COPAY_MO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP1_COPAY_OTH_AMT] [varchar](max) NULL,
	[PBP_B20_GRP1_DRP_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP1_HMO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP1_MO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP1_OTH_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP2_LABEL] [varchar](max) NULL,
	[PBP_B20_GRP2_DRUG_TYPES] [varchar](max) NULL,
	[PBP_B20_GRP2_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B20_GRP2_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B20_GRP2_MAXPLAN_AMT_1YR] [varchar](max) NULL,
	[PBP_B20_GRP2_MAXPLAN_AMT_6MTH] [varchar](max) NULL,
	[PBP_B20_GRP2_MAXPLAN_AMT_3MTH] [varchar](max) NULL,
	[PBP_B20_GRP2_MAXPLAN_AMT_1MTH] [varchar](max) NULL,
	[PBP_B20_GRP2_MAXPLAN_AMT_PRES] [varchar](max) NULL,
	[PBP_B20_GRP2_MAXPLAN_AMT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP2_PLACE] [varchar](max) NULL,
	[PBP_B20_GRP2_COINS_YN] [varchar](max) NULL,
	[PBP_B20_GRP2_COINS_PCT_DRP] [varchar](max) NULL,
	[PBP_B20_GRP2_COINS_PCT_HMO] [varchar](max) NULL,
	[PBP_B20_GRP2_COINS_PCT_MO] [varchar](max) NULL,
	[PBP_B20_GRP2_COINS_PCT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP2_COPAY_YN] [varchar](max) NULL,
	[PBP_B20_GRP2_COPAY_DRP_AMT] [varchar](max) NULL,
	[PBP_B20_GRP2_COPAY_HMO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP2_COPAY_MO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP2_COPAY_OTH_AMT] [varchar](max) NULL,
	[PBP_B20_GRP2_DRP_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP2_HMO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP2_MO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP2_OTH_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP3_LABEL] [varchar](max) NULL,
	[PBP_B20_GRP3_DRUG_TYPES] [varchar](max) NULL,
	[PBP_B20_GRP3_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B20_GRP3_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B20_GRP3_MAXPLAN_AMT_1YR] [varchar](max) NULL,
	[PBP_B20_GRP3_MAXPLAN_AMT_6MTH] [varchar](max) NULL,
	[PBP_B20_GRP3_MAXPLAN_AMT_3MTH] [varchar](max) NULL,
	[PBP_B20_GRP3_MAXPLAN_AMT_1MTH] [varchar](max) NULL,
	[PBP_B20_GRP3_MAXPLAN_AMT_PRES] [varchar](max) NULL,
	[PBP_B20_GRP3_MAXPLAN_AMT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP3_PLACE] [varchar](max) NULL,
	[PBP_B20_GRP3_COINS_YN] [varchar](max) NULL,
	[PBP_B20_GRP3_COINS_PCT_DRP] [varchar](max) NULL,
	[PBP_B20_GRP3_COINS_PCT_HMO] [varchar](max) NULL,
	[PBP_B20_GRP3_COINS_PCT_MO] [varchar](max) NULL,
	[PBP_B20_GRP3_COINS_PCT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP3_COPAY_YN] [varchar](max) NULL,
	[PBP_B20_GRP3_COPAY_DRP_AMT] [varchar](max) NULL,
	[PBP_B20_GRP3_COPAY_HMO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP3_COPAY_MO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP3_COPAY_OTH_AMT] [varchar](max) NULL,
	[PBP_B20_GRP3_DRP_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP3_HMO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP3_MO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP3_OTH_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP4_LABEL] [varchar](max) NULL,
	[PBP_B20_GRP4_DRUG_TYPES] [varchar](max) NULL,
	[PBP_B20_GRP4_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B20_GRP4_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B20_GRP4_MAXPLAN_AMT_1YR] [varchar](max) NULL,
	[PBP_B20_GRP4_MAXPLAN_AMT_6MTH] [varchar](max) NULL,
	[PBP_B20_GRP4_MAXPLAN_AMT_3MTH] [varchar](max) NULL,
	[PBP_B20_GRP4_MAXPLAN_AMT_1MTH] [varchar](max) NULL,
	[PBP_B20_GRP4_MAXPLAN_AMT_PRES] [varchar](max) NULL,
	[PBP_B20_GRP4_MAXPLAN_AMT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP4_PLACE] [varchar](max) NULL,
	[PBP_B20_GRP4_COINS_YN] [varchar](max) NULL,
	[PBP_B20_GRP4_COINS_PCT_DRP] [varchar](max) NULL,
	[PBP_B20_GRP4_COINS_PCT_HMO] [varchar](max) NULL,
	[PBP_B20_GRP4_COINS_PCT_MO] [varchar](max) NULL,
	[PBP_B20_GRP4_COINS_PCT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP4_COPAY_YN] [varchar](max) NULL,
	[PBP_B20_GRP4_COPAY_DRP_AMT] [varchar](max) NULL,
	[PBP_B20_GRP4_COPAY_HMO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP4_COPAY_MO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP4_COPAY_OTH_AMT] [varchar](max) NULL,
	[PBP_B20_GRP4_DRP_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP4_HMO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP4_MO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP4_OTH_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP5_LABEL] [varchar](max) NULL,
	[PBP_B20_GRP5_DRUG_TYPES] [varchar](max) NULL,
	[PBP_B20_GRP5_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B20_GRP5_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B20_GRP5_MAXPLAN_AMT_1YR] [varchar](max) NULL,
	[PBP_B20_GRP5_MAXPLAN_AMT_6MTH] [varchar](max) NULL,
	[PBP_B20_GRP5_MAXPLAN_AMT_3MTH] [varchar](max) NULL,
	[PBP_B20_GRP5_MAXPLAN_AMT_1MTH] [varchar](max) NULL,
	[PBP_B20_GRP5_MAXPLAN_AMT_PRES] [varchar](max) NULL,
	[PBP_B20_GRP5_MAXPLAN_AMT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP5_PLACE] [varchar](max) NULL,
	[PBP_B20_GRP5_COINS_YN] [varchar](max) NULL,
	[PBP_B20_GRP5_COINS_PCT_DRP] [varchar](max) NULL,
	[PBP_B20_GRP5_COINS_PCT_HMO] [varchar](max) NULL,
	[PBP_B20_GRP5_COINS_PCT_MO] [varchar](max) NULL,
	[PBP_B20_GRP5_COINS_PCT_OTH] [varchar](max) NULL,
	[PBP_B20_GRP5_COPAY_YN] [varchar](max) NULL,
	[PBP_B20_GRP5_COPAY_DRP_AMT] [varchar](max) NULL,
	[PBP_B20_GRP5_COPAY_HMO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP5_COPAY_MO_AMT] [varchar](max) NULL,
	[PBP_B20_GRP5_COPAY_OTH_AMT] [varchar](max) NULL,
	[PBP_B20_GRP5_DRP_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP5_HMO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP5_MO_DAYS] [varchar](max) NULL,
	[PBP_B20_GRP5_OTH_DAYS] [varchar](max) NULL,
	[PBP_B20_B_MIN_COPAY] [varchar](max) NULL,
	[PBP_B20_B_COPAY_MAX] [varchar](max) NULL,
	[PBP_B20_CHEMO_COINS_MAX_PCT] [varchar](max) NULL,
	[PBP_B20_B_COINS_MIN_PCT] [varchar](max) NULL,
	[PBP_B20_B_COINS_MAX_PCT] [varchar](max) NULL,
	[PBP_B20_COINS_MC_EHC] [varchar](max) NULL,
	[PBP_B20_COPAY_MC_EHC] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB3]    Script Date: 5/18/2017 4:32:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB3](
	[QID] [varchar](max) NULL,
	[PBP_B3_MAXENR_YN] [varchar](max) NULL,
	[PBP_B3_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B3_MAXENR_PER] [varchar](max) NULL,
	[PBP_B3_COINS_YN] [varchar](max) NULL,
	[PBP_B3_COINS_EHC] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MC_MIN_CRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MC_MAX_CRS] [varchar](max) NULL,
	[PBP_B3_DED_YN] [varchar](max) NULL,
	[PBP_B3_DED_AMT] [varchar](max) NULL,
	[PBP_B3_COPAY_YN] [varchar](max) NULL,
	[PBP_B3_COPAY_EHC] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MC_MIN_CRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MC_MAX_CRS] [varchar](max) NULL,
	[PBP_B3_AUTH] [varchar](max) NULL,
	[PBP_B3_NOTES] [varchar](max) NULL,
	[PBP_B3_BENDESC_YN] [varchar](max) NULL,
	[PBP_B3_BENDESC_EHC] [varchar](max) NULL,
	[PBP_B3_BENDESC_AMO_CRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_LIM_CRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_NUMV_CRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_PER_CRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_AMO_ICRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_LIM_ICRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_PER_ICRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_NUMV_ICRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_LIM_PRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_NUMV_PRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_PER_PRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MC_MIN_ICRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MC_MAX_ICRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MC_MIN_PRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MC_MAX_PRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MIN_CRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MAX_CRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MIN_ICRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MAX_ICRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MIN_PRS] [varchar](max) NULL,
	[PBP_B3_COINS_PCT_MAX_PRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MC_MIN_ICRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MC_MAX_ICRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MC_MIN_PRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MC_MAX_PRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MIN_CRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MAX_CRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MIN_ICRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MAX_ICRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MIN_PRS] [varchar](max) NULL,
	[PBP_B3_COPAY_AMT_MAX_PRS] [varchar](max) NULL,
	[PBP_B3_BENDESC_AMO_PRS] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB4]    Script Date: 5/18/2017 4:33:01 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB4](
	[QID] [varchar](max) NULL,
	[PBP_B4A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B4A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B4A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B4A_COINS_YN] [varchar](max) NULL,
	[PBP_B4A_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B4A_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B4A_MAX_VISIT] [varchar](max) NULL,
	[PBP_B4A_COINS_WAVDIA_YN] [varchar](max) NULL,
	[PBP_B4A_COINS_WAVDIA_DH] [varchar](max) NULL,
	[PBP_B4A_COINS_WAVD_DH] [varchar](max) NULL,
	[PBP_B4A_COPAY_YN] [varchar](max) NULL,
	[PBP_B4A_COPAY_AMT_MC_MIN] [varchar](max) NULL,
	[PBP_B4A_COPAY_AMT_MC_MAX] [varchar](max) NULL,
	[PBP_B4A_COPAY_WAVDIA_YN] [varchar](max) NULL,
	[PBP_B4A_COPAY_WAVDIA_MI_DH] [varchar](max) NULL,
	[PBP_B4A_COPAY_WAVD_DH] [varchar](max) NULL,
	[PBP_B4A_DEDUCT_YN] [varchar](max) NULL,
	[PBP_B4A_NOTES] [varchar](max) NULL,
	[PBP_B4B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B4B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B4B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B4B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B4B_COINS_YN] [varchar](max) NULL,
	[PBP_B4B_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B4B_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B4B_MAX_VISIT] [varchar](max) NULL,
	[PBP_B4B_COINS_WAVDMC_YN] [varchar](max) NULL,
	[PBP_B4B_COINS_WAVD_MCDH] [varchar](max) NULL,
	[PBP_B4B_COINS_WAVDMC_DH] [varchar](max) NULL,
	[PBP_B4B_COPAY_YN] [varchar](max) NULL,
	[PBP_B4B_COPAY_AMT_MC_MIN] [varchar](max) NULL,
	[PBP_B4B_COPAY_AMT_MC_MAX] [varchar](max) NULL,
	[PBP_B4B_COPAY_WAVDIA_YN] [varchar](max) NULL,
	[PBP_B4B_COPAY_WAVD_MCDH] [varchar](max) NULL,
	[PBP_B4B_COPAY_WAVDMC_DH] [varchar](max) NULL,
	[PBP_B4B_DEDUCT_YN] [varchar](max) NULL,
	[PBP_B4B_NOTES] [varchar](max) NULL,
	[PBP_B4C_BENDESC_YN] [varchar](max) NULL,
	[PBP_B4C_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B4C_TRNS_YN] [varchar](max) NULL,
	[PBP_B4C_WWC_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B4C_WWC_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B4C_MAXENR_YN] [varchar](max) NULL,
	[PBP_B4C_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B4C_MAXENR_PER] [varchar](max) NULL,
	[PBP_B4C_COPAY_YN] [varchar](max) NULL,
	[PBP_B4C_COPAY_WAVDWW_YN] [varchar](max) NULL,
	[PBP_B4C_COINS_YN] [varchar](max) NULL,
	[PBP_B4C_COINS_WAVDWW_YN] [varchar](max) NULL,
	[PBP_B4C_DED_YN] [varchar](max) NULL,
	[PBP_B4C_DED_AMT] [varchar](max) NULL,
	[PBP_B4C_NOTES] [varchar](max) NULL,
	[PBP_B4C_WWC_MAXPLAN_SVCS_YN] [varchar](max) NULL,
	[PBP_B4C_COINS_PCT_MIN_WW] [varchar](max) NULL,
	[PBP_B4C_COINS_PCT_MAX_WW] [varchar](max) NULL,
	[PBP_B4C_COPAY_AMT_MIN_WW] [varchar](max) NULL,
	[PBP_B4C_COPAY_AMT_MAX_WW] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB5]    Script Date: 5/18/2017 4:33:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB5](
	[QID] [varchar](max) NULL,
	[PBP_B5_MAXENR_YN] [varchar](max) NULL,
	[PBP_B5_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B5_MAXENR_PER] [varchar](max) NULL,
	[PBP_B5_COINS_YN] [varchar](max) NULL,
	[PBP_B5_DED_YN] [varchar](max) NULL,
	[PBP_B5_DED_AMT] [varchar](max) NULL,
	[PBP_B5_COPAY_YN] [varchar](max) NULL,
	[PBP_B5_AUTH] [varchar](max) NULL,
	[PBP_B5_REFER_YN] [varchar](max) NULL,
	[PBP_B5_NOTES] [varchar](max) NULL,
	[PBP_B5_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B5_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B5_COPAY_MCIA_AMT_MIN] [varchar](max) NULL,
	[PBP_B5_COPAY_MCIA_AMT_MAX] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB6]    Script Date: 5/18/2017 4:33:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB6](
	[QID] [varchar](max) NULL,
	[PBP_B6_MAXENR_YN] [varchar](max) NULL,
	[PBP_B6_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B6_MAXENR_PER] [varchar](max) NULL,
	[PBP_B6_COINS_YN] [varchar](max) NULL,
	[PBP_B6_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B6_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B6_DED_YN] [varchar](max) NULL,
	[PBP_B6_DED_AMT] [varchar](max) NULL,
	[PBP_B6_COPAY_YN] [varchar](max) NULL,
	[PBP_B6_COPAY_MC_AMT_MIN] [varchar](max) NULL,
	[PBP_B6_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B6_AUTH] [varchar](max) NULL,
	[PBP_B6_REFER_YN] [varchar](max) NULL,
	[PBP_B6_NOTES] [varchar](max) NULL,
	[PBP_B6_MM_NMC_YN] [varchar](max) NULL,
	[PBP_B6_MM_SERVICES] [varchar](max) NULL,
	[PBP_B6_MM_ADDL_SERVICE_OTHER1] [varchar](max) NULL,
	[PBP_B6_MM_ADDL_SERVICE_OTHER2] [varchar](max) NULL,
	[PBP_B6_MM_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B6_MM_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B6_MM_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B6_MM_COINS_YN] [varchar](max) NULL,
	[PBP_B6_MM_COINS_SERVICES] [varchar](max) NULL,
	[PBP_B6_MM_COINS_ADDL_MIN] [varchar](max) NULL,
	[PBP_B6_MM_COINS_ADDL_MAX] [varchar](max) NULL,
	[PBP_B6_MM_COINS_PCS_MIN] [varchar](max) NULL,
	[PBP_B6_MM_COINS_PCS_MAX] [varchar](max) NULL,
	[PBP_B6_MM_COINS_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B6_MM_COINS_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B6_MM_COINS_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B6_MM_COINS_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B6_MM_REFER_YN] [varchar](max) NULL,
	[PBP_B6_MM_AUTH] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_ADDL_MAX] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_ADDL_MIN] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_PCS_MAX] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_PCS_MIN] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_YN] [varchar](max) NULL,
	[PBP_B6_MM_COPAY_SERVICES] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_YN] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_SERVICES] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_TYPE_AHC] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_NUM_AHC] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_PER_AHC] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_TYPE_PC] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_NUM_PC] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_PER_PC] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_TYPE_OTH1] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_NUM_OTH1] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_PER_OTH1] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_TYPE_OTH2] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_NUM_OTH2] [varchar](max) NULL,
	[PBP_B6_MM_LIMIT_UNIT_PER_OTH2] [varchar](max) NULL,
	[PBP_B6_MM_NOTES] [varchar](max) NULL,
	[PBP_B6_MMP_WAIVER_YN] [varchar](max) NULL,
	[PBP_B6_MMP_WAIVER_SRVC] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB7]    Script Date: 5/18/2017 4:33:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB7](
	[QID] [varchar](max) NULL,
	[PBP_B7A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7A_COINS_YN] [varchar](max) NULL,
	[PBP_B7A_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B7A_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B7A_DED_YN] [varchar](max) NULL,
	[PBP_B7A_DED_AMT] [varchar](max) NULL,
	[PBP_B7A_COPAY_YN] [varchar](max) NULL,
	[PBP_B7A_COPAY_AMT_MC_MIN] [varchar](max) NULL,
	[PBP_B7A_COPAY_AMT_MC_MAX] [varchar](max) NULL,
	[PBP_B7A_NOTES] [varchar](max) NULL,
	[PBP_B7B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B7B_BENDESC_RC] [varchar](max) NULL,
	[PBP_B7B_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B7B_BENDESC_LIM_RC] [varchar](max) NULL,
	[PBP_B7B_BENDESC_NUM_RC] [varchar](max) NULL,
	[PBP_B7B_BENDESC_PER] [varchar](max) NULL,
	[PBP_B7B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B7B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B7B_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B7B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7B_COINS_YN] [varchar](max) NULL,
	[PBP_B7B_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B7B_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B7B_COINS_PCT_CC_MIN] [varchar](max) NULL,
	[PBP_B7B_COINS_PCT_CC_MAX] [varchar](max) NULL,
	[PBP_B7B_DED_YN] [varchar](max) NULL,
	[PBP_B7B_DED_AMT] [varchar](max) NULL,
	[PBP_B7B_COPAY_YN] [varchar](max) NULL,
	[PBP_B7B_COPAY_MC_AMT_MIN] [varchar](max) NULL,
	[PBP_B7B_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B7B_COPAY_RC_AMT_MIN] [varchar](max) NULL,
	[PBP_B7B_COPAY_RC_AMT_MAX] [varchar](max) NULL,
	[PBP_B7B_AUTH] [varchar](max) NULL,
	[PBP_B7B_REFER_YN] [varchar](max) NULL,
	[PBP_B7B_NOTES] [varchar](max) NULL,
	[PBP_B7C_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7C_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7C_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7C_COINS_YN] [varchar](max) NULL,
	[PBP_B7C_DED_YN] [varchar](max) NULL,
	[PBP_B7C_DED_AMT] [varchar](max) NULL,
	[PBP_B7C_COPAY_YN] [varchar](max) NULL,
	[PBP_B7C_AUTH] [varchar](max) NULL,
	[PBP_B7C_REFER_YN] [varchar](max) NULL,
	[PBP_B7C_NOTES] [varchar](max) NULL,
	[PBP_B7D_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7D_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7D_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7D_COINS_YN] [varchar](max) NULL,
	[PBP_B7D_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B7D_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B7D_DED_YN] [varchar](max) NULL,
	[PBP_B7D_DED_AMT] [varchar](max) NULL,
	[PBP_B7D_COPAY_YN] [varchar](max) NULL,
	[PBP_B7D_COPAY_AMT_MC_MIN] [varchar](max) NULL,
	[PBP_B7D_COPAY_AMT_MC_MAX] [varchar](max) NULL,
	[PBP_B7D_AUTH] [varchar](max) NULL,
	[PBP_B7D_REFER_YN] [varchar](max) NULL,
	[PBP_B7D_NOTES] [varchar](max) NULL,
	[PBP_B7E_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7E_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7E_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7E_COINS_YN] [varchar](max) NULL,
	[PBP_B7E_DED_YN] [varchar](max) NULL,
	[PBP_B7E_DED_AMT] [varchar](max) NULL,
	[PBP_B7E_COPAY_YN] [varchar](max) NULL,
	[PBP_B7E_AUTH] [varchar](max) NULL,
	[PBP_B7E_REFER_YN] [varchar](max) NULL,
	[PBP_B7E_NOTES] [varchar](max) NULL,
	[PBP_B7F_BENDESC_YN] [varchar](max) NULL,
	[PBP_B7F_BENDESC_RF] [varchar](max) NULL,
	[PBP_B7F_BENDESC_AMO_RF] [varchar](max) NULL,
	[PBP_B7F_BENDESC_LIM_RF] [varchar](max) NULL,
	[PBP_B7F_BENDESC_AMT_RF] [varchar](max) NULL,
	[PBP_B7F_BENDESC_PER_RF] [varchar](max) NULL,
	[PBP_B7F_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B7F_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B7F_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B7F_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7F_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7F_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7F_COINS_YN] [varchar](max) NULL,
	[PBP_B7F_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B7F_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B7F_COINS_PCT_RF_MIN] [varchar](max) NULL,
	[PBP_B7F_COINS_PCT_RF_MAX] [varchar](max) NULL,
	[PBP_B7F_DED_YN] [varchar](max) NULL,
	[PBP_B7F_DED_AMT] [varchar](max) NULL,
	[PBP_B7F_COPAY_YN] [varchar](max) NULL,
	[PBP_B7F_COPAY_MC_AMT_MIN] [varchar](max) NULL,
	[PBP_B7F_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B7F_COPAY_RF_AMT_MIN] [varchar](max) NULL,
	[PBP_B7F_COPAY_RF_AMT_MAX] [varchar](max) NULL,
	[PBP_B7F_AUTH] [varchar](max) NULL,
	[PBP_B7F_REFER_YN] [varchar](max) NULL,
	[PBP_B7F_NOTES] [varchar](max) NULL,
	[PBP_B7G_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7G_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7G_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7G_COINS_YN] [varchar](max) NULL,
	[PBP_B7G_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B7G_COINS_MAX_PCT_MC] [varchar](max) NULL,
	[PBP_B7G_DED_YN] [varchar](max) NULL,
	[PBP_B7G_DED_AMT] [varchar](max) NULL,
	[PBP_B7G_COPAY_YN] [varchar](max) NULL,
	[PBP_B7G_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B7G_COPAY_MC_MAX_AMT] [varchar](max) NULL,
	[PBP_B7G_AUTH] [varchar](max) NULL,
	[PBP_B7G_REFER_YN] [varchar](max) NULL,
	[PBP_B7G_NOTES] [varchar](max) NULL,
	[PBP_B7H_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7H_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7H_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7H_COINS_YN] [varchar](max) NULL,
	[PBP_B7H_DED_YN] [varchar](max) NULL,
	[PBP_B7H_DED_AMT] [varchar](max) NULL,
	[PBP_B7H_COPAY_YN] [varchar](max) NULL,
	[PBP_B7H_AUTH] [varchar](max) NULL,
	[PBP_B7H_REFER_YN] [varchar](max) NULL,
	[PBP_B7H_NOTES] [varchar](max) NULL,
	[PBP_B7I_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7I_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7I_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7I_COINS_YN] [varchar](max) NULL,
	[PBP_B7I_DED_YN] [varchar](max) NULL,
	[PBP_B7I_DED_AMT] [varchar](max) NULL,
	[PBP_B7I_COPAY_YN] [varchar](max) NULL,
	[PBP_B7I_AUTH] [varchar](max) NULL,
	[PBP_B7I_REFER_YN] [varchar](max) NULL,
	[PBP_B7I_NOTES] [varchar](max) NULL,
	[PBP_B7E_COINS_MCIS_MINPCT] [varchar](max) NULL,
	[PBP_B7E_COINS_MCIS_MAXPCT] [varchar](max) NULL,
	[PBP_B7E_COINS_MCGS_MINPCT] [varchar](max) NULL,
	[PBP_B7E_COINS_MCGS_MAXPCT] [varchar](max) NULL,
	[PBP_B7E_COPAY_MCIS_MINAMT] [varchar](max) NULL,
	[PBP_B7E_COPAY_MCIS_MAXAMT] [varchar](max) NULL,
	[PBP_B7E_COPAY_MCGS_MINAMT] [varchar](max) NULL,
	[PBP_B7E_COPAY_MCGS_MAXAMT] [varchar](max) NULL,
	[PBP_B7H_COINS_MCIS_MINPCT] [varchar](max) NULL,
	[PBP_B7H_COINS_MCIS_MAXPCT] [varchar](max) NULL,
	[PBP_B7H_COINS_MCGS_MINPCT] [varchar](max) NULL,
	[PBP_B7H_COINS_MCGS_MAXPCT] [varchar](max) NULL,
	[PBP_B7H_COPAY_MCIS_MINAMT] [varchar](max) NULL,
	[PBP_B7H_COPAY_MCIS_MAXAMT] [varchar](max) NULL,
	[PBP_B7H_COPAY_MCGS_MINAMT] [varchar](max) NULL,
	[PBP_B7H_COPAY_MCGS_MAXAMT] [varchar](max) NULL,
	[PBP_B7B_COINS_EHC] [varchar](max) NULL,
	[PBP_B7B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B7E_COINS_EHC] [varchar](max) NULL,
	[PBP_B7E_COPAY_EHC] [varchar](max) NULL,
	[PBP_B7F_COINS_EHC] [varchar](max) NULL,
	[PBP_B7F_COPAY_EHC] [varchar](max) NULL,
	[PBP_B7H_COINS_EHC] [varchar](max) NULL,
	[PBP_B7H_COPAY_EHC] [varchar](max) NULL,
	[PBP_B7C_MM_ADDL_SERVICE_OTHER1] [varchar](max) NULL,
	[PBP_B7C_MM_AUTH] [varchar](max) NULL,
	[PBP_B7C_MM_COINS_YN] [varchar](max) NULL,
	[PBP_B7C_MM_COPAY_YN] [varchar](max) NULL,
	[PBP_B7C_MM_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B7C_MM_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B7C_MM_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B7C_MM_NMC_YN] [varchar](max) NULL,
	[PBP_B7C_MM_REFER_YN] [varchar](max) NULL,
	[PBP_B7I_MM_REFER_YN] [varchar](max) NULL,
	[PBP_B7I_MM_AUTH] [varchar](max) NULL,
	[PBP_B7I_MM_NMC_YN] [varchar](max) NULL,
	[PBP_B7I_MM_SERVICES] [varchar](max) NULL,
	[PBP_B7I_MM_ADDL_SERVICE_OTHER1] [varchar](max) NULL,
	[PBP_B7I_MM_ADDL_SERVICE_OTHER2] [varchar](max) NULL,
	[PBP_B7I_MM_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B7I_MM_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B7I_MM_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B7I_MM_COINS_SERVICES] [varchar](max) NULL,
	[PBP_B7I_MM_COINS_YN] [varchar](max) NULL,
	[PBP_B7I_MM_COPAY_YN] [varchar](max) NULL,
	[PBP_B7I_MM_COPAY_SERVICES] [varchar](max) NULL,
	[PBP_B7I_MM_COPAY_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B7I_MM_COPAY_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B7I_MM_COPAY_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B7I_MM_COPAY_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B7I_MM_COINS_OTHER1_MAX] [varchar](max) NULL,
	[PBP_B7I_MM_COINS_OTHER1_MIN] [varchar](max) NULL,
	[PBP_B7I_MM_COINS_OTHER2_MAX] [varchar](max) NULL,
	[PBP_B7I_MM_COINS_OTHER2_MIN] [varchar](max) NULL,
	[PBP_B7C_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B7C_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B7C_COPAY_MC_AMT_MIN] [varchar](max) NULL,
	[PBP_B7C_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B7I_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B7I_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B7I_COPAY_MC_AMT_MIN] [varchar](max) NULL,
	[PBP_B7I_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B7I_MM_NOTES] [varchar](max) NULL,
	[PBP_B7C_MM_NOTES] [varchar](max) NULL,
	[PBP_B7C_MM_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B7C_MM_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B7C_MM_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B7C_MM_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B7B_COMBINED_BEN] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB8]    Script Date: 5/18/2017 4:34:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB8](
	[QID] [varchar](max) NULL,
	[PBP_B8A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B8A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B8A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B8A_COINS_YN] [varchar](max) NULL,
	[PBP_B8A_COINS_PCT_DMC] [varchar](max) NULL,
	[PBP_B8A_COINS_PCT_DMC_MAX] [varchar](max) NULL,
	[PBP_B8A_COINS_PCT_LAB] [varchar](max) NULL,
	[PBP_B8A_COINS_PCT_LAB_MAX] [varchar](max) NULL,
	[PBP_B8A_COPAY_MAX_YN] [varchar](max) NULL,
	[PBP_B8A_DED_YN] [varchar](max) NULL,
	[PBP_B8A_DED_AMT] [varchar](max) NULL,
	[PBP_B8A_COPAY_YN] [varchar](max) NULL,
	[PBP_B8A_COPAY_MIN_DMC_AMT] [varchar](max) NULL,
	[PBP_B8A_COPAY_MAX_DMC_AMT] [varchar](max) NULL,
	[PBP_B8A_LAB_COPAY_AMT] [varchar](max) NULL,
	[PBP_B8A_LAB_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B8A_AUTH] [varchar](max) NULL,
	[PBP_B8A_REFER_YN] [varchar](max) NULL,
	[PBP_B8B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B8B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B8B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B8B_COINS_YN] [varchar](max) NULL,
	[PBP_B8B_COINS_PCT_CMC] [varchar](max) NULL,
	[PBP_B8B_COINS_PCT_CMC_MAX] [varchar](max) NULL,
	[PBP_B8B_COINS_PCT_TMC] [varchar](max) NULL,
	[PBP_B8B_COINS_PCT_TMC_MAX] [varchar](max) NULL,
	[PBP_B8B_COINS_PCT_DRS] [varchar](max) NULL,
	[PBP_B8B_COINS_PCT_DRS_MAX] [varchar](max) NULL,
	[PBP_B8B_DED_YN] [varchar](max) NULL,
	[PBP_B8B_DED_AMT] [varchar](max) NULL,
	[PBP_B8B_COPAY_YN] [varchar](max) NULL,
	[PBP_B8B_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B8B_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B8B_AUTH] [varchar](max) NULL,
	[PBP_B8B_REFER_YN] [varchar](max) NULL,
	[PBP_B8B_COPAY_AMT_TMC] [varchar](max) NULL,
	[PBP_B8B_COPAY_AMT_TMC_MAX] [varchar](max) NULL,
	[PBP_B8B_COPAY_AMT_DRS] [varchar](max) NULL,
	[PBP_B8B_COPAY_AMT_DRS_MAX] [varchar](max) NULL,
	[PBP_B8B_COPAY_MAX_YN] [varchar](max) NULL,
	[PBP_B8A_COINS_EHC] [varchar](max) NULL,
	[PBP_B8A_COPAY_EHC] [varchar](max) NULL,
	[PBP_B8B_COINS_EHC] [varchar](max) NULL,
	[PBP_B8B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B8A_DMC_NOTES] [varchar](max) NULL,
	[PBP_B8A_LAB_NOTES] [varchar](max) NULL,
	[PBP_B8B_CMC_NOTES] [varchar](max) NULL,
	[PBP_B8B_DRS_NOTES] [varchar](max) NULL,
	[PBP_B8B_TMC_NOTES] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPB9]    Script Date: 5/18/2017 4:34:38 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPB9](
	[QID] [varchar](max) NULL,
	[PBP_B9A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B9A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B9A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B9A_COINS_YN] [varchar](max) NULL,
	[PBP_B9A_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B9A_COINS_PCT_MCMAX] [varchar](max) NULL,
	[PBP_B9A_DED_YN] [varchar](max) NULL,
	[PBP_B9A_DED_AMT] [varchar](max) NULL,
	[PBP_B9A_COPAY_YN] [varchar](max) NULL,
	[PBP_B9A_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B9A_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B9A_AUTH] [varchar](max) NULL,
	[PBP_B9A_REFER_YN] [varchar](max) NULL,
	[PBP_B9A_NOTES] [varchar](max) NULL,
	[PBP_B9B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B9B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B9B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B9B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B9B_COINS_YN] [varchar](max) NULL,
	[PBP_B9B_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B9B_COINS_PCT_MCMAX] [varchar](max) NULL,
	[PBP_B9B_DED_YN] [varchar](max) NULL,
	[PBP_B9B_DED_AMT] [varchar](max) NULL,
	[PBP_B9B_COPAY_YN] [varchar](max) NULL,
	[PBP_B9B_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B9B_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B9B_AUTH] [varchar](max) NULL,
	[PBP_B9B_REFER_YN] [varchar](max) NULL,
	[PBP_B9B_NOTES] [varchar](max) NULL,
	[PBP_B9C_MAXENR_YN] [varchar](max) NULL,
	[PBP_B9C_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B9C_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B9C_MAXENR_PER] [varchar](max) NULL,
	[PBP_B9C_COINS_YN] [varchar](max) NULL,
	[PBP_B9C_DED_YN] [varchar](max) NULL,
	[PBP_B9C_DED_AMT] [varchar](max) NULL,
	[PBP_B9C_COPAY_YN] [varchar](max) NULL,
	[PBP_B9C_AUTH] [varchar](max) NULL,
	[PBP_B9C_REFER_YN] [varchar](max) NULL,
	[PBP_B9C_NOTES] [varchar](max) NULL,
	[PBP_B9D_BENDESC_YN] [varchar](max) NULL,
	[PBP_B9D_BENDEC] [varchar](max) NULL,
	[PBP_B9D_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B9D_MAXENR_YN] [varchar](max) NULL,
	[PBP_B9D_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B9D_MAXENR_PER] [varchar](max) NULL,
	[PBP_B9D_COINS_YN] [varchar](max) NULL,
	[PBP_B9D_DED_YN] [varchar](max) NULL,
	[PBP_B9D_DED_AMT] [varchar](max) NULL,
	[PBP_B9D_COPAY_YN] [varchar](max) NULL,
	[PBP_B9D_AUTH] [varchar](max) NULL,
	[PBP_B9D_REFER_YN] [varchar](max) NULL,
	[PBP_B9D_NOTES] [varchar](max) NULL,
	[PBP_B9C_COINS_MCIS_MINPCT] [varchar](max) NULL,
	[PBP_B9C_COINS_MCIS_MAXPCT] [varchar](max) NULL,
	[PBP_B9C_COINS_MCGS_MINPCT] [varchar](max) NULL,
	[PBP_B9C_COINS_MCGS_MAXPCT] [varchar](max) NULL,
	[PBP_B9C_COPAY_MCIS_MINAMT] [varchar](max) NULL,
	[PBP_B9C_COPAY_MCIS_MAXAMT] [varchar](max) NULL,
	[PBP_B9C_COPAY_MCGS_MINAMT] [varchar](max) NULL,
	[PBP_B9C_COPAY_MCGS_MAXAMT] [varchar](max) NULL,
	[PBP_B9C_COINS_EHC] [varchar](max) NULL,
	[PBP_B9C_COPAY_EHC] [varchar](max) NULL,
	[PBP_B9D_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B9D_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B9D_COPAY_MC_AMT_MIN] [varchar](max) NULL,
	[PBP_B9D_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPC]    Script Date: 5/18/2017 4:34:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPC](
	[QID] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_GROUP_NUM] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_GROUP_NUM] [varchar](max) NULL,
	[PBP_C_OON_YN] [varchar](max) NULL,
	[PBP_C_OON_NOTES] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHS_YN] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHS_BEN_TYPE] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_PCT] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_PCT_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_BGND_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_ENDD_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_PCT_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_BGND_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_ENDD_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_PCT_I3] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_BGND_I3] [varchar](max) NULL,
	[PBP_C_OON_COINS_IHA_ENDD_I3] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_PCT] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_PCT_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_BGND_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_ENDD_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_PCT_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_BGND_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_ENDD_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_PCT_I3] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_BGND_I3] [varchar](max) NULL,
	[PBP_C_OON_COINS_IPH_ENDD_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHS_YN] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHS_BEN_TYPE] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_PS_AMT] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_AMT_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_BGND_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_ENDD_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_AMT_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_BGND_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_ENDD_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_AMT_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_BGND_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IHA_ENDD_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_PS_AMT] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_AMT_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_BGND_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_ENDD_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_AMT_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_BGND_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_ENDD_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_AMT_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_BGND_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_IPH_ENDD_I3] [varchar](max) NULL,
	[PBP_C_OON_IHS_DED_YN] [varchar](max) NULL,
	[PBP_C_OON_IHS_DED_COST_TYPE] [varchar](max) NULL,
	[PBP_C_OON_IHS_DED_ACU_AMT] [varchar](max) NULL,
	[PBP_C_OON_IHS_DED_PSYC_AMT] [varchar](max) NULL,
	[PBP_C_OON_IHS_DED_COMB_AMT] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_YN] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_PCT] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_PCT_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_BGND_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_ENDD_I1] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_PCT_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_BGND_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_ENDD_I2] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_PCT_I3] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_BGND_I3] [varchar](max) NULL,
	[PBP_C_OON_COINS_SNF_ENDD_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_YN] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_AMT] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_AMT_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_BGND_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_ENDD_I1] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_AMT_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_BGND_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_ENDD_I2] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_AMT_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_BGND_I3] [varchar](max) NULL,
	[PBP_C_OON_COPAY_SNF_ENDD_I3] [varchar](max) NULL,
	[PBP_C_OON_SNF_DED_YN] [varchar](max) NULL,
	[PBP_C_OON_SNF_DED_AMT] [varchar](max) NULL,
	[PBP_C_POS_YN] [varchar](max) NULL,
	[PBP_C_POS_BENDESC_AMO] [varchar](max) NULL,
	[PBP_C_POS_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_C_POS_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_C_POS_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_C_POS_MAXENR_OOPC_YN] [varchar](max) NULL,
	[PBP_C_POS_MAXENR_OOPC_AMT] [varchar](max) NULL,
	[PBP_C_POS_DED_YN] [varchar](max) NULL,
	[PBP_C_POS_DED_AMT] [varchar](max) NULL,
	[PBP_C_POS_MAXENR_OOPC_PER] [varchar](max) NULL,
	[PBP_C_POS_AUTH_YN] [varchar](max) NULL,
	[PBP_C_POS_AUTH] [varchar](max) NULL,
	[PBP_C_POS_REFERRAL_YN] [varchar](max) NULL,
	[PBP_C_POS_NOTES] [varchar](max) NULL,
	[PBP_C_POS_IHS_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_C_POS_IHS_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_C_POS_IHS_MAXPLAN_TYPE] [varchar](max) NULL,
	[PBP_C_POS_IHS_MAXPLAN_ACU_AMT] [varchar](max) NULL,
	[PBP_C_POS_IHS_MAXPLAN_PSYC_AMT] [varchar](max) NULL,
	[PBP_C_POS_IHS_MAXPLAN_COMB_AMT] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHS_YN] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHS_BEN_TYPE] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_PCT] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_PCT_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_BGND_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_ENDD_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_PCT_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_BGND_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_ENDD_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_PCT_I3] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_BGND_I3] [varchar](max) NULL,
	[PBP_C_POS_COINS_IHA_ENDD_I3] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_PCT] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_PCT_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_BGND_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_ENDD_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_PCT_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_BGND_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_ENDD_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_PCT_I3] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_BGND_I3] [varchar](max) NULL,
	[PBP_C_POS_COINS_IPH_ENDD_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHS_YN] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHS_BEN_TYPE] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_PS_AMT] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_AMT_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_BGND_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_ENDD_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_AMT_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_BGND_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_ENDD_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_AMT_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_BGND_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IHA_ENDD_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_PS_AMT] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_AMT_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_BGND_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_ENDD_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_AMT_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_BGND_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_ENDD_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_AMT_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_BGND_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_IPH_ENDD_I3] [varchar](max) NULL,
	[PBP_C_POS_IHS_DED_YN] [varchar](max) NULL,
	[PBP_C_POS_IHS_DED_TYPE] [varchar](max) NULL,
	[PBP_C_POS_IHA_DED_AMT] [varchar](max) NULL,
	[PBP_C_POS_IPH_DED_AMT] [varchar](max) NULL,
	[PBP_C_POS_COMB_DED_AMT] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_YN] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_PCT] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_PCT_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_BGND_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_ENDD_I1] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_PCT_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_BGND_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_ENDD_I2] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_PCT_I3] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_BGND_I3] [varchar](max) NULL,
	[PBP_C_POS_COINS_SNF_ENDD_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_YN] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_MC_COST_YN] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_AMT] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_INTRVL_NUM] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_AMT_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_BGND_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_ENDD_I1] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_AMT_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_BGND_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_ENDD_I2] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_AMT_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_BGND_I3] [varchar](max) NULL,
	[PBP_C_POS_COPAY_SNF_ENDD_I3] [varchar](max) NULL,
	[PBP_C_POS_SNF_DED_YN] [varchar](max) NULL,
	[PBP_C_POS_SNF_DED_AMT] [varchar](max) NULL,
	[PBP_C_VT_US_YN] [varchar](max) NULL,
	[PBP_C_VT_US_MO] [varchar](max) NULL,
	[PBP_C_OON_MC_BENDESC_CATS] [varchar](max) NULL,
	[PBP_C_OON_NMC_BENDESC_CATS] [varchar](max) NULL,
	[PBP_C_POS_MC_BENDESC_SUBCATS] [varchar](max) NULL,
	[PBP_C_POS_NMC_BENDESC_SUBCATS] [varchar](max) NULL,
	[PBP_C_POS_MAXPLAN_MC_SUBCATS] [varchar](max) NULL,
	[PBP_C_POS_MAXPLAN_NMC_SUBCATS] [varchar](max) NULL,
	[PBP_C_POS_MAXPLAN_BENS] [varchar](max) NULL,
	[PBP_C_POS_BENDESC_BENS] [varchar](max) NULL,
	[PBP_C_POS_AUTH_MC_SUBCATS] [varchar](max) NULL,
	[PBP_C_POS_AUTH_NMC_SUBCATS] [varchar](max) NULL,
	[PBP_C_POS_AUTH_BENS] [varchar](max) NULL,
	[PBP_C_OON_BENDESC_BENS] [varchar](max) NULL,
	[PBP_C_POS_REFER_MC_SUBCATS] [varchar](max) NULL,
	[PBP_C_POS_REFER_NMC_SUBCATS] [varchar](max) NULL,
	[PBP_C_POS_REFER_BENEFIT_BENS] [varchar](max) NULL,
	[PBP_C_POS_STATE_LIMIT_YN] [varchar](max) NULL,
	[PBP_C_POS_TERRITORIES_YN] [varchar](max) NULL,
	[PBP_C_VT_GEOGRAPHIC_AREA] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPC_OON]    Script Date: 5/18/2017 4:35:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPC_OON](
	[QID] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_GROUP_NUM_ID] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_GRP_LBL] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_COINS_YN] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_COINS_MIN_PCT] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_COINS_MAX_PCT] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_COPAY_YN] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_COPAY_MIN_AMT] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_COPAY_MAX_AMT] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_DED_YN] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_DED_AMT] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_C_OON_OUT_MC_BENDESC_CATS] [varchar](max) NULL,
	[PBP_C_OON_OUT_NMC_BENDESC_CATS] [varchar](max) NULL,
	[PBP_C_OON_OUTPT_BENDESC_BEN] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPC_POS]    Script Date: 5/18/2017 4:35:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPC_POS](
	[QID] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_GROUP_NUM_ID] [varchar](max) NULL,
	[PBP_C_POS_GROUP_LBL] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_COINS_YN] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_COINS_MIN_PCT] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_COINS_MAX_PCT] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_COPAY_YN] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_COPAY_MIN_AMT] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_COPAY_MAX_AMT] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_DEDUCT_YN] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_DEDUCT_AMT] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_MC_BENCATS] [varchar](max) NULL,
	[PBP_C_POS_OUTPT_NMC_BENCATS] [varchar](max) NULL,
	[PBP_C_POS_OUPT_BENCAT_BENS] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPD]    Script Date: 5/18/2017 4:35:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPD](
	[QID] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_PARTB_YN] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_INN_NM_YN] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_INN_M_YN] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_YN] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_AMT] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_INN_M_CATS] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_INN_NM_CATS] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_BENS] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_OON_NM_YN] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_OON_M_CATS] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_OON_M_YN] [varchar](max) NULL,
	[PBP_D_COMB_DEDUCT_OON_NM_CATS] [varchar](max) NULL,
	[PBP_D_INN_DEDUCT_M_YN] [varchar](max) NULL,
	[PBP_D_INN_DEDUCT_NM_YN] [varchar](max) NULL,
	[PBP_D_INN_DEDUCT_PARTB_YN] [varchar](max) NULL,
	[PBP_D_INN_DEDUCT_YN] [varchar](max) NULL,
	[PBP_D_INN_DEDUCT_AMT] [varchar](max) NULL,
	[PBP_D_INN_DEDUCT_M_CATS] [varchar](max) NULL,
	[PBP_D_INN_DEDUCT_NM_CATS] [varchar](max) NULL,
	[PBP_D_INN_DEDUCT_BENS] [varchar](max) NULL,
	[PBP_D_OON_DEDUCT_YN] [varchar](max) NULL,
	[PBP_D_OON_DEDUCT_NM_YN] [varchar](max) NULL,
	[PBP_D_OON_DEDUCT_M_YN] [varchar](max) NULL,
	[PBP_D_OON_DEDUCT_PARTB_YN] [varchar](max) NULL,
	[PBP_D_OON_DEDUCT_AMT] [varchar](max) NULL,
	[PBP_D_OON_DEDUCT_M_CATS] [varchar](max) NULL,
	[PBP_D_OON_DEDUCT_NM_CATS] [varchar](max) NULL,
	[PBP_D_OON_DEDUCT_BENS] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_INN_NM_YN] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_INN_M_YN] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_AMT_TYPE] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_AMT] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_OOPC_BENS] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_OON_M_YN] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_OON_NM_YN] [varchar](max) NULL,
	[PBP_D_INN_MAX_ENR_M_YN] [varchar](max) NULL,
	[PBP_D_INN_MAX_ENR_NM_YN] [varchar](max) NULL,
	[PBP_D_OUT_POCKET_AMT_TYPE] [varchar](max) NULL,
	[PBP_D_OUT_POCKET_AMT] [varchar](max) NULL,
	[PBP_D_INN_MAX_ENR_OOPC_BENS] [varchar](max) NULL,
	[PBP_D_OON_MAX_ENR_OOPC_TYPE] [varchar](max) NULL,
	[PBP_D_OON_MAX_ENR_M_YN] [varchar](max) NULL,
	[PBP_D_OON_MAX_ENR_NM_YN] [varchar](max) NULL,
	[PBP_D_OON_MAX_ENR_OOPC_AMT] [varchar](max) NULL,
	[PBP_D_OON_MAX_ENR_OOPC_BENS] [varchar](max) NULL,
	[PBP_D_OON_MAXPLAN_NM_YN] [varchar](max) NULL,
	[PBP_D_INN_MAXPLAN_NM_YN] [varchar](max) NULL,
	[PBP_D_MAX_PLAN_BEN_COV_YN] [varchar](max) NULL,
	[PBP_D_MAX_PLAN_BEN_COV] [varchar](max) NULL,
	[PBP_D_MAX_PLAN_PERIOD] [varchar](max) NULL,
	[PBP_D_MAX_PLAN_BENS] [varchar](max) NULL,
	[PBP_D_INN_MAXPLAN_NM_CATS] [varchar](max) NULL,
	[PBP_D_OON_MAXPLAN_NM_CATS] [varchar](max) NULL,
	[PBP_D_MPLUSC_PREMIUM] [varchar](max) NULL,
	[PBP_D_MPLUSC_BONLY_PREMIUM] [varchar](max) NULL,
	[PBP_D_MCO_PAY_REDUCT_YN] [varchar](max) NULL,
	[PBP_D_MCO_PAY_REDUCT_AMT] [varchar](max) NULL,
	[PBP_D_BALBILL_YN] [varchar](max) NULL,
	[PBP_D_BALBILL_MIN_PCT] [varchar](max) NULL,
	[PBP_D_BALBILL_MAX_PCT] [varchar](max) NULL,
	[PBP_D_BALBILL_PROV_CATS] [varchar](max) NULL,
	[PBP_D_MSA_DED_AMT] [varchar](max) NULL,
	[PBP_D_MSA_ANNUAL_AMT] [varchar](max) NULL,
	[PBP_D_NOTES] [varchar](max) NULL,
	[PBP_D_NOTES2] [varchar](max) NULL,
	[PBP_D_NON_DEDUCT_YN] [varchar](max) NULL,
	[PBP_D_NON_DEDUCT_PARTB_YN] [varchar](max) NULL,
	[PBP_D_NON_DEDUCT_AMT] [varchar](max) NULL,
	[PBP_D_NON_DEDUCT_BENS] [varchar](max) NULL,
	[PBP_D_NON_DEDUCT_M_YN] [varchar](max) NULL,
	[PBP_D_NON_DEDUCT_M_CATS] [varchar](max) NULL,
	[PBP_D_NON_DEDUCT_NM_YN] [varchar](max) NULL,
	[PBP_D_NON_DEDUCT_NM_CATS] [varchar](max) NULL,
	[PBP_D_NON_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_D_NON_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_D_NON_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_D_NON_MAXPLAN_NM_YN] [varchar](max) NULL,
	[PBP_D_NON_MAXPLAN_NM_CATS] [varchar](max) NULL,
	[PBP_D_MAXENR_OOPC_AMT] [varchar](max) NULL,
	[PBP_D_MAXENR_OOPC_BENS] [varchar](max) NULL,
	[PBP_D_MAXENR_OOPC_M_CATS] [varchar](max) NULL,
	[PBP_D_MAXENR_OOPC_M_CATS_YN] [varchar](max) NULL,
	[PBP_D_MAXENR_OOPC_NM_CATS] [varchar](max) NULL,
	[PBP_D_MAXENR_OOPC_NM_CATS_YN] [varchar](max) NULL,
	[PBP_D_MAXENR_OOPC_TYPE] [varchar](max) NULL,
	[PBP_D_INN_MAX_ENR_NM_CAT_EX] [varchar](max) NULL,
	[PBP_D_INN_MAX_ENR_M_CAT_EX] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_INN_NMCATEX] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_INN_MCAT_EX] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_OON_NMCATEX] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_OON_MCAT_EX] [varchar](max) NULL,
	[PBP_D_OON_MAX_ENR_NM_CAT_EX] [varchar](max) NULL,
	[PBP_D_OON_MAX_ENR_M_CAT_EX] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_14B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_13A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_9C_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_8A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_13B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_10B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_11B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7I_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_10A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_12_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_9B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7F_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_11A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_8B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7G_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_13C_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_11C_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_13D_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_9A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_9D_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7E_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7H_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_14D_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_17A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_17B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_16A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_15_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_14C_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_14E_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_16B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_18A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_18B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7A_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7C_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7B_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_3_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_YN] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_1A_T1_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_1A_T2_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_1A_T3_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_5_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_CATS] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_1B_T1_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_1B_T2_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_1B_T3_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_6_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_7D_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_13E_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_13F_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_13G_AMT] [varchar](max) NULL,
	[PBP_D_OUT_POCKET_AMT_YN] [varchar](max) NULL,
	[PBP_D_OON_MAX_ENR_OOPC_YN] [varchar](max) NULL,
	[PBP_D_COMB_MAX_ENR_AMT_YN] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_YN] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_AMT_TYPE] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_AMT] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_COMB_TYPE] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_14A_YN] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_BENS] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_INN_MC_YN] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_INN_MC_CATS] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_INN_NMC_YN] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_OON_NMC_CATS] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_OON_NMC_YN] [varchar](max) NULL,
	[PBP_D_ANN_DEDUCT_INN_NMC_CATS] [varchar](max) NULL,
	[PBP_D_MMP_MEDCAID_BENDESC_CATS] [varchar](max) NULL,
	[PBP_D_MMP_PLANCOV_BENDESC_CATS] [varchar](max) NULL,
	[PBP_D_MMP_NM_YN] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_2_T1_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_2_T2_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_2_T3_AMT] [varchar](max) NULL,
	[PBP_D_DIFF_DEDUCT_4C_AMT] [varchar](max) NULL,
	[PBP_D_NMC_DEDUCT_YN] [varchar](max) NULL,
	[PBP_D_NMC_DEDUCT_CATS] [varchar](max) NULL,
	[PBP_D_DEDUCT_IHA_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_IHP_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_SNF_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_CPRS_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_WEUC_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_CHIRO_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_POD_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_OBS_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_TRANS_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_ACUPUNC_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_OTC_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_MEAL_AMT] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPD_1]    Script Date: 5/18/2017 4:36:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPD_1](
	[QID] [varchar](max) NULL,
	[PBP_D_DEDUCT_DSNP_HIS_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_OTHER1_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_OTHER2_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_OTHER3_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_PREVDENTAL_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_APE_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_14C_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_COMPDENTAL_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_EYEEXAM_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_EYEWEAR_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_HEAREXAM_AMT] [varchar](max) NULL,
	[PBP_D_DEDUCT_HEARAID_AMT] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPD_OON]    Script Date: 5/18/2017 4:36:21 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPD_OON](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_OON_IDENTIFIER] [varchar](max) NULL,
	[PBP_D_OPT_OON_TYPE_ID] [varchar](max) NULL,
	[PBP_D_OPT_OON_CAT_ID] [varchar](max) NULL,
	[PBP_D_OPT_OON_COINS_YN] [varchar](max) NULL,
	[PBP_D_OPT_OON_COINS_MIN_PCT] [varchar](max) NULL,
	[PBP_D_OPT_OON_COINS_MAX_PCT] [varchar](max) NULL,
	[PBP_D_OPT_OON_COPAY_YN] [varchar](max) NULL,
	[PBP_D_OPT_OON_CSTSHRS_YN] [varchar](max) NULL,
	[PBP_D_OPT_OON_YN] [varchar](max) NULL,
	[PBP_D_OPT_OON_COPAY_MIN_AMT] [varchar](max) NULL,
	[PBP_D_OPT_OON_COPAY_MAX_AMT] [varchar](max) NULL,
	[PBP_D_OPT_OON_NOTES] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPD_OPT]    Script Date: 5/18/2017 4:36:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPD_OPT](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[PBP_D_OPT_MAXPLAN_BEN_COV_YN] [varchar](max) NULL,
	[PBP_D_OPT_MAXPLAN_BEN_COV_AMT] [varchar](max) NULL,
	[PBP_D_OPT_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_D_OPT_DESCRIPTION] [varchar](max) NULL,
	[PBP_D_AMT_OPT_PREMIUM] [varchar](max) NULL,
	[PBP_D_OPT_OTHER_BENEFITS] [varchar](max) NULL,
	[PBP_D_OPT_SECB_CATS] [varchar](max) NULL,
	[PBP_D_OPT_NOTES] [varchar](max) NULL,
	[PBP_D_OPT_DEDUCT_YN] [varchar](max) NULL,
	[PBP_D_OPT_DEDUCT_AMT] [varchar](max) NULL,
	[PBP_D_OPT_DEDUCT_CATS] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPMRX]    Script Date: 5/18/2017 4:37:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPMRX](
	[QID] [varchar](max) NULL,
	[MRX_DRUG_BEN_YN] [varchar](max) NULL,
	[MRX_BENEFIT_TYPE] [varchar](max) NULL,
	[MRX_FORMULARY_TIERS_NUM] [varchar](max) NULL,
	[MRX_PARTD_NETWORK_LOC] [varchar](max) NULL,
	[MRX_QUANTITY_LIMITS] [varchar](max) NULL,
	[MRX_AUTH_YNBA] [varchar](max) NULL,
	[MRX_STEP_THER_DRUGS_YN] [varchar](max) NULL,
	[MRX_FIRST_FILL] [varchar](max) NULL,
	[MRX_OTC_MEDS_PAY_UMP_YN] [varchar](max) NULL,
	[MRX_AE_OON_CSTSHR_STRUCT] [varchar](max) NULL,
	[MRX_AE_CSTSHR_APPLY] [varchar](max) NULL,
	[MRX_AE_OOPTCS_APPLY] [varchar](max) NULL,
	[MRX_ALT_DED_CHARGE] [varchar](max) NULL,
	[MRX_ALT_DED_AMOUNT] [varchar](max) NULL,
	[MRX_ALT_DED_TYPE_YN] [varchar](max) NULL,
	[MRX_ALT_NO_DED_TIER] [varchar](max) NULL,
	[MRX_ALT_DED_TIER_CSTSHR_YN] [varchar](max) NULL,
	[MRX_ALT_GEN_CSTSHR_STRUCT] [varchar](max) NULL,
	[MRX_ALT_OON_CSTSHR_STRUCT] [varchar](max) NULL,
	[MRX_ALT_GEN_COINS_PCT] [varchar](max) NULL,
	[MRX_ALT_GEN_COPAY_AMT] [varchar](max) NULL,
	[MRX_ALT_EXCL_DRUGS_YN] [varchar](max) NULL,
	[MRX_ALT_RED_COST_SHARING] [varchar](max) NULL,
	[MRX_ALT_RED_COST_SHARING_ITEMS] [varchar](max) NULL,
	[MRX_ALT_PRE_ICL_COST_SHARE] [varchar](max) NULL,
	[MRX_ALT_COV_LMT_YN] [varchar](max) NULL,
	[MRX_ALT_COV_LMT_AMT] [varchar](max) NULL,
	[MRX_ALT_GAP_COVG_YN] [varchar](max) NULL,
	[MRX_ALT_CSTSHR_POST_OOPT] [varchar](max) NULL,
	[MRX_ALT_CSTSHR_POST_OOPT_MMP] [varchar](max) NULL,
	[MRX_GEN_LOC_OON] [varchar](max) NULL,
	[MRX_GEN_LOC_LTC] [varchar](max) NULL,
	[MRX_GEN_RSTD_1M] [varchar](max) NULL,
	[MRX_GEN_RSTD_2M] [varchar](max) NULL,
	[MRX_GEN_RSTD_3M] [varchar](max) NULL,
	[MRX_GEN_OON_1M] [varchar](max) NULL,
	[MRX_GEN_OON_OS] [varchar](max) NULL,
	[MRX_GEN_MOSTD_1M] [varchar](max) NULL,
	[MRX_GEN_MOSTD_2M] [varchar](max) NULL,
	[MRX_GEN_MOSTD_3M] [varchar](max) NULL,
	[MRX_GEN_LTC_1M] [varchar](max) NULL,
	[MRX_TIER_EXTD_DAYS_GEN_YN] [varchar](max) NULL,
	[MRX_GEN_EXTD_FIRST_FILL] [varchar](max) NULL,
	[MRX_NOTES] [varchar](max) NULL,
	[MRX_ALT_GAP_COVG_TIER] [varchar](max) NULL,
	[MRX_OTC_MEDS_ATTEST_FLAG] [varchar](max) NULL,
	[MRX_OTC_STEP_THERAPY_YN] [varchar](max) NULL,
	[MRX_ALT_PRE_ICL_EXCLUD_ONLY_YN] [varchar](max) NULL,
	[MRX_POST_EXCLUD_ONLY_YN] [varchar](max) NULL,
	[MRX_FLOOR_PRICE_YN] [varchar](max) NULL,
	[MRX_TIER_FORM_EX] [varchar](max) NULL,
	[MRX_TIER_FORM_EX_2_YN] [varchar](max) NULL,
	[MRX_TIER_FORM_EX_2] [varchar](max) NULL,
	[MRX_FORM_MODEL_TYPE] [varchar](max) NULL,
	[MRX_FORM_MODEL_DESC] [varchar](max) NULL,
	[MRX_LTC_ATTEST_FLAG] [varchar](max) NULL,
	[MRX_ALT_PRE_ICL_COST_SHARE_MMP] [varchar](max) NULL,
	[MRX_ALT_PRE_ICL_LIS_MMP_YN] [varchar](max) NULL,
	[MRX_ALT_PRE_ICL_LIS_MMP_TIERS] [varchar](max) NULL,
	[MRX_CEILING_PRICE_YN] [varchar](max) NULL,
	[MRX_AVG_EXP_COST_SHARE_ATTEST] [varchar](max) NULL,
	[MRX_SNP_ZEROCOST_ATTEST] [varchar](max) NULL,
	[MRX_GEN_LOC_RSTD] [varchar](max) NULL,
	[MRX_GEN_LOC_MOSTD] [varchar](max) NULL,
	[MRX_REDUCT_COST_YN] [varchar](max) NULL,
	[MRX_TIER_GROUP_NUM] [varchar](max) NULL,
	[MRX_V_BID_ATTESTATION] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPMRX_P]    Script Date: 5/18/2017 4:37:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPMRX_P](
	[QID] [varchar](max) NULL,
	[MRX_TIER_POST_BENEFIT_TYPE] [varchar](max) NULL,
	[MRX_TIER_POST_ID] [varchar](max) NULL,
	[MRX_TIER_POST_TYPE_ID] [varchar](max) NULL,
	[MRX_TIER_POST_DRUG_TYPE] [varchar](max) NULL,
	[MRX_TIER_POST_INCLUDES] [varchar](max) NULL,
	[MRX_TIER_POST_COST_STRUCT] [varchar](max) NULL,
	[MRX_TIER_POST_COINS_PCT] [varchar](max) NULL,
	[MRX_TIER_POST_COPAY_AMT] [varchar](max) NULL,
	[MRX_TIER_POST_LABEL_LIST] [varchar](max) NULL,
	[MRX_TIER_POST_FORM_MODEL_DESC] [varchar](max) NULL,
	[MRX_TIER_POST_INCLUDES_MMP] [varchar](max) NULL,
	[MRX_TIER_POST_COPAY_AMT_MMPMIN] [varchar](max) NULL,
	[MRX_TIER_POST_COPAY_AMT_MMPMAX] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPMRX_T]    Script Date: 5/18/2017 4:37:46 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PBPMRX_T](
	[QID] [varchar](max) NULL,
	[MRX_TIER_BENEFIT_TYPE] [varchar](max) NULL,
	[MRX_TIER_TYPE_ID] [varchar](max) NULL,
	[MRX_TIER_ID] [varchar](max) NULL,
	[MRX_TIER_DRUG_TYPE] [varchar](max) NULL,
	[MRX_TIER_INCLUDES] [varchar](max) NULL,
	[MRX_TIER_LOCAT_OON] [varchar](max) NULL,
	[MRX_TIER_LOCAT_LTC] [varchar](max) NULL,
	[MRX_TIER_OONP_1M_NUM] [varchar](max) NULL,
	[MRX_TIER_OONP_OTHNUMN] [varchar](max) NULL,
	[MRX_TIER_LTCP_1M] [varchar](max) NULL,
	[MRX_TIER_CSTSHR_STRUCT_TYPE] [varchar](max) NULL,
	[MRX_TIER_OONP_COINS_1M] [varchar](max) NULL,
	[MRX_TIER_OONP_COINS_OTHNUM] [varchar](max) NULL,
	[MRX_TIER_LTCP_COINS_1M] [varchar](max) NULL,
	[MRX_TIER_OONP_COPAY_1M] [varchar](max) NULL,
	[MRX_TIER_OONP_COPAY_OTHNUM] [varchar](max) NULL,
	[MRX_TIER_LTCP_COPAY_DAILY] [varchar](max) NULL,
	[MRX_TIER_LTCP_COPAY_1M] [varchar](max) NULL,
	[MRX_TIER_GAP_COST_SHARE] [varchar](max) NULL,
	[MRX_TIER_EXTD_DAYS_YN] [varchar](max) NULL,
	[MRX_TIER_EXTD_FIRST_FILL] [varchar](max) NULL,
	[MRX_TIER_LABEL_LIST] [varchar](max) NULL,
	[MRX_TIER_FORM_MODEL_DESC] [varchar](max) NULL,
	[MRX_TIER_GAP_PART_DRUGS] [varchar](max) NULL,
	[MRX_TIER_GAP_PART_INCLUDES] [varchar](max) NULL,
	[MRX_TIER_INCLUDES_MMP] [varchar](max) NULL,
	[MRX_TIER_COPAY_AMT_RSTD_MMPMIN] [varchar](max) NULL,
	[MRX_TIER_COPAY_AMT_RSTD_MMPMAX] [varchar](max) NULL,
	[MRX_TIER_COPAY_AMT_OON_MMPMIN] [varchar](max) NULL,
	[MRX_TIER_COPAY_AMT_OON_MMPMAX] [varchar](max) NULL,
	[MRX_TIER_COPAY_AMT_MSTD_MMPMIN] [varchar](max) NULL,
	[MRX_TIER_COPAY_AMT_MSTD_MMPMAX] [varchar](max) NULL,
	[MRX_TIER_COPAY_AMT_LTC_MMPMIN] [varchar](max) NULL,
	[MRX_TIER_COPAY_AMT_LTC_MMPMAX] [varchar](max) NULL,
	[MRX_TIER_RSPFD_COPAY_DAILY] [varchar](max) NULL,
	[MRX_TIER_RSPFD_COPAY_1M] [varchar](max) NULL,
	[MRX_TIER_RSPFD_COPAY_2M] [varchar](max) NULL,
	[MRX_TIER_RSPFD_COPAY_3M] [varchar](max) NULL,
	[MRX_TIER_RSPFD_COINS_AVG_1M] [varchar](max) NULL,
	[MRX_TIER_RSPFD_COINS_1M] [varchar](max) NULL,
	[MRX_TIER_RSPFD_COINS_2M] [varchar](max) NULL,
	[MRX_TIER_RSPFD_COINS_3M] [varchar](max) NULL,
	[MRX_TIER_LOCAT_RSPLT] [varchar](max) NULL,
	[MRX_TIER_LOCAT_MOSPLT] [varchar](max) NULL,
	[MRX_TIER_RSTD_1M_NUM] [varchar](max) NULL,
	[MRX_TIER_RSTD_2M_NUM] [varchar](max) NULL,
	[MRX_TIER_RSTD_3M_NUM] [varchar](max) NULL,
	[MRX_TIER_RSPLT_1M_NUM] [varchar](max) NULL,
	[MRX_TIER_RSPLT_2M_NUM] [varchar](max) NULL,
	[MRX_TIER_RSPLT_3M_NUM] [varchar](max) NULL,
	[MRX_TIER_MOSTD_1M_NUM] [varchar](max) NULL,
	[MRX_TIER_MOSTD_2M_NUM] [varchar](max) NULL,
	[MRX_TIER_MOSTD_3M_NUM] [varchar](max) NULL,
	[MRX_TIER_MOSPLT_1M_NUM] [varchar](max) NULL,
	[MRX_TIER_MOSPLT_2M_NUM] [varchar](max) NULL,
	[MRX_TIER_MOSPLT_3M_NUM] [varchar](max) NULL,
	[MRX_TIER_RSTD_COPAY_DAILY] [varchar](max) NULL,
	[MRX_TIER_RSTD_COPAY_1M] [varchar](max) NULL,
	[MRX_TIER_RSTD_COPAY_2M] [varchar](max) NULL,
	[MRX_TIER_RSTD_COPAY_3M] [varchar](max) NULL,
	[MRX_TIER_RSTD_COINS_AVG_1M] [varchar](max) NULL,
	[MRX_TIER_RSTD_COINS_1M] [varchar](max) NULL,
	[MRX_TIER_RSTD_COINS_2M] [varchar](max) NULL,
	[MRX_TIER_RSTD_COINS_3M] [varchar](max) NULL,
	[MRX_TIER_RSSTD_COPAY_DAILY] [varchar](max) NULL,
	[MRX_TIER_RSSTD_COPAY_1M] [varchar](max) NULL,
	[MRX_TIER_RSSTD_COPAY_2M] [varchar](max) NULL,
	[MRX_TIER_RSSTD_COPAY_3M] [varchar](max) NULL,
	[MRX_TIER_RSSTD_COINS_AVG_1M] [varchar](max) NULL,
	[MRX_TIER_RSSTD_COINS_1M] [varchar](max) NULL,
	[MRX_TIER_RSSTD_COINS_2M] [varchar](max) NULL,
	[MRX_TIER_RSSTD_COINS_3M] [varchar](max) NULL,
	[MRX_TIER_MOSTD_COPAY_DAILY] [varchar](max) NULL,
	[MRX_TIER_MOSTD_COPAY_1M] [varchar](max) NULL,
	[MRX_TIER_MOSTD_COPAY_2M] [varchar](max) NULL,
	[MRX_TIER_MOSTD_COPAY_3M] [varchar](max) NULL,
	[MRX_TIER_MOSTD_COINS_1M] [varchar](max) NULL,
	[MRX_TIER_MOSTD_COINS_2M] [varchar](max) NULL,
	[MRX_TIER_MOSTD_COINS_3M] [varchar](max) NULL,
	[MRX_TIER_MOSSTD_COPAY_DAILY] [varchar](max) NULL,
	[MRX_TIER_MOSSTD_COPAY_1M] [varchar](max) NULL,
	[MRX_TIER_MOSSTD_COPAY_2M] [varchar](max) NULL,
	[MRX_TIER_MOSSTD_COPAY_3M] [varchar](max) NULL,
	[MRX_TIER_MOSSTD_COINS_1M] [varchar](max) NULL,
	[MRX_TIER_MOSSTD_COINS_2M] [varchar](max) NULL,
	[MRX_TIER_MOSSTD_COINS_3M] [varchar](max) NULL,
	[MRX_TIER_MOSPFD_COPAY_DAILY] [varchar](max) NULL,
	[MRX_TIER_MOSPFD_COPAY_1M] [varchar](max) NULL,
	[MRX_TIER_MOSPFD_COPAY_2M] [varchar](max) NULL,
	[MRX_TIER_MOSPFD_COPAY_3M] [varchar](max) NULL,
	[MRX_TIER_MOSPFD_COINS_1M] [varchar](max) NULL,
	[MRX_TIER_MOSPFD_COINS_2M] [varchar](max) NULL,
	[MRX_TIER_MOSPFD_COINS_3M] [varchar](max) NULL,
	[MRX_TIER_LOCAT_RSTD] [varchar](max) NULL,
	[MRX_TIER_LOCAT_MOSTD] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[STEP10B]    Script Date: 5/18/2017 4:38:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP10B](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B10B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B10B_BENDESC_TRN] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMO_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_LIM_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMT_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_PER_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_TT_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMT_PAL_DAYS] [varchar](max) NULL,
	[PBP_B10B_BENDESC_MT_PAL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMO_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_LIM_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMT_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_PER_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_TT_AL] [varchar](max) NULL,
	[PBP_B10B_BENDESC_AMT_AL_DAYS] [varchar](max) NULL,
	[PBP_B10B_BENDESC_MT_AL] [varchar](max) NULL,
	[PBP_B10B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B10B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B10B_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B10B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B10B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B10B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B10B_COINS_YN] [varchar](max) NULL,
	[PBP_B10B_COINS_PCT_MIN] [varchar](max) NULL,
	[PBP_B10B_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B10B_DED_YN] [varchar](max) NULL,
	[PBP_B10B_DED_AMT] [varchar](max) NULL,
	[PBP_B10B_COPAY_YN] [varchar](max) NULL,
	[PBP_B10B_COPAY_AMT_MIN] [varchar](max) NULL,
	[PBP_B10B_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B10B_AUTH] [varchar](max) NULL,
	[PBP_B10B_REFER_YN] [varchar](max) NULL,
	[PBP_B10B_NOTES] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[STEP16A]    Script Date: 5/18/2017 4:38:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP16A](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B16A_BENDESC_YN] [varchar](max) NULL,
	[PBP_B16A_BENDESC_EHC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_AMO_OE] [varchar](max) NULL,
	[PBP_B16A_BENDESC_LIM_OE] [varchar](max) NULL,
	[PBP_B16A_BENDESC_NUMV_OE] [varchar](max) NULL,
	[PBP_B16A_BENDESC_PER_OE] [varchar](max) NULL,
	[PBP_B16A_BENDESC_AMO_PC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_LIM_PC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_NUMV_PC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_PER_PC] [varchar](max) NULL,
	[PBP_B16A_BENDESC_AMO_FT] [varchar](max) NULL,
	[PBP_B16A_BENDESC_LIM_FT] [varchar](max) NULL,
	[PBP_B16A_BENDESC_NUMV_FT] [varchar](max) NULL,
	[PBP_B16A_BENDESC_PER_FT] [varchar](max) NULL,
	[PBP_B16A_BENDESC_AMO_DX] [varchar](max) NULL,
	[PBP_B16A_BENDESC_LIM_DX] [varchar](max) NULL,
	[PBP_B16A_BENDESC_NUMV_DX] [varchar](max) NULL,
	[PBP_B16A_BENDESC_PER_DX] [varchar](max) NULL,
	[PBP_B16A_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B16A_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B16A_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B16A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B16A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B16A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B16A_COINS_YN] [varchar](max) NULL,
	[PBP_B16A_COINS_EHC] [varchar](max) NULL,
	[PBP_B16A_COINS_CSERV_SC_POV_YN] [varchar](max) NULL,
	[PBP_B16A_COINS_CSERV_SC_POV] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_OV] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_OE] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_MAXOE] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_PC] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_MAXPC] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_FT] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_MAXFT] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_DX] [varchar](max) NULL,
	[PBP_B16A_COINS_PCT_MAXDX] [varchar](max) NULL,
	[PBP_B16A_DED_YN] [varchar](max) NULL,
	[PBP_B16A_DED_AMT] [varchar](max) NULL,
	[PBP_B16A_COPAY_YN] [varchar](max) NULL,
	[PBP_B16A_COPAY_EHC] [varchar](max) NULL,
	[PBP_B16A_COPAY_CSERV_SC_POV_YN] [varchar](max) NULL,
	[PBP_B16A_COPAY_CSERV_SC_POV] [varchar](max) NULL,
	[PBP_B16A_COPAY_OV_AMT] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_OEMIN] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_OEMAX] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_PCMIN] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_PCMAX] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_FTMIN] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_FTMAX] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_DXMIN] [varchar](max) NULL,
	[PBP_B16A_COPAY_AMT_DXMAX] [varchar](max) NULL,
	[PBP_B16A_NOTES] [varchar](max) NULL,
	[PBP_B16A_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B16A_AUTH] [varchar](max) NULL,
	[PBP_B16A_REFER_YN] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[STEP16B]    Script Date: 5/18/2017 4:38:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP16B](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B16B_BENDESC_EHC] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_ES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_ES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_ES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_ES] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_DS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_DS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_DS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_DS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_RS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_RS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_RS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_RS] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_EPE] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_EPE] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_EPE] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_EPE] [varchar](max) NULL,
	[PBP_B16B_BENDESC_AMO_POO] [varchar](max) NULL,
	[PBP_B16B_BENDESC_LIM_POO] [varchar](max) NULL,
	[PBP_B16B_BENDESC_NUMV_POO] [varchar](max) NULL,
	[PBP_B16B_BENDESC_PER_POO] [varchar](max) NULL,
	[PBP_B16B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B16B_MAXBENE_TYPE] [varchar](max) NULL,
	[PBP_B16B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B16B_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B16B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B16B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B16B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B16B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B16B_COINS_YN] [varchar](max) NULL,
	[PBP_B16B_COINS_EHC] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MC] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAX_MC] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_ES] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXES] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_DS] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXDS] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_RS] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXRS] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_EPE] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXEPE] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_POO] [varchar](max) NULL,
	[PBP_B16B_COINS_PCT_MAXPOO] [varchar](max) NULL,
	[PBP_B16B_DED_YN] [varchar](max) NULL,
	[PBP_B16B_DED_AMT] [varchar](max) NULL,
	[PBP_B16B_COPAY_YN] [varchar](max) NULL,
	[PBP_B16B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B16B_COPAY_MC_AMT] [varchar](max) NULL,
	[PBP_B16B_COPAY_MCMAX_AMT] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_ESMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_ESMAX] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_DSMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_DSMAX] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_RSMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_RSMAX] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_EPEMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_EPEMAX] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_POOMIN] [varchar](max) NULL,
	[PBP_B16B_COPAY_AMT_POOMAX] [varchar](max) NULL,
	[PBP_B16B_AUTH] [varchar](max) NULL,
	[PBP_B16B_REFER_YN] [varchar](max) NULL,
	[PBP_B16B_NOTES] [varchar](max) NULL,
	[PBP_B16B_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[STEP17A]    Script Date: 5/18/2017 4:38:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP17A](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B17A_BENDESC_YN] [varchar](max) NULL,
	[PBP_B17A_BENDESC_ENH] [varchar](max) NULL,
	[PBP_B17A_BENDESC_AMO_REX] [varchar](max) NULL,
	[PBP_B17A_BENDESC_LIM] [varchar](max) NULL,
	[PBP_B17A_BENDESC_NUMV] [varchar](max) NULL,
	[PBP_B17A_BENDESC_PER] [varchar](max) NULL,
	[PBP_B17A_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B17A_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B17A_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B17A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B17A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B17A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B17A_COINS_YN] [varchar](max) NULL,
	[PBP_B17A_COINS_EHC] [varchar](max) NULL,
	[PBP_B17A_COINS_MCMIN_PCT] [varchar](max) NULL,
	[PBP_B17A_COINS_MCMAX_PCT] [varchar](max) NULL,
	[PBP_B17A_COINS_REXMIN_PCT] [varchar](max) NULL,
	[PBP_B17A_COINS_REXMAX_PCT] [varchar](max) NULL,
	[PBP_B17A_DED_YN] [varchar](max) NULL,
	[PBP_B17A_DED_AMT] [varchar](max) NULL,
	[PBP_B17A_COPAY_YN] [varchar](max) NULL,
	[PBP_B17A_COPAY_EHC] [varchar](max) NULL,
	[PBP_B17A_COPAY_MCMIN_AMT] [varchar](max) NULL,
	[PBP_B17A_COPAY_MCMAX_AMT] [varchar](max) NULL,
	[PBP_B17A_COPAY_REXMIN_AMT] [varchar](max) NULL,
	[PBP_B17A_COPAY_REXMAX_AMT] [varchar](max) NULL,
	[PBP_B17A_NOTES] [varchar](max) NULL,
	[PBP_B17A_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B17A_AUTH] [varchar](max) NULL,
	[PBP_B17A_REFER_YN] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[STEP17B]    Script Date: 5/18/2017 4:39:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP17B](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B17B_BENDESC_ENH] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_CL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_LIM_CL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_NUMV_CL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_PER_CL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_EGS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_LIM_EGS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_NUMV_EGS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_PER_EGS] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_EGI] [varchar](max) NULL,
	[PBP_B17B_BENDESC_LIM_EGL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_NUMV_EGL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_PER_EGL] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_EGF] [varchar](max) NULL,
	[PBP_B17B_BENDESC_LIM_EGF] [varchar](max) NULL,
	[PBP_B17B_BENDESC_NUMV_EGF] [varchar](max) NULL,
	[PBP_B17B_BENDESC_PER_EGF] [varchar](max) NULL,
	[PBP_B17B_BENDESC_AMO_UPG] [varchar](max) NULL,
	[PBP_B17B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B17B_MAXPLAN_TYPE] [varchar](max) NULL,
	[PBP_B17B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B17B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B17B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B17B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B17B_COINS_YN] [varchar](max) NULL,
	[PBP_B17B_COINS_EHC] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_CL_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_CL_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGS_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGS_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGL_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGL_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGF_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_EGF_MAX] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_UPG_MIN] [varchar](max) NULL,
	[PBP_B17B_COINS_PCT_UPG_MAX] [varchar](max) NULL,
	[PBP_B17B_DED_YN] [varchar](max) NULL,
	[PBP_B17B_DED_AMT] [varchar](max) NULL,
	[PBP_B17B_COPAY_YN] [varchar](max) NULL,
	[PBP_B17B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_MC_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_MC_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_CL_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_CL_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGS_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGS_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGL_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGL_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGF_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_EGF_MAX] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_UPG_MIN] [varchar](max) NULL,
	[PBP_B17B_COPAY_AMT_UPG_MAX] [varchar](max) NULL,
	[PBP_B17B_NOTES] [varchar](max) NULL,
	[PBP_B17B_COMB_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_EGL] [varchar](max) NULL,
	[PBP_B17B_COMB_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_UPG] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_EGS] [varchar](max) NULL,
	[PBP_B17B_COMB_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_EGF] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_EGL] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_EGS] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_CL] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_CL] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_PER_UPG] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_AMT_EGF] [varchar](max) NULL,
	[PBP_B17B_INDV_MAXPLAN_BENDESC] [varchar](max) NULL,
	[PBP_B17B_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBP_B17B_AUTH] [varchar](max) NULL,
	[PBP_B17B_REFER_YN] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[STEP18A]    Script Date: 5/18/2017 4:39:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP18A](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B18A_BENDESC_YN] [varchar](max) NULL,
	[PBP_B18A_BENDESC_ENH] [varchar](max) NULL,
	[PBP_B18A_BENDESC_AMO_RHT] [varchar](max) NULL,
	[PBP_B18A_BENDESC_LIM_RHT] [varchar](max) NULL,
	[PBP_B18A_BENDESC_NUMV_CL] [varchar](max) NULL,
	[PBP_B18A_BENDESC_PER_RHT] [varchar](max) NULL,
	[PBP_B18A_BENDESC_AMO_FHA] [varchar](max) NULL,
	[PBP_B18A_BENDESC_LIM_FHA] [varchar](max) NULL,
	[PBP_B18A_BENDESC_NUMV_FHA] [varchar](max) NULL,
	[PBP_B18A_BENDESC_PER_FHA] [varchar](max) NULL,
	[PBP_B18A_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B18A_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B18A_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B18A_MAXENR_YN] [varchar](max) NULL,
	[PBP_B18A_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B18A_MAXENR_PER] [varchar](max) NULL,
	[PBP_B18A_COINS_YN] [varchar](max) NULL,
	[PBP_B18A_COINS_EHC] [varchar](max) NULL,
	[PBP_B18A_MED_COINS_PCT_MAX] [varchar](max) NULL,
	[PBP_B18A_MED_COINS_PCT] [varchar](max) NULL,
	[PBP_B18A_COINS_PCT_RHT] [varchar](max) NULL,
	[PBP_B18A_COINS_PCT_MAX_RHT] [varchar](max) NULL,
	[PBP_B18A_COINS_PCT_FHA] [varchar](max) NULL,
	[PBP_B18A_COINS_PCT_MAX_FHA] [varchar](max) NULL,
	[PBP_B18A_DED_YN] [varchar](max) NULL,
	[PBP_B18A_DED_AMT] [varchar](max) NULL,
	[PBP_B18A_COPAY_YN] [varchar](max) NULL,
	[PBP_B18A_COPAY_EHC] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT] [varchar](max) NULL,
	[PBP_B18A_MED_COPAY_AMT_MAX] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT_RHT] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT_MAX_RHT] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT_FHA] [varchar](max) NULL,
	[PBP_B18A_COPAY_AMT_MAX_FHA] [varchar](max) NULL,
	[PBP_B18A_AUTH] [varchar](max) NULL,
	[PBP_B18A_REFER_YN] [varchar](max) NULL,
	[PBP_B18A_NOTES] [varchar](max) NULL,
	[PBP_B18A_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[STEP18B]    Script Date: 5/18/2017 4:39:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP18B](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B18B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B18B_BENDESC_ENH] [varchar](max) NULL,
	[PBP_B18B_BENDESC_AMO_AT] [varchar](max) NULL,
	[PBP_B18B_BENDESC_LIM_AT] [varchar](max) NULL,
	[PBP_B18B_BENDESC_NUMV_AT] [varchar](max) NULL,
	[PBP_B18B_BENDESC_PER_AT] [varchar](max) NULL,
	[PBP_B18B_BENDESC_AMO_IE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_LIM_IE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_NUMV_IE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_PER_IE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_AMO_OE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_LIM_OE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_NUMV_OE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_PER_OE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_AMO_OTE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_LIM_OTE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_NUMV_OTE] [varchar](max) NULL,
	[PBP_B18B_BENDESC_PER_OTE] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_PEREAR_YN] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_TYPE] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B18B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B18B_MAXENR_TYPE] [varchar](max) NULL,
	[PBP_B18B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B18B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B18B_COINS_YN] [varchar](max) NULL,
	[PBP_B18B_COINS_EHC] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_AT_MIN] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_AT_MAX] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_IE_MIN] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_IE_MAX] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_OE_MIN] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_OE_MAX] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_OTE_MIN] [varchar](max) NULL,
	[PBP_B18B_COINS_PCT_OTE_MAX] [varchar](max) NULL,
	[PBP_B18B_DED_YN] [varchar](max) NULL,
	[PBP_B18B_DED_AMT] [varchar](max) NULL,
	[PBP_B18B_COPAY_YN] [varchar](max) NULL,
	[PBP_B18B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B18B_COPAY_AT_MIN_AMT] [varchar](max) NULL,
	[PBP_B18B_COPAY_AT_MAX_AMT] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_IE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_IE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_IE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_IE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_OE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_OE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_OE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_OE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_OTE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_PER_OTE_MAX] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_OTE_MIN] [varchar](max) NULL,
	[PBP_B18B_COPAY_AMT_P2_OTE_MAX] [varchar](max) NULL,
	[PBP_B18B_AUTH] [varchar](max) NULL,
	[PBP_B18B_REFER_YN] [varchar](max) NULL,
	[PBP_B18B_NOTES] [varchar](max) NULL,
	[PBP_B18B_MAXPLAN_IN_OON] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[STEP7B]    Script Date: 5/18/2017 4:39:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP7B](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B7B_BENDESC_YN] [varchar](max) NULL,
	[PBP_B7B_BENDESC_RC] [varchar](max) NULL,
	[PBP_B7B_BENDESC_AMO] [varchar](max) NULL,
	[PBP_B7B_BENDESC_LIM_RC] [varchar](max) NULL,
	[PBP_B7B_BENDESC_NUM_RC] [varchar](max) NULL,
	[PBP_B7B_BENDESC_PER] [varchar](max) NULL,
	[PBP_B7B_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B7B_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B7B_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B7B_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7B_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7B_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7B_COINS_YN] [varchar](max) NULL,
	[PBP_B7B_COINS_EHC] [varchar](max) NULL,
	[PBP_B7B_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B7B_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B7B_COINS_PCT_CC_MIN] [varchar](max) NULL,
	[PBP_B7B_COINS_PCT_CC_MAX] [varchar](max) NULL,
	[PBP_B7B_DED_YN] [varchar](max) NULL,
	[PBP_B7B_DED_AMT] [varchar](max) NULL,
	[PBP_B7B_COPAY_YN] [varchar](max) NULL,
	[PBP_B7B_COPAY_EHC] [varchar](max) NULL,
	[PBP_B7B_COPAY_MC_AMT_MIN] [varchar](max) NULL,
	[PBP_B7B_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B7B_COPAY_RC_AMT_MIN] [varchar](max) NULL,
	[PBP_B7B_COPAY_RC_AMT_MAX] [varchar](max) NULL,
	[PBP_B7B_AUTH] [varchar](max) NULL,
	[PBP_B7B_REFER_YN] [varchar](max) NULL,
	[PBP_B7B_NOTES] [varchar](max) NULL,
	[PBP_B7B_COMBINED_BEN] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[STEP7F]    Script Date: 5/18/2017 4:40:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[STEP7F](
	[QID] [varchar](max) NULL,
	[PBP_D_OPT_IDENTIFIER] [varchar](max) NULL,
	[SQUISHID] [varchar](max) NULL,
	[STATUS] [varchar](max) NULL,
	[PBP_B7F_BENDESC_YN] [varchar](max) NULL,
	[PBP_B7F_BENDESC_RF] [varchar](max) NULL,
	[PBP_B7F_BENDESC_AMO_RF] [varchar](max) NULL,
	[PBP_B7F_BENDESC_LIM_RF] [varchar](max) NULL,
	[PBP_B7F_BENDESC_AMT_RF] [varchar](max) NULL,
	[PBP_B7F_BENDESC_PER_RF] [varchar](max) NULL,
	[PBP_B7F_MAXPLAN_YN] [varchar](max) NULL,
	[PBP_B7F_MAXPLAN_AMT] [varchar](max) NULL,
	[PBP_B7F_MAXPLAN_PER] [varchar](max) NULL,
	[PBP_B7F_MAXENR_YN] [varchar](max) NULL,
	[PBP_B7F_MAXENR_AMT] [varchar](max) NULL,
	[PBP_B7F_MAXENR_PER] [varchar](max) NULL,
	[PBP_B7F_COINS_YN] [varchar](max) NULL,
	[PBP_B7F_COINS_EHC] [varchar](max) NULL,
	[PBP_B7F_COINS_PCT_MC_MIN] [varchar](max) NULL,
	[PBP_B7F_COINS_PCT_MC_MAX] [varchar](max) NULL,
	[PBP_B7F_COINS_PCT_RF_MIN] [varchar](max) NULL,
	[PBP_B7F_COINS_PCT_RF_MAX] [varchar](max) NULL,
	[PBP_B7F_DED_YN] [varchar](max) NULL,
	[PBP_B7F_DED_AMT] [varchar](max) NULL,
	[PBP_B7F_COPAY_YN] [varchar](max) NULL,
	[PBP_B7F_COPAY_EHC] [varchar](max) NULL,
	[PBP_B7F_COPAY_MC_AMT_MIN] [varchar](max) NULL,
	[PBP_B7F_COPAY_MC_AMT_MAX] [varchar](max) NULL,
	[PBP_B7F_COPAY_RF_AMT_MIN] [varchar](max) NULL,
	[PBP_B7F_COPAY_RF_AMT_MAX] [varchar](max) NULL,
	[PBP_B7F_AUTH] [varchar](max) NULL,
	[PBP_B7F_REFER_YN] [varchar](max) NULL,
	[PBP_B7F_NOTES] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [Setup].[PBPFormInstance]    Script Date: 5/18/2017 4:40:44 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Setup].[PBPFormInstance](
	[PBPFormInstanceID] [int] IDENTITY(1,1) NOT NULL,
	[PBPImportBatchID] [int] NULL,
	[QID] [varchar](200) NULL,
	[FormInstanceID] [int] NULL,
	[DOCID] [int] NULL,
	[FolderID] [int] NULL,
 CONSTRAINT [PK_PBPFormInstance] PRIMARY KEY CLUSTERED 
(
	[PBPFormInstanceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [Setup].[PBPImportActivityLog]    Script Date: 5/18/2017 4:41:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Setup].[PBPImportActivityLog](
	[PBPImportActivityLogID] [int] IDENTITY(1,1) NOT NULL,
	[PBPImportQueueID] [int] NULL,
	[PBPImportBatchID] [int] NULL,
	[FileName] [varchar](50) NULL,
	[TableName] [varchar](50) NULL,
	[Message] [varchar](4000) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
 CONSTRAINT [PK_PBPImportActivityLog] PRIMARY KEY CLUSTERED 
(
	[PBPImportActivityLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [Setup].[PBPImportBatch]    Script Date: 5/18/2017 4:41:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Setup].[PBPImportBatch](
	[PBPImportBatchID] [int] IDENTITY(1,1) NOT NULL,
	[ProcessStatus1Up] [int] NULL,
	[AddedDate] [datetime] NULL,
	[AddedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_PBPImportBatch] PRIMARY KEY CLUSTERED 
(
	[PBPImportBatchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [Setup].[PBPImportQueue]    Script Date: 5/18/2017 4:42:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Setup].[PBPImportQueue](
	[PBPImportQueueID] [int] IDENTITY(1,1) NOT NULL,
	[PBPImportBatchID] [int] NULL,
	[Description] [nvarchar](50) NULL,
	[FileName] [nvarchar](50) NULL,
	[Location] [nvarchar](300) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [nvarchar](20) NULL,
	[ImportStartDate] [datetime] NULL,
	[ImportEndDate] [datetime] NULL,
	[Status] [smallint] NULL,
	[FolderID] [int] NOT NULL,
	[FolderVersionID] [int] NOT NULL,
	[Year] [int] NOT NULL,
 CONSTRAINT [PK_PBPImportQueue] PRIMARY KEY CLUSTERED 
(
	[PBPImportQueueID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [Setup].[PBPImportTableColumns]    Script Date: 5/18/2017 4:42:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Setup].[PBPImportTableColumns](
	[PBPImportTableColumnsID] [int] IDENTITY(1,1) NOT NULL,
	[PBPTableID] [int] NULL,
	[PBPImportTableColumnName] [varchar](50) NULL,
 CONSTRAINT [PK_PBPImportTableColumns] PRIMARY KEY CLUSTERED 
(
	[PBPImportTableColumnsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [Setup].[PBPImportTables]    Script Date: 5/18/2017 4:42:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [Setup].[PBPImportTables](
	[PBPTableID] [int] IDENTITY(1,1) NOT NULL,
	[PBPTableName] [varchar](50) NULL,
	[PBPTableSequence] [int] NULL,
	[EBSTableName] [varchar](50) NULL,
 CONSTRAINT [PK_PBPImportTables] PRIMARY KEY CLUSTERED 
(
	[PBPTableID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[DELETED_PLANS]    Script Date: 5/18/2017 4:44:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[DELETED_PLANS](
	[PBP_A_CONTRACT_NUMBER] [varchar](max) NULL,
	[PBP_A_PLAN_IDENTIFIER] [varchar](max) NULL,
	[PBP_A_SEGMENT_ID] [varchar](max) NULL,
	[DEL_DATE] [datetime] NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


/****** Object:  Table [PBP].[HISTORY]    Script Date: 5/18/2017 4:44:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[HISTORY](
	[QID] [varchar](max) NULL,
	[DAYTIME] [datetime] NULL,
	[EVENT] [varchar](max) NULL,
	[SECTION] [varchar](max) NULL,
	[USERNAME] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PATCH_HISTORY]    Script Date: 5/18/2017 4:45:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PATCH_HISTORY](
	[PBPVERSION] [varchar](max) NULL,
	[APPLYSTAMP] [datetime] NULL,
	[PBPTARGET] [varchar](max) NULL,
	[PBPACTION] [varchar](max) NULL,
	[USERACTION] [varchar](max) NULL,
	[TAG1] [varchar](max) NULL,
	[TAG2] [varchar](max) NULL,
	[TAG3] [varchar](max) NULL,
	[PBPImportBatchID] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[AuthMapping]    Script Date: 5/25/2017 11:56:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[AuthMapping](
	[AuthMappingID] [int] NOT NULL,
	[BenefitReviewID] [int] NULL,
	[FieldPath] [varchar](50) NULL,
	[FieldName] [varchar](50) NULL,
	[PBPTableName] [varchar](50) NULL,
	[PBPFieldName] [varchar](50) NULL,
	[SequenceNumberforOne] [int] NULL,
	[IsActive] [bit] NULL,
	[IsCustomRule] [bit] NULL,
	[CustomRuleTypeId] [int] NULL,
 CONSTRAINT [PK_AuthMapping] PRIMARY KEY CLUSTERED 
(
	[AuthMappingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[BenefitReview]    Script Date: 5/25/2017 11:57:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[BenefitReview](
	[BenefitReviewID] [int] IDENTITY(1,1) NOT NULL,
	[SectionName] [varchar](50) NULL,
	[FieldPath] [varchar](50) NULL,
	[FieldName] [varchar](50) NULL,
	[BenefitCategory1] [varchar](150) NULL,
	[BenefitCategory2] [varchar](150) NULL,
	[BenefitCategory3] [varchar](150) NULL,
	[ValueToMatch] [varchar](10) NULL,
	[SequenceNumberforOne] [int] NULL,
	[PBPTableName] [varchar](50) NULL,
	[PBPFieldName] [varchar](50) NULL,
	[OONPBPTableName] [varchar](50) NULL,
	[OONPBPFieldName] [varchar](50) NULL,
	[ByDefaultSelection] [bit] NULL,
	[IsActive] [bit] NULL,
	[IsCustomRule] [bit] NULL,
	[CustomRuleTypeId] [int] NULL,
 CONSTRAINT [PK_BenefitReview] PRIMARY KEY CLUSTERED 
(
	[BenefitReviewID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[BenefitReviewAmount]    Script Date: 5/25/2017 11:58:00 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[BenefitReviewAmount](
	[BenefitReviewAmountID] [int] NOT NULL,
	[BenefitReviewID] [int] NOT NULL,
	[NetworkType] [varchar](50) NULL,
	[FieldPath] [varchar](150) NULL,
	[FieldName] [varchar](150) NULL,
	[DeductiblePath] [varchar](150) NULL,
	[DeductiblePBPTableName] [varchar](100) NULL,
	[DeductiblePBPFieldName] [varchar](100) NULL,
	[MinimumCopayPath] [varchar](150) NULL,
	[MinimumCopayPBPTableName] [varchar](100) NULL,
	[MinimumCopayPBPFieldName] [varchar](100) NULL,
	[MaximumCopayPath] [varchar](150) NULL,
	[MaximumCopayPBPTableName] [varchar](100) NULL,
	[MaximumCopayPBPFieldName] [varchar](100) NULL,
	[MinimumCoissurancePath] [varchar](150) NULL,
	[MinimumCoissurancePBPTableName] [varchar](100) NULL,
	[MinimumCoissurancePBPFieldName] [varchar](100) NULL,
	[MaximumCoissurancePath] [varchar](150) NULL,
	[MaximumCoissurancePBPTableName] [varchar](100) NULL,
	[MaximumCoissurancePBPFieldName] [varchar](100) NULL,
	[OOPMValuePath] [varchar](150) NULL,
	[OOPMValuePBPTableName] [varchar](100) NULL,
	[OOPMValuePBPFieldName] [varchar](100) NULL,
	[OOPMPeriodicityPath] [varchar](150) NULL,
	[OOPMPeriodicityPBPTableName] [varchar](100) NULL,
	[OOPMPeriodicityPBPFieldName] [varchar](100) NULL,
	[MaximumPlanBenefitCoverageAmountPath] [varchar](150) NULL,
	[MaximumPlanBenefitCoverageAmountPBPTableName] [varchar](100) NULL,
	[MaximumPlanBenefitCoverageAmountPBPFieldName] [varchar](100) NULL,
	[MaximumPlanBenefitCoveragePeriodicityPath] [varchar](150) NULL,
	[MaximumPlanBenefitCoveragePeriodicityPBPTableName] [varchar](100) NULL,
	[MaximumPlanBenefitCoveragePeriodicityPBPFieldName] [varchar](100) NULL,
	[IsActive] [bit] NULL,
	[IsCustomRule] [bit] NULL,
	[CustomRuleTypeId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[BenefitReviewAmountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[BenefitReviewOON]    Script Date: 5/25/2017 11:58:29 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[BenefitReviewOON](
	[BenefitReviewOONID] [int] NOT NULL,
	[BenefitReviewID] [int] NULL,
	[BC1] [varchar](200) NULL,
	[BC2] [varchar](200) NULL,
	[BC3] [varchar](200) NULL,
	[Code] [varchar](10) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_BenefitReviewOON_IsActive]  DEFAULT ((1)),
 CONSTRAINT [PK_BenefitReviewOON] PRIMARY KEY CLUSTERED 
(
	[BenefitReviewOONID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[CostShare]    Script Date: 5/25/2017 11:59:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [PBP].[CostShare](
	[CostShareID] [int] NOT NULL,
	[SectionName] [nvarchar](200) NULL,
	[FieldPath] [nvarchar](1000) NULL,
	[FieldName] [nvarchar](200) NULL,
	[BenefitCategory1] [nvarchar](1000) NULL,
	[BenefitCategory2] [nvarchar](1000) NULL,
	[BenefitCategory3] [nvarchar](1000) NULL,
	[SequenceNumberforOne] [int] NULL,
	[PBPTable] [nvarchar](1000) NULL,
	[PBPField] [nvarchar](1000) NULL,
	[IntervalFieldName] [nvarchar](200) NULL,
	[IntervalPBPTableName] [nvarchar](200) NULL,
	[IntervalPBPFieldName] [nvarchar](200) NULL,
	[IntervalOONPBPTableName] [nvarchar](200) NULL,
	[IntervalOONPBPFieldName] [nvarchar](200) NULL,
	[BenefitPeriodPBPTableName] [nvarchar](200) NULL,
	[BenefitPeriodPBPFieldName] [nvarchar](200) NULL,
	[IsthisBenefitUnlimitedPBPTableName] [nvarchar](200) NULL,
	[IsthisBenefitUnlimitedPBPFieldName] [nvarchar](200) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_CostShare_IsActive]  DEFAULT ((1)),
	[IsCustomRule] [bit] NOT NULL CONSTRAINT [DF_CostShare_IsCustomRule]  DEFAULT ((0)),
	[CustomRuleTypeId] [int] NULL,
 CONSTRAINT [PK_CostShare] PRIMARY KEY CLUSTERED 
(
	[CostShareID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/****** Object:  Table [PBP].[CostShareAmount]    Script Date: 5/26/2017 12:00:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[CostShareAmount](
	[CostShareAmountID] [int] NOT NULL,
	[CostShareID] [int] NOT NULL,
	[FieldPath] [varchar](200) NULL,
	[NetworkType] [varchar](50) NULL,
	[IntervalNumberPath] [varchar](200) NULL,
	[IntervalNumber] [int] NULL,
	[CopayBeginDayPath] [varchar](200) NULL,
	[CopayBeginDayPBPTableName] [varchar](200) NULL,
	[CopayBeginDayPBPFieldName] [varchar](200) NULL,
	[CopayEndDayPath] [varchar](200) NULL,
	[CopayEndDayPBPTableName] [varchar](200) NULL,
	[CopayEndDayPBPFieldName] [varchar](200) NULL,
	[CopayPath] [varchar](200) NULL,
	[CopayPBPTableName] [varchar](200) NULL,
	[CopayPBPFieldName] [varchar](200) NULL,
	[CoinsuranceBeginDayPath] [varchar](200) NULL,
	[CoinsuranceBeginDayPBPTableName] [varchar](200) NULL,
	[CoinsuranceBeginDayPBPFieldName] [varchar](200) NULL,
	[CoinsuranceEndDayPath] [varchar](200) NULL,
	[CoinsuranceEndDayPBPTableName] [varchar](200) NULL,
	[CoinsuranceEndDayPBPFieldName] [varchar](200) NULL,
	[CoinsurancePath] [varchar](200) NULL,
	[CoinsurancePBPTableName] [varchar](200) NULL,
	[CoinsurancePBPFieldName] [varchar](200) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_CostShareAmount_IsActive]  DEFAULT ((1)),
	[IsCustomRule] [bit] NOT NULL CONSTRAINT [DF_CostShareAmount_IsCustomRule]  DEFAULT ((0)),
	[CustomRuleTypeId] [int] NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

/****** Object:  Table [PBP].[PBPDataMap]    Script Date: 5/26/2017 12:00:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [PBP].[PBPDataMap](
	[PBPDataMapId] [int] IDENTITY(1,1) NOT NULL,
	[QID] [nvarchar](1000) NULL,
	[TableName] [nvarchar](200) NULL,
	[FieldName] [nvarchar](1000) NULL,
	[JsonData] [nvarchar](max) NULL,
	[PBPImportBatchID] [int] NULL,
 CONSTRAINT [PK_PBPDataMap] PRIMARY KEY CLUSTERED 
(
	[PBPDataMapId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

/****** Object:  Table [PBP].[PlanInformation]    Script Date: 5/26/2017 12:01:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [PBP].[PlanInformation](
	[PlanInformationID] [int] IDENTITY(1,1) NOT NULL,
	[SectionName] [nvarchar](2000) NULL,
	[FieldPath] [nvarchar](2000) NULL,
	[FieldName] [nvarchar](2000) NULL,
	[PBPTableName] [nvarchar](2000) NULL,
	[PBPFieldName] [nvarchar](2000) NULL,
	[IsActive] [bit] NULL,
	[IsCustomRule] [bit] NULL,
	[CustomRuleTypeId] [int] NULL,
 CONSTRAINT [PK_PlanInformation] PRIMARY KEY CLUSTERED 
(
	[PlanInformationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [PBP].[PlanType]    Script Date: 5/26/2017 12:01:35 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [PBP].[PlanType](
	[PlanTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NULL,
	[Description] [nvarchar](200) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_PlanType_IsActive]  DEFAULT ((0)),
 CONSTRAINT [PK_PlanType] PRIMARY KEY CLUSTERED 
(
	[PlanTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [PBP].[Prescription]    Script Date: 5/26/2017 12:02:08 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[Prescription](
	[PrescriptionID] [int] NOT NULL,
	[SectionName] [varchar](200) NULL,
	[FieldPath] [varchar](200) NULL,
	[FieldName] [varchar](200) NULL,
	[PBPTableName] [varchar](200) NULL,
	[PBPFieldName] [varchar](200) NULL,
	[PositionForOne] [int] NULL,
	[IsRepeater] [bit] NULL,
	[IsActive] [bit] NOT NULL,
	[IsCustomRule] [bit] NOT NULL,
	[CustomRuleTypeId] [int] NULL,
 CONSTRAINT [PK_Prescription_1] PRIMARY KEY CLUSTERED 
(
	[PrescriptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/****** Object:  Table [PBP].[PrescriptionRepeater]    Script Date: 5/26/2017 12:02:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [PBP].[PrescriptionRepeater](
	[PrescriptionRepeaterID] [int] NOT NULL,
	[RepeaterPath] [varchar](200) NULL,
	[RepeaterName] [varchar](200) NULL,
	[TiersPath] [varchar](200) NULL,
	[TiersFieldName] [varchar](200) NULL,
	[TiersPBPTableName] [varchar](200) NULL,
	[TiersPBPFieldName] [varchar](200) NULL,
	[DescriptionFieldName] [varchar](200) NULL,
	[DescriptionPBPTableName] [varchar](200) NULL,
	[DescriptionPBPFieldName] [varchar](200) NULL,
	[OnemonthsupplyName] [varchar](200) NULL,
	[OnemonthsupplyConiPBPTableName] [varchar](200) NULL,
	[OnemonthsupplyConiPBPFieldName] [varchar](200) NULL,
	[OnemonthsupplyPBPTableName] [varchar](200) NULL,
	[OnemonthsupplyPBPFieldName] [varchar](200) NULL,
	[TwomonthsupplyName] [varchar](200) NULL,
	[TwomonthsupplyConiPBPTableName] [varchar](200) NULL,
	[TwomonthsupplyConiPBPFieldName] [varchar](200) NULL,
	[TwomonthsupplyPBPTableName] [varchar](200) NULL,
	[TwomonthsupplyPBPFieldName] [varchar](200) NULL,
	[ThreemonthsupplyName] [varchar](200) NULL,
	[ThreemonthsupplyConiPBPTableName] [varchar](200) NULL,
	[ThreemonthsupplyConiPBPFieldName] [varchar](200) NULL,
	[ThreemonthsupplyPBPTableName] [varchar](200) NULL,
	[ThreemonthsupplyPBPFieldName] [varchar](200) NULL,
	[DrugsCoveredName] [varchar](200) NULL,
	[DrugsCoveredPBPTableName] [varchar](200) NULL,
	[DrugsCoveredPBPFieldName] [varchar](200) NULL,
	[TypeID] [int] NULL,
	[IsActive] [bit] NULL,
	[IsCustomRule] [bit] NULL,
	[CustomRuleTypeId] [int] NULL,
 CONSTRAINT [PK_PrescriptionRepeater_1] PRIMARY KEY CLUSTERED 
(
	[PrescriptionRepeaterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

