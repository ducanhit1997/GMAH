var dataTable = $("#result").DataTable({
    "language": {
        "url": "/Assests/Data/datatable-vi.json"
    },
    "paging": true,
    "lengthChange": true,
    "searching": true,
    "info": true,
    "autoWidth": true,
    "responsive": true,
    "processing": true,
    "serverSide": true,
    "ordering": false,
    ajax: {
        url: '/api/reportapi/GetReviewReport',
        type: 'POST',
        headers: {
            "Authorization": "Bearer " + _JWT_TOKEN
        },
    },
    columns: [
        { data: "IdReport" },
        { data: "ReportTypeName" },
        { data: "ReportTitle" },
        { data: "FullnameSubmitReport" },
        { data: "FullnameStudent" },
        { data: "SubmitDateString" },
        { data: "ReportStatusName" },

        {
            data: null,
            className: "text-center editor-edit",
            defaultContent: '<i class="fas fa-edit"></i>',
            orderable: false,
            width: "50px",
        },
    ]
});

$('#result tbody').on('click', '.editor-edit', function () {
    let row = dataTable.row($(this).closest("tr")).data();
    location.href = linkInfo + '/' + row.IdReport;
});

$(document).ready(function () {
    LoadReportData();
});

function LoadReportData() {
    $("#mainGroup").hide();
    $("#loading").show();

    dataTable.ajax.url('/api/reportapi/GetReviewReport');
    dataTable.ajax.reload();

    $("#mainGroup").show();
    $("#loading").hide();
}
