var repeaterdialogAG = function (rowId, elementData, repeaterBuilder, isRowReadOnly, repeaterData, userId, userName) {
    this.caption = repeaterBuilder.design.Label;
    this.elementData = elementData;
    this.repeaterData = repeaterData;
    this.currentUserId = userId;
    this.currentUserName = userName;

    repeaterBuilder.rowViewMode = false;
    if (repeaterBuilder.hasChildGrids) {
        var subGridId = repeaterBuilder.gridElementId + '_' + rowId;
        repeaterBuilder.loadChildGrids(subGridId, rowId);
    }

    var row = repeaterData.filter(function (ct) {
        if (ct.RowIDProperty == rowId)
            return ct;
    });
    this.rowData = $.extend(true, {}, row[0]);

    if (repeaterBuilder.design.ChildDataSources != null && repeaterBuilder.design.ChildDataSources != undefined) {
        for (var i = 0; i < repeaterBuilder.design.ChildDataSources.length; i++) {
            if (repeaterBuilder.design.ChildDataSources[i].DisplayMode == "Child") {
                this.childDataSourceDisplayMode = repeaterBuilder.design.ChildDataSources[i].DisplayMode;
                this.childGridDataSourceName = repeaterBuilder.design.ChildDataSources[i].DataSourceName;
            }
            else if (repeaterBuilder.design.ChildDataSources[i].DisplayMode == "In Line") {
                this.inlineDataSourceDisplayMode = repeaterBuilder.design.ChildDataSources[i].DisplayMode;
                this.inlineGridDataSourceName = repeaterBuilder.design.ChildDataSources[i].DataSourceName;
            }
        }
    }

    this.repeaterBuilder = repeaterBuilder;
    this.rowID = this.rowData["RowIDProperty"] || 0;
    this.isRowReadOnly = isRowReadOnly;
    this.elementIDs = {
        repeaterDialogJQ: '#repeaterDialog',
        repeaterRowDataJQ: '#repeaterRowData',
        childGridJQ: '#childGrid' + this.rowID,
        childGrid: 'childGrid' + this.rowID,
        repeaterDialogSaveButton: "#repeaterDialogSaveBtn",
        repeaterDialogCancelButton: "#repeaterDialogCancelBtn",
        repeaterDialogPrevButton: "#repeaterDialogPrevBtn",
        repeaterDialogNextButton: "#repeaterDialogNextBtn",
        repeaterPreviousBtnContainer: "#repeaterPreviousBtnContainer",
        repeaterNextBtnContainer: "#repeaterNextBtnContainer",
        viewTemplateDialog: "#viewTemplateDialog",
        viewTemplateDialogML: "#viewTemplateDialogML",
    };
}

repeaterdialogAG.prototype.generateRowLayout = function () {
    var currentInstance = this;
    var mainSec = "<div class='row'>";
    var rowlayout = currentInstance.CreateLayout();
    mainSec = mainSec + rowlayout + "</div> ";
    $(currentInstance.elementIDs.repeaterRowDataJQ).html(mainSec);


    if (currentInstance.childDataSourceDisplayMode == "Child") {
        var subArray = currentInstance.getsubArrays(currentInstance.rowData, [], currentInstance.inlineGridDataSourceName);

        var childGridId = 'childGrid' + currentInstance.rowID;
        $(currentInstance.elementIDs.repeaterRowDataJQ).append("<div class='grid-wrapper'><table id='" + childGridId + "'> </table></div>");

        currentInstance.loadChildGrid(subArray);
    }
    if (currentInstance.inlineDataSourceDisplayMode == "In Line") {
        var subArray = currentInstance.getsubArrays(currentInstance.rowData, [], currentInstance.childGridDataSourceName);

        for (key in subArray) {
            var inlineRowId = 0;
            $.each(subArray[key], function (k, sectionData) {
                var sec = currentInstance.CreateInLineLayout(sectionData, inlineRowId);
                inlineRowId++;
                $(currentInstance.elementIDs.repeaterRowDataJQ).append(sec);
            });
        }
    }

    for (var idx = 0; idx < currentInstance.repeaterBuilder.design.Elements.length; idx++) {
        if (currentInstance.repeaterBuilder.design.Elements[idx].Type == 'calendar') {
            //currentInstance.pickDates("formEdit" + currentInstance.rowID + "_" + currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName, currentInstance.repeaterBuilder.design.Elements[idx]);
            //var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', currentInstance.rowID);
            var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getLocalRow', currentInstance.rowID);
            var inlineRowId = 0;
            for (var prop in rowData) {
                if (prop.substring(0, 4) == 'INL_') {
                    var propArr = prop.split('_');
                    if (propArr.length == 4) {
                        var dsName = propArr[1];
                        var propIdx = propArr[2];
                        var propName = propArr[3];
                        if (propName == currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName) {
                            var idpluscol = "formEdit" + "_INL_" + inlineRowId + "_" + propName;
                            currentInstance.pickDates(idpluscol, currentInstance.repeaterBuilder.design.Elements[idx], currentInstance.rowID + "_INL_" + dsName + "_" + inlineRowId + "_" + propName);
                            inlineRowId++;
                        }
                    }
                }
                else if (prop == currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName) {
                    var idpluscol = "formEdit" + currentInstance.rowID + "_" + prop;
                    currentInstance.pickDates(idpluscol, currentInstance.repeaterBuilder.design.Elements[idx], currentInstance.rowID + "_" + prop);
                }
            }
        }
        //for dropdown textbox element
        if (currentInstance.repeaterBuilder.design.Elements[idx].Type == 'SelectInput') {
            //var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', currentInstance.rowID);
            var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getLocalRow', currentInstance.rowID);
            var inlineRowId = 0;
            for (var prop in rowData) {
                if (prop.substring(0, 4) == 'INL_') {
                    var propArr = prop.split('_');
                    if (propArr.length == 4) {
                        var dsName = propArr[1];
                        var propIdx = propArr[2];
                        var propName = propArr[3];
                        if (propName == currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName) {
                            var idpluscol = "formEdit" + "_INL_" + inlineRowId + "_" + propName;
                            currentInstance.formatDropdownTextbox(idpluscol, currentInstance.repeaterBuilder.design.Elements[idx], currentInstance.rowID + "_INL_" + dsName + "_" + inlineRowId + "_" + propName);
                            inlineRowId++;
                        }
                    }
                }
                else if (prop == currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName) {
                    var idpluscol = "formEdit" + currentInstance.rowID + "_" + prop;
                    currentInstance.formatDropdownTextbox(idpluscol, currentInstance.repeaterBuilder.design.Elements[idx], currentInstance.rowID + "_" + prop);
                }
            }
        }
    }

    if (!($("#repeater" + currentInstance.repeaterBuilder.gridElementId).hasClass('disabled'))) {
        if (currentInstance.repeaterBuilder.fullName != "BenefitReview.BenefitReviewGrid")
            currentInstance.processRules();
    }

    $(currentInstance.elementIDs.repeaterDialogJQ).find('input,select,textarea').each(function (idx, elem) {
        $(elem).on('change', function () {
            var elem = this;
            currentInstance.loadRulesForElement(elem);
        });
        $(elem).on('focusout', function () {
            var elem = this;
            var designElem = currentInstance.repeaterBuilder.design.Elements.filter(function (el) {
                return $(elem).attr('id').indexOf(el.GeneratedName) >= 0;
            });
            if (designElem != null && designElem.length > 0) {
                designElem = designElem[0];
                var rules = currentInstance.repeaterBuilder.formInstanceBuilder.rules.getRulesForElement(designElem.FullName);
                if (rules == null || rules.length <= 0) {
                    currentInstance.loadValidationsForElement(elem);
                    currentInstance.loadDuplicationsForElement(elem);
                }
            }
        });
    });

}
repeaterdialogAG.prototype.generateRowLayoutPQ = function () {
    var currentInstance = this;
    var richTextBoxIds = [];
    var mainSec = "<div class=''>";
    var rowlayout = currentInstance.CreateLayoutPQ(richTextBoxIds);
    mainSec = mainSec + rowlayout + "</div> ";
    $(currentInstance.elementIDs.repeaterRowDataJQ).html(mainSec);

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
    $(richTextBoxIds).each(function (index, value) {
        currentInstance.loadTinyMCE('#' + value);
    });
    if (currentInstance.childDataSourceDisplayMode == "Child") {
        var subArray = currentInstance.getsubArrays(currentInstance.rowData, [], currentInstance.inlineGridDataSourceName);

        var childGridId = 'childGrid' + currentInstance.rowID;
        $(currentInstance.elementIDs.repeaterRowDataJQ).append("<div class='grid-wrapper'><table id='" + childGridId + "'> </table></div>");

        currentInstance.loadChildGrid(subArray);
    }
    if (currentInstance.inlineDataSourceDisplayMode == "In Line") {
        var subArray = currentInstance.getsubArrays(currentInstance.rowData, [], currentInstance.childGridDataSourceName);

        for (key in subArray) {
            var inlineRowId = 0;
            $.each(subArray[key], function (k, sectionData) {
                var sec = currentInstance.CreateInLineLayout(sectionData, inlineRowId);
                inlineRowId++;
                $(currentInstance.elementIDs.repeaterRowDataJQ).append(sec);
            });
        }
    }

    for (var idx = 0; idx < currentInstance.repeaterBuilder.design.Elements.length; idx++) {
        if (currentInstance.repeaterBuilder.design.Elements[idx].Type == 'calendar') {
            //currentInstance.pickDates("formEdit" + currentInstance.rowID + "_" + currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName, currentInstance.repeaterBuilder.design.Elements[idx]);
            //var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', currentInstance.rowID);
            //   var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getLocalRow', currentInstance.rowID);
            // var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("getRowData", { rowIndx: currentInstance.rowID });
            var rowData = currentInstance.rowData;
            var inlineRowId = 0;
            for (var prop in rowData) {
                if (prop.substring(0, 4) == 'INL_') {
                    var propArr = prop.split('_');
                    if (propArr.length == 4) {
                        var dsName = propArr[1];
                        var propIdx = propArr[2];
                        var propName = propArr[3];
                        if (propName == currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName) {
                            var idpluscol = "formEdit" + "_INL_" + inlineRowId + "_" + propName;
                            currentInstance.pickDates(idpluscol, currentInstance.repeaterBuilder.design.Elements[idx], currentInstance.rowID + "_INL_" + dsName + "_" + inlineRowId + "_" + propName);
                            inlineRowId++;
                        }
                    }
                }
                else if (prop == currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName) {
                    var idpluscol = "formEdit" + currentInstance.rowID + "_" + prop;
                    currentInstance.pickDates(idpluscol, currentInstance.repeaterBuilder.design.Elements[idx], currentInstance.rowID + "_" + prop);
                }
            }
        }
        //for dropdown textbox element
        if (currentInstance.repeaterBuilder.design.Elements[idx].Type == 'SelectInput') {
            //var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', currentInstance.rowID);
            //nj var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("getRowData", { rowIndx: currentInstance.rowID });
            var rowData = currentInstance.rowData;

            var inlineRowId = 0;
            for (var prop in rowData) {
                if (prop.substring(0, 4) == 'INL_') {
                    var propArr = prop.split('_');
                    if (propArr.length == 4) {
                        var dsName = propArr[1];
                        var propIdx = propArr[2];
                        var propName = propArr[3];
                        if (propName == currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName) {
                            var idpluscol = "formEdit" + "_INL_" + inlineRowId + "_" + propName;
                            currentInstance.formatDropdownTextbox(idpluscol, currentInstance.repeaterBuilder.design.Elements[idx], currentInstance.rowID + "_INL_" + dsName + "_" + inlineRowId + "_" + propName);
                            inlineRowId++;
                        }
                    }
                }
                else if (prop == currentInstance.repeaterBuilder.design.Elements[idx].GeneratedName) {
                    var idpluscol = "formEdit" + currentInstance.rowID + "_" + prop;
                    currentInstance.formatDropdownTextbox(idpluscol, currentInstance.repeaterBuilder.design.Elements[idx], currentInstance.rowID + "_" + prop);
                }
            }
        }
    }

    if (!($("#repeater" + currentInstance.repeaterBuilder.gridElementId).hasClass('disabled'))) {
        if (currentInstance.repeaterBuilder.fullName != "BenefitReview.BenefitReviewGrid")
            currentInstance.processRules();
    }

    $(currentInstance.elementIDs.repeaterDialogJQ).find('input,select,textarea').each(function (idx, elem) {
        $(elem).on('change', function () {
            var elem = this;
            currentInstance.loadRulesForElement(elem);
        });
        $(elem).on('focusout', function () {
            var elem = this;
            var designElem = currentInstance.repeaterBuilder.design.Elements.filter(function (el) {
                return $(elem).attr('id').indexOf(el.GeneratedName) >= 0;
            });
            if (designElem != null && designElem.length > 0) {
                designElem = designElem[0];
                var rules = currentInstance.repeaterBuilder.formInstanceBuilder.rules.getRulesForElement(designElem.FullName);
                if (rules == null || rules.length <= 0) {
                    currentInstance.loadValidationsForElementPQ(elem);
                    currentInstance.loadDuplicationsForElementPQ(elem);
                }
            }
        });
    });

}

