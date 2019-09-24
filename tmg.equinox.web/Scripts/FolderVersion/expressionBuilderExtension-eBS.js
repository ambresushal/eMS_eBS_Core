var expressionBuilderExtension = function () {
    this.FormDesignId = 0;

    this.URLs = {
        getANOCChartService: "/ExpressionBuilder/GetANOCChartServices",
        getAdditionalServicesData: "/ExpressionBuilder/GetAdditionalServicesData",
        getLimitDescriptionForBRGService: "/ExpressionBuilder/GetLimitDescriptionForBRGServiceMedical",
        getRuntimeEBResult: "/ExpressionBuilder/GetRuntimeEBResult"
    }

    this.sectionName = {
        SECTIONASECTIONA1: "SECTION A SECTION A1",
        SECTIONB161718SUMMARYOFPACKAGES: "SECTION B: #16, #17, 18 SUMMARY OF PACKAGES",
        ProofingAttachments: "ProofingAttachments",
        ANOCChartSection: "ANOCChartPlanDetails",
        QHPSection: "QHPDetails",
        PlanIdentifiers: "PlanIdentifiers",
        ProductRules: "ProductRules",
        PlanInformation: "PlanInformation",
        ProductWideProvisions: "ProductWideProvisions",
        BenefitReview: "BenefitReview",
        ServiceWideServiceNetworkWideProvisions: "ServiceWideServiceNetworkWideProvisions",
        Network: "Network",
        Rx: "Rx",
        RxProductInformation: "RxProductInformation",
        DentalProductInformation: "DentalProductInformation",
        AllowableProvisionProductWide: "AllowableProvisionProductWide",
        AllowableProvisionsServiceWide: "AllowableProvisionsServiceWide",
        AllowableProvisionNetworkWide: "AllowableProvisionNetworkWide",
        ProductNetworkWideProvisions: "ProductNetworkWideProvisions",
        HSA: "HSA",
        HSAProductInformation: "HSAProductInformation",
        CascadingCostShare: "CascadingCostShare",
        CostShareGroup: "CostShareGroup",
        GuardrailBRG: "Benefit Review Guardrail",
        ProductRule: "Product Rules"
    }

    this.sectionID = {

    }

    this.repeaterName = {
        Proofing: "Proofing",
        PlanInformation: "PlanInformation",
        OutofPocketInformation: "OutofPocketInformation",
        PlanPremiumInformation: "PlanPremiumInformation",
        PrescriptionInformation: "PrescriptionInformation",
        InitialCoverageLimitInformation: "PreInitialCoverageLimitInformation",
        GapCoverageInformation: "GapCoverageInformation",
        ANOCBenefitsCompare: "ANOCBenefitsCompare",
        ANOCCPDoctorOfficeVisit: "DoctorOfficeVisits",
        ANOCCPHospitalStay: "InpatientHospitalStays",
        ANOCOfficeVisitInfo: "DoctorOfficeVisitInfo",
        ANOCIHSInfo: "InpatientHospitalStayInfo",
        PreferredRetailInfo: "PreferredRetailInformation",
        CostShareDetails: "CostShareDetails",
        AllowableProvisions: "AllowableProvisions",
        RxCostShare: "RxCostShare",
        AllowableProductWideProvisions: "AllowableProductWideProvisions",
        AllowableProductNetworkWideProvisions: "AllowableProductNetworkWideProvisions",
        AllowableServiceWideProvisions: "AllowableServiceWideProvisions",
        AllowableProvisionProductWide: "AllowableProvisionProductWide",
        AllowableProvisionsServiceWide: "AllowableProvisionsServiceWide",
        NetworkTierList: "Network.NetworkTierList",
        AllowableHSAProductWideProvisions: "AllowableHSAProductWideProvisions",
        CostShareGroupList: "CostShareGroupList"
    }

    this.sectionElementName = {
        MarketTier: "MetalTier",
        MarketCoverage: "MarketCoverage",
        CSRVariationType: "CSRVariationType",
        MarketSegment: "MarketSegment",
        SubMarket: "SubMarket",
        ExchangeStatus: "ExchangeStatus",
        GuradrailBRGColumnGroup:
            { 		
                Copay:"Copay",						
                Coinsurance:"Coinsurance",					
                IndDeductible:"IndDeductible",				
                TwoPersonDeductible:"TwoPersonDeductible",		
                FamDeductible:"FamDeductible",				
                IndOOPM:"IndOOPM",					
                TwoPersonOOPM:"TwoPersonOOPM",				
                FamilyOOPM: "FamilyOOPM",
                PenaltyAmount:"PenaltyAmount" 
            },
        BenefitReviewGuardrail: "BenefitReviewGuardrail"
    }

    this.repeaterElementName = {

        PreviousProcessingRule: "PreviousProcessingRule",
        BenefitServiceCode: "BenefitServiceCode",
        CostShareGroup: "CostShareGroup",
        NetworkTier: "NetworkTier",
        NetworkName: "NetworkName",
        Copay: "Copay",
        CopayFrequency: "CopayFrequency",
        Coinsurance: "Coinsurance",
        IndDeductible: "IndDeductible",
        FamDeductible: "FamDeductible",
        IndOOPM: "IndOOPM",
        FamilyOOPM: "FamilyOOPM",
        Covered: "Covered",
        ManualOverride: "ManualOverride",
        ProvisionTextOptions: "ProvisionTextOptions",
        ProvisionNameSERVICEPROVISION: "ProvisionNameSERVICEPROVISION",
        NetworkNameBNTNM: "NetworkNameBNTNM",
        ServiceNameSVNM: "ServiceNameSVNM",
        Network: "Network",
        CostShareType: "CostShareType",
        IndividualDeductible: "IndividualDeductible",
        FamilyDeductible: "FamilyDeductible",
        IndividualOOPM: "IndividualOOPM",
        FamilyOOPM: "FamilyOOPM",
        RefreshCoinsurance: "RefreshCoinsurance",
        ServiceNameSVNM: "ServiceNameSVNM",
        CoinsuranceAmount: "CoinsuranceAmount",
        IsCovered: "IsCovered",
        ProcessingRule: "ProcessingRule",
        CoverageName: "CoverageName",
        CoverageDisplayName: "CoverageDisplayName",
        RefreshCoinsuranceGuardrail: "RefreshCoinsuranceGuardrail",
        AllowableCostShareType: "AllowableCostShareType",
        CostShareInterval: "CostShareInterval",
        EndIntervalLow: "EndIntervalLow",
        StartIntervalLow: "StartIntervalLow",
        StartIntervaHigh: "StartIntervaHigh"
    }

    this.sectionElementFullName = {
        selectDentalPackage: "SECTIONBSUMMARYOFPACKAGES.DentalBenefits.SelectDentalPackage",
        selectVisionBenefitsPackage: "SECTIONBSUMMARYOFPACKAGES.VisionBenefits.SelectVisionBenefitsPackage",
        selectHearingBenefitsPackage: "SECTIONBSUMMARYOFPACKAGES.HearingBenefits.SelectHearingBenefitsPackage",
        contractNumber: "SECTIONASECTIONA1.ContractNumber",
        qhpCSRDependentSource: ["QHPDetails.PlanIdentifiers.MetalTier", "QHPDetails.PlanIdentifiers.MarketCoverage", "ProductRules.PlanInformation.MarketSegment", "ProductRules.PlanInformation.ExchangeStatus"],

        drpCascadeSource: ["QHPDetails.PlanIdentifiers.MetalTier", "QHPDetails.PlanIdentifiers.MarketCoverage", "ProductRules.PlanInformation.MarketSegment", "ProductRules.PlanInformation.ExchangeStatus"],
        drpMarketSegment: "ProductRules.PlanInformation.MarketSegment",
        chkBenefitReviewGuardrail: ["BenefitReviewGuardrail.Copay", "BenefitReviewGuardrail.Coinsurance", "BenefitReviewGuardrail.IndDeductible", "BenefitReviewGuardrail.TwoPerson Deductible", "BenefitReviewGuardrail.FamDeductible", "BenefitReviewGuardrail.IndOOPM", "BenefitReviewGuardrail.TwoPersonOOPM", "BenefitReviewGuardrail.FamOOPM", "BenefitReviewGuardrail.PenaltyAmount"]
    }
    this.sectionEleValues = {
        selectDentalPackage: "Pending",
        selectVisionBenefitsPackage: "Pending",
        selectHearingBenefitsPackage: "Pending",
        subMarketOptions: ["Grandfathered", "ACA", "Grandmothered", "ASO Blue Balance Funding", "Association Health Plan", "No Sub Market"],
        subMarketIndividualMedigapOptions: ["Grandfathered", "No Sub Market"],
        MarketSegmentIndividualOptionSelected: "Individual",
        MarketSegmentSmallGroupOptionSelected: "Small Group",
        MarketSegmentIndividualMedigapOptionSelected: "Individual Medigap",
        MarketSegmentLargeGruopOptionSelected: "Large Group",
        exchangeStatusOptions: ["ACA On Exchange", "ACA Off Exchange", "Private Exchange", "No Exchange"],
        exchangeStatusLargeGruopOptionSelected: ["ACA On Exchange", "ACA Off Exchange", "No Exchange"]
    }

    this.DDLEleValues = {

    }
    this.DDLEleIDs = {
    }

    this.reapterElementFullName = {
        COMMERCIALMEDICAL: {
            brgLimits: "BenefitReview.BenefitReviewGrid"
        }
    }

    this.reapterFullName = {
        ANOCChartPlanInformation: "ANOCChartPlanDetails.PlanInformation",
        ANOCChartOutofPocketInformation: "ANOCChartPlanDetails.OutofPocketInformation",
        ANOCChartPlanPremiumInformation: "ANOCChartPlanDetails.PlanPremiumInformation",
        ANOCChartANOCBenefitsCompare: "ANOCChartPlanDetails.ANOCBenefitsCompare",
        ANOCChartPrescriptionInformation: "ANOCChartPlanDetails.PrescriptionInformation",
        ANOCChartInitialCoverageLimitInformation: "ANOCChartPlanDetails.PreInitialCoverageLimitInformation",
        ANOCChartGapCoverageInformation: "ANOCChartPlanDetails.GapCoverageInformation",
        ANOCChartDoctorOfficeVisitInfo: "ANOCChartPlanDetails.DoctorOfficeVisitInfo",
        ANOCChartHospitalStayServiceInfo: "ANOCChartPlanDetails.InpatientHospitalStayInfo",
        ANOCChartPreferredRetailInfo: "ANOCChartPlanDetails.PreferredRetailInformation",
        ANOCChartPlanDetailsCostShareDetails: "ANOCChartPlanDetails.CostShareDetails",
        CostShareCopay: "CostShare.CostShareGroup.CostShareGroupList",
        CostShareCoinsurance: "CostShare.Coinsurance.CoinsuranceList",
        CostShareDeductible: "CostShare.Deductible.DeductibleList",
        CostShareOOPM: "CostShare.OutofPocketMaximum.OutofPocketMaximumList",
        CostShareROB: "CostShare.ReductionofBenefits.WhatisthePlansReductionofBenefitAmount",
        LimitSummary: "Configuration.Limits.LimitSummary",
        SBCCalculator: "SBCCalculator.CoverageExample",
        COMMERCIALMEDICAL: {
            BenefitReview: "BenefitReview.BenefitReviewGrid",
            SelectProvisions: "ProductRules.SelectProvisions.SelectProvisions",
            BRAGSelectProvisions: "BenefitReview.SelectProvisions.SelectProvisions",
            NetworkAllowableProvisions: "Network.AllowableProvisions",
            NetworkSelectProvisions: "Network.SelectProvisions.SelectProvisions",
            NetworkTierList: "Network.NetworkTierList",
            RxBenefitReview: "Rx.RxProductInformation.RxBenefitReview",
            DentalBenefitReview: "Dental.DentalProductInformation.DentalBenefitReview",
            CostShareGroupList: "CascadingCostShare.CostShareGroup.CostShareGroupList",
            LimitInformationDetail: "Limits.LimitInformationDetail",
            SelectHSAProvisions: "HSA.HSAProductInformation.SelectHSAProductWideProvisions",
            SpecialServiceSelectionRepeater: "ProductRules.SpecialServiceSelectionRepeater",
            CoverageLevelList: "Network.CoverageLevelList",
            GuardrailsCostShareGroupList: "CascadingCostShare.CostShareGuardrail.GuardrailsCostShareGroupList",
            GuardrailsSlidingCostShareList: "CascadingCostShare.SlidingCostShareGuardrail.GuardrailsSlidingCostShareList",
            SlidingCostShareList: "SlidingCostShare.SlidingCostShareGroup.SlidingCostShareList",
            ProviderNetworkMap: "Network.ProviderNetworkMap.ProviderNetworkList",
            GuardrailsBenefitReviewGrid: "BenefitReviewGuardrail.GuardrailsBenefitReviewGrid",
            GuardrailBenefitReviewSlidingCostShare: "BenefitReviewGuardrail.BenefitReviewSlidingCostShareGuardrail.GuardrailBenefitReviewSlidingCostShare",
            BenefitReviewGridSlidingCostShare: "BenefitReview.BenefitReviewSlidingCostShare.BenefitReviewGridSlidingCostShare",
            GuardrailsBenefitReview: "BenefitReviewGuardrail.GuardrailsBenefitReviewGrid",
            SlidingCostShareGuardrail: "SlidingCostShare.SlidingCostShareGuardrail.GuardrailsSlidingCostShareList",
            SlidingCostShareList: "SlidingCostShare.SlidingCostShareGroup.SlidingCostShareList",
            GuardrailBRG: "GuardrailsBenefitReviewGrid",
            AllowableLimitsList: "Limits.AllowableLimits.AllowableLimitsList"
        },
        BUSINESS: {
            ServiceGroupDetails: "StandardServices.ServiceGroupDetails",
        },
        RX: {
            RxBenefitReview: "RxProductInformation.RxBenefitReview",
            RxSelectProductWideProvisions: "RxProductInformation.SelectProductWideProvisions",
            RxSelectProductNetworkWideProvisions: "RxProductInformation.SelectProductNetworkWideProvisions",
            RxSelectServiceNetworkWideProvisions: "RxProductInformation.SelectServiceNetworkWideProvisions"

        },
        DENTAL: {
            DentalSelectProductWideProvisions: "DentalProductInformation.SelectProductWideProvisions",
            DentalSelectServiceWideProvisions: "DentalProductInformation.SelectServiceWideProvisions",
            DentalBenefitReview: "DentalProductInformation.DentalBenefitReview"
        },
        PlansandBenefitTemplate: {
            GeneralInformation: "BenefitInformation.GeneralInformation"
        },
        SBCCalculatorView: {
            CoverageExampleDiabetic: "CoverageExample.CoverageExampleDiabetic",
            CoverageExampleFracture: "CoverageExample.CoverageExampleFracture",
            CoverageExampleMaternity: "CoverageExample.CoverageExampleMaternity"
        },

        LimitInformationDetail: "Limits.LimitsInformation.LimitInformationDetail",
        LIMITSML: {
            LimitDetailsList: "LimitDetails.LimitDetailList",
        }
    }

    this.repeatersHavingDatasource = {
        CostShareGroupList: "CostShare.CostShareGroup.CostShareGroupList",
        CoinsuranceList: "CostShare.Coinsurance.CoinsuranceList",
        DeductibleList: "CostShare.Deductible.DeductibleList",
        OutofPocketMaximumList: "CostShare.OutofPocketMaximum.OutofPocketMaximumList",
        ReductionofBenefitList: "CostShare.ReductionofBenefits.WhatisthePlansReductionofBenefitAmount",
        StandardServiceList: "StandardServices.StandardServiceList",
        BenefitReviewGrid: "BenefitReview.BenefitReviewGrid",
        AdditionalServiceList: "AdditionalServices.AdditionalServiceList",
    }

    this.KeyName = {
        RowIDProperty: "RowIDProperty",
        BenefitCategory1: "BenefitCategory1",
        BenefitCategory2: "BenefitCategory2",
        BenefitCategory3: "BenefitCategory3",
        PlaceofService: "PlaceofService",
        NetworkName: "NetworkName",
        NetworkNameBNTNM: "NetworkNameBNTNM",
        SVID: "SVID",
        SVNM: "SVNM",
        ServiceGroup: "ServiceGroup",
        ServiceNameSVNM: "ServiceNameSVNM",
        SameAsNetworkTier: "SameAsNetworkTier",
        SameAsCostShare: "SameAsCostShare",
        CostShareGroup: "CostShareGroup",
        CostShareType: "CostShareType",
        CoinsuranceAmount: "CoinsuranceAmount",
        CopayFrequency: "CopayFrequency",
        CopayValue: "CopayValue",
        NetworkTier: "NetworkTier",
        CopaySupplement: "CopaySupplement",

        Combined: "Combined",
        Accums: "Accums",
        Accums: "Accums",
        LimitAdditionalDetails: "LimitAdditionalDetails",
        Limit2AdditionalDetails: "Limit2AdditionalDetails",
        AllowedLimits: "AllowedLimits"
    }

    this.elementIDs = {
        effectiveDate: "#WEL2359TextBox58405",
        termDate: "#WEL2359TextBox58406",
        selectDentalPackage: "#WEL2359DropDown58399",
        selectVisionBenefitsPackage: "#WEL2359DropDown58409",
        hearingBenefits: "#WEL2359DropDown58414",
        contractNumber: "WEL2359TextBox58027",
        ProofingRepeater: "#WEL2359Repeater66420",
        searchANOCChartID: "ANO2386TextBox71467",
        searchLabel: "ANO2386TextBox71467",
        tdFolderNameJQ: "#ANO2386TextBox71469",
        tdFolderVersionNumberJQ: "#ANO2386TextBox71470",
        tdEffectiveDateJQ: "#ANO2386TextBox71471",
        thSourceDocumentJQ: "#ANO2386TextBox71472",
        thSourceDocumentIDJQ: "#ANO2386TextBox71473",
        isThisanEGWPPlan: "WEL2359DropDown74423",
        csrVariationType: "#COM2405DropDown9003855200",
        searchCalculatorSBCId: "SBC1322TextBox77508",
        searchCalculatorSBCLabel: "SBC1322TextBox77508",
    }

    this.StandardValue = {

    }

    this.defaultValue = {
        borderColor: "border-color",
        borderWidth: "border-width",
        color: "aqua",
        borderPixcel: "2px",
    }

    this.sourceDocument = {};
    this.groupHeaders = [];

}

expressionBuilderExtension.prototype.hasExpressionBuilderRule = function (formDesignId) {
    var result = false;
    if (formDesignId == FormTypes.COMMERCIALMEDICAL || formDesignId == FormTypes.SBCDESIGN || formDesignId == FormTypes.RX || formDesignId == FormTypes.DENTAL || formDesignId == FormTypes.PlansandBenefitTemplate || formDesignId == FormTypes.SystemConfiguration || formDesignId == FormTypes.LIMITSML) {
        this.FormDesignId = formDesignId;
        result = true;
    }
    return result;
}

expressionBuilderExtension.prototype.hasCustomRuleColumn = function (formDesignId, repeaterFullName, columnName) {
    var ebInstance = this;
    var result = false;
    if ((repeaterFullName.toUpperCase().indexOf('GUARDRAILS') != -1) || (repeaterFullName.toUpperCase().indexOf('GUARDRAIL') != -1)) {
        return true;
    }
    switch (formDesignId) {
        case FormTypes.COMMERCIALMEDICAL:
            switch (repeaterFullName) {
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.SelectProvisions:
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.BRAGSelectProvisions:
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.NetworkSelectProvisions:
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.SelectHSAProvisions:
                    switch (columnName) {
                        case ebInstance.repeaterElementName.ProvisionTextOptions:
                            result = true;
                            break;
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.SpecialServiceSelectionRepeater:
                    switch (columnName) {
                        case ebInstance.KeyName.BenefitCategory3:
                            result = true;
                            break;
                    }
                    break;

                case ebInstance.reapterFullName.LimitInformationDetail:
                    switch (columnName) {
                        case ebInstance.KeyName.LimitAdditionalDetails:
                            result = true;
                            break;
                        case ebInstance.KeyName.Limit2AdditionalDetails:
                            result = true;
                            break;
                    }
                    break;

            }


            break;
        case FormTypes.LIMITSML:
            switch (repeaterFullName) {
                case ebInstance.reapterFullName.LIMITSML.LimitDetailsList:
                    switch (columnName) {
                        case ebInstance.KeyName.LimitAdditionalDetails:
                            result = true;
                            break;
                        case ebInstance.KeyName.Limit2AdditionalDetails:
                            result = true;
                            break;
                    }
                    break;
            }
            break;
        case FormTypes.RX:
            switch (repeaterFullName) {
                case ebInstance.reapterFullName.RX.RxSelectProductWideProvisions:
                case ebInstance.reapterFullName.RX.RxSelectProductNetworkWideProvisions:
                case ebInstance.reapterFullName.RX.RxSelectServiceNetworkWideProvisions:
                    switch (columnName) {
                        case ebInstance.repeaterElementName.ProvisionTextOptions:
                            result = true;
                            break;
                    }
                    break;
            }
            break;
        case FormTypes.DENTAL:
            switch (repeaterFullName) {
                case ebInstance.reapterFullName.DENTAL.DentalSelectProductWideProvisions:
                case ebInstance.reapterFullName.DENTAL.DentalSelectServiceWideProvisions:
                    switch (columnName) {
                        case ebInstance.repeaterElementName.ProvisionTextOptions:
                            result = true;
                            break;
                    }
                    break;
            }
            break;
        case FormTypes.SBCDESIGN:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.SBCCalculatorView.CoverageExampleDiabetic:
                case ebInstance.reapterFullName.SBCCalculatorView.CoverageExampleFracture:
                case ebInstance.reapterFullName.SBCCalculatorView.CoverageExampleMaternity:
                    result = true;
                    break;
            }
            break;
    }
    return result;
}

expressionBuilderExtension.prototype.hideAddButtonforRepeater = function (repeaterName) {
    var result = false;
    var currentInstance = this;
    $.each(currentInstance.repeatersHavingDatasource, function (index, name) {
        if (name == repeaterName) {
            result = true;
            return;
        }
    });

    return result;
}


expressionBuilderExtension.prototype.hideMinusButtonforRepeater = function (repeaterName) {
    var result = false;
    var currentInstance = this;
    $.each(currentInstance.repeatersHavingDatasource, function (index, name) {
        if (name == repeaterName) {
            result = true;
            return;
        }
    });

    return result;
}

expressionBuilderExtension.prototype.hideCopyButtonforRepeater = function (repeaterName) {
    var result = false;
    var currentInstance = this;
    $.each(currentInstance.repeatersHavingDatasource, function (index, name) {
        if (name == repeaterName) {
            result = true;
            return;
        }
    });

    return result;
}

expressionBuilderExtension.prototype.sectionElementOnChange = function (currentInstance, elementPath, value) {
    var ebInstance = this;
    var currentInstance = currentInstance;
    if (currentInstance.formDesignId == FormTypes.COMMERCIALMEDICAL) {
        var validElementPathIndex = ebInstance.sectionElementFullName.qhpCSRDependentSource.indexOf(elementPath)
        if (validElementPathIndex != -1) {
            ebInstance.DisableCSRDropDownItems(currentInstance, ebInstance);
        }

        if (currentInstance.selectedSectionName == ebInstance.sectionName.ProductRule && elementPath == ebInstance.sectionElementFullName.drpMarketSegment) {

            if (currentInstance.formData.ProductRules.PlanInformation.hasOwnProperty(ebInstance.sectionElementName.SubMarket))
                ebInstance.DisableSubMarketExchangeStatusDropDownItems(currentInstance, ebInstance, ebInstance.sectionElementName.SubMarket);

            if (currentInstance.formData.ProductRules.PlanInformation.hasOwnProperty(ebInstance.sectionElementName.ExchangeStatus))
                ebInstance.DisableSubMarketExchangeStatusDropDownItems(currentInstance, ebInstance, ebInstance.sectionElementName.ExchangeStatus);

        }

        if (currentInstance.selectedSectionName == ebInstance.sectionName.GuardrailBRG) {
            ebInstance.ShowHideRepeaterColumn(currentInstance, ebInstance, ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailBRG, ebInstance.sectionElementName.GuradrailBRGColumnGroup);
        }


        if (elementPath == currentInstance.selectedSectionName + "." + currentInstance.selectedSectionName + "ProductSelection.Select" + currentInstance.selectedSectionName + "Product") {
            ebInstance.getProductInformation(currentInstance, value);
        }
    }
}

