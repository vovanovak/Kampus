$(document).ready(function() {
    $("#inputSignInBtn").click(function () {
        var _username = $("#inputSignInUsername").val();
        var _email = $("#inputSignInPassword").val();

        $.post("/Settings/RecoverPassword", {username: _username, email: _email}, function() {
            alert("На ваш email надіслано повідомлення, перейдіть по посиланню, щоб відновити пароль");
        });
    }); 
});