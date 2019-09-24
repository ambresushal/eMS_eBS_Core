var rulescomparer = (function () {
    var _compareData = [
        { index: 0, data: {} },
        { index: 1, data: {} },
        { index: 2, data: {} },
        { index: 3, data: {} }
    ];

    var elementIDs = {
        clearRuleJQ: '.clear-rule',
        ruleListGridJQ: '#rulemasterlist',
        ruleContainerJQ: '#rulcontainer{idx}',
        ruleCodeJQ: '#thRuleCode{idx}',
        ruleNameJQ: '#tdRuleName{idx}',
        ruleTypeJQ: '#tdRuleType{idx}',
        ruleDescriptionJQ: '#tdDescription{idx}'
    };

    function _resigterEvents() {
        $(elementIDs.clearRuleJQ).off('click').on('click', function () {
            var index = this.id;
            _removeRule(index);
        });
    }

    function _removeRule(index) {
        var container = elementIDs.ruleContainerJQ.replace(/\{idx\}/g, index);
        $(container).attr('title', 'Select the rule from below list and click on compare button to add rule.')
        $(container).find('.addCricle').show();
        $(container).find('.addTitle').show();
        $(container).find('.basic-grid').hide();
        $(elementIDs.ruleCodeJQ.replace(/\{idx\}/g, index)).text('');
        $(elementIDs.ruleNameJQ.replace(/\{idx\}/g, index)).text('');
        $(elementIDs.ruleTypeJQ.replace(/\{idx\}/g, index)).text('');
        $(elementIDs.ruleDescriptionJQ.replace(/\{idx\}/g, index)).html('');
        $.each(_compareData, function (idx, row) {
            if (idx == index) {
                row.data = {};
            }
        });
    }

    function _validate() {
        var isValid = false;
        $.each(_compareData, function (idx, row) {
            if (!row.data.hasOwnProperty('RuleID')) {
                isValid = true;
                return;
            }
        });
        return isValid;
    }

    function _checkIfRuleExist(ruleData) {
        var isExist = false;
        $.each(_compareData, function (idx, row) {
            if (row.data.hasOwnProperty('RuleID')) {
                if (row.data.RuleID == ruleData.RuleID) {
                    isExist = true;
                    return;
                }
            }
        });

        return isExist;
    }

    function _buildComparer(ruleData) {
        var rowAdded = false;
        var ruleIdx = -1;
        if (_validate()) {
            if (!_checkIfRuleExist(ruleData)) {
                $.each(_compareData, function (idx, row) {
                    if (!row.data.hasOwnProperty('RuleID') && rowAdded == false) {
                        row.data = ruleData;
                        ruleIdx = idx;
                        rowAdded = true;
                    }
                });
                var container = elementIDs.ruleContainerJQ.replace(/\{idx\}/g, ruleIdx);
                $(container).removeAttr('title');
                $(container).find('.addCricle').hide();
                $(container).find('.addTitle').hide();
                $(container).find('.basic-grid').show();
                $(elementIDs.ruleCodeJQ.replace(/\{idx\}/g, ruleIdx)).text('Rule - ' + (ruleIdx + 1));
                $(elementIDs.ruleNameJQ.replace(/\{idx\}/g, ruleIdx)).text(ruleData.RuleName);
                $(elementIDs.ruleTypeJQ.replace(/\{idx\}/g, ruleIdx)).text(ruleData.Type);
                $(elementIDs.ruleDescriptionJQ.replace(/\{idx\}/g, ruleIdx)).html('<p class="ellipsis">' + ruleData.Description + '</p>');
            }
            else {
                messageDialog.show('This rule is already added for comparision.');
            }

        }
        else {
            messageDialog.show('Maximum 4 rules can be compared.');
        }
    }

    function _cleanUp() {
        $.each(_compareData, function (idx, row) {
            if (row.data.hasOwnProperty("RuleID")) {
                _removeRule(idx);
            }
        });
    }

    return {
        compare: function (comparedata) {
            _resigterEvents();
            _buildComparer(comparedata);
        },
        cleanUp: function () {
            _cleanUp();
        }
    };
}());
