function RenderAttendance(data) {
    var html = ``;

    var headerHtml = `<th>MSHS</th>
                    <th>Họ tên</th>
                    <th>Có mặt</th>
                    <th>Có phép</th>
                    <th>Vắng mặt</th>`;
    var bodyHtml = ``;

    for (let i = 0; i < data.Students.length; i++) {
        let studentData = data.Students[i];

        let option1 = `<div class="form-check">
                          <input class="form-check-input" type="radio" name="radioAttendance${studentData.IdStudent}" value="0" ${(studentData.AttendanceStatus == 0 ? "checked" : "")}>
                        </div>`;
        let option2 = `<div class="form-check">
                          <input class="form-check-input" type="radio" name="radioAttendance${studentData.IdStudent}" value="1" ${(studentData.AttendanceStatus == 1 ? "checked" : "")}>
                        </div>`;
        let option3 = `<div class="form-check">
                          <input class="form-check-input" type="radio" name="radioAttendance${studentData.IdStudent}" value="2" ${(studentData.AttendanceStatus == 2 ? "checked" : "")}>
                        </div>`;

        let rowHtml = `<th scope="row">${studentData.StudentCode}</th>
                       <td>${studentData.Fullname}</td>
                       <td>${option1}</td>
                       <td>${option2}</td>
                       <td>${option3}</td>`;

        bodyHtml += `<tr idStudent="${studentData.IdStudent}" class="attendance-row">${rowHtml}</tr>`;
    }

    attendenceBy = ``;
    if (data.AssistantID > 0) {
        attendenceBy = `<i>Ngày học này được điểm danh bởi <b>${data.AssistantName}</b></i>`;
    }
    else {
        attendenceBy = `<b style="color: red">Ngày học này chưa có thông tin điểm danh</b>`;
    }

    html = `${attendenceBy}
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