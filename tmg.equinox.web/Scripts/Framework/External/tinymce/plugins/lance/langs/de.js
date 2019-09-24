(function () {
	"use strict";
	var langs = ["de", "de_AT", "de_DE", "de_LU", "de_CH", "de_LI"];
	langs.forEach(function (lang) {
		tinymce.addI18n(lang, {
			"lance_save": "Speichern ",
			"lance_cancel": "Abbrechen",
			"lance_edit": "Bearbeitung",
			"lance_reply": "Reply",
			"lance_delete": "Delete",
			"lance-resolve": "Entschlossenheit",
			"lance_delete-resolve": "Delete",
			"lance_comment": "Comment",
			"lance_1 minute ago": "vor 1 Minute",
			"lance_minutes ago": "vor %d Minuten",
			"lance_on date": "am %MMM %d",
			"lance_on time": "am %hh:%mm",
			"lance_on full date": "am %F",
			"lance_now": "jetzt",
			"lance_on": "am",
			"lance_months": ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
			"lance_full_months": ["Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember"],
			"lance_insert comment": "Insert Comment",
			"lance_enter comment": "Enter comment",
			"lance_delete this annotation?": "Delete this annotation?",
			"lance_delete this comment?": "Delete this comment?",
			"lance_enter your comment": "Enter your comment",
			"lance_reply to this comment": "Reply to this comment",
			"lance_Insert annotation": "Insert annotation"
		});
	});
}());