var rulesmanager = (function () {
    var _formDesignId;
    var _formDesignVersionId;
    var _designVersions;
    var _tabElement;
    var _tabs;
    var _viewType = 'target';
    var _elementList;
    var _currentRuleId;
    var _ruleData;
    var _readOnlyRuleData;
    var _expressionControl = null;
    var _initialized = false;
    var _isEditable = false;

    var elementIDs = {
        formDesignJQ: '#drpFormDesign',
        formDesignVersionJQ: '#drpFormDesignVersion',
        ruleListGrid: 'rulemasterlist',
        ruleListGridJQ: '#rulemasterlist',
        ruleListContainerJQ: '#ruleListContainer',
        ruleManagerTabs: '#rulemanagertabs',
        drpViewTypeJQ: '#drpViewType',
        expressionBuilder: 'expressionbuilder',
        expressionBuilderJQ: '#expressionbuilder',
        ruleTypeJQ: '#drpRuleType',
        ruleNameJQ: '#txtRuleName',
        resultSuccessJQ: '#txtExpTrue',
        resultSuccessValueJQ: '#txtExpTrueValue',
        resultFailureJQ: '#txtExpFalse',
        resultFailureValueJQ: '#txtExpFalseValue',
        messageJQ: '#txtValMessage',
        runOnLoadJQ: '#chkRunOnLoad',
        isStandard: '#chkIsStandard',
        btnSaveJQ: '#btnSaveRule',
        btnClearJQ: '#btnClearRule',
        btnSearchSuccessJQ: '#btnSearchSuccess',
        btnSearchFailureJQ: '#btnSearchFailure',
        compileContainerJQ: '#divCompile',
        btnCompileJQ: '#btnCompile',
        copyDialogJQ: '#copyruledialog',
        btnCopyJQ: '#btnCopy',
        copyRuleNameJQ: '#chkRuleName',
        copyDescriptionJQ: '#chkDescription',
        copyRuleTypeJQ: '#chkRuleType',
        copySourcesJQ: '#chkSources',
        //copyTargetsJQ: '#chkTargets'
        assignTargetGridJQ: '#grdtargetelement',
        rulesTesterGridJQ: '#rulestestergrid',
    };

    var URLs = {
        getDesigns: '/RulesManager/GetFormDesignList?tenantId=1',
        getDesignVersions: '/RulesManager/GetFormDesignVersionList?tenantId=1&formDesignId={formDesignId}',
        getRules: '/RulesManager/GetRuleList?tenantId=1&formDesignVersionId={formDesignVersionId}',
        getRulesByTarget: '/RulesManager/GetRuleListByTarget?tenantId=1&formDesignVersionId={formDesignVersionId}',
        getRulesBySource: '/RulesManager/GetRuleListBySource?tenantId=1&formDesignVersionId={formDesignVersionId}',
        formDesignUIElementList: '/RulesManager/GetOperandList?tenantId=1&formDesignVersionId={formDesignVersionId}',
        getRuleById: '/RulesManager/GetRule?tenantId=1&formDesignVersionId={formDesignVersionId}&ruleId={ruleId}',
        getTargetsById: '/RulesManager/GetTargetsByRuleID?tenantId=1&formDesignVersionId={formDesignVersionId}&ruleId={ruleId}',
        saveRule: '/RulesManager/SaveRule?tenantId=1&formDesignVersionId={formDesignVersionId}',
        deleteRule: '/RulesManager/DeleteRule?tenantId=1&formDesignVersionId={formDesignVersionId}&ruleId={ruleId}',
        compileDesignVersion: '/FormDesign/CompileFormDesignVersion',
        download: '/RulesManager/Download'
    };


    function _fillFormDesignDropdown() {
        var promise = ajaxWrapper.getJSON(URLs.getDesigns);
        promise.done(function (xhr) {
            var $prevGroup, prevGroupName;
            $.each(xhr, function () {
                if (prevGroupName != this.Group) {
                    $prevGroup = $('<optgroup />').prop('label', this.Group).appendTo($(elementIDs.formDesignJQ));
                }
                $("<option />").val(this.FormID).text(this.FormName).appendTo($prevGroup);
                prevGroupName = this.Group;
            });
        });
        promise.fail(_showError);

        //register event - load design version
        $(elementIDs.formDesignJQ).on('change', function () {
            _cleanUp();
            _formDesignId = this.value;
            _fillFormDesignVersionDropdown(this.value);
        });
    }

    function _fillFormDesignVersionDropdown(formDesignId) {
        var promise = ajaxWrapper.getJSON(URLs.getDesignVersions.replace(/\{formDesignId\}/g, formDesignId));
        promise.done(function (xhr) {
            _designVersions = xhr;
            $(elementIDs.formDesignVersionJQ).empty();
            $("<option />").val('0').text('--Select--').appendTo($(elementIDs.formDesignVersionJQ));
            $.each(xhr, function () {
                $("<option />").val(this.FormID).text(this.VersionNumber).appendTo($(elementIDs.formDesignVersionJQ));
            });

            //register event - load design version
            $(elementIDs.formDesignVersionJQ).off('change').on('change', function () {
                _formDesignVersionId = this.value;
                _cleanUpTabs();
                if (_formDesignVersionId == '0') {
                    $(elementIDs.drpViewTypeJQ).prop("disabled", true);
                    $(elementIDs.ruleListGridJQ).jqGrid('GridUnload');
                    $(elementIDs.ruleListContainerJQ).hide();
                }
                else {
                    $(elementIDs.ruleListContainerJQ).show();
                    $(elementIDs.drpViewTypeJQ).prop("disabled", false);
                    _handleReleasedVersion(this.selectedOptions[0].text);
                    _registerEvents();
                    _getElementList(_formDesignVersionId);
                    _loadRulesGrid(_formDesignVersionId);
                }
            });
        });
        promise.fail(_showError);
    }

    function _handleReleasedVersion(version) {
        _isEditable = false;
        $(elementIDs.btnSaveJQ).hide();
        $(elementIDs.btnClearJQ).hide();

        var selVer = _designVersions.filter(function (obj) {
            return obj.FormID == _formDesignVersionId
        });

        if (selVer.length > 0) {
            if (selVer[0].Status == 1) {
                $(elementIDs.btnSaveJQ).show();
                $(elementIDs.btnClearJQ).show();
                _isEditable = true;
            }
        }

        //if ((version - Math.floor(version)) != 0) {
        //    $(elementIDs.btnSaveJQ).show();
        //    $(elementIDs.btnClearJQ).show();
        //    _isEditable = true;
        //}
    }

    function _cleanUp() {
        $(elementIDs.ruleListContainerJQ).hide();
        $(elementIDs.ruleListGridJQ).jqGrid('GridUnload');
        _cleanUpTabs();
        if ($(elementIDs.ruleManagerTabs).hasClass('collapse') == false) {
            $(elementIDs.ruleManagerTabs).collapse('hide');
        }
    }

    function _cleanUpTabs() {
        if (_elementList != null && _elementList != undefined) {
            _clear();
        }
        $(elementIDs.assignTargetGridJQ).jqGrid('GridUnload');
        if (rulescomparer != null && rulescomparer != undefined) {
            rulescomparer.cleanUp();
        }
        $(elementIDs.rulesTesterGridJQ).jqGrid('GridUnload');
        if (analyzer != null && analyzer != undefined) {
            analyzer.cleanUp();
        }
    }

    function _registerEvents() {
        //register view type dropdown events
        $(elementIDs.drpViewTypeJQ).off('change').on('change', function () {
            _viewType = this.value;
            _loadRulesGrid(_formDesignVersionId);
        });

        $(elementIDs.ruleTypeJQ).off('change').on('change', function () {
            $(elementIDs.resultSuccessJQ).attr('disabled', 'disabled');
            $(elementIDs.btnSearchSuccessJQ).attr('disabled', 'disabled');
            $(elementIDs.resultFailureJQ).attr('disabled', 'disabled');
            $(elementIDs.btnSearchFailureJQ).attr('disabled', 'disabled');
            $(elementIDs.messageJQ).attr('disabled', 'disabled');
            $(elementIDs.messageJQ).val("");
            $(elementIDs.resultSuccessJQ).val("");
            $(elementIDs.resultFailureJQ).val("");

            if (this.value == '3') {
                $(elementIDs.resultSuccessJQ).removeAttr('disabled');
                $(elementIDs.resultFailureJQ).removeAttr('disabled');
                $(elementIDs.btnSearchFailureJQ).removeAttr('disabled');
                $(elementIDs.btnSearchSuccessJQ).removeAttr('disabled');
            }

            if (this.value == '6') {
                $(elementIDs.messageJQ).removeAttr('disabled');
            }
        });

        $(elementIDs.btnSaveJQ).off('click').on('click', function () {
            _saveRule();
        });

        $(elementIDs.btnCopyJQ).off('click').on('click', function () {
            _copyRule();
        });

        $(elementIDs.btnClearJQ).off('click').on('click', function () {
            _clear();
        });

        $(elementIDs.btnCompileJQ).off('click').on('click', function () {
            _compileDesignVersion();
        });

        $(elementIDs.btnSearchSuccessJQ).off('click').on('click', function () {
            var searchGridData = _elementList.filter(function (obj) {
                return obj.IsContainer == false;
            });
            var searchDialog = new expressionBuilderSearchDialog(searchGridData, $(elementIDs.resultSuccessValueJQ), $(elementIDs.resultSuccessJQ), false, null);
            searchDialog.show();
        });

        $(elementIDs.btnSearchFailureJQ).off('click').on('click', function () {
            var searchGridData = _elementList.filter(function (obj) {
                return obj.IsContainer == false;
            });
            var searchDialog = new expressionBuilderSearchDialog(searchGridData, $(elementIDs.resultFailureValueJQ), $(elementIDs.resultFailureJQ), false, null);
            searchDialog.show();
        });
    }

    function _saveRule() {
        _buildRuleData();

        if (_validateRule() == false) {
            messageDialog.show('Please resolve errors to save rule.');
            return false;
        }

        //if (_isRuleUpdated() == false) {
        //    messageDialog.show('There is no change in the rule to save.');
        //    return false;
        //}

        var dataStr = {
            model: JSON.stringify(_ruleData)
        };
        var promise = ajaxWrapper.postJSON(URLs.saveRule.replace(/\{formDesignVersionId\}/g, _formDesignVersionId), dataStr);
        promise.done(function (xhr) {
            if (xhr.Result == ServiceResult.SUCCESS) {
                messageDialog.show('Rule saved successfully.');
                $(elementIDs.ruleListGridJQ).trigger('reloadGrid');
                _clear();
            }
            else {
                messageDialog.show(Common.errorMsg);
            }
        });
        promise.fail(_showError);
    }

    function _deleteRule(result) {
        if (result) {
            var selrow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
            if (!selrow) {
                messageDialog.show('Please select a row to delete.');
                return false;
            }
            var rules = [];
            var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selrow);
            rules.push(rowData.RuleID);

            var dataStr = { ruleIds: JSON.stringify(rules) };
            var promise = ajaxWrapper.postJSON(URLs.deleteRule.replace(/\{formDesignVersionId\}/g, _formDesignVersionId), dataStr);
            promise.done(function (xhr) {
                if (xhr.Result == ServiceResult.SUCCESS) {
                    messageDialog.show('Rule deleted successfully.');
                    $(elementIDs.ruleListGridJQ).trigger('reloadGrid');
                    _clear();
                }
                else {
                    messageDialog.show(Common.errorMsg);
                }
            });
            promise.fail(_showError);
        }
        yesNoConfirmDialog.hide();
    }

    function _buildRuleData() {
        _expressionControl.saveRule();
        _ruleData.FormDesignVersionId = _formDesignVersionId;
        _ruleData.TargetPropertyId = $(elementIDs.ruleTypeJQ).val();
        _ruleData.RuleName = $(elementIDs.ruleNameJQ).val();
        _ruleData.Message = $(elementIDs.messageJQ).val();
        _ruleData.ResultSuccess = $(elementIDs.resultSuccessValueJQ).val() == '' ? $(elementIDs.resultSuccessJQ).val() : $(elementIDs.resultSuccessValueJQ).val();
        _ruleData.IsResultSuccessElement = $(elementIDs.resultSuccessValueJQ).val() == '' ? false : true;
        _ruleData.ResultFailure = $(elementIDs.resultFailureValueJQ).val() == '' ? $(elementIDs.resultFailureJQ).val() : $(elementIDs.resultFailureValueJQ).val();
        _ruleData.IsResultFailureElement = $(elementIDs.resultFailureValueJQ).val() == '' ? false : true;
        _ruleData.RunOnLoad = $(elementIDs.runOnLoadJQ).prop('checked');
        _ruleData.IsStandard = $(elementIDs.isStandard).prop('checked');
        _ruleData.UIElementID = _tabElement;
    }

    function _clear() {
        _currentRuleId = -1;
        var rule = {
            RuleId: _currentRuleId,
            PropertyRuleMapID: 0,
            TargetPropertyId: 0,
            ResultSuccess: '',
            ResultFailure: '',
            IsResultSuccessElement: '',
            IsResultFailureElement: '',
            Message: '',
            RunOnLoad: false,
            FormDesignVersionId: ''
        };
        _ruleData = rule;
        _readOnlyRuleData = undefined;
        $(elementIDs.ruleTypeJQ).val('0');
        $(elementIDs.ruleTypeJQ).parent('div').removeClass('has-error');
        $(elementIDs.ruleNameJQ).val('');
        $(elementIDs.runOnLoadJQ).prop('checked', false);
        $(elementIDs.isStandard).prop('checked', false);
        $(elementIDs.resultSuccessJQ).val('');
        $(elementIDs.resultSuccessJQ).attr('disabled', 'disabled');
        $(elementIDs.resultSuccessValueJQ).val('');
        $(elementIDs.resultFailureJQ).val('');
        $(elementIDs.resultFailureJQ).attr('disabled', 'disabled');
        $(elementIDs.resultFailureValueJQ).val('');
        $(elementIDs.messageJQ).val('');
        $(elementIDs.messageJQ).attr('disabled', 'disabled');
        _renderExpressionBuilder(_ruleData);
    }

    function _showCopyDialog() {
        if (_initialized == false) {
            $(elementIDs.copyDialogJQ).dialog({
                autoOpen: false,
                height: '200',
                width: '25%',
                modal: true,
                position: { my: 'center', at: 'center' }
            });
            _initialized = true;
        }
        $(elementIDs.copyDialogJQ).dialog('option', 'title', 'Copy Rule');
        $(elementIDs.copyDialogJQ).dialog('open');
        $(elementIDs.copyRuleNameJQ).prop('checked', true);
        $(elementIDs.copyDescriptionJQ).prop('checked', true);
        $(elementIDs.copyRuleTypeJQ).prop('checked', true);
        $(elementIDs.copySourcesJQ).prop('checked', true);
    }

    function _copyRule() {
        var selrow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
        var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selrow);
        var copyName = $(elementIDs.copyRuleNameJQ).prop('checked');
        var copyDesc = $(elementIDs.copyDescriptionJQ).prop('checked');
        var copyType = $(elementIDs.copyRuleTypeJQ).prop('checked');
        var copySources = $(elementIDs.copySourcesJQ).prop('checked');
        //var copyTargets = $(elementIDs.copyTargetsJQ).prop('checked');

        var promise = ajaxWrapper.getJSON(URLs.getRuleById.replace(/\{formDesignVersionId\}/g, _formDesignVersionId).replace(/\{ruleId\}/g, rowData.RuleID));

        promise.done(function (xhr) {
            _currentRuleId = -1;
            _ruleData = xhr;
            xhr.RuleId = -1;
            if (copyName == false) { xhr.RuleName = ''; }
            if (copyDesc == false) { xhr.RuleDescription = ''; }
            if (copyType == false) { xhr.TargetPropertyId = '0' }
            if (copySources == false) {
                xhr.RootExpression = null;
            }
            _renderExpressionBuilder(xhr);
            $(elementIDs.copyRuleNameJQ).prop('checked', false);
            $(elementIDs.copyDescriptionJQ).prop('checked', false);
            $(elementIDs.copyRuleTypeJQ).prop('checked', false);
            $(elementIDs.copySourcesJQ).prop('checked', false);
            //$(elementIDs.copyTargetsJQ).prop('checked', false);
            $(elementIDs.copyDialogJQ).dialog('close');
        });

        promise.fail(_showError);
    }

    function _compileDesignVersion() {
        var url = URLs.compileDesignVersion;
        var data = { tenantId: 1, formDesignVersionId: _formDesignVersionId };
        var promise = ajaxWrapper.postJSON(url, data);

        promise.done(function (xhr) {
            if (xhr.Result === ServiceResult.SUCCESS) {
                messageDialog.show(DocumentDesign.documentCompileMsg);
            }
            else {
                messageDialog.show(DocumentDesign.compileFailureMsg);
            }
        });

        promise.fail(_showError);
    }

    function _renderExpressionBuilder(objRule) {
        var filteredRule = {};
        if (objRule == null) {
            $.extend(filteredRule, -1);
        }
        else {
            filteredRule = objRule;
            $(elementIDs.ruleTypeJQ).val(objRule.TargetPropertyId);
            $(elementIDs.ruleNameJQ).val(objRule.RuleName);
            $(elementIDs.runOnLoadJQ).prop('checked', objRule.RunOnLoad);
            $(elementIDs.isStandard).prop('checked', objRule.IsStandard);
            var elem = _elementList.filter(function (el) {
                return el.UIElementName == objRule.ResultSuccess;
            });
            if (elem != null && elem.length > 0) {
                $(elementIDs.resultSuccessJQ).val(elem[0].Element);
                $(elementIDs.resultSuccessValueJQ).val(objRule.ResultSuccess);
            }
            else {
                $(elementIDs.resultSuccessJQ).val(objRule.ResultSuccess);
            }
            $(elementIDs.resultSuccessJQ).attr('disabled', 'disabled');
            $(elementIDs.btnSearchSuccessJQ).attr('disabled', 'disabled');
            elem = _elementList.filter(function (el) {
                return el.UIElementName == objRule.ResultFailure;
            });
            if (elem != null && elem.length > 0) {
                $(elementIDs.resultFailureJQ).val(elem[0].Element);
                $(elementIDs.resultFailureValueJQ).val(objRule.ResultFailure);
            }
            else {
                $(elementIDs.resultFailureJQ).val(objRule.ResultFailure);
            }
            $(elementIDs.resultFailureJQ).val(objRule.ResultFailure);
            $(elementIDs.resultFailureJQ).attr('disabled', 'disabled');
            $(elementIDs.btnSearchFailureJQ).attr('disabled', 'disabled');
            $(elementIDs.messageJQ).val(objRule.Message);
            $(elementIDs.messageJQ).attr('disabled', 'disabled');
            if (objRule.TargetPropertyId == '3') {
                $(elementIDs.resultSuccessJQ).removeAttr('disabled');
                $(elementIDs.btnSearchSuccessJQ).removeAttr('disabled');
                $(elementIDs.resultFailureJQ).removeAttr('disabled');
                $(elementIDs.btnSearchFailureJQ).removeAttr('disabled');
            }

            if (objRule.TargetPropertyId == '6') {
                $(elementIDs.messageJQ).removeAttr('disabled')
            }
        }

        var searchGridData = _elementList.filter(function (obj) {
            return obj.IsContainer == false;
        });
        //TODO: remove container elements before sending to expressioncontrol     
        if (filteredRule.hasOwnProperty('RootExpression')) {
            _expressionControl = new expressionBuilder(filteredRule, elementIDs.expressionBuilder, searchGridData, _getLogicalOperators(), _getOperatorTypes());
        }
        else {
            filteredRule.RootExpression = null;
            _expressionControl = new expressionBuilder(filteredRule, elementIDs.expressionBuilder, searchGridData, _getLogicalOperators(), _getOperatorTypes());
        }
        _expressionControl.setCurrent();
        _expressionControl.render();
    }

    function _isRuleUpdated() {
        var isUpdated = false;
        var strOriginal = JSON.stringify(_readOnlyRuleData);
        var strModified = JSON.stringify(_ruleData);
        if (strOriginal != strModified || _readOnlyRuleData == undefined) {
            isUpdated = true;
        }
        return isUpdated;
    }

    function _validateRule() {
        var isValid = true;
        $(elementIDs.ruleTypeJQ).parent('div').removeClass('has-error');
        if ($(elementIDs.ruleTypeJQ).val() == '0') {
            $(elementIDs.ruleTypeJQ).parent('div').addClass('has-error');
            isValid = false;
        }

        $(elementIDs.ruleNameJQ).parent('div').removeClass('has-error');
        if ($(elementIDs.ruleNameJQ).val() == '') {
            $(elementIDs.ruleNameJQ).parent('div').addClass('has-error');
            isValid = false;
        }

        $(elementIDs.expressionBuilderJQ + ' #expressionbuilder_group_0').removeClass('custom-error');
        if (_ruleData.RootExpression.Expressions == null) {
            $(elementIDs.expressionBuilderJQ + ' #expressionbuilder_group_0').addClass('custom-error');
            isValid = false;
        }

        return isValid;
    }

    function _getElementList(designVersionId) {
        var promise = ajaxWrapper.getJSON(URLs.formDesignUIElementList.replace(/\{formDesignVersionId\}/g, designVersionId));
        promise.done(function (xhr) {
            if (xhr.length > 0) {
                _elementList = xhr;
                _tabElement = xhr[0].UIElementID;
                var rule = {
                    RuleId: _currentRuleId,
                    PropertyRuleMapID: 0,
                    TargetPropertyId: 0,
                    ResultSuccess: '',
                    ResultFailure: '',
                    IsResultSuccessElement: '',
                    IsResultFailureElement: '',
                    Message: '',
                    RunOnLoad: false
                };
                _ruleData = rule;
                _renderExpressionBuilder(rule);
            }
        });
        promise.fail(_showError);
    }

    function _showError(xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else
            alert(JSON.stringify(xhr));
    }

    //TODO : Make it an ajax call to get the list of operators.
    function _getOperatorTypes() {
        var operators = [];
        operators.push({ 'OperatorTypeID': '1', 'DisplayText': 'Equals ( = )' });
        operators.push({ 'OperatorTypeID': '2', 'DisplayText': 'Greater Than ( > )' });
        operators.push({ 'OperatorTypeID': '3', 'DisplayText': 'Less Than ( < )' });
        operators.push({ 'OperatorTypeID': '4', 'DisplayText': 'Contains' });
        operators.push({ 'OperatorTypeID': '5', 'DisplayText': 'Not Equals ( ! )' });
        operators.push({ 'OperatorTypeID': '6', 'DisplayText': 'Greater Than OR Equal To ( >=)' });
        operators.push({ 'OperatorTypeID': '7', 'DisplayText': 'Less Than OR Equal To ( <=)' });
        operators.push({ 'OperatorTypeID': '8', 'DisplayText': 'Is NULL' });
        operators.push({ 'OperatorTypeID': '9', 'DisplayText': 'Custom' });
        operators.push({ 'OperatorTypeID': '10', 'DisplayText': 'Not Contains' });
        return operators;
    }

    //TODO : Make it an ajax call to get the list of logical operators.
    function _getLogicalOperators() {
        var logicalOperators = [];
        logicalOperators.push({ 'LogicalOperatorTypeID': '1', 'LogicalOperatorTypeCode': 'AND' });
        logicalOperators.push({ 'LogicalOperatorTypeID': '2', 'LogicalOperatorTypeCode': 'OR' });
        return logicalOperators;
    }

    function _getColumnNames(type) {
        var columnNames = ['Section', 'Rule ID', 'RuleCode', 'Rule Name', 'Rule Description', 'Rule Type', 'Source(s)', 'Source Keys', 'Target(s)', 'Target Keys'];

        if (type == 'source') {
            columnNames = ['Source Section', 'Source Element', 'Source Keys', 'Rule ID', 'RuleCode', 'Rule Name', 'Rule Description', 'Rule Type', 'Target(s)', 'Target Keys'];
        }

        if (type == 'target') {
            columnNames = ['Target Section', 'Target Element', 'Target Keys', 'Rule ID', 'RuleCode', 'Rule Name', 'Rule Description', 'Rule Type', 'Source(s)', 'Source Keys'];
        }

        return columnNames;
    }

    function _getColumnList(type) {
        var columns = [];
        columns.push({ name: 'Section', index: 'Section', editable: false, hidden: true, align: 'left', width: 200 });
        //columns.push({ name: 'RuleID', index: 'RuleID', editable: false, key: true, hidden: true, align: 'center', width: 80 });
        columns.push({ name: 'RuleID', index: 'RuleID', editable: false, hidden: true, align: 'center', width: 80 });
        columns.push({ name: 'RuleCode', index: 'RuleCode', editable: false, hidden: true, align: 'left', width: 60 });
        columns.push({ name: 'RuleName', index: 'RuleName', editable: false, align: 'left', width: 100 });
        columns.push({ name: 'Description', index: 'Description', editable: false, align: 'left' });
        columns.push({ name: 'Type', index: 'Type', editable: false, align: 'left', width: 60 });
        if (type == 'source') {
            columns.splice(1, 0, { name: 'SourceElement', index: 'SourceElement', editable: false, align: 'left', width: 150 });
            columns.splice(2, 0, { name: 'KeyFilter', index: 'KeyFilter', editable: false, align: 'left', width: 150 });
            columns[0].hidden = false;
        }
        else {
            columns.push({ name: 'SourceElement', index: 'SourceElement', editable: false, align: 'center', sortable: false, width: 60, formatter: _imageformat });
            columns.push({ name: 'KeyFilter', index: 'KeyFilter', editable: false, hidden: true, align: 'left', width: 200 });
        }

        if (type == 'target') {
            columns[0].hidden = false;
            columns.splice(1, 0, { name: 'TargetElement', index: 'TargetElement', editable: false, sortable: false, align: 'left', width: 150 });
            columns.splice(2, 0, { name: 'KeyFilter', index: 'KeyFilter', editable: false, align: 'left', width: 150 });
        }
        else {
            columns.push({ name: 'TargetElement', index: 'TargetElement', editable: false, align: 'center', sortable: false, width: 60, formatter: _imageformat });
            columns.push({ name: 'KeyFilter', index: 'KeyFilter', editable: false, hidden: true, align: 'left', width: 200 });
        }
        return columns;
    }

    function _imageformat(cellValue, options, rowObject) {
        return "<span class='ui-icon ui-icon-document' style='width:100px;cursor:pointer'></span>";
    }

    function _showSourceTargetElement(ruleId, type) {
        var elementDialog = new sourceTargetElementDialog(ruleId, _formDesignVersionId, type);
        elementDialog.show();
    }

    function _getSortByColumn() {
        var sortBy = "RuleID";
        if (_viewType == 'target' || _viewType == 'source') {
            sortBy = "Section";
        }
        return sortBy;
    }

    function _loadRulesGrid(designVersionId) {
        var postUrl = URLs.getRules.replace(/\{formDesignVersionId\}/g, designVersionId);
        if (_viewType == 'target') {
            postUrl = URLs.getRulesByTarget.replace(/\{formDesignVersionId\}/g, designVersionId);
        }

        if (_viewType == 'source') {
            postUrl = URLs.getRulesBySource.replace(/\{formDesignVersionId\}/g, designVersionId);
        }

        var columnNames = _getColumnNames(_viewType);
        var columns = _getColumnList(_viewType);

        $(elementIDs.ruleListGridJQ).jqGrid('GridUnload');
        //$(elementIDs.ruleListGridJQ).parent().append("<div id='p" + elementIDs.ruleListGrid + "'></div>");

        $(elementIDs.ruleListGridJQ).jqGrid({
            url: postUrl,
            datatype: 'json',
            cache: false,
            colNames: columnNames,
            colModel: columns,
            caption: 'Rule Master List',
            height: '400',
            rowNum: 20,
            rowList: [20, 40, 60, 80, 100],
            ignoreCase: true,
            autowidth: false,
            shrinkToFit: false,
            viewrecords: true,
            rownumbers: true,
            //multiselect: true,
            hidegrid: false,
            altRows: true,
            pager: '#p' + elementIDs.ruleListGrid,
            sortname: _getSortByColumn(),
            sortorder: _viewType == 'rule' ? 'desc' : 'asc',
            altclass: 'alternate',
            ondblClickRow: function (rowid, iRow, iCol, e) {
                $(elementIDs.ruleManagerTabs).collapse('show');
                var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', rowid);
                _currentRuleId = rowData.RuleID;
                _getRule(rowData.RuleID);
                $(elementIDs.ruleManagerTabs).tabs("option", "active", 0);
                $(elementIDs.ruleManagerTabs).collapse('show');
            },
            onSelectRow: function (id, isSelected) {
                var actTab = $(elementIDs.ruleManagerTabs).tabs('option', 'active');
                if (actTab == 1) {
                    var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', id);
                    assigntarget.setCurrentRule(rowData);
                    _getTargets(rowData.RuleID);
                }
            },
            beforeSelectRow: function (rowid, e) {
                var result = true;
                var $self = $(this), $td = $(e.target).closest("td"), iCol = $.jgrid.getCellIndex($td[0]);
                var cm = $self.jqGrid("getGridParam", "colModel");
                if (cm[iCol].name == 'SourceElement' && _viewType != 'source') {
                    var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', rowid);
                    _showSourceTargetElement(rowData.RuleID, 'Source');
                    result = false;
                }

                if (cm[iCol].name == 'TargetElement' && _viewType != 'target') {
                    var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', rowid);
                    _showSourceTargetElement(rowData.RuleID, 'Target');
                    result = false;
                }
                return result;
            }
        });

        var pagerElement = '#p' + elementIDs.ruleListGrid;
        $(elementIDs.ruleListGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
        $(elementIDs.ruleListGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
        if (_isEditable) {
            $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon ui-icon-plus', title: 'Add', id: 'btnRuleAdd',
                    onClickButton: function () {
                        $(elementIDs.ruleListGridJQ).resetSelection()
                        $(elementIDs.ruleManagerTabs).tabs("option", "active", 0);
                        $(elementIDs.ruleManagerTabs).collapse('show');
                        if (_expressionControl) {
                            _expressionControl.close();
                        }
                        var rowId = -1;
                        var rule = {
                            RuleId: rowId,
                            PropertyRuleMapID: 0,
                            TargetPropertyId: 0,
                            ResultSuccess: '',
                            ResultFailure: '',
                            IsResultSuccessElement: '',
                            IsResultFailureElement: '',
                            Message: '',
                            RunOnLoad: false,
                            FormDesignVersionId: ''
                        };
                        _ruleData = rule;
                        _renderExpressionBuilder(_ruleData);
                    }
                });
        }
        $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon ui-icon-pencil', title: 'Edit', id: 'btnRuleEdit',
                onClickButton: function () {
                    var selrow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                    if (!selrow) {
                        messageDialog.show('Please select a row to edit.');
                        return false;
                    }
                    var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selrow);
                    $(elementIDs.ruleManagerTabs).tabs("option", "active", 0);
                    $(elementIDs.ruleManagerTabs).collapse('show');
                    _currentRuleId = rowData.RuleID;
                    _getRule(rowData.RuleID);
                }
            });

        if (_isEditable) {
            $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon ui-icon-copy', title: 'Copy', id: 'btnRuleCopy',
                    onClickButton: function () {
                        var selrow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                        if (!selrow) {
                            messageDialog.show('Please select a row to copy.');
                            return false;
                        }
                        $(elementIDs.ruleManagerTabs).tabs("option", "active", 0);
                        $(elementIDs.ruleManagerTabs).collapse('show');
                        _showCopyDialog();
                    }
                });
        }

        if (_isEditable) {
            $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon ui-icon-trash', title: 'Delete', id: 'btnRuleDelete',
                    onClickButton: function () {
                        var selrow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                        if (!selrow) {
                            messageDialog.show('Please select a row to delete.');
                            return false;
                        }
                        yesNoConfirmDialog.show('Are you sure you want to delete this rule?', _deleteRule);
                    }
                });
        }

        if (_isEditable) {
            $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
                {
                    caption: '', buttonicon: 'ui-icon ui-icon-flag', title: 'Assign Targets', id: 'btnAssignTargets',
                    onClickButton: function () {
                        var selrow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                        if (!selrow) {
                            messageDialog.show('Please select a row to assign target(s).');
                        } else {
                            var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selrow);
                            $(elementIDs.ruleManagerTabs).tabs("option", "active", 1);
                            $(elementIDs.ruleManagerTabs).collapse('show');
                            assigntarget.setCurrentRule(rowData);
                            _getTargets(rowData.RuleID);
                        }
                    }
                });
        }


        $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon ui-icon-transferthick-e-w', title: 'Compare', id: 'btnRuleCompare',
                onClickButton: function () {
                    var selRow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                    if (!selRow) {
                        messageDialog.show('Please select a row to compare.');
                        return false;
                    }
                    $(elementIDs.ruleManagerTabs).tabs("option", "active", 2);
                    $(elementIDs.ruleManagerTabs).collapse('show');
                    var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selRow);
                    rulescomparer.compare(rowData);
                }
            });

        $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon ui-icon-check', title: 'Rule Tester', id: 'btnRuleTester',
                onClickButton: function () {
                    var selRow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                    var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selRow);
                    if (!selRow) {
                        messageDialog.show('Please select a row to test.');
                        return false;
                    }
                    $(elementIDs.ruleManagerTabs).tabs("option", "active", 3);
                    $(elementIDs.ruleManagerTabs).collapse('show');
                    _currentRuleId = rowData.RuleID;
                    _getRule(rowData.RuleID);
                    //ruletest.process();
                }
            });

        $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon ui-icon-search', title: 'Rule Analyzer', id: 'btnRuleAnalyzer',
                onClickButton: function () {
                    var selRow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                    if (!selRow) {
                        messageDialog.show('Please select a row to test.');
                    } else {
                        $(elementIDs.ruleManagerTabs).tabs("option", "active", 4);
                        $(elementIDs.ruleManagerTabs).collapse('show');
                        var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selRow);
                        analyzer.show(rowData, _formDesignVersionId, _viewType);
                    }
                }
            });

        $(elementIDs.ruleListGridJQ).jqGrid('navButtonAdd', pagerElement,
            {
                caption: '', buttonicon: 'ui-icon ui-icon-arrowstop-1-s', title: 'Download', id: 'btnRuleDownload',
                onClickButton: function () {
                    var downloadURL = URLs.download;
                    var stringData = "tenantId=1";
                    stringData += "<&formDesignName=" + $(elementIDs.formDesignJQ).find('option:selected').text();
                    stringData += "<&formDesignVersionId=" + _formDesignVersionId;
                    stringData += "<&viewType=" + _viewType;
                    $.download(downloadURL, stringData, 'post');
                }
            });
    }

    function _getRule(ruleId) {
        var promise = ajaxWrapper.getJSON(URLs.getRuleById.replace(/\{formDesignVersionId\}/g, _formDesignVersionId).replace(/\{ruleId\}/g, ruleId));
        promise.done(function (xhr) {
            _readOnlyRuleData = _ruleData = xhr;
            _renderExpressionBuilder(xhr);
            var actTab = $(elementIDs.ruleManagerTabs).tabs('option', 'active');
            if (actTab == 3) {
                ruletest.process(_formDesignId, _formDesignVersionId, _elementList, _expressionControl);
            }
        });
        promise.fail(_showError);
    }

    function _getTargets(ruleId) {
        var promise = ajaxWrapper.getJSON(URLs.getTargetsById.replace(/\{formDesignVersionId\}/g, _formDesignVersionId).replace(/\{ruleId\}/g, ruleId));
        promise.done(function (xhr) {
            assigntarget.show(_elementList, _tabElement, _isEditable, xhr);
        });
        promise.fail(_showError);
    }

    function _buildTab() {
        _tabs = $(elementIDs.ruleManagerTabs).tabs({
            activate: function (event, ui) {
                if (ui.newPanel[0].id == 'tabdesigner') {
                    _handleReleasedVersion();
                }
                else {
                    $(elementIDs.btnSaveJQ).hide();
                    $(elementIDs.btnClearJQ).hide()
                }

                if (ui.newPanel[0].id == 'tabtargets') {
                    var selrow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                    if (selrow) {
                        var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selrow);
                        assigntarget.setCurrentRule(rowData);
                        _getTargets(rowData.RuleID);
                    }
                }

                if (ui.newPanel[0].id == 'tabcomparer') {
                    //var compareData = [];
                    //var selrow = $(elementIDs.ruleListGridJQ).jqGrid('getGridParam', 'selrow');
                    //if (selrow) {
                    //    var rowData = $(elementIDs.ruleListGridJQ).jqGrid('getRowData', selrow);
                    //    rulescomparer.compare(rowData);
                    //}

                }
            }
        });
    }

    return {
        init: function () {
            _fillFormDesignDropdown();
            _buildTab();
        }
    };
}());



