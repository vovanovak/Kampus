var koViewModel;
var attachments = new Array();

$(document).ready(function () {
    function ViewModel(wallposts, comments, attach, images) {
        self = this;

        this.newWallposts = ko.observableArray(wallposts);
        this.newWallpostComments = ko.observableArray(comments);
        this.attachments = ko.observableArray(attach);
        this.attachmentsImages = ko.observableArray(images);

        this.getWallpostComments = function(Id){
            return ko.utils.arrayFilter(koViewModel.newWallpostComments(), function (item) { return item.Id === Id }).comments;
        }

        this.addWallpost = function (wallpost) {
            wallpost.comments = ko.observableArray(ko.comments);
            self.newWallposts().unshift(wallpost);
            self.newWallposts.valueHasMutated();
        };

        this.addNewAttachment = function (attachment) {
            self.attachments.push(attachment);
        };

        this.addNewAttachImage = function (image) {
            self.attachmentsImages.push(image);
        }

        this.addNewWallPostComment = function (comment) {
            var found = false;

            for (var i = 0; i < self.newWallposts().length; i++) {
                if (self.newWallposts()[i].Id == comment.wallPostId) {
                    found = true;

                    self.newWallposts()[i].comments.push(comment);

                    $(".postid[value='" + self.newWallposts()[i].Id + "']").parents('.post')
                        .find('.postcommentshid').css({ display: "block" });
                    self.newWallposts.valueHasMutated();
                    break;
                }
            }
           
            if (!found) {
                self.newWallpostComments().push(comment);
                self.newWallpostComments.valueHasMutated();
            }

        }
        this.clearAttachments = function () {
            self.attachments = ko.observableArray([]);
        }
        this.clearAttachImages = function () {
            self.attachmentsImages = ko.observableArray([]);
        }
        koViewModel = this;
    }

    notifications_init();

    $.get('/templates/wallpost.html', { async: false })
        .success(function (stream) {
            $('#templateWallpost').html(stream);
        }
    );

    $.get('/templates/wallpost_comment.html', { async: false })
        .success(function (stream) {
            $('#templateWallpostComment').html(stream);
        }
    );

    $.get('/templates/attachment_file.html', { async: false })
       .success(function (stream) {
           $('#templateAttachments').html(stream);
       }
   );

    $.get('/templates/attachment_image.html', { async: false })
      .success(function (stream) {
          $('#templateImages').html(stream);
      }
    );

    ko.applyBindings(new ViewModel([], [], [], []));

    $("#maininputmsgfilesin").change(function () {
        attachments.push($("#maininputmsgfilesin").val());
    });

    $('#addsubscriber').click(function () {
        $.post('/User/AddSubscriber', {}, function (result) {
            if (result == true)
                alert("Ви успішно підписались на користувача @@" + profileUsername);
            else
                alert("Ви не можете підписатись на цього користувача");
        });
    });

    $('#sendmsg').click(function () {
        $.post('/User/WriteMessage', { username: profileUsername }, function () {
            window.location.replace("/Home/Conversation?username=" + profileUsername);
        });
    });

    $(".maininputaddfile").click(function () {
        $("#maininputmsgfilesin").trigger("click");
    });

    $(".maininputaddphoto").click(function () {
        $("#maininputmsgfilesin").trigger("click");
    });

    $(document.body).on('click', '.postcomments', function () {
        $(this).parents('.post').children('.postcommentshid').slideToggle("slow");
        var comments = $(this);
        var count = $(this).parents('.post').find('.postcount')[0];
        
        setInterval(function () {
            var _postId = comments.parents('.post').find('.postid').val();
            var _postCommentId = comments.parents('.post').find('.postcommentid:last').val();

            $.get('/WallPost/GetNewWallPostComments', { postId: _postId, postCommentId: _postCommentId }).done(function (list) {
                if (list.length > 0) {
                    for (var i = 0; i < list.length; i++) {
                        koViewModel.addNewWallPostComment(list[i]);
                    }

                    var value = parseInt($(count).text());

                    $(count).text(value + list.length);
                }
            });
        }, 3000);
    });

    $(document.body).on('click', '.postlikes', function () {
        var _postId = $(this).parents('.post').find('.postid')[0].value;
        var likes = $(this);
        $.post('/WallPost/LikeWallPost', { postId: _postId }, function (data) {
            var count = parseInt(likes.children().first().text());

            if (data == 0)
                likes.children().first().text(count - 1);
            else 
                likes.children().first().text(count + 1);
        });
    });

    $(document.body).on('click', '.postdelete', function () {
        var _post = $(this).parents('.post');
        var _postid = _post.find('.postid')[0].value;

        console.log(_postid);

        $.post('/WallPost/DeleteWallpost', { postId: _postid }, function (response) {
            var result = response.result;
            if (result) {    
                _post.remove();
            }
        });
    });

    $(document.body).on('click', '.maininputcommentsend', function () {
        var _postId = $(this).parents('.post').find('.postid')[0].value;
        var _text = $(this).parent().children('.maininputcommentinput').val();
        var _count = $(this).parents('.post').find('.postcount')[0];
       
        $.post('/WallPost/WritePostComment', { postId: _postId, text: _text }, function (response) {
            koViewModel.addNewWallPostComment(response);

            var value = parseInt($(_count).text());

            $(_count).text(value + 1);
            $(_text).text('');
        });
    });

    $("#maininputmsgfilesin").change(function () {
        var _formData = new FormData();
        var files = $("#maininputmsgfilesin").get(0).files;
        for (var i = 0; i < files.length; i++) {
            _formData.append("FileUpload", files[i]);
        }

        $.ajax({
            type: "POST",
            url: '/WallPost/UploadFileWallpost',
            data: _formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function () {
                var fileName = files[files.length - 1].name;
                var lastIndexOfDot = fileName.lastIndexOf('.');
                var extension = fileName.slice(lastIndexOfDot, fileName.length);

                if (extension.localeCompare('.jpg') == 0 || extension.localeCompare('.png') == 0) {
                    koViewModel.addNewAttachImage({
                        fileName: files[files.length - 1].name,
                        path: URL.createObjectURL(files[files.length - 1]).toLocaleString()
                    });
                }
                else {
                    koViewModel.addNewAttachment({
                        fileName: files[files.length - 1].name,
                        path: URL.createObjectURL(files[files.length - 1]).toLocaleString()
                    });
                }
            },
            error: function (error) {
                alert("Error while uploading file!");
            }
        });

    });
    $(document.body).on('click', '#maininputmsgcontentsend', function () {
        var _text = $('#maininputmsgcontentinput').val();
        $.post('/WallPost/WriteWallPost', { text: _text }, function (data) {
            koViewModel.addWallpost(data);

            koViewModel.clearAttachments();
            koViewModel.clearAttachImages();

            $('#maininputmsgfilesin').val([]);
            
            $('#maininputmsgcontentinput').val("");
            $('#maininputmsgcontent').find('.filerefcont').remove();
            $('#maininputmsgcontent').find('.maininputmsgimages').children().remove();

        });
    });

    setInterval(() => {
        var _postId = $('.post').first().find('.postid').val();

        if (_postId == undefined)
            _postId = -1;

        $.get('/WallPost/GetNewWallPosts', { lastPostId: _postId }).done(function (wallPosts) {
            for (var i = 0; i < wallPosts.length; i++) {
                koViewModel.addWallpost(wallPosts[i]);
            }
        });
    }, 3000);
});