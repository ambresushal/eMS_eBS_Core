function sotManager(folderInstance) {
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
        folderContainerJQ: '.foldercontainer',
        sectionGroupJQ: '.sec-grp',
        filterInputJQ: '#filterInput',
        btnMenuOptions: '#btnMenuOptions',
        btnSaveFormData: '#btnSaveFormData',
        btnBottomSaveFormData: '#btnBottomSaveFormData',
        btnBottomCreateNewVersion: '#btnBottomCreateNewVersion',
        btnBaselineJQ: "#btnBaseline",
        btnNewVersionJQ: "#newVersionHistory",
        btnRetroJQ: "#retroVersionHistory",
        btnBottomGenerateSOTFile: '#btnBottomGenerateSOTFile',
        btnValidateJQ: "#btnValidate",
        btnValidateAllJQ: '#btnValidateAll',
        btnStatusUpdateJQ: "#statusupdatebutton",
        btnJournalEntryJQ: "#btnJournalEntry",
        btnDeleteFormInstance: "#btnDeleteForm",
        assignFolderMemberbutton: '#assignFolderMemberbutton'
    }
}

sotManager.prototype.initialize = function () {
    var currentInstance = this;
    $(currentInstance.elementIDs.documentHeaderContainerJQ).show();
    $(currentInstance.elementIDs.sotContainerDivJQ).show();
    $(currentInstance.elementIDs.sotContainerDivJQ).find('.ui-sotview-titlebar').hide();
    $(currentInstance.elementIDs.sectionMenuContainer).hide();
    $(currentInstance.elementIDs.sectionMenuContainerBottom).hide();
    $(currentInstance.elementIDs.formTabJQ).hide();
    $(currentInstance.elementIDs.folderContainerJQ).css({ "padding-bottom": "25px" });
}

sotManager.prototype.addTab = function (formInstance) {
    var currentInstance = this;
    var preFormInstanceId = "";

    var anchorformInstanceId = formInstance.AnchorDocumentID
    var totalDocuments = Object.keys(currentInstance.objFolder.formInstances).length;
    var pos = currentInstance.getPosition(formInstance, totalDocuments);

    //Add document info as header
    var effDate = currentInstance.getEffectiveDate(formInstance);
    $(currentInstance.elementIDs.documentHeaderContainerJQ).find('.docheader').removeClass('active');
    $(currentInstance.elementIDs.documentHeaderContainerJQ).show();
    var docHeader = currentInstance.addDocumentHeader("header" + anchorformInstanceId, formInstance.FormDesignName, effDate, formInstance.FolderVersionNumber);
    switch (pos.action) {
        case 'Add':
            $(currentInstance.elementIDs.documentHeaderContainerJQ).append(docHeader);
            $(currentInstance.elementIDs.documentContainerDivJQ).append("<div id='tab" + anchorformInstanceId + "' class='doccontent ui-sotview-bdiv'></div>");
            break;
        case 'InsertAfter':
            $(docHeader).insertAfter("#header" + pos.sibling);
            $("<div id='tab" + anchorformInstanceId + "' class='doccontent ui-sotview-bdiv'></div>").insertAfter("#tab" + pos.sibling);
            break;
        case 'InsertBefore':
            $(docHeader).insertBefore("#header" + pos.sibling);
            $("<div id='tab" + anchorformInstanceId + "' class='doccontent ui-sotview-bdiv'></div>").insertBefore("#tab" + pos.sibling);
            break;
    }

    //rearrange divs
    currentInstance.resizeView(currentInstance.objFolder.formInstances, totalDocuments);
    var objHilight = currentInstance.isHilightRequired(pos, formInstance);
    if (objHilight.required == true) {
        folderManager.getInstance().loadFormInstance(formInstance.AnchorDocumentID, preFormInstanceId, objHilight);
    }
    else {
        folderManager.getInstance().loadFormInstance(formInstance.AnchorDocumentID, preFormInstanceId);
    }
    $(currentInstance.elementIDs.documentContainerDivJQ).find("div#tab" + anchorformInstanceId).click(function () { currentInstance.setCurrentTab(this, "tab"); });
    $(currentInstance.elementIDs.documentHeaderContainerJQ).find("div#header" + anchorformInstanceId).click(function () { currentInstance.setCurrentTab(this, "header"); });
    currentInstance.setResizable(anchorformInstanceId);
    var currentUrl = window.location.href;
    var mode = currentUrl.substring(currentUrl.lastIndexOf('=') + 1, window.location.href.length);
    if (currentInstance.objFolder.formInstances[anchorformInstanceId].FormInstance.IsFormInstanceEditable == false || mode == "false")
        currentInstance.disableMenuLevelButtons();
    else
        currentInstance.enableMenuLevelButtons();
}

