(function () {
	"use strict";
	var langs = ["en", "en_US", "en_AU", "en_CA", "en_GB", "en_ZA", "en_NZ", "en_IE", "en_PH"];
	langs.forEach(function (lang) {
		tinymce.addI18n(lang, {
			"lance_save": "Save",
			"lance_cancel": "Cancel",
			"lance_edit": "Edit",
			"lance_reply": "Reply",
			"lance_delete": "Delete",
			"lance_delete-resolve": "Delete",
			"lance_comment": "Comment",
			"lance_1 minute ago": "1 minute ago",
			"lance_minutes ago": "%ma minutes ago",
			"lance_on date": "on %MMM %d",
			"lance_on time": "on %hh:%mm",
			"lance_on full date": "on %F",
			"lance_now": "now",
			"lance_on": "on",
			"lance_months": ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
			"lance_full_months": ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
			"lance_insert comment": "Insert Comment",
			"lance_enter comment": "Enter comment",
			"lance_delete this annotation?": "Delete this annotation?",
			"lance_delete this comment?": "Delete this comment?",
			"lance_enter your comment": "Enter your comment",
			"lance_reply to this comment": "Reply to this comment",
			"lance_insert annotation": "Insert annotation"
		});
	});
}());