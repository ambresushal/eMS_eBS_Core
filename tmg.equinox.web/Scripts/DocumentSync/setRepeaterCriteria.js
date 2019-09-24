function csSetRepeaterCriteria(currentIndex, newIndex, documentSyncData) {
    this.elementIDs = {
        drpRepeater: "#drpRepeater",
        btnAddKeys: "#btnAdd",
        divRepeaterCriteria: "#dvRepeaterCriteria",
        divButtonHolder: "#buttonContainer",
        criteriaGrid: "criteriaGrid",
        criteriaGridJS: '#criteriaGrid',
        criteriaGridPagerJQ: '#pcriteriaGrid',
        stepContainer: "#documentSync"
    };

    this.URLs = {
        repeaterList: '/DocumentSync/GetSelectedRepeater',
        getDropdownData: '/DocumentSync/GetDropdownData',
        saveMacroTemplate: '/DocumentSync/UpdateSyncMacro'
    };

    this.documentSyncData = documentSyncData;
};

csSetRepeaterCriteria.prototype.process = function () {
    this.IsPublicMacro = JSON.parse(this.documentSyncData.SelectedMacro.IsPublic);
    var macroTemplate = JSON.stringify(this.documentSyncData.SelectedMacro.Template);

    if (this.hasRepeaters(macroTemplate)) {
        var that = this;
        this.bindRepeaterList();

        if (roleID != null && roleID != undefined) {
            //if (roleID == parseInt(Role.Superuser) || roleID == parseInt(Role.HNESuperUser) || roleID == parseInt(Role.ProductDesigner) ||
            //    roleID == parseInt(Role.ProductSME) || isMacroEditiable) {
                $(this.elementIDs.btnAddKeys).off("click");
                $(this.elementIDs.btnAddKeys).click(function () {
                    var selectedValue = $(that.elementIDs.drpRepeater).val();
                    if (selectedValue != "Select One") {
                        selectKeyDailog.show(that, selectedValue);
                    }
                });
            //}
            //else {
            //    $(this.elementIDs.btnAddKeys).css('display', 'none');
            //}
        }

        $(this.elementIDs.drpRepeater).change(function () {
            var selectedValue = $(this).val();
            $(that.elementIDs.divRepeaterCriteria).hide();

            if (selectedValue != "Select One") {
                $.each(that.documentSyncData.SelectedRepeater.repeaterList, function (ind, rpt) {
                    if (rpt.Value == selectedValue) {
                        if (rpt.IsSet && rpt.Keys != null && rpt.Keys != undefined) {
                            that.generateCriteriaGrid(rpt.Keys, ind);
                        }
                        else if (rpt.IsSet && (rpt.Keys == null || rpt.Keys != undefined)) {
                            var objData = {
                                formInstanceID: that.documentSyncData.SourceDocument.FormInstanceID,
                                repeaterName: selectedValue.split("#")[0],
                                template: JSON.stringify(that.documentSyncData.SelectedMacro.Template)
                            };
                            var promise = ajaxWrapper.postJSON("/DocumentSync/GetRepeaterCriteria", objData);

                            promise.done(function (xhr) {
                                rpt.Keys = xhr;
                                that.generateCriteriaGrid(rpt.Keys, ind);
                            });

                            promise.fail(that.showError);
                        }
                        return false;
                    }

                });
            }
        });
    }
    else {
        $(this.elementIDs.stepContainer).steps('next');
    }
}

csSetRepeaterCriteria.prototype.bindRepeaterList = function () {
    var that = this;

    var objData = {
        formDesignVersionID: that.documentSyncData.SelectedMacro.FormDesignVersionID,
        macroID: that.documentSyncData.SelectedMacro.MacroID,
        template: JSON.stringify(that.documentSyncData.SelectedMacro.Template)
    }
    var promise = ajaxWrapper.postJSON(that.URLs.repeaterList, objData);

    promise.done(function (xhr) {
        if (xhr.length > 0) {
            $(that.elementIDs.drpRepeater).html("");
            $(that.elementIDs.drpRepeater).append($("<option></option>").val("Select One").html("Select One"));
            that.documentSyncData.SelectedRepeater.repeaterList = xhr;
            //$.extend(that.documentSyncData.SelectedRepeater, { repeaterList: xhr });
            $.each(xhr, function (key, value) {
                $(that.elementIDs.drpRepeater).append($("<option></option>").val(value.Value).html(value.DisplayText));
            });
            $(that.elementIDs.divRepeaterCriteria).hide();
        }
    });

    promise.fail(that.showError);
}

