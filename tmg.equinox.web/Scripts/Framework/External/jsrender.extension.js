$.views.tags({
    debug: function (obj) {
        var props = this.props;
        // output a default message if the user didn't specify a message
        var msg = props.message || 'Debug:';
        console.log(msg, obj);
    }
});

$.views.helpers({
    addRepeater: function addRepeater(repeaterName) {
        this.ctx.repeaterManager.add(repeaterName);
    }
});


$.views.helpers({
    getCssClass: function getCssClass(columnCount, Type, isLabel) {
        var cssClass = "";

        if (Type === 'section') {
            columnCount = columnCount - 1;
            cssClass = "col-xs-12 col-md-12 col-lg-12 col-sm-12"
        }
        else if (Type === 'checkbox') {
            switch (columnCount) {
                case 1:
                    cssClass = "col-xs-12 col-md-12 col-lg-12 col-sm-12"
                    break;
                case 2:
                    cssClass = "col-xs-6 col-md-6 col-lg-6 col-sm-6"
                    break;
                case 3:
                    cssClass = "col-xs-4 col-md-4 col-lg-4 col-sm-6"
                    break;
                default:
                    cssClass = "col-xs-12 col-md-12 col-lg-12 col-sm-12"
            }
        } else {
            switch (columnCount) {
                case 1:
                    cssClass = isLabel ? "col-xs-3 col-md-3 col-lg-3 col-sm-3" : "col-xs-9 col-md-9 col-lg-9 col-sm-9"
                    break;
                case 2:
                    cssClass = "col-xs-3 col-md-3 col-lg-3 col-sm-3"
                    break;
                case 3:
                    cssClass = "col-xs-2 col-md-2 col-lg-2 col-sm-2"
                    break;
                default:
                    cssClass = "col-xs-12 col-md-12 col-lg-12 col-sm-12"
            }
        }
        return cssClass;
    },

    sortDropDownItems: function (items, isSortRequired) {
        if (isSortRequired == true) {
            return sortDropDownElementItems(items);
        }
        else {
            if (items != null && items.length > 0) {
                return items;
            }
        }
    }
});

$.views.converters({
    not: function (val) {
        return !val;
    },
    getCheck: function (val) {
        if (val == 'True' || val == 'true' || val === true) {
            return true;
        }
        else {
            return false;
        }
    },
    setCheck: function (val) {
        return val;
    },
    getRadioValue: function (val) {
        if (val == 'True' || val == 'true' || val === true) {
            return true;
        }
        else if (val == 'False' || val == 'false' || val === false) {
            return false;
        }
        else {
            return '';
        }
    },
    setRadioValue: function (val) {
        return val;
    },
});