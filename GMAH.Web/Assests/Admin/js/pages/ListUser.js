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
        url: '/api/userapi/getlist' + actionName,
        type: 'POST',
        headers: {
            "Authorization": "Bearer " + _JWT_TOKEN
        },
    },
    columns: [
        ...columns,
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

$('#result tbody').on('click', '.editor-edit', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    location.href = linkInfoUser + '/' + row.IdUser;
});

$('#result tbody').on('click', '.editor-delete', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    if (!confirm(`Bạn có muốn xoá người dùng '${row.Fullname}' có tài khoản là '${row.Username}' không?`)) return;

    $.ajax({
        type: "DELETE",
        url: "/api/userapi/deleteuser/" + row.IdUser,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Xoá người dùng này thành công");
                location.reload();
            }
        }
    });
});