repeaterdialogAG.prototype.loadTinyMCE = function (id) {
    tinymce.remove(id);
    tinymce.init({
        selector: id,
        statusbar: false,
        theme: 'modern',
        forced_root_block: "",
        force_br_newlines: true,
        force_p_newlines: false,
        plugins: [
          'advlist autolink lists charmap print preview hr pagebreak',
          'searchreplace wordcount visualblocks visualchars code',
          'insertdatetime save table directionality',
          'emoticons template powerpaste textcolor colorpicker textpattern imagetools codesample toc',
          'image'
        ],
        menubar: "file edit insert view format table tools",
        toolbar1: 'undo redo | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent',
        toolbar2: 'preview forecolor backcolor | fontselect |  fontsizeselect | CenterAlign | RightAlign',
        image_advtab: true,
        powerpaste_word_import: 'prompt',
        powerpaste_html_import: 'prompt',
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

repeaterdialogAG.prototype.getsubArrays = function (data, subSection, gridDataSourceName) {
    var currentInstance = this;
    for (var key in data) {
        if (key.indexOf("jQuery") <= -1) {
            var item = data[key];
            if (gridDataSourceName == undefined || key != gridDataSourceName) {
                if ($.isArray(item)) {
                    subSection.push(item);
                }
                else if (typeof item == "object") {
                    currentInstance.getsubArrays(item);
                }
            }
        }
    }
    return subSection;
}

repeaterdialogAG.prototype.CreateLayout = function () {
    var currentInstance = this;
    var row = currentInstance.rowData;
    var displayName = currentInstance.elementData;
    var rowId = currentInstance.rowID;
    divRow = "<div class='row'>";
    var i = 0;
    for (var key in row) {
        var displayText = undefined;
        $.each(displayName, function (id, name) {
            if (name.Id == key && name.Visible == true) {
                displayText = name.displayText;

                var elementId = "formEdit" + rowId + "_" + name.Id;

                divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                        + "<span class='staticLabel'><b>" + displayText + "</b></span></div>";

                if (name.Type === "checkbox") {
                    var value = row[key];
                    if (value === "Yes") {
                        divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<input type='checkbox' id='" + elementId + "' checked='checked' class='css-checkboxgrid'  value='Yes'/> <span class='css-span'> </div>";
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<input type='checkbox' id='" + elementId + "' class='css-checkboxgrid'  value='No'/> <span class='css-span'> </div>";
                    }
                }
                else {
                    if (name.Type === "select") {
                        if (displayText != "Limits" && displayText != "Message" && displayText != "Tier" && displayText != "Alt Rule") {
                            /*nj if (((currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.customrule.fullName.benefitReviewGridAltRulesData
                                   || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.customrule.fullName.benefitReviewAltRulesGridTierData
                                   || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.customrule.fullName.benefitReviewGridTierData)
                                 && displayText == "Deductible Accumulator")) {
                                 var benefitSetName = row[currentInstance.repeaterBuilder.customrule.BenefitSetName];
                                 var items = currentInstance.repeaterBuilder.customrule.getDeductibleAccumulatorForBenfitAltAndTierGridOnRowViewMode(currentInstance.repeaterBuilder.formInstanceBuilder, benefitSetName);
 
                                 var value = row[key];
                                 divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                         + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getSelectListOptions(items, value, name.Details.IsSortRequired) + "</select> </div>";
                             }
                             else {*/
                            var value = row[key];
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                    + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getSelectListOptions(name.Items, value, name.Details.IsSortRequired) + "</select> </div>";
                            // }
                        }
                        else {
                            var customElementId = "formEdit" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<span class='ui-icon ui-icon-document viewLimits' title='View " + name.Id + "' style='cursor: pointer;' " +
                                "id='" + rowId + "'></span>"
                        }
                    }
                    else if (name.Type === "SelectInput") {
                        if (displayText != "Limits" && displayText != "Message" && displayText != "Tier" && displayText != "Alt Rule") {
                            var value = row[key];
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                        + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getDropdownTextboxOptions(name.Items, value, name.Details.IsSortRequired) + "</select> </div>";
                        }
                        else {
                            var customElementId = "formEdit" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<span class='ui-icon ui-icon-document ' title='View " + name.Id + "' style='margin:0 auto!important;cursor: pointer;' " + "id='" + customElementId + "'></span>" + " </div>";
                        }
                    }
                    else if (name.Type === "label") {
                        if (displayText != "Alt Rule" && displayText != "Tier") {
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                    + "<label style='font-weight:normal;'>" + row[key] + "</label>" + " </div>";
                        }
                        else {
                            var customElementId = "formEdit" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<span class='ui-icon ui-icon-document' title='View " + name.Id + "' style='margin:0 auto!important;cursor: pointer;' " + "id='" + customElementId + "'></span>" + " </div>";
                        }
                    }
                    else if (name.Type === "calendar") {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='text' id='" + elementId + "' class='form-control calendar' value='" + row[key] + "' style='width:76%!important;'/> </div>";
                    }
                    else if (name.Details.Multiline === true) {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<textarea id='" + elementId + "' wrap='hard' cols='25' rows='5' class='form-control'>" + row[key] + "</textarea></div>";
                    }
                    else {
                        if (displayText != "Service Comment" && displayText != "Tier") {
                            var value = (row[key] == null || row[key] == undefined) ? "" : row[key];
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                    + "<input type='text' id='" + elementId + "' class='form-control' value='" + value + "'/> </div>";
                        }
                        else if (displayText == "Service Comment") {
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<span class='ui-icon ui-icon-document viewBRGServiceComment' title='View " + name.Id + "' style='margin:0 auto!important;cursor: pointer;' " + "id='" + rowId + "''></span>" + "</a>" + " </div>";
                        }
                        else {
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<span class='ui-icon ui-icon-document viewBRGAltRuleTier' title='View " + name.Id + "' style='margin:0 auto!important;cursor: pointer;' " + "id='" + rowId + "'" + "></span>" + "</a>" + " </div>";
                        }
                    }
                }
                return false;
            }
        })

        if ((i + 1) % 3 === 0 && (i + 1) != row.length)
            divRow = divRow + " </div>  <div class='row'> ";
        i++;
    }
    return divRow + "</div>";
}

repeaterdialogAG.prototype.CreateLayoutPQ = function (richTextBoxIds) {
    var currentInstance = this;
    var row = currentInstance.rowData;
    var displayName = currentInstance.elementData;
    var rowId = currentInstance.rowID;
    var hasRichTextbox = false;
    divRow = "<div class='row'>";
    var i = 0; var richTextPosition = -1;
    for (var key in row) {
        var displayText = undefined;
        $.each(displayName, function (id, name) {
            if (name.Id == key && name.Visible == true) {
                displayText = name.displayText;

                var elementId = "formEdit" + rowId + "_" + name.Id;

                divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<span class='staticLabel'><b>" + displayText + "</b></span></div>";

                if (name.Type === "checkbox") {
                    var value = row[key];
                    if (value === "Yes") {
                        divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<input type='checkbox' id='" + elementId + "' checked='checked' class='css-checkboxgrid'  value='Yes'/> <span class='css-span'> </div>";
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<input type='checkbox' id='" + elementId + "' class='css-checkboxgrid'  value='No'/> <span class='css-span'> </div>";
                    }
                }
                else {
                    if (name.Type === "select") {
                        if (displayText != "Limits" && displayText != "Message" && displayText != "Tier" && displayText != "Alt Rule") {
                            /*nj if (((currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.customrule.fullName.benefitReviewGridAltRulesData
                                  || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.customrule.fullName.benefitReviewAltRulesGridTierData
                                  || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.customrule.fullName.benefitReviewGridTierData)
                                && displayText == "Deductible Accumulator")) {
                                var benefitSetName = row[currentInstance.repeaterBuilder.customrule.BenefitSetName];
                                var items = currentInstance.repeaterBuilder.customrule.getDeductibleAccumulatorForBenfitAltAndTierGridOnRowViewMode(currentInstance.repeaterBuilder.formInstanceBuilder, benefitSetName);

                                var value = row[key];
                                divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                        + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getSelectListOptions(items, value, name.Details.IsSortRequired) + "</select> </div>";
                            }
                            else {*/
                            if (((currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.COMMERCIALMEDICAL.SelectProvisions
                               || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.COMMERCIALMEDICAL.BRAGSelectProvisions
                               || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.COMMERCIALMEDICAL.NetworkSelectProvisions
                               || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.RX.RxSelectProductWideProvisions
                               || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.RX.RxSelectProductNetworkWideProvisions
                               || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.RX.RxSelectServiceNetworkWideProvisions
                               || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.DENTAL.DentalSelectProductWideProvisions
                               || currentInstance.repeaterBuilder.fullName == currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.reapterFullName.DENTAL.DentalSelectServiceWideProvisions)
                             && displayText == "Provision Text Options")) {
                                var provosionName = row[currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.repeaterElementName.ProvisionNameSERVICEPROVISION];
                                var items = currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.bindProviderOptionDropdown(currentInstance.repeaterBuilder, row);
                                items = items.Items == undefined ? [] : items.Items;
                                var value = row[key];
                                divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                        + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getSelectListOptions(items, value, name.Details.IsSortRequired) + "</select> </div>";
                            }
                            else if( displayText == "Processing Rule" && 
                                (ebsInstance.reapterFullName.SBCCalculatorView.CoverageExampleDiabetic==currentInstance.repeaterBuilder.fullName ||
                                ebsInstance.reapterFullName.SBCCalculatorView.CoverageExampleFracture==currentInstance.repeaterBuilder.fullName || 
                                ebsInstance.reapterFullName.SBCCalculatorView.CoverageExampleMaternity==currentInstance.repeaterBuilder.fullName))
                            {
                                var items = currentInstance.repeaterBuilder.expressionBuilder.expressionBuilderExtension.filterProcesssingRuleItems(currentInstance.repeaterBuilder, row);
                                items = items.Items == undefined ? [] : items.Items;
                                var value = row[key];
                                divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                        + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getSelectListOptions(items, value, name.Details.IsSortRequired) + "</select> </div>";
                            }
                            else
                            {
                                var value = row[key];
                                divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-8'>"
                                        + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getSelectListOptions(name.Items, value, name.Details.IsSortRequired) + "</select> </div>";
                            }
                        }
                        else {
                            var customElementId = "formEdit" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<span class='ui-icon ui-icon-document viewLimits' title='View " + name.Id + "' style='cursor: pointer;' " +
                                "id='" + rowId + "'></span>"
                        }
                    }
                    else if (name.Type === "SelectInput") {
                        if (displayText != "Limits" && displayText != "Message" && displayText != "Tier" && displayText != "Alt Rule") {
                            var value = row[key];
                            divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                        + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getDropdownTextboxOptions(name.Items, value, name.Details.IsSortRequired) + "</select> </div>";
                        }
                        else {
                            var customElementId = "formEdit" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<span class='ui-icon ui-icon-document ' title='View " + name.Id + "' style='margin:0 auto!important;cursor: pointer;' " + "id='" + customElementId + "'></span>" + " </div>";
                        }
                    }
                    else if (name.Type === "label") {
                        if (displayText != "Alt Rule" && displayText != "Tier") {
                            divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                    + "<label style='font-weight:normal;'>" + row[key] + "</label>" + " </div>";
                        }
                        else {
                            var customElementId = "formEdit" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<span class='ui-icon ui-icon-document' title='View " + name.Id + "' style='margin:0 auto!important;cursor: pointer;' " + "id='" + customElementId + "'></span>" + " </div>";
                        }
                    }
                    else if (name.Type === "calendar") {
                        divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<input type='text' id='" + elementId + "' class='form-control calendar' value='" + row[key] + "' style='width:76%!important;'/> </div>";
                    }
                    else if (name.Details.Multiline === true) {
                        divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<textarea id='" + elementId + "' wrap='hard' cols='25' rows='5' class='form-control'>" + row[key] + "</textarea></div>";
                    }
                    else {
                        if (displayText != "Service Comment" && displayText != "Tier") {
                            var value = (row[key] == null || row[key] == undefined) ? "" : row[key];
                            if (name.Details.IsRichTextBox === true) {
                                divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<textarea id='" + elementId + "' wrap='hard' cols='25' rows='5'>" + row[key] + "</textarea></div></div>";
                                richTextBoxIds.push(elementId);
                            }
                            else {
                                divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                        + "<input type='text' id='" + elementId + "' class='form-control' value='" + value + "'/> </div>";
                            }
                        }
                        else if (displayText == "Service Comment") {
                            divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<span class='ui-icon ui-icon-document viewBRGServiceComment' title='View " + name.Id + "' style='margin:0 auto!important;cursor: pointer;' " + "id='" + rowId + "''></span>" + "</a>" + " </div>";
                        }
                        else {
                            divRow = divRow + "<div class='col-xs-9 col-md-9 col-lg-9 col-sm-9'>"
                                + "<span class='ui-icon ui-icon-document viewBRGAltRuleTier' title='View " + name.Id + "' style='margin:0 auto!important;cursor: pointer;' " + "id='" + rowId + "'" + "></span>" + "</a>" + " </div>";
                        }
                    }
                }
                return false;
            }
        })
        divRow = divRow + " </div>  <div class='row'> ";
        i++;
    }
    return divRow + "</div>";
}

repeaterdialogAG.prototype.CreateInLineLayout = function (row, inlineRowId) {
    var currentInstance = this;
    var displayName = currentInstance.elementData;
    var rowId = currentInstance.rowID;
    var titleText = undefined;
    for (var key in row) {
        $.each(displayName, function (id, name) {
            if (name.Id == key && name.Visible == false) {
                if (titleText != undefined) {
                    titleText = titleText + "-" + row[key];
                }
                else {
                    titleText = row[key];
                }
            }
        });
    }

    if (titleText != undefined) {
        divRow = "<div class='panel panel-default subsection' >" + "<div class='panel-heading-gray' style='background-color:#DDDDDD !important;'>" +
                                          "<h3 class='panel-title'>" + titleText + "</h3>" + "</div>" + "<div class='panel-body'>" +
                                          "<div class='container-fluid' style='min-height:inherit !important'>" + "<div class='row'>";
    }
    else {
        divRow = "<div class='panel panel-default subsection' >" + "<div class='panel-heading-gray' style='background-color:#DDDDDD !important;'>" +
                                          "<h3 class='panel-title'></h3>" + "</div>" + "<div class='panel-body'>" +
                                          "<div class='container-fluid' style='min-height:inherit !important'>" + "<div class='row'>";
    }

    var i = 0;
    for (var key in row) {
        var displayText = undefined;
        $.each(displayName, function (id, name) {
            if (name.Id == key && name.Visible == true) {
                displayText = name.displayText;

                var elementId = "formEdit" + "_INL_" + inlineRowId + "_" + name.Id;

                divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                        + "<span class='staticLabel'><b>" + displayText + "</b></span></div>";

                if (name.Type === "checkbox") {
                    var value = row[key];
                    if (value === "Yes") {
                        if (name.Details.Enabled == true) {
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                    + "<input type='checkbox' id='" + elementId + "' checked='checked' class='css-checkboxgrid' style='margin-left: 45%;' value='Yes'/><span class='css-span'></div>";
                        }
                        else {
                            divRow = divRow + "<div class='col-xs-3'>"
                                 + "<input type='checkbox' id='" + elementId + "' checked='checked' class='css-checkboxgrid' style='margin-left: 45%;' value='Yes' disabled='disabled'/><span class='css-span'></div>";
                        }
                    }
                    else {
                        if (name.Details.Enabled == true) {
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<input type='checkbox' id='" + elementId + "' class='css-checkboxgrid' style='margin-left: 45%;' value='No'/><span class='css-span'></div>";
                        }
                        else {
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<input type='checkbox' id='" + elementId + "' class='css-checkboxgrid' style='margin-left: 45%;' value='No' disabled='disabled'/><span class='css-span'></div>";
                        }
                    }
                }
                else {
                    if (name.Type === "select") {
                        if (displayText != "Limits" && displayText != "Message" && displayText != "Tier" && displayText != "Alt Rule") {
                            var value = row[key];
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                    + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getSelectListOptions(name.Items, value, name.Details.IsSortRequired) + "</select></div>";
                        }
                        else if (displayText == "Limits") {
                            var customElementId = "formEdit" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<span class='ui-icon ui-icon-document viewBRGLimits' title='View' id='" + rowId + "' + index='" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + "'style='margin: 0px auto !important; cursor: pointer;'></span>" + " </div>";
                        }
                        else {
                            var customElementId = "formEdit" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                            + "<span class='ui-icon ui-icon-document ' title='View' id='" + customElementId + "' style='margin: 0px auto !important; cursor: pointer;'></span>" + " </div>";
                        }
                    }
                    else if (name.Type === "SelectInput") {
                        if (displayText != "Limits" && displayText != "Message" && displayText != "Tier" && displayText != "Alt Rule") {
                            var value = row[key];
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                    + "<select id='" + elementId + "'  class='form-control' > " + currentInstance.getDropdownTextboxOptions(name.Items, value, name.Details.IsSortRequired) + "</select></div>";
                        }
                        else {
                            var customElementId = "formEdit" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<span class='ui-icon ui-icon-document' title='View' id='" + customElementId + "' style='margin: 0px auto !important; cursor: pointer;'></span>" + " </div>";
                        }
                    }
                    else if (name.Type === "label") {
                        if (displayText != "Alt Rule" && displayText != "Tier") {
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                    + "<label>" + row[key] + "</label>" + " </div>";
                        }
                        else if (displayText == "Alt Rule") {
                            if (row.AltRule == "Show") {
                                var customElementId = "formEdit" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                                divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                    + "<span class='ui-icon ui-icon-document viewBRGAltRule' title='View' id='" + rowId + "'" + "index='" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + "' style='margin: 0px auto !important; cursor: pointer;'></span>" + " </div>";
                            } else {
                                var customElementId = "formEdit" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                                divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                    + "<span class='ui-icon ui-icon-document ui-state-disabled' data-value='" + rowId + "'" + "index='" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + "' style='margin: 0px auto !important; cursor: pointer;'></span>" + " </div>";
                            }

                        }
                        else if (displayText == "Tier") {
                            var customElementId = "formEdit" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + rowId + currentInstance.repeaterBuilder.formInstanceId;
                            divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<span class='ui-icon ui-icon-document viewBRGTier' title='View' id='" + rowId + "'" + "index='" + "INL_" + currentInstance.inlineGridDataSourceName + "_" + inlineRowId + "_" + name.Id + "' style='margin: 0px auto !important; cursor: pointer;'></span>" + " </div>";
                        }
                    }

                    else if (name.Type === "calendar") {
                        divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<input type='text' id='" + elementId + "' class='form-control calendar' value='" + row[key] + "' style='width:76%!important;'/> </div>";
                    }
                    else if (name.Details.Multiline === true) {
                        divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<textarea id='" + elementId + "' wrap='hard' cols='25' rows='5' class='form-control'>" + row[key] + "</textarea></div>";
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-3 col-md-3 col-lg-3 col-sm-3'>"
                                + "<input type='text' id='" + elementId + "' class='form-control' value=" + row[key] + "></input> </div>";
                    }
                }
                return false;
            }
        });

        if ((i + 1) % 3 === 0 && (i + 1) != row.length)
            divRow = divRow + " </div>  <div class='row'> ";
        i++;
    }
    return divRow + "</div></div></div></div>";
}

repeaterdialogAG.prototype.init = function () {
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

    if (!currentInstance.repeaterBuilder.formInstanceBuilder.IsMasterList) {
        $(currentInstance.elementIDs.viewTemplateDialog).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            minHeight: 250,
            width: 500,
            modal: true,
            position: ['middle', 100]
        });
    }
    else {
        $(currentInstance.elementIDs.viewTemplateDialogML).dialog({
            autoOpen: false,
            resizable: false,
            closeOnEscape: true,
            height: 'auto',
            minHeight: 250,
            width: 500,
            modal: true,
            position: ['middle', 100]
        });
    }
}

repeaterdialogAG.prototype.show = function () {
    var currentInstance = this;

    currentInstance.init();
    $(currentInstance.elementIDs.repeaterRowDataJQ).hide().fadeIn("slow");
    currentInstance.generateRowLayout();

    $(currentInstance.elementIDs.repeaterDialogJQ).dialog('option', 'title', currentInstance.caption);
    $(currentInstance.elementIDs.repeaterDialogJQ).dialog("open");

    currentInstance.showNavigateOptions();

    $(currentInstance.elementIDs.repeaterDialogPrevButton).unbind("click");
    $(currentInstance.elementIDs.repeaterDialogPrevButton).bind("click", function () {
        var gridArr = $(currentInstance.repeaterBuilder.gridElementIdJQ).getDataIDs();
        var selrow = $(currentInstance.repeaterBuilder.gridElementIdJQ).getGridParam("selrow");
        var currIndex = 0;
        for (var i = 0; i < gridArr.length; i++) {
            if (gridArr[i] == selrow) {
                currIndex = parseInt(gridArr[i - 1]);
                break;
            }
        }

        var prevRowData = currentInstance.repeaterData.filter(function (dt) {
            return dt.RowIDProperty == currIndex;
        });

        if (prevRowData[0] != undefined) {
            prevRowData = prevRowData[0];
        }

        ajaxDialog.showPleaseWait();
        setTimeout(function () {
            if (prevRowData != undefined) {
                var prevRowID = parseInt(prevRowData.RowIDProperty);
                currentInstance.repeaterBuilder.viewRowData(prevRowID, currentInstance.isRowReadOnly);
            }
            ajaxDialog.hidePleaseWait();
        }, 100);
    });


    $(currentInstance.elementIDs.repeaterDialogNextButton).unbind("click");
    $(currentInstance.elementIDs.repeaterDialogNextButton).bind("click", function () {
        //nj  var gridArr = $(currentInstance.repeaterBuilder.gridElementIdJQ).getDataIDs();
        var selrow = $(currentInstance.repeaterBuilder.gridElementIdJQ).getGridParam("selrow");
        var currIndex = 0;
        /* nj   for (var i = 0; i < gridArr.length; i++) {
             if (gridArr[i] == selrow) {
                 currIndex = parseInt(gridArr[i + 1]);
                 break;
             }
         }*/
        var nextRowData = currentInstance.repeaterData.filter(function (dt) {
            return dt.RowIDProperty == currIndex;
        });

        if (nextRowData[0] != undefined) {
            nextRowData = nextRowData[0];
        }

        ajaxDialog.showPleaseWait();
        setTimeout(function () {
            if (nextRowData != undefined) {
                var nextRowID = parseInt(nextRowData.RowIDProperty);
                currentInstance.repeaterBuilder.viewRowData(nextRowID, currentInstance.isRowReadOnly);
            }
            ajaxDialog.hidePleaseWait();
        }, 100);
    });

    $(currentInstance.elementIDs.repeaterDialogSaveButton).unbind("click");
    if (!currentInstance.isRowReadOnly) {
        $(currentInstance.elementIDs.repeaterDialogSaveButton).bind("click", function () {

            var isValidations = $(currentInstance.elementIDs.repeaterDialogJQ + " .has-error");
            if (isValidations.length > 0) {
                messageDialog.show("Please check Validations first.");
            }
            else {
                //var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', currentInstance.rowID);
                if (currentInstance.repeaterBuilder.design.LoadFromServer == true) {
                    var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', currentInstance.rowID);
                }
                else {
                    var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getLocalRow', currentInstance.rowID);
                }

                for (var prop in rowData) {
                    if (prop.substring(0, 4) != 'INL_' && prop != "RowIDProperty") {
                        var idpluscol = "formEdit" + currentInstance.rowID + "_" + prop;
                        var idpluscolVal = undefined;
                        if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).length > 0) {
                            if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].type == "checkbox") {
                                if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].checked == true) {
                                    idpluscolVal = "Yes";
                                }
                                else {
                                    idpluscolVal = "No";
                                }
                            }
                            else if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].type == "select-one") {
                                if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val() != "Select One") {
                                    idpluscolVal = $(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val();
                                }
                                else {
                                    idpluscolVal = "";
                                }
                            }
                            else {
                                idpluscolVal = $(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val();
                            }
                        }
                        if (idpluscolVal != undefined) {
                            var oldValue = rowData[prop];
                            rowData[prop] = idpluscolVal;
                            if (oldValue != idpluscolVal) {
                                var colName = prop; "Limit Amount - Salary Percent"
                                colName = currentInstance.repeaterBuilder.columnNames.filter(function (dt) {
                                    return dt.replace('<font color=red>*</font>', '').replace('-', '').replace(/ /g, '').trim() == colName;
                                });
                                colName = colName[0];
                                currentInstance.repeaterBuilder.addEntryToAcitivityLogger(rowData.RowIDProperty - 1, Operation.UPDATE, colName, oldValue, idpluscolVal);
                            }
                        }
                    }
                }
                var inlineRowId = 0;
                for (var prop in rowData) {
                    if (prop.substring(0, 4) == 'INL_') {
                        var propArr = prop.split('_');
                        if (propArr.length == 4) {
                            var dsName = propArr[1];
                            var propIdx = propArr[2];
                            var propName = propArr[3];
                            var idpluscol = "formEdit" + "_INL_" + prop.split('_')[2] + "_" + propName;
                            var idpluscolVal = undefined;
                            if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).length > 0) {
                                if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].type == "checkbox") {
                                    if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].checked == true) {
                                        idpluscolVal = "Yes";
                                    }
                                    else {
                                        idpluscolVal = "No";
                                    }
                                }
                                else if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].type == "select-one") {
                                    if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val() != "Select One") {
                                        idpluscolVal = $(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val();
                                    }
                                    else {
                                        idpluscolVal = "";
                                    }
                                }
                                else {
                                    idpluscolVal = $(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val();
                                }
                            }
                            if (idpluscolVal != undefined) {
                                var oldValue = rowData[prop];
                                rowData[prop] = idpluscolVal;
                                if (oldValue != idpluscolVal) {
                                    var colName = propName;
                                    colName = currentInstance.repeaterBuilder.columnNames.filter(function (dt) {
                                        return dt.replace('<font color=red>*</font>', '').replace('-', '').replace(/ /g, '').trim() == colName;
                                    });
                                    colName = colName[0];
                                    currentInstance.repeaterBuilder.addEntryToAcitivityLogger(rowData.RowIDProperty - 1, Operation.UPDATE, colName, oldValue, idpluscolVal);
                                }
                            }
                        }
                    }
                }


                ajaxDialog.showPleaseWait();
                currentInstance.invokeCallback(rowData, currentInstance.rowID);
                $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");
                ajaxDialog.hidePleaseWait();
            }

        });
        $(currentInstance.elementIDs.repeaterDialogSaveButton).prop('disabled', false).removeClass("ui-state-disabled");
    }
    else {
        $(currentInstance.elementIDs.repeaterDialogSaveButton).prop("disabled", true).addClass("ui-state-disabled");
        $(currentInstance.elementIDs.repeaterDialogJQ).find('input, select, img.ui-datepicker-trigger, textarea').prop('disabled', true);
        $(currentInstance.elementIDs.repeaterDialogJQ).find('.ui-search-input input').prop('disabled', false);
    }

    $(currentInstance.elementIDs.repeaterDialogCancelButton).unbind("click");
    $(currentInstance.elementIDs.repeaterDialogCancelButton).bind("click", function () {
        $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");
    });

    //if (currentInstance.repeaterBuilder.customrule.hasCustomRules(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == "BenefitReview.BenefitReviewGrid") {
    //    $(".viewLimits").click(function () {
    //        var element = $(this).attr("Id");
    //        currentInstance.repeaterBuilder.customrule.showLimitPouUp(element, currentInstance.repeaterBuilder.data, currentInstance.repeaterBuilder.tenantId, currentInstance.repeaterBuilder.formInstanceId, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId);
    //    });
    //}
    if (currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId == FormTypes.PRODUCTFORMDESIGNID) {
        if (currentInstance.repeaterBuilder.customrule.hasProduct(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == "BenefitReview.BenefitReviewGrid") {
            $(" .viewBRGServiceComment").click(function () {
                currentInstance.repeaterBuilder.saveRow();
                var element = $(this).attr("Id");
                var benefitSetName = "In Network";// griddata["INL_" + colModelArray[1] + "_" + colModelArray[2] + "_" + currentInstance.customrule.BenefitSetName];
                currentInstance.repeaterBuilder.customrule.showServiceCommentPouUp(element, currentInstance.repeaterBuilder.data, currentInstance.repeaterBuilder.tenantId, currentInstance.repeaterBuilder.formInstanceId, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId, "");
            });

            $(".viewBRGAltRule").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                //currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var data = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', element);
                setTimeout(function () {
                    currentInstance.repeaterBuilder.customrule.showAltRulePopUp(element, isCurrentRowReadOnly, data, currentInstance.repeaterBuilder, colModel);
                }, 5);
            });

            $(".viewBRGTier").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                //currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var colModelArray = colModel.split('_');
                var data = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', element);
                setTimeout(function () {
                    currentInstance.repeaterBuilder.customrule.showTierDataPopUp(element, isCurrentRowReadOnly, data, currentInstance.repeaterBuilder, colModelArray[1], colModelArray[2]);
                }, 5)
            });
            $(".viewBRGLimits").click(function () {
                //currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var griddata = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', element);
                var colModelArray = colModel.split('_');
                var benefitSetName = griddata["INL_" + colModelArray[1] + "_" + colModelArray[2] + "_" + currentInstance.repeaterBuilder.customrule.BenefitSetName];
                currentInstance.repeaterBuilder.customrule.showLimitPouUp(element, currentInstance.repeaterBuilder.data, currentInstance.repeaterBuilder.tenantId, currentInstance.repeaterBuilder.formInstanceId, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId, benefitSetName);
            });
        }
        if (currentInstance.repeaterBuilder.customrule.hasProduct(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == "BenefitReview.BenefitReviewAltRulesGrid") {
            $(" .viewBRGAltRuleTier").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                var element = $(this).attr("Id");
                var data = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', element);
                setTimeout(function () {
                    currentInstance.repeaterBuilder.customrule.showAltRuleTierDataPopUp(element, isCurrentRowReadOnly, data, currentInstance.repeaterBuilder);
                }, 5)
            });
        }
    }
}
repeaterdialogAG.prototype.getGridInstance = function (currentInstance, isNext) {
    var gridOptions = $(currentInstance.repeaterBuilder.gridElementIdJQ)[0].gridOptions;

    var rowCount = gridOptions.api.getDisplayedRowCount() - 1;

    var currentIndex = gridOptions.api.getSelectedNodes()[0].rowIndex;



    var lastGridIndex = rowCount - 1;
    var currentPage = gridOptions.api.paginationGetCurrentPage();
    var pageSize = gridOptions.api.paginationGetPageSize();
    var startPageIndex = currentPage * pageSize;

    var currentPg = currentPage;

    if (isNext == true) {
        currentPg++;
    }

    var endPageIndex = (currentPg * pageSize) - 1;

    if (isNext == true) {
        if (currentIndex <= rowCount) {
            endPageIndex++;
            currentIndex++;
        }
    }
    else

        if (currentIndex > 0) {
            currentIndex--;
        }
    var nextRowData = gridOptions.api.getDisplayedRowAtIndex(currentIndex)
    nextRowData.setSelected(true, true);

    if (endPageIndex == currentIndex) {

        if (isNext == true) {
            currentPage++;
        }
        else {
            currentPage--;
        }
        gridOptions.api.paginationGoToPage(currentPage);
    }
    return nextRowData;
}
repeaterdialogAG.prototype.showPQ = function (hasSavebutton) {
    var currentInstance = this;
    currentInstance.init();
    $(currentInstance.elementIDs.repeaterRowDataJQ).hide().fadeIn("slow");
    currentInstance.generateRowLayoutPQ();

    $(currentInstance.elementIDs.repeaterDialogJQ).dialog('option', 'title', currentInstance.caption);
    $(currentInstance.elementIDs.repeaterDialogJQ).dialog("open");

    currentInstance.showNavigateOptionsPQ();

    $(currentInstance.elementIDs.repeaterDialogPrevButton).unbind("click");
    $(currentInstance.elementIDs.repeaterDialogPrevButton).bind("click", function () {
        /*nj var gridArr = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("option", "dataModel.data");
        var selrow = $(currentInstance.repeaterBuilder.gridElementIdJQ).getGridParam("selrow");
        var currIndex = 0;
        for (var i = 0; i < gridArr.length; i++) {
            if (gridArr[i].pq_rowselect) {
                currIndex = parseInt(gridArr[i - 1].RowIDProperty);
                break;
            }
        }

        var prevRowData = currentInstance.repeaterData.filter(function (dt) {
            return dt.RowIDProperty == currIndex;
        });

        if (prevRowData[0] != undefined) {
            prevRowData = prevRowData[0];
        }
    */
        var prevRowData = currentInstance.getGridInstance(currentInstance, false);


        ajaxDialog.showPleaseWait();
        setTimeout(function () {
            if (prevRowData != undefined) {
                var prevRowID = parseInt(prevRowData.data.RowIDProperty);
                currentInstance.repeaterBuilder.viewRowDataPQ(prevRowID, currentInstance.isRowReadOnly, hasSavebutton);
            }
            ajaxDialog.hidePleaseWait();
        }, 100);
    });


    $(currentInstance.elementIDs.repeaterDialogNextButton).unbind("click");
    $(currentInstance.elementIDs.repeaterDialogNextButton).bind("click", function () {
        //nj var gridArr = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("option", "dataModel.data");
        /*
                var currIndex = 0;
                for (var i = 0; i < gridArr.length; i++) {
                    if (gridArr[i].pq_rowselect) {
                        currIndex = parseInt(gridArr[i + 1].RowIDProperty);
                        break;
                    }
                }
        
                var nextRowData = currentInstance.repeaterData.filter(function (dt) {
                    return dt.RowIDProperty == currIndex;
                });
        
                if (nextRowData[0] != undefined) {
                    nextRowData = nextRowData[0];
                }*/
        var nextRowData = currentInstance.getGridInstance(currentInstance, true);

        ajaxDialog.showPleaseWait();
        setTimeout(function () {
            if (nextRowData != undefined) {
                var nextRowID = parseInt(nextRowData.data.RowIDProperty);
                currentInstance.repeaterBuilder.viewRowDataPQ(nextRowID, currentInstance.isRowReadOnly, hasSavebutton);
            }
            ajaxDialog.hidePleaseWait();
        }, 100);
    });

    // In view mode disable all save button 
    if (hasSavebutton) {
        $(currentInstance.elementIDs.repeaterDialogSaveButton).css("visibility", "visible");
        $(currentInstance.elementIDs.repeaterDialogSaveButton).unbind("click");
        if (!currentInstance.isRowReadOnly) {
            $(currentInstance.elementIDs.repeaterDialogSaveButton).bind("click", function () {

                var isValidations = $(currentInstance.elementIDs.repeaterDialogJQ + " .has-error");
                if (isValidations.length > 0) {
                    messageDialog.show("Please check Validations first.");
                }
                else {
                    //var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', currentInstance.rowID);
                    if (currentInstance.repeaterBuilder.design.LoadFromServer == true) {
                        //var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', currentInstance.rowID);
                        //var rowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("getRowData", { rowIndx: currentInstance.rowID });
                        var gridOptions = GridApi.getCurrentGridInstanceById(currentInstance.repeaterBuilder.gridElementIdJQ).gridOptions;
                        var row = gridOptions.api.getSelectedNodes()

                        if (row && row.length > 0 && row[0].data != undefined) {
                            rowData = row[0].data;
                        }
                    }
                    else {

                        /*nj var gridPQData = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("option", "dataModel.data");
 
                         var rowData = gridPQData.filter(function (dt) {
                             return dt.RowIDProperty == currentInstance.rowID;
                         })
 
                         var rowData = rowData[0];         */
                        var rowData = {};

                        var gridOptions = GridApi.getCurrentGridInstanceById(currentInstance.repeaterBuilder.gridElementIdJQ).gridOptions;
                        var row = gridOptions.api.getSelectedNodes()

                        if (row && row.length > 0 && row[0].data != undefined) {
                            rowData = row[0].data;
                        }


                    }

                    for (var prop in rowData) {
                        if (prop.substring(0, 4) != 'INL_' && prop != "RowIDProperty") {
                            var idpluscol = "formEdit" + currentInstance.rowID + "_" + prop;
                            var idpluscolVal = undefined;
                            if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).length > 0) {
                                if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].type == "checkbox") {
                                    if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].checked == true) {
                                        idpluscolVal = "Yes";
                                    }
                                    else {
                                        idpluscolVal = "No";
                                    }
                                }
                                else if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].type == "select-one") {
                                    if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val() != "Select One") {
                                        idpluscolVal = $(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val();
                                    }
                                    else {
                                        idpluscolVal = "";
                                    }
                                }
                                else {
                                    idpluscolVal = $(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val();
                                }
                            }
                            if (idpluscolVal != undefined) {
                                var oldValue = rowData[prop];
                                rowData[prop] = idpluscolVal;
                                if (oldValue != idpluscolVal) {
                                    var colName = prop; "Limit Amount - Salary Percent"
                                    //colName = currentInstance.repeaterBuilder.columnNames.filter(function (dt) {
                                    //    return dt.title.replace('<font color=red>*</font>', '').replace('-', '').replace(/ /g, '').trim() == colName;
                                    //});
                                    //colName = colName[0];
                                    currentInstance.repeaterBuilder.addEntryToAcitivityLogger(rowData.RowIDProperty, Operation.UPDATE, colName, oldValue, idpluscolVal);
                                }
                            }
                        }
                    }
                    var inlineRowId = 0;
                    for (var prop in rowData) {
                        if (prop.substring(0, 4) == 'INL_') {
                            var propArr = prop.split('_');
                            if (propArr.length == 4) {
                                var dsName = propArr[1];
                                var propIdx = propArr[2];
                                var propName = propArr[3];
                                var idpluscol = "formEdit" + "_INL_" + prop.split('_')[2] + "_" + propName;
                                var idpluscolVal = undefined;
                                if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).length > 0) {
                                    if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].type == "checkbox") {
                                        if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].checked == true) {
                                            idpluscolVal = "Yes";
                                        }
                                        else {
                                            idpluscolVal = "No";
                                        }
                                    }
                                    else if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol)[0].type == "select-one") {
                                        if ($(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val() != "Select One") {
                                            idpluscolVal = $(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val();
                                        }
                                        else {
                                            idpluscolVal = "";
                                        }
                                    }
                                    else {
                                        idpluscolVal = $(currentInstance.elementIDs.repeaterRowDataJQ).find("#" + idpluscol).val();
                                    }
                                }
                                if (idpluscolVal != undefined) {
                                    var oldValue = rowData[prop];
                                    rowData[prop] = idpluscolVal;
                                    if (oldValue != idpluscolVal) {
                                        var colName = propName;
                                        colName = currentInstance.repeaterBuilder.columnNames.filter(function (dt) {
                                            return dt.replace('<font color=red>*</font>', '').replace('-', '').replace(/ /g, '').trim() == colName;
                                        });
                                        colName = colName[0];
                                        currentInstance.repeaterBuilder.addEntryToAcitivityLogger(rowData.RowIDProperty, Operation.UPDATE, colName, oldValue, idpluscolVal);
                                    }
                                }
                            }
                        }
                    }

                    var richTextBoxElement = currentInstance.repeaterBuilder.design.Elements.filter(function (val) {
                        if (val.IsRichTextBox == true) {

                            return val.GeneratedName;
                        }
                    });

                    $.each(richTextBoxElement, function (index, item) {


                        if (rowData[item.GeneratedName].indexOf('<em>') > -1) {

                            val = rowData[item.GeneratedName].replace(/<em>/g, "<i>");
                            val = val.replace(/<\/em>/g, "<\/i>");
                            rowData[item.GeneratedName] = val;

                        }

                    });

                    currentInstance.invokeCallback(rowData, currentInstance.rowID);
                    //$(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("refreshDataAndView");
                    $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");
                }

            });
            $(currentInstance.elementIDs.repeaterDialogSaveButton).prop('disabled', false).removeClass("ui-state-disabled");
        }
        else {
            $(currentInstance.elementIDs.repeaterDialogSaveButton).prop("disabled", true).addClass("ui-state-disabled");
            $(currentInstance.elementIDs.repeaterDialogJQ).find('input, select, img.ui-datepicker-trigger, textarea').prop('disabled', true);
            $(currentInstance.elementIDs.repeaterDialogJQ).find('.ui-search-input input').prop('disabled', false);
        }
    }
    else {
        $(currentInstance.elementIDs.repeaterDialogSaveButton).css("visibility", "hidden");
        $(currentInstance.elementIDs.repeaterDialogJQ).find('input, select, img.ui-datepicker-trigger, textarea').prop('disabled', true);
        $(currentInstance.elementIDs.repeaterDialogJQ).find('.ui-search-input input').prop('disabled', false);
    }
    $(currentInstance.elementIDs.repeaterDialogCancelButton).unbind("click");
    $(currentInstance.elementIDs.repeaterDialogCancelButton).bind("click", function () {
        $(currentInstance.elementIDs.repeaterDialogJQ).dialog("close");
    });

    //if (currentInstance.repeaterBuilder.customrule.hasCustomRules(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == "BenefitReview.BenefitReviewGrid") {
    //    $(".viewLimits").click(function () {
    //        var element = $(this).attr("Id");
    //        currentInstance.repeaterBuilder.customrule.showLimitPouUp(element, currentInstance.repeaterBuilder.data, currentInstance.repeaterBuilder.tenantId, currentInstance.repeaterBuilder.formInstanceId, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId);
    //    });
    //}
    if (currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId == FormTypes.PRODUCTFORMDESIGNID) {
        if (currentInstance.repeaterBuilder.customrule.hasProduct(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == "BenefitReview.BenefitReviewGrid") {
            $(" .viewBRGServiceComment").click(function () {
                currentInstance.repeaterBuilder.saveRow();
                var element = $(this).attr("Id");
                var benefitSetName = "In Network";// griddata["INL_" + colModelArray[1] + "_" + colModelArray[2] + "_" + currentInstance.customrule.BenefitSetName];
                currentInstance.repeaterBuilder.customrule.showServiceCommentPouUp(element, currentInstance.repeaterBuilder.data, currentInstance.repeaterBuilder.tenantId, currentInstance.repeaterBuilder.formInstanceId, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId, "");
            });

            $(".viewBRGAltRule").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                //currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var data = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', element);
                setTimeout(function () {
                    currentInstance.repeaterBuilder.customrule.showAltRulePopUp(element, isCurrentRowReadOnly, data, currentInstance.repeaterBuilder, colModel);
                }, 5);
            });

            $(".viewBRGTier").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                //currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var colModelArray = colModel.split('_');
                var data = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', element);
                setTimeout(function () {
                    currentInstance.repeaterBuilder.customrule.showTierDataPopUp(element, isCurrentRowReadOnly, data, currentInstance.repeaterBuilder, colModelArray[1], colModelArray[2]);
                }, 5)
            });
            $(".viewBRGLimits").click(function () {
                //currentInstance.saveRow();
                var element = $(this).attr("Id");
                var colModel = $(this).attr("index");
                var griddata = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', element);
                var colModelArray = colModel.split('_');
                var benefitSetName = griddata["INL_" + colModelArray[1] + "_" + colModelArray[2] + "_" + currentInstance.repeaterBuilder.customrule.BenefitSetName];
                currentInstance.repeaterBuilder.customrule.showLimitPouUp(element, currentInstance.repeaterBuilder.data, currentInstance.repeaterBuilder.tenantId, currentInstance.repeaterBuilder.formInstanceId, currentInstance.repeaterBuilder.formInstanceBuilder.formDesignVersionId, currentInstance.repeaterBuilder.formInstanceBuilder.folderVersionId, benefitSetName);
            });
        }
        if (currentInstance.repeaterBuilder.customrule.hasProduct(currentInstance.repeaterBuilder.formInstanceBuilder.formDesignId) && currentInstance.repeaterBuilder.fullName == "BenefitReview.BenefitReviewAltRulesGrid") {
            $(" .viewBRGAltRuleTier").click(function () {
                var isCurrentRowReadOnly = $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('tr').prop('disabled') || $(currentInstance.repeaterBuilder.gridElementIdJQ).closest('.repeater-grid').hasClass('disabled');
                var element = $(this).attr("Id");
                var data = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData', element);
                setTimeout(function () {
                    currentInstance.repeaterBuilder.customrule.showAltRuleTierDataPopUp(element, isCurrentRowReadOnly, data, currentInstance.repeaterBuilder);
                }, 5)
            });
        }
    }
}

