var historyLoading = $("#historyModal").find("[name=loading]");
var historyDiv = $("#historyModal").find("[name=inputform]");
var historyResult = $("#historyResult");
var _LOGS = [];
var _ID_SUBJECT_HISTORY = null;

document.addEventListener("DOMContentLoaded", function (event) {
    $('#selectedHistorySubject').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });
});

function ViewHistory(idUser) {
    $("#historyModal").modal("show");
    ShowLoading(true);
    _LOGS = [];

    $.ajax({
        type: "GET",
        url: "/api/scoreapi/GetScoreLog?idUser=" + idUser + "&idSemester=" + _ID_SEMESTER,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                $("#historyModal").modal("hide");
                return;
            }

            // Render select box
            $('#selectedHistorySubject').html($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdSubject, text: v.SubjectName }); }));
            if (result.Object.length > 0) {
                let firstValue = result.Object[0].IdSubject;
                _ID_SUBJECT_HISTORY = firstValue;
            }
            else {
                _ID_SUBJECT_HISTORY = null;
            }

            $('#selectedStudent').val(_ID_SUBJECT_HISTORY).trigger('change');

            // Render log
            _LOGS = result.Object;
            LoadHistoryBySubject();
        },
        complete: function () {
            ShowLoading(false);
        }
    });
}

function LoadHistoryBySubject() {
    let idSubject = $("#selectedHistorySubject").val();
    historyResult.html(`<p class="text-center">Không có dữ liệu</p>`);
    if (idSubject == null) {
        return;
    }

    let listLog = _LOGS.find(x => x.IdSubject == idSubject);
    if (listLog === undefined || listLog === null) {
        return;
    }

    // Render logs
    RenderLogs(listLog.Logs);
}

function RenderLogs(listLog) {
    let htmlTimeline = ``;

    for (let i = 0; i < listLog.length; i++) {
        let log = listLog[i];

        htmlTimeline += `
                <li>
					<a target="_blank" href="#">${log.UpdateByName}</a>
					<a href="#" class="float-right">${log.DateUpdate}</a>
					<p>${log.Log}</p>
				</li>
        `;
    }

    htmlTimeline = `<ul class="timeline">${htmlTimeline}</ul>`;
    historyResult.html(htmlTimeline);
}

function ShowLoading(show) {
    if (show) {
        historyLoading.show();
        historyDiv.hide();
    }
    else {
        historyLoading.hide();
        historyDiv.show();
    }
}