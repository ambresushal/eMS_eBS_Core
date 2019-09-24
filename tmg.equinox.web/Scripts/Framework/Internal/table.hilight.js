(function ($) {
    $.fn.hilight = function (options) {

        var settings = $.extend({
            className: "diff-hilight",
        }, options);

        return this.each(function () {
            var $this = $(this);

            var source_trs = $('#' + settings.source + ' tr');
            var target_trs = $('#' + this.id + ' tr');

            for (var index = 0; index < source_trs.length; index++) {
                var $srcTd = $(source_trs[index].cells[0]);
                var $tgtTd = $(target_trs[index].cells[0]);

                if ($srcTd && $tgtTd) {

                    var $srcElements = $srcTd.find(':input');
                    var $tgtElements = $tgtTd.find(':input');
                    var ischanged = false;
                    if ($srcElements.length == $tgtElements.length) {
                        for (var icnt = 0; icnt < $srcElements.length; icnt++) {
                            var $srcElement = $($srcElements[icnt]);
                            var $tgtElement = $($tgtElements[icnt]);
                            if ($srcElement.length > 0 && $tgtElement.length > 0) {
                                var src_value, tgt_value;
                                if ($srcElement.is('radio') || $srcElement.is('checkbox')) {
                                    src_value = $('input[name="' + $srcElement[0].name + '"]:checked').val();
                                    tgt_value = $('input[name="' + $tgtElement[0].name + '"]:checked').val();
                                }
                                else {
                                    src_value = $srcElement.val();
                                    tgt_value = $tgtElement.val();
                                }

                                if (typeof (src_value) == "object" && typeof (tgt_value) == "object") {
                                    if (src_value != null) {
                                        src_value = src_value.toString();
                                    }
                                    if (tgt_value != null) {
                                        tgt_value = tgt_value.toString();
                                    }
                                }

                                if (src_value !== tgt_value) {
                                    ischanged = true;
                                    //$tgtTd.addClass(settings.className);
                                }
                                //else {
                                //    $tgtTd.removeClass(settings.className);
                                //}
                            }
                        }
                    }

                    if (ischanged == true) {
                        $tgtTd.addClass(settings.className);
                        ischanged = false;
                    }
                    else {
                        $tgtTd.removeClass(settings.className);
                    }
                }
            }

        });
    };
})(jQuery);