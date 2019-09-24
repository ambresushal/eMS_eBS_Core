var formDesignVersionRulesPreLoadManager = function () {

    var instance;

    var formDesignVersionRulesPreLoader = function () {
        this.URL = {
            updateUserRoles: '/FormDesign/FormDesignVersionRules'
        };

        this.formDesignVersionIDS = [];
        this.rulesContainer;
    }

    formDesignVersionRulesPreLoader.prototype.init = function () {
        if (this.hasRule(2339) == false || this.hasRule(2347) == false || this.hasRule(2364) == false || this.hasRule(2372) == false) {
            var currentInstance = this;
            var formDesignData = {
                tenantID: 1,
                formDesignVersionIDS: this.formDesignVersionIDS
            }

            var promise = ajaxWrapper.postJSON(currentInstance.URL.updateUserRoles, formDesignData, false);
            //ajax success callback - for add/edit
            promise.done(function (xhr) {
                currentInstance.rulesContainer = xhr;
            });
        }
    }

    formDesignVersionRulesPreLoader.prototype.hasRule = function (formDesignVersionID) {
        var foundResult = false;
        var found = null;
        if (this.rulesContainer != null && this.rulesContainer.length > 0) {
            //found = this.rulesContainer.find(function (rule) {
            //    return rule.FormDesignVersionId === formDesignVersionID;
            //});
            found = jQuery.grep(this.rulesContainer, function (rule) {
                return rule.FormDesignVersionId === formDesignVersionID;
            });
        }
        if (found != null && found.length > 0 && found[0].Rules != null && found[0].Validations != null) {
            foundResult = true;
        }
        return foundResult;
    }

    formDesignVersionRulesPreLoader.prototype.getRule = function (formDesignVersionID) {
        var found = null;
        if (this.rulesContainer != null && this.rulesContainer.length > 0) {
            //found = this.rulesContainer.find(function (rule) {
            //    return rule.FormDesignVersionId === formDesignVersionID;
            //});
            found = jQuery.grep(this.rulesContainer, function (rule) {
                return rule.FormDesignVersionId === formDesignVersionID;
            });
        }
        return found[0].Rules;
    }

    formDesignVersionRulesPreLoader.prototype.getValidation = function (formDesignVersionID) {
        var found = null;
        if (this.rulesContainer != null && this.rulesContainer.length > 0) {
            //found = this.rulesContainer.find(function (rule) {
            //    return rule.FormDesignVersionId === formDesignVersionID;
            //});
            found = jQuery.grep(this.rulesContainer, function (rule) {
                return rule.FormDesignVersionId === formDesignVersionID;
            });
        }
        return found[0].Validations;
    }

    return {
        getInstance: function () {
            if (instance == undefined) {
                instance = new formDesignVersionRulesPreLoader();
            }
            return instance;
        }
    }
}();


