var koViewModel;

function ViewModel(tasks, comments, executives, subscribers, attach, images) {
    self = this;
    self.newTasks = ko.observableArray(tasks);
    self.newTaskComments = ko.observableArray(comments);
    self.taskExecutives = ko.observableArray(executives);
    self.taskSubscribers = ko.observableArray(subscribers);
    self.attachments = ko.observableArray(attach);
    self.attachmentsImages = ko.observableArray(images);

    self.getTaskComments = function (Id) {
        console.log(Id);
        console.log(ko.utils.arrayFilter(koViewModel.newTaskComments(), function (item) { return item.Id === Id }));
        return ko.utils.arrayFilter(koViewModel.newTaskComments(), function (item) { return item.Id === Id }).Comments;
    }

    self.getTaskExecutive = function (Id) {
        console.log(Id);
        console.log(ko.utils.arrayFilter(koViewModel.taskExecutives(), function (item) { return item.Id === Id }));
        return ko.utils.arrayFilter(koViewModel.taskExecutives(), function (item) { return item.Id === Id }).Executive;
    }

    self.addChild = function (obj) {
        self.newTasks.unshift(obj);
    };

    self.addNewAttachment = function (obj) {
        self.attachments.push(obj);
    };

    self.addNewAttachImage = function (obj) {
        self.attachmentsImages.push(obj);
    }

    self.addNewTaskComment = function (obj) {
        self.newTaskComments.push(obj);
        console.log(self.newTaskComments());
    }

    self.addTaskSubscriber = function (obj) {
        self.taskSubscribers.push(obj);
        console.log(self.taskSubscribers());
    }

    self.addTaskExecutive = function (obj) {
        self.taskExecutives.push(obj);
        console.log(self.taskExecutives());
        self.taskExecutives.valueHasMutated();
    }

    self.clearAttachments = function () {
        self.attachments = ko.observableArray([]);
    }

    self.clearAttachImages = function () {
        self.attachmentsImages = ko.observableArray([]);
    }

    koViewModel = self;
}

function getKnockoutTemplates() {
    $.ajax('/Templates/task.html', { async: false })
        .success(function (stream) {
            $('#templateTask').html(stream);
        }
    );

    $.ajax('/Templates/task_comment.html', { async: false })
        .success(function (stream) {
            $('#templateTaskComment').html(stream);
        }
    );
    $.ajax('/Templates/task_subscriber.html', { async: false })
       .success(function (stream) {
           $('#templateSubscriber').html(stream);
       }
   );
    $.ajax('/Templates/task_executive.html', { async: false })
      .success(function (stream) {
          $('#templateExecutive').html(stream);
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

function setNewSubcategories(self, selector) {
    var nameParam = $(self).find("option:selected").first().text();

    $.get('/Task/GetSubcategories', { name: nameParam })
        .done((list, htmlSubcats) =>
    {
        var htmlSubcats = '<option value=""></option>';
        for (var i = 0; i < list.length; i++) {
           htmlSubcats += '<option value="' + list[i].Id + '">' + list[i].Name + '</option>';
        }

        $(selector).find("select").html(htmlSubcats);
    });

}

function taskIndexInit() {
    if ($('#aside').height() < $('#main').height() + 50) {
        $('#aside').height($('#main').height() + 50);
        $('#aside').css("background", "#e7e7e7");
    }

    getKnockoutTemplates();

    koViewModel = new ViewModel([], [], [], [], [], []);
    ko.applyBindings(koViewModel);

    $(document.body).on('click', '.tasklikes', function () {
        var _taskId = $(this).parents(".task").find('.taskid')[0].value;
        var self = $(this);
        $.post('/Task/LikeTask', { taskId: _taskId }, function (data) {
            var count = parseInt(self.children().first().text());

            if (data == 'Unliked')
                self.children().first().text(count - 1);
            else {
                self.children().first().text(count + 1);
            }
        });
    });

    $(document.body).on('click', '.maininputcommentsend', function () {
        var _taskId = $(this).parents('.task').find('.taskid')[0].value;
        var _text = $(this).parent().children('.maininputcommentinput').val();
        var _count = $(this).parents('.task').find('.taskcount')[0];
        var postthis = $(this);

        $.post('/Task/WriteTaskComment', { taskId: _taskId, text: _text }, function (response) {
            console.log({ Id: _taskId, Comments: response });

            for (var i = 0; i < koViewModel.newTasks.length; i++) {
                if (koViewModel.newTasks[i].Id == _taskId) {
                    koViewModel.newTasks[i].Comments.splice(koViewModel.newTasks[i].Comments.length - 1, 0, response);
                }
            }

            koViewModel.addNewTaskComment({ Id: _taskId, Comments: response });

            var value = parseInt($(_count).text());

            $(_count).text(value + 1);
            $(_text).text('');
        });
    });

    $('#searchcategory').find("select").change(function () {
        setNewSubcategories(this, '#searchsubcategory');
    });

    $(document.body).on('click', '.taskcomments', function () {
        $(this).parents('.task').children('.taskcommentshid').slideToggle("slow", function () { });
        var postthis = $(this);
        var count = $(this).parents('.task').find('.taskcount')[0];
        var appenddiv = $(this).parents('.task').find('.taskcommentscontentc')[0];

        setInterval(function () {
            var _postid = postthis.parents('.task').find('.taskid').val();
            var _postcommentid = postthis.parents('.task').find('.taskcommentid:last').val();

            $.get('/Task/GetNewTaskComments', { taskId: _postid, taskCommentId: _postcommentid }).done(function (list) {
                if (list.length > 0) {
                    for (var i = 0; i < list.length; i++) {
                        koViewModel.addNewTaskComment({ Id: _postid, Comments: list[i] });
                    }

                    var value = parseInt($(count).text());

                    $(count).text(value + list.length);
                }
            });
        }, 3000);


    });

    $(document.body).on('click', '.tasksubscribe', function () {
        var str = parseInt(window.prompt("Введіть ціну"), 10);
        var _taskid = $(this).parents('.task').find('.taskid').val();
        var postthis = $(this);
        if (!isNaN(str) || !str.localeCompare("")) {

            $.post('/Task/SubscribeOnTask', { taskId: _taskid, taskPrice: str }, function (val) {
                if (val.Id == -1) {
                    alert("Ви не можете підписатись на це завдання");
                }
                else {
                    alert("Ви успішно підписались");

                    if (postthis.parents('.task').find('.tasksubscribers').children().length == 0) {
                        postthis.parents('.task').find('.tasksubscribers').append("<span class='tasksubscribersheader'>Підписники </span>");
                    }

                    koViewModel.addTaskSubscriber({ Id: _taskid, Subscriber: val });
                    postthis.parents('.tasksubscribe').attr('class', 'tasksubscribed');
                    postthis.attr('src', '/Images/solved.png');
                }
            });

        }
    });


    $("#maincreateheader").click(() => { $("#maininputmsg").slideToggle("slow"); });
}