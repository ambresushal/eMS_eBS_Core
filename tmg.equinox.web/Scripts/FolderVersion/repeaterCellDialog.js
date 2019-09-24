var repeaterCellDialog = function (params, richTextBoxRenderingFrom, isMultiTextBox) {
    this.isMultiTextBox = isMultiTextBox;
    this.params = params;

    if (richTextBoxRenderingFrom == RichTextBoxRenderingFrom.Cell) {
        this.elementCellData = params.value;
        this.rowIndex = params.rowIndex;
        this.gridOptions = params.api.gridOptionsWrapper.gridOptions;
        this.caption = params.column.colDef.headerName;
        this.colId = params.column.colId;
        this.params = params;
        this.richTextBoxRenderingFrom = richTextBoxRenderingFrom;
        this.txtId = this.gridOptions.currentInstance.gridElementId + this.colId + this.rowIndex;
        this.commentData = this.gridOptions.currentInstance.formInstanceBuilder.commentData;
    }
    else {
        this.headerInstance = params;
        if (isMultiTextBox == true) {
            this.elementCellData = this.headerInstance.eBulkMultiTextbox.value;
        }
        else {
            this.elementCellData = this.headerInstance.eBulkRichTextbox.value;
        }
        this.rowIndex = -1;
        this.gridOptions = this.headerInstance.agParams.api.gridOptionsWrapper.gridOptions;
        this.caption = this.headerInstance.agParams.column.colDef.headerName;
        this.colId = this.headerInstance.agParams.column.colId;
        this.params = this.headerInstance.agParams;
        this.richTextBoxRenderingFrom = richTextBoxRenderingFrom;
    }


    this.isRowReadOnly = true;
    this.elementIDs = {
        repeaterDialogJQ: '#repeaterCellDialog',
        repeaterRowDataJQ: '#repeaterCell',
        repeaterDialogSaveButton: "#repeaterCellDialogSaveBtn",
        repeaterDialogCancelButton: "#repeaterCellDialogCancelBtn",
        repeaterCellDialogClearBtn: "#repeaterCellDialogClearBtn"
    };
}

repeaterCellDialog.prototype.generateRowLayout = function () {
    var currentInstance = this;
    var mainSec = "<div class='row'>";
    var rowlayout = currentInstance.CreateLayout();
    mainSec = mainSec + rowlayout + "</div> ";
    $(currentInstance.elementIDs.repeaterRowDataJQ).html(mainSec);

    if (this.isMultiTextBox == true) {
    }
    else {

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

        currentInstance.loadTinyMCE('#txt' + this.txtId);
    }
}

