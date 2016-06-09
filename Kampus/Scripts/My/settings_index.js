$(document).ready(function () {

    $("#addavatar").change(function () {
        readURL(this);
    });

    $("#changeavatarheader").click(function () {
        $("#changeavatar").slideToggle("slow");
    });
    $("#changepasswordheader").click(function () {
        $("#changepassword").slideToggle("slow");
    });
    $("#changestatusheader").click(function () {
        $("#changestatus").slideToggle("slow");
    });
    $("#changecityheader").click(function () {
        $("#changecity").slideToggle("slow");
    });
    $("#changestudentdataheader").click(function () {
        $("#changestudentdata").slideToggle("slow");
    });

    changeUniversityFaculties();


    notifications_init();
});