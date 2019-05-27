let request = {};
let client_name;
let client_addr;
let client_email;

function setUserInfos(name, addr, email) {
    client_name = name;
    client_addr = addr;
    client_email = email;
}

function addBook(title) {
    if (title in request) {
        request[title]+=1;
    }
    else {
        request[title] = 1;
    }
    let amount_label = document.getElementById(title);
    amount_label.innerHTML = request[title];
}

function removeBook(title) {
    if (title in request && request[title] > 0) {
        request[title]-=1;
    }
    let amount_label = document.getElementById(title);
    amount_label.innerHTML = request[title];
}

function submitRequest() {
    $.ajax({
        url: "/bookstore/request",
        type: "get",
        data: {
            username: client_name,
            email: client_email,
            address: client_addr,
            books: JSON.stringify(request)
        },
        success: function(msg) {
            resetRequest();
            location.reload();
        }
    });
}

function resetRequest() {
    let amounts = document.getElementsByClassName("BookAmount");
    for (let i = 0; i < amounts.length; i++) {
        amounts[i].innerHTML = 0;
    }
    for (let title in request) {
        request[title] = 0;
    }
}