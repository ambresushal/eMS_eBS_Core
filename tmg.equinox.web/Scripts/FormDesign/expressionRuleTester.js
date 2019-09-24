function expressionRuleTester() {
    this.documentRuleId;
    this.formDesignID;
    this.formDesignVersionId;
}

expressionRuleTester.prototype.elementIDs = {
    expressionRuleDialogTabList: "#expressionRuleDialogTabList",
    expressionRulePanel: "#expressionRulePanel",
    expressionRuleTesterPanel: "#expressionRuleTesterPanel",
    documentListJQ: "#documentList",
    documentList: "documentList",
    ruleResultJSON: "#ruleResultJSON",
    libtnFormatRuleText: "#libtnFormatRuleText",
    libtnValidateRule: "#libtnValidateRule",
    libtnSaveExpresssionData: "#libtnSaveExpresssionData",
    libtnDeleteExpresssionRule: "#libtnDeleteExpresssionRule",
    libtnDownloadExpresssionData: "#libtnDownloadExpresssionData",
}

expressionRuleTester.prototype.URLs = {
    documentList: '/ConsumerAccount/GetDocumentsListNew?tenantId=1&formDesignID={formDesignID}',
    processRuleForTester: '/ExpressionBuilder/ProcessRuleForTester?tenantId={tenantId}&ruleID={ruleID}&formInstanceId={formInstanceId}&folderVersionId={folderVersionId}&formDesignVersionId={formDesignVersionId}',
}

expressionRuleTester.prototype.init = function (documentRuleId, formDesignID, formDesignVersionId) {
    var currentInstance = this;
    currentInstance.documentRuleId = documentRuleId;
    currentInstance.formDesignID = formDesignID;
    currentInstance.formDesignVersionId = formDesignVersionId;
    currentInstance.loadTinyMCE(currentInstance.elementIDs.ruleResultJSON);
}


