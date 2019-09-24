function globalutilities() {
    this.stringMethods = this.StringMethods();
}

function disableChildElements(sectionDesignData, formInstanceId) {
    $.each(sectionDesignData.Elements, function (index, element) {
        if (element.Type == "section") {
            if (!element.Enabled) {
                $('#section' + element.Name + formInstanceId).find('input, select, textarea, img.ui-datepicker-trigger, table.ui-jqgrid-btable, div.ui-corner-bottom').attr('disabled', 'disabled');
                $('#section' + element.Name + formInstanceId).find('#btnRepeaterBuilderAdd').addClass('ui-state-disabled');
                $('#section' + element.Name + formInstanceId).find('#btnRepeaterBuilderDelete').addClass('ui-state-disabled');
                $('#section' + element.Name + formInstanceId).find('table.ui-jqgrid-btable').attr('disabled', 'disabled');
                $('#section' + element.Name + formInstanceId).find('td.ui-search-input > input').removeAttr("disabled");
            }
            else
                disableChildElements(element.Section, formInstanceId);//recursion call
        }
        else if (element.Type == "repeater" && !element.Enabled) {
            var elem;
            elem = $('#repeater' + element.Name + formInstanceId);
            elem.find('#btnRepeaterBuilderAdd').addClass('ui-state-disabled');
            elem.find('#btnRepeaterBuilderDelete').addClass('ui-state-disabled');
            elem.find('table.ui-jqgrid-btable').attr('disabled', 'disabled');
        }
    });
}
function invisibleChildElements(sectionDesignData, formInstanceId) {
    $.each(sectionDesignData.Elements, function (index, element) {
        var elem;
        if (element.Type == "section") {
            if (!element.Visible) {
                elem = $('#' + element.Name + formInstanceId);
                elem.parent('div').children().css('display', 'none');
            }
            else
                invisibleChildElements(element.Section, formInstanceId);//recursion call
        }
    });
}

function compareStrings(string1, string2, ignoreCase, useLocale) {
    if (!!ignoreCase) {
        if (!!useLocale) {
            string1 = string1.toLocaleLowerCase();
            string2 = string2.toLocaleLowerCase();
        }
        else {
            string1 = string1.toLowerCase();
            string2 = string2.toLowerCase();
        }
    }

    return string1 === string2;
}


function getDataProperty(fullName, data) {
    var dataPart;
    var nameParts = fullName.split(".");
    for (var idx = 0; idx < nameParts.length; idx++) {
        if (idx == 0) {
            dataPart = data[nameParts[idx]];
        }
        else {
            dataPart = dataPart[nameParts[idx]];
        }
    }
    return dataPart;
}

//filter implementation for IE8
if (!Array.prototype.filter) {
    Array.prototype.filter = function (fun /*, thisp */) {
        "use strict";

        if (this === void 0 || this === null)
            throw new TypeError();

        var t = Object(this);
        var len = t.length >>> 0;
        if (typeof fun !== "function")
            throw new TypeError();

        var res = [];
        var thisp = arguments[1];
        for (var i = 0; i < len; i++) {
            if (i in t) {
                var val = t[i]; // in case fun mutates this
                if (fun.call(thisp, val, i, t))
                    res.push(val);
            }
        }

        return res;
    };
}

function flattenObject(objectToFlatten) {
    var returnVal = new Array();

    for (var property in objectToFlatten) {
        if (!objectToFlatten.hasOwnProperty(property)) continue;

        if ((typeof objectToFlatten[property]) == 'object' && !Array.isArray(objectToFlatten[property])) {
            var flatObject = flattenObject(objectToFlatten[property]);
            for (var prop in flatObject) {
                if (!flatObject.hasOwnProperty(prop))
                    continue;
                if ((property + '.' + flatObject).indexOf("jQ") == -1)
                    returnVal[property + '.' + prop] = flatObject[prop];
            }
        }
        else {
            if (property.indexOf("jQ") == -1)
                returnVal[property] = objectToFlatten[property];
        }
    }
    return returnVal;
};

