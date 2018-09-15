function addMember() {
    let roleSelect = document.getElementById("roleSelect");
    $.ajax({
        url: addMemberUrl,
        type: "POST",
        data: {
            documentId: docId,
            userName: document.getElementById("userName").value,
            roleId: roleSelect.options[roleSelect.options.selectedIndex].value
        },
        success: function (data) {
            if (data != undefined)
                addMemberRow({ userName: data.user.userName, roleName: data.role.name });
        }
    });
}

function addMemberRow(data) {
    let tr = document.createElement("tr");
    document.getElementById("membersTable").appendChild(tr);
    tr.setAttribute("id", "member" + data.userName);

    let td1 = document.createElement("td");
    tr.appendChild(td1);
    td1.innerHTML = data.userName;

    let td2 = document.createElement("td");
    tr.appendChild(td2);
    td2.innerHTML = data.roleName;

    let td3 = document.createElement("td");
    tr.appendChild(td3);
    let delBtn = document.createElement("input");
    td3.appendChild(delBtn);
    delBtn.setAttribute("type", "button");
    delBtn.setAttribute("class", "btn btn-default");
    delBtn.setAttribute("value", "Delete");
    delBtn.setAttribute("onclick", "deleteMember('" + data.userName + "')");
}

function deleteMember(userName) {
    $.ajax({
        url: deleteMemberUrl,
        type: "POST",
        data: {
            documentId: docId,
            userName: userName
        },
        success: function () {
            let element = document.getElementById("member" + userName);
            element.parentNode.removeChild(element);
        }
    });
}