repeaterdialogAG.prototype.showRepeaterTemplate = function () {
    var currentInstance = this;
    currentInstance.init();
    if (!currentInstance.repeaterBuilder.formInstanceBuilder.IsMasterList) {
        $(currentInstance.elementIDs.viewTemplateDialog).dialog('option', 'title', 'View Repeater Template');
        $(currentInstance.elementIDs.viewTemplateDialog).dialog('open');
    }
    else {
        $(currentInstance.elementIDs.viewTemplateDialogML).dialog('option', 'title', 'View Repeater Template');
        $(currentInstance.elementIDs.viewTemplateDialogML).dialog('open');
    }
    this.viewTemplateData();
}

repeaterdialogAG.prototype.viewTemplateData = function () {
    var currentInstance = this;
    if (currentInstance.repeaterBuilder != undefined && currentInstance.repeaterBuilder != null && currentInstance.repeaterBuilder.design.RepeaterUIElementProperties != undefined
        && currentInstance.repeaterBuilder.design.RepeaterUIElementProperties != null) {
        var data = currentInstance.repeaterBuilder.design.RepeaterUIElementProperties;
        var rowTemp = data.RowTemplate; var headerTemp = data.HeaderTemplate; var footerTemp = data.FooterTemplate;
        if (rowTemp == null || rowTemp == undefined) rowTemp = "";
        if (headerTemp == null || headerTemp == undefined) headerTemp = "";
        if (footerTemp == null || footerTemp == undefined) footerTemp = "";

        var mathces = /\{\d+\}/g;
        // rowTemp
        var result = mathces.exec(rowTemp);
        do {
            if (result != null) {
                var key = result[0].replace(/\{/, '').replace(/\}/, '');
                var value = currentInstance.elementData[key] ? currentInstance.rowData[currentInstance.elementData[key].Id] : "";
                rowTemp = rowTemp.replace(result[0], value);
                result = mathces.exec(rowTemp);
            }
        }
        while (result)

        // headerTemp
        result = mathces.exec(headerTemp);
        do {
            if (result != null) {
                var key = result[0].replace(/\{/, '').replace(/\}/, '');
                var value = currentInstance.elementData[key] ? currentInstance.rowData[currentInstance.elementData[key].Id] : "";
                headerTemp = headerTemp.replace(result[0], value);
                result = mathces.exec(headerTemp);
            }
        }
        while (result)

        // footerTemp
        result = mathces.exec(footerTemp);
        do {
            if (result != null) {
                var key = result[0].replace(/\{/, '').replace(/\}/, '');
                var value = currentInstance.elementData[key] ? currentInstance.rowData[currentInstance.elementData[key].Id] : "";
                footerTemp = footerTemp.replace(result[0], value);
                result = mathces.exec(footerTemp);
            }
        }
        while (result)
        if (!currentInstance.repeaterBuilder.formInstanceBuilder.IsMasterList) {
            $(currentInstance.elementIDs.viewTemplateDialog)[0].innerHTML = "";
            $(currentInstance.elementIDs.viewTemplateDialog).append(headerTemp + "<br/>");
            $(currentInstance.elementIDs.viewTemplateDialog).append(rowTemp + "<br/>");
            $(currentInstance.elementIDs.viewTemplateDialog).append(footerTemp);
        }
        else {
            $(currentInstance.elementIDs.viewTemplateDialogML)[0].innerHTML = "";
            $(currentInstance.elementIDs.viewTemplateDialogML).append(headerTemp + "<br/>");
            $(currentInstance.elementIDs.viewTemplateDialogML).append(rowTemp + "<br/>");
            $(currentInstance.elementIDs.viewTemplateDialogML).append(footerTemp);
        }
    }
}

