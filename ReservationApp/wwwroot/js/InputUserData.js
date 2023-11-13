import Cryptor from "./Cryptor.js";
const cryptor = new Cryptor();

$(function () {
 
    sessionStorage.removeItem("ConfrimOrderMessage");
});

$(document).on("click", "#next_btn", function () {
    var userName = $("#userName").val();
    if (userName == null || userName == "") {
        var culture = $("#currentCulture").text();
        if (culture != null && (culture == "zh-TW" || culture == "zh-Hant")) {
            $("#messageDialog p").text("請輸入預約人姓名!");
        }
        else {
            $("#messageDialog p").text("Please input user name!");
        }
        $("#messageDialog").dialog("open");
    }
    else {
        var orderJson = sessionStorage.getItem("OrderData");
        if (orderJson != null) {
            cryptor.decrypt(orderJson)
                .then(decrypted => {
                    var order = JSON.parse(decrypted);
                    order.user.name = userName;
                    orderJson = JSON.stringify(order);

                    return cryptor.encrypt(orderJson);
                }).then(encrypted => {
                    sessionStorage.setItem("OrderData", encrypted);
                    location.href = "/NewOrder/ConfirmOrder";
                }).catch(error => {
                    alert(error);
                });
        }
    }
});

