var datasourcesync = function (formInstanceBuilder, sectionBuilder, dataSource, rowIDProperty, status) {
    this.formInstanceBuilder = formInstanceBuilder;
    this.currentInstance = sectionBuilder;
    this.dataSource = dataSource;
    this.rowIDProperty = rowIDProperty;
    this.status = status;
}

datasourcesync.prototype.syncMapPrimaryDataSource = function () {
    var currentInstance = this.currentInstance;
    var formInstanceBuilder = this.formInstanceBuilder;
    var dataSource = this.dataSource;
    var rowIDProperty = this.rowIDProperty;
    var status = this.status;
    var matchProperties = {};
    matchProperties.RowIndex = -1;
    matchProperties.RowIDProperty = -1;
    //get target repeater fullname
    targetParentName = dataSource.TargetParent;

    //get target repeater data
    var targetRepeaterData = this.getTargetRepeaterData(targetParentName);

    //get datasource mappings of target Repeater
    var targetRepeaterMappings = currentInstance.formInstanceBuilder.designData.DataSources.filter(function (ct) {
        if (ct.TargetParent == dataSource.TargetParent) {
            return ct;
        }
    });

    //get inline mapping of target repeater
    var inlineMapping = targetRepeaterMappings.filter(function (ds) {
        if ((ds.SourceParent != dataSource.SourceParent) && (ds.DisplayMode == "In Line")) {
            return ds;
        }
    });

    if (currentInstance.data.length != undefined) {
        //push new row in target repeater            
        if (status == "add") {
            matchRowPropertyID = this.addNewPrimaryDataSourceRow(targetRepeaterData);
            matchProperties.RowIndex = targetRepeaterData.length -1;
            matchProperties.RowIDProperty = matchRowPropertyID;

        }
    }

    if (currentInstance.data.length != undefined && targetRepeaterData.length > 0) {
        if (status == "save") {
            //Update row in target repeater
            matchRowIndex = this.updatePrimaryDataSource(targetRepeaterData);
            if (matchRowIndex > -1) {
                matchRowPropertyID = targetRepeaterData[matchRowIndex].RowIDProperty;
                matchProperties.RowIndex = matchRowIndex;
                matchProperties.RowIDProperty = matchRowPropertyID;
            }
        }

        //remove row from target repeater         
        if (status == "delete") {
            matchProperties = this.removePrimaryDataSourceRow(targetRepeaterData);
        }
    }

    //add inline dataSource to row if which does not contain inline datasource object
    if (targetRepeaterData[matchProperties.RowIndex] != undefined && inlineMapping[0] != undefined) {
        if (targetRepeaterData[matchProperties.RowIndex][inlineMapping[0].DataSourceName] == undefined) {
            var inlineSourceRepeater = this.getTargetRepeaterData(inlineMapping[0].SourceParent);
            this.bindInlineDataSource(inlineSourceRepeater, targetRepeaterData, inlineMapping, matchProperties.RowIndex);
        }
    }
    return matchProperties;
}

datasourcesync.prototype.syncMapChildDataSource = function () {

    var currentInstance = this.currentInstance;
    var formInstanceBuilder = this.formInstanceBuilder;
    var dataSource = this.dataSource;
    var rowIDProperty = this.rowIDProperty;
    var status = this.status;

    //get target repeater full name
    targetParentName = dataSource.TargetParent;

    //get target repeater data
    var targetRepeaterData = this.getTargetRepeaterData(targetParentName);

    //get datasource mappings
    var dataSourceMap = dataSource.Mappings;

    if (targetRepeaterData != undefined && targetRepeaterData.length > 0) {

        //update target repeater data       
        if (status == "save") {
            this.updateChildDataSource(targetRepeaterData);
        }

        //push new row into target repeater        
        if (status == "add") {
            this.addNewChildDataSourceRow(targetRepeaterData);
        }

        //remove rows from source repeater        
        if (status == "delete") {
            this.removeChildDataSourceRow(targetRepeaterData);
        }
    }
}

