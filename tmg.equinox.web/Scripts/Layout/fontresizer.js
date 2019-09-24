function str_replace(haystack, needle, replacement) {
    if (haystack != undefined) {
        var temp = (haystack).split(needle);
        return temp.join(replacement);
    }
}

//min font size
var min = 9;
//max font size
var max = 25;
//font resize these elements
var elements = ['p', 'h1', 'h2', 'h3', 'h4', 'h5', 'h6', 'td:visible', 'th:visible', 'span', 'input', 'label'];

//Increase font size
$('#font-size-incrase-btn').click(function () {
    //set the active button
    $("#font-resize").find(".btn").each(function (idx, control) {
        $(control).removeClass("active");
    });
    $(this).addClass("active");

    //if the font size is lower or equal than the max value
    $(elements).each(function (key, val) {
        var size = str_replace($(val).css('font-size'), 'px', '');
        if (size <= max) {
            //increase the size
            size++;
            //set the font size
            var sizeString = size + 'px!important'
            $(val).attr("style", "font-size:" + sizeString);
            //$(val).css({ 'font-size': (sized) });
        }
    });
    //cancel a click event
    return false;
});

$('#font-size-decrease-btn').click(function () {
    //set the active button
    $("#font-resize").find(".btn").each(function (idx, control) {
        $(control).removeClass("active");
    });
    $(this).addClass("active");
    $(elements).each(function (key, val) {
        var size = str_replace($(val).css('font-size'), 'px', '');
        if (size >= min) {
            //decrease the size
            size--;
            //set the font size
            var sizeString = size + 'px!important'
            $(val).attr("style", "font-size:" + sizeString);
            //$(val).css({ 'font-size': size });
        }
    });
    //cancel a click event
    return false;
});
$('#font-size-reset-btn').click(function () {
    //set the active button
    $("#font-resize").find(".btn").each(function (idx, control) {
        $(control).removeClass("active");
    });
    $(this).addClass("active");
    $(elements).each(function (key, val) {
        $(val).css({ 'font-size': '11px;' });
    });
    //cancel a click event
    return false;
});