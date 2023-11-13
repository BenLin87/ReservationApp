$(function () {
    var currentCulture = $("#currentCulture").text();
    $("#languageSelector").val(currentCulture);
})

$(document).on("change", "#languageSelector", function () {
    var optionSelected = $(".languageOption:selected", this);
    var valueSelected = this.value;
   
    //localStorage.setItem("Culture", valueSelected);
    var returnUrl = "/Home/SetLanguage?language=" + valueSelected;

    $.post(returnUrl, { language: valueSelected }, function (responseJson) {
    }).done(function () {
        location.reload();
    }).fail(function (responseJson) {
        alert(responseJson);
    });
});