sotManager.prototype.isHilightRequired = function (pos, formInstance) {
    var currentInstance = this;

    var objHilight = { required: false };
    if (pos.sibling != undefined && pos.sibling != null) {
        var isSrcDocumentReleased = currentInstance.objFolder.formInstances[pos.sibling].FormInstance.FolderVersionStateID;
        var isTargetDocumentReleased = currentInstance.objFolder.formInstances[formInstance.FormInstanceID].FormInstance.FolderVersionStateID;
        if (isSrcDocumentReleased == isTargetDocumentReleased) {
            return objHilight;
        }
    }
    if (pos.action == 'InsertAfter') {
        var number = formInstance.FolderVersionNumber.split('.');
        if (number.length > 1 && parseInt(number[1]) > 0) {
            objHilight = { required: true, source: pos.sibling, target: formInstance.FormInstanceID };
        }
    }
    if (pos.action == 'InsertBefore') {
        var forms = currentInstance.objFolder.formInstances.filter(function (n, i) {
            return (i == pos.sibling);
        });

        if (forms.length > 0) {
            var number = forms[0].FormInstance.FolderVersionNumber.split('.');
            if (number.length > 1 && parseInt(number[1]) > 0) {
                objHilight = { required: true, source: formInstance.FormInstanceID, target: pos.sibling };
            }
        }
    }

    return objHilight;
}

sotManager.prototype.getEffectiveDate = function (formInstance) {
    var effectiveDate = '';

    if (formInstance.EffectiveDate) {
        var dt = new Date(formInstance.EffectiveDate);
        effectiveDate = (dt.getMonth() + 1) + '/' + dt.getDate() + '/' + dt.getFullYear();
    }

    return effectiveDate;
}

sotManager.prototype.getPosition = function (formInstance, totalDocuments) {
    var currentInstance = this;
    var result = { action: 'Add', sibling: null }, min, max;

    var forms = currentInstance.objFolder.formInstances.filter(function (n, i) {
        return (n.FormInstance.DocID == formInstance.DocID && i != formInstance.FormInstanceID);
    });

    if (forms.length > 0) {
        $.each(forms, function (index, instance) {
            if (instance.FormInstance.FormInstanceID < formInstance.FormInstanceID) {
                min = instance.FormInstance.FormInstanceID;
            }
            else {
                max = instance.FormInstance.FormInstanceID;
            }
        });
        if (min != null || min != undefined) {
            result = { action: 'InsertAfter', sibling: min };
        }
        else if (max != null || max != undefined) {
            result = { action: 'InsertBefore', sibling: max };
        }
    }

    return result;
}

sotManager.prototype.setCurrentTab = function (element, type) {
    var currentInstance = this;
    var oldFormInstanceID = "";
    var instanceId = element.id.replace(type, "");
    if (currentInstance.objFolder.currentFormInstanceID != instanceId) {
        folderManager.getInstance().loadFormInstance(instanceId, oldFormInstanceID);
        $(currentInstance.elementIDs.documentHeaderContainerJQ).find('.docheader').removeClass('active');
        $(currentInstance.elementIDs.documentHeaderContainerJQ).find("div#header" + instanceId).addClass('active');
        var currentUrl = window.location.href;
        var mode = currentUrl.substring(currentUrl.lastIndexOf('=') + 1, window.location.href.length);
        if (currentInstance.objFolder.formInstances[instanceId].FormInstance.IsFormInstanceEditable == false || mode == "false")
            currentInstance.disableMenuLevelButtons();
        else
            currentInstance.enableMenuLevelButtons();
    }
}

sotManager.prototype.setResizable = function (formInstanceId) {
    var headerid = "div#header" + formInstanceId;
    var contentid = "div#tab" + formInstanceId;

    //set header resizable - also connect with content
    $(headerid).resizable({
        alsoResize: contentid,
        handles: 'e'
    });

    $(contentid).resizable({
        alsoResize: headerid,
        handles: 'e'
    });
}

sotManager.prototype.addDocumentHeader = function (id, name, effdate, version) {
    var currentInstance = this;
    var formInstanceId = id.replace("header", "");

    var docHeader = document.createElement("div");
    docHeader.id = id;
    docHeader.className = "docheader active ui-sotview-hbox";

    var table = document.createElement("table");
    table.className = "ui-sotview-htable";

    var tabHeader = document.createElement("thead");
    var tableRow = document.createElement("tr");
    tableRow.className = "ui-sotview-labels";
    var tableCell = document.createElement("th");

    var nameDiv = document.createElement("div");
    nameDiv.innerText = name;
    var closeDiv = document.createElement("div");
    closeDiv.className = "alert-close";
    closeDiv.innerText = "x";
    closeDiv.onclick = function () {
        currentInstance.closeTab(formInstanceId);
    };
    nameDiv.appendChild(closeDiv);

    tableCell.appendChild(nameDiv);
    tableRow.appendChild(tableCell);
    tabHeader.appendChild(tableRow);

    tableRow = document.createElement("tr");
    tableRow.className = "ui-sotview-shead";
    tableCell = document.createElement("th");
    tableCell.className = "ui-state-default";
    tableCell.innerHTML = effdate == null ? "&nbsp;" : effdate + " - " + version;
    tableRow.appendChild(tableCell);
    tabHeader.appendChild(tableRow);

    table.appendChild(tabHeader);
    docHeader.appendChild(table);

    return docHeader;
}

