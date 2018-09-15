let sendTimeout;
let sendWaitTime = 500;
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