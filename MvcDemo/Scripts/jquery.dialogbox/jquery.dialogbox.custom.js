
function ShowDialog(dialogBox, title, message) {
    dialogBox.dialog({ title: title });
    dialogBox.html(message);
    dialogBox.dialog("open");
}

function SetDialog(dialogBox) {
    dialogBox.dialog({
        autoOpen: false,
        height: 75,
        width: 200,
        modal: true
    });
}