expressionBuilderExtension.prototype.setReadOnlyDefaultValue = function (currentInstance) {
    var ebInstance = this;
    var readOnlyLength = 8;
    var $contractNumber = $("#" + ebInstance.elementIDs.contractNumber + currentInstance.formInstanceId);
    var isThisanEGWPPlan = $("#" + ebInstance.elementIDs.isThisanEGWPPlan + currentInstance.formInstanceId).val();

    if (isThisanEGWPPlan !== null && isThisanEGWPPlan !== "YES") {
        $contractNumber.on('keydown', function () {
            var which = event.which;
            if (((which == 8) && (this.selectionStart <= readOnlyLength))
                    || ((which == 46) && (input.selectionStart < readOnlyLength))) {
                event.preventDefault();
            }
        });

        $contractNumber.on('keypress', function () {
            var which = event.which;
            if ((event.which != 0) && (this.selectionStart < readOnlyLength)) {
                event.preventDefault();
            }
        });

        $contractNumber.on('cut', function () {
            if (this.selectionStart < readOnlyLength) {
                event.preventDefault();
            }
        });

        $contractNumber.on('paste', function () {
            if (this.selectionStart < readOnlyLength) {
                event.preventDefault();
            }
        });
    }
}

expressionBuilderExtension.prototype.onSectionLoad = function (currentInstance) {
    var ebInstance = this;
    var currentInstance = currentInstance;
    ebInstance.setDefaultDate(currentInstance);
    ebInstance.setReadOnlyDefaultValue(currentInstance);
    if (currentInstance.formDesignId == FormTypes.ANOCChartViewFormDesignID) {
        ebInstance.registerControlAndEventForANOCChart(currentInstance);
    }
    if (currentInstance.formDesignId == FormTypes.COMMERCIALMEDICAL) {
        if (currentInstance.selectedSectionName == "QHP Details")
            ebInstance.DisableCSRDropDownItems(currentInstance, ebInstance);
        if (currentInstance.selectedSectionName == ebInstance.sectionName.ProductRule) {
            if (currentInstance.formData.ProductRules.PlanInformation.hasOwnProperty(ebInstance.sectionElementName.SubMarket))
                ebInstance.DisableSubMarketExchangeStatusDropDownItems(currentInstance, ebInstance, ebInstance.sectionElementName.SubMarket);
            if (currentInstance.formData.ProductRules.PlanInformation.hasOwnProperty(ebInstance.sectionElementName.ExchangeStatus))
                ebInstance.DisableSubMarketExchangeStatusDropDownItems(currentInstance, ebInstance, ebInstance.sectionElementName.ExchangeStatus);

        }

        if (currentInstance.selectedSectionName == ebInstance.sectionName.GuardrailBRG) {
            ebInstance.ShowHideRepeaterColumn(currentInstance, ebInstance, ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailBRG, ebInstance.sectionElementName.GuradrailBRGColumnGroup);
        }
    }
    if (currentInstance.formDesignId == FormTypes.SBCDESIGN) {
        ebInstance.registerControlAndEventForSBCCalculator(currentInstance);
    }
}

expressionBuilderExtension.prototype.repeaterElementCellSave = function (currentInstance, element, rowId) {
    try {
        var ebInstance = this;
        var fullName = currentInstance.fullName;
        var formInstanceId = currentInstance.formInstanceId;
        var modelData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
        var modeldataRow = modelData.filter(function (dt) {
            return dt.RowIDProperty == rowId;
        });
    } catch (e) {
        console.log(ex.message);
    }
}

expressionBuilderExtension.prototype.repeaterElementOnChange = function (currentInstance, element, rowId, newValue, isEnterUniqueResponse, oldValue, event) {
    var ebInstance = this;
    var fullName = currentInstance.fullName;
    var formInstanceId = currentInstance.formInstanceId;
    var currentElementColumn = "";
    var previousElementColumn = "";
    var validColumns = ["ManualOverride", "ProcessingRule"];
    var colExcluedList = ["RefreshCoinsurance", "RefreshCoinsuranceGuardrail"];
    if (colExcluedList.indexOf(element.GeneratedName) == -1) {
        ebInstance.executeGuardrailsTree(currentInstance, element, rowId, newValue, isEnterUniqueResponse, oldValue, event);
    }
    var validColumnIndex = -1;
    if (element != undefined) {
        validColumnIndex = validColumns.indexOf(element.GeneratedName)
    }
    //CostShare Values
    switch (ebInstance.FormDesignId) {
        case FormTypes.COMMERCIALMEDICAL:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.BenefitReview:
                    if (element.GeneratedName == "ManualOverride" || element.GeneratedName == "Covered" || element.GeneratedName == "Network") {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.DentalBenefitReview:
                    if (element.GeneratedName == "ManualOverride" || element.GeneratedName == "Covered") {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.CostShareGroupList:
                    if ((element.GeneratedName == "RefreshCoinsurance") || element.GeneratedName == "CostShareType") {
                        var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
                        var rowData = rowNode.data;
                        var refreshCoinsValue = rowData[ebInstance.repeaterElementName.RefreshCoinsurance]
                        if (refreshCoinsValue == "Yes")
                            ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }

                    if (element.GeneratedName == "SameAsCostShare") {
                        var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
                        var updatedRow = rowNode.data;
                        ebInstance.setSameAsCostShareValue(ebInstance, currentInstance, rowId, updatedRow);
                    }
                    if (element.GeneratedName == "CostShareType") {
                        var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
                        ebInstance.updateFollowsTierValues(ebInstance, currentInstance, rowNode.data);
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.RxBenefitReview:
                    if (element.GeneratedName == "Network" || element.GeneratedName == "CostShareType") {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue);
                        currentInstance.runRuleOnChange(element.GeneratedName, rowId, newValue, currentInstance, false);
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.NetworkTierList:
                    if (element.GeneratedName == ebInstance.repeaterElementName.NetworkTier || element.GeneratedName == ebInstance.repeaterElementName.NetworkName) {
                        ebInstance.isSameNetworkAdded(ebInstance, currentInstance, rowId, element, newValue);
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.CoverageLevelList:
                    if (element.GeneratedName == ebInstance.repeaterElementName.CoverageName) {
                        ebInstance.IsSameCoverage(ebInstance, currentInstance, rowId, element, newValue);
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailsCostShareGroupList:
                    if ((element.GeneratedName == ebInstance.repeaterElementName.RefreshCoinsuranceGuardrail) || element.GeneratedName == ebInstance.repeaterElementName.AllowableCostShareType) {
                        var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
                        var rowData = rowNode.data;
                        var refreshCoinsValue = rowData[ebInstance.repeaterElementName.RefreshCoinsuranceGuardrail]
                        if (refreshCoinsValue == "Yes")
                            ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    break;
                    //case ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailsSlidingCostShareList:
                    //    ebInstance.ExecuteSlidingCostShareRules(ebInstance, currentInstance, rowId, element, newValue);

                    //    break;
                    //case ebInstance.reapterFullName.COMMERCIALMEDICAL.SlidingCostShareList:
                    //    ebInstance.ExecuteSlidingCostShareRules(ebInstance, currentInstance, rowId, element, newValue);
                    //    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailsBenefitReview:
                    if (element.GeneratedName == "ManualOverride") {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    if (element.GeneratedName == ebInstance.repeaterElementName.AllowableCostShareType) {		
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue);		
                        currentInstance.runRuleOnChange(element.GeneratedName, rowId, newValue, currentInstance, false);		
                        }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.BenefitReviewGridSlidingCostShare:
                   if (element.GeneratedName == ebInstance.repeaterElementName.CostShareType) {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue);
                        currentInstance.runRuleOnChange(element.GeneratedName, rowId, newValue, currentInstance, false);
                   }
                   if (element.GeneratedName == ebInstance.repeaterElementName.ManualOverride) {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue);
                        currentInstance.runRuleOnChange(element.GeneratedName, rowId, newValue, currentInstance, false);
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.SlidingCostShareGuardrail:
                    if (element.GeneratedName == ebInstance.repeaterElementName.RefreshCoinsuranceGuardrail) {
                        var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
                        var rowData = rowNode.data;
                        var refreshCoinsValue = rowData[ebInstance.repeaterElementName.RefreshCoinsuranceGuardrail]
                        if (refreshCoinsValue == "Yes")
                            ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)

                    }
                    if (element.GeneratedName == ebInstance.repeaterElementName.AllowableCostShareType) {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue);
                        currentInstance.runRuleOnChange(element.GeneratedName, rowId, newValue, currentInstance, false);
                    }

                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.SlidingCostShareList:
                    if (element.GeneratedName == "RefreshCoinsurance") {
                        var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
                        var rowData = rowNode.data;
                        var refreshCoinsValue = rowData[ebInstance.repeaterElementName.RefreshCoinsurance]
                        if (refreshCoinsValue == "Yes")
                            ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    if (element.GeneratedName == "CostShareType") {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue);
                        currentInstance.runRuleOnChange(element.GeneratedName, rowId, newValue, currentInstance, false);
                    }
                    break;
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailBenefitReviewSlidingCostShare:
                    if (element.GeneratedName == ebInstance.repeaterElementName.AllowableCostShareType) {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue);
                        currentInstance.runRuleOnChange(element.GeneratedName, rowId, newValue, currentInstance, false);
                    }
                  if (element.GeneratedName == "ManualOverride") {
                   ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)

                    }
                    
                    break;

                case ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailsBenefitReviewGrid:
                    if ((element.GeneratedName == ebInstance.repeaterElementName.RefreshCoinsuranceGuardrail) || element.GeneratedName == ebInstance.repeaterElementName.AllowableCostShareType) {
                        var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
                        var rowData = rowNode.data;
                        var refreshCoinsValue = rowData[ebInstance.repeaterElementName.RefreshCoinsuranceGuardrail]
                        if (refreshCoinsValue == "Yes")
                            ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    break;
             

            }
            var isSameAsNetworkTier = currentInstance.columnNames.filter(function (dt) {
                return dt.dataIndx == ebInstance.KeyName.SameAsNetworkTier;
            })

            if (isSameAsNetworkTier.length > 0) {
                ebInstance.tiersToFollows(currentInstance, event.data, oldValue, newValue);
            }

            //case FormTypes.SBCDESIGN:
            //    switch (currentInstance.fullName) {
            //        case ebInstance.reapterFullName.SBCCalculator:
            //            previousElementColumn = ebInstance.repeaterElementName.PreviousProcessingRule
            //            break;
            //    }
            break;
        case FormTypes.RX:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.RX.RxBenefitReview:
                    if (element.GeneratedName == "Network" || element.GeneratedName == "CostShareType") {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    break;

            }
        case FormTypes.DENTAL:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.DENTAL.DentalBenefitReview:
                    if (element.GeneratedName == "ManualOverride" || element.GeneratedName == "Covered") {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    break;

            }
        case FormTypes.PlansandBenefitTemplate:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.PlansandBenefitTemplate.GeneralInformation:
                    if (element.GeneratedName == "IsCovered") {
                        ebInstance.getEBRowResult(ebInstance, currentInstance, rowId, element, newValue)
                    }
                    break;
            }
    }
}

expressionBuilderExtension.prototype.repeaterBeforeRowAdded = function (currentInstance, newRow) {
    try {
        var ebInstance = this;
        var fullName = currentInstance.fullName;
        var formInstanceId = currentInstance.formInstanceId;

    } catch (e) {
        console.log(ex.message);
    }
}

expressionBuilderExtension.prototype.getFormatterForGrid = function (colMod, ui, fullName) {
    var ebInstance = this;
    try {
        if (ui) {
            var cellvalue = ui.cellData;
            var rowData = ui.rowData;
            switch (ebInstance.FormDesignId) {

                case FormTypes.COMMERCIALMEDICAL:
                    switch (fullName) {
                        case "BenefitReview.BenefitReviewGrid.Limits":
                        case "BenefitReview.BenefitReviewSlidingCostShare.BenefitReviewGridSlidingCostShare.Limits":
                            var span = "";
                            if (rowData != undefined && rowData != null) {
                                colMod.editable = false;
                                span = "<span class='ui-icon ui-icon-document viewCOMMERCIALBRGLimits' data-value='" + cellvalue + "' title='View " + ui.dataIndx + "' style='margin:0 auto!important;cursor: pointer;' " +
                                            "id='" + ui.rowData.RowIDProperty + "' " + "index='" + ui.dataIndx + "'></span>"


                            }
                            return span;
                            break;
                    }
                    break;
            }

        }
    } catch (e) {
        console.log(e + "getFormattersForBenefirReviewGrid");
    }
}

expressionBuilderExtension.prototype.manualdataSourceSaveButtonClick = function (currentInstance, data, oldData, sourceDataList) {
    var ebInstance = this;
    switch (ebInstance.FormDesignId) {
        case FormTypes.ANOCChartViewFormDesignID:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare:
                    ebInstance.additionalANOCChartServicesOnPopupSave(currentInstance, data, oldData);
                    break;
            }
            break;
        case FormTypes.COMMERCIALMEDICAL:
            switch (currentInstance.fullName) {
                case "Limits.LimitSummary":
                    ebInstance.populateLimitInformation(currentInstance, data, oldData, sourceDataList);
                    break;
            }
            break;      
    }
}

expressionBuilderExtension.prototype.onCellClick = function (currentInstance, ui) {
    var ebInstance = this;
    ui.dataIndx = ui.column.colId;
    if ((currentInstance.fullName.toUpperCase().indexOf('GUARDRAILS') != -1)||(currentInstance.fullName.toUpperCase().indexOf('GUARDRAIL') != -1)) {
        if (null != ui) {
            if (ui.dataIndx.indexOf('Allowable') != -1 || ui.dataIndx.indexOf('Standard') != -1 || ui.dataIndx.indexOf('Guardrail') != -1) {

        var isDisabled = currentInstance.cellRules.cellEnableStyle.filter(function (dt) {
            if (dt.rowIndex == ui.rowIndex && dt.colId == ui.column.colId) {
            return dt;
        }
        });
        if (isDisabled==undefined || isDisabled.length==0){
            return ebInstance.generateGuardrailsOptionsForRepeater(currentInstance, ui.node.data, ui.dataIndx);
        }
            }
        }
    }
    else {
        switch (ebInstance.FormDesignId) {
            case FormTypes.COMMERCIALMEDICAL:
                switch (currentInstance.fullName) {
                    case ebInstance.reapterFullName.COMMERCIALMEDICAL.SelectProvisions:
                    case ebInstance.reapterFullName.COMMERCIALMEDICAL.BRAGSelectProvisions:
                    case ebInstance.reapterFullName.COMMERCIALMEDICAL.NetworkSelectProvisions:
                    case ebInstance.reapterFullName.COMMERCIALMEDICAL.SelectHSAProvisions:
                        if (null != ui && ui.dataIndx == ebInstance.repeaterElementName.ProvisionTextOptions) {
                            return ebInstance.bindProviderOptionDropdown(currentInstance, ui.node.data);
                        }
                        break;
                    case ebInstance.reapterFullName.COMMERCIALMEDICAL.SpecialServiceSelectionRepeater:
                        if (null != ui && ui.dataIndx == currentInstance.customrule.KeyName.BenefitCategory3) {
                            return ebInstance.runExpressionRuleOnCellClick(currentInstance, ui.node.data, ui.dataIndx);
                        }
                        break;
                    case ebInstance.reapterFullName.COMMERCIALMEDICAL.ProviderNetworkMap:
                        if (currentInstance.columnName == ebInstance.KeyName.NetworkTier) {
                            ebInstance.filterNetworkTier(currentInstance, ui);
                        }
                        break;
                }

                if (currentInstance.columnName == ebInstance.KeyName.SameAsNetworkTier) {
                    ebInstance.filterDropdownSameAsTier(currentInstance, ui);
                }
                if (currentInstance.columnName == ebInstance.KeyName.SameAsCostShare) {
                    ebInstance.filterDropdownSameAsCostShare(currentInstance, ui);
                }

                if (currentInstance.fullName == ebInstance.reapterFullName.LimitInformationDetail) {
                    if (currentInstance.columnName == ebInstance.KeyName.Combined || currentInstance.columnName == ebInstance.KeyName.Accums) {
                        ebInstance.filterCombinedAccumDropdown(currentInstance, ui, ebInstance);
                    }
                    if (null != ui && ui.dataIndx == ebInstance.KeyName.LimitAdditionalDetails || ui.dataIndx == ebInstance.KeyName.Limit2AdditionalDetails) {
                        return ebInstance.runExpressionRuleOnCellClick(currentInstance, ui.node.data, ui.dataIndx);
                    }
                }

                if (currentInstance.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.AllowableLimitsList && currentInstance.columnName == ebInstance.KeyName.AllowedLimits && ui.data.AllowedLimits == "Yes") {
                    ebInstance.alertLimitInformation(currentInstance, ui, ebInstance);
                }

                break;
            case FormTypes.LIMITSML:
                switch (currentInstance.fullName) {
                    case ebInstance.reapterFullName.LIMITSML.LimitDetailsList:
                        if (currentInstance.fullName == ebInstance.reapterFullName.LIMITSML.LimitDetailsList) {
                            if (null != ui && ui.dataIndx == ebInstance.KeyName.LimitAdditionalDetails || ui.dataIndx == ebInstance.KeyName.Limit2AdditionalDetails) {
                                return ebInstance.runExpressionRuleOnCellClick(currentInstance, ui.node.data, ui.dataIndx);
                            }
                        }
                }
                break;
            case FormTypes.RX:
                switch (currentInstance.fullName) {
                    case ebInstance.reapterFullName.RX.RxSelectProductWideProvisions:
                    case ebInstance.reapterFullName.RX.RxSelectProductNetworkWideProvisions:
                    case ebInstance.reapterFullName.RX.RxSelectServiceNetworkWideProvisions:
                        if (null != ui && ui.dataIndx == ebInstance.repeaterElementName.ProvisionTextOptions) {
                            return ebInstance.bindProviderOptionDropdown(currentInstance, ui.node.data);
                        }
                        break;
                }
                break;
            case FormTypes.DENTAL:
                switch (currentInstance.fullName) {
                    case ebInstance.reapterFullName.DENTAL.DentalSelectProductWideProvisions:
                    case ebInstance.reapterFullName.DENTAL.DentalSelectServiceWideProvisions:
                        if (null != ui && ui.dataIndx == ebInstance.repeaterElementName.ProvisionTextOptions) {
                            return ebInstance.bindProviderOptionDropdown(currentInstance, ui.node.data);
                        }
                        break;
                }
                break;

            case FormTypes.SBCDESIGN:
                switch (currentInstance.fullName) {
                    case ebInstance.reapterFullName.SBCCalculatorView.CoverageExampleDiabetic:
                    case ebInstance.reapterFullName.SBCCalculatorView.CoverageExampleFracture:
                    case ebInstance.reapterFullName.SBCCalculatorView.CoverageExampleMaternity:
                        if (null != ui && ui.dataIndx == ebInstance.repeaterElementName.ProcessingRule) {
                            return ebInstance.filterProcesssingRuleItems(currentInstance, ui.node.data);
                        }
                        break;
                }
                break;
        }

    }
}

expressionBuilderExtension.prototype.getDropDownValues = function (items) {
    var options = "";
    if (items != null && items.length > 0) {
        options = options + "" + ':' + Validation.selectOne + ';';
        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx] != '') {
                options = options + items[idx] + ':' + items[idx];
                if (idx < items.length - 1) {
                    options = options + ';';
                }
            }
        }
    }
    return options;
}
expressionBuilderExtension.prototype.setDefaultDate = function (currentInstance) {
    var ebInstance = this;

    var SectionName = "";
    if (currentInstance.selectedSectionName == undefined) {
        if (currentInstance.designData.Sections.length > 0) {
            SectionName = currentInstance.designData.Sections[0].Label;
        }
    }
    else {
        SectionName = currentInstance.selectedSectionName;
    }

    if (currentInstance.designData.FormDesignId == FormTypes.MEDICAREFORMDESIGNID && SectionName == ebInstance.sectionName.SECTIONASECTIONA1) {
        if (currentInstance.folderData.isEditable.toLowerCase() == "true") {
            var effdate = new Date(currentInstance.folderData.effectiveDate);
            var year = effdate.getFullYear() + "";
            var month = (effdate.getMonth() + 1) <= 9 ? "0" + (effdate.getMonth() + 1) : (effdate.getMonth() + 1) + "";
            var day = effdate.getDate() <= 9 ? "0" + effdate.getDate() : effdate.getDate();
            var effectiveFormatedDate = month + "/" + day + "/" + year;
            if ($(ebInstance.elementIDs.effectiveDate + currentInstance.formInstanceId).val() == "" || $(ebInstance.elementIDs.effectiveDate + currentInstance.formInstanceId).val() != effectiveFormatedDate) {
                $(ebInstance.elementIDs.effectiveDate + currentInstance.formInstanceId).val(effectiveFormatedDate);
                $(ebInstance.elementIDs.effectiveDate + currentInstance.formInstanceId).trigger('change');
            }
            if ($(ebInstance.elementIDs.termDate + currentInstance.formInstanceId).val() == "" || $(ebInstance.elementIDs.termDate + currentInstance.formInstanceId).val() != "12/31/" + year) {
                $(ebInstance.elementIDs.termDate + currentInstance.formInstanceId).val("12/31/" + year);
                $(ebInstance.elementIDs.termDate + currentInstance.formInstanceId).trigger('change');
            }
        }
    }
}

var expressionBuilderRulesExt = function () {
    this.URLs = {
        ElementChangeRuleUrl: "/ExpressionBuilder/ProcessEBSRuleForSameSection",
        ElementChangeRulePackageUrl: "/ExpressionBuilder/ProcessRuleForElement"
    }
    this.expressionBuilderConstant = {
        IsSameSectionRuleSource: "IsSameSectionRuleSource"
    }
}

expressionBuilderRulesExt.prototype.hasRule = function (elementData, val) {

    if (elementData.hasOwnProperty("design")) {
        elementData = elementData.design;
    }

    if (elementData.hasOwnProperty(this.expressionBuilderConstant.IsSameSectionRuleSource)) {
        if (elementData.IsSameSectionRuleSource == true) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        return false;
    }
}

