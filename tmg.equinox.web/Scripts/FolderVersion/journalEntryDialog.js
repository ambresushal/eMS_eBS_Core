var journalEntryDialog = function () {

    var isInitialized = false;
    var maxCharacters = 300;
    var oldFormInstanceID = undefined;
    
    var URLs = {
        getJournalActionList: '/JournalReport/GetJournalActionList?formInstanceId={formInstanceId}&folderVersionId={folderVersionId}',
        saveJournalEntry: '/JournalReport/SaveJournalEntry?tenantId=1',
        saveJournalResponse: '/JournalReport/SaveJournalResponse?tenantId=1',
        //get All Journal Responses
        getAllJournalResponsesList: '/JournalReport/GetAllJournalResponsesList?journalId={journalId}',
        getCurrentJournal: '/JournalReport/GetCurrentJournal?journalId={journalId}'
    };
    
    var elementIDs = {
        journalEntryAddDialogue: "#journalEntryAddDialogueDiv",
        sectionTrailJQ: "#sectionTrail",
        journalAttributeJQ: "#journalAttribute",
        actionListDDLJQ: "#actionList",
        descriptionJQ: "#description",
        descriptionHistoryJQ: "#descriptionHistory",
        responseDivJQ: "#responseDiv",
        responseHistoryDivJQ: "#responseHistoryDiv",
        responseJQ: "#response",
        DescriptiondivJQ: "Descriptiondiv",
        responsecharacterCounterJQ: "#responsecharacterCounter",
        btnAddJournalEntryJQ: "#addJournalEntry",
        formInstanceIdJQ: "#formInstanceId",
        folderVersionIdJQ: "#folderVersionId",
        elementSectionJQ: "#elementSection",
        tenantIdJQ: "#tenantId",
        journalIdJQ: "#journalId",
        currentFormInstanceIdJQ: "#currentFormInstanceId",
        characterCounterJQ: "#characterCounter",
        responsecharacterCounterJQ: "#responsecharacterCounter",
        responseHistorytblDivJQ: "#responseHistorytblDiv",
        bottomMenuJQ: "#bottom-menu",
        bottomMenuTabsJQ: "#bottom-menu-tabs",
        btnAddJournalResponseJQ: "#addResponse",
        responseDivJQ: "#responseDiv",
        btnJournalEntryJQ: "#btnJournalEntry"
    };
    //this function is called below soon after this JS file is loaded 
    function init() {
        $(elementIDs.journalEntryAddDialogue).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: false,
            height: 'auto',
            width: '560px',
            modal: true,
            position: ['middle', 100]
        });
        $(elementIDs.journalEntryAddDialogue).dialog({
            close: function (event, ui) {
                $(elementIDs.journalIdJQ).text("");
                $(elementIDs.responseJQ).parent().removeClass('has-error');
                $(elementIDs.actionListDDLJQ).parent().removeClass('has-error');
                $(elementIDs.descriptionJQ).parent().removeClass('has-error');
                $(elementIDs.responseDivJQ).hide();
                $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'hidden');
                $(elementIDs.btnAddJournalResponseJQ).css('visibility', 'visibile');
                journalInstance.getInstance().getJournalInstance().highlightButton(false, $(elementIDs.btnJournalEntryJQ));
                journalInstance.getInstance().getJournalInstance().disableJournalAddMode($(elementIDs.elementSectionJQ).text(), $(elementIDs.formInstanceIdJQ).text(), oldFormInstanceID);
                $(elementIDs.responsecharacterCounterJQ).text(maxCharacters + JournalEntry.journalCharacterMaxMsg);
            }
        });
    }
    $(document).ready(function () {
        //initialize the dialog after this js are loaded
        if (isInitialized == false) {
            isInitialized = true;
            fillActionListDLL();
            // add click event for save button
            $(elementIDs.btnAddJournalEntryJQ).click(function (e) {
                var journalID = $(elementIDs.journalIdJQ).text();

                if (journalID != "" && journalID != undefined) {
                    //ajax call to get current journal status.
                    var url = URLs.getCurrentJournal.replace(/\{journalId\}/g, journalID);
                    var promise = ajaxWrapper.getJSON(url);

                    promise.done(function (xhr) {
                        var actionid = xhr[0].ActionID;
                        if (actionid == 2) {
                            var field = xhr[0].FieldName;
                            var sectionTrail = $(elementIDs.sectionTrailJQ).text();
                            var formInstanceId = xhr[0].FormInstanceID;
                            var folderVersionId = xhr[0].FolderVersionID;
                            var sectionName = $(elementIDs.elementSectionJQ).text();
                            var description = xhr[0].Description;
                            $(elementIDs.responseJQ).val('');
                            $(elementIDs.responseDivJQ).hide();
                            messageDialog.show(JournalEntry.journalErrorMsg);
                            journalEntryDialog.updateJournal(field, sectionTrail, actionid, formInstanceId, folderVersionId, sectionName, description, journalID, formInstanceId);
                        }
                        else {
                            return saveJournalEntry();
                        }
                    });
                    promise.fail(showError);
                }
                else {
                    return saveJournalEntry();
                }
            });
            $(elementIDs.btnAddJournalResponseJQ).click(function (e) { return addJournalResponse(); });
            $(elementIDs.actionListDDLJQ).change(function (e){ return enableResponseDiv(); });
            $(elementIDs.descriptionJQ).keyup(function (e) { return journalCharCount(); });
            $(elementIDs.characterCounterJQ).text(maxCharacters + JournalEntry.journalCharacterMaxMsg);
            $(elementIDs.responseJQ).keyup(function (e) { return journalResponseCharCount(); });
            $(elementIDs.responsecharacterCounterJQ).text(maxCharacters + JournalEntry.journalCharacterMaxMsg);
            $(elementIDs.responseDivJQ).hide();
            $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'hidden');
            isInitialized = true;
        }
        init();
    });
    //ajax failure callback
    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }
    //validate controls.
    function validateControls(journalID) {
        var isActionListSelected = false;
        $(elementIDs.actionListDDLJQ).removeClass('form-control');
        $(elementIDs.descriptionJQ).removeClass('form-control');
        $(elementIDs.actionListDDLJQ).parent().removeClass('has-error');
        $(elementIDs.descriptionJQ).parent().removeClass('has-error');

        if ($(elementIDs.actionListDDLJQ).val() != JournalEntryAction.SELECT.toString() && $(elementIDs.descriptionJQ).val() != "") {
            if (journalID == "") {
                $(elementIDs.responseJQ).removeClass('form-control');
                return isActionListSelected = true;
            }
            else if ($(elementIDs.responseJQ).val() == "") {
                $(elementIDs.responseJQ).parent().addClass('has-error');
                $(elementIDs.responseJQ).addClass('form-control');
                isActionListSelected = false;
            }
            else {
                $(elementIDs.responseJQ).removeClass('form-control');
                return isActionListSelected = true;
            }
        }
        else if ($(elementIDs.actionListDDLJQ).val() == JournalEntryAction.SELECT.toString() && $(elementIDs.descriptionJQ).val() == "") {
            $(elementIDs.actionListDDLJQ).parent().addClass('has-error');
            $(elementIDs.actionListDDLJQ).addClass('form-control');
            $(elementIDs.descriptionJQ).parent().addClass('has-error');
            $(elementIDs.descriptionJQ).addClass('form-control');
            isActionListSelected = false;
        }
        else if ($(elementIDs.actionListDDLJQ).val() == JournalEntryAction.SELECT.toString()) {
            $(elementIDs.actionListDDLJQ).parent().addClass('has-error');
            $(elementIDs.actionListDDLJQ).addClass('form-control');
            isActionListSelected = false;
        }
        else if ($(elementIDs.descriptionJQ).val() == "" && journalID == "") {
            $(elementIDs.descriptionJQ).parent().addClass('has-error');
            $(elementIDs.descriptionJQ).addClass('form-control');
            isActionListSelected = false;
        }
        else if ($(elementIDs.responseJQ).val() == "" && journalID != "") {
            $(elementIDs.responseJQ).parent().addClass('has-error');
            $(elementIDs.responseJQ).addClass('form-control');
            isActionListSelected = false;
        }
        else {
            $(elementIDs.actionListDDLJQ).removeClass('form-control');
            $(elementIDs.descriptionJQ).removeClass('form-control');
            isActionListSelected = true;
        }
        return isActionListSelected;
    }
    function saveJournalEntry() {
        var isValid = false;
        var actionID = 0;
        var journalID = $(elementIDs.journalIdJQ).text();
        var currentFormInstanceId = $(elementIDs.currentFormInstanceIdJQ).text();
        isValid = validateControls(journalID);
        if (isValid) {
            if (journalID != null && journalID != undefined && journalID != "") {
                //create url to save data.
                var url = URLs.saveJournalResponse;
                var data = {
                    formInstanceID: $(elementIDs.formInstanceIdJQ).text(),
                    folderVersionID: $(elementIDs.folderVersionIdJQ).text(),
                    response: $(elementIDs.responseJQ).val(),
                    actionID: $(elementIDs.actionListDDLJQ).val(),
                    journalId: $(elementIDs.journalIdJQ).text(),
                    GlobalCloseActionValue: JournalEntryAction.NO.toString()
                }
            }
            else {
                //create url to save data.
                var url = URLs.saveJournalEntry;
                var data = {
                    formInstanceID: $(elementIDs.formInstanceIdJQ).text(),
                    folderVersionID: $(elementIDs.folderVersionIdJQ).text(),

                    description: $(elementIDs.descriptionJQ).val(),
                    fieldName: $(elementIDs.journalAttributeJQ).text(),
                    fieldPath: $(elementIDs.sectionTrailJQ).text(),
                    actionID: $(elementIDs.actionListDDLJQ).val(),
                    tenantId: $(elementIDs.tenantIdJQ).text()
                }
            }
            var promise = ajaxWrapper.postJSON(url, data);
            //callback function for ajax request success.
            promise.done(function (xhr) {
                journalInstance.getInstance().getJournalInstance().disableJournalAddMode($(elementIDs.elementSectionJQ).text(), $(elementIDs.formInstanceIdJQ).text(), oldFormInstanceID);
                //var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(currentFormInstanceId);
                var frmInstId = folderManager.getInstance().getFolderInstance().currentFormInstanceID;
                var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(frmInstId);
                formInstancebuilder.journal.InitializeOpenJournals();
                if (data.response == undefined) {
                    $(elementIDs.journalEntryAddDialogue).dialog('close');
                    if ($('#isPreviousVersion').is(":checked")) {
                        formInstancebuilder.bottomMenu.showJournalGridTab(true);
                    }
                    else {
                        formInstancebuilder.bottomMenu.showJournalGridTab(false);
                    }
                    //reset dialog elements
                    resetAddDialogElements();
                }
                else {
                    formInstancebuilder.bottomMenu.closeBottomMenu();
                    resetUpdateDialogElements(data.actionID, journalID);
                    if ($('#isPreviousVersion').is(":checked")) {
                        formInstancebuilder.journal.loadJournalEntry(true);
                    }
                    else {
                        formInstancebuilder.journal.loadJournalEntry(false);
                    }
                }
                
            });
            journalInstance.getInstance().getJournalInstance().highlightButton(false, $(elementIDs.btnJournalEntryJQ));
            promise.fail(showError);
        }
    }
    function addJournalResponse(e) {
        var journalID = $(elementIDs.journalIdJQ).text();
        //ajax call to get current journal status.
        var url = URLs.getCurrentJournal.replace(/\{journalId\}/g, journalID);
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (xhr) {
            var actionid = xhr[0].ActionID;
            if (actionid == 2) {
                var field = xhr[0].FieldName;
                var sectionTrail = $(elementIDs.sectionTrailJQ).text();
                var formInstanceId = xhr[0].FormInstanceID;
                var folderVersionId = xhr[0].FolderVersionID;
                var sectionName = $(elementIDs.elementSectionJQ).text();
                var description = xhr[0].Description;
                messageDialog.show(JournalEntry.journalErrorMsg);
                journalEntryDialog.updateJournal(field, sectionTrail, actionid, formInstanceId, folderVersionId, sectionName, description, journalID, formInstanceId);
            }
            else {
                $(elementIDs.btnAddJournalResponseJQ).css('visibility', 'hidden');
                $(elementIDs.responseDivJQ).show();
                $(elementIDs.DescriptiondivJQ).hide();
                $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'visibile');
            }
        });
        promise.fail(showError);
    }
    function enableResponseDiv() {
        if ($(elementIDs.actionListDDLJQ).val() == JournalEntryAction.NO.toString() && $(elementIDs.journalEntryAddDialogue).dialog("option", "title") == JournalEntry.updateJournalEntryMsg) {
            addJournalResponse();
        }
    }
    function resetAddDialogElements() {
        $(elementIDs.journalEntryAddDialogue + ' div').removeClass('has-error');
        $(elementIDs.journalIdJQ).text("");
        $(elementIDs.responseDivJQ).hide();
        $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'hidden');
        $(elementIDs.btnAddJournalResponseJQ).css('visibility', 'visibile');
        $(elementIDs.responsecharacterCounterJQ).text(maxCharacters + JournalEntry.journalCharacterMaxMsg);
    }
    function resetUpdateDialogElements(actionid, journalID) {
        $(elementIDs.journalEntryAddDialogue + ' div').removeClass('has-error');
        $(elementIDs.responseDivJQ).hide();
        $(elementIDs.responsecharacterCounterJQ).text(maxCharacters + JournalEntry.journalCharacterMaxMsg);
        $(elementIDs.responseHistoryDivJQ).removeClass('field-validation-valid');

        $(elementIDs.responseHistorytblDivJQ).text("");
        if (actionid == JournalEntryAction.YESOPEN.toString()) {
            $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'hidden');
            $(elementIDs.btnAddJournalResponseJQ).css('visibility', 'visibile');

            $(elementIDs.journalEntryAddDialogue).dialog('option', 'title', JournalEntry.updateJournalEntryMsg);
            $(elementIDs.actionListDDLJQ).val(JournalEntryAction.YESOPEN.toString());
            $(elementIDs.actionListDDLJQ).removeAttr('disabled', 'disabled');
            $(elementIDs.btnAddJournalEntryJQ).removeClass('ui-state-disabled');
            $(elementIDs.responseJQ).val("");
            $(elementIDs.responseDivJQ).removeClass('field-validation-valid');

            loadResponseHistoryTable(journalID);
        }
        else {
            $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'hidden');
            $(elementIDs.btnAddJournalResponseJQ).css('visibility', 'hidden');

            $(elementIDs.journalEntryAddDialogue).dialog('option', 'title', JournalEntry.viewJournalEntryMsg);
            $(elementIDs.actionListDDLJQ).val(JournalEntryAction.NO.toString());
            $(elementIDs.actionListDDLJQ).attr('disabled', 'disabled');
            $(elementIDs.btnAddJournalEntryJQ).addClass('ui-state-disabled');
            $(elementIDs.responseDivJQ).addClass('field-validation-valid');

            loadResponseHistoryTable(journalID);
        }

    }
    function fillActionListDLL() {
        $(elementIDs.actionListDDLJQ).empty();
        $(elementIDs.actionListDDLJQ).append("<option value='0' 'selected'>" + "--Select One--" + "</option>");

        var url = URLs.getJournalActionList;
        var promise = ajaxWrapper.getJSON(url);

        //fill the form list drop down
        promise.done(function (list) {
            for (i = 0; i < list.length; i++) {
                $(elementIDs.actionListDDLJQ).append("<option value=" + list[i].ActionId + ">" + list[i].ActionName + "</option>");
            }
        });
        promise.fail(showError);
    }

    function journalCharCount() {

        var len = $(elementIDs.descriptionJQ).val().length;
        if (len >= maxCharacters) {
            $(elementIDs.characterCounterJQ).text(JournalEntry.journalCharacterLimitMsg);
        } else {
            var ch = maxCharacters - len;
            $(elementIDs.characterCounterJQ).text(ch + JournalEntry.journalCharacterMaxMsg);
        }
    }
    function myDateFormatter(dateObject) {
        var d = new Date(dateObject);
        var day = d.getDate();
        var month = d.getMonth() + 1;
        var year = d.getFullYear();
        if (day < 10) {
            day = "0" + day;
        }
        if (month < 10) {
            month = "0" + month;
        }
        var date = day + "/" + month + "/" + year;
        return date;
    };
    function journalResponseCharCount() {

        var len = $(elementIDs.responseJQ).val().length;
        if (len >= maxCharacters) {
            $(elementIDs.responsecharacterCounterJQ).text(JournalEntry.journalCharacterLimitMsg);
        } else {
            var ch = maxCharacters - len;
            $(elementIDs.responsecharacterCounterJQ).text(ch + JournalEntry.journalCharacterMaxMsg);
        }
    }

    function loadResponseHistoryTable(journalID) {

        //ajax call for journal list of forminstance.       
        var url = URLs.getAllJournalResponsesList.replace(/\{journalId\}/g, journalID);
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (responseList) {
            var journalResponseDescriptionRows = new Array();
            var tblObject = "<table class='table table-condensed' style='table-layout:fixed !important;margin-bottom: 0px !important;'>";
            tblObject = tblObject + "<thead><tr><th class='col-md-8.5'>Response</th><th class='col-md-2'>Added By</th><th class='col-md-2'>Added Date</th></tr></thead></table><div class='div-table-content'><table class='table table-condensed' style='table-layout:fixed !important;margin-bottom: 0px !important;'><tbody>";

            for (var i = 0; i < responseList.length; i++) {
                var responseDescriptionListObject = i + 1 + ". " + responseList[i].Description + "\n";
                journalResponseDescriptionRows.push(responseDescriptionListObject);
                var dt = new Date(responseList[i].AddedDate);
                tblObject = tblObject + "<tr><td class='col-md-8.5' style='word-wrap:break-word !important;'>" + responseList[i].Description + "</td><td class='col-md-2' style='text-align:center !important;'>" + responseList[i].AddedBy + "</td><td class='col-md-2' style='text-align:center !important;'>" + myDateFormatter(responseList[i].AddedDate) + "</td></tr>";
            }
            tblObject = tblObject + "</tbody></table></div>"
            if (journalResponseDescriptionRows.length > 0) {
                $(elementIDs.responseHistorytblDivJQ).append(tblObject);
            }
            else {
                $(elementIDs.responseHistoryDivJQ).addClass('field-validation-valid');
            }
        });
        promise.fail(showError);
    }

    return {
        addJournal: function (sectionTrail, elementName, formInstanceId, folderVersionId, sectionName, tenantId, currentFormInstanceId) {
            $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'visibile');
            $(elementIDs.btnAddJournalResponseJQ).css('visibility', 'hidden');
            $(elementIDs.descriptionHistoryJQ).addClass('field-validation-valid');
            $(elementIDs.descriptionJQ).removeClass('field-validation-valid');
            $(elementIDs.characterCounterJQ).removeClass('field-validation-valid');
            $(elementIDs.responseDivJQ).addClass('field-validation-valid');
            $(elementIDs.responseHistoryDivJQ).addClass('field-validation-valid');
            $(elementIDs.btnAddJournalEntryJQ).removeClass('ui-state-disabled');
            
            //var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(form);
            var frmInstId = folderManager.getInstance().getFolderInstance().currentFormInstanceID;
            var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(frmInstId);
            formInstancebuilder.bottomMenu.closeBottomMenu();

            $(elementIDs.journalEntryAddDialogue).dialog('option', 'title', JournalEntry.addJournalEntryMsg);
            $(elementIDs.journalEntryAddDialogue).css('overflow', 'hidden');
            $(elementIDs.journalEntryAddDialogue).dialog("open");

            $(elementIDs.sectionTrailJQ).text(sectionTrail);
            $(elementIDs.actionListDDLJQ).removeAttr('disabled', 'disabled');
            $(elementIDs.actionListDDLJQ).prop('selectedIndex', JournalEntryAction.SELECT.toString());
            $(elementIDs.descriptionJQ).val("");
            $(elementIDs.characterCounterJQ).text(maxCharacters + JournalEntry.journalCharacterMaxMsg);

            $(elementIDs.formInstanceIdJQ).text(formInstanceId);
            $(elementIDs.folderVersionIdJQ).text(folderVersionId);
            $(elementIDs.elementSectionJQ).text(sectionName);
            $(elementIDs.tenantIdJQ).text(tenantId);
            $(elementIDs.currentFormInstanceIdJQ).text(currentFormInstanceId);
           
            if (elementName != undefined) {
                $(elementIDs.journalAttributeJQ).text(elementName.Label);
            }
            else {
                $(elementIDs.journalAttributeJQ).text(JournalEntry.attributeNotApplicableMsg);
            }

        },

        updateJournal: function (field, sectionTrail, actionid, formInstanceId, folderVersionId, sectionName, description, journalID, currentFormInstanceId) {

            $(elementIDs.descriptionJQ).addClass('field-validation-valid');
            $(elementIDs.characterCounterJQ).addClass('field-validation-valid');
            $(elementIDs.descriptionHistoryJQ).removeClass('field-validation-valid');
            $(elementIDs.descriptionHistoryJQ).text("");
            $(elementIDs.descriptionHistoryJQ).append(description);
            $(elementIDs.responseHistoryDivJQ).removeClass('field-validation-valid');

            //var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(currentFormInstanceId);
            var frmInstId = folderManager.getInstance().getFolderInstance().currentFormInstanceID;
            var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(frmInstId);
            formInstancebuilder.bottomMenu.closeBottomMenu();

            $(elementIDs.responseHistorytblDivJQ).text("");
            $(elementIDs.sectionTrailJQ).text(sectionTrail);
            if (actionid == JournalEntryAction.YESOPEN.toString()) {
                $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'hidden');
                $(elementIDs.btnAddJournalResponseJQ).css('visibility', 'visibile');

                $(elementIDs.journalEntryAddDialogue).dialog('option', 'title', JournalEntry.updateJournalEntryMsg);
                $(elementIDs.actionListDDLJQ).val(JournalEntryAction.YESOPEN.toString());
                $(elementIDs.actionListDDLJQ).removeAttr('disabled', 'disabled');
                $(elementIDs.btnAddJournalEntryJQ).removeClass('ui-state-disabled');
                $(elementIDs.responseJQ).val("");
                $(elementIDs.responseDivJQ).removeClass('field-validation-valid');

                loadResponseHistoryTable(journalID);

                if (!folderManager.getInstance().getFolderInstance().isEditable) {
                    $(elementIDs.btnAddJournalResponseJQ).attr('disabled', 'disabled');
                    $(elementIDs.actionListDDLJQ).attr('disabled', 'disabled');
                    $(elementIDs.btnAddJournalEntryJQ).addClass('ui-state-disabled');
                    $(elementIDs.responseDivJQ).addClass('field-validation-valid');
                }
            }
            else {
                $(elementIDs.btnAddJournalEntryJQ).css('visibility', 'hidden');
                $(elementIDs.btnAddJournalResponseJQ).css('visibility', 'hidden');

                $(elementIDs.journalEntryAddDialogue).dialog('option', 'title', JournalEntry.viewJournalEntryMsg);
                $(elementIDs.actionListDDLJQ).val(JournalEntryAction.NO.toString());
                $(elementIDs.actionListDDLJQ).attr('disabled', 'disabled');
                $(elementIDs.btnAddJournalEntryJQ).addClass('ui-state-disabled');
                $(elementIDs.responseDivJQ).addClass('field-validation-valid');

                loadResponseHistoryTable(journalID);
            }
            
            $(elementIDs.journalEntryAddDialogue).css('overflow', 'hidden');
            $(elementIDs.journalEntryAddDialogue).dialog("open");

            $(elementIDs.characterCounterJQ).text(maxCharacters + JournalEntry.journalCharacterMaxMsg);
            if (field != undefined) {
                $(elementIDs.formInstanceIdJQ).text(formInstanceId);
                $(elementIDs.folderVersionIdJQ).text(folderVersionId);
                $(elementIDs.elementSectionJQ).text(sectionName);
                $(elementIDs.journalAttributeJQ).text(field);
                $(elementIDs.journalIdJQ).text(journalID);
                $(elementIDs.currentFormInstanceIdJQ).text(currentFormInstanceId);
            }
            else {
                $(elementIDs.journalAttributeJQ).text(JournalEntry.attributeNotApplicableMsg);
            }
        },
    }
}();