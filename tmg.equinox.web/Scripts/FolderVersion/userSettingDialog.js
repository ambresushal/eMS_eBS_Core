var userSettingDialog = function (repeaterBuilder, isRowReadOnly) {
    this.caption = "My Settings - " + repeaterBuilder.design.Label;
   
    repeaterBuilder.rowViewMode = false;
    this.repeaterBuilder = repeaterBuilder;
    this.formDesignVersionId = repeaterBuilder.formInstanceBuilder.formDesignVersionId;
    this.isRowReadOnly = isRowReadOnly;
    this.design = { "AllowBulkUpdate": false };
    this.label = "";
    this.saveCounter = 0;
    this.URLs = {
        //tenantId,formInstanceId, formDesignVersionId, folderVersionId, formDesignId, fullName, sData
        getRepeateUserSettings: "/Settings/GetFormDesignUserSetting?formDesignVersionId=" + this.formDesignVersionId + "&key={key}&key1={key1}",
        saveRepeateUserSettings: "/Settings/SaveFormDesignUserSetting"
    };


    this.elementIDs = {
        repeaterDialogJQ: '#gridDialog',
        repeaterRowDataJQ: '#gridCellSection1',
        repeaterRowDataJQ2: '#gridCellSection2',
        repeaterRowDataJQ3: '#gridCellSection3',
        repeaterDialogSaveButton: "#gridCellDialogSaveBtn",
        repeaterDialogCancelButton: "#gridCellDialogCancelBtn"
    };
    this.userSettingService = {};
}

userSettingDialog.prototype.showError = function (xhr) {
    /*if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
        */
    messageDialog.show(Common.errorMsg);
    console.log(JSON.stringify(xhr));
}



userSettingDialog.prototype.addPaginationPanel = function (row) {
    var currentInstance = this;

    var gridOptions = GridApi.getCurrentGridInstance(currentInstance).gridOptions;
}
userSettingDialog.prototype.generateRowLayout = function () {

    var currentInstance = this;
    

    var url = currentInstance.URLs.getRepeateUserSettings.replace(/\{key\}/g, currentInstance.repeaterBuilder.design.FullName);
    var url = url.replace(/\{key1\}/g, currentInstance.repeaterBuilder.formInstanceBuilder.selectedSection);
    var promise = ajaxWrapper.getJSON(url);
    //register ajax success callback
    promise.done(function (result) {
        currentInstance.userSettingService = new userSettings(currentInstance.repeaterBuilder, result);
        var rowData = currentInstance.userSettingService.getColumSettingData(currentInstance.repeaterBuilder);
        var columnDefs = GridApi.columnDef().prepare(currentInstance.userSettingService.ColDef, "local filtering");
        currentInstance.label = "Column Level Settings";
        currentInstance.generateGrid(currentInstance.elementIDs.repeaterRowDataJQ, rowData.Data, columnDefs);

        var columnDefs = GridApi.columnDef().prepare(currentInstance.userSettingService.GridColDef, "local filtering");
        var rowData = currentInstance.userSettingService.getGridSettingData(currentInstance.repeaterBuilder);
        currentInstance.label = "Grid Level Settings";
        currentInstance.generateGrid(currentInstance.elementIDs.repeaterRowDataJQ2, rowData.Data, columnDefs);

        var columnDefs = GridApi.columnDef().prepare(currentInstance.userSettingService.SectionColDef, "local filtering");
        var rowData = currentInstance.userSettingService.getSectionSettingData(currentInstance.repeaterBuilder);
        currentInstance.label = "Section Level Settings";
        currentInstance.generateGrid(currentInstance.elementIDs.repeaterRowDataJQ3, rowData.Data, columnDefs);
        //repeaterBuilder.formInstanceBuilder.selectedSection
    });
    promise.fail(currentInstance.showError);
}


