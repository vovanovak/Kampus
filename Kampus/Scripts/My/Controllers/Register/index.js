$(document).ready(function () {

        $('#IsNotStudent').change(function () {
            $("#divStudentDetails").toggle('slow');
        });

        $("#UniversityName").change(function () {

            var Name = "";
            $("#UniversityName option:selected").each(function () {
                Name = $(this).text();
            });
            // alert("University name: " + Name);

            $.get('@Url.Action("GetUniversityFaculties", "Register")', { name: Name }).done(function (data) {
                // alert("Data Loaded: " + data);
                var string = "";

                alert(data);

                var list = JSON.parse(data);


                for (var i = 0; i < list.length; i++) {
                    console.log(i);
                    console.log(list[i]);
                    console.log(list[i].Id);
                    console.log(list[i].Name);
                    string += '<option value="' + list[i].Id + '">' + list[i].Name + '</option>';
                }

                console.log(string);

                $('#UniversityFaculty').html(string);
            });

        });
    });