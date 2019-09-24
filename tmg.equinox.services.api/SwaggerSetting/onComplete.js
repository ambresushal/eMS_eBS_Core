{
    function getAuth() {

        var key = $('#input_apiKey')[0].value;
        if (key && key.trim() != "") {
            key = "Bearer " + key;
            $("[id^=mAuthorization0]").val(key);
        }
    };

    $('#input_apiKey').change(function () { var key = $('#input_apiKey')[0].value;
        if (key && key.trim() != "") {
            key = "Bearer " + key;
            $("[id^=mAuthorization0]").val(key);
        }});

    $('#explore').click(getAuth());

   
}