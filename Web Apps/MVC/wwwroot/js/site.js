/// <reference path="../signalr/dist/browser/signalr.min.js" />

"use strict";

var connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/usercounterdatahub")
        .build();

connection.on("ReceiveMessage", function (user, message) {
    var count = parseInt(message);
    var el = document.querySelector('.notification');
    el.setAttribute('data-count', count);
    el.classList.remove('notify');
    el.classList.add('notify');
    el.classList.add('show-count');
});

connection.on("ReceiveUserBasketCount", function (user, basketCount) {
    var el = document.querySelector('.userbasket');
    el.setAttribute('data-count', basketCount);
    el.classList.remove('notify');
    el.classList.add('notify');
    if (basketCount) {
        el.classList.add('show-count');
    }
    else {
        el.classList.remove('show-count');
    }
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