userSettingDialog.prototype.generateGrid = function (repeaterId, rowData, columnDefs) {
    var currentInstance = this;
    var gridGlobalSettingOption = new GridApi.gridGlobalSetting();

    var gridOptions = gridGlobalSettingOption.defaultGridOption();
    gridOptions.pivotPanelShow= false;
    gridOptions.pivotTotals= false;
    gridOptions.showToolPanel = false;
    gridOptions.defaultColDef.enablePivot = false;
    gridOptions.defaultColDef.enableValue = false;
    gridOptions.defaultColDef.enableRowGroup = false;
    gridOptions.defaultColDef.headerComponentParams.enableMenu = false;
    gridOptions.toolPanelSuppressSideButtons = true;
    gridOptions.columnDefs = columnDefs;
    gridOptions.rowData = rowData;
    gridOptions.currentInstance = currentInstance;
    gridOptions.onCellValueChanged = currentInstance.onCellValueChanged;
    gridOptions.onGridReady = function (e) {
        //  autoSizeAll(e);
        //e.api.sizeColumnsToFit();
        var currentInstance = this;
        var allColumnIds = [];
        gridOptions.columnApi.getAllColumns().forEach(function (column) {
            allColumnIds.push(column.colId);
        });
        gridOptions.columnApi.autoSizeColumns(allColumnIds);
        gridOptions.api.resetRowHeights();


    };

    agGrid.LicenseManager.setLicenseKey(license.agGrid);

    $(repeaterId).html('');

    var gridDivList = $(repeaterId);//document.querySelector('#myGrid');
    var gridDiv = gridDivList[0];
    gridDiv.gridOptions = gridOptions;
    new agGrid.Grid(gridDiv, gridOptions);


    GridApi.renderGridHeaderRow(repeaterId, currentInstance.label, gridOptions);
}

userSettingDialog.prototype.init = function () {
    var currentInstance = this;
    $(currentInstance.elementIDs.repeaterDialogJQ).dialog({
        autoOpen: false,
        resizable: false,
        closeOnEscape: false,
        height: 'auto',
        width: 850,
        modal: true,
        position: ['middle', 100]
    });
    $(currentInstance.elementIDs.repeaterDialogJQ).dialog({
        close: function (event, ui) {
            $(currentInstance.elementIDs.repeaterPreviousBtnContainer).css('margin-top', '0px');
            $(currentInstance.elementIDs.repeaterNextBtnContainer).css('margin-top', '0px');
        }
    });

   
}

userSettingDialog.prototype.show = function (hasSavebutton) {
    var currentInstance = this;
    currentInstance.init();
    currentInstance.generateRowLayout();

    $(currentInstance.elementIDs.repeaterDialogJQ).dialog('option', 'title', currentInstance.caption);
    $(currentInstance.elementIDs.repeaterDialogJQ).dialog("open");
 
    $(currentInstance.elementIDs.repeaterDialogCancelButton).unbind("click");
    $(currentInstance.elementIDs.repeaterDialogCancelButton).bind("click", function () {
        $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");
    });


    // In view mode disable all save button 

        $(currentInstance.elementIDs.repeaterDialogSaveButton).css("visibility", "visible");
        $(currentInstance.elementIDs.repeaterDialogSaveButton).unbind("click");

            $(currentInstance.elementIDs.repeaterDialogSaveButton).bind("click", function () {



                yesNoConfirmDialog.show(Common.userGridSetting, function (e) {
                    yesNoConfirmDialog.hide();
                    if (e) {
                        if (e==true) {
                            var colSettingData = currentInstance.userSettingService.getColumSettingData();
                            colSettingData.Data = JSON.stringify(colSettingData.Data);
                            currentInstance.save(colSettingData);

                            var gridSettingData = currentInstance.userSettingService.getGridSettingData();
                            gridSettingData.Data = JSON.stringify(gridSettingData.Data);
                            currentInstance.save(gridSettingData);

                            var sectionSettingData = currentInstance.userSettingService.getSectionSettingData();
                            sectionSettingData.Data = JSON.stringify(sectionSettingData.Data);
                            currentInstance.save(sectionSettingData);
                        }
                    }
                    else
                        $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");
                });

                });
    }

