var _ID_SEMESTER = null;
var _ID_STUDENT = null;

$(document).ready(function () {
    LoadSemesterData();
    LoadStudentData();

    $('#DateFrom').datetimepicker({
        format: 'L'
    });

    $("#DateFrom").on('change.datetimepicker', function (e) { LoadAttendance(); });;

    $('#DateTo').datetimepicker({
        format: 'L'
    });

    $("#DateTo").on('change.datetimepicker', function (e) { LoadAttendance(); });;

    var fromDate = new Date();
    fromDate.setDate(fromDate.getDate() - 7);
    $("#DateFrom").datetimepicker("date", fromDate);

    fromDate.setDate(fromDate.getDate() + 14);
    $("#DateTo").datetimepicker("date", fromDate);
});

function LoadSemesterData() {
    $("#selectedSemester").html("");

    $.ajax({
        type: "GET",
        url: "/api/semesterapi/getcurrentsemester?viewSemester=2",
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

function LoadAttendance() {
    $("#mainGroup").hide();

    var from = $("#DateFrom").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null
    var to = $("#DateTo").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null

    if ($("#selectedStudent").length <= 0) {
        _ID_STUDENT = -1;
    }
    else {
        _ID_STUDENT = $("#selectedStudent").val();
    }
    _ID_SEMESTER = $("#selectedSemester").val();

    if (_ID_STUDENT == null || _ID_SEMESTER == 0 || from == null || to == null) return;

    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/viewattendanceapi/ViewAttendance?idStudent=" + _ID_STUDENT + "&idYear=" + _ID_SEMESTER + "&from=" + from + "&to=" + to,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $("#mainGroup").show();
            $("#loading").hide();

            RenderAttendance(result.Object);
        },
    });

    $("#mainGroup").show();
    $("#loading").hide();
}
