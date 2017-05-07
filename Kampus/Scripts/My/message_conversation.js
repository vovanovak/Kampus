function ViewModel(messages, attach, images) {
    self = this;

    self.newMessages = ko.observableArray(messages);
    self.attachments = ko.observableArray(attach);
    self.attachmentsImages = ko.observableArray(images);

    self.addMessage = function (obj) {
        self.newMessages.push(obj);
    };
    self.addNewAttachment = function (obj) {
        self.attachments.push(obj);
    };
    self.addNewAttachImage = function (obj) {
        self.attachmentsImages.push(obj);
    }
    self.clearAttachments = function () {
        self.attachments = ko.observableArray([]);
    }
    self.clearAttachImages = function () {
        self.attachmentsImages = ko.observableArray([]);
    }
    koViewModel = self;
}

function cssOptimizations() {
    $('#toolbar').css("position", "fixed");
    $('#toolbar').css("z-index", "1000");

    $("#maininputmsgfilesin").change(function () {
        _attachments.push($("#maininputmsgfilesin").val());
    });

    $('#aside').height($('#mainmessages').height() + 50);
    $('#aside').css("background", "#e7e7e7");

    $(window).resize(function () {
        $('#aside').height($('#mainmessages').height() + 50);
        $('#aside').css("background", "#e7e7e7");
    });

    $(".maininputaddfile").click(function () {
        $("#maininputmsgfilesin").trigger("click");
    });

    $(".maininputaddphoto").click(function () {
        $("#maininputmsgfilesin").trigger("click");
    });

    $('html, body').scrollTop($(document).height());
}

function loadMessageTemplates() {
    $.ajax('/Templates/message.html', { async: false })
        .success(function (stream) {
            $('#templateMessages').html(stream);
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
}

var _attachments = new Array();
var koViewModel;

$(document).ready(function () {  
    cssOptimizations();
    loadMessageTemplates();

    ko.applyBindings(new ViewModel([], [], []));

    $("#maininputmsgfilesin").change(function () {
        var _formData = new FormData();
        var files = $("#maininputmsgfilesin").get(0).files;
        for (var i = 0; i < files.length; i++) {
            _formData.append("FileUpload", files[i]);
        }

        $.ajax({
            type: "POST",
            url: '/Message/UploadFileMessage',
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


    $('.maininputmsgcontentsend').click(function () {
        var _receiver = $('#receiverid').val();
        var _text = $('.maininputmsgcontentinput').val();

        $.post('/Message/WriteMessage', { receiverId: _receiver, text: _text }, function (json) {
            var data = JSON.parse(json);

            console.log(data);

            koViewModel.addMessage(data);
            koViewModel.clearAttachments();
            koViewModel.clearAttachImages();

            $('.maininputmsgcontentinput').val("");
            $('.maininputmsg').find('.filerefcont').remove();
            $('.maininputmsg').find('.maininputmsgimages').children().remove();


            var links = $('.asideuserdetails').find('a');
            for (var i = 0; i < links.length; i++) {
                if (links[i].text == data.Receiver) {
                    $(links[i]).parents('.asideuserdetails').find('.asideusermessage').text(data.Content);
                }
            }
        });
    });

    setInterval(function () {
        var sender = $('#senderid').val();
        var receiver = $('#receiverid').val();
        var lstmsgid = $('.messageid').last().val();

        $.get('/Message/GetNewMessages',
            {
                senderId: sender,
                receiverId: receiver,
                lastMsgId: lstmsgid
            }).done(function (json) {
                var data = JSON.parse(json);

                for (var i = 0; i < data.length; i++) {
                    if (lstmsgid != data[i].Id) {
                        koViewModel.addMessage(data[i]);
                    }
                }

                $('#aside').height($('#mainmessages').height() + 50);
                $('#aside').css("background", "#e7e7e7");
            });
    }, 3000);
});