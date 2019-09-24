(function () {
	"use strict";
	var langs = ["de", "de_AT", "de_DE", "de_LU", "de_CH", "de_LI"];
	langs.forEach(function (lang) {
		tinymce.addI18n(lang, {
			flite_CHANGE_TYPE_ADDED: "hinzugefügt",
			flite_CHANGE_TYPE_DELETED: "gelöscht",
			flite_MONTHS: ["Januar", "Februar", "März", "April", "Mai", "Juni", "Juli", "August", "September", "Oktober", "November", "Dezember"],
			flite_NOW: "jetzt",
			flite_MINUTE_AGO: "vor 1 Minute",
			flite_MINUTES_AGO: "vor %Minutes Minuten",
			flite_BY: "von",
			flite_ON: "am",
			flite_AT: "um",
			flite_TOGGLE_TRACKING: "Überarbeitungsmodus umschalten",
			flite_TOGGLE_SHOW: "Überarbeitungsmodus umschalten",
			flite_ACCEPT_ALL: "Alle Änderungen annehmen",
			flite_REJECT_ALL: "Alle Änderungen ablehnen",
			flite_ACCEPT_ONE: "Änderung annehmen",
			flite_REJECT_ONE: "Änderung ablehnen",
			flite_START_TRACKING: "Überarbeitungsmodus einschalten",
			flite_STOP_TRACKING: "Überarbeitungsmodus ausschalten",
			flite_PENDING_CHANGES: "Das Dokument beinhaltet Überarbeitungen.\nBitte annehmen oder ablehnen bevor der Überarbeitungsmodus beendet wird.",
			flite_HIDE_TRACKED: "Änderungen ausblenden",
			flite_SHOW_TRACKED: "Äderungen anzeigen",
			
			flite_FLITE_LABELS_DATE: function (day, month, year)
			{
				if(typeof(year) != 'undefined') {
					year = ", " + year;
				}
				else {
					year = "";
				}
				return day + ". " + this.MONTHS[month] + year;
			}
		});
	});
}());