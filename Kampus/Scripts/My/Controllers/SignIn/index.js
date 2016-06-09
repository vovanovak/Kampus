 $(document).ready(function() {
            $('#inputSignInBtn').click(function() {

                var _username = $('#inputSignInUsername').val();
                var _password = $('#inputSignInPassword').val();
                
                $.post('@Url.Action("SignIn", "SignIn")', { username: _username, password: _password })
                    .done(function(data) {
                        if (!data.localeCompare("Successful")) {
                            window.location.replace('@Url.Action("Index", "Home")');
                        } else {
                            alert("Перевірте введені дані!");
                        }
                });
            });
        });