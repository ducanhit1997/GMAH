// Data để gửi lên api
var listSubject = [];

$(document).ready(function () {
    LoadSemesterData();
    LoadClassBySemesterId();
    LoadListSubject();
    LoadRuleData();
});

// Hàm load các list subject
function LoadListSubject() {
    return $.ajax({
        type: "GET",
        url: "/api/subjectapi/getallsubject",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
    }).then(function (result) {
        listSubject = [{
            IdSubject: 0,
            SubjectName: "Toàn bộ môn học",
        }, ...result.Object];
    });
}

// Hàm load data
function LoadRuleData() {
    $("#mainGroup").show();
    $("#loading").hide();

    $.ajax({
        type: "GET",
        url: "/api/GradeAPI/GetGradeRuleById?idRule=" + _ID_RULE,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            // Báo lỗi nếu call api fail
            if (!result.IsSuccess) {
                alert(result.Message);
                return;
            }

            // Nạp class và semester
            _ID_CLASS = result.IdClass;
            _ID_SEMESTER = result.IdSemester;

            $('#selectedSemester').val(_ID_SEMESTER).trigger('change');
            $('#selectedClass').val(_ID_CLASS).trigger('change');

            // Nạp các data luật xếp hạng
            var html = RenderRuleTab(result.Object);
            $("#ruleTab").html(html);
            RenderSubjectSelect();
        },
        complete: function () {
            $("#loading").hide();
            $("#mainGroup").show();
        },
    });
}

// Hàm render các tab xếp hạng
function RenderRuleTab(data) {
    var htmlHeader = '';
    var htmlBody = '';

    // Render từng tab
    for (let i = 0; i < data.length; i++) {
        htmlHeader += `
        <li class="nav-item">
            <a class="nav-link ${(i == 0 ? "active" : "")}" id="custom-tabs-${data[i].IdRank}-tab" data-toggle="pill" href="#custom-tabs-${data[i].IdRank}" role="tab" aria-controls="custom-tabs-${data[i].IdRank}" aria-selected="true">
                ${data[i].GradeName}
            </a>
        </li>`;

        htmlBody += RenderRuleBody(data[i], data[i].IdRank);
    }

    return `
            <div class="card card-primary card-outline card-outline-tabs">
              <div class="card-header p-0 border-bottom-0">
                <ul class="nav nav-tabs" id="custom-tabs-four-tab" role="tablist">
                    ${htmlHeader}
                </ul>
              </div>
              <div class="card-body">
                <div class="tab-content" id="custom-tabs-four-tabContent">
                    ${htmlBody}
                </div>
              </div>
            </div>`;
}

function RenderRuleBody(data, index) {
    var html = '';

    if (data.Details != null) {
        for (let i = 0; i < data.Details.length; i++) {
            html += RenderRuleRow(data.Details[i]);
        }
    }

    var behaviourList = ["Tốt", "Khá", "Trung bình", "Yếu"];
    var optionHtml = ``;
    for (let i = 0; i < behaviourList.length; i++) {
        let selectedText = "";
        if (i === data.IdBehaviour) {
            selectedText = "selected";
        }

        optionHtml += `<option value="${i}" ${selectedText}>${behaviourList[i]}</option>`;
    }

    return `
    <div class="tab-pane fade ${(index == 1 ? "show active" : "")}" id="custom-tabs-${index}" role="tabpanel" aria-labelledby="custom-tabs-${index}-tab">
        <div class="text-right">
            <button type="button" class="btn btn-primary" onclick="CreateNewRule(${index})"><i class="fas fa-plus"></i> Tạo luật mới</button>
        </div>
        <br>
        <div class="form-group">
            <label for="minAvgScore-${index}">Điểm trung bình tối thiểu cần đạt</label>
            <input type="number" class="form-control" id="minAvgScore-${index}" value="${data.MinAvgScore}">
        </div>
        <div class="form-group">
            <label for="minBehaviour-${index}">Hạnh kiểm tối thiểu cần đạt</label>
            <select class="form-control rounded-0" id="minBehaviour-${index}" name="minBehaviour-${index}" value="${data.IdBehaviour}">
                ${optionHtml}
            </select>
        </div>
        <br>
        <div id="ruleList${index}">
            <input value="${data.IdRank}" name="IdRank" hidden/>
            ${html}
        </div>
    </div>
    `;
}