function flattenSortObject(objectToFlatten) {
    var returnVal = new Array();

    for (var property in objectToFlatten) {
        if (!objectToFlatten.hasOwnProperty(property)) continue;

        if (property.indexOf("jQuery") <= -1) {
            if ((typeof objectToFlatten[property]) == 'object' && !Array.isArray(objectToFlatten[property])) {
                returnVal[property] = "";
                var flatObject = flattenSortObject(objectToFlatten[property]);
                for (var prop in flatObject) {
                    if (!flatObject.hasOwnProperty(prop))
                        continue;
                    if ((property + '.' + flatObject).indexOf("jQ") == -1) {
                        returnVal[property + '.' + prop] = flatObject[prop];
                    }
                }
            }
            else {
                if (property.indexOf("jQ") == -1)
                    returnVal[property] = objectToFlatten[property];
            }
        }
    }
    return returnVal;
};

function getElementDetails(formDesignData, fullPath) {
    try {
        if (formDesignData) {
            if (formDesignData["Section"] != null) {
                return getElementDetails(formDesignData.Section, fullPath);
            }
            for (var i = 0; i < formDesignData.Elements.length; i++) {
                var element = getElement(formDesignData.Elements[i], fullPath);
                if (element) {
                    if (formDesignData.ChildDataSources) {
                        $.each(formDesignData.ChildDataSources, function (idx, dataSource) {
                            if (dataSource.Mappings != undefined && dataSource.Mappings.length > 0) {
                                $.each(dataSource.Mappings, function (index, mapping) {
                                    if (mapping.TargetElement === element.GeneratedName) {
                                        element["DataSourceGeneratedName"] = 'INL_' + dataSource.DataSourceName + "_idx_" + element.GeneratedName;
                                    }
                                });
                            }
                        });
                    }
                    return element;
                }
            }
        }
    } catch (e) {
        console.log(e);
    }
}

function getElement(element, fullPath) {
    var fullNameList = element.FullName.split('.');
    var fullPathList = fullPath.split('.');
    if (element.FullName === fullPath) {
        return element;
    }
    if (element["Section"]) {
        return getElementDetails(element, fullPath);
    }
    if (element["Repeater"]) {
        return getElementDetails(element.Repeater, fullPath);
    }
    if (fullNameList[fullNameList.length - 1] == fullPathList[fullPathList.length - 1]) {
        return element;
    }
}

function getRepeaterColumn(repeaterID, rowNumber, columnName, columnNumber) {
    var td;
    try {
        if (columnName) {
            if (columnName.indexOf("idx"))
                td = $("table#" + repeaterID + " tr:eq(" + rowNumber + ") " + " td[aria-describedby='" + repeaterID + "_" + columnName.replace("idx", columnNumber) + "']");
            else
                td = $("table#" + repeaterID + " tr:eq(" + rowNumber + ") " + " td[aria-describedby='" + repeaterID + "_" + columnName + "']");
        }
    } catch (e) {
        console.log(e + " function getRepeaterColumn()");
    }
    return td;
}

function getRepeaterColumnPQ(repeaterID, rowNumber, columnName, columnNumber) {
    var td;
    try {
        var row = $("table#" + repeaterID).find("tr.pq-grid-row")[rowNumber];
        td = $(row).find("td:eq(" + (columnNumber + 1) + ")");
    }
    catch (e) {
        console.log(e + " function getRepeaterColumnPQ()");
    }
    return td;
}

function checkIfPQGridLoaded(isMasterList) {

    //var len = $('script[src*="Scripts/Framework/External/PQGrid/pqgrid.min.js"]').length;

    //if (len === 0)
    if(isMasterList)
        return false;
    else
        return true;
}

function getElementHierarchy(formDesignData, path) {
    var hierarchicalPath = "";
    try {
        if (path.indexOf('.') > -1) {
            var elementDetails = getElementDetails(formDesignData, path.substring(0, path.lastIndexOf('.')));
            if (elementDetails) {
                hierarchicalPath = getElementHierarchy(formDesignData, path.substring(0, path.lastIndexOf('.'))) + hierarchicalPath + " => " + elementDetails.Label;
            }
            else {
                return hierarchicalPath;
            }
        }
    } catch (e) {
        console.log(e);
    }
    return hierarchicalPath;
}


