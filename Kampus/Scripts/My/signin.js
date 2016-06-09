$(document).ready(function () {
    $('#inputSignInBtn').click(function () {

        var _username = $('#inputSignInUsername').val();
        var _password = $('#inputSignInPassword').val();

        $.post('/SignIn/SignIn', { username: _username, password: _password })
            .done(function (data) {
                if (!data.localeCompare("Successful")) {
                    window.location.replace('/Home/Index');
                } else {
                    alert("Перевірте введені дані!");
                }
            });
    });
});