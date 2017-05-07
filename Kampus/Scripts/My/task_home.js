function taskHomeInit() {
    $(document.body).on('click', '.uncheckasmainexecutive', function () {
        var _taskId = $(this).parents(".task").find('.taskid')[0].value;

        var parent = $(this).parents(".taskexecutive");
        var postthis = $(this);

        $.post('/Task/RemoveTaskExecutive', { taskId: _taskId }, function (data) {
            console.log(data);
            if (JSON.parse(data))
                $(parent).remove(); 
        });
    });

    $('#searchcategory').find("select").change(function () {
        setNewSubcategories(this);
    });

    $(document.body).on('click', '.taskdeleteimg', function () {
        var val1 = $(this).parents(".task").find('.taskid')[0].value;
        var postthis = $(this);
        $.post('/Task/RemoveTask', { taskid: val1 }, function (data) {
            postthis.parents(".task").remove();
        });
    });

    $(document.body).on('click', '.tasksolve', function () {
        var val1 = $(this).parents(".task").find('.taskid')[0].value;
        var postthis = $(this);
        $.post('/Task/CheckTaskAsSolved', { taskid: val1 }, function (data) {
            var val = JSON.parse(data);
            postthis.find('img').attr("src", "/Images/solved.png");

            if (val.Id != -1) {
                window.location.replace("/Task/ExecutionReview?taskid=" + val1);
            }
        });
    });

    $(document.body).on('click', '.checkasmainexecutive', function () {
        var _taskid = $(this).parents(".task").find('.taskid')[0].value;
        var _username = $(this).parents(".tasksubscriber").find(".tasksubscribersuser").text();
        var parent = $(this).parents(".tasksubscriber");
        var div = $(this).parents(".taskcommentshid");
        var subscribers = $(div).children(".tasksubscribers");
        var postthis = $(this);
        $.post('/Task/CheckAsTaskMainExecutive', { taskid: _taskid, username: _username }, function (data) {
            if ($(parent).children('.tasksubscriber').length < 2)
                $(parent).parent().parent().remove();
            else
                $(parent).remove();

            var val = JSON.parse(data);
            
            koViewModel.addTaskExecutive({ Id: _taskid, Executive: val });
        });
    });


    $(".maininputaddfile").click(function () {
        $("#maininputmsgfilesin").trigger("click");
    });

    $(".maininputaddphoto").click(function () {
        $("#maininputmsgfilesin").trigger("click");
    });

    $("#maininputmsgfilesin").change(function () {
        var _formData = new FormData();
        var files = $("#maininputmsgfilesin").get(0).files;
        for (var i = 0; i < files.length; i++) {
            _formData.append("FileUpload", files[i]);
        }

        $.ajax({
            type: "POST",
            url: '/Task/UploadFileTask',
            data: _formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function () {
                var fileName = files[files.length - 1].name;
                var lastIndexOfDot = fileName.lastIndexOf('.');
                var extension = fileName.slice(lastIndexOfDot, fileName.length);

                if (extension.localeCompare('.jpg') == 0) {
                    koViewModel.addNewAttachImage({ FileName: files[files.length - 1].name, Path: URL.createObjectURL(files[files.length - 1]).toLocaleString() });
                }
                else {
                    koViewModel.addNewAttachment({ FileName: files[files.length - 1].name, Path: URL.createObjectURL(files[files.length - 1]).toLocaleString() });
                    console.log(files[files.length - 1].name);
                }
            },
            error: function (error) {
                alert("Error while uploading file!");
            }
        });
    });

    $('#category').find("select").change(function () {
        setNewSubcategories(this, '#subcategory');
    });

    $("#btnSubmitTask").click(function () {
        var _header = $("#inputSignInName").val();
        var _price = $("#inputSignInPrice").val();
        var _content = $("#inputSignInContent").val();
        var _category = $("#category > select").val();
        var _subcategory = $("#subcategory > select").val();


        $.post('/Task/CreateTask', {
            header: _header,
            price: _price,
            content: _content,
            category: _category,
            subcategory: _subcategory
        },
        function (json) {
            var data = JSON.parse(json);
            console.log(data);
            koViewModel.addChild(data);
            koViewModel.clearAttachments();
            koViewModel.clearAttachImages();
            $('#maininputmsgfilesin').val([]);
            console.log($('#maininputmsgfilesin').val());

            $("#inputSignInName").val("");
            $("#inputSignInPrice").val("");
            $("#inputSignInContent").val("");

            $('#taskFiles').find('.filerefcont').remove();
            $('#taskImages').find('.attachImg').remove();
        });
    });
}