//Validate Effective Date
function isValidEffectiveDate(effectiveDate) {

    var dtCh = "/";
    var daysInMonth = getDaysInMonth(12);
    var pos1 = effectiveDate.indexOf(dtCh);
    var pos2 = effectiveDate.indexOf(dtCh, pos1 + 1);
    var strMonth = effectiveDate.substring(0, pos1);
    var strDay = effectiveDate.substring(pos1 + 1, pos2);
    var strYear = effectiveDate.substring(pos2 + 1);
    strYr = strYear;
    if (strDay.charAt(0) == "0" && strDay.length > 1) strDay = strDay.substring(1);
    if (strMonth.charAt(0) == "0" && strMonth.length > 1) strMonth = strMonth.substring(1);
    for (var i = 1; i <= 3; i++) {
        if (strYr.charAt(0) == "0" && strYr.length > 1) strYr = strYr.substring(1);
    }
    month = parseInt(strMonth);
    day = parseInt(strDay);
    year = parseInt(strYr);
    if (pos1 == -1 || pos2 == -1) {
        return Common.effectiveDateValidMsg;
    }
    if (strMonth.length < 1 || month < 1 || month > 12) {
        return Common.effectiveDateValidateMonthMsg;
    }
    if (strDay.length < 1 || day < 1 || day > 31 || (month == 2 && day > daysInFebruary(year)) || day > daysInMonth[month]) {
        return Common.effectiveDateValidateDayMsg;
    }
    if (strYear.length != 4 || year == 0) {
        return Common.effectiveDateValidateYearMsg;
    }
    return "";
}

//Validate day for February month 
function daysInFebruary(year) {
    // February has 29 days in any year evenly divisible by four,
    // EXCEPT for centurial years which are not also divisible by 400.
    return (((year % 4 == 0) && ((!(year % 100 == 0)) || (year % 400 == 0))) ? 29 : 28);
}

//Calculate number of day in month
function getDaysInMonth(n) {
    for (var i = 1; i <= n; i++) {
        this[i] = 31
        if (i == 4 || i == 6 || i == 9 || i == 11) { this[i] = 30 }
        if (i == 2) { this[i] = 29 }
    }
    return this;
}

//url and data options required 
//param should be string(multiple param can be sent with separator '<&') 
jQuery.download = function (url, param, method) {
    //url and data options required 
    if (url && param) {
        //split params into form inputs 
        var inputs = '';
        jQuery.each(param.split('<&'), function () {
            var pair = this.split('=');
            inputs += '<input type="hidden" name="' + pair[0] + '" value="' + pair[1] + '" />';
        });
        //send request 
        jQuery('<form action="' + url + '" method="' + (method || 'post') + '">' + inputs + '</form>')
        .appendTo('body').submit().remove();
    };
};

function sortDropDownElementItems(items) {
    if (items != null && items.length > 0) {
        items = items.sort(function (itemA, itemB) {
            if (!(itemA.hasOwnProperty("ItemValue") && itemB.hasOwnProperty("ItemValue"))) {
                throw "Item does not have a property 'ItemValue'.";
            }

            var itemA = itemA.ItemValue.toLowerCase().replace(/[^a-zA-Z0-9_-]/g, ''), itemB = itemB.ItemValue.toLowerCase().replace(/[^a-zA-Z0-9_-]/g, '');
            if (isFinite(itemA) && isFinite(itemB)) {
                return itemA - itemB;
            }
            else if (!isFinite(itemA) && !isFinite(itemB)) {
                if (itemA < itemB)
                    return -1;
                if (itemA > itemB)
                    return 1;
            }
            else if (isFinite(itemA)) {
                return 1;
            }
            else if (isFinite(itemB)) {
                return -1;
            }
            else {
                return 0;
            }
        });
    }
    return items;
}

