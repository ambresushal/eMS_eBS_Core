(function() {
	"use strict";
	var langs = ["fr", "fr_CA", "fr_FR", "fr_BE", "fr_CH", "fr_LU", "fr_MC"];
	langs.forEach(function(lang) {
		tinymce.addI18n(lang,{
			flite_TOGGLE_TRACKING: "Activer/désactiver le suivi des modifications",
			flite_TOGGLE_SHOW: "Activer/désactiver le suivi des modifications",
			flite_ACCEPT_ALL: "Accepter toutes les modifications",
			flite_REJECT_ALL: "Refuser toutes les modifications",
			flite_ACCEPT_ONE: "Accepter",
			flite_REJECT_ONE: "Refuser",
			flite_START_TRACKING: "Activer le suivi des modifications",
			flite_STOP_TRACKING: "Désactiver le suivi des modifications",
			flite_PENDING_CHANGES: "Votre document contient des modifications en attente.\nVeuillez les traiter avant de désactiver le suivi des modifications.",
			flite_HIDE_TRACKED: "Masquer les marques de modifications",
			flite_SHOW_TRACKED: "Afficher les marques de modification",
			flite_CHANGE_TYPE_ADDED: "inséré",
			flite_CHANGE_TYPE_DELETED: "supprimé",
			flite_MONTHS: ["Janv.", "Févr.", "Mars", "Avr.", "Mai", "Juin", "Juill.", "Août", "Sept.", "Oct.", "Nov.", "Déc."],
			flite_NOW: "maintenant",
			flite_MINUTE_AGO: "il y a 1 minute",
			flite_MINUTES_AGO: "il y a %Minutes minutes",
			flite_BY: "par",
			flite_ON: "en",
			flite_AT: "à",
			flite_FLITE_LABELS_DATE: function(day, month, year)
			{
				if(typeof(year) != 'undefined') {
					year = " " + year;
				}
				else {
					year = "";
				}
				return day + " " + this.MONTHS[month] + year;
			}
		});
	});
}());