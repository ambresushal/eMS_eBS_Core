function formUtilities(currentFormInstanceId) {
    this.currentFormInstanceId = currentFormInstanceId;
    this.sectionManipulation = this.formDesignContainerManipulationMethods();
}

formUtilities.prototype.formDesignContainerManipulationMethods = function () {
    var currentInstance = this;

    var fullName = {
        brgShowCopay: "BenefitReview.Search.ShowCopayinGrid",
        brgShowCoinsurance: "BenefitReview.Search.ShowCoinsuranceinGrid",
        brgShowAllowedAmt: "BenefitReview.Search.ShowAllowedAmtinGrid",
        brgShowAllowedCtr: "BenefitReview.Search.ShowAllowedCtrinGrid",
        brgShowDeductibles: "BenefitReview.Search.ShowDeductiblesinGrid",
        brgShowLimits: "BenefitReview.Search.ShowLimitsinGrid",
        brgShowMessages: "BenefitReview.Search.ShowMessagesinGrid",
        brgShowOptions: "BenefitReview.Search.ShowOptionsinGrid",
        brgShowPenalty: "BenefitReview.Search.ShowPenaltyinGrid",
    }

    function DisableRepeater(repeaterID) {
        var pqRepeaterID = repeaterID.replace('#repeater', '#');
      //nj  $(pqRepeaterID).pqGrid("option", "editable", false);

        elem = $(repeaterID);

        // disabled navigation buttons of disabled PQ grid repeater.
        //elem.find('.customPager').find(':not([title="View"])').addClass('ui-state-disabled-pq');
        elem.find('.customPager').find('.ui-icon-plus, .ui-icon-minus, .ui-icon-copy, [title="Export To Excel"], [title="Import Excel"], [title="View Reference DropDown"], [title="Add/Remove Services"], [title="View Reference DropDown"]').addClass('ui-state-disabled-pq');
        elem.find('.bucontrol').addClass('ui-state-disabled-pq');
        // disabled navigation buttons of disabled JQ grid repeater.

        elem.find('#btnManualDataPopup').addClass('ui-state-disabled');
        elem.find('#btnRepeaterBuilderAdd').addClass('ui-state-disabled');
        elem.find('#btnRepeaterBuilderCopy').addClass('ui-state-disabled');
        elem.find('#btnRepeaterBuilderDelete').addClass('ui-state-disabled');
        elem.find("table.ui-jqgrid-btable").find(".jqgrow").prop('disabled', true);
        elem.find("table.ui-jqgrid-btable").find(".jqgrow").addClass("not-editable-row");
        elem.addClass('disabled');
        elem.find("table.ui-jqgrid-btable").find(".jqgrow").attr("editable", false);
        elem.find('#btnAddOrRemoveTierData').addClass('ui-state-disabled');
        elem.find('#btnAddOrRemoveAltTierData').addClass('ui-state-disabled');
        elem.find('#btnRepeaterBuilderRuleAssignment').addClass('ui-state-disabled');
        elem.find('#btnRepeaterBuilderALtRuleAssignment').addClass('ui-state-disabled');
        if (repeaterID.substr(0, 9) == "#repeater")
            repeaterRefreshId = repeaterID.replace('#repeater', '#refresh_');
        else
            repeaterRefreshId = repeaterID.replace('#', '#refresh_');
        elem.find('#btnRepeaterBuilderView,' + repeaterRefreshId).click(function () {
            elem.find("table.ui-jqgrid-btable").find('input, select, img.ui-datepicker-trigger, table.ui-jqgrid-btable, textarea').prop("disabled", true);
            elem.find("table.ui-jqgrid-btable").find(".jqgrow").prop("disabled", true);
            elem.find("table.ui-jqgrid-btable").find(".jqgrow").addClass("not-editable-row");
            elem.find("table.ui-jqgrid-btable").find(".jqgrow").attr("editable", false);
            elem.find("table.ui-jqgrid-btable").find("img.ui-datepicker-trigger").prop('disabled', true);
        });
        elem.find(".jqgrow, .sgcollapsed").click(function () {
            $(this).closest("table.ui-jqgrid-btable").closest(".jqgrow").find('input, select, img.ui-datepicker-trigger, textarea').prop("disabled", true);
            $(this).prop("disabled", true);
            $(this).attr("editable", false);
            $(this).closest("table.ui-jqgrid-btable").find(".jqgrow").addClass("not-editable-row");
            $(this).closest("table.ui-jqgrid-btable").find("img.ui-datepicker-trigger").prop("disabled", true);
            $(this).closest("table.ui-jqgrid-btable").find(".jqgrow").prop("disabled", true);
            $(this).closest("table.ui-jqgrid-btable").find(".ui-search-input input").prop("disabled", false).first().focus();
        });
        elem.click(function () {
            elem.find("table.ui-jqgrid-btable").find('input, select, img.ui-datepicker-trigger, textarea').prop("disabled", true);
            elem.find("table.ui-jqgrid-btable").find(".jqgrow").prop("disabled", true);
            elem.find("table.ui-jqgrid-btable").find(".jqgrow").attr("editable", false);
            elem.find("table.ui-jqgrid-btable").find(".jqgrow").addClass("not-editable-row");
            elem.find("table.ui-jqgrid-btable").find("img.ui-datepicker-trigger").prop("disabled", true);
        });
    }

    function DisableAllChildElements(elements) {
        $.each(elements, function (index, element) {
            var elem;
            if (element.Type == "repeater") {
                elemId = '#repeater' + element.Name + currentInstance.currentFormInstanceId;
                DisableRepeater(elemId);
            }
            else if (element.Type == "section") {
                elem = $('#section' + element.Name + currentInstance.currentFormInstanceId);
                var section = element.Section;
                if (section.Elements != null) {
                    DisableAllChildElements(section.Elements);
                }
                elem.addClass('disabled');
            }
            else {
                //elem.parent('div').children().prop('disabled', 'disabled'); 
                if (element.Type != "radio") {
                    elem = $('#' + element.Name + currentInstance.currentFormInstanceId);
                }
                else {
                    elem = $('input[name=' + element.Name + currentInstance.currentFormInstanceId + ']');
                }
                skipTheseElements(element.FullName, elem);
            }
        });
    }

    function HideAllChildElements(elements) {
        $.each(elements, function (index, element) {
            var elem;
            if (element.Type == "repeater") {
                elem = $('#repeater' + element.Name + currentInstance.currentFormInstanceId);
                elem.css('display', 'none');
            }
            else if (element.Type == "section") {
                elem = $('#section' + element.Name + currentInstance.currentFormInstanceId);
                var section = element.Section;
                HideAllChildElements(section.Elements);
                elem.css('display', 'none');
            }
            else {
                elem = $('#' + element.Name + currentInstance.currentFormInstanceId);
                elem.parent('div').children().css('display', 'none');
            }
        });
    }
    function EnableRepeater(elemId) {
        // enable bottom menu buttons of disabled repeater.
        elem.find('#btnRepeaterBuilderExportToExcel.ui-state-disabled').removeClass('ui-state-disabled');
        //elem.find('#btnRepeaterBuilderView').addClass('ui-state-disabled');
        elem.find('#btnRepeaterBuilderAdd.ui-state-disabled').removeClass('ui-state-disabled');
        elem.find('#btnRepeaterBuilderDeleteui-state-disabled').removeClass('ui-state-disabled');
        elem.find("table.ui-jqgrid-btable").find(".jqgrow").prop('disabled', false);
        if (elem.hasClass('disabled')) { elem.removeClass('disabled') };
        elem.unbind("click");
    }
    function EnableAllChildElements(elements) {
        $.each(elements, function (index, element) {
            var elem;
            if (element.Type == "repeater") {
                elemId = '#repeater' + element.Name + currentInstance.currentFormInstanceId;
                EnableRepeater(elemId);
            }
            else if (element.Type == "section") {
                elem = $('#section' + element.Name + currentInstance.currentFormInstanceId);
                var section = element.Section;
                if (section.Elements != null) {
                    EnableAllChildElements(section.Elements);
                }
                if (elem.hasClass('disabled')) { elem.removeClass('disabled') };
            }
            else {
                elem = $('#' + element.Name + currentInstance.currentFormInstanceId);
                elem.parent('div').children().prop('disabled', false);
            }
        });
    }

    function ShowAllChildElements(elements) {
        $.each(elements, function (index, element) {
            var elem;
            if (element.Type == "repeater") {
                elem = $('#repeater' + element.Name + currentInstance.currentFormInstanceId);
                elem.css('display', 'block');
            }
            else if (element.Type == "section") {
                elem = $('#section' + element.Name + currentInstance.currentFormInstanceId);
                var section = element.Section;
                HideAllChildElements(section.Elements);
                elem.css('display', 'block');
            }
            else {
                elem = $('#' + element.Name + currentInstance.currentFormInstanceId);
                elem.parent('div').children().css('display', 'block');
            }
        });
    }

    function skipTheseElements(uiElementName, elem) {
        if (uiElementName === fullName.brgShowCopay || uiElementName === fullName.brgShowCoinsurance || uiElementName === fullName.brgShowAllowedAmt
            || uiElementName === fullName.brgShowAllowedCtr || uiElementName === fullName.brgShowDeductibles || uiElementName === fullName.brgShowLimits
            || uiElementName === fullName.brgShowMessages || uiElementName === fullName.brgShowOptions || uiElementName === fullName.brgShowPenalty)
            elem.prop('disabled', false);
        else {
            elem.prop('disabled', true);
        }
    }

    return {
        //functions to Disable Section
        SetSectionDisable: function (section) {
            elem = $('#section' + section.Name + currentInstance.currentFormInstanceId);
            if (section.Elements != null) {
                DisableAllChildElements(section.Elements);
            }
            elem.addClass('disabled');
        },

        //functions to Enable Section
        SetSectionEnable: function (section) {
            elem = $('#section' + section.SectionID + currentInstance.currentFormInstanceId);
            if (section.Elements != null) {
                EnableAllChildElements(section.Elements);
            }
            if (elem.hasClass('disabled')) { elem.removeClass('disabled') };
        },

        //functions to Hide Section
        SetSectionHidden: function (section) {
            elem = $('#section' + section.Name + currentInstance.currentFormInstanceId);
            if (section.Elements != null) {
                HideAllChildElements(section.Elements);
            }
            elem.css('display', 'none');
        },

        disableRepeater: function (repeaterID) {
            DisableRepeater(repeaterID)
        },

        //functions to Show Section
        SetSectionVisible: function (section) {
            elem = $('#section' + section.Name + currentInstance.currentFormInstanceId);
            if (section.Elements != null) {
                ShowAllChildElements(section.Elements);
            }
            elem.css('display', 'block');
        }
    }
}