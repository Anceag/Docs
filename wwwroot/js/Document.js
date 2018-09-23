let textChangeTimeout;
const textChangeWaitTime = 500;

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/docsHub")
    .build();
connection.start().then(function () {
    connection.invoke("JoinDocument", docId.toString());
});

connection.on("TextChange", changeText);
connection.on("NameChange", changeName);
connection.on("ChangeOnlineUsers", changeOnlineUsersList);

function changeText(docInput) {
    textarea.prop("disabled", true);
    clearTimeout(textChangeTimeout);

    textChangeTimeout = setTimeout(function () {
        textarea.prop("disabled", false);
    }, textChangeWaitTime);




    let text = textarea.val();
    text = text.substring(0, docInput.selStart) + docInput.text + text.substring(docInput.selEnd);
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