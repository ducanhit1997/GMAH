function RenderTimeline(data) {
    var html = ``;

    var headerHtml = `<th></th>`;
    var bodyHtml = ``;

    for (let i = 0; i < data.length; i++) {
        headerHtml += `<th scope="col" style="text-align: center;">${data[i].DayOfWeek}<br>${data[i].DateString}</th>`;
    }

    // Tbody
    for (let j = 1; j <= 10; j++) {
        let rowHtml = `<th scope="row">Tiết ${j}</th>`;
        for (let i = 0; i < data.length; i++) {
            let subjectData = data[i].Detail.find(x => x.Period == j);

            if (subjectData !== null && subjectData !== undefined) {
                rowHtml += `<td style="text-align: center;">${subjectData.SubjectName}<br>${subjectData.TeacherFullname}</td>`;
            }
            else {
                rowHtml += `<td></td>`;
            }
        }
        bodyHtml += `<tr>${rowHtml}</tr>`;
    }


    html = `<table class="table table-bordered">
              <thead class="thead-light">
                ${headerHtml}
              </thead>
              <tbody>
                ${bodyHtml}
              </tbody>
            </table>`;

    $("#timelineData").html(html);
}