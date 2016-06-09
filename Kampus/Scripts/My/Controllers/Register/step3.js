function readURL(input) {

        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('.imgavatar').attr('src', e.target.result);
            }

            reader.readAsDataURL(input.files[0]);
        }
    }

    $(document).ready(function () {
        $("#addavatar").change(function () {
            readURL(this);
        });

        $("#inputSignInUsername").change(function () {
            $.get('@Url.Action("ContainsUserWithSuchUsername")', { username: $(this).val() }, function (data) {
                if (data.localeCompare('contains'))
                {
                    $("#inputval").attr('src', '@Url.Content("../../Images/yes.png")');
                    
                }
                else
                {
                    $("#inputval").attr('src', '@Url.Content("../../Images/no.png")');
                }
            });

        });
    });

