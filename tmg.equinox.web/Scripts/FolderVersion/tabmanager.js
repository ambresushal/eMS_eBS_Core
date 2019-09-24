function tabManager(folderInstance) {
    this.IsMasterList = folderInstance.IsMasterList;
    this.objFolder = folderInstance;
    this.tabs = undefined;
    this.elementIDs = {
        forminstancelistjq: '#foldertabs',
        formTabJQ: '#formTabs',
        sectionMenuContainer: '#sectionMenuContainer',
        sectionMenuContainerBottom: '#sectionMenuContainerBottom',
        documentDesignContainerJQ: "#documentDesignContainer",
        documentContainerDivJQ: "#documentContainer",
        documentHeaderContainerJQ: "#documentheadercontainer",
        sotContainerDivJQ: '#sotviewcontainer',
        btnMenuOptions: '#btnMenuOptions',
        btnSaveFormData: '#btnSaveFormData',
        btnBottomSaveFormData: '#btnBottomSaveFormData',
        bottommenu: "#bottom-menu",
        btnReloadFormData: '#btnReloadFormData',
        btnValidate: '#btnValidate',
        btnJournalEntry: '#btnJournalEntry',
        btnReplaceText: '#btnReplaceText',
        btnExportToPDf: '#btnExportToPDf',
        btnViewFaxBackReport: '#btnViewFaxBackReport',
        btnViewBenefitMatrixReport: '#btnViewBenefitMatrixReport',
        btnCascadeML: '#btnCascadeML',
        btnViewCollateral: '#btnViewCollateral',
        btnExitValidate: '#btnExitValidate',
        btnValidateSection: '#btnValidateSection',
        collateralReportMenuItems: '.collateralReportMenuItem'
    }
}

tabManager.prototype.initialize = function () {
    var currentInstance = this;
    $(currentInstance.elementIDs.sotContainerDivJQ).hide();
    $(currentInstance.elementIDs.sectionMenuContainer).show();
    $(currentInstance.elementIDs.sectionMenuContainerBottom).show();
    $(currentInstance.elementIDs.formTabJQ).show();
    currentInstance.tabs = $(currentInstance.elementIDs.forminstancelistjq).tabs();
    $(currentInstance.elementIDs.folderContainerJQ).css({ "padding-bottom": "0px" });
}

