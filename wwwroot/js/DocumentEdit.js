let sendTimeout;
const sendWaitTime = 500;
let changesSavedTimeout;

let docInput = { selStart: 0, selEnd: 0, text: "" };
let prevText, pasted = false;
textarea.keydown(keyDown);
textarea.on("cut", docCut);
textarea.on("paste", docPaste);

function keyDown(e) {
    console.log(e.key);

    docInput.selStart = textarea.prop("selectionStart");
    docInput.selEnd = textarea.prop("selectionEnd");
    docInput.text = e.key;

    if (e.key == "Tab") {
        e.preventDefault();
        docInput.text = "";
    }
    else if (e.key == "Backspace") {
        if (docInput.selStart == docInput.selEnd)
            docInput.selStart--;
        docInput.text = "";
    }
    else if (e.key == "Delete") {
        if (docInput.selStart == docInput.selEnd)
            docInput.selEnd++;
        docInput.text = "";
    }
    else if (e.ctrlKey && (e.key == "z" || e.key == "Z")) {
        e.preventDefault();
    }
}

function docCut() {
    docInput.selStart = textarea.prop("selectionStart");
    docInput.selEnd = textarea.prop("selectionEnd");
    docInput.text = "";
}

function docPaste() {
    docInput.selStart = textarea.prop("selectionStart");
    docInput.selEnd = textarea.prop("selectionEnd");
    prevText = textarea.val();
    pasted = true;
}

function textChange() {
    if (pasted) {
        pasted = false;
        curText = textarea.val();
        docInput.text = curText.substr(docInput.selStart, curText.length - prevText.length + docInput.selEnd - docInput.selStart);
    }

    if (sendTimeout == null)
        sendTimeout = setTimeout(sendText, sendWaitTime);
    connection.invoke("TextChange", docId, docInput);
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
    sendTimeout = null;
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
        connection.invoke("NameChange", docId, docNameInput.val());
    });
    docNameInput.focusout(function () {
        let docNameInput1 = $("#docName");
        let docName1 = $("<span></span>");
        docName1.attr("id", docNameInput1.attr("id"));
        docName1.text(docNameInput1.val());
        docNameInput1.replaceWith(docName1);
    });
}