repeaterdialogAG.prototype.showNavigateOptions = function () {
    var currentInstance = this;
    var rowIds = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getRowData')

    var firstRowId = rowIds[0].RowIDProperty;
    var lastRowId = rowIds[rowIds.length - 1].RowIDProperty;

    if (firstRowId == lastRowId) {
        $(currentInstance.elementIDs.repeaterDialogPrevButton).attr("disabled", "disabled");
        $(currentInstance.elementIDs.repeaterDialogNextButton).attr("disabled", "disabled");
    }
    else if (currentInstance.rowID == firstRowId) {
        $(currentInstance.elementIDs.repeaterDialogPrevButton).attr("disabled", "disabled");
        $(currentInstance.elementIDs.repeaterDialogNextButton).removeAttr("disabled");
    }
    else if (currentInstance.rowID == lastRowId) {
        $(currentInstance.elementIDs.repeaterDialogNextButton).attr("disabled", "disabled");
        $(currentInstance.elementIDs.repeaterDialogPrevButton).removeAttr("disabled");
    }
    else {
        $(currentInstance.elementIDs.repeaterDialogPrevButton).removeAttr("disabled");
        $(currentInstance.elementIDs.repeaterDialogNextButton).removeAttr("disabled");
    }

    $(currentInstance.elementIDs.repeaterPreviousBtnContainer).css("margin-top", Math.max(0, (($(currentInstance.elementIDs.repeaterDialogJQ).height() - $(currentInstance.elementIDs.repeaterPreviousBtnContainer).innerHeight()) / 2) +
                                                $(currentInstance.elementIDs.repeaterDialogJQ).scrollTop()) + "px");

    $(currentInstance.elementIDs.repeaterNextBtnContainer).css("margin-top", Math.max(0, (($(currentInstance.elementIDs.repeaterDialogJQ).height() - $(currentInstance.elementIDs.repeaterNextBtnContainer).innerHeight()) / 2) +
                                                $(currentInstance.elementIDs.repeaterDialogJQ).scrollTop()) + "px");
}
repeaterdialogAG.prototype.showNavigateOptionsPQ = function () {
    var currentInstance = this;
    //nj  var rowIds = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("option", "dataModel.data");
    var rowIds = this.repeaterData;

    var firstRowId = rowIds[0].RowIDProperty;
    var lastRowId = rowIds[rowIds.length - 1].RowIDProperty;

    if (firstRowId == lastRowId) {
        $(currentInstance.elementIDs.repeaterDialogPrevButton).attr("disabled", "disabled");
        $(currentInstance.elementIDs.repeaterDialogNextButton).attr("disabled", "disabled");
    }

    //var firstRowId = rowIds[0].RowIDProperty;
    //var lastRowId = rowIds[rowIds.length - 1].RowIDProperty;

    //if (firstRowId == lastRowId) {
    //    $(currentInstance.elementIDs.repeaterDialogPrevButton).attr("disabled", "disabled");
    //    $(currentInstance.elementIDs.repeaterDialogNextButton).attr("disabled", "disabled");
    //}
    //else if (currentInstance.rowID == firstRowId) {
    //    $(currentInstance.elementIDs.repeaterDialogPrevButton).attr("disabled", "disabled");
    //    $(currentInstance.elementIDs.repeaterDialogNextButton).removeAttr("disabled");
    //}
    //else if (currentInstance.rowID == lastRowId) {
    //    $(currentInstance.elementIDs.repeaterDialogNextButton).attr("disabled", "disabled");
    //    $(currentInstance.elementIDs.repeaterDialogPrevButton).removeAttr("disabled");
    //}
    //else {
    //    $(currentInstance.elementIDs.repeaterDialogPrevButton).removeAttr("disabled");
    //    $(currentInstance.elementIDs.repeaterDialogNextButton).removeAttr("disabled");
    //}

    $(currentInstance.elementIDs.repeaterPreviousBtnContainer).css("margin-top", Math.max(0, (($(currentInstance.elementIDs.repeaterDialogJQ).height() - $(currentInstance.elementIDs.repeaterPreviousBtnContainer).innerHeight()) / 2) +
                                                $(currentInstance.elementIDs.repeaterDialogJQ).scrollTop()) + "px");

    $(currentInstance.elementIDs.repeaterNextBtnContainer).css("margin-top", Math.max(0, (($(currentInstance.elementIDs.repeaterDialogJQ).height() - $(currentInstance.elementIDs.repeaterNextBtnContainer).innerHeight()) / 2) +
                                                $(currentInstance.elementIDs.repeaterDialogJQ).scrollTop()) + "px");
}
repeaterdialogAG.prototype.loadChildGrid = function (childGridData) {
    var currentInstance = this;
    var colArray = [];
    var colModel = [];

    $.each(currentInstance.elementData, function (index, element) {
        if (childGridData.length > 0) {
            for (key in childGridData[0][0]) {
                if (element.Id == key && element.Visible) {
                    colArray.push(element.displayText);
                    if (element.Type == 'checkbox') {
                        colModel.push({ name: element.Id, index: element.Id, editable: false, align: 'left', formatter: 'checkbox' });
                    }
                    else if (element.Type == 'select') {
                        colModel.push({ name: element.Id, index: element.Id, editable: false, align: 'left', formatter: 'select' });
                    }
                    else {
                        colModel.push({ name: element.Id, index: element.Id, editable: false, align: 'left' });
                    }
                }
            }
        }
    });

    $(currentInstance.elementIDs.childGridJQ).jqGrid('GridUnload');
    $(currentInstance.elementIDs.childGridJQ).parent().append("<div id='p" + currentInstance.elementIDs.childGrid + "'></div>");
    $(currentInstance.elementIDs.childGridJQ).jqGrid({
        datatype: 'local',
        data: childGridData[0],
        cache: false,
        colNames: colArray,
        colModel: colModel,
        caption: currentInstance.childGridDataSourceName,
        height: '175',
        rowNum: 10000,
        ignoreCase: true,
        loadonce: true,
        shrinkToFit: true,
        viewrecords: true,
        hidegrid: false,
        altRows: true,
        pager: '#p' + currentInstance.elementIDs.childGrid,
        altclass: 'alternate'

    });
    var pagerElement = '#p' + currentInstance.elementIDs.childGrid;
    $(currentInstance.elementIDs.childGridJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, search: false });
    $(pagerElement).find(pagerElement + '_center').remove();
    $(pagerElement).find(pagerElement + '_right').remove();
    $(currentInstance.elementIDs.childGridJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: false, defaultSearch: "cn" });
}

