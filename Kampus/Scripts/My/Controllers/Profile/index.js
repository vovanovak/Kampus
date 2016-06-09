function resizeAside() {

        if ($('#aside').height() < $('#posts').height() + 50) {
            $('#aside').height($('#posts').height() + 50);
            $('#aside').css("background", "white");
        }

    }

    $(document).ready(function() {
        if ($('#aside').height() < $('#posts').height() + 50) {
            $('#aside').height($('#posts').height() + 50);
            $('#aside').css("background", "white");
        }

        $(window).resize(function() {
            resizeAside();
        });

        $(document.body).on('click', '.postcomments', function () {
            $(this).parents('.post').children('.postcommentshid').slideToggle("slow");
            var postthis = $(this);
            var count = $(this).parents('.post').find('.postcount')[0];
            var appenddiv = $(this).parents('.post').find('.postcommentscontent')[0];

            setInterval(function () {
                var _postid = postthis.parents('.post').find('.postid').val();
                var _postcommentid = postthis.parents('.post').find('.postcommentid:last').val();

               

                $.get('@Url.Action("GetNewWallPostComments", "Home")', { postid: _postid, postcommentid: _postcommentid }).done(function(data) {
                    var list = JSON.parse(data);


                    if (list.length > 0) {
                        for (var i = 0; i < list.length; i++) {
                            $(appenddiv).append(' <div class="postcomment"> \
                                        <div class="postcommentcontent"> \
                                            <div class="postcommentcontenttext"> '
                                + list[i].Content + '\
                                            </div> \
                                        </div> \
                                        <div class="postcommenttriangle"></div> \
                                        <div class="postcommentcreator">@@'+ list[i].User.Username + '</div> \
                                        <div><input class="postcommentid" type="hidden" value="' + list[i].Id + '"/></div> \
                                    </div>');
                            
                        }

                        var value = parseInt($(count).text());

                        $(count).text(value + list.length);
                    }
                });
            }, 3000);

            resizeAside();
        });




        $(document.body).on('click', '.postlikes', function() {
            var val1 = $(this).parents(1).find('.postid')[0].value;
            var postthis = $(this);
            $.post('@Url.Action("LikeWallPost", "Profile")', { postid: val1 }, function (data) {
                var count = parseInt(postthis.children().first().text());

                if (data == 0)
                    postthis.children().first().text(count - 1);
                else {
                    postthis.children().first().text(count + 1);
                }
            });
        });

        $(document.body).on('click', '.maininputcommentsend', function() {
            var val1 = $(this).parents('.post').find('.postid')[0].value;
            var text1 = $(this).parent().children('.maininputcommentinput').val();
            var count = $(this).parents('.post').find('.postcount')[0];
            var postthis = $(this);

            $.post('@Url.Action("WritePostComment", "Profile")', { postid: val1, text: text1 }, function (data) {
                postthis.parents('.postcommentshid').children('.postcommentscontent').append(' <div class="postcomment"> \
                                        <div class="postcommentcontent"> \
                                            <div class="postcommentcontenttext"> '
                    + text1 + '\
                                            </div> \
                                        </div> \
                                        <div class="postcommenttriangle"></div> \
                                        <div class="postcommentcreator">@@@ViewBag.CurrentUser.Username</div> \
                                        <div><input class="postcommentid" type="hidden" value="' + data +'"/></div> \
                                    </div>');


                    var value = parseInt($(count).text());

                    $(count).text(value + 1);
                    text1.text('');
                    resizeAside();
                });
            });

            $(document.body).on('click', '#maininputmsgcontentsend', function() {
                var _text = $('#maininputmsgcontentinput').val();
                $.post('@Url.Action("WriteWallPost", "Profile")', { text: _text }, function (data) {
                $('#postscontent').prepend('<div class="post"> \
                            <div class="postcontent"> \
                                <div class="postcontenttext"> \ ' + _text +
                    '</div> \
                            </div> \
                            <div class="postcommentshid"> \
                                <div class="postcommentscontent"> \
                                </div> \
                                <div class="maininputcomment"> \
                                    <div class="maininputcommentcontent"> \
                                        <textarea type="text" name="text" class="maininputcommentinput"></textarea> \
                                        <input type="button" value="Відправити" class="maininputcommentsend" /> \
                                    </div> \
                                    <div class="maininputcommenttriangle"></div> \
                                </div> \
                            </div> \
                            <div class="posttriangle"></div> \
                            <div class="postunder"> \
                                <div class="postcreator">@@@ViewBag.CurrentUser.Username</div> \
                                <div class="postcomments"> \
                                    <span class="postcount">0</span> <img src="../../Images/comments.png" align="middle" width="28" /> \
                                </div> \
                                <div class="postlikes"> \
                                    <span class="postcount">0</span> <img src="../../Images/like.png" width="28" /> \
                                </div> \
                            </div> \
                            <input type="hidden" class="postid" value="' + data + '" /> \
                        </div>');

                resizeAside();
            });
        });

        $('#addsubscriber').click(function () {
            $.post('@Url.Action("AddSubscriber", "Profile")', {}, function() {
                alert("Ви успішно підписались на користувача @@@ViewBag.UserProfile.Username");
            });
        });

        $('#sendmsg').click(function() {
            $.post('@Url.Action("WriteMessage", "Profile")', {username: '@ViewBag.UserProfile.Username'}, function () {
                window.location.replace("@Url.Action("Conversation", "Home", new { username = ViewBag.UserProfile.Username })");
            });
        });

       

        notifications_init();

    });