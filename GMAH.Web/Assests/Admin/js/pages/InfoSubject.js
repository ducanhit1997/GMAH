var _ID_SUBJECT = 0;

function CreateNewSubject() {
    _ID_SUBJECT = 0;
    OpenSubjectModal();
}

function OpenSubjectModal() {
    $("#subjectModal").modal("show");
}

function GetSubject(id) {
    OpenSubjectModal();

    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/subjectapi/getsubject/" + id,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {

                $("[name=SubjectCode]").val(result.Object.SubjectCode);
                $("[name=SubjectName]").val(result.Object.SubjectName);

                _ID_SUBJECT = id;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}

function SaveSubject() {
    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "POST",
        url: "/api/subjectapi/savesubject",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        data: {
            IdSemester: _ID_SUBJECT,
            SubjectCode: $("[name=SubjectCode]").val(),
            SubjectName: $("[name=SubjectName]").val(),
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Lưu dữ liệu thành công");
                dataTable.ajax.reload();
                $("#subjectModal").modal("hide");
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}