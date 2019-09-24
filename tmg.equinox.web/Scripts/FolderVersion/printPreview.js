function printPreview(formInstances) {
    this.formInstanceList = formInstances;
    this.previewGridDesigns = [];
    this.htmlstring = '';
    this.URLs = {
        getFormInstanceDesignData: "/FormInstance/GetFormInstanceData?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}",
        loadCustomRules: "/FormInstance/LoadCustomRules?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}",
        getpdfConfigureData: '/PrintTemplate/LoadTemplateDesignUIElementList?tenantId=1&formDesignVersionId={formDesignVersionId}&templateId={templateId}',
        getFormInstanceRepeaterDesignData: "/FormInstance/GetFormInstanceRepeaterDesignData?tenantId={tenantId}&formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}&folderVersionId={folderVersionId}",
        checkMilestoneChecklistSection: "/FolderVersion/CheckMilestoneChecklistSection?folderVersionId={folderVersionId}&sectionName={sectionName}"

    };
    this.elementIDs = {
        previewCanvas: '#previewInstances',
        sectionTemplate: '#SectionTemplate',
        formTemplate: '#FormTemplate'
    };
    //this.accessPermissions = new formInstanceBuilder.prototype.accessPermissionsMethods();
    this.formInstanceId = null;
    this.formDesignVersionId = null;
    this.formDesignId = null;
    this.folderVersionId = null;
    this.accessPermissions = this.accessPermissionsMethods();    
}

printPreview.prototype.loadAllFormInstances = function (tenantId, folderId, folderVersionId, roleID) {
    var currentInstance = this;
    //Get form design for each instance
    $(currentInstance.formInstanceList).each(function (index, instance) {
        var url = currentInstance.URLs.getFormInstanceDesignData.replace(/\{tenantId\}/g, tenantId).replace(/\{formInstanceId\}/g, instance.FormInstanceID).replace(/\{formDesignVersionId\}/g, instance.FormDesignVersionID).replace(/\{folderVersionId\}/g, folderVersionId);
        var promise = ajaxWrapper.getJSON(url);
        //register ajax success callback
        promise.done(function (xhr) {
            currentInstance.loadFormDesign(xhr, instance.FormInstanceID, instance.FormDesignName, roleID)
        });
    });
}

