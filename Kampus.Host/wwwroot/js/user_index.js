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
            return ko.utils.arrayFilter(koViewModel.newWallpostComments(), function (item) { return item.Id === Id }).Comments;
        }

        this.addWallpost = function (wallpost) {
            wallpost.Comments = ko.observableArray(ko.Comments);
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
                if (self.newWallposts()[i].Id == comment.WallPostId) {
                    found = true;

                    self.newWallposts()[i].Comments.push(comment);

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

    $.ajax('/Templates/wallpost.html', { async: false })
        .success(function (stream) {
            $('#templateWallpost').html(stream);
        }
    );

    $.ajax('/Templates/wallpost_comment.html', { async: false })
        .success(function (stream) {
            $('#templateWallpostComment').html(stream);
        }
    );

    $.ajax('/Templates/attachment_file.html', { async: false })
       .success(function (stream) {
           $('#templateAttachments').html(stream);
       }
   );

    $.ajax('/Templates/attachment_image.html', { async: false })
      .success(function (stream) {
          $('#templateImages').html(stream);
      }
    );

    ko.applyBindings(new ViewModel([], [], [], []));

    $("#maininputmsgfilesin").change(function () {
        attachments.push($("#maininputmsgfilesin").val());
    });

    $('#addsubscriber').click(function () {
        $.post('/User/AddSubscriber', {}, function (data) {
            var result = JSON.parse(data);
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

            $.get('/WallPost/GetNewWallPostComments', { postId: _postId, postCommentId: _postCommentId }).done(function (data) {
                var list = JSON.parse(data);

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
            
            if (data == 'Unliked')
                likes.children().first().text(count - 1);
            else 
                likes.children().first().text(count + 1);
            
        });
    });

    $(document.body).on('click', '.postdelete', function () {
        var _post = $(this).parents('.post');
        var _postid = _post.find('.postid')[0].value;

        console.log(_postid);

        $.post('/WallPost/DeleteWallpost', { postId: _postid }, function (res) {
            var result = JSON.parse(res).Result;
            if (result) {    
                _post.remove();
            }
        });
    });

    $(document.body).on('click', '.maininputcommentsend', function () {
        var _postId = $(this).parents('.post').find('.postid')[0].value;
        var _text = $(this).parent().children('.maininputcommentinput').val();
        var _count = $(this).parents('.post').find('.postcount')[0];
       
        $.post('/WallPost/WritePostComment', { postId: _postId, text: _text }, function (data) {
            koViewModel.addNewWallPostComment(JSON.parse(data));

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
                        FileName: files[files.length - 1].name,
                        Path: URL.createObjectURL(files[files.length - 1]).toLocaleString()
                    });
                }
                else {
                    koViewModel.addNewAttachment({
                        FileName: files[files.length - 1].name,
                        Path: URL.createObjectURL(files[files.length - 1]).toLocaleString()
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
        $.post('/WallPost/WriteWallPost', { text: _text }, function (json) {
            var data = JSON.parse(json);
            
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

        $.get('/WallPost/GetNewWallPosts', { lastPostId: _postId }).done(function (data) {
            var wallPosts = JSON.parse(data);

            for (var i = 0; i < wallPosts.length; i++) {
                koViewModel.addWallpost(wallPosts[i]);
            }
        });
    }, 3000);
});