csSetRepeaterCriteria.prototype.hasRepeaters = function (obj) {
    var hasRepeaters = false;
    var searchFor = '"Field"';

    var pattern = /\],(.*?)\},/g;
    var matches = obj.match(pattern);
    for (var i = 0; i < matches.length; i++) {
        if (matches[i].indexOf(searchFor) >= 0) {
            hasRepeaters = true;
            break;
        }
    }
    return hasRepeaters;
}

csSetRepeaterCriteria.prototype.skipStep = function (action) {
    if ($(this.elementIDs.drpRepeater).children('option').length <= 1) {
        $(this.elementIDs.stepContainer).steps(action);
    }
}

csSetRepeaterCriteria.prototype.saveCriteria = function (data) {
    var that = this;
    var flag = false;

    for (var property in data) {
        if (data[property] != "Select One") {
            flag = true;
            break;
        }
    }

    if (flag) {
        var repaterName = $(that.elementIDs.drpRepeater).val();
        $.each(that.documentSyncData.SelectedRepeater.repeaterList, function (key, value) {
            if (value.Value == repaterName) {
                repeaterIndex = key;
            }
        });

        $.each(that.documentSyncData.SelectedRepeater.repeaterList[repeaterIndex].Keys, function (ind, objKey) {
            if (objKey.isChecked == "true") {
                objKey.Source = data[objKey.Key] != "Select One" ? data[objKey.Key] : "";
            }
            else {
                var elementID = objKey.Key + "_Source";
                objKey.Source = data[elementID] != "Select One" ? data[elementID] : "";
                elementID = objKey.Key + "_Target";
                objKey.Target = data[elementID] != "Select One" ? data[elementID] : "";
            }
        });
        that.documentSyncData.SelectedRepeater.repeaterList[repeaterIndex].IsSet = true;
    }
}

csSetRepeaterCriteria.prototype.updateMacro = function () {
    var that = this;
    //Update template and save to database
    if (that.documentSyncData.SelectedRepeater.repeaterList != null) {
        $.each(that.documentSyncData.SelectedRepeater.repeaterList, function (index, repeater) {
            var objMacro = null; var path = repeater.Value.split('#')[0].split(".");
            for (var i = 0; i < path.length; i++) {
                if (objMacro == null) { objMacro = that.documentSyncData.SelectedMacro.Template; }
                objMacro = objMacro[path[i]];
            }
            if (repeater.Keys != null && repeater.Keys != undefined) {
                $.each(repeater.Keys, function (ind, key) {
                    $.each(objMacro.Keys, function (i, j) {
                        if (j.Key == key.Key) {
                            j.SourceValue = key.Source;
                            j.TargetValue = key.Target;
                            return false;
                        }
                    });
                });
            }
        });
    }
    if (JSON.stringify(that.documentSyncData.SelectedMacro.Template) != that.documentSyncData.SelectedMacro.MacroJSON) {
        var objData = {
            macroID: that.documentSyncData.SelectedMacro.MacroID,
            macroJSON: JSON.stringify(that.documentSyncData.SelectedMacro.Template)
        };
        var promise = ajaxWrapper.postJSON(that.URLs.saveMacroTemplate, objData);

        promise.done(function (xhr) {
            console.log("template saved...");
        });

        promise.fail(that.showError);
    }
}

csSetRepeaterCriteria.prototype.setCriteria = function (selectedKeys) {
    var that = this; var repeaterIndex = null;
    var repaterName = $(that.elementIDs.drpRepeater).val();

    //set keys to repeater object
    $.each(selectedKeys, function (i, j) {
        var keyName = j.UIElementPath.split(".");
        if (keyName.length > 1) {
            $.extend(j, { Key: keyName[keyName.length - 1], Data: '', Source: '', Target: '' });
        }
    });

    //Get data for all dropdowns
    var objData = {
        formInstanceID: this.documentSyncData.SourceDocument.FormInstanceID,
        repeaterName: repaterName.split("#")[0],
        keyList: JSON.stringify(selectedKeys)
    }
    var promise = ajaxWrapper.postJSON(that.URLs.getDropdownData, objData);

    promise.done(function (xhr) {
        //Set to Global Object for further use
        $.each(that.documentSyncData.SelectedRepeater.repeaterList, function (key, value) {
            if (value.Value == repaterName) {
                repeaterIndex = key;
            }
        });
        that.documentSyncData.SelectedRepeater.repeaterList[repeaterIndex].Keys = xhr;

        //Generate Table to set criteria
        that.generateCriteriaGrid(xhr, repaterName);
    });

    promise.fail(that.showError);
}