//code to get HashColumn Data for DuplicationCheck Elements of Repeater
function getRepeaterHashColumnData(gridData, duplicationCheckEnabledElements) {
    var gridRowData = new Array();

    $.each(gridData, function (i, el) {
        if (el.RowIDProperty == undefined) {
            var id = null;
        }
        else {
            var id = el.RowIDProperty;
        }
        var hashString = "";
        for (var j = 0; j < duplicationCheckEnabledElements.length; j++) {
            $.each(el, function (elName, val) {
                if (val == null)
                {
                    val = "";
                }
                val = val.toString().trim();
                if (elName == duplicationCheckEnabledElements[j].GeneratedName) {
                    if (duplicationCheckEnabledElements[j].Type == "select" || duplicationCheckEnabledElements[j].Type == "SelectInput" || duplicationCheckEnabledElements[j].Type == "Dropdown List" || duplicationCheckEnabledElements[j].Type == "Dropdown TextBox") {
                        if (val == "[Select One]" || val == "") {
                            val = "[selectone]";
                        }
                    }
                    else if (val == "") {
                        val = "blank";
                    }

                    if (!isNaN(val)) {
                        var newVal = Number(val);
                    }
                    else {
                        var newVal = val.toLowerCase();
                    }

                    //#& is used as separator between column values.
                    //if we change separator(#) then we need to change it in handleDuplication and handleDuplications methods
                    //of formvalidationmanager.js 
                    hashString += newVal + "#";

                    return false;
                }
            });            
        }
        var hashRowData = {
            ID: id,
            Data: hashString,
        };
        gridRowData.push(hashRowData);
    });
    return gridRowData;
}

globalutilities.prototype.StringMethods = function () {
    //Checking if input isNullOrEmpty
    return {
        isNullOrEmpty: function (input) {
            var retValue = true;
            if (input !== undefined && input != null) {
                if (Array.isArray(input))
                {
                    if (input.length > 0)
                    {
                        if (input.length == 1)
                        {
                            if (!(input[0].trim() == '' || input[0] == Validation.selectOne))
                                retValue = false;
                        }
                        else {
                            retValue = false;
                        }
                    }
                }
                else {
                    if (!(input.trim() == '' || input == Validation.selectOne))
                        retValue = false;
                }
            }
            return retValue;
        },

        setEmptyString: function (input) {
            var emptyString;
            emptyString = '';
            return emptyString;
        }
    }
}

// This function is used to validate regular expression
function validateRegex(regex) {
    var isValid = true;
    try {
        new RegExp(regex);
    }
    catch (e) {

        isValid = false;
    }
    return isValid;
}

$.downloadNew = function (url, param, method) {

    var inputs = '';
    var iframeX;
    var downloadInterval;

    if (url && param) {

        // remove old iframe if has
        if (jQuery("#iframeX")) jQuery("#iframeX").remove();

        // creater new iframe
        iframeX = jQuery('<iframe src="" name="iframeX" id="iframeX"></iframe>').appendTo('body').hide();
        ajaxDialog.showPleaseWait();
        var browserInfo = getBrowserInfo();
        if (browserInfo.browser.toLowerCase() === "msie") {
            downloadInterval = setInterval(function () {
                // if loading then readyState is loading else readyState is interactive
                var state = document.getElementsByTagName("iframe").iframeX.contentDocument.readyState;
                if (state == "complete" || state == "interactive") {
                    ajaxDialog.hidePleaseWait();
                    clearInterval(downloadInterval);
                }
            }, 100);
        }
        else if (browserInfo.browser.toLowerCase() === "firefox") {
            downloadInterval = setInterval(function () {
                // if loading then readyState is loading else readyState is interactive
                var state = document.getElementsByTagName("iframe").iframeX.contentDocument.readyState;
                //var state = iframeX[0].contentDocument.readyState;
                if (state == "complete" || state == "interactive") {
                    ajaxDialog.hidePleaseWait();
                    clearInterval(downloadInterval);
                }
            }, 100);

            /* Stop the infinite loading loop after 8secs */
            setTimeout(function () {
                ajaxDialog.hidePleaseWait();
                clearInterval(downloadInterval);
            }, 8000);
        }
        else {//for Crome Browser
            downloadInterval = setInterval(function () {
                // if loading then readyState is loading else readyState is interactive
                var state = document.getElementsByTagName("iframe").iframeX.contentDocument.readyState;
                //var state = iframeX[0].contentDocument.readyState;
                if (state == "complete" || state == "interactive") {
                    ajaxDialog.hidePleaseWait();
                    clearInterval(downloadInterval);
                }
            }, 3000);
        }

        //split params into form inputs
        jQuery.each(param.split('<&'), function () {
            var pair = this.split('=');
            inputs += '<input type="hidden" name="' + pair[0] + '" value="' + pair[1] + '" />';
        });
        
        //create form to send request
        jQuery('<form action=' + url + ' method=' + (method || 'post') + ' target="iframeX">' + inputs + '</form>').appendTo('body').submit().remove();       
    };
};

