
var errorHandlerUrl = '/Error/LogJavaScriptError?errorMessage={errorMessage}';

window.onerror = function (msg, url, line, column, errorObj) {
 
    var errormessage = url + "$" + msg + " on line " + line;   
    errorHandlerUrl = errorHandlerUrl.replace(/\{errorMessage\}/g, errormessage);
    ajaxWrapper.postJSON(errorHandlerUrl);
    errorHandlerUrl = '/Error/LogJavaScriptError?errorMessage={errorMessage}';
}