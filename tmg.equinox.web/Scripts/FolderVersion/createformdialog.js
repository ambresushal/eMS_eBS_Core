var createFormDialog = function () {

    var isInitialized = false;
    var isReferenceRequired = false;
    var usesMasterListAliasDesign = false;
    var formDesignVersionId = 0;
    var folderId = 0;
    var folderVersionId = 0;
    //urls to be accessed for create/copy form.
    var URLs = {
        getFormTypeList: '/FolderVersion/GetFormTypeList?tenantId=1&folderType={folderType}&effectiveDate={effectiveDate}&folderId={folderId}',
        getFormList: '/FolderVersion/GetFormList?tenantId=1&folderVersionId={folderVersionId}',
        getFolderList: '/FolderVersion/GetFolderList?tenantId=1&accountType={accountType}&accountId={accountId}&categoryId={categoryId}&isFoundation={isFoundation}',
        saveFormInstance: '/FolderVersion/SaveFormInstance?tenantId=1',
        getFormNameToCopy: '/FolderVersion/GetFormNameToCopy?tenantId=1&formInstanceId={formInstanceId}',
        getAccountList: '/FolderVersion/GetAccountList?tenantId=1',
        getFolderVersionList: '/FolderVersion/GetFolderVersionList?tenantId=1&folderId={folderId}',

    };
    var isPortfolioScreen = false;
    //element ID's required for create/copy form.
    //added in Views/FolderVersion/Index.cshtml
    var elementIDs = {
        createFormPopup: '#createForm',
        divDocumentName: '#dvDocumentName',
        divDocumentType: '#dvDocumentType',
        divAccount: '#dvAccount',
        divFolder: '#dvFolder',
        divFolderVersion: '#dvFolderVersion',
        divDocument: '#dvDocument',

        divDocumentNameError: '#dvDocumentNameError',
        divDocumentTypeError: '#dvDocumentTypeError',
        divAccountError: '#dvAccountError',
        divFolderError: '#dvFolderError',
        divFolderVersionError: '#dvFolderVersionError',
        divDocumentError: '#dvDocumentError',

        rdoCreateTypeJQ: 'createType',
        documentNameJQ: '#txtDocumentName',
        documentTypeJQ: '#drpDocumentType',
        accountJQ: '#drpAccount',
        folderJQ: '#drpFolder',
        folderVersionJQ: '#drpFolderVersion',
        documentJQ: '#drpDocument',
        btnSaveJQ: '#btnSave',
        rdoCreateNew: '#createNew',
        divReference: '#referenceDiv',
        dvFolderType: '#dvFolderType',
        dvFolderTypeError: '#dvFolderTypeError',

        drpFolderType: '#drpFolderType'
    };

    //this function is called below soon after this JS file is loaded
    function init() {
        var currentInstance = this;
        $(document).ready(function () {
            if (isInitialized == false) {
                var width = 670;
                if (isReferenceRequired == false) {
                    width = 500;
                }
                $(elementIDs.createFormPopup).dialog({ autoOpen: false, width: width, height: 400, modal: true });

                //set the create or copy mode depending upon radio button selected.
                showHideElements();

                $(elementIDs.documentTypeJQ).change(function (e) {
                    var documentTypeId = $(this).val();
                    if (documentTypeId != '0') {
                        $(elementIDs.divDocumentTypeError).empty();
                    }
                });

                $(elementIDs.documentNameJQ).change(function (e) {
                    var name = $(this).val();
                    $(elementIDs.divDocumentNameError).empty();
                });

                // add click event for save button
                $(elementIDs.btnSaveJQ).click(function (e) {
                    return saveFormInstance();
                });
                $('input:radio[name="' + elementIDs.rdoCreateTypeJQ + '"]').change(function () {
                    $(elementIDs.documentNameJQ).val('');
                    setDefaultDropDownValue(elementIDs.folderJQ);
                    setDefaultDropDownValue(elementIDs.folderVersionJQ);
                    setDefaultDropDownValue(elementIDs.documentJQ);
                    $(elementIDs.documentTypeJQ).val("0");
                    $(elementIDs.accountJQ).val("0").autoDropdown("refresh");

                    $(elementIDs.divDocumentNameError).empty();
                    $(elementIDs.divDocumentTypeError).empty();
                    $(elementIDs.divAccountError).empty();
                    $(elementIDs.dvFolderTypeError).empty();
                    $(elementIDs.divFolderError).empty();
                    $(elementIDs.divFolderVersionError).empty();
                    $(elementIDs.divDocumentError).empty();
                });

                var dialogInstance = currentInstance;
                //Auto complete Start
                $(function () {
                    $(elementIDs.accountJQ).autoDropdown({
                        select: function (event, ui) {
                            var accountId = $(this).val();
                            fillFolderDropDown(2, dialogInstance.folderData.CategoryId, accountId, false);
                            $(elementIDs.divAccountError).empty();
                        }
                    });
                    $(elementIDs.drpFolderType).autoDropdown({
                        select: function (event, ui) {
                            $(elementIDs.accountJQ).empty();
                            $(elementIDs.folderJQ).empty();
                            var typeID = $(this).val();
                            if ($("#drpFolderType option:selected").text() == 'Account') {
                                typeID = 2;
                            }
                            if (typeID == 1 || typeID == 3) {
                                $(elementIDs.divAccount).hide();
                                fillFolderDropDown(typeID, dialogInstance.folderData.CategoryId, 0, typeID == 3 ? true : false);
                                $(elementIDs.dvFolderTypeError).empty();
                            }
                            if (typeID == 2) {
                                $(elementIDs.divAccount).show();
                                fillAccountDropDown();
                                $(elementIDs.dvFolderTypeError).empty();
                            }
                        }
                    });
                    $(elementIDs.folderJQ).autoDropdown({
                        select: function (event, ui) {
                            var folderId = $(this).val();
                            if (folderId != '0') {
                                fillFolderVersionDropDown(folderId);
                                $(elementIDs.divFolderError).empty();
                            }
                        }
                    });
                    $(elementIDs.folderVersionJQ).autoDropdown({
                        select: function (event, ui) {
                            var folderVersionId = $(this).val();
                            if (folderVersionId != '0') {
                                fillDocumentDropDown(folderVersionId);
                                $(elementIDs.divFolderVersionError).empty();
                            }
                        }
                    });
                    $(elementIDs.documentJQ).autoDropdown({
                        select: function (event, ui) {
                            var documentId = $(this).val();
                            if (documentId != '0') {
                                $(elementIDs.divDocumentError).empty();
                            }
                        }
                    });
                });

                isInitialized = true;
            }
        });
    }
    init();

    function showHideElements() {
        $(elementIDs.divDocumentType).hide();
        $("input[name=createType]:radio").on("change", function () {
            if (usesMasterListAliasDesign == true) {
                if (this.value == 'new') {
                    $(elementIDs.divDocument).hide();
                }
                else {
                    $(elementIDs.divDocument).show();
                }
            }
            else {
                if (this.value == 'new') {
                    $(elementIDs.divDocumentType).show();
                    $(elementIDs.dvFolderType).hide();
                    $(elementIDs.divFolder).hide();
                    $(elementIDs.divFolderVersion).hide();
                    $(elementIDs.divDocument).hide();
                }
                else {
                    $(elementIDs.divDocumentType).hide();
                    $(elementIDs.dvFolderType).show();
                    $(elementIDs.divFolder).show();
                    $(elementIDs.divFolderVersion).show();
                    $(elementIDs.divDocument).show();
                }
                $(elementIDs.divAccount).hide();

            }
        });
    }

    function fillDocumentTypeDropDown() {
        $(elementIDs.documentTypeJQ).empty();
        $(elementIDs.documentTypeJQ).append("<option value='0'>" + "Select One" + "</option>");

        //ajax call for drop down list of account names
        var promise = ajaxWrapper.getJSON(URLs.getFormTypeList.replace(/{folderType}/g, folderData.folderType)
                                                    .replace(/{effectiveDate}/g, folderData.effectiveDate).replace(/{folderId}/g, folderData.folderId));
        var currentInstance = this;
        promise.done(function (list) {
            if (list.length != 0) {

                if (folderData.isPortfolio == "False" || folderData.isPortfolio == false) {
                    var list = list.filter(function (obj) {
                        return obj.FormDesignID != FormTypes.MEDICAREFORMDESIGNID;
                    });
                }
                //else if(folderData.isPortfolio=="True"||folderData.isPortfolio==true)
                //{
                //    var list = list.filter(function(obj) {
                //        return obj.FormDesignID != FormTypes.COMMERCIALMEDICAL;
                //    });
                //}

                for (i = 0; i < list.length; i++) {
                    if (list[i].FormDesignID != null) {
                        $(elementIDs.documentTypeJQ).append("<option value=" + list[i].FormVersionDesignID + ">" + list[i].FormTypeName + "</option>");
                    }
                }
            }
        });
        promise.fail(showError);
    }

    function fillAccountDropDown() {
        $(elementIDs.accountJQ).empty();
        $(elementIDs.accountJQ).append("<option value='0'>" + "Select One" + "</option>");

        var promise = ajaxWrapper.getJSON(URLs.getAccountList);
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.accountJQ).append("<option value=" + list[i].AccountID + ">" + list[i].AccountName + "</option>");
            }
        });

        promise.fail(showError);
    }

    function fillFolderTypeDropDown(folderType) {
        $(elementIDs.drpFolderType).empty();
        if (folderType == "Medicare") {
            $(elementIDs.drpFolderType).append("<option value='0'>Select One</option>");
            $(elementIDs.drpFolderType).append("<option value='1'>Portfolio</option>");
            if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                $(elementIDs.drpFolderType).append("<option value='3'>Foundation</option>");
            }
        }
        else {
            $(elementIDs.drpFolderType).append("<option value='0'>Select One</option>");
            $(elementIDs.drpFolderType).append("<option value='1'>Portfolio</option>");
            $(elementIDs.drpFolderType).append("<option value='2'>Account</option>");
            if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                $(elementIDs.drpFolderType).append("<option value='3'>Foundation</option>");
            }
        }
    }


    function fillFolderDropDown(accountType, categoryId, accountId, isFoundation) {
        $(elementIDs.folderJQ).empty();
        if (accountId == null) {
            accountId = 0;
        }
        $(elementIDs.folderJQ).append("<option value='0'>" + "Select One" + "</option>");
        var url = URLs.getFolderList.replace(/\{accountId\}/g, accountId).replace(/\{accountType\}/g, accountType).replace(/\{categoryId\}/g, categoryId).replace(/\{isFoundation\}/g, isFoundation);
        var promise = ajaxWrapper.getJSON(url);
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.folderJQ).append("<option value=" + list[i].FolderId + ">" + list[i].FolderName + "</option>");
            }

            $(elementIDs.folderJQ).val("0").autoDropdown("refresh");
            setDefaultDropDownValue(elementIDs.folderVersionJQ);
            setDefaultDropDownValue(elementIDs.documentJQ);
        });

        promise.fail(showError);
    }


    function fillFolderVersionDropDown(folderId) {
        $(elementIDs.folderVersionJQ).empty();
        $(elementIDs.folderVersionJQ).append("<option value='0'>" + "Select One" + "</option>");

        if (folderId != "0") {
            var url = URLs.getFolderVersionList.replace(/\{folderId\}/g, folderId);

            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.folderVersionJQ).append("<option value=" + list[i].FolderVersionId + ">" + list[i].FolderVersionNumber + "</option>");
                }
                $(elementIDs.folderVersionJQ).val("0").autoDropdown("refresh");
                setDefaultDropDownValue(elementIDs.documentJQ);
            });
            promise.fail(showError);
        }
    }

    function fillDocumentDropDown(folderVersionId) {
        $(elementIDs.documentJQ).empty();
        $(elementIDs.documentJQ).append("<option value='0'>" + "Select One" + "</option>");

        if (folderVersionId != "0") {
            var url = URLs.getFormList.replace(/\{folderVersionId\}/g, folderVersionId);

            var promise = ajaxWrapper.getJSON(url);
            promise.done(function (list) {
                for (i = 0; i < list.length; i++) {
                    $(elementIDs.documentJQ).append("<option value=" + list[i].FormInstanceID + ">" + list[i].FormDesignName + "</option>");
                }
                $(elementIDs.documentJQ).val("0").autoDropdown("refresh");
            });

            promise.fail(showError);
        }
    }

    function fillDocumentDropDownML(currentInstance) {
        $(elementIDs.documentJQ).empty();
        for (var prop in currentInstance.folderInstance.formInstances) {
            var fi = currentInstance.folderInstance.formInstances[prop];
            $(elementIDs.documentJQ).append("<option value=" + fi.FormInstance.FormInstanceID + ">" + fi.FormInstance.FormInstanceName + "</option>");
        }
    }

    function setDefaultDropDownValue(e) {
        if (usesMasterListAliasDesign == false) {
            $(e).empty();
            $(e).append("<option value='0'>" + "Select One" + "</option>");
            $(e).val("0").autoDropdown("refresh");
        }
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }


    //validate controls.
    function validateControls(selectedOption) {
        var isValid = true;

        if ($(elementIDs.documentNameJQ).val() == '') {
            $(elementIDs.divDocumentNameError).empty();
            var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divDocumentNameError);
            $span.html(FolderVersion.documentNameRequiredMsg);
            isValid = false;
        }
        var rx = /[<>:"\/\\|?*\x00-\x1F]|^(?:aux|con|clock\$|nul|prn|com[1-9]|lpt[1-9])$/i;
        if (rx.test($(elementIDs.documentNameJQ).val())) {
            $(elementIDs.divDocumentNameError).empty();
            var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divDocumentNameError);
            $span.html("Document Name contains invalid character.");
            isValid = false;
        }
        if ($("input[name=" + elementIDs.rdoCreateTypeJQ + "]:checked").val() == "new") {
            if ($(elementIDs.documentTypeJQ).val() == '0') {
                $(elementIDs.divDocumentTypeError).empty();
                var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divDocumentTypeError);
                $span.html(FolderVersion.documentTypeRequiredMsg);
                isValid = false;
            }
        }
        else {

            if ($(elementIDs.drpFolderType).val() == "0") {
                $(elementIDs.dvFolderTypeError).empty();
                var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.dvFolderTypeError);
                $span.html(FolderVersion.selectFolderTypeRequiredMsg);
                isValid = false;
            }
            else {
                if ($(elementIDs.drpFolderType).val() == "2") {
                    if ($(elementIDs.accountJQ).val() == '0') {
                        $(elementIDs.divAccountError).empty();
                        var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divAccountError);
                        $span.html(FolderVersion.selectAccountRequiredMsg);
                        isValid = false;
                    }
                }
            }

            if ($(elementIDs.folderJQ).val() == '0') {
                $(elementIDs.divFolderError).empty();
                var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divFolderError);
                $span.html(FolderVersion.selectFolderRequiredMsg);
                isValid = false;
            }
            if ($(elementIDs.folderVersionJQ).val() == '0') {
                $(elementIDs.divFolderVersionError).empty();
                var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divFolderVersionError);
                $span.html(FolderVersion.selectFoldeVersionRequiredMsg);
                isValid = false;
            }
            if ($(elementIDs.documentJQ).val() == '0') {
                $(elementIDs.divDocumentError).empty();
                var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divDocumentError);
                $span.html(FolderVersion.selectDocumentRequiredMsg);
                isValid = false;
            }



        }

        if (!isValid) { $(".messageContainer").addClass('has-error'); }

        return isValid;
    }

    function validateControlsML() {
        var isValid = true;
        var docName = $(elementIDs.documentNameJQ).val();
        if (docName == 'Select One') {
            $(elementIDs.divDocumentNameError).empty();
            var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divDocumentNameError);
            $span.html(FolderVersion.documentNameRequiredMsg);
            isValid = false;
        }
        var rx = /[<>:"\/\\|?*\x00-\x1F]|^(?:aux|con|clock\$|nul|prn|com[1-9]|lpt[1-9])$/i;
        if (rx.test(docName)) {
            $(elementIDs.divDocumentNameError).empty();
            var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divDocumentNameError);
            $span.html("Document Name contains invalid character.");
            isValid = false;
        }
        $(elementIDs.documentJQ + ' > option').each(function () {
            var txt = $(this).text();
            if (txt == docName) {
                var $span = $('<small/>').addClass('help-block').appendTo(elementIDs.divDocumentNameError);
                $span.html("Document Name already exists in Folder.");
                isValid = false;
            }
        });

        if (!isValid) { $(".messageContainer").addClass('has-error'); }
        return isValid;
    }
    function saveFormInstance() {
        var selectOption = $("input[name=" + elementIDs.rdoCreateTypeJQ + "]:checked").val();
        var formDesignVersionID = 0;
        var formInstanceID = 0;
        var accountID = 0;
        var folderID = 0;
        var folderVersionID = 0;
        var isCopyFlag = false;
        var isReferenceFlag = false;
        var isValid = false;
        //check all input fields for validation.
        if (usesMasterListAliasDesign == false) {
            isValid = validateControls(selectOption);
        }
        else {
            isValid = validateControlsML();
        }

        if (isValid) {

            newFormName = $(elementIDs.documentNameJQ).val();
            if (usesMasterListAliasDesign == false) {
                formDesignVersionID = $(elementIDs.documentTypeJQ).val();
                accountID = $(elementIDs.accountJQ).val() != null ? $(elementIDs.accountJQ).val() : 0;
                folderID = $(elementIDs.folderJQ).val();
                folderVersionID = $(elementIDs.folderVersionJQ).val();
                formInstanceID = $(elementIDs.documentJQ).val();
                isReferenceFlag = $("input[name=" + elementIDs.rdoCreateTypeJQ + "]:checked").val() == "reference" && isReferenceRequired == true ? true : false;
            }
            else {
                folderID = folderData.folderId;
                formDesignVersionID = formDesignVersionId;
                formInstanceID = $(elementIDs.documentJQ).val();
            }
            isCopyFlag = $("input[name=" + elementIDs.rdoCreateTypeJQ + "]:checked").val() == "copy" ? true : false;
            isReferenceFlag = $("input[name=" + elementIDs.rdoCreateTypeJQ + "]:checked").val() == "reference" && isReferenceRequired == true ? true : false;

            //create url to save data.
            var url = URLs.saveFormInstance;

            var data = {
                folderVersionId: folderData.folderVersionId,
                formDesignVersionId: formDesignVersionID,
                formInstanceId: formInstanceID,
                accountId: accountID,
                folderId: folderID,
                refFolderId: folderVersionID,
                isCopy: isCopyFlag,
                isReference: isReferenceFlag,
                formName: newFormName
            }

            var promise = ajaxWrapper.postJSON(url, data);
            //callback function for ajax request success.
            promise.done(function (xhr) {
                if (xhr.Result === ServiceResult.FAILURE) {
                    //if result is failuer the show that form instance alreaady exists.
                    if (xhr.Items.length > 0) {
                        messageDialog.show(xhr.Items[0].Messages[0]);
                    } else {
                        messageDialog.show(FolderVersion.duplicateDocumentInstanceMsg);
                    }
                }
                else {
                    //add form instance to selected folder instance.
                    //createFormDialog.folderInstance.addFormInstance(xhr);
                    folderManager.getInstance().getFolderInstance().addFormInstance(xhr);
                    $(elementIDs.createFormPopup).dialog('close');
                }
                //reset dialog elements
                $(elementIDs.createFormPopup + ' div').removeClass('has-error');
            });
            promise.fail(showError);
        }
        return false;
    }

	function clearDropdowns() {
        $(elementIDs.documentNameJQ).val('');
        $(elementIDs.documentTypeJQ).val('0');
        $(elementIDs.accountJQ).empty();
        $(elementIDs.accountJQ).append("<option value='0'>" + "Select One" + "</option>");
        $(elementIDs.accountJQ).val('0').autoDropdown("refresh");
        $(elementIDs.folderJQ).empty();
        $(elementIDs.folderJQ).append("<option value='0'>" + "Select One" + "</option>");
        $(elementIDs.folderJQ).val('0').autoDropdown("refresh");
        $(elementIDs.folderVersionJQ).empty();
        $(elementIDs.folderVersionJQ).append("<option value='0'>" + "Select One" + "</option>");
        $(elementIDs.folderVersionJQ).val('0').autoDropdown("refresh");
        $(elementIDs.documentJQ).empty();
        $(elementIDs.documentJQ).append("<option value='0'>" + "Select One" + "</option>");
        $(elementIDs.documentJQ).val('0').autoDropdown("refresh");
    }
	
    return {
        show: function (isPortfolio) {
            var currentInstance = this;
            usesMasterListAliasDesign = false;
            var fi = this.folderInstance;
            if (fi.formInstances != null && fi.formInstances.length > 0) {
                for (var prop in fi.formInstances) {
                    if (fi.formInstances[prop].FormInstance.UsesMasterListAliasDesign == true) {
                        usesMasterListAliasDesign = true;
                        formDesignVersionId = fi.formInstances[prop].FormInstance.FormDesignVersionID;
                        break;
                    }
                };
            }
            if (fi.IsMasterList == true && usesMasterListAliasDesign == true) {
                $(elementIDs.dvFolderType).hide();
                $(elementIDs.divAccount).hide();
                $(elementIDs.divFolder).hide();
                $(elementIDs.divFolderVersion).hide();
                $(elementIDs.divReference).hide();
                $(elementIDs.createFormPopup).dialog('option', 'title', 'New Document')
                $(elementIDs.createFormPopup).dialog('open');
                fillDocumentDropDownML(currentInstance);
            }
            else {
                $(elementIDs.createFormPopup + ' div').removeClass('has-error');
                $(elementIDs.createFormPopup + ' .help-block').text('');
                isPortfolioScreen = isPortfolio;
                //refresh form name 
                $(elementIDs.createFormPopup).dialog('option', 'title', 'New Document')
                $(elementIDs.createFormPopup).dialog('open');
                if (isReferenceRequired == false) {
                    $(elementIDs.divReference).hide();
                }
                else {
                    $(elementIDs.divReference).show();
                }
                clearDropdowns();
                if (isPortfolio == true) {
                    fillFolderTypeDropDown("Medicare");
                    $(elementIDs.drpFolderType).val('0').autoDropdown("refresh");
                    $(elementIDs.divAccount).hide();
                }
                else {
                    fillFolderTypeDropDown();
                    $(elementIDs.drpFolderType).val('0').autoDropdown("refresh");
                    $(elementIDs.dvFolderType).removeAttr("disabled");
                }
                fillDocumentTypeDropDown();
            }
            //if (folderData.RoleId != Role.EBAAnalyst && folderData.RoleId != Role.TPAAnalyst
            //    && folderData.RoleId != Role.SuperUser) {
            //    $(":radio[value=new]").hide();
            //    $(elementIDs.rdoCreateNew).hide();
            //}


        },
        setFolder: function (folderinst) {
            this.folderInstance = folderinst;
        },
        folderInstance: undefined
    }
}();