expressionRuleTester.prototype.loadDocumentsGrid = function () {
    var currentInstance = this;
    var colArray = ['Account', 'Folder', 'Consortium', 'Folder Version Number', 'Effective Date', 'Document Name', 'Document Type', '', '', '', '', '', ''];
    var colModel = [];
    colModel.push({ name: 'AccountName', index: 'AccountName', editable: false, align: 'left', width: 200 });
    colModel.push({ name: 'FolderName', index: 'FolderName', editable: false, align: 'left' });
    colModel.push({ name: 'ConsortiumName', index: 'ConsortiumName', editable: false, align: 'left', hidden: true });
    colModel.push({ name: 'VersionNumber', index: 'VersionNumber', editable: false, align: 'left' });
    colModel.push({ name: 'EffectiveDate', index: 'EffectiveDate', editable: false, formatter: 'date', formatoptions: JQGridSettings.DateFormatterOptions, align: 'center' });
    colModel.push({ name: 'FormInstanceName', index: 'FormInstanceName', editable: false, align: 'left' });
    colModel.push({ name: 'DesignType', index: 'DesignType', editable: false, align: 'left' });
    colModel.push({ name: 'TenantID', index: 'TenantID', hidden: true });
    colModel.push({ name: 'AccountID', index: 'AccountID', hidden: true });
    colModel.push({ name: 'FolderID', index: 'FolderID', hidden: true });
    colModel.push({ name: 'FolderVersionID', index: 'FolderVersionID', hidden: true });
    colModel.push({ name: 'FormInstanceID', index: 'FormInstanceID', hidden: true });
    colModel.push({ name: 'FormDesignID', index: 'FormDesignID', hidden: true });


    $(currentInstance.elementIDs.documentListJQ).jqGrid('GridUnload');
    $(currentInstance.elementIDs.documentListJQ).parent().append("<div id='p" + currentInstance.elementIDs.documentList + "'></div>");
    var url = currentInstance.URLs.documentList.replace('{formDesignID}', currentInstance.formDesignID);
    $(currentInstance.elementIDs.documentListJQ).jqGrid({
        url: url,
        datatype: 'json',
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: 'Select Documents',
        height: '200',
        rowNum: 20,
        rowList: [10, 20, 30],
        ignoreCase: true,
        autowidth: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        pager: '#p' + currentInstance.elementIDs.documentList,
        sortname: 'FormInstanceName',
        altclass: 'alternate',
        gridComplete: function () {
            console.log("grid complete");
        }
    });
    var pagerElement = '#p' + currentInstance.elementIDs.documentList;
    //remove default buttons
    $(currentInstance.elementIDs.documentListJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(currentInstance.elementIDs.documentListJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
    $(currentInstance.elementIDs.documentListJQ).jqGrid('navButtonAdd', pagerElement,
       {
           caption: '', buttonicon: 'ui-icon-play', title: 'Execute Rule on selected document',
           onClickButton: function () {
               var selRowId = $(currentInstance.elementIDs.documentListJQ).getGridParam('selrow');
               if (selRowId != null) {
                   $(currentInstance.elementIDs.ruleResultJSON).val('');
                   var rowData = $(currentInstance.elementIDs.documentListJQ).jqGrid("getRowData", selRowId);
                   var url = currentInstance.URLs.processRuleForTester.replace('{tenantId}', 1)
                            .replace('{ruleID}', currentInstance.documentRuleId).replace('{formInstanceId}', rowData.FormInstanceID)
                            .replace('{folderVersionId}', rowData.FolderVersionID).replace('{formDesignVersionId}', currentInstance.formDesignVersionId);
                   var promise = ajaxWrapper.postJSONCustom(url);
                   promise.done(function (data) {
                       $(currentInstance.elementIDs.ruleResultJSON).val(JSON.parse(data));
                   });
               } else {
                   messageDialog.show('Please select the document to execute the rule.');
               }
           }
       });
}

expressionRuleTester.prototype.registerTabEvents = function () {
    var currentInstance = this;
    $(currentInstance.elementIDs.expressionRuleDialogTabList + ' a[data-toggle="tab"]').unbind().bind('click', function (e) {
        if (e.target.hash == currentInstance.elementIDs.expressionRuleTesterPanel) {
            if (currentInstance.documentRuleId != null && currentInstance.documentRuleId != undefined) {
                if (!currentInstance.ifRuleExist()) {
                    messageDialog.show("Rule not found or saved. Please save the rule and try again.");
                    return false;
                }
                $(currentInstance.elementIDs.libtnValidateRule).hide();
                $(currentInstance.elementIDs.libtnFormatRuleText).hide();
                $(currentInstance.elementIDs.libtnSaveExpresssionData).hide();
                $(currentInstance.elementIDs.libtnDeleteExpresssionRule).hide();
                $(currentInstance.elementIDs.libtnDownloadExpresssionData).hide();
                currentInstance.loadDocumentsGrid();
            } else {
                messageDialog.show("Rule not found, please create the Rule and Save and try again.");
                return false;
            }
        } else if (e.target.hash == currentInstance.elementIDs.expressionRulePanel) {
            $(currentInstance.elementIDs.libtnValidateRule).show();
            $(currentInstance.elementIDs.libtnFormatRuleText).show();
            $(currentInstance.elementIDs.libtnSaveExpresssionData).show();
            $(currentInstance.elementIDs.libtnDeleteExpresssionRule).show();
            $(currentInstance.elementIDs.libtnDownloadExpresssionData).show();

        }
    });
}

expressionRuleTester.prototype.ifRuleExist = function () {
    var currentInstance = this;
    if (currentInstance.documentRuleId <= 0) {
        return false
    } else {
        return true;
    }
}

expressionRuleTester.prototype.loadTinyMCE = function (id) {
    var currentInstance = this;
    tinymce.remove(id);
    tinymce.initialized = false;
    $(id).val('');
    var editor = $(id).tinymce({
        statusbar: false,
        readonly: true,
        height: 220,
        theme: 'modern',
        forced_root_block: "",
        force_br_newlines: true,
        force_p_newlines: false,
        plugins: [
                  'advlist autolink lists charmap print preview hr pagebreak',
                  'searchreplace wordcount visualblocks visualchars code fullscreen',
                  'insertdatetime save table contextmenu directionality',
                  'emoticons template textcolor colorpicker textpattern imagetools codesample toc',
                  'image',
                  'powerpaste',
                  'tma_annotate'
        ],
        menubar: false,
        toolbar: 'preview fullscreen',
        content_css: '/Content/css/tmaannotation.css',
        setup: function (editor) {
            editor.on('init', function () {
                if (editor.readonly) {
                    enableTinyMceEditorPlugin(id, editor, 'preview', 'mcePreview');
                    enableTinyMceEditorPlugin(id, editor, 'fullscreen', 'mceFullscreen');
                }
            });
        }
    });

    function enableTinyMceEditorPlugin(editorId, editor, pluginName, commandName) {
        var htmlEditorDiv = $(editorId).prev();
        var buttonDiv = htmlEditorDiv.find('.mce-i-' + pluginName.toLowerCase())[0].parentElement.parentElement;
        buttonDiv.className = buttonDiv.className.replace(' mce-disabled', '');
        buttonDiv.removeAttribute('aria-disabled');
        buttonDiv.firstChild.onclick = function () {
            editor.execCommand(commandName);
        };
    }
}