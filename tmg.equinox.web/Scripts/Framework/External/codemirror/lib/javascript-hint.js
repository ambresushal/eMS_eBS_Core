// CodeMirror, copyright (c) by Marijn Haverbeke and others
// Distributed under an MIT license: https://codemirror.net/LICENSE

(function (mod) {
    //if (typeof exports == "object" && typeof module == "object") // CommonJS
    //  mod(require("../../lib/codemirror"));
    //else if (typeof define == "function" && define.amd) // AMD
    //  define(["../../lib/codemirror"], mod);
    //else // Plain browser env
    mod(CodeMirror);
})(function (CodeMirror) {
    var Pos = CodeMirror.Pos;

    function forEach(arr, f) {
        for (var i = 0, e = arr.length; i < e; ++i) f(arr[i]);
    }

    function arrayContains(arr, item) {
        if (!Array.prototype.indexOf) {
            var i = arr.length;
            while (i--) {
                if (arr[i] === item) {
                    return true;
                }
            }
            return false;
        }
        return arr.indexOf(item) != -1;
    }

    function scriptHint(editor, keywords, getToken, options) {
        // Find the token at the cursor
        var cur = editor.getCursor(), token = getToken(editor, cur);
        if (/\b(?:string|comment)\b/.test(token.type)) return;
        var innerMode = CodeMirror.innerMode(editor.getMode(), token.state);
        if (innerMode.mode.helperType === "json") return;
        token.state = innerMode.state;

        // If it's not a 'word-style' token, ignore the token.
        if (!/^[\w$_]*$/.test(token.string)) {
            token = {
                start: cur.ch, end: cur.ch, string: "", state: token.state,
                type: token.string == "." ? "property" : null
            };
        } else if (token.end > cur.ch) {
            token.end = cur.ch;
            token.string = token.string.slice(0, cur.ch - token.start);
        }

        var tprop = token;
        // If it is a property, find out what it is a property of.
        while (tprop.type == "property") {
            tprop = getToken(editor, Pos(cur.line, tprop.start));
            if (tprop.string != ".") return;
            tprop = getToken(editor, Pos(cur.line, tprop.start));
            if (!context) var context = [];
            context.push(tprop);
        }
        return {
            list: getCompletions(token, context, keywords, options),
            from: Pos(cur.line, token.start),
            to: Pos(cur.line, token.end)
        };
    }

    function javascriptHint(editor, options) {
        return scriptHint(editor, javascriptKeywords,
                          function (e, cur) { return e.getTokenAt(cur); },
                          options);
    };
    CodeMirror.registerHelper("hint", "javascript", javascriptHint);

    function getCoffeeScriptToken(editor, cur) {
        // This getToken, it is for coffeescript, imitates the behavior of
        // getTokenAt method in javascript.js, that is, returning "property"
        // type and treat "." as indepenent token.
        var token = editor.getTokenAt(cur);
        if (cur.ch == token.start + 1 && token.string.charAt(0) == '.') {
            token.end = token.start;
            token.string = '.';
            token.type = "property";
        }
        else if (/^\.[\w$_]*$/.test(token.string)) {
            token.type = "property";
            token.start++;
            token.string = token.string.replace(/\./, '');
        }
        return token;
    }

    function coffeescriptHint(editor, options) {
        return scriptHint(editor, coffeescriptKeywords, getCoffeeScriptToken, options);
    }
    CodeMirror.registerHelper("hint", "coffeescript", coffeescriptHint);

    var stringProps = ("APPENDARRAY(target,source,sync,column name);"+
                        "|APPENDTEXT(target,value);"+
                        "|CONTAINS(variable,texttofind);"+
                        "|CONVERTTOTYPE(variable,type);"+
                        "|COUNT(variable);"+
                        "|COUNTARRAY(variable,separator);"+
                        "|CREATETOKEN(variable);"+
                        "|CROSSJOIN(variable1,variable2);"+
                        "|EQUALS(variable1, variable2);"+
                        "|EXCEPT(source1, source2, keys);"+
                        "|FILTERLIST(source, rowFilter, childContainer, childFilter, columns);"+
                        "|GETARRAY(variable, filter);"+
                        "|GETDISTINCTCOLVAL(variable, column);"+
                        "|GETVAL(variable, prop);"+
                        "|INTERSECT(variable1, variable 2, keys);"+
                        "|MERGEARRAY(variable1, variable 2, keys);"+
                        "|NUMBERTOWORDS(number);"+
                        "|REPLACEALL(variable,find,replace);"+
                        "|SETARRAY(target,value);"+
                        "|SETDEFAULTROW(target,value);"+
                        "|SETSOURCE(name,value);"+
                        "|SETTEXT(target,value);"+
                        "|SETTOKENVALUE(variable, sourceval, targetpath, sourcepath);"+
                        "|SLICEARRAY(target, rows, keys);"+
                        "|SORTARRAY(variable, columns, direction);"+
                        "|TOBOOLEAN(variable);"+
                        "|UNION(variabl1, variable2,keys);"+
                        "|UPDATEARRAY(variable,filtercondition,updates);"+
                        "|UPDATETEXT(target,find,replace);" +
                        "|TRUE()").split("|");
    var arrayProps = ("").split("|");
    var funcProps = ("").split("|");
    var javascriptKeywords = ("").split("|");
    var coffeescriptKeywords = ("").split("|");

    function forAllProps(obj, callback) {
        if (!Object.getOwnPropertyNames || !Object.getPrototypeOf) {
            for (var name in obj) callback(name)
        } else {
            for (var o = obj; o; o = Object.getPrototypeOf(o))
                Object.getOwnPropertyNames(o).forEach(callback)
        }
    }

    function getCompletions(token, context, keywords, options) {
        var found = [], start = token.string, global = options && options.globalScope || window;
        function maybeAdd(str) {
            if (str.lastIndexOf(start, 0) == 0 && !arrayContains(found, str)) found.push(str);
        }
        function gatherCompletions(obj) {
            forEach(stringProps, maybeAdd);
            //if (typeof obj == "string") forEach(stringProps, maybeAdd);
            //else if (obj instanceof Array) forEach(arrayProps, maybeAdd);
            //else if (obj instanceof Function) forEach(funcProps, maybeAdd);
            //forAllProps(obj, maybeAdd)
        }

        if (context && context.length) {
            // If this is a property, see if it belongs to some object we can
            // find in the current environment.
            var obj = context.pop(), base;
            if (obj.type && obj.type.indexOf("variable") === 0) {
                if (options && options.additionalContext)
                    base = options.additionalContext[obj.string];
                if (!options || options.useGlobalScope !== false)
                    base = base || global[obj.string];
            } else if (obj.type == "string") {
                base = "";
            } else if (obj.type == "atom") {
                base = 1;
            } else if (obj.type == "function") {
                if (global.jQuery != null && (obj.string == '$' || obj.string == 'jQuery') &&
                    (typeof global.jQuery == 'function'))
                    base = global.jQuery();
                else if (global._ != null && (obj.string == '_') && (typeof global._ == 'function'))
                    base = global._();
            }
            while (base != null && context.length)
                base = base[context.pop().string];
            if (base != null) gatherCompletions(base);
        } else {
            // If not, just look in the global object and any local scope
            // (reading into JS mode internals to get at the local and global variables)
            for (var v = token.state.localVars; v; v = v.next) maybeAdd(v.name);
            for (var v = token.state.globalVars; v; v = v.next) maybeAdd(v.name);
            if (!options || options.useGlobalScope !== false)
                gatherCompletions(global);
            forEach(keywords, maybeAdd);
        }
        return found;
    }
});