printPreview.prototype.loadFormDesign = function (xhr, pdfConfigureData, formInstanceId, formDesignName, folderVersionId, roleID) {
    var currentInstance = this;
    currentInstance.formInstanceId = formInstanceId;
    currentInstance.formDesignVersionId = xhr.FormDesignVersionId;
    currentInstance.formDesignId = xhr.FormDesignId;
    currentInstance.folderVersionId = folderVersionId;
    var designData = xhr;
    var data = designData.JSONData.replace(/\[Select One]/g, '').replace(/\Select One/g, '');
    
    var formData = JSON.parse(data);
    
    var formDataAsString = JSON.stringify(formData);
    var formDesignId = xhr.FormDesignId;
    var currentInstanceData = {
        formDesignId: formDesignId,
        formData: formData,        
    };
    
    var formName = [{
        FormName: formInstanceId,
        FormDesignName: formDesignName
    }];

    $(currentInstance.elementIDs.previewCanvas).append(
         $(currentInstance.elementIDs.formTemplate).render(formName)
     );
    divFormJQ = '#' + formInstanceId;

    printPreview.prototype.IsSectionVisibleForPDF = function (sectionName) {
        if (!pdfConfigureData || pdfConfigureData.length == 0)
            return true;
        var validSection = pdfConfigureData.filter(function (ct) {
            if (ct.Label == sectionName && ct.isActive == true)
                return true;
        });
        if (validSection.length > 0)
            return true;
        else
            return false;
    }

    $.each(designData.Sections, function (index, sectionData) {
        if (sectionData != null && currentInstance.IsSectionVisibleForPDF(sectionData.Label)) {
            $.templates({ sectionTmpl: { markup: currentInstance.elementIDs.sectionTemplate, layout: true, } });
            //// add unique response as a option in DropDownTextBox element
            setDropDownTextBoxOption(sectionData.Elements, formData, sectionData.FullName);
            var sectionHtml = $.render.sectionTmpl(sectionData,
            {
                repeaterCallback: function (x) { currentInstance.previewGridDesigns.push(x); },
                hasRequiredValidation: function (x)
                { return currentInstance.hasRequiredValidation(designData, x); },
                getFormInstanceId: function () { return formInstanceId },
                renderCustomHtml: function (customHtml, elementCount, name) {
                    var html = "<script id=\"" + name.replace("#", "") + "\" type=\"text/x-jsrender\">";
                    html = html + customHtml;

                    for (i = 1; i <= elementCount; i++) {
                        //class="repeater-grid {{>~getCssClass(~layout)}}"
                        html = html.replace("{{" + i + "}}", "{{for Elements tmpl=\"#CustomElementTemplate\" ~cnt = " + i + " ~layout = LayoutColumn/}}");
                    }

                    html = html + "</script>";
                    $("body").append(html);
                },
                getCheckBoxValueFromJSONData: function (path) {
                    try {

                        var _formData = JSON.parse(formDataAsString);

                        var patharr = path.split('.');
                        var dataToReturn = "";
                        $.each(patharr, function (i, obj) {
                            dataToReturn = _formData[obj];
                            _formData = dataToReturn;
                        });

                        if (dataToReturn == "false" || dataToReturn == null || dataToReturn == "False" || dataToReturn == false)
                            return false;
                        else
                            return true;
                    }
                    catch (exx) {
                        var vfsd = "";
                    }
                },
                isSectionVisible: function (sectionName) {
                    var IsVisible = currentInstance.IsSectionVisibleForPDF(sectionName);
                    if (IsVisible) {
                        var fdgf = '';
                    }
                    return IsVisible;
                },
                setRadioButtonDataLink: function (fullName) {
                    return 'data-link="{getRadioValue:' + fullName + ':setRadioValue}"';
                },
                setCheckBoxDataLink: function (fullName) {
                    return 'data-link="{getCheck:' + fullName + ':setCheck}"';
                },
                getLabelsValueFromJSON: function (path) {
                    var dataToReturn = "";
                    try {
                        if (formData && path) {
                            var _formData = JSON.parse(formDataAsString);
                            var patharr = path.split('.');

                            $.each(patharr, function (i, obj) {
                                if (_formData) {
                                    dataToReturn = _formData[obj];
                                    _formData = dataToReturn;
                                }
                            });
                        }
                    }
                    catch (ex) {
                        dataToReturn = "";
                    }

                    return dataToReturn;
                }
            });
            var tmpl = $.templates(sectionHtml);
            var data = {};
            var wrapperSectionId = sectionData.Name + formInstanceId + "wrapper";
            var sectionDiv = "<div id='" + wrapperSectionId + "'>";

            $(divFormJQ).append(sectionDiv);
            data[sectionData.FullName] = formData[sectionData.FullName];
            tmpl.link("#" + wrapperSectionId, data);
            //load grids
            if (currentInstance.previewGridDesigns != null && currentInstance.previewGridDesigns.length > 0) {

                var previewVisibleGridDesigns = currentInstance.previewGridDesigns.filter(function (ct) {
                    return ct.Visible == true;
                });
                $.each(previewVisibleGridDesigns, function (idx, item) {
                    //Split into section
                    var displayName = [];
                    $.each(item.Repeater.Elements, function (i, elem) {
                        displayName.push({ Id: elem.GeneratedName, displayText: elem.Label, Visible: elem.Visible, Type: elem.Type });
                    });
                    var mainSec = "<div class='panel panel-default subsection' >" + "<div class='panel-heading-gray' >" +

                    "<label class='panel-title' style='font-family: Calibri,sans-serif !important; line-height: 10px !important;font-size: 14px !important;'>" + item.Repeater.Label + "</label> </div>" + " <div class='panel-body'>" +
                                  "<div class='container-fluid' >" + "<div class='row'>";
                    if (item.Repeater.ChildDataSources) {
                        for (var i = 0; i < item.Repeater.ChildDataSources.length; i++) {
                            if (item.Repeater.ChildDataSources[i].DisplayMode == "Child") {
                                var childDataSourceName = item.Repeater.ChildDataSources[i].DataSourceName;
                            }
                            else if (item.Repeater.ChildDataSources[i].DisplayMode == "In Line") {
                                var inlineDataSourceName = item.Repeater.ChildDataSources[i].DataSourceName;
                            }
                        }
                    }

                    var data = getDataProperty(item.Repeater.FullName, formData);
                    if (item.Visible) {
                        $.each(data, function (i, row) {
                            if (inlineDataSourceName) {
                                var subArray = getsubArrays(row, [], childDataSourceName);
                                var rowlayout = CreateChildLayout(row, displayName, item.Repeater);
                                mainSec = mainSec + rowlayout;
                                for (var key in subArray) {
                                    $.each(subArray[key], function (k, sectionData) {
                                        var sec = CreateInLineLayout(sectionData, displayName);
                                        mainSec = mainSec + sec;
                                    });
                                    mainSec = mainSec + "</div>";
                                }
                            }
                            else {
                                var rowlayout = CreateLayout(row, displayName, item.Repeater);
                                mainSec = mainSec + rowlayout;
                            }

                        });
                    }
                    mainSec = mainSec + "</div></div></div></div>";
                    $('#' + item.Repeater.Name + formInstanceId).append(mainSec);
                    //end split section
                });
                currentInstance.previewGridDesigns = [];
            }

            //Apply Access permissions for PDF generation
            currentInstance.accessPermissions.applyAccessPermission(sectionData, roleID);
        }
    });

    //set selected elements as "Select One" where no value is selected for dropdowns.
    $(divFormJQ).find('select').each(function () {
        if ($(this).val() === '' || $(this).val() === null) {
            //$(this).val(Validation.selectOne);
            var selectValue = 'Select One';
            $(this).val(selectValue);
        }
    });

    //set selected elements as "Select One" where no value is selected for bootstrap buttons used as dropdowns.
    $(divFormJQ).find('button').each(function () {
        $(this).find('span:first').each(function () {
            if ($(this)[0].innerHTML === '' || $(this)[0].innerHTML === null) {
                //$(this).val(Validation.selectOne);
                var spanValue = 'Select One';
                $(this)[0].innerHTML = spanValue;
            }
        });
    });

    //code to replace text with textarea in divFormJQ if text is wrapped
    var inputTagNodeList = $(divFormJQ).find("input:text");
    $.each(inputTagNodeList, function (i, oldInputText) {
        if (oldInputText.style.display !== "none") {
            if (oldInputText.value.length > 33) {
                newTextArea = document.createElement('textarea');
                newTextArea.innerHTML = oldInputText.value;
                newTextArea.className = 'form-control';
                $(oldInputText).replaceWith(newTextArea);
            }
        }
    });

    

    //code to replace checkbox with img in divFormJQ //TODO: checkbox image not loaded
    var checkboxList = $(divFormJQ).find("input:checkbox");
    $.each(checkboxList, function (i, oldInputCheckbox) {
        if (oldInputCheckbox.style.display !== "none") {
            newImg = document.createElement('img');
            newImg.style.marginBottom = '3px';
            newImg.style.paddingRight = '5px';
            newImg.style.cssFloat = 'left';
            if (oldInputCheckbox.checked == true) {
                newImg.src = "/Content/css/custom-theme/images/chrome-style-checked.png";
            }
            else {
                newImg.src = "/Content/css/custom-theme/images/chrome-style-unchecked.png";
            }
            $(oldInputCheckbox).replaceWith(newImg);
        }
    });

    

    var textareaListS = $(document).find("textarea");
    $.each(textareaListS, function (i, oldtextareaElement) {
        if (oldtextareaElement.style.display !== "none") {
            var newDivElement = '<div style="border-style:regular;border-color:black;height:auto;"><pre style="background-color:white;height:auto;border-color:lightgray;font-family:inherit;font-size:10px;padding-top:8px;padding-bottom:8px;padding-left:0px;margin:0px;">' + oldtextareaElement.innerHTML + '</pre></div>';
            var data = oldtextareaElement.innerText;
            newDivElement = newDivElement.replace(/{{data}}/g, data.substring(1, 8));
            $(oldtextareaElement).replaceWith($(newDivElement));
        }
    });
    
}

