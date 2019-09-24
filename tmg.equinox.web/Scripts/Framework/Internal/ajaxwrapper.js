var ajaxWrapper = function () {
    return {
        getJSON: function (url) {
            var promise = $.ajax({
                dataType: "json",
                cache: false,
                url: url
            });
            return promise;
        },
        getJSON: function (url, global) {
            var settings = { dataType: "json", cache: false, url: url };
            if (global != null && global != undefined) {
                settings.global = global;
            }
            var promise = $.ajax(settings);
            return promise;
        },
        getJSONAsync: function (url) {
            var promise = $.ajax({
                dataType: "json",
                cache: false,
                url: url,
                async: true
            });
            return promise;
        },
        getJSONSync: function (url) {
            var promise = $.ajax({
                dataType: "json",
                cache: false,
                url: url,
                async: false
            });
            return promise;
        },
        getJSONCache: function (url) {
            var promise = $.ajax({
                dataType: "json",
                cache: true,
                url: url
            });
            return promise;
        },
        postJSON: function (url, data, global) {
            var settings = { type: "POST", url: url, data: data, traditional: true };
            if (global != null && global != undefined) {
                settings.global = global;
            }
            var promise = $.ajax(settings);
            return promise;
        },
        postJSONCustom: function (url, data) {
            var promise = $.ajax({
                type: 'POST',
                url: url,
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify(data)
            });
            return promise;
        },
        postAsyncJSONCustom: function (url, data) {
            var promise = $.ajax({
                type: 'POST',
                url: url,
                contentType: 'application/json',
                dataType: 'json',
                async: false,
                data: JSON.stringify(data)
            });
            return promise;
        },
        uploadFile: function (url, data) {
            var promise = $.ajax({
                type: "POST",
                url: url,
                contentType: false,
                processData: false,
                data: data
            });
            return promise;
        },
        getPartialView: function (url) {
            var promise = $.ajax({
                dataType: "html",
                cache: false,
                url: url
            });
            return promise;
        }
    }
}();


