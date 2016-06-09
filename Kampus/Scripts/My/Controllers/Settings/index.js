 function readURL(input) {

            if (input.files && input.files[0]) {
                var reader = new FileReader();

                reader.onload = function (e) {
                    $('.imgavatar').attr('src', e.target.result);
                }

                reader.readAsDataURL(input.files[0]);
            }
        }

        $(document).ready(function() {

            $("#addavatar").change(function () {
                readURL(this);
            });

            $("#changeavatarheader").click(function() {
                $("#changeavatar").slideToggle("slow");
            });
            $("#changepasswordheader").click(function () {
                $("#changepassword").slideToggle("slow");
            });
            $("#changestatusheader").click(function () {
                $("#changestatus").slideToggle("slow");
            });
            $("#changecityheader").click(function () {
                $("#changecity").slideToggle("slow");
            });
            $("#changestudentdataheader").click(function () {
                $("#changestudentdata").slideToggle("slow");
            });

            $("#UniversityName").change(function () {

                var Name = "";
                $("#UniversityName option:selected").each(function () {
                    Name = $(this).text();
                });

                alert("University name: " + Name);

                $.get('@Url.Action("GetUniversityFaculties", "Register")', { name: Name }).done(function (data) {
                    alert("Data Loaded: " + data);
                    var string = "";

                    alert(data);

                    var list = JSON.parse(data);


                    for (var i = 0; i < list.length; i++) {
                        console.log(i);
                        console.log(list[i]);
                        console.log(list[i].Id);
                        console.log(list[i].Name);
                        string += '<option value="' + list[i].Name + '">' + list[i].Name + '</option>';
                    }

                    console.log(string);

                    $('#UniversityFaculty').html(string);
                });

            });

            
            notifications_init();
        });