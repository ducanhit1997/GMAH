var _ID_USER_MAIL = 0;

function LoadSemesterInYearData() {
    if (_ID_YEAR === null || _ID_YEAR == 0) {
        $("#mailForm").hide();
    }

    return $.ajax({
        type: "GET",
        url: "/api/SemesterAPI/GetSemesterInYear?idSemester=" + _ID_YEAR,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
    }).then(function (result) {
        $("#MailIdSemester").html("");

        if (result.Object !== undefined && result.Object !== null) {
            result.Object.push({ IdSemester: null, TitleName: 'Cả năm' });
            $('#MailIdSemester').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
        }

        $('#selectedSemester').val(_ID_SEMESTER);

        $("#mailForm").show();
    });
}

function SendMail() {
    let idSemester = $("#MailIdSemester").val();
    $("#mailLoading").show();
    $("#mailForm").hide();

    $.ajax({
        type: "POST",
        url: "/api/ScoreAPI/SendMailScore?IdSemester=" + idSemester + "&idUser=" + _ID_USER_MAIL + "&idYear=" + _ID_YEAR + "&idClass=" + _ID_CLASS,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert(result.Message);
            }
        },
        complete: function () {
            $("#mailLoading").hide();
            $("#mailForm").show();
        }
    });
}

function OpenSendMailModal(id) {
    _ID_USER_MAIL = id;
    LoadSemesterInYearData().then(() => {
        $("#sendMailModal").modal("show");
    });
}

function SendClassMail() {
    OpenSendMailModal(null);
}