repeaterdialogAG.prototype.getSelectListOptions = function (items, selectedValue, isSortRequired) {
    if (isSortRequired == true) {
        items = sortDropDownElementItems(items);
    }
    var options = "<option value='Select One'>Select One</option>";
    if (items) {
        if (items.length > 0) {
            $.each(items, function (idx, value) {
                options += "<option value='" + value.ItemValue + "' " + (selectedValue === value.ItemValue ? "selected" : "") + ">" + value.ItemText + "</option>";
            });
        }
    }
    return options;
}

repeaterdialogAG.prototype.getDropdownTextboxOptions = function (items, selectedValue, isSortRequired) {
    if (isSortRequired == true) {
        items = sortDropDownElementItems(items);
    }
    var isValuePresentInItems = items.filter(function (ct) {
        return ct.ItemValue == selectedValue;
    });
    var options = "<option class='standard-optn' value='Select One'>Select One</option>";
    if (items) {
        if (items.length > 0) {
            $.each(items, function (idx, value) {
                options += "<option value='" + value.ItemValue + "' " + (selectedValue === value.ItemValue ? "selected " : "") + "class='standard-optn'>" + value.ItemValue + "</option>";
            });
        }
        if (isValuePresentInItems != undefined && isValuePresentInItems.length == 0) {
            if (selectedValue != undefined) {
                options += '<option selected value="' + selectedValue + '" class="non-standard-optn">' + selectedValue + '</option>';
            }
        }
        options += "<option class='standard-optn' value='newItem'>Enter Unique Response</option>";
    }
    return options;
}