var sourceTargetElementDialog = function (ruleId, formDesignVersionId, type) {
    this.type = type;
    this.ruleId = ruleId;
    this.formDesignVersionId = formDesignVersionId;
    this.elementDialogJQ = '#sourcetargetelementdialog';
    this.gridElementJQ = '#elementgrid';
    this.gridElement = 'elementgrid';
    this.initialized = false;
}

sourceTargetElementDialog.prototype.show = function () {
    if (this.initialized == false) {
        $(this.elementDialogJQ).dialog({
            autoOpen: false,
            height: '200',
            width: '50%',
            modal: true,
            position: { my: 'center', at: 'center' }
        });
        this.initialized = true;
    }
    $(this.elementDialogJQ).dialog('option', 'title', this.type + ' Elements');
    $(this.elementDialogJQ).dialog('open');
    this.loadGrid();
}


sourceTargetElementDialog.prototype.loadGrid = function () {
    var currentInstance = this;
    //set column list
    var colArray = ['Section', 'Element', 'Key Filter'];
    //set column models
    var colModel = [];
    colModel.push({ name: 'Section', index: 'Section', width: 200, align: 'left', editable: false });
    colModel.push({ name: 'Element', index: 'Element', align: 'left', editable: false });
    colModel.push({ name: 'KeyFilter', index: 'KeyFilter', align: 'left', editable: false });
    var currentInstance = this;
    var postUrl = '/RulesManager/GetSourcesByRule?tenantId=1&formDesignVersionId={formDesignVersionId}&ruleId={ruleId}';
    if (currentInstance.type == 'Target') {
        postUrl = '/RulesManager/GetTargetsByRule?tenantId=1&formDesignVersionId={formDesignVersionId}&ruleId={ruleId}';
    }
    postUrl = postUrl.replace(/\{formDesignVersionId\}/g, currentInstance.formDesignVersionId).replace(/\{ruleId\}/g, currentInstance.ruleId);
    //unload previous grid values
    $(this.gridElementJQ).jqGrid('GridUnload');
    $(this.gridElementJQ).jqGrid({
        url: postUrl,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        height: '250',
        //caption: 'Target Elements',
        rowList: [10, 20, 30],
        rowNumber: 10,
        autowidth: false,
        shrinkToFit: false,
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        ignoreCase: true,
        hidegrid: false,
        gridComplete: function () {
            $(currentInstance.elementDialogJQ).dialog({ position: { my: 'center', at: 'center' }, });
        }
    });
}

sourceTargetElementDialog.prototype.close = function () {
    $(this.containerElementJQ).dialog('close');
}