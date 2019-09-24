var keyBuilderDialog = function (tenantId, effectiveDate, gridRow) {
    this.tenantId = tenantId;
    this.effectiveDate = effectiveDate;
    this.gridRow = gridRow;
    this.aliases = null;
    this.rules = { "condition": "AND", rules: [] };
    this.containerElementJQ = '#aliasBuilder';
    this.saveKeyBuilder = "#saveKeyBuilder";
    this.isInit = false;
    this.preRenderMethodInst = this.preRenderMethods();


    this.URLs = {
        getDesignAliases: '/MasterList/GetDesignAliases?tenantId={tenantId}&effectiveDate={effectiveDate}',
        getQueryBuilderRules: '/MasterList/GetQueryBuilderRules?tenantId={tenantId}&key={key}',
        getKey: '/MasterList/GetKey'
    }
}

keyBuilderDialog.prototype.init = function () {
    var currentInstance = this;
    $("#aliasBuilder").dialog({
        autoOpen: false,
        width: '80%',
        height: 500,
        modal: true,
        beforeClose: function () {
            currentInstance.gridRow = null;
            return true;
        }
    });
};

keyBuilderDialog.prototype.show = function () {
    if (this.isInit == false) {
        this.init();
        this.isInit = true;
    }
    var currentInstance = this;
    $(this.containerElementJQ).dialog('option', 'title', 'Generate Key');
    $(this.containerElementJQ).dialog('open');
    var url = this.URLs.getDesignAliases.replace(/{tenantId}/g, this.tenantId).replace(/{effectiveDate}/g, this.effectiveDate);
    var promise = ajaxWrapper.getJSON(url);
    var urlKey = this.URLs.getQueryBuilderRules.replace(/{tenantId}/g, this.tenantId).replace(/{key}/g, encodeURIComponent(this.gridRow.data.Key));
    var promiseKey = ajaxWrapper.getJSON(urlKey);
    $.when(promise, promiseKey).then(currentInstance.preRenderMethodInst.preRender);
}

keyBuilderDialog.prototype.preRenderMethods = function () {
    var currentInstance = this;
    return {
        preRender: function (xhrPromise, xhrPromiseKey) {
            currentInstance.aliases = xhrPromise[0];
            currentInstance.rules = xhrPromiseKey[0];
            currentInstance.render();
        },
        saveKey: function () {
            if (currentInstance.gridRow != null) {
                var res = $("#keyBuilder").queryBuilder('getRules');
                if (res != null) {
                    var url = currentInstance.URLs.getKey;
                    var keyData = {
                        tenantId: 1,
                        key: JSON.stringify(res)
                    };

                    var promise = ajaxWrapper.postJSON(url, keyData);
                    promise.done(function (xhr) {
                        $(currentInstance.saveKeyBuilder).off("click", currentInstance.preRenderMethodInst.saveKey);
                        currentInstance.gridRow.setDataValue("Key", xhr);
                        currentInstance.close();
                    });
                }
            }
        }
    }
}