tabManager.prototype.addTab = function (formInstance) {
    var currentInstance = this;
    var tabName = formInstance.FormDesignName.substring(0, formInstance.FormDesignName.indexOf("@@") == -1 ? formInstance.FormDesignName.length : formInstance.FormDesignName.indexOf("@@"));
    var anchorformInstanceId = formInstance.AnchorDocumentID
    var tabTemplate = "<li><a href='#{href}' id='form-tab-" + anchorformInstanceId + "' data-journal='" + tabName + "'>{label}</a><span class='ui-icon ui-icon-close' data-tabid=" + anchorformInstanceId + " role='presentation'>Remove Tab</span></li>";
    //replace based on version being loaded for this form
    var li = $(tabTemplate.replace(/#\{href\}/g, '#tab' + anchorformInstanceId).replace(/\{label\}/g, tabName));
    //create div for the tab page content
    if (currentInstance.IsMasterList) {
        currentInstance.tabs.find('.ui-tabs-nav').find('li').remove();
        currentInstance.tabs.tabs('refresh');
    }
    currentInstance.tabs.find('.ui-tabs-nav').append(li);
    currentInstance.tabs.append("<div id='tab" + anchorformInstanceId + "'></div>");
    var currentTabIndex = currentInstance.tabs.find('li[aria-selected="true"]').index();
    var allTabs = currentInstance.tabs.find('li').length;
    if (currentInstance.IsMasterList) {
        currentInstance.tabs.find('span.ui-icon-close').remove();
    }
    else {
        currentInstance.tabs.find('.ui-tabs-nav #form-tab-' + anchorformInstanceId + ' ~ span.ui-icon-close').click(function () {
            var closestLi = $(this).closest('li');
            if (closestLi != undefined) {
                currentInstance.closeTab(closestLi, anchorformInstanceId);
            }
        });
    }
    currentInstance.tabs.tabs('refresh');

}

tabManager.prototype.closeTab = function (element, anchorformInstanceId) {
    var currentInstance = this;
    var message = "";
    message = message + Common.closeView;
    yesNoConfirmDialog.show(message, function (e) {
        yesNoConfirmDialog.hide();
        if (e) {
            var isClosingActiveTab = element.attr('aria-selected') == "true";
            var currentTabIndex = currentInstance.tabs.find('li[aria-selected="true"]').index();
            var allTabs = currentInstance.tabs.find('li').length;
            if (currentTabIndex == 0 && allTabs == 1) {
                $(currentInstance.elementIDs.btnMenuOptions).hide();
                $(currentInstance.elementIDs.btnSaveFormData).hide();
                currentInstance.disableButtons();
            }
            //currentInstance.tabs.remove(currentTabIndex);
            var panelId = element.remove().attr('aria-controls');
            $('#' + panelId).remove();
            currentInstance.objFolder.openDocuments[anchorformInstanceId].Status = OpenDocumentStatus.Close;
            if (isClosingActiveTab) {
                $(currentInstance.elementIDs.sectionMenuContainer).css('display', 'none');
                if (currentTabIndex > currentInstance.tabs.find("li:not(.ui-state-disabled)").length - 1)
                    currentTabIndex = currentTabIndex - 1;
                if (currentTabIndex > -1) {
                    currentInstance.tabs.tabs('option', 'active', currentTabIndex + 1);
                }
            }
            var url = '/FolderVersion/ReleaseDocumentLock?tenantId=1&formInstanceId={FormInstanceId}';
            var promise = ajaxWrapper.getJSON(url.replace(/{FormInstanceId}/g, anchorformInstanceId));

            promise.done(function (result) {
                if (result.Result === ServiceResult.SUCCESS)
                    console.log("Document Lock for " + anchorformInstanceId + " Released..");
            });
            currentInstance.tabs.tabs('refresh');
        }

    });
}

tabManager.prototype.registerEvent = function () {
    var currentInstance = this;

    //register tab event - on tab click, set the current tab/forminstance
    currentInstance.tabs.tabs({
        activate: function (event, ui) {
            //fetch oldPanel data
            var oldFormInstanceID = ui.oldPanel.selector.replace(/#tab/g, '');
            if (oldFormInstanceID) {
                oldFormInstanceID = currentInstance.objFolder.openDocuments[oldFormInstanceID].SelectedViewID;
            }

            //check if previous tab form has some changes if yes, save the data
            //get old form instance data from the array.
            var oldformInstanceData = currentInstance.objFolder.formInstances[oldFormInstanceID];
            if (oldformInstanceData != undefined && oldformInstanceData.FormInstanceBuilder != undefined) {
                var oldFormInstanceBuilderObj = oldformInstanceData.FormInstanceBuilder;
                oldFormInstanceBuilderObj.autosave.clearAutoSaveTimeOut();
            }
            if (oldFormInstanceBuilderObj != undefined && oldFormInstanceBuilderObj.form.hasChanges() && folderData.isEditable == "True") {
                if (folderData.isAutoSaveEnabled == "True") {
                    autoConfirmDialog.show((AutoSave.confirmMsg).replace(/\{#documnentName}/g, oldFormInstanceBuilderObj.formName), function (e) {
                        if (e == true) {
                            oldFormInstanceBuilderObj.autosave.postMessageAndSaveData();
                        }
                        else {
                            console.log("saved in cache.");
                            currentInstance.objFolder.formInstances[oldFormInstanceID].FormInstanceBuilder.reload(false);
                            //oldFormInstanceBuilderObj.form.saveSectionData(oldFormInstanceBuilderObj.selectedSection, null);
                        }
                        autoConfirmDialog.hide(function () {
                            $(oldFormInstanceBuilderObj.elementIDs.folderAutoSaveAlertJQ).hide();
                        });
                    });
                } else {
                    oldFormInstanceBuilderObj.form.saveFormInstanceData(false, true);
                }
            }

            var docId = ui.newPanel.selector.replace(/#tab/g, '');
            folderManager.getInstance().loadFormInstance(docId, oldFormInstanceID);
        }
    });
}

tabManager.prototype.setActiveTab = function () {
    var currentInstance = this;
    var indexToActive;
    indexToActive = currentInstance.tabs.find("li:not(.ui-state-disabled)").last().index();
    currentInstance.tabs.tabs('option', 'active', indexToActive);
    currentInstance.tabs.tabs('refresh');
}

tabManager.prototype.setAttributes = function () {
    var currentInstance = this;
    if ($(currentInstance.elementIDs.forminstancelistjq + ' li:not(.ui-state-disabled)').length == 0) {
        currentInstance.objFolder.disableButton();
    }
}

tabManager.prototype.disableButtons = function () {
    var currentInstance = this;
    $(currentInstance.elementIDs.bottommenu).hide();
    $(currentInstance.elementIDs.btnReloadFormData).addClass('disabled-button');
    $(currentInstance.elementIDs.btnValidate).addClass('disabled-button');
    $(currentInstance.elementIDs.btnJournalEntry).addClass('disabled-button');
    $(currentInstance.elementIDs.btnReplaceText).addClass('disabled-button');
    $(currentInstance.elementIDs.btnExportToPDf).addClass('disabled-button');
    $(currentInstance.elementIDs.btnViewFaxBackReport).addClass('disabled-button');
    $(currentInstance.elementIDs.btnViewBenefitMatrixReport).addClass('disabled-button');
    $(currentInstance.elementIDs.btnCascadeML).addClass('disabled-button');
    $(currentInstance.elementIDs.btnViewCollateral).addClass('disabled-button');
    $(currentInstance.elementIDs.btnExitValidate).addClass('disabled-button');
    $(currentInstance.elementIDs.btnValidateSection).addClass('disabled-button');
    $(currentInstance.elementIDs.collateralReportMenuItems).children().addClass('disabled-button');
}
