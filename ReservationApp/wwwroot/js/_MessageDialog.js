$(function () {
    var culture = $("#messageDialogCulture").text();
    var msgDlgBtn = "Confirm";
    var msgDlgTitle = "Tips";
    if (culture != null && (culture == "zh-TW" || culture == "zh-Hant")) {
        msgDlgBtn = "確認";
        msgDlgTitle = "提示";
    }

    var dialog_buttons = {};
    dialog_buttons[msgDlgBtn] = function () {
        $(this).dialog("close");
    };

    $("#messageDialog").dialog({
        title: msgDlgTitle,
        modal: true,
        autoOpen: false,
        buttons: dialog_buttons,
        closeOnEscape: false,
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close", ui.dialog | ui).hide();
        }
    });
});