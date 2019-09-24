function repeaterTemplateDialog(uiElement, status, formDesignVersionId, formDesignVersionInstance, uiElementDetail) {
    this.uiElement = uiElement;
    this.formDesignVersionId = formDesignVersionId;
    this.tenantId = 1;
    this.statustext = status;
    this.formDesignVersionInstance = formDesignVersionInstance;
    this.uiElementDetail = uiElementDetail;
    this.templateData = [];
}

repeaterTemplateDialog.elementIDs = {
    templateDialog: '#templateDialog',
    templateAccordian: '#templateAccordian',
    rowTemplate: '#rowTemplate',
    headerTemplate: '#headerTemplate',
    footerTemplate: '#footerTemplate',
}

$(document).on('click', '.mce-menubtn', function () {
    menuItem = $(this);
    $('.mce-menu').css('z-index', '999999');
});

$(document).on('mouseover', '.mce-menu', function () {
    menuItem = $(this);
    $('.mce-menu-sub-tr-tl').css('z-index', '99999999');
    $('.mce-menu-sub-br-bl').css('z-index', '99999999');
    $('.mce-menu').css('z-index', '999999');
});

$(document).on('mouseover', '.mce-menubtn', function () {
    $('.mce-menu').css('z-index', '99999999');
});

repeaterTemplateDialog._isInitialized = false;

//init dialog
repeaterTemplateDialog.init = function () {
    var currenInstance = this;
    if (repeaterTemplateDialog._isInitialized == false) {
        $(repeaterTemplateDialog.elementIDs.templateDialog).dialog({
            autoOpen: false,
            resizable: false,
            width: '45%',
            modal: true,
            position: ["middle", 100],
            open: function () {
                $(repeaterTemplateDialog.elementIDs.templateAccordian).accordion({ autoHeight: false, collapsible: true, active: false });
            },
            close: function () {
                tinyMCE.editors[0].undoManager.clear();
                tinyMCE.editors[1].undoManager.clear();
                tinyMCE.editors[2].undoManager.clear();
            }
        });
        repeaterTemplateDialog._isInitialized = true;
    }
}

repeaterTemplateDialog.init();


//show dialog
repeaterTemplateDialog.prototype.show = function (isLoaded) {
    if (!isLoaded) {
        $(repeaterTemplateDialog.elementIDs.templateDialog).dialog('option', 'title', 'Template Configuration - ' + this.uiElement.Label);
        $(repeaterTemplateDialog.elementIDs.templateDialog).dialog('open');
        this.loadConfigration();
    }
    else {
        $(repeaterTemplateDialog.elementIDs.templateDialog).dialog('open');
    }
}

repeaterTemplateDialog.prototype.loadConfigration = function () {
    var currentInstance = this;
    if (currentInstance.uiElementDetail != undefined && currentInstance.uiElementDetail != null && currentInstance.uiElementDetail.RepeaterUIElementProperties != undefined
        && currentInstance.uiElementDetail.RepeaterUIElementProperties != null) {
        var data = currentInstance.uiElementDetail.RepeaterUIElementProperties;
        if (data.RowTemplate == null || data.RowTemplate == undefined)
            data.RowTemplate = "";
        if (data.HeaderTemplate == null || data.HeaderTemplate == undefined)
            data.HeaderTemplate = "";
        if (data.FooterTemplate == null || data.FooterTemplate == undefined)
            data.FooterTemplate = "";

        $(repeaterTemplateDialog.elementIDs.rowTemplate).val(data.RowTemplate);
        $(repeaterTemplateDialog.elementIDs.headerTemplate).val(data.HeaderTemplate);
        $(repeaterTemplateDialog.elementIDs.footerTemplate).val(data.FooterTemplate);

        currentInstance.loadTinyMCE(repeaterTemplateDialog.elementIDs.rowTemplate);
        currentInstance.loadTinyMCE(repeaterTemplateDialog.elementIDs.headerTemplate);
        currentInstance.loadTinyMCE(repeaterTemplateDialog.elementIDs.footerTemplate);



    } else {
        $(repeaterTemplateDialog.elementIDs.rowTemplate).val("");
        $(repeaterTemplateDialog.elementIDs.headerTemplate).val("");
        $(repeaterTemplateDialog.elementIDs.footerTemplate).val("");

        currentInstance.loadTinyMCE(repeaterTemplateDialog.elementIDs.rowTemplate);
        currentInstance.loadTinyMCE(repeaterTemplateDialog.elementIDs.headerTemplate);
        currentInstance.loadTinyMCE(repeaterTemplateDialog.elementIDs.footerTemplate);
    }
}

