var folderDialog = function () {
    var URLs = {
        //Gets the Account List
        accountList: '/ConsumerAccount/GetAccountList?tenantId=1',
        //Gets the MarketSegments List
        marketSegmentList: '/ConsumerAccount/GetMarketSegmentsList?tenantId=1',
        //Add new Folder
        folderAdd: '/ConsumerAccount/AddFolder',
        //Copy from existing folder
        folderCopy: '/ConsumerAccount/CopyFolder',

        folderVersionIndex: '/FolderVersion/Index?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&foldeViewMode={foldeViewMode}&mode={mode}',

        folderVersionGetNonPortfolioBasedAccountFolders: '/FolderVersion/GetNonPortfolioBasedAccountFolders?tenantId={tenantId}&folderVersionId={folderVersionId}&folderId={folderId}&mode={mode}',
        //Add Workflow States
        applicableTeamsAdd: '/FolderVersion/AddApplicableTeams',
        //Gets the Category List
        categoryListAccount: '/Settings/GetFolderVersionCategoryForDropdown?tenantId=1&isPortfolio=false&folderVersionID=0&isFinalized=true',
        categoryListPortfolio: '/Settings/GetFolderVersionCategoryForDropdown?tenantId=1&isPortfolio=true&folderVersionID=0&isFinalized=true',
        getMasterList: "/FormDesign/MasterListFormDesignList?tenantId=1",
    };
    var isPortfolioFolder;
    var foldeViewMode = FolderViewMode.DefaultView;
    var applicableTeamsRequired = false;
    var elementIDs = {
        //folder dialog element
        folderDialog: '#folderDialog',
        //id for folderName textbox
        folderName: "#folderName",
        //id for effectiveDate textbox
        effectiveDate: '#effectiveDate',
        //id for dropdownbox of account names
        accountNamesDDL: "#accountNames",
        //id for dropdownbox of marketSegment
        marketSegmentDDL: "#marketSegment",
        //id of hidden field for string folderID
        folderId: "#folderId",
        //id of hidden field for string folder Version
        folderVersion: "#folderVersion",
        //id for span of folder name
        folderNameSpan: "#folderNameSpan",
        //id for span of folder effective date
        effectiveDateSpan: "#effectiveDateSpan",
        accountNamesSpan: "#accountNamesSpan",
        marketSegmentSpan: "#marketSegmentSpan",
        //table element for the Non Portfolio Grid 
        nonPortfolioBasedAccountDetailGrid: 'npadg',
        //with hash for use with jQuery
        nonPortfolioBasedAccountDetailGridJQ: '#npadg',
        //table element for the Portfolio Grid 
        portfolioBasedAccountDetailGrid: 'padg',
        //with hash for use with jQuery
        portfolioBasedAccountDetailGridJQ: '#padg',
        portfolioDetailsGridJQ: '#pdg',
        checkAllId: '#checkAll',
        accountNamesAutoCompltDDLJQ: '#accountNamesAutoCompltDDL',
        divIsFoundationFolder: '#divIsFoundationFolder',
        chkIsFoundationFolderJQ: "#chkIsFoundationFolder",
		divCategory: '#divCategory',
        categoryNamesDDLJQ: '#categoryDDL',
        folderViewModeDDLJQ: '#folderViewModeDDL',
        categoryNamesAutoCompltDDLJQ: '#categoryNamesAutoCompltDDL',
        catIDJQ: '#catIDTXT',
        dopMasterListJQ: "#dopMasterList",
        dopMasterListHelpBlockJQ: "#dopMasterList-help-block",
        categorySpan: '#categorySpan',
        folderDialogSelectAccount: '#folderDialogSelectAccount',
        folderDialogApplicableTeams: '#folderDialogApplicableTeams'
    };

    //maintains dialog state - add  or copy
    var folderDialogState;
    function folderSuccess(xhr) {
        if (xhr.Result === ServiceResult.FAILURE || xhr.Result === 'undefined') {
            $(elementIDs.folderName).parent().addClass('has-error');
            $(elementIDs.folderName).addClass('form-control');
            $(elementIDs.folderNameSpan).text(ConsumerAccount.folderNameExistsMsg);
        }
        else {
            //code to save selected workflow states            
            if (applicableTeamsRequired == true) {
                var applicableTeamsIDs = new Array();
                $(elementIDs.folderDialog).find('.applicableTeamsCheckbox').each(function () {
                    if ($(this).is(':checked'))
                        applicableTeamsIDs.push(parseInt($(this).attr('id')));
                });
                var applicableTeamsAddData = {
                    FolderID: parseInt(xhr.Items[0].Messages[2]),
                    FolderVersionID: parseInt(xhr.Items[0].Messages[1]),
                    ApplicableTeamsIDList: applicableTeamsIDs,
                };

                //ajax call to add folder workflow states
                var promise = ajaxWrapper.postJSONCustom(URLs.applicableTeamsAdd, applicableTeamsAddData);
                //register ajax success callback
                promise.done(function (result) {
                    if (result.Result === ServiceResult.FAILURE) {
                        messageDialog.show("Applicable Teams are not saved.");
                        $(elementIDs.folderDialog).dialog('close');
                        return;
                    }
                    else {
                        redirectToFolder(xhr);
                    }
                });
                //register ajax failure callback
                promise.fail(showError);
            }
            else {
                redirectToFolder(xhr);
            }
        }
    }

    function redirectToFolder(xhr) {
        var Tempurl = URLs.folderVersionIndex
        if ($(elementIDs.portfolioYes) != undefined && $(elementIDs.portfolioYes) != null && $(elementIDs.portfolioYes).length > 0 && $(elementIDs.portfolioYes).is(":checked") == false) {
            var Tempurl = URLs.folderVersionGetNonPortfolioBasedAccountFolders
        }
        $(elementIDs.folderDialog).dialog('close');
        var url = Tempurl
            .replace(/{tenantId}/g, parseInt(xhr.Items[0].Messages[0]))
            .replace(/{folderVersionId}/g, parseInt(xhr.Items[0].Messages[1]))
            .replace(/{folderId}/g, parseInt(xhr.Items[0].Messages[2]))
            .replace(/{foldeViewMode}/g, foldeViewMode)
            .replace(/{mode}/g, true); //since edit button is clicked mode needs to be true
        window.location.href = url;
    }

    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }


    //To check  Account name is selected or not
    function validAccountName() {
        validAccount = false;
        if (($(elementIDs.accountNamesDDL)[0].selectedIndex == 0 || $(elementIDs.accountNamesDDL)[0].selectedIndex == -1) && $(elementIDs.accountNamesDDL).is(":disabled") == false) {
            $(elementIDs.accountNamesDDL).addClass('form-control');
            $(elementIDs.accountNamesDDL).parent().addClass('has-error');
            $(elementIDs.accountNamesSpan).text(ConsumerAccount.accountNameRequiredMsg);
            validAccount = false;
        } else {
            $(elementIDs.accountNamesDDL).removeClass('form-control');
            $(elementIDs.accountNamesDDL).parent().removeClass('has-error');
            $(elementIDs.accountNamesAutoCompltDDLJQ).removeClass('highlightBorder');
            $(elementIDs.accountNamesAutoCompltDDLJQ).siblings('a').removeClass('highlightBorder');
            $(elementIDs.accountNamesSpan).text('');
            validAccount = true;
        }
        return validAccount;
    }
    //To check Market Segment is selected or not
    function validMarketSegment() {
        validMarketSegmentt = false;
        if ($(elementIDs.marketSegmentDDL)[0].selectedIndex == 0) {
            $(elementIDs.marketSegmentDDL).addClass('form-control');
            $(elementIDs.marketSegmentDDL).parent().addClass('has-error');
            $(elementIDs.marketSegmentSpan).text(ConsumerAccount.marketSegmentRequiredMsg);
            validMarketSegmentt = false;
        } else {
            $(elementIDs.marketSegmentDDL).removeClass('form-control');
            $(elementIDs.marketSegmentDDL).parent().removeClass('has-error');
            $(elementIDs.marketSegmentSpan).text('');
            validMarketSegmentt = true;
        }
        return validMarketSegmentt;
    }
    //To check all validations for folder name
    function validFolderName() {
        validFolderNameFlag = true;
        var newFolderName = $(elementIDs.folderName).val();
        var newAccountName = $(elementIDs.accountNamesDDL).find('option:selected').text();
        if (newFolderName == '') {
            $(elementIDs.folderName).parent().addClass('has-error');
            $(elementIDs.folderName).addClass('form-control');
            $(elementIDs.folderNameSpan).text(ConsumerAccount.folderNameRequiredMsg);
            validFolderNameFlag = false;
        }
        else {
            $(elementIDs.folderNameSpan).text('')
            $(elementIDs.effectiveDateSpan).text('');
            $(elementIDs.folderName).parent().removeClass('has-error');
            $(elementIDs.folderName).removeClass('form-control');

            var rowIds = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getDataIDs();
            for (index = 0; index < rowIds.length; index++) {
                var folderData = $(elementIDs.nonPortfolioBasedAccountDetailGridJQ).getRowData(rowIds[index]);
                if (newFolderName === folderData.FolderName && newAccountName === folderData.AccountName) {
                    $(elementIDs.folderName).parent().addClass('has-error');
                    $(elementIDs.folderName).addClass('form-control');
                    $(elementIDs.folderNameSpan).text(ConsumerAccount.folderNameExistsMsg);
                    validFolderNameFlag = false;
                }
            }
            //}
            var check = $(elementIDs.folderNameSpan).text();
            if (check != ConsumerAccount.folderNameExistsMsg) {
                $(elementIDs.folderName).parent().removeClass('has-error');
                $(elementIDs.folderName).removeClass('form-control');
                $(elementIDs.folderNameSpan).text('')
                validFolderNameFlag = true;
            }
        }
        return validFolderNameFlag;
    }
    //To check effective date is specified or not
    function validEffectiveDate() {
        validEffectiveDateFlag = false;
        var newFolderEffectiveDate = $(elementIDs.effectiveDate).val();
        if (newFolderEffectiveDate == '') {
            $(elementIDs.effectiveDate).parent().addClass('has-error');
            $(elementIDs.effectiveDate).addClass('form-control');
            $(elementIDs.effectiveDateSpan).text(Common.folderEffectiveDateRequiredMsg);
            validEffectiveDateFlag = false
        }
        else {
            var effectiveDateMessage = isValidEffectiveDate(newFolderEffectiveDate);
            if (effectiveDateMessage == "") {
                $(elementIDs.effectiveDate).removeClass('form-control');
                $(elementIDs.effectiveDate).parent().addClass('has-error');
                $(elementIDs.effectiveDateSpan).text('');
                validEffectiveDateFlag = true;
            }
            else {
                $(elementIDs.effectiveDate).parent().addClass('has-error');
                $(elementIDs.effectiveDate).addClass('form-control');
                $(elementIDs.effectiveDateSpan).text(effectiveDateMessage);
                validEffectiveDateFlag = false
            }
        }
        return validEffectiveDateFlag;
    }

    function validateCategory() {
        validCategory = false;
        if ($(elementIDs.categoryNamesDDLJQ)[0].selectedIndex == 0 || $(elementIDs.categoryNamesDDLJQ)[0].selectedIndex == -1) {
            $(elementIDs.categoryNamesDDLJQ).addClass('form-control');
            $(elementIDs.categoryNamesDDLJQ).parent().addClass('has-error');
            $(elementIDs.categorySpan).text(ConsumerAccount.categoryRequiredMsg);
            validCategory = false;
        } else {
            $(elementIDs.categoryNamesDDLJQ).removeClass('form-control');
            $(elementIDs.categoryNamesDDLJQ).parent().removeClass('has-error');
            $(elementIDs.categoryNamesAutoCompltDDLJQ).removeClass('highlightBorder');
            $(elementIDs.categoryNamesAutoCompltDDLJQ).siblings('a').removeClass('highlightBorder');
            $(elementIDs.categorySpan).text('');
            validCategory = true;
        }
        return validCategory;
    }
    //To check flags from above functions
    function validateFolderData() {
        var isMarketSegmentValid = validMarketSegment();
        var isAccountNameValid = true;
        if (isPortfolioFolder == false) {
            isAccountNameValid = validAccountName();
        }
        var isFolderNameValid = validFolderName();
        var isEffectiveDateValid = validEffectiveDate();
        var isCategoryValid = validateCategory();

        if (isMarketSegmentValid == false || isAccountNameValid == false ||
        isFolderNameValid == false || isEffectiveDateValid == false || isCategoryValid == false) {
            return false;
        }
        return true;
    }

    function init() {
        //register dialog for grid row add/edit
        $(elementIDs.folderDialog).dialog({
            autoOpen: false,
            height: 'auto',
            width: 500,
            modal: true,
            close: function (event, ui) {
                $(elementIDs.categoryNamesDDLJQ).autoCompleteDropDown("destroy");
            }
        });

        //To display datepicker for folder effective date.
        $(elementIDs.effectiveDate).datepicker({
            dateFormat: 'mm/dd/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-61:c+20',
            showOn: "both",
            //CalenderIcon path declare in golbalvariable.js
            buttonImage: Icons.CalenderIcon,
            buttonImageOnly: true,
        }).parent().find('img').css('margin-bottom', '7px');

        $(elementIDs.chkIsFoundationFolderJQ).click(function (event) {
            if (this.checked) {
                $(elementIDs.categoryNamesAutoCompltDDLJQ).val("Foundation Build");
                $(elementIDs.categoryNamesDDLJQ + " > option").each(function () {
                    if (this.text == "Foundation Build")
                        $(elementIDs.categoryNamesDDLJQ).val(this.value);
                });
				$(elementIDs.categoryNamesDDLJQ).prop("disabled", true);
                $(elementIDs.categoryNamesAutoCompltDDLJQ).prop("disabled", true);
                $(elementIDs.divCategory + " .custom-combobox .ui-icon-triangle-1-s").attr("style", "display:none !important;");
            } else {
                $(elementIDs.categoryNamesAutoCompltDDLJQ).val("--Select Category--");
                $(elementIDs.categoryNamesDDLJQ).val(0);
				$(elementIDs.categoryNamesDDLJQ).prop("disabled", false);
                $(elementIDs.categoryNamesAutoCompltDDLJQ).prop("disabled", false);
                $(elementIDs.divCategory + " .custom-combobox .ui-icon-triangle-1-s").attr("style", "display:inline !important;");
            }
        });

        if (applicableTeamsRequired == true) {
            $(elementIDs.checkAllId).click(function (event) {
                if (this.checked) {
                    $('.applicableTeamsCheckbox').each(function () {
                        this.checked = true;
                    });

                } else {
                    $('.applicableTeamsCheckbox').each(function () {
                        if (!($(this).is(":disabled"))) {
                            this.checked = false;
                        }
                    });
                }
            });

            $(elementIDs.folderDialog).find('.applicableTeamsCheckbox').each(function () {
                $(this).click(function (event) {
                    if (!$(this).is(':checked')) {
                        $(elementIDs.checkAllId)[0].checked = false;
                    }
                });
            });
        }




        $(elementIDs.folderDialog + ' button').on('click', function () {
            var rowId = $(elementIDs.accountDetailGridJQ).getGridParam('selrow');
            var newFolderEffectiveDate = $(elementIDs.effectiveDate).val();
            var newFolderName = $(elementIDs.folderName).val();
            var categoryId = $(elementIDs.categoryNamesDDLJQ).val();
            var catId = $(elementIDs.catIDJQ).val();
            var isFoundation= $(elementIDs.chkIsFoundationFolderJQ).prop('checked');

            foldeViewMode = $(elementIDs.folderViewModeDDLJQ).val();
            if (categoryId == 0) categoryId = null;
            if (catId == undefined) catId = "";

            var validateFolderdata = validateFolderData();
            if (validateFolderdata == true) {
                if (applicableTeamsRequired == true) {
                    var applicableTeamsIDs = new Array();
                    $(elementIDs.folderDialog).find('.applicableTeamsCheckbox').each(function () {
                        if ($(this).is(':checked'))
                            applicableTeamsIDs.push(parseInt($(this).attr('id')));
                    });
                }
                if ((applicableTeamsRequired == false) || (applicableTeamsRequired == true && applicableTeamsIDs.length > 0)) {
                    var newFolderName = $(elementIDs.folderName).val();
                    var accountId;
                    var url;
                    var marketSegmentId = $(elementIDs.marketSegmentDDL).val();
                    var userId = 0;
                    var userName = "";
                    if (isPortfolioFolder == false) {
                        accountId = $(elementIDs.accountNamesDDL).val();
                    }

                    var folderAddData = {
                        tenantId: 1,
                        accountID: accountId,
                        folderName: newFolderName,
                        folderEffectiveDate: newFolderEffectiveDate,
                        isPortfolio: isPortfolioFolder,
                        userId: userId,
                        userName: userName,
                        marketSegmentId: marketSegmentId,
                        categoryID: categoryId,
                        catID: catId,
                        isFoundation: isFoundation
                    };

                    if (folderDialogState === 'add') {
                        url = URLs.folderAdd;
                    }
                    else {
                        folderAddData.originalFolderID = $(elementIDs.folderId).val();
                        folderAddData.originalFolderVersionID = $(elementIDs.folderVersion).val();
                        url = URLs.folderCopy;
                    }
                    //ajax call to add folder
                    var promise = ajaxWrapper.postJSON(url, folderAddData);
                    //register ajax success callback
                    promise.done(folderSuccess);
                    //register ajax failure callback
                    promise.fail(showError);
                }
                else {
                    messageDialog.show(WorkFlowStateMessages.atleastOneApplicableTeamMsg);
                    $(elementIDs.folderDialog).find('.applicableTeamsCheckbox').each(function () {
                        $(this).parent().addClass("has-error");
                    });
                }
            }
        });

    }
    //initialize the dialog after this js is loaded
    init();

    //to fill drop down list of account names
    function fillAccountNameDDL(selectedIndex) {
        var accName;
        //To refresh dropdown of account names
        $(elementIDs.accountNamesDDL).empty();
        $(elementIDs.accountNamesDDL).append("<option value=0>" + "--Select Account Name--" + "</option>");
        //ajax call for drop down list of account names
        var promise = ajaxWrapper.getJSON(URLs.accountList);
        promise.done(function (names) {
            for (i = 0; i < names.length; i++) {
                $(elementIDs.accountNamesDDL).append("<option value=" + names[i].AccountID + ">" + names[i].AccountName + "</option>");
                if (names[i].AccountID == selectedIndex)
                    accName = names[i].AccountName;
            }
            $(elementIDs.accountNamesDDL).val(selectedIndex);
            $(elementIDs.accountNamesAutoCompltDDLJQ).val(accName);
        });
        promise.fail(showError);

    }

    //to fill drop down list of Category names
    function fillCategoryDDL(categoryID) {
        var categoryName;
        //var defaultCategoryText = "-- Select Category --";
        //$(elementIDs.categoryNamesDDLJQ).children().remove();

        $(elementIDs.categoryNamesDDLJQ).empty();
        $(elementIDs.categoryNamesDDLJQ).append("<option value=0>" + "--Select Category--" + "</option>");
        //ajax call for drop down list of account names
        var url = "";
        if (isPortfolioFolder == true) {
            url = URLs.categoryListPortfolio;
        }
        else {
            url = URLs.categoryListAccount;
        }
        var promise = ajaxWrapper.getJSON(url);
        var folderVersionCategoryName = "";
        promise.done(function (names) {
            for (i = 0; i < names.length; i++) {
                // For creating Folder the group Change Request and JAA Change Request should not be include , Refer ANT-13
                //if (names[i].FolderVersionCategoryName != AntWFCategoriesGroups.ChangeRequest && names[i].FolderVersionCategoryName != AntWFCategoriesGroups.JAAChangeRequest) {
                // If Role is TPA Analyst then include only JAA New Group, else for all other roles (SuperUser,EBA Analyst) should include all
                //if (vbRole == Role.TPAAnalyst) {
                //if (names[i].FolderVersionCategoryName == AntWFCategoriesGroups.JAANewGroup) {
                //        $(elementIDs.categoryNamesDDLJQ).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                //}
                //}
                //else {
                $(elementIDs.categoryNamesDDLJQ).append("<option value=" + names[i].FolderVersionCategoryID + ">" + names[i].FolderVersionCategoryName + "</option>");
                //}
                //}
                if (categoryID == names[i].FolderVersionCategoryID) {
                    folderVersionCategoryName = names[i].FolderVersionCategoryName;
                }
            }
            if (categoryID > 0) {
                $(elementIDs.categoryNamesAutoCompltDDLJQ).val(folderVersionCategoryName);
                $(elementIDs.categoryNamesDDLJQ).val(categoryID);
            } else {
                $(elementIDs.categoryNamesAutoCompltDDLJQ).val("--Select Category--");
            }
        });

        promise.fail(showError);
    }

    //to fill drop down list of Category names
    function fillFolderViewModeDDL() {
        $(elementIDs.folderViewModeDDLJQ).empty();
        $(elementIDs.folderViewModeDDLJQ).append("<option value=" + FolderViewMode.DefaultView + ">Classic Folder View</option>");
        if (GLOBAL.applicationName.toLowerCase() != 'ebenefitsync') {
            $(elementIDs.folderViewModeDDLJQ).append("<option value=" + FolderViewMode.SOTView + ">SOT View</option>");
        }
    }

    //to check the folder dialog user claims
    function checkFolderDialogUserClaims() {
        //to check for user permissions.                                   
        if (claims != null && claims !== undefined) {
            var showFlag = true;
            var sectionData = [];
            claims.forEach(function (section) {
                var tempString = "ConsumerAccount/GetAccountList";
                if (tempString.toLowerCase() === section.Resource.toLowerCase()) {
                    sectionData.push(section);
                }
            });

            sectionData.forEach(function (claim) {
                if (claim.Action.toLowerCase() === "na") {
                    showFlag = false;
                }
                if (claim.Action.toLowerCase() === ConsumerAccount.doNotDisplayPortFolioQuestionMsg) {
                    showFlag = false;
                }
            });

            sectionData.forEach(function (claim) {
                if (claim.Action.toLowerCase() === ConsumerAccount.diplayPortFolioQuesMsg) {
                    showFlag = true;
                }
            });

            if (!showFlag) {
                $(elementIDs.folderWillBeUsedAsAPortfolioFolder).hide();
                //sales rep1 user should not create portfolio folders.
                $(elementIDs.folderNotRelatedToExistingAccount).prop("disabled", true);
            }
        } else {
            $(elementIDs.folderWillBeUsedAsAPortfolioFolder).hide();
        }
    }

    //to fill drop down list of market segment
    function fillMarketSegmentDDL(selectedIndex) {
        //To refresh dropdown of marketSegment
        $(elementIDs.marketSegmentDDL).empty();
        $(elementIDs.marketSegmentDDL).append("<option value=0>" + "--Select MarketSegment--" + "</option>");
        //ajax call for drop down list of market segment
        promise = ajaxWrapper.getJSON(URLs.marketSegmentList);
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.marketSegmentDDL).append("<option value=" + list[i].MarketSegmentId + " >" + list[i].MarketSegment + "</option>");
            }
            $(elementIDs.marketSegmentDDL).val(selectedIndex);
        });
        promise.fail(showError);
    }

    return {
        show: function (isPortfolio, row, action) {

            //Auto complete Start
            $(function () {
                $(elementIDs.accountNamesDDL).autoCompleteDropDown({ ID: 'accountNamesAutoCompltDDL' });
                $(elementIDs.accountNamesDDL).click(function () {
                    $(elementIDs.accountNamesDDL).toggle();
                });

                $(elementIDs.categoryNamesDDLJQ).autoCompleteDropDown({ ID: 'categoryNamesAutoCompltDDL' });
                $(elementIDs.categoryNamesDDLJQ).click(function () {
                    $(elementIDs.categoryNamesDDLJQ).toggle();
                });
            });

            isPortfolioFolder = isPortfolio;
            folderDialogState = action;

            $(elementIDs.folderDialog).removeClass('has-error');
            $(elementIDs.marketSegmentDDL).removeClass('form-control');
            if (isPortfolioFolder == false) {
                $(elementIDs.accountNamesDDL).removeClass('form-control');
            }
            if (applicableTeamsRequired == false) {
                $(elementIDs.folderDialogApplicableTeams).hide();
            }
            $(elementIDs.folderName).removeClass('form-control');
            $(elementIDs.effectiveDate).removeClass('form-control');

            if (folderDialogState == 'add') {
                $(elementIDs.folderDialog).dialog('option', 'title', 'Create Folder');
                //To refresh dialog
                $(elementIDs.folderName).val("");
                $(elementIDs.effectiveDate).val("");

                //find all the radio buttons in the dialog & remove the checked property for each of the radio button
                $(elementIDs.folderDialog + ' :radio').each(function (idx, control) {
                    $(control).prop('checked', false);
                });
                $(elementIDs.folderDialog + ' .help-block').text('');
                $(elementIDs.divIsFoundationFolder).hide();
                $(elementIDs.chkIsFoundationFolderJQ).prop('checked', false);

                if (isPortfolioFolder == true) {
                    $(elementIDs.folderDialogSelectAccount).hide();
                    if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                        $(elementIDs.divIsFoundationFolder).show();
                    }
                }
                else {
                    $(elementIDs.folderDialogSelectAccount).show();
                    fillAccountNameDDL("0");
                }
                fillMarketSegmentDDL("1");
                fillCategoryDDL(0);
                fillFolderViewModeDDL();
                $(elementIDs.categoryNamesDDLJQ).prop("disabled", false);
                $(elementIDs.categoryNamesAutoCompltDDLJQ).prop("disabled", false);

            }

            else {
                $(elementIDs.folderDialog).dialog('option', 'title', 'Copy Folder');
                $(elementIDs.folderName).val(row.FolderName);
                $(elementIDs.effectiveDate).val(row.EffectiveDate);

                $(elementIDs.folderId).val(row.FolderID);
                $(elementIDs.folderVersion).val(row.FolderVersionID);
                $(elementIDs.folderDialog + ' .help-block').text('');

                $(elementIDs.divIsFoundationFolder).hide();
                $(elementIDs.chkIsFoundationFolderJQ).prop('checked', false);
                //For disabling account names dropdown while copying from portfolio search grid   
                if (isPortfolioFolder == true) {
                    $(elementIDs.folderDialogSelectAccount).hide();
                    if (GLOBAL.applicationName.toLowerCase() == 'ebenefitsync') {
                        $(elementIDs.divIsFoundationFolder).show();
                    }
                }
                else {
                    $(elementIDs.folderDialogSelectAccount).show();
                    fillAccountNameDDL(row.AccountID);
                }
                fillMarketSegmentDDL("1");
                if (row.CategoryID == 2 || row.CategoryID == 4) {
                    fillCategoryDDL(0);
                } else {
                    fillCategoryDDL(row.CategoryID);
                }
                fillFolderViewModeDDL();
                $(elementIDs.categoryNamesDDLJQ).prop("disabled", true);
                $(elementIDs.categoryNamesAutoCompltDDLJQ).prop("disabled", true);
            }
            //open the dialog - uses jqueryui dialog
            $(elementIDs.folderDialog).dialog("open");
            //call the function to check for user claims for folder dialog.
            checkFolderDialogUserClaims();
        }
    }

}();


