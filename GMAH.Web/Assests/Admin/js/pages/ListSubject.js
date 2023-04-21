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
        url: '/api/subjectapi/getlistsubject',
        type: 'POST',
        headers: {
            "Authorization": "Bearer " + _JWT_TOKEN
        },
    },
    columns: [
        {
            data: 'SubjectCode'
        },
        {
            data: 'SubjectName'
        },
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
            orderable: false,
            width: "50px",
        }
    ],
});

$('#result tbody').on('click', '.editor-list', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    location.href = linkTeacher + "/" + row.IdSubject
});

$('#result tbody').on('click', '.editor-edit', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    GetSubject(row.IdSubject);
});

$('#result tbody').on('click', '.editor-delete', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    if (!confirm(`Bạn có muốn xoá môn học '${row.SubjectName}' không?`)) return;

    $.ajax({
        type: "DELETE",
        url: "/api/subjectapi/deletesubject/" + row.IdSubject,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Xoá môn học thành công");
                dataTable.ajax.reload();
            }
        }
    });
});