function CreateChildLayout(row, displayName, repeaterDesign) {
    divRow = "<div class='panel panel-default subsection'>" +
                                      "   <div class='panel-body'>" +
                                      "<div class='container-fluid'>" + "<div class='row'>"
    var i = 0;
    var cnt = 0;
    for (var key in row) {
        var displayText = undefined;
        var repeaterDesignData = repeaterDesign.Elements[i];
        for (var j = 0; j < displayName.length; j++) {
            var col = displayName[j];
            if (col['Id'] == key && col['Visible'] == true) {
                displayText = col['displayText'];
                cnt++;
                divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                + "<span class='staticLabel'>" + displayText + " </span></div>"
                if (col['Type'] === "checkbox") {
                    var value = row[key];
                    if (value == undefined || value == null)
                        value = '';

                    if (value.toLowerCase() === "yes" || value.toLowerCase() === "true") {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='checkbox' checked='checked' class='css-checkboxgrid' value='true'/> <span class='css-span'> </div>"
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='checkbox' class='css-checkboxgrid' value='true'/> <span class='css-span'> </div>"
                    }
                }
                else {
                    if (col['Type'] === "select" || col['Type'] === "SelectInput") {
                        var value = row[key];
                        var css = '';
                        if (value === '' || value === null) {
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                        + "<button class='btn btn-default btn-xs dropdown-toggle' type='button'>Select One<span style='padding-top:6px !important;float:right !important;margin-top:5px !important;' class='caret'></span></button></div>"
                        }
                        else {

                            if (col['Type'] === "SelectInput") {
                                var count = 0;
                                if (repeaterDesignData.Items != null) {
                                    $.each(repeaterDesignData.Items, function (index, item) {
                                        if (item.ItemValue === value) {
                                            count++;
                                        }
                                    });
                                }
                                if (count == 0) { css = 'background-color:yellow !important;'; }
                            }
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                        + "<button class='btn btn-default btn-xs dropdown-toggle' type='button'><div style='" + css + "'>" + row[key] + "<span style='padding-top:6px !important;float:right !important;margin-top:5px !important;' class='caret'></span></div></button></div>"
                        }
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='text' class='form-control' value='" + row[key] + "'/> </div>"
                    }
                }
            }
        }
        //if ((i + 1) % 3 === 0 && (i + 1) != row.length)
        if (((cnt == 3) && !($.isArray(row))) || (cnt == 3 && $.isArray(row) && cnt + 1 != row.length)) {
            divRow = divRow + " </div>  <div class='row'> "
            cnt = 0;
        }
    }
    return divRow + "</div> </div> </div>";
}

