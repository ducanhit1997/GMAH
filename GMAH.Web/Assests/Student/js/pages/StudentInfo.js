var _ID_STUDENT = null;

$(document).ready(function () {
    LoadStudentData();
});

function SaveStudentInfo() {
    $.ajax({
        type: "POST",
        url: "/api/parentAPI/changechildinfo",
        data: {
            idChild: _ID_STUDENT,
            Email: $("#studentEmail").val(),
            Phone: $("#studentPhone").val(),
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
                alert("Thay đổi thông tin thành công");
            }
        }
    });
}

function LoadStudentData() {
    $("#selectedStudent").html("");

    $.ajax({
        type: "GET",
        url: "/api/viewscoreapi/getlistchild",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedStudent').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdUser, text: v.Fullname }); }));
            }

            $('#selectedStudent').select2({
                placeholder: 'Chọn học sinh',
                theme: 'bootstrap4'
            });

            $('#selectedStudent').val(_ID_STUDENT == 0 ? null : _ID_STUDENT).trigger('change');
        }
    });
}

function LoadStudentInfo() {
    $("#mainGroup").hide();

    _ID_STUDENT = $("#selectedStudent").val();

    if (_ID_STUDENT == 0 || _ID_STUDENT == null) {
        $("#loading").hide();
        $("#mainGroup").hide();

    }

    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/ParentAPI/ViewChildInfo?idChild=" + _ID_STUDENT,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                // Render danh sách học sinh
                RenderInfo(result.Object);
                $("#mainGroup").show();
            }
        },
        complete: function () {
            $("#loading").hide();
        }
    });
}

function RenderInfo(data) {
    $("#studentFullname").val(data.Fullname);
    $("#studentEmail").val(data.Email);
    $("#studentPhone").val(data.Phone);
    $("#studentCitizenID").val(data.CitizenID);
}