sotManager.prototype.resizeView = function (formInstances, totalDocuments) {
    var currentInstance = this;
    var doccontentwidth = "20%";
    var designcontentwidth = "20%";

    if (totalDocuments < 4) {
        switch (totalDocuments) {
            case 0: doccontentwidth = "100%"; designcontentwidth = "100%";
                break;
            case 1: doccontentwidth = "50%"; designcontentwidth = "50%";
                break;
            case 2: doccontentwidth = "33%"; designcontentwidth = "34%";
                break;
            case 3: doccontentwidth = "25%"; designcontentwidth = "25%";
                break;
        }
    }

    //set width to design div
    $(currentInstance.elementIDs.documentContainerDivJQ).find("div#documentDesignContainer").css("width", designcontentwidth);
    $(currentInstance.elementIDs.documentHeaderContainerJQ).find("div#docdesignheader").css("width", designcontentwidth);

    //set width to div containing the elements
    formInstances.forEach(function (index, val) {
        $(currentInstance.elementIDs.documentContainerDivJQ).find("div#tab" + val).css("width", doccontentwidth);
        $(currentInstance.elementIDs.documentHeaderContainerJQ).find("div#header" + val).css("width", doccontentwidth);
    });

    //reset the label size 
    if (totalDocuments > 1) {
        var containerwidth = parseInt($(currentInstance.elementIDs.documentDesignContainerJQ).css('width'), 10);
        $(currentInstance.elementIDs.documentDesignContainerJQ).find('label').css('width', ((containerwidth - 25) + "px"));
    }
}

sotManager.prototype.closeTab = function (formInstanceId) {
    var currentInstance = this;

    if (currentInstance.objFolder.formInstances[formInstanceId] != undefined) {
        currentInstance.objFolder.formInstances[formInstanceId].FormInstanceBuilder.cleanup();
        delete currentInstance.objFolder.formInstances[formInstanceId];
    }
    var totalDocuments = Object.keys(currentInstance.objFolder.formInstances).length;

    currentInstance.objFolder.openDocuments[formInstanceId].Status = OpenDocumentStatus.Close;

    $(currentInstance.elementIDs.documentHeaderContainerJQ).find("div#header" + formInstanceId).remove();
    $(currentInstance.elementIDs.documentContainerDivJQ).find("div#tab" + formInstanceId).remove();

    if (totalDocuments == 0) {
        //$(currentInstance.elementIDs.documentHeaderContainerJQ).find(".fixedcolumn").html('');
        $(currentInstance.elementIDs.documentContainerDivJQ).find(".fixedcolumn").html('');
        $(currentInstance.elementIDs.documentHeaderContainerJQ).hide();
        $(currentInstance.elementIDs.btnMenuOptions).hide();
        $(currentInstance.elementIDs.btnSaveFormData).hide();
        $('#bottom-menu').hide();
    }
    currentInstance.resizeView(currentInstance.objFolder.formInstances, totalDocuments);
    var url = '/FolderVersion/ReleaseDocumentLock?tenantId=1&formInstanceId={FormInstanceId}';
    var promise = ajaxWrapper.getJSON(url.replace(/{FormInstanceId}/g, formInstanceId));

    promise.done(function (result) {
        if (result.Result === ServiceResult.SUCCESS)
            console.log("Document Lock for " + formInstanceId + " Released..");
    });
}

sotManager.prototype.registerEvent = function () {
    var currentInstance = this;

    $(currentInstance.elementIDs.documentContainerDivJQ).unbind('scroll');
    $(currentInstance.elementIDs.documentContainerDivJQ).on('scroll', function () {
        //Match scrolls - for header and container
        $(currentInstance.elementIDs.documentHeaderContainerJQ).scrollLeft($(this).scrollLeft());
        //Freeze the top most left column
        $(currentInstance.elementIDs.documentHeaderContainerJQ).find('.fixedcolumn').css('left', $(this).scrollLeft());
        $(this).find('.fixedcolumn').css('left', $(this).scrollLeft());
    });

    $(currentInstance.elementIDs.documentDesignContainerJQ).off('click', '.sec-grp');
    $(currentInstance.elementIDs.documentDesignContainerJQ).on('click', '.sec-grp', function (event) {
        event.stopPropagation();
        event.preventDefault();
        return currentInstance.groupingToggle(this);
    });

    currentInstance.enableFilter();
}