datasourcesync.prototype.syncMapInLineDataSource = function () {
    var currentInstance = this.currentInstance;
    var formInstanceBuilder = this.formInstanceBuilder;
    var dataSource = this.dataSource;
    var rowIDProperty = this.rowIDProperty;
    var status = this.status;

    //get target repeater full name
    targetParentName = dataSource.TargetParent;

    //get target repeater data
    var targetRepeaterData = this.getTargetRepeaterData(targetParentName);

    //get datasource mappings
    var dataSourceMap = dataSource.Mappings;

    if (targetRepeaterData != undefined && targetRepeaterData.length > 0) {

        //update target repeater data        
        if (status == "save") {
            //change group header in design
            this.updateChildDataSource(targetRepeaterData);
        }

        //push new row into target repeater        
        if (status == "add") {
            if (targetRepeaterData[0][dataSource.DataSourceName] == undefined) {
                this.bindDataSourceObjectToNewRow(targetRepeaterData, 0);
            }
            if (targetRepeaterData[0][dataSource.DataSourceName][rowID] == undefined) {
                this.addNewChildDataSourceRow(targetRepeaterData);
            }
        }

        //delet row from target repeater        
        if (status == "delete") {
            this.removeChildDataSourceRow(targetRepeaterData);
        }
    }
}

datasourcesync.prototype.getTargetRepeaterData = function (targetParentName) {
    var formInstanceBuilder = this.formInstanceBuilder;

    //split targetParent name 
    var targetParentArray = targetParentName.split('.');

    //select selected section name
    var targetParentSectionName = targetParentArray[0];

    //get section data
    var targetdata = formInstanceBuilder.formData[targetParentSectionName];

    //get repeater data
    for (var n = 1; n < targetParentArray.length; n++) {
        targetdata = targetdata[targetParentArray[n]];
    }

    return targetdata;
}

datasourcesync.prototype.addNewPrimaryDataSourceRow = function (targetRepeaterData) {
    var currentInstance = this.currentInstance;
    var rowIDProperty = this.rowIDProperty;
    var isRowID = false;

    var matchRowIndex = this.getRepeaterMatchRow(targetRepeaterData);
    if (matchRowIndex == -1) {
        //get target repeater design       
        var targetRepeaterDesign = this.getTargetRepeaterDesign();

        repeaterID = '#' + targetRepeaterDesign.Name + currentInstance.formInstanceId;

        //var colmodel = $(repeaterID).jqGrid('getGridParam', 'colModel');
        var colmodel = $(repeaterID).pqGrid("getColModel");

        var newRow = {};

        if (colmodel != undefined) {
            for (var idx = 0; idx < colmodel.length; idx++) {
                if (colmodel[idx].index != undefined) {
                    if (colmodel[idx].index.split("_")[0] != "INL") {
                        newRow[colmodel[idx].index] = "";
                    }
                }
            }
        }
        else {
            colmodel = targetRepeaterDesign.Repeater.Elements;
            for (var t = 0; t < colmodel.length; t++) {
                newRow[colmodel[t].GeneratedName] = "";
            }
        }
        matchRowIndex = this.getNextRowPropertyID(targetRepeaterData);
        newRow.RowIDProperty = matchRowIndex;
        targetRepeaterData.push(newRow);
    }
    return matchRowIndex;
}

datasourcesync.prototype.updatePrimaryDataSource = function (targetRepeaterData) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var rowIDProperty = this.rowIDProperty;
    var dataSourceMap = dataSource.Mappings;
    var matchRowIndex = -1;
    //need to get row index from rowIDProperty here
    if (rowIDProperty != undefined) {
        //determine matching record
        var matchRowIndex = this.getRepeaterMatchIndex(targetRepeaterData, rowIDProperty);
        var sourceRowIndex = this.getRepeaterMatchIndex(currentInstance.data, rowIDProperty);
        if (matchRowIndex > -1) {
            //update target data
            for (var j = 0; j < dataSourceMap.length; j++) {
                if (dataSourceMap[j].CopyModeID == 1 || isKeyChange == true) {
                    if (currentInstance.data[sourceRowIndex][dataSourceMap[j].SourceElement] != undefined)
                        targetRepeaterData[matchRowIndex][dataSourceMap[j].TargetElement] = currentInstance.data[sourceRowIndex][dataSourceMap[j].SourceElement];
                }
                else {
                    if (targetRepeaterData[matchRowIndex][dataSourceMap[j].TargetElement] != undefined && targetRepeaterData[matchRowIndex][dataSourceMap[j].TargetElement] == '') {
                        if (currentInstance.data[sourceRowIndex][dataSourceMap[j].SourceElement] != undefined) {
                            targetRepeaterData[matchRowIndex][dataSourceMap[j].TargetElement] = currentInstance.data[sourceRowIndex][dataSourceMap[j].SourceElement];
                        }
                    }
                }
            }
        }
    }
    return matchRowIndex;
}

