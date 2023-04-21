var _ATTENDANCE_DATA = null;

$(document).ready(function () {
    LoadSemesterData();
    LoadClassData();
    $('#DateFrom').datetimepicker({
        format: 'L'
    });

    $("#DateFrom").on('change.datetimepicker', function (e) { LoadAttendanceByClassId(); });;

    $('#DateTo').datetimepicker({
        format: 'L'
    });

    $("#DateTo").on('change.datetimepicker', function (e) { LoadAttendanceByClassId(); });;

    var fromDate = new Date();
    fromDate.setDate(fromDate.getDate() - 7);
    $("#DateFrom").datetimepicker("date", fromDate);

    fromDate.setDate(fromDate.getDate() + 14);
    $("#DateTo").datetimepicker("date", fromDate);
});

function LoadAttendanceByClassId() {
    _ID_CLASS = $("#selectedClass").val();
    LoadAttendanceInClass();
}

function LoadClassBySemesterId() {
    _ID_SEMESTER = $("#selectedSemester").val();
    LoadClassData();
}

function LoadSemesterData() {
    $('#selectedSemester').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    if (_ID_SEMESTER === null || _ID_SEMESTER == 0) {
        $("#mainGroup").hide();
    }

    return $.ajax({
        type: "GET",
        url: "/api/semesterapi/getcurrentsemester?viewSemester=2",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
    }).then(function (result) {
        $("#selectedSemester").html("");

        if (result.Data !== undefined && result.Data !== null) {
            $('#selectedSemester').append($.map(result.Data, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
        }

        $('#selectedSemester').val(_ID_SEMESTER == 0 ? null : _ID_SEMESTER).trigger('change');
    });
}

function LoadClassData() {
    $('#selectedClass').attr('disabled', true);
    $('#selectedClass').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    if (_ID_CLASS === null || _ID_CLASS == 0) {
        $("#mainGroup").hide();
    }

    $.ajax({
        type: "GET",
        url: "/api/ClassAPI/GetTeacherClassBySemester?IdYear=" + _ID_SEMESTER,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $('#selectedClass').attr('disabled', false);
            $("#selectedClass").html("");

            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedClass').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdClass, text: v.ClassName }); }));
            }

            $('#selectedClass').val(_ID_CLASS == 0 ? null : _ID_CLASS).trigger('change');
        },
    });
}

function LoadAttendanceInClass() {
    $("#mainGroup").hide();

    var from = $("#DateFrom").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null
    var to = $("#DateTo").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null

    if (_ID_CLASS == null || _ID_CLASS == 0 || from == null || to == null) return;

    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/attendanceapi/GetStudentAttendance?idClass=" + _ID_CLASS + "&from=" + from + "&to=" + to,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $("#mainGroup").show();
            $("#loading").hide();

            _ATTENDANCE_DATA = result.Object;
            RenderAttendance(result.Object);
        },
    });

    $("#mainGroup").show();
    $("#loading").hide();
}