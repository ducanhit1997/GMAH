$(document).ready(function () {
    LoadSemesterData();
    LoadStudentData();

    $('#DateAttendance').datetimepicker({
        format: 'L'
    });
    $("#DateAttendance").datetimepicker("date", new Date());
});

ClassicEditor
    .create(document.querySelector('#ReportContent'), {
        toolbar: {
            items: [
                'heading',
                'fontFamily',
                'fontSize',
                'fontColor',
                'fontBackgroundColor',
                'highlight',
                '|',
                'alignment',
                'bold',
                'italic',
                'underline',
                'link',
                'bulletedList',
                'numberedList',
                '|',
                'outdent',
                'indent',
                '|',
                'imageUpload',
                'blockQuote',
                'insertTable',
                'mediaEmbed',
                'undo',
                'redo'
            ]
        },
        language: 'vi',
        image: {
            toolbar: [
                'imageTextAlternative',
                'imageStyle:full',
                'imageStyle:side'
            ]
        },
        table: {
            contentToolbar: [
                'tableColumn',
                'tableRow',
                'mergeTableCells'
            ]
        },
        licenseKey: '',


    })
    .then(editor => {
        window.editor = editor;
    })
    .catch(error => {
        console.error('Oops, something went wrong!');
        console.error('Please, report the following error on https://github.com/ckeditor/ckeditor5/issues with the build id and the error stack trace:');
        console.warn('Build id: ik3ej8cbcmlo-rl2o0zrya9to');
        console.error(error);
    });

function LoadReportField() {
    $("#field1").hide();
    $("#field2").hide();

    var type = $("#status").val();
    $("#field" + type).show();
}

function LoadSemesterData() {
    $("#selectedSemester").html("");

    $.ajax({
        type: "GET",
        url: "/api/semesterapi/getcurrentsemester",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Data !== undefined && result.Data !== null) {
                $('#selectedSemester').append($.map(result.Data, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
            }

            $('#selectedSemester').select2({
                placeholder: 'Chọn học kỳ',
                theme: 'bootstrap4'
            });

            _ID_SEMESTER = result.SelectedId;
            $('#selectedSemester').val(_ID_SEMESTER).trigger('change');
        }
    });
}

function LoadStudentData() {
    if ($("#selectedStudent").length <= 0) return;

    $("#selectedStudent").html("");

    $.ajax({
        type: "GET",
        url: "/api/viewscoreapi/getlistchild",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedStudent').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdUser, text: v.Fullname }); }));
            }

            $('#selectedStudent').select2({
                placeholder: 'Chọn học sinh',
                theme: 'bootstrap4'
            });

            $('#selectedStudent').val(null).trigger('change');
        }
    });
}

function LoadScoreType() {
    $("#selectedScoreType").html("");
    let idSemester = $("#selectedSemester").val();
    let idStudent = $("#selectedStudent").val();
    let idSubject = $("#selectedSubject").val();

    if ($("#selectedStudent").length <= 0) idStudent = _ID_STUDENT;

    if (idSemester == null || idStudent == null || idSubject == null) {
        $('#selectedScoreType').select2({
            placeholder: 'Chọn cột điểm',
            theme: 'bootstrap4'
        });

        $('#selectedScoreType').val(null).trigger('change');
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/reportapi/GetScoreType?idSemester=" + idSemester + "&idStudent=" + idStudent + "&idSubject=" + idSubject,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedScoreType').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdScoreType, text: v.ScoreName }); }));
            }

            $('#selectedScoreType').select2({
                placeholder: 'Chọn cột điểm',
                theme: 'bootstrap4'
            });

            $('#selectedScoreType').val(null).trigger('change');
        }
    });
}

function LoadSubject() {
    $("#selectedSubject").html("");
    let idSemester = $("#selectedSemester").val();
    let idStudent = $("#selectedStudent").val();
    if ($("#selectedStudent").length <= 0) idStudent = _ID_STUDENT;

    if (idSemester == null || idStudent == null) {
        $('#selectedSubject').select2({
            placeholder: 'Chọn môn học',
            theme: 'bootstrap4'
        });

        $('#selectedSubject').val(null).trigger('change');
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/reportapi/GetSubjectInClass?idSemester=" + idSemester + "&idStudent=" + idStudent,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedSubject').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdSubject, text: v.SubjectName }); }));
            }

            $('#selectedSubject').select2({
                placeholder: 'Chọn môn học',
                theme: 'bootstrap4'
            });

            $('#selectedSubject').val(null).trigger('change');
        }
    });
}

function GetEditValueData() {
    var type = $("#status").val();
    if (type == 1) {
        let idSemester = $("#selectedSemester").val();
        let idStudent = $("#selectedStudent").val();
        let idSubject = $("#selectedSubject").val();
        let idScoreType = $("#selectedScoreType").val();

        var editValue = $("#editvalue1").val();
        if ($("#selectedStudent").length <= 0) idStudent = _ID_STUDENT;

        if (idSemester == null || idStudent == null || idSubject == null || editValue == "") {
            alert("Vui lòng chọn đủ thông tin bên trên");
            return null;
        }

        return {
            IdSemester: idSemester,
            IdStudent: idStudent,
            IdSubject: idSubject,
            IdScoreType: idScoreType,
            Value: editValue,
        };
    }
    else {
        let idSemester = $("#selectedSemester").val();
        var date = $("#DateAttendance").datetimepicker("date")?.format('yyyy-MM-DDT00:00:00') ?? null;
        var editValue = $("#editvalue2").val();

        if (idSemester == null || date == null || editValue == "") {
            alert("Vui lòng chọn đủ thông tin bên trên");
            return null;
        }

        return {
            IdSemester: idSemester,
            Date: date,
            Value: editValue,
        };
    }
}

function SubmitReport() {
    var dataEditValue = GetEditValueData();
    if (dataEditValue == null) {
        return;
    }

    $("#inputform").hide();
    $("#loading").show();

    let idStudent = $("#selectedStudent").val();
    if ($("#selectedStudent").length <= 0) idStudent = _ID_STUDENT;

    var formData = new FormData();
    jQuery.each(jQuery('#files')[0].files, function (i, file) {
        formData.append('file' + i, file);
    });
    formData.append('EditField', JSON.stringify(dataEditValue));
    formData.append('EditValue', dataEditValue.Value);
    formData.append('ReportTitle', $("#ReportTitle").val());
    formData.append('ReportContent', window.editor.getData());
    formData.append('SubmitForIdUser', idStudent);
    formData.append('ReportType', $("#status").val());

    // Call api
    $.ajax({
        type: "POST",
        url: "/api/reportapi/submitreport",
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
                alert("Gửi báo cáo thành công");
                location.href = linkReport;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}