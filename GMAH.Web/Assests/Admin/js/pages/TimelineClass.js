$(document).ready(function () {
    LoadSemesterData();
    LoadClassData();
});

function LoadTimelineByClassId() {
    _ID_CLASS = $("#selectedClass").val();
    LoadTimelineInClass();
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
        url: "/api/semesterapi/getcurrentsemester",
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
        url: "/api/ClassAPI/GetTeacherClassBySemester?viewScore=true&IdSemester=" + _ID_SEMESTER,
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

function LoadTimelineInClass() {
    if (_ID_CLASS == null || _ID_CLASS == 0) return;

    $("#mainGroup").hide();
    $("#loading").show();

    LoadTimelineDateRangeViewModel();

    $("#mainGroup").show();
    $("#loading").hide();
}

function LoadTimelineDateRangeViewModel() {
    $('#selectedDate').html("");

    $("#mainGroup").show();
    $("#timelineData").html("");

    $.ajax({
        type: "GET",
        url: "/api/timelineapi/GetTimelineDateRangeViewModel?IdSemester=" + _ID_SEMESTER + "&idClass=" + _ID_CLASS,
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

            LoadTimelineData();
        },
    });
}

function LoadTimelineData() {
    let selectedOption = $('#selectedDate').find(":selected");
    if (selectedOption.length == 0) return;
    let dateFrom = selectedOption.attr("from");
    let dateTo = selectedOption.attr("to");

    $("#mainGroup").hide();
    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/timelineapi/GetTimeLine?IdSemester=" + _ID_SEMESTER + "&idClass=" + _ID_CLASS + "&from=" + dateFrom + "&to=" + dateTo,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $("#mainGroup").show();
            $("#loading").hide();

            RenderTimeline(result.Object);
        },
    });
}

function DeleteTimeline() {
    if (!confirm("Bạn có thực sự muốn xoá toàn bộ thời khoá biểu của lớp tại học kỳ hiện tại?")) return;

    $.ajax({
        type: "DELETE",
        url: "/api/timelineapi/deletetimeline?IdSemester=" + _ID_SEMESTER + "&idClass=" + _ID_CLASS,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                return;
            }

            alert("Xoá toàn bộ thời khoá biểu trong học kỳ thành công");
            $('#selectedDate').html("");    
            LoadTimelineDateRangeViewModel();
        },
    });
}