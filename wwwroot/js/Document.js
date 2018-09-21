let textChangeTimeout;
const textChangeWaitTime = 500;

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/docsHub")
    .build();
connection.start().then(function () {
    connection.invoke("JoinDocument", docId.toString());
});

let textarea = $("#textArea");

connection.on("TextChange", changeText);
connection.on("NameChange", changeName);
connection.on("ChangeOnlineUsers", changeOnlineUsersList);

window.onunload = function () {
    connection.invoke("LeaveDocument", docId.toString());
}

function changeText(text) {
    textarea.prop("disabled", true);
    clearTimeout(textChangeTimeout);
    textChangeTimeout = setTimeout(function () {
        textarea.prop("disabled", false);
    }, textChangeWaitTime);
    textarea.val(text);
}

function changeName(name) {
    document.getElementById("docName").innerHTML = name;
}

function changeOnlineUsersList(userNamesList) {
    let onlineUsersList = document.getElementById("onlineUsersList");
    onlineUsersList.innerHTML = userNamesList.length > 1 ? "" : "no members online";
    for (let i = 0; i < userNamesList.length; i++) {
        if (userNamesList[i] == curUserName)
            continue;
        let userDiv = document.createElement("div");
        userDiv.innerHTML = userNamesList[i];
        onlineUsersList.appendChild(userDiv);
    }
}