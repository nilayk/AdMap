
var map;

var addBubbles = function (bubbles) {

	var projection = map.projection;

	var options = map.options.bubbleConfig;

	var bubbleContainer = map.svg.append('g').attr('class', 'bubbles');
	bubbleContainer
		.selectAll('circle.bubble')
		.data(bubbles)
		.enter()
		.append('svg:circle')
		.attr('cx', function (datum) {
			return projection([datum.longitude, datum.latitude])[0];
		})
		.attr('cy', function (datum, index) {
			return projection([datum.longitude, datum.latitude])[1];
		})
		.style('fill', function (datum) {
			return map.options.bubbleConfig.highlightFillColor;
		})
		.style('stroke', function (datum) {
			return map.options.borderColor;
		})
		.attr('class', 'bubble')
		.style('stroke-width', map.options.borderWidth)
		.attr('fill-opacity', map.options.fillOpacity)
		.attr('r', 0)
		.transition()
		.duration(400)
		.attr('r', function (datum) {
			return datum.radius;
		});
};

var updateMap = function (ad) {

	addBubbles([{
		radius: 5,
		latitude: ad.Latitude,
		longitude: ad.Longitude,
		// city: data[i].city,
		fillKey: '#FFFFFF'
	}]);

	$('.bubbles:not(.animation-applied)').animate({
		opacity: 0
	}, {
		duration: 10000,
		start: function () {
			this.classList.add("animation-applied");
		},
		complete: function () {
			this.remove();
		}
	});
};

$(document).ready(function () {

	// var conn;
	map = new Datamap({
		element: document.getElementById('container'),
		scope: 'world', //currently supports 'usa' and 'world', however with custom map data you can specify your own
		projection: 'equirectangular', //style of projection to be used. try "mercator"
		done: function () { }, //callback when the map is done drawing
		fills: {
			defaultFill: '#423D59' //the keys in this object map to the "fillKey" of [data] or [bubbles]
		},
		geographyConfig: {
			hideAntarctica: false,
			popupOnHover: false, //disable the popup while hovering
			highlightOnHover: false,
			borderColor: '#FCB816'
		},
		bubbleConfig: {
			borderWidth: 0,
			borderColor: '#FCB816',
			popupOnHover: true,
			fillOpacity: 0.75,
			animate: true,
			highlightOnHover: true,
			highlightBorderColor: '#C02533',
			highlightFillColor: '#ff3300',
			highlightBorderWidth: 1,
			highlightFillOpacity: 0.85
		},
		fillOpacity: 1
	});

	/* conn = io.connect(connAddr, {
		'force new connection': true
	});

	conn.emit('subscribe', 'userloc');*/

	$(document).ready(function () {

		var adMapUpdater = $.connection.adMapUpdaterHub; // the generated client-side hub proxy

		var init = function () {
			adMapUpdater.server.initAdMap().done(function (ads) {
				for (var i = 0; i < ads.length; i++) {
					updateMap(ads[i]);
				}
			});
		};

		 // Client-side hub method that the server will call
		adMapUpdater.client.updateAdMap = function (ad) {
			updateMap(ad);
		};

		// Start the connection
		 $.connection.hub.start().done(init);
		//.done(function () {
		//	$('#sendmessage').click(function () {
		//		// Call the Send method on the hub. 
		//		chat.server.send($('#displayname').val(), $('#message').val());
		//		// Clear text box and reset focus for next comment. 
		//		$('#message').val('').focus();
		//	});
		//});
	});

});