csSetRepeaterCriteria.prototype.generateCriteriaGrid = function (selectedKeys, repeaterIndex) {
    var that = this;
    this.IsPublicMacro = JSON.parse(that.documentSyncData.SelectedMacro.IsPublic);
    $(that.elementIDs.divRepeaterCriteria).show();

    var colArray = [];
    var colModel = [];
    var headers = [];
    var data = {};

    $.each(selectedKeys, function (i, j) {
        if (j.isChecked == "true" || j.isChecked == true) {
            colArray.push(j.Label);
        }
        else {
            colArray.push("Source");
            colArray.push("Target");
        }
    });

    $.each(selectedKeys, function (i, j) {
        if (j.isChecked == "true" || j.isChecked == true) {
            colModel.push({ name: j.Key, index: j.Key, editable: true, edittype: "select", editoptions: { value: j.Data } });
            data[j.Key] = j.Source == "" ? "Select One" : j.Source;
        }
        else {
            var index = j.Key + "_Source";
            colModel.push({ name: index, index: index, editable: true, edittype: "select", editoptions: { value: j.Data } });
            data[index] = j.Source == "" ? "Select One" : j.Source;
            colModel.push({ name: (j.Key + "_Target"), index: (j.Key + "_Target"), editable: true, edittype: "select", editoptions: { value: j.Data } });
            index = j.Key + "_Target";
            data[index] = j.Target == "" ? "Select One" : j.Target;
            headers.push({ startColumnName: (j.Key + "_Source"), numberOfColumns: 2, titleText: j.Label });
        }
    });

    var mydata = []; mydata.push(data);
    var lastsel2;
    $(that.elementIDs.criteriaGridJS).jqGrid("GridUnload");
    $(that.elementIDs.criteriaGridJS).parent().append("<div id='p" + that.elementIDs.criteriaGrid + "'></div>");
    $(that.elementIDs.criteriaGridJS).jqGrid({
        data: mydata,
        datatype: "local",
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Set Repeater Criteria',
        height: '150',
        autowidth: true,
        shrinkToFit: false,
        forceFit: true,
        ignoreCase: true,
        pager: that.elementIDs.criteriaGridPagerJQ,
        autowidth: true,
        pgbuttons: false,
        viewrecords: false,
        pgtext: "",
        pginput: false,
        hidegrid: false,
        altRows: true,
        altclass: 'alternate',
        onSelectRow: function (id) {
            if (id && id !== lastsel2) {
                jQuery(that.elementIDs.criteriaGridJS).jqGrid('restoreRow', lastsel2);
                jQuery(that.elementIDs.criteriaGridJS).jqGrid('editRow', id, true);
                lastsel2 = id;
            }
        }
    });
    //Set Group Header
    jQuery(that.elementIDs.criteriaGridJS).jqGrid('setGroupHeaders', { useColSpanStyle: true, groupHeaders: headers });

    //Set Pager
    var pagerElement = that.elementIDs.criteriaGridPagerJQ;
    //if (roleID == parseInt(Role.Superuser) || roleID == parseInt(Role.HNESuperUser) || roleID == parseInt(Role.ProductDesigner) ||
      //  roleID == parseInt(Role.ProductSME) || isMacroEditiable) {
        $(that.elementIDs.criteriaGridJS).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false, refresh: false });
        $(that.elementIDs.criteriaGridJS).jqGrid('navButtonAdd', pagerElement, {
            caption: 'Save', buttonicon: 'ui-icon-disk', title: 'Save',
            onClickButton: function () {
                $(that.elementIDs.criteriaGridJS).jqGrid("saveRow", lastsel2);
                var data = $(that.elementIDs.criteriaGridJS).jqGrid("getRowData", lastsel2);
                that.saveCriteria(data);
            }
        });
    //}
}

