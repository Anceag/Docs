
let connection = new signalR.HubConnectionBuilder()
    .withUrl("/docsHub")
    .build();
connection.start().then(function () {
    connection.invoke("JoinDocument", docId.toString());
});

let textarea = $("#textArea");

connection.on("TextChange", function (text) { textarea.val(text); });
connection.on("ChangeOnlineUsers", changeOnlineUsersList);

window.onunload = function () {
    connection.invoke("LeaveDocument", docId.toString());
}

function changeOnlineUsersList(userNamesList) {
    let onlineUsersList = document.getElementById("onlineUsersList");
    onlineUsersList.innerHTML = "";
    for (let i = 0; i < userNamesList.length; i++) {
        if (userNamesList[i] == curUserName)
            continue;
        let userDiv = document.createElement("div");
        userDiv.innerHTML = userNamesList[i];
        onlineUsersList.appendChild(userDiv);
    }
}