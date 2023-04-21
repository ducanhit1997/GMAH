var _ID_SEMESTER = null;
var _ID_STUDENT = null;

$(document).ready(function () {
    LoadSemesterData();
    LoadStudentData();
});

function LoadSemesterData() {
    $("#selectedSemester").html("");

    $.ajax({
        type: "GET",
        url: "/api/semesterapi/getcurrentsemester",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {    
            if (result.Data !== undefined && result.Data !== null) {
                $('#selectedSemester').append($.map(result.Data, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
            }

            $('#selectedSemester').select2({
                placeholder: 'Chọn học kỳ',
                theme: 'bootstrap4'
            });

            _ID_SEMESTER = result.SelectedId;
            $('#selectedSemester').val(_ID_SEMESTER == 0 ? null : _ID_SEMESTER).trigger('change');
        }
    });
}

function LoadStudentData() {
    if ($("#selectedStudent").length <= 0) return;

    $("#selectedStudent").html("");

    $.ajax({
        type: "GET",
        url: "/api/viewscoreapi/getlistchild",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedStudent').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdUser, text: v.Fullname }); }));
            }

            $('#selectedStudent').select2({
                placeholder: 'Chọn học sinh',
                theme: 'bootstrap4'
            });

            $('#selectedStudent').val(_ID_STUDENT == 0 ? null : _ID_STUDENT).trigger('change');
        }
    });
}

function LoadScoreData() {
    $("#mainGroup").hide();
    $("#notfound").hide();

    _ID_SEMESTER = $("#selectedSemester").val();

    if ($("#selectedStudent").length <= 0) {
        _ID_STUDENT = -1;
    }
    else {
        _ID_STUDENT = $("#selectedStudent").val();
    }

    if (_ID_STUDENT == 0 || _ID_STUDENT == null ||
        _ID_SEMESTER == 0 || _ID_SEMESTER == null) {
        $("#loading").hide();
        $("#notfound").show();
        $("#mainGroup").hide();

    }

    $("#loading").show();

    let viewType = $("#selectedViewType").val();
    $.ajax({
        type: "GET",
        url: "/api/ViewScoreAPI/GetScore?idStudent=" + _ID_STUDENT + "&idSemester=" + _ID_SEMESTER + "&viewType=" + viewType,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                $("#notfound").show();
            }
            else {
                // Render danh sách học sinh
                RenderTable(result);
                $("#mainGroup").show();
            }
        },
        complete: function () {
            $("#loading").hide();
        }
    });
}