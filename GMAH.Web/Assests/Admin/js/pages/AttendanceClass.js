var _ATTENDANCE_DATA = null;

$(document).ready(function () {
    LoadSemesterData();
    LoadClassData();
    $('#DateAttendance').datetimepicker({
        format: 'L'
    });

    $("#DateAttendance").on('change.datetimepicker', function (e) { LoadAttendanceByClassId(); });;

    $("#DateAttendance").datetimepicker("date", new Date());

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

    var date = $("#DateAttendance").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null
    if (_ID_CLASS == null || _ID_CLASS == 0 || date == null) return;

    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/attendanceapi/GetClassAttendanceByDate?idClass=" + _ID_CLASS + "&date=" + date,
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

function SaveAttendance() {
    var date = $("#DateAttendance").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null
    if (_ID_CLASS == null || _ID_CLASS == 0 || date == null) return;

    for (let i = 0; i < _ATTENDANCE_DATA.Students.length; i++) {
        let idStudent = _ATTENDANCE_DATA.Students[i].IdStudent;
        _ATTENDANCE_DATA.Students[i].AttendanceStatus = $('input[name=radioAttendance' + idStudent + ']:checked').val();
    }

    $("#mainGroup").hide();
    $("#loading").show();

    $.ajax({
        type: "POST",
        url: "/api/attendanceapi/SaveClassAttendanceByDate",
        dataType: 'json',
        data: _ATTENDANCE_DATA,
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Cập nhật dữ liệu điểm danh thành công");
            }

            LoadAttendanceInClass();
        },
    });
}