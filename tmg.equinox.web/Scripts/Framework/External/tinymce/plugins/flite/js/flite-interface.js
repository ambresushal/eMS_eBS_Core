/*!
 * Source version: 1.1.6 
 * Copyright (C) 2018 LoopIndex - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of the LoopIndex Comments CKEditor plugin license.
 * Attributions
 * Rangy library:
 * Copyright 2018, Tim Down
 * https://github.com/timdown/rangy
 * inspired by work by Matthew DeLambo, The New York Times, CMS Group
 * https://github.com/NYTimes/ice
 * Opentip adapter
 * Copyright (c) 2018, Matias Meno  
 * http://www.opentip.org
 * Mutation summary
 * Copyright (c) 2018 Rafael Weinstein
 * https://github.com/rafaelw/mutation-summary
 * You should have received a copy of the LoopIndex Comments CKEditor plugin license with
 * this file. If not, please write to: contact@loopindex.com, or visit http://www.loopindex.com
 * written by (David *)Frenkiel (https://github.com/imdfl) 
 * **/
(function (global, $) {
	$.extend(true, global, { 
		FLITE: {
			Events: {
				INIT: "flite:init",
				ACCEPT: "flite:accept",
				REJECT: "flite:reject",
				SHOW_HIDE: "flite:showHide",
				TRACKING: "flite:tracking",
				CHANGE: "flite:change",
				HOVER_IN: "flite:hover-in",
				HOVER_OUT: "flite:hover-out"
			},

			Commands: {
				TOGGLE_TRACKING: "flite-toggletracking",
				TOGGLE_SHOW: "flite-toggleshow",
				ACCEPT_ALL: "flite-acceptall",
				REJECT_ALL: "flite-rejectall",
				ACCEPT_ONE: "flite-acceptone",
				REJECT_ONE: "flite-rejectone",
				TOGGLE_TOOLTIPS: "flite-toggletooltips"
			}
		}
	});
}(window, window.jQuery));
