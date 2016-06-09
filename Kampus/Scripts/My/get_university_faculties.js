function changeUniversityFaculties() {
    $("#UniversityName").change(function () {

        var Name = "";
        $("#UniversityName option:selected").each(function () {
            Name = $(this).text();
        });

        $.get('/Register/GetUniversityFaculties', { name: Name }).done(function (data) {

            var string = "";

            //alert(data);

            var list = JSON.parse(data);


            for (var i = 0; i < list.length; i++) {
                console.log(i);
                console.log(list[i]);
                console.log(list[i].Id);
                console.log(list[i].Name);
                string += '<option value="' + list[i].Name + '">' + list[i].Name + '</option>';
            }

            console.log(string);

            $('.inputUniversityFaculty').html(string);
        });

    });
}