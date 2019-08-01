function changeUniversityFaculties() {
    $("#UniversityName").change(function () {
        var _name = "";

        _name = $("#UniversityName option:selected").first().text();

        $.get('/Register/GetUniversityFaculties', { name: _name }).done(function (data) {
            var string = "";
            var list = JSON.parse(data);

            for (var i = 0; i < list.length; i++) {
                string += '<option value="' + list[i].Name + '">' + list[i].Name + '</option>';
            }

            $('.inputUniversityFaculty').html(string);
        });

    });
}