repeaterdialogAG.prototype.pickDates = function (idpluscol, elementDesign) {
    var currentInstance = this;
    $("#" + idpluscol, this.elementIDs.repeaterRowDataJQ).width('76%');
    var minDate = "", maxDate = "", defaultDate = "";
    minDate = elementDesign.MinDate;
    maxDate = elementDesign.MaxDate;
    defaultDate = elementDesign.DefaultValue;

    $("#" + idpluscol, this.elementIDs.repeaterRowDataJQ).datepicker({
        dateFormat: "mm/dd/yy",
        changeMonth: true,
        changeYear: true,
        yearRange: 'c-61:c+20',
        showOn: "both",
        minDate: minDate == '' || minDate == null ? null : new Date(minDate),
        maxDate: maxDate == '' || maxDate == null ? null : new Date(maxDate),
        buttonImage: Icons.CalenderIcon,
        buttonImageOnly: true,
        disabled: (!elementDesign.Enabled)
    }).parent().find('img').css('margin-top', '0px');

    $("#" + idpluscol, currentInstance.elementIDs.repeaterRowDataJQ).on("focusout", function (e) {
        var date = new Date($("#" + idpluscol, currentInstance.elementIDs.repeaterRowDataJQ).val());
        currentInstance.repeaterBuilder.formInstanceBuilder.validation.handleDateRangeValidation(date, minDate, maxDate, $("#" + idpluscol, currentInstance.elementIDs.repeaterRowDataJQ));
    });
}

