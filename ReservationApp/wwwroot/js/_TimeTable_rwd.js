import Cryptor from "./Cryptor.js";
const cryptor = new Cryptor();

var mobileWidth = 750;

function windowSize() {
    if ($(window).width() < mobileWidth) {
        $(".table_container").removeClass("desktop");
        $(".table_container").addClass("mobile");
    }
    else {
        $(".table_container").removeClass("mobile");
        $(".table_container").addClass("desktop");
    }
};

$(window).resize(function () {
    windowSize();
});

$(function () {
    windowSize();
    var existedReservationsJson = sessionStorage.getItem("ExistedReservations");
    if (existedReservationsJson != null) {
        cryptor.decrypt(existedReservationsJson)
            .then(encrptyed => { 
                var existedReservations = JSON.parse(encrptyed);
                TraverseReservations(existedReservations, SetExistedReservationCell);
            }).catch(error => {
                alert(error);
            })
    }

    var modifingOrderJson = sessionStorage.getItem("OrderData");
    if (modifingOrderJson != null) {
        cryptor.decrypt(modifingOrderJson)
            .then(encrptyed => {
                var modifingOrder = JSON.parse(encrptyed);
                TraverseReservations(modifingOrder.reservations, SetSeletedReservationCell);
            }).catch(error => {
                alert(error);
            })
    }
    $("*[data-bs-toggle=\"tooltip\"]").tooltip();
})

function SetSeletedReservationCell(tableCell) {
    if (tableCell != null) {
        tableCell.removeClass("unavailable");
        tableCell.addClass("available");
        tableCell.addClass("selected");
        tableCell.addClass("originalTime");
    }
}

function SetExistedReservationCell(tableCell) {
    if (tableCell != null) {
        tableCell.removeClass("available");
        tableCell.addClass("unavailable");
    }
}

function TraverseReservations(reservations, funcToDo) {
    var dateCount = reservations.length;

    for (let i = 0; i < dateCount; i++) {
        var timeCount = reservations[i].times.length;
        var date = reservations[i].date;
        for (let j = 0; j < timeCount; j++) {
            var timeData = reservations[i].times[j];
            var cellSelector = ".table_cell[date^='" + date + "'][time='" + timeData.time + "']";
            var cell = $(cellSelector);

            //If reservation time has Guid, store it.
            if (cell != null && timeData.Guid != null) {
                cell.attr("Guid", timeData.Guid.toString());
            }
            funcToDo(cell);
        }
    }
}

$(document).on("click", ".table_root", function () {

    if ($(this).hasClass("selected")) {
        $(".table_root, .table_row_header, .table_col_header, .table_cell.available").removeClass("selected");
    } else {
        $(".table_root, .table_row_header, .table_col_header, .table_cell.available").addClass("selected");
    }
});

$(document).on("click", ".table_row_header", function () {
    var row = $(this).attr("row");
    var selector = ".table_cell.available[row='" + row + "']";

    if ($(this).hasClass("selected")) {
        $(this).removeClass("selected");
        $(selector).removeClass("selected");
    }
    else {
        $(this).addClass("selected");
        $(selector).addClass("selected");
    }
});

$(document).on("click", ".table_col_header", function () {
    var column = $(this).attr("column");
    var selector = ".column_container[column='" + column + "'], .table_cell.available[column='" + column + "']";

    if ($(this).hasClass("selected")) {
        $(this).removeClass("selected");
        $(selector).removeClass("selected");
    }
    else {
        $(this).addClass("selected");
        $(selector).addClass("selected");
    }
});

$(document).on("click", ".table_cell.available", function () {
    if ($(this).hasClass("selected")) {
        $(this).removeClass("selected");
    } else {
        $(this).addClass("selected");
    }
});