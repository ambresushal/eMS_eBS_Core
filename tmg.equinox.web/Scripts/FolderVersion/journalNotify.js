﻿self.onmessage = function (oEvent) {

    var xhr = new XMLHttpRequest();

    if (oEvent.data.url != undefined) {
        xhr.open('POST', oEvent.data.url, true);
        xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        xhr.setRequestHeader('Content-Length', oEvent.data.saveData.length);
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                postMessage(xhr.response);
            }
        }
        xhr.send(oEvent.data.saveData);
    }
};