repeaterCellDialog.prototype.loadTinyMCE = function (id) {
    var currentInstance = this;
    tinymce.remove(id);
    tinymce.initialized = false;
    var editor = $(id).tinymce({
        statusbar: false,
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
                  'tma_annotate',
                  'flite',
                  'model_language',
                  'tinycomments',
        ],
        menubar: "file edit insert view format table tools",
        toolbar1: 'undo redo | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | fontselect |  fontsizeselect',
        toolbar2: 'preview fullscreen forecolor backcolor | override | tma_annotate tma_annotatedelete | flite-toggletracking flite-toggleshow flite-acceptall flite-rejectall flite-acceptone flite-rejectone tinycomments | model_language',
        image_advtab: true,
        powerpaste_word_import: 'prompt',
        powerpaste_html_import: 'prompt',
        removed_menuitems: 'pastetext bold italic',
        extended_valid_elements: "p[*],span[*],delete[*],insert[*],mlstart[contenteditable<false]",
        custom_elements: "~mlstart",
        custom_user_name: currentInstance.params.userInfo == null ? currentInstance.currentUserName : currentInstance.params.userInfo.userName,
        custom_user_id: currentInstance.params.userInfo == null ? currentInstance.currentUserId : currentInstance.params.userInfo.userId,
        templates: [
          { title: 'Test template 1', content: 'Test 1' },
          { title: 'Test template 2', content: 'Test 2' }
        ],
        image_list: [
                   { title: 'Apple', value: '/Content/tinyMce/Apple.png' },
                   { title: 'Question Mark', value: '/Content/tinyMce/Question Mark.png' },
                   { title: 'Tick Mark', value: '/Content/tinyMce/Tick Mark.png' },
                   { title: 'Square Bullet', value: '/Content/tinyMce/Square Bullet.png' },
                   { title: 'ID Card No Rx - Back', value: '/Content/tinyMce/ID Card No Rx - Back.png' },
                   { title: 'ID Card No Rx - Front', value: '/Content/tinyMce/ID Card No Rx - Front.png' },
                   { title: 'Medicare Rx Membership Card', value: '/Content/tinyMce/Medicare Rx Membership Card.png' },
                   { title: 'Back Membership Card', value: '/Content/tinyMce/Back Membership Card.png' },
                   { title: 'Tick [Large]', value: '/Content/tinyMce/Tick [Large].png' },
                   { title: 'Tick Bullets', value: '/Content/tinyMce/Tick Bullets.png' },
                   { title: 'Tick Not Valid', value: '/Content/tinyMce/Tick Not Valid.png' }
        ],
        style_formats: [
                           { title: 'Heading 1', block: 'h1' },
                           { title: 'Heading 2', block: 'h2' },
                           { title: 'Heading 3', block: 'h3' },
                           { title: 'Heading 4', block: 'h4' },
                           { title: 'Heading 5', block: 'h5' },
                           { title: 'Heading 6', block: 'h6' },
        ],
        content_css: '/Content/css/tmaannotation.css',
        tinycomments_create: create,
        tinycomments_reply: reply,
        tinycomments_delete: del,
        tinycomments_lookup: lookup,
        setup: function (editor) {
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
            editor.on('focusin', function (e) {
                var content = editor.getContent();
                try {
                    var flitePlugin = editor.plugins.flite;
                    if (flitePlugin != undefined) {
                        flitePlugin.setUserInfo({ id: currentInstance.params.userInfo.userId, name: currentInstance.params.userInfo.userName });
                    }
                } catch (e) {
                    console.log('Error while adding wrapper.');
                    editor.setContent(content);
                }

            });
        }
    });
    function randomString() {
        return Math.random().toString(36).substring(2, 14);
    }

    function getConversation(uid) {
        var el = currentInstance.commentData;
        var conv = el[uid];
        if (!conv) {
            el[uid] = [];
        }
        return el[uid];
    }

    function setConversation(uid, conversation) {
        var el = currentInstance.commentData;
        el[uid] = conversation;
    }

    function deleteConversation(uid) {
        var el = currentInstance.commentData;
        delete el[uid];
    }

    function create(content, done, fail) {
        var uid = 'ant-' + randomString();
        try {
            setConversation(
                uid, [{
                    userName: currentInstance.params.userInfo.userName,
                    userID: currentInstance.params.userInfo.userId,
                    richTextBoxId: id,
                    comment: content
                }]
            );
            done(uid);
        } catch (ex) {
            fail(new Error('Error creating conversation...'));
        }
    }

    function reply(uid, content, done, fail) {
        try {
            var comments = getConversation(uid);
            comments.push({
                userName: currentInstance.params.userInfo.userName,
                userID: currentInstance.params.userInfo.userId,
                richTextBoxId: id,
                comment: content
            });
            setConversation(uid, comments);
            done();
        } catch (ex) {
            fail(new Error('Error replying to conversation...'));
        }
    }

    function del(uid, done, fail) {
        deleteConversation(uid);
        done();
    }

    function lookup(uid, done, fail) {
        try {
            var comments = getConversation(uid).map(function (item) {
                return {
                    author: item.userName, //getAuthorDisplayName(item.user),
                    content: item.comment
                };
            });
            done({
                comments: comments
            });
        } catch (ex) {
            fail(new Error('Error looking up conversation...'));
        }
    }
}



repeaterCellDialog.prototype.CreateLayout = function () {
    var currentInstance = this;

    if (this.isMultiTextBox == true) {
        divRow = "<div class='form-group'><textarea class='form-control'  rows ='5' style='height:400px;' id='txt" + this.txtId + "'>" + currentInstance.elementCellData + "</textarea></div>";

    }
    else {
        divRow = "<div class='richTextboxDialog'>"
                                       + "<textarea id='txt" + this.txtId + "' wrap='hard' cols='25' rows='5' style='height:400px;z-index:0;' class='ag-popup-editor'>" + currentInstance.elementCellData + "</textarea></div>";
    }
    return divRow;
}




repeaterCellDialog.prototype.init = function () {
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
}

