// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Navigation

function openNav() {
    document.getElementById("mySidenav").style.width = "80%";
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}

$(document).ready(function () {
    /*
    *  ------------------------------------------------ Navigation Scripts  ----------------------------------------------------------------
    */
    $('.border-nav').mouseenter(function () {
        $(this).css("border", "1px solid blue");
    });
    $('.border-nav').mouseleave(function () {
        $(this).css("border", "1px solid grey");
    });

    /*
     *  ------------------------------------------------ Landing Page Scripts ----------------------------------------------------------------
     */

    // on page load we pre-select the checbox depending on if its opened or closed
    $('#open-door').prop('checked', true);
    $('#close-door').prop('checked', false);

    // Lock Icon changer
    // allows the changed icon to show unlocked icon change before the server applies the change from the delayed refresh
    $("body > main > div.access > div > form > div:nth-child(3) > button").click(function () {
        // only when it shows locked class
        if ($(this).hasClass("locked")) {
            // we remove the class and add the other class to this button
            $('body > main > div.access > div > form > div:nth-child(3) > button > svg').remove();
            $(this)
                .append("<i class='fas fa-lock-open'></i>")
                .addClass("unlocked")
                .removeClass("locked");
        }
    });

    // when we click the open button(given user is not inside and door), first show remove the hidden attr from the scanning, so it will show scanning
    // after 5 seconds it will remove scanning
    var btn = $("#submit-icon");

    btn.click(function () {
        var scan = $('body > main > div.access > div > div.door-status.temp-scan.hidden').removeClass('hidden');
        setTimeout(function () {
            scan.addClass('hidden');
        }, 5000);
    });

    /*
    *  ------------------------------------------------ Geolocation Scripts ----------------------------------------------------------------
    */

    // Modal animation for map and camera
    // Get the modal
    var mapModal = document.getElementById("map");
    var camModal = document.getElementById("camera");

    // Get the button that opens the modal
    var mapBtn = document.getElementById("map-button");
    var camBtn = document.getElementById("camera-button");

    // Get the <span> element that closes the modal (TODO: may need to modify due to being two)
    var span = document.getElementsByClassName("close")[0];
    var span2 = document.getElementsByClassName("close")[1];

    // When the user clicks the button, open the modal 
    mapBtn.onclick = function () {
        mapModal.style.display = "block";
    }
    camBtn.onclick = function () {
        camModal.style.display = "block";
    }

    // When the user clicks on <span> (x), close the modal
    span.onclick = function () {
        mapModal.style.display = "none";
    }
    // When the user clicks on <span> (x), close the modal
    span2.onclick = function () {
        camModal.style.display = "none";
    }

    // When the user clicks anywhere outside of the modal, close it
    window.onclick = function (event) {
        if (event.target == mapModal) {
            mapModal.style.display = "none";
        }
        if (event.target == camModal) {
            mapModal.style.display = "none";
            camModal.style.display = "none";
        }
    }

    // Option to fill in location services and pass users current location data to the server
    var x = document.getElementById("user-location");
    var lat = "";
    var long = "";

    // get the lat1 and lon1 for the current user 
    function getLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.watchPosition(showPosition);
            console.log(long);
        } else {
            alert("Geolocation is not supported by this browser.");
        }
    }

    function showPosition(position) {
        x.innerHTML = "Latitude: " + position.coords.latitude +
            "<br>Longitude: " + position.coords.longitude;
        lat = position.coords.latitude;
        long = position.coords.longitude;
        // Calculate the difference between the gym location
        function getDistanceFromLatLonInKm(lat1, lon1, lat2, lon2) {
            var R = 6371000; // Radius of the earth in m
            var dLat = deg2rad(lat2 - lat1);  // deg2rad below
            var dLon = deg2rad(lon2 - lon1);
            var a =
                Math.sin(dLat / 2) * Math.sin(dLat / 2) +
                Math.cos(deg2rad(lat1)) * Math.cos(deg2rad(lat2)) *
                Math.sin(dLon / 2) * Math.sin(dLon / 2)
                ;
            var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        function deg2rad(deg) {
            return deg * (Math.PI / 180)
        }

        // sets the default gym to values on hidden fields
        var defaultGymLat = $('#dlat').html();
        var defaultGymLong = $('#dlong').html();

        var differenceBetweenUser = getDistanceFromLatLonInKm(lat, long, defaultGymLat, defaultGymLong).toFixed(1);

        if (differenceBetweenUser < 40) {
            $('#user-location').prop('checked', true);
        }
        else {
            console.log("false");
            $('#user-location').prop('checked', false);
        }
    }
        getLocation();

    // Trial HERE Maps
    // Instantiate a map and platform object:
    var platform = new H.service.Platform({
        apikey: `USVLHLFNdt2wR2V9WyYvCy4fwsof7enWCDq-xQn2rK8`
    });

    // Get an instance of the geocoding service:
    var service = platform.getSearchService();


    // Call the geocode method with the geocoding parameters,
    // the callback and an error callback function (called if a
    // communication error occurs):
    service.geocode({
        q: '44 Brenda Street'
    }, (result) => {
        // Add a marker for each location found
            result.items.forEach((item) => {
                alert(item.position.coords.latitude);
            map.addObject(new H.map.Marker(item.position));
        });
    }, alert);




    // TODO: Progress Bar Depelete each time the open door button is pressed.
    //$(function () {
    //    var current_progress = 100;
    //    var interval = setInterval(function () {
    //        current_progress -= 10;
    //        $("#dynamic")
    //            .css("width", current_progress + "%")
    //            .attr("aria-valuenow", current_progress)
    //            .text(current_progress + "% Complete");
    //        if (current_progress <= 0)
    //            clearInterval(interval);
    //    }, 500);
    //});
    //<h3>Dynamic Progress Bar</h3>
    //    <p>Running progress bar from 0% to 100% in 10 seconds</p>
    //    <div class="progress">
    //        <div id="dynamic" class="progress-bar progress-bar-success progress-bar-striped active" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%">
    //            <span id="current-progress"></span>
    //        </div>
    //    </div>
});