// This function is used to handle Invalid Page Navigation
function IsEnteredPageExist(jqGridId) {
    var enteredPgNumber = parseInt($('#pg_p' + jqGridId + ' .ui-pg-table .ui-pg-input').val());
    var gridTotalPages = parseInt($('#' + jqGridId).getGridParam('lastpage'));
    var gridCurrentPage = parseInt($('#' + jqGridId).getGridParam('page'));
    if (enteredPgNumber > gridTotalPages || isNaN(enteredPgNumber)) {
        $('#pg_p' + jqGridId + ' .ui-pg-table .ui-pg-input').val(gridCurrentPage);
        return false;
    }
    return true;
}

// This function returns column index of selected column
function getColumnSrcIndexByName(grid, columnName) {   
   var cm = grid.jqGrid('getGridParam', 'colModel'), 
        i = 0, index = 0, l = cm.length, cmName;
    while (i < l) {
        cmName = cm[i].name;
        i++;
        if (cmName === columnName) {
            return index;
        } else if (cmName !== 'rn' && cmName !== 'cb' && cmName !== 'subgrid') {
            index++;
        }
    }
    return -1;
}
function getColumnSrcIndexByNamePQ(grid, columnName) {
    //var cm = grid.jqGrid('getGridParam', 'colModel'),
    var cm = grid.pqGrid("option", "colModel"),
          i = 0, index = 0, l = cm.length, cmName;
    while (i < l) {
        cmName = cm[i].dataIndx;
        i++;
        if (cmName === columnName) {
            return index;
        } else if (cmName !== 'rn' && cmName !== 'cb' && cmName !== 'subgrid') {
            index++;
        }
    }
    return -1;
}
//This function used for removing white spaces after resizing window
function autoResizing(formDesignGridId) {
    var totalColumnWidth = $(formDesignGridId).jqGrid().width();
    var gridWidth = $(formDesignGridId).getGridParam("width");
    if (totalColumnWidth != 0 && totalColumnWidth < gridWidth) {
        if ($(formDesignGridId).getGridParam('records') > 0) {
            $(formDesignGridId).setGridWidth(gridWidth, true);
        }
    }
}
// This function returns duplicate element count from array
function checkElementExists(data, id) {
    var t = 0;
    data.filter(function (ct) {
        if (ct == id) {
            t++;
        }
    });
    return t;    
}

function getBrowserInfo() {

    var a = {};

    try {

        navigator.vendor ?

                     /Chrome/.test(navigator.userAgent) ?

                     (a.browser = "Chrome", a.version = parseFloat(navigator.userAgent.split("Chrome/")[1].split("Safari")[0])) : /Safari/.test(navigator.userAgent) ? (a.browser = "Safari", a.version = parseFloat(navigator.userAgent.split("Version/")[1].split("Safari")[0])) : /Opera/.test(navigator.userAgent) && (a.Opera = "Safari", a.version = parseFloat(navigator.userAgent.split("Version/")[1])) : /Firefox/.test(navigator.userAgent) ? (a.browser = "Firefox",

                           a.version = parseFloat(navigator.userAgent.split("Firefox/")[1])) : (a.browser = "MSIE", /MSIE/.test(navigator.userAgent) ? a.version = parseFloat(navigator.userAgent.split("MSIE")[1]) : a.version = "edge")

    } catch (e) { a = e; }

    return a;

}

