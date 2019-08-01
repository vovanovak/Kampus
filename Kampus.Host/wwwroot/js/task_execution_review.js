$(document).ready(function () {

    if ($('#aside').height() < $('#main').height() + 50) {
        $('#aside').height($('#main').height() + 50);
        $('#aside').css("background", "#e7e7e7");
    }

    $("#maincreateheader").click(function () {
        $("#maininputmsg").slideToggle("slow");
    });
});