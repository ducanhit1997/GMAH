var dataTableParent = null;
var listParent = [];
var listAllParent = [];

document.addEventListener("DOMContentLoaded", function (event) {
    RenderTable();
    LoadParentSelectData();
    LoadParentListData();

    // Add event
    $('#resultParent tbody').on('click', '.editor-delete', function () {
        let row = dataTableParent.row($(this).closest("tr")).data();
        if (!confirm(`Bạn có muốn xoá phụ huynh '${row.Fullname}' ra khỏi danh sách quản lý học sinh này không?`)) return;

        RemoveParent(row.IdUser);
    });
});

function LoadParentSelectData() {
    $('#selectedParent').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    listAllChild = [];

    $.ajax({
        type: "GET",
        url: "/api/userapi/GetAllParent",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $("#selectedParent").html("");

            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedParent').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdUser, text: v.Fullname }); }));
            }

            listAllParent = result.Object;

            $('#selectedParent').val(null).trigger('change');
        }
    });
}

function AddParent() {
    let idUser = $("#selectedParent").val();
    if (idUser == null || idUser == 0) {
        alert("Vui lòng chọn một phụ huynh muốn thêm");
    }

    var parent = listAllParent.find(x => x.IdUser == idUser);
    var checkAny = listParent.findIndex(x => x.IdUser == idUser);

    if (checkAny >= 0) {
        alert("Phụ huynh này đã trong danh sách quản lý")
        return;
    }

    listParent.push(parent);
    RenderTable();
}

function RemoveParent(idUser) {
    var index = listParent.findIndex(x => x.IdUser == idUser);
    listParent.splice(index, 1);
    RenderTable();
}

function LoadParentListData() {
    listParent = [];

    if (_ID_STUDENT == undefined || _ID_STUDENT == null || _ID_STUDENT == 0) {
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/StudentAPI/GetParentByStudentID?idUser=" + _ID_STUDENT,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                return;
            }

            listParent = result.Object;
            RenderTable();
        }
    });
}

function RenderTable() {
    dataTableParent = $("#resultParent").DataTable({
        destroy: true,
        data: listParent,
        "paging": true,
        "lengthChange": true,
        "language": {
            "url": "/Assests/Data/datatable-vi.json"
        },
        columns: [
            { data: "Fullname" },
            { data: "Phone" },
            { data: "Email" },
            {
                data: null,
                className: "text-center editor-delete",
                defaultContent: '<i class="fa fa-trash"/>',
                orderable: false
            },
        ],
    });
}