expressionBuilderRulesExt.prototype.processRule = function (elementData, val, formDesignVersionId, folderVersionId, formInstanceId, formData, currentInstanceObj) {
    var multiSelectFlag = elementData.MultiSelect == undefined ? false : elementData.MultiSelect;
    var currentSectionData;
    if (currentInstanceObj.formInstanceBuilder != undefined) {
        var formInstanceData = currentInstanceObj.formInstanceBuilder.form.getFormInstanceData();
        currentSectionData = formInstanceData.formInstanceData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');
    }
    else {
        var path = elementData.FullName.split('.');
        if (path.length > 0) {
            currentSectionData = JSON.stringify(formData[path[0]]);
        }
    }
    var input = {
        formDesignVersionId: formDesignVersionId,
        folderVersionId: folderVersionId,
        formInstanceId: formInstanceId,
        sourceElementPath: "",
        ElementValue: "",
        ElementJSONPath: "",
        isMultiselect: multiSelectFlag,
        sectionData: currentSectionData
    }


    if (elementData.hasOwnProperty("design")) {

        input.ElementValue = JSON.stringify(elementData.data);
        input.sourceElementPath = elementData.design.FullName;
        input.ElementJSONPath = elementData.design.FullName;
    }
    else {
        input.ElementValue = elementData.MultiSelect && val.value != null ? val.value.slice(0, val.value.length) : val.value;
        input.sourceElementPath = elementData.FullName;
        input.ElementJSONPath = elementData.FullName
    }

    var promise = ajaxWrapper.postJSON(this.URLs.ElementChangeRuleUrl, input, true);
    promise.done(function (xhr) {
        try {
            if (xhr != null) {
                if (xhr != "[]") {
                    var res = JSON.parse(xhr);
                    $.each(res, function (idx, val) {
                        var obj = {};
                        obj[val.TargetPath] = val.Data;
                        $.observable(formData).setProperty(obj);
                        var reptrInstance = null;
                        if (elementData.hasOwnProperty("design")) {
                            reptrInstance = currentInstanceObj.formInstanceBuilder.repeaterBuilders.filter(function (dt) {
                                return dt.fullName == val.TargetPath;
                            })
                            $.each(currentInstanceObj.formInstanceBuilder.repeaterBuilders, function (index, value) {
                                if (value.fullName == val.TargetPath) {
                                    if (val.Data != null) {
                                        for (var i = 0; i < val.Data.length; i++) {
                                            val.Data[i]["RowIDProperty"] = i;
                                        }
                                    }
                                    else {
                                        val.Data = [];
                                    }
                                    value.data = val.Data;
                                }
                            });
                        }
                        else {

                            reptrInstance = currentInstanceObj.repeaterBuilders.filter(function (dt) {
                                return dt.fullName == val.TargetPath;
                            })
                            $.each(currentInstanceObj.repeaterBuilders, function (index, value) {
                                if (value.fullName == val.TargetPath) {
                                    if (val.Data != null) {
                                        for (var i = 0; i < val.Data.length; i++) {
                                            val.Data[i]["RowIDProperty"] = i;
                                        }
                                    }
                                    else {
                                        val.Data = [];
                                    }
                                    value.data = val.Data;
                                }
                            });
                        }
                        if (reptrInstance.length != 0) {
                            if (GridDisplayMode.PQ == CurrentGridDisplayMode) {
                                var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                                dataModel.data = val.Data;//JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                                $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");
                            }
                            //refresh AG grid
                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = val.Data;
                            gridOptions.api.setRowData(gridOptions.rowData);
                            //gridOptions.api.sizeColumnsToFit();
                            //currentInstanceObj.formInstanceBuilder.formData[val.TargetPath] =val.Data;

                            if (GridDisplayMode.AG == CurrentGridDisplayMode) {
                                var gridOptions = GridApi.getCurrentGridInstance(reptrInstance[0]).gridOptions;
                                gridOptions.api.setRowData(val.Data);
                                gridOptions.api.setFilterModel(null);
                                try {
                                    reptrInstance[0].expressionBuilderRulesExt.runRulesOnDisplayRows(reptrInstance[0]);
                                }
                                catch (ex) {

                                }
                                //reptrInstance[0].executeAllRules(true);

                                if (elementData.fullName == reptrInstance[0].fullName) {
                                    var rowNodeFound = {};
                                    reptrInstance[0].gridOptions.api.forEachNode(function (rowNode, index) {
                                        if (rowNode.data.RowIDProperty == elementData.lastSelectedRow) {
                                            isRowIDExist = index;
                                            rowNodeFound = rowNode;
                                            return;
                                        }
                                    });
                                    if (rowNodeFound) {
                                        var colindex = undefined;
                                        var col = reptrInstance[0].colModel;
                                        $.each(col, function (idx, ct) {
                                            if (ct.dataIndx == elementData.columnName) {
                                                colindex = idx;
                                                return false;
                                            }
                                        });

                                        GridApi.setCellFocus(reptrInstance[0], rowNodeFound, elementData.columnName, colindex);
                                    }
                                }
                            }
                        }
                        else {
                            var elemt = $("[data-journal='" + val.TargetPath + "']");
                            if (elemt != undefined && elemt.length > 0) {
                                var elemetid = elemt[0].id;
                                if (elemt[0].type == 'radio') {
                                    var result = false;
                                    if (val.Data == "true") {
                                        result = true;
                                    }
                                    $('#' + elemetid).prop('checked', result);
                                }
                                else {
                                    $('#' + elemetid).val(val.Data);
                                }
                                $('#' + elemetid).trigger('change');
                            }
                        }
                    });
                }
            }
        }
        catch (ex) {
            console.log(ex.message);
        }
    });
}

expressionBuilderRulesExt.prototype.runRulesOnDisplayRows = function (currentInstance) {
    var targetData = currentInstance.data;
    var excludeList = ["RowIDProperty", "expandCollapse"];
    if (targetData != null && targetData.length > 0) {
        data = currentInstance.ruleProcessor.getDisplayedRow(currentInstance.gridOptions);
        for (var idx = 0; idx < data.length; idx++) {
            for (var colInx = 0; colInx < currentInstance.colModel.length; colInx++) {
                var colName = currentInstance.colModel[colInx].dataIndx;
                if (excludeList.indexOf(colName) == -1) {
                    currentInstance.runRuleOnChange(colName, idx, data[idx][colName], currentInstance, false);
                }
            }
        }
    };

}

expressionBuilderRulesExt.prototype.bindFieldValue = function (path, formData, sectionData) {
    try {
        var dataPart = getDataProperty(path, sectionData);
        var obj = {};
        obj[path] = dataPart;
        $.observable(formData).setProperty(obj);
    }
    catch (ex) { }
}

expressionBuilderRulesExt.prototype.getMultiSelectKeyArray = function (values) {
    var keyArray = new Array();

    var excludeList = ["Deductible, Copay, then Coinsurance", "Copay, Deductible, then Coinsurance"];

    if (values != undefined && values != null) {
        if (values.indexOf('$') != -1) {
            keyArray = values.split(",$");
            $.each(keyArray, function (ind, val) {
                if (val.indexOf('$') == -1) {
                    keyArray[ind] = "$" + val;
                }
            });
        }
        else if (values.indexOf('%') != -1) {
            Default = "%,";
            keyArray = values.split("%,");
            $.each(keyArray, function (ind, val) {
                if (val.indexOf('%') == -1) {
                    keyArray[ind] = val + "%";
                }
            });
        }
        else {
            $.each(excludeList, function (ind, item) {
                if (values.indexOf(item) != -1) {
                    values = values.replace(item, "");
                    keyArray.push(item);
                }
            });

            if (values != undefined && values != "") {
                if (values.charAt(0) == ',') {
                    values = values.slice(1);
                }
                keyArray = keyArray.concat(values.split(","));
            }
        }
    }
    return keyArray;
}

expressionBuilderExtension.prototype.OpenFileUploadDialog = function (currentInstance, operationType, selectedRow) {
    var URLs = {
        UploadProofingDocuments: "/FolderVersion/UploadProofingDocuments?formInstanceId={FormInstanceId}",
        getDatabaseNameList: "/PBPImport/GetPBPDatabaseNameList"
    };

    var elementIDs = {
        uploadProofingDocumentDialog: "#uploadProofingDocumentDialog",
        dropDownProofingRound: '#proofingRound',
        dropDownProofingRoundStatus: '#proofingRoundStatus',
        btnUploadDoc: '#btnUploadDoc',
        divUploadDocumentArea: '#divUploadDocumentArea',
        btnCancelUpload: '#btnCancelUpload',
        btnAddRow: '#btnAddProofingTemplate',
        uploadProofingDoc: '#UploadProofingDoc-',
        documentType: '#documentType-',
        documentName: '#documentName-',
        proofingCommments: '#proofingCommments',
        uploadDocumentTemplate: '#uploadDocumentTemplate-',
        ChangeUploadProofingDoc: '#ChangeUploadProofingDoc',
        UploadProofingDocSpan: '#UploadProofingDocSpan-',
        documentNameHelpBlock: '#documentNameHelpBlock-',
        documentNameSection: '#documentNameSection',
        sharePointSection: '#sharePointSection',
        uploadDocumentSection: '#uploadDocumentSection',
        uploadDocumentAddSection: '#uploadDocumentAddSection',
        btnpbpSharePointLink: '#btnpbpSharePointLink'
    };

    $(elementIDs.uploadProofingDocumentDialog).dialog({
        autoOpen: false,
        height: '750',
        width: '85%',
        modal: true,
        position: 'center',
        close: function (event, ui) {
        }
    });

    var ebInstance = this;
    //var numberOfFiles = 1;
    var totalRounds = [];
    var _roundDropdown = $('<select>');
    var documentUploadTemplateIndices = [1];
    var documentUploadTemplateIndex = 1;
    totalRounds.push('Select  One');
    for (var roundNum = 1 ; roundNum <= 10 ; roundNum++)
        totalRounds.push("Round " + roundNum);

    $.each(totalRounds, function (index, value) {
        var option = $('<option></option>').val(value).html(value);
        _roundDropdown.append(option);
    });
    $(elementIDs.dropDownProofingRound).html(_roundDropdown.html());

    var roundStatus = ['Select  One', 'Pass', 'Fail'];
    var _roundStatusDropdown = $('<select>');
    $.each(roundStatus, function (index, value) {
        var option = $('<option></option>').val(value).html(value);
        _roundStatusDropdown.append(option);
    });

    $(elementIDs.dropDownProofingRoundStatus).html(_roundStatusDropdown.html());
    $(elementIDs.ChangeUploadProofingDoc).text('');
    $(elementIDs.uploadProofingDoc + '1').val('');
    $(elementIDs.uploadProofingDoc + '1').parent().removeClass('has-error');
    $(elementIDs.dropDownProofingRound).parent().removeClass('has-error');
    $(elementIDs.documentName + '1').parent().removeClass('has-error');
    $(elementIDs.documentType + '1').parent().removeClass('has-error');
    $(elementIDs.UploadProofingDocSpan + '1').text('');
    $(elementIDs.documentNameHelpBlock + '1').text('');
    $(elementIDs.sharePointSection + "-1").hide();
    var sharePointLink = DashBoard.PBPSharePointLink;

    if (operationType == 'EDIT') {
        //$(elementIDs.dropDownProofingRoundStatus).removeAttr('disabled');
        $(elementIDs.dropDownProofingRound).attr('disabled', 'disabled');
        $(elementIDs.btnAddRow).css('visibility', 'hidden');

        $(elementIDs.documentType + '1').val(selectedRow.ProofingDocument);
        $(elementIDs.documentName + '1').val(selectedRow.DocumentName);
        if (selectedRow.ProofingDocument == "PBP Template") {
            sharePointLink = selectedRow.ClientDocumentPath;
            $(elementIDs.documentNameSection + "-1").hide();
            $(elementIDs.uploadDocumentSection + "-1").hide();
            $(elementIDs.sharePointSection + "-1").show();
        }
        else {
            $(elementIDs.documentNameSection + "-1").show();
            $(elementIDs.uploadDocumentSection + "-1").show();
            $(elementIDs.sharePointSection + "-1").hide();
            if (selectedRow.ClientDocumentPath != '')
                $(elementIDs.ChangeUploadProofingDoc).text('Old File -> ' + selectedRow.ClientDocumentPath);
        }
        $(elementIDs.proofingCommments).val(selectedRow.Comments);
        $(elementIDs.dropDownProofingRound).val(selectedRow.Round);
        if (selectedRow.RoundStatus == "")
            selectedRow.RoundStatus = 'Select  One';
        $(elementIDs.dropDownProofingRoundStatus).val(selectedRow.RoundStatus);
    }
    else {
        //$(elementIDs.dropDownProofingRoundStatus).attr('disabled', 'disabled');
        $(elementIDs.dropDownProofingRound).removeAttr('disabled');
        $(elementIDs.btnAddRow).css('visibility', 'visible');

        $(elementIDs.documentType + '1').val(0);
        $(elementIDs.documentName + '1').val('');
        $(elementIDs.proofingCommments).val('');
    }

    $(elementIDs.divUploadDocumentArea).html('');
    $(elementIDs.uploadProofingDocumentDialog).dialog('option', 'title', "Proofing Attachment");
    $(elementIDs.uploadProofingDocumentDialog).dialog("open");

    //$(".ui-icon-trash").unbind().on('click', function () {
    //    var element = $(this).attr("id").split('-');
    //    var templateIndex = parseInt(element[1]);
    //    $(elementIDs.uploadDocumentTemplate + templateIndex).html('');
    //    var index = documentUploadTemplateIndices.indexOf(templateIndex);
    //    documentUploadTemplateIndices.splice(index, 1);
    //});

    $(elementIDs.btnAddRow).unbind().on('click', function () {
        documentUploadTemplateIndex++;

        $(elementIDs.divUploadDocumentArea)
        .append("<div class='row' id='uploadDocumentTemplate-" + documentUploadTemplateIndex + "'><div class='col-sm-2'><label>Document Type:</label>" +
                        "<select id='documentType-" + documentUploadTemplateIndex + "' name='documentType' class='DocumentType'>" +
                            "<option value='0'>Select  One</option>" +
                            "<option value='PBP Template'>PBP Template</option>" +
                            "<option value='SOT File'>SOT File</option>" +
                            "<option value='PBP Report'>PBP Report</option>" +
                        "</select>" +
                        "<span class='help-block' id='documentTypeSpan' style='color:red'></span>" +
                    "</div>" +
                    "<div class='col-sm-4'>" +
                        "<div id='documentNameSection-" + documentUploadTemplateIndex + "'>" +
                           "<label for='documentName'>Name: </label>" +
                           "<input id='documentName-" + documentUploadTemplateIndex + "' type='text' class='form-control' name='description' maxlength='150' />" +
                           "<span class='help-block' id='documentNameHelpBlock-" + documentUploadTemplateIndex + "' style='color:red'></span>" +
                        "</div>" +
                        "<div id='sharePointSection-" + documentUploadTemplateIndex + "'>" +
                           "<button id='btnpbpSharePointLink' type='button' class='btn pull-left PBPSharePointLink' style='margin-top:5.5%;margin-right:1%'>View PBP Templates</button>" +
                        "</div>" +
                    "</div>" +
                    "<div class='col-sm-5'>" +
                      "<div id='uploadDocumentSection-" + documentUploadTemplateIndex + "'>" +
                        "<label for='UploadProofingDoc'></label>" +
                        "<input id='UploadProofingDoc-" + documentUploadTemplateIndex + "' name='UploadProofingDoc' class='form-control UploadProofingDoc' type='file' />" +
                        "<span class='help-block' id='UploadProofingDocSpan-" + documentUploadTemplateIndex + "' style='color:red'></span>" +
                      "</div>" +
                    "</div>" +
                    "<div class='col-sm-1'>" +
                        "<label for='UploadProofingDoc'></label>" +
                        "<span id='deleteTemplate-" + documentUploadTemplateIndex + "' class='ui-icon ui-icon-trash'></span>" +
                    "</div>" +
                "</div>");

        documentUploadTemplateIndices.push(documentUploadTemplateIndex);
        $(elementIDs.sharePointSection + "-" + documentUploadTemplateIndex).hide();
        $(".ui-icon-trash").unbind().on('click', function () {
            var element = $(this).attr("id").split('-');
            var templateIndex = parseInt(element[1]);
            $(elementIDs.uploadDocumentTemplate + templateIndex).html('');
            var index = documentUploadTemplateIndices.indexOf(templateIndex);
            documentUploadTemplateIndices.splice(index, 1);
        });

        $(".UploadProofingDoc").unbind().on('change', function () {
            var fileUploadControl = this;
            var templateIndex = fileUploadControl.id.split('-')[1];
            if (fileUploadControl.value.length == 0) { $(elementIDs.documentName + templateIndex).val(''); }
            else
            {
                var existingName = $(elementIDs.documentName + templateIndex).val();
                if (existingName = "PBP Template") {
                    existingName = "";
                }
                if (existingName != undefined) {
                    if (existingName.length == 0) {
                        var dynamicFile = $(elementIDs.uploadProofingDoc + templateIndex).get(0);
                        if (dynamicFile != undefined && dynamicFile != null) {
                            var uploadedFile = dynamicFile.files;
                            if (uploadedFile.length > 0)
                                $(elementIDs.documentName + templateIndex).val(uploadedFile[0].name.split('.')[0]);
                        }
                    }
                }
            }
        });

        $(".PBPSharePointLink").unbind().on('click', function () {
            var selectedObject = this;
            var templateIndex = selectedObject.id.split('-')[1];

            if (sharePointLink != null && sharePointLink != undefined && sharePointLink != "") {
                FolderLockAction.ISREPEATERACTION = 1;
                window.open(sharePointLink);
            }
        });

        $(".DocumentType").unbind().on('change', function () {
            var selectedObject = this;
            var valueSelected = this.value;
            var templateIndex = selectedObject.id.split('-')[1];
            if (valueSelected == "PBP Template") {
                $(elementIDs.documentNameSection + "-" + templateIndex).hide();
                $(elementIDs.uploadDocumentSection + "-" + templateIndex).hide();
                $(elementIDs.sharePointSection + "-" + templateIndex).show();
                var url = '/FolderVersion/GetPBPDocumentsSharePointLink';
                var promise = ajaxWrapper.getJSON(url);
                promise.done(function (list) {
                    if (list != null && list != undefined && list != "") {
                        sharePointLink = list;
                    }
                });
                promise.fail(showError);
            }
            else {
                $(elementIDs.documentNameSection + "-" + templateIndex).show();
                $(elementIDs.uploadDocumentSection + "-" + templateIndex).show();
                $(elementIDs.sharePointSection + "-" + templateIndex).hide();
            }
        });
    });

    $(elementIDs.btnCancelUpload).unbind().on('click', function () {
        $(elementIDs.uploadProofingDocumentDialog).dialog("close");
    });

    $(".UploadProofingDoc").unbind().on('change', function () {
        var fileUploadControl = this;
        var templateIndex = fileUploadControl.id.split('-')[1];
        if (fileUploadControl.value.length == 0) { $(elementIDs.documentName + templateIndex).val(''); }
        else
        {
            var existingName = $(elementIDs.documentName + templateIndex).val();
            if (existingName = "PBP Template") {
                existingName = "";
            }
            if (existingName != undefined) {
                if (existingName.length == 0) {
                    var dynamicFile = $(elementIDs.uploadProofingDoc + templateIndex).get(0);
                    if (dynamicFile != undefined && dynamicFile != null) {
                        var uploadedFile = dynamicFile.files;
                        if (uploadedFile.length > 0)
                            $(elementIDs.documentName + templateIndex).val(uploadedFile[0].name.split('.')[0]);
                    }
                }
            }
        }
    });

    $(elementIDs.btnUploadDoc).unbind().on('click', function () {
        var validationMessages = [];
        var data = documentUploadTemplateIndices;
        var proofingCommments = $(elementIDs.proofingCommments)[0].value;
        var newRowData = [];
        var isRoundValid = true;
        var isDocumentTypeValid = true;
        var isDocumentNameValid = true;
        var isFileValid = true;

        var fileInfo = [];
        var fileData = new FormData();

        var proofingRoundSelected = $(elementIDs.dropDownProofingRound + ' option:selected').text();
        if (proofingRoundSelected == 'Select  One') {
            $(elementIDs.dropDownProofingRound).parent().addClass('has-error');
            $(elementIDs.dropDownProofingRound).addClass('form-control');
            isRoundValid = false;
        }
        else {
            $(elementIDs.dropDownProofingRound).parent().removeClass('has-error');
            isRoundValid = true;
        }

        for (var fileIndex = 0 ; fileIndex < documentUploadTemplateIndices.length ; fileIndex++) {
            //var errorMsg = "";
            var fileToUpload = $(elementIDs.uploadProofingDoc + documentUploadTemplateIndices[fileIndex]).get(0);
            var documentType = $(elementIDs.documentType + documentUploadTemplateIndices[fileIndex] + ' option:selected').text();
            var documentName = $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex])[0].value;
            var file = fileToUpload.files;
            var fileName = "";
            if (file.length > 0) {
                fileData.append(file[0].name, file[0]);
                fileName = file[0].name;
            }
            if (documentType == "Select  One") {
                $(elementIDs.documentType + documentUploadTemplateIndices[fileIndex]).parent().addClass('has-error');
                $(elementIDs.documentType + documentUploadTemplateIndices[fileIndex]).addClass('form-control');
                isDocumentTypeValid = false;
            }
            else {
                $(elementIDs.documentType + documentUploadTemplateIndices[fileIndex]).parent().removeClass('has-error');
                isDocumentTypeValid = true;
            }
            // Add contion to eliminate validaton for File name and document name for document Type is PBP Template , as this files has SP link
            if (documentType == "PBP Template") {
                $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).parent().removeClass('has-error');
                isDocumentNameValid = true;
            }
            else {
                if (documentName == "") {
                    $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).parent().addClass('has-error');
                    $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).addClass('form-control');
                    isDocumentNameValid = false;
                }
                else {
                    $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).parent().removeClass('has-error');
                    isDocumentNameValid = true;
                }
            }
            var isValidFilePath = true;
            // Add contion to eliminate validaton for File name and document name for document Type is PBP Template , as this files has SP link
            if (documentType == "PBP Template") {
                $(elementIDs.uploadProofingDoc + documentUploadTemplateIndices[fileIndex]).parent().removeClass('has-error');
                isFileValid = true;
                documentName = "PBP Template";
                docPath = sharePointLink;
            }
            else {
                if ($(elementIDs.ChangeUploadProofingDoc).text() == '')
                    isValidFilePath = false;
                else {
                    if ($(elementIDs.ChangeUploadProofingDoc).text().split('->')[1] == undefined)
                        isValidFilePath = false;
                }

                var docPath = file.length == 0 ? $(elementIDs.ChangeUploadProofingDoc).text().split('->')[1] : fileName;
                //if (file.length == 0 && isValidFilePath == false) {
                if (docPath != "" && docPath != undefined) {
                    var allowedExtensions = ["doc", "docx", "pdf", "xls", "xlsx"];

                    var fName = docPath.split('.');
                    if ($.inArray(fName[fName.length - 1], allowedExtensions) == -1) {
                        $(elementIDs.UploadProofingDocSpan + documentUploadTemplateIndices[fileIndex]).text('Only Excel, Word and PDF files are allowed!');
                        isValidFilePath = false;
                        return false;
                    }
                    else {
                        $(elementIDs.UploadProofingDocSpan + documentUploadTemplateIndices[fileIndex]).text('');
                        isValidFilePath = true;
                    }
                }
                if (isValidFilePath == false) {
                    $(elementIDs.uploadProofingDoc + documentUploadTemplateIndices[fileIndex]).parent().addClass('has-error');
                    $(elementIDs.uploadProofingDoc + documentUploadTemplateIndices[fileIndex]).addClass('form-control');
                    isFileValid = false;
                }
                else {
                    $(elementIDs.uploadProofingDoc + documentUploadTemplateIndices[fileIndex]).parent().removeClass('has-error');
                    isFileValid = true;
                }
            }

            var roundSts = '';

            //if (operationType == 'ADD')
            //    roundSts = ''
            //else {
            if ($(elementIDs.dropDownProofingRoundStatus + ' option:selected').text() != "Select  One")
                roundSts = $(elementIDs.dropDownProofingRoundStatus + ' option:selected').text();
            //}

            var newRow = {
                Round: proofingRoundSelected,
                ProofingDocument: documentType,
                DocumentName: documentName,
                Comments: proofingCommments,
                RoundStatus: roundSts,
                ClientDocumentPath: docPath
            }

            var proofingRepeaterRows = [];
            var gridData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
            $.each(gridData, function (i, row) {
                proofingRepeaterRows.push(row);
            });

            if (newRowData.length > 0) {
                $.each(newRowData, function (i, row) {
                    proofingRepeaterRows.push(row);
                });
            }

            var rowID = 0;

            if (selectedRow != undefined)
                rowID = selectedRow.RowIDProperty;

            var duplicateDocumentNames = proofingRepeaterRows.filter(function (el) {
                return (el.DocumentName.toLowerCase() === documentName.toLowerCase() && documentName != "" && el.ProofingDocument != "PBP Template");
            });

            if (operationType == 'ADD' && duplicateDocumentNames.length > 0) {
                $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).parent().addClass('has-error');
                $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).addClass('form-control');
                $(elementIDs.documentNameHelpBlock + documentUploadTemplateIndices[fileIndex]).text('Duplicate name is NOT allowed.');
                isDocumentNameValid = false;
                return false;
            }
            else if (operationType == 'EDIT') {
                var duplicateEditedNames = proofingRepeaterRows.filter(function (el) {
                    return (el.DocumentName === documentName && el.RowIDProperty != rowID);
                });

                if (duplicateEditedNames.length == 1) {
                    $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).parent().addClass('has-error');
                    $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).addClass('form-control');
                    $(elementIDs.documentNameHelpBlock + documentUploadTemplateIndices[fileIndex]).text('Duplicate name is NOT allowed.');
                    isDocumentNameValid = false;
                }
                else {
                    newRowData.push(newRow);
                    var existingName = $(elementIDs.documentName + documentUploadTemplateIndex).val();
                    if (existingName.length > 0) {
                        $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).parent().removeClass('has-error');
                        $(elementIDs.documentNameHelpBlock + documentUploadTemplateIndices[fileIndex]).text('');
                        isDocumentNameValid = true;
                    }
                }
            }
            else {
                newRowData.push(newRow);
                var existingName = $(elementIDs.documentName + documentUploadTemplateIndex).val();
                if (existingName != undefined) {
                    if (existingName.length > 0) {
                        $(elementIDs.documentName + documentUploadTemplateIndices[fileIndex]).parent().removeClass('has-error');
                        $(elementIDs.documentNameHelpBlock + documentUploadTemplateIndices[fileIndex]).text('');
                        isDocumentNameValid = true;
                    }
                }
            }
        }

        if (!(isRoundValid && isDocumentTypeValid && isDocumentNameValid && isFileValid))
            return false;
        var url = URLs.UploadProofingDocuments.replace(/\{FormInstanceId\}/g, currentInstance.formInstanceBuilder.formInstanceId)

        $.ajax({
            url: url,
            type: "POST",
            contentType: false,
            processData: false,
            data: fileData,
            success: function (result) {
                $(elementIDs.uploadProofingDocumentDialog).dialog("close");
                documentUploadTemplateIndex = 1;
                var rowsToInsert = [];
                for (var rowIdx = 0 ; rowIdx < newRowData.length; rowIdx++) {
                    var newRow = {
                        Round: newRowData[rowIdx].Round,
                        ProofingDocument: newRowData[rowIdx].ProofingDocument,
                        DocumentName: newRowData[rowIdx].DocumentName,
                        Comments: newRowData[rowIdx].Comments,
                        RoundStatus: newRowData[rowIdx].RoundStatus,
                        UserId: result.CurrentUserName,
                        DateTimestamp: result.timeStamp,
                        ClientDocumentPath: newRowData[rowIdx].ClientDocumentPath,
                        ServerDocumentPath: result.dictFilePathMappings[newRowData[rowIdx].ClientDocumentPath]
                    }
                    rowsToInsert.push(newRow);
                }
                if (operationType == 'ADD') {
                    var rowId = 0; // RowIDProperty of default empty row
                    if (currentInstance.data[rowId]['Round'] == "") {
                        currentInstance.deleteRow(rowId);// Delete default empty row
                        currentInstance.rowDataBeforeEdit = undefined;
                    }
                    var activityLogMsg = rowsToInsert[0]['Round'] + " added with documents : ";
                    $.each(rowsToInsert, function (i, row) {
                        //currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.ProofingAttachments][ebInstance.repeaterName.Proofing].push(row);
                        currentInstance.data.push(row);
                        activityLogMsg = activityLogMsg + row.DocumentName + ',';
                    });
                    activityLogMsg = activityLogMsg.substring(0, activityLogMsg.lastIndexOf(','));
                    currentInstance.addEntryToAcitivityLogger(undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, activityLogMsg);
                }
                else {
                    var repeaterColumns = {
                        ProofingDocument: 'ProofingDocument',
                        DocumentName: 'DocumentName',
                        RoundStatus: 'RoundStatus',
                        Comments: 'Comments',
                        DateTimestamp: 'DateTimestamp',
                        Round: 'Round',
                        ClientDocumentPath: 'ClientDocumentPath',
                        ServerDocumentPath: 'ServerDocumentPath',
                        UserId: 'UserId'
                    }

                    var rowBeingEdited = currentInstance.data.filter(function (el) {
                        return (el.RowIDProperty === selectedRow.RowIDProperty);
                    });
                    var existingRowData = rowBeingEdited[0];
                    //var existingRowData = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.ProofingAttachments][ebInstance.repeaterName.Proofing][selectedRow.RowIDProperty];
                    var oldRoundStatus = existingRowData[repeaterColumns.RoundStatus]
                    if (existingRowData[repeaterColumns.ProofingDocument] != rowsToInsert[0][repeaterColumns.ProofingDocument])
                        currentInstance.addEntryToAcitivityLogger(selectedRow.RowIDProperty, Operation.UPDATE, repeaterColumns.ProofingDocument, existingRowData[repeaterColumns.ProofingDocument], rowsToInsert[0][repeaterColumns.ProofingDocument], undefined, undefined, undefined);

                    if (existingRowData[repeaterColumns.DocumentName] != rowsToInsert[0][repeaterColumns.DocumentName])
                        currentInstance.addEntryToAcitivityLogger(selectedRow.RowIDProperty, Operation.UPDATE, repeaterColumns.DocumentName, existingRowData[repeaterColumns.DocumentName], rowsToInsert[0][repeaterColumns.DocumentName], undefined, undefined, undefined);

                    if (existingRowData[repeaterColumns.Comments] != rowsToInsert[0][repeaterColumns.Comments])
                        currentInstance.addEntryToAcitivityLogger(selectedRow.RowIDProperty, Operation.UPDATE, repeaterColumns.Comments, existingRowData[repeaterColumns.Comments], rowsToInsert[0][repeaterColumns.Comments], undefined, undefined, undefined);

                    if (existingRowData[repeaterColumns.RoundStatus] != rowsToInsert[0][repeaterColumns.RoundStatus]) {
                        var isRoundStatusChanged = false;
                        var proofingRows = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.ProofingAttachments][ebInstance.repeaterName.Proofing];
                        $.each(proofingRows, function (rowId, row) {
                            if (row.Round == rowsToInsert[0][repeaterColumns.Round]) {
                                currentInstance.data[rowId][repeaterColumns.RoundStatus] = rowsToInsert[0][repeaterColumns.RoundStatus];
                                isRoundStatusChanged = true;
                            }
                        });
                        if (isRoundStatusChanged) {
                            //var message = 'Status of ' + existingRowData[repeaterColumns.Round] + " changed from " + existingRowData[repeaterColumns.RoundStatus] + ' To ' + rowsToInsert[0][repeaterColumns.RoundStatus];
                            currentInstance.addEntryToAcitivityLogger(selectedRow.RowIDProperty, Operation.UPDATE, repeaterColumns.RoundStatus, oldRoundStatus, rowsToInsert[0][repeaterColumns.RoundStatus], undefined, undefined, undefined, undefined);
                        }
                    }

                    for (var idx = 0; idx < currentInstance.data.length ; idx++) {
                        if (currentInstance.data[idx].RowIDProperty == existingRowData.RowIDProperty) {
                            currentInstance.data[idx][repeaterColumns.ProofingDocument] = rowsToInsert[0][repeaterColumns.ProofingDocument];
                            currentInstance.data[idx][repeaterColumns.DocumentName] = rowsToInsert[0][repeaterColumns.DocumentName];
                            currentInstance.data[idx][repeaterColumns.Round] = rowsToInsert[0][repeaterColumns.Round];
                            currentInstance.data[idx][repeaterColumns.Comments] = rowsToInsert[0][repeaterColumns.Comments];
                            currentInstance.data[idx][repeaterColumns.ClientDocumentPath] = rowsToInsert[0][repeaterColumns.ClientDocumentPath];
                            currentInstance.data[idx][repeaterColumns.DateTimestamp] = rowsToInsert[0][repeaterColumns.DateTimestamp];
                            currentInstance.data[idx][repeaterColumns.UserId] = rowsToInsert[0][repeaterColumns.UserId];
                            if (result.result == 0)
                                currentInstance.data[idx][repeaterColumns.ServerDocumentPath] = rowsToInsert[0][repeaterColumns.ServerDocumentPath];
                        }
                    }
                }

                var proofingRows = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.ProofingAttachments][ebInstance.repeaterName.Proofing];

                var tempData = [];
                for (var i = 0; i < proofingRows.length; i++) {
                    proofingRows[i].RowIDProperty = i;
                    tempData.push(proofingRows[i]);
                }

                $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data", tempData);
                $(currentInstance.gridElementIdJQ).pqGrid('refreshDataAndView');

                for (var i = 0; i < tempData.length; i++) {
                    currentInstance.lastSelectedRow = i;
                    currentInstance.saveRow();
                }
            },
            error: function (err) {
                messageDialog.show(err.statusText);
            }
        });
    });

    $(".DocumentType").unbind().on('change', function () {
        var selectedObject = this;
        var valueSelected = this.value;
        var templateIndex = selectedObject.id.split('-')[1];
        if (valueSelected == "PBP Template") {
            $(elementIDs.documentNameSection + "-" + templateIndex).hide();
            $(elementIDs.uploadDocumentSection + "-" + templateIndex).hide();
            $(elementIDs.sharePointSection + "-" + templateIndex).show();
            var url = '/FolderVersion/GetPBPDocumentsSharePointLink';
            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                if (list != null && list != undefined && list != "") {
                    sharePointLink = list;
                }
            });
            promise.fail(showError);
        }
        else {
            $(elementIDs.documentNameSection + "-" + templateIndex).show();
            $(elementIDs.uploadDocumentSection + "-" + templateIndex).show();
            $(elementIDs.sharePointSection + "-" + templateIndex).hide();
        }
    });

    $(".PBPSharePointLink").unbind().on('click', function () {
        var selectedObject = this;
        var templateIndex = selectedObject.id.split('-')[1];

        if (sharePointLink != null && sharePointLink != undefined && sharePointLink != "") {
            FolderLockAction.ISREPEATERACTION = 1;
            window.open(sharePointLink);
        }
    });
}

