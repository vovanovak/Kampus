$(document).ready(function () {

    $('#IsNotStudent').change(function () {
        $("#divStudentDetails").toggle('slow');
    });

    $("#addavatar").change(function () {
        readURL(this);
    });

    $("#inputSignInUsername").change(function () {
        $.get('/Register/ContainsUserWithSuchUsername', { username: $(this).val() }, function (data) {

            
            if (data.localeCompare('contains')) {
                $("#inputval").attr('src', '../../../Images/yes.png');
                
            }
            else {
                $("#inputval").attr('src', '../../../Images/no.png');
                
            }
        });
    });

    changeUniversityFaculties();
});
