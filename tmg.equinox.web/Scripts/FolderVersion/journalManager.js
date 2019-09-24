var journalManager = function () {
    var elementIDs = {
        journalGridJQ: "#journalGrid",
        isPreviousVersionJQ: "#isPreviousVersion"
    }

    var URLs = {
        //get All Journal Responses
        getAllJournalResponsesList: '/JournalReport/GetAllJournalResponsesList?journalId={journalId}'
    }

    function buildJournalGrid(currentFormInstanceId, isPreviousVersion) {
        $(elementIDs.journalGridJQ).jqGrid('GridUnload');

        $(elementIDs.journalGridJQ).jqGrid({
            datatype: "local",
            colNames: ['JournalID', 'FormInstanceID', 'FolderVersionID', 'ActionID', 'Action Required', 'Field Path', 'Field Name', 'Description', 'Added Workflow State', 'Closed Workflow State', 'Added By', 'Added Date', 'Updated By', 'Updated Date', 'Version Number'],
            colModel: [{ name: 'JournalID', index: 'JournalID', key: true, hidden: true },
                       { name: 'FormInstanceID', index: 'FormInstanceID', hidden: true },
                       { name: 'FolderVersionID', index: 'FolderVersionID', hidden: true },
                       { name: 'ActionID', index: 'ActionID', hidden: true },
                       { name: 'ActionName', index: 'ActionName', width: '60' },
                       { name: 'FieldPath', index: 'FieldPath', width: '250' },
                       { name: 'FieldName', index: 'FieldName', width: '150' },
                       { name: 'Description', index: 'Description', width: '300' },
                       { name: 'AddedWFStateName', index: 'AddedWFStateName', width: '120' },
                       { name: 'ClosedWFStateName', index: 'ClosedWFStateName', width: '120' },
                       { name: 'AddedBy', index: 'AddedBy', width: '80' },
                       { name: 'AddedDate', index: 'AddedDate', width: '70', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' },
                       { name: 'UpdatedBy', index: 'UpdatedBy', width: '80' },
                       { name: 'UpdatedDate', index: 'UpdatedDate', width: '80', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' },
                       { name: 'FolderVersionNumber', index: 'FolderVersionNumber', width: '100' }],
            pager: '#pjournalGrid',
            autowidth: true,

            height: '150',
            altRows: true,
            altclass: 'alternate',
            rownumbers: true,
            rownumWidth: 40,
            pgbuttons: false,
            pgtext: null,
            hiddengrid: false,
            hidegrid: false,
            rowNum: 500000,
            caption: '<div id="previousVersionDiv" class="ui-jqgrid-titlebar-close HeaderButton"><span style="float:left;">Include Previous Version Entries</span><input type="checkbox" id="isPreviousVersion" style="float:right;"/></div> <span class="ui-jqgrid-title">Journal List</span>',
            subGrid: true,
            subGridRowExpanded: function (subgrid_id, rowid) {
                var subgrid_table_id, pager_id;
                subgrid_table_id = subgrid_id + "_t";
                pager_id = "p_" + subgrid_table_id;
                $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");
                $("#" + subgrid_table_id).jqGrid({
                    datatype: "local",
                    colNames: ['JournalResponseID', 'JournalID', 'Response', 'Added By', 'Added Date'],
                    colModel: [{ name: 'JournalResponseID', index: 'JournalResponseID', key: true, hidden: true },
                               { name: 'JournalID', index: 'JournalID', hidden: true },
                               { name: 'Description', index: 'Description', width: '650' },
                               { name: 'AddedBy', index: 'AddedBy', width: '80' },
                               { name: 'AddedDate', index: 'AddedDate', width: '80', formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' }],
                    rowNum: 500000,
                    height: '100%',
                    autowidth: true,
                    caption: 'Responses List',
                    shrinkToFit: true,
                    altRows: true,
                    altclass: 'alternate',
                    rownumWidth: 40,
                    pgbuttons: false,
                    hiddengrid: false,
                    hidegrid: false
                });

                var row = $(elementIDs.journalGridJQ).jqGrid("getLocalRow", rowid);
                if (row) {
                    loadJournalResponse(row.JournalID, "#" + subgrid_table_id);
                }
            },
            onSelectRow: function (rowId) {
                journalgridRowSelect(rowId, currentFormInstanceId);
            },
            gridComplete: function () {
                if (isPreviousVersion) {
                    $(elementIDs.isPreviousVersionJQ)[0].checked = true;
                }
                if ($(elementIDs.journalGridJQ).getGridParam('records') == 0) {
                    $(elementIDs.journalGridJQ).css({ height: '1px' })
                }
            },
            afterInsertRow: function (rowId, rowData) {
                if (rowData.ActionID === JournalEntryAction.YESOPEN) {
                    $("#" + rowId, elementIDs.journalGridJQ).css({ color: 'red' })
                }
            }
        });
        $(elementIDs.journalGridJQ).jqGrid('navGrid', "#pjournalGrid", { edit: false, add: false, del: false, refresh: true, search: false }, {}, {}, {});
        $(elementIDs.journalGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });

        $(elementIDs.isPreviousVersionJQ).bind("click", function (e) {
            e = e || event;/* get IE event ( not passed ) */
            e.stopPropagation ? e.stopPropagation() : e.cancelBubble = true;
            //var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(currentFormInstanceId);
            var frmInstId = folderManager.getInstance().getFolderInstance().currentFormInstanceID;
            var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(frmInstId);
            if ($(elementIDs.isPreviousVersionJQ).is(":checked")) {
                formInstancebuilder.journal.loadJournalEntry(true);
            }
            else {
                formInstancebuilder.journal.loadJournalEntry(false);
            }
        });
    }

    function journalgridRowSelect(rowId, currentFormInstanceId) {
        var gridRow = $(elementIDs.journalGridJQ).jqGrid("getRowData", rowId);
        if (gridRow != undefined) {
            var formInstanceID = gridRow.FormInstanceID;
            var journalID = gridRow.JournalID;
            var sectionTrail = gridRow.FieldPath;
            var field = gridRow.FieldName;
            var folderVersionID = gridRow.FolderVersionID;
            var description = gridRow.Description;
            var actionid = gridRow.ActionID;

            //code for highlighting
            if (formInstanceID == currentFormInstanceId) {
                //var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(currentFormInstanceId);
                var frmInstId = folderManager.getInstance().getFolderInstance().currentFormInstanceID;
                var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(frmInstId);
                var FormInstance = 'tab' + formInstancebuilder.formInstanceId;
                if (field != JournalEntry.attributeNotApplicableMsg) {
                    var path = "";
                    var generatedName = getGeneratedName(field);
                    var prop = sectionTrail + ' => ' + generatedName;
                    prop = prop.split('=>');
                    for (var i = 0; i <= prop.length - 1; i++) {
                        if (i < prop.length - 1) {
                            path = path + prop[i].replace(/[^a-zA-Z0-9]/g, '').replace(/\s/g, '') + '.';
                        }
                        else {
                            path = path + prop[i].replace(/[^a-zA-Z0-9]/g, '').replace(/\s/g, '');
                        }
                    }
                    var sectionName = path.substring(0, path.indexOf('.'));
                    var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                        return ct.FullName == sectionName;
                    });

                    var elementDetails = getElementDetails(sectionDetails[0], path);
                    var SectionID = sectionDetails[0].Name;
                    var ElementID = elementDetails.Name + formInstancebuilder.formInstanceId;

                    //formInstancebuilder.menu.setSectionMenuSelection(SectionID);                    
                }
                else {
                    var prop = sectionTrail;
                    var path = prop.replace(/\=>/g, '.').replace(/\s/g, '');
                    var sectionName = path;

                    var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
                        return ct.FullName == sectionName;
                    });

                    if (sectionDetails.length > 0) {
                        var SectionID = sectionDetails[0].Name;
                        var ElementID = sectionDetails[0].Name + formInstancebuilder.formInstanceId;

                        //formInstancebuilder.menu.setSectionMenuSelection(SectionID);                        
                    }
                }
                args = [formInstancebuilder, ElementID, gridRow, currentFormInstanceId, elementDetails, SectionID];
                if (formInstancebuilder instanceof sotFormInstanceBuilder == false) {
                    if (formInstancebuilder.selectedSection != SectionID) {
                        var callbackMetaData = {
                            callback: journalManager.highlightJournalElement,
                            callbackArgs: args
                        };
                        formInstancebuilder.form.saveSectionData(formInstancebuilder.selectedSection, SectionID, callbackMetaData);
                    } else {
                        highlightJournalElement(args);
                    }
                }
                else {
                    highlightJournalElement(args);
                }
            }
            else {
                journalEntryDialog.updateJournal(field, sectionTrail, actionid, formInstanceID, folderVersionID, SectionID, description, journalID, currentFormInstanceId);
            }
        }
    }

    function highlightJournalElement(args) {
        var formInstancebuilder = args[0];
        var ElementID = args[1];
        var gridRow = args[2];
        var currentFormInstanceId = args[3];
        var elementDetails = args[4];
        var SectionID = args[5];
        var formInstanceID = gridRow.FormInstanceID;
        var journalID = gridRow.JournalID;
        var sectionTrail = gridRow.FieldPath;
        var field = gridRow.FieldName;
        var folderVersionID = gridRow.FolderVersionID;
        var description = gridRow.Description;
        var actionid = gridRow.ActionID;

        if (gridRow.FieldName != JournalEntry.attributeNotApplicableMsg) {
            if (elementDetails.Name.indexOf("Section") != -1) {
                $(formInstancebuilder.formInstanceDivJQ).find("#section" + ElementID).addClass("selected-control");
            }
            else if (elementDetails.Name.indexOf("Repeater") != -1) {
                $(formInstancebuilder.formInstanceDivJQ).find("#repeater" + ElementID).addClass("selected-control");
            }
            else {
                $(formInstancebuilder.formInstanceDivJQ).find("#" + ElementID).addClass("selected-control");
            }
        }
        else {
            $(formInstancebuilder.formInstanceDivJQ).find("#section" + ElementID).children().addClass("selected-control");
        }

        journalEntryDialog.updateJournal(field, sectionTrail, actionid, formInstanceID, folderVersionID, SectionID, description, journalID, currentFormInstanceId);
    }

    function loadJournalGridData(gridData) {
        $(elementIDs.journalGridJQ).jqGrid('clearGridData');

        if (gridData != undefined) {
            $(elementIDs.journalGridJQ).jqGrid('setGridParam', { data: gridData }).trigger('reloadGrid');
        }
    }

    function loadJournalResponseGridData(data, subGridId) {
        $(subGridId).jqGrid('clearGridData');

        if (data != undefined) {
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                $(subGridId).jqGrid('addRowData', i, data[i]);
            }
        }
    }

    function loadJournalResponse(journalId, subGridId) {

        //ajax call for journal list of forminstance.       
        var url = URLs.getAllJournalResponsesList.replace(/\{journalId\}/g, journalId);
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (responseList) {
            var journalRows = new Array();
            for (var i = 0; i < responseList.length; i++) {
                var responseListObject = responseList[i];
                journalRows.push(responseListObject);
            }
            loadJournalResponseGridData(journalRows, subGridId);
        });
        promise.fail(showError);
    }

    function showError(xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else
            messageDialog.show(Common.errorMsg);
    }

    return {
        journalGridElementID: elementIDs,
        buildJournalGrid: function (currentFormInstanceId, isPrevious) {
            buildJournalGrid(currentFormInstanceId, isPrevious);
        },
        loadJournalGridData: function (gridData) {
            loadJournalGridData(gridData);
        },
        highlightJournalElement: function (args) {
            highlightJournalElement(args);
        }
    }

}();