expressionBuilderExtension.prototype.DeleteProofingDocument = function (currentInstance) {
    var row = $(currentInstance.gridElementIdJQ).pqGrid("selection", { type: 'row', method: 'getSelection' });
    if (row && row.length > 0 && row[0].rowData != undefined) {
        var rowId = row[0].rowData.RowIDProperty;
        var round = row[0].rowData.Round;
        var repeaterRows = currentInstance.data;

        var numberOfRounds = repeaterRows.filter(function (el) {
            return (el.Round === round);
        });

        if (rowId !== undefined && rowId !== null) {
            if (round != "") {
                if (numberOfRounds.length >= 2) { // if only one perticular round exists, then dont delete.
                    currentInstance.deleteRow(rowId);
                    currentInstance.rowDataBeforeEdit = undefined;
                }
                else
                    messageDialog.show("This document can not be deleted as there has to be at least one " + round);
            }
            else {
                currentInstance.deleteRow(rowId);
                currentInstance.rowDataBeforeEdit = undefined;
            }
        }
        else {
            messageDialog.show(Common.pleaseSelectRowMsg);
        }
    }
    else {
        messageDialog.show(Common.pleaseSelectRowMsg);
    }
}

expressionBuilderExtension.prototype.registerControlAndEventForANOCChart = function (formInstancebuilder) {
    var ebInstance = this;
    var elementName = ebInstance.elementIDs.searchANOCChartID + formInstancebuilder.formInstanceId;
    var searchLabel = ebInstance.elementIDs.searchLabel + formInstancebuilder.formInstanceId;

    $('#' + elementName).parent().append('<button class="btn btn-sm but-align pull-left btnSearch" style="margin:0px 5px; position:relative;top:-28px;left:300px;" id="' + elementName + '"> Search </button>' +
        '      <button class="btn btn-sm but-align pull-left btnProcess" style="margin:0px 5px; position:relative;top:-28px;left:300px;" id="' + elementName + '"> Process </button>');
    $('#' + searchLabel).parent().attr('class', 'col-xs-3 col-md-3 col-lg-3 col-sm-3');
    $('.btnSearch').off("click");
    $('.btnSearch').on("click", function () {
        var res = ebInstance.selectSourceDialog.show(ebInstance, formInstancebuilder);
    });

    $('.btnProcess').off("click");
    $('.btnProcess').on("click", function () {
        ebInstance.processANOCChart(formInstancebuilder);
    });
}

expressionBuilderExtension.prototype.setSourceDocumentValues = function (currentInstance) {
    var formInstanceID = currentInstance.formInstanceId;
    $(this.elementIDs.thSourceDocumentJQ + formInstanceID).val(this.sourceDocument.FormInstanceName);
    $(this.elementIDs.tdFolderNameJQ + formInstanceID).val(this.sourceDocument.FolderName);
    $(this.elementIDs.tdFolderVersionNumberJQ + formInstanceID).val(this.sourceDocument.VersionNumber);
    $(this.elementIDs.tdEffectiveDateJQ + formInstanceID).val(this.sourceDocument.EffectiveDate);
    $(this.elementIDs.thSourceDocumentIDJQ + formInstanceID).val(this.sourceDocument.FormInstanceID);

    currentInstance.formData['ANOCChartPlanDetails']['SourceDocument']['FolderName'] = this.sourceDocument.FolderName;
    currentInstance.formData['ANOCChartPlanDetails']['SourceDocument']['FolderVersion'] = this.sourceDocument.VersionNumber;
    currentInstance.formData['ANOCChartPlanDetails']['SourceDocument']['EffectiveDate'] = this.sourceDocument.EffectiveDate;
    currentInstance.formData['ANOCChartPlanDetails']['SourceDocument']['SourceDocumentID'] = this.sourceDocument.FormInstanceID;
    currentInstance.formData['ANOCChartPlanDetails']['SourceDocument']['DocumentName'] = this.sourceDocument.FormInstanceName;
}

expressionBuilderExtension.prototype.selectSourceDialog = function () {
    var ebInstance = this;
    var URLs = {
        docSearch: '/ConsumerAccount/GetDocumentsListForMedicareANOCChart?tenantId=1&formDesignId={formDesignId}&planType={planType}',
        getFormInstanceDataCompressed: '/ExpressionBuilder/GetFormInstanceDataCompressed'
    };
    var elementIDs = {
        sourceDialogJQ: "#sourceDocumentDialog",
        docSearchGrid: "docSearch",
        docSearchGridJS: "#docSearch",
        docSearchGridPagerJQ: "#pdocSearch"
    }

    function init() {
        //register dialog 
        $(elementIDs.sourceDialogJQ).dialog({
            autoOpen: false,
            height: 600,
            width: 800,
            modal: true
        });
    }

    function loadSelectGrid(dialogParent, formInstancebuilder) {
        var currentInstance = this;
        var url = URLs.getFormInstanceDataCompressed;
        var messageToPost = { tenantId: 1, formInstanceId: formInstancebuilder.anchorFormInstanceID };
        var promise = ajaxWrapper.postJSON(url, messageToPost);
        promise.done(function (xhr) {
            if (xhr !== undefined && xhr != null) {
                $(elementIDs.sourceDialogJQ).dialog('option', 'title', "Select Source Document");
                //$(elementIDs.openDocumentDialogJQ).dialog({
                //    position: {
                //        my: 'center',
                //        at: 'center'
                //    },
                //}); 
                $(elementIDs.sourceDialogJQ).dialog("open");
                var colArray = ['Folder', 'Folder Version Number', 'Effective Date', 'Document Name', '', '', '', ''];
                var colModel = [];
                colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
                colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left' });
                colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
                colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
                colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
                colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
                colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
                colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true });

                $(elementIDs.docSearchGridJS).jqGrid('GridUnload');
                $(elementIDs.docSearchGridJS).parent().append("<div id='p" + elementIDs.docSearchGrid + "'></div>");
                var url = URLs.docSearch;
                var planType = xhr["SECTIONASECTIONA1"]["PlanType"];
                //url = url.replace(/\{formDesignId\}/g, formInstancebuilder.formDesignId).replace(/\{planType\}/g, planType);
                url = url.replace(/\{formDesignId\}/g, 2359).replace(/\{planType\}/g, planType);
                $(elementIDs.docSearchGridJS).jqGrid({
                    url: url,
                    datatype: 'json',
                    cache: false,
                    colNames: colArray,
                    colModel: colModel,
                    caption: 'Select Documents',
                    height: '350',
                    rowNum: 20,
                    rowList: [10, 20, 30],
                    ignoreCase: true,
                    autowidth: true,
                    viewrecords: true,
                    hidegrid: false,
                    altRows: true,
                    pager: elementIDs.docSearchGridPagerJQ,
                    sortname: 'FormInstanceName',
                    altclass: 'alternate',
                    gridComplete: function () {

                    }
                });
                var pagerElement = elementIDs.docSearchGridPagerJQ;
                //var formInstanceID = currentInstance.formInstanceId;
                //remove default buttons
                $(elementIDs.docSearchGridJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
                $(elementIDs.docSearchGridJS).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
                $(elementIDs.docSearchGridJS).jqGrid('navButtonAdd', pagerElement,
                   {
                       caption: '', buttonicon: 'ui-icon-check', title: 'Select',
                       onClickButton: function () {
                           setDocumentSelection(dialogParent, formInstancebuilder);
                       }
                   });
            }
        });
    }

    function setDocumentSelection(dialogParent, formInstancebuilder) {
        var selectedRowId = $(elementIDs.docSearchGridJS).jqGrid('getGridParam', 'selrow');
        if (selectedRowId != null && selectedRowId != undefined) {
            var rowData = $(elementIDs.docSearchGridJS).jqGrid('getRowData', selectedRowId);
            dialogParent.sourceDocument = rowData;
            dialogParent.setSourceDocumentValues(formInstancebuilder);
            $(elementIDs.sourceDialogJQ).dialog("close");
        }
        else {
            messageDialog.show("Please select a row.");
        }
    }

    return {
        show: function (dialogParent, formInstancebuilder) {
            init();
            loadSelectGrid(dialogParent, formInstancebuilder);
        }
    }
}();

expressionBuilderExtension.prototype.processANOCChart = function (currentInstance) {
    var ebInstance = this;
    var documentSourceDetails = {};
    var formInstanceID = currentInstance.formInstanceId;
    documentSourceDetails.FormInstanceName = $(this.elementIDs.thSourceDocumentJQ + formInstanceID).val();
    documentSourceDetails.FolderName = $(this.elementIDs.tdFolderNameJQ + formInstanceID).val();
    documentSourceDetails.VersionNumber = $(this.elementIDs.tdFolderVersionNumberJQ + formInstanceID).val();
    documentSourceDetails.EffectiveDate = $(this.elementIDs.tdEffectiveDateJQ + formInstanceID).val();
    documentSourceDetails.FormInstanceID = $(this.elementIDs.thSourceDocumentIDJQ + formInstanceID).val();
    if (documentSourceDetails.FormInstanceName != undefined && documentSourceDetails.FormInstanceName != null && documentSourceDetails.FormInstanceName != ""
        && documentSourceDetails.FormInstanceID != undefined && documentSourceDetails.FormInstanceID != null && documentSourceDetails.FormInstanceID != ""
        && documentSourceDetails.FolderName != undefined && documentSourceDetails.FolderName != null && documentSourceDetails.FolderName != "") {
        ebInstance.getANOCChartServices(currentInstance, documentSourceDetails);
    }
    else {
        messageDialog.show(Common.pleaseSelectSourceMsg);
    }
}

expressionBuilderExtension.prototype.getANOCChartServices = function (currentInstance, documentSourceDetails) {
    var ebInstance = this;
    var postData = {
        tenantId: currentInstance.tenantId,
        nextYearFormInstanceId: currentInstance.anchorFormInstanceID,
        previousYearFormInstanceId: documentSourceDetails.FormInstanceID,
        effectiveDate: currentInstance.folderData.effectiveDate,
        // Added anocViewFormInstanceId for testing BRG repeater compaarision 
        anocViewFormInstanceId: currentInstance.formInstanceId
        //formDesignVersionId: currentInstance.formDesignVersionId,
        //folderVersionId: currentInstance.folderVersionId,
    }

    var promise = ajaxWrapper.postJSON(ebInstance.URLs.getANOCChartService, postData);
    promise.done(function (result) {
        var chartServiceArray = [];
        chartServiceArray = JSON.parse(result);
        var newData = JSON.parse(result);
        if (chartServiceArray != undefined && chartServiceArray != null && chartServiceArray.length > 0) {
            chartServiceArray = chartServiceArray[0];
            newData = newData[0];
            var result = Object.keys(chartServiceArray);
            var length = result.length;
            for (var i = 0; i < length; i++) {
                var repeaterFullPath = result[i];
                var reptrInstance = currentInstance.repeaterBuilders.filter(function (dt) {
                    return dt.fullName == repeaterFullPath;
                })
                switch (repeaterFullPath) {
                    case ebInstance.reapterFullName.ANOCChartPlanInformation:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PlanInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            //var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            //gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            //gridOptions.api.setRowData(gridOptions.rowData);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PlanInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);

                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartPlanPremiumInformation:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PlanPremiumInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            //var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            //gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            //gridOptions.api.setRowData(gridOptions.rowData);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PlanPremiumInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);

                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartOutofPocketInformation:
                        try {
                            var processedData = [];
                            processedData = ebInstance.prepareGridData(JSON.parse(newData[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]), reptrInstance[0].columnModels);
                            var dataModelData = ebInstance.prepareGridData(JSON.parse(newData[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]), reptrInstance[0].columnModels);
                            reptrInstance[0].data = processedData;
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.OutofPocketInformation] = processedData;
                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = dataModelData;
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            //var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            //gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]);
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]);
                            //gridOptions.api.setRowData(gridOptions.rowData);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.OutofPocketInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]);

                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare:
                        try {
                            var processedData = [];
                            processedData = ebInstance.prepareGridData(JSON.parse(newData[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]), reptrInstance[0].columnModels);
                            var dataModelData = ebInstance.prepareGridData(JSON.parse(newData[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]), reptrInstance[0].columnModels);

                            reptrInstance[0].data = processedData;
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCBenefitsCompare] = processedData;

                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = dataModelData;
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            //var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            //gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]);
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]);
                            //gridOptions.api.setRowData(gridOptions.rowData);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCBenefitsCompare] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]);

                        } catch (e) {
                            console.log(e.message);
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartPrescriptionInformation:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PrescriptionInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            //var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            //gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            //gridOptions.api.setRowData(gridOptions.rowData);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PrescriptionInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);

                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.InitialCoverageLimitInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);

                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            //var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            //gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            //gridOptions.api.setRowData(gridOptions.rowData);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.InitialCoverageLimitInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);

                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartGapCoverageInformation:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartGapCoverageInformation]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCCPDoctorOfficeVisit] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInformation]);
                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInformation]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            //var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            //gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartGapCoverageInformation]);
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartGapCoverageInformation]);
                            //gridOptions.api.setRowData(gridOptions.rowData);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.GapCoverageInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartGapCoverageInformation]);

                        } catch (e) {
                        }
                        break;
                        //Doctor Office Visits
                    case ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCOfficeVisitInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");


                        } catch (e) {
                        }
                        break;
                        //Inpatient Health Service
                    case ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCIHSInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");


                        } catch (e) {
                        }
                        break;

                    case ebInstance.reapterFullName.ANOCChartPreferredRetailInfo:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PreferredRetailInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);

                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails:
                        try {
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.CostShareDetails] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            $(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                        } catch (e) {
                        }

                }
            }
        }
    });
}
expressionBuilderExtension.prototype.prepareGridData = function (chartServiceArray, columnModels) {
    var gridData = [];
    try {
        var data = chartServiceArray;
        for (var index = 0; index < data.length; index++) {
            var row = data[index];
            var newRow = $.extend({}, row);
            $.each(columnModels, function (idx, col) {
                if (col.dataIndx.toString().substring(0, 4) == 'INL_') {
                    var propArr = col.dataIndx.toString().split('_');
                    if (propArr.length == 4) {
                        var dsName = propArr[1];
                        var propIdx = propArr[2];
                        var propName = propArr[3];
                        newRow[col.dataIndx] = row[dsName][propIdx][propName];

                    }
                }
            });
            gridData.push(newRow);
        }
    }
    catch (err) {
        console.log(err.message);
    }
    return gridData;
}

expressionBuilderExtension.prototype.additionalANOCChartServicesOnPopupSave = function (currentInstance, data, oldData) {
    var ebInstance = this;
    var postData = {
        tenantId: currentInstance.tenantId,
        nextYearFormInstanceId: currentInstance.formInstanceBuilder.anchorFormInstanceID,
        previousYearFormInstanceId: currentInstance.formInstanceBuilder.formData["ANOCChartPlanDetails"]["SourceDocument"]["SourceDocumentID"],
        repeaterData: JSON.stringify(oldData),
        repeaterSelectedData: JSON.stringify(data),
        effectiveDate: currentInstance.formInstanceBuilder.folderData.effectiveDate,
        anocViewFormInstanceId: currentInstance.formInstanceId
    }
    var promise = ajaxWrapper.postJSON(ebInstance.URLs.getAdditionalServicesData, postData);
    promise.done(function (result) {
        var chartServiceArray = [];
        chartServiceArray = JSON.parse(result);
        var newData = JSON.parse(result);
        if (chartServiceArray != undefined && chartServiceArray != null && chartServiceArray.length > 0) {
            try {
                var processedData = [];
                var targetReptrInstance = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (dt) {
                    return dt.fullName == ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare;
                })
                processedData = ebInstance.prepareGridData(newData, targetReptrInstance[0].columnModels);
                var dataModelData = ebInstance.prepareGridData(newData, targetReptrInstance[0].columnModels);
                targetReptrInstance[0].data = processedData;
                currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCBenefitsCompare] = processedData;

                var dataModel = $(targetReptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                dataModel.data = dataModelData;
                $(targetReptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");
            } catch (e) {
            }
        }
    });

    //targetReptrInstance[0].columnNames = [];
    //targetReptrInstance[0].columnModels = [];
    //targetReptrInstance[0].groupHeaders = [];
    //$(targetReptrInstance[0].gridElementIdJQ).pqGrid("destroy");
    //targetReptrInstance[0].build();
}

