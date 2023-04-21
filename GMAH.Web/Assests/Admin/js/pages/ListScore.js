$(document).ready(function () {
    LoadSemesterData();
    LoadClassData();
    LoadClassScore();
});

function LoadSemesterData() {
    $('#selectedSemester').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    if (_ID_SEMESTER === null || _ID_SEMESTER == 0) {
        $("#mainGroup").hide();
    }

    return $.ajax({
        type: "GET",
        url: "/api/semesterapi/getcurrentsemester",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
    }).then(function (result) {
        $("#selectedSemester").html("");

        if (result.Data !== undefined && result.Data !== null) {
            $('#selectedSemester').append($.map(result.Data, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
        }

        $('#selectedSemester').val(_ID_SEMESTER == 0 ? null : _ID_SEMESTER).trigger('change');
    });
}

function LoadClassBySemesterId() {
    _ID_SEMESTER = $("#selectedSemester").val();
    LoadClassData();
}

function LoadScoreByClassId() {
    _ID_CLASS = $("#selectedClass").val();
    LoadClassScore();
}

function LoadClassData() {
    $('#selectedClass').attr('disabled', true);
    $('#selectedClass').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    if (_ID_SEMESTER == null || _ID_SEMESTER == 0) {
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/ClassAPI/GetTeacherClassBySemester?IdSemester=" + _ID_SEMESTER + "&viewScore=true",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $('#selectedClass').attr('disabled', false);
            $("#selectedClass").html("");

            if (result.Object !== undefined && result.Object !== null) {
                if (result.Object.length > 0) _ID_YEAR = result.Object[0].IdYear;
                $('#selectedClass').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdClass, text: v.ClassName }); }));
            }

            $('#selectedClass').val(_ID_CLASS == 0 ? null : _ID_CLASS).trigger('change');
        },
    });
}

function LoadClassScore() {
    if (_ID_CLASS === null || _ID_CLASS == 0) {
        $("#mainGroup").hide();
        return;
    }

    $("#mainGroup").hide();
    $("#loading").show();

    let viewType = $("#selectedViewType").val();
    return $.ajax({
        type: "GET",
        url: "/api/ScoreAPI/GetClassScore?idClass=" + _ID_CLASS + "&idSemester=" + _ID_SEMESTER + "&viewType=" + viewType,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }

            // Render danh sách học sinh
            RenderTable(result);
        },
        complete: function () {
            if (viewType == 1) {
                $("#btnImportExcel").show();
                $("#btnSendClassMail").hide();
            }
            else {
                $("#btnImportExcel").hide();
                $("#btnSendClassMail").show();
            }
            $("#loading").hide();
            $("#mainGroup").show();
        }
    });
}

// Render function
function RenderTable(result) {
    let viewType = $("#selectedViewType").val();
    var htmlHeader = '';
    var htmlBody = '';

    // Render header
    var header1 = `<th colspan="3"></th>`;
    var header2 = '';
    header2 += `<th></th><th>MSHS</th><th>Họ tên học sinh</th>`;


    for (let i = 0; i < result.ScoreComponent.length; i++) {
        let component = result.ScoreComponent[i];
        let totalComponent = component.Column.length;

        header1 += `<th colspan="${totalComponent}">${component.SubjectName}</th>`;

        for (let j = 0; j < totalComponent; j++) {
            let scoreName = component.Column[j];
            header2 += `<th style="font-size: smaller;">${scoreName}</th>`;
        }
    }

    htmlHeader = `<tr>${header1}</tr>
                  <tr>${header2}</tr>`;

    // Render body
    for (let i = 0; i < result.Object.length; i++) {
        let studentData = result.Object[i];
        let tempHtml = '';

        // Render từng row
        if (viewType == 1) {
            tempHtml += `
                <td>
                    <button type="button" 
                            onclick="ViewHistory(${studentData.IdUser})"
                            class="btn btn-block btn-secondary btn-sm">
                        <i class='fas fa-history'></i>
                    </button>
                </td>
            `;
        }
        else {
            // Gửi mail
            tempHtml += `
                <td>
                    <button type="button"
                            onclick="OpenSendMailModal(${studentData.IdUser})"
                            class="btn btn-block btn-secondary btn-sm">
                        <i class='fas fa-envelope'></i>
                    </button>
                </td>
            `;
        }

        tempHtml += `
            <td>${studentData.StudentCode}</td>
            <td>${studentData.StudentName}</td>
        `;

        // Render score component
        for (let j = 0; j < result.ScoreComponent.length; j++) {
            let component = result.ScoreComponent[j];
            let subjectName = component.SubjectName;
            let totalComponent = component.Column.length;
            let subjectData = studentData.Subjects.find(x => x.SubjectName === subjectName);

            for (let m = 0; m < totalComponent; m++) {
                let scoreName = component.Column[m];
                let idScoreType = component.ColumnId[m];
                let isReadOnly = false;
                let isOption = false;
                let note = null;
                let listOption = null;
                let selectedOption = "null";

                let score = '';
                if (subjectData !== null && subjectData !== undefined) {
                    var scoreData = subjectData.Details.find(x => x.ScoreName === scoreName);
                    if (scoreData !== null && scoreData !== undefined) {
                        score = scoreData.Score;
                        isReadOnly = scoreData.IsReadOnly;
                        isOption = scoreData.IsOption;
                        listOption = scoreData.ListOption;
                        note = scoreData.Note;
                        selectedOption = scoreData.SelectedValueOption;

                        if (scoreData.Text !== null && isReadOnly == true) {
                            score = scoreData.Text;
                        }
                    }
                }

                if (score == undefined || score === null) score = '';

                if (isOption == true) {
                    if (!isReadOnly) {
                        var optionHtml = ``;
                        for (let o = 0; o < listOption.length; o++) {
                            let selectedText = "";
                            if (selectedOption === listOption[o].Value) {
                                selectedText = "selected";
                            }

                            optionHtml += `<option value="${listOption[o].Value}" ${selectedText}>${listOption[o].Text}</option>`;
                        }

                        tempHtml += `
                        <td>
                            <select class="form-control rounded-0 option-input"
                                    style="min-width: 100px;"
                                    id="option-${studentData.IdUser}-${subjectData.IdSemester}"
                                    onchange="SaveOption(${studentData.IdUser}, '${studentData.StudentName}', ${subjectData.IdSemester}, ${subjectData.IdYear})"
                                    value="${selectedOption}">
                                ${optionHtml}
                            </select>
                        </td>
                        `;
                    }
                    else {
                        tempHtml += `
                        <td>
                            ${score}
                        </td>
                        `;
                    }
                }
                else if (isReadOnly == true) {
                    tempHtml += `
                    <td>
                        ${score}
                    </td>
                    `;
                }
                else {
                    let newScoreName = scoreName.replaceAll("'", "");
                    let noteHtml = ``;
                    if (note != null) {
                        noteHtml = `
                          <div class="input-group-prepend" data-toggle="tooltip" data-placement="top" title="${note}">
                            <span class="input-group-text"><i class="fas fa-info-circle"></i></span>
                          </div>`;
                    }

                    tempHtml += `
                    <td>
                        <div class="input-group mb-3">
                            <input id-user="${studentData.IdUser}" id-score="${idScoreType}" class="form-control form-control-sm score-input"
                                    style="min-width: 60px"
                                    id="score-${studentData.IdUser}-${subjectData.IdSubject}-${m}"
                                    onchange="SaveScore(${studentData.IdUser}, '${studentData.StudentName}', ${subjectData.IdSubject}, '${subjectName}', ${m}, '${newScoreName}', '${score}', ${idScoreType})"
                                    type="number"
                                    value="${score}">
                            ${noteHtml}
                        </div>
                    </td>`;
                }
            }
        }

        htmlBody += `<tr>${tempHtml}</tr>`;
    }

    // Đưa vào document
    $("#result").find("[name=header]").html(htmlHeader);
    $("#result").find("[name=body]").html(htmlBody);

    // Add event
    $('.score-input').on('focusin', function () {
        $(this).attr('oldVal', $(this).val());
    });

    $('.option-input').on('focusin', function () {
        $(this).attr('oldVal', $(this).val());
    });
}

