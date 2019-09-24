var errorManager = function (formInstance) {

    var formInstancebuilder = formInstance;

    var elementIDs = {
        errorGridJQ: "#errorGrid",
        elementIDColumnName: "ElementID",
        rowNumColumnName: "RowNum",
        descriptionColumnName: "Description",
        errormenuId: "#errorMenu",
        errorGridTitleBarJQ: "#gbox_errorGrid"
    }

    function buildErrorGrid() {
        $(elementIDs.errorGridJQ).jqGrid('GridUnload');

        $(elementIDs.errorGridJQ).jqGrid({
            datatype: "local",
            olNames: ['ID', 'SectionID', 'Document', 'ErrorRows', 'Section'],
            colModel: [{ name: 'ID', index: 'ID', key: true, hidden: true },
                       { name: 'SectionID', index: 'SectionID', hidden: true },
                       { name: 'Form', index: 'Form', width: '400', hidden: true },
                       { name: 'FormInstanceID', index: 'FormInstanceID', width: '400', hidden: true },
                       { name: 'ErrorRows', index: 'ErrorRows', width: '400', hidden: true },
                       { name: 'Section', index: 'Section', width: '100' }],
            pager: '#perrorGrid',
            autowidth: true,
            shrinkToFit: true,
            height: '200',
            altRows: true,
            rownumbers: true,
            rownumWidth: 40,
            pgbuttons: false,
            pgtext: null,
            hiddengrid: false,
            hidegrid: false,
            rowNum: 500000,
            caption: 'Error List' + ' - <b>' + formInstancebuilder.formName + '</b>' + '  (<b>Note :</b> Error List contains first 200 validations)</b>',
            subGrid: true,
            subGridRowExpanded: function (subgrid_id, rowid) {
                var subgrid_table_id, pager_id;
                subgrid_table_id = subgrid_id + "_t";
                pager_id = "p_" + subgrid_table_id;
                $("#" + subgrid_id).html("<table id='" + subgrid_table_id + "' class='scroll'></table><div id='" + pager_id + "' class='scroll'></div>");

                var row = $(elementIDs.errorGridJQ).jqGrid("getLocalRow", rowid);
                var subGridData = row.ErrorRows;

                $("#" + subgrid_table_id).jqGrid({
                    datatype: "local",
                    data: subGridData,
                    loadonce: true,
                    colNames: ['ID', 'RepeaterGridID', 'ElementID', 'SectionID', 'FormInstance', 'FormInstanceID', 'Document', 'Section', 'GeneratedName', 'ColumnNumber', 'Element Path', 'Field', 'RowNum', 'Row', '', 'Description', 'RowIdProperty'],
                    colModel: [{ name: 'ID', index: 'ID', key: true, hidden: true },
                               { name: 'RepeaterGridID', index: 'RepeaterGridID', hidden: true },
                               { name: 'ElementID', index: 'ElementID', hidden: true },
                               { name: 'SectionID', index: 'SectionID', hidden: true },
                               { name: 'FormInstance', index: 'FormInstance', hidden: true },
                               { name: 'FormInstanceID', index: 'FormInstanceID', hidden: true },
                               { name: 'Form', index: 'Form', width: '100', hidden: true },
                               { name: 'Section', index: 'Section', width: '100', hidden: true },
                               { name: 'GeneratedName', index: 'GeneratedName', width: '100', hidden: true },
                               { name: 'ColumnNumber', index: 'ColumnNumber', width: '100', hidden: true },
                               { name: 'SubSectionName', index: 'SubSectionName', width: '200' },
                               { name: 'Field', index: 'Field', width: '100' },
                               { name: 'RowNum', index: 'RowNum', width: '150', align: 'left', hidden: true },
                               { name: 'KeyValue', index: 'KeyValue', width: '80', align: 'left' },
                               { name: 'Value', index: 'Value', width: '80', align: 'center', hidden: true },
                               { name: 'Description', index: 'Description', width: '300' },
                               { name: 'RowIdProperty', index: 'RowIdProperty', hidden: true }],
                    rowNum: 500000,
                    height: '100%',
                    autowidth: true,
                    //caption: 'Errors in Section',
                    shrinkToFit: true,
                    altRows: true,
                    rownumbers: true,
                    rownumWidth: 40,
                    pgbuttons: false,
                    hiddengrid: false,
                    hidegrid: false,
                    onSelectRow: function (rowId) {
                        subgridRowSelect(rowId, "#" + subgrid_table_id);
                    }
                });

                var row = $(elementIDs.errorGridJQ).jqGrid("getLocalRow", rowid);
                //if (row) {
                //    var subGridData = row.ErrorRows;
                //    for (var i = 0; i <= subGridData.length; i++)
                //        $("#" + subgrid_table_id).jqGrid('addRowData', i + 1, subGridData[i]);
                //}
            },
            subGridOptions: { reloadOnExpand: false },
            onSelectRow: function (rowId) {
                //errorgridRowSelect(rowId);
            },
            gridComplete: function () {
                setErrorGridColor();
            }
        });
        $(elementIDs.errorGridJQ).jqGrid('navGrid', "#perrorGrid", { edit: false, add: false, del: false, refresh: false, search: false }, {}, {}, {});
    }

    function subgridRowSelect(rowId, subGridJQ) {
        var gridRow = $(subGridJQ).jqGrid("getRowData", rowId);
        if (gridRow != undefined) {
            //var formInstancebuilder = folderManager.getInstance().getFolderInstance().getFormInstanceBuilder(gridRow.FormInstanceID);
            args = [gridRow, formInstancebuilder];
            if (formInstancebuilder.selectedSection != gridRow.SectionID && formInstancebuilder instanceof sotFormInstanceBuilder == false) {
                var callbackMetaData = {
                    callback: formInstancebuilder.errorManager.highlightErrorElement,
                    callbackArgs: args
                };
                //formInstancebuilder.heighlightValidationError = args;
                formInstancebuilder.form.saveSectionData(formInstancebuilder.selectedSection, gridRow.SectionID, callbackMetaData);
            }
            else if (formInstancebuilder instanceof sotFormInstanceBuilder == true) {
                highlightErrorElement(args);
            }
            else {
                if (formInstancebuilder.IsMasterList == true) {
                    highlightErrorElementAG(args);
                }
                    //else if (checkIfPQGridLoaded())
                    //    highlightErrorElementPQ(args);
                else
                    highlightErrorElementAG(args);
            }
        }
    }

    function highlightErrorElementPQ(args) {
        var gridRow = args[0];
        var formInstancebuilder = args[1];

        var isDropDownFilter = false;
        var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
            return ct.Label == gridRow.Section;
        });
        if (sectionDetails !== undefined && sectionDetails !== null && sectionDetails.length > 0) {
            var elementDetails = getElementDetails(sectionDetails[0], sectionDetails[0].FullName + '.' + gridRow.GeneratedName);
            if (elementDetails !== undefined && elementDetails !== null) {
                if ((elementDetails.Type == 'select' || elementDetails.Type == 'SelectInput') && elementDetails.IsDropDownFilterable) {
                    isDropDownFilter = true;
                }
            }
        }
        if (gridRow.RowNum === "" || gridRow.RowNum === null || gridRow.RowNum === undefined) {
            if (isDropDownFilter) {
                $("#" + gridRow.ElementID + gridRow.FormInstanceID + 'DDL').focus();
            } else {
                $("#" + gridRow.ElementID).focus();
                //$("#" + gridRow.FormInstance).scrollTop($(" #" + gridRow.ElementID).position().top);
                if ($(" #" + gridRow.ElementID).length > 0) {
                    $("#" + gridRow.FormInstance).scrollTop($(" #" + gridRow.ElementID).position().top);
                    //$(".container-fluid " + formInstancebuilder.formInstanceDivJQ).find("#" + gridRow.ElementID).parent().addClass("has-error");
                    //$(window).scrollTop($(".container-fluid " + formInstancebuilder.formInstanceDivJQ).find("#" + gridRow.ElementID).offset().top - 50);
                    //$(".container-fluid " + formInstancebuilder.formInstanceDivJQ).find("#" + gridRow.ElementID).focus();
                }
            }
        } else {
            ajaxDialog.showPleaseWait();
            var fieldName = (gridRow.RowNum - 1) + "_" + gridRow.Field.replace(/\s/g, '');
            var repeaterID = gridRow.ElementID.substring(0, gridRow.ElementID.indexOf("_")) + gridRow.FormInstanceID;

            $("#" + repeaterID).pqGrid("refreshDataAndView");

            var totalPages = $("#" + repeaterID).pqGrid("option", "pageModel.totalPages");
            var currentPage = $("#" + repeaterID).pqGrid("option", "pageModel.curPage");
            var data = $("#" + repeaterID).pqGrid("pageData");




            var isRowIDExist = data.filter(function (ct) {
                if (ct.RowIDProperty == gridRow.RowIdProperty) {
                    return ct;
                }
            });

            if (isRowIDExist.length == 0) {
                for (i = 1; i <= totalPages; i++) {
                    if (currentPage != i) {
                        $("#" + repeaterID).pqGrid("goToPage", { page: i });
                        var data = $("#" + repeaterID).pqGrid("pageData");
                        var isRowIDExist = data.filter(function (ct) {
                            return ct.RowIDProperty == gridRow.RowIdProperty;
                        });
                        if (isRowIDExist.length != 0) {
                            break;
                        }
                    }
                }
            }
            //var selectedRowID = $("#" + repeaterID).pqGrid("selection", { type: 'row', method: 'getSelection' });
            //if (selectedRowID !== null) {
            //    $("#" + repeaterID).pqGrid("commit");
            //}

            var ind = $("#" + repeaterID).pqGrid("getRow", { rowIndx: gridRow.RowIdProperty });
            if (ind) {
                var colindex = undefined;
                var col = $("#" + repeaterID).pqGrid("option", "colModel");
                $.each(col, function (idx, ct) {
                    if (ct.dataIndx == gridRow.GeneratedName) {
                        colindex = idx;
                        return false;
                    }
                });
                var tcell = $("td:eq(" + (colindex + 1) + ")", ind);
                var td = getRepeaterColumnPQ(repeaterID, gridRow.RowIdProperty, gridRow.GeneratedName, colindex);
                $(tcell).addClass("repeater-has-error");
                $("input, select, textarea", td).focus();
                $("#" + repeaterID).pqGrid("scrollRow", { rowIndx: gridRow.RowIdProperty });
            }
            ajaxDialog.hidePleaseWait();
        }
        formInstancebuilder.bottomMenu.closeBottomMenu();
    }

    function highlightErrorElementAG(args) {
        var gridRow = args[0];
        var formInstancebuilder = args[1];

        var isDropDownFilter = false;
        var sectionDetails = formInstancebuilder.designData.Sections.filter(function (ct) {
            return ct.Label == gridRow.Section;
        });
        if (sectionDetails !== undefined && sectionDetails !== null && sectionDetails.length > 0) {
            var elementDetails = getElementDetails(sectionDetails[0], sectionDetails[0].FullName + '.' + gridRow.GeneratedName);
            if (elementDetails !== undefined && elementDetails !== null) {
                if ((elementDetails.Type == 'select' || elementDetails.Type == 'SelectInput') && elementDetails.IsDropDownFilterable) {
                    isDropDownFilter = true;
                }
            }
        }
        if (gridRow.RowNum === "" || gridRow.RowNum === null || gridRow.RowNum === undefined) {
            if (isDropDownFilter) {
                $("#" + gridRow.FormInstance + "#" + gridRow.SectionID + " #" + gridRow.ElementID + gridRow.FormInstanceID + 'DDL').focus();
            } else {
                $("#" + gridRow.ElementID).focus();
                $("#" + gridRow.FormInstance).scrollTop($(" #" + gridRow.ElementID).position().top);
            }
        } else {
            ajaxDialog.showPleaseWait();
            var fieldName = (gridRow.RowNum - 1) + "_" + gridRow.Field.replace(/\s/g, '');
            var repeaterID = gridRow.ElementID.substring(0, gridRow.ElementID.indexOf("_")) + gridRow.FormInstanceID;

            var rowNodeFound = {};
            var isRowIDExist;
            var staggerdButtonName = '#btn' + repeaterID;
            var staggerdButton = $(staggerdButtonName);

            if (staggerdButton.length > 0) {
                if (staggerdButton.is(":visible") == true) {
                    staggerdButton.click();
                }
            }
            var currentGridInstance = GridApi.getCurrentGridInstanceById(repeaterID);

            currentGridInstance.gridOptions.api.forEachNode(function (rowNode, index) {
                if (rowNode.data.RowIDProperty == gridRow.RowIdProperty) {
                    isRowIDExist = index;
                    rowNodeFound = rowNode;
                    return;
                }
            });

            if (rowNodeFound) {
                var colindex = undefined;
                var col = currentGridInstance.colModel;
                $.each(col, function (idx, ct) {
                    if (ct.dataIndx == gridRow.GeneratedName) {
                        colindex = idx;
                        return false;
                    }
                });

                GridApi.setCellFocus(currentGridInstance, rowNodeFound, gridRow.GeneratedName, colindex);
            }
            ajaxDialog.hidePleaseWait();
        }
        formInstancebuilder.bottomMenu.closeBottomMenu();
    }

    function highlightErrorElement(args) {
        var gridRow = args[0];
        var formInstancebuilder = args[1];

        if (gridRow.RowNum === "" || gridRow.RowNum === null || gridRow.RowNum === undefined) {
            $("#" + gridRow.ElementID).focus();
        } else {
            ajaxDialog.showPleaseWait();
            var fieldName = (gridRow.RowNum - 1) + "_" + gridRow.Field.replace(/\s/g, '');
            var repeaterID = gridRow.ElementID.substring(0, gridRow.ElementID.indexOf("_")) + gridRow.FormInstanceID;

            var rowNum = $("#" + repeaterID).getGridParam('rowNum');
            var allRecords = $("#" + repeaterID).getGridParam('records');
            var totalPages = parseInt((allRecords / rowNum) + 1);

            var currentPage = $("#" + repeaterID).getGridParam('page');
            var ID = $("#" + repeaterID).jqGrid("getDataIDs");

            var isRowIDExist = ID.filter(function (ct) {
                if (ct == gridRow.RowIdProperty) {
                    return ct;
                }
            });

            if (isRowIDExist.length == 0) {
                for (i = 1; i <= totalPages; i++) {
                    if (currentPage != i) {
                        $("#" + repeaterID).trigger("reloadGrid", [{ page: i }]);
                        var ID = $("#" + repeaterID).jqGrid("getDataIDs");
                        var isRowIDExist = ID.filter(function (ct) {
                            return ct == gridRow.RowIdProperty;
                        });
                        if (isRowIDExist.length != 0) {
                            break;
                        }
                    }
                }
            }

            var selectedRowID = $("#" + repeaterID).jqGrid('getGridParam', 'selrow');

            if (selectedRowID !== null) {
                $("#" + repeaterID).jqGrid('saveRow', selectedRowID);
            }
            $("#" + repeaterID).jqGrid('setSelection', gridRow.RowIdProperty);


            var ind = $("#" + repeaterID).jqGrid('getGridRowById', gridRow.RowIdProperty);
            if (ind) {
                var colindex = undefined;
                var col = $("#" + repeaterID).jqGrid("getGridParam", "colModel");
                $.each(col, function (idx, ct) {
                    if (ct.name == gridRow.GeneratedName) {
                        colindex = idx;
                        return false;
                    }
                });

                var tcell = $("td:eq(" + colindex + ")", ind);
                var td = getRepeaterColumn(repeaterID, (ind.rowIndex), gridRow.GeneratedName, colindex);
                $(tcell).addClass("repeater-has-error");
                $("input, select, textarea", td).focus();
            }
            ajaxDialog.hidePleaseWait();
        }
        formInstancebuilder.bottomMenu.closeBottomMenu();
    }

    function errorgridRowSelect(rowId) {
        var gridRow = $(elementIDs.errorGridJQ).jqGrid("getLocalRow", rowId);
        if (gridRow != undefined) {
            if (formInstancebuilder.selectedSection != gridRow.SectionID) {
                formInstancebuilder.form.saveSectionData(formInstancebuilder.selectedSection, gridRow.SectionID);
            }
            formInstancebuilder.bottomMenu.openBottomMenu();
        }
        $(elementIDs.errorGridJQ).toggleSubGridRow(rowId);
    }

    function addErrorRow(row) {
        var elementIDList = $(elementIDs.errorGridJQ).jqGrid('getCol', elementIDs.elementIDColumnName);
        var allRowNumList = $(elementIDs.errorGridJQ).jqGrid('getCol', elementIDs.rowNumColumnName);
        var descriptionList = $(elementIDs.errorGridJQ).jqGrid('getCol', elementIDs.descriptionColumnName);
        var rowNumList = new Array();
        for (var k = 0; k < allRowNumList.length; k++) {
            if (allRowNumList[k] != null || allRowNumList[k] != "") {
                rowNumList.push(parseInt(allRowNumList[k]));
            }
        }
        var elementId, rowNumId, descriptionId;
        var isExist = false;
        for (elementId = 0, rowNumId = 0, descriptionId = 0; elementId < elementIDList.length && rowNumId < rowNumList.length && descriptionId < descriptionList.length; elementId++, rowNumId++, descriptionId++) {
            if (row.ElementID === elementIDList[elementId] && row.RowNum === rowNumList[rowNumId] && row.Description === descriptionList[descriptionId]) {
                isExist = true;
                break;
            }
        }
        if (isExist == false) {
            $(elementIDs.errorGridJQ).jqGrid('addRowData', row.ID, row);
        }
        setErrorGridColor();
    }

    function removeErrorRow(row) {
        $(elementIDs.errorGridJQ).jqGrid('delRowData', row.ID);
        setErrorGridColor();
    }

    function setErrorGridColor() {
        var errorCount = $("#errorGrid").getGridParam("reccount");
        if (errorCount > 0) {
            $(elementIDs.errorGridTitleBarJQ).find("div.ui-jqgrid-titlebar").attr("style", "");
            $(elementIDs.errormenuId).removeClass("side-menu-container-green");
            $(elementIDs.errormenuId).addClass("side-menu-container-red");
        }
        else {
            $(elementIDs.errorGridTitleBarJQ).find("div.ui-jqgrid-titlebar").attr("style", "");
            $(elementIDs.errormenuId).addClass("side-menu-container-green");
            $(elementIDs.errormenuId).removeClass("side-menu-container-red");
        }
    }

    function loadErrorGridData(data) {
        $(elementIDs.errorGridJQ).jqGrid('clearGridData');

        if (data != undefined) {
            for (var i = 0; i < data.length; i++) {
                var item = data[i];
                $(elementIDs.errorGridJQ).jqGrid('addRowData', i, data[i]);
            }
        }
        setErrorGridColor();
    }

    return {
        errorGridElementID: elementIDs,
        buildErrorGrid: function () {
            buildErrorGrid();
        },
        addErrorRow: function (row) {
            addErrorRow(row);
        },
        removeErrorRow: function (row, index) {
            removeErrorRow(row, index);
        },
        loadErrorGridData: function (data) {
            loadErrorGridData(data);
        },
        highlightErrorElement: function (args) {
            if (checkIfPQGridLoaded(formInstancebuilder.IsMasterList))
                highlightErrorElementAG(args);
            else if (formInstancebuilder.IsMasterList == true) {
                highlightErrorElementAG(args);
            }
            else {
                highlightErrorElement(args);
            }
        },
        evSubgridRowSelect: function (rowID, subGridJQ) {
            subgridRowSelect(rowID, subGridJQ);
        },
    }
}
