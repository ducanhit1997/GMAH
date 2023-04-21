function OpenPasswordModal() {
    $("#password").val("");
    $("#repassword").val("");
    $("#currentpassword").val("");

    $('#passwordModal').modal("show");
}

function SubmitChangePassword() {
    $('#password').prop('readonly', true);
    $('#repassword').prop('readonly', true);
    $('#currentpassword').prop('readonly', true);
    $('#btnSubmitPassword').prop('readonly', true);


    $.ajax({
        type: "POST",
        url: "/api/userapi/updatepassword",
        data: {
            password: $("#password").val(),
            repassword: $("#repassword").val(),
            currentpassword: $("#currentpassword").val(),
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
                $('#passwordModal').modal("hide");
            }
        },
        complete: function () {
            $('#password').prop('readonly', false);
            $('#repassword').prop('readonly', false);
            $('#currentpassword').prop('readonly', false);
            $('#btnSubmitPassword').prop('readonly', false);
        }
    });
}