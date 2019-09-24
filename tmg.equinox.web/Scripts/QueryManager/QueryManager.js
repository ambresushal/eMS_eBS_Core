var settings = function () {
    isInitialized = false;

    URLs = {
        executeQuery: '/QueryManager/ExecuteQuery',
        getSQLObjectList: '/QueryManager/GetSQLObjectList?cnName={schema}&selectedIndex={index}',
        getSQLObjectScript: '/QueryManager/GetSQLObjectScript'
    };
    
    var elementIDs = {
        settingsTabJQ: "#QTabs",
        btnNewQueryJQ: "#btnNewQuery",
        btnExecuteJQ: "#btnExecute",
        btnParseJQ: "#btnParse",
        ddSchemaName: '#ddSchemaName',
        ddObjectTypeJQ: '#ddObjectType',
        lstSqlObjectsJQ: '#lstSqlObjects',
        queryTextJQ: '#queryText',
        chkCommitJQ: '#chkCommit',
        resultTextJQ: '#resultText',
        btnExportScriptJQ: '#btnExportScript'
    };

    //properties
    property = {
        schema: $(elementIDs.ddSchemaName).val() == "0" ? "IntegrationContext" : "UIFrameworkContext",
        selObjType: $(elementIDs.ddObjectTypeJQ).val(),
        selObjId: 0
    };

    //this function is called below soon after this JS file is loaded
    function init() {
        $(document).ready(function () {
            if (isInitialized == false) {
                $(elementIDs.settingsTabJQ).tabs().tabs("select", 0);
                bindOnChangeOfDatabaseDropdown();
                ObjectTypeChangeDropdown();
                bindOnChangeOfSelectObject();

                $(elementIDs.btnExecuteJQ).click(function (e) {
                    executeQuery();
                });

                //$(elementIDs.btnParseJQ).click(function (e) {
                
                //});

                $(elementIDs.btnNewQueryJQ).click(function (e) {
                    $(elementIDs.queryTextJQ).val('');
                    $(elementIDs.resultTextJQ).val('');
                    $(elementIDs.chkCommitJQ).prop("checked", false);
                });

                $(elementIDs.btnExportScriptJQ).click(function (e) {
                    exportSQLObjectScript();
                });

                isInitialized = true;
            }
        });
    }

    function showError(xhr) {
        if (xhr.status == 999)
            this.location = '/Account/LogOn';
        else
            messageDialog.show(JSON.stringify(xhr));
    }

    function executeQuery() {
        var isExecute = true;

        var query = $(elementIDs.queryTextJQ).val();//.replace("\n"," ");
        if ($(elementIDs.chkCommitJQ).is(":checked")) {
            if (query.toLowerCase().indexOf("select") === -1) {
                messageDialog.show(QManager.selectbeforeafterMsg);
                isExecute = false;
            }
        }

        if (isExecute)
        {
            var encodeString = encodeURIComponent(query);
            var stringData = "cnName=" + property.schema;
            stringData += "<&queryText=" + encodeString;
            stringData += "<&IsCommit=" + $(elementIDs.chkCommitJQ).is(":checked");
            
            $.download(URLs.executeQuery, stringData, 'post');
        }
    }

    //Bind Schema change dropdown change event.
    function bindOnChangeOfDatabaseDropdown() {
        $(elementIDs.ddSchemaName).on("change", function (e) {
            property.schema = e.originalEvent.srcElement.selectedIndex === 0 ? "IntegrationContext" : "UIFrameworkContext";
            $(elementIDs.lstSqlObjectsJQ).empty();
            $(elementIDs.ddObjectTypeJQ).val(0);
        });
    }

    //Bind value to property.selObjType on select option change event.
    function bindOnChangeOfSelectObject() {
        $(elementIDs.lstSqlObjectsJQ).on("change", function (e) {
            property.selObjId = $(this).val();
        });

        $(elementIDs.lstSqlObjectsJQ).dblclick(function () {
            property.selObjId = $(this).val();
            if (property.selObjId != null)
                exportSQLObjectScript();
        });
    }

    //Load sql objects.
    function ObjectTypeChangeDropdown() {
        $(elementIDs.ddObjectTypeJQ).on("change", function (e) {            
            var url = URLs.getSQLObjectList.replace(/\{schema\}/g, property.schema).replace(/\{index\}/g, e.originalEvent.srcElement.selectedIndex);
            var promise1 = ajaxWrapper.getJSON(url);
            property.selObjType = e.originalEvent.srcElement.selectedIndex;

            $(elementIDs.lstSqlObjectsJQ).empty();

            //callback function for ajax request success.
            promise1.done(function (xhr) {
                $.each(xhr, function (index, element) {
                    $(elementIDs.lstSqlObjectsJQ).append("<option value=" + element['id'] + ">" + element['schema'] + '.' + element['name'] + "</option>");
                });                                       
            });

            promise1.fail(showError);
            
        });
    }

    function exportSQLObjectScript() {
        if (property.selObjId === "0")
        {
            messageDialog.show(QManager.selectatleastoneobjectMsg);
        }
        else
        {
            var stringData = "cnName=" + property.schema;
            stringData += "<&id=" + property.selObjId;
            stringData += "<&objectType=" + property.selObjType;

            $.download(URLs.getSQLObjectScript, stringData, 'post');            
        }
    }

    init();
}();


