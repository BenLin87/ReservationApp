import Cryptor from "./Cryptor.js";

var msgDlgTitle = "Tip";
var msgDlgTitle_LoginFail = "Login fail";
var requireAcctMsg = "Please input admin account!";
var requirePwdMsg = "Please input admin password!"
var msgDlgBtn = "Confirm";

const cryptor = new Cryptor();

$(function () {
   
    if ($("#message").text().length > 0) {
        window.setTimeout(function () {
            location.href = "/Home/Index";
        }, 4000);
    }
    var culture = $("#currentCulture").text();
    if (culture != null && (culture == "zh-TW" || culture == "zh-Hant")) {
        msgDlgTitle = "提示";
        msgDlgTitle_LoginFail = "登入失敗";
        requireAcctMsg = "請輸入管理者帳號!";
        requirePwdMsg = "請輸入管理者密碼!";
        msgDlgBtn = "確認";
    }
})

/*
document.addEventListener("DOMContentLoaded", function () {
    var inputElement = document.getElementById("userNameInput"); // 替換為實際的元素 ID

    if (inputElement != null) {
        // 綁定自訂的驗證失敗事件處理程序
        inputElement.addEventListener("input", function () {
            // 驗證條件
            var isValid = inputElement.checkValidity();

            // 顯示/隱藏 Tooltip
            if (!isValid) {
                console.log("invalid");
            } else {
                console.log("valid");
            }
        });
    }
})
*/
function LoginData(name, password) {
    this.name = name;
    this.password = password;
}

var isLogIn = false;


$(document).on("click", "#login_btn", function () {

    var adminName = $("#adminUsername").val();
    var adminPwd = $("#adminPassword").val();
    $("#messageDialog").dialog({
        title: msgDlgTitle
    });

    if (adminName == null || adminName == "") {
        $("#messageDialog p").text(requireAcctMsg);
        $("#messageDialog").dialog("open");
        return;
    }
    if(adminPwd == null || adminPwd == "") {
        $("#messageDialog p").text(requirePwdMsg);
        $("#messageDialog").dialog("open");
        return;
    }
    var loginDataJson = new LoginData(adminName, adminPwd);
    var loginData = JSON.stringify(loginDataJson);
    var url = "/Login/AdminLogin";
   

    
    cryptor.encrypt(loginData).then(
        encrypted => {
          
            $.post(url, { jsonData: encrypted }, function (responseJson) {
            }).always(function (responseJson) {
                var response = JSON.parse(responseJson);
                isLogIn = response.result;
                if (response.result == false) {
                    $("#messageDialog").dialog({
                        title: msgDlgTitle_LoginFail
                    });
                }
                else {
                    var dialog_buttons = {};
                    dialog_buttons[msgDlgBtn] = function () {
                        $(this).dialog("close");
                        ReloadPage();
                    };
                    $("#messageDialog").dialog({
                        buttons: dialog_buttons
                    });
                }

                $("#messageDialog p").text(response.message);
                $("#messageDialog").dialog("open");
            });
        }
    );

   
});

function ReloadPage() {
    if (isLogIn) {
        location.reload();
    }
}

