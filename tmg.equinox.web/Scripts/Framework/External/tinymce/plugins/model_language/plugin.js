/**
 * plugin.js
 *
 * Released under LGPL License.
 * Copyright (c) 1999-2015 Ephox Corp. All rights reserved
 *
 * License: http://www.tinymce.com/license
 * Contributing: http://www.tinymce.com/contributing
 */

/*jshint unused:false */
/*global tinymce:true */

/**
 * Example plugin that adds a toolbar button and menu item.
 */
tinymce.PluginManager.add('model_language', function (editor, url) {
    // Add a button that opens a window
    editor.addButton('model_language', {
        title: 'Model Language',
        icon: false,
        image: tinymce.baseURL + '/plugins/model_language/ms-icon-70x70.png',
        onclick: function () {
            var selectedText = editor.selection.getContent();
            var selectedRng = editor.selection.getRng();
            var startContainer = selectedRng.startContainer;
            var endContainer = selectedRng.endContainer;
            var selectedTextLength = selectedText.length;
            if (selectedTextLength > 0) {
                editor.selection.setContent('<mlstart contenteditable=false>' + selectedText + '</mlstart>');
            }
            else {
                editor.windowManager.alert('Please select some text for creating model language');
            }
        }
    });
});