"use strict";

import { signalR } from "../wwwroot/lib/signalr/dist/browser/signalr";

var timerId;
const connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5000/orderhub')
    .configureLogging(signalR.LogLevel.Debug)
    .withAutomaticReconnect().build();

connection.on('UpdateOrders', (message, orderId) => {
    const encoding = message + ":" + orderId;
    console.log(encoding);
    if (orderId && orderId.length) {
        toastr.success(orderId + 'Updated to status ' + message);
        refreshPage();
    }
});

function refreshPage() {
    clearTimeout(timerId);
    timerId = setTimeout(function () {
        window.location.reload();
    }, 3000)
}
connection.start().catch(function (err) {
    console.error(err.toString());
});