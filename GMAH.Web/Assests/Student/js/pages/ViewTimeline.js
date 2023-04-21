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

function LoadTimelineDateRangeViewModel() {
    $('#selectedDate').html("");

    $("#mainGroup").show();
    $("#timelineData").html("");

    if ($("#selectedStudent").length <= 0) {
        _ID_STUDENT = -1;
    }
    else {
        _ID_STUDENT = $("#selectedStudent").val();
    }
    _ID_SEMESTER = $("#selectedSemester").val();

    if (_ID_STUDENT == 0 || _ID_STUDENT == null ||
        _ID_SEMESTER == 0 || _ID_SEMESTER == null) {
        $("#loading").hide();
        $("#mainGroup").hide();
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/ViewTimelineAPI/GetTimelineDateRangeViewModel?IdSemester=" + _ID_SEMESTER + "&idStudent=" + _ID_STUDENT,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Object !== undefined && result.Object !== null) {

                let optionHTML = ``;
                for (let i = 0; i < result.Object.length; i++) {
                    let selectedText = ``;

                    if (result.Object.IsCurrentWeek == true) {
                        selectedText = "selected";
                    }

                    optionHTML += `<option ${selectedText} from='${result.Object[i].DateFromJs}' 
                                                       to='${result.Object[i].DateToJs}'>${result.Object[i].DateFromString} đến ${result.Object[i].DateToString}</option>`;
                }

                $('#selectedDate').html(optionHTML);
            }

            if (!result.IsSuccess) {
                alert(result.Message);
            }

            LoadTimelineData();
        },
    });
}

function LoadTimelineData() {
    $("#mainGroup").hide();

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
        $("#mainGroup").hide();
        return;
    }

    let selectedOption = $('#selectedDate').find(":selected");
    if (selectedOption.length == 0) return;
    let dateFrom = selectedOption.attr("from");
    let dateTo = selectedOption.attr("to");

    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/ViewTimelineAPI/GetTimeLine?IdSemester=" + _ID_SEMESTER + "&idStudent=" + _ID_STUDENT + "&from=" + dateFrom + "&to=" + dateTo,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $("#mainGroup").show();
            $("#loading").hide();

            if (result.IsSuccess) {
                RenderTimeline(result.Object);
            }
            else {
                alert(result.Message);
            }
        },
    });
}