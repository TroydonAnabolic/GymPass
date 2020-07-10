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
    //$('div.sidenav a:nth-child(1), div.sidenav a:nth-child(2), div.sidenav a:nth-child(3)').css("border", "1px solid grey");

    $('.border-nav').mouseenter(function () {
        $(this).css("border", "1px solid blue");
    });
    $('.border-nav').mouseleave(function () {
        $(this).css("border", "1px solid grey");
    });

});