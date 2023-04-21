var _ID_USER = 0;

function SaveUserInfo() {
    $("#inputform").hide();
    $("#loading").show();

    // Thông số dành cho phụ huynh
    let idChilds = [];
    if (typeof listChild !== 'undefined' && listChild != null) {
        idChilds = listChild.map(x => x.IdUser);
    }

    // Thông số dành cho học sinh
    let idParents = [];
    if (typeof listParent !== 'undefined' && listParent != null) {
        idParents = listParent.map(x => x.IdUser);
    }

    $.ajax({
        type: "POST",
        url: "/api/userapi/saveuser",
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        data: {
            IdUser: _ID_USER,
            Fullname: $("[name=FullName]").val(),
            IdRole: $("[name=IdRole]").val(),
            Username: $("[name=Username]").val(),
            Password: $("[name=Password]").val(),
            CitizenId: $("[name=CitizenId]").val(),
            Address: $("#Address").val(),
            Repassword: $("[name=Repassword]").val(),
            Email: $("[name=Email]").val(),
            Phone: $("[name=Phone]").val(),
            TeacherCode: $("[name=TeacherCode]").val(),
            StudentCode: $("[name=StudentCode]").val(),
            IdChilds: idChilds,
            IdParents: idParents,
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
            }
            else {
                alert("Lưu dữ liệu thành công");
                location.href = listLink;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}

function GetUserInfo(id) {
    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/userapi/getuser/" + id,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                alert(result.Message);
                location.href = listLink;
            }
            else {
                $("[name=FullName]").val(result.Object.Fullname);
                $("[name=IdRole]").val(result.Object.IdRole);
                $("[name=Username]").val(result.Object.Username);
                $("[name=Phone]").val(result.Object.Phone);
                $("#Address").val(result.Object.Address);
                $("[name=CitizenId]").val(result.Object.CitizenID);
                $("[name=Email]").val(result.Object.Email);
                $('[name=Username]').attr('readonly', true);

                // Set student code và teacher code nếu có
                if (result.Object.StudentCode != undefined) {
                    $("[name=StudentCodeForm]").show();
                    $("[name=StudentCode]").val(result.Object.StudentCode);
                }

                if (result.Object.TeacherCode != undefined) {
                    $("[name=TeacherCodeForm]").show();
                    $("[name=TeacherCode]").val(result.Object.TeacherCode);
                }

                _ID_USER = id;
            }
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}

function SendEmailPassword() {
    if (_ID_USER == 0) {
        alert("Vui lòng lưu lại thông tin người dùng này trước khi gửi mail");
        return;
    }

    $("#inputform").hide();
    $("#loading").show();

    $.ajax({
        type: "GET",
        url: "/api/userapi/sendmailpassword/" + _ID_USER,
        dataType: 'json',
        headers: {
            "Authorization": "Baerer " + _JWT_TOKEN
        },
        success: function (result) {
            if (!result.IsSuccess) {
                Toastify({
                    text: result.Message,
                    gravity: 'bottom',
                    position: 'center',
                    className: "bg-danger",
                }).showToast();
                return;
            }

            Toastify({
                text: 'Gửi mail và tạo mật khẩu thành công',
                gravity: 'bottom',
                position: 'center',
                className: "bg-info",
            }).showToast();
        },
        complete: function () {
            $("#loading").hide();
            $("#inputform").show();
        }
    });
}