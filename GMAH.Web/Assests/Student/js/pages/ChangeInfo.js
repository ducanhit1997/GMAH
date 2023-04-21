function OpenInfoModal() {
    $('#userPhone').val("Đang tải...");
    $('#userEmail').val("Đang tải...");
    $('#userFullname').val("Đang tải...");
    $('#userCitizenID').val("Đang tải...");

    $('#userEmail').prop('readonly', true);
    $('#userPhone').prop('readonly', true);
    $('#btnSubmitInfo').prop('readonly', true);
    $('#infoModal').modal("show");

    $.ajax({
        type: "GET",
        url: "/api/userapi/viewinfo",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                $('#infoModal').modal("hide");
            }
            else {
                $("#userFullname").val(result.Object.Fullname);
                $("#userPhone").val(result.Object.Phone);
                $("#userEmail").val(result.Object.Email);
                $("#userAddress").val(result.Object.Address);
                $("#userCitizenID").val(result.Object.CitizenID);
            }
        },
        complete: function () {
            $('#userEmail').prop('readonly', false);
            $('#userPhone').prop('readonly', false);
            $('#btnSubmitInfo').prop('readonly', false);
        }
    });
    
}

function SubmitChangeInfo() {
    $('#userEmail').prop('readonly', true);
    $('#userPhone').prop('readonly', true);
    $('#btnSubmitInfo').prop('readonly', true);


    $.ajax({
        type: "POST",
        url: "/api/userapi/updateinfo",
        data: {
            Email: $("#userEmail").val(),
            Phone: $("#userPhone").val(),
            Address: $("#userAddress").val(),
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
                alert("Đổi thông tin thành công");
                $('#infoModal').modal("hide");
            }
        },
        complete: function () {
            $('#userEmail').prop('readonly', false);
            $('#userPhone').prop('readonly', false);
            $('#btnSubmitInfo').prop('readonly', false);
        }
    });
}