function CreateLayout(row, displayName, repeaterDesign) {
    divRow = "<div class='panel panel-default subsection' >" +
                                      "   <div class='panel-body'>" +
                                      "<div class='container-fluid' >" + "<div class='row'>"
    var i = 0;
    var cnt = 0;
    for (var key in row) {
        var displayText = undefined;
        var repeaterDesignData = repeaterDesign.Elements[i];
        for (var j = 0; j < displayName.length; j++) {
            var col = displayName[j];
            if (col['Id'] == key && col['Visible'] == true) {
                displayText = col['displayText'];
                cnt++;
                divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                + "<span class='staticLabel'>" + displayText + " </span></div>"
                if (col['Type'] === "checkbox") {
                    var value = row[key];
                    if (value == undefined || value == null)
                        value = '';

                    if (value.toLowerCase() === "yes" || value.toLowerCase() === "true") {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='checkbox' checked='checked' class='css-checkboxgrid' value='true'/> <span class='css-span'> </div>"
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='checkbox' class='css-checkboxgrid' value='true'/> <span class='css-span'> </div>"
                    }
                }
                else {
                    if (col['Type'] === "select" || col['Type'] === "SelectInput") {
                        var value = row[key];
                        var css = '';
                        if (value === '' || value === null) {
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                        + "<button class='btn btn-default btn-xs dropdown-toggle' type='button'>Select One<span style='padding-top:6px !important;float:right !important;margin-top:5px !important;' class='caret'></span></button></div>"
                        }
                        else {

                            if (col['Type'] === "SelectInput") {
                                var count = 0;
                                if (repeaterDesignData.Items != null) {
                                    $.each(repeaterDesignData.Items, function (index, item) {
                                        if (item.ItemValue === value) {
                                            count++;
                                        }
                                    });
                                }
                                if (count == 0) { css = 'background-color:yellow !important;'; }
                            }
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                        + "<button class='btn btn-default btn-xs dropdown-toggle' type='button'><div style='" + css + "'>" + row[key] + "<span style='padding-top:6px !important;float:right !important;margin-top:5px !important;' class='caret'></span></div></button></div>"
                        }
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='text' class='form-control' value='" + row[key] + "'/> </div>"
                    }
                }
            }
        }
        if (((cnt == 3) && !($.isArray(row))) || (cnt == 3 && $.isArray(row) && cnt + 1 != row.length)) {
            divRow = divRow + " </div>  <div class='row'> "
            cnt = 0;
        }
    }
    return divRow + "</div> </div> </div></div> ";
}

