"use strict"

var connection;
var endpoint = "https://smadsot-notification.southcentralus.cloudapp.azure.com/";

function Storage() {

    this.get = function (name) {
        return JSON.parse(window.localStorage.getItem(name));
    };

    this.set = function (name, value) {
        window.localStorage.setItem(name, JSON.stringify(value));
    };

    this.clear = function () {
        window.localStorage.clear();
    };
}

var store = new Storage();

var NotificacionSignalR = function () {
    var init = function () {

        var id = Number(idUserLogLayout);
        connection = new signalR.HubConnectionBuilder().withUrl(endpoint + "notificacionHub", {
            skipNegotiation: true,
            transport: signalR.HttpTransportType.WebSockets
        }).withAutomaticReconnect() //when server get disconnects it reconnects with the serevr
            .configureLogging(signalR.LogLevel.Information).build();

        connection.start().then(function () {
            connection.invoke('GetConnectionId', id);
        }).catch(function (err) {
            return /*console.error(err.toString())*/;
        });

        connection.on("nueva_notificacion", function (message) {
            Alertas.push(message);
            if (message.tableName == 'Verificacion' || message.tableName == 'Equipo') {
                toastr.info(message.movimiento, "SMADSOT");
                //let id = message.id;
                //Swal.fire({
                //    html: message.movimientoInicial,
                //    icon: "info",
                //    buttonsStyling: false,
                //    showCancelButton: true,
                //    allowOutsideClick: false,
                //    allowEscapeKey: false,
                //    allowEnterKey: false,
                //    confirmButtonText: "Ok, got it!",
                //    cancelButtonText: 'Nope, cancel it',
                //    customClass: {
                //        confirmButton: "btn btn-primary",
                //        cancelButton: 'btn btn-thertiary'
                //    }
                //}).then((result) => {
                //    if (result.isConfirmed) {
                //        AutorizacionVerificacion.autorizar(id, true);
                //    } else {
                //        AutorizacionVerificacion.autorizar(id, false);
                //    }
                //});
            };
        });

        connection.on("actualizacionPrueba", function (message) {
            if ($('#rowDashboardLineasLayoutSMADSOT').length > 0)
                UpdateDashboardLineasByNotification.init();
        });
    };

    return {
        init: function () {
            init();
        }
    };
}();

jQuery(document).ready(function () {
    NotificacionSignalR.init();
});