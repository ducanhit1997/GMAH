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
        url: '/api/classapi/GetClassBySemester?idSemester=' + _ID_SEMESTER,
        type: 'POST',
        headers: {
            "Authorization": "Bearer " + _JWT_TOKEN
        },
    },
    columns: [
        { data: "ClassName" },
        { data: "FormTeacherFullname" },
        {
            data: null,
            className: "text-center editor-edit",
            defaultContent: '<i class="fas fa-edit"></i>',
            orderable: false,
            width: "50px",
        },
        {
            data: null,
            className: "text-center editor-list",
            defaultContent: '<i class="fas fa-user-edit"></i>',
            orderable: false,
            width: "50px",
        },
        {
            data: null,
            className: "text-center editor-delete",
            defaultContent: '<i class="fa fa-trash"/>',
            orderable: false
        }
    ]
});

$(document).ready(function () {
    LoadSemesterData();
    LoadClassData();
});

function LoadClassBySemesterId() {
    _ID_SEMESTER = $("#selectedSemester").val();
    LoadClassData();
}

function LoadSemesterData() {
    $("#selectedSemester").html("");
    $("#IdSemester").html("");

    if (_ID_SEMESTER === null || _ID_SEMESTER == 0) {
        $("#mainGroup").hide();
    }

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
                $('#IdSemester').append($.map(result.Data, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
            }

            $('#selectedSemester').val(_ID_SEMESTER == 0 ? null : _ID_SEMESTER);

            $('#selectedSemester').select2({
                placeholder: 'Chọn một giá trị',
                theme: 'bootstrap4'
            });

            $('#IdSemester').select2({
                placeholder: 'Chọn một giá trị',
                theme: 'bootstrap4'
            });
        }
    });
}

function LoadClassData() {
    $("#mainGroup").hide();
    $("#loading").show();

    dataTable.ajax.url('/api/classapi/GetClassBySemester?idSemester=' + _ID_SEMESTER);
    dataTable.ajax.reload();

    $("#mainGroup").show();
    $("#loading").hide();
}

$('#result tbody').on('click', '.editor-list', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    location.href = linkStudent + "/" + row.IdClass
});

$('#result tbody').on('click', '.editor-edit', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    location.href = linkInfo + '/' + row.IdClass;
});

$('#result tbody').on('click', '.editor-delete', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    if (!confirm(`Bạn có muốn xoá lớp '${row.ClassName}' không?`)) return;

    $.ajax({
        type: "DELETE",
        url: "/api/classapi/deleteclass/" + row.IdClass,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Xoá lớp học thành công");
                dataTable.ajax.reload();
            }
        }
    });
});