keyBuilderDialog.prototype.buildfilters = function (operators) {
    var filters = [];
    var operators = ['ebs_equals', 'ebs_ne','ebs_gt', 'ebs_gte','ebs_lt','ebs_lte','ebs_all','ebs_any','ebs_notany', 'ebs_contains'];
    $.each(this.aliases, function (idx, val) {
        var entry = {};
        entry.id = val.FullName;
        entry.label = val.DesignName + "- "  + val.Alias;
        entry.type = "string";
        entry.operators = operators;
        if (val.ElementType == "Dropdown List") {
            entry.input = "select";
            entry.values = {};
            var items = JSON.parse(val.Items);
            $.each(items, function (idx, val) {
                entry.values[val.Value] = val.DisplayText;
            });
        }
        else {
            entry.input = "text";
            var filteredOps = [];
            entry.operators = $.each(entry.operators, function (idx, val) {
                if (val != 'ebs_all' && val != 'ebs_any' && val != 'ebs_notany') {
                    filteredOps.push(val);
                }
            });
            entry.operators = filteredOps;
        }
        filters.push(entry);
    });
    return filters;
}
keyBuilderDialog.prototype.render = function() {
    var filters = this.buildfilters();
    var currentInstance = this;
    var containerElementJQ = "#keyBuilder";
    $(containerElementJQ).queryBuilder('destroy');
    $(containerElementJQ).queryBuilder({
        operators: $.fn.queryBuilder.constructor.DEFAULTS.operators.concat([
			    {
			        type: 'ebs_equals', nb_inputs: 1, multiple: false, apply_to: ['string']
			    },
                {
                    type: 'ebs_ne', nb_inputs: 1, multiple: false, apply_to: ['string']
                },
                {
                    type: 'ebs_gt', nb_inputs: 1, multiple: false, apply_to: ['string']
                },
                {
                    type: 'ebs_gte', nb_inputs: 1, multiple: false, apply_to: ['string']
                },
                {
                    type: 'ebs_lt', nb_inputs: 1, multiple: false, apply_to: ['string']
                },
                {
                    type: 'ebs_lte', nb_inputs: 1, multiple: false, apply_to: ['string']
                },
                {
                    type: 'ebs_all', nb_inputs: 1, multiple: true, apply_to: ['string']
                },
                {
                    type: 'ebs_any', nb_inputs: 1, multiple: true, apply_to: ['string']
                },
                {
                    type: 'ebs_notany', nb_inputs: 1, multiple: true, apply_to: ['string']
                },
                {
                    type: 'ebs_contains', nb_inputs: 1, multiple: false, apply_to: ['string']
                }
        ]),

        lang: {
            operators: {
                ebs_equals: '= (EQUALS)',
                ebs_ne: '!= (NOT EQUALS)',
                ebs_gt: '> (GREATER THAN)',
                ebs_gte: '>= (GREATER THAN OR EQUAL TO)',
                ebs_lt: '< (LESS THAN)',
                ebs_lte: '<= (LESS THAN OR EQUAL TO)',
                ebs_all: '₳ (ALL)',
                ebs_any: 'Ɐ (ANY)',
                ebs_notany: '!Ɐ (NOT ANY)',
                ebs_contains: '^ (CONTAINS)'
            }
        },

        allow_empty: true,

        plugins: ['ebs-aliaspicker'],

        filters: filters,

        rules: currentInstance.rules,
        allow_groups: false,
        conditions : ['AND','OR']
    });

    $(currentInstance.saveKeyBuilder).off("click", currentInstance.preRenderMethodInst.saveKey);
    $(currentInstance.saveKeyBuilder).on("click", currentInstance.preRenderMethodInst.saveKey);

}

keyBuilderDialog.prototype.close = function () {
    $(this.containerElementJQ).dialog('close');
}


var expressionBuilderSearchDialog = function (tenantId, formDesignName, effectiveDate, gridRow) {
    this.tenantId = tenantId;
    this.formDesignName = formDesignName;
    this.effectiveDate = effectiveDate;
    this.gridRow = gridRow;
    this.containerElementJQ = '#expressionBuilderSearch';
    this.gridElementJQ = '#expressionBuilderSearchGrid';
    this.gridElement = 'expressionBuilderSearchGrid';
    this.isInit = false;
    this.URLs = {
        getElementList: '/UIElement/GetUIElementListForAliases?tenantId={tenantId}&formDesignName={formDesignName}&effectiveDate={effectiveDate}',
        getElementFullPath: '/UIElement/GetUIElementFullPath?tenantId={tenantId}&uiElementId={uiElementId}&formDesignName={formDesignName}&effectiveDate={effectiveDate}',
        getDropDown: '/UIElement/GetDropDownForElement?tenantId={tenantId}&uiElementId={uiElementId}&formDesignName={formDesignName}&effectiveDate={effectiveDate}'
    }
}

//this function is called below soon after clicking on
//pencil icon for custom rule on uielement property grid.
expressionBuilderSearchDialog.prototype.init = function () {
    $("#expressionBuilderSearch").dialog({
        autoOpen: false,
        width: 1100,
        height: 600,
        modal: true
    });
};

expressionBuilderSearchDialog.prototype.show = function () {
    if (this.isInit == false) {
        this.init();
        this.isInit = true;
    }
    $(this.containerElementJQ).dialog('option', 'title', 'Select Element');
    $(this.containerElementJQ).dialog('open');
    this.loadGrid();
}


