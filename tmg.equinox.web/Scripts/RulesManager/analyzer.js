var analyzer = (function () {
    var URLs = {
        getHierarchy: '/RulesManager/GetRuleHierarchy?tenantId=1&formDesignVersionId={formDesignVersionId}&ruleId={ruleId}',
        getRuleSources: '/RulesManager/GetSourcesByRule?tenantId=1&formDesignVersionId={formDesignVersionId}&ruleId={ruleId}'
    };

    var elementIDs = {
        ruleAnalyzer: '#ruleAnalyzer',
        ruleAnalyzerDetails: '#ruleAnalyzerDetails',
        sourceElement: '#radSourceElement',
        targetElement: '#radTargetElement',
        ruleDesc: '#radRuleDesc',
        ruleTarget: '#radRuleTarget',
    }
    var ruleList;
    var tree;
    function buildRuleAnalyzer(rowData, formDesignVersionId) {
        getRuleHierarchy(rowData, formDesignVersionId);
    }

    function getRuleHierarchy(ruleData, formDesignVersionId) {
        var currentInstance = this;
        var analyzerUrl = URLs.getHierarchy.replace(/\{formDesignVersionId}/g, formDesignVersionId).replace(/\{ruleId\}/g, ruleData.RuleID);
        var promise = ajaxWrapper.getJSON(analyzerUrl);
        promise.done(function (xhr) {
            if (xhr != null && xhr.length > 0) {
                var configContainer = {};
                configContainer.config = {
                    container: "#ruleanalyzer",
                    callback: {
                        onTreeLoaded: function () {
                            var $oNodes = $('.Treant .node');
                            $oNodes.on('click', function (oEvent) {
                                var nodeId = this.id;
                                var rule = $(ruleList).filter(function (idx, item) {
                                    return item.htmlid == nodeId;
                                });
                                //$("#radSourceElement").html(rule[0].source);
                                //get rule sources
                                var ruleSourceUrl = URLs.getRuleSources.replace(/\{formDesignVersionId}/g, formDesignVersionId).replace(/\{ruleId\}/g, ruleData.RuleID);
                                $("#radSourceElement").html("");
                                var promiseRS = ajaxWrapper.getJSON(ruleSourceUrl);
                                promiseRS.done(function (xhr) {
                                    var sources = "";
                                    for (var xh in xhr) {
                                        if (xh < xhr.length - 1) {
                                            sources = sources + xhr[xh].Section + " > " + xhr[xh].Element + "<br/>";
                                        } else {
                                            sources = sources + xhr[xh].Section + " > " + xhr[xh].Element;
                                        }
                                    }
                                    $("#radSourceElement").html(sources);
                                    $("#radTargetElement").html(rule[0].target);
                                    $("#radRuleDesc").html(rule[0].ruledesc);
                                    $("#radRuleTarget").html(rule[0].targetprop);
                                });
                            });
                        }
                    },
                    rootOrientation: 'WEST',
                    siblingSeparation: 75,
                    subTeeSeparation: 75,
                    connectors: {
                        style: {
                            "stroke-width": 1,
                            'arrow-end': 'block-wide-long'
                        }
                    },
                    animateOnInit: true,
                    node: {
                        collapsable: true
                    },
                    animation: {
                        nodeAnimation: "easeOutBounce",
                        nodeSpeed: 700,
                        connectorsAnimation: "bounce",
                        connectorsSpeed: 700
                    }
                };
                ruleList = xhr;
                $.each(xhr, function (idx, val) {
                    configContainer[val.key] = {};
                    if (val.collapse == 1) {
                        configContainer[val.key].collapsed = true;
                    }
                    configContainer[val.key].HTMLid = val.htmlid;
                    configContainer[val.key].HTMLClass = val.htmlid;
                    if (val.parent != null) {
                        configContainer[val.key].parent = configContainer[val.parent];
                    }
                    configContainer[val.key].text = val.text;
                });
                var configArr = [];
                for (var prop in configContainer) {
                    configArr.push(configContainer[prop]);
                }
                if (tree != null) {
                    tree.destroy();
                    $("#radSourceElement").html("");
                    $("#radTargetElement").html("");
                    $("#radRuleDesc").html("");
                    $("#radRuleTarget").html("");
                }
                tree = new Treant(configArr);
            }
        });
        promise.fail(currentInstance.showError);
    }

    function showError(xhr) {
        if (xhr.status == 999)
            window.location.href = '/Account/LogOn';
        else
            alert(JSON.stringify(xhr));
    }

    function _cleanUp() {
        $(elementIDs.ruleAnalyzer).html("");
        $(elementIDs.sourceElement).html("");
        $(elementIDs.targetElement).html("");
        $(elementIDs.ruleDesc).html("");
        $(elementIDs.ruleTarget).html("");
    }
    return {
        show: function (ruleData, formDesignVersionId, viewType) {
            _cleanUp();
            buildRuleAnalyzer(ruleData, formDesignVersionId, viewType);
        },
        hide: function () { },
        cleanUp: function () {
            _cleanUp();
        }
    };
}());