datasourcesync.prototype.removePrimaryDataSourceRow = function (targetRepeaterData) {
    var rowIDProperty = this.rowIDProperty;
    var matchProperties = {};
    matchProperties.RowIndex = -1;
    matchProperties.RowIDProperty = -1;
    // delete row from target repeater
    if (rowIDProperty != undefined) {
        var matchRowIndex = this.getRepeaterMatchRow(targetRepeaterData);
        if (matchRowIndex > -1) {
            matchProperties.RowIndex = matchRowIndex;
            matchProperties.RowIDProperty = targetRepeaterData[matchRowIndex].RowIDProperty;
            targetRepeaterData.splice(matchRowIndex, 1);
        }
    }
    return matchProperties;
}

datasourcesync.prototype.updateChildDataSource = function (targetRepeaterData) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var rowID = this.rowID;

    var dataSourceMap = dataSource.Mappings;

    if (rowID != undefined) {
        var isKeyChange = false;
        $.each(dataSourceMap, function (idx, ds) {
            if (ds.IsKey == true) {
                if (targetRepeaterData[0][dataSource.DataSourceName][rowID][ds.TargetElement] != currentInstance.data[rowID][ds.SourceElement]) {
                    isKeyChange = true;
                }
                else {
                    isKeyChange = false;
                    return false;
                }
            }
        });

        //update group header in design
        $.each(currentInstance.formInstanceBuilder.designData.DataSources, function (idx, ct) {
            if (ct.SourceParent == currentInstance.fullName) {
                ct.GroupHeader = [];
                $.each(currentInstance.data, function (idx, val) {
                    var str = '{';
                    var keyMaps = ct.Mappings.filter(function (val) {
                        return val.IsKey == true;
                    });
                    var mapCount = keyMaps.length;
                    $.each(ct.Mappings, function (indx, map) {
                        if (val[map.SourceElement] != null && map.IsKey == true) {
                            str = str + '"' + map.SourceElement + '":"' + val[map.SourceElement] + '"';
                            if (indx < mapCount - 1) {
                                str = str + ',';
                            }
                        }
                    });
                    str = str + '}';
                    var ghObj = JSON.parse(str);
                    ct.GroupHeader.push(ghObj);
                });
            }
        });

        for (var k = 0; k < targetRepeaterData.length; k++) {
            if (targetRepeaterData[k][dataSource.DataSourceName] == undefined) {
                this.bindDataSourceObjectToNewRow(targetRepeaterData, k);
            }
            else if (targetRepeaterData[k][dataSource.DataSourceName].length == 0) {
                this.bindDataSourceObjectToNewRow(targetRepeaterData, k);
            }

            if (targetRepeaterData[k][dataSource.DataSourceName][rowID] == undefined) {
                this.addNewChildDataSourceRow(targetRepeaterData);
            }

            for (var j = 0; j < dataSource.Mappings.length; j++) {
                if (dataSource.Mappings[j].CopyModeID == 1 || isKeyChange == true) {
                    if (currentInstance.data[rowID][dataSource.Mappings[j].SourceElement] != undefined) {
                        targetRepeaterData[k][dataSource.DataSourceName][rowID][dataSource.Mappings[j].TargetElement] = currentInstance.data[rowID][dataSource.Mappings[j].SourceElement];
                    }
                }
                else {
                    if (targetRepeaterData[k][dataSource.DataSourceName][rowID][dataSource.Mappings[j].TargetElement] != undefined && targetRepeaterData[k][dataSource.DataSourceName][rowID][dataSource.Mappings[j].TargetElement] == '')
                        if (currentInstance.data[rowID][dataSource.Mappings[j].SourceElement] != undefined)
                            targetRepeaterData[k][dataSource.DataSourceName][rowID][dataSource.Mappings[j].TargetElement] = currentInstance.data[rowID][dataSource.Mappings[j].SourceElement];
                }
            }
        }
    }
}