expressionBuilderExtension.prototype.setPreviousCostShareValue = function (currentInstance, rowId, currentElementPath, previousElementPath, newValue, oldValue) {

    if (GridDisplayMode.PQ == CurrentGridDisplayMode) {
        var originalRow = currentInstance.orignalData.filter(function (ct) {
            if (ct.RowIDProperty == rowId) {
                return ct;
            }
        });

        var modelData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
        var modeldataRow = modelData.filter(function (dt) {
            return dt.RowIDProperty == rowId;
        });
        modeldataRow[0][previousElementPath] = originalRow[0][currentElementPath];
        $(currentInstance.gridElementIdJQ).pqGrid("refreshCell", { rowIndx: rowId, dataIndx: previousElementPath });
        $(currentInstance.gridElementIdJQ).pqGrid("refreshView");

    }
    if (GridDisplayMode.AG == CurrentGridDisplayMode) {

        var originalRowData = currentInstance.orignalData.filter(function (ct) {
            if (ct.RowIDProperty == rowId) {
                return ct;
            }
        });

        if (originalRowData.length > 0) {
            var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
            var rowData = currentInstance.getRowDataFromInstanceGrid(rowId);
            if (originalRowData[0][currentElementPath] != undefined) {
                rowData[0][previousElementPath] = originalRowData[0][currentElementPath];;
                rowData[0][currentElementPath] = newValue;
                gridOptions.api.updateRowData({ update: rowData });
            }
        }
    }


}

expressionBuilderExtension.prototype.DisableCSRDropDownItems = function (currentInstance, ebInstance) {

    var marketCoverage = currentInstance.formData[ebInstance.sectionName.QHPSection][ebInstance.sectionName.PlanIdentifiers][ebInstance.sectionElementName.MarketCoverage];
    var metalTier = currentInstance.formData[ebInstance.sectionName.QHPSection][ebInstance.sectionName.PlanIdentifiers][ebInstance.sectionElementName.MarketTier];
    var csrVariationType = currentInstance.formData[ebInstance.sectionName.QHPSection][ebInstance.sectionName.PlanIdentifiers][ebInstance.sectionElementName.CSRVariationType];

    var EnabledOption = [];
    if (marketCoverage == "Individual") {
        if (metalTier == "Silver") {
            EnabledOption.push("On Exchange Plan");
            EnabledOption.push("Off Exchange Plan");
            EnabledOption.push("Both (Display as On/Off Exchange)");
            EnabledOption.push("Limited Cost Sharing Plan Variation");
            EnabledOption.push("Zero Cost Sharing Plan Variation");
            EnabledOption.push("73% AV Level Silver Plan");
            EnabledOption.push("87% AV Level Silver Plan");
            EnabledOption.push("94% AV Level Silver Plan");
        }
        if (metalTier == "Catastrophic") {
            EnabledOption.push("On Exchange Plan");
            EnabledOption.push("Off Exchange Plan");
            EnabledOption.push("Both (Display as On/Off Exchange)");
        }
        if (metalTier != "Silver" && metalTier != "Catastrophic") {
            EnabledOption.push("On Exchange Plan");
            EnabledOption.push("Off Exchange Plan");
            EnabledOption.push("Both (Display as On/Off Exchange)");
            EnabledOption.push("Limited Cost Sharing Plan Variation");
            EnabledOption.push("Zero Cost Sharing Plan Variation");
        }
    }
    else if (marketCoverage == "SHOP (Small Group)") {
        EnabledOption.push("On Exchange Plan");
        EnabledOption.push("Off Exchange Plan");
        EnabledOption.push("Both (Display as On/Off Exchange)");
    }
    var elementName = "#COM2405DropDown90038";
    $(elementName + currentInstance.formInstanceId + " option").each(function (i) {
        var optionValue = $(this).val();
        if (optionValue != "[Select One]") {
            var valueIndex = EnabledOption.indexOf(optionValue);
            var dropdownOptionPath = elementName + currentInstance.formInstanceId + " option[value='" + optionValue + "']";
            if (valueIndex != -1) {
                $(dropdownOptionPath).prop('disabled', false);
            }
            else {
                $(dropdownOptionPath).prop('disabled', true);
            }
        }
    });
}


expressionBuilderExtension.prototype.prepareCustomDataForRepeater = function (currentInstance, data) {
    var customData = [];
    var exitingData = currentInstance.data;
    switch (currentInstance.fullName) {
        case this.reapterFullName.LimitSummary:
            var colArray = ['LimitDescription', 'LimitAmount', 'LimitFrequency', 'LimitType', 'MLMedicalServicList'];

            $.each(data, function (idx, dt) {
                var isdataExist = currentInstance.data.filter(function (ct) {
                    return ct["LimitDescription"] == dt["LimitDescription"]
                    //&& ct[currentInstance.customrule.KeyName.BenefitCategory2] == dt[currentInstance.customrule.KeyName.BenefitCategory2]
                    //&& ct[currentInstance.customrule.KeyName.BenefitCategory3] == dt[currentInstance.customrule.KeyName.BenefitCategory3]
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

expressionBuilderExtension.prototype.registerControlAndEventForSBCCalculator = function (formInstancebuilder) {
    var ebInstance = this;

    var elementName = ebInstance.elementIDs.searchCalculatorSBCId + formInstancebuilder.formInstanceId;
    var searchLabel = ebInstance.elementIDs.searchCalculatorSBCLabel + formInstancebuilder.formInstanceId;

    $('#' + elementName).append('<button class="btn btn-sm but-align pull-right btnProcess" style="margin-right:10px;" id="' + elementName + '"> Calculate </button>');
    $('.btnProcess').off("click");
    $('.btnProcess').on("click", function () {
        ebInstance.processSBCCalculator(formInstancebuilder);
    });
}

expressionBuilderExtension.prototype.processSBCCalculator = function (formInstancebuilder) {
    var ebInstance = this;
    URLs = {
        ProcessRuleForSameSection: "/ExpressionBuilder/ProcessEBSRuleForSameSection"
    };

    var formInstanceData = formInstancebuilder.form.getFormInstanceData();
    formInstanceData.formInstanceData = formInstanceData.formInstanceData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');

    var rptData = JSON.stringify(formInstancebuilder.formData.SBCCalculator.CoverageExample);
    var rptFullPath = "SBCCalculator.CoverageExample";
    var input = {
        formDesignVersionId: formInstancebuilder.designData.FormDesignVersionId,
        folderVersionId: formInstancebuilder.folderVersionId,
        formInstanceId: formInstancebuilder.formInstanceId,
        sourceElementPath: rptFullPath,
        ElementValue: rptData,
        sectionData: formInstanceData.formInstanceData,
        isMultiselect: false
    }
    var currentInstance = this;
    var promise = ajaxWrapper.postJSON(URLs.ProcessRuleForSameSection, input);
    promise.done(function (xhr) {
        if (xhr != null) {
            if (xhr != "[]") {
                var res = JSON.parse(xhr);
                $.each(res, function (idx, val) {
                    var reptrInstance = null;
                    reptrInstance = formInstancebuilder.repeaterBuilders.filter(function (dt) {
                        return dt.fullName == val.TargetPath;
                    })
                    $.each(formInstancebuilder.repeaterBuilders, function (index, value) {
                        if (value.fullName == val.TargetPath) {
                            value.data = val.Data;
                            //value.lastSelectedRow = val.Data.length;
                        }
                    });
                    if (reptrInstance.length != 0) {
                        //refresh AG grid
                        var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                        gridOptions.rowData = val.Data;
                        gridOptions.api.setRowData(gridOptions.rowData);
                        //gridOptions.api.sizeColumnsToFit();
                    }
                });
            }
        }
    });
}

expressionBuilderExtension.prototype.registerEventForButtonInRepeater = function (currentInstance) {
    var ebInstance = this;
    switch (ebInstance.FormDesignId) {
        case FormTypes.COMMERCIALMEDICAL:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.BenefitReview:
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.BenefitReviewGridSlidingCostShare:
                    $(".viewCOMMERCIALBRGLimits").off().on('click', function () {
                        // currentInstance.saveRow();
                        var element = $(this).attr("Id");
                        var griddata = currentInstance.data.filter(function (dt) {
                            return dt[ebInstance.KeyName.RowIDProperty] == element;
                        });
                        var elementDesignData = currentInstance.design.Elements.filter(function (dt) {
                            if (dt.GeneratedName == "Limits") {
                                return dt;
                            }
                        });
                        ebInstance.showLimitPouUp(element, currentInstance.data, currentInstance.tenantId, currentInstance.formInstanceId, currentInstance.formInstanceBuilder.formDesignVersionId, currentInstance.formInstanceBuilder.folderVersionId, elementDesignData);
                    });
                    break;
            }
            break;
    }
}

expressionBuilderExtension.prototype.showLimitPouUp = function (rowId, data, tenantId, formInstanceId, formDesignVersionId, folderVersionId, elementDesignData) {

    var ebInstance = this;
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

    var paramArray = [];
    paramArray[0] = JSON.stringify(benefitRowData)
    var elementFullPath = elementDesignData[0].FullName;
    var postData = {
        sourceElementPath: elementFullPath,
        folderVersionId: folderVersionId,
        formInstanceId: formInstanceId,
        formDesignVersionId: formDesignVersionId,
        paramArray: paramArray
    }


    var promise = ajaxWrapper.postJSON(ebInstance.URLs.getRuntimeEBResult, postData);
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
                width: 900,
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

            limitData = "<tr>" +
                   " <th style='width:20%' bgcolor='#333333'><font color='#fff'>Limit Description</font></th>" +
                    "<th style='width:15%' bgcolor='#333333'><font color='#fff'>Limit Amount</font></th>" +
                    "<th style='width:15%' bgcolor='#333333'><font color='#fff'>Limit Frequency</font></th>" +
                    "<th style='width:50%' bgcolor='#333333'><font color='#fff'>Limit Type</font></th>" +
                  "</tr>";


            for (var i = 0; i < limitDetailsArray.length; i++) {

                limitData += "<tr>" +
                      "<td>" + limitDetailsArray[i]["LimitDescription"] + "</td>" +
                      "<td>" + limitDetailsArray[i]["LimitAmount"] +
                      "<td>" + limitDetailsArray[i]["LimitFrequency"] + "</td>" +
                      "<td>" + limitDetailsArray[i]["LimitType"] + "</td>" +
                 "</tr>";
            }

            limitData = "<div class='row'><div class='col-sm-9' style='width:100%'><table>" + limitData + "</table></div></div>"; ''
        }


        $(limitDialogJQ).append(limitData);
        var title = "Limits";// + benefitRowData.BenefitCategory1;
        $(elementIDs.limitDataDialogJQ).dialog('option', 'title', title);
        $(elementIDs.limitDataDialogJQ).dialog("open");
    });

    promise.fail(showError);
}

expressionBuilderExtension.prototype.getEBRowResult = function (ebInstance, currentInstance, rowId, element, newValue) {

    if (GridDisplayMode.AG == CurrentGridDisplayMode) {
        var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
        var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
        var rowData = rowNode.data;
        var paramArray = [];
        paramArray[0] = JSON.stringify(rowData);
        switch (element.GeneratedName) {
            case ebInstance.repeaterElementName.ManualOverride:
                switch (newValue) {
                    case "Yes":
                        {
                            var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
                            var rowData = rowNode.data;
                            var coveredValue = rowData[ebInstance.repeaterElementName.Covered];
                            var manualOverrideValue = rowData[ebInstance.repeaterElementName.ManualOverride];
                            rowData[ebInstance.repeaterElementName.Covered] = "Yes";
                            rowData[ebInstance.repeaterElementName.ManualOverride] = newValue;
                            rowNode.setData(rowData);
                            rowData[ebInstance.repeaterElementName.CostShareGroup] = "Not Applicable";
                            if (coveredValue == "No" || coveredValue == "false") {
                                var defaultNotCoveredvalue = "Not Covered";
                                rowData[ebInstance.repeaterElementName.Copay] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.Coinsurance] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.IndDeductible] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.FamDeductible] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.IndOOPM] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.FamilyOOPM] = defaultNotCoveredvalue;
                                if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.DENTAL) {
                                    rowData[ebInstance.repeaterElementName.IndividualDeductible] = defaultNotCoveredvalue;
                                    rowData[ebInstance.repeaterElementName.FamilyDeductible] = defaultNotCoveredvalue;
                                    rowData[ebInstance.repeaterElementName.IndividualOOPM] = defaultNotCoveredvalue;
                                    rowData[ebInstance.repeaterElementName.FamilyOOPM] = defaultNotCoveredvalue;
                                }
                                //gridOptions.api.updateRowData({ update: rowData });
                            }
                            ebInstance.setUpdatedRowInCurrentInstance(currentInstance, ebInstance, rowId, rowData);
                            rowNode.setData(rowData);
                        }
                        break;
                    case "No":
                        {
                            var postData = {
                                sourceElementPath: element.FullName,
                                folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
                                formInstanceId: currentInstance.formInstanceId,
                                formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
                                paramArray: paramArray,
                            }
                            ebInstance.runExpressionRuleOnCellChange(currentInstance, ebInstance, rowId, postData);
                        }
                        break;
                }
                break;
            case ebInstance.repeaterElementName.IsCovered:
                {
                    var postData = {
                        sourceElementPath: element.FullName,
                        folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
                        formInstanceId: currentInstance.formInstanceId,
                        formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
                        paramArray: paramArray,
                    }
                    ebInstance.runExpressionRuleOnCellChange(currentInstance, ebInstance, rowId, postData);
                }
                break;
            case ebInstance.repeaterElementName.Covered:
                {
                    switch (newValue) {
                        case "No":
                            {
                                var defaultNotCoveredvalue = "Not Covered";
                                rowData[ebInstance.repeaterElementName.Copay] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.Coinsurance] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.IndDeductible] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.FamDeductible] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.IndOOPM] = defaultNotCoveredvalue;
                                rowData[ebInstance.repeaterElementName.FamilyOOPM] = defaultNotCoveredvalue;
                                if (currentInstance.formInstanceBuilder.formDesignId == FormTypes.DENTAL) {
                                    rowData[ebInstance.repeaterElementName.IndividualDeductible] = defaultNotCoveredvalue;
                                    rowData[ebInstance.repeaterElementName.FamilyDeductible] = defaultNotCoveredvalue;
                                    rowData[ebInstance.repeaterElementName.IndividualOOPM] = defaultNotCoveredvalue;
                                    rowData[ebInstance.repeaterElementName.FamilyOOPM] = defaultNotCoveredvalue;
                                }
                            }
                    }
                }
                break;
            case ebInstance.repeaterElementName.Network:
            case ebInstance.repeaterElementName.CostShareType:
            case ebInstance.repeaterElementName.RefreshCoinsurance:
            case ebInstance.repeaterElementName.RefreshCoinsuranceGuardrail:
            case ebInstance.repeaterElementName.AllowableCostShareType:
                {
                    switch (ebInstance.FormDesignId) {
                        case FormTypes.COMMERCIALMEDICAL:
                            if (currentInstance.design.FullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.RxBenefitReview) {
                                var RxCostShare = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.Rx][ebInstance.sectionName.RxProductInformation][ebInstance.repeaterName.RxCostShare];
                                paramArray[1] = JSON.stringify(RxCostShare);
                            }
                            break;
                        case FormTypes.RX:
                            if (currentInstance.design.FullName == ebInstance.reapterFullName.RX.RxBenefitReview) {
                                var RxCostShare = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.RxProductInformation][ebInstance.repeaterName.RxCostShare];
                                paramArray[1] = JSON.stringify(RxCostShare);
                            }
                            break;
                    }
                    var postData = {
                        sourceElementPath: element.FullName,
                        folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
                        formInstanceId: currentInstance.formInstanceId,
                        formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
                        paramArray: paramArray,
                    }
                    ebInstance.runExpressionRuleOnCellChange(currentInstance, ebInstance, rowId, postData);
                }
                break;
        }
        ebInstance.setAGGridColSelection(currentInstance, rowId, element.GeneratedName);
    }
}

expressionBuilderExtension.prototype.getProductInformation = function (currentInstance, newValue) {
    var sectionProductSelection = currentInstance.selectedSectionName + "ProductSelection";
    var selectProduct = "RequestNew" + currentInstance.selectedSectionName + "Product";
    if (currentInstance.formData[currentInstance.selectedSectionName][sectionProductSelection][selectProduct] == "false"
        || currentInstance.formData[currentInstance.selectedSectionName][sectionProductSelection][selectProduct] == "False"
        || currentInstance.formData[currentInstance.selectedSectionName][sectionProductSelection][selectProduct] == false
        || currentInstance.formData[currentInstance.selectedSectionName][sectionProductSelection][selectProduct] == "No") {
        currentInstance.reload(false, newValue);
    }
}

expressionBuilderExtension.prototype.bindProviderOptionDropdown = function (currentInstance, rowData) {
    var repeaterBuilder = currentInstance;
    var ebInstance = this;
    var provisionOptionList = new Array();
    var provisionOptionObj = {};
    var provisionName = "";
    if (null != rowData) {
        provisionName = rowData[ebInstance.repeaterElementName.ProvisionNameSERVICEPROVISION];
    }
    if (provisionName != "" && provisionName != undefined) {
        //var defaultObj = {
        //    Enabled: null,
        //    ItemID: 0,
        //    ItemText: "",
        //    ItemValue: "0"
        //};
        //provisionOptionList.push(defaultObj);
        var provisionNameList = undefined;
        if (repeaterBuilder.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.SelectProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.ProductRules][ebInstance.sectionName.ProductWideProvisions][ebInstance.repeaterName.AllowableProvisions];
        }
        else if (repeaterBuilder.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.BRAGSelectProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.BenefitReview][ebInstance.sectionName.ServiceWideServiceNetworkWideProvisions][ebInstance.repeaterName.AllowableProvisions];
        }
        else if (repeaterBuilder.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.NetworkSelectProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.Network][ebInstance.sectionName.ProductNetworkWideProvisions][ebInstance.repeaterName.AllowableProvisions];
        }
        else if (repeaterBuilder.fullName == ebInstance.reapterFullName.RX.RxSelectProductWideProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.RxProductInformation][ebInstance.sectionName.AllowableProvisionProductWide][ebInstance.repeaterName.AllowableProductWideProvisions];
        }
        else if (repeaterBuilder.fullName == ebInstance.reapterFullName.RX.RxSelectProductNetworkWideProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.RxProductInformation][ebInstance.sectionName.AllowableProvisionNetworkWide][ebInstance.repeaterName.AllowableProductNetworkWideProvisions];
        }
        else if (repeaterBuilder.fullName == ebInstance.reapterFullName.RX.RxSelectServiceNetworkWideProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.RxProductInformation][ebInstance.sectionName.AllowableProvisionsServiceWide][ebInstance.repeaterName.AllowableServiceWideProvisions];
        }
        else if (repeaterBuilder.fullName == ebInstance.reapterFullName.DENTAL.DentalSelectProductWideProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.DentalProductInformation][ebInstance.sectionName.AllowableProvisionProductWide][ebInstance.repeaterName.AllowableProductWideProvisions];
        }
        else if (repeaterBuilder.fullName == ebInstance.reapterFullName.DENTAL.DentalSelectServiceWideProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.DentalProductInformation][ebInstance.sectionName.AllowableProvisionsServiceWide][ebInstance.repeaterName.AllowableServiceWideProvisions];
        }
        else if (repeaterBuilder.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.SelectHSAProvisions) {
            provisionNameList = currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.HSA][ebInstance.sectionName.HSAProductInformation][ebInstance.repeaterName.AllowableHSAProductWideProvisions];
        }

        if (provisionNameList != null && provisionNameList != undefined && provisionNameList.length > 0) {
            if (repeaterBuilder.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.SelectProvisions
                || repeaterBuilder.fullName == ebInstance.reapterFullName.RX.RxSelectProductWideProvisions
                || repeaterBuilder.fullName == ebInstance.reapterFullName.DENTAL.DentalSelectProductWideProvisions
                || repeaterBuilder.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.SelectHSAProvisions
                ) {
                provisionNameList = provisionNameList.filter(function (dt) {
                    return dt.ProvisionNameSERVICEPROVISION == provisionName && dt.Select == "Yes";
                });
            }
            else if (repeaterBuilder.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.BRAGSelectProvisions || repeaterBuilder.fullName == ebInstance.reapterFullName.RX.RxSelectServiceNetworkWideProvisions
                || repeaterBuilder.fullName == ebInstance.reapterFullName.DENTAL.DentalSelectServiceWideProvisions) {
                var networkName = ""; var serviceName = "";
                if (null != rowData) {
                    networkName = rowData[ebInstance.repeaterElementName.NetworkNameBNTNM];
                    serviceName = rowData[ebInstance.repeaterElementName.ServiceNameSVNM];
                }
                provisionNameList = provisionNameList.filter(function (dt) {
                    return dt.ProvisionNameSERVICEPROVISION == provisionName && dt.Select == "Yes" &&
                        dt.NetworkNameBNTNM == networkName && dt.ServiceNameSVNM == serviceName;
                });
            }
            else if (repeaterBuilder.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.NetworkSelectProvisions || repeaterBuilder.fullName == ebInstance.reapterFullName.RX.RxSelectProductNetworkWideProvisions) {
                var networkName = "";
                if (null != rowData) {
                    networkName = rowData[ebInstance.repeaterElementName.NetworkNameBNTNM];
                }
                provisionNameList = provisionNameList.filter(function (dt) {
                    return dt.ProvisionNameSERVICEPROVISION == provisionName && dt.Select == "Yes" &&
                        dt.NetworkNameBNTNM == networkName;
                });
            }

            $.each(provisionNameList, function (i, v) {
                var obj = {
                    Enabled: null,
                    ItemID: 0,
                    ItemText: v.ProvisionTextOptions,
                    ItemValue: v.ProvisionTextOptions
                };

                var item = provisionOptionList.filter(function (dt) {
                    return dt.ItemText == v.ProvisionTextOptions && dt.ItemValue == v.ProvisionTextOptions;
                });
                // If not exist add (add only distinct values)
                if (item == undefined || item == null || item.length == 0) {
                    provisionOptionList.push(obj);
                }
            });
            var elementDesign = currentInstance.design.Elements.filter(function (ct) {
                return ct.GeneratedName == ebInstance.repeaterElementName.ProvisionTextOptions;
            });
            elementDesign[0].Items = provisionOptionList;

            var dd = [];
            if (provisionOptionList != null && provisionNameList.length > 0) {
                var defaultValue = {}; defaultValue[Validation.selectOne] = Validation.selectOne;
                dd.push(defaultValue);

                var data = provisionOptionList;
                for (var idx = 0; idx < data.length; idx++) {
                    if (data[idx].ItemValue != '') {
                        var d = {};
                        d[data[idx].ItemValue] = data[idx].ItemText == null ? data[idx].ItemValue : data[idx].ItemText;
                        dd.push(d);
                    }
                }
            }
            provisionOptionObj["Items"] = provisionOptionList;
            provisionOptionObj["Values"] = dd;
        }
    }
    return provisionOptionObj;
}