repeaterdialogAG.prototype.formatDropdownTextbox = function (idpluscol, elementDesign, idpluscolRepeater) {
    var currentInstance = this;
    var count = 0;
    //get current value of the row element
    var currentVal = $("#" + idpluscolRepeater, currentInstance.repeaterBuilder.gridElementIdJQ).closest('td').attr('title');

    function highlightDropdownTextBox(dropDownTextBoxElement) {
        dropDownTextBoxElement.removeClass('standard-optn non-standard-optn');
        dropDownTextBoxElement.addClass(dropDownTextBoxElement.find(":selected").attr('class'));
    }

    if (currentVal != undefined) {
        $.each(elementDesign.Items, function (key, item) {
            if (item.ItemValue.toUpperCase() === currentVal.toUpperCase())
                count++;
        });
        if (currentVal != "" && count == 0) {
            $('<option class="non-standard-optn" value="' + currentVal + '">' + currentVal + '</option>').insertBefore($('#' + idpluscol).find('option').last()).siblings().addClass("standard-optn");
            $('#' + idpluscol).val(currentVal);
            highlightDropdownTextBox($('#' + idpluscol));
        }
    }

    $("#" + idpluscol, currentInstance.elementIDs.repeaterRowDataJQ).parent().append('<input id="' + idpluscol + 'Textbox" type="text" style="display:none;" class="ddt-textbox"></input>');

    //$("#" + idpluscol).unbind('change');
    $("#" + idpluscol).change(function () {
        if ($(this).val() == 'newItem') {
            $(this).val('');
            $(this).parent().find('.ddt-textbox').toggle().focus();
            $(this).toggle();
        }
        highlightDropdownTextBox($(this));
    });
    //$('.ddt-textbox').unbind('focusout');
    $('.ddt-textbox', currentInstance.elementIDs.repeaterRowDataJQ).on("focusout", function (e) {
        var dropdownTextboxControl = $(this).parent().find('#' + idpluscol);
        if (dropdownTextboxControl.length > 0) {
            var newValue = $(this).val();
            var stringUtility = new globalutilities();
            if (stringUtility.stringMethods.isNullOrEmpty(newValue)) {
                $('#' + idpluscol).val('Select One').trigger('change');
            }
            $(this).toggle();
            dropdownTextboxControl.toggle();
            highlightDropdownTextBox(dropdownTextboxControl);
        }
    });

    //$('.ddt-textbox').unbind('keyup');
    $('.ddt-textbox', currentInstance.elementIDs.repeaterRowDataJQ).on("keyup", function (e) {
        var newValue = $(this).val();
        var dropdownTextboxControl = $(this).parent().find('#' + idpluscol);
        var stringUtility = new globalutilities();
        if (!stringUtility.stringMethods.isNullOrEmpty(newValue)) {
            if ($(this).parent().find('#' + idpluscol + ' option').hasClass("non-standard-optn") == true) {
                $(this).parent().find('#' + idpluscol + ' option.non-standard-optn').remove();
            }
            //check if unique response is the same item as Items from document design
            var existingItem = false;
            $.each(elementDesign.Items, function (key, elem) {
                if (elem.ItemValue.toUpperCase() === newValue.toUpperCase()) {
                    existingItem = true;
                    dropdownTextboxControl.val(elem.ItemValue);
                }
            });
            if (existingItem == false) {
                $('<option value="' + newValue + '" class="non-standard-optn">' + newValue + '</option>').insertBefore($(this).parent().find('#' + idpluscol + ' option').last()).siblings().addClass("standard-optn");
                dropdownTextboxControl.val(newValue);
            }
            highlightDropdownTextBox(dropdownTextboxControl);
        }

    });
}

repeaterdialogAG.prototype.invokeCallback = function (rowData, rowID) {
    var currentInstance = this;
    currentInstance.repeaterBuilder.updateCallBack(rowData, rowID);
}

repeaterdialogAG.prototype.runRuleForRepeaterDialog = function (rule, parentRowId, childRowId) {
    var currentInstance = this;
    currentInstance.repeaterBuilder.ruleProcessor.processRule(rule, parentRowId, childRowId, this);
}

repeaterdialogAG.prototype.setElementErrorState = function (result, rule, row) {
    var currentInstance = this;
    var elem = $('#' + row.RowIDProperty + "_" + rule.UIElementName);
    if (currentInstance.repeaterBuilder.formInstanceBuilder.designData.Validations == null) {
        currentInstance.repeaterBuilder.formInstanceBuilder.designData.Validations = [];
    }
    var validation = currentInstance.repeaterBuilder.formInstanceBuilder.designData.Validations.filter(function (val) {
        return val.UIElementName == rule.UIElementFormName;
    });
    if (result == true) {
        if (validation != null && validation.length > 0) {
            validation[0].IsError = false;
        }
    }
    else {
        if (validation != null && validation.length > 0) {
            validation[0].IsError = true;
        }
        else {
            validation = { FullName: rule.UIElementFullName, UIElementName: rule.UIElementFormName, IsRequired: false, IsError: true, Regex: '', ValidationMessage: '', HasMaxLength: '', MaxLength: '', DataType: '', IsActive: true, ValidationType: 'Temporary' };
            currentInstance.repeaterBuilder.formInstanceBuilder.designData.Validations.push(validation);
        }
    }

    var repeaterIndexes = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getGridParam', '_index');

    var rowIndex = repeaterIndexes[row.RowIDProperty];

    //var val = currentInstance.formInstanceBuilder.ruleProcessor.getOperandValue(rule.UIElementFullName);
    var uiElementFullNameArray = rule.UIElementFullName.split(".");
    var val = row[uiElementFullNameArray[uiElementFullNameArray.length - 1]];
    var validationError = currentInstance.repeaterBuilder.formInstanceBuilder.formValidationManager.handleValidation(rule.UIElementFullName, val, rowIndex, '', row.RowIDProperty);
    if (validationError) {
        currentInstance.repeaterBuilder.formInstanceBuilder.validation.handleObjectChangeValidation(validationError);
        currentInstance.repeaterBuilder.showValidatedControlsOnRepeaterElementChange(validationError, currentInstance.repeaterBuilder);
    }
}

repeaterdialogAG.prototype.visibleRuleResultCallBack = function (rule, result) {
    var currentInstance = this;
    if (rule.ParentRepeaterType == 'In Line') {
        //get in line columns to hide
        var nameParts = rule.UIElementFullName.split('.');
        var dsName = nameParts[nameParts.length - 2];
        var colNamePart = 'INL_' + dsName + '_';
        var elemPart = '_' + rule.UIElementName;
        var colModels = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getGridParam', 'colModel');
        var cols = colModels.filter(function (model) {
            return model.name.indexOf(colNamePart) == 0 && model.name.indexOf(elemPart) > 0;
        });
        var colNames = [];
        for (var idx = 0; idx < cols.length; idx++) {
            var dialogCols = 'formEdit_' + cols[idx].name.replace(dsName + '_', '');
            colNames.push(dialogCols);
        }
        if (result == true) {
            for (var idx = 0; idx < colNames.length; idx++) {
                $("#repeaterDialog").find("#" + colNames[idx]).parent().parent().show();
            }
        }
        else {
            for (var idx = 0; idx < colNames.length; idx++) {
                $("#repeaterDialog").find("#" + colNames[idx]).parent().parent().hide();
            }
        }

    }
    else if (rule.ParentRepeaterType == 'Child') {
        //get child grid/column to hide
    }
    else {
        //get column to hide
        if (result == true) {
            $("#repeaterDialog").find("#" + "formEdit" + currentInstance.rowID + "_" + rule.UIElementName).parent().show();
            $("#repeaterDialog").find("#" + "formEdit" + currentInstance.rowID + "_" + rule.UIElementName).parent().prev('div').show();
        }
        else {
            $("#repeaterDialog").find("#" + "formEdit" + currentInstance.rowID + "_" + rule.UIElementName).parent().hide();
            $("#repeaterDialog").find("#" + "formEdit" + currentInstance.rowID + "_" + rule.UIElementName).parent().prev('div').hide();
        }
    }
}

repeaterdialogAG.prototype.ruleResultCallBack = function (rule, row, retVal, childName, childIdx) {
    var currentInstance = this;
    if (currentInstance.rowID == row.RowIDProperty) {
        if (rule.ParentRepeaterType == null) {
            currentInstance.setParentRow(rule, row, retVal);
        }
        if (rule.ParentRepeaterType == 'In Line' || rule.ParentRepeaterType == 'Child') {
            currentInstance.setChildRow(rule, row, retVal, childName, childIdx);
        }
    }
}

repeaterdialogAG.prototype.setParentRow = function (rule, row, retVal) {
    var currentInstance = this;
    var elementID = "#" + "formEdit" + currentInstance.rowID + "_" + rule.UIElementName;
    if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
        $("#repeaterDialog").find(elementID).val(retVal);
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
        if (retVal == true) {
            $("#repeaterDialog").find(elementID).parent().show();
            $("#repeaterDialog").find(elementID).parent().prev('div').show();
        }
        else {
            $("#repeaterDialog").find(elementID).parent().hide();
            $("#repeaterDialog").find(elementID).parent().prev('div').hide();
        }
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
        if (retVal == true) {
            $("#repeaterDialog").find(elementID).removeAttr('disabled');
            if (rule.UIElementTypeID == 6) {
                $("#repeaterDialog").find(elementID).siblings().removeAttr('disabled');
            }
        }
        else {
            $("#repeaterDialog").find(elementID).attr('disabled', 'disabled');
            if (rule.UIElementTypeID == 6) {
                $("#repeaterDialog").find(elementID).siblings().attr('disabled', 'disabled');
            }
        }
    }
    else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
        var uielement = $("#repeaterDialog").find(elementID);
        if ($(uielement).css('display') != "none" && !($(uielement).is(":disabled"))) {
            currentInstance.setElementErrorState(retVal, rule, row);
            if (retVal == true) {
                $("#repeaterDialog").find(elementID).parent().removeClass("has-error");
            }
            else {
                $("#repeaterDialog").find(elementID).parent().addClass("has-error");
            }
        }
    }
}

repeaterdialogAG.prototype.setChildRow = function (rule, row, retVal, childName, childIdx) {
    var currentInstance = this;
    if (rule.ParentRepeaterType == 'In Line') {
        var elementID = "#" + "formEdit_" + 'INL_' + childIdx + '_' + rule.UIElementName;
        if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
            $("#repeaterDialog").find(elementID).val(retVal);
        }
        else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
            if (retVal == true) {
                $("#repeaterDialog").find(elementID).parent().parent().show();
            }
            else {
                $("#repeaterDialog").find(elementID).parent().parent().hide();
            }
        }
        else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
            if (retVal == true) {
                $("#repeaterDialog").find(elementID).removeAttr('disabled');
                if (rule.UIElementTypeID == 6) {
                    $("#repeaterDialog").find(elementID).siblings().removeAttr('disabled');
                }
            }
            else {
                $("#repeaterDialog").find(elementID).attr('disabled', 'disabled');
                if (rule.UIElementTypeID == 6) {
                    $("#repeaterDialog").find(elementID).siblings().attr('disabled', 'disabled');
                }
            }
        }
        else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
            var uielement = $("#repeaterDialog").find(elementID);
            if ($(uielement).css('display') != "none" && !($(uielement).is(":disabled"))) {
                currentInstance.setElementErrorState(retVal, rule, row);
                if (retVal == true) {
                    $("#repeaterDialog").find(elementID).parent().removeClass("has-error");
                }
                else {
                    $("#repeaterDialog").find(elementID).parent().addClass("has-error");
                }
            }
        }
    }
    else {
        var parentRowId = row.RowIDProperty;
        var childBuilder = currentInstance.repeaterBuilder.childGridBuilders.filter(function (builder) {
            return builder.parentRowId == parentRowId;
        });
        if (childBuilder != null && childBuilder.length > 0) {
            if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Value) {
                $("#repeaterDialog").find("#" + "childGrid" + childIdx).jqGrid('setCell', childBuilder[0].data[childIdx][currentInstance.repeaterBuilder.KeyProperty] + 1, rule.UIElementName, retVal);
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Visible) {
                if (retVal == true) {
                    $("#repeaterDialog").find("#" + "childGrid" + childIdx).jqGrid('setCell', childBuilder[0].data[childIdx][currentInstance.repeaterBuilder.KeyProperty] + 1, rule.UIElementName, '', { visibility: 'visible' });
                }
                else {
                    $("#repeaterDialog").find("#" + "childGrid" + childIdx).jqGrid('setCell', childBuilder[0].data[childIdx][currentInstance.repeaterBuilder.KeyProperty] + 1, rule.UIElementName, '', { visibility: 'hidden' });
                }
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Enabled) {
                if (retVal == true) {
                    $("#repeaterDialog").find("#" + "childGrid" + childIdx).jqGrid('setCell', childBuilder[0].data[childIdx][currentInstance.repeaterBuilder.KeyProperty] + 1, rule.UIElementName, '', '', { disabled: null });
                }
                else {
                    $("#repeaterDialog").find("#" + "childGrid" + childIdx).jqGrid('setCell', childBuilder[0].data[childIdx][currentInstance.repeaterBuilder.KeyProperty] + 1, rule.UIElementName, '', '', { disabled: 'disabled' });
                }
            }
            else if (rule.TargetPropertyTypeId == ruleProcessor.TargetPropertyTypes.Error) {
                var uielement = $("#repeaterDialog").find("#" + "childGrid" + childIdx);
                if ($(uielement).css('visibility') != undefined && $(uielement).css('visibility') == 'visible' && $(uielement)[0].attributes.disabled == undefined) {
                    if (retVal == true) {
                        $("#repeaterDialog").find("#" + "childGrid" + childIdx).jqGrid('setCell', childBuilder[0].data[childIdx][currentInstance.repeaterBuilder.KeyProperty] + 1, rule.UIElementName, '', '');
                    }
                    else {
                        $("#repeaterDialog").find("#" + "childGrid" + childIdx).jqGrid('setCell', childBuilder[0].data[childIdx][currentInstance.repeaterBuilder.KeyProperty] + 1, rule.UIElementName, '', 'has-error');
                    }
                }
            }
        }
    }
}

