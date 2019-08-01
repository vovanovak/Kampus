function taskTaskInit() {

    $(document.body).on('click', '.tasksolve', function () {
        var val1 = $(this).parents(".task").find('.taskid')[0].value;
        var postthis = $(this);
        $.post('/Task/CheckTaskAsSolved', { taskid: val1 }, function () {
            window.location.replace("");
        });
    });
}