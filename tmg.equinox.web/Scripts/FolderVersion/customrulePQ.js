var customRulePQ = function () {
    this.loadCustomDataInRepeater = this.loadCustomDataInRepeaterMethods();
    this.customServiceRules = this.customRuleServiceMethods();
    this.PDBCTypes = [];

    //for activity log from datasource manual mapping
    this.isActivityLogEntries = true;

    this.URLs = {
        getAddtionalServiesList: "/CustomRule/GetAdditionalServicesListData?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId} &folderVersionId={folderVersionId}&formDesignId={formDesignId}&fullName={fullName}&filterFrom={filterFrom}",
        getAccumuNumberDropDownItem: "/CustomRule/GetAccumuNumberDropDownItem",
        getMandateServiceGroupingDetails: "/CustomRule/GetMandateServiceGroupingDetails",
        getAdditionalServicesDetails: "/CustomRule/GetAdditionalServicesDetails",
        getLimitsDeatails: "/CustomRule/GetLimitsDeatails",
        getLimitDescriptionForBRGService: "/CustomRule/GetLimitDescriptionForBRGService",
        getSERLAndDisallowedDropdownItems: "/CustomRule/GetSERLAndDisallowedDropdownItems",
        getSourceDataForLTSERepeaters: "/CustomRule/GetSourceDataForLTSERepeaters?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId} &folderVersionId={folderVersionId}&formDesignId={formDesignId}&fullName={fullName}&filterFrom={filterFrom}&LTLTData={LTLTData}",
        updateAuditGeneralInfo: "/CustomRule/UpdateAuditGeneralInfo",
        getServiceCommentDescriptionForBRGService: "/CustomRule/GetServiceCommentsDescriptionForBRGService",
        getMessageDropDownItem: "/CustomRule/GetMessageDropDownItem",
        getMasterListDeductibleAccumDescription: "/CustomRule/GetMasterListAccumDescription",
        getProductCategoryDropDownItems: "/CustomRule/GetProductCategoryDropDownItems",
        folderVersionDetailsNonPortfolioBasedAccount: '/FolderVersion/GetNonPortfolioBasedAccountFolders?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&folderType={folderType}&mode={mode}',
        getResearchWorkstationDetails: "/Research/Index?productId={productId}&folderVersion={folderVersion}",
        checkIfProductTranslated: "/Research/CheckIfProductTranslated?productId={productId}&effDate={effDate}&folderVersionNumber={folderVersionNumber}",
        updateBenefitSetGridLockStatus: "/CustomRule/UpdateBenefitSetGridLockStatus",
        getBenefitSummaryTablesList: "/CustomRule/GetBenefitSummaryTableGridData?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId} &folderVersionId={folderVersionId}&formDesignId={formDesignId}&fullName={fullName}&filterFrom={filterFrom}",
        getBenefitSummaryDetails: "/CustomRule/GetBenefitSummaryDetails",
        getBenSummaryDetailsTypeDropDownItem: "/CustomRule/GetBenSummaryDetailsTypeDropDownItem",
        getPDBCTypeDropDownItem: "/CustomRule/GetPDBCTypes",
        getPDBCPrefixValueDropDownItem: "/CustomRule/GetPDBCPrefixesForPDBCType",
        getSourceDataForLimitRepeaters: "/CustomRule/GetSourceDataForLimitRepeaters?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId} &folderVersionId={folderVersionId}&formDesignId={formDesignId}&fullName={fullName}&filterFrom={filterFrom}",
        getServiceRuleDataForServiceConfig: "/Translator/GetServiceRuleDataForServiceConfig",
        getModelSESE_IDDataForServiceConfig: "/CustomRule/GetModelSESE_IDDataForServiceConfig?tenantId={tenantId}&formInstanceId={formInstanceId}",
        getBenefitSetNameDropDownItem: "/CustomRule/GetBenefitSetNameDropDownItem",
        getServiceRulesDropDownItem: "/CustomRule/GetServiceRules",
        ruleAssignment: "/CustomRule/RuleAssignment",
        getBenefitReviewAltData: "/CustomRule/GetBenefitReviewAltData"
    }

    this.isAltRulesDialogInitialized = false;
    this.BenefitReviewGridAltRulesData = new Array();
    this.BenefitReviewAltRulesGridTierData = new Array();

    //DataSourceName
    this.ProductNetworkDS = "ProductNetworkList"

    //KeyName
    this.BenefitSetName = "BenefitSetName";
    this.NetworkDescription = "NetworkDescription";

    //FullPath        
    this.MessageServiceList = "Messages.MessageServiceList";



    this.BenefitReviewGridServiceCommentFullPath = "BenefitReview.BenefitReviewGrid.ServiceComment";



    this.sectionName = {
        ServiceGroupMandatesSectionName: "Service Group",
        AdditionalServicesSectionName: "Additional Services",
        deductibles: "Deductibles",
        benefitReview: "Benefit Review",
        LimitsSectionName: "Limits",
        auditChecklist: "Audit Checklist",
        auditCheckListGenrated: "AuditChecklist",
        generalAuditInfoGenrated: "GeneralAuditInfo",
        checkListDetailsGenrated: "CheckListDetails",
        shadowLimits: "ShadowLimits",
        benefitReviewGridGenrated: "BenefitReview",
        benefitReviewGridRepeaterGenrated: "BenefitReviewGrid",
        benefitReviewGridAltRulesDataGenrated: "BenefitReviewAltRulesGrid",
        benefitReviewAltRulesGridTierDataGenrated: "BenefitReviewAltRulesGridTierData",
        benefitReviewGridTierDataGenerated: "BenefitReviewGridTierData",
        productDefinition: "ProductDefinition",
        facetsProductInformation: "FacetsProductInformation",
        newProductID: "NewProductID",
        productDefinitionSectionName: "Product Definition",
        customRulesSettings: "CustomRulesSettings",
        benefitSetNetwork: "Benefit Set / Network",
        benefitSetNetworkGenerated: "BenefitSetNetwork",
        benefitSummary: "Benefit Summary",
        facetsProductComponentsPDBC: "FacetProductComponentsPDBC",
        generalInformation: "GeneralInformation"
    }

    this.elementName = {
        serviceGroup: "ServiceGroup",
        serviceGrouping: "ServiceGrouping",
        serviceGroupingDetails: "ServiceGroupingDetails",
        serviceGroupHeader: "ServiceGroupHeader",
        additionalServices: "AdditionalServices",
        additionalServicesDetails: "AdditionalServicesDetails",
        limits: "Limits",
        limitsList: "LimitsList",
        facetsLimits: "FacetsLimits",
        limitDescription: "LimitDescription",
        limitServicesLTSE: "LimitServicesLTSE",
        limitRulesLTLT: "LimitRulesLTLT",
        limitProcedureTableLTIP: "LimitProcedureTableLTIP",
        limitDiagnosisTableLTID: "LimitDiagnosisTableLTID",
        limitProviderTableLTPR: "LimitProviderTableLTPR",
        auditAssignedTo: "AssignedTo",
        auditCompletedBy: "CompletedBy",
        auditDateCompleted: "DateCompleted",
        auditAccelerated: "Accelerated",
        auditCheckListProductPoints: "ProductPoints",
        auditCheckListRXPoints: "RXPoints",
        auditCheckListDentalPoints: "DentalPoints",
        auditCheckListVisionPoints: "VisionPoints",
        auditCheckListStoplossPoints: "StoplossPoints",
        auditCheckListDEDEPoints: "DEDEPoints",
        auditCheckListLTLTPoints: "LTLTPoints",
        auditCheckListEBCLPoints: "EBCLPoints",
        auditCheckListSEPY1Points: "SEPY1Points",
        auditCheckListSEPY2Points: "SEPY2Points",
        auditCheckListSEPY3Points: "SEPY3Points",
        auditCheckListSEPY4Points: "SEPY4Points",
        auditCheckListSEPY5Points: "SEPY5Points",
        auditCheckListSEPY6Points: "SEPY6Points",
        auditCheckListBSBSBSDLPoints: "BSBSBSDLPoints",
        auditCheckListHRAAdminInfoPointsPoints: "HRAAdminInfoPoints",
        auditCheckListHRAAllocationRulesPoints: "HRAAllocationRulesPoints",
        auditCheckListMARISPoints: "MARISPoints",
        auditCheckListTotalPoint: "TotalPoint",
        auditCheckListAuditScore: "AuditScore",
        shadowServiceGroup: "ShadowServiceGroup",
        shadowServiceGrouping: "ShadowServiceGrouping",
        shadowServiceGroupDetail: "ShadowServiceGroupDetail",
        shadowAdditionalServices: "ShadowAdditionalServices",
        shadowAdditionalServicesDetails: "ShadowAdditionalServicesDetails",
        shadowLimitsList: "ShadowLimitsList",
        shadowFacetsLimits: "ShadowFacetsLimits",
        shadowLimitDescription: "ShadowLimitDescription",
        shadowLimitServicesLTSE: "ShadowLimitServicesLTSE",
        shadowLimitRulesLTLT: "ShadowLimitRulesLTLT",
        shadowLimitProcedureTableLTIP: "ShadowLimitProcedureTableLTIP",
        shadowLimitDiagnosisTableLTID: "ShadowLimitDiagnosisTableLTID",
        shadowLimitProviderTableLTPR: "ShadowLimitProviderTableLTPR",
        deductibles: "Deductibles",
        deductibleList: "DeductiblesList",
        benefitSetNetwork: "BenefitSetNetwork",
        networkList: "NetworkList",
        messageSERL: "MessageSERL",
        disallowedMessage: "DisallowedMessage",
        sERLDescription: "SERLDescription",
        disAllowedMessage: "DisAllowedMessage",
        productID: "ProductID",
        SERLmessage: "SERLMessage",
        typeofAccumulator: "TypeofAccumulator",
        isProductNew: "IsProductNew",
        altRuleAdditionalServicesDetails: "AltRuleAdditionalServicesDetails",
        altRuleServiceGroupDetail: "AltRuleServiceGroupDetail",
        generateNewProductID: "GenerateNewProductID",
        isBenefitSetGridLock: "IsBenefitSetGridLock",
        SEPYPFXDescription: "SEPYPFXDescription",
        LTLTPFXDescription: "LTLTPFXDescription",
        DEDEPFXDescription: "DEDEPFXDescription",
        deductibleAccumulator: "DeductibleAccumulator",
        benefitSummary: "BenefitSummary",
        BenefitSummaryType: "BenefitSummaryType",
        accumNumber: "AccumNumber",
        BanefitSummaryTable: "BenefitSummaryTable",
        BenefitSummaryText: "BenefitSummaryText",
        pdbcPrefix: "PDBCPrefix",
        pdbcType: "PDBCType",
        deductibleAccumDescription: "Description",
        premiumIndicator: "PremiumIndicator",
        pdbcPrefixList: "PDBCPrefixList",
        gridElementIdJQ: "gridElementIdJQ",
        covered: "Covered",
        productPeriodIndicator: "ProductPeriodIndicator",
        subSection: "Subsection"
    }

    this.fullName = {
        ServiceGrouping: "ServiceGroup.ServiceGrouping",
        mandateServiceGroupingDetailsRepeater: "ServiceGroup.ServiceGroupingDetails",
        altRuleServiceGroupDetailRepeater: "ServiceGroup.AltRuleServiceGroupDetail",
        additionalServicesList: "AdditionalServices.AdditionalServicesList",
        additionalServicesDetailsRepeater: "AdditionalServices.AdditionalServicesDetails",
        altRuleAdditionalServicesDetailsRepeater: "AdditionalServices.AltRuleAdditionalServicesDetails",
        limitsLimitsList: "Limits.LimitsList",
        limitServicesLTSE: "Limits.FacetsLimits.LimitServicesLTSE",
        limitRulesLTLT: "Limits.FacetsLimits.LimitRulesLTLT",
        limitProcedureTableLTIP: "Limits.FacetsLimits.LimitProcedureTableLTIP",
        limitDiagnosisTableLTID: "Limits.FacetsLimits.LimitDiagnosisTableLTID",
        limitProviderTableLTPR: "Limits.FacetsLimits.LimitProviderTableLTPR",
        auditCheckListProductPoints: "AuditChecklist.CheckListDetails.ProductPoints",
        auditCheckListRXPoints: "AuditChecklist.CheckListDetails.RXPoints",
        auditCheckListDentalPoints: "AuditChecklist.CheckListDetails.DentalPoints",
        auditCheckListVisionPoints: "AuditChecklist.CheckListDetails.VisionPoints",
        auditCheckListStoplossPoints: "AuditChecklist.CheckListDetails.StoplossPoints",
        auditCheckListDEDEPoints: "AuditChecklist.CheckListDetails.DEDEPoints",
        auditCheckListLTLTPoints: "AuditChecklist.CheckListDetails.LTLTPoints",
        auditCheckListEBCLPoints: "AuditChecklist.CheckListDetails.EBCLPoints",
        auditCheckListSEPY1Points: "AuditChecklist.CheckListDetails.SEPY1Points",
        auditCheckListSEPY2Points: "AuditChecklist.CheckListDetails.SEPY2Points",
        auditCheckListSEPY3Points: "AuditChecklist.CheckListDetails.SEPY3Points",
        auditCheckListSEPY4Points: "AuditChecklist.CheckListDetails.SEPY4Points",
        auditCheckListSEPY5Points: "AuditChecklist.CheckListDetails.SEPY5Points",
        auditCheckListSEPY6Points: "AuditChecklist.CheckListDetails.SEPY6Points",
        auditCheckListBSBSBSDLPoints: "AuditChecklist.CheckListDetails.BSBSBSDLPoints",
        auditCheckListHRAAdminInfoPointsPoints: "AuditChecklist.CheckListDetails.HRAAdminInfoPoints",
        auditCheckListHRAAllocationRulesPoints: "AuditChecklist.CheckListDetails.HRAAllocationRulesPoints",
        auditCheckListMARISPoints: "AuditChecklist.CheckListDetails.MARISPoints",
        productRulesFacetsProductInformationNewProductIDGenerateNewProductID: "ProductDefinition.FacetsProductInformation.NewProductID.GenerateNewProductID",
        shadowServiceGrouping: "ShadowServiceGroup.ShadowServiceGrouping",
        shadowServiceGroupingDetailsRepeater: "ShadowServiceGroup.ShadowServiceGroupDetail",
        shadowAdditionalServiceList: "ShadowAdditionalServices.ShadowAdditionalServicesList",
        shadowAdditionalServicesDetailsRepeater: "ShadowAdditionalServices.ShadowAdditionalServicesDetails",
        shadowLimitsList: "ShadowLimits.ShadowLimitsList",
        shadowLimitServicesLTSE: "ShadowLimits.FacetsLimits.ShadowLimitServicesLTSE",
        shadowLimitRulesLTLT: "ShadowLimits.FacetsLimits.ShadowLimitRulesLTLT",
        shadowLimitProcedureTableLTIP: "ShadowLimits.FacetsLimits.ShadowLimitProcedureTableLTIP",
        shadowLimitDiagnosisTableLTID: "ShadowLimits.FacetsLimits.ShadowLimitDiagnosisTableLTID",
        shadowLimitProviderTableLTPR: "ShadowLimits.FacetsLimits.ShadowLimitProviderTableLTPR",
        benefitReviewGrid: "BenefitReview.BenefitReviewGrid",
        benefitReviewGridAltRulesData: "BenefitReview.BenefitReviewAltRulesGrid",
        benefitReviewGridTierData: "BenefitReview.BenefitReviewGridTierData",
        benefitReviewAltRulesGridTierData: "BenefitReview.BenefitReviewAltRulesGridTierData",
        shadowBenefitReviewGridTierData: "ShadowBenefitReview.ShadowBenefitReviewGridTierData",
        shadowBenefitReviewAltRulesGrid: "ShadowBenefitReview.ShadowBenefitReviewAltRulesGrid",
        shadowBenefitReviewAltRulesGridTierData: "ShadowBenefitReview.ShadowBenefitReviewAltRulesGridTierData",
        benefitReviewLimits: "BenefitReview.BenefitReviewGrid.Limits",
        benefitReviewAltRule: "BenefitReview.BenefitReviewGrid.AltRule",
        benefitReviewTier: "BenefitReview.BenefitReviewGrid.Tier",
        benefitAltRuleGridTier: "BenefitReview.BenefitReviewAltRulesGrid.Tier",
        brgdeductibleAccum: "BenefitReview.BenefitReviewGrid.DeductibleAccumulator",
        productLimitAccumNo: "Limits.FacetsLimits.LimitRulesLTLT.AccumNumber",
        productDeductibleAccumNo: "Deductibles.DeductiblesList.AccumNumber",
        productDeductibleRelatedAccumNo: "Deductibles.DeductiblesList.RelatedAccumNumber",
        MasterListLimitAccumNo: "Limits.LimitRulesLTLT.AccumNumber",
        additionalServicesDetailsMessageSERL: "AdditionalServices.AdditionalServicesDetails.MessageSERL",
        additionalServicesDetailsDisallowedMessages: "AdditionalServices.AdditionalServicesDetails.DisallowedMessage",
        additionalServicesDetailsCovered: "AdditionalServices.AdditionalServicesDetails.Covered",
        serviceGroupMessageSERL: "ServiceGroup.ServiceGroupingDetails.MessageSERL",
        serviceGroupDisallowedMessage: "ServiceGroup.ServiceGroupingDetails.DisallowedMessage",
        serviceGroupCovered: "ServiceGroup.ServiceGroupingDetails.Covered",
        altRuleServiceGroupDetailRepeater: "ServiceGroup.AltRuleServiceGroupDetail",
        altRuleServiceGroupDetailMessageSERL: "ServiceGroup.AltRuleServiceGroupDetail.MessageSERL",
        altRuleServiceGroupDetailDisallowedMessage: "ServiceGroup.AltRuleServiceGroupDetail.DisallowedMessage",
        altRuleServiceGroupDetailCovered: "ServiceGroup.AltRuleServiceGroupDetail.Covered",
        altRuleAdditionalServicesDetailsRepeater: "AdditionalServices.AltRuleAdditionalServicesDetails",
        altRuleAdditionalServicesDetailsMessageSERL: "AdditionalServices.AltRuleAdditionalServicesDetails.MessageSERL",
        altRuleAdditionalServicesDetailsDisallowedMessage: "AdditionalServices.AltRuleAdditionalServicesDetails.DisallowedMessage",
        altRuleAdditionalServicesDetailsCovered: "AdditionalServices.AltRuleAdditionalServicesDetails.Covered",
        benefitReviewGridAltRulesDataMessageSERL: "BenefitReview.BenefitReviewAltRulesGrid.MessageSERL",
        benefitReviewGridAltRulesDataDisallowedMessage: "BenefitReview.BenefitReviewAltRulesGrid.DisallowedMessage",
        benefitReviewGridAltRulesDataCovered: "BenefitReview.BenefitReviewAltRulesGrid.Covered",
        benefitReviewGridAltRulesDataSERLMessage: "BenefitReview.BenefitReviewAltRulesGrid.SERLMessage",
        productContainsShadow: "ProductDefinition.GeneralInformation.ContainsShadow",
        productRulesFacetsProductInformationProductCategory: "ProductDefinition.FacetsProductInformation.ProductCategory",
        productRulesFacetsProductInformationProductLineofBusiness: "ProductDefinition.FacetsProductInformation.ProductLineofBusiness",
        benefitSummaryAccumulationDataforDeductiblesandLimits: "BenefitSummary.AccumulationDataforDeductiblesandLimits",
        benefitSummaryTable: "BenefitSummary.BenefitSummaryTable",
        benefitSummaryText: "BenefitSummary.BenefitSummaryText",
        benefitSummaryType: "BenefitSummary.BenefitSummaryType",
        networkListBenefitSetName: "BenefitSetNetwork.NetworkList.BenefitSetName",
        networkList: "BenefitSetNetwork.NetworkList",
        SEPYPFXDescription: "BenefitSetNetwork.NetworkList.SEPYPFXDescription",
        LTLTPFXDescription: "BenefitSetNetwork.NetworkList.LTLTPFXDescription",
        DEDEPFXDescription: "BenefitSetNetwork.NetworkList.DEDEPFXDescription",
        facetProductVariableComponent: "BenefitSetNetwork.FacetsVariableComponentPDVC.FacetProductVariableComponent",
        benefitSummaryDetailTableDetailsType: "BenefitSummary.BenefitSummaryDetailTable.DetailsType",
        benefitCategory1List: "BenefitCategories.BenefitCategory1List",
        benefitCategory2List: "BenefitCategories.BenefitCategory2List",
        benefitCategory3List: "BenefitCategories.BenefitCategory3List",
        placeOfServiceMasterList: "BenefitCategories.PlaceofServiceList",
        seseIDListMasterList: "BenefitCategories.SESEIDList",
        serviceGroupHeaderMasterList: "ServiceGroupDefinition.ServiceGroupingHeader",
        facetProductComponentsPDBCRepeater: "ProductDefinition.FacetsProductInformation.FacetProductComponentsPDBC.PDBCPrefixList",
        pdbcTypeFullName: "ProductDefinition.FacetsProductInformation.FacetProductComponentsPDBC.PDBCPrefixList.PDBCType",
        LimitProcedureTableLTIPAccumNumber: "Limits.FacetsLimits.LimitProcedureTableLTIP.AccumNumber",
        LimitDiagnosisTableLTIDAccumNumber: "Limits.FacetsLimits.LimitDiagnosisTableLTID.AccumNumber",
        LimitProviderTableLTPRAccumNumber: "Limits.FacetsLimits.LimitProviderTableLTPR.AccumNumber",
        deductibleList: "Deductibles.DeductiblesList",
        serviceGroupingChildServiceDetail: "ServiceGroupDefinition.ServiceGroupingDetail.MasterListServices",
        serviceGroupingDetailServiceConfiguration: "ServiceGroupDefinition.ServiceGroupingDetail.ServiceConfiguration",
        benefitReviewAltRulesGridDeductibleAccumulator: "BenefitReview.BenefitReviewAltRulesGrid.DeductibleAccumulator",
        benefitReviewGridTierDataDeductibleAccumulator: "BenefitReview.BenefitReviewGridTierData.DeductibleAccumulator",
        benefitReviewAltRulesGridTierDataDeductibleAccumulator: "BenefitReview.BenefitReviewAltRulesGridTierData.DeductibleAccumulator",
        premiumIndicator: "ProductDefinition.FacetsProductInformation.PremiumIndicator",
        periodIndicator: "ProductDefinition.GeneralInformation.ProductPeriodIndicator",
        limitRuleLTLTRelations: "Limits.FacetsLimits.LimitRulesLTLT.Relations"

    }

    this.KeyName = {
        SESEID: "SESEID",
        BenefitCategory1: "BenefitCategory1",
        BenefitCategory2: "BenefitCategory2",
        BenefitCategory3: "BenefitCategory3",
        PlaceofService: "PlaceofService",
        prodCategoryMessage: "Change in Benefit Set is tied to Product ID generation.Please check the 'Generate New Product' checkbox if you intend to create a new Product ID.",
        lobSwitchIndicatorMessage: "Change in Line Of Business is tied to Product ID generation.Please check the 'Generate New Product' checkbox if you intend to create a new Product ID."

    }

    this.elemenIDs = {
        serviceGroupingMandateDetailsRepeater: '#PRO3Repeater393',
        additionalServicesDetailsRepeater: '#PRO3Repeater526',
        limitLTSERepeater: '#PRO3Repeater461',
        limitLTLTRepeater: '#PRO3Repeater471',
        limitLTIPRepeater: '#PRO3Repeater485',
        limitLTIDRepeater: '#PRO3Repeater490',
        limitLTPRRepeater: '#PRO3Repeater495',
        limitsListsRepeater: '#PRO3Repeater453',
        serviceGroupBenefitSetNameDDLJQ: '#PRO3DropDown392',
        additionalServiceBenefitSetNameDDLJQ: '#PRO3DropDown525',
        searchMandesServiceGropDetailID: 'PRO3TextBox743',
        searchMandesServiceGropDetailIDJQ: '#PRO3TextBox743',
        resetMandesServiceGropDetailID: 'resetPRO3TextBox744',
        resetMandesServiceGropDetailIDJQ: '#resetPRO3TextBox744',
        serviceGroupingDDLJQ: '#PRO3DropDown388',
        serviceGroupingbenefitSetDDLJQ: '#PRO3DropDown389',
        searchAdditionalServiceGropDetailID: 'PRO3TextBox745',
        searchAdditionalServiceGropDetailIDJQ: '#PRO3TextBox745',
        resetAdditionalServiceGropDetailID: 'resetPRO3TextBox746',
        resetAdditionalServiceGropDetailIDJQ: '#resetPRO3TextBox746',
        benefitSetAdditionalServiceGropDDLJQ: '#PRO3DropDown523',
        searchLimitListID: 'PRO3TextBox499',
        searchLimitListIDJQ: '#PRO3TextBox499',
        resetLimitListID: 'resetPRO3TextBox500',
        resetLimitListIDJQ: '#resetPRO3TextBox500',
        limitsBenefitSetDDLJQ: '#PRO3DropDown452',
        bulkUpdateServiceGroupingDDLJQ: '#PRO3DropDown391',
        messageServiceListRepeater: '#PRO3Repeater502',
        benefitSetBulkAdditionalServiceGropDDLJQ: '#PRO3DropDown525',
        serviceGroupingBulkbenefitSetDDLJQ: '#PRO3DropDown392',
        auditTotalPoint: '#PRO3TextBox811',
        auditScore: '#PRO3TextBox814',
        auditAssignedToJQ: '#PRO3TextBox635',
        auditCompletedByJQ: '#PRO3TextBox638',
        auditDateCompletedJQ: '#PRO3Calendar749',
        auditAcceleratedJQ: '#PRO3TextBox832',
        generateNewProductIdJQ: "#PRO3CheckBox2253",
        ProductIdJQ: "#PRO3TextBox322",
        isProductNewJQ: "#PRO3CheckBox2254",
        shadowServiceGroupingDetailsRepeater: '#PRO3Repeater878',
        shadowAdditionalServicesDetailsRepeater: '#PRO3Repeater2021',
        shadowLimitLTSERepeater: '#PRO3Repeater1932',
        shadowLimitLTLTRepeater: '#PRO3Repeater1926',
        shadowLimitLTIPRepeater: '#PRO3Repeater1929',
        shadowLimitLTIDRepeater: '#PRO3Repeater1930',
        shadowLimitLTPRRepeater: '#PRO3Repeater1931',
        shadowLimitsListsRepeater: '#PRO3Repeater1905',
        benefitReviewGridTierDataJQ: "#PRO3Repeater592",
        benefitReviewGridAltRulesDataJQ: "#PRO3Repeater2270",
        benefitReviewGridAltRuleGridTierDataJQ: "#PRO3Repeater1995",
        newproductId: "#PRO3TextBox322",
        altRuleServiceGroupDetailRepeater: "#PRO3Repeater845",
        altRuleAdditionalServicesDetailsRepeater: "#PRO3Repeater1928",
        benefitReviewAltRulesGridRepeater: "#PRO3Repeater2270",
        standardProduct: "#PRO3DropDown2323",
        benefitSummaryAccumulationDataforDeductiblesandLimitsRepeater: "#PRO3Repeater2218",
        altRuleAdditionalServicesDetailsRepeater: "#PRO3Repeater1928",
        altRuleServiceGroupDetail: "#PRO3Repeater845",
        productCategory: "#PRO3DropDown2310",
        referenceFormLinkLabelElementId: "#REF8TextBox2405",
        referenceFormFolderVersionId: "#REF8TextBox2408",
        referenceFormFolderId: "#REF8TextBox2407",
        referenceFormFormInstanceId: "#REF8TextBox2409",
        referenceResearchWorkstationLink: "#PRO3TextBox2503",
        benefitSummaryTextGrid: "#PRO3Repeater2478",
        benefitSetNetworkRepeater: "#repeaterPRO3Repeater351",
        benefitSetNetwork: "#PRO3Repeater351",
        BenefitSummaryTextRepeater: "#PRO3Repeater2478",
        ProductLineOfBusiness: '#PRO3DropDown324',
        facetComponentsPDBCRepeater: '#PRO3Repeater346',
        serviceGroupSection: 'PRO3Section383',
        additionalServices: "PRO3Section513"
    }

    this.masterListSectionName = {
        serviceGroupDefinition: 'ServiceGroupDefinition',
        serviceRuleConfigurationList: 'ServiceRuleConfigurationList'
    }

    this.masterListElementIDs = {
        serviceConfigDialogJQ: "#ServiceConfigurationDialog",
        defaultRuleJQ: "#defaultRule",
        modelSESE_IDJQ: "#modelSESE_ID",
        defaultServiceRuleConfigJQ: "#DefaultServiceRuleConfig",
        regulerRuleDDLJQ: "#regularRule",
        alternateRuleDDLJQ: "#alternateRule",
        modleSESE_IDRuleConfigJQ: "#ModleSESE_IDRuleConfig",
        serviceSESE_IDJQ: "#serviceSESE_ID",
        limitServiceConfigJQ: "#LimitServiceConfig",
        accumulatorsJQ: "#accumulators",
        limitModelSESE_IDJQ: "#LimitModelSESE_ID",
        accumulatorsRuleConfigJQ: "#accumulatorsRuleConfig",
        accumulatorsGridJQ: "#accumulatorsGrid",
        accumulatorsGrid: "accumulatorsGrid",
        limitModelSESE_IDConfigJQ: "#limitModelSESE_IDConfig",
        limitServiceSESE_IDJQ: "#LimitServiceSESE_ID",
        serviceRuleConfigurationListGridData: "#MAS1Repeater2610",
        serviceGroupDetailsGridJQ: "#MAS1Repeater149",
        serviceGroupDetailsGridParentDivJQ: "#repeaterMAS1Repeater149",
        serviceConfigDialogSave: "#ServiceConfigDialogSave",
        serviceConfigDialogClose: "#ServiceConfigDialogClose",
        altRuleConditionJQ: "#altRuleCondition",
        serviceConfigurationHelpBlockJQ: '#serviceConfigurationHelpBlock',
        serviceGroupHeaderJQ: '#serviceGroupHeader',
        SESEIDJQ: '#SESEID'
    }
}

customRulePQ.prototype.hasMasterList = function (formDesignId) {
    var result = false;
    if (formDesignId == FormTypes.MASTERLISTFORMID) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.hasProduct = function (formDesignId) {
    var result = false;
    if (formDesignId == FormTypes.PRODUCTFORMDESIGNID) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.hideAddButtonforRepeater = function (repeaterName) {
    var result = true;

    if (repeaterName == this.fullName.mandateServiceGroupingDetailsRepeater || repeaterName == this.fullName.benefitReviewGrid
        //|| repeaterName == this.fullName.limitServicesLTSE
        || repeaterName == this.fullName.limitRulesLTLT
        //|| repeaterName == this.fullName.limitProcedureTableLTIP
        //|| repeaterName == this.fullName.limitDiagnosisTableLTID
        //|| repeaterName == this.fullName.limitProviderTableLTPR
        ) {
        result = false;
    }
    return result;
}

customRulePQ.prototype.hideRemoveButtonforRepeater = function (repeaterName) {
    var result = true;

    if (repeaterName == this.fullName.benefitCategory1List || repeaterName == this.fullName.benefitCategory2List
        || repeaterName == this.fullName.benefitCategory3List || repeaterName == this.fullName.placeOfServiceMasterList || repeaterName == this.fullName.serviceGroupHeaderMasterList) {
        result = false;
    }
    return result;
}

customRulePQ.prototype.checkCustomRepeater = function (currentInstance) {
    var repeaterName = currentInstance.fullName;
    var result = false;
    if (currentInstance.customrule.hasProduct(currentInstance.formInstanceBuilder.formDesignId)) {
        if (repeaterName == this.fullName.mandateServiceGroupingDetailsRepeater || repeaterName == this.fullName.additionalServicesDetailsRepeater
            || repeaterName == this.fullName.benefitReviewGridAltRulesData || repeaterName == this.fullName.benefitReviewGridTierData ||
            repeaterName == this.fullName.benefitReviewAltRulesGridTierData || repeaterName == this.fullName.benefitSummaryAccumulationDataforDeductiblesandLimits || repeaterName == this.fullName.benefitSummaryText) {
            result = true;
        }
    }
    return result;
}

customRulePQ.prototype.isLimitList = function (fullName) {
    var result = false;
    if (fullName == "Limits.LimitsList") {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isCustomRulesOnRowSelectOfRepeater = function (fullName) {
    var result = false;
    if (fullName == this.fullName.benefitSummaryAccumulationDataforDeductiblesandLimits || fullName == this.fullName.benefitSummaryTable
        //|| fullName == this.fullName.benefitReviewGridAltRulesData || fullName == this.fullName.benefitReviewGridTierData
        //|| fullName == this.fullName.benefitReviewAltRulesGridTierData
        || fullName == this.fullName.limitsLimitsList || fullName == this.fullName.facetProductComponentsPDBCRepeater
        || fullName == this.fullName.limitRulesLTLT
        )
        result = true;
    return result;
}

customRulePQ.prototype.isUpdateDeductibleAccumulatorDropdownItem = function (fullName) {
    var result = false;
    if (fullName == this.fullName.brgdeductibleAccum) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isRuleOnRepeaterPropertyChangeForProductForm = function (fullName) {
    var result = false;
    if (fullName == this.fullName.additionalServicesDetailsRepeater || fullName == this.fullName.mandateServiceGroupingDetailsRepeater
         || fullName == this.fullName.altRuleServiceGroupDetailRepeater || fullName == this.fullName.altRuleAdditionalServicesDetailsRepeater
        || fullName == this.fullName.benefitReviewGridAltRulesData || fullName == this.fullName.facetProductComponentsPDBCRepeater || fullName == this.fullName.deductibleList || fullName == this.fullName.limitRulesLTLT) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isRuleOnRepeaterLoadingForProductForm = function (fullName) {
    var result = false;
    if (fullName == this.fullName.additionalServicesDetailsRepeater || fullName == this.fullName.mandateServiceGroupingDetailsRepeater
        || fullName == this.fullName.altRuleServiceGroupDetailRepeater || fullName == this.fullName.altRuleAdditionalServicesDetailsRepeater
        || fullName == this.fullName.benefitReviewGridAltRulesData || fullName == this.fullName.benefitSummaryAccumulationDataforDeductiblesandLimits
        || fullName == this.fullName.limitRulesLTLT
        ) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isProductCustomDropdownItem = function (fullName) {
    var result = false;
    if (fullName == this.fullName.productDeductibleAccumNo || fullName == this.fullName.productDeductibleRelatedAccumNo
        || fullName == this.fullName.benefitSummaryDetailTableDetailsType //|| fullName == this.fullName.additionalServicesDetailsMessageSERL
        // || fullName == this.fullName.additionalServicesDetailsDisallowedMessages || fullName == this.fullName.serviceGroupMessageSERL
        // || fullName == this.fullName.serviceGroupDisallowedMessage || fullName == this.fullName.altRuleServiceGroupDetailMessageSERL
        // || fullName == this.fullName.altRuleServiceGroupDetailDisallowedMessage || fullName == this.fullName.altRuleAdditionalServicesDetailsMessageSERL
        // || fullName == this.fullName.altRuleAdditionalServicesDetailsDisallowedMessage         
        || fullName == this.fullName.benefitReviewAltRulesGridDeductibleAccumulator || fullName == this.fullName.benefitReviewGridTierDataDeductibleAccumulator
        || fullName == this.fullName.benefitReviewAltRulesGridTierDataDeductibleAccumulator
        || fullName == this.fullName.benefitSummaryDetailTableDetailsType || fullName == this.fullName.pdbcTypeFullName
        || fullName == this.fullName.LimitProcedureTableLTIPAccumNumber || fullName == this.fullName.LimitDiagnosisTableLTIDAccumNumber || fullName == this.fullName.LimitProviderTableLTPRAccumNumber) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isMasterListLimitRulesLTLTAccumulatorNoDropdown = function (fullName) {
    var result = false;
    if (fullName == this.fullName.MasterListLimitAccumNo) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isProductFormMessageRepeaterBuilder = function (fullName) {
    var result = false;
    if (fullName == this.fullName.additionalServicesDetailsRepeater || fullName == this.fullName.mandateServiceGroupingDetailsRepeater
        || fullName == this.fullName.altRuleServiceGroupDetailRepeater || fullName == this.fullName.altRuleAdditionalServicesDetailsRepeater) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isDropdownListValueTobeGetOnGridRowEditMode = function (fullName) {
    var result = false;
    if (fullName == this.fullName.additionalServicesDetailsRepeater || fullName == this.fullName.mandateServiceGroupingDetailsRepeater
        || fullName == this.fullName.altRuleServiceGroupDetailRepeater || fullName == this.fullName.altRuleAdditionalServicesDetailsRepeater
        //|| fullName == this.fullName.benefitReviewGridTierData || fullName == this.fullName.benefitReviewAltRulesGridTierData
        //|| fullName == this.fullName.benefitReviewGridAltRulesData
        || fullName == this.fullName.facetProductComponentsPDBCRepeater) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isProductCustomFormatter = function (fullName) {
    var result = false;
    if (fullName == this.BenefitReviewGridServiceCommentFullPath || fullName == this.fullName.benefitReviewLimits
        || fullName == this.fullName.benefitReviewAltRule || fullName == this.fullName.benefitReviewTier
        || fullName == this.fullName.benefitAltRuleGridTier) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isMasterListCustomFormatter = function (fullName) {
    var result = false;
    if (fullName == this.fullName.serviceGroupingDetailServiceConfiguration) {
        result = true;
    }
    return result;
}
customRulePQ.prototype.isRegisterButtonOnProductFormGrid = function (fullName) {
    var result = false;
    if (fullName == this.fullName.benefitReviewGrid || fullName == this.fullName.mandateServiceGroupingDetailsRepeater
        || fullName == this.fullName.additionalServicesDetailsRepeater
        || fullName == this.fullName.benefitReviewGridAltRulesData) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isServiceGroupMandateORAdditionalServicesSection = function (fullName) {
    var result = false;
    if (fullName == this.sectionName.ServiceGroupMandatesSectionName || fullName == this.sectionName.AdditionalServicesSectionName || fullName == this.sectionName.LimitsSectionName) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isRegisterButtonOnMasterListGrid = function (fullName) {
    var result = false;
    if (fullName == this.fullName.serviceGroupingChildServiceDetail) {
        result = true;
    }
    return result;
}
customRulePQ.prototype.isProductDefinition = function (fullName) {
    var result = false;
    if (fullName == this.sectionName.productDefinitionSectionName) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isBenefitSetGridLock = function (formInstanceBuilder) {
    var result = false;
    if (formInstanceBuilder.formData[this.sectionName.customRulesSettings][this.elementName.isBenefitSetGridLock] != "Yes") {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isProductRepeaterCollapseByDefault = function (fullName) {
    var result = false;
    if (//fullName == this.fullName.benefitReviewGridAltRulesData || fullName == this.fullName.benefitReviewGridTierData || fullName == this.fullName.benefitReviewAltRulesGridTierData ||
        fullName == this.fullName.shadowBenefitReviewGridTierData || fullName == this.fullName.shadowBenefitReviewAltRulesGrid || fullName == this.fullName.shadowBenefitReviewAltRulesGridTierData) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isProductPropertyChangedElements = function (fullName) {
    var result = false;
    if (fullName == this.fullName.auditCheckListProductPoints || fullName == this.fullName.auditCheckListRXPoints
    || fullName == this.fullName.auditCheckListDentalPoints || fullName == this.fullName.auditCheckListVisionPoints || fullName == this.fullName.auditCheckListStoplossPoints
    || fullName == this.fullName.auditCheckListDEDEPoints || fullName == this.fullName.auditCheckListLTLTPoints || fullName == this.fullName.auditCheckListEBCLPoints || fullName == this.fullName.auditCheckListSEPY1Points
    || fullName == this.fullName.auditCheckListSEPY2Points || fullName == this.fullName.auditCheckListSEPY3Points || fullName == this.fullName.auditCheckListSEPY4Points
    || fullName == this.fullName.auditCheckListSEPY5Points || fullName == this.fullName.auditCheckListSEPY6Points || fullName == this.fullName.auditCheckListBSBSBSDLPoints
    || fullName == this.fullName.auditCheckListHRAAdminInfoPointsPoints || fullName == this.fullName.auditCheckListHRAAllocationRulesPoints || fullName == this.fullName.auditCheckListMARISPoints
    || fullName == this.fullName.productRulesFacetsProductInformationNewProductIDGenerateNewProductID || fullName == this.fullName.productContainsShadow || fullName == this.fullName.productRulesFacetsProductInformationProductCategory || fullName == this.fullName.productRulesFacetsProductInformationProductLineofBusiness || fullName == this.fullName.premiumIndicator || fullName == this.fullName.periodIndicator) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isAuditWorkFlow = function (id) {
    var result = false;
    if (id == WorkFlowState.FACETSTestAudit) {
        result = true;
    }
    return result;
}
customRulePQ.prototype.isFacetProdWorkFlow = function (id) {
    var result = false;
    if (id == WorkFlowState.FACETSPROD) {
        result = true;
    }
    return result;
}


customRulePQ.prototype.isSectionNameToBeDisabledOnCondition = function (fullName) {
    var result = false;
    if (fullName == this.sectionName.auditChecklist
        || fullName == this.sectionName.ServiceGroupMandatesSectionName
        || fullName == this.sectionName.AdditionalServicesSectionName
        || fullName == this.sectionName.deductibles
        || fullName == this.sectionName.benefitReview
        || fullName == this.sectionName.LimitsSectionName
        || fullName == this.sectionName.benefitSetNetwork
        || fullName == this.sectionName.benefitSummary
        || fullName == this.sectionName.productDefinitionSectionName) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isRepeaterToRemoveDefaultRow = function (fullName) {
    var result = false;
    if (fullName == this.fullName.altRuleServiceGroupDetailRepeater || fullName == this.fullName.mandateServiceGroupingDetailsRepeater
        || fullName == this.fullName.additionalServicesList || fullName == this.fullName.additionalServicesDetailsRepeater
        || fullName == this.fullName.altRuleAdditionalServicesDetailsRepeater || fullName == this.fullName.benefitReviewGrid
        || fullName == this.fullName.benefitReviewGridAltRulesData || fullName == this.fullName.benefitReviewGridTierData
        || fullName == this.fullName.benefitReviewAltRulesGridTierData || fullName == this.fullName.ServiceGrouping) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.isCustomRuleOnSectionLoad = function (fullName) {
    var result = false;
    if (fullName == this.sectionName.benefitSetNetwork) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.updateAccumNumberDropDownItem = function (designData, formInstanceID, folderVersionId, elementdesign) {
    var postData = {
        tenantId: designData.TenantID,
        formInstanceId: formInstanceID,
        formDesignVersionId: designData.FormDesignVersionId,
        folderVersionId: folderVersionId,
        formDesignId: designData.FormDesignId,
        fullName: elementdesign.FullName
    }

    var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getAccumuNumberDropDownItem, postData);

    promise.done(function (result) {
        elementdesign.Items = JSON.parse(result);
    });

    return elementdesign.Items;
}

customRulePQ.prototype.loadCustomDataInRepeaterMethods = function () {
    var currentInstance = this;

    return {
        runCustomRuleForProduct: function (repeaterBuilder, repeaterSelectedData, dataSourceMapping, repeaterBuilders) {
            switch (repeaterBuilder.fullName) {
                case currentInstance.fullName.ServiceGrouping:
                    currentInstance.loadCustomDataInRepeater.mandateServiceGroupingDetails(repeaterBuilder, repeaterSelectedData, dataSourceMapping, repeaterBuilder.fullName);
                    break;

                case currentInstance.fullName.additionalServicesDetailsRepeater:
                    currentInstance.loadCustomDataInRepeater.additionalServicesDetails(repeaterBuilder, repeaterSelectedData, dataSourceMapping, repeaterBuilder.fullName);
                    break;

                case currentInstance.fullName.limitsLimitsList:
                    currentInstance.loadCustomDataInRepeater.limitsDetails(repeaterBuilder, repeaterSelectedData, dataSourceMapping, repeaterBuilder.fullName);
                    break;
                case currentInstance.fullName.shadowServiceGrouping:
                    //  currentInstance.loadCustomDataInRepeater.shadowServiceGroupingDetails(repeaterBuilder, repeaterSelectedData, repeaterBuilder.fullName);
                    break;
                case currentInstance.fullName.shadowAdditionalServiceList:
                    // currentInstance.loadCustomDataInRepeater.shadowAdditionalServicesDetails(repeaterBuilder, repeaterSelectedData, repeaterBuilder.fullName);
                    break;
                case currentInstance.fullName.shadowLimitsList:
                    //  currentInstance.loadCustomDataInRepeater.shadowLimitsDetails(repeaterBuilder, repeaterSelectedData, repeaterBuilder.fullName);
                    break;
                case currentInstance.fullName.benefitSummaryTable:
                    currentInstance.loadCustomDataInRepeater.BenefitSummaryDetails(repeaterBuilder, repeaterSelectedData, dataSourceMapping, repeaterBuilder.fullName);
                    break;
                case currentInstance.fullName.limitServicesLTSE:
                    currentInstance.loadCustomDataInRepeater.limitServicesLTSE(repeaterBuilder, repeaterSelectedData, dataSourceMapping, repeaterBuilder.fullName);
                    break;
                default:
                    currentInstance.isActivityLogEntries = false;
                    break;
            }
        },

        mandateServiceGroupingDetails: function (repeaterBuilder, repeaterSelectedData, dataSourceMapping, fullName) {
            var customRuleInstance = repeaterBuilder.customrule;
            var oldData = repeaterBuilder.data;

            if (repeaterSelectedData.length == 0) {
                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.serviceGroup][customRuleInstance.elementName.serviceGroupingDetails] = [];
              
                $(currentInstance.elemenIDs.serviceGroupingMandateDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.serviceGroupingMandateDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.serviceGroup][customRuleInstance.elementName.altRuleServiceGroupDetail] = [];
                
                $(currentInstance.elemenIDs.altRuleServiceGroupDetail + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.altRuleServiceGroupDetail + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                return false;
            }

            var serviceGroupHeader = repeaterSelectedData[0][customRuleInstance.elementName.serviceGroupHeader].trim();
            if (repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.serviceGroup][customRuleInstance.elementName.serviceGrouping][0] != undefined) {
                if (repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.serviceGroup][customRuleInstance.elementName.serviceGrouping][0].ServiceGroupHeader == serviceGroupHeader) {
                    return false;
                }
            }

            var postData = {
                tenantId: repeaterBuilder.tenantId,
                formInstanceId: repeaterBuilder.formInstanceId,
                formDesignVersionId: repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                folderVersionId: repeaterBuilder.formInstanceBuilder.folderVersionId,
                formDesignId: repeaterBuilder.formInstanceBuilder.formDesignId,
                mandates: serviceGroupHeader,
                fullName: fullName
            }

            var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.getMandateServiceGroupingDetails, postData);

            promise.done(function (result) {
                var mandateMappings = JSON.parse(result);
                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.serviceGroup][customRuleInstance.elementName.serviceGroupingDetails] = mandateMappings;
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.mandateServiceGroupingDetailsRepeater;
                });

                if (mandateMappings.length > 0)
                    currentInstance.addActivityLogEntries(repeaterBuilder, repeaterSelectedData, dataSourceMapping, oldData);

                targetRepeater[0].data = mandateMappings;
                //$(currentInstance.elemenIDs.serviceGroupingMandateDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.serviceGroupingMandateDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.serviceGroupingMandateDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.serviceGroupingMandateDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: mandateMappings });
                $(currentInstance.elemenIDs.serviceGroupingMandateDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.serviceGroupingMandateDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: mandateMappings }).trigger("reloadGrid");

                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.serviceGroup][customRuleInstance.elementName.altRuleServiceGroupDetail] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.altRuleServiceGroupDetailRepeater;
                });

                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.altRuleServiceGroupDetail + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.altRuleServiceGroupDetail + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.altRuleServiceGroupDetail + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.altRuleServiceGroupDetail + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data:  targetRepeater[0].data });
                $(currentInstance.elemenIDs.altRuleServiceGroupDetail + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.altRuleServiceGroupDetail + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: targetRepeater[0].data }).trigger("reloadGrid");
            });
        },

        additionalServicesDetails: function (repeaterBuilder, repeaterSelectedData, dataSourceMapping, fullName) {
            var customRuleInstance = repeaterBuilder.customrule;
            var selectedServices = [];
            var oldData = repeaterBuilder.data;

            //for (var i = 0; i < repeaterSelectedData.length; i++) {
            //    selectedServices.push("" + repeaterSelectedData[i] + "");
            //}

            additonalServicesDetailsData = repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.additionalServices][customRuleInstance.elementName.additionalServicesDetails];
            altRuleAdditionalServiceData = repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.additionalServices][customRuleInstance.elementName.altRuleAdditionalServicesDetails];

            if (repeaterSelectedData.length == 0) {
                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.additionalServices][customRuleInstance.elementName.additionalServicesDetails] = [];
                //$(currentInstance.elemenIDs.additionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.additionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.additionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.additionalServices][customRuleInstance.elementName.altRuleAdditionalServicesDetails] = [];
                //$(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                return false;
            }

            var postData = {
                tenantId: repeaterBuilder.tenantId,
                formInstanceId: repeaterBuilder.formInstanceId,
                formDesignVersionId: repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                folderVersionId: repeaterBuilder.formInstanceBuilder.folderVersionId,
                formDesignId: repeaterBuilder.formInstanceBuilder.formDesignId,
                selectedServices: JSON.stringify(repeaterSelectedData),
                fullName: fullName,
                additonalServicesDetailsData: JSON.stringify(additonalServicesDetailsData),
                altRuleAdditionalServiceData: JSON.stringify(altRuleAdditionalServiceData)
            }

            var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.getAdditionalServicesDetails, postData);

            promise.done(function (result) {
                var additionalServicesAndAltRuleService = JSON.parse(result);
                var additionalServices = JSON.parse(additionalServicesAndAltRuleService["AdditionalServices"]);
                var altRuleAdditionalServices = JSON.parse(additionalServicesAndAltRuleService["AltAdditionalServices"]);
                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.additionalServices][customRuleInstance.elementName.additionalServicesDetails] = additionalServices;

                if (additionalServices.length > 0)
                    currentInstance.addActivityLogEntries(repeaterBuilder, repeaterSelectedData, dataSourceMapping, oldData);

                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.additionalServicesDetailsRepeater;
                });

                targetRepeater[0].data = additionalServices;
                //$(currentInstance.elemenIDs.additionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');

               // $(currentInstance.elemenIDs.additionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: additionalServices }).trigger("reloadGrid");

                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.additionalServices][customRuleInstance.elementName.altRuleAdditionalServicesDetails] = altRuleAdditionalServices;

                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.altRuleAdditionalServicesDetailsRepeater;
                });

                targetRepeater[0].data = altRuleAdditionalServices;
                //$(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: altRuleAdditionalServices });
                $(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: altRuleAdditionalServices }).trigger("reloadGrid");
            });
        },

        limitsDetails: function (repeaterBuilder, repeaterSelectedData, dataSourceMapping, fullName) {
            var customRuleInstance = repeaterBuilder.customrule;
            var Limit = {};
            Limit[customRuleInstance.elementName.limitsList] = {};
            var limitDetails = [];
            var limits = repeaterBuilder.formInstanceBuilder.formData.Limits;
            var oldData = repeaterBuilder.data;

            for (var i = 0; i < repeaterSelectedData.length; i++) {
                var limitDetail = {};
                limitDetail[customRuleInstance.elementName.limitDescription] = repeaterSelectedData[i][customRuleInstance.elementName.limitDescription];
                limitDetail[customRuleInstance.elementName.accumNumber] = repeaterSelectedData[i][customRuleInstance.elementName.accumNumber];
                limitDetail[customRuleInstance.BenefitSetName] = repeaterSelectedData[i][customRuleInstance.BenefitSetName];
                limitDetails.push(limitDetail);
            }

            Limit[customRuleInstance.elementName.limitsList] = limitDetails;

            if (repeaterSelectedData.length == 0) {
                limits[customRuleInstance.elementName.limitsList] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitsLimitsList;
                });
                targetRepeater[0].data = [];
               // $(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.limitServicesLTSE] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitServicesLTSE;
                });
                targetRepeater[0].data = [];
               // $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.limitRulesLTLT] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitRulesLTLT;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                var items = customRuleInstance.getAccumNumberForLimitRepeater(repeaterBuilder.formInstanceBuilder);
                var options = "";
                if (items != null && items.length > 0) {
                    options = options + Validation.selectOne + ':' /*+ Validation.selectOne */ + ';';
                    for (var idx = 0; idx < items.length; idx++) {
                        if (items[idx].ItemValue != '') {
                            options = options + items[idx].ItemValue + ':' + items[idx].ItemValue;
                            if (idx < items.length - 1) {
                                options = options + ';';
                            }
                        }
                    }
                }
                $(customRuleInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceBuilder.formInstanceId).setColProp('AccumNumber', { editoptions: { value: options } });
                $(customRuleInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceBuilder.formInstanceId).setColProp('AccumNumber', { editoptions: { value: options } });
                $(customRuleInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceBuilder.formInstanceId).setColProp('AccumNumber', { editoptions: { value: options } });


                var repeaterFullNameArray = [customRuleInstance.fullName.limitProcedureTableLTIP, customRuleInstance.fullName.limitDiagnosisTableLTID, customRuleInstance.fullName.limitProviderTableLTPR]
                var repeaterIdArray = [currentInstance.elemenIDs.limitLTIPRepeater, currentInstance.elemenIDs.limitLTIDRepeater, currentInstance.elemenIDs.limitLTPRRepeater];
                for (var r = 0; r < repeaterFullNameArray.length; r++) {
                    var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                        return rb.fullName == repeaterFullNameArray[r];
                    });
                    //compare previous item list with new item list 
                    $.each(targetRepeater[0].data, function (el, kt) {
                        var isAccumExist = items.filter(function (dt) {
                            return dt.ItemValue == kt.AccumNumber
                        })
                        if (isAccumExist.length == 0) {
                            kt[customRuleInstance.elementName.accumNumber] = "";
                        }
                    });
                   // $(repeaterIdArray[r] + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: targetRepeater[0].data }).trigger("reloadGrid");
                    $(repeaterIdArray[r] + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: targetRepeater[0].data });
                    $(repeaterIdArray[r] + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                }

                limits.FacetsLimits[customRuleInstance.elementName.limitProcedureTableLTIP] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitProcedureTableLTIP;
                });
                targetRepeater[0].data = [];
               // $(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.limitDiagnosisTableLTID] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitDiagnosisTableLTID;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.limitProviderTableLTPR] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitProviderTableLTPR;
                });
                targetRepeater[0].data = [];
                // $(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                return false;
            }

            var postData = {
                tenantId: repeaterBuilder.tenantId,
                formInstanceId: repeaterBuilder.formInstanceId,
                formDesignVersionId: repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                folderVersionId: repeaterBuilder.formInstanceBuilder.folderVersionId,
                formDesignId: repeaterBuilder.formInstanceBuilder.formDesignId,
                selectedLimits: JSON.stringify(Limit),
                limitsLimits: JSON.stringify(limits),
                fullName: fullName
            }

            var promise = ajaxWrapper.postJSON(currentInstance.URLs.getLimitsDeatails, postData);

            promise.done(function (result) {
                var limitDetails = JSON.parse(result);
                var facetLimits = limitDetails[customRuleInstance.elementName.facetsLimits];

                limits[customRuleInstance.elementName.limitsList] = limitDetails[customRuleInstance.elementName.limitsList];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitsLimitsList;
                });

                if (limitDetails[customRuleInstance.elementName.limitsList].length > 0)
                    currentInstance.addActivityLogEntries(repeaterBuilder, repeaterSelectedData, dataSourceMapping, oldData);

                targetRepeater[0].data = limitDetails[customRuleInstance.elementName.limitsList];
               // $(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: limitDetails[customRuleInstance.elementName.limitsList] }).trigger("reloadGrid");
                $(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: limitDetails[customRuleInstance.elementName.limitsList] });
                $(currentInstance.elemenIDs.limitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.limitServicesLTSE] = facetLimits[customRuleInstance.elementName.limitServicesLTSE];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitServicesLTSE;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.limitServicesLTSE];
                //$(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.limitServicesLTSE] });
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
               // $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.limitServicesLTSE] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.limitRulesLTLT] = facetLimits[customRuleInstance.elementName.limitRulesLTLT];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitRulesLTLT;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.limitRulesLTLT];
                //$(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.limitRulesLTLT] });
                $(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
               // $(currentInstance.elemenIDs.limitLTLTRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.limitRulesLTLT] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.limitProcedureTableLTIP] = facetLimits[customRuleInstance.elementName.limitProcedureTableLTIP];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitProcedureTableLTIP;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.limitProcedureTableLTIP];
                //$(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.limitProcedureTableLTIP] });
                $(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.limitProcedureTableLTIP] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.limitDiagnosisTableLTID] = facetLimits[customRuleInstance.elementName.limitDiagnosisTableLTID];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitDiagnosisTableLTID;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.limitDiagnosisTableLTID];
                //$(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.limitDiagnosisTableLTID] });
                $(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
               // $(currentInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.limitDiagnosisTableLTID] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.limitProviderTableLTPR] = facetLimits[customRuleInstance.elementName.limitProviderTableLTPR];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitProviderTableLTPR;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.limitProviderTableLTPR];
               // $(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.limitProviderTableLTPR] });
                $(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.limitProviderTableLTPR] }).trigger("reloadGrid");

                var items = customRuleInstance.getAccumNumberForLimitRepeater(repeaterBuilder.formInstanceBuilder);
                var options = "";
                if (items != null && items.length > 0) {
                    options = options + Validation.selectOne + ':' /*+ Validation.selectOne */ + ';';
                    for (var idx = 0; idx < items.length; idx++) {
                        if (items[idx].ItemValue != '') {
                            options = options + items[idx].ItemValue + ':' + items[idx].ItemValue;
                            if (idx < items.length - 1) {
                                options = options + ';';
                            }
                        }
                    }
                }
                $(customRuleInstance.elemenIDs.limitLTIPRepeater + repeaterBuilder.formInstanceBuilder.formInstanceId).setColProp('AccumNumber', { editoptions: { value: options } });
                $(customRuleInstance.elemenIDs.limitLTIDRepeater + repeaterBuilder.formInstanceBuilder.formInstanceId).setColProp('AccumNumber', { editoptions: { value: options } });
                $(customRuleInstance.elemenIDs.limitLTPRRepeater + repeaterBuilder.formInstanceBuilder.formInstanceId).setColProp('AccumNumber', { editoptions: { value: options } });


                var repeaterFullNameArray = [customRuleInstance.fullName.limitProcedureTableLTIP, customRuleInstance.fullName.limitDiagnosisTableLTID, customRuleInstance.fullName.limitProviderTableLTPR]
                var repeaterIdArray = [currentInstance.elemenIDs.limitLTIPRepeater, currentInstance.elemenIDs.limitLTIDRepeater, currentInstance.elemenIDs.limitLTPRRepeater];
                for (var r = 0; r < repeaterFullNameArray.length; r++) {
                    var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                        return rb.fullName == repeaterFullNameArray[r];
                    });
                    //compare previous item list with new item list 
                    $.each(targetRepeater[0].data, function (el, kt) {
                        var isAccumExist = items.filter(function (dt) {
                            return dt.ItemValue == kt.AccumNumber
                        })
                        if (isAccumExist.length == 0) {
                            kt[customRuleInstance.elementName.accumNumber] = "";
                        }
                    });
                    $(repeaterIdArray[r] + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: targetRepeater[0].data });
                    $(repeaterIdArray[r] + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                   // $(repeaterIdArray[r] + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: targetRepeater[0].data }).trigger("reloadGrid");
                }
            });
        },

        BenefitSummaryDetails: function (repeaterBuilder, repeaterSelectedData, dataSourceMapping, fullName) {
            var customRuleInstance = repeaterBuilder.customrule;
            var oldData = repeaterBuilder.data;
            var postData = {
                tenantId: repeaterBuilder.tenantId,
                formInstanceId: repeaterBuilder.formInstanceId,
                formDesignVersionId: repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                folderVersionId: repeaterBuilder.formInstanceBuilder.folderVersionId,
                formDesignId: repeaterBuilder.formInstanceBuilder.formDesignId,
                selectedBenefits: JSON.stringify(repeaterSelectedData),
            }

            var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.getBenefitSummaryDetails, postData);

            promise.done(function (result) {
                var benefitSummaryDetails = JSON.parse(result);

                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.benefitSummary][customRuleInstance.elementName.BenefitSummaryText] = benefitSummaryDetails;
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.benefitSummaryText;
                });

                if (benefitSummaryDetails.length > 0)
                    currentInstance.addActivityLogEntries(repeaterBuilder, repeaterSelectedData, dataSourceMapping, oldData);

                targetRepeater[0].data = benefitSummaryDetails;
                //$(currentInstance.elemenIDs.BenefitSummaryTextRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.BenefitSummaryTextRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.BenefitSummaryTextRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.BenefitSummaryTextRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: benefitSummaryDetails });
                $(currentInstance.elemenIDs.BenefitSummaryTextRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.BenefitSummaryTextRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', {
                //    data: benefitSummaryDetails
                //}).trigger("reloadGrid");
            });
        },

        shadowServiceGroupingDetails: function (repeaterBuilder, repeaterSelectedData, fullName) {
            var customRuleInstance = repeaterBuilder.customrule;
            var serviceGroupHeader = [];

            for (var i = 0; i < repeaterSelectedData.length; i++) {
                if (repeaterSelectedData[i][customRuleInstance.elementName.serviceGroupHeader].trim() != "") {
                    serviceGroupHeader.push("" + repeaterSelectedData[i][customRuleInstance.elementName.serviceGroupHeader] + "");
                }
            }

            if (serviceGroupHeader.length == 0) {
                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.shadowServiceGroup][customRuleInstance.elementName.shadowServiceGroupDetail] = [];
                //$(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                return false;
            }

            var postData = {
                tenantId: repeaterBuilder.tenantId,
                formInstanceId: repeaterBuilder.formInstanceId,
                formDesignVersionId: repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                folderVersionId: repeaterBuilder.formInstanceBuilder.folderVersionId,
                formDesignId: repeaterBuilder.formInstanceBuilder.formDesignId,
                mandates: serviceGroupHeader,
                fullName: fullName
            }

            var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.getMandateServiceGroupingDetails, postData);

            promise.done(function (result) {
                var mandateMappings = JSON.parse(result);
                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.shadowServiceGroup][customRuleInstance.elementName.shadowServiceGroupDetail] = mandateMappings;
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowServiceGroupingDetailsRepeater;
                });

                targetRepeater[0].data = mandateMappings;
                //$(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: mandateMappings });
                $(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.shadowServiceGroupingDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: mandateMappings }).trigger("reloadGrid");
            });
        },

        shadowAdditionalServicesDetails: function (repeaterBuilder, repeaterSelectedData, fullName) {
            var customRuleInstance = repeaterBuilder.customrule;
            var selectedServices = [];

            if (repeaterSelectedData.length == 0) {
                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.shadowAdditionalServices][customRuleInstance.elementName.shadowAdditionalServicesDetails] = [];
               // $(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                return false;
            }

            var postData = {
                tenantId: repeaterBuilder.tenantId,
                formInstanceId: repeaterBuilder.formInstanceId,
                formDesignVersionId: repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                folderVersionId: repeaterBuilder.formInstanceBuilder.folderVersionId,
                formDesignId: repeaterBuilder.formInstanceBuilder.formDesignId,
                selectedServices: JSON.stringify(repeaterSelectedData),
                fullName: fullName
            }

            var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.getAdditionalServicesDetails, postData);

            promise.done(function (result) {
                var additionalServices = JSON.parse(result);
                repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.elementName.shadowAdditionalServices][customRuleInstance.elementName.shadowAdditionalServicesDetails] = additionalServices;

                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowAdditionalServicesDetailsRepeater;
                });

                targetRepeater[0].data = additionalServices;
               // $(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: additionalServices });
                $(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.shadowAdditionalServicesDetailsRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: additionalServices }).trigger("reloadGrid");

            });
        },

        shadowLimitsDetails: function (repeaterBuilder, repeaterSelectedData, fullName) {
            var customRuleInstance = repeaterBuilder.customrule;
            var Limit = {};
            Limit[customRuleInstance.elementName.shadowLimitsList] = {};
            var limitDescriptions = [];
            var limits = repeaterBuilder.formInstanceBuilder.formData[customRuleInstance.sectionName.shadowLimits];

            for (var i = 0; i < repeaterSelectedData.length; i++) {
                var limitDescription = {};
                limitDescription[customRuleInstance.elementName.limitDescription] = repeaterSelectedData[i][customRuleInstance.elementName.limitDescription];
                limitDescriptions.push(limitDescription);
            }

            Limit[customRuleInstance.elementName.shadowLimitsList] = limitDescriptions;

            if (repeaterSelectedData.length == 0) {
                limits[customRuleInstance.elementName.shadowLimitsList] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitsList;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitServicesLTSE] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitServicesLTSE;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);   
                $(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitRulesLTLT] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitRulesLTLT;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.shadowLimitLTLTRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTLTRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitLTLTRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitProcedureTableLTIP] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitProcedureTableLTIP;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitDiagnosisTableLTID] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitDiagnosisTableLTID;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitProviderTableLTPR] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitProviderTableLTPR;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                return false;
            }

            var postData = {
                tenantId: repeaterBuilder.tenantId,
                formInstanceId: repeaterBuilder.formInstanceId,
                formDesignVersionId: repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                folderVersionId: repeaterBuilder.formInstanceBuilder.folderVersionId,
                formDesignId: repeaterBuilder.formInstanceBuilder.formDesignId,
                selectedLimits: JSON.stringify(Limit),
                limitsLimits: JSON.stringify(limits),
                fullName: fullName
            }

            var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.getLimitsDeatails, postData);

            promise.done(function (result) {
                var limitDetails = JSON.parse(result);
                var facetLimits = limitDetails[customRuleInstance.elementName.facetsLimits];

                limits[customRuleInstance.elementName.shadowLimitsList] = limitDetails[customRuleInstance.elementName.shadowLimitsList];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitsList;
                });
                targetRepeater[0].data = limitDetails[customRuleInstance.elementName.shadowLimitsList];
               // $(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: limitDetails[customRuleInstance.elementName.shadowLimitsList] });
                $(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.shadowLimitsListsRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: limitDetails[customRuleInstance.elementName.shadowLimitsList] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitServicesLTSE] = facetLimits[customRuleInstance.elementName.shadowLimitServicesLTSE];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitServicesLTSE;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.shadowLimitServicesLTSE];
                //$(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.shadowLimitServicesLTSE] });
                $(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
               // $(currentInstance.elemenIDs.shadowLimitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.shadowLimitServicesLTSE] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitRulesLTLT] = facetLimits[customRuleInstance.elementName.shadowLimitRulesLTLT];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitRulesLTLT;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.shadowLimitRulesLTLT];
                $(currentInstance.elemenIDs.shadowLimitLTLTRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTLTRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.shadowLimitRulesLTLT] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitProcedureTableLTIP] = facetLimits[customRuleInstance.elementName.shadowLimitProcedureTableLTIP];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitProcedureTableLTIP;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.shadowLimitProcedureTableLTIP];
               // $(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId) .pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.shadowLimitProcedureTableLTIP] });
                $(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.shadowLimitLTIPRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.shadowLimitProcedureTableLTIP] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.shadowLimitDiagnosisTableLTID] = facetLimits[customRuleInstance.elementName.shadowLimitDiagnosisTableLTID];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitDiagnosisTableLTID;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.shadowLimitDiagnosisTableLTID];
                //$(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId) .pqGrid('refreshView');
                $(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.shadowLimitDiagnosisTableLTID] });
                $(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                //$(currentInstance.elemenIDs.shadowLimitLTIDRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.shadowLimitDiagnosisTableLTID] }).trigger("reloadGrid");

                limits.FacetsLimits[customRuleInstance.elementName.limitProviderTableLTPR] = facetLimits[customRuleInstance.elementName.shadowLimitProviderTableLTPR];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.shadowLimitProviderTableLTPR;
                });
                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.shadowLimitProviderTableLTPR];
                //$(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.shadowLimitProviderTableLTPR] });
                $(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
               // $(currentInstance.elemenIDs.shadowLimitLTPRRepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.shadowLimitProviderTableLTPR] }).trigger("reloadGrid");
            });
        },

        limitServicesLTSE: function (repeaterBuilder, repeaterSelectedData, dataSourceMapping, fullName) {
            var customRuleInstance = repeaterBuilder.customrule;
            var Limit = {};
            Limit[customRuleInstance.elementName.limitsList] = {};
            var limitDetails = [];
            var limits = repeaterBuilder.formInstanceBuilder.formData.Limits;
            var oldData = repeaterBuilder.data;

            for (var i = 0; i < repeaterSelectedData.length; i++) {
                var limitDetail = {};
                limitDetail[customRuleInstance.elementName.limitDescription] = repeaterSelectedData[i][customRuleInstance.elementName.limitDescription];
                limitDetail[customRuleInstance.elementName.accumNumber] = repeaterSelectedData[i][customRuleInstance.elementName.accumNumber];
                limitDetail[customRuleInstance.BenefitSetName] = repeaterSelectedData[i][customRuleInstance.BenefitSetName];
                limitDetails.push(limitDetail);
            }

            Limit[customRuleInstance.elementName.limitsList] = limitDetails;

            if (repeaterSelectedData.length == 0) {

                limits.FacetsLimits[customRuleInstance.elementName.limitServicesLTSE] = [];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitServicesLTSE;
                });
                targetRepeater[0].data = [];
                //$(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');

                return false;
            }

            var postData = {
                tenantId: repeaterBuilder.tenantId,
                formInstanceId: repeaterBuilder.formInstanceId,
                formDesignVersionId: repeaterBuilder.formInstanceBuilder.formDesignVersionId,
                folderVersionId: repeaterBuilder.formInstanceBuilder.folderVersionId,
                formDesignId: repeaterBuilder.formInstanceBuilder.formDesignId,
                selectedLimits: JSON.stringify(Limit),
                limitsLimits: JSON.stringify(limits),
                fullName: fullName
            }

            var promise = ajaxWrapper.postJSONCustom(currentInstance.URLs.getLimitsDeatails, postData);

            promise.done(function (result) {
                var limitDetails = JSON.parse(result);
                var facetLimits = limitDetails[customRuleInstance.elementName.facetsLimits];

                limits.FacetsLimits[customRuleInstance.elementName.limitServicesLTSE] = facetLimits[customRuleInstance.elementName.limitServicesLTSE];
                var targetRepeater = repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                    return rb.fullName == customRuleInstance.fullName.limitServicesLTSE;
                });

                if (facetLimits[customRuleInstance.elementName.limitServicesLTSE].length > 0)
                    currentInstance.addActivityLogEntries(repeaterBuilder, repeaterSelectedData, dataSourceMapping, oldData);

                targetRepeater[0].data = facetLimits[customRuleInstance.elementName.limitServicesLTSE];
               // $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('clearGridData');
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid("option", "dataModel", { data: facetLimits[customRuleInstance.elementName.limitServicesLTSE] });
                $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).pqGrid('refreshView');
               // $(currentInstance.elemenIDs.limitLTSERepeater + repeaterBuilder.formInstanceId).jqGrid('setGridParam', { data: facetLimits[customRuleInstance.elementName.limitServicesLTSE] }).trigger("reloadGrid");

            });
        },
    }
}

customRulePQ.prototype.addActivityLogEntries = function (repeaterBuilder, repeaterSelectedData, dataSourceMapping, oldData) {
    var currentInstance = this;

    $.each(repeaterSelectedData, function (idx, row) {
        //Activity Log for Added Rows                        
        var resultRecordExits = currentInstance.IsRecordExists(row, oldData, dataSourceMapping);
        if (resultRecordExits)
            repeaterBuilder.addEntryToAcitivityLogger(row.RowIDProperty, Operation.ADD);
    });
}

customRulePQ.prototype.IsRecordExists = function (compareElement, ComparerList, dataSourceMapping) {

    var dataSourceLength = dataSourceMapping.Mappings.length

    if (dataSourceMapping.Mappings) {
        var isRecordExists = ComparerList.filter(function (dt) {
            var diff = dataSourceMapping.Mappings.filter(function (map) {
                return compareElement[map.TargetElement] == dt[map.TargetElement];
            })

            if (diff.length == dataSourceLength) {
                return dt;
            }
        });
    }
    return isRecordExists.length == 0 ? true : false;
}

customRulePQ.prototype.customRuleServiceMethods = function () {
    var currentInstance = this;

    return {
        runCustomRule: function (tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping, repeaterData, repeaterName, repeaterBuilders) {

            switch (repeaterName) {
                case currentInstance.fullName.additionalServicesDetailsRepeater:
                    return currentInstance.customServiceRules.getAdditionalServicesData(tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping);
                    break;
                case currentInstance.fullName.shadowAdditionalServiceList:
                    return currentInstance.customServiceRules.getShadowAdditionalServicesData(tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping);
                    break;
                case currentInstance.fullName.benefitSummaryTable:
                    return currentInstance.customServiceRules.getBenefitSummaryTablesData(tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping);
                    break;
                case currentInstance.fullName.limitsLimitsList:
                    return currentInstance.customServiceRules.getSourceDataForLimitRepeaters(tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping, repeaterName);
                    break;
                case currentInstance.fullName.limitServicesLTSE:
                    return currentInstance.customServiceRules.getSourceDataForLTSERepeaters(tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping, repeaterName, repeaterBuilders);
                    break;
            }
        },

        getSourceDataForLTSERepeaters: function (tenantId, formInanceId, formDesignVersionId, folderVersionId, dataSourceMapping, repeaterName, repeaterBuilders) {
            var filterFrom = repeaterName;

            var repeaterData = repeaterBuilders.filter(function (dt) {
                return dt.fullName == "Limits.FacetsLimits.LimitRulesLTLT"
            })

            var postData = {
                tenantId: tenantId,
                formInstanceId: formInstanceId,
                formDesignVersionId: formDesignVersionId,
                folderVersionId: folderVersionId,
                formDesignId: dataSourceMapping[0].FormDesignID,
                filterFrom: filterFrom,
                LTLTData: JSON.stringify(repeaterData[0].data)
            }
            var promise = ajaxWrapper.postJSON(currentInstance.URLs.getSourceDataForLTSERepeaters, postData);


            return promise;

        },
        getAdditionalServicesData: function (tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping) {

            var filterFrom = currentInstance.fullName.ServiceGrouping;
            url = currentInstance.URLs.getAddtionalServiesList.replace(/\{tenantId\}/g, tenantId)
                                                                .replace(/\{formInstanceId\}/g, formInstanceId)
                                                                .replace(/\{formDesignVersionId\}/g, formDesignVersionId)
                                                                .replace(/\{folderVersionId\}/g, folderVersionId)
                                                                .replace(/\{formDesignId\}/g, dataSourceMapping[0].FormDesignID)
                                                                .replace(/\{fullName\}/g, dataSourceMapping[0].SourceParent)
                                                                .replace(/\{filterFrom\}/g, filterFrom);

            var promise = ajaxWrapper.getJSON(url);
            return promise;
        },

        getShadowAdditionalServicesData: function (tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping) {

            var filterFrom = currentInstance.fullName.shadowServiceGrouping;
            url = currentInstance.URLs.getAddtionalServiesList.replace(/\{tenantId\}/g, tenantId)
                                                                .replace(/\{formInstanceId\}/g, formInstanceId)
                                                                .replace(/\{formDesignVersionId\}/g, formDesignVersionId)
                                                                .replace(/\{folderVersionId\}/g, folderVersionId)
                                                                .replace(/\{formDesignId\}/g, dataSourceMapping[0].FormDesignID)
                                                                .replace(/\{fullName\}/g, dataSourceMapping[0].SourceParent)
                                                                .replace(/\{filterFrom\}/g, filterFrom);

            var promise = ajaxWrapper.getJSON(url);
            return promise;
        },

        getBenefitSummaryTablesData: function (tenantId, formInstanceId, formDesignVersionId, folderVersionId, dataSourceMapping) {

            var filterFrom = currentInstance.fullName.benefitSummaryTable;
            url = currentInstance.URLs.getBenefitSummaryTablesList.replace(/\{tenantId\}/g, tenantId)
                                                                .replace(/\{formInstanceId\}/g, formInstanceId)
                                                                .replace(/\{formDesignVersionId\}/g, formDesignVersionId)
                                                                .replace(/\{folderVersionId\}/g, folderVersionId)
                                                                .replace(/\{formDesignId\}/g, dataSourceMapping[0].FormDesignID)
                                                                .replace(/\{fullName\}/g, dataSourceMapping[0].SourceParent)
                                                                .replace(/\{filterFrom\}/g, filterFrom);

            var promise = ajaxWrapper.getJSON(url);
            return promise;
        },

        getSourceDataForLimitRepeaters: function (tenantId, formInanceId, formDesignVersionId, folderVersionId, dataSourceMapping, repeaterName) {
            var filterFrom = repeaterName;
            url = currentInstance.URLs.getSourceDataForLimitRepeaters.replace(/\{tenantId\}/g, tenantId)
                                                                .replace(/\{formInstanceId\}/g, formInstanceId)
                                                                .replace(/\{formDesignVersionId\}/g, formDesignVersionId)
                                                                .replace(/\{folderVersionId\}/g, folderVersionId)
                                                                .replace(/\{formDesignId\}/g, dataSourceMapping[0].FormDesignID)
                                                                .replace(/\{filterFrom\}/g, filterFrom);

            var promise = ajaxWrapper.getJSON(url);
            return promise;
        }
    }
}

customRulePQ.prototype.filterFacetLimitsChildGrid = function (currentInstance) {
    try {
        rowData = currentInstance.data.filter(function (dt) {
            return dt.RowIDProperty == currentInstance.selectedRowId;
        });
        //filtering LTLT Grid
        //The following gridID represents Limits LTLT Grid      
        var gridID = $(this.elemenIDs.limitLTLTRepeater + currentInstance.formInstanceId);
        this.applyFilterOnFacetLimitsGrid(rowData[0], gridID);

        //filtering LTIP Grid
        //The following gridID represents Limits LTIP Grid
        var gridID = $(this.elemenIDs.limitLTIPRepeater + currentInstance.formInstanceId);
        this.applyFilterOnFacetLimitsGrid(rowData[0], gridID);

        //filtering LTPR Grid
        //The following gridID represents Limits LTPR Grid
        var gridID = $(this.elemenIDs.limitLTPRRepeater + currentInstance.formInstanceId);
        this.applyFilterOnFacetLimitsGrid(rowData[0], gridID);

        //filtering LTID Grid
        //The following gridID represents Limits LTID Grid
        var gridID = $(this.elemenIDs.limitLTIDRepeater + currentInstance.formInstanceId);
        this.applyFilterOnFacetLimitsGrid(rowData[0], gridID);

        //filtering LTSE Grid
        //The following gridID represents Limits LTSE Grid
        var gridID = $(this.elemenIDs.limitLTSERepeater + currentInstance.formInstanceId);
        this.applyFilterOnFacetLimitsGrid(rowData[0], gridID);
    }
    catch (e) {
        throw e;
        console.log(JSON.stringify(e));
    }
}

customRulePQ.prototype.applyFilterOnFacetLimitsGrid = function (rowData, gridID) {

    //here checks for the Grid information using gridID
    //postdata returns undefined if Grid has not yet loaded for once
    var postdata = gridID.jqGrid('getGridParam', 'postData');

    //Here  searchField is 'LimitDescription' column present in both 'Limit List' grid
    //and child Limit grids,
    //      searchOperator is equal
    // and  searchstring is LimiDescription value for that selected row
    var rule = [];
    rule.push({ "field": "AccumNumber", "op": "cn", "data": rowData.AccumNumber });

    rule.push({ "field": "BenefitSetName", "op": "cn", "data": rowData.BenefitSetName });

    if (rule.length > 0) {
        $.extend(postdata,
                       {
                           filters: JSON.stringify({ "groupOp": "AND", "rules": rule }),
                       });
        $(gridID).jqGrid('setGridParam', { search: true, postData: postdata });
    }
    else {
        $(gridID).jqGrid('setGridParam', { search: false, postData: postdata });
    }
    $(gridID).trigger("reloadGrid");
}

customRulePQ.prototype.getFormatterForProductForm = function (colMod, ui, fullName) {
    var customRuleInstance = this;
    try {
        if (ui) {
            var cellvalue = ui.cellData;
                switch (fullName) {
                    case "BenefitReview.BenefitReviewGrid.ServiceComment":
                        colMod.editable = false;
                        return '<span class="ui-icon ui-icon-document viewBRGServiceComment" data-value="' + cellvalue + '" title="View ' + ui.dataIndx + '" style="margin:0 auto!important;cursor: pointer;" ' +
                       'id="' + ui.rowData.RowIDProperty + '" ' + 'index="' + ui.dataIndx + '"></span>';
                        break;
                    case customRuleInstance.fullName.benefitReviewLimits:
                        colMod.editable = false;
                        var span = "<span class='ui-icon ui-icon-document viewBRGLimits' data-value='" + cellvalue + "' title='View " + ui.dataIndx + "' style='margin:0 auto!important;cursor: pointer;' " +
                                        "id='" + ui.rowData.RowIDProperty + "' " + "index='" + ui.dataIndx + "'></span>"
                        return span;
                        break;
                    case customRuleInstance.fullName.benefitReviewAltRule:
                        //if (ui.rowData.ProductNetworkList!= undefined) {
                        //    if (ui.rowData.ProductNetworkList[0].AltRule == "Show") {
                                var span = "<span class='ui-icon ui-icon-document viewBRGAltRule' data-value='" + cellvalue + "'  title='View " + ui.dataIndx + "' style='margin:0 auto!important;cursor: pointer;' " +
                                               "id='" + ui.rowData.RowIDProperty + "' " + "index='" + ui.dataIndx + "'></span>"
                                return span;

                        //    }
                        //        else {
                        //            var span = "<span class='ui-icon ui-icon-document ui-state-disabled' data-value='" + cellvalue + "' title='View " + ui.dataIndx + "' style='margin:0 auto!important;cursor: pointer;' " +
                        //                           "id='" + ui.rowData.RowIDProperty + "' " + "index='" + ui.dataIndx + "'></span>"
                        //            return span;
                        //        }
                        //    }
                        //else {
                        //    var span = "<span class='ui-icon ui-icon-document ui-state-disabled' data-value='" + cellvalue + "' title='View " + ui.dataIndx + "' style='margin:0 auto!important;cursor: pointer;' " +
                        //                   "id='" + ui.rowData.RowIDProperty + "' " + "index='" + ui.dataIndx + "'></span>"
                        //    return span;
                        //}
                        break;
                    case customRuleInstance.fullName.benefitReviewTier:
                        var span = "<span class='ui-icon ui-icon-document viewBRGTier' data-value='" + cellvalue + "' title='View " + ui.dataIndx + "' style='margin:0 auto!important;cursor: pointer;' " +
                                      "id='" + ui.rowData.RowIDProperty + "' " + "index='" + ui.dataIndx + "'></span>"
                        return span;
                        break;
                    case customRuleInstance.fullName.benefitAltRuleGridTier:
                        var span = "<span class='ui-icon ui-icon-document viewBRGAltRuleTier' data-value='" + cellvalue + "' title='View " + ui.dataIndx + "' style='margin:0 auto!important;cursor: pointer;' " +
                                      "id='" + ui.rowData.RowIDProperty + "' " + "index='" + ui.dataIndx + "'></span>"
                        return span;
                        break;
                }
            }
        

    } catch (e) {
        console.log(e + "getFormattersForBenefirReviewGrid");
    }
}

customRulePQ.prototype.getUnforamttersForProductForm = function () {
    return {
        unformat: function (cellvalue, options, rowObject) {
            //return cellvalue;
            return $(rowObject).find('span').attr('data-value');
        }
    }
}

customRulePQ.prototype.registerEventForButtonInRepeaterOfProductForm = function (currentInstance) {
    switch (currentInstance.fullName) {
        case this.fullName.benefitReviewGrid:
            $(".viewBRGServiceComment").click(function () {
                currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var griddata = $(currentInstance.gridElementIdJQ).pqGrid('getRowData', { rowIndxPage: element });
                var colModelArray = colModel.split('_');
                var benefitSetName = "In Network";// griddata["INL_" + colModelArray[1] + "_" + colModelArray[2] + "_" + currentInstance.customrule.BenefitSetName];
                currentInstance.customrule.showServiceCommentPouUp(element, currentInstance.data, currentInstance.tenantId, currentInstance.formInstanceId, currentInstance.formInstanceBuilder.formDesignVersionId, currentInstance.formInstanceBuilder.folderVersionId, benefitSetName);
            });
            $(".viewBRGLimits").click(function () {
                currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var colModelArray = colModel.split('_');
                var griddata = $(currentInstance.gridElementIdJQ).pqGrid('getRowData', { rowIndxPage: element });
                var colModelArray = colModel.split('_');
                var benefitSetName = griddata["INL_" + colModelArray[1] + "_" + colModelArray[2] + "_" + currentInstance.customrule.BenefitSetName];
                currentInstance.customrule.showLimitPouUp(element, currentInstance.data, currentInstance.tenantId, currentInstance.formInstanceId, currentInstance.formInstanceBuilder.formDesignVersionId, currentInstance.formInstanceBuilder.folderVersionId, benefitSetName);
            });
            $(".viewBRGAltRule").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var data = $(currentInstance.gridElementIdJQ).pqGrid('getRowData', { rowIndx: element });
                setTimeout(function () {
                    currentInstance.customrule.showAltRulePopUp(element, isCurrentRowReadOnly, data, currentInstance, colModel);
                }, 5);
            });
            $(".viewBRGTier").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var colModelArray = colModel.split('_');
                var data = $(currentInstance.gridElementIdJQ).pqGrid('getRowData', { rowIndx: element });
                setTimeout(function () {
                    currentInstance.customrule.showTierDataPopUp(element, isCurrentRowReadOnly, data, currentInstance, colModelArray[1], colModelArray[2]);
                }, 5)

            });
            break;
        case this.fullName.benefitReviewGridAltRulesData:
            $(".viewBRGAltRuleTier").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var colModelArray = colModel.split('_');
                var data = $(currentInstance.gridElementIdJQ).pqGrid('getRowData', { rowIndxPage: element });
                setTimeout(function () {
                    currentInstance.customrule.showAltRuleTierDataPopUp(element, isCurrentRowReadOnly, data, currentInstance, colModelArray[1], colModelArray[2]);
                }, 5)

            });
            break;
    }
}

customRulePQ.prototype.getFormatterForMasterListForm = function (colMod, ui, fullName) {
    var customRuleInstance = this;
    try {
        if (ui) {
            var cellvalue = ui.cellData;
            switch (fullName) {
                case "ServiceGroupDefinition.ServiceGroupingDetail.ServiceConfiguration":
                    return '<span class="ui-icon ui-icon-document viewServiceConfiguration" data-value="' + cellvalue + '" title="View ' + ui.dataIndx + '" style="margin:0 auto!important;cursor: pointer;" ' +
                                    'id=" ' + ui.rowData.RowIDProperty + ' " ' + 'index="' + ui.dataIndx + '"></span>';
                    break;

            }
        }

    } catch (e) {
        console.log(e + "getFormattersForBenefirReviewGrid");
    }

    function unFormatColumn(cellValue, options) {
        var result;
        result = $(this).find('#' + options.rowId).find('input').prop('checked');

        if (result == true || result == "true")
            return 'Yes';
        else
            return 'No';
    }

    function formaterWeightCounter(cellValue, options, rowObject) {
        var result;
        if (isCurrentRowReadOnly) {
            result = "<input type='text' class='form-control WeightCounterText'  id = 'textbox_" + options.rowId + "' name= 'textbox_" + options.gid + "' value='" + rowObject.WeightCounter + "' />";
        }
        else {
            result = "<input type='text' class='form-control WeightCounterText'  id = 'textbox_" + options.rowId + "' name= 'textbox_" + options.gid + "' value='" + rowObject.WeightCounter + "' disabled=disabled />";
        }

        return result;
    }

    function unFormatWeightCounter(cellValue, options, rowObject) {
        var result;
        result = $(this).find('#' + options.rowId).find('input[name="textbox_accumulatorsGrid"]').val();

        return result;
    }

    var defaultRule = $(customRuleInstance.masterListElementIDs.defaultRuleJQ).is(':checked');
    var modelSESE_ID = $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).is(':checked');

    var defaultRulerules = {
        regulerRule: '',
        alternateRule: '',
        altRuleCondition: '',
    }

    var ModelRulerules = {
        regulerRule: '',
        alternateRule: '',
        altRuleCondition: '',
    }
    if (defaultRule) {
        defaultRulerules.regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
        defaultRulerules.alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
        defaultRulerules.altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();
    }

    if (modelSESE_ID) {
        ModelRulerules.regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
        ModelRulerules.alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
        ModelRulerules.altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();
    }


    //var serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data');
    var serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data");
    var accumulators = $(customRuleInstance.masterListElementIDs.accumulatorsJQ).is(':checked');
    var limitModelSESE_ID = $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).is(':checked');

    var accumulatorsGrid = {
        serviceRuleConfigData: '',
    }

    var LimitaccumulatorsGrid = {
        serviceRuleConfigData: '',
    }

    if (accumulators) {
        //accumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data');
        accumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data");
    }

    if (limitModelSESE_ID) {
        //LimitaccumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data');
        LimitaccumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data");
    }

    $(customRuleInstance.masterListElementIDs.defaultRuleJQ).click(function (e) {
        $(customRuleInstance.masterListElementIDs.modleSESE_IDRuleConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.defaultServiceRuleConfigJQ).show();
        $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).prop('checked', false);
        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).removeClass('has-error');

        ModelRulerules.regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
        ModelRulerules.alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
        ModelRulerules.altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();

        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val(defaultRulerules.regulerRule);
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val(defaultRulerules.alternateRule);
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val(defaultRulerules.altRuleCondition);
    });

    $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).click(function (e) {
        $(customRuleInstance.masterListElementIDs.modleSESE_IDRuleConfigJQ).show();
        //$(customRuleInstance.masterListElementIDs.defaultServiceRuleConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.defaultRuleJQ).prop('checked', false);
        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).removeClass('has-error');

        defaultRulerules.regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
        defaultRulerules.alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
        defaultRulerules.altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();

        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val(ModelRulerules.regulerRule);
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val(ModelRulerules.alternateRule);
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val(ModelRulerules.altRuleCondition);
    });

    $(customRuleInstance.masterListElementIDs.accumulatorsJQ).click(function (e) {

        var limitModelSESE_ID = $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).is(':checked');
        //if (limitModelSESE_ID) { LimitaccumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data'); }
        if (limitModelSESE_ID) { LimitaccumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data"); }

        //$(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid("clearGridData");
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('option', 'dataModel.data', []);
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('refreshView');
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel", { data: accumulatorsGrid.serviceRuleConfigData == "" ? defaultGridData : accumulatorsGrid.serviceRuleConfigData });
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('refreshView');
        //$(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('setGridParam',
        // {
        //     data: accumulatorsGrid.serviceRuleConfigData == "" ? defaultGridData : accumulatorsGrid.serviceRuleConfigData
        // }).trigger("reloadGrid");

        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.accumulatorsRuleConfigJQ).show();
        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).prop('checked', false);

    });

    $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).click(function (e) {

        var accumulators = $(customRuleInstance.masterListElementIDs.accumulatorsJQ).is(':checked');
        if (accumulators) { accumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data'); }

       // $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid("clearGridData");
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('option', 'dataModel.data', []);
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('refreshView');
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel", { data: LimitaccumulatorsGrid.serviceRuleConfigData == "" ? defaultGridData : LimitaccumulatorsGrid.serviceRuleConfigData });
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('refreshView');
        //$(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('setGridParam',
        // {
        //     data: LimitaccumulatorsGrid.serviceRuleConfigData == "" ? defaultGridData : LimitaccumulatorsGrid.serviceRuleConfigData
        // }).trigger("reloadGrid");
        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDConfigJQ).show();
        //$(customRuleInstance.masterListElementIDs.accumulatorsRuleConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.accumulatorsJQ).prop('checked', false);
    });

    $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).change(function (e) {
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('');
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).removeClass('form-control');
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).parent().removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).parent().addClass('has-error');
    });

}

customRulePQ.prototype.showServiceCommentPouUp = function (rowId, data, tenantId, formInstanceId, formDesignVersionId, folderVersionId, benefitSetName) {
    var benefitRowData = data.filter(function (ct) {
        return ct.RowIDProperty == rowId;
    });
    if (benefitRowData != undefined) {
        benefitRowData = benefitRowData[0];
    }
    var elementIDs = {
        serviceCommentDataDialogJQ: "#serviceCommentDialog",
    };
    var postData = {
        tenantId: tenantId,
        formInstanceId: formInstanceId,
        formDesignVersionId: formDesignVersionId,
        folderVersionId: folderVersionId,
        formDesignId: FormTypes.PRODUCTFORMDESIGNID,
        benefitServiceRowData: JSON.stringify(benefitRowData),
        benefitSetName: benefitSetName
    }
    var promise = ajaxWrapper.postJSON(this.URLs.getServiceCommentDescriptionForBRGService, postData);
    promise.done(function (result) {

        var serviceCommentDetailsArray = JSON.parse(result);
        if (!$(elementIDs.serviceCommentDataDialogJQ).hasClass('ui-dialog')) {
            $(elementIDs.serviceCommentDataDialogJQ).dialog({
                modal: true,
                autoOpen: false,
                draggable: true,
                resizable: true,
                zIndex: 1005,
                width: 400,
                height: 200,
                closeOnEscape: false,
                title: 'Service Comment Details',
                buttons: {
                    Close: function () {
                        $(this).empty();
                        $(this).dialog("close");
                    }
                }
            });
        }

        var serviceCommentData = "";
        $(elementIDs.serviceCommentDataDialogJQ).empty();
        if (serviceCommentDetailsArray != null) {
            serviceCommentData += serviceCommentDetailsArray;
        }
        else {
            serviceCommentData = "No Service Comment Exist.";
        }
        $(elementIDs.serviceCommentDataDialogJQ).append(serviceCommentData);
        var title = "Service Comment - ";
        $(elementIDs.serviceCommentDataDialogJQ).dialog('option', 'title', title);
        $(elementIDs.serviceCommentDataDialogJQ).dialog("open");
    });

}


customRulePQ.prototype.showLimitPouUp = function (rowId, data, tenantId, formInstanceId, formDesignVersionId, folderVersionId, benefitSetName) {
    var benefitRowData = data.filter(function (ct) {
        return ct.RowIDProperty == rowId;
    });

    if (benefitRowData != undefined) {
        benefitRowData = benefitRowData[0];
    }

    var elementIDs = {
        limitDataDialogJQ: "#limitDialog",
    };

    if (!$(elementIDs.limitDataDialogJQ).hasClass('ui-dialog-content')) {
        $(elementIDs.limitDataDialogJQ).dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            width: 700,
            closeOnEscape: false,
            buttons: [{
                text: "Close",
                click: function () {
                    $(this).dialog("close");
                }
            }],
        });
    }

    var postData = {
        tenantId: tenantId,
        formInstanceId: formInstanceId,
        formDesignVersionId: formDesignVersionId,
        folderVersionId: folderVersionId,
        formDesignId: FormTypes.PRODUCTFORMDESIGNID,
        benefitServiceRowData: JSON.stringify(benefitRowData),
        benefitSetName: benefitSetName
    }

    var promise = ajaxWrapper.postJSON(this.URLs.getLimitDescriptionForBRGService, postData);
    promise.done(function (result) {
        var limitDetailsArray = [];

        limitDetailsArray = JSON.parse(result);

        var limitDialogJQ = "#limitDialog";

        $(limitDialogJQ).empty();

        if (!$(limitDialogJQ).hasClass('ui-dialog')) {
            $(limitDialogJQ).dialog({
                modal: true,
                autoOpen: false,
                draggable: true,
                resizable: true,
                zIndex: 1005,
                width: 550,
                height: 320,
                closeOnEscape: false,
                title: 'Limit Details',
                buttons: {
                    Close: function () {
                        $(this).dialog("close");
                    }
                }
            });
        }

        var limitData = "";

        if (limitDetailsArray.length > 0) {
            for (var i = 0; i < limitDetailsArray.length; i++) {
                limitData += "<div class='row'>" +
                                "<div class='col-sm-9'>" +
                                "<b>" + (i + 1) + "</b>" +
                                ". " +
                                limitDetailsArray[i] +
                                "</div>" +
                                "</div>";
            }
        }
        else {
            limitData = "No Limits selected.";
        }

        $(limitDialogJQ).append(limitData);
        var title = "Limits - " + benefitRowData.BenefitCat1;
        $(limitDialogJQ).dialog('option', 'title', title);
        $(limitDialogJQ).dialog("open");
        var title = "Limits - " + benefitRowData.BenefitCategory1;
        $(elementIDs.limitDataDialogJQ).dialog('option', 'title', title);
        $(elementIDs.limitDataDialogJQ).dialog("open");
    });

}


customRulePQ.prototype.showAltRulePopUp = function (rowId, isCurrentRowReadOnly, benefitRowData, currentInstance, colModel) {
    var customRuleInstance = currentInstance.customrule;
    try {
        var elementIDs = {
            altRuleGrid: "altRuleGrid",
            altRuleGridJQ: "#altRuleGrid",
            altRuleDialogJQ: "#altRuleDialog",
            spanAltRulesDataBenefitCategory1JQ: "#spanAltRulesDataBenefitCategory1",
            spanAltRulesDataBenefitCategory2JQ: "#spanAltRulesDataBenefitCategory2",
            spanAltRulesDataBenefitCategory3JQ: "#spanAltRulesDataBenefitCategory3",
            spanAltRulesDataPlaceofServiceJQ: "#spanAltRulesDataPlaceofService",
            spanAltRulesDataSESEIDJQ: "#spanAltRulesDataSESEID",
            spanAltRulesBenefitSetNameJQ: "#spanAltRulesBenefitSetName",

            checkboxAltRulesPopupSectionCoveredJQ: "#checkboxAltRulesPopupSection_Covered",
            selectAltRulesPopupSectionSERLMessageJQ: "#selectAltRulesPopupSection_SERLMessage",
            selectAltRulesPopupSectionDisallowedMessagesJQ: "#selectAltRulesPopupSection_DisallowedMessages",
            spanAltRulesPopupSectionTierNoJQ: "#spanAltRulesPopupSection_TierNo",
            selectAltRulesPopupSectionCopayJQ: "#selectAltRulesPopupSection_Copay",
            selectAltRulesPopupSectionCoinsuranceJQ: "#selectAltRulesPopupSection_Coinsurance",

            sectionAltRulesPopupSectionAllowedAmountJQ: "#sectionAltRulesPopupSection_AllowedAmount",
            sectionAltRulesPopupSectionAllowedCounterJQ: "#sectionAltRulesPopupSection_AllowedCounter",
            sectionAltRulesPopupSectionDeductibleAccumulatorJQ: "#sectionAltRulesPopupSection_DeductibleAccumulator",
            textboxAltRulesPopupSectionServicePenaltyAmountJQ: "#textboxAltRulesPopupSection_ServicePenaltyAmount",
            textboxAltRulesPopupSectionServicePenaltyPercentJQ: "#textboxAltRulesPopupSection_ServicePenaltyPercent",
            textboxAltRulesPopupSectionMaximumPenaltyAmountJQ: "#textboxAltRulesPopupSection_MaximumPenaltyAmount",
            textboxAltRulesPopupSectionPenaltyExplanationCodeJQ: "#textboxAltRulesPopupSection_PenaltyExplanationCode",
            textboxAltRulesPopupSectionServicePenaltyOptionsJQ: "#textboxAltRulesPopupSection_ServicePenaltyOptions",
            selectAltRulesPopupSectionPenaltyTypeJQ: "#selectAltRulesPopupSection_PenaltyType",
            selectAltRulesPopupSectionPenaltyCalculationIndicatorJQ: "#selectAltRulesPopupSection_PenaltyCalculationIndicator",
            //selectAltRulesPopupSectionExcessPerCounterIndicatorJQ: "textboxAltRulesPopupSectionExcessPerCounterIndicator",
            selectAltRulesPopupSectionExcessPerCounterIndicatorJQ: "#selectAltRulesPopupSection_ExcessPerCounterIndicator",
            spanAltRulesDataIndexJQ: "#spanAltRulesDataIndex",
            labelAltRulesPopupSectionSERLMessageJQ: "#labelAltRulesPopupSection_SERLMessage",
            labelAltRulesPopupSectionDisallowedMessageJQ: "labelAltRulesPopupSection_DisallowedMessage",
            textboxAltRulesPopupSectionAlternateRuleConditionJQ: "#textboxAltRulesPopupSection_AlternateRuleCondition",
        };

        var customRuleInstance = this;
        var colModelArray = colModel.split('_');
        var formInstancebuilder = currentInstance.formInstanceBuilder;
        var dataSourceName = colModelArray[1];
        var index = colModelArray[2];

        var benefitReviewAltRuleData = new Array();
        formInstancebuilder.designData.Sections.filter(function (ct) {
            if (ct.GeneratedName === customRuleInstance.sectionName.benefitReviewGridGenrated) {
                ct.Elements.filter(function (ele) {
                    if (ele.GeneratedName === customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated) {
                        if (ele.Repeater.GeneratedName === customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated) {
                            benefitReviewAltRuleData = ele.Repeater;
                            return;
                        }
                    }
                });
            }
        });

        var benefitReviewAltRulesGridTierData = new Array();
        formInstancebuilder.designData.Sections.filter(function (ct) {
            if (ct.GeneratedName === customRuleInstance.sectionName.benefitReviewGridGenrated) {
                ct.Elements.filter(function (ele) {
                    if (ele.GeneratedName === customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated) {
                        if (ele.Repeater.GeneratedName === customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated) {
                            benefitReviewAltRulesGridTierData = ele.Repeater;
                            return;
                        }
                    }
                });
            }
        });

        // Initialise details section controls

        this.InitializeAltRulesZeroTierSection(benefitRowData, formInstancebuilder, dataSourceName, benefitReviewAltRuleData, index, elementIDs)

        $(elementIDs.spanAltRulesDataBenefitCategory1JQ).text(benefitRowData.BenefitCategory1);
        $(elementIDs.spanAltRulesDataBenefitCategory2JQ).text(benefitRowData.BenefitCategory2);
        $(elementIDs.spanAltRulesDataBenefitCategory3JQ).text(benefitRowData.BenefitCategory3);
        $(elementIDs.spanAltRulesDataPlaceofServiceJQ).text(benefitRowData.PlaceofService);
        $(elementIDs.spanAltRulesDataSESEIDJQ).text(benefitRowData.SESEID);
        $(elementIDs.spanAltRulesBenefitSetNameJQ).text(benefitRowData["INL_" + dataSourceName + "_" + index + "_" + customRuleInstance.BenefitSetName]);

        if (isCurrentRowReadOnly) {
            $(elementIDs.altRuleDialogJQ).find('.subsection').find('input, select, img.ui-datepicker-trigger, textarea').prop('disabled', true);
        }

        if (!$(elementIDs.altRuleDialogJQ).hasClass('ui-dialog-content')) {
            $(elementIDs.altRuleDialogJQ).dialog({
                modal: true,
                autoOpen: false,
                draggable: true,
                resizable: true,
                zIndex: 1005,
                width: 700,
                closeOnEscape: false,
                title: 'AltRulesData Details',
                buttons: [{
                    id: "AltRulesDialogSave",
                    text: "Save",
                    click: function () {
                        var rowId = $(elementIDs.altRuleGridJQ).jqGrid('getGridParam', 'selrow');
                        $(elementIDs.altRuleGridJQ).saveRow(rowId);
                        var rowData = $(elementIDs.altRuleGridJQ).jqGrid('getGridParam', 'data');
                        var benifitSetIndex = $(elementIDs.spanAltRulesDataIndexJQ).val();
                        var zeroTierAltRuleDataObj = {};
                        zeroTierAltRuleDataObj.RowIDProperty = '0'
                        zeroTierAltRuleDataObj.SESEID = $(elementIDs.spanAltRulesDataSESEIDJQ).text();
                        zeroTierAltRuleDataObj.BenefitCategory1 = $(elementIDs.spanAltRulesDataBenefitCategory1JQ).text();
                        zeroTierAltRuleDataObj.BenefitCategory2 = $(elementIDs.spanAltRulesDataBenefitCategory2JQ).text();
                        zeroTierAltRuleDataObj.BenefitCategory3 = $(elementIDs.spanAltRulesDataBenefitCategory3JQ).text();
                        zeroTierAltRuleDataObj.PlaceofService = $(elementIDs.spanAltRulesDataPlaceofServiceJQ).text();
                        zeroTierAltRuleDataObj.TierNo = $(elementIDs.spanAltRulesPopupSectionTierNoJQ).text();

                        zeroTierAltRuleDataObj.Copay = $(elementIDs.selectAltRulesPopupSectionCopayJQ).val();
                        zeroTierAltRuleDataObj.Coinsurance = $(elementIDs.selectAltRulesPopupSectionCoinsuranceJQ).val();
                        zeroTierAltRuleDataObj.AllowedAmount = $(elementIDs.sectionAltRulesPopupSectionAllowedAmountJQ).val();
                        zeroTierAltRuleDataObj.Covered = $(elementIDs.checkboxAltRulesPopupSectionCoveredJQ).val();
                        if ($(elementIDs.checkboxAltRulesPopupSectionCoveredJQ).prop('checked')) zeroTierAltRuleDataObj.Covered = 'Yes'; else zeroTierAltRuleDataObj.Covered = 'No';
                        zeroTierAltRuleDataObj.AllowedCounter = $(elementIDs.sectionAltRulesPopupSectionAllowedCounterJQ).val();
                        zeroTierAltRuleDataObj.SERLMessage = $(elementIDs.selectAltRulesPopupSectionSERLMessageJQ).val();
                        zeroTierAltRuleDataObj.DisallowedMessage = $(elementIDs.selectAltRulesPopupSectionDisallowedMessagesJQ).val();
                        zeroTierAltRuleDataObj.DeductibleAccumulator = $(elementIDs.sectionAltRulesPopupSectionDeductibleAccumulatorJQ).val();
                        zeroTierAltRuleDataObj.BenefitSetName = benefitRowData["INL_" + dataSourceName + "_" + benifitSetIndex + "_" + customRuleInstance.BenefitSetName];
                        zeroTierAltRuleDataObj.PenaltyType = $(elementIDs.selectAltRulesPopupSectionPenaltyTypeJQ).val();
                        zeroTierAltRuleDataObj.PenaltyCalculationIndicator = $(elementIDs.selectAltRulesPopupSectionPenaltyCalculationIndicatorJQ).val();
                        zeroTierAltRuleDataObj.ExcessPerCounterIndicator = $(elementIDs.selectAltRulesPopupSectionExcessPerCounterIndicatorJQ).val();
                        zeroTierAltRuleDataObj.ServicePenaltyAmount = $(elementIDs.textboxAltRulesPopupSectionServicePenaltyAmountJQ).val();
                        zeroTierAltRuleDataObj.ServicePenaltyPercent = $(elementIDs.textboxAltRulesPopupSectionServicePenaltyPercentJQ).val();
                        zeroTierAltRuleDataObj.MaximumPenaltyAmount = $(elementIDs.textboxAltRulesPopupSectionMaximumPenaltyAmountJQ).val();
                        zeroTierAltRuleDataObj.PenaltyExplanationCode = $(elementIDs.textboxAltRulesPopupSectionPenaltyExplanationCodeJQ).val();
                        zeroTierAltRuleDataObj.ServicePenaltyOptions = $(elementIDs.textboxAltRulesPopupSectionServicePenaltyOptionsJQ).val();
                        zeroTierAltRuleDataObj.AlternateRuleCondition = $(elementIDs.textboxAltRulesPopupSectionAlternateRuleConditionJQ).val();

                        customRuleInstance.saveBenefitReviewGridAltRulesData(benefitRowData, rowData, zeroTierAltRuleDataObj, formInstancebuilder, currentInstance, dataSourceName, index);
                        $(this).dialog("close");
                    }
                }, {
                    id: "AltRulesDialogClose",
                    text: "Close",
                    click: function () {
                        $("#altRuleGrid").jqGrid("GridUnload");
                        $(this).dialog("close");
                    }
                }],
                open: function (event, ui) {
                }
            });
        }

        this.loadAltRulesDataGrid(isCurrentRowReadOnly, benefitRowData, formInstancebuilder, dataSourceName, index, benefitReviewAltRuleData);
        var title = "AltRulesData - " + benefitRowData.BenefitCategory1;
        $(elementIDs.altRuleDialogJQ).dialog('option', 'title', title);
        $(elementIDs.altRuleDialogJQ).dialog("open");
        if (isCurrentRowReadOnly) {
            var altGrid = $(elementIDs.altRuleDialogJQ);
            altGrid.find('#AltRulesAdd, #AltRulesClose').addClass('ui-state-disabled');
            $("#AltRulesDialogSave").prop("disabled", true).addClass("ui-state-disabled");
            //altGrid.dialog('option', 'buttons').addClass('ui-state-disabled');
            altGrid.click(function () {
                $(this).find("table.ui-jqgrid-btable").find('input, select, img.ui-datepicker-trigger').prop('disabled', 'disabled');
                $(this).find("table.ui-jqgrid-btable").find(".jqgrow").attr('disabled', 'disabled');
                $(this).find("table.ui-jqgrid-btable").find("img.ui-datepicker-trigger").prop('disabled', 'disabled');
            });
        }

    } catch (e) {
        console.log("error occurred in getAltRulesData - " + e);
    }
}

customRulePQ.prototype.InitializeAltRulesZeroTierSection = function (benefitRowData, formInstancebuilder, dataSourceName, benefitReviewAltRuleData, index, elementIDs) {
    var opt = "";

    opt = "";
    $(elementIDs.selectAltRulesPopupSectionSERLMessageJQ).children('option:not(:first)').remove();
    opt = opt = opt + "<option value=''></option>";//this.updateSERLMessageOnAltRulePopup(formInstancebuilder, benefitRowData, '', index);
    $(elementIDs.selectAltRulesPopupSectionSERLMessageJQ).append(opt);

    opt = "";
    $(elementIDs.selectAltRulesPopupSectionDisallowedMessagesJQ).children('option:not(:first)').remove();
    opt = "<option value=''></option>";//this.updateDisallowedMessageOnAltRulePopup(formInstancebuilder, benefitRowData, '', index);
    $(elementIDs.selectAltRulesPopupSectionDisallowedMessagesJQ).append(opt);

    if (!this.isAltRulesDialogInitialized) {
        // below statements to fill dropdowns.
        opt = "";
        $(elementIDs.selectAltRulesPopupSectionCopayJQ).children('option:not(:first)').remove();
        $.each(getElementDetails(benefitReviewAltRuleData, 'Copay').Items, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.selectAltRulesPopupSectionCopayJQ).append(opt);

        opt = "";
        $(elementIDs.selectAltRulesPopupSectionCoinsuranceJQ).children('option:not(:first)').remove();
        $.each(getElementDetails(benefitReviewAltRuleData, 'Coinsurance').Items, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.selectAltRulesPopupSectionCoinsuranceJQ).append(opt);

        opt = "";
        $(elementIDs.sectionAltRulesPopupSectionAllowedAmountJQ).children('option:not(:first)').remove();
        $.each(getElementDetails(benefitReviewAltRuleData, 'AllowedAmount').Items, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.sectionAltRulesPopupSectionAllowedAmountJQ).append(opt);

        opt = "";
        $(elementIDs.sectionAltRulesPopupSectionAllowedCounterJQ).children('option:not(:first)').remove();
        $.each(getElementDetails(benefitReviewAltRuleData, 'AllowedCounter').Items, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.sectionAltRulesPopupSectionAllowedCounterJQ).append(opt);

        opt = "";
        $(elementIDs.sectionAltRulesPopupSectionDeductibleAccumulatorJQ).children('option:not(:first)').remove();
        opt = opt + '<option value="0">0</option>'
        $.each(formInstancebuilder.formData.Deductibles.DeductiblesList, function (i, v) {
            if (v["BenefitSetName"] == benefitRowData["INL_" + dataSourceName + "_" + index + "_" + "BenefitSetName"]) {
                opt = opt + '<option value="' + v.AccumNumber + "-" + v.Description + '">' + v.AccumNumber + "-" + v.Description + '</option>'
            }
        });
        $(elementIDs.sectionAltRulesPopupSectionDeductibleAccumulatorJQ).append(opt);

        opt = "";
        $(elementIDs.selectAltRulesPopupSectionPenaltyTypeJQ).children('option:not(:first)').remove();
        $.each(getElementDetails(benefitReviewAltRuleData, 'PenaltyType').Items, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.selectAltRulesPopupSectionPenaltyTypeJQ).append(opt);

        opt = "";
        $(elementIDs.selectAltRulesPopupSectionPenaltyCalculationIndicatorJQ).children('option:not(:first)').remove();
        $.each(getElementDetails(benefitReviewAltRuleData, 'PenaltyCalculationIndicator').Items, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.selectAltRulesPopupSectionPenaltyCalculationIndicatorJQ).append(opt);

        opt = "";
        $(elementIDs.selectAltRulesPopupSectionExcessPerCounterIndicatorJQ).children('option:not(:first)').remove();
        $.each(getElementDetails(benefitReviewAltRuleData, 'ExcessPerCounterIndicator').Items, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.selectAltRulesPopupSectionExcessPerCounterIndicatorJQ).append(opt);

        this.BenefitReviewGridAltRulesData = new Array();
        this.BenefitReviewGridAltRulesData = formInstancebuilder.formData[this.sectionName.benefitReviewGridGenrated][this.sectionName.benefitReviewGridAltRulesDataGenrated];

        isAltRulesDialogInitialized = true;
    }

    if (this.BenefitReviewGridAltRulesData.length == 0)
        this.BenefitReviewGridAltRulesData = formInstancebuilder.formData.BenefitReview.BenefitReviewGridAltRulesData;

    $(elementIDs.checkboxAltRulesPopupSectionCoveredJQ).prop('checked', false);
    $(elementIDs.textboxAltRulesPopupSectionServicePenaltyAmountJQ).val('');
    $(elementIDs.textboxAltRulesPopupSectionServicePenaltyPercentJQ).val('');
    $(elementIDs.textboxAltRulesPopupSectionMaximumPenaltyAmountJQ).val('');
    $(elementIDs.textboxAltRulesPopupSectionPenaltyExplanationCodeJQ).val('');
    $(elementIDs.selectAltRulesPopupSectionExcessPerCounterIndicatorJQ).val('');
    $(elementIDs.selectAltRulesPopupSectionSERLMessageJQ).val('');
    $(elementIDs.selectAltRulesPopupSectionDisallowedMessagesJQ).val('');
    $(elementIDs.spanAltRulesPopupSectionTierNoJQ).text('0');
    $(elementIDs.selectAltRulesPopupSectionCopayJQ).val('');
    $(elementIDs.selectAltRulesPopupSectionCoinsuranceJQ).val('');
    $(elementIDs.selectAltRulesPopupSectionPenaltyTypeJQ).val('');
    $(elementIDs.selectAltRulesPopupSectionPenaltyCalculationIndicatorJQ).val('');
    $(elementIDs.sectionAltRulesPopupSectionAllowedAmountJQ).val('');
    $(elementIDs.sectionAltRulesPopupSectionAllowedCounterJQ).val('');
    $(elementIDs.sectionAltRulesPopupSectionDeductibleAccumulatorJQ).val('');
    $(elementIDs.spanAltRulesDataIndexJQ).val(index);
    $(elementIDs.labelAltRulesPopupSectionSERLMessageJQ).val('');
    $(elementIDs.labelAltRulesPopupSectionDisallowedMessageJQ).val('');
    $(elementIDs.textboxAltRulesPopupSectionServicePenaltyOptionsJQ).val('');
    $(elementIDs.textboxAltRulesPopupSectionAlternateRuleConditionJQ).val('');

    var BenefitSetName = benefitRowData["INL_" + dataSourceName + "_" + index + "_" + this.BenefitSetName];

    //To fill 'Zero Tier Data' to Details Section
    var ZeroTieraltRuleData = new Array();

    if (this.BenefitReviewGridAltRulesData != undefined) {
        // benefitReviewGridAltRulesDatalength = this.BenefitReviewGridAltRulesData.length;
        if (this.BenefitReviewGridAltRulesData.length > 0) {
            ZeroTieraltRuleData = this.BenefitReviewGridAltRulesData.filter(function (ct) {
                if (ct.BenefitSetName === BenefitSetName) {
                    if (ct.SESEID === benefitRowData.SESEID
                        && ct.BenefitCategory1 === benefitRowData.BenefitCategory1
                        && ct.BenefitCategory2 === benefitRowData.BenefitCategory2
                        && ct.BenefitCategory3 === benefitRowData.BenefitCategory3
                        && ct.PlaceofService === benefitRowData.PlaceofService) {
                        return ct;
                    }
                }
            });
        }
    }

    if (ZeroTieraltRuleData.length > 0) {
        if (ZeroTieraltRuleData[0].Covered == 'Yes') $(elementIDs.checkboxAltRulesPopupSectionCoveredJQ).prop('checked', true);
        //$(elementIDs.checkboxAltRulesPopupSectionCoveredJQ).val(ZeroTieraltRuleData[0].Covered);
        $(elementIDs.labelAltRulesPopupSectionSERLMessageJQ).text(ZeroTieraltRuleData[0].SERLMessage);
        $(elementIDs.labelAltRulesPopupSectionDisallowedMessageJQ).text(ZeroTieraltRuleData[0].DisallowedMessage);
        $(elementIDs.spanAltRulesPopupSectionTierNoJQ).text(ZeroTieraltRuleData[0].TierNo);
        $(elementIDs.selectAltRulesPopupSectionCopayJQ).val(ZeroTieraltRuleData[0].Copay);
        $(elementIDs.selectAltRulesPopupSectionCoinsuranceJQ).val(ZeroTieraltRuleData[0].Coinsurance);
        $(elementIDs.selectAltRulesPopupSectionPenaltyTypeJQ).val(ZeroTieraltRuleData[0].PenaltyType);
        $(elementIDs.selectAltRulesPopupSectionPenaltyCalculationIndicatorJQ).val(ZeroTieraltRuleData[0].PenaltyCalculationIndicator);
        $(elementIDs.sectionAltRulesPopupSectionAllowedAmountJQ).val(ZeroTieraltRuleData[0].AllowedAmount);
        $(elementIDs.sectionAltRulesPopupSectionAllowedCounterJQ).val(ZeroTieraltRuleData[0].AllowedCounter);
        $(elementIDs.sectionAltRulesPopupSectionDeductibleAccumulatorJQ).val(ZeroTieraltRuleData[0].DeductibleAccumulator);
        $(elementIDs.textboxAltRulesPopupSectionServicePenaltyAmountJQ).val(ZeroTieraltRuleData[0].ServicePenaltyAmount);
        $(elementIDs.selectAltRulesPopupSectionExcessPerCounterIndicatorJQ).val(ZeroTieraltRuleData[0].ExcessPerCounterIndicator);
        $(elementIDs.textboxAltRulesPopupSectionServicePenaltyPercentJQ).val(ZeroTieraltRuleData[0].ServicePenaltyPercent);
        $(elementIDs.textboxAltRulesPopupSectionMaximumPenaltyAmountJQ).val(ZeroTieraltRuleData[0].MaximumPenaltyAmount);
        $(elementIDs.textboxAltRulesPopupSectionPenaltyExplanationCodeJQ).val(ZeroTieraltRuleData[0].PenaltyExplanationCode);
        $(elementIDs.textboxAltRulesPopupSectionServicePenaltyOptionsJQ).val(ZeroTieraltRuleData[0].ServicePenaltyOptions);
        $(elementIDs.textboxAltRulesPopupSectionAlternateRuleConditionJQ).val(ZeroTieraltRuleData[0].AlternateRuleCondition);
    }

}

customRulePQ.prototype.loadAltRulesDataGrid = function (isCurrentRowReadOnly, benefitRowData, formInstancebuilder, dataSourceName, index, benefitReviewAltRuleData) {
    var customRuleInstance = this;

    var elementIDs = {
        altRuleGrid: "altRuleGrid",
        altRuleGridJQ: "#altRuleGrid",
        altRuleDialogJQ: "#altRuleDialog"
    };
    var altRuleDetailsArray = new Array();

    altRuleDetailsArray = this.getAltRulesData(benefitRowData, formInstancebuilder, dataSourceName, index)

    $(elementIDs.altRuleGridJQ).jqGrid("GridUnload");

    //code to generate Benefit review grid
    var lastsel2;
    $(elementIDs.altRuleGridJQ).jqGrid({
        datatype: "local",
        editurl: 'clientArray',
        cache: false,
        autowidth: true,
        width: 700,
        rowNum: 100000,
        rowheader: true,
        loadonce: true,
        altRows: true,
        altclass: 'alternate',
        scrollrows: true,
        hidegrid: false,
        pager: '#p' + elementIDs.altRuleGrid,
        colNames: ['RowIDProperty', 'SESEID', 'BenefitCategory1', 'BenefitCategory2', 'BenefitCategory3', 'PlaceofService', 'Tier No', 'Copay', 'Coinsurance', 'Allowed Amount', 'Covered', 'Allowed Counter', 'SERLMessage', 'DisallowedMessage', 'Deductible Accumulator', 'Benefit Set Name', 'Penalty Type', 'Penalty Calculation Indicator', 'Service Penalty Amount', 'Service Penalty Percent', 'Maximum Penalty Amount', 'Penalty Explanation Code', 'Excess Per Counter Indicator'],
        colModel: [
          { name: 'RowIDProperty', index: 'RowIDProperty', width: 100, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'SESEID', index: 'SESEID', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'BenefitCategory1', index: 'BenefitCategory1', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'BenefitCategory2', index: 'BenefitCategory2', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'BenefitCategory3', index: 'BenefitCategory3', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'PlaceofService', index: 'PlaceofService', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'TierNo', index: 'TierNo', width: 100, fixed: true, editable: false, sortable: false },
          {
              name: 'Copay', index: 'Copay', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                  value: this.getValues("Copay", benefitReviewAltRuleData)
              },
          },
        {
            name: 'Coinsurance', index: 'Coinsurance', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("Coinsurance", benefitReviewAltRuleData)
            },
        },
        {
            name: 'AllowedAmount', index: 'AllowedAmount', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("AllowedAmount", benefitReviewAltRuleData)
            },
        },
        {
            name: 'Covered', index: 'Covered', width: 100, fixed: true, align: 'center', sortable: false, editable: true, hidden: true,
            formatter: function (cellvalue, options, rowObject) {
                if (rowObject.Covered == "Yes" || rowObject.Covered == "True") {
                    return '<span align="center" class="ui-icon ui-icon-check" style="margin:auto" ></span>';
                }
                else {
                    return '<span align="center" class="ui-icon ui-icon-close" style="margin:auto" ></span>';
                }
            },
            unformat: function (cellvalue, options, cell) {
                var checked = $(cell).children('span').attr('class');
                if (checked == "ui-icon ui-icon-check")
                    return 'Yes';
                else
                    return 'No';
            },
            editoptions: { value: "Yes:No" },
            edittype: "checkbox",
        },
        {
            name: 'AllowedCounter', index: 'AllowedCounter', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("AllowedCounter", benefitReviewAltRuleData)
            },
        },
          {
              name: 'SERLMessage', index: 'SERLMessage', width: 100, fixed: true, sortable: false, editable: true, hidden: true, formatter: "select", edittype: "select", editoptions: {
                  value: ""// this.updateSERLMessageOnAltRulePopup(formInstancebuilder, benefitRowData, "SERLMessage", index)

              },
          },
          {
              name: 'DisallowedMessage', index: 'DisallowedMessage', width: 100, fixed: true, sortable: false, editable: true, hidden: true, formatter: "select", edittype: "select", editoptions: {
                  value: ""// this.updateDisallowedMessageOnAltRulePopup(formInstancebuilder, benefitRowData, "DisallowedMessage", index)
              },
          },
        {
            name: 'DeductibleAccumulator', index: 'DeductibleAccumulator', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getDeductibleAccumNumberForBenefitReviewGridPopup(formInstancebuilder, benefitRowData["INL_" + dataSourceName + "_" + index + "_" + customRuleInstance.BenefitSetName])
            },
        },
        { name: 'BenefitSetName', index: 'BenefitSetName', width: 100, fixed: true, sortable: false, editable: false, hidden: true, formatter: "text", edittype: "text" },
         {
             name: 'PenaltyType', index: 'PenaltyType', width: 100, fixed: true, sortable: false, editable: true, hidden: true, formatter: "select", edittype: "select", editoptions: {
                 value: this.getValues("PenaltyType", benefitReviewAltRuleData)
             },
         },
        {
            name: 'PenaltyCalculationIndicator', index: 'PenaltyCalculationIndicator', width: 100, fixed: true, sortable: false, editable: true, hidden: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("PenaltyCalculationIndicator", benefitReviewAltRuleData)
            },
        },
        { name: 'ServicePenaltyAmount', index: 'ServicePenaltyAmount', width: 100, fixed: true, sortable: false, hidden: true, editable: true, formatter: "text", edittype: "text" },
        { name: 'ServicePenaltyPercent', index: 'ServicePenaltyPercent', width: 100, fixed: true, sortable: false, hidden: true, editable: true, formatter: "text", edittype: "text" },
        { name: 'MaximumPenaltyAmount', index: 'MaximumPenaltyAmount', width: 100, fixed: true, sortable: false, hidden: true, editable: true, formatter: "text", edittype: "text" },
        { name: 'PenaltyExplanationCode', index: 'PenaltyExplanationCode', width: 100, fixed: true, sortable: false, hidden: true, editable: true, formatter: "text", edittype: "text" },
        {
            name: 'ExcessPerCounterIndicator', index: 'ExcessPerCounterIndicator', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("ExcessPerCounterIndicator", benefitReviewAltRuleData)
            },
        },
        ],
        onSelectRow: function (id) {
            if (id && id !== lastsel2) {
                $(elementIDs.altRuleGridJQ).saveRow(lastsel2);
                $(elementIDs.altRuleGridJQ).restoreRow(lastsel2);
                $(elementIDs.altRuleGridJQ).editRow(id, true);
                lastsel2 = id;
            }
        },
        caption: "Alt Rules",
        gridComplete: function () {
            if (isCurrentRowReadOnly) {
                var currentPHQFormUtilities = new formUtilities(formInstancebuilder.formInstanceId);
                currentPHQFormUtilities.sectionManipulation.disableRepeater(elementIDs.altRuleGridJQ);
            }
        }
    });

    var pagerElement = '#p' + elementIDs.altRuleGrid;
    //remove default buttons
    $(elementIDs.altRuleGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    $(elementIDs.altRuleGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus', id: "AltRulesAdd",
        onClickButton: function () {
            //var rowID = benefitReviewGridAltRulesDatalength + 1;
            var tierNoID = altRuleDetailsArray.length + 1;
            var rowToAdd = {
                RowIDProperty: tierNoID,
                SESEID: benefitRowData.SESEID,
                BenefitCategory1: benefitRowData.BenefitCategory1,
                BenefitCategory2: benefitRowData.BenefitCategory2,
                BenefitCategory3: benefitRowData.BenefitCategory3,
                PlaceofService: benefitRowData.PlaceofService,
                TierNo: tierNoID,
                Copay: "",
                Coinsurance: "",
                AllowedAmount: "",
                Covered: "",
                AllowedCounter: "",
                SERLMessage: "",
                DisallowedMessage: "",
                DeductibleAccumulator: "",
                BenefitSetName: benefitRowData["INL_" + dataSourceName + "_" + index + "_" + customRuleInstance.BenefitSetName],
                PenaltyType: "",
                PenaltyCalculationIndicator: "",
                ServicePenaltyAmount: "",
                ServicePenaltyPercent: "",
                MaximumPenaltyAmount: "",
                PenaltyExplanationCode: "",
                ExcessPerCounterIndicator: ""
            };
            altRuleDetailsArray.push(rowToAdd);

            $(elementIDs.altRuleGridJQ).jqGrid('addRowData', tierNoID, rowToAdd);

            $(elementIDs.altRuleGridJQ).jqGrid('setSelection', tierNoID);
            //benefitReviewGridAltRulesDatalength++;
        }
    });

    $(elementIDs.altRuleGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-trash', id: "AltRulesClose",
        onClickButton: function () {
            var rowId = $(elementIDs.altRuleGridJQ).getGridParam('selrow');
            if (rowId) {
                $(elementIDs.altRuleGridJQ).jqGrid('delRowData', rowId);
                $(elementIDs.altRuleGridJQ).jqGrid('setSelection', rowId - 1);

            }
        }
    });

    $(elementIDs.altRuleGridJQ).jqGrid("clearGridData");
    //$(elementIDs.altRuleGridJQ).pqGrid('option', 'dataModel.data', []);
    //$(elementIDs.altRuleGridJQ).pqGrid('refreshView');
    //$(elementIDs.altRuleGridJQ).pqGrid("option", "dataModel", { data: altRuleDetailsArray });
    //$(elementIDs.altRuleGridJQ).pqGrid('refreshView');
    $(elementIDs.altRuleGridJQ).jqGrid('setGridParam', { data: altRuleDetailsArray }).trigger("reloadGrid");
    $(elementIDs.altRuleGridJQ).jqGrid('setSelection', 0);
    $(elementIDs.altRuleGridJQ).editRow(0, true);
}

customRulePQ.prototype.getAltRulesData = function (benefitRowData, formInstancebuilder, dataSourceName, index) {
    var altRuleDataArray = new Array();
    try {

        var BenefitSetName = benefitRowData["INL_" + dataSourceName + "_" + index + "_" + this.BenefitSetName];

        var altRuleData = new Array();
        // benefitReviewGridAltRulesDatalength = formInstancebuilder.formData.BenefitReview.BenefitReviewAltRulesGridTierData.length;
        if (formInstancebuilder.formData.BenefitReview.BenefitReviewAltRulesGridTierData.length > 0) {
            altRuleData = formInstancebuilder.formData.BenefitReview.BenefitReviewAltRulesGridTierData.filter(function (ct) {
                if (ct.BenefitSetName === BenefitSetName) {
                    if (ct.SESEID === benefitRowData.SESEID &&
                        ct.BenefitCategory1 === benefitRowData.BenefitCategory1 &&
                        ct.BenefitCategory2 === benefitRowData.BenefitCategory2 &&
                        ct.BenefitCategory3 === benefitRowData.BenefitCategory3 &&
                        ct.PlaceofService === benefitRowData.PlaceofService
                            && ct.TierNo > '0') {
                        return ct;
                    }
                }
            });
        }

        if (altRuleData.length === 0) {
            //var altRuleFirstRow = formInstancebuilder.formData.BenefitReview.BenefitReviewGridAltRulesData[0];
            //var altRuleRow = $.extend({}, true, altRuleFirstRow);
            //altRuleDataArray.push(altRuleRow);
        }
        else {
            for (var i = 0; i < altRuleData.length; i++) {
                altRuleData[i].TierNo = i + 1;
                altRuleData[i].id = i + 1;
                altRuleDataArray.push(altRuleData[i]);
            }
        }

    } catch (e) {
        console.log("error occurred in getAltRule - " + e);
    }

    return altRuleDataArray;
}

customRulePQ.prototype.getValues = function (name, design) {
    var items = design.Elements.filter(function (ct) {
        if (ct.GeneratedName === name)
            return ct.Items;
    });
    items = items[0].Items;

    var options = "";
    if (items != null && items.length > 0) {
        options = options + "" + ':' + Validation.selectOne + ';';
        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                options = options + items[idx].ItemValue + ':' + items[idx].ItemValue;
                if (idx < items.length - 1) {
                    options = options + ';';
                }
            }
        }
    }
    return options;

}

customRulePQ.prototype.getDeductibleAccumNumberForBenefitReviewGridPopup = function (formInstanceBuilder, BenefitSetName) {
    try {
        // if (FormTypes.PRODUCTFACETSQHPFORMID == formInstanceBuilder.formDesignId) {
        var deductibleList = new Array();
        var customRuleInstance = this;
        var defaultObj = {
            Enabled: null,
            ItemID: 0,
            ItemValue: 0
        };
        deductibleList.push(defaultObj);

        $.each(formInstanceBuilder.formData[customRuleInstance.elementName.deductibles][customRuleInstance.elementName.deductibleList], function (i, v) {
            //if (v[customRuleInstance.BenefitSetName] == BenefitSetName) {
            var obj = {
                Enabled: null,
                ItemID: 0,
                ItemValue: v.AccumNumber + '-' + v.Description
            };

            var isObjExist = deductibleList.filter(function (dt) {
                return dt.ItemValue == obj.ItemValue;
            })

            if (isObjExist.length == 0) {
                deductibleList.push(obj);
            }
            //}
        });
        var options = "";
        if (deductibleList != null && deductibleList.length > 0) {
            options = options + "" + ':' + Validation.selectOne + ';';
            for (var idx = 0; idx < deductibleList.length; idx++) {
                if (deductibleList[idx].ItemValue != '' || deductibleList[idx].ItemValue == 0) {
                    options = options + deductibleList[idx].ItemValue + ':' + deductibleList[idx].ItemValue;
                    if (idx < deductibleList.length - 1) {
                        options = options + ';';
                    }
                }
            }
        }

        return options;
        //}
    } catch (e) {

    }
    return new Array();
}

customRulePQ.prototype.saveBenefitReviewGridAltRulesData = function (benefitRowData, rowData, zeroTeirData, formInstancebuilder, currentInstance, dataSourceName, networkindex) {
    var customRuleInstance = this;
    var benefitReviewAltRulesGridTierDataLength = currentInstance.formInstanceBuilder.formData.BenefitReview.BenefitReviewAltRulesGridTierData.length - 1;
    var oldAltRuleData = formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].filter(function (tRows) {
        return zeroTeirData.SESEID == tRows.SESEID && benefitRowData["INL_" + dataSourceName + "_" + networkindex + "_" + customRuleInstance.BenefitSetName] == tRows.BenefitSetName;
    })
    if (rowData.length > 0) {
        for (var i = 0; i < rowData.length; i++) {
            rowData[i].TierNo = i + 1;
            var isDataExist = oldAltRuleData.filter(function (dt) {
                if (rowData[i].id == dt.id) {
                    var colCount = 0;
                    //repeater existing row change activity log
                    for (var prop in rowData[i]) {
                        if (dt.hasOwnProperty(prop)) {
                            if (dt[prop] != rowData[i][prop] && prop != "TierNo" && prop != "Tier" && prop != "RowIDProperty" && prop != "id") {
                                var oldValue = dt[prop] == undefined ? "" : dt[prop];
                                var newValue = rowData[i][prop];
                                var colName = prop;
                                colName = currentInstance.columnNames.filter(function (dt) {
                                    return dt.index == colName;
                                });
                                colName = colName[0];
                                //Update row activity
                                currentInstance.addEntryToAcitivityLogger(rowData[i].RowIDProperty - 1, Operation.UPDATE, colName, oldValue, newValue, undefined, "Benefit Review Alt Rules Grid Tier Data", prop);
                            }
                        }
                        colCount++;
                    }
                    return dt;
                }
            })
            if (isDataExist.length == 0) {
                //ADD new row activity
                currentInstance.addEntryToAcitivityLogger(benefitReviewAltRulesGridTierDataLength, Operation.ADD, undefined, undefined, undefined, undefined, "Benefit Review Alt Rules Grid Tier Data")
                benefitReviewAltRulesGridTierDataLength++;
            }
        }
    }
    //DELETE row activity
    $.grep(oldAltRuleData, function (el) {
        var isDataExist = rowData.filter(function (dt) {
            if (el.id == dt.id)
                return dt;
        })
        if (isDataExist.length == 0) {
            currentInstance.addEntryToAcitivityLogger(el.RowIDProperty - 1, Operation.DELETE, undefined, undefined, undefined, undefined, "Benefit Review Alt Rules Grid Tier Data")
        }
    });

    var oldZeroTeirData = new Array();
    var benefitReviewAltRulesGridTierData = formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated];
    var benefitReviewGridAltRulesData = formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated];
    var dataToUpdate = new Array();
    var zeroTeir = new Array();
    zeroTeir.push(zeroTeirData);
    $.each(rowData, function (i, row) { dataToUpdate.push(row) });
    var indexToDelete = new Array();
    if (benefitReviewGridAltRulesData != undefined) {
        for (var i = 0; i < benefitReviewGridAltRulesData.length; i++) {
            if ((zeroTeirData.SESEID === benefitReviewGridAltRulesData[i].SESEID &&
                        zeroTeirData.BenefitSetName === benefitReviewGridAltRulesData[i].BenefitSetName
                && zeroTeirData.BenefitCategory1 === benefitReviewGridAltRulesData[i].BenefitCategory1
                && zeroTeirData.BenefitCategory2 === benefitReviewGridAltRulesData[i].BenefitCategory2
                && zeroTeirData.BenefitCategory3 === benefitReviewGridAltRulesData[i].BenefitCategory3
                && zeroTeirData.PlaceofService === benefitReviewGridAltRulesData[i].PlaceofService)
                || (benefitReviewGridAltRulesData[i].SESEID === "" &&
                benefitReviewGridAltRulesData[i].BenefitCategory1 === "" &&
                benefitReviewGridAltRulesData[i].BenefitCategory2 === "" &&
                benefitReviewGridAltRulesData[i].BenefitCategory3 === "" &&
                benefitReviewGridAltRulesData[i].PlaceofService === "")) {
                oldZeroTeirData.push(benefitReviewGridAltRulesData[i]);//Capture old data for activity log
                formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated].splice(i, 1);
                i--;
            }
        }
    }
    //Activity for "Benefit Review Alt Rules Grid"
    if (oldZeroTeirData.length > 0) {
        if (oldZeroTeirData[0].SESEID == zeroTeirData.SESEID) {
            var colCount = 0;
            for (var prop in oldZeroTeirData[0]) {
                if (zeroTeirData[prop] != oldZeroTeirData[0][prop] && prop != "TierNo" && prop != "Tier" && prop != "RowIDProperty" && prop != "id") {
                    var newValue = zeroTeirData[prop] == undefined ? "" : zeroTeirData[prop];
                    var oldValue = oldZeroTeirData[0][prop];
                    var colName = prop;
                    colName = currentInstance.columnNames.filter(function (zeroTeirData) {
                        //return zeroTeirData.replace('<font color=red>*</font>', '').replace(/ /g, '').trim() == colName;
                        return zeroTeirData.index == colName;
                    });
                    colName = colName[0];
                    currentInstance.addEntryToAcitivityLogger(oldZeroTeirData[0].RowIDProperty + 1, Operation.UPDATE, colName, oldValue, newValue, undefined, "Benefit Review Alt Rules Grid", prop);
                }
                colCount++;
            }
        }
    }
    if (zeroTeir.length > 0) {
        if (this.BenefitReviewGridAltRulesData === undefined || this.BenefitReviewGridAltRulesData === null) {
            BenefitReviewGridAltRulesData = new Array();
        }
        for (var i = 0; i < zeroTeir.length; i++) {
            formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated].push(zeroTeirData);
        }
    }

    if (benefitReviewAltRulesGridTierData != undefined) {
        for (var i = 0; i < benefitReviewAltRulesGridTierData.length; i++) {
            if ((zeroTeirData.SESEID === benefitReviewAltRulesGridTierData[i].SESEID &&
                 zeroTeirData.BenefitSetName === benefitReviewAltRulesGridTierData[i].BenefitSetName
                && zeroTeirData.BenefitCategory1 === benefitReviewAltRulesGridTierData[i].BenefitCategory1
                && zeroTeirData.BenefitCategory2 === benefitReviewAltRulesGridTierData[i].BenefitCategory2
                && zeroTeirData.BenefitCategory3 === benefitReviewAltRulesGridTierData[i].BenefitCategory3
                && zeroTeirData.PlaceofService === benefitReviewAltRulesGridTierData[i].PlaceofService)
                || (benefitReviewAltRulesGridTierData[i].SESEID === "" &&
                benefitReviewAltRulesGridTierData[i].BenefitCategory1 === "" &&
                benefitReviewAltRulesGridTierData[i].BenefitCategory2 === "" &&
                benefitReviewAltRulesGridTierData[i].BenefitCategory3 === "" &&
                benefitReviewAltRulesGridTierData[i].PlaceofService === "")) {
                formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].splice(i, 1);
                i--;
            }
        }
    }

    if (dataToUpdate.length > 0) {
        if (this.BenefitReviewAltRulesGridTierData === undefined || this.BenefitReviewAltRulesGridTierData === null) {
            BenefitReviewGridAltRulesData = new Array();
        }
        for (var i = 0; i < dataToUpdate.length; i++) {
            dataToUpdate[i].TierNo = i + 1;
            formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].push(dataToUpdate[i]);
        }
    }

    if (formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated] != undefined) {
        for (var i = 0; i < formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated].length; i++) {
            formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated][i].RowIDProperty = i;
            formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated][i]["id"] = i;
        }
    }
    if (formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated] != undefined) {
        for (var i = 0; i < formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].length; i++) {
            formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated][i].RowIDProperty = i;
            formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated][i]["id"] = i;
        }
    }


    //$(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRulesDataJQ + formInstancebuilder.formInstanceId).jqGrid("clearGridData");
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRulesDataJQ + formInstancebuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRulesDataJQ + formInstancebuilder.formInstanceId).pqGrid('refreshDataAndView');
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRulesDataJQ + formInstancebuilder.formInstanceId).pqGrid("option", "dataModel", {
        data: formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated]
    });
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRulesDataJQ + formInstancebuilder.formInstanceId).pqGrid('refreshDataAndView');
    //$(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRulesDataJQ + formInstancebuilder.formInstanceId).jqGrid('setGridParam',
    //    {
    //        data: formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated]
    //    }).trigger("reloadGrid");

    //$(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).jqGrid("clearGridData");
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid('refreshDataAndView');
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid("option", "dataModel", {
        data: formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated]
    });
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid('refreshDataAndView');
    //$(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).jqGrid('setGridParam',
    //    {
    //        data: formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated]
    //    }).trigger("reloadGrid");
}


customRulePQ.prototype.showTierDataPopUp = function (rowId, isCurrentRowReadOnly, benefitRowData, currentInstance, dataSourceName, networkindex) {
    try {
        var elementIDs = {
            tierDataGrid: "tierDataGrid",
            tierDataGridJQ: "#tierDataGrid",
            tierDataDialogJQ: "#tierDataDialog",
            spanTierDataBenefitCategory1JQ: "#spanTierDataBenefitCategory1",
            spanTierDataBenefitCategory2JQ: "#spanTierDataBenefitCategory2",
            spanTierDataBenefitCategory3JQ: "#spanTierDataBenefitCategory3",
            spanTierDataPlaceofServiceJQ: "#spanTierDataPlaceofService",
            spanTierDataSESEIDJQ: "#spanTierDataSESEID",
            hiddenTierDataIndexJQ: "#hiddenTierDataIndexJQ",
            spanTierBenefitSetNameJQ: "#spanTierBenefitSetName",
        };

        var customRuleInstance = this;
        var formInstancebuilder = currentInstance.formInstanceBuilder;

        $(elementIDs.spanTierDataBenefitCategory1JQ).text(benefitRowData.BenefitCategory1);
        $(elementIDs.spanTierDataBenefitCategory2JQ).text(benefitRowData.BenefitCategory2);
        $(elementIDs.spanTierDataBenefitCategory3JQ).text(benefitRowData.BenefitCategory3);
        $(elementIDs.spanTierDataPlaceofServiceJQ).text(benefitRowData.PlaceofService);
        $(elementIDs.spanTierDataSESEIDJQ).text(benefitRowData.SESEID);
        $(elementIDs.spanTierBenefitSetNameJQ).text(benefitRowData["INL_" + dataSourceName + "_" + networkindex + "_" + customRuleInstance.BenefitSetName]);
        // $(elementIDs.hiddenTierDataIndexJQ).val(index);

        //if (!$(elementIDs.tierDataDialogJQ).hasClass('ui-dialog-content')) {
        $(elementIDs.tierDataDialogJQ).dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            width: 700,
            closeOnEscape: false,
            title: 'TierData Details',
            buttons: [{
                id: "TierDialogSave",
                text: "Save",
                click: function () {
                    var rowId = $(elementIDs.tierDataGridJQ).jqGrid('getGridParam', 'selrow');
                    $(elementIDs.tierDataGridJQ).saveRow(rowId);
                    var rowData = $(elementIDs.tierDataGridJQ).jqGrid('getGridParam', 'data');

                    //  var tierDataIndex = $(elementIDs.hiddenTierDataIndexJQ).val();
                    if (rowData)
                        customRuleInstance.saveBenefitTierData(benefitRowData, formInstancebuilder, dataSourceName, networkindex, rowData, currentInstance);
                    $(this).dialog("close");
                }
            }, {
                id: "TierDialogClose",
                text: "Close",
                click: function () {
                    // $("#tierDataGrid").jqGrid("GridUnload");
                    $(this).dialog("close");
                    //tierDataDetailsArray
                }
            }],
            open: function (event, ui) {
            }
        });
        //}

        this.loadTierDataGrid(isCurrentRowReadOnly, benefitRowData, formInstancebuilder, dataSourceName, networkindex);
        var title = "TierData - " + benefitRowData.BenefitCategory1;
        $(elementIDs.tierDataDialogJQ).dialog('option', 'title', title);
        $(elementIDs.tierDataDialogJQ).dialog("open");
        if (isCurrentRowReadOnly) {
            var tierGridDialog = $(elementIDs.tierDataDialogJQ);
            tierGridDialog.find('#TierDataAdd, #TierDataDelete').addClass('ui-state-disabled');
            $("#TierDialogSave").prop("disabled", true).addClass("ui-state-disabled");
            tierGridDialog.click(function () {
                $(this).find("table.ui-jqgrid-btable").find('input, select, img.ui-datepicker-trigger').prop('disabled', 'disabled');
                $(this).find("table.ui-jqgrid-btable").find(".jqgrow").attr('disabled', 'disabled');
                $(this).find("table.ui-jqgrid-btable").find("img.ui-datepicker-trigger").prop('disabled', 'disabled');
            });
        }

    } catch (e) {
        console.log("error occurred in getTierData - " + e);
    }
}

customRulePQ.prototype.loadTierDataGrid = function (isCurrentRowReadOnly, benefitRowData, formInstancebuilder, dataSourceName, networkindex) {
    var elementIDs = {
        tierDataGrid: "tierDataGrid",
        tierDataGridJQ: "#tierDataGrid",
        tierDataDialogJQ: "#tierDataDialog"
    };

    customRuleInstance = this;
    benefitReviewGridRowData = benefitRowData;

    formInstancebuilder.designData.Sections.filter(function (ct) {
        if (ct.GeneratedName === customRuleInstance.sectionName.benefitReviewGridGenrated) {
            ct.Elements.filter(function (ele) {
                if (ele.GeneratedName === customRuleInstance.sectionName.benefitReviewGridTierDataGenerated) {
                    if (ele.Repeater.GeneratedName === customRuleInstance.sectionName.benefitReviewGridTierDataGenerated) {
                        benefitReviewTierData = ele.Repeater;
                        return false;
                    }
                }
            });
        }
    });

    tierDataDetailsArray = this.getTierData(benefitRowData, formInstancebuilder, dataSourceName, networkindex);

    //if (!$(elementIDs.tierDataGridJQ).jqGrid('getGridParam')) {
    $(elementIDs.tierDataGridJQ).jqGrid("GridUnload");
    //code to generate Benefit review grid
    var lastsel2;
    $(elementIDs.tierDataGridJQ).jqGrid({
        datatype: "local",
        editurl: 'clientArray',
        cache: false,
        autowidth: true,
        width: 700,
        rowheader: true,
        loadonce: false,
        rowNum: 100000,
        scrollrows: true,
        altRows: true,
        altclass: 'alternate',
        hidegrid: false,
        pager: '#p' + elementIDs.tierDataGrid,
        colNames: ['RowIDProperty', 'Benefit Set Name', 'Tier No', 'SESEID', 'Benefit Category1', 'Benefit Category2', 'Benefit Category3', 'Place of Service', 'Copay', 'Coinsurance', 'Allowed Amount', 'Allowed Counter', 'Deductible Accumulator', 'Excess Per Counter Indicator', ],
        colModel: [
          { name: 'RowIDProperty', index: 'RowIDProperty', width: 100, sorttype: "int", editable: false, hidden: true, sortable: false },
        { name: 'BenefitSetName', index: 'BenefitSetName', width: 100, fixed: true, sortable: false, editable: false, hidden: true, formatter: "text", edittype: "text" },
          { name: 'TierNo', index: 'TierNo', width: 100, fixed: true, editable: false, sortable: false, formatter: "text", edittype: "text" },
          { name: 'SESEID', index: 'SESEID', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false, formatter: "text", edittype: "text" },
          { name: 'BenefitCategory1', index: 'BenefitCategory1', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'BenefitCategory2', index: 'BenefitCategory2', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'BenefitCategory3', index: 'BenefitCategory3', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'PlaceofService', index: 'PlaceofService', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          {
              name: 'Copay', index: 'Copay', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                  value: this.getValues("Copay", benefitReviewTierData)
              },
          },
          {
              name: 'Coinsurance', index: 'Coinsurance', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                  value: this.getValues("Coinsurance", benefitReviewTierData)
              },
          },
        {
            name: 'AllowedAmount', index: 'AllowedAmount', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("AllowedAmount", benefitReviewTierData)
            },
        },
        {
            name: 'AllowedCounter', index: 'AllowedCounter', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("AllowedCounter", benefitReviewTierData)
            },
        },
        {
            name: 'DeductibleAccumulator', index: 'DeductibleAccumulator', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getDeductibleAccumNumberForBenefitReviewGridPopup(formInstancebuilder, benefitRowData["INL_" + dataSourceName + "_" + networkindex + "_" + customRuleInstance.BenefitSetName])
            },
        },
         {
             name: 'ExcessPerCounterIndicator', index: 'ExcessPerCounterIndicator', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                 value: this.getValues("ExcessPerCounterIndicator", benefitReviewTierData)
             },
         }
        ],
        onSelectRow: function (id) {
            if (id && id !== lastsel2) {
                $(elementIDs.tierDataGridJQ).saveRow(lastsel2);
                $(elementIDs.tierDataGridJQ).restoreRow(lastsel2);
                $(elementIDs.tierDataGridJQ).editRow(id, true);
                lastsel2 = id;
            }
        },
        caption: "Tiers",
        gridComplete: function () {
            if (isCurrentRowReadOnly) {
                var currentPHQFormUtilities = new formUtilities(formInstancebuilder.formInstanceId);
                currentPHQFormUtilities.sectionManipulation.disableRepeater(elementIDs.tierDataGridJQ);
            }
        }
    });


    var pagerElement = '#p' + elementIDs.tierDataGrid;
    //remove default buttons
    $(elementIDs.tierDataGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    $(elementIDs.tierDataGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus', id: "TierDataAdd",
        onClickButton: function () {
            var rowID = tierDataDetailsArray.length + 1;
            var tierNoID = tierDataDetailsArray.length + 1;

            var rowToAdd = {
                RowIDProperty: rowID,
                BenefitSetName: benefitReviewGridRowData["INL_" + customRuleInstance.ProductNetworkDS + "_" + networkindex + "_" + customRuleInstance.BenefitSetName],
                TierNo: tierNoID,
                SESEID: benefitReviewGridRowData.SESEID,
                BenefitCategory1: benefitReviewGridRowData.BenefitCategory1,
                BenefitCategory2: benefitReviewGridRowData.BenefitCategory2,
                BenefitCategory3: benefitReviewGridRowData.BenefitCategory3,
                PlaceofService: benefitReviewGridRowData.PlaceofService,
                Copay: "",
                Coinsurance: "",
                AllowedAmount: "",
                AllowedCounter: "",
                DeductibleAccumulator: "",
                ExcessPerCounterIndicator: ""
            };
            tierDataDetailsArray.push(rowToAdd);

            $(elementIDs.tierDataGridJQ).jqGrid('addRowData',
                rowID, rowToAdd);

            $(elementIDs.tierDataGridJQ).jqGrid('setSelection', rowID);
            $(elementIDs.tierDataGridJQ).editRow(rowID, true);

            //benefitRowGridCount++;
            //tierNoID++;
        }
    });

    $(elementIDs.tierDataGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-trash', id: "TierDataDelete",
        onClickButton: function () {
            var rowId = $(elementIDs.tierDataGridJQ).getGridParam('selrow');
            if (rowId) {
                $(elementIDs.tierDataGridJQ).jqGrid('delRowData', rowId);

                //for (var idx = 0; idx < tierDataDetailsArray.length; idx++) {
                //    if (tierDataDetailsArray[idx].RowIDProperty == rowId) {
                //        tierDataDetailsArray.splice(idx, 1);
                //        break;
                //    }
                //}

                //var rowIds = $(elementIDs.tierDataGridJQ).getDataIDs();
                //if (rowIds.length > 0) {
                $(elementIDs.tierDataGridJQ).jqGrid('setSelection', rowId - 1);
                //}

            }
        }
    });
    //}

    $(elementIDs.tierDataGridJQ).jqGrid("clearGridData");

    //for (var i = 0; i < tierDataDetailsArray.length; i++) {
    //    tierDataDetailsArray[i].TierNo = i + 1;
    //    $(elementIDs.tierDataGridJQ).jqGrid('addRowData', tierDataDetailsArray[i].RowIDProperty, tierDataDetailsArray[i]);
    //}
    $(elementIDs.tierDataGridJQ).jqGrid('setGridParam', { data: tierDataDetailsArray }).trigger("reloadGrid");
    $(elementIDs.tierDataGridJQ).jqGrid('setSelection', 0);
    $(elementIDs.tierDataGridJQ).editRow(0, true);
    //benefitRowGridCount = $(elementIDs.tierDataGridJQ).getGridParam("reccount");
    $(elementIDs.tierDataGridJQ).jqGrid('setGridWidth', '650');

}

customRulePQ.prototype.getTierData = function (benefitRowData, formInstancebuilder, dataSourceName, networkindex) {
    var tierData = new Array();
    try {

        var BenefitSetName = benefitRowData["INL_" + dataSourceName + "_" + networkindex + "_" + customRuleInstance.BenefitSetName];


        if (formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData.length > 0) {
            tierData = formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData.filter(function (ct) {
                if (ct.BenefitSetName === BenefitSetName && ct.SESEID === benefitRowData.SESEID &&
                    ct.BenefitCategory1 == benefitRowData.BenefitCategory1 && ct.BenefitCategory2 == benefitRowData.BenefitCategory2
                    && ct.BenefitCategory3 == benefitRowData.BenefitCategory3 && ct.PlaceofService == ct.PlaceofService) {
                    return ct;
                }
            });
        }


        for (var i = 0; i < tierData.length; i++) {
            //tierDataDetailsArray[i].RowIDProperty = i;
            tierData[i].TierNo = i + 1;
            tierData[i].id = i + 1;
        }
    } catch (e) {
        console.log("error occurred in getTierData - " + e);
    }

    return tierData;
}

customRulePQ.prototype.getValuesInDecimal = function (name, design) {
    var items = design.Elements.filter(function (ct) {
        if (ct.GeneratedName === name)
            return ct.Items;
    });
    items = items[0].Items;

    var options = "";
    if (items != null && items.length > 0) {
        options = options + "" + ':' + Validation.selectOne + + ';';
        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                options = options + parseFloat(items[idx].ItemValue).toFixed(2) + ':' + parseFloat(items[idx].ItemValue).toFixed(2);
                if (idx < items.length - 1) {
                    options = options + ';';
                }
            }
        }
    }
    return options;
}

customRulePQ.prototype.saveBenefitTierData = function (benefitRowData, formInstancebuilder, dataSourceName, networkindex, rowData, currentInstance) {
    var newTierData = [];
    var benefitReviewGridTierDataLength = currentInstance.formInstanceBuilder.formData.BenefitReview.BenefitReviewGridTierData.length - 1;
    var oldTierData = formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData.filter(function (tRows) {
        return benefitRowData.SESEID == tRows.SESEID && benefitRowData["INL_" + dataSourceName + "_" + networkindex + "_" + customRuleInstance.BenefitSetName] == tRows.BenefitSetName;
    })
    if (rowData.length > 0) {
        for (var i = 0; i < rowData.length; i++) {
            rowData[i].TierNo = i + 1;
            var isDataExist = oldTierData.filter(function (dt) {
                if (rowData[i].id == dt.id) {
                    var colCount = 0;
                    //repeater existing row change activity log
                    for (var prop in rowData[i]) {
                        if (dt.hasOwnProperty(prop)) {
                            if (dt[prop] != rowData[i][prop] && prop != "TierNo" && prop != "Tier" && prop != "RowIDProperty" && prop != "id") {
                                var oldValue = dt[prop] == undefined ? "" : dt[prop];
                                var newValue = rowData[i][prop];
                                var colName = prop;
                                //colName = currentInstance.columnNames.filter(function (dt) {
                                //    return dt.dataIndx == colName;
                                //});
                                //colName = colName[0];
                                currentInstance.addEntryToAcitivityLogger(rowData[i].RowIDProperty - 1, Operation.UPDATE, colName, oldValue, newValue, undefined, "Benefit Review Grid Tier Data", prop);
                            }
                        }
                        colCount++;
                    }
                    return dt;
                }
            })
            if (isDataExist.length == 0) {
                //ADD new row activity
                currentInstance.addEntryToAcitivityLogger(benefitReviewGridTierDataLength, Operation.ADD, undefined, undefined, undefined, undefined, "Benefit Review Grid Tier Data")
                benefitReviewGridTierDataLength++;
            }
        }
    }
    //DELETE new row activity
    $.grep(oldTierData, function (el) {
        var isDataExist = rowData.filter(function (dt) {
            if (el.id == dt.id)
                return dt;
        })
        if (isDataExist.length == 0) {
            currentInstance.addEntryToAcitivityLogger(el.RowIDProperty - 1, Operation.DELETE, undefined, undefined, undefined, undefined, "Benefit Review Grid Tier Data")
        }
    });

    var benefitReviewGridTierData = formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData;
    for (var i = 0; i < benefitReviewGridTierData.length; i++) {
        if ((benefitReviewGridTierData[i].SESEID === benefitRowData.SESEID &&
            benefitReviewGridTierData[i].BenefitCategory1 === benefitRowData.BenefitCategory1 &&
            benefitReviewGridTierData[i].BenefitCategory2 === benefitRowData.BenefitCategory2 &&
            benefitReviewGridTierData[i].BenefitCategory3 === benefitRowData.BenefitCategory3 &&
            benefitReviewGridTierData[i].PlaceofService === benefitRowData.PlaceofService &&
            benefitReviewGridTierData[i].BenefitSetName === benefitRowData["INL_" + dataSourceName + "_" + networkindex + "_" + customRuleInstance.BenefitSetName])
            || (benefitReviewGridTierData[i].SESEID === "" &&
            benefitReviewGridTierData[i].BenefitCategory1 === "" &&
            benefitReviewGridTierData[i].BenefitCategory2 === "" &&
            benefitReviewGridTierData[i].BenefitCategory3 === "" &&
            benefitReviewGridTierData[i].PlaceofService === "")) {
            formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData.splice(i, 1);
            i--;
        }
    }

    if (rowData.length > 0) {
        for (var i = 0; i < rowData.length; i++) {
            rowData[i].TierNo = i + 1;
            formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData.push(rowData[i]);
        }
    }

    for (var i = 0; i < formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData.length; i++) {
        formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData[i].RowIDProperty = i;
        //if (formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData[i].id != undefined) {
        formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData[i].id = i;
        //}
    }

   // $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridTierDataJQ + formInstancebuilder.formInstanceId).jqGrid("clearGridData");
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridTierDataJQ + formInstancebuilder.formInstanceId) .pqGrid('option', 'dataModel.data', []);
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid('refreshView');
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid("option", "dataModel", { data: formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData });
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid('refreshView');
    //$(formInstancebuilder.customRules.elemenIDs.benefitReviewGridTierDataJQ + formInstancebuilder.formInstanceId).jqGrid('setGridParam', { data: formInstancebuilder.formData.BenefitReview.BenefitReviewGridTierData }).trigger("reloadGrid");
}


customRulePQ.prototype.createControlsAndRegisterEventsForSectionInProductForm = function (formInstancebuilder) {
    switch (formInstancebuilder.selectedSectionName) {
        case this.sectionName.ServiceGroupMandatesSectionName:
            //this.registerControlAndEventForMandateServiceGroup(formInstancebuilder);
            break;
        case this.sectionName.AdditionalServicesSectionName:
            //this.registerControlAndEventForAdditionalServices(formInstancebuilder);
            break;
        case this.sectionName.LimitsSectionName:
            this.registerControlAndEventForLimits(formInstancebuilder);
            break;
    }
}

customRulePQ.prototype.registerControlAndEventForLimits = function (formInstancebuilder) {
    var customRuleInstance = this;
    var elementName = this.elemenIDs.searchLimitListID + formInstancebuilder.formInstanceId;
    var resetElementName = this.elemenIDs.resetLimitListID + formInstancebuilder.formInstanceId;
    $('#' + elementName).parent().append('<button class="btn btn-sm but-align pull-right "style="margin:0px 5px" id="' + resetElementName + '"> Reset </button>' +
      '    <button class="btn btn-sm but-align pull-right"  id="' + elementName + '">Search</button>');
    $('#' + elementName).parent().attr('class', 'col-xs-2 col-md-2 col-lg-2 col-sm-2');
    $('label[name="' + elementName + '"]').remove();


    $(this.elemenIDs.searchLimitListIDJQ + formInstancebuilder.formInstanceId).off("click");
    $(this.elemenIDs.searchLimitListIDJQ + formInstancebuilder.formInstanceId).on("click", function () {
        customRuleInstance.filteringProductGrids(formInstancebuilder);
    });

    $(this.elemenIDs.resetLimitListIDJQ + formInstancebuilder.formInstanceId).off("click");
    $(this.elemenIDs.resetLimitListIDJQ + formInstancebuilder.formInstanceId).on("click", function () {
        limitsListsgridId = customRuleInstance.elemenIDs.limitsListsRepeater + formInstancebuilder.formInstanceId;
        LTSEgridId = customRuleInstance.elemenIDs.limitLTSERepeater + formInstancebuilder.formInstanceId;
        LTLTgridId = customRuleInstance.elemenIDs.limitLTLTRepeater + formInstancebuilder.formInstanceId;
        LTIPgridId = customRuleInstance.elemenIDs.limitLTIPRepeater + formInstancebuilder.formInstanceId;
        LTIDgridId = customRuleInstance.elemenIDs.limitLTIDRepeater + formInstancebuilder.formInstanceId;
        LTPRgridId = customRuleInstance.elemenIDs.limitLTPRRepeater + formInstancebuilder.formInstanceId;

        var array = ['' + limitsListsgridId + '', '' + LTSEgridId + '', '' + LTLTgridId + '', '' + LTIPgridId + '', '' + LTIDgridId + '', '' + LTPRgridId + '']

        for (var i = 0; i < array.length; i++) {
            var gridId = array[i];
            $(gridId).jqGrid('setGridParam', { search: false });
            $(gridId).trigger("reloadGrid");
        }
        $(customRuleInstance.elemenIDs.limitsBenefitSetDDLJQ + formInstancebuilder.formInstanceId).prop("selectedIndex", 0);
    });
}

customRulePQ.prototype.filteringProductGrids = function (formInstancebuilder) {

    var postdata = '';
    var rule = new Array();
    var gridId = '';
    if (formInstancebuilder.selectedSectionName == this.sectionName.LimitsSectionName) {
        limitsListsgridId = this.elemenIDs.limitsListsRepeater + formInstancebuilder.formInstanceId;
        LTSEgridId = this.elemenIDs.limitLTSERepeater + formInstancebuilder.formInstanceId;
        LTLTgridId = this.elemenIDs.limitLTLTRepeater + formInstancebuilder.formInstanceId;
        LTIPgridId = this.elemenIDs.limitLTIPRepeater + formInstancebuilder.formInstanceId;
        LTIDgridId = this.elemenIDs.limitLTIDRepeater + formInstancebuilder.formInstanceId;
        LTPRgridId = this.elemenIDs.limitLTPRRepeater + formInstancebuilder.formInstanceId;

        var array = ['' + limitsListsgridId + '', '' + LTSEgridId + '', '' + LTLTgridId + '', '' + LTIPgridId + '', '' + LTIDgridId + '', '' + LTPRgridId + '']

        for (var i = 0; i < array.length; i++) {
            var gridId = array[i];
            postdata = $(gridId).jqGrid('getGridParam', 'data');
            var limitsBenefitSet = $(this.elemenIDs.limitsBenefitSetDDLJQ + formInstancebuilder.formInstanceId).val();

            if (limitsBenefitSet != '[Select One]') {
                rule.push({ "field": this.BenefitSetName, "op": "eq", "data": limitsBenefitSet });
            }
            if (limitsBenefitSet == '[Select One]' || limitsBenefitSet == '') {
                $(gridId).jqGrid('setGridParam', { search: false });
                $(gridId).trigger("reloadGrid");
                return false;
            }

            if (rule.length > 0) {
                $.extend(postdata,
                           {
                               filters: JSON.stringify({ "groupOp": "AND", "rules": rule }),
                           });
                $(gridId).jqGrid('setGridParam', { search: true, postData: postdata });
                $(gridId).trigger("reloadGrid");
            }
        }
    }
}

customRulePQ.prototype.getDropDownValueOnRowEditMode = function (currentInstance) {
    var customRuleInstance = this;
    switch (currentInstance.fullName) {
        case customRuleInstance.fullName.benefitReviewGridTierData:
        case customRuleInstance.fullName.benefitReviewAltRulesGridTierData:
        case customRuleInstance.fullName.benefitReviewGridAltRulesData:
            customRuleInstance.getDeductibleAccumulatorDropdownValue(currentInstance);
            break;
        case this.fullName.additionalServicesDetailsRepeater:
        case this.fullName.mandateServiceGroupingDetailsRepeater:
        case this.fullName.altRuleServiceGroupDetailRepeater:
        case this.fullName.altRuleAdditionalServicesDetailsRepeater:
            // currentInstance.customrule.getMessageDropdownValue(currentInstance);
            break;
        case this.fullName.facetProductComponentsPDBCRepeater:
            currentInstance.customrule.getPDBCPrefixValue(currentInstance);
            break;
    }
}

customRulePQ.prototype.getMessageDropdownValue = function (currentInstance) {
    var rowdata = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.selectedRowId;
    });;
    var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.messageSERL);
    currentInstance.SERLMessagesValue = $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val();

    if (currentInstance.SERLMessagesValue == undefined) {
        currentInstance.SERLMessagesValue = rowdata[0][currentInstance.customrule.elementName.messageSERL];
    }


    var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.disallowedMessage);
    currentInstance.DisallowedMessageValue = $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val();
    if (currentInstance.DisallowedMessageValue == undefined) {
        currentInstance.DisallowedMessageValue = rowdata[0][currentInstance.customrule.elementName.disallowedMessage];
    }

}

customRulePQ.prototype.getDeductibleAccumulatorDropdownValue = function (currentInstance) {
    var rowdata = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.selectedRowId;
    });;
    var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.deductibleAccumulator);
    currentInstance.DeductibleAccumulatorValue = $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val();

    if (currentInstance.DeductibleAccumulatorValue == undefined) {
        currentInstance.DeductibleAccumulatorValue = rowdata[0][currentInstance.customrule.elementName.deductibleAccumulator];
    }
}

customRulePQ.prototype.bindSERLAndDisallowedMessages = function (currentInstance, isrowEitable) {
    var rowdata = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.selectedRowId;
    });
    if (currentInstance.SERLMessages != undefined) {
        var options = [];
        var selectOneValue = $('<option value="' + Validation.selectOne + '" class="standard-optn"></option>');
        for (var idx = 0; idx < currentInstance.SERLMessages.length; idx++) {
            options[idx] = $('<option value="' + currentInstance.SERLMessages[idx][currentInstance.customrule.elementName.sERLDescription] + '" class="standard-optn">' + currentInstance.SERLMessages[idx][currentInstance.customrule.elementName.sERLDescription] + '</option>');
        }
        options[options.length] = selectOneValue;
        var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.messageSERL);
        $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').empty().append(options);

        if (isrowEitable == "Yes" || currentInstance.isGridDivClick == true) {
            $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(rowdata[0][currentInstance.customrule.elementName.messageSERL]);
        }
        else if (currentInstance.SERLMessagesValue != null && currentInstance.SERLMessagesValue != "[Select One]") {
            $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(currentInstance.SERLMessagesValue);
        }
    }

    if (currentInstance.DisallowedMessage != undefined) {
        var options = [];
        var selectOneValue = $('<option value="' + Validation.selectOne + '" class="standard-optn"></option>');
        for (var idx = 0; idx < currentInstance.DisallowedMessage.length; idx++) {
            options[idx] = $('<option value="' + currentInstance.DisallowedMessage[idx][currentInstance.customrule.elementName.disAllowedMessage] + '" class="standard-optn">' + currentInstance.DisallowedMessage[idx][currentInstance.customrule.elementName.disAllowedMessage] + '</option>');
        }
        options[options.length] = selectOneValue;
        var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.disallowedMessage);
        $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').empty().append(options);

        if (isrowEitable == "Yes" || currentInstance.isGridDivClick == true) {
            $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(rowdata[0][currentInstance.customrule.elementName.disallowedMessage]);
        }
        else if (currentInstance.DisallowedMessageValue != null && currentInstance.DisallowedMessageValue != "[Select One]") {
            $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(currentInstance.DisallowedMessageValue);
        }
    }
}

customRulePQ.prototype.getSERLAndDisallowedMessages = function (currentInstance) {
    var customRuleInstance = this;
    var data = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.selectedRowId;
    });


    var postData = {
        tenantId: currentInstance.formInstanceBuilder.tenantId,
        formInstanceId: currentInstance.formInstanceBuilder.formInstanceId,
        formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
        folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
        formDesignId: currentInstance.formInstanceBuilder.formDesignId,
        fullName: data[0].SESEID
    }

    var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getMessageDropDownItem, postData, false);

    promise.done(function (result) {
        var messages = JSON.parse(result);
        var SERL = messages.SERL;
        var disallowed = messages.Disallowed;
        currentInstance.SERLMessages = [];
        currentInstance.SERLMessages = SERL;
        currentInstance.DisallowedMessage = [];
        currentInstance.DisallowedMessage = disallowed;
    });
}

customRulePQ.prototype.enabledDisabledMessageDropdownOnCoveredChanges = function (repeaterName, value, rowId, currentInstance) {
    var customRuleInstance = this;
    if (value == false) {
        //var iCol = getColumnSrcIndexByName($(repeaterName + formInstanceId), customRuleInstance.elementName.messageSERL);
        //$("tr#" + rowId, repeaterName + formInstanceId).find('td').eq(iCol).attr('disabled', 'disabled');
        var iCol = getColumnSrcIndexByNamePQ($(repeaterName + formInstanceId), customRuleInstance.elementName.disallowedMessage);
        $("tr#" + rowId, repeaterName + formInstanceId).find('td').eq(iCol).removeAttr('disabled')
        $("#" + rowId + "_" + customRuleInstance.elementName.disallowedMessage).removeAttr('disabled');
        //var iCol = getColumnSrcIndexByName($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.messageSERL);
        //$("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(0);        
    }
    else {
        //var iCol = getColumnSrcIndexByNamePQ($(repeaterName + formInstanceId), customRuleInstance.elementName.messageSERL);
        //$("tr#" + rowId, repeaterName + formInstanceId).find('td').eq(iCol).removeAttr('disabled');
        //$("#" + rowId + "_" + customRuleInstance.elementName.messageSERL).removeAttr('disabled');
        var iCol = getColumnSrcIndexByNamePQ($(repeaterName + formInstanceId), customRuleInstance.elementName.disallowedMessage);
        $("tr#" + rowId, repeaterName + formInstanceId).find('td').eq(iCol).attr('disabled', 'disabled');
        $("tr#" + rowId, repeaterName + formInstanceId).find('td').eq(iCol).find('select').attr('disabled', 'disabled');
        var iCol = getColumnSrcIndexByName($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.disallowedMessage);
        $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(0);
    }

}

customRulePQ.prototype.getDeductibleDescription = function (repeaterName, value, rowId, currentInstance) {
    var customRuleInstance = this;
    if (value != null) {
        var customRuleInstance = this;
        var data = currentInstance.data.filter(function (dt) {
            return dt.RowIDProperty == currentInstance.selectedRowId;
        });

        var postData = {
            tenantId: currentInstance.formInstanceBuilder.tenantId,
            formInstanceId: currentInstance.formInstanceBuilder.formInstanceId,
            formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
            folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
            formDesignId: currentInstance.formInstanceBuilder.formDesignId,
            val: value
        }

        var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getMasterListDeductibleAccumDescription, postData, false);

        promise.done(function (result) {
            var messages = JSON.parse(result);
            var data = currentInstance.data.filter(function (dt) {
                return dt.RowIDProperty == rowId;
            });

            data[0][customRuleInstance.elementName.deductibleAccumDescription] = messages;
            var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), customRuleInstance.elementName.deductibleAccumDescription);
            $(currentInstance.gridElementIdJQ).jqGrid('setCell', rowId, customRuleInstance.elementName.deductibleAccumDescription, messages);
        });
    }
}


customRulePQ.prototype.repeaterPropertyChangeMethod = function (currentInstance, element, rowId, value, elementValue) {
    var customRuleInstance = this;
    var currentInstance = currentInstance;
    var fullName = currentInstance.fullName;
    var formInstanceId = currentInstance.formInstanceId
    var data = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.selectedRowId;
    });

    switch (element.FullName) {
        case this.fullName.additionalServicesDetailsCovered:
            this.enabledDisabledMessageDropdownOnCoveredChanges(customRuleInstance.elemenIDs.additionalServicesDetailsRepeater, value, rowId, currentInstance);
            break;
        case this.fullName.serviceGroupCovered:
            this.enabledDisabledMessageDropdownOnCoveredChanges(customRuleInstance.elemenIDs.serviceGroupingMandateDetailsRepeater, value, rowId, currentInstance);
            break;
        case this.fullName.benefitReviewGridAltRulesDataCovered:
            break;
        case this.fullName.altRuleAdditionalServicesDetailsCovered:
            this.enabledDisabledMessageDropdownOnCoveredChanges(customRuleInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater, value, rowId, currentInstance);
            break;
        case this.fullName.altRuleServiceGroupDetailCovered:
            this.enabledDisabledMessageDropdownOnCoveredChanges(customRuleInstance.elemenIDs.altRuleServiceGroupDetailRepeater, value, rowId, currentInstance);
            break;
        case this.fullName.productDeductibleAccumNo:
            this.getDeductibleDescription(currentInstance.gridElementIdJQ, elementValue, rowId, currentInstance);
            break;
        case this.fullName.pdbcTypeFullName: //DropDown-871
            //case "temporarypdbc":
            if (elementValue != 'BSBS' && elementValue != 'EBCL') {
                currentInstance.customrule.getPdbcPrefixes(currentInstance, elementValue);
                currentInstance.customrule.bindPdbcPrefixes(currentInstance, "Yes");
            }
            break;
        case this.fullName.limitRuleLTLTRelations:
            currentInstance.customrule.SetSubSectionValue(currentInstance, elementValue);
            break;

    }
}

customRulePQ.prototype.enabledDisabledMessageDropdownOnRepeaterLoad = function (repeaterName) {
    var customRuleInstance = this;
    var iCoveredCol = getColumnSrcIndexByNamePQ($(repeaterName + formInstanceId), customRuleInstance.elementName.covered);

    var data = $(repeaterName + formInstanceId).pqGrid("option", "dataModel.data");
    $.each(data, function (d) {
        if (data[d]['DisallowedMessage'] == "" || data[d]['DisallowedMessage'] == " ") {
            $("tr#" + data[d]["RowIDProperty"], repeaterName + formInstanceId).find('td').eq(iCoveredCol).find('span').removeClass('ui-icon-close').addClass('ui-icon-check');
        }

        else {
            $("tr#" + data[d]["RowIDProperty"], repeaterName + formInstanceId).find('td').eq(iCoveredCol).find('span').removeClass('ui-icon-check').addClass('ui-icon-close');
        }

        if (data[d]["Covered"] == "No" || data[d]["Covered"] == "") {
            var iCol = getColumnSrcIndexByNamePQ($(repeaterName + formInstanceId), customRuleInstance.elementName.disallowedMessage);
            $("tr#" + data[d]["RowIDProperty"], repeaterName + formInstanceId).find('td').eq(iCol).attr('disabled', false);
        }
        else {
            var iCol = getColumnSrcIndexByNamePQ($(repeaterName + formInstanceId), customRuleInstance.elementName.disallowedMessage);
            $("tr#" + data[d]["RowIDProperty"], repeaterName + formInstanceId).find('td').eq(iCol).attr('disabled', true);
        }
    });

}

customRulePQ.prototype.repeaterOnLoadPropertyChangesMethod = function (currentInstance) {
    var customRuleInstance = this;
    var currentInstance = currentInstance;
    var fullName = currentInstance.fullName;
    var formInstanceId = currentInstance.formInstanceId;
    switch (fullName) {
        case this.fullName.additionalServicesDetailsRepeater:
            this.enabledDisabledMessageDropdownOnRepeaterLoad(customRuleInstance.elemenIDs.additionalServicesDetailsRepeater);
            break;
        case this.fullName.mandateServiceGroupingDetailsRepeater:
            this.enabledDisabledMessageDropdownOnRepeaterLoad(customRuleInstance.elemenIDs.serviceGroupingMandateDetailsRepeater);
            break;
        case this.fullName.altRuleServiceGroupDetailRepeater:
            this.enabledDisabledMessageDropdownOnRepeaterLoad(customRuleInstance.elemenIDs.altRuleServiceGroupDetailRepeater);
            break;
        case this.fullName.altRuleAdditionalServicesDetailsRepeater:
            this.enabledDisabledMessageDropdownOnRepeaterLoad(customRuleInstance.elemenIDs.altRuleAdditionalServicesDetailsRepeater);
            break;
        case this.fullName.benefitReviewGridAltRulesData:
            break;
        case this.fullName.benefitSummaryAccumulationDataforDeductiblesandLimits:
            var repeaterName = customRuleInstance.elemenIDs.benefitSummaryAccumulationDataforDeductiblesandLimitsRepeater;
            
            var data = $(repeaterName + formInstanceId).pqGrid("option", "dataModel.data");
            $.each(data, function (d) {
                if (data[d]["TypeofAccumulator"] == "L-Limits") {
                    var iCol = getColumnSrcIndexByNamePQ($(repeaterName + formInstanceId), customRuleInstance.elementName.typeofAccumulator);
                    $("tr#" + data[d]["RowIDProperty"], repeaterName + formInstanceId).find('td').eq(iCol).attr('disabled', true);
                }
            });

            break;

        case this.fullName.limitRulesLTLT:
            var repeaterName = customRuleInstance.elemenIDs.limitLTLTRepeater;
            var data = $(repeaterName + formInstanceId).pqGrid("option", "dataModel.data");
            $.each(data, function (d) {
                if (data[d]["Relations"] == "A-Include All") {
                    data[d]["Subsection"] = "";
                    $(currentInstance.gridElementIdJQ).pqGrid("refreshRow", { rowIndx: data[d]["RowIDProperty"] });
                }
            });

            break;

    }
}

customRulePQ.prototype.repeaterOnRowSelectCustomRulesMethod = function (currentInstance, isRowEditable) {
    var customRuleInstance = this;
    var currentInstance = currentInstance;
    var fullName = currentInstance.fullName;
    var formInstanceId = currentInstance.formInstanceId;
    switch (fullName) {
        case this.fullName.benefitSummaryAccumulationDataforDeductiblesandLimits:
            if (currentInstance.formInstanceBuilder.folderData.isEditable == "True") {
                var dropdownID = "#" + currentInstance.selectedRowId + "_TypeofAccumulator";
                var value = $(dropdownID).val();
                if (value == "D-Deductible With Carryover" || value == "C-Deductible With Carryover" || value == "[Select One]") {
                    $(dropdownID + " option[value='L-Limits']").remove();
                }
            }
            break;
        case this.fullName.benefitSummaryTable:
            customRuleInstance.filterBenefitSummaryTextGrid(currentInstance);
            break;
        case this.fullName.benefitReviewGridAltRulesData:
        case this.fullName.benefitReviewGridTierData:
        case this.fullName.benefitReviewAltRulesGridTierData:
            if (currentInstance.lastSelectedRow != null && currentInstance.selectedRowId != null) {
                var data = currentInstance.data.filter(function (dt) {
                    return dt.RowIDProperty == currentInstance.selectedRowId;
                });
                var item = customRuleInstance.getDeductibleAccumulatorForBenefitAltAndTierGrid(currentInstance.formInstanceBuilder, data[0][customRuleInstance.BenefitSetName]);
                var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.deductibleAccumulator);
                $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').empty().append(item);
                if (currentInstance.isGridDivClick == true || currentInstance.isViewRowDataClick == true || (currentInstance.selectedRowId != currentInstance.lastSelectedRow)) {
                    $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(data[0][currentInstance.customrule.elementName.deductibleAccumulator]);
                }
                else {
                    $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(currentInstance.DeductibleAccumulatorValue);
                }
            }
            else {
                if (currentInstance.lastSelectedRow != null) {
                    var data = currentInstance.data.filter(function (dt) {
                        return dt.RowIDProperty == currentInstance.lastSelectedRow;
                    });
                    var item = customRuleInstance.getDeductibleAccumulatorForBenefitAltAndTierGrid(currentInstance.formInstanceBuilder, data[0][customRuleInstance.BenefitSetName]);
                    var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.deductibleAccumulator);
                    $("tr#" + currentInstance.lastSelectedRow, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').empty().append(item);
                    $("tr#" + currentInstance.lastSelectedRow, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(data[0][currentInstance.customrule.elementName.deductibleAccumulator]);
                }
            }
            break;
        case this.fullName.limitsLimitsList:
            customRuleInstance.filterFacetLimitsChildGrid(currentInstance);
            break;

        case this.fullName.facetProductComponentsPDBCRepeater:
            // case "temporarypdbc":
            if (currentInstance.lastSelectedRow != null && currentInstance.selectedRowId != null) {

                $("#" + currentInstance.selectedRowId + "_" + customRuleInstance.elementName.pdbcType).find("option[value='EBCL']").attr('disabled', 'disabled');
                $("#" + currentInstance.selectedRowId + "_" + customRuleInstance.elementName.pdbcType).find("option[value='BSBS']").attr('disabled', 'disabled');

                if (currentInstance.selectedRowId != currentInstance.lastSelectedRow) {
                    var data = currentInstance.data.filter(function (ct) {
                        return ct.RowIDProperty == currentInstance.selectedRowId;
                    })

                    var pdbcTypeOfSelectedRow = data[0][currentInstance.customrule.elementName.pdbcType];
                    var pdbcPrefixOfSelectedRow = data[0][currentInstance.customrule.elementName.pdbcPrefix];

                    if (pdbcTypeOfSelectedRow == 'BSBS' || pdbcTypeOfSelectedRow == 'EBCL') {
                        setPDBCPrefixtoDefault(pdbcPrefixOfSelectedRow, currentInstance);
                    }

                    else {
                        currentInstance.customrule.getPdbcPrefixes(currentInstance, pdbcTypeOfSelectedRow);
                        currentInstance.customrule.bindPdbcPrefixes(currentInstance, "Yes");
                    }
                }
                else {
                    if (currentInstance.selectedRowId != null) {
                        var data = currentInstance.data.filter(function (ct) {
                            return ct.RowIDProperty == currentInstance.selectedRowId;
                        })

                        var pdbcTypeOfSelectedRow = data[0][currentInstance.customrule.elementName.pdbcType];
                        var pdbcPrefixOfSelectedRow = data[0][currentInstance.customrule.elementName.pdbcPrefix];

                        if (currentInstance.selectedRowId == 0) {
                            currentInstance.customrule.getPdbcPrefixes(currentInstance, pdbcTypeOfSelectedRow);
                            currentInstance.customrule.bindPdbcPrefixes(currentInstance, "Yes");
                        }
                        else {

                            if (pdbcTypeOfSelectedRow == 'BSBS' || pdbcTypeOfSelectedRow == 'EBCL') {
                                setPDBCPrefixtoDefault(pdbcPrefixOfSelectedRow, currentInstance);
                            }
                            else {
                                currentInstance.customrule.bindPdbcPrefixes(currentInstance, "No");
                            }
                        }

                    }
                }
            }
            break;
    }
}

customRulePQ.prototype.sectionElementPropertyChangeMethod = function (currentInstance, elementPath, value) {
    var customRuleInstance = this;
    var currentInstance = currentInstance;
    var value = value;

    switch (elementPath) {
        case this.fullName.auditCheckListProductPoints:
        case this.fullName.auditCheckListRXPoints:
        case this.fullName.auditCheckListDentalPoints:
        case this.fullName.auditCheckListVisionPoints:
        case this.fullName.auditCheckListStoplossPoints:
        case this.fullName.auditCheckListDEDEPoints:
        case this.fullName.auditCheckListLTLTPoints:
        case this.fullName.auditCheckListEBCLPoints:
        case this.fullName.auditCheckListSEPY1Points:
        case this.fullName.auditCheckListSEPY2Points:
        case this.fullName.auditCheckListSEPY3Points:
        case this.fullName.auditCheckListSEPY4Points:
        case this.fullName.auditCheckListSEPY5Points:
        case this.fullName.auditCheckListSEPY6Points:
        case this.fullName.auditCheckListBSBSBSDLPoints:
        case this.fullName.auditCheckListHRAAllocationRulesPoints:
        case this.fullName.auditCheckListHRAAdminInfoPointsPoints:
        case this.fullName.auditCheckListMARISPoints:
            customRuleInstance.updateAuditTotalPointsAndAuditScore(currentInstance);
            break;
        case this.fullName.productRulesFacetsProductInformationNewProductIDGenerateNewProductID:
            customRuleInstance.applyGenerateNewProductIDChanges(currentInstance);
            break;
        case this.fullName.productContainsShadow:
            customRuleInstance.loadShadowPrefixDropdown(currentInstance, value);
            break;
            // case this.fullName.productRulesFacetsProductInformationProductLineofBusiness:
        case this.fullName.productRulesFacetsProductInformationProductCategory:
            customRuleInstance.showProductCategoryPopUp(currentInstance, elementPath);
            break;
        case this.fullName.productRulesFacetsProductInformationProductLineofBusiness:
            customRuleInstance.showProductCategoryPopUp(currentInstance, elementPath);
            customRuleInstance.populateProductCategoryDropDown(currentInstance);
            break;
        case this.fullName.premiumIndicator:
            customRuleInstance.checkPDBCTypeExist(currentInstance, elementPath, value);
            break;
        case this.fullName.periodIndicator:
            currentInstance.sections[currentInstance.elementIDs.deductiblesSection].IsLoaded = false;
            currentInstance.sections[currentInstance.elementIDs.limitSection].IsLoaded = false;
            break;
    }
}


customRulePQ.prototype.checkPDBCTypeExist = function (currentInstance, elementPath, value) {
    var customRuleInstance = this;
    var premiumIndicator = currentInstance.formData[customRuleInstance.sectionName.productDefinition][customRuleInstance.sectionName.facetsProductInformation][customRuleInstance.elementName.premiumIndicator];
    if (premiumIndicator != "" || premiumIndicator != undefined) {
        var prefix = premiumIndicator.split('-')[0].trim();
        var pdbcPrefixList = currentInstance.formData[customRuleInstance.sectionName.productDefinition][customRuleInstance.sectionName.facetsProductInformation][customRuleInstance.sectionName.facetsProductComponentsPDBC][customRuleInstance.elementName.pdbcPrefixList];
        var pdafType = "";
        if (pdbcPrefixList != "") {
            pdafType = $(pdbcPrefixList).filter(function (index, obj) {
                return obj.PDBCType == "PDAF";
            });
        }
        var repeaterID = customRuleInstance.elemenIDs.facetComponentsPDBCRepeater + currentInstance.formInstanceId;
        var facetComponentPDBCRepeater = $(currentInstance.repeaterBuilders).filter(function (index, obj) {
            return obj.gridElementIdJQ == repeaterID;
        });
        if (prefix && (prefix == 'P' || prefix == 'A')) {
            if (pdafType.length == 0) {
                var rowIDs = new Array();
                $(pdbcPrefixList).each(function (index, obj) {
                    rowIDs.push(obj.RowIDProperty);
                });
                var row = {
                    CreateNewPrefix: "",
                    IsPrefixNew: "",
                    PDBCPrefix: "0001",
                    PDBCType: "PDAF",
                    RowIDProperty: Math.max.apply(Math, rowIDs) + 1
                }
                facetComponentPDBCRepeater[0].data.push(row);
                $(repeaterID).jqGrid('setGridParam', { data: facetComponentPDBCRepeater[0].data }).trigger("reloadGrid");
            }
        }
        else if (prefix && prefix !== "[Select One]") {
            if (pdafType.length > 0) {
                $(pdafType).each(function (index, obj) {
                    $(repeaterID).delRowData(obj.RowIDProperty);
                    facetComponentPDBCRepeater[0].data.splice(facetComponentPDBCRepeater[0].data.indexOf(obj), 1);
                });
            }
        }
    }
}

customRulePQ.prototype.updateAuditTotalPointsAndAuditScore = function (currentInstance) {
    var customRuleInstance = this;
    var poductPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListProductPoints]);

    var rXPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListRXPoints]);
    var dentalPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListDentalPoints]);
    var visionPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListVisionPoints]);

    var stoplossPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListStoplossPoints]);
    var dEDEPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListDEDEPoints]);
    var ltltPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListLTLTPoints]);

    var eBCLPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListEBCLPoints]);
    var sEPY1Points = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListSEPY1Points]);
    var sEPY2Points = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListSEPY2Points]);
    var sEPY3Points = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListSEPY3Points]);
    var sEPY4Points = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListSEPY4Points]);
    var sEPY5Points = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListSEPY5Points]);
    var sEPY6Points = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListSEPY6Points]);

    var bSBSBSDLPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListBSBSBSDLPoints]);
    var hRAAdminInfoPointsPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListHRAAdminInfoPointsPoints]);
    var hRAAllocationRulesPoints = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListHRAAllocationRulesPoints]);
    var mARISPointsFullPath = parseInt(currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListMARISPoints]);

    var total = poductPoints + rXPoints + dentalPoints + visionPoints + stoplossPoints + dEDEPoints + ltltPoints + eBCLPoints + sEPY1Points + sEPY2Points + sEPY3Points
                  + sEPY4Points + sEPY5Points + sEPY6Points + bSBSBSDLPoints + hRAAdminInfoPointsPoints + hRAAllocationRulesPoints + mARISPointsFullPath;

    currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListTotalPoint] = total;

    var auditScore = parseFloat(parseInt(total) / 146).toFixed(2);
    currentInstance.formData[customRuleInstance.sectionName.auditCheckListGenrated][customRuleInstance.sectionName.checkListDetailsGenrated][customRuleInstance.elementName.auditCheckListAuditScore] = auditScore;

    $(customRuleInstance.elemenIDs.auditTotalPoint + currentInstance.formInstanceId).val(total);
    $(customRuleInstance.elemenIDs.auditScore + currentInstance.formInstanceId).val(auditScore);
}

customRulePQ.prototype.isSectionDisable = function (currentInstance, role) {
    var customRuleInstance = this;
    switch (currentInstance.selectedSectionName) {
        case customRuleInstance.sectionName.auditChecklist:
            if (customRuleInstance.isAuditWorkFlow(currentInstance.folderData.WFStateName) && (role == Role.Audit || role == Role.Superuser || role == Role.TMGSuperUser || role == Role.ProductAudit)) {
                return true;
            }
            else {
                return false;
            }
            break;
        case customRuleInstance.sectionName.ServiceGroupMandatesSectionName:
        case customRuleInstance.sectionName.AdditionalServicesSectionName:
        case customRuleInstance.sectionName.deductibles:
        case customRuleInstance.sectionName.benefitReview:
        case customRuleInstance.sectionName.LimitsSectionName:
        case customRuleInstance.sectionName.benefitSummary:
        case customRuleInstance.sectionName.benefitSetNetwork:
        case customRuleInstance.sectionName.productDefinitionSectionName:
            //if (currentInstance.formData[customRuleInstance.sectionName.customRulesSettings][customRuleInstance.elementName.isBenefitSetGridLock] != "Yes") {
            //    return false;
            //}
            //else {
            //    return true;
            //}


            if ((customRuleInstance.isAuditWorkFlow(currentInstance.folderData.WFStateName))
                || (customRuleInstance.isFacetProdWorkFlow(currentInstance.folderData.WFStateName))) {
                return false;
            } else {
                return true;
            }
            break;
    }
}

customRulePQ.prototype.updateAuditGenralInfo = function (folderData, workflowstate, approvalstatus, folderInstance, updateWorkflowData) {
    var formInstanceAarray = [];
    for (var formInstance in folderInstance.formInstances) {
        var form = folderInstance.formInstances[formInstance];
        formInstanceAarray.push(form.FormInstance);
    }

    var postData = {
        tenantId: folderData.tenantId,
        formInstances: JSON.stringify(formInstanceAarray),
        workflowstate: workflowstate,
        approved: approvalstatus,
    }

    var promise = ajaxWrapper.postJSONCustom(folderInstance.customRules.URLs.updateAuditGeneralInfo, postData);

    promise.done(function (result) {
        folderStatus.postWorkFlowData(updateWorkflowData);
    });
}

customRulePQ.prototype.applyGenerateNewProductIDChanges = function (currentInstance) {
    var generateNewProductIdJQ = $(this.elemenIDs.generateNewProductIdJQ + currentInstance.formInstanceId);
    var ProductIdJQ = this.elemenIDs.ProductIdJQ + currentInstance.formInstanceId;
    var isProductNewJQ = this.elemenIDs.isProductNewJQ + currentInstance.formInstanceId;
    if (currentInstance.formDesignId == FormTypes.PRODUCTFORMDESIGNID) {
        if (generateNewProductIdJQ) {
            if ($(generateNewProductIdJQ).is(':checked')) {
                yesNoConfirmDialog.show(DocumentDesign.AreYouSureYouWantToGenerateANewProductID, function (e) {
                    yesNoConfirmDialog.hide();
                    if (e) {
                        $(isProductNewJQ).prop("checked", true).trigger('change');
                        currentInstance.isPeodutIdConfirmClickedNo = false;
                    }
                    else {
                        currentInstance.isPeodutIdConfirmClickedNo = true;
                        $(generateNewProductIdJQ).prop("checked", false).trigger('change');
                    }
                })
            }
            else if (!currentInstance.isPeodutIdConfirmClickedNo) {
                $(isProductNewJQ).prop("checked", false).trigger('change');
            }
        }
    }
}

customRulePQ.prototype.showAltRuleTierDataPopUp = function (rowId, isCurrentRowReadOnly, benefitRowData, currentInstance, dataSourceName, networkindex) {
    try {
        var elementIDs = {
            altRuleTierDataGrid: "altRuleTierDataGrid",
            altRuleTierDataGridJQ: "#altRuleTierDataGrid",
            altRuleTierDataDialogJQ: "#altRuleTierDataDialog",
            spanAltRuleTierDataBenefitCategory1JQ: "#spanAltRuleTierDataBenefitCategory1",
            spanAltRuleTierDataBenefitCategory2JQ: "#spanAltRuleTierDataBenefitCategory2",
            spanAltRuleTierDataBenefitCategory3JQ: "#spanAltRuleTierDataBenefitCategory3",
            spanAltRuleTierDataPlaceofServiceJQ: "#spanAltRuleTierDataPlaceofService",
            spanAltRuleTierDataSESEIDJQ: "#spanAltRuleTierDataSESEID",
            hiddenTierDataIndexJQ: "#hiddenTierDataIndexJQ",
            spanAltRuleTierBenefitSetNameJQ: "#spanAltRuleTierBenefitSetName",
        };

        var customRuleInstance = this;
        var formInstancebuilder = currentInstance.formInstanceBuilder;

        $(elementIDs.spanAltRuleTierDataBenefitCategory1JQ).text(benefitRowData.BenefitCategory1);
        $(elementIDs.spanAltRuleTierDataBenefitCategory2JQ).text(benefitRowData.BenefitCategory2);
        $(elementIDs.spanAltRuleTierDataBenefitCategory3JQ).text(benefitRowData.BenefitCategory3);
        $(elementIDs.spanAltRuleTierDataPlaceofServiceJQ).text(benefitRowData.PlaceofService);
        $(elementIDs.spanAltRuleTierDataSESEIDJQ).text(benefitRowData.SESEID);
        $(elementIDs.spanAltRuleTierBenefitSetNameJQ).text(benefitRowData[customRuleInstance.BenefitSetName]);
        // $(elementIDs.hiddenTierDataIndexJQ).val(index);

        //if (!$(elementIDs.tierDataDialogJQ).hasClass('ui-dialog-content')) {
        $(elementIDs.altRuleTierDataDialogJQ).dialog({
            modal: true,
            autoOpen: false,
            draggable: true,
            resizable: true,
            zIndex: 1005,
            width: 700,
            closeOnEscape: false,
            title: 'AltRule TierData Details',
            buttons: [{
                id: "TierDialogSave",
                text: "Save",
                click: function () {
                    var rowId = $(elementIDs.altRuleTierDataGridJQ).jqGrid('getGridParam', 'selrow');
                    $(elementIDs.altRuleTierDataGridJQ).saveRow(rowId);
                    var rowData = $(elementIDs.altRuleTierDataGridJQ).jqGrid('getGridParam', 'data');

                    //  var tierDataIndex = $(elementIDs.hiddenTierDataIndexJQ).val();
                    if (rowData)
                        customRuleInstance.saveAltRuleBenefitTierData(benefitRowData, formInstancebuilder, dataSourceName, networkindex, rowData, currentInstance);
                    $(this).dialog("close");
                }
            }, {
                id: "TierDialogClose",
                text: "Close",
                click: function () {
                    // $("#tierDataGrid").jqGrid("GridUnload");
                    $(this).dialog("close");
                    //tierDataDetailsArray
                }
            }],
            open: function (event, ui) {
            }
        });
        //}

        this.loadAltRuleTierDataGrid(isCurrentRowReadOnly, benefitRowData, formInstancebuilder, dataSourceName, networkindex);
        var title = "AltRule TierData - " + benefitRowData.BenefitCategory1;
        $(elementIDs.altRuleTierDataDialogJQ).dialog('option', 'title', title);
        $(elementIDs.altRuleTierDataDialogJQ).dialog("open");
        if (isCurrentRowReadOnly) {
            var tierGridDialog = $(elementIDs.altRuleTierDataDialogJQ);
            tierGridDialog.find('#TierDataAdd, #TierDataDelete').addClass('ui-state-disabled');
            $("#TierDialogSave").prop("disabled", true).addClass("ui-state-disabled");
            tierGridDialog.click(function () {
                $(this).find("table.ui-jqgrid-btable").find('input, select, img.ui-datepicker-trigger').prop('disabled', 'disabled');
                $(this).find("table.ui-jqgrid-btable").find(".jqgrow").attr('disabled', 'disabled');
                $(this).find("table.ui-jqgrid-btable").find("img.ui-datepicker-trigger").prop('disabled', 'disabled');
            });
        }

    } catch (e) {
        console.log("error occurred in getTierData - " + e);
    }
}

customRulePQ.prototype.loadAltRuleTierDataGrid = function (isCurrentRowReadOnly, benefitRowData, formInstancebuilder, dataSourceName, networkindex) {
    var customRuleInstance = this;
    var elementIDs = {
        altRuleTierDataGrid: "altRuleTierDataGrid",
        altRuleTierDataGridJQ: "#altRuleTierDataGrid",
        altRuleTierDataDialogJQ: "#altRuleTierDataDialog"
    };

    customRuleInstance = this;
    benefitReviewGridRowData = benefitRowData;

    formInstancebuilder.designData.Sections.filter(function (ct) {
        if (ct.GeneratedName === customRuleInstance.sectionName.benefitReviewGridGenrated) {
            ct.Elements.filter(function (ele) {
                if (ele.GeneratedName === customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated) {
                    if (ele.Repeater.GeneratedName === customRuleInstance.sectionName.benefitReviewGridAltRulesDataGenrated) {
                        benefitReviewTierData = ele.Repeater;
                        return false;
                    }
                }
            });
        }
    });

    tierDataDetailsArray = this.getAltRuleTierData(benefitRowData, formInstancebuilder, dataSourceName, networkindex);

    //if (!$(elementIDs.tierDataGridJQ).jqGrid('getGridParam')) {
    $(elementIDs.altRuleTierDataGridJQ).jqGrid("GridUnload");
    //code to generate Benefit review grid
    var lastsel2;
    $(elementIDs.altRuleTierDataGridJQ).jqGrid({
        datatype: "local",
        editurl: 'clientArray',
        cache: false,
        autowidth: true,
        width: 700,
        rowheader: true,
        loadonce: false,
        rowNum: 100000,
        scrollrows: true,
        altRows: true,
        altclass: 'alternate',
        hidegrid: false,
        pager: '#p' + elementIDs.altRuleTierDataGrid,
        colNames: ['RowIDProperty', 'BenefitSetName', 'TierNo', 'SESEID', 'BenefitCategory1', 'BenefitCategory2', 'BenefitCategory3', 'PlaceofService', 'Copay', 'Coinsurance', 'Allowed Amount', 'Allowed Counter', 'Deductible Accumulator', 'Excess Per Counter Indicator'],
        colModel: [
          { name: 'RowIDProperty', index: 'RowIDProperty', width: 100, sorttype: "int", editable: false, hidden: true, sortable: false },
        { name: 'BenefitSetName', index: 'BenefitSetName', width: 100, fixed: true, sortable: false, editable: false, hidden: true, formatter: "text", edittype: "text" },
          { name: 'TierNo', index: 'TierNo', width: 100, fixed: true, editable: false, sortable: false, formatter: "text", edittype: "text" },
          { name: 'SESEID', index: 'SESEID', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false, formatter: "text", edittype: "text" },
          { name: 'BenefitCategory1', index: 'BenefitCategory1', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'BenefitCategory2', index: 'BenefitCategory2', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'BenefitCategory3', index: 'BenefitCategory3', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          { name: 'PlaceofService', index: 'PlaceofService', width: 100, fixed: true, sorttype: "int", editable: false, hidden: true, sortable: false },
          {
              name: 'Copay', index: 'Copay', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                  value: this.getValues("Copay", benefitReviewTierData)
              },
          },
          {
              name: 'Coinsurance', index: 'Coinsurance', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                  value: this.getValues("Coinsurance", benefitReviewTierData)
              },
          },
        {
            name: 'AllowedAmount', index: 'AllowedAmount', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("AllowedAmount", benefitReviewTierData)
            },
        },
        {
            name: 'AllowedCounter', index: 'AllowedCounter', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("AllowedCounter", benefitReviewTierData)
            },
        },
        {
            name: 'DeductibleAccumulator', index: 'DeductibleAccumulator', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getDeductibleAccumNumberForBenefitReviewGridPopup(formInstancebuilder, benefitRowData[customRuleInstance.BenefitSetName])
            },
        },
        {
            name: 'ExcessPerCounterIndicator', index: 'ExcessPerCounterIndicator', width: 100, fixed: true, sortable: false, editable: true, formatter: "select", edittype: "select", editoptions: {
                value: this.getValues("ExcessPerCounterIndicator", benefitReviewTierData)
            },
        }
        ],
        onSelectRow: function (id) {
            if (id && id !== lastsel2) {
                $(elementIDs.altRuleTierDataGridJQ).saveRow(lastsel2);
                $(elementIDs.altRuleTierDataGridJQ).restoreRow(lastsel2);
                $(elementIDs.altRuleTierDataGridJQ).editRow(id, true);
                lastsel2 = id;
            }
        },
        caption: "Alt Rule Tiers",
        gridComplete: function () {
            if (isCurrentRowReadOnly) {
                var currentPHQFormUtilities = new formUtilities(formInstancebuilder.formInstanceId);
                currentPHQFormUtilities.sectionManipulation.disableRepeater(elementIDs.altRuleTierDataGridJQ);
            }
        }
    });


    var pagerElement = '#p' + elementIDs.altRuleTierDataGrid;
    //remove default buttons
    $(elementIDs.altRuleTierDataGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    //remove paging
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();

    $(elementIDs.altRuleTierDataGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-plus', id: "TierDataAdd",
        onClickButton: function () {
            var rowID = tierDataDetailsArray.length + 1;
            var tierNoID = tierDataDetailsArray.length + 1;

            var rowToAdd = {
                RowIDProperty: rowID,
                BenefitSetName: benefitReviewGridRowData[customRuleInstance.BenefitSetName],
                TierNo: tierNoID,
                SESEID: benefitReviewGridRowData.SESEID,
                BenefitCategory1: benefitReviewGridRowData.BenefitCategory1,
                BenefitCategory2: benefitReviewGridRowData.BenefitCategory2,
                BenefitCategory3: benefitReviewGridRowData.BenefitCategory3,
                PlaceofService: benefitReviewGridRowData.PlaceofService,
                Copay: "",
                Coinsurance: "",
                AllowedAmount: "",
                AllowedCounter: "",
                DeductibleAccumulator: "",
                ExcessPerCounterIndicator: ""
            };
            tierDataDetailsArray.push(rowToAdd);

            $(elementIDs.altRuleTierDataGridJQ).jqGrid('addRowData', rowID, rowToAdd);

            $(elementIDs.altRuleTierDataGridJQ).jqGrid('setSelection', rowID);
            $(elementIDs.altRuleTierDataGridJQ).editRow(rowID, true);

            //benefitRowGridCount++;
            //tierNoID++;
        }
    });

    $(elementIDs.altRuleTierDataGridJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-trash', id: "TierDataDelete",
        onClickButton: function () {
            var rowId = $(elementIDs.altRuleTierDataGridJQ).getGridParam('selrow');
            if (rowId) {
                $(elementIDs.altRuleTierDataGridJQ).jqGrid('delRowData', rowId);
                $(elementIDs.altRuleTierDataGridJQ).jqGrid('setSelection', rowId - 1);
            }
        }
    });
    //}

    $(elementIDs.altRuleTierDataGridJQ).jqGrid("clearGridData");


    $(elementIDs.altRuleTierDataGridJQ).jqGrid('setGridParam', { data: tierDataDetailsArray }).trigger("reloadGrid");
    $(elementIDs.altRuleTierDataGridJQ).jqGrid('setSelection', 0);
    $(elementIDs.altRuleTierDataGridJQ).editRow(0, true);
}

customRulePQ.prototype.getAltRuleTierData = function (benefitRowData, formInstancebuilder, dataSourceName, networkindex) {
    var customRuleInstance = this;
    var tierData = new Array();
    try {

        var BenefitSetName = benefitRowData[customRuleInstance.BenefitSetName];


        if (formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].length > 0) {
            tierData = formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].filter(function (ct) {
                if (ct.BenefitSetName === BenefitSetName && ct.SESEID === benefitRowData.SESEID &&
                    ct.BenefitCategory1 == benefitRowData.BenefitCategory1 && ct.BenefitCategory2 == benefitRowData.BenefitCategory2
                    && ct.BenefitCategory3 == benefitRowData.BenefitCategory3 && ct.PlaceofService == ct.PlaceofService) {
                    return ct;
                }
            });
        }


        for (var i = 0; i < tierData.length; i++) {
            //tierDataDetailsArray[i].RowIDProperty = i;
            tierData[i].TierNo = i + 1;
            tierData[i].id = i + 1;
        }
    } catch (e) {
        console.log("error occurred in getTierData - " + e);
    }

    return tierData;
}

customRulePQ.prototype.saveAltRuleBenefitTierData = function (benefitRowData, formInstancebuilder, dataSourceName, networkindex, rowData, currentInstance) {
    var newTierData = [];
    var customRuleInstance = this;
    var benefitReviewAltRulesGridTierDataLength = currentInstance.formInstanceBuilder.formData.BenefitReview.BenefitReviewAltRulesGridTierData.length - 1;
    var oldTierData = formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].filter(function (tRows) {
        return benefitRowData.SESEID == tRows.SESEID && benefitRowData.BenefitSetName == tRows.BenefitSetName;
    })
    if (rowData.length > 0) {
        for (var i = 0; i < rowData.length; i++) {
            rowData[i].TierNo = i + 1;
            var isDataExist = oldTierData.filter(function (dt) {
                if (rowData[i].id == dt.id) {
                    var colCount = 0;
                    //repeater existing row change activity log
                    for (var prop in rowData[i]) {
                        if (dt.hasOwnProperty(prop)) {
                            if (dt[prop] != rowData[i][prop] && prop != "TierNo" && prop != "Tier" && prop != "RowIDProperty" && prop != "id") {
                                var oldValue = dt[prop] == undefined ? "" : dt[prop];
                                var newValue = rowData[i][prop];
                                var colName = prop;
                                //colName = currentInstance.columnNames.filter(function (dt) {
                                //    //return dt.replace('<font color=red>*</font>', '').replace(/ /g, '').trim() == colName;
                                //    return dt == colName;
                                //});
                                //colName = colName[0];
                                //Update row activity
                                currentInstance.addEntryToAcitivityLogger(rowData[i].RowIDProperty - 1, Operation.UPDATE, colName, oldValue, newValue, undefined, "Benefit Review Alt Rules Grid Tier Data", prop);
                            }
                        }
                        colCount++;
                    }
                    return dt;
                }
            })
            if (isDataExist.length == 0) {
                //ADD new row activity
                currentInstance.addEntryToAcitivityLogger(benefitReviewAltRulesGridTierDataLength, Operation.ADD, undefined, undefined, undefined, undefined, "Benefit Review Alt Rules Grid Tier Data")
                benefitReviewAltRulesGridTierDataLength++;
            }
        }
    }
    //DELETE row activity
    $.grep(oldTierData, function (el) {
        var isDataExist = rowData.filter(function (dt) {
            if (el.id == dt.id)
                return dt;
        })
        if (isDataExist.length == 0) {
            currentInstance.addEntryToAcitivityLogger(el.RowIDProperty - 1, Operation.DELETE, undefined, undefined, undefined, undefined, "Benefit Review Alt Rules Grid Tier Data")
        }
    });

    var benefitReviewGridTierData = formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated];
    for (var i = 0; i < benefitReviewGridTierData.length; i++) {
        if ((benefitReviewGridTierData[i].SESEID === benefitRowData.SESEID &&
            benefitReviewGridTierData[i].BenefitCategory1 === benefitRowData.BenefitCategory1 &&
            benefitReviewGridTierData[i].BenefitCategory2 === benefitRowData.BenefitCategory2 &&
            benefitReviewGridTierData[i].BenefitCategory3 === benefitRowData.BenefitCategory3 &&
            benefitReviewGridTierData[i].PlaceofService === benefitRowData.PlaceofService &&
            benefitReviewGridTierData[i].BenefitSetName === benefitRowData[customRuleInstance.BenefitSetName])
            || (benefitReviewGridTierData[i].SESEID === "" &&
            benefitReviewGridTierData[i].BenefitCategory1 === "" &&
            benefitReviewGridTierData[i].BenefitCategory2 === "" &&
            benefitReviewGridTierData[i].BenefitCategory3 === "" &&
            benefitReviewGridTierData[i].PlaceofService === "")) {
            formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].splice(i, 1);
            i--;
        }
    }

    if (rowData.length > 0) {
        for (var i = 0; i < rowData.length; i++) {
            rowData[i].TierNo = i + 1;
            formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].push(rowData[i]);
        }
    }

    for (var i = 0; i < formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].length; i++) {
        formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated][i].RowIDProperty = i;
        //if (formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].RowIDProperty != undefined) {
        formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated][i]["id"] = i;
        //}
    }

    //$(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).jqGrid("clearGridData");
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid('refreshView');
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid("option", "dataModel", {
        data: formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated]
    });
    $(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).pqGrid('refreshView');
    //$(formInstancebuilder.customRules.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstancebuilder.formInstanceId).jqGrid('setGridParam',
    //    {
    //        data: formInstancebuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated]
    //    }).trigger("reloadGrid");
}

customRulePQ.prototype.getDeductibleAccumNumberForBenefitReviewGrid = function (currentInstance, elementName) {
    var deductibleList = new Array();
    var customRuleInstance = this;
    var defaultObj = {
        Enabled: null,
        ItemID: 0,
        ItemValue: "0"
    };
    deductibleList.push(defaultObj);
    var benefitSetName = undefined;
    var index = elementName.split('_')[2];
    var benefitSetName = undefined;
    if (currentInstance.data[0] != undefined && currentInstance.data[0][currentInstance.customrule.ProductNetworkDS] != undefined
        && currentInstance.data[0][currentInstance.customrule.ProductNetworkDS][index] != undefined) {
        benefitSetName = currentInstance.data[0][currentInstance.customrule.ProductNetworkDS][index][currentInstance.customrule.BenefitSetName];
    }

    $.each(currentInstance.formInstanceBuilder.formData[customRuleInstance.elementName.deductibles][customRuleInstance.elementName.deductibleList], function (i, v) {
        if (v[customRuleInstance.BenefitSetName].trim() == benefitSetName.trim()) {
            var obj = {
                Enabled: null,
                ItemID: 0,
                ItemValue: v.AccumNumber + '-' + v.Description
            };
            var isObjExist = deductibleList.filter(function (dt) {
                return dt.ItemValue == obj.ItemValue;
            })

            if (isObjExist.length == 0) {
                deductibleList.push(obj);
            }
        }
    });

    return deductibleList;
}

customRulePQ.prototype.setProductId = function (newId, currentInstance) {
    $(this.elemenIDs.newproductId + currentInstance.currentFormInstanceID).val(newId).trigger('change');
    $(this.elemenIDs.generateNewProductIdJQ + currentInstance.currentFormInstanceID).attr('checked', false);
    $(this.elemenIDs.generateNewProductIdJQ + currentInstance.currentFormInstanceID).removeAttr("disabled");
    var customRuleInstance = this;
    currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder.formData[customRuleInstance.sectionName.productDefinition][customRuleInstance.sectionName.facetsProductInformation][customRuleInstance.sectionName.newProductID][customRuleInstance.elementName.productID] = newId;
    currentInstance.formInstances[currentInstance.currentFormInstanceID].FormInstanceBuilder.formData[customRuleInstance.sectionName.productDefinition][customRuleInstance.sectionName.facetsProductInformation][customRuleInstance.sectionName.newProductID][customRuleInstance.elementName.generateNewProductID] = "False";
}

customRulePQ.prototype.getEmptyDropDownItem = function () {
    var items = new Array();
    var defaultObj = {
        Enabled: null,
        ItemID: 0,
        ItemValue: " "
    };
    items.push(defaultObj);
    return items;

}

customRulePQ.prototype.loadShadowPrefixDropdown = function (currentInstance, value, selectedValue) {
    var customRuleInstance = this;
    if (value) {
        $(customRuleInstance.elemenIDs.standardProduct + currentInstance.formInstanceId).children('option:not(:first)').remove();
        for (var formInstance in folderManager.getInstance().getFolderInstance().formInstances) {
            var instanceName = folderManager.getInstance().getFolderInstance().formInstances[formInstance].FormInstance.FormDesignName;
            if (instanceName != currentInstance.formName) {
                $(customRuleInstance.elemenIDs.standardProduct + currentInstance.formInstanceId).append($("<option></option>")
                      .attr("value", instanceName)
                       .text(instanceName));
            }
        }
        if (selectedValue != null || selectedValue != undefined) {
            $(customRuleInstance.elemenIDs.standardProduct + currentInstance.formInstanceId).val(selectedValue);
        }
    }

}

customRulePQ.prototype.showProductCategoryPopUp = function (currentInstance, elementPath) {

    var elementIDs = {
        productCategoryDialogJQ: "#productCategoryDialog",
    };
    var customRuleInstance = this;
    var isCheckedProductId = currentInstance.formData[customRuleInstance.sectionName.productDefinition][customRuleInstance.sectionName.facetsProductInformation][customRuleInstance.sectionName.newProductID][customRuleInstance.elementName.isProductNew];

    if (isCheckedProductId != "True" && !isCheckedProductId) {
        if (!$(elementIDs.productCategoryDialogJQ).hasClass('ui-dialog-content')) {
            $(elementIDs.productCategoryDialogJQ).dialog({
                modal: true,
                autoOpen: false,
                draggable: true,
                resizable: true,
                zIndex: 1005,
                width: 350,
                height: 200,
                closeOnEscape: false,
                title: ' ',
                buttons: {
                    Close: function () {
                        $(this).empty();
                        $(this).dialog("close");
                    }
                }
            });
        }
        var message = "";
        $(elementIDs.productCategoryDialogJQ).empty();
        if (elementPath == this.fullName.productRulesFacetsProductInformationProductLineofBusiness) {
            message = this.KeyName.lobSwitchIndicatorMessage;
        }
        else if (elementPath == this.fullName.productRulesFacetsProductInformationProductCategory) {
            message = this.KeyName.prodCategoryMessage;
        }

        $(elementIDs.productCategoryDialogJQ).append(message);
        var title = "Warning!";
        $(elementIDs.productCategoryDialogJQ).dialog('option', 'title', title);
        $(elementIDs.productCategoryDialogJQ).dialog("open");
    }

}

customRulePQ.prototype.prepareCustomDataForRepeater = function (currentInstance, data) {
    var customData = [];
    var exitingData = currentInstance.data;
    switch (currentInstance.fullName) {
        case currentInstance.customrule.fullName.altRuleServiceGroupDetailRepeater:
        case currentInstance.customrule.fullName.altRuleAdditionalServicesDetailsRepeater:
            var colArray = ['BenefitCategory1', 'BenefitCategory2', 'BenefitCategory3', 'PlaceofService', 'SESEID', 'BenefitSetName',
                'RuleCategory', 'RuleType', 'EligibleforFSAReimbursement', 'EligibleforHRAReimbursement',
                'HRADeductibleApplies', 'Warnwhencopaypercentageexceeds', 'CalculationIndicator', 'Gender', 'GenderExplanationCode', 'AgeFrom',
                'AgeTo', 'AgeExplanationCode', 'CoveredMembers', 'CoveredMembersExplanationCode', 'DisplayCaseManagementWarningMessage', 'UserMessage',
                'PreAuthChargeRequired', 'PreAuthUnitsRequired', 'PreAuthProcedureRequired', 'ServiceRuleOptions', 'Covered', 'MessageSERL',
                'DisallowedMessage', 'SESEMAXCPAYACTNVL', 'SESECPAYEXCDIDNVL', 'Specialty'];

            $.each(data, function (idx, dt) {
                var isdataExist = currentInstance.data.filter(function (ct) {
                    return ct[currentInstance.customrule.KeyName.BenefitCategory1] == dt[currentInstance.customrule.KeyName.BenefitCategory1]
                    && ct[currentInstance.customrule.KeyName.BenefitCategory2] == dt[currentInstance.customrule.KeyName.BenefitCategory2]
                    && ct[currentInstance.customrule.KeyName.BenefitCategory3] == dt[currentInstance.customrule.KeyName.BenefitCategory3]
                    && ct[currentInstance.customrule.KeyName.PlaceofService] == dt[currentInstance.customrule.KeyName.PlaceofService]
                    && ct[currentInstance.customrule.KeyName.SESEID] == dt[currentInstance.customrule.KeyName.SESEID]
                    && ct[currentInstance.customrule.BenefitSetName] == dt[currentInstance.customrule.BenefitSetName];
                });

                if (isdataExist != undefined && isdataExist.length > 0) {
                    var newRow = {};
                    for (var i = 0; i < colArray.length; i++) {
                        newRow[colArray[i]] = isdataExist[0][colArray[i]];
                        newRow["RowIDProperty"] = idx;

                    }
                    customData.push(newRow);
                }
                else {
                    var newRow = {};
                    for (var i = 0; i < colArray.length; i++) {
                        newRow[colArray[i]] = dt[colArray[i]];
                        newRow["RowIDProperty"] = idx;

                    }
                    customData.push(newRow)
                }
            });
            break;
    }
    return customData;
}

customRulePQ.prototype.removeDefualtRowFromRepeater = function (currentInstance) {
    var customRuleInstance = this;
    switch (currentInstance.fullName) {
        case customRuleInstance.fullName.altRuleServiceGroupDetailRepeater:
        case customRuleInstance.fullName.mandateServiceGroupingDetailsRepeater:
        case customRuleInstance.fullName.additionalServicesDetailsRepeater:
        case customRuleInstance.fullName.altRuleAdditionalServicesDetailsRepeater:
        case customRuleInstance.fullName.benefitReviewGridAltRulesData:
        case customRuleInstance.fullName.benefitReviewGridTierData:
        case customRuleInstance.fullName.benefitReviewAltRulesGridTierData:
            if (currentInstance.data.length == 1) {
                if (currentInstance.data[0][customRuleInstance.KeyName.BenefitCategory1] == ""
                    && currentInstance.data[0][customRuleInstance.KeyName.BenefitCategory2] == ""
                    && currentInstance.data[0][customRuleInstance.KeyName.BenefitCategory3] == ""
                    && currentInstance.data[0][customRuleInstance.KeyName.PlaceofService] == ""
                    && currentInstance.data[0][customRuleInstance.KeyName.SESEID] == ""
                    && currentInstance.data[0][customRuleInstance.BenefitSetName] == "") {
                    currentInstance.data = [];
                }
            }
            break;
        case customRuleInstance.fullName.benefitReviewGrid:
        case customRuleInstance.fullName.additionalServicesList:
            if (currentInstance.data.length == 1) {
                if (currentInstance.data[0][customRuleInstance.KeyName.BenefitCategory1] == ""
                    && currentInstance.data[0][customRuleInstance.KeyName.BenefitCategory2] == ""
                    && currentInstance.data[0][customRuleInstance.KeyName.BenefitCategory3] == ""
                    && currentInstance.data[0][customRuleInstance.KeyName.PlaceofService] == ""
                    && currentInstance.data[0][customRuleInstance.KeyName.SESEID] == "") {
                    currentInstance.data = [];
                }
            }
            break;
        case customRuleInstance.fullName.ServiceGrouping:
            if (currentInstance.data.length == 1) {
                if (currentInstance.data[0][customRuleInstance.elementName.serviceGroupHeader] == "") {
                    currentInstance.data = [];
                }
            }
            break;
    }
}


customRulePQ.prototype.lockBenefitSetGrid = function (currentInstance, formInstanceID) {
    var customRuleInstance = this;
    var formInstanceBuilder = currentInstance.formInstances[formInstanceID].FormInstanceBuilder
    yesNoConfirmDialog.show('Are you sure you want to finalize the Network List?', function (e) {
        yesNoConfirmDialog.hide();
        if (e) {
            var benefitSerGrid = formInstanceBuilder.repeaterBuilders.filter(function (dt) {
                return dt.fullName == customRuleInstance.fullName.networkList
            })

            var postData = {
                tenantId: formInstanceBuilder.tenantId,
                formInstanceId: formInstanceBuilder.formInstanceId,
                formDesignVersionId: formInstanceBuilder.formDesignVersionId,
                folderVersionId: formInstanceBuilder.folderVersionId,
                formDesignId: formInstanceBuilder.formDesignId
            }

            var hasError = customRuleInstance.validateBenefitSetGrid(formInstanceBuilder, benefitSerGrid);

            if (hasError == false) {
                formInstanceBuilder.formData[customRuleInstance.sectionName.customRulesSettings][customRuleInstance.elementName.isBenefitSetGridLock] = "Yes";

                var promise = ajaxWrapper.postAsyncJSONCustom(customRuleInstance.URLs.updateBenefitSetGridLockStatus, postData);
                promise.done(function (result) {
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId).find('#btnRepeaterBuilderAdd').addClass('ui-state-disabled');
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId).find('#btnRepeaterBuilderCopy').addClass('ui-state-disabled');
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId).find('#btnRepeaterBuilderDelete').addClass('ui-state-disabled');
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId).find('#btnRepeaterBuilderView').addClass('ui-state-disabled');
                    $(customRuleInstance.elemenIDs.benefitSetNetwork + formInstanceBuilder.formInstanceId).setColProp('BenefitSetName', { editable: false });
                    formInstanceBuilder.form.saveFormInstanceData(true);

                });
            }
            else {
                messageDialog.show("Network List Grid has some validation error");
            }
        }
        else {
            formInstanceBuilder.form.saveFormInstanceData(true);
        }
    })

}

customRulePQ.prototype.populateProductCategoryDropDown = function (currentInstance, selectedValue) {
    var customRuleInstance = this;
    var postData = {
        tenantId: currentInstance.tenantId,
        formInstanceId: currentInstance.formInstanceId,
        formDesignVersionId: currentInstance.formDesignVersionId,
        folderVersionId: currentInstance.folderVersionId,
        formDesignId: currentInstance.formDesignId,
        fullName: currentInstance.formData.ProductDefinition.FacetsProductInformation.ProductLineofBusiness
    }
    var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getProductCategoryDropDownItems, postData);
    promise.done(function (result) {
        var options = [],
        category = JSON.parse(result);
        options[0] = $('<option value="[Select One]"> [Select One]</option>');
        for (var i = 0; i < category.length; i++) {
            options[i + 1] = $('<option value="' + category[i] + '">' + category[i] + '</option>');
        }
        $(customRuleInstance.elemenIDs.productCategory + currentInstance.formInstanceId).empty().append(options);
        if (selectedValue != null || selectedValue != undefined) {
            $(customRuleInstance.elemenIDs.productCategory + currentInstance.formInstanceId).val(selectedValue);
        }
        else {
            // $(customRuleInstance.elemenIDs.productCategory + currentInstance.formInstanceId).val('[Select One]').trigger('change');
            currentInstance.formData.ProductDefinition.FacetsProductInformation.ProductCategory = " ";
        }
    });



}

customRulePQ.prototype.setProductReferenceLink = function (currentInstance) {
    var referenceFormFolderID = $(this.elemenIDs.referenceFormFolderId + currentInstance.formInstanceId).val();
    var referenceFormFolderVersionID = $(this.elemenIDs.referenceFormFolderVersionId + currentInstance.formInstanceId).val();
    var referenceFormFormInstanceID = $(this.elemenIDs.referenceFormFormInstanceId + currentInstance.formInstanceId).val();
    var referenceFormFolderType = "PRODREF_" + referenceFormFormInstanceID;
    var url = this.URLs.folderVersionDetailsNonPortfolioBasedAccount.replace(/{tenantId}/g, currentInstance.tenantId).replace(/{folderVersionId}/g, referenceFormFolderVersionID).replace(/{folderId}/g, referenceFormFolderID).replace(/{folderType}/g, referenceFormFolderType);
    url = url.replace(/{mode}/g, false); //since product reference link is clicked mode needs to be false
    var documentName = $(this.elemenIDs.referenceFormLinkLabelElementId + currentInstance.formInstanceId).val();
    $(this.elemenIDs.referenceFormLinkLabelElementId + currentInstance.formInstanceId).replaceWith('<a href="' + url + '" class="documentLink" style="padding:3px !important;">' + documentName + '</a>');
}


customRulePQ.prototype.setResearchWorkstationLink = function (currentInstance) {
    if (this.isProductDefinition(currentInstance.selectedSectionName)) {
        var productId = $(this.elemenIDs.newproductId + currentInstance.formInstanceId)[0].value;
        var effDate = currentInstance.folderData.effectiveDate;
        var folderVersionNumber = currentInstance.folderData.versionNumber;
        var self = this;
        $(this.elemenIDs.referenceResearchWorkstationLink + currentInstance.formInstanceId).html('<a href class="documentLink" style="padding:3px !important;">Research Workstation Direct Link</a>');

        $(this.elemenIDs.referenceResearchWorkstationLink + currentInstance.formInstanceId).click(function () {
            var url = self.URLs.checkIfProductTranslated.replace(/{productId}/g, productId).replace(/{effDate}/g, effDate).replace(/{folderVersionNumber}/g, folderVersionNumber);
            var data = {
                productId: productId,
                effDate: effDate
            }
            var promise = ajaxWrapper.postAsyncJSONCustom(url, data);
            promise.done(function (result) {
                if (result == true) {
                    var url1 = self.URLs.getResearchWorkstationDetails.replace(/{productId}/g, productId).replace(/{folderVersion}/g, folderVersionNumber);
                    window.location.href = url1;
                }
                else {
                    messageDialog.show("This Product is not translated yet.");
                }
            });
            return false;
        });
    }
}

customRulePQ.prototype.lockBenefitSetGridOnSectionChange = function (currentInstance, sectionDetails, newSection, callbackMetaData) {
    var customRuleInstance = this;
    var formInstanceBuilder = currentInstance;
    yesNoConfirmDialog.show('Are you sure you want to finalize the Network List?', function (e) {
        yesNoConfirmDialog.hide();
        if (e) {
            var benefitSerGrid = currentInstance.repeaterBuilders.filter(function (dt) {
                return dt.fullName == customRuleInstance.fullName.networkList
            })

            var postData = {
                tenantId: currentInstance.tenantId,
                formInstanceId: currentInstance.formInstanceId,
                formDesignVersionId: currentInstance.formDesignVersionId,
                folderVersionId: currentInstance.folderVersionId,
                formDesignId: currentInstance.formDesignId
            }

            var hasError = customRuleInstance.validateBenefitSetGrid(formInstanceBuilder, benefitSerGrid);

            if (hasError == false) {
                currentInstance.formData[customRuleInstance.sectionName.customRulesSettings][customRuleInstance.elementName.isBenefitSetGridLock] = "Yes";
            }
            var sectionData = currentInstance.formData[sectionDetails.FullName];
            var messageToPost = { tenantId: currentInstance.tenantId, formInstanceId: currentInstance.formInstanceId, folderVersionId: currentInstance.folderVersionId, formDesignId: currentInstance.formDesignId, formDesignVersionId: currentInstance.formDesignVersionId, sectionName: sectionDetails.FullName, sectionData: JSON.stringify(sectionData) };
            var promise = ajaxWrapper.postJSON(currentInstance.URLs.saveFormInstanceSectionData, messageToPost);
            promise.done(function (xhr) {
                if (hasError == false) {
                    var benefitSetLock = ajaxWrapper.postAsyncJSONCustom(customRuleInstance.URLs.updateBenefitSetGridLockStatus, postData);

                    //var currentformUtilities = new formUtilities(formInstanceBuilder.formInstanceId);
                    //currentformUtilities.sectionManipulation.disableRepeater(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId);
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId).find('#btnRepeaterBuilderAdd').addClass('ui-state-disabled');
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId).find('#btnRepeaterBuilderCopy').addClass('ui-state-disabled');
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId).find('#btnRepeaterBuilderDelete').addClass('ui-state-disabled');
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + currentInstance.formInstanceId).find('#btnRepeaterBuilderView').addClass('ui-state-disabled');
                    $(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + formInstanceBuilder.formInstanceId).setColProp('BenefitSetName', { editable: false });
                    benefitSetLock.done(function (result) {
                        //Check sections updated and set reload true;
                        if (typeof (xhr) === "object" && xhr.length > 0) {
                            $.each(xhr, function () {
                                var that = this.SectionName;
                                var thatData = this.SectionData;
                                $.each(currentInstance.sections, function () {
                                    if (this.FullName === that) {
                                        this.IsLoaded = false;
                                        currentInstance.formData[that] = JSON.parse(thatData);
                                    }
                                });
                            });
                        }
                        //end
                        if (newSection !== null && newSection !== undefined) {
                            currentInstance.selectedSection = newSection;
                            currentInstance.form.getSectionDataFromServer(callbackMetaData);
                        }
                    });
                }
                else {
                    //Check sections updated and set reload true;
                    if (typeof (xhr) === "object" && xhr.length > 0) {
                        $.each(xhr, function () {
                            var that = this.SectionName;
                            var thatData = this.SectionData;
                            $.each(currentInstance.sections, function () {
                                if (this.FullName === that) {
                                    this.IsLoaded = false;
                                    currentInstance.formData[that] = JSON.parse(thatData);
                                }
                            });
                        });
                    }
                    //end
                    if (newSection !== null && newSection !== undefined) {
                        currentInstance.selectedSection = newSection;
                        currentInstance.form.getSectionDataFromServer(callbackMetaData);
                    }
                }
            });

            if (hasError == true) {
                messageDialog.show("Network List Grid has some validation error");
            }
            promise.fail(this.showError);

        }
        else {
            var sectionData = currentInstance.formData[sectionDetails.FullName];
            var messageToPost = { tenantId: currentInstance.tenantId, formInstanceId: currentInstance.formInstanceId, folderVersionId: currentInstance.folderVersionId, formDesignId: currentInstance.formDesignId, formDesignVersionId: currentInstance.formDesignVersionId, sectionName: sectionDetails.FullName, sectionData: JSON.stringify(sectionData) };
            var promise = ajaxWrapper.postJSON(currentInstance.URLs.saveFormInstanceSectionData, messageToPost);
            promise.done(function (xhr) {
                //Check sections updated and set reload true;
                if (typeof (xhr) === "object" && xhr.length > 0) {
                    $.each(xhr, function () {
                        var that = this.SectionName;
                        var thatData = this.SectionData;
                        $.each(currentInstance.sections, function () {
                            if (this.FullName === that) {
                                this.IsLoaded = false;
                                currentInstance.formData[that] = JSON.parse(thatData);
                            }
                        });
                    });
                }
                //end
                if (newSection !== null && newSection !== undefined) {
                    currentInstance.selectedSection = newSection;
                    currentInstance.form.getSectionDataFromServer(callbackMetaData);
                }

            });
            promise.fail(this.showError);
        }
    })
}

customRulePQ.prototype.validateBenefitSetGrid = function (formInstanceBuilder, benefitSerGrid) {
    var customRuleInstance = this;
    var rowIndex = undefined;
    var result = false;
    var benefitdata = $(customRuleInstance.elemenIDs.benefitSetNetwork + formInstanceBuilder.formInstanceId).jqGrid('getGridParam', 'data');

    for (var i = 0; i < benefitdata.length; i++) {
        var ID = $(customRuleInstance.elemenIDs.benefitSetNetwork + formInstanceBuilder.formInstanceId).jqGrid("getDataIDs");
        $.each(ID, function (idx, ct) {
            if (ct == benefitdata[i].RowIDProperty) {
                rowIndex = idx;
                return false;
            }
        });
        var selectedRowId = $(customRuleInstance.elemenIDs.benefitSetNetwork + formInstanceBuilder.formInstanceId).jqGrid('getGridParam', 'selrow');
        var currentPage = $(customRuleInstance.elemenIDs.benefitSetNetwork + formInstanceBuilder.formInstanceId).getGridParam('page');
        var rowNum = $(customRuleInstance.elemenIDs.benefitSetNetwork + formInstanceBuilder.formInstanceId).getGridParam('rowNum');
        if (currentPage != 1) {
            rowIndex = ((currentPage - 1) * rowNum) + rowIndex;
        }

        var validationError = formInstanceBuilder.formValidationManager.handleValidation(customRuleInstance.fullName.networkListBenefitSetName, benefitdata[i].BenefitSetName, rowIndex, '', benefitdata[i].RowIDProperty);
        if (validationError) {
            formInstanceBuilder.validation.handleObjectChangeValidation(validationError);
            benefitSerGrid[0].showValidatedControlsOnRepeaterElementChange(validationError);
        }

        var duplicationObject = formInstanceBuilder.designData.Duplications.filter(function (ct) {
            return ct.FullName == "BenefitSetNetwork.NetworkList.BenefitSetName";
        });

        if (duplicationObject.length > 0) {
            var gridData;
            gridData = benefitdata.filter(function (ct) {
                if (ct.RowIDProperty == benefitdata[i].RowIDProperty) {
                    ct["BenefitSetName"] = benefitdata[i].BenefitSetName;
                }
                return ct;
            });

            var duplicationErrorArray = formInstanceBuilder.formValidationManager.handleDuplication(duplicationObject[0].ParentUIElementName, benefitdata, benefitdata[i].RowIDProperty);
            if (duplicationErrorArray != undefined && duplicationErrorArray.length > 0) {
                for (var k = 0; k < duplicationErrorArray.length; k++) {
                    var duplicationError = duplicationErrorArray[k];
                    formInstanceBuilder.validation.handleObjectChangeValidation(duplicationError);
                    benefitSerGrid[0].showDuplicateControlsOnRepeaterElementChange(duplicationError);
                }
            }
        }
    }
    formInstanceBuilder.bottomMenu.closeBottomMenu();
    formInstanceBuilder.validation.loadValidationErrorGrid();
    var benefitErrorSection = formInstanceBuilder.errorGridData.filter(function (dt) {
        return dt.Section == customRuleInstance.sectionName.benefitSetNetwork;
    })

    if (benefitErrorSection != undefined && benefitErrorSection.length > 0 && benefitErrorSection[0].ErrorRows != undefined && benefitErrorSection[0].ErrorRows.length > 0) {
        var networKListSection = benefitErrorSection[0].ErrorRows.filter(function (dt) {
            return dt.SubSectionName == "Benefit Set / Network => Network List";
        })
    }

    if (networKListSection != undefined && networKListSection.length > 0) {
        result = true;
    }
    return result;
}

customRulePQ.prototype.filterBenefitSummaryTextGrid = function (currentInstance) {
    try {
        rowData = currentInstance.data.filter(function (dt) {
            return dt.RowIDProperty == currentInstance.selectedRowId;
        });
        //filtering Benefit Summary Grid
        //The following gridID represents BenefitSummaryText Grid      
        var gridID = $(this.elemenIDs.benefitSummaryTextGrid + currentInstance.formInstanceId);
        this.applyFilterOnBenefitSummaryGrid(rowData[0], gridID);
    }
    catch (e) {
        throw e;
        console.log(JSON.stringify(e));
    }
}

customRulePQ.prototype.applyFilterOnBenefitSummaryGrid = function (rowData, gridID) {

    var postdata = gridID.jqGrid('getGridParam', 'postData');

    // Here  searchField is 'BenefitSummaryType' column present in both 'BenefitSummary' grid
    // searchOperator is equal
    // and  searchstring is BenefitSummaryType value for that selected row
    var rule = [];
    rule.push({ "field": "BenefitSummaryType", "op": "cn", "data": rowData.BenefitSummaryType });

    if (rule.length > 0) {
        $.extend(postdata,
                       {
                           filters: JSON.stringify({ "groupOp": "AND", "rules": rule }),
                       });
        $(gridID).jqGrid('setGridParam', { search: true, postData: postdata });
    }
    else {
        $(gridID).jqGrid('setGridParam', { search: false, postData: postdata });
    }
    $(gridID).trigger("reloadGrid");
}

customRulePQ.prototype.customRuleOnSectionLoad = function (currentInstance) {
    var customRuleInstance = this;
    switch (currentInstance.selectedSectionName) {
        case customRuleInstance.sectionName.benefitSetNetwork:
            //if (currentInstance.formData[customRuleInstance.sectionName.customRulesSettings][customRuleInstance.elementName.isBenefitSetGridLock] == "Yes") {
            //$(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + currentInstance.formInstanceId).find('#btnRepeaterBuilderAdd').addClass('ui-state-disabled');
            //$(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + currentInstance.formInstanceId).find('#btnRepeaterBuilderCopy').addClass('ui-state-disabled');
            //$(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + currentInstance.formInstanceId).find('#btnRepeaterBuilderDelete').addClass('ui-state-disabled');
            //$(customRuleInstance.elemenIDs.benefitSetNetworkRepeater + currentInstance.formInstanceId).find('#btnRepeaterBuilderView').addClass('ui-state-disabled');
            //$(customRuleInstance.elemenIDs.benefitSetNetwork + currentInstance.formInstanceId).setColProp('BenefitSetName', { editable: false });
            //}
            break;
    }
}

customRulePQ.prototype.getDeductibleAccumulatorForBenefitAltAndTierGrid = function (formInstanceBuilder, BenefitSetName) {
    try {
        var deductibleList = new Array();
        var customRuleInstance = this;

        //var selectOne = {
        //    Enabled: null,
        //    ItemID: 0,
        //    ItemValue: "[Select One]"
        //};
        //deductibleList.push(selectOne);

        var defaultObj = {
            Enabled: null,
            ItemID: "0",
            ItemValue: "0"
        };
        deductibleList.push(defaultObj);
        var options = [];
        $.each(formInstanceBuilder.formData[customRuleInstance.elementName.deductibles][customRuleInstance.elementName.deductibleList], function (i, v) {
            //if (v[customRuleInstance.BenefitSetName].trim() == BenefitSetName.trim()) {
            var obj = {
                Enabled: null,
                ItemID: 0,
                ItemValue: v.AccumNumber + '-' + v.Description
            };

            var isObjExist = deductibleList.filter(function (dt) {
                return dt.ItemValue == obj.ItemValue;
            })

            if (isObjExist.length == 0) {
                deductibleList.push(obj);
            }
            //}
        });

        //if (deductibleList != null && deductibleList.length > 1) {
        //    for (var idx = 0; idx < deductibleList.length; idx++) {
        //        if (deductibleList[idx].ItemValue == "[Select One]") {
        //            options[options.length] = $('<option value="[Select One]" class="standard-optn"></option>');
        //        }
        //        else {
        //            options[idx] = $('<option value="' + deductibleList[idx].ItemValue + '" class="standard-optn">' + deductibleList[idx].ItemValue + '</option>');
        //        }
        //    }
        //}

        // return options;
        return deductibleList;
    } catch (e) {

    }
    return new Array();
}

customRulePQ.prototype.getDeductibleAccumulatorForBenfitAltAndTierGridOnRowViewMode = function (formInstanceBuilder, BenefitSetName) {
    try {
        var deductibleList = new Array();
        var customRuleInstance = this;

        var defaultObj = {
            Enabled: null,
            ItemID: 0,
            ItemValue: "0"
        };
        deductibleList.push(defaultObj);
        var options = [];
        $.each(formInstanceBuilder.formData[customRuleInstance.elementName.deductibles][customRuleInstance.elementName.deductibleList], function (i, v) {
            //if (v[customRuleInstance.BenefitSetName].trim() == BenefitSetName.trim()) {
            var obj = {
                Enabled: null,
                ItemID: 0,
                ItemValue: v.AccumNumber + '-' + v.Description
            };

            var isObjExist = deductibleList.filter(function (dt) {
                return dt.ItemValue == obj.ItemValue;
            })

            if (isObjExist.length == 0) {
                deductibleList.push(obj);
            }
            //}
        });

        return deductibleList;
    } catch (e) {

    }
    return new Array();
}

customRulePQ.prototype.updateBenSummaryDetailsTypeDropDownItem = function (designData, formInstanceId, folderVersionId, element) {
    var postData = {
        tenantId: designData.TenantID,
        formInstanceId: formInstanceId,
        formDesignVersionId: designData.FormDesignVersionId,
        folderVersionId: folderVersionId,
        formDesignId: designData.FormDesignId,
        fullName: element.FullName
    }

    var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getBenSummaryDetailsTypeDropDownItem, postData);

    promise.done(function (result) {
        element.Items = JSON.parse(result);
    });

    return element.Items;
}

customRulePQ.prototype.getCustomDropdownItems = function (fullName, designData, formInstanceID, folderVersionId, elementdesign, formInstanceBuilder) {
    var customRuleInstance = this;
    var items = null;
    switch (fullName) {
        case customRuleInstance.fullName.productDeductibleAccumNo:
        case customRuleInstance.fullName.productDeductibleRelatedAccumNo:
        case customRuleInstance.fullName.MasterListLimitAccumNo:
            items = customRuleInstance.updateAccumNumberDropDownItem(designData, formInstanceID, folderVersionId, elementdesign);
            break
        case customRuleInstance.fullName.additionalServicesDetailsMessageSERL:
        case customRuleInstance.fullName.additionalServicesDetailsDisallowedMessages:
        case customRuleInstance.fullName.serviceGroupMessageSERL:
        case customRuleInstance.fullName.serviceGroupDisallowedMessage:
        case customRuleInstance.fullName.altRuleServiceGroupDetailMessageSERL:
        case customRuleInstance.fullName.altRuleServiceGroupDetailDisallowedMessage:
        case customRuleInstance.fullName.altRuleAdditionalServicesDetailsMessageSERL:
        case customRuleInstance.fullName.altRuleAdditionalServicesDetailsDisallowedMessage:
            items = customRuleInstance.getEmptyDropDownItem();
            break
        case customRuleInstance.fullName.benefitSummaryDetailTableDetailsType:
            items = customRuleInstance.updateBenSummaryDetailsTypeDropDownItem(designData, formInstanceID, folderVersionId, elementdesign);
            break;
        case customRuleInstance.fullName.pdbcTypeFullName:
            //case "temporarypdbc":
            items = customRuleInstance.updatePDBCTypeDropDownItem(elementdesign, designData, formInstanceID);
            break;
        case customRuleInstance.fullName.LimitProcedureTableLTIPAccumNumber:
        case customRuleInstance.fullName.LimitDiagnosisTableLTIDAccumNumber:
        case customRuleInstance.fullName.LimitProviderTableLTPRAccumNumber:
            items = customRuleInstance.getAccumNumberForLimitRepeaterOnPeriodIndicator(designData, formInstanceID, folderVersionId, elementdesign);
            break;
        case customRuleInstance.fullName.benefitReviewAltRulesGridDeductibleAccumulator:
        case customRuleInstance.fullName.benefitReviewGridTierDataDeductibleAccumulator:
        case customRuleInstance.fullName.benefitReviewAltRulesGridTierDataDeductibleAccumulator:
            items = customRuleInstance.getDeductibleAccumulatorForBenefitAltAndTierGrid(formInstanceBuilder);
            break;
    }
    return items;
}

customRulePQ.prototype.updatePDBCTypeDropDownItem = function (elementdesign, designData, formInstanceID) {
    var currentInstance = this;
    var postData = {
        tenantId: designData.TenantID,
        formInstanceId: formInstanceID
    }

    if (this.PDBCTypes.length > 0) {
        elementdesign.Items = this.PDBCTypes;
    }
    else {
        var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getPDBCTypeDropDownItem, postData);
        promise.done(function (result) {
            currentInstance.PDBCTypes = JSON.parse(result);
            elementdesign.Items = currentInstance.PDBCTypes;
        });
    }

    return elementdesign.Items;
}

customRulePQ.prototype.bindPdbcPrefixes = function (currentInstance, isRowEditable) {
    var customRuleInstance = this;
    var data = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.lastSelectedRow;
    });

    if ((currentInstance.PDBCPrefixes != undefined)) {

        var options = [];
        //var selectOneValue = $('<option value="' + Validation.selectOne + '" class="standard-optn">' + Validation.selectOne + '</option>');

        for (var idx = 0; idx < currentInstance.PDBCPrefixes.length; idx++) {
            options[idx] = $('<option value="' + currentInstance.PDBCPrefixes[idx].ItemValue + '" class="standard-optn">' + currentInstance.PDBCPrefixes[idx].ItemValue + '</option>');
        }

        //options[options.length] = selectOneValue;

        var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.pdbcPrefix);
        $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').empty().append(options);

        if (isRowEditable == "Yes" || currentInstance.isGridDivClick == true) {
            $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(data[0][currentInstance.customrule.elementName.pdbcPrefix]);
        }
        else if (currentInstance.PDBCPrefixValue != null && currentInstance.PDBCPrefixValue != "[Select One]") {
            $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(currentInstance.PDBCPrefixValue);
        }

    }

}

customRulePQ.prototype.getPdbcPrefixes = function (currentInstance, value) {
    var customRuleInstance = this;
    var data = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.selectedRowId;
    });

    var pdbcTypeVal = value;//data[0].PDBCType;
    var tenantID = currentInstance.formInstanceBuilder.tenantId;
    var formInstanceID = currentInstance.formInstanceBuilder.formInstanceId;

    var postData = {
        pdbcType: pdbcTypeVal,
        tenantId: tenantID,
        formInstanceId: formInstanceID
    }
    var elementDesign = currentInstance.design.Elements.filter(function (ct) {
        return ct.GeneratedName == "PDBCPrefix";
    });
    var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getPDBCPrefixValueDropDownItem, postData);
    promise.done(function (result) {
        var elementDesign = currentInstance.design.Elements.filter(function (ct) {
            return ct.GeneratedName == "PDBCPrefix";
        });
        elementDesign[0].Items = JSON.parse(result);
        var items = elementDesign[0].Items;

        currentInstance.PDBCPrefixes = [];
        currentInstance.PDBCPrefixes = items;

    });
}

customRulePQ.prototype.getPDBCPrefixValue = function (currentInstance) {
    var rowdata = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty == currentInstance.selectedRowId;
    });;
    var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.pdbcPrefix);
    currentInstance.PDBCPrefixValue = $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val();

    if (currentInstance.PDBCPrefixValue == undefined) {
        currentInstance.PDBCPrefixValue = rowdata[0][currentInstance.customrule.elementName.pdbcPrefix];
    }
}

customRulePQ.prototype.customRowNumberForRepeater = function (fullName) {
    var customRuleInstance = this;
    switch (fullName) {
        case customRuleInstance.fullName.facetProductComponentsPDBCRepeater:
            return 50;
            break;
        default:
            return Repeater.ROWNUMBER;
            break;
    }
}

customRulePQ.prototype.getAccumNumberForLimitRepeater = function (formInstanceBuilder) {
    var customRuleInstance = this;
    var accumNumberList = new Array();
    var accumNumbers = formInstanceBuilder.formData[customRuleInstance.elementName.limits][customRuleInstance.elementName.limitsList];
    if (accumNumbers != undefined && accumNumbers != [] && accumNumbers.length != undefined && accumNumbers.length > 0) {
        $.each(accumNumbers, function (i, v) {
            var isAccumExit = accumNumberList.filter(function (dt) {
                return dt.ItemValue == v.AccumNumber
            });

            if (isAccumExit.length == 0) {
                var obj = {
                    Enabled: null,
                    ItemID: 0,
                    ItemValue: v.AccumNumber
                };
                accumNumberList.push(obj);
            }
        });
    }

    return accumNumberList;
}

customRulePQ.prototype.getAccumNumberForLimitRepeaterOnPeriodIndicator = function (designData, formInstanceID, folderVersionId, elementdesign) {
    var postData = {
        tenantId: designData.TenantID,
        formInstanceId: formInstanceID,
        formDesignVersionId: designData.FormDesignVersionId,
        folderVersionId: folderVersionId,
        formDesignId: designData.FormDesignId,
        fullName: elementdesign.FullName
    }

    var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getAccumuNumberDropDownItem, postData);

    promise.done(function (result) {
        elementdesign.Items = JSON.parse(result);
    });

    return elementdesign.Items;
}

customRulePQ.prototype.addActivityLog = function (currentInstance, rowData, buValueArray, buFilterCriteria) {
    var colCount = 0;
    if (buValueArray == undefined) {
        //repeater non bulk-update case
        for (var prop in rowData) {
            if (currentInstance.rowDataBeforeEdit[prop] != rowData[prop]) {
                var oldValue = currentInstance.rowDataBeforeEdit[prop] == undefined ? "" : currentInstance.rowDataBeforeEdit[prop];
                var newValue = rowData[prop];
                var colName = $(currentInstance.gridElementIdJQ).jqGrid("getGridParam", "colNames")[colCount];
                currentInstance.addEntryToAcitivityLogger(currentInstance.lastSelectedRow, Operation.UPDATE, colName, oldValue, newValue, undefined, undefined, prop);
            }
            colCount++;
        }
    }
    else {
        for (var prop in rowData) {
            var dsName, propIdx, propName = null;
            var oldDataValue = null;
            if (currentInstance.design.RepeaterType == 'child') {
                if (prop.substring(0, 4) == 'INL_') {
                    var propArr = prop.split('_');
                    if (propArr.length == 4) {
                        dsName = propArr[1];
                        propIdx = propArr[2];
                        propName = propArr[3];
                    }
                    oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][dsName][propIdx][propName];
                }
                else
                    oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][prop];
            }
            else
                oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][prop];

            if (oldDataValue != rowData[prop]) {
                if (buValueArray.hasOwnProperty(prop)) {
                    var oldValue = oldDataValue;
                    var newValue = buValueArray[prop];
                    propName = propName == null ? prop : propName;
                    var colName = currentInstance.columnNames.filter(function (dt) {
                        return dt.replace('<font color=red>*</font>', '').replace(/ /g, '').trim() == propName;
                    });
                    colName = colName[0];
                    currentInstance.addEntryToAcitivityLogger(currentInstance.lastSelectedRow, Operation.UPDATE, colName, oldValue, newValue, buFilterCriteria, undefined, prop);
                }
            }
        }
    }
    currentInstance.formInstanceBuilder.hasChanges = true;
    currentInstance.hasChanges = true;
}

customRulePQ.prototype.addActivityLogPQ = function (currentInstance, rowData, buValueArray, buFilterCriteria) {
    var colCount = 0;
    if (buValueArray == undefined) {
        //repeater non bulk-update case
        for (var prop in rowData) {
            var type = typeof rowData[prop];
            if (type !== "object" && prop !== "pq_rowselect" && prop !== "pq_cellselect")
            {
                if (currentInstance.rowDataBeforeEdit[prop] != rowData[prop]) {
                    var oldValue = currentInstance.rowDataBeforeEdit[prop] == undefined ? "" : currentInstance.rowDataBeforeEdit[prop];
                    var storeOld = oldValue;
                    var newValue = rowData[prop];
                    var storeNew = newValue;
                    var colNames = $(currentInstance.gridElementIdJQ).pqGrid("getColModel");
                    //var colName = colNames[colCount].dataIndx;
                    var colName = prop;
                    if (prop.substring(0, 4) == 'INL_') {
                        var propArr = prop.split('_');
                        if (propArr.length == 4) {
                            colName = propArr[3];
                        }
                    }
                    var type = colNames[colCount].editor.type;
                    if (type == 'select') {
                        var itemLen = colNames[colCount].editor.options.length;
                        if (itemLen > 0) {
                            for (i = 0; i < itemLen; i++) {
                                if (colNames[colCount].editor.options[i][storeOld] != undefined) {
                                    oldValue = colNames[colCount].editor.options[i][storeOld];
                                }
                                if (colNames[colCount].editor.options[i][storeNew] != undefined) {
                                    newValue = colNames[colCount].editor.options[i][storeNew];
                                    
                                }
                            }

                        }
                    }
                    currentInstance.addEntryToAcitivityLogger(currentInstance.lastSelectedRow, Operation.UPDATE, colName, oldValue, newValue, undefined, undefined, prop);
                }
            }
            colCount++;
        }
    }
    else {
        for (var prop in rowData) {
            var dsName, propIdx, propName = null;
            var oldDataValue = null;
            if (currentInstance.design.RepeaterType == 'child') {
                if (prop.substring(0, 4) == 'INL_') {
                    var propArr = prop.split('_');
                    if (propArr.length == 4) {
                        dsName = propArr[1];
                        propIdx = propArr[2];
                        propName = propArr[3];
                    }
                    oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][dsName][propIdx][propName];
                }
                else
                    oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][prop];
            }
            else
                oldDataValue = currentInstance.data[currentInstance.lastSelectedRow][prop];

            if (oldDataValue != rowData[prop]) {
                if (buValueArray.hasOwnProperty(prop)) {
                    var oldValue = oldDataValue;
                    var newValue = buValueArray[prop];
                    propName = propName == null ? prop : propName;
                    //var colName = currentInstance.columnNames.filter(function (dt) {
                    //    return dt.replace('<font color=red>*</font>', '').replace(/ /g, '').trim() == propName;
                    //});
                    //colName = colName[0];
                    colName = propName;
                    currentInstance.addEntryToAcitivityLogger(currentInstance.lastSelectedRow, Operation.UPDATE, colName, oldValue, newValue, buFilterCriteria, undefined, prop);
                }
            }
        }
    }
    currentInstance.formInstanceBuilder.hasChanges = true;
    currentInstance.hasChanges = true;
}

customRulePQ.prototype.registerEventForButtonInRepeaterOfMasterList = function (currentInstance) {
    switch (currentInstance.fullName) {
        case this.fullName.serviceGroupingChildServiceDetail:
            $(".viewServiceConfiguration").click(function () {
                currentInstance.saveRow();
                var hasdisable = null;
                var hasDisabledParent = $(currentInstance.customrule.masterListElementIDs.serviceGroupDetailsGridParentDivJQ + currentInstance.formInstanceId).hasClass('disabled');
                var isCurrentRowReadOnly = currentInstance.formInstanceBuilder.folderData.isEditable;
                if (isCurrentRowReadOnly == "True" && hasDisabledParent == false) { hasdisable = true } else { hasdisable = false }
                var element = $(this).attr("Id");
                var parentRowData = $(currentInstance.customrule.masterListElementIDs.serviceGroupDetailsGridJQ + currentInstance.formInstanceId).pqGrid('getRowData', { rowIndx: currentInstance.parentRowId });
                currentInstance.customrule.showMasterListServiceConfigrationPouUp(element, currentInstance.data, parentRowData, currentInstance.tenantId, currentInstance.formInstanceId, currentInstance.formInstanceBuilder, hasdisable);
            });
    }
}
customRulePQ.prototype.showMasterListServiceConfigrationPouUp = function (rowId, data, parentRowData, tenantId, formInstanceId, formInstanceBuilder, isCurrentRowReadOnly) {
    var customRuleInstance = this;

    var serviceGroupDetailsRowData = data.filter(function (ct) {
        return ct.RowIDProperty == rowId;
    });
    if (serviceGroupDetailsRowData != undefined) {
        serviceGroupDetailsRowData = serviceGroupDetailsRowData[0];
    }


    customRuleInstance.initilization();


    var SESE_IDUrl = this.URLs.getModelSESE_IDDataForServiceConfig.replace(/\{tenantId\}/g, tenantId)
                                                               .replace(/\{formInstanceId\}/g, formInstanceId);
    //Ajax call for get 
    $.when(ajaxWrapper.getJSON(this.URLs.getServiceRuleDataForServiceConfig),
            ajaxWrapper.getJSON(SESE_IDUrl)
            ).done(function (ServiceRuleData, ModelSESE_IDData) {
                var sese_IDOpt = "";

                $.each(ModelSESE_IDData[0], function (i, row) {
                    sese_IDOpt = sese_IDOpt + '<option value="' + row.SESEIDName + '">' + row.SESEIDName + '</option>';
                });
                $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).append(sese_IDOpt);
                $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).append(sese_IDOpt);
                var serviceRuleDataOpt = "";
                $.each(ServiceRuleData[0], function (i, row) {
                    serviceRuleDataOpt = serviceRuleDataOpt + '<option value="' + row.SESE_RULE + '">' + row.SESE_RULE + '</option>';
                });

                $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).append(serviceRuleDataOpt);
                $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).append(serviceRuleDataOpt);

                if (!$(customRuleInstance.masterListElementIDs.serviceConfigDialogJQ).hasClass('ui-dialog')) {
                    $(customRuleInstance.masterListElementIDs.serviceConfigDialogJQ).dialog({
                        modal: true,
                        autoOpen: false,
                        draggable: true,
                        resizable: true,
                        zIndex: 1005,
                        width: 870,
                        height: 610,
                        closeOnEscape: false,
                        title: 'Service Configuration',
                        buttons: [{
                            id: "ServiceConfigDialogSave",
                            text: "Save",
                            click: function () {

                                customRuleInstance.serviceRuleConfigData(serviceGroupDetailsRowData, parentRowData, formInstanceBuilder);
                            }
                        }, {
                            id: "ServiceConfigDialogClose",
                            text: "Close",
                            click: function () {
                                $("#accumulatorsGrid").jqGrid("GridUnload");
                                $(this).dialog("close");
                            }
                        }],
                    });
                }

                customRuleInstance.bindDataToControlsAndAccumNumberGrid(formInstanceBuilder, serviceGroupDetailsRowData, parentRowData, isCurrentRowReadOnly);

                var title = "Service Configuration";
                $(customRuleInstance.masterListElementIDs.serviceConfigDialogJQ).dialog('option', 'title', title);
                $(customRuleInstance.masterListElementIDs.serviceConfigDialogJQ).dialog("open");

                if (!isCurrentRowReadOnly) {
                    $(customRuleInstance.masterListElementIDs.serviceConfigDialogJQ).find('input, select, img.ui-datepicker-trigger').prop('disabled', 'disabled');
                    var accumulatorsGrid = $(customRuleInstance.masterListElementIDs.accumulatorsGrid);

                    $(customRuleInstance.masterListElementIDs.serviceConfigDialogSave).prop("disabled", true).addClass("ui-state-disabled");
                    accumulatorsGrid.addClass('ui-state-disabled');

                    $(customRuleInstance.masterListElementIDs.accumulatorsGrid).find("table.ui-jqgrid-btable").find('input, select, img.ui-datepicker-trigger').prop('disabled', 'disabled');
                    $(customRuleInstance.masterListElementIDs.accumulatorsGrid).find("table.ui-jqgrid-btable").find(".jqgrow").attr('disabled', 'disabled');
                }


                $(customRuleInstance.masterListElementIDs.serviceGroupHeaderJQ).text(parentRowData.ServiceGroupHeader);
                $(customRuleInstance.masterListElementIDs.SESEIDJQ).text(serviceGroupDetailsRowData.SESEID);

            }).fail(this.showError);


}

customRulePQ.prototype.serviceRuleConfigData = function (serviceGroupDetailsRowData, parentRowData, formInstanceBuilder) {
    var customRuleInstance = this;

    var defaultRule = $(customRuleInstance.masterListElementIDs.defaultRuleJQ).is(':checked');
    var modelSESE_ID = $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).is(':checked');
    var regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
    var alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
    var altRuleCondtion = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();
    var serviceSESE_ID = $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).val();
    var accumulators = $(customRuleInstance.masterListElementIDs.accumulatorsJQ).is(':checked');
    var limitModelSESE_ID = $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).is(':checked');
    var limitServiceSESE_ID = $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).val();
    var serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data');;

    var validate = false;

    validate = customRuleInstance.validateServiceRule(serviceGroupDetailsRowData);

    if (validate) {
        if (defaultRule) {
            serviceSESE_ID = "";

        }
        else if (modelSESE_ID) {
            //regulerRule = '';
            //alternateRule = '';
            //altRuleCondtion = '';
        }

        if (accumulators) {
            limitServiceSESE_ID = '';
        }
        else {
            //serviceRuleConfigData = [];
        }

        if (accumulators == false && limitModelSESE_ID == false) {
            serviceRuleConfigData = [];
            limitServiceSESE_ID = '';
        }

        var serviceConfigData = {
            defaultRule: defaultRule == true ? "Yes" : "No",
            modelSESE_ID: modelSESE_ID == true ? "Yes" : "No",
            regulerRule: regulerRule,
            alternateRule: alternateRule,
            serviceSESE_ID: serviceSESE_ID,
            accumulators: accumulators == true ? "Yes" : "No",
            limitModelSESE_ID: limitModelSESE_ID == true ? "Yes" : "No",
            accumulatorsData: serviceRuleConfigData,
            limitServiceSESE_ID: limitServiceSESE_ID,
            SESERULEALTCOND: altRuleCondtion
        }
        var currentServiceRuleConfigData = $(customRuleInstance.masterListElementIDs.serviceRuleConfigurationListGridData + formInstanceBuilder.formInstanceId).jqGrid('getGridParam', 'data');;
        customRuleInstance.saveServiceConfigData(serviceConfigData, currentServiceRuleConfigData, serviceGroupDetailsRowData, parentRowData, formInstanceBuilder);
        $(customRuleInstance.masterListElementIDs.serviceConfigDialogJQ).dialog("close");
    }
}

customRulePQ.prototype.validateServiceRule = function (serviceGroupDetailsRowData) {
    var customRuleInstance = this;
    var valid = true;
    var altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val()
    //var serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data');
    var serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data");

    var defaultRule = $(customRuleInstance.masterListElementIDs.defaultRuleJQ).is(':checked');
    var modelSESE_ID = $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).is(':checked');
    var regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
    var alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
    var serviceSESE_ID = $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).val();
    var accumulators = $(customRuleInstance.masterListElementIDs.accumulatorsJQ).is(':checked');
    var limitModelSESE_ID = $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).is(':checked');
    var limitServiceSESE_ID = $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).val();

    var isSelected = serviceRuleConfigData.filter(function (row) {
        return row.Select == "Yes";
    });


    if (regulerRule == "" && defaultRule) {
        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).parent().addClass('has-error');
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('Please select reguler Rule');
        valid = false;
    }



    if (modelSESE_ID) {
        if (serviceSESE_ID == "") {
            $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).parent().addClass('has-error');
            $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('Please select SESE_ID');
            valid = false;
        }
    }

    if (limitModelSESE_ID) {
        if (limitServiceSESE_ID == "") {
            $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).parent().addClass('has-error');
            valid = false;
        }
    }

    if ((accumulators) && isSelected.length == 0) {
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).parent().addClass('has-error');
        if (valid) {
            $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('Please select Atleast one accumulators');
        }
        valid = false;
    }

    if (modelSESE_ID) {
        if (serviceSESE_ID == serviceGroupDetailsRowData.SESEID) {
            $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).parent().addClass('has-error');
            $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('Please select the valid SESE_ID');
            valid = false;
        }
    }

    if (limitModelSESE_ID) {
        if (limitServiceSESE_ID == serviceGroupDetailsRowData.SESEID) {
            $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).parent().addClass('has-error');
            $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('Please select the valid limit Service SESE_ID');
            valid = false;
        }
    }

    if (altRuleCondition == "" && alternateRule != "") {
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).parent().addClass('has-error');
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('Please filed alt Rule Condition');
        valid = false;
    }

    return valid;
}

customRulePQ.prototype.initilization = function () {
    var customRuleInstance = this;

    $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).empty();
    $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).empty();

    $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).append("<option value=''>" + "--Select--" + "</option>");
    $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).append("<option value=''>" + "--Select--" + "</option>");

    $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).empty();
    $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).empty();

    $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).append("<option value=''>" + "--Select--" + "</option>");
    $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).append("<option value=''>" + "--Select--" + "</option>");

    $(customRuleInstance.masterListElementIDs.serviceConfigDialogSave).addClass();

    $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('');
    $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).removeClass('form-control');
    $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val('');

    $(customRuleInstance.masterListElementIDs.serviceGroupHeaderJQ).text('');
    $(customRuleInstance.masterListElementIDs.SESEIDJQ).text('');
    $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).parent().removeClass('has-error');
    $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).parent().removeClass('has-error');
    $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).parent().removeClass('has-error');
    $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).parent().removeClass('has-error');
    $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).parent().removeClass('has-error');

}

customRulePQ.prototype.bindDataToControlsAndAccumNumberGrid = function (formInstanceBuilder, serviceGroupDetailsRowData, parentRowData, isCurrentRowReadOnly) {
    var customRuleInstance = this;

    var accumulatorListData = formInstanceBuilder.formData.Accumulators.AccumulatorList.filter(function (row) {
        return row.Type == "L-Limits";
    });

    var serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.serviceRuleConfigurationListGridData + formInstanceBuilder.formInstanceId).pqGrid("option", "dataModel.data");

    var seelctedSESEIDData = serviceRuleConfigData.filter(function (row) {
        return row.SESEID == serviceGroupDetailsRowData.SESEID && row.ServiceGroupHeader == parentRowData.ServiceGroupHeader
    });

    var accumulatorData = getSelectedAccumulatorListData(seelctedSESEIDData, accumulatorListData);

    var defaultGridData = new Array();
    accumulatorData.filter(function (row) {
        var rowData = {};
        rowData.Select = "No",
        rowData.WeightCounter = ''
        rowData.AccumNumber = row["AccumNumber"]
        rowData.Description = row["Description"]
        rowData.ID = row["ID"]
        defaultGridData.push(rowData);
    });

    customRuleInstance.setServiceRuleData(seelctedSESEIDData);


    var colArray = ['Select', 'Accum Number', 'Description', 'Weight Counter', "ID"];

    var colModel = [];
    colModel.push({
        name: 'Select', index: 'Select', align: 'left', editable: false, width: '100', align: 'center', edittype: "checkbox", editoptions: { value: "true:false", defaultValue: "false" },
        formatter: formaterColumn, unformat: unFormatColumn, sorttype: function (value, item) {
            return item.Select == "Yes" ? true : false;
        }
    });
    colModel.push({ name: 'AccumNumber', index: 'AccumNumber', align: 'center' });
    colModel.push({ name: 'Description', index: 'Description', width: '300', align: 'left' });
    colModel.push({
        name: 'WeightCounter', index: 'WeightCounter', editable: false, align: 'center'
        , formatter: formaterWeightCounter, unformat: unFormatWeightCounter
    });
    colModel.push({
        name: 'ID', align: 'center', width: 80, sortable: true, key: true, hidden: true
    });
    //adding the pager element
    $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).parent().append("<div id='p" + customRuleInstance.masterListElementIDs.accumulatorsGrid + "'></div>");

    var grid = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ), i;
    //clean up the grid first - only table element remains after this
    $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('GridUnload');
    $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid({
        datatype: 'local',
        cache: false,
        data: accumulatorData,
        altclass: 'alternate',
        colNames: colArray,
        colModel: colModel,
        caption: '',
        height: '150',
        width: '800',
        rowNum: 20,
        loadonce: true,
        shrinkToFit: true,
        viewrecords: true,
        hidegrid: false,
        hiddengrid: false,
        ignoreCase: true,
        sortname: 'Select',
        sortorder: 'desc',
        pager: '#p' + customRuleInstance.masterListElementIDs.accumulatorsGrid,
        altRows: true,
        loadComplete: function () {
            var p = this.p, data = p.data, item, $this = $(this), index = p._index, rowid;
            for (rowid in index) {
                if (index.hasOwnProperty(rowid)) {
                    item = data[index[rowid]];
                    if (item.Select == "false" || item.Select == "false" || item.Select == "true" || item.Select == false || item.Select == true) {
                        $this.jqGrid('setSelection', rowid, false);
                    }
                }
            }
        },
    });

    var pagerElement = '#p' + customRuleInstance.masterListElementIDs.accumulatorsGrid;
    $(pagerElement).find('input').css('height', '20px');
    //remove default buttons
    $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

    $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).on('change', 'input[name="checkbox_accumulatorsGrid"]', function (e) {
        var element = $(this).attr("Id");
        var id = element.split('_');
        var cellValue = $(this).is(":checked");
        if (cellValue) {
            cellValue = "Yes";
        }
        else {
            cellValue = "No";
        }

        var weightCounterValue = $('#textbox_' + id[1] + '').val();
        if (weightCounterValue == "" && cellValue == "Yes") {
            weightCounterValue = "100"
            $('#textbox_' + id[1] + '').val("100");
        }
        if (cellValue == "No") {
            weightCounterValue = ""
            $('#textbox_' + id[1] + '').val("");
        }

        //$(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('setCell', id[1], 'Select', cellValue);
        var selectRowData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).getLocalRow(id[1]);
        selectRowData.Select = cellValue;
        selectRowData.WeightCounter = weightCounterValue;
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid("saveRow", id[1], selectRowData);
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).editRow(id[1], true);
    });

    $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).on('change', 'input[name="textbox_accumulatorsGrid"]', function (e) {
        var element = $(this).attr("Id");
        var id = element.split('_');
        var cellValue = $(this).val();
        var selectRowData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).getLocalRow(id[1]);
        selectRowData.WeightCounter = cellValue;
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid("saveRow", id[1], selectRowData);
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).editRow(id[1], true);
    });

    function formaterColumn(cellValue, options, rowObject) {
        var result;
        if (cellValue == "Yes") {
            if (isCurrentRowReadOnly) {
                result = '<input type="checkbox"' + ' class="Accum" id = checkbox_' + options.rowId + ' name= checkbox_' + options.gid + '' + ' checked />';
            }
            else {
                result = "<input type='checkbox' + ' class='Accum' id = checkbox_" + options.rowId + " name= checkbox_" + options.gid + "' + ' checked disabled=disabled'/>";
            }
        }
        else {
            if (isCurrentRowReadOnly) {
                result = "<input type='checkbox' class='Accum' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "'/>";
            }
            else {
                result = "<input type='checkbox' class='Accum' id='checkbox_" + options.rowId + "' name= 'checkbox_" + options.gid + "' disabled='disabled' />";
            }

        }
        return result;
    }

    function unFormatColumn(cellValue, options) {
        var result;
        result = $(this).find('#' + options.rowId).find('input').prop('checked');

        if (result == true || result == "true")
            return 'Yes';
        else
            return 'No';
    }

    function formaterWeightCounter(cellValue, options, rowObject) {
        var result;
        if (isCurrentRowReadOnly) {
            result = "<input type='text' class='form-control WeightCounterText'  id = 'textbox_" + options.rowId + "' name= 'textbox_" + options.gid + "' value='" + rowObject.WeightCounter + "' />";
        }
        else {
            result = "<input type='text' class='form-control WeightCounterText'  id = 'textbox_" + options.rowId + "' name= 'textbox_" + options.gid + "' value='" + rowObject.WeightCounter + "' disabled=disabled />";
        }

        return result;
    }

    function unFormatWeightCounter(cellValue, options, rowObject) {
        var result;
        result = $(this).find('#' + options.rowId).find('input[name="textbox_accumulatorsGrid"]').val();

        return result;
    }

    var defaultRule = $(customRuleInstance.masterListElementIDs.defaultRuleJQ).is(':checked');
    var modelSESE_ID = $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).is(':checked');

    var defaultRulerules = {
        regulerRule: '',
        alternateRule: '',
        altRuleCondition: '',
    }

    var ModelRulerules = {
        regulerRule: '',
        alternateRule: '',
        altRuleCondition: '',
    }
    if (defaultRule) {
        defaultRulerules.regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
        defaultRulerules.alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
        defaultRulerules.altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();
    }

    if (modelSESE_ID) {
        ModelRulerules.regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
        ModelRulerules.alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
        ModelRulerules.altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();
    }


    //var serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data');
    var serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data");
    var accumulators = $(customRuleInstance.masterListElementIDs.accumulatorsJQ).is(':checked');
    var limitModelSESE_ID = $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).is(':checked');

    var accumulatorsGrid = {
        serviceRuleConfigData: '',
    }

    var LimitaccumulatorsGrid = {
        serviceRuleConfigData: '',
    }

    if (accumulators) {
        //accumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data');
        accumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data");
    }

    if (limitModelSESE_ID) {
        //LimitaccumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data');
        LimitaccumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data");
    }

    $(customRuleInstance.masterListElementIDs.defaultRuleJQ).click(function (e) {
        $(customRuleInstance.masterListElementIDs.modleSESE_IDRuleConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.defaultServiceRuleConfigJQ).show();
        $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).prop('checked', false);
        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).removeClass('has-error');

        ModelRulerules.regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
        ModelRulerules.alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
        ModelRulerules.altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();

        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val(defaultRulerules.regulerRule);
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val(defaultRulerules.alternateRule);
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val(defaultRulerules.altRuleCondition);
    });

    $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).click(function (e) {
        $(customRuleInstance.masterListElementIDs.modleSESE_IDRuleConfigJQ).show();
        //$(customRuleInstance.masterListElementIDs.defaultServiceRuleConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.defaultRuleJQ).prop('checked', false);
        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).removeClass('has-error');

        defaultRulerules.regulerRule = $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val();
        defaultRulerules.alternateRule = $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val();
        defaultRulerules.altRuleCondition = $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val();

        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val(ModelRulerules.regulerRule);
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val(ModelRulerules.alternateRule);
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val(ModelRulerules.altRuleCondition);
    });

    $(customRuleInstance.masterListElementIDs.accumulatorsJQ).click(function (e) {

        var limitModelSESE_ID = $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).is(':checked');
        //if (limitModelSESE_ID) { LimitaccumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data'); }
        if (limitModelSESE_ID) { LimitaccumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data"); }

        //$(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid("clearGridData");
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('option', 'dataModel.data', []);
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ) .pqGrid('refreshView');
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel", { data: accumulatorsGrid.serviceRuleConfigData == "" ? defaultGridData : accumulatorsGrid.serviceRuleConfigData });
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('refreshView');
        //$(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('setGridParam',
        // {
        //     data: accumulatorsGrid.serviceRuleConfigData == "" ? defaultGridData : accumulatorsGrid.serviceRuleConfigData
        // }).trigger("reloadGrid");

        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.accumulatorsRuleConfigJQ).show();
        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).prop('checked', false);

    });

    $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).click(function (e) {

        var accumulators = $(customRuleInstance.masterListElementIDs.accumulatorsJQ).is(':checked');
        //if (accumulators) { accumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('getGridParam', 'data'); }
        if (accumulators) { accumulatorsGrid.serviceRuleConfigData = $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel.data"); }

       // $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid("clearGridData");
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('option', 'dataModel.data', []);
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('refreshView');
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid("option", "dataModel", { data: LimitaccumulatorsGrid.serviceRuleConfigData == "" ? defaultGridData : LimitaccumulatorsGrid.serviceRuleConfigData });
        $(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).pqGrid('refreshView');
        //$(customRuleInstance.masterListElementIDs.accumulatorsGridJQ).jqGrid('setGridParam',
        // {
        //     data: LimitaccumulatorsGrid.serviceRuleConfigData == "" ? defaultGridData : LimitaccumulatorsGrid.serviceRuleConfigData
        // }).trigger("reloadGrid");
        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDConfigJQ).show();
        //$(customRuleInstance.masterListElementIDs.accumulatorsRuleConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.accumulatorsJQ).prop('checked', false);
    });

    $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).change(function (e) {
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).text('');
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).removeClass('form-control');
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).parent().removeClass('has-error');
        $(customRuleInstance.masterListElementIDs.serviceConfigurationHelpBlockJQ).parent().addClass('has-error');
    });

}

customRulePQ.prototype.setServiceRuleData = function (seelctedSESEIDData) {
    var customRuleInstance = this;

    if (seelctedSESEIDData.length > 0) {
        $(customRuleInstance.masterListElementIDs.defaultRuleJQ).prop('checked', seelctedSESEIDData[0].DefaultRule == "Yes" ? true : false);
        $(customRuleInstance.masterListElementIDs.regulerRuleDDLJQ).val(seelctedSESEIDData[0].RegularRule);
        $(customRuleInstance.masterListElementIDs.alternateRuleDDLJQ).val(seelctedSESEIDData[0].AlternateRule);
        $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).prop('checked', seelctedSESEIDData[0].ServiceModelSESEID == "Yes" ? true : false);
        $(customRuleInstance.masterListElementIDs.serviceSESE_IDJQ).val(seelctedSESEIDData[0].ServiceSESEID);
        $(customRuleInstance.masterListElementIDs.accumulatorsJQ).prop('checked', seelctedSESEIDData[0].Accumulators == "Yes" ? true : false);
        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).prop('checked', seelctedSESEIDData[0].LimitModelSESEID == "Yes" ? true : false);
        $(customRuleInstance.masterListElementIDs.limitServiceSESE_IDJQ).val(seelctedSESEIDData[0].LimitServiceSESEID);
        $(customRuleInstance.masterListElementIDs.altRuleConditionJQ).val(seelctedSESEIDData[0].SESERULEALTCOND);

        if (seelctedSESEIDData[0].DefaultRule == "Yes") {

            $(customRuleInstance.masterListElementIDs.modleSESE_IDRuleConfigJQ).hide();
            $(customRuleInstance.masterListElementIDs.defaultServiceRuleConfigJQ).show();
            $(customRuleInstance.masterListElementIDs.defaultRuleJQ).prop('checked', true);
            $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).prop('checked', false);
        }

        if (seelctedSESEIDData[0].ServiceModelSESEID == "Yes") {
            $(customRuleInstance.masterListElementIDs.modleSESE_IDRuleConfigJQ).show();
            //$(customRuleInstance.masterListElementIDs.defaultServiceRuleConfigJQ).hide();
            $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).prop('checked', true);
            $(customRuleInstance.masterListElementIDs.defaultRuleJQ).prop('checked', false);
        }

        if (seelctedSESEIDData[0].Accumulators == "Yes") {
            $(customRuleInstance.masterListElementIDs.limitModelSESE_IDConfigJQ).hide();
            $(customRuleInstance.masterListElementIDs.accumulatorsRuleConfigJQ).show();
            $(customRuleInstance.masterListElementIDs.accumulatorsJQ).prop('checked', true);
            $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).prop('checked', false);
        }

        if (seelctedSESEIDData[0].LimitModelSESEID == "Yes") {
            //$(customRuleInstance.masterListElementIDs.accumulatorsRuleConfigJQ).hide();
            $(customRuleInstance.masterListElementIDs.limitModelSESE_IDConfigJQ).show();
            $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).prop('checked', true);
            $(customRuleInstance.masterListElementIDs.accumulatorsJQ).prop('checked', false);
        }

    }
    else {
        $(customRuleInstance.masterListElementIDs.modleSESE_IDRuleConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.defaultServiceRuleConfigJQ).show();
        $(customRuleInstance.masterListElementIDs.defaultRuleJQ).prop('checked', true);
        $(customRuleInstance.masterListElementIDs.modelSESE_IDJQ).prop('checked', false);


        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDConfigJQ).hide();
        $(customRuleInstance.masterListElementIDs.accumulatorsRuleConfigJQ).show();
        $(customRuleInstance.masterListElementIDs.accumulatorsJQ).prop('checked', false);
        $(customRuleInstance.masterListElementIDs.limitModelSESE_IDJQ).prop('checked', false);
    }

}

function getSelectedAccumulatorListData(serviceRuleConfigData, accumulatorListData) {

    var keyElement = ['AccumNumber', 'Description']

    var accumulatorData = []
    var j = 1;
    $.each(accumulatorListData, function (idx, data) {
        var rowData = {};
        var selectedData = serviceRuleConfigData.filter(function (row) {
            var count = 0;
            $.each(keyElement, function (idx, mapEle) {

                if (row[mapEle].toString().trim() == data[mapEle].toString().trim()) {
                    count++;
                }

            });
            if (count == keyElement.length)
                return row;
        });


        if (selectedData.length > 0) {
            rowData['Select'] = "Yes";
            rowData['WeightCounter'] = selectedData[0]['WeightCounter'];
        }
        else {
            rowData['Select'] = "No";
            rowData['WeightCounter'] = '';
        }
        rowData['AccumNumber'] = data['AccumNumber'];
        rowData['Description'] = data['Description'];
        rowData["ID"] = j++;
        accumulatorData.push(rowData);
    });

    return accumulatorData;
}

customRulePQ.prototype.saveServiceConfigData = function (serviceConfigData, currentServiceRuleConfigData, serviceGroupDetailsRowData, parentRowData, formInstanceBuilder) {
    var customRuleInstance = this;

    var rowData = []
    var serviceRuleConfigurationListData = formInstanceBuilder.formData[customRuleInstance.masterListSectionName.serviceGroupDefinition][customRuleInstance.masterListSectionName.serviceRuleConfigurationList];
    // select Service Rule Data

    $.each(serviceConfigData.accumulatorsData, function (i, accum) {
        var data = {
            SESEID: serviceGroupDetailsRowData.SESEID,
            ServiceGroupHeader: parentRowData.ServiceGroupHeader,
            DefaultRule: serviceConfigData.defaultRule,
            AlternateRule: serviceConfigData.alternateRule,
            RegularRule: serviceConfigData.regulerRule,
            ServiceModelSESEID: serviceConfigData.modelSESE_ID,
            ServiceSESEID: serviceConfigData.serviceSESE_ID,
            Accumulators: serviceConfigData.accumulators,
            AccumNumber: '',
            Description: '',
            LimitModelSESEID: serviceConfigData.limitModelSESE_ID,
            LimitServiceSESEID: serviceConfigData.limitServiceSESE_ID,
            SESERULEALTCOND: serviceConfigData.SESERULEALTCOND,
            WeightCounter: accum.WeightCounter
        }

        if (accum.Select == "Yes") {
            data.AccumNumber = accum['AccumNumber'];
            data.Description = accum['Description'];
            rowData.push(data);
        }
    });

    if (rowData.length == 0) {
        var data = {
            SESEID: serviceGroupDetailsRowData.SESEID,
            ServiceGroupHeader: parentRowData.ServiceGroupHeader,
            DefaultRule: serviceConfigData.defaultRule,
            AlternateRule: serviceConfigData.alternateRule,
            RegularRule: serviceConfigData.regulerRule,
            ServiceModelSESEID: serviceConfigData.modelSESE_ID,
            ServiceSESEID: serviceConfigData.serviceSESE_ID,
            Accumulators: serviceConfigData.accumulators,
            AccumNumber: '',
            Description: '',
            LimitModelSESEID: serviceConfigData.limitModelSESE_ID,
            LimitServiceSESEID: serviceConfigData.limitServiceSESE_ID,
            SESERULEALTCOND: serviceConfigData.SESERULEALTCOND,
            WeightCounter: ''
        }
        rowData.push(data);
    }


    if (serviceRuleConfigurationListData != undefined) {
        for (var i = 0; i < serviceRuleConfigurationListData.length; i++) {
            if ((serviceGroupDetailsRowData.SESEID === serviceRuleConfigurationListData[i].SESEID &&
                parentRowData.ServiceGroupHeader == serviceRuleConfigurationListData[i].ServiceGroupHeader) || serviceRuleConfigurationListData[i].SESEID == "") {

                formInstanceBuilder.formData[customRuleInstance.masterListSectionName.serviceGroupDefinition][customRuleInstance.masterListSectionName.serviceRuleConfigurationList].splice(i, 1);
                i--;
            }
        }
    }

    $.each(rowData, function (i, row) {
        formInstanceBuilder.formData[customRuleInstance.masterListSectionName.serviceGroupDefinition][customRuleInstance.masterListSectionName.serviceRuleConfigurationList].push(row);
    });

    //Load
   // $(customRuleInstance.masterListElementIDs.serviceRuleConfigurationListGridData + formInstanceBuilder.formInstanceId).jqGrid("clearGridData");
    $(customRuleInstance.masterListElementIDs.serviceRuleConfigurationListGridData + formInstanceBuilder.formInstanceId).pqGrid('option', 'dataModel.data', []);
    $(customRuleInstance.masterListElementIDs.serviceRuleConfigurationListGridData + formInstanceBuilder.formInstanceId).pqGrid('refreshView');
    $(customRuleInstance.masterListElementIDs.serviceRuleConfigurationListGridData + formInstanceBuilder.formInstanceId).pqGrid("option", "dataModel", {
        data: formInstanceBuilder.formData[customRuleInstance.masterListSectionName.serviceGroupDefinition][customRuleInstance.masterListSectionName.serviceRuleConfigurationList]
    });
    $(customRuleInstance.masterListElementIDs.serviceRuleConfigurationListGridData + formInstanceBuilder.formInstanceId).pqGrid('refreshView');

    //$(customRuleInstance.masterListElementIDs.serviceRuleConfigurationListGridData + formInstanceBuilder.formInstanceId).jqGrid('setGridParam',
    //    {
    //        data: formInstanceBuilder.formData[customRuleInstance.masterListSectionName.serviceGroupDefinition][customRuleInstance.masterListSectionName.serviceRuleConfigurationList]
    //    }).trigger("reloadGrid");

}

customRulePQ.prototype.formatIsManagerColumn = function (cellValue, options, rowObject) {
    var result;
    if (cellValue == "Yes") {
        result = '<input type="checkbox"' + ' class="Accum" id = checkbox_' + rowObject.AccumNumber + ' name= checkbox_' + options.gid + '' + ' checked />';
    }
    else {
        result = "<input type='checkbox' class='Accum' id='checkbox_" + rowObject.AccumNumber + "' name= 'checkbox_" + options.gid + "' />";
    }
    return result;
}

customRulePQ.prototype.unFormatColumn = function (cellValue, options) {
    var result;
    result = $(this).find('#' + options.rowId).find('input').prop('checked');

    if (result == true || result == "true")
        return 'Yes';
    else
        return 'No';
}

customRulePQ.prototype.isRuleforconfirmDeleteSESEID = function (fullName) {
    var result = false;
    if (fullName == this.fullName.seseIDListMasterList) {
        result = true;
    }
    return result;
}


customRulePQ.prototype.loadBenefitReviewGridTierPopup = function (currentInstance) {
    var formInstanceBuilder = currentInstance;
    var customRuleInstance = this;
    var elementIDs = {
        BenefitReviewTierDialogJQ: "#BenefitRevwTierDialog",
        BenefitSetNameJQ: "#BenefitSetName",
        NoOfTiersJQ: '#NoOfTiers',
        TierDetailsGridJQ: '#TierDetailsGrid',
        TierDetailsGrid: 'TierDetailsGrid',
        BenefitRevwTierHelpBlockJQ: '#BenefitRevwTierHelpBlock'
    }


    $(elementIDs.BenefitReviewTierDialogJQ).dialog({
        modal: true,
        autoOpen: false,
        draggable: true,
        resizable: true,
        zIndex: 2000,
        width: 700,
        closeOnEscape: false,
        title: 'Add or Remove TierData',
        buttons: [{
            id: "BRTierDialogAdd",
            text: "Add",
            click: function () {
                var BenefitSetName = $(elementIDs.BenefitSetNameJQ).val();
                var tiers = $(elementIDs.NoOfTiersJQ).val();
                //var rowData = $(elementIDs.TierDetailsGridJQ).jqGrid('getGridParam', 'data');
                var rowData = $(elementIDs.TierDetailsGridJQ).pqGrid("option", "dataModel.data");
                var isValid = customRulePQ.prototype.validateBenefitSetName(BenefitSetName, elementIDs.BenefitSetNameJQ, elementIDs.BenefitRevwTierHelpBlockJQ);
                if (isValid)
                    customRuleInstance.addTierData(formInstanceBuilder, BenefitSetName, tiers, rowData);
            }
        },
        {
            id: "BRTierDialogDelete",
            text: "Delete",
            click: function () {
                var BenefitSetName = $(elementIDs.BenefitSetNameJQ).val();
                var tiers = $(elementIDs.NoOfTiersJQ).val();
                //var rowData = $(elementIDs.TierDetailsGridJQ).jqGrid('getGridParam', 'data');
                var rowData = $(elementIDs.TierDetailsGridJQ).pqGrid("option", "dataModel.data");
                var isValid = customRulePQ.prototype.validateBenefitSetName(BenefitSetName, elementIDs.BenefitSetNameJQ, elementIDs.BenefitRevwTierHelpBlockJQ);
                if (isValid)
                    customRuleInstance.deleteTierData(formInstanceBuilder, BenefitSetName, tiers, rowData);
            }
        },
        {
            id: "BRTierDialogClose",
            text: "Close",
            click: function () {
                $(this).dialog("close");
            }
        }],
        open: function (event, ui) {
        }
    });
    $(elementIDs.BenefitReviewTierDialogJQ).dialog("open");
    $(elementIDs.BenefitReviewTierDialogJQ).dialog({ position: 'top' });

    $(elementIDs.NoOfTiersJQ).val("1");
    this.updateBenefitSetNameDropDownItem(formInstanceBuilder.designData, formInstanceBuilder.formInstanceId, elementIDs.BenefitSetNameJQ);
    this.loadTierDetailsGrid(formInstanceBuilder);
}

customRulePQ.prototype.loadTierDetailsGrid = function (formInstanceBuilder) {
    var elementIDs = {
        TierDetailsGridJQ: '#TierDetailsGrid',
        TierDetailsGrid: 'TierDetailsGrid',
        reloadGridJQ: '#refresh_TierDetailsGrid'
    }
    customRuleInstance = this;
    tierDataDetailsArray = this.getTierDetailsData(formInstanceBuilder);

    $(elementIDs.TierDetailsGridJQ).jqGrid("GridUnload");
    //adding the pager element
    $(elementIDs.TierDetailsGridJQ).parent().append("<div id='p" + elementIDs.TierDetailsGrid + "'></div>");

    //code to generate TierDetails grid
    var lastsel2;
    $(elementIDs.TierDetailsGridJQ).jqGrid({
        datatype: "local",
        data: tierDataDetailsArray,
        editurl: 'clientArray',
        cache: false,
        autowidth: true,
        width: 700,
        rowheader: true,
        loadonce: false,
        rowNum: 50,
        viewrecords: true,
        scrollrows: true,
        altRows: true,
        altclass: 'alternate',
        hidegrid: false,
        pager: '#p' + elementIDs.TierDetailsGrid,
        rowList: [50, 100, 150],
        colNames: ['RowIDProperty', 'Select', 'SESEID', 'Benefit Category1', 'Benefit Category2', 'Benefit Category3', 'Place of Service'],

        colModel: [
          { name: 'RowIDProperty', index: 'RowIDProperty', width: 100, sorttype: "int", Key: true, editable: false, hidden: true, sortable: false },
          {
              name: 'Select', index: 'Select', align: 'left', editable: false, width: '50', align: 'center', edittype: "checkbox", editoptions: { value: "true:false", defaultValue: "false" },
              formatter: formaterColumn,
              sorttype: function (value, item) {
                  return item.Select == "Yes" ? true : false;
              }
          },
          { name: 'SESEID', index: 'SESEID', width: 60, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false, formatter: "text", edittype: "text" },
          { name: 'BenefitCategory1', index: 'BenefitCategory1', width: 150, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false },
          { name: 'BenefitCategory2', index: 'BenefitCategory2', width: 150, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false },
          { name: 'BenefitCategory3', index: 'BenefitCategory3', width: 120, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false },
          { name: 'PlaceofService', index: 'PlaceofService', width: 90, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false },
        ],
        caption: "Tier Details",
        gridComplete: function () {
            var p = this.p, data = p.data, item, $this = $(this), index = p._index, rowid;
            for (rowid in index) {
                if (index.hasOwnProperty(rowid)) {
                    item = data[index[rowid]];
                    if (item.Select == "false" || item.Select == "false" || item.Select == "true" || item.Select == false || item.Select == true) {
                        $this.jqGrid('setSelection', rowid, false);
                    }
                }
            }
        }
    });

    $(elementIDs.TierDetailsGridJQ).on('change', 'input[name="checkbox_TierDetailsGrid"]', function (e) {
        var element = $(this).attr("Id");
        var id = element.split('_');
        var rowId = parseInt(id[1]) + 1;
        var cellValue = $(this).is(":checked");
        if (cellValue) {
            cellValue = "Yes";
        }
        else {
            cellValue = "No";
        }
        $(elementIDs.TierDetailsGridJQ).jqGrid('setCell', rowId, 'Select', cellValue);
        var selectRowData = $(elementIDs.TierDetailsGridJQ).getLocalRow(rowId);
        selectRowData.Select = cellValue;
        $(elementIDs.TierDetailsGridJQ).jqGrid("saveRow", rowId, selectRowData);
        $(elementIDs.TierDetailsGridJQ).editRow(rowId, true);
    });

    $(elementIDs.reloadGridJQ).on('click', function (e) {
        $(elementIDs.TierDetailsGridJQ).trigger('reloadGrid');
    });

    function formaterColumn(cellValue, options, rowObject) {
        var result;
        if (cellValue == "Yes") {
            result = '<input type="checkbox"' + ' id = checkbox_' + rowObject.RowIDProperty + ' name= checkbox_' + options.gid + '' + ' checked />';
        }
        else {
            result = "<input type='checkbox' id='checkbox_" + rowObject.RowIDProperty + "' name= 'checkbox_" + options.gid + "'/>";
        }
        return result;
    }

    function unFormatColumn(cellValue, options) {
        var result;
        result = $(this).find('#' + options.rowId).find('input').prop('checked');
        if (result == true || result == "true")
            return 'Yes';
        else
            return 'No';
    }

    var pagerElement = '#p' + elementIDs.TierDetailsGrid;
    $(pagerElement).find('input').css('height', '20px');

    //remove default buttons
    $(elementIDs.TierDetailsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

    $(elementIDs.TierDetailsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

}

customRulePQ.prototype.getTierDetailsData = function (formInstanceBuilder) {
    var tierData = new Array();
    try {

        if (formInstanceBuilder.formData.BenefitReview.BenefitReviewGrid.length > 0) {
            tierData = formInstanceBuilder.formData.BenefitReview.BenefitReviewGrid;
        }

    } catch (e) {
        console.log("error occurred in getTierData - " + e);
    }
    return tierData;
}

customRulePQ.prototype.updateBenefitSetNameDropDownItem = function (designData, formInstanceId, BenefitSetName) {
    //var elementIDs = {
    //    BenefitSetNameJQ: "#BenefitSetName"
    //}
    var postData = {
        tenantId: designData.TenantID,
        formInstanceId: formInstanceId,
    }

    var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getBenefitSetNameDropDownItem, postData);

    promise.done(function (result) {
        var items = JSON.parse(result);
        var benefitSet = "";
        $(BenefitSetName).empty();
        $(BenefitSetName).append("<option value=" + "Select One" + ">" + "Select" + "</option>");
        $.each(items, function (i, row) {
            benefitSet = benefitSet + '<option value="' + row.ItemValue + '">' + row.ItemValue + '</option>';
        });
        $(BenefitSetName).append(benefitSet);
    });
}

customRulePQ.prototype.addTierData = function (formInstanceBuilder, BenefitSetName, tiersToAdd, rowData) {
    var elementIDs = {
        TierDetailsGridJQ: '#TierDetailsGrid',
        BenefitReviewTierDialogJQ: "#BenefitRevwTierDialog",
        BenefitSetNameJQ: "#BenefitSetName",
        BenefitRevwTierHelpBlockJQ: "#BenefitRevwTierHelpBlock"
    }


    tiersToAdd = parseInt(tiersToAdd);
    var newTierData = [];

    var benefitTierData = formInstanceBuilder.formData.BenefitReview.BenefitReviewGridTierData;
    //var benefitReviewData = formInstanceBuilder.formData.BenefitReview.BenefitReviewGrid;

    var selectedrecords = rowData.filter(function (dt) {
        return dt.Select == 'Yes';
    });

    if (selectedrecords.length > 0) {
        $.each(selectedrecords, function (index, val) {
            var oldData = benefitTierData.filter(function (dt) {
                return val.SESEID == dt.SESEID && BenefitSetName == dt.BenefitSetName;
            });
            //var BRData = benefitReviewData.filter(function (bt) {
            //    return val.SESEID == bt.SESEID;
            //});

            //var brNetworkData = BRData[0].ProductNetworkList.filter(function (ct) {
            //    return ct.BenefitSetName == BenefitSetName;
            //});
            //brNetworkData = brNetworkData.length > 0 ? brNetworkData[0] : brNetworkData;
            var highestTierNo = 0;
            if (oldData.length > 0) {
                $.each(oldData, function (index, item) {
                    if (highestTierNo < parseInt(item.TierNo)) {
                        highestTierNo = parseInt(item.TierNo);
                    }
                });
                highestTierNo = highestTierNo == 0 ? 1 : highestTierNo;

                for (var j = highestTierNo; j <= (highestTierNo + tiersToAdd) - 1 ; j++) {
                    var tierObj = {
                        SESEID: oldData[0].SESEID,
                        BenefitCategory1: oldData[0].BenefitCategory1,
                        BenefitCategory2: oldData[0].BenefitCategory2,
                        BenefitCategory3: oldData[0].BenefitCategory3,
                        PlaceofService: oldData[0].PlaceofService,
                        TierNo: j + 1,
                        BenefitSetName: BenefitSetName,
                        Copay: '0.00',
                        Coinsurance: '100.00',
                        AllowedAmount: '9999999.99',
                        AllowedCounter: '9999',
                        DeductibleAccumulator: '0',
                        ExcessPerCounterIndicator: ''
                    }
                    newTierData.push(tierObj);
                }
            }
            else {
                for (var j = 1 ; j <= tiersToAdd ; j++) {
                    var tierObj = {
                        SESEID: val.SESEID,
                        BenefitCategory1: val.BenefitCategory1,
                        BenefitCategory2: val.BenefitCategory2,
                        BenefitCategory3: val.BenefitCategory3,
                        PlaceofService: val.PlaceofService,
                        TierNo: j,
                        BenefitSetName: BenefitSetName,
                        Copay: '0.00',
                        Coinsurance: '100.00',
                        AllowedAmount: '9999999.99',
                        AllowedCounter: '9999',
                        DeductibleAccumulator: '0',
                        ExcessPerCounterIndicator: ''
                    }
                    newTierData.push(tierObj);
                }
            }
        });
    }

    if (newTierData != undefined && newTierData.length > 0) {
        $.each(newTierData, function (i, row) {
            formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridTierDataGenerated].push(row);
        });
        messageDialog.show("Records added successfully.");
        $(elementIDs.BenefitReviewTierDialogJQ).dialog("close");
    }

    $(customRuleInstance.elemenIDs.benefitReviewGridTierDataJQ + formInstanceBuilder.formInstanceId).jqGrid('setGridParam',
        {
            data: formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridTierDataGenerated]
        }).trigger("reloadGrid");

}

customRulePQ.prototype.deleteTierData = function (formInstanceBuilder, BenefitSetName, tiersToDelete, rowData) {
    var elementIDs = {
        TierDetailsGridJQ: '#TierDetailsGrid',
        BenefitReviewTierDialogJQ: "#BenefitRevwTierDialog"
    }
    tiersToDelete = parseInt(tiersToDelete);
    var tierDetailsData = ['SESE_ID', 'BenefitSetName'];
    var newTierDataToDelete = [];

    var benefitTierData = formInstanceBuilder.formData.BenefitReview.BenefitReviewGridTierData;
    //var benefitReviewData = formInstanceBuilder.formData.BenefitReview.BenefitReviewGrid;

    var selectedrecords = rowData.filter(function (dt) {
        return dt.Select == 'Yes';
    });



    if (selectedrecords.length > 0) {
        $.each(selectedrecords, function (index, val) {
            var oldData = benefitTierData.filter(function (dt) {
                return val.SESEID == dt.SESEID && BenefitSetName == dt.BenefitSetName;
            });
            //var BRData = benefitReviewData.filter(function (bt) {
            //    return val.SESEID == bt.SESEID;
            //});

            //var brNetworkData = BRData[0].ProductNetworkList.filter(function (ct) {
            //    return ct.BenefitSetName == BenefitSetName;
            //});
            //brNetworkData = brNetworkData.length > 0 ? brNetworkData[0] : brNetworkData;
            var highestTierNo = 0;
            if (oldData.length > 0) {
                $.each(oldData, function (index, item) {
                    if (highestTierNo < parseInt(item.TierNo)) {
                        highestTierNo = parseInt(item.TierNo);
                    }
                });

                var indexToDel = highestTierNo - tiersToDelete;

                for (var j = highestTierNo ; j >= indexToDel + 1 ; j--) {
                    var tierObj = {
                        SESEID: oldData[0].SESEID,
                        BenefitCategory1: oldData[0].BenefitCategory1,
                        BenefitCategory2: oldData[0].BenefitCategory2,
                        BenefitCategory3: oldData[0].BenefitCategory3,
                        PlaceofService: oldData[0].PlaceofService,
                        TierNo: j,
                        BenefitSetName: BenefitSetName,
                        Copay: '0.00',
                        Coinsurance: '100.00',
                        AllowedAmount: '9999999.99',
                        AllowedCounter: '9999',
                        DeductibleAccumulator: '0',
                        ExcessPerCounterIndicator: ''
                    }
                    newTierDataToDelete.push(tierObj);
                }
            }
            else {
            }
        });
    }

    if (newTierDataToDelete != undefined && newTierDataToDelete.length > 0) {
        for (var i = 0; i < benefitTierData.length ; i++) {
            var isTierExist = newTierDataToDelete.filter(function (dt) {
                return benefitTierData[i].BenefitSetName == dt.BenefitSetName && benefitTierData[i].BenefitCategory1 == dt.BenefitCategory1 && benefitTierData[i].BenefitCategory2 == dt.BenefitCategory2
                    && benefitTierData[i].BenefitCategory3 == dt.BenefitCategory3 && benefitTierData[i].PlaceofService == dt.PlaceofService && benefitTierData[i].SESEID == dt.SESEID && parseInt(benefitTierData[i].TierNo) == dt.TierNo;
            });
            if (isTierExist.length > 0) {
                formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridTierDataGenerated].splice(i, 1);
                i--;
            }
        }
        messageDialog.show("Records deleted successfully.");
        $(elementIDs.BenefitReviewTierDialogJQ).dialog("close");
    }
    else {
        messageDialog.show("No records to delete are present in Benefit Review Tier data for selected service.");
        $(elementIDs.BenefitReviewTierDialogJQ).dialog("close");
    }

    //$(customRuleInstance.elemenIDs.benefitReviewGridTierDataJQ + formInstanceBuilder.formInstanceId).jqGrid("clearGridData");

    $(customRuleInstance.elemenIDs.benefitReviewGridTierDataJQ + formInstanceBuilder.formInstanceId) .pqGrid('option', 'dataModel.data', []);
    $(customRuleInstance.elemenIDs.benefitReviewGridTierDataJQ + formInstanceBuilder.formInstanceId) .pqGrid('refreshView');
    $(customRuleInstance.elemenIDs.benefitReviewGridTierDataJQ + formInstanceBuilder.formInstanceId).pqGrid("option", "dataModel", { data: formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridTierDataGenerated] });
    $(customRuleInstance.elemenIDs.benefitReviewGridTierDataJQ + formInstanceBuilder.formInstanceId).pqGrid('refreshView');
    //$(customRuleInstance.elemenIDs.benefitReviewGridTierDataJQ + formInstanceBuilder.formInstanceId).jqGrid('setGridParam',
    //    {
    //        data: formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewGridTierDataGenerated]
    //    }).trigger("reloadGrid");
}

customRulePQ.prototype.validateBenefitSetName = function (BenefitSetName, BenefitSetNameID, HelpBlockID) {
    var valid;

    $(BenefitSetNameID).on('change', function (e) {
        if ($(this).val() !== 'Select') {
            $(BenefitSetNameID).parent().removeClass('has-error');
            $(HelpBlockID).text('');
            valid = true;
        }
    });

    if (BenefitSetName == 'Select') {
        $(BenefitSetNameID).parent().addClass('has-error');
        $(HelpBlockID).text('Please select BenefitSetName');
        $(HelpBlockID).parent().addClass('has-error');
        valid = false;
    }
    else {
        valid = true;
    }
    return valid;
}

customRulePQ.prototype.loadBenefitReviewGridAltTierPopup = function (currentInstance) {
    var formInstanceBuilder = currentInstance;
    var customRuleInstance = this;
    var elementIDs = {
        BenefitRevwAltTierDialogJQ: "#BenefitRevwAltTierDialog",
        BenefitSetNameJQ: "#AltBenefitSetName",
        NoOfTiersJQ: '#AltNoOfTiers',
        TierAltDetailsGridJQ: '#TierAltDetailsGrid',
        TierAltDetailsGrid: 'TierAltDetailsGrid',
        BenefitRevwAltTierHelpBlockJQ: '#BenefitRevwAltTierHelpBlock'
    }


    $(elementIDs.BenefitRevwAltTierDialogJQ).dialog({
        modal: true,
        autoOpen: false,
        draggable: true,
        resizable: true,
        zIndex: 2000,
        width: 700,
        closeOnEscape: false,
        title: 'Add or Remove TierData',
        buttons: [{
            id: "BRAltTierDialogAdd",
            text: "Add",
            click: function () {
                var BenefitSetName = $(elementIDs.BenefitSetNameJQ).val();
                var tiers = $(elementIDs.NoOfTiersJQ).val();
                var rowData = $(elementIDs.TierAltDetailsGridJQ).jqGrid('getGridParam', 'data');
                var isValid = customRulePQ.prototype.validateBenefitSetName(BenefitSetName, elementIDs.BenefitSetNameJQ, elementIDs.BenefitRevwAltTierHelpBlockJQ);
                if (isValid)
                    customRuleInstance.addAltTierData(formInstanceBuilder, BenefitSetName, tiers, rowData);
            }
        },
        {
            id: "BRAltTierDialogDelete",
            text: "Delete",
            click: function () {
                var BenefitSetName = $(elementIDs.BenefitSetNameJQ).val();
                var tiers = $(elementIDs.NoOfTiersJQ).val();
                var rowData = $(elementIDs.TierAltDetailsGridJQ).jqGrid('getGridParam', 'data');
                var isValid = customRulePQ.prototype.validateBenefitSetName(BenefitSetName, elementIDs.BenefitSetNameJQ, elementIDs.BenefitRevwAltTierHelpBlockJQ);
                if (isValid)
                    customRuleInstance.deleteAltTierData(formInstanceBuilder, BenefitSetName, tiers, rowData);
            }
        },
        {
            id: "BRAltTierDialogClose",
            text: "Close",
            click: function () {
                $(this).dialog("close");
            }
        }],
        open: function (event, ui) {
        }
    });
    $(elementIDs.BenefitRevwAltTierDialogJQ).dialog("open");
    $(elementIDs.BenefitRevwAltTierDialogJQ).dialog({ position: 'top' });
    $(elementIDs.NoOfTiersJQ).val("1");
    this.updateBenefitSetNameDropDownItem(formInstanceBuilder.designData, formInstanceBuilder.formInstanceId, elementIDs.BenefitSetNameJQ);
    this.loadAltTierDetailsGrid(formInstanceBuilder);
}

customRulePQ.prototype.loadAltTierDetailsGrid = function (formInstanceBuilder) {
    var elementIDs = {
        TierAltDetailsGridJQ: '#TierAltDetailsGrid',
        TierAltDetailsGrid: 'TierAltDetailsGrid'
    }
    customRuleInstance = this;
    tierAltDataDetailsArray = this.getAltTierDetailsData(formInstanceBuilder);

    $(elementIDs.TierAltDetailsGridJQ).jqGrid("GridUnload");
    //adding the pager element
    $(elementIDs.TierAltDetailsGridJQ).parent().append("<div id='p" + elementIDs.TierAltDetailsGrid + "'></div>");

    //code to generate TierDetails grid
    var lastsel2;
    $(elementIDs.TierAltDetailsGridJQ).jqGrid({
        datatype: "local",
        data: tierAltDataDetailsArray,
        editurl: 'clientArray',
        cache: false,
        autowidth: true,
        width: 700,
        rowheader: true,
        loadonce: false,
        rowNum: 50,
        viewrecords: true,
        scrollrows: true,
        altRows: true,
        altclass: 'alternate',
        hidegrid: false,
        pager: '#p' + elementIDs.TierAltDetailsGrid,
        rowList: [50, 100, 150],
        colNames: ['RowIDProperty', 'Select', 'SESEID', 'Benefit Category1', 'Benefit Category2', 'Benefit Category3', 'Place of Service'],

        colModel: [
          { name: 'RowIDProperty', index: 'RowIDProperty', width: 100, sorttype: "int", Key: true, editable: false, hidden: true, sortable: false },
          {
              name: 'Select', index: 'Select', align: 'left', editable: false, width: '50', align: 'center', edittype: "checkbox", editoptions: { value: "true:false", defaultValue: "false" },
              formatter: formaterColumn, unformat: unFormatColumn,
              sorttype: function (value, item) {
                  return item.Select == "Yes" ? true : false;
              }
          },
          { name: 'SESEID', index: 'SESEID', width: 60, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false, formatter: "text", edittype: "text" },
          { name: 'BenefitCategory1', index: 'BenefitCategory1', width: 150, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false },
          { name: 'BenefitCategory2', index: 'BenefitCategory2', width: 150, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false },
          { name: 'BenefitCategory3', index: 'BenefitCategory3', width: 120, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false },
          { name: 'PlaceofService', index: 'PlaceofService', width: 90, fixed: true, sorttype: "int", editable: false, hidden: false, sortable: false },
        ],
        caption: "Alt Tier Details",
        gridComplete: function () {
            var p = this.p, data = p.data, item, $this = $(this), index = p._index, rowid;
            for (rowid in index) {
                if (index.hasOwnProperty(rowid)) {
                    item = data[index[rowid]];
                    if (item.Select == "false" || item.Select == "false" || item.Select == "true" || item.Select == false || item.Select == true) {
                        $this.jqGrid('setSelection', rowid, false);
                    }
                }
            }
        }
    });

    $(elementIDs.TierAltDetailsGridJQ).on('change', 'input[name="checkbox_TierAltDetailsGrid"]', function (e) {
        var element = $(this).attr("Id");
        var id = element.split('_');
        var rowId = parseInt(id[1]) + 1;
        var cellValue = $(this).is(":checked");
        if (cellValue) {
            cellValue = "Yes";
        }
        else {
            cellValue = "No";
        }
        $(elementIDs.TierAltDetailsGridJQ).jqGrid('setCell', rowId, 'Select', cellValue);
        var selectRowData = $(elementIDs.TierAltDetailsGridJQ).getLocalRow(rowId);
        selectRowData.Select = cellValue;
        $(elementIDs.TierAltDetailsGridJQ).jqGrid("saveRow", rowId, selectRowData);
        $(elementIDs.TierAltDetailsGridJQ).editRow(rowId, true);
    });

    function formaterColumn(cellValue, options, rowObject) {
        var result;
        if (cellValue == "Yes") {
            result = '<input type="checkbox"' + ' id = checkbox_' + rowObject.RowIDProperty + ' name= checkbox_' + options.gid + '' + ' checked />';
        }
        else {
            result = "<input type='checkbox' class='Accum' id='checkbox_" + rowObject.RowIDProperty + "' name= 'checkbox_" + options.gid + "'/>";
        }
        return result;
    }

    function unFormatColumn(cellValue, options) {
        var result;
        result = $(this).find('#' + options.rowId).find('input').prop('checked');

        if (result == true || result == "true")
            return 'Yes';
        else
            return 'No';
    }

    var pagerElement = '#p' + elementIDs.TierAltDetailsGrid;
    $(pagerElement).find('input').css('height', '20px');

    //remove default buttons
    $(elementIDs.TierAltDetailsGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

    $(elementIDs.TierAltDetailsGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });

}

customRulePQ.prototype.getAltTierDetailsData = function (formInstanceBuilder) {
    var tierAltData = new Array();
    var currentInstance = this;
    try {

        var postData = {
            altData: JSON.stringify(formInstanceBuilder.formData[formInstanceBuilder.customRules.sectionName.benefitReviewGridGenrated][formInstanceBuilder.customRules.sectionName.benefitReviewGridAltRulesDataGenrated])
        }

        var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getBenefitReviewAltData, postData);
        promise.done(function (result) {
            tierAltData = result;
        });

    } catch (e) {
        console.log("error occurred in getTierData - " + e);
    }

    return tierAltData;
}

customRulePQ.prototype.addAltTierData = function (formInstanceBuilder, BenefitSetName, tiersToAdd, rowData) {
    var elementIDs = {
        TierAltDetailsGridJQ: '#TierAltDetailsGrid',
        BenefitRevwAltTierDialogJQ: "#BenefitRevwAltTierDialog"
    }
    tiersToAdd = parseInt(tiersToAdd);
    var newAltTierData = [];

    var benefitAltTierData = formInstanceBuilder.formData.BenefitReview.BenefitReviewAltRulesGridTierData;
    var benefitReviewAltData = formInstanceBuilder.formData.BenefitReview.BenefitReviewAltRulesGrid;

    var selectedrecords = rowData.filter(function (dt) {
        return dt.Select == 'Yes';
    });

    if (selectedrecords.length > 0) {
        $.each(selectedrecords, function (index, val) {
            var oldData = benefitAltTierData.filter(function (dt) {
                return val.SESEID == dt.SESEID && BenefitSetName == dt.BenefitSetName;
            });
            //var BRAltData = benefitReviewAltData.filter(function (bt) {
            //    return val.SESEID == bt.SESEID && bt.BenefitSetName == BenefitSetName;
            //});
            //BRAltData = BRAltData.length > 0 ? BRAltData[0] : BRAltData;
            var highestTierNo = 0;
            if (oldData.length > 0) {
                $.each(oldData, function (index, item) {
                    if (highestTierNo < parseInt(item.TierNo)) {
                        highestTierNo = parseInt(item.TierNo);
                    }
                });
                highestTierNo = highestTierNo == 0 ? 1 : highestTierNo;

                for (var j = highestTierNo; j <= (highestTierNo + tiersToAdd) - 1 ; j++) {
                    var tierObj = {
                        SESEID: oldData[0].SESEID,
                        BenefitCategory1: oldData[0].BenefitCategory1,
                        BenefitCategory2: oldData[0].BenefitCategory2,
                        BenefitCategory3: oldData[0].BenefitCategory3,
                        PlaceofService: oldData[0].PlaceofService,
                        TierNo: j + 1,
                        BenefitSetName: BenefitSetName,
                        Copay: '0.00',
                        Coinsurance: '100.00',
                        AllowedAmount: '9999999.99',
                        AllowedCounter: '9999',
                        DeductibleAccumulator: '0',
                        ExcessPerCounterIndicator: ''
                    }
                    newAltTierData.push(tierObj);
                }
            }
            else {
                for (var j = 1; j <= tiersToAdd; j++) {
                    var tierObj = {
                        SESEID: val.SESEID,
                        BenefitCategory1: val.BenefitCategory1,
                        BenefitCategory2: val.BenefitCategory2,
                        BenefitCategory3: val.BenefitCategory3,
                        PlaceofService: val.PlaceofService,
                        TierNo: j,
                        BenefitSetName: BenefitSetName,
                        Copay: '0.00',
                        Coinsurance: '100.00',
                        AllowedAmount: '9999999.99',
                        AllowedCounter: '9999',
                        DeductibleAccumulator: '0',
                        ExcessPerCounterIndicator: ''
                    }
                    newAltTierData.push(tierObj);
                }
            }
        });
    }

    if (newAltTierData != undefined && newAltTierData.length > 0) {
        $.each(newAltTierData, function (i, row) {
            formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].push(row);
        });
        messageDialog.show("Records added successfully.");
        $(elementIDs.BenefitRevwAltTierDialogJQ).dialog("close");
    }


    $(customRuleInstance.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstanceBuilder.formInstanceId).jqGrid('setGridParam',
        {
            data: formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated]
        }).trigger("reloadGrid");
}

customRulePQ.prototype.deleteAltTierData = function (formInstanceBuilder, BenefitSetName, tiersToDelete, rowData) {
    var elementIDs = {
        TierAltDetailsGridJQ: '#TierAltDetailsGrid',
        BenefitRevwAltTierDialogJQ: "#BenefitRevwAltTierDialog"
    }
    tiersToDelete = parseInt(tiersToDelete);
    var newTierDataToDelete = [];

    var benefitAltTierData = formInstanceBuilder.formData.BenefitReview.BenefitReviewAltRulesGridTierData;
    //var benefitReviewAltData = formInstanceBuilder.formData.BenefitReview.BenefitReviewAltRulesGrid;

    var selectedrecords = rowData.filter(function (dt) {
        return dt.Select == 'Yes';
    });

    if (selectedrecords.length > 0) {
        $.each(selectedrecords, function (index, val) {
            var oldData = benefitAltTierData.filter(function (dt) {
                return val.SESEID == dt.SESEID && BenefitSetName == dt.BenefitSetName;
            });
            //var BRAltData = benefitReviewAltData.filter(function (bt) {
            //    return val.SESEID == bt.SESEID;
            //});           
            //BRAltData = BRAltData.length > 0 ? BRAltData[0] : BRAltData;
            var highestTierNo = 0;
            if (oldData.length > 0) {
                $.each(oldData, function (index, item) {
                    if (highestTierNo < parseInt(item.TierNo)) {
                        highestTierNo = parseInt(item.TierNo);
                    }
                });

                var indexToDel = highestTierNo - tiersToDelete;

                for (var j = highestTierNo ; j >= indexToDel + 1 ; j--) {
                    var tierObj = {
                        SESEID: oldData[0].SESEID,
                        BenefitCategory1: oldData[0].BenefitCategory1,
                        BenefitCategory2: oldData[0].BenefitCategory2,
                        BenefitCategory3: oldData[0].BenefitCategory3,
                        PlaceofService: oldData[0].PlaceofService,
                        TierNo: j,
                        BenefitSetName: BenefitSetName,
                        Copay: '0.00',
                        Coinsurance: '100.00',
                        AllowedAmount: '9999999.99',
                        AllowedCounter: '9999',
                        DeductibleAccumulator: '0',
                        ExcessPerCounterIndicator: ''
                    }
                    newTierDataToDelete.push(tierObj);
                }
            }
            else {
            }
        });
    }

    if (newTierDataToDelete != undefined && newTierDataToDelete.length > 0) {
        for (var i = 0; i < benefitAltTierData.length ; i++) {
            var isTierExist = newTierDataToDelete.filter(function (dt) {
                return benefitAltTierData[i].BenefitSetName == dt.BenefitSetName && benefitAltTierData[i].BenefitCategory1 == dt.BenefitCategory1 && benefitAltTierData[i].BenefitCategory2 == dt.BenefitCategory2
                    && benefitAltTierData[i].BenefitCategory3 == dt.BenefitCategory3 && benefitAltTierData[i].PlaceofService == dt.PlaceofService && benefitAltTierData[i].SESEID == dt.SESEID && parseInt(benefitAltTierData[i].TierNo) == dt.TierNo;
            });
            if (isTierExist.length > 0) {
                formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated].splice(i, 1);
                i--;
            }
        }
        messageDialog.show("Records deleted successfully.");
        $(elementIDs.BenefitRevwAltTierDialogJQ).dialog("close");
    }
    else {
        messageDialog.show("No records to delete are present in Benefit Review Alt Rules Tier data for selected service.");
        $(elementIDs.BenefitRevwAltTierDialogJQ).dialog("close");
    }

    $(customRuleInstance.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstanceBuilder.formInstanceId).jqGrid("clearGridData");
    $(customRuleInstance.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + formInstanceBuilder.formInstanceId).jqGrid('setGridParam',
        {
            data: formInstanceBuilder.formData[customRuleInstance.sectionName.benefitReviewGridGenrated][customRuleInstance.sectionName.benefitReviewAltRulesGridTierDataGenrated]
        }).trigger("reloadGrid");
}


customRulePQ.prototype.showRuleAssignmentPopUp = function (currentInstance) {
    var customRuleInstance = this;

    var elementIDs = {
        ruleAssignmentDialogJQ: "#ruleAssignmentDialog",
        selectServiceRuleJQ: "#selectServiceRule",
        selectBenefitSetNameJQ: "#selectBenefitSetName",
    };

    //if (!$(elementIDs.ruleAssignmentDialogJQ).hasClass('ui-dialog-content')) {
    $(elementIDs.ruleAssignmentDialogJQ).dialog({
        modal: true,
        autoOpen: false,
        draggable: true,
        resizable: true,
        zIndex: 1005,
        width: 400,
        closeOnEscape: false,
        buttons: [
            {
                id: "buttonUpdate",
                text: "Update",
                click: function () {
                    //var gridData = $(currentInstance.gridElementIdJQ).jqGrid('getGridParam', 'data');
                    //var objFilterCriteria = JSON.parse($(currentInstance.gridElementIdJQ).getGridParam("postData").filters);
                    var gridData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
                    var objFilterCriteria = JSON.parse($(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel"));
                    $.each(objFilterCriteria.rules, function (ind, val) {
                        gridData = $.grep(gridData, function (a) {
                            if (a[val.field] != null) {
                                return (a[val.field].toLowerCase().indexOf(val.data.toLowerCase()) >= 0);
                            }
                        });
                    });

                    var filterData = gridData;

                    var rule = $(elementIDs.selectServiceRuleJQ).val();
                    var benefitSetName = $(elementIDs.selectBenefitSetNameJQ).val();

                    var postData = {
                        tenantId: currentInstance.tenantId,
                        formInstanceID: currentInstance.formInstanceId,
                        formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
                        folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
                        formDesignId: currentInstance.formInstanceBuilder.formDesignId,
                        fullName: currentInstance.customrule.fullName.benefitReviewGrid,
                        brgData: JSON.stringify(currentInstance.formInstanceBuilder.formData[currentInstance.customrule.sectionName.benefitReviewGridGenrated]),
                        brgFilterData: JSON.stringify(filterData),
                        rule: rule,
                        benefitSetName: benefitSetName
                    }

                    var promise = ajaxWrapper.postJSON(currentInstance.customrule.URLs.ruleAssignment, postData);

                    promise.done(function (result) {
                        var benefitReviewSectionData = JSON.parse(result);

                        currentInstance.data = benefitReviewSectionData[currentInstance.customrule.sectionName.benefitReviewGridRepeaterGenrated];
                        var brgTierData = benefitReviewSectionData[currentInstance.customrule.sectionName.benefitReviewGridTierDataGenerated];

                        currentInstance.formInstanceBuilder.formData[currentInstance.customrule.sectionName.benefitReviewGridGenrated][currentInstance.customrule.sectionName.benefitReviewGridRepeaterGenrated] = currentInstance.data;

                        $(currentInstance.gridElementIdJQ).jqGrid('GridUnload');
                        currentInstance.columnModels = [];
                        currentInstance.columnNames = [];
                        currentInstance.gridElementId = null;
                        currentInstance.gridElementIdJQ = null;
                        currentInstance.groupHeaders = [];
                        currentInstance.build();

                        var targetRepeater = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                            return rb.fullName == currentInstance.customrule.fullName.benefitReviewGridTierData;
                        });

                        targetRepeater[0].data = brgTierData;

                        $(currentInstance.customrule.elemenIDs.benefitReviewGridTierDataJQ + currentInstance.formInstanceBuilder.formInstanceId).jqGrid('clearGridData');
                        currentInstance.formInstanceBuilder.formData[currentInstance.customrule.sectionName.benefitReviewGridGenrated][currentInstance.customrule.sectionName.benefitReviewGridTierDataGenerated] = brgTierData;
                        $(currentInstance.customrule.elemenIDs.benefitReviewGridTierDataJQ + currentInstance.formInstanceBuilder.formInstanceId).jqGrid('setGridParam',
                          {
                              data: brgTierData
                          }).trigger("reloadGrid");

                        currentInstance.formInstanceBuilder.sections[currentInstance.customrule.elemenIDs.serviceGroupSection].IsLoaded = false;
                        currentInstance.formInstanceBuilder.sections[currentInstance.customrule.elemenIDs.additionalServices].IsLoaded = false;

                        customRuleInstance.activitylogForRuleAssignment(currentInstance, rule, benefitSetName, objFilterCriteria);

                        $(elementIDs.ruleAssignmentDialogJQ).dialog("close");
                    });
                }
            },
            {
                text: "Close",
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
    });
    //  }
    $(elementIDs.ruleAssignmentDialogJQ).dialog('option', 'title', "Rule Assignment");
    $(elementIDs.ruleAssignmentDialogJQ).dialog("open");

    var opt = "";

    var postData = {
        tenantId: currentInstance.tenantId,
        formInstanceID: currentInstance.formInstanceId,
        formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
        folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
        formDesignId: currentInstance.formInstanceBuilder.formDesignId,
    }

    var promise = ajaxWrapper.postJSON(this.URLs.getServiceRulesDropDownItem, postData);

    //var promise = ajaxWrapper.postAsyncJSONCustom(this.URLs.getServiceRulesDropDownItem);

    promise.done(function (result) {
        var rules = JSON.parse(result);
        var seseRules = JSON.parse(rules["rule"]);
        var benefitSetName = JSON.parse(rules["benefitSetName"]);
        $(elementIDs.selectServiceRuleJQ).children('option:not(:first)').remove();
        $.each(seseRules, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.selectServiceRuleJQ).append(opt);

        var opt = "";
        $(elementIDs.selectBenefitSetNameJQ).children('option:not(:first)').remove();
        $.each(benefitSetName, function (i, v) {
            opt = opt + '<option value="' + v.BenefitSetName + '">' + v.BenefitSetName + '</option>'
        });

        $(elementIDs.selectBenefitSetNameJQ).append(opt);
    });

    $(elementIDs.selectBenefitSetNameJQ).attr('disabled', 'disabled');
    $("#buttonUpdate").attr('disabled', 'disabled').addClass('ui-state-disabled');

    $(elementIDs.selectServiceRuleJQ).on('change', function () {
        var rule = $(elementIDs.selectServiceRuleJQ).val();

        if (rule != "") {
            $(elementIDs.selectBenefitSetNameJQ).removeAttr('disabled', 'disabled');
        }
        else {
            $(elementIDs.selectBenefitSetNameJQ).val("Select One");
            $(elementIDs.selectBenefitSetNameJQ).attr('disabled', 'disabled');
            $("#buttonUpdate").attr('disabled', 'disabled').addClass('ui-state-disabled');
        }
    });

    $(elementIDs.selectBenefitSetNameJQ).on('change', function () {
        var benefitSetName = $(elementIDs.selectBenefitSetNameJQ).val();
        var rule = $(elementIDs.selectServiceRuleJQ).val();
        if (benefitSetName != "" && rule != "") {
            $("#buttonUpdate").removeAttr('disabled', 'disabled').removeClass('ui-state-disabled');
        }
        else {
            $("#buttonUpdate").attr('disabled', 'disabled').addClass('ui-state-disabled');
        }
    });
}

customRulePQ.prototype.showAltRuleAssignmentPopUp = function (currentInstance) {
    var customRuleInstance = this;

    var elementIDs = {
        ruleAssignmentDialogJQ: "#altRuleAssignmentDialog",
        selectServiceRuleJQ: "#selectAltServiceRule",
        selectBenefitSetNameJQ: "#selectAltBenefitSetName",
    };

    //if (!$(elementIDs.ruleAssignmentDialogJQ).hasClass('ui-dialog-content')) {
    $(elementIDs.ruleAssignmentDialogJQ).dialog({
        modal: true,
        autoOpen: false,
        draggable: true,
        resizable: true,
        zIndex: 1005,
        width: 400,
        closeOnEscape: false,
        buttons: [
            {
                id: "altRuleButtonUpdate",
                text: "Update",
                click: function () {
                    var gridData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
                    var objFilterCriteria = JSON.parse($(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data").filter);

                    $.each(objFilterCriteria.rules, function (ind, val) {
                        gridData = $.grep(gridData, function (a) {
                            if (a[val.field] != null) {
                                return (a[val.field].toLowerCase().indexOf(val.data.toLowerCase()) >= 0);
                            }
                        });
                    });

                    var filterData = gridData;

                    var rule = $(elementIDs.selectServiceRuleJQ).val();
                    var benefitSetName = $(elementIDs.selectBenefitSetNameJQ).val();

                    var postData = {
                        tenantId: currentInstance.tenantId,
                        formInstanceID: currentInstance.formInstanceId,
                        formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
                        folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
                        formDesignId: currentInstance.formInstanceBuilder.formDesignId,
                        fullName: currentInstance.customrule.fullName.benefitReviewGridAltRulesData,
                        brgData: JSON.stringify(currentInstance.formInstanceBuilder.formData[currentInstance.customrule.sectionName.benefitReviewGridGenrated]),
                        brgFilterData: JSON.stringify(filterData),
                        rule: rule,
                        benefitSetName: benefitSetName
                    }

                    var promise = ajaxWrapper.postJSON(currentInstance.customrule.URLs.ruleAssignment, postData);

                    promise.done(function (result) {
                        var benefitReviewSectionData = JSON.parse(result);

                        currentInstance.data = benefitReviewSectionData[currentInstance.customrule.sectionName.benefitReviewGridAltRulesDataGenrated];
                        var brgTierData = benefitReviewSectionData[currentInstance.customrule.sectionName.benefitReviewAltRulesGridTierDataGenrated];

                        currentInstance.formInstanceBuilder.formData[currentInstance.customrule.sectionName.benefitReviewGridGenrated][currentInstance.customrule.sectionName.benefitReviewGridAltRulesDataGenrated] = currentInstance.data;
                        $(currentInstance.gridElementIdJQ).jqGrid('GridUnload');
                        currentInstance.columnModels = [];
                        currentInstance.columnNames = [];
                        currentInstance.gridElementId = null;
                        currentInstance.gridElementIdJQ = null;
                        currentInstance.groupHeaders = [];

                        currentInstance.build();

                        var targetRepeater = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (rb) {
                            return rb.fullName == currentInstance.customrule.fullName.benefitReviewAltRulesGridTierData;
                        });

                        targetRepeater[0].data = brgTierData;


                        $(currentInstance.customrule.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + currentInstance.formInstanceBuilder.formInstanceId).jqGrid('clearGridData');
                        currentInstance.formInstanceBuilder.formData[currentInstance.customrule.sectionName.benefitReviewGridGenrated][currentInstance.customrule.sectionName.benefitReviewAltRulesGridTierDataGenrated] = brgTierData;
                        $(currentInstance.customrule.elemenIDs.benefitReviewGridAltRuleGridTierDataJQ + currentInstance.formInstanceBuilder.formInstanceId).jqGrid('setGridParam',
                          {
                              data: brgTierData
                          }).trigger("reloadGrid");

                        currentInstance.formInstanceBuilder.sections[currentInstance.customrule.elemenIDs.serviceGroupSection].IsLoaded = false;
                        currentInstance.formInstanceBuilder.sections[currentInstance.customrule.elemenIDs.additionalServices].IsLoaded = false;

                        customRuleInstance.activitylogForRuleAssignment(currentInstance, rule, benefitSetName, objFilterCriteria);

                        $(elementIDs.ruleAssignmentDialogJQ).dialog("close");
                    });
                }
            },
            {
                text: "Close",
                click: function () {
                    $(this).dialog("close");
                }
            }
        ],
    });
    //  };
    $(elementIDs.ruleAssignmentDialogJQ).dialog('option', 'title', "Alt Rule Assignment");
    $(elementIDs.ruleAssignmentDialogJQ).dialog("open");

    var opt = "";

    var postData = {
        tenantId: currentInstance.tenantId,
        formInstanceID: currentInstance.formInstanceId,
        formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
        folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
        formDesignId: currentInstance.formInstanceBuilder.formDesignId,
    }

    var promise = ajaxWrapper.postJSON(this.URLs.getServiceRulesDropDownItem, postData);

    promise.done(function (result) {
        var rules = JSON.parse(result);
        var seseRules = JSON.parse(rules["rule"]);
        var benefitSetName = JSON.parse(rules["benefitSetName"]);
        $(elementIDs.selectServiceRuleJQ).children('option:not(:first)').remove();
        $.each(seseRules, function (i, v) {
            opt = opt + '<option value="' + v.ItemValue + '">' + v.ItemValue + '</option>'
        });
        $(elementIDs.selectServiceRuleJQ).append(opt);

        var opt = "";
        $(elementIDs.selectBenefitSetNameJQ).children('option:not(:first)').remove();
        $.each(benefitSetName, function (i, v) {
            opt = opt + '<option value="' + v.BenefitSetName + '">' + v.BenefitSetName + '</option>'
        });

        $(elementIDs.selectBenefitSetNameJQ).append(opt);
    });

    $(elementIDs.selectBenefitSetNameJQ).attr('disabled', 'disabled');
    $("#altRuleButtonUpdate").attr('disabled', 'disabled').addClass('ui-state-disabled');

    $(elementIDs.selectServiceRuleJQ).on('change', function () {
        var rule = $(elementIDs.selectServiceRuleJQ).val();

        if (rule != "") {
            $(elementIDs.selectBenefitSetNameJQ).removeAttr('disabled', 'disabled');
        }
        else {
            $(elementIDs.selectBenefitSetNameJQ).val("Select One");
            $(elementIDs.selectBenefitSetNameJQ).attr('disabled', 'disabled');
            $("#buttonUpdate").attr('disabled', 'disabled').addClass('ui-state-disabled');
        }
    });

    $(elementIDs.selectBenefitSetNameJQ).on('change', function () {
        var benefitSetName = $(elementIDs.selectBenefitSetNameJQ).val();
        var rule = $(elementIDs.selectServiceRuleJQ).val();
        if (benefitSetName != "" && rule != "") {
            $("#altRuleButtonUpdate").removeAttr('disabled', 'disabled').removeClass('ui-state-disabled');
        }
        else {
            $("#altRuleButtonUpdate").attr('disabled', 'disabled').addClass('ui-state-disabled');
        }
    });
}

function setPDBCPrefixtoDefault(pdbcPrefix, currentInstance) {

    var options = [];
    var deafaultOption = $('<option value="' + pdbcPrefix + '" class="standard-optn">' + pdbcPrefix + '</option>');
    options[0] = deafaultOption;

    var iCol = getColumnSrcIndexByNamePQ($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.pdbcPrefix);
    $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').empty().append(options);
    $("tr#" + currentInstance.selectedRowId, currentInstance.gridElementIdJQ).find('td').eq(iCol).find('select').val(pdbcPrefix);
}

customRulePQ.prototype.activitylogForRuleAssignment = function (currentInstance, rule, benefitSetName, objFilterCriteria) {
    var customRuleInstance = this;
    var ruleMessage = "Rule " + rule + " was assigned to Benefit Set " + benefitSetName;
    var filterMessage = " where ";

    for (var i = 0; i < objFilterCriteria.rules.length; i++) {
        var splitfield = objFilterCriteria.rules[i].field.split("_");
        var field = undefined;
        if (splitfield.length > 1 && splitfield[0] == 'INL') {
            var benefitSetName = currentInstance.groupHeaders[splitfield[2]];
            field = splitfield[splitfield.length - 1] + " ( " + benefitSetName.titleText + " )";
        }
        else {
            field = splitfield[0];
        }

        if (i == 0) {
            filterMessage = filterMessage.concat(field + " is " + objFilterCriteria.rules[i].data);
        }
        else {
            filterMessage = filterMessage.concat(" and " + field + " is " + objFilterCriteria.rules[i].data);
        }
    }

    var activityLogRuleAssignmentMsg = ruleMessage.concat(filterMessage);

    currentInstance.addEntryToAcitivityLogger(undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, activityLogRuleAssignmentMsg);

}

customRulePQ.prototype.SetSubSectionValue = function (currentInstance, relations) {
    if (relations == "A-Include All") {
        var iCol = getColumnSrcIndexByName($(currentInstance.gridElementIdJQ), currentInstance.customrule.elementName.subSection);
        $(currentInstance.gridElementIdJQ).jqGrid('setCell', currentInstance.selectedRowId, currentInstance.customrule.elementName.subSection, " ");
    }
}

