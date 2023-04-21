$(document).ready(function () {
    LoadSemesterData();
    LoadTeacherData();
    LoadListSubject();
});

function GetClass(id) {
    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/classapi/getclass/" + id,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                location.href = linkList;
                return;
            }
            else {

                $("[name=ClassName]").val(result.Object.ClassName);
                $("[name=IdFormTeacher]").val(result.Object.IdFormTeacher).trigger('change');
                $("[name=IdSemester]").val(result.Object.IdYear).trigger('change');

                SetSubjectTeacherOption(result.Object.Subject);

                _ID_CLASS = id;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}

function SaveClass() {
    $("#inputform").hide();
    $("#loading").show();
    let subjectPayload = GetSubjectTeacherOption();

    $.ajax({
        type: "POST",
        url: "/api/classapi/saveclass",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        data: {
            IdClass: _ID_CLASS,
            ClassName: $("[name=ClassName]").val(),
            IdFormTeacher: $("[name=IdFormTeacher]").val(),
            IdYear: $("[name=IdSemester]").val(),
            Subject: subjectPayload,
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Lưu dữ liệu thành công");
                location.href = linkList;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}

function LoadSemesterData() {
    $("#selectedSemester").html("");
    $("#IdSemester").html("");

    $.ajax({
        type: "GET",
        url: "/api/semesterapi/getcurrentsemester?viewSemester=2",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Data !== undefined && result.Data !== null) {
                $('#IdSemester').append($.map(result.Data, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
            }

            $('#IdSemester').select2({
                placeholder: 'Chọn một giá trị',
                theme: 'bootstrap4'
            });

            $('#IdSemester').val(_ID_SEMESTER).trigger('change');
        }
    });
}

// Hàm load các list subject
function LoadListSubject() {
    return $.ajax({
        type: "GET",
        url: "/api/subjectapi/GetAllSubjectAndTeacher",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
    }).then(function (result) {
        if (result.Object !== undefined && result.Object !== null) {
            RenderSubjectCheckbox(result.Object);
        }

        if (_ID_CLASS != 0) {
            GetClass(_ID_CLASS);
        }
    });
}

function LoadTeacherData() {

    $.ajax({
        type: "GET",
        url: "/api/userapi/GetAllTeacher",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Object !== undefined && result.Object !== null) {
                $('#IdFormTeacher').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdTeacher, text: v.Fullname }); }));
            }
            $("[name=IdFormTeacher]").val(null).trigger('change');

            $('#IdFormTeacher').select2({
                placeholder: 'Chọn một giá trị',
                theme: 'bootstrap4'
            });
        }
    });
}

// Render subject checkbox
function RenderSubjectCheckbox(data) {
    var html = ``;

    for (let i = 0; i < data.length; i++) {
        let subjectData = data[i];
        let listTeacherOption = ``;
        for (let j = 0; j < subjectData.Teacher.length; j++) {
            let teacherData = subjectData.Teacher[j];
            listTeacherOption += `<option value="${teacherData.IdTeacherSubject}">${teacherData.Fullname}</option>`;
        }

        var subjectHtml = `
            <div class="row teachersubjectdiv"
                 style="padding-top: 8px"
                 idSubject="${subjectData.Subject.IdSubject}">
                <div class="col-1">
                    <div class="icheck-primary d-inline">
                        <input type="checkbox" onchange="OnChangedCheckboxSubject(this, ${subjectData.Subject.IdSubject})" id="checkboxsubject-${subjectData.Subject.IdSubject}" name="checkboxsubject-${subjectData.Subject.IdSubject}">
                        <label for="checkboxsubject-${subjectData.Subject.IdSubject}">
                            ${subjectData.Subject.SubjectName}
                        </label>
                    </div>
                </div>
                <div class="col-2">
                    giảng dạy bởi
                </div>
                <div class="col">
                    <select class="form-control selectteacher" name="selectteacher-${subjectData.Subject.IdSubject}" id="selectteacher-${subjectData.Subject.IdSubject}">
                        ${listTeacherOption}
                    </select>
                </div>
            </div>
        `;

        html += subjectHtml;
    }

    $("#subjectCheckbox").html(html);

    // Add event
    $(".selectteacher").each(function (index) {
        if ($(this).data('select2')) return;

        $(this).select2({
            placeholder: 'Chọn một giá trị',
            allowClear: true,
            theme: 'bootstrap4'
        });
    });
}

function OnChangedCheckboxSubject(cb, idSubject) {
    $('#selectteacher-' + idSubject).attr('disabled', !cb.checked);
}

function GetSubjectTeacherOption() {
    let payload = [];
    var elements = $(".teachersubjectdiv");

    for (let i = 0; i < elements.length; i++) {
        let element = $(elements[i]);
        let idSubject = element.attr("idSubject");
        let checkbox = element.find("[name=checkboxsubject-" + idSubject + "]");
        let teacher = element.find("[name=selectteacher-" + idSubject + "]");

        if (!checkbox.is(":checked")) {
            continue;
        }

        let idTeacherSubject = teacher.val();
        payload.push({
            IdSubject: idSubject,
            IdTeacherSubject: idTeacherSubject,
        });
    }

    return payload;
}

function SetSubjectTeacherOption(data) {
    // Disable all control
    var elements = $(".teachersubjectdiv");

    for (let i = 0; i < elements.length; i++) {
        let element = $(elements[i]);
        let idSubject = element.attr("idSubject");
        let checkbox = element.find("[name=checkboxsubject-" + idSubject + "]");
        let teacher = element.find("[name=selectteacher-" + idSubject + "]");

        checkbox.prop('checked', false);
        teacher.val(null).trigger('change');
        teacher.attr('disabled', true);
    }

    // Enable các control từ api
    for (let i = 0; i < data.length; i++) {
        let subjectData = data[i];
        let idSubject = subjectData.IdSubject;

        let checkbox = $("[name=checkboxsubject-" + idSubject + "]");
        let teacher = $("[name=selectteacher-" + idSubject + "]");

        checkbox.prop('checked', true);
        teacher.val(subjectData.IdTeacherSubject).trigger('change');
        teacher.attr('disabled', false);
    }
}