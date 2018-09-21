let sendTimeout;
const sendWaitTime = 500;
let changesSavedTimeout;

function textChange() {
    clearTimeout(sendTimeout);
    sendTimeout = setTimeout(sendText, sendWaitTime);
    connection.invoke("TextChange", textarea.val());
}

function sendText() {
    $.ajax({
        url: documentChangeUrl,
        type: "POST",
        data: {
            id: docId,
            content: textarea.val()
        },
        success: showChangesSaved
    });
}

function showChangesSaved() {
    $("#changesSavedText").css("visibility", "visible");
    clearTimeout(changesSavedTimeout);
    changesSavedTimeout = setTimeout(function () {
        $("#changesSavedText").css("visibility", "hidden");
    }, 1500);
}

function docNameChange() {
    let docName = $("#docName");
    let docNameInput = $("<input type='text'>");
    docNameInput.attr("id", docName.attr("id"));
    docNameInput.val(docName.text());
    docName.replaceWith(docNameInput);
    docNameInput.focus();

    docNameInput.on("change", function () {
        $.ajax({
            url: documentNameChangeUrl,
            type: "POST",
            data: {
                id: docId,
                name: docNameInput.val()
            }
        });
        connection.invoke("NameChange", docNameInput.val());
    });
    docNameInput.focusout(function () {
        let docNameInput1 = $("#docName");
        let docName1 = $("<span></span>");
        docName1.attr("id", docNameInput1.attr("id"));
        docName1.text(docNameInput1.val());
        docNameInput1.replaceWith(docName1);
    });
}
