var expressionBuilderExtension = function () {
    this.FormDesignId = 0;
    this.isSectionLoad = false;
    this.URLs = {
        getANOCChartService: "/ExpressionBuilder/GetANOCChartServices",
        getAdditionalServicesData: "/ExpressionBuilder/GetAdditionalServicesData"
    }

    this.sectionName = {
        SECTIONASECTIONA1: "SECTION A SECTION A1",
        SECTIONB161718SUMMARYOFPACKAGES: "SECTION B: #16, #17, 18 SUMMARY OF PACKAGES",
        ProofingAttachments: "ProofingAttachments",
        ANOCChartSection: "ANOCChartPlanDetails",
        VBIDPreICL: "VBID: Pre-ICL",
        VBIDGap: "VBID: Gap"
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
        CostShareDetails: "CostShareDetails"
    }

    this.repeatersHavingDatasource = {

    }

    this.sectionElementName = {
    }

    this.repeaterElementName = {

        prevCopayAmount: "PreviousCopayAmount",
        prePreventiveCovered: "PreviousPreventiveServicesCovered",
        prevROBAmount: "PreviousReductionofBenefitsAmount",
        prevOOPMAmount: "PreviousOOPMAmount",
        prevDedAmount: "PreviousDeductibleAmount",
        prevCoinsAmount: "PreviousCoinsuranceAmount",
        Describethecomponentsofyournetwork: "Describethecomponentsofyournetwork",
        Indicatethetypeofcostsharingstructure: "Indicatethetypeofcostsharingstructure",
        SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier: "SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier",
        SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier: "SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier",
    }

    this.sectionElementFullName = {
        selectDentalPackage: "SECTIONBSUMMARYOFPACKAGES.DentalBenefits.SelectDentalPackage",
        selectVisionBenefitsPackage: "SECTIONBSUMMARYOFPACKAGES.VisionBenefits.SelectVisionBenefitsPackage",
        selectHearingBenefitsPackage: "SECTIONBSUMMARYOFPACKAGES.HearingBenefits.SelectHearingBenefitsPackage",
        contractNumber: "SECTIONASECTIONA1.ContractNumber",
        vBIDPreICL: "VBIDPreICL.VBIDTierCoveragePreICL",
        vBIDGap: "VBIDGap.VBIDTierCoverageGap",
        SelectthetiersthatincludereducedcostsharingselectallthatapplyPreICL: "VBIDPreICL.VBIDPackageTiersPreICL.Selectthetiersthatincludereducedcostsharingselectallthatapply",
	    SelectthetiersthatincludereducedcostsharingselectallthatapplyGap: "VBIDGap.VBIDPackageTiersGap.Selectthetiersthatincludereducedcostsharingselectallthatapply",
	    SelectthetiersthatincludereducedcostsharingselectallthatapplyOOP: "VBIDOOP.VBIDPackageTiersOOP.Selectthetiersthatincludereducedcostsharingselectallthatapply",
    }
    this.sectionEleValues = {
        selectDentalPackage: "Pending",
        selectVisionBenefitsPackage: "Pending",
        selectHearingBenefitsPackage: "Pending",
    }

    this.DDLEleValues = {

    }
    this.DDLEleIDs = {
    }

    this.reapterElementFullName = {
        Indicatethetypeofcostsharingstructure: "VBIDPreICL.VBIDTierCoveragePreICL.Indicatethetypeofcostsharingstructure",
        SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier: "VBIDPreICL.VBIDTierLocationsPreICL.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier",
        SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier: "VBIDPreICL.VBIDTierLocationsPreICL.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier",

        IndicatethetypeofcostsharingstructureGap: "VBIDGap.VBIDTierCoverageGap.Indicatethetypeofcostsharingstructure",
        SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTierGap: "VBIDGap.VBIDTierLocationsGap.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier",
        SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTierGap: "VBIDGap.VBIDTierLocationsGap.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier",

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
        CostShareCopay: "CostShare.Copay.CopayList",
        CostShareCoinsurance: "CostShare.Coinsurance.CoinsuranceList",
        CostShareDeductible: "CostShare.Deductible.DeductibleList",
        CostShareOOPM: "CostShare.OutofPocketMaximum.OutofPocketMaximumList",
        CostShareROB: "CostShare.ReductionofBenefits.WhatisthePlansReductionofBenefitAmount",
        CostSharePreventiveCovered: "CostShare.PreventCostShare.PreventServicescovered",
        LimitSummary: "Configuration.Limits.LimitSummary",
        VBIDTierCoveragePreICL: "VBIDPreICL.VBIDTierCoveragePreICL",
        VBIDTierLocationsPreICL: "VBIDPreICL.VBIDTierLocationsPreICL",
        VBIDTierCoverageGap: "VBIDGap.VBIDTierCoverageGap",
        VBIDTierLocationsGap: "VBIDGap.VBIDTierLocationsGap",
        VBIDDailyCopaymentsPreICL: "VBIDPreICL.VBIDDailyCopaymentsPreICL",
        VBIDDailyCopaymentsGap: "VBIDGap.VBIDDailyCopaymentsGap",
    }

    this.KeyName = {
    }

    this.elementIDs = {
        effectiveDate: "#WEL2359TextBox58405",
        termDate: "#WEL2359TextBox58406",
        selectDentalPackage: "#WEL2359DropDown64938",
        selectVisionBenefitsPackage: "#WEL2359DropDown64950",
        hearingBenefits: "#WEL2359DropDown64956",
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

        StandardRetailCopaymentPreICL: "#repeaterVBI2402Repeater79695",
        StandardPreferredRetailCopaymentPreICL: "#repeaterVBI2402Repeater79696",
        PreferredRetailCopaymentPreICL: "#repeaterVBI2402Repeater79697",
        StandardRetailCoinsurancePreICL: "#repeaterVBI2402Repeater79698",
        StandardPreferredRetailCoinsurancePreICL: "#repeaterVBI2402Repeater79699",
        PreferredRetailCoinsurancePreICL: "#repeaterVBI2402Repeater79700",
        StandardMailOrderCopaymentPreICL: "#repeaterVBI2402Repeater79701",
        StandardPreferredMailOrderCopaymentPreICL: "#repeaterVBI2402Repeater79702",
        PreferredMailOrderCopaymentPreICL: "#repeaterVBI2402Repeater79703",
        StandardMailOrderCoinsurancePreICL: "#repeaterVBI2402Repeater79704",
        StandardPreferredMailOrderCoinsurancePreICL: "#repeaterVBI2402Repeater79705",
        PreferredMailOrderCoinsurancePreICL: "#repeaterVBI2402Repeater79706",
        OutofNetwork1MPreICL: "#repeaterVBI2402Repeater79707",
        OutofNetworkOtherPreICL: "#repeaterVBI2402Repeater79832",
        LongtermcarePreICL: "#repeaterVBI2402Repeater79833",

        StandardRetailCopaymentGap: "#repeaterVBI2402Repeater79869",
        StandardPreferredRetailCopaymentGap: "#repeaterVBI2402Repeater79870",
        PreferredRetailCopaymentGap: "#repeaterVBI2402Repeater79871",
        StandardRetailCoinsuranceGap: "#repeaterVBI2402Repeater79872",
        StandardRetailPreferredCoinsuranceGap: "#repeaterVBI2402Repeater79873",
        PreferredRetailCoinsuranceGap: "#repeaterVBI2402Repeater79874",
        StandardMailOrderCopaymentGap: "#repeaterVBI2402Repeater79875",
        StandardPreferredMailOrderCopaymentGap: "#repeaterVBI2402Repeater79876",
        PreferredMailOrderCopaymentGap: "#repeaterVBI2402Repeater79877",
        StandardMailOrderCoinsuranceGap: "#repeaterVBI2402Repeater79878",
        StandardPreferredMailOrderCoinsuranceGap: "#repeaterVBI2402Repeater79879",
        PreferredMailOrderCoinsuranceGap: "#repeaterVBI2402Repeater79880",
        OutofNetwork1MGap: "#repeaterVBI2402Repeater79881",
        OutofNetworkOtherGap: "#repeaterVBI2402Repeater79882",
        LongtermcareGap: "#repeaterVBI2402Repeater79883",
        VBIDRxPreICLCalculateButton: "VBI2402TextBox112000",
        VBIDRxGapCalculateButton: "VBI2402TextBox112001",
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
    if (formDesignId == FormTypes.VBIDDESIGN || formDesignId == FormTypes.ANTHEMANCHORDESIGN || formDesignId == FormTypes.MEDICAREFORMDESIGNID || formDesignId == FormTypes.ANOCChartViewFormDesignID) {
        this.FormDesignId = formDesignId;
        result = true;
    }
    return result;
}

expressionBuilderExtension.prototype.hideAddButtonforRepeater = function (repeaterName) {
    var result = false;
    return result;
}


expressionBuilderExtension.prototype.hideMinusButtonforRepeater = function (repeaterName) {
    var result = false;
    return result;
}

expressionBuilderExtension.prototype.hideCopyButtonforRepeater = function (repeaterName) {
    var result = false;
    return result;
}

expressionBuilderExtension.prototype.sectionElementOnChange = function (currentInstance, elementPath, value) {
    var ebInstance = this;
    var currentInstance = currentInstance;
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
    // Register AnocChart button and event for ANOCChartView
    if (currentInstance.formDesignId == FormTypes.ANOCChartViewFormDesignID) {
        ebInstance.registerControlAndEventForANOCChart(currentInstance);
    }
    if ((currentInstance.formDesignId == FormTypes.VBIDDESIGN) && (currentInstance.selectedSectionName == this.sectionName.VBIDPreICL || currentInstance.selectedSectionName == this.sectionName.VBIDGap)) {
        var element = currentInstance.formData.VBIDPreICL.VBIDTierCoveragePreICL;
        var newValue = "";
        currentInstance.expressionBuilder.repeaterVisibleOnElementChange(currentInstance, element, newValue, true);
        ebInstance.registerControlAndEventForVBIDRxCalculator(currentInstance);
    }
}
expressionBuilderExtension.prototype.registerControlAndEventForVBIDRxCalculator = function (formInstancebuilder) {
    var ebInstance = this;
    if(formInstancebuilder.selectedSectionName == ebInstance.sectionName.VBIDPreICL)
    {
        var elementName = ebInstance.elementIDs.VBIDRxPreICLCalculateButton + formInstancebuilder.formInstanceId;
        $('#' + elementName).append('<button class="btn btn-sm but-align pull-right btnProcessVBIDPreICL" style="margin:10px;" id="' + elementName + '"> Calculate </button>');
        $('.btnProcessVBIDPreICL').off("click");
        $('.btnProcessVBIDPreICL').on("click", function () {
            ebInstance.processVBIDRxCalculator(formInstancebuilder, ebInstance.reapterFullName.VBIDDailyCopaymentsPreICL);
        });
    }
    if(formInstancebuilder.selectedSectionName == ebInstance.sectionName.VBIDGap)
    {
        var elementName = ebInstance.elementIDs.VBIDRxGapCalculateButton + formInstancebuilder.formInstanceId;
        $('#' + elementName).append('<button class="btn btn-sm but-align pull-right btnProcessVBIDGap" style="margin:10px;" id="' + elementName + '"> Calculate </button>');
        $('.btnProcessVBIDGap').off("click");
        $('.btnProcessVBIDGap').on("click", function () {
            ebInstance.processVBIDRxCalculator(formInstancebuilder, ebInstance.reapterFullName.VBIDDailyCopaymentsGap);
        });
    }
}

expressionBuilderExtension.prototype.processVBIDRxCalculator = function (formInstancebuilder, targetRepeater) {
    var ebInstance = this;
    switch(targetRepeater){
        case ebInstance.reapterFullName.VBIDDailyCopaymentsPreICL:
            var selectedTiersInPreICL = formInstancebuilder.formData.VBIDPreICL.VBIDPackageTiersPreICL.Selectthetiersthatincludereducedcostsharingselectallthatapply;
            if(selectedTiersInPreICL == null || selectedTiersInPreICL.length < 1){
                messageDialog.show("Please select the tier(s) in Package Tiers section and try!");
                return false;
            }
        break;
        case ebInstance.reapterFullName.VBIDDailyCopaymentsGap:
            var selectedTiersInGap = formInstancebuilder.formData.VBIDGap.VBIDPackageTiersGap.Selectthetiersthatincludereducedcostsharingselectallthatapply;
            if(selectedTiersInGap == null || selectedTiersInGap.length < 1){
                messageDialog.show("Please select the tier(s) in Package Tiers section and try!");
                return false;
            }
        break;
    }

    URLs = {
        ProcessRuleForSameSection: "/ExpressionBuilder/ProcessRuleForSameSectionVBIDRxCalculation"
    };

    var formInstanceData = formInstancebuilder.form.getFormInstanceData();
    formInstanceData.formInstanceData = formInstanceData.formInstanceData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');

    var rptData = JSON.stringify(formInstancebuilder.formData.VBIDPreICL.VBIDPackageTiersPreICL.Selectthetiersthatincludereducedcostsharingselectallthatapply);
    var rptFullPath = targetRepeater;
    var input = {
        formDesignVersionId: formInstancebuilder.designData.FormDesignVersionId,
        folderVersionId: formInstancebuilder.folderVersionId,
        formInstanceId: formInstancebuilder.formInstanceId,
        sourceElementPath: rptFullPath,
        ElementValue: rptData,
        isMultiselect: false,
        sectionData: formInstanceData.formInstanceData
    }
    var currentInstance = this;
    var promise = ajaxWrapper.postJSON(URLs.ProcessRuleForSameSection, input);
    promise.done(function (xhr) {
        if (xhr != null) {
            if (xhr != "[]") {
                var res = JSON.parse(xhr);
                $.each(res, function (idx, val) {
                    $.each(val.Data, function (idx, row) {
                        if (!Object.prototype.hasOwnProperty.call(row, "RowIDProperty")) {
                            row.RowIDProperty = idx;
                        }
                    });
                    var reptrInstance = null;
                    reptrInstance = formInstancebuilder.repeaterBuilders.filter(function (dt) {
                        return dt.fullName == val.TargetPath;
                    })
                    $.each(formInstancebuilder.repeaterBuilders, function (index, value) {
                        if (value.fullName == val.TargetPath) {
                            value.data = val.Data;
                        }
                    });
                    if (reptrInstance.length != 0) {
                        //refresh AG grid
                        var gridOptions = GridApi.getCurrentGridInstance(reptrInstance[0]).gridOptions;
                        gridOptions.api.setRowData(val.Data);
                        reptrInstance[0].executeAllRules(false);
                        messageDialog.show("Daily Copayments are updated!");
                    }
                });
            }
        }
    });
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

expressionBuilderExtension.prototype.repeaterElementOnChange = function (currentInstance, element, rowId, newValue, isEnterUniqueResponse, oldValue) {
    var ebInstance = this;
    var fullName = currentInstance.fullName;
    var formInstanceId = currentInstance.formInstanceId;
    var currentElementColumn = "";
    var previousElementColumn = "";
    var validColumns = ["CopayAmount", "CoinsuranceAmount", "DeductibleAmount", "OOPMAmount", "ReductionofBenefitsAmount", "PreventiveServicesCovered"];
    var validColumnIndex = -1;
    if (element != undefined) {
        validColumnIndex = validColumns.indexOf(element.GeneratedName);
    }
    //CostShare Values
    switch (ebInstance.FormDesignId) {
        case FormTypes.COMMERCIALMEDICAL:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.CostShareCopay:
                    previousElementColumn = ebInstance.repeaterElementName.prevCopayAmount
                    break;

                case ebInstance.reapterFullName.CostShareCoinsurance:
                    previousElementColumn = ebInstance.repeaterElementName.prevCoinsAmount
                    break;

                case ebInstance.reapterFullName.CostShareDeductible:
                    previousElementColumn = ebInstance.repeaterElementName.prevDedAmount
                    break;

                case ebInstance.reapterFullName.CostShareOOPM:
                    previousElementColumn = ebInstance.repeaterElementName.prevOOPMAmount
                    break;

                case ebInstance.reapterFullName.CostShareROB:
                    previousElementColumn = ebInstance.repeaterElementName.prevROBAmount
                    break;

                case ebInstance.reapterFullName.CostSharePreventiveCovered:
                    previousElementColumn = ebInstance.repeaterElementName.prePreventiveCovered
                    break;
            }
            if ((previousElementColumn != "") && (validColumnIndex != -1)) {
                ebInstance.setPreviousCostShareValue(currentInstance, rowId, element.GeneratedName, previousElementColumn, newValue, oldValue)
            }
            break;
    }
    //Visible rule Pre-ICL repeaters.
    if (currentInstance.fullName == ebInstance.reapterFullName.VBIDTierCoveragePreICL || currentInstance.fullName == ebInstance.reapterFullName.VBIDTierLocationsPreICL
        || currentInstance.fullName == ebInstance.reapterFullName.VBIDTierCoverageGap || currentInstance.fullName == ebInstance.reapterFullName.VBIDTierLocationsGap)
        ebInstance.repeaterVisibleOnElementChange(currentInstance, element, newValue, false);
}

expressionBuilderExtension.prototype.repeaterVisibleOnElementChange = function (currentInstance, element, newValue, isSectionLoad) {
    try {
        var ebInstance = this;
        var formInstanceId = currentInstance.formInstanceId;
        var selectedRepeater = "";
        if (isSectionLoad) {
            if (currentInstance.selectedSectionName == this.sectionName.VBIDPreICL)
                selectedRepeater = this.sectionElementFullName.vBIDPreICL;
            else
                selectedRepeater = this.sectionElementFullName.vBIDGap;
            ebInstance.isSectionLoad = true;
        }
        else {
            selectedRepeater = currentInstance.design.FullName;
            ebInstance.isSectionLoad = false;
        }
        if (selectedRepeater == this.reapterFullName.VBIDTierCoveragePreICL || selectedRepeater == this.reapterFullName.VBIDTierLocationsPreICL) {
            if (isSectionLoad) {
                ebInstance.repeaterPreICLShowHide(currentInstance, ebInstance.reapterElementFullName.Indicatethetypeofcostsharingstructure, "@~@");
                ebInstance.repeaterPreICLShowHide(currentInstance, ebInstance.reapterElementFullName.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier, "@~@");
                ebInstance.repeaterPreICLShowHide(currentInstance, ebInstance.reapterElementFullName.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier, "@~@");
            }
            else
                ebInstance.repeaterPreICLShowHide(currentInstance, element, newValue);
        }
        else if (selectedRepeater == this.reapterFullName.VBIDTierCoverageGap || selectedRepeater == this.reapterFullName.VBIDTierLocationsGap) {
            if (isSectionLoad) {
                ebInstance.repeaterGapShowHide(currentInstance, ebInstance.reapterElementFullName.IndicatethetypeofcostsharingstructureGap, "@~@");
                ebInstance.repeaterGapShowHide(currentInstance, ebInstance.reapterElementFullName.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTierGap, "@~@");
                ebInstance.repeaterGapShowHide(currentInstance, ebInstance.reapterElementFullName.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTierGap, "@~@");
            }
            else
                ebInstance.repeaterGapShowHide(currentInstance, element, newValue);
        }
    } catch (ex) {
        console.log(ex.message);
    }
}

expressionBuilderExtension.prototype.repeaterPreICLShowHide = function (currentInstance, element, newValue) {
    var ebInstance = this;
    var sourceField = "";
    if (newValue == "@~@") {
        sourceField = element;
        var describeCompOfNetwork = ebInstance.getRepeaterValue(currentInstance.formData.NineteenAReducedCostSharingforVBIDUFGroup1.VBIDUFGeneralInformation, ebInstance.repeaterElementName.Describethecomponentsofyournetwork);
        var indicateTypeOf = ebInstance.getRepeaterValue(currentInstance.formData.VBIDPreICL.VBIDTierCoveragePreICL, ebInstance.repeaterElementName.Indicatethetypeofcostsharingstructure);
        var selectAllOONPharmLocation = ebInstance.getRepeaterValue(currentInstance.formData.VBIDPreICL.VBIDTierLocationsPreICL, ebInstance.repeaterElementName.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier);
        var selectAllLTCPharmLocation = ebInstance.getRepeaterValue(currentInstance.formData.VBIDPreICL.VBIDTierLocationsPreICL, ebInstance.repeaterElementName.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier);
    }
    else {
        sourceField = element.FullName;
        var describeCompOfNetwork = ebInstance.getRepeaterValue(currentInstance.formInstanceBuilder.formData.NineteenAReducedCostSharingforVBIDUFGroup1.VBIDUFGeneralInformation, ebInstance.repeaterElementName.Describethecomponentsofyournetwork);
        var indicateTypeOf = ebInstance.getRepeaterValue(currentInstance.formInstanceBuilder.formData.VBIDPreICL.VBIDTierCoveragePreICL, ebInstance.repeaterElementName.Indicatethetypeofcostsharingstructure);
        var selectAllOONPharmLocation = ebInstance.getRepeaterValue(currentInstance.formInstanceBuilder.formData.VBIDPreICL.VBIDTierLocationsPreICL, ebInstance.repeaterElementName.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier);
        var selectAllLTCPharmLocation = ebInstance.getRepeaterValue(currentInstance.formInstanceBuilder.formData.VBIDPreICL.VBIDTierLocationsPreICL, ebInstance.repeaterElementName.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier);
    }
    switch (sourceField) {
        case ebInstance.reapterElementFullName.Indicatethetypeofcostsharingstructure:
            if ((indicateTypeOf.includes("1") && indicateTypeOf.includes("2")) || (indicateTypeOf.includes("3") || indicateTypeOf.includes("4")))
                newValue = "ShowAll";
            else if (indicateTypeOf.includes("1"))
                newValue = "Coinsurance";
            else if (indicateTypeOf.includes("2"))
                newValue = "Copayment";
            else
                newValue = "";
            switch (newValue) {
                case "Copayment":
                    if (describeCompOfNetwork.includes("010000")) {
                        $(ebInstance.elementIDs.StandardRetailCopaymentPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailCopaymentPreICL);
                    } else {
                        $(ebInstance.elementIDs.StandardRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("001000")) {
                        $(ebInstance.elementIDs.StandardPreferredRetailCopaymentPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredRetailCopaymentPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredRetailCopaymentPreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredRetailCopaymentPreICL);
                    } else {
                        $(ebInstance.elementIDs.StandardPreferredRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.StandardRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    if (describeCompOfNetwork.includes("000010")) {
                        $(ebInstance.elementIDs.StandardMailOrderCopaymentPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardMailOrderCopaymentPreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000001")) {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredMailOrderCopaymentPreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredMailOrderCopaymentPreICL);
                    } else {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.StandardMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    break;
                case "Coinsurance":
                    $(ebInstance.elementIDs.StandardRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                    if (describeCompOfNetwork.includes("010000")) {
                        $(ebInstance.elementIDs.StandardRetailCoinsurancePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailCoinsurancePreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("001000")) {
                        $(ebInstance.elementIDs.StandardPreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredRetailCoinsurancePreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredRetailCoinsurancePreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardPreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.StandardMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    if (describeCompOfNetwork.includes("000010")) {
                        $(ebInstance.elementIDs.StandardMailOrderCoinsurancePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardMailOrderCoinsurancePreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000001")) {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredMailOrderCoinsurancePreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredMailOrderCoinsurancePreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "ShowAll":
                    if (describeCompOfNetwork.includes("010000")) {
                        $(ebInstance.elementIDs.StandardRetailCopaymentPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.StandardRetailCoinsurancePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailCoinsurancePreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailCopaymentPreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.StandardRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("001000")) {
                        $(ebInstance.elementIDs.StandardPreferredRetailCopaymentPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredRetailCopaymentPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.StandardPreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredRetailCopaymentPreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredRetailCopaymentPreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredRetailCoinsurancePreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredRetailCoinsurancePreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardPreferredRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.StandardPreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000010")) {
                        $(ebInstance.elementIDs.StandardMailOrderCopaymentPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.StandardMailOrderCoinsurancePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardMailOrderCoinsurancePreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardMailOrderCopaymentPreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.StandardMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000001")) {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredMailOrderCopaymentPreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredMailOrderCopaymentPreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredMailOrderCoinsurancePreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredMailOrderCoinsurancePreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000100")) {
                        $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetwork1MPreICL);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetworkOtherPreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("100000")) {
                        $(ebInstance.elementIDs.LongtermcarePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.LongtermcarePreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.LongtermcarePreICL + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "":
                    $(ebInstance.elementIDs.StandardRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredRetailCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredRetailCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredMailOrderCopaymentPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredMailOrderCoinsurancePreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.LongtermcarePreICL + currentInstance.formInstanceId).hide();
                    break;
            }
            break;
        case ebInstance.reapterElementFullName.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier:
            if (selectAllOONPharmLocation.includes("01") && selectAllOONPharmLocation.includes("10"))
                newValue = "ShowAll";
            else if (selectAllOONPharmLocation.includes("01"))
                newValue = "01";
            else if (selectAllOONPharmLocation.includes("10"))
                newValue = "10";
            else
                newValue = "";
            switch (newValue) {
                case "01":
                    if (describeCompOfNetwork.includes("000100")) {
                        $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetwork1MPreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).hide();
                    break;
                case "10":
                    if (describeCompOfNetwork.includes("000100")) {
                        $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetworkOtherPreICL);
                    } else {
                        $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).hide();
                    break;
                case "ShowAll":
                    if (describeCompOfNetwork.includes("000100")) {
                        $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetwork1MPreICL);
                        $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetworkOtherPreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "":
                    $(ebInstance.elementIDs.OutofNetwork1MPreICL + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.OutofNetworkOtherPreICL + currentInstance.formInstanceId).hide();
                    break;
            }
            break;
        case ebInstance.reapterElementFullName.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier:
            if (selectAllLTCPharmLocation.includes("1"))
                newValue = "1";
            else
                newValue = "";
            switch (newValue) {
                case "1":
                    if (describeCompOfNetwork.includes("100000")) {
                        $(ebInstance.elementIDs.LongtermcarePreICL + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.LongtermcarePreICL);
                    }
                    else {
                        $(ebInstance.elementIDs.LongtermcarePreICL + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "":
                    $(ebInstance.elementIDs.LongtermcarePreICL + currentInstance.formInstanceId).hide();
                    break;
            }
            break;
    }
}

expressionBuilderExtension.prototype.repeaterGapShowHide = function (currentInstance, element, newValue) {
    var ebInstance = this;
    var sourceField = "";
    if (newValue == "@~@") {
        sourceField = element;
        var describeCompOfNetwork = ebInstance.getRepeaterValue(currentInstance.formData.NineteenAReducedCostSharingforVBIDUFGroup1.VBIDUFGeneralInformation, ebInstance.repeaterElementName.Describethecomponentsofyournetwork);
        var indicateTypeOf = ebInstance.getRepeaterValue(currentInstance.formData.VBIDGap.VBIDTierCoverageGap, ebInstance.repeaterElementName.Indicatethetypeofcostsharingstructure);
        var selectAllOONPharmLocation = ebInstance.getRepeaterValue(currentInstance.formData.VBIDGap.VBIDTierLocationsGap, ebInstance.repeaterElementName.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier);
        var selectAllLTCPharmLocation = ebInstance.getRepeaterValue(currentInstance.formData.VBIDGap.VBIDTierLocationsGap, ebInstance.repeaterElementName.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier);
    }
    else {
        sourceField = element.FullName;
        var describeCompOfNetwork = ebInstance.getRepeaterValue(currentInstance.formInstanceBuilder.formData.NineteenAReducedCostSharingforVBIDUFGroup1.VBIDUFGeneralInformation, ebInstance.repeaterElementName.Describethecomponentsofyournetwork);
        var indicateTypeOf = ebInstance.getRepeaterValue(currentInstance.formInstanceBuilder.formData.VBIDGap.VBIDTierCoverageGap, ebInstance.repeaterElementName.Indicatethetypeofcostsharingstructure);
        var selectAllOONPharmLocation = ebInstance.getRepeaterValue(currentInstance.formInstanceBuilder.formData.VBIDGap.VBIDTierLocationsGap, ebInstance.repeaterElementName.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTier);
        var selectAllLTCPharmLocation = ebInstance.getRepeaterValue(currentInstance.formInstanceBuilder.formData.VBIDGap.VBIDTierLocationsGap, ebInstance.repeaterElementName.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTier);
    }
    switch (sourceField) {
        case ebInstance.reapterElementFullName.IndicatethetypeofcostsharingstructureGap:
            if ((indicateTypeOf.includes("1") && indicateTypeOf.includes("2")) || (indicateTypeOf.includes("3") || indicateTypeOf.includes("4")))
                newValue = "ShowAll";
            else if (indicateTypeOf.includes("1"))
                newValue = "Coinsurance";
            else if (indicateTypeOf.includes("2"))
                newValue = "Copayment";
            else
                newValue = "";
            switch (newValue) {
                case "Copayment":
                    if (describeCompOfNetwork.includes("010000")) {
                        $(ebInstance.elementIDs.StandardRetailCopaymentGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailCopaymentGap);
                    } else {
                        $(ebInstance.elementIDs.StandardRetailCopaymentGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("001000")) {
                        $(ebInstance.elementIDs.StandardPreferredRetailCopaymentGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredRetailCopaymentGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredRetailCopaymentGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredRetailCopaymentGap);
                    } else {
                        $(ebInstance.elementIDs.StandardPreferredRetailCopaymentGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredRetailCopaymentGap + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.StandardRetailCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardRetailPreferredCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredRetailCoinsuranceGap + currentInstance.formInstanceId).hide();
                    if (describeCompOfNetwork.includes("000010")) {
                        $(ebInstance.elementIDs.StandardMailOrderCopaymentGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardMailOrderCopaymentGap);
                    } else {
                        $(ebInstance.elementIDs.StandardMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000001")) {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredMailOrderCopaymentGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredMailOrderCopaymentGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredMailOrderCopaymentGap);
                    } else {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    break;
                case "Coinsurance":
                    $(ebInstance.elementIDs.StandardRetailCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredRetailCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredRetailCopaymentGap + currentInstance.formInstanceId).hide();
                    if (describeCompOfNetwork.includes("010000")) {
                        $(ebInstance.elementIDs.StandardRetailCoinsuranceGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailCoinsuranceGap);
                    } else {
                        $(ebInstance.elementIDs.StandardRetailCoinsuranceGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("001000")) {
                        $(ebInstance.elementIDs.StandardRetailPreferredCoinsuranceGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredRetailCoinsuranceGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailPreferredCoinsuranceGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredRetailCoinsuranceGap);
                    } else {
                        $(ebInstance.elementIDs.StandardRetailPreferredCoinsuranceGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredRetailCoinsuranceGap + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.StandardMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                    if (describeCompOfNetwork.includes("000010")) {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredMailOrderCoinsuranceGap);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000001")) {
                        $(ebInstance.elementIDs.StandardMailOrderCoinsuranceGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardMailOrderCoinsuranceGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredMailOrderCoinsuranceGap);
                    } else {
                        $(ebInstance.elementIDs.StandardMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "ShowAll":
                    if (describeCompOfNetwork.includes("010000")) {
                        $(ebInstance.elementIDs.StandardRetailCopaymentGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.StandardRetailCoinsuranceGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailCoinsuranceGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailCopaymentGap);
                    } else {
                        $(ebInstance.elementIDs.StandardRetailCopaymentGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.StandardRetailCoinsuranceGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("001000")) {
                        $(ebInstance.elementIDs.StandardPreferredRetailCopaymentGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredRetailCopaymentGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.StandardRetailPreferredCoinsuranceGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredRetailCoinsuranceGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardRetailPreferredCoinsuranceGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredRetailCoinsuranceGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredRetailCopaymentGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredRetailCopaymentGap);
                    } else {
                        $(ebInstance.elementIDs.StandardPreferredRetailCopaymentGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredRetailCopaymentGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.StandardRetailPreferredCoinsuranceGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredRetailCoinsuranceGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000010")) {
                        $(ebInstance.elementIDs.StandardMailOrderCopaymentGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.StandardMailOrderCoinsuranceGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardMailOrderCopaymentGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardMailOrderCoinsuranceGap);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.StandardMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000001")) {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredMailOrderCopaymentGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.PreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredMailOrderCoinsuranceGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredMailOrderCoinsuranceGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.StandardPreferredMailOrderCopaymentGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.PreferredMailOrderCopaymentGap);
                    }
                    else {
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.PreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("000100")) {
                        $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).show();
                        $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).hide();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetwork1MGap);
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetworkOtherGap);
                    }
                    else {
                        $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).hide();
                    }
                    if (describeCompOfNetwork.includes("100000")) {
                        $(ebInstance.elementIDs.LongtermcareGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.LongtermcareGap);
                    }
                    else {
                        $(ebInstance.elementIDs.LongtermcareGap + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "":
                    $(ebInstance.elementIDs.StandardRetailCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredRetailCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredRetailCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardRetailCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardRetailPreferredCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredRetailCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardPreferredMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredMailOrderCopaymentGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.StandardMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.PreferredMailOrderCoinsuranceGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.LongtermcareGap + currentInstance.formInstanceId).hide();
                    break;
            }
            break;
        case ebInstance.reapterElementFullName.SelectallOutofNetworkPharmacyLocationsupplyamountsthatapplyforthisTierGap:
            if (selectAllOONPharmLocation.includes("01") && selectAllOONPharmLocation.includes("10"))
                newValue = "ShowAll";
            else if (selectAllOONPharmLocation.includes("01"))
                newValue = "01";
            else if (selectAllOONPharmLocation.includes("10"))
                newValue = "10";
            else
                newValue = "";
            switch (newValue) {
                case "01":
                    if (describeCompOfNetwork.includes("000100")) {
                        $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetwork1MGap);
                    } else {
                        $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).hide();
                    }
                    $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).hide();
                    break;
                case "10":
                    $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).hide();
                    if (describeCompOfNetwork.includes("000100")) {
                        $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetworkOtherGap);
                    } else {
                        $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "ShowAll":
                    if (describeCompOfNetwork.includes("000100")) {
                        $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetwork1MGap);
                        $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.OutofNetworkOtherGap);
                    } else {
                        $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).hide();
                        $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "":
                    $(ebInstance.elementIDs.OutofNetwork1MGap + currentInstance.formInstanceId).hide();
                    $(ebInstance.elementIDs.OutofNetworkOtherGap + currentInstance.formInstanceId).hide();
                    break;
            }
            break;
        case ebInstance.reapterElementFullName.SelectallLongTermCarePharmacyLocationsupplyamountsthatapplyforthisTierGap:
            if (selectAllLTCPharmLocation.includes("1"))
                newValue = "1";
            else
                newValue = "";
            switch (newValue) {
                case "1":
                    if (describeCompOfNetwork.includes("100000")) {
                        $(ebInstance.elementIDs.LongtermcareGap + currentInstance.formInstanceId).show();
                        ebInstance.repeaterColumnResize(currentInstance, ebInstance.elementIDs.LongtermcareGap);
                    }
                    else {
                        $(ebInstance.elementIDs.LongtermcareGap + currentInstance.formInstanceId).hide();
                    }
                    break;
                case "":
                    $(ebInstance.elementIDs.LongtermcareGap + currentInstance.formInstanceId).hide();
                    break;
            }
            break;
    }
}

expressionBuilderExtension.prototype.getRepeaterValue = function (repeaterData, fieldName) {
    var ebInstance = this;
    var arr = [];
    var arrOption = [];
    if (repeaterData.Describethecomponentsofyournetwork != null && fieldName == ebInstance.repeaterElementName.Describethecomponentsofyournetwork) {
        for (var idx = 0; idx < repeaterData.Describethecomponentsofyournetwork.length; idx++) {
            arr.push(repeaterData.Describethecomponentsofyournetwork[idx]);
        }
    }
    else if (repeaterData != null && repeaterData.length > 0) {
        for (var idx = 0; idx < repeaterData.length; idx++) {
            arrOption = repeaterData[idx][fieldName].split(',');
            if (arrOption.length > 1) {
                for (var idxOpt = 0; idxOpt < arrOption.length; idxOpt++) {
                    arr.push(arrOption[idxOpt]);
                }
            }
            else
                arr.push(repeaterData[idx][fieldName]);
        }
    }
    return arr;
}
expressionBuilderExtension.prototype.repeaterColumnResize = function (currentInstance, repeaterName) {
    var ebInstance = this;
    var repToResizeColumn = "";
    if (ebInstance.isSectionLoad) {
        repToResizeColumn = currentInstance.repeaterBuilders.filter(function (elemName) {
            return "#repeater" + elemName.design.Name == repeaterName;
        });
    }
    else {
        repToResizeColumn = currentInstance.formInstanceBuilder.repeaterBuilders.filter(function (elemName) {
            return "#repeater" + elemName.design.Name == repeaterName;
        });
    }
    var gridOptions = GridApi.getCurrentGridInstance(repToResizeColumn[0]).gridOptions;
    var allColumnIds = [];
    gridOptions.columnApi.getAllColumns().forEach(function (column) {
        allColumnIds.push(column.colId);
    });
    gridOptions.columnApi.autoSizeColumns(allColumnIds);
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

expressionBuilderExtension.prototype.manualdataSourceSaveButtonClick = function (currentInstance, data, oldData, sourceDataList) {
    var ebInstance = this;
    switch (ebInstance.FormDesignId) {
        case FormTypes.ANOCChartViewFormDesignID:
            switch (currentInstance.repeaterBuilder.fullName) {
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
        if (currentInstance.IsFormInstanceEditable.toString().toLowerCase() == "true") {
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
        ElementChangeRuleUrl: "/ExpressionBuilder/ProcessRuleForSameSection",
        ElementChangeRulePackageUrl: "/ExpressionBuilder/ProcessRuleForElement"
    }
    this.expressionBuilderConstant = {
        IsSameSectionRuleSource: "IsSameSectionRuleSource"
    }
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
    if ($('#' + elementName).val() == "") {
        $('#'+ elementName).text("Select Source Document");
    }

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
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PlanInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PlanInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanInformation]);
                            gridOptions.api.refreshCells();

                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartPlanPremiumInformation:
                        try {
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PlanPremiumInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PlanPremiumInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanPremiumInformation]);
                            gridOptions.api.refreshCells();
                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartOutofPocketInformation:
                        try {
                            //var processedData = [];
                            //processedData = ebInstance.prepareGridData(JSON.parse(newData[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]), reptrInstance[0].columnModels);
                            //var dataModelData = ebInstance.prepareGridData(JSON.parse(newData[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]), reptrInstance[0].columnModels);
                            //reptrInstance[0].data = processedData;
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.OutofPocketInformation] = processedData;
                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = dataModelData;
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.OutofPocketInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartOutofPocketInformation]);
                            gridOptions.api.refreshCells();
                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare:
                        try {
                            //var processedData = [];
                            //processedData = ebInstance.prepareGridData(JSON.parse(newData[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]), reptrInstance[0].columnModels);
                            //var dataModelData = ebInstance.prepareGridData(JSON.parse(newData[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]), reptrInstance[0].columnModels);

                            //reptrInstance[0].data = processedData;
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCBenefitsCompare] = processedData;

                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = dataModelData;
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCBenefitsCompare] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare]);
                            gridOptions.api.refreshCells();

                        } catch (e) {
                            console.log(e.message);
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartPrescriptionInformation:
                        try {
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PrescriptionInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PrescriptionInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPrescriptionInformation]);
                            gridOptions.api.refreshCells();
                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation:
                        try {
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.InitialCoverageLimitInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);

                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.InitialCoverageLimitInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartInitialCoverageLimitInformation]);
                            gridOptions.api.refreshCells();
                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartGapCoverageInformation:
                        try {
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartGapCoverageInformation]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCCPDoctorOfficeVisit] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInformation]);
                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInformation]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            // Uncomment below code for Ag grid
                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartGapCoverageInformation]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartGapCoverageInformation]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.GapCoverageInformation] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartGapCoverageInformation]);
                            gridOptions.api.refreshCells();
                        } catch (e) {
                        }
                        break;
                        //Doctor Office Visits
                    case ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo:
                        try {
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCOfficeVisitInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCOfficeVisitInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartDoctorOfficeVisitInfo]);
                            gridOptions.api.refreshCells();

                        } catch (e) {
                        }
                        break;
                        //Inpatient Health Service
                    case ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo:
                        try {
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCIHSInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCIHSInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartHospitalStayServiceInfo]);
                            gridOptions.api.refreshCells();

                        } catch (e) {
                        }
                        break;

                    case ebInstance.reapterFullName.ANOCChartPreferredRetailInfo:
                        try {
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PreferredRetailInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);

                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.PreferredRetailInfo] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPreferredRetailInfo]);
                            gridOptions.api.refreshCells();

                        } catch (e) {
                        }
                        break;
                    case ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails:
                        try {
                            //reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            //currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.CostShareDetails] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            //var dataModel = $(reptrInstance[0].gridElementIdJQ).pqGrid("option", "dataModel");
                            //dataModel.data = JSON.parse(newData[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            //$(reptrInstance[0].gridElementIdJQ).pqGrid("refreshDataAndView");

                            var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            gridOptions.rowData = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            reptrInstance[0].data = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            gridOptions.api.setRowData(gridOptions.rowData);
                            currentInstance.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.CostShareDetails] = JSON.parse(chartServiceArray[ebInstance.reapterFullName.ANOCChartPlanDetailsCostShareDetails]);
                            gridOptions.api.refreshCells();


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
        tenantId: currentInstance.repeaterBuilder.formInstanceBuilder.tenantId,
        nextYearFormInstanceId: currentInstance.repeaterBuilder.formInstanceBuilder.anchorFormInstanceID,
        previousYearFormInstanceId: currentInstance.repeaterBuilder.formInstanceBuilder.formData.ANOCChartPlanDetails.SourceDocument.SourceDocumentID,
		repeaterData: JSON.stringify(oldData),
        repeaterSelectedData: JSON.stringify(data),
        effectiveDate: currentInstance.repeaterBuilder.formInstanceBuilder.folderData.effectiveDate,
        anocViewFormInstanceId: currentInstance.repeaterBuilder.formInstanceBuilder.formInstanceId
    }
    var promise = ajaxWrapper.postJSON(ebInstance.URLs.getAdditionalServicesData, postData);
    promise.done(function (result) {
        var chartServiceArray = [];
        chartServiceArray = JSON.parse(result);
        var newData = JSON.parse(result);
        if (chartServiceArray != undefined && chartServiceArray != null && chartServiceArray.length > 0) {
            try {
                var processedData = [];
                var targetReptrInstance = currentInstance.repeaterBuilder.formInstanceBuilder.repeaterBuilders.filter(function (dt) {
                    return dt.fullName == ebInstance.reapterFullName.ANOCChartANOCBenefitsCompare;
                })
                processedData = ebInstance.prepareGridData(newData, targetReptrInstance[0].columnModels);
                var gridOptions = $(targetReptrInstance[0].gridElementIdJQ)[0].gridOptions;
                gridOptions.rowData = processedData;
                targetReptrInstance[0].data = processedData;
                gridOptions.api.setRowData(gridOptions.rowData);
                currentInstance.repeaterBuilder.formInstanceBuilder.formData[ebInstance.sectionName.ANOCChartSection][ebInstance.repeaterName.ANOCBenefitsCompare] = processedData;
                gridOptions.api.refreshCells();
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
    /*PQ Grid*/

    //var modelData = $(currentInstance.gridElementIdJQ).pqGrid("option", "dataModel.data");
    //var modeldataRow = modelData.filter(function (dt) {
    //    return dt.RowIDProperty == rowId;
    //});
    //modeldataRow[0][previousElementPath] = modeldataRow[0][currentElementPath];
    //$(currentInstance.gridElementIdJQ).pqGrid("refreshCell", { rowIndx: rowId, dataIndx: previousElementPath });
    //$(currentInstance.gridElementIdJQ).pqGrid("refreshView");

    /*AG Grid*/

    try {
        var originalRow = currentInstance.orignalData.filter(function (ct) {
            if (ct.RowIDProperty == rowId) {
                return ct;
            }
        });


        if (originalRow.length > 0) {
            var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
            var rowData = currentInstance.getRowDataFromInstanceGrid(rowId);
            rowData[0][previousElementPath] = originalRow[0][currentElementPath];;
            rowData[0][currentElementPath] = newValue;
            gridOptions.api.updateRowData({ update: rowData });

        }
    }
    catch (e) {
        console.log(e.message);

    }

    //updateRowData({update: itemsToUpdate});
    //gridOptions.api.refreshCells();
    //gridOptions.api.refreshView();

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

expressionBuilderRulesExt.prototype.hasRule = function (elementData, val) {

    if (elementData.hasOwnProperty("design")) {
        elementData = elementData.design;
    }

                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    var elems = ["INSelectDentalPackage", "OONSelectDentalPackage", "INTier1SelectDentalPackage", "INTier2SelectDentalPackage", "INTier3SelectDentalPackage", "INSelectHearingBenefitsPackage", "OONSelectHearingBenefitsPackage", "INTier1SelectHearingBenefitsPackage", "INTier2SelectHearingBenefitsPackage", "INTier3SelectHearingBenefitsPackage", "INSelectVisionBenefitsPackage", "OONSelectVisionBenefitsPackage", "INTier1SelectVisionBenefitsPackage", "INTier2SelectVisionBenefitsPackage", "INTier3SelectVisionBenefitsPackage", "AcupuncturePackage", "ChiropracticPackage", "MealsBenefitPackage", "DSNPHighlyIntegratedServicesPackage", "SelectEnhancedEligibleBenefits", "RoutineFootCare", "TransportationPackage", "HealthEducationPackage", "InHomeSafetyAssessmentPackage", "CounselingServicePackage", "TelemonitoringServicesPackage", "AdditionalSessionsofSmokingandTobaccoCessationCounselingPackage", "NutritionDietaryBenefitPackage", "Other1Package", "Other2Package", "Other3Package","EnhancedDiseaseManagementPackage","AlternativeTherapiesPackage","NursingHotlinePackage","BathroomSafetyDevicesPackage","TherapeuticMassagePackage"];
    var idx = elems.indexOf(elementData.GeneratedName);
    if (idx > -1)
        return true;


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
    var objelementData;
    if (elementData.hasOwnProperty("design")) {
        objelementData = elementData.design.FullName;
    }
    else{
        objelementData = elementData.FullName
    }
    if (objelementData.indexOf("SECTIONBSUMMARYOFPACKAGES") > -1 || objelementData.indexOf("SECTIONBAcupunctureOTCPostAcuteMeals") > -1 || objelementData.indexOf("SECTIONB7ProfessionalServices") > -1 || objelementData.indexOf("SECTIONBandSECTIONBMedicareCoveredPreventiveFitnessPartBRx") > -1 || 
        objelementData.indexOf("SECTIONB10AmbulanceTransportationServices") > -1)
    {
        var input = {
            FormDesignVersionID: formDesignVersionId,
            FolderVersionID: folderVersionId,
            FormInstanceID: formInstanceId,
            ElementJSONPath: objelementData,
            ElementValue: val.value
        }

        var currentInstance = this;
        var promise = ajaxWrapper.postJSON(this.URLs.ElementChangeRulePackageUrl, input);
        promise.done(function (xhr) {
            
            var res = JSON.parse(xhr);
            if (res.JSONData != null && res.JSONData != undefined) {
                var sectionData = JSON.parse(res.JSONData);
                var fieldArray = res.TargetFieldPaths.split(",");
                $.each(fieldArray, function (idx, val) {
                    currentInstance.bindFieldValue(val, formData, sectionData);
                });
            }
        });
    }
    else {
        var multiSelectFlag = elementData.MultiSelect == undefined ? false : elementData.MultiSelect;
        var input = {
            formDesignVersionId: formDesignVersionId,
            folderVersionId: folderVersionId,
            formInstanceId: formInstanceId,
            sourceElementPath: "",
            ElementValue: "",
            ElementJSONPath: "",
            isMultiselect: multiSelectFlag
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
            if (xhr != null) {
                if (xhr != "[]") {
                    var res = JSON.parse(xhr);
                    $.each(res, function (idx, val) {
                        $.each(val.Data, function (idx, row) {
                            if (!Object.prototype.hasOwnProperty.call(row, "RowIDProperty")) {
                                row.RowIDProperty = idx;
                            }
                        });
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
                                    value.data = val.Data;
                                    //value.lastSelectedRow = val.Data.length;
                                }
                            });
                        }
                        else {

                            reptrInstance = currentInstanceObj.repeaterBuilders.filter(function (dt) {
                                return dt.fullName == val.TargetPath;
                            })
                            $.each(currentInstanceObj.repeaterBuilders, function (index, value) {
                                if (value.fullName == val.TargetPath) {
                                    value.data = val.Data;
                                    //value.lastSelectedRow = val.Data.length;
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
                            //var gridOptions = $(reptrInstance[0].gridElementIdJQ)[0].gridOptions;
                            //gridOptions.rowData = val.Data;
                            //gridOptions.api.setRowData(gridOptions.rowData);
                            //gridOptions.api.sizeColumnsToFit();
                            //currentInstanceObj.formInstanceBuilder.formData[val.TargetPath] =val.Data;

                            if (GridDisplayMode.AG == CurrentGridDisplayMode) {
                                var gridOptions = GridApi.getCurrentGridInstance(reptrInstance[0]).gridOptions;
                                gridOptions.api.setRowData(val.Data);

                                var objElement;
                                if (elementData.hasOwnProperty("design")) 
                                    objElement = elementData.design.FullName;
                                else
                                    objElement = elementData.FullName
                                
                                if (objElement == "VBIDPreICL.VBIDPackageTiersPreICL.Selectthetiersthatincludereducedcostsharingselectallthatapply"
                                    || objElement == "VBIDGap.VBIDPackageTiersGap.Selectthetiersthatincludereducedcostsharingselectallthatapply"
                                    || objElement == "VBIDOOP.VBIDPackageTiersOOP.Selectthetiersthatincludereducedcostsharingselectallthatapply") {
                                        reptrInstance[0].executeAllRules(true);
                                    }
                                else{
                                        reptrInstance[0].executeAllRules(false);
                                    }
                            }
                        }
                    });
                }
            }
        });
    }
}

expressionBuilderExtension.prototype.registerEventForButtonInRepeater = function (currentInstance) {
    var ebInstance = this;

    switch (ebInstance.FormDesignId) {

        case FormTypes.COMMERCIALMEDICAL:
            switch (currentInstance.fullName) {
                case ebInstance.reapterFullName.COMMERCIALMEDICAL.BenefitReview:
                    $(".viewCOMMERCIALBRGLimits").off().on('click', function () {
                        // currentInstance.saveRow();
                        var element = $(this).attr("Id");
                        var griddata = currentInstance.data.filter(function (dt) {
                            return dt[ebInstance.KeyName.RowIDProperty] == element;
                        })

                        ebInstance.showLimitPouUp(element, currentInstance.data, currentInstance.tenantId, currentInstance.formInstanceId, currentInstance.formInstanceBuilder.formDesignVersionId, currentInstance.formInstanceBuilder.folderVersionId);
                    });
                    break;
            }
            break;

    }
}