repeaterTemplateDialog.prototype.loadTinyMCE = function (id) {
    //tinymce.remove(id);
    $(id).tinymce({
        statusbar: false,
        theme: 'modern',
        height: 180,
        plugins: [
          'advlist autolink lists charmap print preview hr pagebreak',
          'searchreplace wordcount visualblocks visualchars code',
          'insertdatetime save table directionality',
          'emoticons template powerpaste textcolor colorpicker textpattern imagetools codesample toc'
        ],
        powerpaste_word_import: 'prompt',
        powerpaste_html_import: 'prompt',
        menubar: "file edit insert view format table",
        toolbar1: 'undo redo | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent',
        toolbar2: 'preview forecolor backcolor | fontselect |  fontsizeselect | CenterAlign | RightAlign',
        image_advtab: true,
        templates: [
          { title: 'Test template 1', content: 'Test 1' },
          { title: 'Test template 2', content: 'Test 2' }
        ],
        removed_menuitems: 'pastetext bold italic',
        style_formats: [
                            { title: 'Heading 1', block: 'h1' },
                            { title: 'Heading 2', block: 'h2' },
                            { title: 'Heading 3', block: 'h3' },
                            { title: 'Heading 4', block: 'h4' },
                            { title: 'Heading 5', block: 'h5' },
                            { title: 'Heading 6', block: 'h6' },
        ],
        //content_css: [
        //  '//fonts.googleapis.com/css?family=Lato:300,300i,400,400i',
        //  '//www.tinymce.com/css/codepen.min.css'
        //],
        setup: function (editor) {
            editor.on('focus', function () {
                $(this.contentAreaContainer.parentElement).find("div.mce-toolbar-grp").show();
                $(this.contentAreaContainer.parentElement).find("div.mce-menubar").show();

            });
            editor.on('blur', function () {
                $(this.contentAreaContainer.parentElement).find("div.mce-toolbar-grp").hide();
                $(this.contentAreaContainer.parentElement).find("div.mce-menubar").hide();
            });
            editor.on("init", function () {
                $(this.contentAreaContainer.parentElement).find("div.mce-toolbar-grp").hide();
                $(this.contentAreaContainer.parentElement).find("div.mce-menubar").hide();
            });

            editor.addButton('CenterAlign', {
                text: 'Center',
                icon: false,
                onclick: function () {
                    editor.insertContent('<div align="center">Enter Table Here<p style="margin: 0in 0in 10pt; line-height: 115%; font-size: 11pt; font-family: Calibri, sans-serif;">&nbsp;</p></div>');
                }
            });
            editor.addButton('RightAlign', {
                text: 'Right',
                icon: false,
                onclick: function () {
                    editor.insertContent('<div align="right">Enter Table Here<p style="margin: 0in 0in 10pt; line-height: 115%; font-size: 11pt; font-family: Calibri, sans-serif;">&nbsp;</p></div>');
                }
            });
        }
    });

}

// Get updated values for templates
repeaterTemplateDialog.prototype.getConfigurationData = function () {
    var currentInstance = this;
    currentInstance.templateData.push({
        RowTemplate: $(repeaterTemplateDialog.elementIDs.rowTemplate).val(),
        HeaderTemplate: $(repeaterTemplateDialog.elementIDs.headerTemplate).val(),
        FooterTemplate: $(repeaterTemplateDialog.elementIDs.footerTemplate).val(),
    });
    return currentInstance.templateData;
}