sotManager.prototype.enableFilter = function () {
    var currentInstance = this;

    $(currentInstance.elementIDs.filterInputJQ).on('keyup', function (e) {
        var code = e.which;
        if (code == 13) {
            var filter, table, tr, td, i, dt;
            filter = this.value.toUpperCase();
            table = document.getElementById("mainsection");
            tr = table.getElementsByTagName("tr");

            // Loop through all table rows, and hide those who don't match the search query
            for (i = 0; i < tr.length; i++) {
                if (tr[i].className.indexOf('ui-state-highlight') < 0) {
                    dt = tr[i].dataset.filter;
                    if (dt) {
                        td = tr[i].getElementsByTagName("td")[1];
                        if (td) {
                            if (td.childNodes[0].innerHTML.toUpperCase().indexOf(filter) > -1) {
                                //tr[i].style.display = "";
                                $(currentInstance.elementIDs.documentContainerDivJQ).find("tr[data-filter='" + dt + "']").css('display', '');
                            } else {
                                //tr[i].style.display = "none";
                                $(currentInstance.elementIDs.documentContainerDivJQ).find("tr[data-filter='" + dt + "']").css('display', 'none');
                            }
                        }
                    }
                }
            }
        }
    });
}

sotManager.prototype.groupingToggle = function (element) {
    var currentInstance = this;
    var collapse = false;

    if ($(element).hasClass('ui-sotview-expanded')) {
        $(element).removeClass('ui-sotview-expanded').addClass('ui-sotview-collapsed');
        $(element).find('span').removeClass('ui-icon-minus').addClass('ui-icon-plus');
        collapse = true;
    }
    else {
        $(element).removeClass('ui-sotview-collapsed').addClass('ui-sotview-expanded');
        $(element).find('span').removeClass('ui-icon-plus').addClass('ui-icon-minus');
        collapse = false;
    }

    $(currentInstance.elementIDs.documentContainerDivJQ).find("tr[data-group='" + element.id + "']").css('display', collapse ? 'none' : '');

    return false;
}

sotManager.prototype.setActiveTab = function () {
}

sotManager.prototype.setAttributes = function () {
    var currentInstance = this;

    if (currentInstance.objFolder.formInstances.length == 0) {
        currentInstance.objFolder.disableButton();
    }
}

sotManager.prototype.disableMenuLevelButtons = function () {
    var currentInstance = this;

    //$(currentInstance.elementIDs.btnSOTAddJQ).addClass('disabled-button');
    $(currentInstance.elementIDs.btnSaveFormData).addClass('disabled-button');
    $(currentInstance.elementIDs.btnBottomSaveFormData).addClass('disabled-button');
    $(currentInstance.elementIDs.btnBottomCreateNewVersion).addClass('disabled-button');
    $(currentInstance.elementIDs.btnBottomGenerateSOTFile).addClass('disabled-button');
    //$(currentInstance.elementIDs.btnSOTSaveJQ).addClass('disabled-button');
    $(currentInstance.elementIDs.btnValidateJQ).addClass('disabled-button');
    $(currentInstance.elementIDs.btnValidateAllJQ).addClass('disabled-button');
    $(currentInstance.elementIDs.btnStatusUpdateJQ).addClass('disabled-button');
    $(currentInstance.elementIDs.btnBaselineJQ).addClass('disabled-button');
    $(currentInstance.elementIDs.btnNewVersionJQ).attr("disabled", "disabled");
    $(currentInstance.elementIDs.btnRetroJQ).attr("disabled", "disabled");
    $(currentInstance.elementIDs.btnJournalEntryJQ).addClass('disabled-button');
    $(currentInstance.elementIDs.btnDeleteFormInstance).addClass('disabled-button');
    $(currentInstance.elementIDs.assignFolderMemberbutton).addClass('disabled-button');
}

sotManager.prototype.enableMenuLevelButtons = function () {
    var currentInstance = this;

    //$(currentInstance.elementIDs.btnSOTAddJQ).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnSaveFormData).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnBottomSaveFormData).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnBottomCreateNewVersion).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnBottomGenerateSOTFile).removeClass('disabled-button');
    //$(currentInstance.elementIDs.btnSOTSaveJQ).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnValidateJQ).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnValidateAllJQ).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnStatusUpdateJQ).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnBaselineJQ).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnNewVersionJQ).removeAttr("disabled", "disabled");
    $(currentInstance.elementIDs.btnRetroJQ).removeAttr("disabled", "disabled");
    $(currentInstance.elementIDs.btnJournalEntryJQ).removeClass('disabled-button');
    $(currentInstance.elementIDs.btnDeleteFormInstance).removeClass('disabled-button');
    $(currentInstance.elementIDs.assignFolderMemberbutton).removeClass('disabled-button');
}