expressionBuilderExtension.prototype.tiersToFollows = function (currentInstance, data, oldValue, newValue) {
    var ebInstance = this;
    var column = data[ebInstance.KeyName.SameAsCostShare];

    if (currentInstance.columnName == ebInstance.KeyName.SameAsNetworkTier || currentInstance.columnName == ebInstance.KeyName.SameAsCostShare) {
        if (column == "" || column == undefined) { return }
        var sourceRow = currentInstance.data.filter(function (dt) {
            return dt[ebInstance.KeyName.CostShareGroup] == data[ebInstance.KeyName.CostShareGroup]
            && dt[ebInstance.repeaterElementName.NetworkTier] == data[ebInstance.KeyName.SameAsNetworkTier];
        })

        if (sourceRow.length > 0 && column != undefined) {
            if (column.indexOf(",") != -1) {
                var columns = column.split(",");

                $.each(columns, function (idx, dt) {
                    if (data[dt] != undefined) {
                        data[dt] = sourceRow[0][dt];
                    }
                    if (dt == ebInstance.repeaterElementName.CoinsuranceAmount) {
                        data[ebInstance.repeaterElementName.RefreshCoinsurance] = "No";
                    }

                    if (oldValue.indexOf(ebInstance.repeaterElementName.CoinsuranceAmount) != -1 && newValue.indexOf(ebInstance.repeaterElementName.CoinsuranceAmount) == -1) {
                        data[ebInstance.repeaterElementName.RefreshCoinsurance] = "Yes";
                        var element = currentInstance.getElementDesign(ebInstance.repeaterElementName.RefreshCoinsurance);
                        ebInstance.getEBRowResult(ebInstance, currentInstance, data.RowIDProperty, element, "Yes");
                    }
                })
            }
            else {
                if (data[column] != undefined) {
                    data[column] = sourceRow[0][column];
                }

                if (column == ebInstance.repeaterElementName.CoinsuranceAmount) {
                    data[ebInstance.repeaterElementName.RefreshCoinsurance] = "No";
                }

                if (oldValue.indexOf(ebInstance.repeaterElementName.CoinsuranceAmount) != -1 && newValue.indexOf(ebInstance.repeaterElementName.CoinsuranceAmount) == -1) {
                    data[ebInstance.repeaterElementName.RefreshCoinsurance] = "Yes";
                    var element = currentInstance.getElementDesign(ebInstance.repeaterElementName.RefreshCoinsurance);
                    ebInstance.getEBRowResult(ebInstance, currentInstance, data.RowIDProperty, element, "Yes");
                }
            }
        }
    }
    else {
        var targetRow = currentInstance.data.filter(function (dt) {
            return dt[ebInstance.KeyName.CostShareGroup] == data[ebInstance.KeyName.CostShareGroup]
            && dt[ebInstance.KeyName.SameAsNetworkTier] == data[ebInstance.repeaterElementName.NetworkTier];
        })

        if (targetRow.length > 0) {
            $.each(targetRow, function (idx, row) {
                if (row[ebInstance.KeyName.SameAsCostShare].indexOf(currentInstance.columnName) != -1) {
                    row[currentInstance.columnName] = data[currentInstance.columnName];

                    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
                    gridOptions.api.updateRowData({ update: [row] });
                }
            })
        }
    }
}

expressionBuilderExtension.prototype.filterDropdownSameAsTier = function (currentInstance, event) {
    var ebInstance = this;
    var rowData = event.data;
    var elementDesign = currentInstance.design.Elements.filter(function (dt) {
        if (dt.GeneratedName == currentInstance.columnName) {
            return dt;
        }
    })

    if (elementDesign.length != 0) {

        var targetRow = currentInstance.data.filter(function (dt) {
            return dt[ebInstance.KeyName.CostShareGroup] == rowData[ebInstance.KeyName.CostShareGroup]
            && dt[ebInstance.KeyName.SameAsNetworkTier] == rowData[ebInstance.repeaterElementName.NetworkTier];
        });

        var nonAllow = new Array();
        $.each(elementDesign[0].Items, function (idx, dt) {
            var result = ebInstance.isTierIsFollowing(ebInstance, currentInstance.data, dt.ItemText, rowData, nonAllow);
            if (result != null && result != undefined && result.length > 0) {
                $.each(result, function (ind, nt) {
                    if (nonAllow.indexOf(nt) == -1) {
                        nonAllow.push(nt[ebInstance.KeyName.NetworkTier]);
                    }
                });
            }
        });
        if (targetRow.length == 0) {
            $.each(elementDesign[0].Items, function (idx, dt) {
                if (dt.ItemText == rowData[ebInstance.repeaterElementName.NetworkTier] || ebInstance.isNonAllowedTierFound(nonAllow, dt.ItemText)) {
                    $(currentInstance.gridElementIdJQ).find('.Dropdown').find('option[value=  "' + dt.ItemText + '"  ]').remove();
                }
            })
        }
        else {
            $.each(elementDesign[0].Items, function (idx, dt) {
                if (dt.ItemText != "Not Applicable") {
                    $(currentInstance.gridElementIdJQ).find('.Dropdown').find('option[value=  "' + dt.ItemText + '"  ]').remove();
                }
            })
        }
    }
}

expressionBuilderExtension.prototype.isSameNetworkAdded = function (ebInstance, currentInstance, rowId, element, newValue, event) {
    if (currentInstance.data.length > 0) {
        var sourceRow = currentInstance.data.filter(function (dt) {
            return dt[element.GeneratedName] == newValue && dt["RowIDProperty"] != rowId;
        });
        if (sourceRow.length > 0) {
            var error = Common.CanNotBeDuplicated.replace("{0}", element.Label);
            messageDialog.show(error);
            var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
            var rowNode = gridOptions.api.getRowNode(rowId);
            if (rowNode != undefined) {
                var rowData = rowNode.data;
                rowData[element.GeneratedName] = "";
                rowNode.setData(rowData);
            }
        }
    }
}
expressionBuilderExtension.prototype.IsSameCoverage = function (ebInstance, currentInstance, rowId, element, newValue, event) {
    if (currentInstance.data.length > 0) {
        var sourceRow = currentInstance.data.filter(function (dt) {
            return dt[element.GeneratedName] == newValue && dt["RowIDProperty"] != rowId;
        });
        if (sourceRow.length > 0) {
            var error = Common.CanNotBeDuplicated.replace("{0}", element.Label);
            messageDialog.show(error);
            var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
            var rowNode = gridOptions.api.getRowNode(rowId);
            if (rowNode != undefined) {
                var rowData = rowNode.data;
                rowData[element.GeneratedName] = "";
                rowNode.setData(rowData);
            }
        }
    }
}

expressionBuilderExtension.prototype.BulkUpdateLimitInformationDetailRepeaterData = function (operationType, currentInstance) {
    var LTSERepeaterData = currentInstance.repeaterBuilder.formInstanceBuilder.formData.Limits.LimitInformationDetail;
    var LTSERecordCount = LTSERepeaterData.length;
    var oldData = currentInstance.repeaterBuilder.data;
    var repeaterSelectedData = [];
    var selectedData = new Object();
    selectedData.list = new Array();
    var ebsInstance = this;
    var servicesToAddDelete = [];
    var allRowData = $(currentInstance.elementIDs.manualGridJQ).jqGrid('getGridParam', 'data');
    var k = 0;
    for (var i = 0 ; i < allRowData.length; i++) {
        if (allRowData[i].IsSelect == "Yes" && currentInstance.sourceDataList[i] != undefined) {
            selectedData.list[k] = currentInstance.sourceDataList[i];
            delete allRowData[i];
            delete selectedData.list[k]['IsSelect'];
            k++;
        }
    }
    var prevCount = 0;
    var count = 0;
    var rowCount = 0;
    for (j = 0; j < selectedData.list.length ; j++) {
        if (currentInstance.repeaterBuilder.data.length > 0) {
            for (i = 0; i < currentInstance.repeaterBuilder.data.length; i++) {
                rowCount = 0;
                count = 0;
                for (var key in currentInstance.repeaterBuilder.data[i]) {
                    if (selectedData.list[j][key] != undefined && $.type(selectedData.list[j][key]) === "string") {
                        rowCount++;
                        if (selectedData.list[j][key] == currentInstance.repeaterBuilder.data[i][key]) {
                            count++;
                        }
                    }
                }
                if (rowCount == count) {
                    prevCount = i + 1;
                    var repeaterData = {};
                    $.each(currentInstance.repeaterBuilder.data[i], function (idx, ele) {
                        if (idx != 'RowIDProperty')
                            repeaterData[idx] = currentInstance.repeaterBuilder.data[i][idx];
                    });
                    repeaterSelectedData.push(repeaterData);
                    var length = currentInstance.repeaterBuilder.data.length;
                    break;
                }
            }
            if (count < rowCount) {
                currentInstance.AddSelectedRowData(repeaterSelectedData, selectedData.list[j]);
            }
        }
        else {
            currentInstance.AddSelectedRowData(repeaterSelectedData, selectedData.list[j]);
        }
    }

    if (repeaterSelectedData != undefined && repeaterSelectedData != null && repeaterSelectedData.length > 0) {
        for (var i = 0; i < repeaterSelectedData.length; i++) {
            repeaterSelectedData[i]["RowIDProperty"] = i;
        }
    }

    $.each(repeaterSelectedData, function (idx, row) {
        servicesToAddDelete.push(row);
        //Activity Log for Added Rows
    });

    var LTSEgridData = [];

    var LTSEgridData = currentInstance.repeaterBuilder.parentFilterData;

    currentInstance = this.setLimitInformationDetailRepeaterDataProperty(currentInstance.repeaterBuilder.fullName, servicesToAddDelete, LTSEgridData, operationType, currentInstance);
}

