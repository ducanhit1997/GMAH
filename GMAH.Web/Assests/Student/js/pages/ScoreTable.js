
// Render function
function RenderTable(result) {
    let viewType = $("#selectedViewType").val();
    var htmlHeader = '';
    var htmlBody = '';

    // Render header
    var header1 = `<th colspan="${(viewType == 1 ? 2 : 2)}"></th>`;
    var header2 = '';

    header2 += `<th>MSHS</th><th>Họ tên học sinh</th>`;


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
                let isReadOnly = false;

                let score = '';
                if (subjectData !== null && subjectData !== undefined) {
                    var scoreData = subjectData.Details.find(x => x.ScoreName === scoreName);
                    if (scoreData !== null && scoreData !== undefined) {
                        score = scoreData.Score;
                        isReadOnly = scoreData.IsReadOnly;
                        isOption = scoreData.IsOption;
                        listOption = scoreData.ListOption;
                        selectedOption = scoreData.SelectedValueOption;

                        if (scoreData.Text !== null) {
                            score = scoreData.Text;
                        }
                    }
                }

                if (score == undefined || score === null) score = '';

                tempHtml += `
                    <td>
                        ${score}
                    </td>
                    `;
            }
        }

        htmlBody += `<tr>${tempHtml}</tr>`;
    }

    // Đưa vào document
    $("#mainGroup").find("[name=header]").html(htmlHeader);
    $("#mainGroup").find("[name=body]").html(htmlBody);
}