userSettingDialog.prototype.save = function(settingData)
{

    var currentInstance = this;
    var url = currentInstance.URLs.saveRepeateUserSettings;
    var promise = ajaxWrapper.postJSON(url, settingData);
    //register ajax success callback
    promise.done(function (xhr) {
        try {
            //show appropriate message
            if (xhr.Result == ServiceResult.SUCCESS) {
                currentInstance.saveCounter++;

                if (currentInstance.saveCounter == 2) {
              //      messageDialog.show("Your Settings saved successfully");
                    $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");
                   currentInstance.repeaterBuilder.reload();
                }
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        }
        catch (ex) {
            HasError = true;
        }
    });
    //register ajax failure callback
    promise.fail(this.showError);
}

userSettingDialog.prototype.onCellValueChanged = function (event) {
    var currentInstance = this.currentInstance;


    if (event.oldValue == event.newValue) {
        return;
    }
    if (event.newValue == '[Select One]') {
        event.newValue = event.oldValue;
        event.data[event.column.colId] = event.oldValue;
        return;
    }

   // currentInstance.userSettingService.updateRow(event);
    
    event.api.resetRowHeights();
};

userSettingDialog.prototype.render = function (repeaterId)
{
    var currentInstance = this;
    var table = document.createElement("TABLE");
//    table.className = "ag-ltr";//"customPager";
    var row = document.createElement("TR");
 //    row.className = "customPager";

    currentInstance.addGridLabel(table, row, td, cell);

    $(repeaterId).find('.ag-paging-panel').prepend(table);
}
userSettingDialog.prototype.addGridLabel = function (table, row) {
    var currentInstance = this;
    td = document.createElement("TD");

    cell = document.createElement("SPAN");
    cell.className = "pq-grid-title ui-corner-top";
    cell.innerText = currentInstance.label;
    td.appendChild(cell);
    row.appendChild(td);
    table.appendChild(row);


}
var userSettings = function (repeaterBuilder, userSettingData) {
    this.repeaterBuilder = repeaterBuilder;
   // this.userSettingData = userSettingData;
    var currentInstance = this;

   /*sample this.userSettingData = [
	{
	    "FormUserSettingID": 1,
	    "USERID": 1,
	    "FormDesignVersionId": 1,
	    "LEVELAT": "COLUMN",
	    "KEY": "REP_ID",
	    "DATA": [{
	        "Columns": "Col1",
	        "FieldValue":
                {
                    "filter": "Filter by condition",
                    "sorting": "Yes",
                    "filterbutton" : "Both"
                }
	    },
		{
		    "Columns": "Col2",
		    "FieldValue":
                {
                    "filter": "Filter by value",
                    "sorting": "no",
                    "filterbutton": "Apply"
                }
		},
		{
		    "Columns": "Col2",
		    "FieldValue":
                {
                    "filter": "Filter by text",
                    "sorting": "no",
                    "filterbutton": "Apply"
                }
		}]
	},
	{
	    "FormUserSettingID": 1,
	    "USERID": 1,
	    "FormDesignVersionId": 1,
	    "LEVELAT": "GRID",
	    "KEY": "REP_ID",
	    "DATA": {
	        "pageSize": 50
	    }
	}
    ];*/
    this.userSettingData = [
        {
            "FormUserSettingID": 1,
            "USERID": 1,
            "FormDesignVersionId": 1,
            "LEVELAT": "COLUMN",
            "KEY": "REP_ID",
            "DATA":null
           /* "DATA": [{
                "colname": "Col1",
                "filter": "Filter by condition",
                "filterbutton": "Both"
            },
            {
                "colname": "Col2",               
                "filter": "Filter by value",
                "filterbutton": "Apply"
            },
            {
                "colname": "Col2",
                "filter": "Filter by text",
                "filterbutton": "Apply"                           
            }]*/
        },
        {
            "FormUserSettingID": 1,
            "USERID": 1,
            "FormDesignVersionId": 1,
            "LEVELAT": "GRID",
            "KEY": "REP_ID",
            "DATA": {
                "pageSize": 50
            }
        }
    ];

    this.columnSettingData = {};
    this.gridSettingData = {};
    this.sectionSettingData = {};
    //  this.userSettingData = JSON.parse(this.userSettingData);

    this.commonConst = {
        filter: "filter",
        casesensitive: "casesensitive",
        menu: "menu",
        pagesize:"pagesize",
        pivot: "pivot",
        height: "height",
        GRID: "GRID",
        COLUMN: "COLUMN",
        FilterByCondition: "Filter by Condition",        
        FilterByText: "Filter by Text" ,
        FilterByWildcard: "Filter by Contain",
        repeaterRender: "RenderRepeaterOption",
        Staggered: "Staggered",
        SECTION: "SECTION",
        RenderAll: "Render All"
    };

    this.ColDef = [{
        "dataIndx": "colname",
        "dataType": "text",
        "items": "",
        "title": "Field Name",
        "toolTip": "Field Name",
        "width": "250",
        "hidden": true,
        "editable": false,
        "editor": null,
        "filterMode": "both",
        "default":""
    },
    {
        "dataIndx": "displayname",
        "dataType": "text",
        "items": "",
        "title": "Field Name",
        "toolTip": "Field Name",
        "width": "250",
        "hidden": false,
        "editable": false,
        "editor": null,
        "filterMode": "both",
        "default": ""
    },
    {
        "dataIndx": this.commonConst.filter,
        "dataType": "select",
        "items": "",
        "title": "Filter",
        "toolTip": "Filter",
        "width": "100",
        "hidden": false,
        "editable": true,
        "editor": {
            "type": "select",
            "options": this.getDropdownTextboxOptions(this.getfilterOptions())
        },
        "filterMode": "both",
        "edittype": "select",
        "default": this.getDefaultFiltervalue()
    },
    {
        "dataIndx": this.commonConst.casesensitive,
        "dataType": "select",
        "items": "",
        "title": "Enable CaseSensitive",
        "toolTip": "Enable CaseSensitive on Filter",
        "width": "100",
        "hidden": false,
        "editable": true,
        "editor": {
            "type": "select",
            "options": this.getDropdownTextboxOptions(this.getYesNoOptions())
        },
        "filterMode": "both",
        "edittype": "select",
        "default": "No"
    },
    {
        "dataIndx": this.commonConst.menu,
        "dataType": "select",
        "items": "",
        "title": "Enable Menu",
        "toolTip": "Enable Menu For Pin Column",
        "width": "100",
        "hidden": false,
        "editable": true,
        "editor": {
            "type": "select",
            "options": this.getDropdownTextboxOptions(this.getYesNoOptions())
        },
        "filterMode": "both",
        "edittype": "select",
        "default": "Yes"
    }
    ]
    this.GridColDef = [{
        "dataIndx":  this.commonConst.pagesize,
       "dataType": "select",
       "items": "",
       "title": "Page Size",
       "toolTip": "Page Size",
       "width": "100",
       "hidden": false,
       "editable": true,
       "editor": {
           "type": "select",
           "options": this.getDropdownTextboxOptions(this.getPageSizeOptions())
       },
       "filterMode": "both",
       "edittype": "select",
       "default": "20"
    },
    {
        "dataIndx": this.commonConst.pivot,
        "dataType": "select",
        "items": "",
        "title": "Enable Pivot",
        "toolTip": "Enable Pivot",
        "width": "100",
        "hidden": false,
        "editable": true,
        "editor": {
            "type": "select",
            "options": this.getDropdownTextboxOptions(this.getYesNoOptions())
        },
        "filterMode": "both",
        "edittype": "select",
        "default": "Yes"
    },
    {
        "dataIndx": this.commonConst.height,
        "dataType": "select",
        "items": "",
        "title": "Height",
        "toolTip": "Set Grid Height",
        "width": "100",
        "hidden": false,
        "editable": true,
        "editor": {
            "type": "select",
            "options": this.getDropdownTextboxOptions(this.getHeightSizeOptions())
        },
        "filterMode": "both",
        "edittype": "select",
        "default": "300px"
    }
    ]
   
    this.SectionColDef = [
    {
        "dataIndx": this.commonConst.repeaterRender,
        "dataType": "select",
        "items": "",
        "title": "Render Repeater Option",
        "toolTip": "Render Repeater Option",
        "width": "100",
        "hidden": false,
        "editable": true,
        "editor": {
            "type": "select",
            "options": this.getDropdownTextboxOptions(this.getRepeaterRenderOptions())
        },
        "filterMode": "both",
        "edittype": "select",
        "default": "Staggered"
    }
    ]

    $.each(userSettingData, function (index, item) {

        var isArray = item.Data instanceof Array;

        if (isArray===false) {
            item.Data = JSON.parse(item.Data);
        }
        if (item.LevelAt === currentInstance.commonConst.COLUMN) {
                currentInstance.columnSettingData = item;
        }
        if (item.LevelAt === currentInstance.commonConst.GRID) {
            currentInstance.gridSettingData = item;
        } 
        else if (item.LevelAt === currentInstance.commonConst.SECTION) {
            currentInstance.sectionSettingData = item;
        }
    });
}
userSettings.prototype.getUserColumnChoiceRow = function (fieldName) {
    var currentInstance = this;
    var row = {};
    if (currentInstance.columnSettingData.Data !==undefined) {
        $.each(currentInstance.columnSettingData.Data, function (index, item) {
            if (item.colname === fieldName) {
                row = item;
                return;
            }
        });
    }
    return row;
}
userSettings.prototype.getUserChoiceData = function (settingName) {
    var currentInstance = this;
    var value = "";
    if (currentInstance.gridSettingData.Data !== undefined) {
        $.each(currentInstance.gridSettingData.Data, function (index, item) {
            value = item[settingName];
            return value;
        });
    }
    return value;
}
userSettings.prototype.getUserSectionChoiceData = function (settingName) {
    var currentInstance = this;
    var value = "";
    if (currentInstance.sectionSettingData.Data !== undefined) {
        $.each(currentInstance.sectionSettingData.Data, function (index, item) {
            value = item[settingName];
            return value;
        });
    }
    return value;
}

userSettings.prototype.getGridSettingData = function (repeaterBuilder) {
    var currentInstance = this;
    if (currentInstance.gridSettingData.Data === undefined) {

        return currentInstance.createNewGridSettingData(repeaterBuilder);
    }
    else {
        return currentInstance.gridSettingData;
    }
}

userSettings.prototype.createNewGridSettingData = function (repeaterBuilder) {
    var currentInstance = this;
    var newRow = currentInstance.createRow(repeaterBuilder,'GRID');

    var row = {
    }
    currentInstance.GridColDef.forEach(function (item) {
            row[item.dataIndx] = item.default;                
    });

    newRow.Data.push(row);
    
    currentInstance.gridSettingData = newRow;
    return currentInstance.gridSettingData;
}

userSettings.prototype.getSectionSettingData = function (repeaterBuilder) {
    var currentInstance = this;
    if (currentInstance.sectionSettingData.Data === undefined) {

        return currentInstance.createNewSectionSettingData(repeaterBuilder);
    }
    else {
        return currentInstance.sectionSettingData;
    }
}

userSettings.prototype.createNewSectionSettingData = function (repeaterBuilder) {
    var currentInstance = this;
    var newRow = currentInstance.createRow(repeaterBuilder, 'SECTION');
    newRow.key = repeaterBuilder.formInstanceBuilder.selectedSection;

    var row = {
    }
    currentInstance.SectionColDef.forEach(function (item) {
        row[item.dataIndx] = item.default;
    });

    newRow.Data.push(row);

    currentInstance.sectionSettingData = newRow;
    return currentInstance.sectionSettingData;
}

userSettings.prototype.getUpdatedUserSettingData = function()
{
    var currentInstance= this;
    currentInstance.columnSettingData.Data = JSON.stringify(currentInstance.columnSettingData.Data);
                        
    currentInstance.gridSettingData.Data = JSON.stringify(currentInstance.gridSettingData.Data);

    currentInstance.sectionSettingData.Data = JSON.stringify(currentInstance.sectionSettingData.Data);

    var dataCollection = [];
    dataCollection.push(currentInstance.columnSettingData);
    dataCollection.push(currentInstance.gridSettingData);
    dataCollection.push(currentInstance.sectionSettingData);
    return dataCollection
}
userSettings.prototype.getColumSettingData = function (repeaterBuilder)
{
    var currentInstance = this;
  
    if (currentInstance.columnSettingData.Data === undefined) {

        return currentInstance.createNewColumSettingData(repeaterBuilder);
    }
    else {
        return currentInstance.columnSettingData;
    }
}
//no need to call this fuunction grid data is already updated to columnSettingData  and gridSettingData
userSettings.prototype.updateRow = function (event) {
    var currentInstance = this;

    var isColumnSetting = currentInstance.isColumnGrid(event);
    if(isColumnSetting===true)
    {
        currentInstance.columnSettingData.Data.forEach(function (item) {
            if (item.colname === event.Data.colname) {
                item[event.column.colId] == event.newValue
            }
        });
    }
    else
    {
        currentInstance.gridSettingData.Data.forEach(function (item) {
            if (item.colname === event.Data.colname) {
                item[event.column.colId] == event.newValue
            }
        });
    }
}

userSettings.prototype.isColumnGrid = function (event) {
    var currentInstance = this;

    var isColumnSettingGrid = false;

    currentInstance.ColDef.forEach(function (item) {
        if (item.dataIndx === event.colDef.field) {
            isColumnSettingGrid = true;
        }
    });
    return isColumnSettingGrid;

    //create a save object and called api 
}

userSettings.prototype.createRow = function (repeaterBuilder, levelAt)
{
    var currentInstance = this;
    var newRow = {
        "userId": 0,
        "formDesignVersionId": repeaterBuilder.formInstanceBuilder.formDesignVersionId,
        "levelat": levelAt,
        "key": repeaterBuilder.design.FullName,
        "Data": []
    };
    return newRow;
}
userSettings.prototype.createNewColumSettingData = function (repeaterBuilder) {
    var currentInstance = this;
    var newRow = currentInstance.createRow(repeaterBuilder,'COLUMN');

    repeaterBuilder.columnModels.forEach(function (column) {
        //toolTip

        if (column.dataIndx !== "RowIDProperty") {
            var row = {
            }
            row["colname"] = column.dataIndx;
            row["displayname"] = column.toolTip;

            //setting default value
            currentInstance.ColDef.forEach(function (item) {
                if (item.dataIndx !== "colname" && item.dataIndx !== "displayname") {
                    row[item.dataIndx] = item.default;
                }
            });

            newRow.Data.push(row);
        }
    });
    currentInstance.columnSettingData = newRow;
    return currentInstance.columnSettingData;
}

userSettings.prototype.getDropdownTextboxOptions = function (items) {
    var options = "";
    var dd = [];
    if (items != null && items.length > 0) {
        /*var defaultValue = {};
        defaultValue[""] = "";
        dd.push(defaultValue);*/


        for (var idx = 0; idx < items.length; idx++) {
            if (items[idx].ItemValue != '') {
                defaultValue = {};
                defaultValue[items[idx].ItemValue] = items[idx].ItemText == null ? items[idx].ItemValue : items[idx].ItemText;
                dd.push(defaultValue);
            }
        }
    }

    return dd;
}

userSettings.prototype.getDefaultFiltervalue = function ()
{
    if (this.repeaterBuilder.design === undefined) {
        return "Filter by Text";
    }
    if (this.repeaterBuilder.design.FilterMode.toLowerCase() == "local filtering" || this.repeaterBuilder.design.FilterMode.toLowerCase() == "both")
    {
        return "Filter by Condition";
    }
    return "Filter by Text";
}

userSettings.prototype.getPageSizeOptions = function()
{
   
    var options = [
        { "ItemValue": "10" },
        { "ItemValue": "20" },
        { "ItemValue": "50" },
        { "ItemValue": "100" }
    ]

    return options;
}

userSettings.prototype.getfilterOptions = function ()
{
    var currentInstance = this;
    var options = [{ "ItemValue": currentInstance.commonConst.FilterByCondition },
        { "ItemValue": currentInstance.commonConst.FilterByText  },
        { "ItemValue": currentInstance.commonConst.FilterByWildcard}
    ]

    return options;
}

userSettings.prototype.getfilterButtonOptions = function () {
    var options = [{ "ItemValue": "Cancel Button" },
        { "ItemValue": "Apply Button" },
        { "ItemValue": "Both" }
    ]
    return options;
}
userSettings.prototype.getYesNoOptions = function () {
    var options = [{ "ItemValue": "Yes" },
        { "ItemValue": "No" }
    ]
    return options;
}
userSettings.prototype.getHeightSizeOptions = function () {
    var options = [{ "ItemValue": "300px" },
       { "ItemValue": "400px" },
       { "ItemValue": "500px" },
       { "ItemValue": "600px" },
       { "ItemValue": "700px" },
       { "ItemValue": "800px" },
       { "ItemValue": "900px" },
       { "ItemValue": "1000px" }
    ]
    return options;
}
userSettings.prototype.getRepeaterRenderOptions = function () {
    var options = [{ "ItemValue": "Render All" },
        { "ItemValue": "Staggered" }
    ]
    return options;
}




