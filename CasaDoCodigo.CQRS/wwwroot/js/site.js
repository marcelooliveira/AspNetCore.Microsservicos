/// <reference path="../signalr/dist/browser/signalr.min.js" />
//var el = document.querySelector('.notification');

//document.querySelector('.button-notification').addEventListener('click', function () {
//    var count = Number(el.getAttribute('data-count')) || 0;
//    el.setAttribute('data-count', count + 1);
//    el.classList.remove('notify');
//    el.offsetWidth = el.offsetWidth;
//    el.classList.add('notify');
//    if (count === 0) {
//        el.classList.add('show-count');
//    }
//}, false);

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/usernotificationhub").build();

connection.on("ReceiveUserNotification", function (user, message) {
    var count = Number(el.getAttribute('data-count')) || 0;
    el.setAttribute('data-count', count + 1);
    el.classList.remove('notify');
    el.offsetWidth = el.offsetWidth;
    el.classList.add('notify');
    if (count === 0) {
        el.classList.add('show-count');
    }
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});
