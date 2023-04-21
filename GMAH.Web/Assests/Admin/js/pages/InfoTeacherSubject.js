var dataTableTeacher = null;
var dataTableHead = null;

$(document).ready(function () {
    LoadData();
});

function LoadDataBySubjectId() {
    _ID_SUBJECT = $("#selectedSubject").val();
    LoadData();
}


function AddTeacher(type) {
    var id = $("#selected" + type).val();
    var prefixUrl = type == "headSubject" ? "SetHeadOfSubject" : "SetTeacherSubject";

    $.ajax({
        type: "PUT",
        url: "/api/SubjectAPI/" + prefixUrl,
        data: {
            IdUser: id,
            IdSubject: _ID_SUBJECT,
            FromYear: $("#fromYear").val(),
            ToYear: $("#toYear").val(),
        },
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
                LoadData();
            }
        }
    });
}

function LoadData() {
    $("#mainGroup").hide();
    $("#loading").show();

    $("#selectedSubject").html("");
    $("#selectedheadSubject").html("");
    $("#selectedteacherSubject").html("");

    $.ajax({
        type: "GET",
        url: "/api/SubjectAPI/GetTeacherInSubject?idSubject=" + _ID_SUBJECT,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            // Nếu ko thành công thì báo lỗi
            if (!result.IsSuccess) {
                alert(result.Message);
                return;
            }

            if (result.Object.Subjects !== undefined && result.Object.Subjects !== null) {
                $('#selectedSubject').append($.map(result.Object.Subjects, function (v, i) { return $('<option>', { val: v.IdSubject, text: v.SubjectName }); }));
            }

            // Set default data cho select box subject
            if (_ID_SUBJECT > 0) {
                $('#selectedSubject').val(_ID_SUBJECT);
                $("#mainGroup").show();
            }
            else {
                $('#selectedSubject').val(null);
                $("#mainGroup").hide();
            }

            $('#selectedSubject').select2({
                placeholder: 'Chọn một giá trị',
                theme: 'bootstrap4'
            });


            // Set dropdown box add teacher
            var headOfSubjectColumns = [
                { data: "TeacherCode" },
                { data: "Fullname" },
                { data: "Phone" },
                { data: "Email" },
                { data: "FromYear" },
                { data: "ToYear" }
            ];
            if (result.Object.IsAllowEditHeadOfSubject) {
                if (result.Object.HeadOfSubject !== undefined && result.Object.HeadOfSubject !== null) {
                    $('#selectedheadSubject').append($.map(result.Object.HeadOfSubject, function (v, i) { return $('<option>', { val: v.IdUser, text: v.Fullname }); }));
                }
                $('#selectedheadSubject').val(null);

                $('#selectedheadSubject').select2({
                    placeholder: 'Chọn một giá trị',
                    theme: 'bootstrap4'
                });

                $("[name=selectedheadSubjectdiv]").each(function (index) {
                    $(this).show();
                });

                headOfSubjectColumns.push({
                    data: null,
                    className: "text-center headsubject-delete",
                    defaultContent: '<i class="fa fa-trash"/>',
                    orderable: false
                });
            }
            else {
                $("[name=selectedheadSubjectdiv]").each(function (index) {
                    $(this).hide();
                });
            }

            if (result.Object.Teachers !== undefined && result.Object.Teachers !== null) {
                $('#selectedteacherSubject').append($.map(result.Object.Teachers, function (v, i) { return $('<option>', { val: v.IdUser, text: v.Fullname }); }));
            }
            $('#selectedteacherSubject').val(null);

            $('#selectedteacherSubject').select2({
                placeholder: 'Chọn một giá trị',
                theme: 'bootstrap4'
            });

            // Set list teacher
            dataTableHead = $("#headOfSubject").find("[name=result]").DataTable({
                destroy: true,
                data: result.Object.HeadOfCurrentSubject,
                "paging": true,
                "lengthChange": true,
                "ordering": false,
                "language": {
                    "url": "/Assests/Data/datatable-vi.json"
                },
                columns: headOfSubjectColumns
            });

            dataTableTeacher = $("#teacherInSubject").find("[name=result]").DataTable({
                destroy: true,
                data: result.Object.TeacherInCurrentSubject,
                "paging": true,
                "lengthChange": true,
                "language": {
                    "url": "/Assests/Data/datatable-vi.json"
                },
                columns: [
                    { data: "TeacherCode" },
                    { data: "Fullname" },
                    { data: "Phone" },
                    { data: "Email" },
                    {
                        data: null,
                        className: "text-center teachersubject-delete",
                        defaultContent: '<i class="fa fa-trash"/>',
                        orderable: false
                    }
                ]
            });
        },
        complete: function () {
            $("#loading").hide();
        }
    });
}


$('tbody').on('click', '.headsubject-delete', function () {
    let row = dataTableHead.row($(this).closest("tr")).data();
    if (!confirm(`Bạn có muốn xoá '${row.Fullname}' khỏi danh sách trưởng bộ môn không?`)) return;

    $.ajax({
        type: "DELETE",
        url: "/api/SubjectAPI/RemoveHeadOfSubject",
        data: {
            IdUser: row.IdUser,
            IdSubject: _ID_SUBJECT,
        },
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Xoá dữ liệu thành công");
                LoadData();
            }
        }
    });
});

$('tbody').on('click', '.teachersubject-delete', function () {
    let row = dataTableTeacher.row($(this).closest("tr")).data();
    if (!confirm(`Bạn có muốn xoá '${row.Fullname}' khỏi danh sách giáo viên bộ môn không?`)) return;

    $.ajax({
        type: "DELETE",
        url: "/api/SubjectAPI/RemoveTeacherSubject",
        data: {
            IdUser: row.IdUser,
            IdSubject: _ID_SUBJECT,
        },
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Xoá dữ liệu thành công");
                LoadData();
            }
        }
    });
});