expressionBuilderExtension.prototype.setLimitInformationDetailRepeaterDataProperty = function (fullName, data, LTSEgridData, operation, currentInstance) {
    var ebInstance = this;
    var formData;
    var dataSourceName;
    var fullpath = fullName.split('.');
    var rowPropertyIds = [];
    var filterCondtion = currentInstance.repeaterBuilder.parentFilterConstion;
    var message;
    if (currentInstance.repeaterBuilder.gridType == "child") {
        dataSourceName = fullpath[fullpath.length - 1];
        fullpath.pop();
    }

    var addedData = "";
    for (var idx = 0; idx < LTSEgridData.length; idx++) {
        rowPropertyIds.push(LTSEgridData[idx].RowIDProperty);
    }

    for (var index = 0; index < fullpath.length; index++) {
        if (index == 0) {
            formData = currentInstance.repeaterBuilder.formInstanceBuilder.formData[fullpath[index]];
        }
        else if (index == (fullpath.length - 1)) {
            if (currentInstance.repeaterBuilder.gridType == "parent") {
                formData[fullpath[index]] = data;
            }
            else {
                var LTSEMasterListData = formData[fullpath[index]].filter(function (row) {
                    return $.inArray(row.RowIDProperty, rowPropertyIds);
                });

                if (operation == 'ADD') {
                    var isRowAdded = false;
                    $.each(formData[fullpath[index]], function (id, row) {
                        for (var idx = 0; idx < rowPropertyIds.length; idx++) {
                            if (row.RowIDProperty == rowPropertyIds[idx]) {
                                //Append to existing services.
                                if (row[dataSourceName] != null && row[dataSourceName].length > 0) {
                                    for (var dataLen = 0 ; dataLen < data.length ; dataLen++) {
                                        var isRowExists = false;
                                        for (var mstrService = 0; mstrService < row[dataSourceName].length; mstrService++) {
                                            if (data[dataLen]["BenefitServiceCode"] == row[dataSourceName][mstrService]["BenefitServiceCode"]) {
                                                isRowExists = true;
                                            }
                                        }
                                        if (isRowExists == false) {
                                            var maxRowIDProperty = row[dataSourceName].length;
                                            data[dataLen].RowIDProperty = maxRowIDProperty;
                                            row[dataSourceName].push(data[dataLen]);
                                            isRowAdded = true;
                                        }
                                    }
                                }
                                else {
                                    row[dataSourceName] = data;
                                    isRowAdded = true;
                                }
                                break;
                            }
                        }
                    });
                    if (isRowAdded) {
                        messageDialog.show(LimitBulkUpdate.Recordsaddedsuccessfully);
                        var activityLogMsg = addedData;
                        this.activitylogForLimitInformationDetailBulkUpdate(currentInstance, activityLogMsg, operation, filterCondtion);
                    }
                    else if (data.length > 0)
                        messageDialog.show(LimitBulkUpdate.Norecordsareaddedfortheselectedservice);
                }
                else {
                    var isRowDeleted = false;
                    $.each(formData[fullpath[index]], function (id, row) {
                        for (var idx = 0; idx < rowPropertyIds.length; idx++) {
                            if (row.RowIDProperty == rowPropertyIds[idx]) {
                                if (row[dataSourceName].length > 0) {
                                    for (var dataLen = 0 ; dataLen < data.length ; dataLen++) {
                                        for (var mstrService = 0; mstrService < row[dataSourceName].length; mstrService++) {
                                            if (data[dataLen]["BenefitServiceCode"] == row[dataSourceName][mstrService]["BenefitServiceCode"]) {
                                                row[dataSourceName].splice(mstrService, 1);
                                                isRowDeleted = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    });
                    if (isRowDeleted) {
                        messageDialog.show(LimitBulkUpdate.Recordsdeletedsuccessfully);
                        var activityLogMsg = addedData;
                        this.activitylogForLimitInformationDetailBulkUpdate(currentInstance, activityLogMsg, operation, filterCondtion);
                    }
                    else if (data.length > 0)
                        messageDialog.show(LimitBulkUpdate.Norecordsareavailabletodeletefortheselectedservice);
                }
            }
        }
        else {
            formData = formData[fullpath[index]];
        }
    }
    return currentInstance;

}

expressionBuilderExtension.prototype.IsRecordExists = function (compareElement, ComparerList, dataSourceMapping) {

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

expressionBuilderExtension.prototype.activitylogForLimitInformationDetailBulkUpdate = function (currentInstance, selectedServices, operationType, filterCondtion) {
    var ebInstance = this;
    var message = "Service(s) ";
    if (operationType == 'ADD')
        message += "added ";
    else if (operationType == 'DELETE')
        message += " deleted ";
    message += selectedServices + "where ";
    var i = 0;
    if (filterCondtion != null) {
        $.each(filterCondtion, function (id, val) {
            var field = id;
            var value = filterCondtion[id]

            if (i == 0) {
                message = message.concat(field + " is " + value);
            }
            else {
                message = message.concat(" and " + field + " is " + value);
            }
            i++;
        });
    }

    //selectedServices = selectedServices.slice(",", -1);
    var activityLogMsg = message;

    currentInstance.repeaterBuilder.addEntryToAcitivityLogger(undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, activityLogMsg);
}

expressionBuilderExtension.prototype.setUpdatedRowInCurrentInstance = function (currentInstance, ebInstance, rowId, updatedRow) {
    if (currentInstance != null && currentInstance != undefined) {
        var targetRepeaterData = currentInstance.data;
        for (var i = 0; i < targetRepeaterData.length; i++) {
            if (rowId == targetRepeaterData[i].RowIDProperty) {
                targetRepeaterData[i] = updatedRow;
                break;
            }
        }
        if (currentInstance.fullName == ebInstance.reapterFullName.COMMERCIALMEDICAL.CostShareGroupList) {
            updatedRow = ebInstance.setSameAsCostShareValue(ebInstance, currentInstance, rowId, updatedRow);
        }
        var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
        gridOptions.rowData = targetRepeaterData;
        currentInstance.data = targetRepeaterData;
        gridOptions.api.setRowData(gridOptions.rowData);
        ebInstance.removeCellStyle(currentInstance, rowId);
    }
}

expressionBuilderExtension.prototype.filterDropdownSameAsCostShare = function (currentInstance, event) {
    var ebInstance = this;
    var rowData = event.data;
    var targetRow = null;
    var elementDesign = currentInstance.design.Elements.filter(function (dt) {
        if (dt.GeneratedName == currentInstance.columnName) {
            return dt;
        }
    });

    if (elementDesign.length != 0) {
        targetRow = currentInstance.data.filter(function (dt) {
            return dt["RowIDProperty"] == rowData["RowIDProperty"];
        })

        var costShareType = ebInstance.getCostShareType(ebInstance, currentInstance, rowData);

        if (targetRow != null && targetRow != undefined) {
            $(":checkbox[value=0]").parent().parent().hide();
            if (elementDesign[0].Items != null && elementDesign[0].Items != undefined) {
                for (var index = 0; index < elementDesign[0].Items.length; index++) {
                    if (elementDesign[0].Items[index].ItemValue != 'NotApplicable') {
                        $(":checkbox[value=" + elementDesign[0].Items[index].ItemValue + "]").parent().parent().hide();
                    }
                }
                if (costShareType.indexOf('Copay') != -1) {
                    $(":checkbox[value=CopayValue]").parent().parent().show();
                    $(":checkbox[value=CopayFrequency]").parent().parent().show();
                    $(":checkbox[value=CopaySupplement]").parent().parent().show();
                }
                if (costShareType.indexOf('Coinsurance') != -1) {
                    $(":checkbox[value=CoinsuranceAmount]").parent().parent().show();
                }
                if (costShareType.indexOf('Deductible') != -1) {
                    $(":checkbox[value=Deductible]").parent().parent().show();
                }
                $(':checkbox[value=Deductible]').change(function () {
                    ebInstance.setNotApplicable($(this).is(":checked"));
                });

                $(':checkbox[value=CoinsuranceAmount]').change(function () {
                    ebInstance.setNotApplicable($(this).is(":checked"));
                });

                $(':checkbox[value=CopayValue]').change(function () {
                    ebInstance.setNotApplicable($(this).is(":checked"));
                });

                $(':checkbox[value=CopayFrequency]').change(function () {
                    ebInstance.setNotApplicable($(this).is(":checked"));
                });

                $(':checkbox[value=CopaySupplement]').change(function () {
                    ebInstance.setNotApplicable($(this).is(":checked"));
                });

                $(':checkbox[value=NotApplicable]').change(function () {
                    if ($(this).is(":checked")) {
                        $.each(elementDesign[0].Items, function (idx, val) {
                            if (val.ItemValue != "NotApplicable") {
                                $(":checkbox[value=" + val.ItemValue + "]").prop('checked', false);
                            }
                        });
                    }
                });
            }
        }
    }
}

expressionBuilderExtension.prototype.setSameAsCostShareValue = function (ebInstance, currentInstance, rowId, updatedRow) {
    var TempSameAsCostShare = "";
    var costShareType = ebInstance.getCostShareType(ebInstance, currentInstance, updatedRow);
    var colArray = ["CopayValue", "CopayFrequency", "CopaySupplement", "CoinsuranceAmount"];
    if (updatedRow != null && updatedRow != undefined) {
        if (updatedRow[ebInstance.KeyName.SameAsCostShare] != undefined && updatedRow[ebInstance.KeyName.SameAsCostShare] != null
            && updatedRow[ebInstance.KeyName.SameAsCostShare] != "" && updatedRow[ebInstance.KeyName.SameAsCostShare] != "Not Applicable"
            ) {
            if (updatedRow[ebInstance.KeyName.SameAsCostShare].indexOf('NotApplicable') != -1) {

                TempSameAsCostShare = "NotApplicable";
            }
            else {
                if (costShareType.indexOf('Copay') != -1) {
                    if (updatedRow[ebInstance.KeyName.SameAsCostShare].indexOf('CopayValue') != -1) {
                        if (TempSameAsCostShare != "") {
                            TempSameAsCostShare = TempSameAsCostShare + ",";
                        }
                        TempSameAsCostShare = TempSameAsCostShare + "CopayValue";
                    }
                    if (updatedRow[ebInstance.KeyName.SameAsCostShare].indexOf('CopayFrequency') != -1) {

                        if (TempSameAsCostShare != "") {
                            TempSameAsCostShare = TempSameAsCostShare + ",";
                        }
                        TempSameAsCostShare = TempSameAsCostShare + "CopayFrequency";
                    }
                    if (updatedRow[ebInstance.KeyName.SameAsCostShare].indexOf('CopaySupplement') != -1) {

                        if (TempSameAsCostShare != "") {
                            TempSameAsCostShare = TempSameAsCostShare + ",";
                        }
                        TempSameAsCostShare = TempSameAsCostShare + "CopaySupplement";
                    }
                }
                if (costShareType.indexOf('Coinsurance') != -1) {
                    if (updatedRow[ebInstance.KeyName.SameAsCostShare].indexOf('Coinsurance') != -1) {
                        if (TempSameAsCostShare != "") {
                            TempSameAsCostShare = TempSameAsCostShare + ",";
                        }
                        TempSameAsCostShare = TempSameAsCostShare + "CoinsuranceAmount";
                    }
                }

                if (costShareType.indexOf('Deductible') != -1) {
                    if (updatedRow[ebInstance.KeyName.SameAsCostShare].indexOf('Deductible') != -1) {
                        if (TempSameAsCostShare != "") {
                            TempSameAsCostShare = TempSameAsCostShare + ",";
                        }
                        TempSameAsCostShare = TempSameAsCostShare + "Deductible";
                    }
                }
            }
            updatedRow.SameAsCostShare = TempSameAsCostShare;

            //$.each(colArray, function (int, val) {
            //    if (updatedRow.SameAsCostShare.indexOf(val) == -1) {
            //        try {
            //            updatedRow[val] = "Not Applicable";
            //        }
            //        catch (ex) {

            //        }
            //    }
            //});
        }
        ebInstance.updateCostShareGroupListData(ebInstance, currentInstance, updatedRow);
        return updatedRow;
    }
}

expressionBuilderExtension.prototype.removeCellStyle = function (currentInstance, rowId) {
    currentInstance.cellRules.cellEnableStyle = currentInstance.cellRules.cellEnableStyle.filter(function (dt) {
        if (dt.rowIndex != rowId) {
            return dt;
        }
    });
    currentInstance.cellRules.cellErrorStyle = currentInstance.cellRules.cellErrorStyle.filter(function (dt) {
        if (dt.rowIndex != rowId) {
            return dt;
        }
    });
    currentInstance.cellRules.cellHighlightStyle = currentInstance.cellRules.cellHighlightStyle.filter(function (dt) {
        if (dt.rowIndex != rowId) {
            return dt;
        }
    });
}

expressionBuilderExtension.prototype.runExpressionRuleOnCellChange = function (currentInstance, ebInstance, rowId, postData) {
    if (postData != null && postData != undefined) {
        var promise = ajaxWrapper.postJSON(ebInstance.URLs.getRuntimeEBResult, postData);
        promise.done(function (result) {
            try {
                var csgroupRowData = [];
                csgroupRowData = JSON.parse(result);
                ebInstance.setUpdatedRowInCurrentInstance(currentInstance, ebInstance, rowId, csgroupRowData[0]);

                var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
                gridOptions.api.forEachNode(function (rowNode, index) {
                    if (rowNode.data.RowIDProperty == rowId) {
                        rowNode.setSelected(true, true);
                        return;
                    }
                });
            }
            catch (ex) {

            }
        });
        promise.fail(showError);
    }
}

expressionBuilderExtension.prototype.setNotApplicable = function () {
    if ($(':checkbox[value=CopayValue]').is(":checked") || $(':checkbox[value=CoinsuranceAmount]').is(":checked")
        || $(':checkbox[value=Deductible]').is(":checked") || $(':checkbox[value=CopayFrequency]').is(":checked")
        || $(':checkbox[value=CopaySupplement]').is(":checked")
        ) {
        $(':checkbox[value=NotApplicable]').prop('checked', false);
        $(':checkbox[value=NotApplicable]').prop('disabled', true);
    }
    else {
        $(':checkbox[value=NotApplicable]').prop('checked', true);
        $(':checkbox[value=NotApplicable]').prop('disabled', true);
    }
    $(':checkbox[value=NotApplicable]').trigger('change');
}

expressionBuilderExtension.prototype.isRecalculationIconVisibled = function (path) {

    var result = false;
    switch (path) {
        case "CoverageExample.CoverageExampleDiabetic":
        case "CoverageExample.CoverageExampleFracture":
        case "CoverageExample.CoverageExampleMaternity":
            result = true;
            break;
    }
    return result;
}

expressionBuilderExtension.prototype.processSBCCalculation = function (currentInstance) {
    var ebInstance = this;
    URLs = {
        ProcessRuleForSameSection: "/ExpressionBuilder/ProcessEBSRuleForSameSection"
    };

    var formInstanceData = formInstancebuilder.form.getFormInstanceData();
    formInstanceData.formInstanceData = formInstanceData.formInstanceData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');
    var sectionObj = JSON.parse(formInstanceData.formInstanceData);
    sectionObj[currentInstance.design.GeneratedName] = currentInstance.data;
    var input = {
        formDesignVersionId: formInstancebuilder.designData.FormDesignVersionId,
        folderVersionId: formInstancebuilder.folderVersionId,
        formInstanceId: formInstancebuilder.formInstanceId,
        sourceElementPath: currentInstance.fullName,
        ElementValue: currentInstance.data,
        sectionData: JSON.stringify(sectionObj),
        isMultiselect: false
    }
    var isDataChange = false;
    if (currentInstance.data.length > 0 && currentInstance.orignalData.length > 0) {
        isDataChange = true;
        //if (JSON.stringify(currentInstance.data) != JSON.stringify(currentInstance.orignalData))
        //{
        //    isDataChange = true;
        //}
    }

    if (isDataChange) {
        var promise = ajaxWrapper.postJSON(URLs.ProcessRuleForSameSection, input);
        promise.done(function (xhr) {
            try {
                if (xhr != null) {
                    if (xhr != "[]") {
                        var res = JSON.parse(xhr);
                        $.each(res, function (idx, val) {
                            var reptrInstance = null;
                            reptrInstance = formInstancebuilder.repeaterBuilders.filter(function (dt) {
                                return dt.fullName == val.TargetPath;
                            })
                            $.each(formInstancebuilder.repeaterBuilders, function (index, value) {
                                if (value.fullName == val.TargetPath) {
                                    value.data = val.Data;
                                    return false;
                                }
                            });
                            if (reptrInstance.length != 0) {
                                var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                                gridOptions.rowData = val.Data;
                                gridOptions.api.setRowData(gridOptions.rowData);

                                var obj = {};
                                obj[val.TargetPath] = val.Data;
                                $.observable(formData).setProperty(obj);

                                //currentInstance.orignalData = val.Data
                            }
                        });
                    }
                }

            }
            catch (ex) {

            }
        });
    }
    else {
        messageDialog.show(SBCCalculator.NoChangeFound);
    }
}

expressionBuilderExtension.prototype.setAGGridColSelection = function (currentInstance, rowId, columnName) {
    var rowNodeFound = {};
    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
    gridOptions.api.forEachNode(function (rowNode, index) {
        if (rowNode.data.RowIDProperty == rowId) {
            isRowIDExist = index;
            rowNodeFound = rowNode;
            rowNode.setSelected(true, true);
            return;
        }
    });
    if (rowNodeFound) {
        var colindex = undefined;
        var col = currentInstance.colModel;
        $.each(col, function (idx, ct) {
            if (ct.dataIndx == columnName) {
                colindex = idx;
                return false;
            }
        });

        GridApi.setCellFocus(currentInstance, rowNodeFound, columnName, colindex);
    }
}

expressionBuilderExtension.prototype.filterProcesssingRuleItems = function (currentInstance, rowData) {
    var applicableOptionList = new Array();
    var rowIndex = 0;

    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
    gridOptions.api.forEachNode(function (rowNode, index) {
        if (rowNode.data.RowIDProperty == rowData.RowIDProperty) {
            rowIndex = rowNode.rowIndex;
            return;
        }
    });

    var processingRule = {
        NoRule: "No Rule",
        AllCopay: "All Copay",
        AllCoins: "All Coinsurance",
        AllDeductible: "All Deductible",
        FirstDedThenCopay: "First Deductible Then Copay",
        FirstCoinThenDed: "First Coinsurance Then Deductible",
        FirstDeductibleThenCoinsurance: "First Deductible Then Coinsurance",
        FirstCopayThenDeductible: "First Copay Then Deductible",
        FirstCopayDeductibleThenCoinsurance: "First Copay, Deductible Then Coinsurance",
        CopayThenCoinsurance: "Copay Then Coinsurance"
    }

    if (rowData != null) {

        var Copay = rowData["Copay"];
        var Coinsurance = rowData["Coinsurance"];
        var Deductible = rowData["Deductible"];
        var existingProcessingRule = rowData["ProcessingRule"];
        if (Copay.indexOf('$') != -1) {
            applicableOptionList.push(processingRule.AllCopay);
        }
        if (Coinsurance.indexOf('%') != -1) {
            applicableOptionList.push(processingRule.AllCoins);
        }
        if (Deductible.indexOf('$') != -1) {
            applicableOptionList.push(processingRule.AllDeductible);
        }
        if (Coinsurance.indexOf('%') != -1 && Deductible.indexOf('$') != -1) {
            applicableOptionList.push(processingRule.FirstDeductibleThenCoinsurance);
            applicableOptionList.push(processingRule.FirstCoinThenDed);
        }
        if (Copay.indexOf('$') != -1 && Deductible.indexOf('$') != -1) {
            applicableOptionList.push(processingRule.FirstCopayThenDeductible);
            applicableOptionList.push(processingRule.FirstDedThenCopay);
        }
        if (Copay.indexOf('$') != -1 && Coinsurance.indexOf('%') != -1 && Deductible.indexOf('$') != -1) {
            applicableOptionList.push(processingRule.FirstCopayDeductibleThenCoinsurance);
        }
        if (Copay.indexOf('$') != -1 && Coinsurance.indexOf('%') != -1) {
            applicableOptionList.push(processingRule.CopayThenCoinsurance);
        }
        applicableOptionList.push(processingRule.NoRule);
        var applicableRuleItems = new Array();

        var itemshtml = "", setExistingRule = false;
        $.each(applicableOptionList, function (index, value) {
            if (existingProcessingRule == value) {
                setExistingRule = true;
            }
            itemshtml += "<option value='" + value + "'>" + value + "</option>";
        });
        var elemetid = "#idProcessingRule_" + rowIndex;
        $(elemetid).html(itemshtml);
        if (setExistingRule == true) {
            $(elemetid).val(existingProcessingRule);
        }
    }
}

expressionBuilderExtension.prototype.runExpressionRuleOnCellClick = function (currentInstance, rowData, columnName) {
    var paramArray = [];
    ebsExtInstance = this;
    var elementDesign = currentInstance.design.Elements.filter(function (ct) {
        return ct.GeneratedName == columnName;
    });
    var items = elementDesign[0].Items;
    paramArray[0] = JSON.stringify(rowData);
    var obj = {};
    var postData = {
        sourceElementPath: currentInstance.fullName + "." + columnName,
        folderVersionId: currentInstance.formInstanceBuilder.folderVersionId,
        formInstanceId: currentInstance.formInstanceId,
        formDesignVersionId: currentInstance.formInstanceBuilder.formDesignVersionId,
        paramArray: paramArray,
    }

    if (postData != null && postData != undefined) {
        var promise = ajaxWrapper.postAsyncJSONCustom(ebsExtInstance.URLs.getRuntimeEBResult, postData);
        promise.done(function (result) {
            dataList = JSON.parse(result);
            itemList = new Array();
            $.each(dataList, function (ind, obj) {
                { itemList.push(obj); }
            });
            obj = ebsExtInstance.bindCustomDropDownItems(currentInstance, itemList, columnName, rowData);
        });
        promise.fail(showError);
    }
    return obj;
}

expressionBuilderExtension.prototype.getCostShareType = function (ebInstance, currentInstance, currentSelectedRow) {
    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
    currentInstance.data = gridOptions.rowData;
    var csType = "";
    if (currentSelectedRow.SameAsNetworkTier != undefined && currentSelectedRow.SameAsNetworkTier != "" && currentSelectedRow.SameAsNetworkTier != "Not Applicable") {
        var FollowTierRow = currentInstance.data.filter(function (dt) {
            return dt.RowIDProperty != currentSelectedRow.RowIDProperty &&
                   dt.NetworkTier == currentSelectedRow.SameAsNetworkTier &&
                   dt.CostShareGroup == currentSelectedRow.CostShareGroup;
        });
        if (FollowTierRow.length > 0) {
            csType = ebInstance.getCostShareIntersect(FollowTierRow[0].CostShareType, currentSelectedRow.CostShareType);
        }
        else {
            csType = currentSelectedRow.CostShareType;
        }
    }
    else {
        csType = currentSelectedRow.CostShareType;
    }
    return csType;
}

expressionBuilderExtension.prototype.updateFollowsTierValues = function (ebInstance, currentInstance, currentSelectedRow) {
    var costShareType = "", TempSameAsCostShare = "";
    var CsType = ["Copay", "Coinsurance", "Deductible"];
    var colArray = ["CopayValue", "CopayFrequency", "CopaySupplement"];
    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
    currentInstance.data = gridOptions.rowData;
    var FollowTierRows = currentInstance.data.filter(function (dt) {
        return dt.RowIDProperty != currentSelectedRow.RowIDProperty &&
               dt.SameAsNetworkTier == currentSelectedRow.NetworkTier &&
               dt.CostShareGroup == currentSelectedRow.CostShareGroup;
    });
    if (FollowTierRows.length > 0) {
        $.each(FollowTierRows, function (index, FollowTierRow) {
            TempSameAsCostShare = "";
            costShareType = ebInstance.getCostShareIntersect(FollowTierRow["CostShareType"], currentSelectedRow["CostShareType"]);
            if (costShareType == undefined && costShareType == "") {
                FollowTierRow[ebInstance.KeyName.CopayValue] = "Not Applicable";
                FollowTierRow[ebInstance.KeyName.CopayFrequency] = "Not Applicable";
                FollowTierRow[ebInstance.KeyName.CopaySupplement] = "Not Applicable";
                FollowTierRow[ebInstance.KeyName.CoinsuranceAmount] = "Not Applicable";
            }
            else {
                for (ind = 0; ind < CsType.length; ind++) {
                    var val = CsType[ind];
                    if (costShareType.indexOf(val) != -1) {
                        if (FollowTierRow[ebInstance.KeyName.SameAsCostShare].indexOf(val) != -1) {
                            if (val == "Copay") {
                                $.each(colArray, function (int, colVal) {
                                    if (FollowTierRow[ebInstance.KeyName.SameAsCostShare].indexOf(colVal) != -1) {
                                        if (TempSameAsCostShare == "") {
                                            TempSameAsCostShare = colVal;
                                        }
                                        else {
                                            TempSameAsCostShare = TempSameAsCostShare + "," + colVal;
                                        }
                                    }
                                });
                            }
                            else if (val == "Coinsurance") {
                                if (FollowTierRow[ebInstance.KeyName.SameAsCostShare].indexOf(val) != -1) {
                                    if (TempSameAsCostShare == "") {
                                        TempSameAsCostShare = "CoinsuranceAmount";
                                    }
                                    else {
                                        TempSameAsCostShare = TempSameAsCostShare + "," + "CoinsuranceAmount";
                                    }
                                }
                            }
                            else if (val == "Deductible") {
                                if (FollowTierRow[ebInstance.KeyName.SameAsCostShare].indexOf(val) != -1) {
                                    if (TempSameAsCostShare == "") {
                                        TempSameAsCostShare = "Deductible";
                                    }
                                    else {
                                        TempSameAsCostShare = TempSameAsCostShare + "," + "Deductible";
                                    }
                                }
                            }
                        }
                    }
                    else {
                        if (val == "Copay") {
                            FollowTierRow[ebInstance.KeyName.CopayValue] = "Not Applicable";
                            FollowTierRow[ebInstance.KeyName.CopayFrequency] = "Not Applicable";
                            FollowTierRow[ebInstance.KeyName.CopaySupplement] = "Not Applicable";
                        }
                        else if (val == "Coinsurance") {
                            FollowTierRow[ebInstance.KeyName.CoinsuranceAmount] = "Not Applicable";
                            FollowTierRow[ebInstance.repeaterElementName.RefreshCoinsurance] = "No";
                        }
                    }
                }
            }
            FollowTierRow[ebInstance.KeyName.SameAsCostShare] = TempSameAsCostShare != undefined ? TempSameAsCostShare : "";
            if (currentInstance != null && currentInstance != undefined) {
                ebInstance.updateCostShareGroupListData(ebInstance, currentInstance, FollowTierRow);
                ebInstance.updateCostShareGroupListData(ebInstance, currentInstance, currentSelectedRow);
                ebInstance.removeCellStyle(currentInstance, FollowTierRow.RowIDProperty);
            }
        });
    }
    else {
        if (currentSelectedRow != undefined) {
            ebInstance.setSameAsCostShareValue(ebInstance, currentInstance, currentSelectedRow.RowIDProperty, currentSelectedRow);
        }
    }

}

expressionBuilderExtension.prototype.getCostShareIntersect = function (dependentCsType, currentCsType) {
    var csType = "";
    if (dependentCsType != undefined && dependentCsType != "" && currentCsType != undefined && currentCsType != "") {

        var dependentCsTypeArray = new Array();

        if (dependentCsType.indexOf("Copay") != -1 && currentCsType.indexOf("Copay") != -1) {
            dependentCsTypeArray.push("Copay");
        }
        if (dependentCsType.indexOf("Coinsurance") != -1 && currentCsType.indexOf("Coinsurance") != -1) {
            dependentCsTypeArray.push("Coinsurance");
        }
        if (dependentCsType.indexOf("Deductible") != -1 && currentCsType.indexOf("Deductible") != -1) {
            dependentCsTypeArray.push("Deductible");
        }
        csType = dependentCsTypeArray.join(",");
    }
    return csType;
}

expressionBuilderExtension.prototype.updateCostShareGroupListData = function (ebInstance, currentInstance, row) {

    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
    currentInstance.data = gridOptions.rowData;

    var targetRepeaterData = currentInstance.data;
    for (var i = 0; i < targetRepeaterData.length; i++) {
        if (row["CostShareGroup"] == targetRepeaterData[i].CostShareGroup && row["NetworkTier"] == targetRepeaterData[i].NetworkTier) {
            targetRepeaterData[i] = row;
            break;
        }
    }
    gridOptions.rowData = targetRepeaterData;
    currentInstance.data = targetRepeaterData;
    gridOptions.api.setRowData(gridOptions.rowData);

    if (currentInstance.formInstanceBuilder != undefined) {
        currentInstance.formInstanceBuilder.formData[ebInstance.sectionName.CascadingCostShare][ebInstance.sectionName.CostShareGroup][ebInstance.repeaterName.CostShareGroupList] = targetRepeaterData;
    }
}

expressionBuilderExtension.prototype.isTierIsFollowing = function (ebInstance, data, Tier, rowData) {
    var targetRows = null;
    if (Tier != "Not Applicable") {
        targetRows = currentInstance.data.filter(function (dt) {
            return dt[ebInstance.KeyName.CostShareGroup] == rowData[ebInstance.KeyName.CostShareGroup]
            && dt[ebInstance.KeyName.SameAsNetworkTier] == Tier;
        });
    }
    return targetRows;
}

expressionBuilderExtension.prototype.isNonAllowedTierFound = function (list, Tier) {
    var result = false;
    $.each(list, function (list, val) {
        if (val == Tier) {
            result = true;
            return;
        }
    });
    return result;
}

expressionBuilderExtension.prototype.bindCustomDropDownItems = function (currentInstance, itemsList, columnName, rowData) {
    ebsExtInstance = this;
    var itemsObject = {};
    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
    var dd = [];
    optionsList = new Array();
    $.each(itemsList, function (i, v) {
        var obj = { Enabled: null, ItemID: 0, ItemText: v[columnName], ItemValue: v[columnName] };
        optionsList.push(obj);
    });
    var elementDesign = currentInstance.design.Elements.filter(function (ct) {
        return ct.GeneratedName == columnName;
    });
    elementDesign[0].Items = optionsList;
    itemsObject["optionsList"] = optionsList;
    var dd = [];
    var options = "";
    var items = elementDesign[0].Items;
    if (items != undefined && items != null && items.length > 0) {
        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                var d = {};
                d[items[idx].ItemValue] = items[idx].ItemText == null ? items[idx].ItemValue : items[idx].ItemText;
                dd.push(d);
                options = options + items[idx].ItemValue + ':' + (items[idx].ItemText == null ? items[idx].ItemValue : items[idx].ItemText);
                if (idx < items.length - 1) {
                    options = options + ';';
                }
            }
        }
    }
    itemsObject["Values"] = dd;
    $.each(currentInstance.colModel, function (index, model) {
        if (model.dataIndx == columnName) {
            model.editor.options = options;
        }
    });
    return itemsObject;
}

expressionBuilderExtension.prototype.generateGuardrailsOptionsForRepeater = function (currentInstance, rowData, columnName) {
    ebsExtInstance = this;
    var prop = columnName.indexOf('Standard') != -1 ? columnName.replace('Standard', "") : columnName.replace('Allowable', "");
    currentInstance.guardrailsRepeaterInstance = {};
    currentInstance.guardrailsRepeaterInstance.guardrailsLowColumnName = prop + 'Low';
    currentInstance.guardrailsRepeaterInstance.guardrailsHighColumnName = prop + 'High';
    currentInstance.guardrailsRepeaterInstance.guardrailsIncrementColumnName = prop + 'Increment';
    currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName = "Allowable" + prop;
    currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName = "Standard" + prop;
    currentInstance.guardrailsRepeaterInstance.guardrailsGuardrailColumnName = prop.indexOf('Guardrail') != -1 ? prop : prop + 'Guardrail';
    var guardrailType = ebsExtInstance.getGuardrailType(currentInstance, rowData, true);
    var elementDesign = currentInstance.design.Elements.filter(function (ct) {
        return ct.GeneratedName == columnName;
    });
    var allow, standard;

    switch (guardrailType) {
        case "Standard":
            allow = rowData['Allowable' + prop];
            standard = rowData['Standard' + prop];
            if ((columnName.indexOf("Allowable") != -1)) {
                $(":checkbox[value=0]").parent().parent().hide();
                if (rowData[currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName] == undefined || rowData[currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName] == "") {
                    var itemsList = new Array();
                    var valList = new Array();;
                    $.each(elementDesign[0].Items, function (ind, val) {
                        if (val != undefined && val != "") {
                            var optionobj = {};
                            optionobj[columnName] = val.ItemValue;
                            itemsList.push(optionobj);
                            valList.push(val.ItemValue);
                        }
                    });
                    if (valList.length > 0) {
                        var result = ebsExtInstance.bindCustomDropDownItems(currentInstance, itemsList, columnName, rowData);
                        result.SelectedValues = valList.join(',');
                        return result;
                    }
                }
            }
            if (columnName.indexOf("Standard") != -1) {
                var itemsList = new Array();
                allowArr = currentInstance.expressionBuilderRulesExt.getMultiSelectKeyArray(allow);
                $.each(allowArr, function (ind, val) {
                    if (val != undefined && val != "") {
                        var optionobj = {};
                        optionobj[columnName] = val;
                        itemsList.push(optionobj);
                    }
                });
                return ebsExtInstance.bindCustomDropDownItems(currentInstance, itemsList, columnName, rowData);
            }
            break;
        case "Range":
            allow = rowData['Allowable' + prop];
            standard = rowData['Standard' + prop];
            var nonNumerical = ebsExtInstance.hasRangeGuardrails(currentInstance, prop);
            var Valid = ebsExtInstance.GuardrailsValidation(rowData, prop, elementDesign[0], nonNumerical);
            var ValidValues = ebsExtInstance.rangeGuardrailsValidValues(rowData, prop, elementDesign[0]);
            if (ValidValues.isValid && Valid.isValid) {
                var symbol = "";
                if (prop.indexOf('Interval') != -1) {
                    symbol = "";
                }
                else {
                    if (prop.indexOf('Coinsurance') != -1) {
                        symbol = '%';
                    }
                    else {
                        symbol = '$';
                    }
                }
                var result = ebsExtInstance.generateOptionRange(ebsExtInstance, ValidValues.low, ValidValues.hight, ValidValues.increment, symbol);
                var itemsList = new Array();
                if (columnName == currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName) {
                    $.each(currentInstance.colModel, function (index, model) {
                        if (model.dataIndx == columnName) {
                            model.editor.options = result["agGridOptions"];
                        }
                    });
                    elementDesign[0].Items = result["DesignArray"];
                    var resultObj = {
                        Values: result["agGridOptions"]
                    };

                    if (rowData[currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName] == undefined || rowData[currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName] == "") {
                        resultObj.SelectedValues = result["ValueArray"].join(',');
                    }
                    return resultObj;
                }
                if (columnName == currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName) {
                    if (allow != undefined && allow != "") {
                        var splitBy = "";
                        if (symbol == '$') {
                            splitBy = ",$";
                        }
                        else {
                            splitBy = ",";
                        }
                        var allowAmount = allow.split(splitBy);
                        $.each(allowAmount, function (ind, val) {
                            if (symbol == "$") {
                                if (val.indexOf('$') == -1) {
                                    val = "$" + val;
                                }
                            }
                            else if (symbol == "%") {
                                if (val.indexOf('%') == -1) {
                                    val = val + "%";
                                }
                            }
                            var optionobj = {
                            };
                            optionobj[columnName] = val;
                            itemsList.push(optionobj);
                        });
                    }
                    return ebsExtInstance.bindCustomDropDownItems(currentInstance, itemsList, columnName, rowData);
                }
            }
            else {
                elementDesign[0].Items = [];
                rowData[currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName] = "";
                rowData[currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName] = "";
                rowData[elementDesign[0].GeneratedName] = "";
                $.each(currentInstance.colModel, function (index, model) {
                    if (model.dataIndx == columnName) {
                        model.editor.options = "";
                        rowData[columnName] = "";
                    }
                });
                if (Valid.errorMsg != undefined && Valid.errorMsg != "") {
                    messageDialog.show(Valid.errorMsg);
                }
                return ebsExtInstance.bindCustomDropDownItems(currentInstance, elementDesign[0].Items, columnName, rowData);
            }
            break;
        case "Guardrail":
            if (elementDesign[0].MultiSelect == true) {
                $(":checkbox[value=0]").parent().parent().hide();
            }
            break;
    }
}

expressionBuilderExtension.prototype.generateOptionRange = function (ebsExtInstance, low, hight, increment, symbol, nonNumerical) {

    var ValueArray = new Array();
    var agGridOptions = new Array();
    var DesignArray = new Array();
    if (low <= hight) {
        const len = Math.floor((hight - low) / increment) + 1;
        Array(len).fill().map(
            (_, idx) => ebsExtInstance.createItemsObject(ValueArray, agGridOptions, DesignArray, low + (idx * increment), symbol));
    }
    var obj = {
        ValueArray: ValueArray, agGridOptions: agGridOptions, DesignArray: DesignArray
    }
    return obj;
}

expressionBuilderExtension.prototype.createItemsObject = function (ValueArray, agGridOptions, DesignArray, val, symbol) {
    ebsExtInstance = this;
    var result = val.toString();
    var copayFormatter = "${0}", coinsFormatter = "{0}%";
    if (result != undefined && result != null && result.indexOf(symbol) == -1) {
        if (symbol == "$") {
            result = parseFloat(result).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
            result = copayFormatter.replace('{0}', result);
        }
        else {
            result = coinsFormatter.replace('{0}', parseFloat(result).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,'));
        }
    }
    DesignArray.push({ Enabled: null, ItemID: 0, ItemText: result, ItemValue: result });
    ValueArray.push(result);
    var d = {};
    d[result] = result == null ? result : result;
    agGridOptions.push(d);
}

expressionBuilderExtension.prototype.executeGuardrailsTree = function (currentInstance, element, rowId, newValue, isEnterUniqueResponse, oldValue, event) {
    var hasSourceGuardrailChange = false;
    var ebInstance = this;
    var colName = "";
    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
    var selectedRow = gridOptions.api.getSelectedNodes();
    rowData = selectedRow[0].data;
    var col = "";
    var sourceGuardrailColList = ["Low", "High", "Increment", "Allowable", "Standard", "Guardrail"];
    if (currentInstance.fullName.toUpperCase().indexOf('GUARDRAILS') != -1) {
        for (var i = 0; i < sourceGuardrailColList.length; i++) {
            if (element.GeneratedName.indexOf(sourceGuardrailColList[i]) != -1) {
                col = sourceGuardrailColList[i];
                hasSourceGuardrailChange = true;
                break;
            }
        }
    }

    if (hasSourceGuardrailChange && col != undefined && col != "") {
        colName = element.GeneratedName.replace(col, "");
        rowData = ebInstance.guardrailsChange(currentInstance, ebInstance, colName, rowData, col, element);
        ebInstance.setUpdatedRowInCurrentInstance(currentInstance, ebInstance, rowData["RowIDProperty"], rowData);
        ebInstance.setAGGridColSelection(currentInstance, rowId, element.GeneratedName);
        var ExcluedList = [ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailsBenefitReviewGrid,
                          ebInstance.reapterFullName.COMMERCIALMEDICAL.GuardrailBenefitReviewSlidingCostShare];
        if (ExcluedList.indexOf(currentInstance.fullName) == -1) {
            ebInstance.updateTargetGuardrailsRow(currentInstance, ebInstance, colName, rowData, col, element);
        }
    }
}

expressionBuilderExtension.prototype.guardrailsChange = function (currentInstance, ebInstance, colName, rowData, col, element) {

    var allow = rowData[currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName];
    var standard = rowData[currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName];
    var isNonhasRangeGuardrails = ebInstance.hasRangeGuardrails(currentInstance, colName);
    var Valid = ebInstance.GuardrailsValidation(rowData, colName, element, isNonhasRangeGuardrails);
    var ValidValues = ebInstance.rangeGuardrailsValidValues(rowData, colName, element);
    var allowArr = new Array();
    var standardArr = new Array();
    var allowStr = "", standardStr = "";
    var guardrailType = ebInstance.getGuardrailType(currentInstance, rowData, true);
    if (guardrailType == "Standard") {
        allowArr = currentInstance.expressionBuilderRulesExt.getMultiSelectKeyArray(allow);
        if (standard != undefined) {
            standardArr = currentInstance.expressionBuilderRulesExt.getMultiSelectKeyArray(standard);
            $.each(standardArr, function (ind, val) {
                if (standard.indexOf(val) != -1 && allowArr.indexOf(val) != -1) {
                    if (standardStr != undefined && standardStr != "") {
                        standardStr += "," + val;
                    }
                    else {
                        standardStr = val;
                    }
                }
            });
        }
        if (allow == undefined || allow == "") {
            rowData[col] = "";
            rowData[currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName] = "";
        }
        else {
            rowData[currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName] = standardStr;
        }
    }
    else
        if (ValidValues.isValid && Valid.isValid) {
            if (colName.indexOf('Interval') != -1) {
                symbol = "";
            }
            else if (colName.indexOf('Coinsurance') != -1) {
                symbol = '%';
            }
            else {
                symbol = '$';
            }

            var result = ebInstance.generateOptionRange(ebInstance, ValidValues.low, ValidValues.hight, ValidValues.increment, symbol);
            if (result != undefined) {
                if (allow != undefined) {
                    allowArr = currentInstance.expressionBuilderRulesExt.getMultiSelectKeyArray(allow);
                    $.each(allowArr, function (ind, val) {
                        if (result["ValueArray"].indexOf(val) != -1) {
                            if (allowStr != undefined && allowStr != "") {
                                allowStr += "," + val;
                            }
                            else {
                                allowStr = val;
                            }
                        }
                    });
                }
                if (standard != undefined) {
                    standardArr = currentInstance.expressionBuilderRulesExt.getMultiSelectKeyArray(standard);
                    $.each(standardArr, function (ind, val) {
                        if (result["ValueArray"].indexOf(val) != -1 && allowArr.indexOf(val) != -1) {
                            if (standardStr != undefined && standardStr != "") {
                                standardStr += "," + val;
                            }
                            else {
                                standardStr = val;
                            }
                        }
                    });
                }
                rowData[currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName] = allowStr;
                rowData[currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName] = standardStr;
            }
        }
        else {
            if (Valid.errorMsg != undefined && Valid.errorMsg != "") {
                messageDialog.show(Valid.errorMsg);
                rowData[element.GeneratedName] = "";
                rowData[currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName] = "";
                rowData[currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName] = "";
            }
        }
    return rowData;
}

expressionBuilderExtension.prototype.updateTargetGuardrailsRow = function (currentInstance, ebInstance, colName, SourceRowData, col, element) {
    var keyColumns = currentInstance.colModel.filter(function (dt) {
        return dt.iskey == true;
    });

    var targetRepeaterPath = currentInstance.design.GeneratedName.replace('Guardrails', '');
    var targetRepeaterInstance = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (dt) {
        return dt.design.GeneratedName == targetRepeaterPath;
    });
    if (targetRepeaterInstance != undefined) {
        targetRepeaterInstance = targetRepeaterInstance[0];
        var targetGuardrailsRow = targetRepeaterInstance.data.filter(function (row) {
            var temp = row;
            $.each(keyColumns, function (ind, val) {
                if (temp[val.dataIndx] == SourceRowData[val.dataIndx]) {
                    temp = temp;
                }
                else {
                    temp = "";
                }
            });
            return temp == row;
        });

        if (targetGuardrailsRow != undefined && targetGuardrailsRow.length > 0) {
            targetGuardrailsRow = targetGuardrailsRow[0];
            var value = targetGuardrailsRow[colName];
            if (value != undefined && value != "Not Covered" && value != "Not Applicable") {
                var allow = SourceRowData['Allowable' + colName];
                var standard = SourceRowData['Standard' + colName];
                var validArray = new Array();
                var allowArr = new Array();
                var standardArr = new Array();
                if (allow != undefined && allow.length > 0) {
                    allowArr = currentInstance.expressionBuilderRulesExt.getMultiSelectKeyArray(allow);
                    if (allowArr != undefined && allowArr.length > 0) {
                        validArray = allowArr;
                    }
                }
                if (standard != undefined && standard.length > 0) {
                    standardArr = currentInstance.expressionBuilderRulesExt.getMultiSelectKeyArray(standard);
                    if (standardArr != undefined && standardArr.length > 0) {
                        validArray = validArray.concat(allowArr);
                    }

                }

                if (col == "Guardrail") {
                    if (element.MultiSelect == false) {
                        targetGuardrailsRow[colName] = SourceRowData[colName + col];
                    }
                    else {

                        if (SourceRowData[colName + col].indexOf(targetGuardrailsRow[colName]) == -1) {
                            targetGuardrailsRow[colName] = '';
                        }
                    }
                    ebInstance.setUpdatedRowInCurrentInstance(targetRepeaterInstance, ebInstance, targetGuardrailsRow["RowIDProperty"], targetGuardrailsRow);
                }
                else if (validArray.indexOf(value) == -1) {
                    targetGuardrailsRow[colName] = "";
                    ebInstance.setUpdatedRowInCurrentInstance(targetRepeaterInstance, ebInstance, targetGuardrailsRow["RowIDProperty"], targetGuardrailsRow);
                }
            }
            if (element.GeneratedName == ebInstance.repeaterElementName.RefreshCoinsuranceGuardrail && SourceRowData[colName + col] == "Yes") {

                var elementDesign = targetRepeaterInstance.design.Elements.filter(function (dt) {
                    if (dt.GeneratedName == ebInstance.repeaterElementName.RefreshCoinsurance) {
                        return dt;
                    }
                });

                ebInstance.getEBRowResult(ebInstance, targetRepeaterInstance, targetGuardrailsRow["RowIDProperty"], elementDesign[0], targetGuardrailsRow[colName]);
            }
        }
    }
}

expressionBuilderExtension.prototype.rangeGuardrailsValidValues = function (rowData, colName) {
    var obj = {
        isValid: false
    };
    if (rowData[colName + 'Low'] != undefined && rowData[colName + 'High'] != undefined && rowData[colName + 'Increment'] != undefined) {
        var low = parseFloat(rowData[colName + 'Low'].replace(/[^\d\.]*/g, ''));
        var hight = parseFloat(rowData[colName + 'High'].replace(/[^\d\.]*/g, ''));
        var increment = parseFloat(rowData[colName + 'Increment'].replace(/[^\d\.]*/g, ''));
        if (!isNaN(low) && !isNaN(hight) && !isNaN(increment)) {
            obj.isValid = increment > 0 ? true : false;
            obj.low = low;
            obj.hight = hight;
            obj.increment = increment;
        }
    }
    return obj;
}

expressionBuilderExtension.prototype.hasRangeGuardrails = function (currentInstance, colName) {
    lowAmountcolModel = currentInstance.colModel.filter(function (dt) {
        return dt.dataIndx == colName + "Low";
    });

    highColModel = currentInstance.colModel.filter(function (dt) {
        return dt.dataIndx == colName + "High";
    });

    incrementColModel = currentInstance.colModel.filter(function (dt) {
        return dt.dataIndx == colName + "Increment";
    });

    var nonNumerical = false;

    if (lowAmountcolModel.length == 0 && highColModel.length == 0 && incrementColModel.length == 0) {
        nonNumerical = true;
    }
    return nonNumerical;
}

expressionBuilderExtension.prototype.GuardrailsValidation = function (rowData, colName, element, isNonhasRangeGuardrails) {
    var obj = {
        isValid: false, errorMsg: ""
    };
    var errorMsg = "";
    var colDisplayName = element.Label.replace('High', '');
    colDisplayName = colDisplayName.replace('Low', '');
    colDisplayName = colDisplayName.replace('Standard', '');
    colDisplayName = colDisplayName.replace('Allowable', '');
    var low = rowData[colName + 'Low'];
    var hight = rowData[colName + 'High'];
    var increment = rowData[colName + 'Increment'];
    var numberWithDecimal = /^[0-9]*\.[0-9]{1,9}$|^[0-9]+$/;
    var numberOnly = /^[0-9]*$/;
    var numStr = element.Label.indexOf('Interval') != -1 ? numberOnly : numberWithDecimal;

    if (element.GeneratedName.indexOf('Low') != -1 || element.GeneratedName.indexOf('High') != -1 || element.GeneratedName.indexOf('Increment') != -1) {
        if (numStr.test(rowData[element.GeneratedName]) == false) {
            errorMsg = Guardrails.OnlyNumber.replace('{0}', element.Label);
        }
        else if (low != undefined && hight != undefined && low != "" && hight != "") {
            if (parseFloat(low) > parseFloat(hight) && (element.GeneratedName.indexOf('Low') != -1 || element.GeneratedName.indexOf('High') != -1)) {
                errorMsg = Guardrails.HighShouldBeGreaterThenLow.replace('{0}', colDisplayName + " High").replace('{1}', colDisplayName + " Low");
                obj.isValid = false;
            }
            else {
                obj.isValid = true;
            }
        }
        if (element.GeneratedName.indexOf('Increment') != -1 && parseFloat(increment) <= 0) {
            errorMsg = Guardrails.Increment.replace('{0}', colDisplayName + ' Increment');
            obj.isValid = false;
        }
    }
    else {
        if (isNonhasRangeGuardrails == false) {
            if (numStr.test(low) == false) {
                errorMsg = Guardrails.OnlyNumber.replace('{0}', colDisplayName + ' Low');
                obj.isValid = false;
            }
            else if (numStr.test(hight) == false) {
                errorMsg = Guardrails.OnlyNumber.replace('{0}', colDisplayName + ' High');
                obj.isValid = false;
            }
            else if (numStr.test(increment) == false) {
                errorMsg = Guardrails.OnlyNumber.replace('{0}', colDisplayName + ' Increment');
                obj.isValid = false;
            }
            else if (parseFloat(increment) <= 0) {
                errorMsg = Guardrails.Increment.replace('{0}', colDisplayName + ' Increment');
                obj.isValid = false;
            }
            else if (parseFloat(low) > parseFloat(hight)) {
                errorMsg = Guardrails.HighShouldBeGreaterThenLow.replace('{0}', colDisplayName + " High").replace('{1}', colDisplayName + " Low");
                obj.isValid = false;
            }
            else {
                obj.isValid = true;
            }
        }
    }
    obj.errorMsg = errorMsg;
    return obj;
}

expressionBuilderExtension.prototype.ExecuteSlidingCostShareRules = function (ebInstance, currentInstance, rowId, element, newValue) {
    var keyColumns = currentInstance.colModel.filter(function (dt) {
        return dt.iskey == true;
    });
    delete keyColumns[2];
    var rowNode = currentInstance.getRowNodeFromInstanceGrid(rowId);
    var rowData = rowNode.data;

    var rows = currentInstance.data.filter(function (row) {
        var temp = row;
        $.each(keyColumns, function (ind, val) {
            if (val != undefined) {
                if (temp[val.dataIndx] == rowData[val.dataIndx]) {
                    temp = temp;
                }
                else {
                    temp = "";
                }
            }
        });
        return temp == row;
    });

    var currentRowCostShareInterval = rowData[ebInstance.repeaterElementName.CostShareInterval];
    currentRowCostShareInterval = (currentRowCostShareInterval != undefined || currentRowCostShareInterval != "") ? currentRowCostShareInterval : 0;

    var rowsToProcess = currentInstance.data.filter(function (row) {
        return parseInt(row[ebInstance.repeaterElementName.CostShareInterval]) > parseInt(currentRowCostShareInterval)
    });

    var currentRowEndIntervalLow = rowData[ebInstance.repeaterElementName.EndIntervalLow];
    currentRowEndIntervalLow = (currentRowEndIntervalLow != undefined || "" != currentRowEndIntervalLow) ? currentRowEndIntervalLow : 0;
    currentRowEndIntervalLow = parseInt(currentRowEndIntervalLow);
    $.each(rowsToProcess, function (ind, row) {
        var nextcurrentRowCostShareInterval = parseInt(currentRowCostShareInterval) + 1;
        var rowToCompare = null;
        if (ind == 0) {
            rowToCompare = currentRowEndIntervalLow;
        }
        else {
            rowToCompare = rowsToProcess[ind - 1];
        }
        if (parseInt(row[ebInstance.repeaterElementName.StartIntervalLow]) <= parseInt(rowToCompare[ebInstance.repeaterElementName.StartIntervalLow])) {

        }
        if (parseInt(row[ebInstance.repeaterElementName.StartIntervaHigh]) <= parseInt(rowToCompare[ebInstance.repeaterElementName.StartIntervaHigh])) {

        }
        if (parseInt(row[ebInstance.repeaterElementName.EndIntervalLow]) <= parseInt(rowToCompare[ebInstance.repeaterElementName.EndIntervalLow])) {

        }
        if (parseInt(row[ebInstance.repeaterElementName.EndIntervalHigh]) <= parseInt(rowToCompare[ebInstance.repeaterElementName.EndIntervalHigh])) {

        }
    });
}

expressionBuilderExtension.prototype.getGuardrailType = function (currentInstance, rowData, isInSameRow) {
    var GuardrailType = "";
    if (isInSameRow) {
        rowData = rowData;
    }
    else {
        var guardrailsRepeaterInstance = null;
        var guardrailsRepeaterName = "Guardrails" + currentInstance.design.GeneratedName;
        guardrailsRepeaterInstance = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (dt) {
            return guardrailsRepeaterName == dt.design.GeneratedName;
        });

        if (guardrailsRepeaterInstance != null && guardrailsRepeaterInstance != undefined && guardrailsRepeaterInstance.length > 0) {
            TempguardrailsRepeaterInstance = guardrailsRepeaterInstance[0];
            currentInstance.guardrailsRepeaterInstance = {};
            currentInstance.guardrailsRepeaterInstance.guardrailsRepeaterFullName = TempguardrailsRepeaterInstance.fullName;
            currentInstance.guardrailsRepeaterInstance.guardrailsRepeaterData = TempguardrailsRepeaterInstance.data;
            currentInstance.isStandardRangeGuardrails = true;
            rowData = currentInstance.guardrailsRepeaterInstance.guardrailsRepeaterData[0];
        }
        else {
            if (currentInstance.fullName == "BenefitReview.BenefitReviewGrid" || currentInstance.fullName == "BenefitReview.BenefitReviewSlidingCostShare.BenefitReviewGridSlidingCostShare") {
                currentInstance.guardrailsRepeaterInstance = {};
                if (currentInstance.fullName == "BenefitReview.BenefitReviewGrid") {
                    currentInstance.guardrailsRepeaterInstance.guardrailsRepeaterFullName = "BenefitReviewGuardrail.GuardrailsBenefitReviewGrid";
                }
                else if (currentInstance.fullName == "BenefitReview.BenefitReviewSlidingCostShare.BenefitReviewGridSlidingCostShare") {
                    currentInstance.guardrailsRepeaterInstance.guardrailsRepeaterFullName = "BenefitReviewGuardrail.BenefitReviewSlidingCostShareGuardrail.GuardrailBenefitReviewSlidingCostShare";
                }
                if (currentInstance.formInstanceBuilder.formData != undefined) {
                    var Arr = currentInstance.guardrailsRepeaterInstance.guardrailsRepeaterFullName.split('.');
                    var guardrailsRepeaterData = null;
                    $.each(Arr, function (ind, val) {
                        if (guardrailsRepeaterData == null) {
                            guardrailsRepeaterData = currentInstance.formInstanceBuilder.formData[val];
                        }
                        else {
                            guardrailsRepeaterData = guardrailsRepeaterData[val];
                        }
                    });
                    currentInstance.guardrailsRepeaterInstance.guardrailsRepeaterData = guardrailsRepeaterData;
                    currentInstance.isStandardRangeGuardrails = true;
                    rowData = currentInstance.guardrailsRepeaterInstance.guardrailsRepeaterData[0];
                }
            }
        }
    }
    if (currentInstance.hasOwnProperty('guardrailsRepeaterInstance') == true) {
        if (rowData.hasOwnProperty(currentInstance.guardrailsRepeaterInstance.guardrailsLowColumnName) &&
                rowData.hasOwnProperty(currentInstance.guardrailsRepeaterInstance.guardrailsHighColumnName) &&
                rowData.hasOwnProperty(currentInstance.guardrailsRepeaterInstance.guardrailsIncrementColumnName)) {
            GuardrailType = "Range";
        }
        else if (rowData.hasOwnProperty(currentInstance.guardrailsRepeaterInstance.guardrailsAllowableColumnName) &&
            rowData.hasOwnProperty(currentInstance.guardrailsRepeaterInstance.guardrailsStandardColumnName)) {
            GuardrailType = "Standard";
        }
        else if (rowData.hasOwnProperty(currentInstance.guardrailsRepeaterInstance.guardrailsGuardrailColumnName)) {
            GuardrailType = "Guardrail";
        }
    }
    return GuardrailType;
}

expressionBuilderExtension.prototype.filterNetworkTier = function (currentInstance, event) {
    var ebInstance = this;
    var rowData = event.data;
    var elementDesign = currentInstance.design.Elements.filter(function (dt) {
        if (dt.GeneratedName == currentInstance.columnName) {
            return dt;
        }
    })

    if (elementDesign.length != 0) {

        var targetRepeaterInstance = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (dt) {
            return dt.fullName == "Network.NetworkTierList";
        });

        var ntArrr = new Array();
        if (targetRepeaterInstance != undefined && targetRepeaterInstance.length > 0) {
            $.each(targetRepeaterInstance[0].data, function (idx, row) {

                ntArrr.push(row["NetworkTier"]);

            });

            $.each(elementDesign[0].Items, function (idx, dt) {
                if (ntArrr.indexOf(dt.ItemText) == -1) {
                    $(currentInstance.gridElementIdJQ).find('.Dropdown').find('option[value=  "' + dt.ItemText + '"  ]').remove();
                }

            })
        }
    }
}

expressionBuilderExtension.prototype.DisableSubMarketExchangeStatusDropDownItems = function (currentInstance, ebInstance, sectionDropDownName) {
    var marketSeg = currentInstance.formData[ebInstance.sectionName.ProductRules][ebInstance.sectionName.PlanInformation][ebInstance.sectionElementName.MarketSegment];
    var EnabledOption = [];
    var elementId;

    currentInstance.designData.Sections.filter(function (ts) {
        ts.Elements.filter(function (dt) {
            if (dt.GeneratedName == ebInstance.sectionName.PlanInformation) {
                dt.Section.Elements.filter(function (dq) {
                    if (dq.GeneratedName == sectionDropDownName) {
                        elementId = dq.Name;
                        return dq.Name;
                    }
                });
            }
        });
    });

    if (sectionDropDownName == ebInstance.sectionElementName.SubMarket) {
        EnabledOption = ebInstance.sectionEleValues.subMarketOptions;

        if (marketSeg == ebInstance.sectionEleValues.MarketSegmentIndividualOptionSelected)
            EnabledOption = ebInstance.sectionEleValues.subMarketOptions.slice(0, 2);

        if (marketSeg == ebInstance.sectionEleValues.MarketSegmentSmallGroupOptionSelected)
            EnabledOption = ebInstance.sectionEleValues.subMarketOptions.slice(1, 5);

        if (marketSeg == ebInstance.sectionEleValues.MarketSegmentIndividualMedigapOptionSelected)
            EnabledOption = ebInstance.sectionEleValues.subMarketIndividualMedigapOptions;
    }

    if (sectionDropDownName == ebInstance.sectionElementName.ExchangeStatus) {
        EnabledOption = ebInstance.sectionEleValues.exchangeStatusOptions;

        if (marketSeg == ebInstance.sectionEleValues.MarketSegmentIndividualOptionSelected || marketSeg == ebInstance.sectionEleValues.MarketSegmentSmallGroupOptionSelected)
            EnabledOption = ebInstance.sectionEleValues.exchangeStatusOptions.slice(2, 4);

        if (marketSeg == ebInstance.sectionEleValues.MarketSegmentIndividualMedigapOptionSelected || marketSeg == ebInstance.sectionEleValues.MarketSegmentLargeGruopOptionSelected)
            EnabledOption = ebInstance.sectionEleValues.exchangeStatusLargeGruopOptionSelected;
    }

    $("#" + elementId + currentInstance.formInstanceId + " option").each(function (i) {
        var optionValue = $(this).val();
        if (optionValue != "[Select One]") {
            var valueIndex = EnabledOption.indexOf(optionValue);
            var dropdownOptionPath = "#" + elementId + currentInstance.formInstanceId + " option[value='" + optionValue + "']";
            if (valueIndex != -1)
                $(dropdownOptionPath).show();
            else
                $(dropdownOptionPath).hide();

        }
    });
}

expressionBuilderExtension.prototype.filterCombinedAccumDropdown = function (currentInstance, event) {
    var ebInstance = this;
    var rowData = event.data

    var elementDesign = currentInstance.design.Elements.filter(function (dt) {
        if (dt.GeneratedName == currentInstance.columnName)
            return dt;
    });

    $.each(elementDesign[0].Items, function (idx, dt) {
        if (dt.ItemValue == rowData.NetworkTier)
            $(":checkbox[value='" + dt.ItemValue + "']").parent().parent().hide();
    })
}

expressionBuilderExtension.prototype.alertLimitInformation = function (currentInstance, event) {
    var ebInstance = this;
    var rowData = event.data

    var elementDesign = currentInstance.formInstanceBuilder.formData.Limits.LimitsInformation.LimitList.filter(function (dt) {
        if (dt.LimitDescription == event.data.LimitDescription) {
            errorMsg = Guardrails.limitDescriptionAlert.replace('{0}', event.data.LimitDescription);
            messageDialog.show(errorMsg);
        }
    });
}

expressionBuilderExtension.prototype.ShowHideRepeaterColumn = function (CurrentInstance, ebInstance, repaterFullPath, ColumnDictionary) {
    var showHideTargetRepeaterInstance = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (dt) {
        return dt.design.GeneratedName == repaterFullPath;
    });

    var AllColumnArray = [];
    var HideColumnArray = [];

    for (var i = 0; i < showHideTargetRepeaterInstance[0].columnNames.length; i++) {
        AllColumnArray.push(showHideTargetRepeaterInstance[0].columnNames[i].dataIndx);
    }

    showHideTargetRepeaterInstance[0].gridOptions.columnApi.setColumnsVisible(AllColumnArray, true);

    for (var key in ebInstance.sectionElementName.GuradrailBRGColumnGroup) {
        if (CurrentInstance.formData[ebInstance.sectionElementName.BenefitReviewGuardrail].BenefitReviewGuardrail[key] == "False" || CurrentInstance.formData[ebInstance.sectionElementName.BenefitReviewGuardrail].BenefitReviewGuardrail[key] == false ||CurrentInstance.formData[ebInstance.sectionElementName.BenefitReviewGuardrail].BenefitReviewGuardrail[key] == "") {
            HideColumnArray.push.apply(HideColumnArray, filterItems(AllColumnArray, key));
            showHideTargetRepeaterInstance[0].gridOptions.columnApi.setColumnsVisible(HideColumnArray, false);
        }
    }

    showHideTargetRepeaterInstance[0].gridOptions.columnApi.autoSizeColumns(AllColumnArray);
}

function filterItems(arr, query) {
    return arr.filter(function (el) {
        return el.toLowerCase().indexOf(query.toLowerCase()) !== -1;
    })
}
