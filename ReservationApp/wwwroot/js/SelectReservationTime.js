import Cryptor from "./Cryptor.js";
const cryptor = new Cryptor();

$(function () {
});

function ReservationTime(time) {
    this.time = time;
}

function Reservation(date, times) {
    this.date = date;
    this.times = times;
}

function User(name) {
    this.name = name;
}

function Order(user, orderId, reservations) {
    this.user = user;
    this.id = orderId;
    this.reservations = reservations;
}

function ModifiedReservations(orderId, addReservations, deleteReservations) {
    this.id = orderId;
    this.addReservations = addReservations;
    this.deleteReservations = deleteReservations;
}

function createReservations(objs) {
    var dict = [];
    objs.each(function () {
        //Add times
        var date = $(this).attr("date");
        var time = $(this).attr("time");
        if (date in dict) {
            dict[date].push(new ReservationTime(time));
        }
        else {
            var times = [];
            times.push(new ReservationTime(time));
            dict[date] = times;
        }
    });
    var reservations = [];
    for (const date in dict) {
        var reservation = new Reservation(date, dict[date]);
        reservations.push(reservation);
    }
    return reservations;
}

async function saveModifiedReservationTimes() {
    var encryptedOrder = sessionStorage.getItem("OrderData");
    if (encryptedOrder == null)
        return;
    try {
        var decryptedOrder = await cryptor.decrypt(encryptedOrder);

        var addObjs = $(".table_cell.selected").not(".originalTime");
        var addReservations = createReservations(addObjs);

        var removeObjs = $(".table_cell.originalTime").not(".selected");
        var deleteReservations = removeObjs.map(function () {
            return $(this).attr("Guid");
        }).get();

        var orderId = JSON.parse(decryptedOrder).id;
        var modifiedReservations = new ModifiedReservations(orderId, addReservations, deleteReservations);
        const modifiedReservationsJson = JSON.stringify(modifiedReservations);

        var encryptedModifiedReservations = await cryptor.encrypt(modifiedReservationsJson)
        sessionStorage.setItem("ModifiedReservations", encryptedModifiedReservations);
    } catch(error){
        alert(error);
    };
}

async function saveSelectedReservationTimes() {
    var selectedObjs = $(".table_cell.selected");
    var selectedReservations = createReservations(selectedObjs);

    let orderJson = "";
    var encryptedModifingOrder = sessionStorage.getItem("OrderData");

    try {
        if (encryptedModifingOrder != null) {
            //Modify Order
            var decryptedModifingOrder = await cryptor.decrypt(encryptedModifingOrder);
            var modifingOrder = JSON.parse(decryptedModifingOrder);
            modifingOrder.reservations = selectedReservations;
            orderJson = JSON.stringify(modifingOrder);
        }
        else {
            //New Order
            var user = new User(null);
            var order = new Order(user, null, selectedReservations);
            orderJson = JSON.stringify(order);
        }
   
        var encryptedOrder = await cryptor.encrypt(orderJson);
        sessionStorage.setItem("OrderData", encryptedOrder);
    }catch(error)  {
        alert(error);
    };
}

$(document).on("click", "#next_btn", async function () {
    if ($(".table_cell.selected").length == 0) {
        var culture = $("#currentCulture").text();
        if (culture != null && (culture == "zh-TW" || culture == "zh-Hant")) {
            $("#messageDialog p").text("請選擇至少一筆預約時段!");
        }
        else {
            $("#messageDialog p").text("Please select at least one reservation time!");
        }
        $("#messageDialog").dialog("open");
        return;
    }
    var nextUrl = $("#nextUrl").text();
    await saveSelectedReservationTimes();
    await saveModifiedReservationTimes();
    location.href = nextUrl;
});

