$(document).ready(function() {
    $("#inputSignInBtn").click(function () {
        var _passwordNewPassword = $("#inputSignInUsername").val();
        var _passwordReentered = $("#inputSignInPassword").val();

        $.post("/Settings/TotalRecover", { password: _passwordNewPassword, password1: _passwordReentered }, function (data) {
            if (data == 1) {
                alert("Ваш пароль відновлено");
                window.location.href = "/SignIn/Index";
            } else {
                alert("Перевірте ваші дані");
            }
        });
    });
});