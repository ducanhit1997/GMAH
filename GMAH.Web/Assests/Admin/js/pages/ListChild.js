var dataTableChild = null;
var listChild = [];
var listAllChild = [];

document.addEventListener("DOMContentLoaded", function (event) {
    RenderTable();
    LoadStudentData();
    LoadChildData();

    // Add event
    $('#resultChild tbody').on('click', '.editor-delete', function () {
        let row = dataTableChild.row($(this).closest("tr")).data();
        if (!confirm(`Bạn có muốn xoá học sinh '${row.Fullname}' ra khỏi danh sách quản lý của phụ huynh không?`)) return;

        RemoveStudent(row.IdUser);
    });
});

function LoadStudentData() {
    $('#selectedStudent').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    listAllChild = [];

    $.ajax({
        type: "GET",
        url: "/api/userapi/GetAllStudent",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $("#selectedStudent").html("");

            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedStudent').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdUser, text: v.Fullname }); }));
            }

            listAllChild = result.Object;

            $('#selectedStudent').val(null).trigger('change');
        }
    });
}

function AddStudent() {
    let idUser = $("#selectedStudent").val();
    if (idUser == null || idUser == 0) {
        alert("Vui lòng chọn một học sinh muốn thêm");
    }

    var student = listAllChild.find(x => x.IdUser == idUser);
    var checkAny = listChild.findIndex(x => x.IdUser == idUser);

    if (checkAny >= 0) {
        alert("Học sinh này đã trong danh sách quản lý của phụ huynh")
        return;
    }

    listChild.push(student);
    RenderTable();
}

function RemoveStudent(idUser) {
    var index = listChild.findIndex(x => x.IdUser == idUser);
    listChild.splice(index, 1);
    RenderTable();
}

function LoadChildData() {
    listChild = [];

    if (_ID_PARENT == undefined || _ID_PARENT == null || _ID_PARENT == 0) {
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/ParentAPI/GetChildByParentId?idUser=" + _ID_PARENT,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                return;
            }

            listChild = result.Object;
            RenderTable();
        }
    });
}

function RenderTable() {
    dataTableChild = $("#resultChild").DataTable({
        destroy: true,
        data: listChild,
        "paging": true,
        "lengthChange": true,
        "ordering": false,
        "language": {
            "url": "/Assests/Data/datatable-vi.json"
        },
        columns: [
            { data: "StudentCode" },
            { data: "Fullname" },
            {
                data: null,
                className: "text-center editor-delete",
                defaultContent: '<i class="fa fa-trash"/>',
                orderable: false
            },
        ],
    });
}