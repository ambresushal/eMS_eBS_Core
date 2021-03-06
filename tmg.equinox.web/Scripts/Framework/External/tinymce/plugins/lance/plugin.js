/*!
 * Source version: 1.1.08 
 * Copyright (C) 2018 LoopIndex - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the LoopIndex Comments CKEditor plugin license.
 *
 * Open source libraries
 * jQuery scrollintoview plugin Copyright (c) 2011 Robert Koritnik https://github.com/litera/jquery-scrollintoview
 * MutationSummary copyright (C) Rafael Weinstein https://github.com/rafaelw/mutation-summary/
 * jQuery autogrow based on code by Jevin O. Sewaruth (https://github.com/jevin/Autogrow-Textarea)

 * You should have received a copy of the LoopIndex Comments CKEditor plugin license with
 * this file. If not, please write to: loopindex@gmail.com, or visit http://www.loopindex.com
 * written by (David *)Frenkiel (https://github.com/imdfl) 
 */
! function (t) {
    var e = {};

    function n(i) {
        if (e[i]) return e[i].exports;
        var o = e[i] = {
            i: i,
            l: !1,
            exports: {}
        };
        return t[i].call(o.exports, o, o.exports, n), o.l = !0, o.exports
    }
    n.m = t, n.c = e, n.d = function (t, e, i) {
        n.o(t, e) || Object.defineProperty(t, e, {
            enumerable: !0,
            get: i
        })
    }, n.r = function (t) {
        "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(t, Symbol.toStringTag, {
            value: "Module"
        }), Object.defineProperty(t, "__esModule", {
            value: !0
        })
    }, n.t = function (t, e) {
        if (1 & e && (t = n(t)), 8 & e) return t;
        if (4 & e && "object" == typeof t && t && t.__esModule) return t;
        var i = Object.create(null);
        if (n.r(i), Object.defineProperty(i, "default", {
            enumerable: !0,
            value: t
        }), 2 & e && "string" != typeof t)
            for (var o in t) n.d(i, o, function (e) {
                return t[e]
            }.bind(null, o));
        return i
    }, n.n = function (t) {
        var e = t && t.__esModule ? function () {
            return t.default
        } : function () {
            return t
        };
        return n.d(e, "a", e), e
    }, n.o = function (t, e) {
        return Object.prototype.hasOwnProperty.call(t, e)
    }, n.p = "", n(n.s = 0)
}([function (t, e) {
    ! function () {
        "use strict";
        ! function () {
            var t = window.$LOOPINDEX$;
            if (!t || "object" != typeof t) {
                t = window.$LOOPINDEX$ = {};
                var e = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                ! function () {
                    var e = [],
                        n = null;
                    if ("object" == typeof CKEDITOR && CKEDITOR.document) {
                        var i = CKEDITOR.document.getWindow;
                        CKEDITOR.document.getWindow = function () {
                            var o = Array.prototype.slice.call(arguments),
                                r = i.apply(CKEDITOR.document, o);
                            return e.push(r), n && t.overrideViewPaneSize(n), r
                        }
                    }
                    t.overrideViewPaneSize = function (t) {
                        n = t, e.forEach(function (e) {
                            if (!e.__OVERRIDE_GVPS__) {
                                var n = e.getViewPaneSize;
                                e.__OVERRIDE_GVPS__ = !0, e.getViewPaneSize = function () {
                                    var i = Array.prototype.slice.call(arguments),
                                        o = n.apply(e, i);
                                    return t(o)
                                }
                            }
                        })
                    }
                }();
                var n = function () {
                    var t = navigator.userAgent.toLowerCase(),
                        e = function (t) {
                            t = t.toLowerCase();
                            var e = /(chrome)[ \/]([\w.]+)/.exec(t) || /(webkit)[ \/]([\w.]+)/.exec(t) || /(opera)(?:.*version|)[ \/]([\w.]+)/.exec(t) || /(msie) ([\w.]+)/.exec(t) || t.indexOf("compatible") < 0 && /(mozilla)(?:.*? rv:([\w.]+)|)/.exec(t) || [];
                            return {
                                browser: e[1] || "",
                                version: e[2] || "0"
                            }
                        }(t),
                        n = {
                            type: "unknown",
                            version: 0,
                            msie: !1
                        };
                    return e.browser && (n[e.browser] = !0, n.version = e.version || 0, n.type = e.browser), n.chrome ? n.webkit = !0 : n.webkit && (n.safari = !0), n.webkit && (n.type = "webkit"), n.firefox = !0 === /firefox/.test(t), n.msie || (n.msie = Boolean(/trident/.test(t))), n
                }();
                t.EventsMixin = {
                    _listeners: function (t) {
                        this._listeners_ = this._listeners_ || {};
                        var e = this._listeners_;
                        return e[t = t || ""] || (e[t] = []), e[t]
                    },
                    _cleanup: function () {
                        var t = this._listeners_;
                        t && Object.keys(t).forEach(function (e) {
                            var n = t[e];
                            n && n.length || delete t[e]
                        })
                    },
                    addListener: function (e, n, i, o) {
                        if (!e) return t.Logger.warn("cannot register to empty event"), this;
                        if (e instanceof Array) {
                            var r = this;
                            e.forEach(function (t) {
                                r.addListener(t, n, i)
                            })
                        } else if ("function" == typeof n) {
                            this._listeners(e).push({
                                scope: i,
                                callback: n,
                                options: o
                            })
                        } else t.Logger.error("events mixin addListener: bad callback", n);
                        return this
                    },
                    removeListener: function (t, e) {
                        if (!t && !e) return this;
                        if (e || "object" != typeof t || (e = t, t = null), t instanceof Array) {
                            var n = this;
                            return t.forEach(function (t) {
                                n.removeListener(t, e)
                            }), this
                        }

                        function i(t, n) {
                            return t && (!e || t.scope === e || t.callback === e)
                        }
                        var o = this._listeners_ || {};
                        return Object.keys(o).forEach(function (e) {
                            t && t !== e || o[e].removeAll_li(i)
                        }), this._cleanup(), this
                    },
                    removeAllListeners: function () {
                        this._listeners_ = {}
                    },
                    _notifyListeners: function (e) {
                        if (void 0 === e || null === e || "" === e) return t.Logger.warn("cannot trigger empty event");
                        var n = this._listeners(e),
                            i = n.slice(),
                            o = [],
                            r = Array.prototype.slice.call(arguments, 1);
                        i.forEach(function (t) {
                            t.callback.apply(t.scope, r), t.options && t.options.count && (--t.options.count, t.options.count <= 0 && o.push(t))
                        }), o.length && (o.forEach(function (t) {
                            n.remove_li(t)
                        }), this._cleanup())
                    },
                    once: function (t, e, n, i) {
                        return (i = i || {}).count = 1, this.on(t, e, n, i)
                    }
                }, t.EventsMixin.trigger = t.EventsMixin.fire = t.EventsMixin._notifyListeners, t.EventsMixin.on = t.EventsMixin.bind = t.EventsMixin.addListener, t.EventsMixin.off = t.EventsMixin.removeListener, t.Utils = t.Utils || t.utils || {}, t.utils = t.Utils, t.utils.padString = function (t, e, n, i) {
                    t = null === t || void 0 === t ? "" : String(t);
                    for (var o = (n = String(n)).length, r = t.length; r < e; r += o) i ? t += padWidth : t = n + t;
                    return t
                }, t.utils.padNumber = function (e, n) {
                    return n = n || 2, t.utils.padString(e, n, "0")
                }, t.utils.compareNodePosition = function (t, e) {
                    if (t == e) return 0;
                    var n = (t.$ || t).compareDocumentPosition(e.$ || e);
                    return 4 & n ? -1 : 8 & n ? -1 : 1
                }, t.utils.relativeDateFormat = function (n, i) {
                    if (!n) return "";
                    var r, s, a, u, l = new Date,
                        c = l.getDate(),
                        d = l.getMonth(),
                        h = l.getFullYear(),
                        f = i || function (t) {
                            return t
                        };
                    "string" != (r = typeof n) && "number" != r || (n = new Date(1e3 * Number(n)));
                    var g = f("months") || e,
                        m = {
                            dateString: n.toLocaleDateString()
                        };
                    return "string" == typeof g && (g = g.split(",").map(function (t) {
                        return t.trim()
                    })), m.months = g, c == n.getDate() && d == n.getMonth() && h == n.getFullYear() ? (a = Math.floor((l.getTime() - n.getTime()) / 6e4), m.minutesAgo = a, a < 1 ? f("now") : a < 2 ? f("1 minute ago") : a < 60 ? (s = f("minutes ago")) ? o(s, n, f, m) : a + "minutes ago" : (s = f("on time")) ? o(s, n, f, m) : (u = n.getHours(), a = n.getMinutes(), "on " + t.utils.padNumber(u, 2) + ":" + t.utils.padNumber(a, 2, "0"))) : h == n.getFullYear() ? (s = f("on date")) ? o(s, n, f, m) : "on " + g[n.getMonth()] + " " + n.getDate() : (s = f("on full date")) ? o(s, n, f, m) : n.toLocaleDateString()
                }, t.utils.getElementClampInWindowValues = function (t, e) {
                    var n = t.offset(),
                        i = {
                            x: 0,
                            y: 0
                        },
                        o = t.width(),
                        r = t.height(),
                        s = t[0].ownerDocument,
                        a = s && (s.defaultView || s.parentWindow);
                    return a ? (e = e || 0, n.top - e <= a.pageYOffset ? i.y = a.pageYOffset - n.top + e : n.top + r + e > a.pageYOffset + a.innerHeight && (i.y = a.pageYOffset + a.innerHeight - n.top - r - e), n.left - e < a.pageXOffset ? i.x = a.pageXOffset - n.left + e : n.left + o + e > a.pageXOffset + a.innerWidth && (i.x = a.pageXOffset + a.innerWidth - n.left - o - e), i) : i
                }, t.utils.isElementOnScreen = function (e) {
                    var n = t.utils.getElementClampInWindowValues(e);
                    return 0 === n.x && 0 === n.y
                }, t.utils.scrollToElement = function (e, n) {
                    if (e) return t.utils.initJquery(), e.scrollintoview()
                }, t.utils.randomString = function (t) {
                    t = t || 6;
                    var e = (new Date).getTime().toString(),
                        n = btoa(e).replace(/[^a-z]/gi, "");
                    return n.length > t && (n = n.substring(n.length - t)), n
                }, t.utils.timeOfTimestamp = function (e) {
                    if (!e) return "";
                    var n = typeof e;
                    "string" != n && "number" != n || (e = new Date(1e3 * e));
                    var i = e.getHours(),
                        o = e.getMinutes();
                    return t.utils.padString(i, 2, "0") + ":" + t.utils.padString(o, 2, "0")
                }, t.utils.datetimeOfTimestamp = function (e) {
                    if (!e) return "";
                    var n = typeof date;
                    return "string" != n && "number" != n || (date = new Date(1e3 * date)), t.utils.dateOfTimestamp(e) + ", " + t.utils.timeOfTimestamp(e)
                }, t.utils.minuteAgoOfTimestamp = function (t) {
                    if (!t) return "";
                    var e = new Date;
                    t instanceof Date && (t = t.getTime());
                    var n = Math.round(e.getTime() / 1e3) - t;
                    return parseInt(n / 60)
                }, t.utils.secondAgoOfTimestamp = function (t) {
                    if (!t) return "";
                    var e = new Date;
                    t instanceof Date && (t = t.getTime());
                    var n = Math.round(e.getTime() / 1e3) - t;
                    return n < 0 && (n = 0), n
                }, t.utils.fullDateFormat = function (t) {
                    if (!t) return "";
                    var e = typeof t;
                    "string" != e && "number" != e || (t = new Date(1e3 * t));
                    return t.getDate() + " " + ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"][t.getMonth()] + " " + t.getFullYear()
                }, t.utils.fullDateTimeFormat = function (e) {
                    return t.utils.fullDateFormat(e) + " " + t.utils.timeOfTimestamp(e)
                }, t.utils.fullLocalDateFormat = function (t) {
                    return t ? (date = "string" == typeof t || "number" == typeof t ? new Date(1e3 * t) : t, date.toLocaleDateString()) : ""
                }, t.utils.fullLocalDateTimeFormat = function (t) {
                    if (!t) return "";
                    var e = typeof t;
                    return "string" != e && "number" != e || (t = new Date(1e3 * t)), t.toLocaleString()
                }, t.utils.shorthandDateTime = function (e) {
                    if (!e) return "";
                    var n = new Date,
                        i = n.getDate(),
                        o = n.getMonth(),
                        r = n.getFullYear(),
                        s = typeof e;
                    "string" != s && "number" != s || (e = new Date(1e3 * e));
                    var a = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
                    return i == e.getDate() && o == e.getMonth() && r == e.getFullYear() ? t.utils.timeOfTimestamp(e) : r == e.getFullYear() ? a[e.getMonth()] + " " + e.getDate() : a[e.getMonth()] + " " + e.getDate() + ", " + e.getFullYear()
                }, t.utils.URLArguments = {
                    getArgument: function (t, e) {
                        return this._parseLocation(), this._args.hasOwnProperty(t) ? decodeURIComponent(this._args[t]) : e
                    },
                    getNumericArgument: function (t, e) {
                        var n = this.getArgument(t, ""),
                            i = parseInt(n, 10);
                        return isNaN(i) && (i = e), i
                    },
                    getAllArguments: function () {
                        this._parseLocation();
                        var t, e = {};
                        for (t in this._args) this._args.hasOwnProperty(t) && (e[t] = this._args[t]);
                        return e
                    },
                    createSearchString: function (t) {
                        if (!t) return "";
                        var e, n, i = "";
                        for (e in t) t.hasOwnProperty(e) && "function" != typeof (n = t[e]) && (i.length && (i += "&"), i += e + "=" + decodeURIComponent(n));
                        return i
                    },
                    parse: function (t, e) {
                        if (e = e || {}, "string" != typeof t || t.length < 1) return {};
                        var n = t.charAt(0);
                        "?" !== n && "#" !== n || (t = t.substring(1));
                        var i, o, r, s, a = t.split("&"),
                            u = {
                                true: !0,
                                false: !1
                            };
                        for (i = a.length - 1; i >= 0; --i) (o = a[i]) && (r = o.indexOf("=")) > 0 && (s = decodeURIComponent(o.substring(r + 1)), u.hasOwnProperty(s) && (s = u[s]), e[o.substring(0, r)] = s);
                        return e
                    },
                    _args: null,
                    _url: "",
                    _parseLocation: function () {
                        var t = window.location;
                        if (this._url !== t.href || !this._args) {
                            this._url = t.href;
                            var e = {};
                            this.parse(t.search, e), this.parse(t.hash, e), this._args = e
                        }
                        return this._args
                    }
                }, Object.defineProperties(t.Utils, {
                    browser: {
                        get: function () {
                            return jQuery.extend({}, n)
                        }
                    }
                });
                var i = Array.prototype;
                i.contains_li || Object.defineProperty(i, "contains_li", {
                    enumerable: !1,
                    value: function (t) {
                        if (null == this) throw new TypeError;
                        return this.indexOf(t) >= 0
                    }
                }), i.select_li || Object.defineProperty(i, "select_li", {
                    enumerable: !1,
                    value: function (t) {
                        if (null == this) throw new TypeError;
                        var e = Object(this).length >>> 0;
                        if (0 === e) return [];
                        var n = new Array(e);
                        return this.each_li(function (e, i) {
                            n[e] = t(i, e)
                        }), n
                    }
                }), i.removeAll_li || Object.defineProperty(i, "removeAll_li", {
                    enumerable: !1,
                    value: function (t) {
                        if (null == this) throw new TypeError;
                        var e = Object(this).length >>> 0;
                        if (0 === e) return -1;
                        for (var n = e - 1; n >= 0; --n) t(this[n], n) && this.splice(n, 1)
                    }
                }), i.first_li || Object.defineProperty(i, "first_li", {
                    enumerable: !1,
                    value: function (t) {
                        if (null == this) throw new TypeError;
                        var e = this.firstIndexOf_li(t);
                        return e >= 0 ? this[e] : null
                    }
                }), i.firstIndexOf_li || Object.defineProperty(i, "firstIndexOf_li", {
                    enumerable: !1,
                    value: function (t) {
                        if (null == this) throw new TypeError;
                        var e = Object(this).length >>> 0;
                        if (0 === e) return -1;
                        for (var n = 0; n < e; ++n) {
                            if (t(this[n])) return n
                        }
                        return -1
                    }
                }), i.where_li || Object.defineProperty(i, "where_li", {
                    enumerable: !1,
                    value: function (t) {
                        if (null == this) throw new TypeError;
                        if (0 === Object(this).length >>> 0) return [];
                        var e = [];
                        return this.each_li(function (n, i) {
                            t(i, n) && e.push(i)
                        }), e
                    }
                }), i.each_li || Object.defineProperty(i, "each_li", {
                    enumerable: !1,
                    value: function (e) {
                        if (null == this) throw new TypeError;
                        var n = Object(this).length >>> 0;
                        if (0 === n) return [];
                        for (var i = 0; i < n; ++i) try {
                            e.apply(null, [i, this[i]])
                        } catch (e) {
                            t.Logger.error(e)
                        }
                    }
                }), i.remove_li || Object.defineProperty(i, "remove_li", {
                    enumerable: !1,
                    value: function (t) {
                        if (null == this) throw new TypeError;
                        if (0 === Object(this).length >>> 0) return -1;
                        var e = this.indexOf(t);
                        return e >= 0 ? (this.splice(e, 1), e) : -1
                    }
                }), t.Logger = function () {
                    var t = {
                        log: !1,
                        debug: !1,
                        warn: !1,
                        error: !1,
                        trace: !1
                    };
                    return {
                        _logs: [],
                        config: function (e) {
                            $.extend(t, e)
                        },
                        ignore: function () { },
                        log: function () {
                            t.log && this._log("log", Array.prototype.slice.call(arguments))
                        },
                        error: function () {
                            t.error && this._log("error", Array.prototype.slice.call(arguments))
                        },
                        debug: function () {
                            t.debug && this._log("log", Array.prototype.slice.call(arguments))
                        },
                        trace: function () {
                            if (t.trace) {
                                var e = Error().stack,
                                    n = e ? [e.replace("Error", "Debug:"), "\n"] : [];
                                this._log("log", n.concat(Array.prototype.slice.call(arguments)))
                            }
                        },
                        warn: function () {
                            this._log("warn", Array.prototype.slice.call(arguments))
                        },
                        _log: function (t, e) {
                            var n = new Date,
                                i = [n.getMinutes(), n.getSeconds(), n.getMilliseconds()].join(":");
                            e.unshift(i + ">"), this._logs.push({
                                method: t,
                                logs: e
                            }), this.scheduleDump()
                        },
                        dump: function () {
                            var t = this._logs;
                            this._logs = [], t.forEach(function (t) {
                                console[t.method].apply(console, t.logs)
                            })
                        },
                        timeout: null,
                        scheduleDump: function () {
                            this.timeout || (this.timeout = setTimeout(function () {
                                this.timeout = null, this.dump()
                            }.bind(this), 500))
                        }
                    }
                }(), t.utils.initJquery = function () {
                    window.jQuery && !window.jQuery.fn.scrollIntoView && function (t) {
                        var e = {
                            vertical: {
                                x: !1,
                                y: !0
                            },
                            horizontal: {
                                x: !0,
                                y: !1
                            },
                            both: {
                                x: !0,
                                y: !0
                            },
                            x: {
                                x: !0,
                                y: !1
                            },
                            y: {
                                x: !1,
                                y: !0
                            }
                        },
                            n = {
                                duration: "fast",
                                direction: "both"
                            },
                            i = /^(?:html)$/i,
                            o = function (e) {
                                var n = t(window),
                                    o = i.test(e[0].nodeName);
                                return {
                                    border: o ? {
                                        top: 0,
                                        left: 0,
                                        bottom: 0,
                                        right: 0
                                    } : function (e, n) {
                                        n = n || (document.defaultView && document.defaultView.getComputedStyle ? document.defaultView.getComputedStyle(e, null) : e.currentStyle);
                                        var i = !(!document.defaultView || !document.defaultView.getComputedStyle),
                                            o = {
                                                top: parseFloat(i ? n.borderTopWidth : t.css(e, "borderTopWidth")) || 0,
                                                left: parseFloat(i ? n.borderLeftWidth : t.css(e, "borderLeftWidth")) || 0,
                                                bottom: parseFloat(i ? n.borderBottomWidth : t.css(e, "borderBottomWidth")) || 0,
                                                right: parseFloat(i ? n.borderRightWidth : t.css(e, "borderRightWidth")) || 0
                                            };
                                        return {
                                            top: o.top,
                                            left: o.left,
                                            bottom: o.bottom,
                                            right: o.right,
                                            vertical: o.top + o.bottom,
                                            horizontal: o.left + o.right
                                        }
                                    }(e[0]),
                                    scroll: {
                                        top: (o ? n : e).scrollTop(),
                                        left: (o ? n : e).scrollLeft()
                                    },
                                    scrollbar: {
                                        right: o ? 0 : e.innerWidth() - e[0].clientWidth,
                                        bottom: o ? 0 : e.innerHeight() - e[0].clientHeight
                                    },
                                    rect: function () {
                                        var t = e[0].getBoundingClientRect();
                                        return {
                                            top: o ? 0 : t.top,
                                            left: o ? 0 : t.left,
                                            bottom: o ? e[0].clientHeight : t.bottom,
                                            right: o ? e[0].clientWidth : t.right
                                        }
                                    }()
                                }
                            };
                        t.fn.extend({
                            scrollintoview: function (r) {
                                (r = t.extend({}, n, r)).direction = e["string" == typeof r.direction && r.direction.toLowerCase()] || e.both;
                                var s = "";
                                !0 === r.direction.x && (s = "horizontal"), !0 === r.direction.y && (s = s ? "both" : "vertical");
                                var a = this.eq(0),
                                    u = a.closest(":scrollable(" + s + ")");
                                if (u.length > 0) {
                                    u = u.eq(0);
                                    var l = {
                                        e: o(a),
                                        s: o(u)
                                    },
                                        c = {
                                            top: l.e.rect.top - (l.s.rect.top + l.s.border.top),
                                            bottom: l.s.rect.bottom - l.s.border.bottom - l.s.scrollbar.bottom - l.e.rect.bottom,
                                            left: l.e.rect.left - (l.s.rect.left + l.s.border.left),
                                            right: l.s.rect.right - l.s.border.right - l.s.scrollbar.right - l.e.rect.right
                                        },
                                        d = {};
                                    !0 === r.direction.y && (c.top < 0 ? d.scrollTop = l.s.scroll.top + c.top : c.top > 0 && c.bottom < 0 && (d.scrollTop = l.s.scroll.top + Math.min(c.top, -c.bottom))), !0 === r.direction.x && (c.left < 0 ? d.scrollLeft = l.s.scroll.left + c.left : c.left > 0 && c.right < 0 && (d.scrollLeft = l.s.scroll.left + Math.min(c.left, -c.right))), t.isEmptyObject(d) ? t.isFunction(r.complete) && r.complete.call(u[0]) : (i.test(u[0].nodeName) && (u = t("html,body")), u.animate(d, r.duration).eq(0).queue(function (e) {
                                        t.isFunction(r.complete) && r.complete.call(u[0]), e()
                                    }))
                                }
                                return this
                            }
                        });
                        var r = {
                            auto: !0,
                            scroll: !0,
                            visible: !1,
                            hidden: !1
                        };
                        t.extend(t.expr[":"], {
                            scrollable: function (t, n, o, s) {
                                var a = e[o && "string" == typeof o[3] && o[3].toLowerCase()] || e.both,
                                    u = document.defaultView && document.defaultView.getComputedStyle ? document.defaultView.getComputedStyle(t, null) : t.currentStyle,
                                    l = {
                                        x: r[u.overflowX.toLowerCase()] || !1,
                                        y: r[u.overflowY.toLowerCase()] || !1,
                                        isRoot: i.test(t.nodeName)
                                    };
                                if (!l.x && !l.y && !l.isRoot) return !1;
                                var c = {
                                    height: {
                                        scroll: t.scrollHeight,
                                        client: t.clientHeight
                                    },
                                    width: {
                                        scroll: t.scrollWidth,
                                        client: t.clientWidth
                                    },
                                    scrollableX: function () {
                                        return (l.x || l.isRoot) && this.width.scroll > this.width.client
                                    },
                                    scrollableY: function () {
                                        return (l.y || l.isRoot) && this.height.scroll > this.height.client
                                    }
                                };
                                return a.y && c.scrollableY() || a.x && c.scrollableX()
                            }
                        })
                    }(window.jQuery)
                }
            }

            function o(e, n, i, o) {
                return e.replace(/%MMM/g, (o.months || [])[n.getMonth()]).replace(/%M/g, (i("full_months") || [])[n.getMonth()]).replace(/%YY/g, n.getYear() % 100).replace(/%Y/g, n.getFullYear()).replace(/%YY/g, n.getYear() % 100).replace(/%hh/g, t.utils.padNumber(n.getHours())).replace(/%h/g, n.getHours()).replace(/%ma/g, o.minutesAgo).replace(/%mm/g, t.utils.padNumber(n.getMinutes())).replace(/%m/g, n.getMinutes()).replace(/%dd/g, t.utils.padNumber(n.getDate())).replace(/%d/g, n.getDate()).replace(/%F/g, o.dateString)
            }
        }(),
        function (t, e) {
            t || (t = window.App = {});
            var n = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",
                i = [118, 104, 119, 87, 108, 112, 104, 114, 120, 119],
                o = [103, 104, 111, 104, 119, 104, 68, 113, 113, 114, 119, 100, 119, 108, 114, 113],
                r = window,
                s = {
                    edit: {
                        first: "owner",
                        last: "owner",
                        default: "owner"
                    },
                    delete: {
                        first: "owner",
                        last: "owner",
                        default: "owner"
                    },
                    resolve: {
                        first: "opener",
                        last: "opener",
                        default: "opener"
                    }
                },
                a = Math.round(1e3 * Math.random()),
                u = "opener",
                l = "owner",
                c = "none",
                d = "user",
                h = "any";

            function f(n) {
                if (!n) return null;
                var i = e.extend({}, {
                    edit: {},
                    delete: {},
                    resolve: {}
                }),
                    o = function (t, e) {
                        t && ("string" == typeof t ? e.first = e.last = e.default = t : "object" == typeof t && ["first", "last", "default"].forEach(function (n) {
                            e[n] = t[n] || t.default || void 0
                        }))
                    };
                return o(n.edit, i.edit), o(n.delete, i.delete), o(n.resolve, i.resolve),
                    function e(n) {
                        if (!n) return null;
                        var i, o;
                        for (var r in n)
                            if (n.hasOwnProperty(r) && (o = typeof (i = n[r]), void 0 !== i)) {
                                if (null === i) return t.Logger.error("invalid null permissions key", r, "\nUsing default permissions"), null;
                                if ("string" === o) {
                                    if (n[r] = i = i.toLowerCase(), i !== u && i !== h && i !== l && i !== d && i !== c) return t.Logger.error("invalid permission; key:", r, "value:", i, "\nUsing default permissions"), null
                                } else {
                                    if ("object" !== o) return t.Logger.error("invalid permission; key:", r, "value:", i), null;
                                    if (!e(i)) return null
                                }
                            }
                        return n
                    }(i)
            }

            function g(t) {
                var e = Math.round(Math.random() * [300, 500, 512].length);
                return t._temp = e, t.map(function (t) {
                    return t - 3
                }).map(function (t) {
                    return String.fromCharCode(t)
                }).join("")
            }

            function m(t) {
                t = t || 6;
                var e = function (t) {
                    for (var e, i, o = String(t), r = 0, s = n, a = ""; o.charAt(0 | r) || (s = "=", r % 1) ; a += s.charAt(63 & e >> 8 - r % 1 * 8)) (i = o.charCodeAt(r += .75)) > 255 && encodeURIComponent(String(t)), e = e << 8 | i;
                    return a
                }((new Date).getTime().toString()).replace(/[^a-z]/gi, "");
                return e.length > t && (e = e.substring(e.length - t)), e
            }

            function _(t) {
                var e, n = {};
                return Object.keys(t).forEach(function (i) {
                    "_" != i.charAt(0) && void 0 !== (e = t[i]) && "function" != typeof e && (n[i] = t[i])
                }), n
            }

            function v(t) {
                return t = t || {}, {
                    id: String(t.id),
                    name: String(t.name),
                    picture: String(t.picture || "")
                }
            }

            function p(t, e, n, i) {
                return null === e || void 0 === e || (null === n || void 0 === n ? i.all : n === t && e === t ? i.opener || i.owner : e === n ? i.owner : i.user)
            }
            t.Annotations = function (t) {
                if (t = t || {}, this.setPermissions(t.permissions, {
                    notify: !1
                }), this._defaultPicture = t.defaultPicture, this._userId = t.userId || 0, this._disableCount = 0, t.idGenerator) {
                    if ("function" != typeof t.idGenerator) throw new Error("options.idGenerator: bad type", typeof t.idGenerator);
                    this._idGenerator = t.idGenerator
                } else this._idGenerator = null;
                return this._statusCallback = t.statusCallback, this._isEnabled = !0, this._requestUserCallback = t.requestUser, this._validate = r[g(i)].bind(r), Object.defineProperties(this, {
                    _annotations: {
                        value: []
                    },
                    _annotationsMap: {
                        value: {}
                    },
                    _idSeed: {
                        value: m(6)
                    },
                    _users: {
                        value: {}
                    },
                    _customAttributes: {
                        value: {}
                    },
                    getLocalizedString: {
                        value: t.getLocalizedString
                    },
                    dictionary: {
                        value: t.dictionary
                    }
                }), this.addCustomAttributes(t.customAttributes), this._baseId = 0, t.users && this.addUsers(t.users), this
            }, t.Annotations.Events = {
                ANNOTATION_CREATED: "annotation:created",
                ANNOTATION_DELETED: "annotation:deleted",
                ANNOTATION_RESOLVED: "annotation:resolved",
                ANNOTATION_SELECTED: "annotation:selected",
                ANNOTATION_UPDATED: "annotation:updated",
                COMMENT_ADDED: "annotation:commentcreated",
                COMMENT_DELETED: "annotation:commentdeleted",
                COMMENT_CHANGED: "annotation:commentchanged",
                COMMENT_SELECTED: "annotation:commentselected",
                RESET: "annotation:reset",
                RELOAD: "annotation:reload",
                ENABLED_CHANGED: "annotation:enable",
                SIZE_CHANGED: "annotation:resize",
                DONE_EDITING: "annotation:done-editing",
                ATTRIBUTE_CHANGED: "annotation:attribute-changed",
                ANNOTATIONS_RENUMBERED: "annotation:renumbered"
            }, t.Annotations.extractAnnotation = function (t) {
                if (!t) return null;
                if (t.$ && (t = t.$), !t.getAttribute) return null;
                try {
                    var e = t.getAttribute("data-ant");
                    if (e) return e = unescape(e), JSON.parse(e)
                } catch (t) {
                    return null
                }
            };
            var A = t.Annotations.prototype = e.extend({}, t.EventsMixin),
                b = {
                    id: "",
                    userId: 0,
                    userName: "",
                    time: 0,
                    text: ""
                },
                E = function (t) {
                    if (!t) throw new Error("Annotation constructed with empty data");
                    if (!t.id) throw new Error("cannot create an annotation without an id");
                    this._selected = !1, this._resolved = Boolean(t.resolved), Object.defineProperties(this, {
                        id: {
                            value: t.id
                        },
                        attributes: {
                            value: {}
                        },
                        _comments: {
                            value: []
                        },
                        _commentId: {
                            value: m(3)
                        }
                    }), this.sequence = 0;
                    var e = t.attributes;
                    if (e && e instanceof Object) {
                        var n = this.attributes;
                        Object.keys(e).forEach(function (t) {
                            n[t] = e[t]
                        })
                    }
                    return this
                },
                y = E.prototype = {},
                O = function (t) {
                    t = function (t, e) {
                        var n = {};
                        for (var i in t = t || {}, e) t.hasOwnProperty(i) ? n[i] = t[i] : n[i] = e[i];
                        return n
                    }(t, b), e.extend(this, t), this._selected = !1, this._userPicture = ""
                },
                w = O.prototype = {};
            w.isSelected = function () {
                return this._selected
            }, w.setSelected = function (t) {
                this._selected = Boolean(t)
            }, w.setTime = function (t) {
                if (t instanceof Date) this.time = Math.round(t.getTime() / 1e3);
                else {
                    var e = typeof t;
                    this.time = "string" === e ? parseInt(t) : t
                }
            }, w.getUserPicture = function () {
                return this._userPicture
            }, w.setUserPicture = function (t) {
                this._userPicture = t || ""
            }, y.text = function () {
                var t, e, n = [],
                    i = this._comments,
                    o = i.length;
                for (t = 0; t < o; ++t) e = i[t], n.push((e.userName || "user") + ": " + e.text);
                return n.join("\n")
            }, y.saveToObject = function () {
                var t = this.attributes,
                    e = {},
                    n = {
                        id: this.id,
                        resolved: this._resolved,
                        sequence: this.sequence,
                        attributes: e
                    };
                Object.keys(t).forEach(function (n) {
                    e[n] = t[n]
                });
                for (var i = n.comments = [], o = this._comments, r = 0, s = o.length; r < s; ++r) {
                    var a = o[r];
                    a.text && i.push(_(a))
                }
                return n
            }, y.isSelected = function () {
                return this._selected
            }, y.isResolved = function () {
                return this._resolved
            }, y.setResolved = function (t) {
                this._resolved = Boolean(t)
            }, y.lastComment = function () {
                return this.getCommentByIndex(this._comments.length - 1)
            }, y.firstComment = function () {
                return this.getCommentByIndex(0)
            }, y.isFirst = function (t) {
                return 0 === this.getCommentIndexById(t)
            }, y.isLast = function (t) {
                var e = this.getCommentIndexById(t);
                return e >= 0 && e === this._comments.length - 1
            }, y.count = function () {
                return this._comments.length
            }, y.getCommentIndexById = function (t) {
                return this._comments.firstIndexOf_li(function (e) {
                    return e.id == t
                })
            }, y.getCommentById = function (t) {
                var e = this.getCommentIndexById(t);
                return e >= 0 ? this._comments[e] : null
            }, y.getCommentByIndex = function (t) {
                return this._comments[t] || null
            }, y.setCommentText = function (t, e) {
                var n = this.getCommentById(t);
                n && (n.text = e, n.setTime(new Date))
            }, y.addComment = function (t) {
                (t = t || {}).id = t.id || this._getNextCommentId();
                var e = new O(t);
                return e.setTime(t.time || new Date), this._comments.push(e), e
            }, y.setSelected = function (t) {
                this._selected = Boolean(t), t || this._comments.forEach(function (t) {
                    t.setSelected(!1)
                })
            }, y.selectComment = function (t, e) {
                var n = this.getCommentById(t);
                n && (e ? (this._comments.forEach(function (t) {
                    t.setSelected(!1)
                }), n.setSelected(!0)) : n.setSelected(!1))
            }, y.deleteComment = function (t) {
                var e = this.getCommentIndexById(t),
                    n = e >= 0;
                return n && this._comments.splice(e, 1), n
            }, y.getOpenerId = function () {
                var t = this.getCommentByIndex(0);
                return t && t.userId
            }, y._getNextCommentId = function () {
                return this._commentId + a++
            }, A.setPermissions = function (n, i) {
                var o = e.extend(!0, {}, s, f(n)),
                    r = o.delete,
                    a = o.resolve,
                    c = i && i.notify,
                    g = o.edit,
                    m = function (t, e, n) {
                        return {
                            delete: {
                                all: e === h,
                                user: e === d || e === h,
                                owner: e === l || e === d || e === h,
                                opener: e === u || e === d || e === h
                            },
                            edit: {
                                all: t === h,
                                user: t === d || t === h,
                                owner: t === l || t === d || t === h,
                                opener: t === u || t === d || t === h
                            },
                            resolve: {
                                all: n === h,
                                user: n === d || n === h,
                                owner: n === l || n === d || n === h,
                                opener: n === u || n === d || n === h
                            }
                        }
                    };
                this._permissions = {
                    last: m(g.last, r.last, a.last),
                    first: m(g.first, r.first, a.first),
                    default: m(g.default, r.default, a.default)
                }, !1 !== c && this.trigger(t.Annotations.Events.RELOAD)
            }, A.setDefaultAvatar = function (t) {
                this._defaultPicture = t
            }, A.doneEditing = function () {
                this.trigger(t.Annotations.Events.DONE_EDITING)
            }, A.setUserId = function (n) {
                if (n !== this._userId) {
                    var i = this;
                    this._userId = String(n), e.each(this._annotations, function (e, n) {
                        i.trigger(t.Annotations.Events.ANNOTATION_UPDATED, {
                            annotation: n
                        })
                    })
                }
            }, A.getUserId = function () {
                return this._userId
            }, A.unselectAll = function (t) {
                t = t || [], this._annotations.forEach(function (e) {
                    t.indexOf(e) < 0 && this._selectOneAnnotation(e, !1)
                }.bind(this))
            }, A._findAnnotation = function (t) {
                return this._annotationsMap[t]
            }, A._selectOneAnnotation = function (e, n) {
                "string" == typeof e && (e = this._findAnnotation(e)), e && (n = Boolean(n)) !== e.isSelected() && (e.setSelected(n), this.trigger(t.Annotations.Events.ANNOTATION_SELECTED, {
                    isSelected: n,
                    annotation: e
                }))
            }, A.disable = function (t) {
                !0 === t && this._disableCount > 0 || 1 == ++this._disableCount && this._setEnabled(!1)
            }, A.enable = function (t) {
                t ? this._disableCount = 0 : this._disableCount > 0 && --this._disableCount, 0 === this._disableCount && this._setEnabled(!0)
            }, A._setEnabled = function (e) {
                this._isEnabled = e, e || this.unselectAll(), this.trigger(t.Annotations.Events.ENABLED_CHANGED, {
                    isEnabled: e
                })
            }, A.isEnabled = function () {
                return this._isEnabled
            }, A.countAnnotations = function () {
                return this._annotations.length
            }, A.getAnnotationByIndex = function (t) {
                return this._annotations[t] || null
            }, A.getAnnotationById = function (t) {
                return this._findAnnotation(t)
            }, A.compactAnnotation = function (t) {
                var e = this._findAnnotation(t);
                if (e) {
                    var n, i = e.count() - 1,
                        o = [];
                    if (i < 0) return this._adjustAnnotationLength(e);
                    for (; i >= 0;) (n = e.getCommentByIndex(i)).text || o.push(n.id), --i;
                    o.forEach(function (t) {
                        this._adjustCommentLength(e, t)
                    }.bind(this))
                }
            }, A.deleteAnnotation = function (t) {
                console.warn(" delete annotation is disabled in the demo version (but it really works in the full version).")
            }, A.insertAnnotation = function (t) {
                var e = {
                    id: this._createId()
                },
                    n = new E(e),
                    i = n.addComment({}),
                    o = this._getUser(this._userId);
                this._loadCommentUserDetails(i, o), i.setUserPicture(this._getUserPicture(this._userId)), this._addAnnotation(n, {
                    position: t.position,
                    noNotify: !1
                }), t.select && this.selectAnnotation(n.id, !0), t.edit && (i = n.firstComment(), this.selectComment(n.id, i.id, !0, !0))
            }, A.validateAnnotation = function (t) {
                return Boolean(t && this._annotations[t])
            }, A.addCustomAttributes = function (t) {
                if (t) {
                    t instanceof Array || (t = String(t).split(","));
                    for (var e, n = t.length; --n >= 0;) (e = (e = t[n]) && String(e).trim()) && (this._customAttributes[e] = !0)
                }
            }, A.setAttribute = function (e, n, i) {
                var o = this._getOrSetAttribute(!1, e, n, i);
                return o && this.trigger(t.Annotations.Events.ATTRIBUTE_CHANGED, {
                    annotation: o,
                    attributes: {
                        attrName: i
                    }
                }), o
            }, A.setAnnotationsSequence = function (t) {
                var e, n, i = t && t.length || 0,
                    o = this._annotations;
                if (i !== o.length) throw new Error("set annotations sequence: array length mismatch");
                for (e = 0; e < i; ++e) (n = this._findAnnotation(t[e])) && (n.sequence = e, o[e] = n)
            }, A.getAttribute = function (t, e) {
                return this._getOrSetAttribute(!0, t, e)
            }, A._getOrSetAttribute = function (e, n, i, o) {
                var r = this._annotations.first_li(function (t) {
                    return t.id === n
                }),
                    s = e ? "getAttribute" : "setAttribute";
                return r ? this._customAttributes.hasOwnProperty(i) ? e ? r.attributes[i] : (r.attributes[i] = o, r) : (t.Logger.error(s, ": attribute", i, "not allowed"), null) : (t.Logger.error(s, ": no annotation with id", n), null)
            }, A._addAnnotation = function (e, n) {
                var i = this._annotations,
                    r = "number" == typeof n.position ? n.position : 999999,
                    s = function (t, e) {
                        return (e || "").split("").reduce(function (t, e) {
                            return t + e.charCodeAt(0)
                        }, 16843009 & t | 12e4)
                    }(r),
                    a = this[g(o)].bind(this, e.id);
                if (!this._validate(a, s)) throw new Error("Invalid annotation");
                r = Math.min(i.length, Math.max(0, r)), e.sequence = r, i.splice(r, 0, e), this._annotationsMap[e.id] = e;
                var u = r < i.length - 1 ? i[r + 1].id : null;
                n.noNotify || this.trigger(t.Annotations.Events.ANNOTATION_CREATED, {
                    annotation: e,
                    before: u
                }), r < i.length && this._renumberAnnotations()
            }, A._renumberAnnotations = function () {
                var e = [];
                this._annotations.forEach(function (t, n) {
                    t.sequence = n, e.push(t.id)
                }), this.trigger(t.Annotations.Events.ANNOTATIONS_RENUMBERED, {
                    sequence: e
                })
            }, A._adjustAnnotationLength = function (e) {
                var n = e.firstComment(),
                    i = this._getAccess(e, n);
                !n || i.delete || "delete" === arguments[1] ? (e.isSelected() && this._selectOneAnnotation(e, !1), this._annotations.remove_li(e), delete this._annotationsMap[e.id], this.trigger(t.Annotations.Events.ANNOTATION_DELETED, {
                    id: e.id
                })) : t.Logger.error("deleteAnnotation: access denied")
            }, A._createId = function () {
                for (var t, e = 0; e < 10; ++e)
                    if (t = this._idGenerator ? this._idGenerator() : this._idSeed + String(this._baseId++), !this._annotationsMap[t]) return t;
                throw new Error("cannot generate a new annotation id, 10 attempts resulted in an existing id")
            }, A._getUserPicture = function (t) {
                var e = this._getUser(t);
                return e && e.picture || this._defaultPicture
            }, A.setCommentText = function (e, n, i) {
                var o = this._findAnnotation(e);
                if (o) {
                    var r = o.getCommentById(n);
                    if (r) {
                        if (!this._getAccess(o, r).edit) return void t.Logger.error("setCommentText: permission denied");
                        if (i) o.setCommentText(r.id, i);
                        else if (1 === o.count() || !o.isFirst(n)) return this._adjustCommentLength(o, n);
                        this.trigger(t.Annotations.Events.COMMENT_CHANGED, {
                            annotation: o,
                            comment: r,
                            status: this._getCommentStatus(o, r)
                        })
                    }
                }
            }, A.deleteComment = function (t, e) {
                console.warn(" delete comment is disabled in the demo version (but it really works in the full version).")
            }, A.addComment = function (e, n, i, o) {
                var r, s = this._findAnnotation(e);
                if (s) {
                    if ((r = s.getCommentById(n)) && !r.text) return i ? this.setCommentText(e, n, i) : 1 === s.count() ? this.deleteAnnotation(e) : void 0;
                    r = s.addComment({
                        text: i
                    });
                    var a = this._getUser(this._userId);
                    this._loadCommentUserDetails(r, a), r.setUserPicture(this._getUserPicture(this._userId)), o || this.trigger(t.Annotations.Events.COMMENT_ADDED, {
                        annotation: s,
                        comment: r,
                        status: this._getCommentStatus(s, r)
                    })
                }
            }, A.revertComment = function (t, e, n) {
                var i = this._findAnnotation(t);
                if (i) {
                    var o = i.getCommentById(e);
                    o && n && !o.text && this._adjustCommentLength(i, e)
                }
            }, A.getComment = function (t, e) {
                var n = this._findAnnotation(t),
                    i = n && n.getCommentById(e);
                return {
                    annotation: n,
                    comment: i,
                    status: i && this._getCommentStatus(n, i)
                }
            }, A.selectComment = function (e, n, i, o) {
                i = Boolean(i);
                var r = this._findAnnotation(e);
                if (r) {
                    var s = r.getCommentById(n);
                    s && (i && this.selectAnnotation(e, i), r.selectComment(n, i), this.trigger(t.Annotations.Events.COMMENT_SELECTED, {
                        annotation: r,
                        comment: s,
                        isSelected: i,
                        isEdit: o && !r.isResolved()
                    }))
                }
            }, A.getCommentStatus = function (t, e) {
                var n = this._findAnnotation(t);
                if (n) {
                    var i = n.getCommentById(e);
                    if (i) return this._getCommentStatus(n, i)
                }
                return {}
            }, A._getAccess = function (t, e) {
                if (!t || !e) return !1;
                var n, i = t.isFirst(e.id),
                    o = t.isLast(e.id),
                    r = this._userId,
                    s = t.getOpenerId(),
                    a = e.userId,
                    u = this._permissions,
                    l = function (t, e, n) {
                        return {
                            edit: p(s, a, r, t),
                            delete: p(s, a, r, e),
                            resolve: p(s, a, r, n)
                        }
                    };
                if (i && o) {
                    var c = l(u.first.edit, u.first.delete, u.first.resolve),
                        d = l(u.last.edit, u.last.delete, u.last.resolve);
                    n = {
                        edit: c.edit || d.edit,
                        delete: c.delete || d.delete,
                        resolve: c.resolve || d.resolve
                    }
                } else n = i ? l(u.first.edit, u.first.delete, u.first.resolve) : o ? l(u.last.edit, u.last.delete, u.last.resolve) : l(u.default.edit, u.default.delete, u.default.resolve);
                return e.text || e.userId !== this._userId || (n.edit = !0), n
            }, A._adjustCommentLength = function (e, n) {
                var i = e.getCommentById(n);
                if (i) {
                    if (!this._getAccess(e, i).delete) return void t.Logger.error("delete comment: access denied");
                    e.deleteComment(n) && (e.count() > 0 ? this.trigger(t.Annotations.Events.COMMENT_DELETED, {
                        annotation: e,
                        id: n
                    }) : this._adjustAnnotationLength(e))
                }
            }, A._getCommentStatus = function (t, e) {
                var n = t.isLast(e.id),
                    i = t.isResolved(),
                    o = i ? {
                        delete: !1,
                        edit: !1,
                        resolve: !1
                    } : this._getAccess(t, e),
                    r = {
                        isSelected: e.isSelected(),
                        isLast: n,
                        isFirst: t.isFirst(e.id),
                        isNew: !e.text,
                        isOwnerComment: e.userId == this._userId,
                        canDelete: o.delete,
                        canEdit: o.edit,
                        canResolve: o.resolve,
                        canReply: !i && Boolean(this._userId)
                    };
                return this._statusCallback && this._statusCallback({
                    comment: e,
                    annotation: t,
                    status: r,
                    owner: this
                }), r
            }, A.selectAnnotation = function (t, e) {
                var n = this._findAnnotation(t);
                (e = void 0 === e || Boolean(e), n) && (n.isSelected() !== e && (e && this.unselectAll([n]), this._selectOneAnnotation(n, e)))
            }, A.resolveAnnotation = function (e, n, i) {
                var o = this._findAnnotation(e);
                o && (o.setResolved(i), this.trigger(t.Annotations.Events.ANNOTATION_RESOLVED, {
                    annotation: o
                }), this.trigger(t.Annotations.Events.ANNOTATION_UPDATED, {
                    annotation: o
                }))
            }, A.onClientReady = function (t) { }, A.hasAnnotation = function (t) {
                return Boolean(this._findAnnotation(t))
            }, A.getAllAnnotations = function (t) {
                if (!0 === t) {
                    for (var e = {}, n = this._annotations, i = 0, o = n.length; i < o; ++i) e[n[i].id] = 1;
                    return e
                }
                return this._annotations.select_li(function (t) {
                    return t.id
                })
            }, A.serializeAnnotation = function (t) {
                return t ? ("string" === typeof t && (t = this._findAnnotation(t)), t ? t.saveToObject() : null) : ""
            }, A.loadFromHTMLString = function (t) {
                var e = window.document.createElement("div");
                e.innerHTML = t, this.loadFromDOMNode(e)
            }, A.loadFromDOMNode = function (e) {
                if (e) {
                    var n, i = [];
                    try {
                        var o, r, s = e.getElementsByTagName("annotation"),
                            a = s.length;
                        for (r = 0; r < a; ++r) o = s[r], (n = t.Annotations.extractAnnotation(o)) && i.push(n)
                    } finally {
                        this.loadFromData(i)
                    }
                }
            }, A.destroy = function () {
                this.loadFromData(null), this.removeAllListeners()
            }, A.loadFromData = function (e) {
                e = e || [], this.trigger(t.Annotations.Events.RESET), this._annotations.splice(0);
                var n = this._annotationsMap;
                for (var i in n) n.hasOwnProperty(i) && delete n[i];
                var o = e.length;
                if (!(isNaN(o) || o < 1))
                    for (var r = 0; r < o; ++r) {
                        var s = e[r];
                        this.loadAnnotationFromData(s)
                    }
            }, A.loadAnnotationFromData = function (e, n) {
                var i;
                if (void 0 === n && (n = 999999), e && (i = e.id) && !this._findAnnotation(i)) try {
                    var o = new E(e),
                        r = {};
                    if (e.comments && e.comments.length)
                        for (var s, a, u, l = e.comments, c = l && l.length, d = 0; d < c; ++d) (s = l[d]).text && (a = o.addComment(s), u = this._getUser(a.userId), this._loadCommentUserDetails(a, u), u || r[s.userId] || (r[s.userId] = v({
                            id: s.userId,
                            name: s.userName
                        })));
                    o.count() && this._addAnnotation(o, {
                        position: n
                    }), Object.keys(r).forEach(function (t) {
                        var e = r[t];
                        this._requestUserCallback ? this._requestUserCallback(e, function (t) {
                            this.addUsers([t])
                        }.bind(this)) : this.addUsers([e])
                    }.bind(this))
                } catch (e) {
                    t.Logger.error(e)
                }
            }, A._refreshUser = function (e) {
                var n = e.id,
                    i = n === this._userId;
                this._annotations.forEach(function (o) {
                    for (var r, s = 0, a = o.count() ; s < a; ++s) ((r = o.getCommentByIndex(s)).userId === n || !r.userId && i) && (this._loadCommentUserDetails(r, e), this.trigger(t.Annotations.Events.COMMENT_CHANGED, {
                        annotation: o,
                        comment: r,
                        status: this._getCommentStatus(o, r)
                    }))
                }.bind(this))
            }, A._loadCommentUserDetails = function (t, e) {
                return t.userId = e && e.id || t.userId || 0, t.userName = e && e.name || "user", t.setUserPicture(e && e.picture || this._defaultPicture), t
            }, A._getUser = function (t) {
                return t && this._users[t] || null
            }, A.addUsers = function (t) {
                t && t.forEach(function (t) {
                    t = v(t);
                    var e = this._users[t.id];
                    e && e.name === t.name && e.picture === t.picture || (this._users[t.id] = t, this._refreshUser(t))
                }.bind(this))
            }, A._findComments = function (t, e) {
                for (var n, i = [], o = 0, r = t.count() ; o < r; ++o) e(n = t.getCommentByIndex(o)) && i.push(n);
                return i
            }
        }(window && window.$LOOPINDEX$, window && window.jQuery),
        function (t, e) {
            var n, i = new RegExp("cke_annotation"),
                o = t.Utils.browser,
                r = o.msie ? "data-lance-contenteditable" : "contenteditable",
                s = "data-annotation-id",
                a = "data-ant",
                u = "data-annotation-seq";

            function l() {
                return Array.prototype.slice.apply(arguments, []).join("/").replace(/(^|[^:])\/[\/]+/g, "$1/")
            }

            function c(t) {
                return t && t.className && t.className.indexOf("mce-offscreen-selection") >= 0
            }

            function d(t, e) {
                if (!t) return "";
                var n = 0 === t.indexOf("lance_") ? t : "lance_" + t,
                    i = e || this._editor,
                    o = i && i.editorManager.i18n.translate(n) || t || t;
                return o && (o = o.replace(/^lance_/, "")), o
            }

            function h(t) {
                (t && t.style || {})["background-image"] && (t.style["background-image"] = null)
            }
            tinymce.PluginManager.requireLangPack("lance", "en,fr,de"), tinymce.PluginManager.add("lance", function (t, e) {
                var i = new n(e);
                return i.initPlugin(t, t.settings && t.settings.lance), i
            }), (n = function (t) {
                this.path = t, this._domLoaded = !1, this._editor = null, this._manager = null, this._deletedAnnotations = {}, this._mutationObserver = null, this._syncInterval = null, this._isSelecting = !1
            }).prototype = {
                version: "1.1.08",
                initPlugin: function (n, i) {
                    this._editor = n, this._config = i || {}, t.Logger.config(this._config.debug), this._customAttributes = i.customAttributes || [], this._setPluginFeatures(n, this._customAttributes), this._onDragCancel = this._onDragCancel.bind(this), this._onAfterEdit = this._onAfterEdit.bind(this), this._syncNodes = this._syncNodes.bind(this);
                    var o = e.extend(!0, {}, i.annotations);
                    o.getLocalizedString = d.bind(this), o.dictionary = this._editor.lang && this._editor.lang.lance, o.customAttributes = this._customAttributes.slice();
                    var r = new t.Annotations(o);
                    this._manager = r, this._bindToOwner(r, !0)
                },
                init: function (t, e) {
                    var n = this._removeBindings = [],
                        i = this;

                    function o(e, o) {
                        n.push(t.on(e, o.bind(i)))
                    }
                    t.addButton("lance", {
                        tooltip: d("insert comment", t),
                        image: l(this.path, "/css/images/marker3tb.png"),
                        onclick: this._onAnnotate.bind(this)
                    }), o("SetContent", this._onDomLoaded), o("Click", this._onDocumentClick), this._domLoaded = !1, t.fire("lance::init", {
                        lance: this
                    }, !1)
                },
                _uninit: function () {
                    this._bindToOwner(!1), this._manager = null
                },
                getAnnotations: function () {
                    return this._manager
                },
                getAnnotationNodeForId: function (t) {
                    var n = this._getDocument();
                    return n && e(n).find("[" + s + "='" + t + "']")[0]
                },
                _onDomLoaded: function (e) {
                    if (!e || !e.selection) {
                        var n = this._editor,
                            i = this._editor.getDoc(),
                            o = n.getBody();
                        this._domLoaded = !0;
                        try {
                            this._loadCSS(i)
                        } catch (e) {
                            t.Logger.error(e)
                        }
                        o.addEventListener("drop", this._onDrop.bind(this)), this.startObserving() || (this._removeBindings.push(this._editor.on("paste", this._onAfterEdit)), this._syncInterval && clearInterval(this._syncInterval)), this._syncAnnotations()
                    }
                },
                _syncAnnotations: function () {
                    if (this._manager) {
                        this._deletedAnnotations = {};
                        for (var n, i, o = this._getAnnotationNodes(), r = {}, a = [], u = 0, l = o.length; u < l; ++u) !(i = (n = o[u]) && n.getAttribute(s)) || i in r ? e(n).remove() : (a.push(n), r[i] = n);
                        a.sort(t.utils.compareNodePosition);
                        var c, d, h = [];
                        a.forEach(function (e) {
                            try {
                                var n = t.Annotations.extractAnnotation(e);
                                n && h.push(n)
                            } catch (e) {
                                t.Logger.error(e)
                            }
                        }.bind(this)), this._bindToOwner(this._manager, !1), this._manager.loadFromData(h), a.forEach(function (t) {
                            c = t.getAttribute(s), (d = this._manager.getAnnotationById(c)) ? (this._setupNode(t), this._populateAnnotation(t, d)) : e(t).remove()
                        }.bind(this)), this._bindToOwner(this._manager, !0)
                    }
                },
                _loadCSS: function (t) {
                    var e = t.getElementsByTagName("head")[0],
                        n = t.createElement("link");
                    n.setAttribute("rel", "stylesheet"), n.setAttribute("type", "text/css"), n.setAttribute("href", l(this.path, "/css/annotate.css")), e.appendChild(n)
                },
                _getDocument: function () {
                    return this._editor && this._editor.getDoc()
                },
                _getBody: function () {
                    return this._editor && this._editor.getBody()
                },
                _bindToOwner: function (e, n) {
                    e && (e.removeListener(null, this), n && (e.bind(t.Annotations.Events.ANNOTATION_CREATED, this._onAnnotationCreated.bind(this), this), e.bind(t.Annotations.Events.ANNOTATION_DELETED, this._onAnnotationDeleted.bind(this), this), e.bind(t.Annotations.Events.ANNOTATION_SELECTED, this._onAnnotationSelected.bind(this), this), e.bind(t.Annotations.Events.ANNOTATION_UPDATED, this._onAnnotationChanged.bind(this), this), e.bind(t.Annotations.Events.ATTRIBUTE_CHANGED, this._onAnnotationAttributeChanged.bind(this), this), e.bind(t.Annotations.Events.COMMENT_DELETED, this._onCommentDeleted.bind(this), this), e.bind(t.Annotations.Events.COMMENT_CHANGED, this._onCommentChanged.bind(this), this), e.bind(t.Annotations.Events.COMMENT_ADDED, this._onCommentAdded.bind(this), this), e.bind(t.Annotations.Events.ENABLED_CHANGED, this._onAnnotationsEnabledChanged.bind(this), this), e.bind(t.Annotations.Events.DONE_EDITING, this._onCommentEditingDone, this), e.bind(t.Annotations.Events.ANNOTATIONS_RENUMBERED, this._onAnnotationsRenumbered, this)))
                },
                _onAnnotationCreated: function (t) {
                    var n, i, o = t && t.annotation;
                    if (o && this._editor) {
                        i = this._createAnnotation(o), this._populateAnnotation(i, o), this.stopObserving(), this._editor.focus(), (n = this._editor.selection) && !n.isCollapsed() && n.collapse(!1), this._editor.insertContent(i.outerHTML);
                        var r = this._findAnnotationNode(o.id);
                        r && e(r).find("object").remove(), this.startObserving()
                    }
                },
                _findAnnotationNode: function (t) {
                    var n = this._getBody();
                    return n && e(n).find("annotation[data-annotation-id='" + t + "']")
                },
                _onAnnotationDeleted: function (t) {
                    var e = t && t.id,
                        n = this._findAnnotationNode(e);
                    n && n.remove()
                },
                _onAnnotationChanged: function (t) {
                    var e = t && t.annotation;
                    if (e) {
                        var n = this._findAnnotationNode(e.id);
                        n && n.length && this._populateAnnotation(n[0], e)
                    }
                },
                _onAnnotationAttributeChanged: function (t) {
                    return this._onAnnotationChanged(t)
                },
                _onAnnotationSelected: function (t) {
                    var e = t && t.annotation;
                    if (e) {
                        var n = this._findAnnotationNode(e.id);
                        n && n.length && this._selectAnnotation(n[0], e)
                    }
                },
                _onCommentDeleted: function (t) {
                    this._onAnnotationChanged(t)
                },
                _onCommentChanged: function (t) {
                    this._onAnnotationChanged(t)
                },
                _onCommentAdded: function (t) {
                    this._onAnnotationChanged(t)
                },
                _onCommentEditingDone: function () {
                    this._editor.focus()
                },
                _onAnnotationsEnabledChanged: function (t) {
                    var e = t && t.isEnabled,
                        n = this._editor.getCommand("annotate");
                    n && n.setState(e ? CKEDITOR.TRISTATE_OFF : CKEDITOR.TRISTATE_DISABLED)
                },
                _onAnnotate: function (t) {
                    this._manager && this._manager.insertAnnotation({
                        select: !0,
                        edit: !0,
                        position: this._getInsertPosition()
                    })
                },
                _onAfterEdit: function (t) {
                    this._syncTimeout || (this._syncTimeout = setTimeout(this._syncNodes, 20))
                },
                _createAnnotation: function (t) {
                    var e = this._getDocument(),
                        n = e.createElement("annotation");
                    return this._setupNode(n), n.appendChild(e.createElement("object")), n
                },
                _setupNode: function (t) {
                    t.setAttribute("data-selected", !1), t.setAttribute("data-track-changes-ignore", !0)
                },
                _selectAnnotation: function (n, i) {
                    n && (i.isSelected() ? (n.setAttribute("data-selected", "true"), t.utils.scrollToElement(e(n))) : n.removeAttribute("data-selected"))
                },
                _populateAnnotation: function (t, n) {
                    this._selectAnnotation(t, n);
                    var i = n.text();
                    i.length > 50 && (i = i.substring(0, 50) + "..."), e(t).attr(n.attributes || {}), t.setAttribute(r, !1), t.setAttribute("title", i), t.setAttribute(u, n.sequence), t.setAttribute(s, n.id);
                    var o = n.saveToObject();
                    t.setAttribute(a, escape(JSON.stringify(o)))
                },
                _onEditorNodeMouseDown: function (t) {
                    var n = t && t.data && t.data.target;
                    if (n && (n && n.getAttribute(s))) {
                        var i = this._getBody(),
                            o = e(i).find("annotation[" + s + "]"),
                            a = function () {
                                this._mayBeginDrag = !1, ed$.removeEventListener("mouseup", a, !0), o.attr(r, "false")
                            };
                        o.attr(r, ""), ed$.addEventListener("mouseup", a, !0), this._mayBeginDrag = !0
                    }
                },
                _onEditorNodeClicked: function (t) {
                    if (this._manager && t) {
                        var e = t.getAttribute && t.getAttribute(s);
                        e ? (t.setAttribute(r, !0), setTimeout(function () {
                            t.setAttribute(r, !1)
                        }, 1), this._isSelecting = !0, this._manager.selectAnnotation(e, !0), this._isSelecting = !1) : this._manager.unselectAll()
                    }
                },
                _onDocumentClick: function (t) {
                    this._manager && t.target && this._onEditorNodeClicked(t.target)
                },
                _syncNodes: function () {
                    if (this._syncTimeout = null, this._manager) {
                        var n, i, o, r, u, l, c, d, f = this._getAnnotationNodes(),
                            g = [],
                            m = {};
                        for (u = 0, l = f.length; u < l; ++u) !(d = (n = f[u]) && n.getAttribute(s)) || d in m ? e(n).remove() : (m[d] = !0, g.push(n), "IMG" !== (c = n && n.nodeName) && "img" !== c && "ANNOTATION" !== c && "annotation" !== c || h(n));
                        for (r = this._manager.getAllAnnotations(), u = g.length - 1; u >= 0; --u) (i = g[u].getAttribute(s)) && (o = r.indexOf(i)) >= 0 && (r.splice(o, 1), g.splice(u, 1));
                        0 != r.length || 0 != g.length ? (this._bindToOwner(this._manager, !1), r.forEach(function (t) {
                            var e = this._manager.serializeAnnotation(t);
                            e && (this._deletedAnnotations[t] = e), this._manager.deleteAnnotation(t, "delete")
                        }.bind(this)), g.forEach(function (e) {
                            this._setupNode(e);
                            var n, i = e.getAttribute(s),
                                o = i && this._deletedAnnotations[i];
                            o && (e.setAttribute(a, escape(JSON.stringify(o))), delete this._deletedAnnotations[i]), (n = t.Annotations.extractAnnotation(e)) && this._manager.loadAnnotationFromData(n, this._getInsertPosition(e))
                        }.bind(this)), this._renumberNodes(), this._bindToOwner(this._manager, !0)) : this._renumberNodes(f)
                    }
                },
                _renumberNodes: function (e) {
                    if ((e = e || this._getAnnotationNodes()).length) {
                        e.sort(t.utils.compareNodePosition);
                        var n = [];
                        e.forEach(function (t, e) {
                            t.setAttribute(u, e), n.push(t.getAttribute(s))
                        }), this._manager.setAnnotationsSequence(n)
                    }
                },
                _onAnnotationsRenumbered: function (t) {
                    var e = t && t.sequence;
                    if (e && e.length) {
                        var n = {};
                        this._getAnnotationNodes().forEach(function (t) {
                            var e = t.getAttribute(s);
                            e && (n[e] = t)
                        }), e.forEach(function (t, e) {
                            var i = n[t];
                            i && i.setAttribute(u, e)
                        })
                    }
                },
                _onDragCancel: function (t) {
                    var e, n = t.data,
                        i = !1;
                    if (o.msie) i = this._mayBeginDrag;
                    else {
                        if (!n) return;
                        if (!(e = n.dataTransfer ? n.dataTransfer.$ : n.$ && n.$.dataTransfer) || !e.getData) return;
                        var r = e.getData(o.msie ? "text" : "text/html");
                        i = r && r.indexOf(s) > 0 && r.indexOf(a) > 0
                    }
                    if (i) return t.cancel(), n.preventDefault && n.preventDefault(), !1
                },
                _onDrop: function (t) {
                    var n = t && t.dataTransfer,
                        o = n && n.items && n.items[0];
                    if (o && "text/html" === o.type) {
                        var r = t.target.ownerDocument;
                        o.getAsString(function (t) {
                            var n = e(t),
                                o = n && n[0];
                            if (o.id && o.className.match(i)) {
                                var s = o.id;
                                setTimeout(function () {
                                    h(r.getElementById(s))
                                }, 10)
                            }
                        })
                    }
                },
                _getInsertPosition: function (t) {
                    try {
                        var e, n;
                        if (t) e = t.getAddress(), n = t;
                        else {
                            n = null;
                            var i = this._editor.getSelection().getRanges(),
                                o = i && i.length && i[i.length - 1],
                                r = 999999;
                            if (!o || !o.endContainer) return r;
                            e = o.endContainer.getAddress()
                        }
                        for (var s = this._getAnnotationNodes(), a = 0, u = s.length; a < u; ++a) {
                            var l = s[a];
                            if (l === n) return a;
                            var c = l.getAddress();
                            if (this._compareAddresses(e, c) <= 0) return a
                        }
                    } catch (t) { }
                    return r
                },
                onDOMMutation: function (t) {
                    var e, n = function (t) {
                        var e, n, i;
                        if (!t || !(e = t.length)) return !1;
                        for (var o = 0; o < e; ++o)
                            if (i = t[o], ("ANNOTATION" === (n = String(i.tagName)) || "ANNOTATION" === n.toUpperCase()) && !c(i.parentNode)) return !0;
                        return !1
                    };
                    if (t && t.length)
                        for (var i = t.length - 1; i >= 0; --i)
                            if (!c((e = t[i]).target) && e && (n(e.addedNodes) || n(e.removedNodes))) return this._syncNodes()
                },
                stopObserving: function () {
                    this._mutationObserver && this._mutationObserver.disconnect()
                },
                startObserving: function () {
                    if (!window.MutationObserver) return !1;
                    this._mutationObserver || (this._mutationObserver = new MutationObserver(this.onDOMMutation.bind(this)));
                    return this._mutationObserver.disconnect(), this._mutationObserver.observe(this._getBody(), {
                        childList: !0,
                        subtree: !0
                    }), !0
                },
                _getAnnotationNodes: function () {
                    var t = this._editor ? this._editor.getDoc() : null;
                    if (!t) return [];
                    for (var e = [], n = t.getElementsByTagName("annotation"), i = 0, o = n.length; i < o; ++i) {
                        var r = n[i];
                        r && r.getAttribute(s) && !c(r.parentNode) && e.push(r)
                    }
                    return e
                },
                _compareAddresses: function (t, e) {
                    for (var n = Math.min(t.length, e.length), i = 0; i < n; i++) {
                        if ((o = t[i] - e[i]) < 0) return -1;
                        if (o > 0) return 1
                    }
                    var o;
                    return (o = e.length - t.length) > 0 ? 1 : o < 0 ? -1 : 0
                },
                _setPluginFeatures: function (t, e) {
                    t && t.filter && t.filter.addFeature && (t.filter.addFeature({
                        allowedContent: "annotation[*]"
                    }), t.filter.addFeature({
                        allowedContent: "img[" + s + "]"
                    }))
                }
            }
        }(window && window.$LOOPINDEX$, window.jQuery)
    }()
}]);