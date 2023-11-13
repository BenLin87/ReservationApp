import Cryptor from "./Cryptor.js";

const cryptor = new Cryptor();

function show_Message_By_Action_And_State() {
    var state = sessionStorage.getItem("ConfirmOrderState");
    var message = "";
   
    if (!state) state = "";
    var messageId = "#action" + state + "Message";
    message = $(messageId).text();

    var additionMessage = sessionStorage.getItem("ConfrimOrderMessage");
    if (additionMessage) {
        message += additionMessage;
    }
    
    if (message.length > 0)
        $("#message").removeClass("hidden");
    else
        $("#message").addClass("hidden");
    $("#message").text(message);

    //If message has <br>, use innerHTML
    //document.getElementById("message").innerHTML = message;
}

function update_ui_by_state() {
    var state = sessionStorage.getItem("ConfirmOrderState");

    if (state === "Done") {
        $("#confirm_btn").addClass("hidden");
    }
    else {
        $("#confirm_btn").removeClass("hidden");
    }
}

$(function () {
    var orderJson = sessionStorage.getItem("OrderData");
    //This page requires order data.
    if (!orderJson) {
        var culture = $("#currentCulture").text();
        if (culture != null && (culture == "zh-TW" || culture == "zh-Hant"))
            $("#message").text("頁面已過期");
        else
            $("#message").text("Page has expired.");
        $("#message").removeClass("hidden");
        $(".table_col_header").addClass("hidden");
        $("#confirm_btn").addClass("hidden");
        return;
    }

    cryptor.decrypt(orderJson).then(decrypted => {
        showOrderData(decrypted);
    }).catch(error => {
        alert(error);
    })
    show_Message_By_Action_And_State();
    update_ui_by_state();
})

function showOrderData(orderJson) {
    if (!orderJson) {
        return;
    }
    var orderData = JSON.parse(orderJson);
    if (!orderData) {
        return;
    }
    let dateCount = 0;
    if (orderData.reservations != null && orderData.reservations.length > 0) {
        dateCount = orderData.reservations.length;
        var row = "<tr><td id='userName' class='table_cell'";
        if (dateCount > 1) {
            row += " rowSpan='" + dateCount + "'";
        }
        row += ">" + orderData.user.name + "</td>";


        if (orderData.id != null) {
            //Load an existed Order data
            row += "<td id='orderId' class='table_cell'";
            if (dateCount > 1) {
                row += " rowSpan='" + dateCount + "'";
            }
            row += ">" + orderData.id + "</td>";
        }
        else {
            //Load a new Order data without Id
            $(".table_col_header.orderId").addClass("hidden");
        }
    }
    for (let i = 0; i < dateCount; i++) {

        row += "<td class='table_cell reservateDate'>" + orderData.reservations[i].date + "</td>";
        if (orderData.reservations[i].times.length > 0) {
            row += "<td class='table_cell reservateTime'>";
            for (let j = 0; j < orderData.reservations[i].times.length; j++) {
                if (j > 0) {
                    row += "<br/>";
                }
                row += orderData.reservations[i].times[j].time;
            }
            row += "</td>";
        }
        row += "</tr>";
        $(".timetable").append(row);
        row = "<tr>";
    }
}

async function getPostDataAsync() {
    var orderDataJson = sessionStorage.getItem("OrderData");
    let result = "";
    if (orderDataJson == null)
        return Promise.resolve("");

    try {
        var decryptedOrderData = await cryptor.decrypt(orderDataJson);
        var orderData = JSON.parse(decryptedOrderData);
        if (orderData.id != null && orderData.id != "") {
            var modifiedReservationsJson = sessionStorage.getItem("ModifiedReservations");
            if (modifiedReservationsJson == null) {
                //Delete Order
                return await cryptor.encrypt(orderData.id);
            }
            else {
                //Modify Order
                return modifiedReservationsJson;
            }
        }
        else {
            //New Order
            return orderDataJson;
        }
    }catch(error){
        alert(error);
    };
}

$(document).on("click", "#confirm_btn", async function () {
    //$("#confirm_btn").disabled = true;
    
    var state = sessionStorage.getItem("ConfirmOrderState");
    if (state == null) state = "";
    if (state == "Done") return;

    $("#confirm_btn").addClass("disabled");
    var returnUrl = $("#returnUrl").text();
    try {
        var postData = await getPostDataAsync();
        $.post(returnUrl, { jsonData: postData }, function (responseJson) {
        }).always(function (responseJson) {
            var response = JSON.parse(responseJson);
            if (response.result == true) {
                $("#confirm_btn").addClass("hidden");
                state = "Done";
            }
            else {
                $("#confirm_btn").removeClass("disabled");
                state = "Fail";
            }

            sessionStorage.setItem("ConfrimOrderMessage", response.message);
            sessionStorage.setItem("ConfirmOrderState", state);
            show_Message_By_Action_And_State();
        });
    } catch(error) {
        alert(error);
    };
})