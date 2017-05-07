$(document).ready(function () {
    $('#inputSignInBtn').click(function () {

        var _username = $('#inputSignInUsername').val();
        var _password = $('#inputSignInPassword').val();

        $.post('/Main/SignIn', { username: _username, password: _password })
            .done(function (data) {
                if (!data.localeCompare("Successful")) {
                    window.location.replace('/User/Index');
                } else {
                    alert("Перевірте введені дані!");
                }
            });
    });
});