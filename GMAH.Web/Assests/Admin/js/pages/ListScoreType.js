var _ID_CLASS = 0;
var _ID_SUBJECT = 0;
var dataTable = $("#result").DataTable({
    "language": {
        "url": "/Assests/Data/datatable-vi.json"
    },
    "paging": true,
    "lengthChange": true,
    "searching": true,
    "info": true,
    "autoWidth": true,
    "responsive": true,
    "processing": true,
    "serverSide": true,
    "ordering": false,
    ajax: {
        url: '/api/classapi/GetClassSubjectScoreType?idClass=' + _ID_CLASS + "&idSubject=" + _ID_SUBJECT,
        type: 'POST',
        headers: {
            "Authorization": "Bearer " + _JWT_TOKEN
        },
    },
    columns: [
        { data: "ScoreName" },
        { data: "ScoreWeight" },
        {
            data: null,
            className: "text-center editor-edit",
            defaultContent: '<i class="fas fa-edit"></i>',
            orderable: false,
            width: "50px",
        },
        {
            data: null,
            className: "text-center editor-delete",
            defaultContent: '<i class="fa fa-trash"/>',
            orderable: false,
            width: "50px",
        }
    ]
});

$(document).ready(function () {
    LoadSemesterData();
    LoadClassData();
});

function LoadSemesterData() {
    _ID_CLASS = null;
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

function LoadClassBySemesterId() {
    _ID_SEMESTER = $("#selectedSemester").val();

    $('#selectedSubject').attr('disabled', true);
    $("#selectedSubject").html("");
    $('#selectedSubject').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    LoadClassData();
}

function LoadClassData() {
    $('#mainGroup').hide();
    $('#selectedClass').attr('disabled', true);
    $('#selectedClass').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    if (_ID_SEMESTER == null || _ID_SEMESTER == 0) {
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/ClassAPI/GetTeacherClassBySemester?IdYear=" + _ID_SEMESTER + "&viewScore=true",
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

function LoadSubjectByClassId() {
    _ID_CLASS = $("#selectedClass").val();
    LoadClassSubject();
}

function LoadClassSubject() {
    $('#mainGroup').hide();

    $('#selectedSubject').attr('disabled', true);
    $('#selectedSubject').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    if (_ID_CLASS == null || _ID_CLASS == 0) {
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/ClassAPI/GetSubjectTeacherClass?IdClass=" + _ID_CLASS,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $('#selectedSubject').attr('disabled', false);
            $("#selectedSubject").html("");

            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedSubject').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdSubject, text: v.SubjectName }); }));
            }

            $('#selectedSubject').val(_ID_SUBJECT == 0 ? null : _ID_SUBJECT).trigger('change');
        },
    });
}

function LoadScoreType() {
    _ID_SUBJECT = $("#selectedSubject").val();

    if (_ID_SUBJECT != 0 && _ID_SUBJECT != null) {
        LoadScoreTypeData();
    }
}

function LoadScoreTypeData() {
    $('#mainGroup').show();

    dataTable.ajax.url('/api/classapi/GetClassSubjectScoreType?idClass=' + (_ID_CLASS ?? 0) + "&idSubject=" + (_ID_SUBJECT ?? 0));
    dataTable.ajax.reload();
}


$('#result tbody').on('click', '.editor-edit', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    GetScoreType(row.IdScoreType);
});

$('#result tbody').on('click', '.editor-delete', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    if (!confirm(`Bạn có muốn xoá thành phần điểm '${row.ScoreName}' không?`)) return;

    $.ajax({
        type: "DELETE",
        url: "/api/scoreapi/deletescoretype/" + row.IdScoreType,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Xoá thành phần điểm thành công");
                dataTable.ajax.reload();
            }
        }
    });
});