csSetRepeaterCriteria.prototype.showError = function showError(xhr) {
    if (xhr.status == 999)
        this.location = '/Account/LogOn';
    else
        messageDialog.show(JSON.stringify(xhr));
}

var selectKeyDailog = function () {
    var parentInstance;
    var URLs = {
        getKeys: '/DocumentSync/RepeaterKeys'
    };

    var elementIDs = {
        //id for dialog div
        keyDailogJQ: "#selectRepeaterKeysDailog",
        keyGrid: "keysGrid",
        keyGridJS: "#keysGrid",
        btnSelectKeys: "#btnAddKeys",
        chkSelectAll: "#selectAllKeys"
    }

    function init() {
        //register dialog 
        $(elementIDs.keyDailogJQ).dialog({
            autoOpen: false,
            height: 400,
            width: 400,
            modal: true
        });

        $(elementIDs.btnSelectKeys).click(function () {
            var data = $(elementIDs.keyGridJS).getRowData();
            var selectedKeys = [];
            $.each(data, function (index, value) {
                if (value.isChecked == "true") {
                    selectedKeys.push(value.UIElementPath);
                }
            });
            if (selectedKeys.length > 0) {
                $(elementIDs.errorDiv).hide();
                parentInstance.setCriteria(data);
                $(elementIDs.keyDailogJQ).dialog("close");
            }
            else {
                messageDialog.show("Please select keys for match.");
            }
        });
    }

    //initialize the dialog after this js is loaded
    init();

    function loadKeyGrid(dialogParent, selectedValue) {
        var that = this;

        var colArray = ['UIElementID', 'Name', 'UIElementPath', '<input type="checkbox" id="selectAllKeys"/>'];
        var colModel = [];
        colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, editable: false, align: 'left' });
        colModel.push({ name: 'Label', index: 'Label', editable: false, align: 'left' });
        colModel.push({ name: 'UIElementPath', index: 'UIElementPath', hidden: true });
        colModel.push({ name: 'isChecked', index: 'isChecked', width: 20, align: 'left', editable: true, formatter: 'checkbox', editoptions: { value: 'true:false' }, formatoptions: { disabled: false } });

        var element = selectedValue.split("#");

        if (element.length > 1) {
            $(elementIDs.keyGridJS).jqGrid('GridUnload');

            var objData = {
                formDesignVersionID: dialogParent.documentSyncData.SelectedMacro.FormDesignVersionID,
                elementID: element[1],
                macroID: dialogParent.documentSyncData.SelectedMacro.MacroID,
                elementPath: element[0],
                template: JSON.stringify(dialogParent.documentSyncData.SelectedMacro.Template)
            };

            var url = URLs.getKeys;
            $(elementIDs.keyGridJS).jqGrid({
                url: url,
                mtype: 'POST',
                postData: objData,
                datatype: 'json',
                cache: false,
                colNames: colArray,
                colModel: colModel,
                caption: 'Keys',
                height: '230',
                loadonce: true,
                rowNum: 20,
                rowList: [10, 20, 30],
                ignoreCase: true,
                autowidth: true,
                viewrecords: true,
                hidegrid: false,
                altRows: true,
                altclass: 'alternate',
                loadComplete: function () {

                }
            });

            $(elementIDs.chkSelectAll).click(function (e) {
                e = e || event; e.stopPropagation ? e.stopPropagation() : e.cancelBubble = false;
                var value = $(elementIDs.chkSelectAll).is(':checked');

                var gridData = $(elementIDs.keyGridJS).jqGrid('getGridParam', 'data');
                $.each(gridData, function (idx, row) {
                    row.isChecked = value;
                });
                $(elementIDs.keyGridJS).jqGrid('setGridParam', { datatype: 'local', data: gridData }).trigger('reloadGrid');
                if (value) {
                    $(elementIDs.chkSelectAll).attr('checked', 'checked');
                }
                else {
                    $(elementIDs.chkSelectAll).removeAttr('checked');
                }
            });
        }
        else {
            console.log("UIElement path is not valid.");
        }
    }

    return {
        show: function (dialogParent, selectedValue) {
            parentInstance = dialogParent;
            $(elementIDs.keyDailogJQ).dialog('option', 'title', "Select Keys");
            $(elementIDs.keyDailogJQ).dialog("open");
            loadKeyGrid(dialogParent, selectedValue);
        }
    }
}();