datasourcesync.prototype.addNewChildDataSourceRow = function (targetRepeaterData) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var rowID = this.rowID;

    for (var l = 0; l < targetRepeaterData.length; l++) {

        var newRow = {};

        for (var c = 0; c < dataSource.Mappings.length; c++) {
            newRow[dataSource.Mappings[c].TargetElement] = "";
        }

        //add data source collection to new row
        if (targetRepeaterData[l][dataSource.DataSourceName] == undefined) {
            targetRepeaterData[l][dataSource.DataSourceName] = [];
            targetRepeaterData[l][dataSource.DataSourceName].push(newRow);
        }
        else {
            //push new row in target repeater
            if (targetRepeaterData[l][dataSource.DataSourceName][rowID] == undefined) {
                targetRepeaterData[l][dataSource.DataSourceName].push(newRow);
            }
        }
    }
}

datasourcesync.prototype.removeChildDataSourceRow = function (targetRepeaterData) {
    var rowID = this.rowID;
    var dataSource = this.dataSource;
    var rowid = undefined;

    for (var l = 0; l < targetRepeaterData.length; l++) {
        //delete rowid from target repeater
        if (rowID != undefined) {
            targetRepeaterData[l][dataSource.DataSourceName].splice(rowID, 1);
        }
    }
}

datasourcesync.prototype.bindDataSourceObjectToNewRow = function (targetRepeaterData, rowindx) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var rowID = this.rowID;

    if (targetRepeaterData[rowindx][dataSource.DataSourceName] == undefined) {
        //bind new data source array to new row
        targetRepeaterData[rowindx][dataSource.DataSourceName] = [];

        for (var x = 0; x < currentInstance.data.length; x++) {

            var newRow = {};

            for (var c = 0; c < dataSource.Mappings.length; c++) {
                newRow[dataSource.Mappings[c].TargetElement] = "";
            }

            //add row to datasource array
            targetRepeaterData[rowindx][dataSource.DataSourceName].push(newRow);

            //assign  data to row as per data source mappings
            for (var y = 0; y < dataSource.Mappings.length; y++) {
                if (currentInstance.data[x][dataSource.Mappings[y].SourceElement] != undefined) {
                    targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] = currentInstance.data[x][dataSource.Mappings[y].SourceElement];
                }
                else {
                    if (targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] == undefined || targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] == "")
                        targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] = "";
                }
            }
        }
    }
}

datasourcesync.prototype.bindChildDataSource = function (targetRepeaterData, rowindx) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var rowID = this.rowID;

    if (targetRepeaterData[rowindx][dataSource.DataSourceName] == undefined) {
        //bind new data source array to new row
        targetRepeaterData[rowindx][dataSource.DataSourceName] = [];

        if (dataSource.DataSourceModeType == "Auto") {
            for (var x = 0; x < currentInstance.length; x++) {

                var newRow = {};

                for (var c = 0; c < dataSource.Mappings.length; c++) {
                    newRow[dataSource.Mappings[c].TargetElement] = "";
                }

                //add row to datasource array
                targetRepeaterData[rowindx][dataSource.DataSourceName].push(newRow);

                //assign  data to row as per data source mappings
                for (var y = 0; y < dataSource.Mappings.length; y++) {
                    if (currentInstance[x][dataSource.Mappings[y].SourceElement] != undefined) {
                        targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] = currentInstance[x][dataSource.Mappings[y].SourceElement];
                    }
                    else {
                        if (targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] == undefined || targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] == "")
                            targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] = "";
                    }
                }
            }
        }
        else if (dataSource.DataSourceModeType == "Manual") {
            var newRow = {};

            for (var c = 0; c < dataSource.Mappings.length; c++) {
                newRow[dataSource.Mappings[c].TargetElement] = "";
            }
            //add row to datasource array
            targetRepeaterData[rowindx][dataSource.DataSourceName].push(newRow);
        }

    }
}

datasourcesync.prototype.bindInlineDataSource = function (inlineSourceRepeater, targetRepeaterData, inlineMappingdataSource, rowindx) {
    var inlineSourceRepeater = inlineSourceRepeater;
    var dataSource = inlineMappingdataSource[0];
    var rowID = rowindx;

    if (targetRepeaterData[rowindx][dataSource.DataSourceName] == undefined) {
        //bind new data source array to new row
        targetRepeaterData[rowindx][dataSource.DataSourceName] = [];

        for (var x = 0; x < inlineSourceRepeater.length; x++) {

            var newRow = {};

            for (var c = 0; c < dataSource.Mappings.length; c++) {
                newRow[dataSource.Mappings[c].TargetElement] = "";
            }

            //add row to datasource array
            targetRepeaterData[rowindx][dataSource.DataSourceName].push(newRow);

            //assign  data to row as per data source mappings
            for (var y = 0; y < dataSource.Mappings.length; y++) {
                if (inlineSourceRepeater[x][dataSource.Mappings[y].SourceElement] != undefined) {
                    targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] = inlineSourceRepeater[x][dataSource.Mappings[y].SourceElement];
                }
                else {
                    targetRepeaterData[rowindx][dataSource.DataSourceName][x][dataSource.Mappings[y].TargetElement] = "";
                }
            }
        }
    }
}

