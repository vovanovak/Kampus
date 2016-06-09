$(document).ready(function() {
            $("#inputSignInBtn").click(function () {
                var _password = $("#inputSignInUsername").val();
                var _password1 = $("#inputSignInPassword").val();

                $.post("@Url.Action("TotalRecover")", { password: _password, password1: _password1 }, function(data) {
                    if (data == 1) {
                        alert("Ваш пароль відновлено");
                        window.location.href = "@Url.Action("Index", "SignIn")";
                    } else {
                        alert("Перевірте ваші дані");
                    }
                });
            });
        });
        
