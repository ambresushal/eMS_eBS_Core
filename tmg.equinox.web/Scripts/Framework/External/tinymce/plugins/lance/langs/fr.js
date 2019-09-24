(function() {
	"use strict";
	var langs = ["fr", "fr_CA", "fr_FR", "fr_BE", "fr_CH", "fr_LU", "fr_MC"];
	langs.forEach(function(lang) {
		tinymce.addI18n(lang,{
			"lance_save": "Enregistrer",
			"lance_cancel": "Annuler",
			"lance_edit": "Modifier",
			"lance_reply": "Répondre",
			"lance_delete": "Supprimer",
			"lance-resolve": "Résoudre",
			"lance_delete-resolve": "Supprimer",
			"lance_comment": "Commentaire",
			"lance_1 minute ago": "il y a 1 minute",
			"lance_minutes ago": "il y a %d minutes",
			"lance_on date": "sur le %d %MMM",
			"lance_on time": "lance_en %hh:%mm",
			"lance_on full date": "sur %F",
			"lance_now": "maintentant",
			"lance_on": "dans",
			"lance_months": [ "janv", "févr", "mars", "avr", "mai", "juin", "juil", "août", "sept", "oct", "nov", "déc" ],
			"lance_full_months": [ "Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre" ],
			"lance_insert comment": "Entrez commentaire",
			"lance_enter comment": "Entrez commentaire",
			"lance_delete this annotation?": "Supprimer cette annotation?",
			"lance_delete this comment?": "Supprimer ce commentaire?",
			"lance_enter your comment": "Entrez votre commentaire",
			"lance_reply to this comment": "Répondre à ce commentaire"
		});
	});
}());