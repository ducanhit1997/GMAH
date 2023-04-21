document.getElementById('excelFile').addEventListener('change', function (e) {
    var fileName = document.getElementById("excelFile").files[0].name;
    var nextSibling = e.target.nextElementSibling
    nextSibling.innerText = fileName
})

function OpenModalImportTimeline() {
    $("#excelFile").val(null);
    $("#uploadModal").modal("show");
}

function ImportFile() {
    if (_ID_CLASS === null || _ID_CLASS == 0) {
        $("#uploadModal").modal("hide");
        return;
    }

    var fileName = $("#excelFile").val();
    if (!fileName) {
        alert("Vui lòng chọn ít nhất 1 file");
        return;
    }

    $("#uploadMainGroup").hide();
    $("#uploadLoading").show();

    // Lấy file đưa vào payload
    var formData = new FormData();
    formData.append('excelFile', document.querySelector('#excelFile').files[0]);

    // Call api
    $.ajax({
        type: "POST",
        url: "/api/TimelineAPI/ImportTimeLine?idClass=" + _ID_CLASS + "&idSemester=" + _ID_SEMESTER,
        processData: false,
        contentType: false,
        data: formData,
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN,
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                LoadTimelineDateRangeViewModel();
                alert("Nhập thời khoá biểu thành công");
                $("#uploadModal").modal("hide");
            }
        },
        complete: function () {
            $("#uploadLoading").hide();
            $("#uploadMainGroup").show();
        }
    });
}