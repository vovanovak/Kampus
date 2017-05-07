function notifications_init() {
    setInterval(function () {
        $.get('/Notification/GetNewNotifications').done(function (data) {
            var list = JSON.parse(data);

            if ($('.notificationicon').attr('src').localeCompare('/Images/toolbar/not_new.png') != 0)
                $(".notifications").children().remove();

            if (list.length > 0) {
                for (var i = 0; i < list.length; i++) {
                    console.log(list[i].Sender.Avatar);
                    $('#notificationMenu').prepend('<li class=" notif unread"> \
                            <a class="notificationlink" href="' + list[i].Link + '"> \
                                <div class="imageblock" > \
                                    <img class="notifimage" src="' + list[i].Sender.Avatar + '" />   \
                                </div> \
                                <div class="messageblock"> \
                                    <div class="messageusername"> \
                                       @'+ list[i].Sender.Username + '        \
                                    </div> \
                                    <div class="message"> ' + list[i].Message + ' \
                                </div> \
                            </a> \
                        </li>');
                }

                if ($('.notifications').css('display') == 'none') {
                    $('.notificationicon').attr('src', '/Images/toolbar/not_new.png');
                }

                $.post('/Notification/ViewNotifications');
            }
        });
    }, 5000);

    $(".notificationicon").click(function () {
        if ($(".notifications").height() != 0) {
            $('.notificationicon').attr('src', '/Images/toolbar/not.png');  
        }
        if ($(".notifications > *").length > 0) {
            $(".notifications").animate({ height: "toggle" }, 500);
        }
    });
}

//notifications_init();