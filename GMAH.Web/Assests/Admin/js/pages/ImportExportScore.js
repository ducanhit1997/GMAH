document.getElementById('excelFile').addEventListener('change', function (e) {
    var fileName = document.getElementById("excelFile").files[0].name;
    var nextSibling = e.target.nextElementSibling
    nextSibling.innerText = fileName
})

function OpenModalImportScore() {
    $("#inputform").show();
    $("#confirmform").hide();
    $("#loading").hide();
    $("#excelFile").val(null);
    $("#uploadModal").modal("show");
}

function ImportFile(confirm = false) {

    if (_ID_CLASS === null || _ID_CLASS == 0) {
        $("#uploadModal").modal("hide");
        return;
    }

    var fileName = $("#excelFile").val();
    if (!fileName) {
        alert("Vui lòng chọn ít nhất 1 file");
        return;
    }

    $("#inputform").hide();
    $("#confirmform").hide();
    $("#loading").show();

    // Lấy file đưa vào payload
    var formData = new FormData();
    formData.append('excelFile', document.querySelector('#excelFile').files[0]);

    // Call api
    $.ajax({
        type: "POST",
        url: "/api/ScoreAPI/ImportScore?idClass=" + _ID_CLASS + "&idSemester=" + _ID_SEMESTER + "&confirm=" + confirm,
        processData: false,
        contentType: false,
        data: formData,
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN,
        },
        success: function (result) {
            if (confirm) {
                if (!result.IsSuccess) {
                    alert(result.Message);
                }
                else {
                    LoadClassScore().then(() => { UpdatedScore(result.Object); });


                    alert("Nhập điểm thành công");
                    $("#inputform").show();
                    $("#confirmform").hide();
                    $("#uploadModal").modal("hide");
                }
            }
            else {
                if (result.IsSuccess && (result.Message == null || result.Message == "")) {
                    ImportFile(true);
                } else if (result.IsSuccess) {
                    $("#inputform").hide();
                    $("#confirmform").show();
                    $("#missingMsg").html(result.Message);
                } else {
                    alert(result.Message);
                    $("#inputform").show();
                    $("#confirmform").hide();
                }
            }
        },
        complete: function () {
            $("#loading").hide();
        }
    });
}

function ExportScore() {
    if (_ID_CLASS === null || _ID_CLASS === 0) {
        return;
    }

    let viewType = $("#selectedViewType").val();
    location.href = "/quantri/xuatdiem/" + _ID_CLASS + "?viewType=" + viewType + "&idSemester=" + _ID_SEMESTER;
}