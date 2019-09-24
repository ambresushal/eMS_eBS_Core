﻿using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmg.equinox.integration.data.Models;

namespace tmg.equinox.integration.translator.dao.Models.Mapping
{
    public class SESERuleConfigMasterMap : EntityTypeConfiguration<SESERuleConfigMaster>
    {
        public SESERuleConfigMasterMap()
        {
            // Primary Key
            this.HasKey(t => t.SESE_RULE);

            // Table & Column Mappings
            this.ToTable("SESERULEConfig", "Master");
            this.Property(t => t.SESE_RULE).HasColumnName("SESE_RULE");
            this.Property(t => t.SESE_DESC).HasColumnName("SESE_DESC");
            this.Property(t => t.SESE_CM_IND).HasColumnName("SESE_CM_IND");
            this.Property(t => t.SESE_PA_AMT_REQ).HasColumnName("SESE_PA_AMT_REQ");
            this.Property(t => t.SESE_PA_UNIT_REQ).HasColumnName("SESE_PA_UNIT_REQ");
            this.Property(t => t.SESE_PA_PROC_REQ).HasColumnName("SESE_PA_PROC_REQ");
            this.Property(t => t.SESE_VALID_SEX).HasColumnName("SESE_VALID_SEX");
            this.Property(t => t.SESE_SEX_EXCD_ID).HasColumnName("SESE_SEX_EXCD_ID");
            this.Property(t => t.SESE_MIN_AGE).HasColumnName("SESE_MIN_AGE");
            this.Property(t => t.SESE_MAX_AGE).HasColumnName("SESE_MAX_AGE");
            this.Property(t => t.SESE_AGE_EXCD_ID).HasColumnName("SESE_AGE_EXCD_ID");
            this.Property(t => t.SESE_COV_TYPE).HasColumnName("SESE_COV_TYPE");
            this.Property(t => t.SESE_COV_EXCD_ID).HasColumnName("SESE_COV_EXCD_ID");
            this.Property(t => t.SESE_RULE_TYPE).HasColumnName("SESE_RULE_TYPE");
            this.Property(t => t.SESE_CALC_IND).HasColumnName("SESE_CALC_IND");
            this.Property(t => t.SERL_REL_ID).HasColumnName("SERL_REL_ID");
            this.Property(t => t.SESE_OPTS).HasColumnName("SESE_OPTS");
            this.Property(t => t.WMDS_SEQ_NO).HasColumnName("WMDS_SEQ_NO");
            this.Property(t => t.SESE_ID_XLOW).HasColumnName("SESE_ID_XLOW");
            this.Property(t => t.SESE_DESC_XLOW).HasColumnName("SESE_DESC_XLOW");
            this.Property(t => t.SESE_DIS_EXCD_ID).HasColumnName("SESE_DIS_EXCD_ID");
            this.Property(t => t.SESE_MAX_CPAY_PCT).HasColumnName("SESE_MAX_CPAY_PCT");
            this.Property(t => t.SESE_FSA_REIMB_IND).HasColumnName("SESE_FSA_REIMB_IND");
            this.Property(t => t.SESE_HSA_REIMB_IND).HasColumnName("SESE_HSA_REIMB_IND");
            this.Property(t => t.SESE_HRA_DED_IND).HasColumnName("SESE_HRA_DED_IND");
            this.Property(t => t.SESE_MAX_CPAY_ACT_NVL).HasColumnName("SESE_MAX_CPAY_ACT_NVL");
            this.Property(t => t.SESE_CPAY_EXCD_ID_NVL).HasColumnName("SESE_CPAY_EXCD_ID_NVL");
            this.Property(t => t.Specialty).HasColumnName("Specialty");       
        }
    }
}