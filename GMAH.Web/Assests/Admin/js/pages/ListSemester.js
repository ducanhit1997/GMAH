var dataTable = $('#result').DataTable({
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
        url: '/api/semesterapi/getlistsemester',
        type: 'POST',
        headers: {
            "Authorization": "Bearer " + _JWT_TOKEN
        },
    },
    columns: [
        {
            data: 'SemesterYear'
        },
        {
            data: 'SemesterName'
        },
        {
            data: 'DateStartText'
        },
        {
            data: 'DateEndText'
        },
        {
            data: null,
            className: "text-center editor-edit",
            defaultContent: '<i class="fas fa-edit"></i>',
            orderable: false
        },
        {
            data: null,
            className: "text-center editor-delete",
            defaultContent: '<i class="fa fa-trash"/>',
            orderable: false
        }
    ],
});

function LoadSemesterSetting() {
    $("#settingDefaultSemester").html("");
    $.ajax({
        type: "GET",
        url: "/api/semesterapi/getcurrentsemester",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Data !== undefined && result.Data !== null) {
                $('#settingDefaultSemester').append($.map(result.Data, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
            }
            $('#settingDefaultSemester').val(result.SelectedId);
            $('#settingDefaultSemester').select2({
                theme: 'bootstrap4'
            });
        }
    });
}

function SetCurrentSemester() {
    var id = $("#settingDefaultSemester").val();
    $.ajax({
        type: "PUT",
        url: "/api/semesterapi/setcurrentsemester/" + id,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Lưu dữ liệu thành công");
            }
        }
    });
}

$('#result tbody').on('click', '.editor-edit', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    GetSemester(row.IdSemester);
});

$('#result tbody').on('click', '.editor-delete', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    if (!confirm(`Bạn có muốn xoá học kỳ '${row.SemesterName}' năm học '${row.SemesterYear}' không?`)) return;

    $.ajax({
        type: "DELETE",
        url: "/api/semesterapi/deletesemester/" + row.IdSemester,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Xoá học kỳ thành công");
                dataTable.ajax.reload();
            }
        }
    });
});

$(document).ready(function () {
    LoadSemesterSetting();
});