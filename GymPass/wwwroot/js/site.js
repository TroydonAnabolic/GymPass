﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Navigation

function openNav() {
    document.getElementById("mySidenav").style.width = "80%";
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}

// webcam
function take_snapshot() {
    // take snapshot and get image data  
    Webcam.snap(function (data_uri) {
        // display results in page  
        document.getElementById('results').innerHTML =
            '<img id="base64image" src="' +
            data_uri +
            '"/>';

        // try javascript
        var request = new XMLHttpRequest();

        request.open("POST", "/upload/url", true);

        var data = new FormData();
        var dataURI = snapshot.firstChild.getAttribute("src");
        var imageData = dataURItoBlob(dataURI);
        data.append("image", imageData, "myimage");
        request.send(data);

        // upload webcam api
        Webcam.upload(data_uri,
            '/Facilities/Capture',
            function (code, text) {
                // since to above is not sending to controller, try ajax

                console.log('Photo Captured');  
            });

        $.ajax({
            type: "POST",
            url: '@Url.Action("Capture", "Facilities")', // trying to get: https://localhost:44314/Home/Index/10
            // may need to convert data_uri to string here
            data: JSON.stringify({ webcam: data_uri }),
            dataType: "text",
            success: function (msg) {
                console.log("Success");
                console.log("data_uri");
                console.log(data_uri);
                //  console.log("data"); not defined error
                //console.log(data);
                //console.log("webcam");
                //console.log(webcam);
                //console.log("stringdata");
                //console.log(stringData);


            },
            error: function (req, status, error) {
                console.log("Error");
                console.log("data_uri");
                console.log(data_uri);
                  console.log("data"); //not defined error
               // console.log("webcam");
               // console.log(webcam);
               // console.log("stringdata");
               // console.log(stringData);
            }
        });

    });
}  

// submit form for check estimated total with divs instead of submit button
function submitForm() {
    document.getElementById('est-form').submit();
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

    // ------------------ Webcam Script -----------------------
    // set the camera and attach it
    //Webcam.set({
    //    width: 240,
    //    height: 240,
    //    image_format: 'jpeg',
    //    jpeg_quality: 90
    //});
    //Webcam.attach('#my_camera');  



    // ---------- Submit main button script ---------------------
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

    $(" header").hide();
    // Change between estimated total and actual total
    // When we click user total, show est total
    $("#total-in-gym-icon").click(function () {
        $(this).css("display", "none");
        $("#est-total-in-gym-icon")
            .css("display", "block");
    });
    // other way around
    $("#est-total-in-gym-icon").click(function () {
        $(this).css("display", "none");
        $("#total-in-gym-icon")
            .css("display", "block");
    });

    /*
    *  ------------------------------------------------ Geolocation Scripts ----------------------------------------------------------------
    */
    // allows lat and long to be globally accessible within anything requiring it to be accessed.
    navigator.geolocation.watchPosition(function (position) {
        var pos = {
            lat: position.coords.latitude,
            lng: position.coords.longitude
        };

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
            initializeMap()
        }
        camBtn.onclick = function () {
            camModal.style.display = "block";
        }

        // When the user clicks on <span> (x), close the modal
        span.onclick = function () {
            mapModal.style.display = "none";
            terminateMap();
        }
        // When the user clicks on <span> (x), close the modal
        span2.onclick = function () {
            camModal.style.display = "none";
        }

        // When the user clicks anywhere outside of the modal, close it
        window.onclick = function (event) {
            if (event.target == mapModal) {
                mapModal.style.display = "none";
                terminateMap();
            }
            if (event.target == camModal) {
                mapModal.style.display = "none";
                camModal.style.display = "none";
            }
        }

        // sets the default gym to values on hidden elements
        var defaultGymLat = $('#dlat').html();
        var defaultGymLong = $('#dlong').html();

        // Calculate the difference between the gym location in metres
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

        var differenceBetweenUser = getDistanceFromLatLonInKm(pos.lat, pos.lng, defaultGymLat, defaultGymLong).toFixed(1);

        if (differenceBetweenUser < 40) {
            $('#user-location').prop('checked', true);
        }
        else {
            console.log("false");
            $('#user-location').prop('checked', false);
        }

        //Step 1: initializeMap communication with the platform
        async function initializeMap() {
            var platform = new H.service.Platform({
                apikey: 'USVLHLFNdt2wR2V9WyYvCy4fwsof7enWCDq-xQn2rK8'
            });
            var defaultLayers = platform.createDefaultLayers();
            //Step 2: initializeMap a map - this map is centered over Europe
            var map = new H.Map(document.getElementById('mapContainer'),
                defaultLayers.vector.normal.map,
                {
                    center: { lat: defaultGymLat, lng: defaultGymLong },
                    zoom: 15,
                    pixelRatio: window.devicePixelRatio || 1
                }
            );
            // This adds a resize listener to make sure that the map occupies the whole container
            window.addEventListener('resize', () => map.getViewPort().resize());
            //Step 3: make the map interactive
            // MapEvents enables the event system
            // Behavior implements default interactions for pan/zoom (also on mobile touch environments)
            var behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));

            // Create the default UI components
            var ui = H.ui.UI.createDefault(map, defaultLayers);

            var LocationOfGym = { lat: defaultGymLat, lng: defaultGymLong };
            //// Create a marker icon from an image URL: 
            var pngIcon = new H.map.Icon('/images/gym-map.png', { size: { w: 56, h: 56 } });
            var myIcon = new H.map.Icon('/images/your-location.png', { size: { w: 56, h: 56 } });
            //// Create a marker using the previously instantiated icon:
            var marker = new H.map.Marker(LocationOfGym, { icon: pngIcon });
            // show your location
            // Try HTML5 geolocation.
            var LocationOfYou = { lat: pos.lat, lng: pos.lng };
            var myMarker = new H.map.Marker(LocationOfYou, { icon: myIcon });
            //// Add the marker to the map:
            map.addObject(myMarker);
            map.addObject(marker);
            // Optionally, 
            //Show the gym in the center of the map
            map.setCenter(LocationOfGym);
            //Zooming so that the marker can be clearly visible
            map.setZoom(15);
        }

        // function to remove all elements inside a map to avoid duplicate map showing.
        function terminateMap() {
            $('#mapContainer > div').remove();
        }
    });

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

