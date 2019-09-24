function formDesignVersion(formDesignVersion, formDesignId, formDesignName, finalizedstatus, tabNamePrefix, tabIndex, tabCount, tabs) {
    this.formDesignId = formDesignId;
    this.formDesignVersion = formDesignVersion;
    this.tabNamePrefix = tabNamePrefix;
    this.tabs = tabs;
    this.tabCount = tabCount;
    this.tabIndex = tabIndex;
    this.formDesignName = formDesignName;
    this.status = finalizedstatus;
    this.viewFactory = new FormDesignViewFactory();
    this.objFormDesignView = null;
    this.designActivityLogger = new formDesignActivityLogger(formDesignId,formDesignName);
}

formDesignVersion.prototype.loadTabPage = function () {
    var tabName = this.formDesignName + '-' + this.formDesignVersion.Version;

    //create link for the tab page
    var tabTemplate = "<li><a id= '" + this.formDesignName + "-" + this.formDesignVersion.Version + "' href='#{href}'>#{label}</a> <span class='ui-icon ui-icon-close' data-tabid=" + this.formDesignVersion.FormDesignVersionId + " role='presentation'>Remove Tab</span></li>";

    //replace based on version being loaded for this form
    var li = $(tabTemplate.replace(/#\{href\}/g, '#' + this.tabNamePrefix).replace(/#\{label\}/g, this.formDesignName + '-' + this.formDesignVersion.Version));

    //create div for the tab page content
    var tabContentTemplate = "<div class='form-inline margin-10'><div class='form-group'><label>View:</label><span class='select-wrapper'><select class='form-control' id='viewTypeDDL{formDesignVersionId}'><option value='1'>Config View</option><option value='2'>Default View</option></select></span></div><div id='actionContainer{formDesignVersionId}' class='action-container clearfix'></div></div><div class='row hot-wrapper'><div id='divfdvuielems{formDesignVersionId}'></div></div><div class='row grid-wrapper'><div class='col-xs-7 scroll-helpe'><div class='left-sidebar'><table id='fdvuielems{formDesignVersionId}'></table></div></div><div class='col-xs-5'><div id='fdvuielemdetail{formDesignVersionId}container' class='right-sidebar'><div class='grid-wrapper'><table id='fdvuielemdetail{formDesignVersionId}'></table></div></div></div></div>";
    var tabContent = tabContentTemplate.replace(/\{formDesignVersionId\}/g, this.formDesignVersion.FormDesignVersionId);

    //create tab page using jqueryui tab methods
    this.tabs.find('.ui-tabs-nav').append(li);
    this.tabs.append("<div id='" + this.tabNamePrefix + "' style ='overflow:auto'>" + tabContent + "</div><div style='clear:both'></div>");
    this.tabs.tabs('refresh');

    if (Finalized.length > 0) {
        this.tabs.tabs({ selected: (this.tabCount - 1) });
    }
    else
        this.tabs.tabs('option', 'active', this.tabCount);

    var currentInstance = this;

    //This function is used to nullify the instance of formdesignversion on its close event.
    $(currentInstance.tabs).find('span[data-tabid=' + currentInstance.formDesignVersion.FormDesignVersionId + ']').on("click", function () {
        currentInstance = null;
    });

    tabs = $('#formdesigntabs').tabs();
    //click event of tab
    tabs.delegate('.ui-tabs-anchor', 'click', function (ctx) {
        var id = ctx.currentTarget.id;

        if (id == "ui-id-42") $('#bottom-menu').hide();

        if (currentInstance != null && currentInstance != undefined) {
            var tabName = currentInstance.formDesignName + "-" + currentInstance.formDesignVersion.Version;
            for (var i = 0; i < Finalized.length ; i++) {
                if (Finalized[i].FORMDESIGNVERSIONID == currentInstance.formDesignVersion.FormDesignVersionId && Finalized[i].ISFINALIZED == 1) {
                    if (tabName == id && currentInstance.formDesignName != "Document Designs") {
                        if (Finalized[i].ISMESSAGEDISPLAYED != true) {
                            messageDialog.show(DocumentDesign.versionFinalizedMsg);
                            Finalized[i].ISMESSAGEDISPLAYED = true;
                            $('#fdvuielemdetail' + currentInstance.formDesignVersion.FormDesignVersionId).trigger('reloadGrid');
                            var rowID = $('#fdvuielems' + currentInstance.formDesignVersion.FormDesignVersionId).getGridParam('selrow');
                            if (rowID != null) {
                                var row = $('#fdvuielems' + currentInstance.formDesignVersion.FormDesignVersionId).getRowData(rowID);
                                var propGrid = new uiElementPropertyGrid(row, currentInstance.formDesignId.formDesignId, currentInstance.formDesignVersion.FormDesignVersionId, currentInstance.status, currentInstance.getGridData(), currentInstance);
                                propGrid.loadPropertyGrid();
                            }
                        }
                    }
                }
            }

            if (tabName == id && currentInstance.formDesignName != "Document Designs") {
                $('#bottom-menu').show();
                $(".bottom-menu-button").removeClass("glyphicon-plus-sign");
                $(".bottom-menu-button").addClass("glyphicon-minus-sign");
                currentInstance.designActivityLogger.loadActivityLogGrid(currentInstance.formDesignVersion);
            }
        }
    });

    var drpViewType = '#viewTypeDDL' + this.formDesignVersion.FormDesignVersionId;
    $(drpViewType).val('2');
    $(drpViewType).on('change', function () {
        var viewType = '';
        if (this.value == '1') {
            viewType = 'ExcelView';
        }
        currentInstance.loadUIElementGrid(viewType);
    });

    this.loadUIElementGrid();
    this.designActivityLogger.init(this.formDesignVersion);
}

formDesignVersion.prototype.loadUIElementGrid = function (viewType) {
    var currentInstance = this;

    var params = {
        FormDesignVersion: currentInstance.formDesignVersion,
        FormDesignId: currentInstance.formDesignId,
        FormDesignName: currentInstance.formDesignName,
        Status: currentInstance.status,
    }

    if (currentInstance.objFormDesignView != null) {
        currentInstance.objFormDesignView.destroy();
    }
    currentInstance.objFormDesignView = currentInstance.viewFactory.createFormInstance(viewType, params);
    currentInstance.objFormDesignView.init();
}

function FormDesignViewFactory() {
    this.createFormInstance = function (viewType, params) {
        var objFormDesignView;

        if (viewType == 'ExcelView') {
            objFormDesignView = new formDesignView(params.FormDesignVersion, params.FormDesignId, params.FormDesignName, params.Status);
        }
        else {
            objFormDesignView = new formDesignViewjq(params.FormDesignVersion, params.FormDesignId, params.FormDesignName, params.Status);
        }

        return objFormDesignView;
    }
}
