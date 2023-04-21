function RenderAttendance(data) {
    var html = ``;
    if (data == null || data.Headers == null || data.Students == null) {
        $("#attendanceHTML").html(html);
        return;
    }

    var headerHtml = `<th>MSHS</th>
                    <th>Họ tên</th>`;
    var bodyHtml = ``;
    for (let i = 0; i < data.Headers.length; i++) {
        let dateString = data.Headers[i];
        headerHtml += `<th>${dateString}</th>`;
    }

    for (let j = 0; j < data.Students.length; j++) {
        let studentData = data.Students[j];
        let rowHtml = `<th scope="row">${studentData.StudentCode}</th>
                       <td>${studentData.Fullname}</td>`;

        for (let i = 0; i < data.Headers.length; i++) {
            let dateString = data.Headers[i];
            let studentAttendanceData = data.StudentData.find(x => x.IdStudent == studentData.IdUser && x.DateString == dateString);
            if (studentAttendanceData.AssistantName != null) {
                rowHtml += `<td class="attendance${studentAttendanceData.AttendanceStatus}" title="Điểm danh bởi ${studentAttendanceData.AssistantName}"></td>`;
            }
            else {
                rowHtml += `<td class="attendance${studentAttendanceData.AttendanceStatus}"></td>`;
            }
        }

        bodyHtml += `<tr>${rowHtml}</tr>`;
    }

    html = `<i class="fas fa-circle attendance0"></i> Hiện diện <i class="fas fa-circle attendance1"></i> Vắng có phép <i class="fas fa-circle attendance2"></i> Vắng không phép
            <table class="table table-bordered">
              <thead class="thead-light">
                ${headerHtml}
              </thead>
              <tbody>
                ${bodyHtml}
              </tbody>
            </table>`;

    $("#attendanceHTML").html(html);
}