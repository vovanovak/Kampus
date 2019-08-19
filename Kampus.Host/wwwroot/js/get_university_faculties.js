function changeUniversityFaculties() {
    $("#UniversityName").change(function () {
        var _name = "";

        _name = $("#UniversityName option:selected").first().text();

        $.get('/Register/GetUniversityFaculties', { name: _name }).done(function (list) {
            var string = "";

            for (var i = 0; i < list.length; i++) {
                string += '<option value="' + list[i].Name + '">' + list[i].Name + '</option>';
            }

            $('.inputUniversityFaculty').html(string);
        });

    });
}