(function ($) {
    $.widget("custom.autoDropdown", {
        _create: function () {
            this.wrapper = $("<span>")
                .addClass("custom-combobox")
                .insertAfter(this.element);

            this.element.hide();
            this._createAutocomplete();
            this._createShowAllButton();
        },

        _createAutocomplete: function () {
            var selected = this.element.children(":selected"),
                value = selected.val() ? selected.text() : "";

            this.input = $("<input>")
                .appendTo(this.wrapper)
                .val(value)
                .attr("title", "")
                .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
                .autocomplete({
                    delay: 0,
                    minLength: 0,
                    source: $.proxy(this, "_source")
                })
                .tooltip({
                    tooltipClass: "ui-state-highlight"
                });

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                },

                autocompletechange: "_removeIfInvalid"
            });
        },

        _createShowAllButton: function () {
            var input = this.input,
                wasOpen = false;
            $("<a> <span class='ui-button-icon-primary ui-icon ui-icon-triangle-1-s' style='margin: 0 10 0 0 !important;'><span class='ui-button-text'>")
            .attr("tabIndex", -1)
            .attr("title", "Show All Items")
            .tooltip()
            .appendTo(this.wrapper)
            .button({
                icons: {
                    primary: "ui-icon-triangle-1-s"
                },
                text: false
            })
            .removeClass("ui-corner-all")
            .addClass("ui-button ui-widget ui-state-default ui-button-icon-only custom-combobox-toggle ui-corner-right")
            .mousedown(function () {
                wasOpen = input.autocomplete("widget").is(":visible");
            })
            .click(function () {
                input.focus();

                // Close if already visible
                if (wasOpen) {
                    return;
                }

                // Pass empty string as value to search for, displaying all results
                input.autocomplete("search", "");
            });
        },

        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text))) return {
                    label: text,
                    value: text,
                    option: this
                };
            }));
        },

        _removeIfInvalid: function (event, ui) {

            // Selected an item, nothing to do
            if (ui.item) {
                return;
            }

            // Search for a match (case-insensitive)
            var value = this.input.val(),
                valueLowerCase = value.toLowerCase(),
                valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });

            // Found a match, nothing to do
            if (valid) {
                return;
            }

            // Remove invalid value
            this.input.val("")
                .attr("title", value + " didn't match any item")
                .tooltip("open");
            this.element.val("");
            this._delay(function () {
                this.input.tooltip("close").attr("title", "");
            }, 2500);
            this.input.autocomplete("instance").term = "";
        },
        // added- inserted refresh function -BD
        refresh: function () {
            selected = this.element.children(":selected");
            this.input.val(selected.text());
        },
        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        }
    });
})(jQuery);