var floatingsectionheaders = function () {
    return {
        float: function (sectionId) {
            //Checked if user has scrolled into an area which should have a persistent section header,if yes then  
            //cloned header is made visible and its position is fixed at the top. 
            function UpdateSectionHeaders() {
                var allHeaders = [], count = 0;
                //Finding number of visible section headers at present.
                $(".panel-heading-gray").each(function () {
                    floatingHeader = $(".panel-header-floating", this)                   
                    if ($(floatingHeader).css('visibility') === 'visible') {
                        count++;
                    }
                });
                $(".panel-heading-gray").each(function () {
                    var div = $(this);
                    offset = div.offset(),

                    scrolledDistance = $(window).scrollTop()
                    //Actual scrolled distance plus distance required for all visible section headers.
                    scrollTop = scrolledDistance + (23 * count);
                    floatingHeader = $(".panel-header-floating", this)                    
                    
                    if ((scrollTop > offset.top) && (scrollTop < offset.top + div.height())) {
                        floatingHeader.css({
                            "visibility": "visible",
                            "width": "97%",
                            "left": "1.5%"
                        });
                        allHeaders.push(this);
                    }
                    else {
                        floatingHeader.css({
                            "visibility": "hidden"
                        });
                    };
                });
                //Current active section is shown with dark color shade, 
                //while the other sections with lighter color shade.
                if (allHeaders.length != 0) {
                    for (var k = 0; k < (allHeaders.length) ; k++) {
                        floatingHeader = $(".panel-header-floating", allHeaders[k]);
                        if (k < (allHeaders.length) - 1) {
                            floatingHeader.css('background-color', '#DDDDDD');
                        }
                        else
                            floatingHeader.css('background-color', '#AAAAAA');
                    }
                }
            }
            var clonedHeaderRow;
            var mainSectionWidth;
            //Find main section and clone the section header.
            $("#" + sectionId).find('.mainSection').each(function () {
                $("#" + sectionId).find(".panel-header-floating").remove();
                clonedHeaderRow = $(".panel-title", this);
                mainSectionWidth = clonedHeaderRow.width();
                clonedHeaderRow
                  .before(clonedHeaderRow.clone())
                  .css("height", "23px")
                  .css("width", clonedHeaderRow.width())
                  .addClass("panel-header-floating")
                  .addClass(".panel-heading-gray");

            });
            mainSectionWidth = mainSectionWidth - 10;
            //Loop through each sub section and clone the section header
            $("#" + sectionId).find('.subsections').each(function () {
                var top = 0, j = 0;
                clonedHeaderRow = $(".panel-title", this);
                var newSubSectionWidth = clonedHeaderRow.width();

                for (i = mainSectionWidth ; i > 0 ; i = (i - 10)) {
                    j = j + 24;
                    if (newSubSectionWidth == i) {
                        top = j;
                    }
                }
                clonedHeaderRow
                  .before(clonedHeaderRow.clone())
                  .css("width", clonedHeaderRow.width())
                  .css("height", "23px")
                  .css('top', top + "px")
                  .addClass("panel-header-floating");
            });
            $(window)
               .scroll(UpdateSectionHeaders)
               .trigger("scroll");
        }

    }
}();