function RenderRuleRow(data) {
    if (data == null) {
        data = {
            IdSubject: null,
            MinAvgScore: 0,
        };
    }

    return `
            <div class="row detail-data" style="padding-top: 5px">
                <div class="col-2">
                    Điểm của môn học
                </div>
                <div class="col">
                    <select class="form-control" name="IdSubject" value="${(data.IdSubject ?? 0)}">
                    </select>
                </div>
                <div class="col-2">
                    phải lớn hơn hoặc bằng
                </div>
                <div class="col">
                    <input type="number" class="form-control" name="MinAvgScore" placeholder="Điểm số" value="${data.MinAvgScore}">
                </div>
                <div class="col-1">
                    <button type="button" class="btn btn-block btn-danger" onclick="DeleteRule($(this))">Xoá luật</button>
                </div>
            </div>
        `;
}

function RenderSubjectSelect() {
    $("[name=IdSubject]").each(function (index) {
        if ($(this).data('select2')) return;

        let idSubject = $(this).attr("value");
        $(this).select2({
            placeholder: 'Chọn một giá trị',
            theme: 'bootstrap4'
        });        

        $(this).html($.map(listSubject, function (v, i) { return $('<option>', { val: v.IdSubject, text: v.SubjectName }); }));
        if (idSubject !== undefined) $(this).select2("val", idSubject);
    });
}

// Xoá luật
function DeleteRule(element) {
    if (!confirm("Bạn có muốn xoá luật này không?")) return;
    element.parent().parent().remove();
}

// Tạo luật mới
function CreateNewRule(index) {
    var html = RenderRuleRow(null);
    $("#ruleList" + index).append(html);
    RenderSubjectSelect();
}

// Hàm lưu dữ liệu
function SaveData() {
    // Kiểm tra dữ liệu
    var idSemester = $("#selectedSemester").val();
    var idClass = $("#selectedClass").val();

    if (idClass.length < 1) {
        alert("Vui lòng chọn các lớp được áp dụng");
        return;
    }

    // Payload api
    var payload = [];

    // Soạn dữ liệu
    let index = 1;
    while (true) {
        let tabIndex = $("#ruleList" + index);
        if (tabIndex.length == 0) {
            break;
        }

        let data = {
            IdRule: _ID_RULE,
            IdSemester: idSemester,
            IdRank: index,
            IdClass: idClass,
            MinAvgScore: $("#minAvgScore-" + index).val(),
            IdBehaviour: $("#minBehaviour-" + index).val(),
            Details: [],
        };

        // Add detail
        tabIndex.find(".detail-data").each(function () {
            let idSubject = $(this).find("[name=IdSubject]").val();
            if (idSubject == 0) idSubject = null;
            let minAvgScore = $(this).find("[name=MinAvgScore]").val();

            data.Details.push({
                IdSubject: idSubject,
                MinAvgScore: minAvgScore,
            });
        });

        payload.push(data);

        // Tăng biến đếm
        index++;
    }

    // Submit data
    $("#mainGroup").hide();
    $("#loading").show();

    $.ajax({
        type: "POST",
        url: "/api/GradeAPI/Save",
        data: {
            data: payload
        },
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Lưu thành công dữ liệu");
                location.href = linkListRule + "/" + _ID_SEMESTER;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#mainGroup").show();
        }
    });
}

// Hàm lấy thông tin học kỳ
function LoadSemesterData() {
    $('#selectedSemester').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    $.ajax({
        type: "GET",
        url: "/api/semesterapi/getcurrentsemester?viewSemester=2",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $("#selectedSemester").html("");

            if (result.Data !== undefined && result.Data !== null) {
                $('#selectedSemester').append($.map(result.Data, function (v, i) { return $('<option>', { val: v.IdSemester, text: v.TitleName }); }));
            }

            $('#selectedSemester').val(_ID_SEMESTER == 0 ? null : _ID_SEMESTER).trigger('change');
        }
    });
}

function LoadClassBySemesterId() {
    _ID_SEMESTER = $("#selectedSemester").val();
    LoadClassData();
}

// Hàm lấy thông tin danh sách lớp
function LoadClassData() {
    $('#selectedClass').select2({
        placeholder: 'Chọn một giá trị',
        theme: 'bootstrap4'
    });

    if (_ID_SEMESTER == null || _ID_SEMESTER == 0) {
        return;
    }

    $.ajax({
        type: "GET",
        url: "/api/ClassAPI/GetTeacherClassBySemester?IdYear=" + _ID_SEMESTER,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            $('#selectedClass').attr('disabled', false);
            $("#selectedClass").html("");

            if (result.Object !== undefined && result.Object !== null) {
                $('#selectedClass').append($.map(result.Object, function (v, i) { return $('<option>', { val: v.IdClass, text: v.ClassName }); }));
            }

            $('#selectedClass').val(_ID_CLASS == 0 ? null : _ID_CLASS).trigger('change');
        },
    });
}