var _ID_SEMESTER = 0;

function CreateNewSemester() {
    _ID_SEMESTER = 0;
    OpenSemesterModal();
}

function OpenSemesterModal() {
    $("#semesterModal").modal("show");
}

function GetSemester(id) {
    OpenSemesterModal();

    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/semesterapi/getsemester/" + id,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {

                $("[name=SemesterYear]").val(result.Object.SemesterYear);
                $("[name=SemesterName]").val(result.Object.SemesterName);
                $("[name=ScoreWeight]").val(result.Object.ScoreWeight);

                $("#DateStart").datetimepicker("date", result.Object.DateStart)
                $("#DateEnd").datetimepicker("date", result.Object.DateEnd)
                $("[name=IsCurrentSemester]").prop('checked', result.Object.IsCurrentSemester);

                _ID_SEMESTER = id;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}

function SaveSemester() {
    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "POST",
        url: "/api/semesterapi/savesemester",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        data: {
            IdSemester: _ID_SEMESTER,
            SemesterYear: $("[name=SemesterYear]").val(),
            SemesterName: $("[name=SemesterName]").val(),
            ScoreWeight: $("[name=ScoreWeight]").val(),
            DateStart: $("#DateStart").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null,
            DateEnd: $("#DateEnd").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null,
            IsCurrentSemester: $("[name=IsCurrentSemester]").is(":checked"),
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Lưu dữ liệu thành công");
                dataTable.ajax.reload();
                $("#semesterModal").modal("hide");
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}