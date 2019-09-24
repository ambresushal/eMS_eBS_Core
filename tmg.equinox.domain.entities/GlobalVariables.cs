namespace tmg.equinox.domain.entities
{
    public static class GlobalVariables
    {
        public const string MAJOR = "MAJOR";
        public const string MINOR = "MINOR";
        public const string NEW = "New";
        public const string RETRO = "Retro";
        public const string APPROVED = "Approved";
        public const string NOTAPPROVED = "Not Approved";
        public const string NOTAPPLICABLE = "NOTAPPLICABLE";
        public const string INNETWORK = "In Network";
        public const string OUTOFNETWORK = "Out of Network";
        public const string MASTERLIST = "MasterList";
        public const string MASTERLIST_HSB = "MasterList_HSB";
        public const string MASTERLIST_CBC = "MasterList";

        public const int MILESTONECHECKLISTFORMDESIGNID = 9;
        public const int PRODUCTFORMDESIGNID = 2405;
        public const string NOTCOPYFROMDOCUMENT = "Not Copied";
        public const int FOLDERVERSIONINPROGRESSSTATEID = 1;
        public const int DOCUMENTREFERENCEFORMDESIGNID = 8;

        public const int MASTERLISTFORMDESIGNID = 1;
        public const int GB_STATUS_INPROGRESSID = 1;
        public const int GB_STATUS_COMPLETEID = 2;
        public const int GB_STATUS_IASDOWNLOADINPROGRESSID = 3;
        public const int GB_STATUS_IASDOWNLOADCOMPLETEID = 4;
        public const int GB_STATUS_ERRORLOGDOWNLOADINPROGRESSID = 5;
        public const int GB_STATUS_ERRORLOGDOWNLOADCOMPLETEID = 6;
        public const int GB_STATUS_IASGENERATIONFAILED = 7;
        public const int GB_STATUS_IASSCHEDULEDEXECUTION = 12;
        public const int GB_WIZARDSTEP_SETUPID = 1;
        public const int GB_WIZARDSTEP_ELEMENTSELECTIONID = 2;
        public const int GB_WIZARDSTEP_UPDATESELECTIONID = 3;
        public const int GB_WIZARDSTEP_GENERATEIASID = 4;
        public const string GB_BASELINE_COMMENT = "Global Update Baseline";
        public const string GB_BASELINE_STATUS = "Success";
        public const string GB_RETRO_COMMENT = "Retro";
        public const string GB_RETRO_STATUS = "Success";
        public const string GB_RealtimeExecutionType = "Realtime";
        public const string GB_ScheduledExecutionType = "Scheduled";
        public const int GB_BatchExecutionStatus = 1;
        public const int GB_BatchExecutionStatusRollbacked = 3;
        public const string GB_BatchExecutionComment = "Executed";
        public const string GB_BatchRollBackComment = "Rollbacked";

        public const string BenefitMatrix = "Benefit Matrix";
        public const string FaxBack = "Fax Back";
        public const string SPDMatrix = "SPD";

        public const string VisionMatrix = "Vision Matrix";
        public const string VisionFaxBack = "Vision FaxBack";
        public const string DentalMatrix = "Dental Matrix";
        public const string DentalFaxBack = "Dental FaxBack";
        public const string STDMatrix = "STD Matrix";
        public const string BenAdminBenefitMatrix = "BenAdmin Benefit Matrix";

        public const int AdminDesignID = 1083;
        public const int MedicalDesignID = 2359;
        public const int PBPDesignID = 2367;
        public const int VisionDesignID = 1100;
        public const int DentalDesignID = 1101;
        public const int STDDesignID = 1102;
        public const int BenAdminID = 1122;
        public const int VBIDDesignID = 2409;

        public const int MedicareFormDesignID = 2359;
        public const string ContractNumberSectionPath = "SECTIONASECTIONA1";
        public const string MiscellaneousSectionPath = "Miscellaneous";
        public const string ContractNumber = "ContractNumber";
        public const string IsPBPImport = "IsPBPImport";
        public const string CompletedFail = "Completed - Fail";
        public const string CompletedPass = "Completed - Pass";

        public static readonly string[] OptionalSupplementalPackageTables = { "STEP16B", "STEP16A", "PBPD_OPT", "STEP18B", "STEP18A", "STEP17B", "STEP17A", "STEP10B", "STEP7F", "STEP7B" };
    }
}