repeaterCellDialog.prototype.show = function () {
    var currentInstance = this;

    currentInstance.init();
    $(currentInstance.elementIDs.repeaterRowDataJQ).hide().fadeIn("slow");
    currentInstance.generateRowLayout();

    $(currentInstance.elementIDs.repeaterDialogJQ).dialog('option', 'title', currentInstance.caption);
    $(currentInstance.elementIDs.repeaterDialogJQ).dialog("open");




    $(currentInstance.elementIDs.repeaterDialogSaveButton).unbind("click");
    //if (!currentInstance.isRowReadOnly) {

    $(currentInstance.elementIDs.repeaterDialogSaveButton).bind("click", function () {


        if (RichTextBoxRenderingFrom.Cell == currentInstance.richTextBoxRenderingFrom) {
            var newValue = $('#txt' + currentInstance.txtId).val();
            var oldValue = "";
            var itemsToUpdate = {};
            var rowNode = {};
            //   if (currentInstance.rowIndex < currentInstance.gridOptions.rowData.length)
            if (currentInstance.rowIndex < currentInstance.gridOptions.api.getDisplayedRowCount()) {
                //If filter is applied then rowIndex is not correct, so in this case select the rowindex of correct one and then replace

                var rowIndexToChange = currentInstance.rowIndex;
                var rowIDPropertyCurrent = 0;//currentInstance.gridOptions.rowData[currentInstance.rowIndex]["RowIDProperty"].toString();
                if (currentInstance.gridOptions.api.getSelectedRows().length > 0) {
                    rowIDPropertyCurrent = currentInstance.gridOptions.api.getSelectedRows()[0]["RowIDProperty"];
                }
                var rowIDPropertyByValue = currentInstance.rowIndex;

                var eleOldVal = currentInstance.elementCellData;
                var filterListByOldValue = currentInstance.gridOptions.rowData.filter(function (ct) {
                    if (ct[currentInstance.colId] == eleOldVal)
                        return ct;
                });
                if (filterListByOldValue != null && filterListByOldValue != undefined && filterListByOldValue.length > 0) {
                    rowIDPropertyByValue = filterListByOldValue[0].RowIDProperty;
                }

                if (rowIDPropertyCurrent != rowIDPropertyByValue) {
                    rowIndexToChange = rowIDPropertyByValue;
                }

                //oldValue = currentInstance.gridOptions.rowData[rowIndexToChange][currentInstance.colId].toString();
                if (currentInstance.gridOptions.api.getSelectedRows().length > 0) {
                    oldValue = currentInstance.gridOptions.api.getSelectedRows()[0][currentInstance.colId];
                }
                //itemsToUpdate = currentInstance.gridOptions.rowData[rowIndexToChange];
                rowNode = currentInstance.gridOptions.api.getRowNode(rowIDPropertyCurrent);
            }
            else {
                itemsToUpdate = currentInstance.params.node.data;
                rowNode = currentInstance.params.node;
            }
            if (rowNode) {
                rowNode.setDataValue(currentInstance.colId, newValue);

                // currentInstance.params.node.setRowData(data);
                // currentInstance.params.api.gridOptionsWrapper.gridOptions.api.updateRowData({ update: itemsToUpdate });
                var parentInstance = currentInstance.params.api.gridOptionsWrapper.gridOptions.currentInstance;

                if (oldValue !== newValue) {
                    var event = {
                        data: currentInstance.params.node.data,
                        node: currentInstance.params.node,
                        colDef: currentInstance.params.column.colDef,
                        oldValue: oldValue,
                        newValue: newValue,
                        api: currentInstance.params.api,
                        column: currentInstance.params.column.colId
                    }
                    parentInstance.updateActivityLog(parentInstance, event);
                }
            }

            try
            {
                if(currentInstance.gridOptions.currentInstance != null && currentInstance.gridOptions.currentInstance.fullName == "ANOCChartPlanDetails.ANOCBenefitsCompare")
                {
                    currentInstance.gridOptions.currentInstance.formInstanceBuilder.formData["ANOCChartPlanDetails"]["ANOCBenefitsCompare"]= currentInstance.gridOptions.rowData;
                }
            }
            catch(ex)
            {
                
            }

            //   currentInstance.params.api.refreshCells();
            currentInstance.params.api.refreshCells();
            currentInstance.gridOptions.api.resetRowHeights();
            currentInstance.gridOptions.currentInstance.formInstanceBuilder.commentData = currentInstance.commentData;
        }
        else {

            var item = {};

            if (currentInstance.isMultiTextBox == true) {

                currentInstance.headerInstance.eBulkMultiTextbox.value = $('#txt' + currentInstance.txtId).val();
                var item = {
                    id: currentInstance.headerInstance.eBulkMultiTextbox.id,
                    value: currentInstance.headerInstance.eBulkMultiTextbox.value
                };
            }
            else {
                currentInstance.headerInstance.eBulkRichTextbox.value = $('#txt' + currentInstance.txtId).val();
                var item = {
                    id: currentInstance.headerInstance.eBulkRichTextbox.id,
                    value: currentInstance.headerInstance.eBulkRichTextbox.value
                };
            }
            //console.log(item);
            currentInstance.headerInstance.addBulkItems(item, currentInstance.headerInstance.agParams.api.gridOptionsWrapper.gridOptions.currentInstance);
        }

        $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");

    });


    $(currentInstance.elementIDs.repeaterDialogCancelButton).unbind("click");
    $(currentInstance.elementIDs.repeaterDialogCancelButton).bind("click", function () {
        $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");
    });

    $(currentInstance.elementIDs.repeaterCellDialogClearBtn).unbind("click");
    $(currentInstance.elementIDs.repeaterCellDialogClearBtn).bind("click", function () {

        $('#txt' + currentInstance.txtId).val('');
    });

}




repeaterCellDialog.prototype.visibleRuleResultCallBack = function (rule, result) {
    var currentInstance = this;

}

repeaterCellDialog.prototype.ruleResultCallBack = function (rule, row, retVal, childName, childIdx) {
    var currentInstance = this;

}


