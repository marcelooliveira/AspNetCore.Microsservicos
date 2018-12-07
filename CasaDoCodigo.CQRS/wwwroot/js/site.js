/// <reference path="../signalr/dist/browser/signalr.min.js" />

"use strict";

var connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/usernotificationhub")
        .build();

connection.on("ReceiveMessage", function (user, message) {
    var el = document.querySelector('.notification');
    var count = Number(el.getAttribute('data-count')) || 0;
    el.setAttribute('data-count', count + 1);
    el.classList.remove('notify');
    el.classList.add('notify');
    if (count === 0) {
        el.classList.add('show-count');
    }
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
