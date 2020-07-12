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
                // .remove("<i class='fa fa-lock'></i>")
                .append("<i class='fas fa-lock-open'></i>")
                .addClass("unlocked")
                .removeClass("locked");
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