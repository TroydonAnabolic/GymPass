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
});