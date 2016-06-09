 function resizeAside() {

        if ($('#aside').height() < $('#main').height() + 50) {
            $('#aside').height($('#main').height() + 50);
            $('#aside').css("background", "#e7e7e7");
        }

    }


    $(document).ready(function () {

        if ($('#aside').height() < $('#main').height() + 50) {
            $('#aside').height($('#main').height() + 50);
            $('#aside').css("background", "#e7e7e7");
        }

        $(window).resize(function () {
            resizeAside();
        });

        $("#maincreateheader").click(function () {
            $("#maininputmsg").slideToggle("slow");
            resizeAside();
        });

        notifications_init();
    });