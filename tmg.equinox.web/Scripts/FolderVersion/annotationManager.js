var annotationManager = function (formInstance) {

    var formInstancebuilder = formInstance;

    var URLs = {
        getAnnotations: '/FormInstance/GetAnnotations?formInstanceId={formInstanceId}&formDesignVersionId={formDesignVersionId}',
        annotationReport: '/FormInstance/AnnotationReport',
    }

    var elementIDs = {
        annotationGridJQ: "#annotationGrid",
    }

    function GetDataAndAddRows() {
        var url = URLs.getAnnotations.replace("{formInstanceId}", formInstancebuilder.formInstanceId)
                                    .replace("{formDesignVersionId}", formInstancebuilder.formDesignVersionId);
        var promise = ajaxWrapper.getJSON(url);

        promise.done(function (result) {
            try {
                for (var i = 0; i < result.length; i++) {
                    result[i]["ID"] = i + 1;
                    $(elementIDs.annotationGridJQ).jqGrid('addRowData', i + 1, result[i]);
                }
            } catch (e) {
                console.log(e.message);
            }
        });
    }

    function buildAnnotationGrid() {
        $(elementIDs.annotationGridJQ).jqGrid('GridUnload');

        $(elementIDs.annotationGridJQ).jqGrid({
            datatype: "local",
            colNames: ['ID', 'Element Path', 'Field', 'Annotation For', 'Annotation', 'Added By'],
            colModel: [{ name: 'ID', index: 'ID', key: true, hidden: true },
                       { name: 'ElementPath', index: 'ElementPath', width: '250' },
                       { name: 'Field', index: 'Field', width: '200' },
                       { name: 'AnnotationFor', index: 'AnnotationFor', width: '200' },
                       { name: 'Annotation', index: 'Annotation', width: '300' },
                       { name: 'AuthorName', index: 'AuthorName', width: '100' }
            ],
            pager: '#pannotationGrid',
            autowidth: true,
            shrinkToFit: true,
            height: '200',
            altRows: true,
            rownumbers: true,
            rownumWidth: 40,
            pgbuttons: false,
            pgtext: null,
            hiddengrid: false,
            hidegrid: false,
            caption: 'Annotaion List',
            onSelectRow: function (rowId) {
                //errorgridRowSelect(rowId);
            },
            gridComplete: function () {
            }
        });
        $(elementIDs.annotationGridJQ).jqGrid('navGrid', "#pannotationGrid", { edit: false, add: false, del: false, refresh: false, search: false }, {}, {}, {});
        $(elementIDs.annotationGridJQ).jqGrid('navButtonAdd', "#pannotationGrid", {
            caption: 'Get Annotations', buttonicon: 'ui-icon-comment', title: 'Load',
            onClickButton: function () {
                GetDataAndAddRows();
            }
        });

        $(elementIDs.annotationGridJQ).jqGrid('navButtonAdd', "#pannotationGrid", {
            caption: 'Download', buttonicon: 'ui-icon-arrowthickstop-1-s', title: 'Excel Download',
            onClickButton: function () {
                var sourceData = JSON.stringify($(elementIDs.annotationGridJQ).jqGrid("getRowData"));
                var stringData = "tenantId=" + 1;
                stringData += "<&source=" + encodeURIComponent(sourceData);
                $.downloadNew(URLs.annotationReport, stringData, 'post');
            }
        });
    }

    return {
        buildAnnotationGrid: function () {
            buildAnnotationGrid();
        }
    }
}
