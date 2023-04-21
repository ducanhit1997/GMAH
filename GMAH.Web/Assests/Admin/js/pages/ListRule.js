var dataTable = null;
var listRule = [];

$(document).ready(function () {
    LoadSemesterData();
    LoadRuleData();

    $('#result tbody').on('click', '.editor-edit', function () {
        let row = dataTable.row($(this).closest("tr")).data();
        location.href = linkInfoRule + '/' + row.IdRule;
    });

    $('#result tbody').on('click', '.editor-delete', function () {
        let row = dataTable.row($(this).closest("tr")).data();
        if (!confirm(`Bạn có muốn xoá luật xếp hạng này không?`)) return;

        $.ajax({
            type: "DELETE",
            url: "/api/gradeapi/delete/" + row.IdRule,
            dataType: 'json',
            headers: {
                "Authorization": "Baerer " + _JWT_TOKEN
            },
            success: function (result) {
                if (!result.IsSuccess) {
                    alert(result.Message);
                }
                else {
                    alert("Xoá luật xếp hạng thành công");
                    location.reload();
                }
            }
        });
    });
});

function LoadRuleBySemesterId() {
    _ID_SEMESTER = $("#selectedSemester").val();
    LoadRuleData();
}

function LoadRuleData() {
    $("#mainGroup").hide();
    listRule = [];
    RenderTable();

    if (_ID_SEMESTER == null || _ID_SEMESTER == 0) {
        return;
    }

    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/GradeAPI/GetAll?idSemester=" + _ID_SEMESTER,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                return;
            }

            listRule = result.Object;
        },
        complete: function () {
            RenderTable();
            $("#loading").hide();
            $("#mainGroup").show();
        }
    });
}

function RenderTable() {
    dataTable = $("#result").DataTable({
        destroy: true,
        data: listRule,
        "paging": true,
        "lengthChange": true,
        "searching": true,
        "info": true,
        "autoWidth": false,
        "responsive": true,
        "processing": true,
        "language": {
            "url": "/Assests/Data/datatable-vi.json"
        },
        columns: [
            { data: "IdRule" },
            { data: "ClassName" },
            {
                data: null,
                className: "text-center editor-edit",
                defaultContent: '<i class="fa fa-edit"/>',
                orderable: false,
                width: "50px",
            },
            {
                data: null,
                className: "text-center editor-delete",
                defaultContent: '<i class="fa fa-trash"/>',
                orderable: false,
                width: "50px",
            },
        ],
    });
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