function getFormattedDateTime(currentDate) {
    var hours = currentDate.getHours();
    var minutes = currentDate.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var currentDateTime = (currentDate.getMonth() + 1) + "/"
                    + currentDate.getDate() + "/"
                    + currentDate.getFullYear() + " "
                    + hours + ":"
                    + minutes + " " + ampm;

    return currentDateTime;
}


function getFormattedDate(date) {
    var year = date.getFullYear();
    var month = (1 + date.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;
    var day = date.getDate().toString();
    day = day.length > 1 ? day : '0' + day;
    return month + '/' + day + '/' + year;
}

function serverTime() {
    var xmlHttp;
    try {
        //FF, Opera, Safari, Chrome
        xmlHttp = new XMLHttpRequest();
    }
    catch (err1) {
        //IE
        try {
            xmlHttp = new ActiveXObject('Msxml2.XMLHTTP');
        }
        catch (err2) {
            try {
                xmlHttp = new ActiveXObject('Microsoft.XMLHTTP');
            }
            catch (eerr3) {
                //AJAX not supported, use CPU time.
                alert("AJAX not supported");
            }
        }
    }
    xmlHttp.open('HEAD', window.location.href.toString(), false);
    xmlHttp.setRequestHeader("Content-Type", "text/html");
    xmlHttp.send('');
    return xmlHttp.getResponseHeader("Date");
}

function getGeneratedName(label) {
    var generatedName = "";
    if (label != null) {
        generatedName = label.replace(/[^a-zA-Z0-9]/g, '');
        if (generatedName.length > 70) {
            generatedName = generatedName.substring(0, 70);
        }
    }
    return generatedName;
}

function SetCursorAtEnd(elem) {
    var elemLen = elem.value.length;
    // For IE Only
    if (document.selection) {
        // Set focus
        elem.focus();
        // Use IE Ranges
        var oSel = document.selection.createRange();
        // Reset position to 0 & then set at end
        oSel.moveStart('character', -elemLen);
        oSel.moveStart('character', elemLen);
        oSel.moveEnd('character', 0);
        oSel.select();
    }
    else if (elem.selectionStart || elem.selectionStart == '0') {
        // Firefox/Chrome
        elem.selectionStart = elemLen;
        elem.selectionEnd = elemLen;
        elem.focus();
    } // if
}

function loadGridIfRequire(elementID, callback) {
    if (!$(elementID)[0].grid) {
        callback();
    }
}

function setDatePickerForInputType(elementId) {
    $(elementId).datepicker({
        dateFormat: 'mm/dd/yy',
        changeMonth: true,
        changeYear: true,
        yearRange: 'c-61:c+20',
        showOn: "both",
        buttonImage: Icons.CalenderIcon,
        buttonImageOnly: true,
        disabled: false
    }).parent().find('img').css('margin-bottom', '8px');
}

function filterDate(el, gridName) {
    $(el).datepicker({
        changeMonth: true,
        changeYear: true
    }).change(function () { gridName[0].triggerToolbar(); })

}

function adjustGridWidth(gridId, colModel, currentInstance) {
    var width = $(currentInstance.parent().closest('div').first()).width();
    $(gridId).jqGrid().setGridWidth(width, false);

    //code for adjusting width of grid coloum..               
    var columnCounter = 0;
    $.each(colModel, function (colId, colProp) {
        if (colProp.hidden === false) {
            columnCounter++;
        }
    });
    if (width > (150 * columnCounter)) {
        //to check no of coloumns and its width 
        //with the totalgrid width to be adjusted with the totalgrid width
        var totalColumnWidth = $(gridId).jqGrid().width();
        var gridWidth = $(gridId).getGridParam("width");

        if (totalColumnWidth != 0 && totalColumnWidth < gridWidth) {
            //if ($(elementIDs.tblLTIPJQ).getGridParam('records') > 0) {
            $(gridId).setGridWidth(gridWidth, true);
            //}
        }
    }
}

function adjustGridHeight(griId, IsSubGrid) {
    var height = 250;
    if (IsSubGrid) {
        height = 150;
    }
    var tableHeight = parseInt($(griId).css('height')) + 20;
    if (tableHeight < 40) {
        tableHeight = 40;
    }
    $(griId).jqGrid('setGridHeight', Math.min(height, tableHeight));
}

function getQueryString(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) { return pair[1]; }
    }
    return (null);
}

