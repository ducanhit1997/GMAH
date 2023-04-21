
ClassicEditor
    .create(document.querySelector('#Comment'), {
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

var _UPDATE_STATUS = 0;

function Approve() {
    _UPDATE_STATUS = 100;
    SubmitReview();
}

function Reject() {
    _UPDATE_STATUS = 101;
    SubmitReview();
}

function SubmitReview() {
    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "POST",
        url: "/api/reportapi/SubmitReviewReport",
        dataType: 'json',
        data: {
            IdReport: _ID_REPORT,
            ReportStatus: _UPDATE_STATUS,
            Comment: window.editor.getData(),
        },
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                return;
            }
            else {
                alert(_UPDATE_STATUS = 100 ? "Duyệt báo cáo thành công" : "Đã từ chối báo cáo này");
                location.href = linkListReport;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}