datasourcesync.prototype.getRepeaterDropdownElement = function (targetRepeater) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;

    var dropdwonelement = undefined;

    $.each(targetRepeater.design.Elements, function (idx, dl) {
        if ((dl.Type == 'select' || dl.Type == "SelectInput") && dl.GeneratedName == [dataSource.Mappings[0].TargetElement]) {
            return dropdwonelement = dl;
        }
    });

    return dropdwonelement;
}

datasourcesync.prototype.getSourceDropdownItems = function () {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;

    var items = [];

    //get items from source repeater
    for (var j = 0; j < currentInstance.data.length; j++) {
        items[j] = currentInstance.data[j][dataSource.Mappings[0].SourceElement];
    }

    // items = items.sort();

    return items;
}

datasourcesync.prototype.updateRepeaterDropDown = function (dropDownElement, status) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;

    var newitems = [], targetItems = [], items = [], updatedItems = [];

    //get items from source repeater field
    newitems = this.getSourceDropdownItems();

    //get items from terget dropdownelement
    for (var t = 0; t < dropDownElement.Items.length; t++) {
        targetItems[t] = dropDownElement.Items[t].ItemValue;
    }

    //update dropdownelement items
    this.updateDropDownItems(dropDownElement, newitems, status);


    for (var s = 0; s < dropDownElement.Items.length; s++) {
        if (dropDownElement.Items[s].ItemValue == undefined) {
            dropDownElement.Items[s].ItemValue = "";
        }
    }


    var updatedDropdownItem = currentInstance.sortOptions(dropDownElement.Items);

    for (var m = 0; m < updatedDropdownItem.length; m++) {
        updatedItems[m] = updatedDropdownItem[m].ItemValue;
    }

    items[0] = updatedItems;
    items[1] = targetItems;

    return items;
}

datasourcesync.prototype.updateSectionDropDown = function (dropdwonelement, status) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;

    //get items from source repeater
    var newitems = [], targetItems = [], updatedItems = [];

    //get items from source repeater field
    newitems = this.getSourceDropdownItems();

    var updatedDropdownItem = undefined;
    //update dropdownelement items
    if (dropdwonelement.Type == "select") {
        this.updateDropDownItems(dropdwonelement, newitems, status);
    }
    else {
        this.updateDropDownTextBoxItems(dropdwonelement, newitems, status);
    }

    updatedDropdownItem = currentInstance.sortOptions(dropdwonelement.Items);

    //get dropdown items into updatedItems variable excluding highlighted item
    for (var m = 0; m < updatedDropdownItem.length; m++) {
        if (updatedDropdownItem[m] != undefined) {
            //if (updatedDropdownItem[m].cssclass == undefined)
            updatedItems[m] = updatedDropdownItem[m].ItemValue;
        }
    }
    return updatedItems;
}

datasourcesync.prototype.updateDropDownItems = function (dropdownelement, items, status) {
    if (status == 'delete') {
        dropdownelement.Items.splice(dropdownelement.Items.length - 1, 1);
    }

    for (var i = 0; i < items.length; i++) {
        if (dropdownelement.Items[i] == undefined) {
            var obj = {
                Enabled: null,
                ItemID: 0,
                ItemValue: ""
            }
            dropdownelement.Items.push(obj);
        }
        dropdownelement.Items[i].ItemValue = items[i];
    }
}

