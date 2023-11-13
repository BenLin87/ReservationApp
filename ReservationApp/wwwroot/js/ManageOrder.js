var datepickerDateFormat = "yy/mm/dd";
var momentDateFormat = "YYYY/MM/DD";

var searchResultMsg = "Search result";
var searchDetailMsgPart1 = "Found ";
var searchDetailMsgPart2 = " orders."
var searchFailedMsg = "Search failed";

$(function () {
    var culture = $("#currentCulture").text();
    if (culture != null) { 
        if(culture == "zh-TW" || culture == "zh-Hant") {
             $.datepicker.regional[culture] = {
                dayNames: ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
                dayNamesMin: ["日", "一", "二", "三", "四", "五", "六"],
                monthNames: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
                monthNamesShort: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
                prevText: "上月",
                nextText: "次月",
                weekHeader: "週"
            };
            $.datepicker.setDefaults($.datepicker.regional[culture]);

            searchResultMsg = "搜尋結果";
            searchDetailMsgPart1 = "搜尋到";
            searchDetailMsgPart2 = "筆訂單"
            searchFailedMsg = "搜尋失敗";
        }
    }
    from = $("#dateFrom")
        .datepicker({
            setDate: "-7d",
            defaultDate: "-7d",
            changeMonth: true,
            dateFormat: datepickerDateFormat
        })
        .val(moment().add(-7, 'days').format(momentDateFormat))
        .on("change", function () {
            to.datepicker("option", "minDate", getDate(this));
        }),
    to = $("#dateTo").datepicker({
            setDate: "+7d",
            defaultDate: "+7d",
            changeMonth: true,
            dateFormat: datepickerDateFormat
        })
        .val(moment().add(7, 'days').format(momentDateFormat))
        .on("change", function () {
            from.datepicker("option", "maxDate", getDate(this));
        });

    function getDate(element) {
        var date;
        try {
            date = $.datepicker.parseDate(datepickerDateFormat, element.value);
        } catch (error) {
            date = null;
        }
        return date;
    }
    $("#displayMode").prop("selectedIndex", 2);
    $("*[data-bs-toggle=\"tooltip\"]").tooltip();
});


function getOrderList() {
   
    var searchConditions = getConditions();
    var searchConditionsJson = JSON.stringify(searchConditions);
    var targetUrl = $("#controller").text() + "/GetOrderList";
    var msgTitle = searchResultMsg;
    
    $.ajax({
        url: targetUrl,
        data: { searchConditions: searchConditionsJson },
        type: "POST",
        beforeSend:function () {
             $("#searchButton").addClass("disabled");
        }
    }).
    done(function (response) {
        $("#orderListContent").html(response);
        var message = searchDetailMsgPart1 + $(".orderColumn").length + searchDetailMsgPart2;
        $("#messageDialog p").text(message);
    })
    .fail(function (response) {
        var message = response.responseJSON;
        $("#messageDialog p").text(message);
        msgTitle = searchFailedMsg;
    })
    .always(function () {
        $("#searchButton").removeClass("disabled");
        $("#messageDialog").dialog({
            title: msgTitle
        });
        $("#messageDialog").dialog("open");
    });
}

function getConditions() {
    var userNameValue = $("#userName").val();
    var orderIdValue = $("#orderId").val();
    var dateFromValue = $("#dateFrom").val();
    var dateToValue = $("#dateTo").val();
    var displaySelector = $("#displayMode");
    var displayValue = $("#displayMode").prop('selectedIndex');

    var searchConditions = {
        userName: userNameValue,
        orderId: orderIdValue,
        dateFrom: dateFromValue,
        dateTo: dateToValue,
        displayMode: displayValue
    }
    return searchConditions;
}

function resetConditions() {
    $("#userName").val("");
    $("#orderId").val("");
    $("#dateFrom").val(moment().add(-7, 'days').format(momentDateFormat));
    $("#dateTo").val(moment().add(7, 'days').format(momentDateFormat));
    $("#displayMode").prop("selectedIndex", 2);
}

$(document).on("click", "#searchButton", getOrderList);
$(document).on("click", "#resetButton", resetConditions);