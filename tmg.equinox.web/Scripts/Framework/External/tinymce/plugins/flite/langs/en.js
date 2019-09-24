(function () {
	"use strict";
	var langs = ["en", "en_US", "en_AU", "en_CA", "en_GB", "en_ZA", "en_NZ", "en_IE", "en_PH"];
	langs.forEach(function (lang) {
		tinymce.addI18n(lang, {
			flite_TOGGLE_TRACKING: "Toggle Tracking Changes",
			flite_TOGGLE_SHOW: "Toggle Tracking visibility",
			flite_ACCEPT_ALL: "Accept all changes",
			flite_REJECT_ALL: "Reject all changes",
			flite_ACCEPT_ONE: "Accept Change",
			flite_REJECT_ONE: "Reject Change",
			flite_START_TRACKING: "Start tracking changes",
			flite_STOP_TRACKING: "Stop tracking changes",
			flite_PENDING_CHANGES: "Your document contains some pending changes.\nPlease resolve them before turning off change tracking.",
			flite_HIDE_TRACKED: "Hide tracked changes",
			flite_SHOW_TRACKED: "Show tracked changes",
			flite_CHANGE_TYPE_ADDED: "added",
			flite_CHANGE_TYPE_DELETED: "deleted",
			flite_MONTHS: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
			flite_NOW: "now",
			flite_MINUTE_AGO: "1 minute ago",
			flite_MINUTES_AGO: "%Minutes minutes ago",
			flite_BY: "by",
			flite_ON: "on",
			flite_AT: "on"

		});
	});
}());