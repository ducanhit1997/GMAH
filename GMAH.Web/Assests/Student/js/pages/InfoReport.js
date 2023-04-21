$(document).ready(function () {
    GetReport(_ID_REPORT);
});

function GetReport(id) {
    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/reportapi/getreport/" + id,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                $("#inputform").html("Lỗi");
                return;
            }
            else {
                $("#FullnameSubmitReport").html(result.Object.FullnameSubmitReport);
                $("#FullnameStudent").html(result.Object.FullnameStudent);
                $("#SubmitDateString").html(result.Object.SubmitDateString);
                $("#ReportStatusName").html(result.Object.ReportStatusName);
                $("#ReportTitle").html(result.Object.ReportTitle);
                $("#ReportContent").html(result.Object.ReportContent);
                $("#LastUpdateDateString").html(result.Object.LastUpdateDateString);
                $("#Issue").html(result.Object.Issue);

                RenderReportHistory(result.Object.History);
                RenderReportFiles(result.Object.Files);

                _ID_REPORT = id;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}

function RenderReportFiles(listFile) {
    let htmlFiles = ``;

    for (let i = 0; i < listFile.length; i++) {
        let file = listFile[i];
        let link = "/xemfile/" + file;

        htmlFiles += `
                <div class="col-4">
					<div class="card" style="cursor: pointer;" onclick="OpenFile('${link}')">
                      <div class="card-body">
                        <i class="fas fa-file-alt"></i> ${file}
                      </div>
                    </div>
				</div>
        `;
    }

    $("#ReportFile").html(htmlFiles);
}

function OpenFile(url) {
    window.open(url, '_blank');
}

function RenderReportHistory(listLog) {
    let htmlTimeline = ``;

    if (listLog.length < 1) {
        $("#HistoryDiv").hide();
    }
    else {
        $("#HistoryDiv").show();
    }

    for (let i = 0; i < listLog.length; i++) {
        let log = listLog[i];
        if (log.Comment == null) log.Comment = "<Không có nội dung>";
        htmlTimeline += `
                <li>
					<a target="_blank" href="#">${log.FullnameUserUpdate}</a>
					<a href="#" class="float-right">${log.HistoryDateString}</a>
                    <b>${log.ReportStatusName}</b>
					<p>${log.Comment}</p>
				</li>
        `;
    }

    htmlTimeline = `<ul class="timeline">${htmlTimeline}</ul>`;
    $("#ReportHistory").html(htmlTimeline);
}
