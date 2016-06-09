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

        $("#nextstep").click(function () {

        });

        setInterval(function () {
            var _postid = $('.task').find('.taskid').val();
            var _postcommentid = $('.task').find('.taskcommentid:last').val();



            $.get('@Url.Action("GetNewTaskComments", "Home")', { taskid: _postid, taskcommentid: _postcommentid }).done(function (data) {
                var list = JSON.parse(data);



                if (list.length > 0) {
                    for (var i = 0; i < list.length; i++) {
                        $(appenddiv).append(' <div class="taskcomment"> \
                                        <div class="taskcommentcontent"> \
                                            <div class="taskcommentcontenttext"> '
                            + list[i].Content + '\
                                            </div> \
                                        </div> \
                                        <div class="taskcommenttriangle"></div> \
                                        <div class="taskcommentcreator">@@'+ list[i].User.Username + '</div> \
                                        <div><input class="taskcommentid" type="hidden" value="' + list[i].Id + '"/></div> \
                                    </div>');

                    }

                    var value = parseInt($(count).text());

                    $(count).text(value + list.length);
                }
            });
        }, 3000);

        //$(document.body).on('click', '.taskcomments', function () {
        //    $(this).parents('.task').children('.taskcommentshid').slideToggle("slow", function () { resizeAside(); });
        //    var postthis = $(this);
        //    var count = $(this).parents('.task').find('.taskcount')[0];
        //    var appenddiv = $(this).parents('.task').find('.taskcommentscontentc')[0];



        //    resizeAside();
        //});

        $(document.body).on('click', '.tasksubscribe2', function () {
            var str = parseInt(window.prompt("Введіть ціну"), 10);
            alert(isNaN(str));
            var _taskid = $(this).parents('.task').find('.taskid').val();
            var postthis = $(this);
            if (!isNaN(str) || !str.localeCompare("")) {

                $.post('@Url.Action("SubscribeOnTask", "Profile")', { taskid: _taskid, taskprice: str }, function () {
                    alert("Ви успішно підписались");
                    postthis.parents(".task").find(".tasksubscribers").append('<div class="tasksubscriber"> \
                                            <a style="text-decoration: none;" href="@Url.Action("Id", "Profile", new {Id = ViewBag.CurrentUser.Id})"><span class="tasksubscribersuser">@@@ViewBag.CurrentUser.Username ' + (_price == null) ? "" : _price + '</span></a> \
                    </div>');
                });

            }
            else {

            }
        });

        $(document.body).on('click', '.tasksubscribe', function () {
            var val1 = $(this).parents(".task").find('.taskid')[0].value;
            var postthis = $(this);
            $.post('@Url.Action("CheckTaskAsSolved", "Home")', { taskid: val1 }, function () {
                window.location.replace("");

            });
        });

        $(document.body).on('click', '.tasklikes', function () {
            var val1 = $(this).parents(".task").find('.taskid')[0].value;
            var postthis = $(this);
            $.post('@Url.Action("LikeTask", "Profile")', { taskid: val1 }, function (data) {
                var count = parseInt(postthis.children().first().text());

                if (data == 0)
                    postthis.children().first().text(count - 1);
                else {
                    postthis.children().first().text(count + 1);
                }
            });
        });

        $(document.body).on('click', '.maininputcommentsend', function () {
            var val1 = $(this).parents('.task').find('.taskid')[0].value;
            var text1 = $(this).parent().children('.maininputcommentinput').val();
            var count = $(this).parents('.task').find('.taskcount')[0];
            var postthis = $(this);

            $.post('@Url.Action("WriteTaskComment", "Profile")', { taskid: val1, text: text1 }, function (data) {


                postthis.parents('.taskcommentshid').find('.taskcommentscontentc').append(' <div class="taskcomment"> \
                                        <div class="taskcommentcontent"> \
                                            <div class="taskcommentcontenttext"> '
                    + text1 + '\
                                            </div> \
                                        </div> \
                                        <div class="taskcommenttriangle"></div> \
                                        <div class="taskcommentcreator">@@@user.Username</div> \
                                        <div><input class="taskcommentid" type="hidden" value="' + data + '"/></div> \
                                    </div>');


                var value = parseInt($(count).text());

                $(count).text(value + 1);
                resizeAside();
            });
        });

        $('#searchcategory').find("select").change(function () {

            var Name = "";
            $(this).find("option:selected").each(function () {
                Name = $(this).text();
            });


            $.get('@Url.Action("GetSubcategories", "Home")', { name: Name }).done(function (data) {
                // alert("Data Loaded: " + data);
                var string = "";


                var list = JSON.parse(data);


                for (var i = 0; i < list.length; i++) {
                    console.log(i);
                    console.log(list[i]);
                    console.log(list[i].Id);
                    console.log(list[i].Name);
                    string += '<option value="' + list[i].Id + '">' + list[i].Name + '</option>';
                }

                string += '<option value=""></option>';

                console.log(string);

                $('#searchsubcategory').find("select").html(string);
            });

        });
        $("#Category").change(function () {

            var Name = "";
            $("#Category option:selected").each(function () {
                Name = $(this).text();
            });


            $.get('@Url.Action("GetSubcategories", "Home")', { name: Name }).done(function (data) {
                // alert("Data Loaded: " + data);
                var string = "";

                alert(data);

                var list = JSON.parse(data);


                for (var i = 0; i < list.length; i++) {
                    console.log(i);
                    console.log(list[i]);
                    console.log(list[i].Id);
                    console.log(list[i].Name);
                    string += '<option value="' + list[i].Id + '">' + list[i].Name + '</option>';
                }

                console.log(string);

                $('#Subcategory').html(string);
            });

        });

        notifications_init();
    });