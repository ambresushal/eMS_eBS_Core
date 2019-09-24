var assigntarget = (function () {
    var _tabElementId;
    var _idsOfSelectedRows = [];
    var _currentRule;
    var _initialized = false;
    var lastsel2;

    var elementIDs = {
        assignTargetGrid: 'grdtargetelement',
        assignTargetGridJQ: '#grdtargetelement',
        rptKeyDialog: '#repeaterKeydialog',
        rptKeysGrid: 'repeaterkeygrid',
        rptKeysGridJQ: '#repeaterkeygrid',
        formDesignVersionJQ: '#drpFormDesignVersion',
    };

    var URLs = {
        assignTargets: '/RulesManager/AssignTargets?tenantId=1&uiElementId={uiElementId}&formDesignVersionID={formDesignVersionID}',
        targetKeyList: '/UIElement/GetTargetRepeaterKeys?tenantId=1&uiElementId={uiElementId}&ruleId={ruleId}'
    };

    function _registerEvents(rowData) {
        if (_initialized == false) {
            $(elementIDs.rptKeyDialog).dialog({ autoOpen: false, height: '300', width: '40%', modal: true });
            _initialized = true;
        }
        $(elementIDs.rptKeyDialog).off("dialogclose");
        $(elementIDs.rptKeyDialog).on("dialogclose", function (event, ui) {
            $(elementIDs.rptKeysGridJQ).jqGrid('saveRow', lastsel2, false);
            var filterData = $(elementIDs.rptKeysGridJQ).getRowData();
            if (filterData.length > 0) {
                rowData.TargetKeyFilter = JSON.stringify(filterData);
                $(elementIDs.assignTargetGridJQ).jqGrid('setRowData', rowData.UIElementID, rowData);
            }
        });
    }

    function _showTargetKeyDialog(rowData) {
        _registerEvents(rowData);
        $(elementIDs.rptKeyDialog).dialog('option', 'title', 'Target Key Filter');
        $(elementIDs.rptKeyDialog).dialog('open');

        if (rowData != null && rowData.TargetKeyFilter != null && rowData.TargetKeyFilter.length > 0) {
            _loadTargetKeyFilterGrid(JSON.parse(rowData.TargetKeyFilter));
            return;
        }
        else {
            var keyUrl = URLs.targetKeyList.replace(/\{uiElementId\}/g, rowData.Parent).replace(/\{ruleId\}/g, _currentRule.RuleID);
            var promise = ajaxWrapper.getJSON(keyUrl);
            promise.done(function (xhr) {
                _loadTargetKeyFilterGrid(xhr);
            });
            promise.fail(_showError);
        }
    }

    function _loadTargetKeyFilterGrid(filterData) {
        var colArray = ['UIElementID', 'Key', 'UIElementPath', 'Value'];
        var colModel = [];
        colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, editable: false, align: 'left' });
        colModel.push({ name: 'Label', index: 'Label', editable: false, align: 'left' });
        colModel.push({ name: 'UIElementPath', index: 'UIElementPath', hidden: true });
        colModel.push({ name: 'FilterValue', index: 'FilterValue', editable: true, align: 'left' });

        $(elementIDs.rptKeysGridJQ).jqGrid('GridUnload');
        $(elementIDs.rptKeysGridJQ).jqGrid({
            datatype: 'local',
            data: filterData,
            cache: false,
            colNames: colArray,
            colModel: colModel,
            caption: 'Key Filter',
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
            onSelectRow: function (id) {
                if (id && id !== lastsel2) {
                    $(elementIDs.rptKeysGridJQ).jqGrid('restoreRow', lastsel2);
                    $(elementIDs.rptKeysGridJQ).jqGrid('editRow', id, true);
                    lastsel2 = id;
                }
            },
            beforeSelectRow: function (id, e) {
                if (id) {
                    if (id != lastsel2)
                        $(elementIDs.rptKeysGridJQ).jqGrid('saveRow', lastsel2, false);
                }
                return true;
            },
            gridComplete: function () {
                $(elementIDs.rptKeyDialog).dialog({ position: { my: 'center', at: 'center' }, });
            }
        });
    }

    function _setCurrentRule(rule) {
        _currentRule = rule;
    }

    function _buildTargetGrid(data, isEditable, mapping) {
        //set column list
        var colArray = ['Select', 'UIElementID', 'UIElementName', 'Section', 'Element', 'Parent', 'IsContainer', 'Key', 'Key Filter'];
        //set column models
        var colModel = [];
        colModel.push({ name: 'Select', index: 'Select', hidden: true, editable: false });
        colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, editable: false });
        colModel.push({ name: 'UIElementName', index: 'UIElementName', hidden: true, editable: false });
        colModel.push({ name: 'Section', index: 'Section', width: 300, align: 'left', editable: false });
        colModel.push({ name: 'Element', index: 'Element', width: 300, align: 'left', editable: false });
        colModel.push({ name: 'Parent', index: 'Parent', hidden: true, align: 'left', editable: false });
        colModel.push({ name: 'IsContainer', index: 'IsContainer', hidden: true, align: 'left', editable: false });
        colModel.push({ name: 'Key', index: 'Key', width: 40, align: 'center', search: false, editable: false, formatter: _imageformat });
        colModel.push({ name: 'TargetKeyFilter', index: 'TargetKeyFilter', hidden: true, search: false, editable: false });

        $(elementIDs.assignTargetGridJQ).jqGrid('GridUnload');
        $(elementIDs.assignTargetGridJQ).parent().append("<div id='p" + elementIDs.assignTargetGrid + "'></div>");

        $(elementIDs.assignTargetGridJQ).jqGrid({
            datatype: 'local',
            data: data,
            cache: false,
            colNames: colArray,
            colModel: colModel,
            height: '210',
            caption: 'Assign Targets',
            rowList: [25, 50, 75, 100],
            rowNum: 25,
            autowidth: false,
            shrinkToFit: false,
            multiselect: true,
            viewrecords: true,
            altRows: true,
            sortname: 'Select',
            sortorder: 'desc',
            altclass: 'alternate',
            ignoreCase: true,
            hidegrid: false,
            pager: '#p' + elementIDs.assignTargetGrid,
            beforeSelectRow: function (rowid, e) {
                var result = true;
                var $self = $(this), $td = $(e.target).closest("td"), iCol = $.jgrid.getCellIndex($td[0]);
                var rowData = $(elementIDs.assignTargetGridJQ).jqGrid("getRowData", rowid);
                var cm = $self.jqGrid("getGridParam", "colModel");
                if (cm[iCol].name === "Key" && e.target.tagName.toUpperCase() === "SPAN") {
                    _showTargetKeyDialog(rowData);
                    result = false;
                }
                else {
                    if (_currentRule.Type != 'Enable' && _currentRule.Type != 'Visible' && rowData.IsContainer == "true") {
                        if (cm[iCol].name === 'cb') {
                            $td.find('input[type=checkbox]').prop('checked', false);
                        }
                        messageDialog.show("Rule type - " + _currentRule.Type + " can not be assigned to Section/Repeater.");
                        result = false;
                    }
                }
                return result;
            },
            onSelectRow: function (id, isSelected) {
                var p = this.p, item = p.data[p._index[id]], i = $.inArray(id, _idsOfSelectedRows);
                item.Select = isSelected;
                if (!isSelected && i >= 0) {
                    _idsOfSelectedRows.splice(i, 1); // remove id from the list
                } else if (i < 0) {
                    _idsOfSelectedRows.push(id);
                }
            },
            loadComplete: function () {
                var p = this.p, data = p.data, item, $this = $(this), index = p._index, rowid, i, selCount;
                for (i = 0, selCount = _idsOfSelectedRows.length; i < selCount; i++) {
                    rowid = _idsOfSelectedRows[i];
                    item = data[index[rowid]];
                    var ifMappingExists = mapping.find((o) => { return o["UIElementID"] === parseInt(rowid) })
                    if (item.Select && ifMappingExists == undefined) {
                        $this.jqGrid('setSelection', rowid, false);
                    }
                }

                for (var j = 0; j < mapping.length; j++) {
                    $this.jqGrid('setSelection', mapping[j].UIElementID, false);
                }
            }
        });
        var pagerElement = '#p' + elementIDs.assignTargetGrid;
        $(elementIDs.assignTargetGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.assignTargetGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        if (isEditable) {
            $(elementIDs.assignTargetGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon ui-icon-disk', title: 'Assign', id: 'btnAssign', caption: 'Assign',
                    onClickButton: function () {
                        _saveTargetElements();
                    }
                });
        }
    }

    function _saveTargetElements() {
        var elements = $(elementIDs.assignTargetGridJQ).jqGrid('getGridParam', 'selarrrow');

        if (!_currentRule) {
            messageDialog.show('Please select a rule to assign target(s).');
            return false;
        }

        if (elements.length <= 0) {
            messageDialog.show('Please select an element to assign as target');
            return false;
        }

        var elementMap = [];
        for (var idx = 0; idx < elements.length; idx++) {
            var obj = { RuleID: _currentRule.RuleID };
            var elementData = $(elementIDs.assignTargetGridJQ).jqGrid('getRowData', elements[idx]);
            obj.UIElementID = elementData.UIElementID;
            if (elementData.TargetKeyFilter != '') {
                obj.TargetKeyFilter = JSON.parse(elementData.TargetKeyFilter);
            }
            elementMap.push(obj);
        }

        var dataStr = {
            targetMap: JSON.stringify(elementMap),
        };

        var formDesignVersionId = $(elementIDs.formDesignVersionJQ).val();
        var promise = ajaxWrapper.postJSON(URLs.assignTargets.replace(/\{uiElementId\}/g, _tabElementId).replace(/\{formDesignVersionID\}/g, formDesignVersionId), dataStr);
        promise.done(function (xhr) {
            if (xhr.Result == ServiceResult.SUCCESS) {
                messageDialog.show('Target assigned successfully.');
                //$(elementIDs.assignTargetGridJQ).trigger('reloadGrid');
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        });
        promise.fail(_showError);

    }

    function _imageformat(cellValue, options, rowObject) {
        if (rowObject.UIElementName.indexOf('Repeater') >= 0 && (rowObject.IsContainer == false || rowObject.IsContainer == "false")) {
            return "<span class='ui-icon ui-icon-key' style='cursor:pointer'></span>";
        }
        return "";
    }

    function _showError(xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else
            alert(JSON.stringify(xhr));
    }

    return {
        show: function (data, tabElementId, isEditable, mapping) {
            _tabElementId = tabElementId;
            elements = data.filter(function (obj) {
                return obj.Parent !== null;
            });
            $.each(elements, function (ind, obj) {
                obj.Select = false;
                $.each(mapping, function (index, map) {
                    if (obj.UIElementID == map.UIElementID) {
                        obj.Select = true;
                    }
                })
            });
            _buildTargetGrid(elements, isEditable, mapping);
        },
        setCurrentRule: function (rule) {
            _setCurrentRule(rule);
        }
    };
}());