function getsubArrays(data, subSection, childDataSourceName) {
    for (var key in data) {
        var item = data[key];
        if (key == childDataSourceName) {
            delete data[key];
        }
        else {
             //see if this item is an array
            if ($.isArray(item)) {
               //  remove this item from the parent object
                subSection.push(data[key])
                delete data[key];
            }
              //   if this item is an object, then recurse into it 
              //   to remove empty arrays in it too
            else if (typeof item == "object") {
                getsubArrays(item);
            }
        }
    }
    return subSection;
}

function setDropDownTextBoxOption(element, item, sectionName) {
    var ItemID = "0001";
    var flag = false;
    $.each(element, function (key, value) {
        if (element[key].IsDropDownTextBox == true) {
            var count = 0;
            var stringUtility = new globalutilities();
            if (!stringUtility.stringMethods.isNullOrEmpty(item[sectionName][element[key].GeneratedName])) {

                $.each(element[key].Items, function (index, value) {
                    if (value.ItemValue.toUpperCase() == (item[sectionName][element[key].GeneratedName]).toUpperCase())
                        count++;
                });
                if (count == 0) {
                    var newItem = { 'Enabled': null, 'ItemID': ItemID++, 'ItemValue': item[sectionName][element[key].GeneratedName], 'cssclass': 'highlight' };
                    element[key].Items.push(newItem);
                }
            }
        }
    });
}

//function for nested grid header
function CreateInLineLayout(row, displayName) {
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
        divRow = "<div class='panel panel-default subsection' >" + "<div class='panel-heading-gray' >" +
                   "<span style='font-family: Calibri,sans-serif !important; font-size: 12px !important;'>" + titleText + "</span>" + "</div>" + "<div class='panel-body'>" +
                   "<div class='container-fluid' >" + "<div class='row'>";
    }
    else {
        divRow = "<div class='panel panel-default subsection' >" + "<div class='panel-heading-gray' >" +
                    " <label class='panel-title' style='font-family: Calibri,sans-serif !important; line-height: 10px !important;'></label>" + "</div>" + "<div class='panel-body'>" +
                    "<div class='container-fluid' >" + "<div class='row'>";
    }
    var i = 0;
    var cnt = 0;
    for (var key in row) {
        var displayText = undefined;
        $.each(displayName, function (id, name) {
            if (name.Id == key && name.Visible == true) {
                displayText = name.displayText;
                cnt++;
                divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>" + "<span class='staticLabel'>" + displayText + " </span></div>"
                if (name.Type === "checkbox") {
                    var value = row[key];
                    if (value === "Yes") {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='checkbox' checked='checked' class='css-checkboxgrid' value='true'/> <span class='css-span'> </div>"
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='checkbox' class='css-checkboxgrid' value='true'/> <span class='css-span'> </div>"
                    }
                }
                else {
                    if (name.Type === "select") {
                        var value = row[key];
                        if (value === '' || value === null) {
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                        + "<button class='btn btn-default btn-xs dropdown-toggle' type='button'>Select One<span style='padding-top:6px !important;float:right !important;margin-top:5px !important;' class='caret'></span></button></div>"
                        }
                        else {
                            divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                         + "<button class='btn btn-default btn-xs dropdown-toggle' type='button'>" + row[key] + "<span style='padding-top:6px !important;float:right !important;margin-top:5px !important;' class='caret'></span></button></div>"
                        }
                    }
                    else {
                        divRow = divRow + "<div class='col-xs-2 col-md-2 col-lg-2 col-sm-2'>"
                                + "<input type='text' class='form-control' value='" + row[key] + "'/> </div>"
                    }
                }
                return false;
            }
        })
        if (((cnt == 3) && !($.isArray(row))) || (cnt == 3 && $.isArray(row) && cnt + 1 != row.length)) {
            divRow = divRow + " </div>  <div class='row'> "
            cnt = 0;
        }
    }
    return divRow + "</div></div></div></div> ";
}

//code for mandatory fields validation
printPreview.prototype.hasRequiredValidation = function (designData, x) {
    var designData = designData;
    var fullName = x;
    var validationObjectList = designData.Validations.filter(function (ct) {
        if (ct.FullName === fullName)
            return ct;
    });
    var validationObject = validationObjectList[0];

    if (validationObject) {
        if (validationObject.IsRequired) {
            return true;
        }
    }
    return false;
}

//ajax failure callback
printPreview.prototype.showError = function (xhr) {
    alert(JSON.stringify(xhr));
}

