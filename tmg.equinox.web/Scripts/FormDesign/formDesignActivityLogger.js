﻿function formDesignActivityLogger(formDesignId, formDesignName) {
    this.formDesignId = formDesignId;
    this.formDesignName = formDesignName;
    this.formDesignVersion;
    this.bottomMenu;
    this.disclaimer = "Disclaimer:The Activity Log description is generated by comparing the current version of the design with the previous version and may not always reflect the latest action on the element.";

    this.URLs = {
        getActivityLogData: "/FormDesign/GetFormDesignVersionActivityLog?formDesignId={formDesignId}&formDesignVersionId={formDesignVersionId}",
        exportToExcelJqgrid: "/FormDesign/FormDesignVersionActivityLogToExcel"
    }

    this.elementIDs = {
        bottomMenuJQ: "#bottom-menu",
        bottomMenuTabsJQ: "#bottom-menu-tabs",
        activityLogGridNameJQ: "#designActivityLog",
        activityLogGridName: "designActivityLog",
        spnDisclaimer: "#spnDisclaimer"
    }
}

formDesignActivityLogger.prototype.init = function (formDesignVersion) {
    var currentInstance = this;
    currentInstance.formDesignVersion = formDesignVersion;
    currentInstance.bottomMenu = currentInstance.bottomMenuMethods();
    currentInstance.bottomMenu.renderBottomMenu();
    $(currentInstance.elementIDs.spnDisclaimer).html(currentInstance.disclaimer);
}

formDesignActivityLogger.prototype.loadActivityLogGrid = function (formDesignVersion) {
    var currentInstance = this;
    if (formDesignVersion != undefined) {
        currentInstance.formDesignVersion = formDesignVersion;
    }
       
    $(currentInstance.elementIDs.activityLogGridNameJQ).jqGrid('GridUnload');
    var pagerElement = "#p_" + currentInstance.elementIDs.activityLogGridName;
    var url = currentInstance.URLs.getActivityLogData
            .replace(/{formDesignId}/g, currentInstance.formDesignId)
            .replace(/{formDesignVersionId}/g, currentInstance.formDesignVersion.FormDesignVersionId)

    $(currentInstance.elementIDs.activityLogGridNameJQ).parent().append("<div id='p_" + currentInstance.elementIDs.activityLogGridName + "'></div>");
    $(currentInstance.elementIDs.activityLogGridNameJQ).jqGrid({
        url: url,
        datatype: "json",
        rowList: [10, 20, 30],
        colNames: ['Element Path', 'Field', 'Description', 'Version', 'Updated By', 'Updated Last'],
        colModel: [{ name: 'ElementPath',  dataIndx: 'ElementPath'   },                            
                   { name: 'ElementLabel', dataIndx: 'ElementLabel' },                               
                   { name: 'Description',  dataIndx: 'Description', classes: 'ignorcare' },          
                   { name: 'Version',      dataIndx: 'Version' },                                            
                   { name: 'UpdatedBy',    dataIndx: 'UpdatedBy' },                                    
                   { name: 'UpdatedDate',  dataIndx: 'UpdatedDate', formatter: 'date', formatoptions: JQGridSettings.DateTimeFormatterOptions, align: 'center' }
        ],
        pager: pagerElement,
        altRows: true,
        loadonce: true,
        cache: false,
        altclass: 'alternate',
        height: '160',
        hidegrid: false,
        rowNum: 50,
        autowidth: true,
        shrinkToFit: false,
        viewrecords: true,
        sortname: 'UpdatedDate',
        sortorder: 'desc',
    });

    $(currentInstance.elementIDs.activityLogGridNameJQ).jqGrid('filterToolbar', { stringResult: true, searchOnEnter: true, defaultSearch: "cn" });
    $(currentInstance.elementIDs.activityLogGridNameJQ).jqGrid('navGrid', pagerElement, { edit: false, add: false, del: false, refresh: false, search: false }, {}, {}, {});
    $(pagerElement + "_left").css("width", "");

    $(currentInstance.elementIDs.activityLogGridNameJQ).jqGrid('navButtonAdd', pagerElement,
   {
       caption: '', buttonicon: 'ui-icon-arrowstop-1-s', title: 'Download Activity Log To Excel', id: 'btnActivityLogExportToExcel',
       onClickButton: function () {
           var jqGridtoCsv = new JQGridtoCsv(currentInstance.elementIDs.activityLogGridNameJQ, false, currentInstance);
           jqGridtoCsv.buildExportOptions();
           var stringData = "csv=" + jqGridtoCsv.csvData.replace(/(<([^>]+)>)|\n|>/ig, "").replace(/=/g, "->");
           stringData += "<&header=" + "Form Design Version " + currentInstance.formDesignName + "-" + currentInstance.formDesignVersion.Version + "\n\n" + currentInstance.disclaimer;
           stringData += "<&noOfColInGroup=" + jqGridtoCsv.noofGroupedColumns;
           stringData += "<&repeaterName=" + "Form Design Version Activity Log";

           $.download(currentInstance.URLs.exportToExcelJqgrid, stringData, 'post');
       }
   });
}

formDesignActivityLogger.prototype.bottomMenuMethods = function () {
    var currentInstance = this;

    return {
        renderBottomMenu: function () {
            $(currentInstance.elementIDs.bottomMenuJQ).show();
            currentInstance.loadActivityLogGrid();
            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs();
            currentInstance.bottomMenu.registerEvents();
            $(currentInstance.elementIDs.bottomMenuTabsJQ + " .ui-jqgrid").show();
        },
        registerEvents: function () {
            $(currentInstance.elementIDs.bottomMenuJQ).css('height', '44px');

            $(currentInstance.elementIDs.bottomMenuTabsJQ + " .bottom-menu-button").unbind("click");
            $(currentInstance.elementIDs.bottomMenuTabsJQ + " .bottom-menu-button").bind("click", function () {
                if ($(this).hasClass("glyphicon-plus-sign")) {
                    currentInstance.bottomMenu.openBottomMenu();
                }
                else {
                    currentInstance.bottomMenu.closeBottomMenu();
                }
            });

            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs({
                activate: function (event, ui) {
                    if (ui.newTab[0].innerText == "Activity Log") {
                        currentInstance.loadActivityLogGrid();
                    }
                }
            });
        },
        openBottomMenu: function () {
            $(currentInstance.elementIDs.bottomMenuJQ).css('height', '330px');

            $(".bottom-menu-button").removeClass("glyphicon-plus-sign");
            $(".bottom-menu-button").addClass("glyphicon-minus-sign");
        },
        closeBottomMenu: function () {
            if ($(currentInstance.elementIDs.bottomMenuJQ).is(":visible")) {
                $(currentInstance.elementIDs.bottomMenuJQ).css('height', '46px');

                $(".bottom-menu-button").removeClass("glyphicon-minus-sign");
                $(".bottom-menu-button").addClass("glyphicon-plus-sign");
            }
        },
        showActivityLogGridTab: function () {
            currentInstance.bottomMenu.openBottomMenu();
            $(currentInstance.elementIDs.bottomMenuTabsJQ).tabs("option", "active", 0);
        }
    }
}