repeaterdialogAG.prototype.loadRulesForElement = function (ruleElem) {
    var currentInstance = this;
    var elem = ruleElem;
    var rowId = currentInstance.rowID;
    var designElem = currentInstance.repeaterBuilder.design.Elements.filter(function (el) {
        return $(elem).attr('id').indexOf(el.GeneratedName) >= 0;
    });
    if (designElem != null && designElem.length > 0) {
        designElem = designElem[0];
        var rules = currentInstance.repeaterBuilder.formInstanceBuilder.rules.getRulesForElement(designElem.FullName);
        if (rules != null && rules.length > 0) {
            var row = currentInstance.repeaterData.filter(function (dt) {
                return dt.RowIDProperty == rowId;
            });
            if (row != null && row.length > 0) {
                row = row[0];
                for (var idx = 0; idx < rules.length; idx++) {
                    var parentId = rowId;
                    var rule = rules[idx];
                    var childId;
                    if (rule.ParentRepeaterType == 'In Line') {
                        var elemId = elem.id;
                        var idParts = elemId.split('_');
                        childId = idParts[idParts.length - 2];
                        var childName = idParts[idParts.length - 3];
                        row[childName][childId][designElem.GeneratedName] = currentInstance.repeaterBuilder.getElementValue(designElem.Type, this);
                    }
                    else if (rule.ParentRepeaterType == 'Child') {
                        parentId = currentInstance.repeaterBuilder.parentRowId;
                        childId = rowId;
                        row[designElem.GeneratedName] = currentInstance.repeaterBuilder.getElementValue(designElem.Type, elem);
                    }
                    else {
                        row[designElem.GeneratedName] = currentInstance.repeaterBuilder.getElementValue(designElem.Type, elem);
                    }
                    currentInstance.runRuleForRepeaterDialog(rule, parentId, childId);
                }
            }
        }
    }
}

repeaterdialogAG.prototype.loadValidationsForElement = function (validElem) {
    var currentInstance = this;
    var elem = validElem;
    var rowId = currentInstance.rowID;
    var path = currentInstance.repeaterBuilder.fullName + '.' + elem.id.split('_')[1];

    var rowIndex = undefined;

    var ID = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid("getDataIDs");
    $.each(ID, function (idx, ct) {
        if (ct == rowId) {
            rowIndex = idx;
            return false;
        }
    });
    selectedRowId = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getGridParam', 'selrow');
    currentPage = $(currentInstance.repeaterBuilder.gridElementIdJQ).getGridParam('page');
    var rowNum = $(currentInstance.repeaterBuilder.gridElementIdJQ).getGridParam('rowNum');
    if (currentPage != 1) {
        rowIndex = ((currentPage - 1) * rowNum) + rowIndex;
    }

    var validationError = currentInstance.repeaterBuilder.formInstanceBuilder.formValidationManager.handleValidation(path, elem.value, rowIndex, '', selectedRowId);
    if (validationError) {
        currentInstance.repeaterBuilder.formInstanceBuilder.validation.handleObjectChangeValidation(validationError);
        currentInstance.repeaterBuilder.showValidatedControlsOnRepeaterElementChange(validationError, currentInstance.repeaterBuilder);
    }
}

repeaterdialogAG.prototype.loadValidationsForElementPQ = function (validElem) {
    var currentInstance = this;
    var elem = validElem;
    var rowId = currentInstance.rowID;
    var path = currentInstance.repeaterBuilder.fullName + '.' + elem.id.split('_')[1];

    var rowIndex = undefined;
    rowIndex = rowId;
    //var ID = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid("getDataIDs");
    //$.each(ID, function (idx, ct) {
    //    if (ct == rowId) {
    //        rowIndex = idx;
    //        return false;
    //    }
    //});
    //selectedRowId = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getGridParam', 'selrow');
    selectedRowId = rowIndex;
    //currentPage = $(currentInstance.repeaterBuilder.gridElementIdJQ).getGridParam('page');
    //var rowNum = $(currentInstance.repeaterBuilder.gridElementIdJQ).getGridParam('rowNum');
    /*NJ currentPage = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("option", "pageModel.curPage");
     var rowNum = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("option", "pageModel.rPP");*/

    var gridOptions = GridApi.getCurrentGridInstanceById(currentInstance.repeaterBuilder.gridElementIdJQ).gridOptions;
    currentPage = gridOptions.api.paginationGetCurrentPage();

    var rowNum = gridOptions.api.paginationGetPageSize()

    if (currentPage != 1) {
        rowIndex = ((currentPage - 1) * rowNum) + rowIndex;
    }

    var validationError = currentInstance.repeaterBuilder.formInstanceBuilder.formValidationManager.handleValidation(path, elem.value, rowIndex, '', selectedRowId);
    if (validationError) {
        currentInstance.repeaterBuilder.formInstanceBuilder.validation.handleObjectChangeValidation(validationError);
        currentInstance.repeaterBuilder.showValidatedControlsOnRepeaterElementChange(validationError, currentInstance.repeaterBuilder);
    }
}

repeaterdialogAG.prototype.loadDuplicationsForElement = function (duplicateElem) {
    var currentInstance = this;
    var elem = duplicateElem;
    var rowId = currentInstance.rowID;
    var path = currentInstance.repeaterBuilder.fullName + '.' + elem.id.split('_')[1];

    var designElem = currentInstance.repeaterBuilder.design.Elements.filter(function (el) {
        return $(elem).attr('id').indexOf(el.GeneratedName) >= 0;
    });
    if (designElem != null && designElem.length > 0) {
        designElem = designElem[0];
        if (currentInstance.repeaterBuilder.formInstanceBuilder.designData.Duplications != null || currentInstance.repeaterBuilder.formInstanceBuilder.designData.Duplications != undefined) {
            //code to remove Duplication Errors from Error Grid
            var duplicationObject = currentInstance.repeaterBuilder.formInstanceBuilder.designData.Duplications.filter(function (ct) {
                return ct.FullName == designElem.FullName;
            });

            if (duplicationObject.length > 0) {
                var gridRowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getGridParam', 'data');
                var gridData = gridRowData.filter(function (ct) {
                    if (ct.RowIDProperty == rowId) {
                        ct[designElem.GeneratedName] = elem.value;
                    }
                    return ct;
                });
                var duplicationErrorArray = currentInstance.repeaterBuilder.formInstanceBuilder.formValidationManager.handleDuplication(duplicationObject[0].ParentUIElementName, gridData, currentInstance.rowID);
                if (duplicationErrorArray.length > 0) {
                    for (var i = 0; i < duplicationErrorArray.length; i++) {
                        var duplicationError = duplicationErrorArray[i];
                        currentInstance.repeaterBuilder.formInstanceBuilder.validation.handleObjectChangeValidation(duplicationError);
                        currentInstance.repeaterBuilder.showValidatedControlsOnRepeaterElementChange(duplicationError, currentInstance.repeaterBuilder);
                    }
                }
            }
        }
    }
}

repeaterdialogAG.prototype.loadDuplicationsForElementPQ = function (duplicateElem) {
    var currentInstance = this;
    var elem = duplicateElem;
    var rowId = currentInstance.rowID;
    var path = currentInstance.repeaterBuilder.fullName + '.' + elem.id.split('_')[1];

    var designElem = currentInstance.repeaterBuilder.design.Elements.filter(function (el) {
        return $(elem).attr('id').indexOf(el.GeneratedName) >= 0;
    });
    if (designElem != null && designElem.length > 0) {
        designElem = designElem[0];
        if (currentInstance.repeaterBuilder.formInstanceBuilder.designData.Duplications != null || currentInstance.repeaterBuilder.formInstanceBuilder.designData.Duplications != undefined) {
            //code to remove Duplication Errors from Error Grid
            var duplicationObject = currentInstance.repeaterBuilder.formInstanceBuilder.designData.Duplications.filter(function (ct) {
                return ct.FullName == designElem.FullName;
            });

            if (duplicationObject.length > 0) {
                //var gridRowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).jqGrid('getGridParam', 'data');
                //nj var gridRowData = $(currentInstance.repeaterBuilder.gridElementIdJQ).pqGrid("option", "dataModel.data");
                var gridOptions = GridApi.getCurrentGridInstanceById(currentInstance.repeaterBuilder.gridElementIdJQ).gridOptions;
                var gridData = {};
                if (gridOptions.api.getSelectedNodes().length > 0) {
                    var gridRowData = gridOptions.api.getSelectedNodes()[0].data;
                    gridRowData[designElem.GeneratedName] = elem.value;
                    gridData = gridOptions.rowData;
                    /*gridData = gridRowData.filter(function (ct) {
                       if (ct.RowIDProperty == rowId) {
                           ct[designElem.GeneratedName] = elem.value;
                       }
                       return ct;
                   });*/
                }
                var duplicationErrorArray = currentInstance.repeaterBuilder.formInstanceBuilder.formValidationManager.handleDuplication(duplicationObject[0].ParentUIElementName, gridData, currentInstance.rowID);
                if (duplicationErrorArray.length > 0) {
                    for (var i = 0; i < duplicationErrorArray.length; i++) {
                        var duplicationError = duplicationErrorArray[i];
                        currentInstance.repeaterBuilder.formInstanceBuilder.validation.handleObjectChangeValidation(duplicationError);
                        currentInstance.repeaterBuilder.showValidatedControlsOnRepeaterElementChange(duplicationError, currentInstance.repeaterBuilder);
                    }
                }
            }
        }
    }
}

repeaterdialogAG.prototype.processRules = function () {
    var currentInstance = this;
    for (var idx = 0; idx < currentInstance.repeaterBuilder.rules.length; idx++) {
        if (currentInstance.repeaterBuilder.rules[idx].TargetPropertyTypeId != ruleProcessor.TargetPropertyTypes.Value) {
            if (currentInstance.repeaterBuilder.gridType == 'child') {
                currentInstance.runRuleForRepeaterDialog(currentInstance.repeaterBuilder.rules[idx], currentInstance.repeaterBuilder.parentRowId);
            }
            else {
                currentInstance.runRuleForRepeaterDialog(currentInstance.repeaterBuilder.rules[idx]);
            }
        }
    }
}
