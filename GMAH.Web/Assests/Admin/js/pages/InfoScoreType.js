_ID_SCORE_TYPE = 0;

function CreateNew() {
    _ID_SCORE_TYPE = 0;
    ClearInput();
    OpenModal();
}

function ClearInput() {
    $("[name=ScoreName]").val("");
    $("[name=ScoreName]").val("");
}

function OpenModal() {
    $("#scoreTypeModal").modal("show");
}

function GetScoreType(id) {
    OpenModal();
    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/scoreapi/getscoretype/" + id,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                return;
            }
            else {

                $("[name=ScoreName]").val(result.Object.ScoreName);
                $("[name=ScoreWeight]").val(result.Object.ScoreWeight);

                _ID_SCORE_TYPE = id;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}

function SaveScoreType() {
    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "POST",
        url: "/api/scoreapi/savescoretype",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        data: {
            IdScoreType: _ID_SCORE_TYPE,
            ScoreName: $("[name=ScoreName]").val(),
            ScoreWeight: $("[name=ScoreWeight]").val(),
            IdClass: _ID_CLASS,
            IdSubject: _ID_SUBJECT,
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Lưu dữ liệu thành công");
                LoadScoreTypeData();
                $("#scoreTypeModal").modal("hide");
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}
