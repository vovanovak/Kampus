 var prevScrollHeightVal = 0;

        function resizeAside() {

            if ($('#aside').height() < document.body.scrollHeight - prevScrollHeightVal + 20) {
                $('#aside').height(document.body.scrollHeight - prevScrollHeightVal + 20);
                //$('#main').height(document.body.scrollHeight - prevScrollHeightVal + 20);
                $('#aside').css("background", "#e7e7e7");
                prevScrollHeightVal = $('#aside').height();
            }
        }


        $(document).ready(function() {

           
            resizeAside();

            $(window).resize(function() {
                resizeAside();
            });

            $(window).scroll(function() {
                resizeAside();
            });

            notifications_init();
        });