(function ($) {
    $.widget("custom.autoCompleteDropDown", {    
        _create: function () {
            this.wrapper = $("<span>")
              .addClass("custom-combobox")
              .insertAfter(this.element);
            var callback = this.options.value;
            var ID = this.options.ID;
            var disableField = this.options.isDisabled;
            this.element.hide();
            this._createAutocomplete(callback, ID);
            this._createShowAllButton();
            this._disableFields(disableField, ID);
        },
        _createAutocomplete: function (callback, ID) {
            var selected = this.element.children(":selected"),
              value = selected.val() ? selected.text() : "";
            this.input = $("<input>")
              .appendTo(this.wrapper)
              .val(value)
              .attr("title", "")
              .attr("id", ID)
              .addClass("custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left")
              .autocomplete({
                  delay: 0,
                  minLength: 0,
                  source: $.proxy(this, "_source")
              })
              .tooltip({
                  tooltipClass: "ui-state-highlight"
              });

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item.option.selected = true;
                    this._trigger("select", event, {
                        item: ui.item.option
                    });
                    if(callback != undefined)
                        callback(ui.item.option.value);
                },
                autocompletechange: "_removeIfInvalid"
            });
        },
        _createShowAllButton: function () {
            var input = this.input,
              wasOpen = false;
            $("<a> <span class='ui-button-icon-primary ui-icon ui-icon-triangle-1-s' style='margin: 5px 0 0 !important;'><span class='ui-button-text'>")
              .attr("tabIndex", -1)
              .attr("title", "Show All Items")
              //.attr("type", "text")
              .tooltip()
              .appendTo(this.wrapper)
              .button({
                  icons: {
                      primary: "ui-icon-triangle-1-s"
                  },
                  text: false
              })
              .removeClass("ui-corner-all")
              .addClass("ui-button ui-widget ui-state-default ui-button-icon-only custom-combobox-toggle ui-corner-right")
              .mousedown(function () {
                  wasOpen = input.autocomplete("widget").is(":visible");
              })
              .click(function () {
                  input.focus();
                  // Close if already visible
                  if (wasOpen) {
                      return;
                  }
                  // Pass empty string as value to search for, displaying all results
                  input.autocomplete("search", "");
              });
        },
        _disableFields: function (disabled, Id)
        {
            if (disabled == true) {
                                
                $('#' + Id).addClass('disabledField');
                $('#' + Id).siblings('a').addClass('disabledField');           
            }
        },
        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), "i");
            response(this.element.children("option").map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
        },
        _removeIfInvalid: function (event, ui) {
            // Selected an item, nothing to do
            if (ui.item) {
                return;
            }
            // Search for a match (case-insensitive)
            var value = this.input.val(),
              valueLowerCase = value.toLowerCase(),
              valid = false;
            this.element.children("option").each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });
            // Found a match, nothing to do
            if (valid) {
                return;
            }
            // Remove invalid value
            try
            {
                this.input.val("")
                   .attr("title", value + " didn't match any item")
                   .tooltip("open");
            }
            catch(e)
            {
                console.log(e);
            }
            this.element.val("");
            this._delay(function () {
                try
                {
                    this.input.tooltip("close").attr("title", "");
                }
                catch (e) {
                    console.log(e);
                }
            }, 2500);            
        },
        // added- inserted refresh function -BD
        refresh: function () {
            selected = this.element.children(":selected");
            this.input.val(selected.text());
        },        
        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        }
    });
})(jQuery);

function isHTML(str) {
    var a = document.createElement('div');
    a.innerHTML = str;
    for (var c = a.childNodes, i = c.length; i--;) {
        if (c[i].nodeType == 1) return true;
    }
    return false;
}

function isJSON(str) {
    try {
        JSON.parse(str);
    } catch (e) {
        return false;
    }
    return true;
}


function trim(x) {
    return x.replace(/^\s+|\s+$/gm, '');
}