datasourcesync.prototype.updateDropDownTextBoxItems = function (dropdownelement, items, status) {
    if (status == 'delete') {
        if (dropdownelement.Items[dropdownelement.Items.length - 1].cssclass == "highlight")
            dropdownelement.Items.splice(dropdownelement.Items.length - 2, 1);
        else
            dropdownelement.Items.splice(dropdownelement.Items.length - 1, 1);
    }

    var enterUinqueResponse = dropdownelement.Items.filter(function (dt) {
        return dt.cssclass == "highlight"

    });

    if (enterUinqueResponse != undefined && enterUinqueResponse.length != 0) {
        var isenterUniqueResponseExistInSourceRepeater = items.filter(function (itm) {
            return itm == enterUinqueResponse[0].ItemValue;
        });
    }

    if (isenterUniqueResponseExistInSourceRepeater != undefined && isenterUniqueResponseExistInSourceRepeater.length != 0) {
        $.each(dropdownelement.Items, function (idx, kt) {
            if (dropdownelement.Items[idx].cssclass == "highlight") {
                dropdownelement.Items.splice(idx, 1);
                return false;
            }
        });
    }

    if (enterUinqueResponse != undefined && enterUinqueResponse.length != 0 && isenterUniqueResponseExistInSourceRepeater.length == 0) {
        if (items.length == dropdownelement.Items.length) {
            var obj = {
                Enabled: null,
                ItemID: 0,
                ItemValue: ""
            }
            dropdownelement.Items.push(obj);
        }
    }
    else if (items.length > dropdownelement.Items.length) {
        var obj = {
            Enabled: null,
            ItemID: 0,
            ItemValue: ""
        }
        dropdownelement.Items.push(obj);
    }

    var j = 0;
    for (var i = 0; i < items.length; i++) {
        if (items[i] != undefined && dropdownelement.Items[j] != undefined) {
            if (dropdownelement.Items[j].cssclass != "highlight") {
                dropdownelement.Items[j].ItemValue = items[i];

            }
            else {
                i--;
            }
            j++;
        }
    }
}

datasourcesync.prototype.getTargetRepeaterDesign = function () {
    var currentInstance = this.currentInstance;
    var formInstanceBuilder = this.formInstanceBuilder;
    var dataSource = this.dataSource;

    var fullNameArray = dataSource.TargetParent.split('.');

    var targetsection = undefined;

    targetsection = currentInstance.formInstanceBuilder.designData.Sections.filter(function (ts) {
        return ts.GeneratedName == fullNameArray[0];
    });

    if (fullNameArray[1] != undefined) {
        targetsection = targetsection[0].Elements.filter(function (sd) {
            return sd.GeneratedName == fullNameArray[1];
        });


        for (var l = 2; l < fullNameArray.length; l++) {
            targetsection = targetsection[0].Section.Elements.filter(function (sd) {
                return sd.GeneratedName == fullNameArray[l];
            });
        }
    }
    return targetsection[0];
}

datasourcesync.prototype.getRepeaterMatchRow = function (repeaterData) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var rowIDProperty = this.rowIDProperty;
    var dataSourceMap = dataSource.Mappings;
    var sourceIndex = this.getRepeaterMatchIndex(currentInstance.data, rowIDProperty);
    var matchRowIndex = -1;
    if (sourceIndex != undefined) {
        //determine matching record
        $.each(repeaterData, function (idx, rec) {
            var match = false;
            $.each(dataSourceMap, function (index, ds) {
                if (ds.IsKey == true) {
                    if (rec[ds.TargetElement] == undefined)
                        rec[ds.TargetElement] = "";
                    if ((rec[ds.TargetElement] == currentInstance.data[sourceIndex][ds.SourceElement]) &&
                        (rec["RowIDProperty"] == currentInstance.data[sourceIndex]["RowIDProperty"])) {
                        match = true;
                    }
                }
            });
            if (match == true) {
                matchRowIndex = idx;
            }
        });
    }
    return matchRowIndex;
}

datasourcesync.prototype.getRepeaterMatchIndex = function (repeaterData, rowIDProperty) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var matchRowIndex = -1;
    if (rowIDProperty != undefined) {
        $.each(repeaterData, function (idx, val) {
            if (val.RowIDProperty == rowIDProperty) {
                matchRowIndex = idx;
            }
        });
    }
    return matchRowIndex;
}

datasourcesync.prototype.getNextRowPropertyID = function (repeaterData) {
    var currentInstance = this.currentInstance;
    var dataSource = this.dataSource;
    var rowID = this.rowID;
    var newRowIDProperty = -1;
    var dataSourceMap = dataSource.Mappings;

    $.each(repeaterData, function (idx, rec) {
        if (rec.RowIDProperty != undefined) {
            var curr = parseInt(rec.RowIDProperty);
            if (curr != NaN) {
                if (curr > newRowIDProperty) {
                    newRowIDProperty = curr;
                }
            }
        }
    });
    newRowIDProperty = newRowIDProperty + 1;
    return newRowIDProperty;
}