expressionBuilderSearchDialog.prototype.loadGrid = function () {
    //set column list
    var colArray = ['UIElementID', 'UIElementName', 'Section', 'Element', 'ElementType'];
    //set column models
    var colModel = [];
    colModel.push({ name: 'UIElementID', index: 'UIElementID', key: true, hidden: true, search: false });
    colModel.push({ name: 'UIElementName', index: 'UIElementName', hidden: true });
    colModel.push({ name: 'Section', index: 'Section', align: 'left', editable: false });
    colModel.push({ name: 'Element', index: 'Element', align: 'left', editable: false });
    colModel.push({ name: 'ElementType', index: 'ElementType', hidden: true });

    var currentInstance = this;
    var fdName = this.formDesignName.replace("AliasList", "");
    var url = this.URLs.getElementList.replace(/{tenantId}/g, this.tenantId).replace(/{formDesignName}/g, fdName).replace(/{effectiveDate}/g, this.effectiveDate);

    //unload previous grid values
    $(this.gridElementJQ).jqGrid('GridUnload');


    $(this.gridElementJQ).parent().append("<div id='p" + currentInstance.gridElement + "'></div>");
    $(this.gridElementJQ).jqGrid({
        datatype: 'json',
        url: url,
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Elements',
        height: '300px',
        pager: '#p' + currentInstance.gridElement,
        //this is added for pagination.
        rowList: [10, 20, 30],
        rowNumber: 10,
        loadonce: true,
        autowidth: true,
        viewrecords: true,
        altRows: true,
        altclass: 'alternate',
        ignoreCase: true,
        hidegrid: false
    });
    $(this.gridElementJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });
    var pagerElement = '#p' + this.gridElement;
    $('#p' + this.gridElement).find('input').css('height', '20px');
    //remove default buttons
    $(this.gridElementJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });

    $(this.gridElementJQ).jqGrid('navButtonAdd', pagerElement,
    {
        caption: '', buttonicon: 'ui-icon-check',
        onClickButton: function () {
            var rowId = $(currentInstance.gridElementJQ).getGridParam('selrow');
            var selectedElement = $(currentInstance.gridElementJQ).jqGrid('getRowData', rowId);
            var fdName = currentInstance.formDesignName.replace("AliasList", "");
            var uiElementId = 1;
            var url = currentInstance.URLs.getElementFullPath.replace(/{tenantId}/g, currentInstance.tenantId).replace(/{uiElementId}/g, selectedElement.UIElementID).replace(/{formDesignName}/g, fdName).replace(/{effectiveDate}/g, currentInstance.effectiveDate);
            var promise = ajaxWrapper.getJSON(url);
            if (selectedElement.ElementType == "Dropdown List" || selectedElement.ElementType == "Dropdown TextBox") {
                var urlDD = currentInstance.URLs.getDropDown.replace(/{tenantId}/g, currentInstance.tenantId).replace(/{uiElementId}/g, selectedElement.UIElementID).replace(/{formDesignName}/g, fdName).replace(/{effectiveDate}/g, currentInstance.effectiveDate);
                var promiseDD = ajaxWrapper.getJSON(urlDD);
                $.when(promise, promiseDD).done(function (xhr, xhrDD) {
                    var elementFullPath = xhr[0];
                    var items = xhrDD[0].Items;
                    var elementType = selectedElement.ElementType;
                    currentInstance.gridRow.setDataValue("ElementPath", elementFullPath);
                    currentInstance.gridRow.setDataValue("ElementType", elementType);
                    var itemsArr = [];
                    $.each(items, function (idx, val) {
                        var item = {};
                        item.Value = val.Value;
                        item.DisplayText = val.DisplayText;
                        itemsArr.push(item);
                    });
                    currentInstance.gridRow.setDataValue("Items", JSON.stringify(itemsArr));
                    currentInstance.close();
                });
            }
            else {
                promise.done(function (xhr) {
                    var elementFullPath = xhr;
                    var elementType = selectedElement.ElementType;
                    currentInstance.gridRow.setDataValue("ElementPath", elementFullPath);
                    currentInstance.gridRow.setDataValue("ElementType", elementType);
                    currentInstance.gridRow.setDataValue("Items", '[]');
                    currentInstance.close();
                });
            }
        }
    });
}

expressionBuilderSearchDialog.prototype.close = function () {
    $(this.containerElementJQ).dialog('close');
}
