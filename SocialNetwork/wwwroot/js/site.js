// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function getJson(url, callback) {
    let xhr = new XMLHttpRequest();
    xhr.open('GET', url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            let json = JSON.parse(xhr.responseText);
            callback(json);
        }
    };
    xhr.send();
}
function GetUserFromChat(users, id_user) {
    for (let i = 0; i < users.length; i++) {
        if (users[i]["id"] == id_user)
            return users[i];
    }
}