function SaveOption(idUser, studentName, idSemester, idYear) {
    var optionInput = $("#option-" + idUser + "-" + idSemester);
    var oldScore = optionInput.attr("oldVal");
    var newScore = optionInput.val();

    if (oldScore === undefined) oldScore = "null";

    if (!confirm(`Bạn có muốn cập nhật hạnh kiểm cho học sinh ${studentName} không?`)) {
        optionInput.val(oldScore);
        return;
    }

    // Submit kết quả
    optionInput.prop('disabled', true);
    $.ajax({
        type: "POST",
        url: "/api/ScoreAPI/SaveStudentBehaviour",
        dataType: 'json',
        data: {
            IdUser: idUser,
            IdSemester: idSemester,
            IdYear: idYear,
            IdBehaviour: newScore,
        },
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                Toastify({
                    text: result.Message,
                    gravity: 'bottom',
                    position: 'center',
                    className: "bg-danger",
                }).showToast();
                return;
            }

            Toastify({
                text: 'Hạnh kiểm của học sinh đã được lưu lại',
                gravity: 'bottom',
                position: 'center',
                className: "bg-info",
            }).showToast();

            RefreshData();
        },
        complete: function () {
            optionInput.prop('disabled', false);
        }
    });
}

function SaveScore(idUser, studentName, idSubject, subjectName, indexScore, scoreName, oldScore, idScoreType) {
    var scoreInput = $("#score-" + idUser + "-" + idSubject + "-" + indexScore);
    var oldScore = scoreInput.attr("oldVal");
    var newScore = scoreInput.val();

    if (oldScore === undefined) oldScore = "";

    if (!confirm(`Bạn có muốn cập nhật điểm cho học sinh ${studentName}, môn học ${subjectName}, cập nhật ${scoreName} từ '${oldScore}' thành '${newScore}' không?`)) {
        scoreInput.val(oldScore);
        return;
    }

    // Submit kết quả
    scoreInput.prop('disabled', true);
    $.ajax({
        type: "POST",
        url: "/api/ScoreAPI/SaveStudentScore",
        dataType: 'json',
        data: {
            IdUser: idUser,
            IdSubject: idSubject,
            ScoreTypeId: idScoreType,
            IdSemester: _ID_SEMESTER,
            ScoreType: indexScore,
            Score: newScore,
        },
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                Toastify({
                    text: result.Message,
                    gravity: 'bottom',
                    position: 'center',
                    className: "bg-danger",
                }).showToast();
                return;
            }

            Toastify({
                text: 'Điểm của học sinh đã được lưu lại',
                gravity: 'bottom',
                position: 'center',
                className: "bg-info",
            }).showToast();

            RefreshData().then(() => { UpdatedScore(result.Object); });

        },
        complete: function () {
            scoreInput.prop('disabled', false);
        }
    });
}

function OnChangedViewType() {
    LoadClassScore();
}

function RefreshData() {
    return LoadClassScore();
}

function UpdatedScore(list) {
    for (let i = 0; i < list.length; i++) {
        let updatedScore = list[i];
        $(`input[id-user=${updatedScore.IdUser}][id-score=${updatedScore.IdScoreType}]`).css('background', 'yellow');
    }
}