printPreview.prototype.loadFormInstance = function (formInstanceId, formDesignVersionId, folderVersionId, templateId, roleID) {
    var currentInstance = this;
    
    var formInstanceDesignUrl;
    formInstanceDesignUrl = currentInstance.URLs.getFormInstanceRepeaterDesignData.replace(/\{tenantId\}/g, 1)
                                .replace(/\{formInstanceId\}/g, formInstanceId)
                                .replace(/\{formDesignVersionId\}/g, formDesignVersionId)
                                .replace(/\{folderVersionId\}/g, folderVersionId);

    var pdfConfigureDataUrl = this.URLs.getpdfConfigureData.replace(/\{formDesignVersionId\}/g, formDesignVersionId)
                                                          .replace(/\{templateId\}/g, templateId);

    ajaxDialog.showPleaseWait();
    $.when(ajaxWrapper.getJSON(formInstanceDesignUrl),
           ajaxWrapper.getJSON(pdfConfigureDataUrl)
           ).done(function (pdfDesignData, pdfConfigureData) {
               currentInstance.loadFormDesign(pdfDesignData[0], pdfConfigureData[0], formInstanceId, previewFolderData.formName, folderVersionId, roleID);
               ajaxDialog.hidePleaseWait();
           }).fail(currentInstance.showError);
}

//section access permission for user role
printPreview.prototype.accessPermissionsMethods = function () {
    var currentInstance = this;
    function checkViewPermission(Permissions, roleId) {
        var rtnVal = false;
        $.each(Permissions, function (index, item) { if (item.RoleID == roleId && item.IsVisible === true) rtnVal = true; });
        return rtnVal;
    }

    //member functions for View Permissions
    function SetViewPermissionToSection(isVisible, section, roleID) {

        if (isVisible == false) {
            elem = $('#' + section.Name + currentInstance.formInstanceId + "wrapper");
            if (section.Elements != null && section.Elements.length != 0) {
                HideAllChildElements(section);
            }
            elem.css('display', 'none');
        }
        else {
            if (section.Elements != null) {
                $.each(section.Elements, function (index, element) {
                    SetViewForElements(element, roleID);
                });
            }
        }
    }

    function checkMilestoneChecklistSectionViewPermission(currentFolderVersionId, section, roleID) {
        //ajax call to check if current section exists for current folderVersionId in WorkFlowStateFolderVersionMap table.
        var url = currentInstance.URLs.checkMilestoneChecklistSection.replace(/\{folderVersionId\}/g, currentFolderVersionId)
                            .replace(/\{sectionName\}/g, section.Label);
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (xhr) {
            if (xhr) {
                SetViewPermissionToSection(true, section, roleID);
            }
            else {
                SetViewPermissionToSection(false, section, roleID);
            }
        });
        promise.fail(currentInstance.showError);
    }

    function SetViewForElements(Element, roleID) {
        if (Element.Section != null) {
            var section = Element.Section;
            var isSectionVisible = checkViewPermission(section.AccessPermissions, roleID);
            SetViewPermissionToSection(isSectionVisible, section, roleID);
        }
        else {
            var elem = $(Element.Name + currentInstance.formInstanceId);
            elem.css('display', 'none');
        }
    }

    // Hides all the child UI elements of section
    function HideAllChildElements(sectionData) {

        if (sectionData.Elements != null) {
            $.each(sectionData.Elements, function (index, element) {
                var elem;
                if (element.Name.indexOf("Section") > 0) {
                    elem = $('#' + element.Name + currentInstance.formInstanceId + "wrapper");
                    var section = element.Section;
                    HideAllChildElements(section.Elements);
                    elem.css('display', 'none');
                }
                else if (element.Name.indexOf("Repeater") > 0) {
                    elem = $('#' + element.Name + currentInstance.formInstanceId);
                    elem.css('display', 'none');
                }

                else {
                    elem = $('#' + element.Name + currentInstance.formInstanceId);
                    elem.css('display', 'none');
                }


            });
        }

        var elem = $('#' + sectionData.Name + currentInstance.formInstanceId + "wrapper")
        elem.css('display', 'none');


    }

    return {
        applyAccessPermission: function (section, roleID) {
            if (section.AccessPermissions != null) {
                var isVisible = checkViewPermission(section.AccessPermissions, roleID);
                SetViewPermissionToSection(isVisible, section, roleID);
                if (currentInstance.formDesignId == MilestoneChecklist.MilestoneChecklistFormDesignID) {
                    checkMilestoneChecklistSectionViewPermission(currentInstance.folderVersionId, section, roleID);
                }
            }
        }

    }
}

