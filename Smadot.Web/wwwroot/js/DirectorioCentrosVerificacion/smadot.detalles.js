"use strict"

//var datatable;


//$(document).ready(function () {
    //$('#tblAutorizadoVerificentro').DataTable({
    //    language: {
    //        "sProcessing": "Procesando...",
    //        "sLengthMenu": "Mostrar _MENU_ registros",
    //        "sZeroRecords": "No se encontraron resultados",
    //        "sEmptyTable": "Ningún dato disponible en esta tabla",
    //        "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
    //        "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
    //        "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
    //        "sInfoPostFix": "",
    //        "sSearch": "Buscar:",
    //        "sUrl": "",
    //        "sInfoThousands": ",",
    //        "sLoadingRecords": "Cargando...",
    //        "oPaginate": {
    //            "sFirst": "Primero",
    //            "sLast": "Último",
    //            "sNext": "Siguiente",
    //            "sPrevious": "Anterior"
    //        },
    //        "oAria": {
    //            "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
    //            "sSortDescending": ": Activar para ordenar la columna de manera descendente"
    //        }
    //    },
    //    searching: true,
    //    "lengthMenu": [[5, 10, 15, 20, -1], [5, 10, 15, 20, "Mostrar todo"]],
    //    autoWidth: true,
    //    columnDefs: [
    //        {
    //            targets: ['_all'],
    //            className: 'mdc-data-table__cell',
    //        },
    //    ],       
    //});
//});





var KTDatatableRemoteAjax = function () {
    var init = function () {
        var watchId;
        var mapa = null;
        var mapaMarcador = null;

        if (navigator.geolocation) {
            watchId = navigator.geolocation.watchPosition(mostrarPosicion, mostrarErrores, opciones);
        } else {
            alert("Tu navegador no soporta la geolocalización, actualiza tu navegador.");
        }

        function mostrarPosicion(posicion) {
            var latitud = window.latitud;
            var lat = latitud.replace(",", ".");

            var longitud = window.longitud;
            var lng = longitud.replace(",", ".");

            var precision = posicion.coords.accuracy;

            var miPosicion = new google.maps.LatLng(lng, lat);

            // Se comprueba si el mapa se ha cargado ya
            if (mapa == null) {
                // Crea el mapa y lo pone en el elemento del DOM con ID mapa
                var configuracion = { center: miPosicion, zoom: 16, mapTypeId: google.maps.MapTypeId.HYBRID };
                mapa = new google.maps.Map(document.getElementById("mapa"), configuracion);

                // Crea el marcador en la posicion actual
                mapaMarcador = new google.maps.Marker({ position: miPosicion, title: "Ubicacion de Verificentro" });
                mapaMarcador.setMap(mapa);
            } else {
                // Centra el mapa en la posicion actual
                mapa.panTo(miPosicion);
                // Pone el marcador para indicar la posicion
                mapaMarcador.setPosition(miPosicion);
            }
        }

        function mostrarErrores(error) {
            switch (error.code) {
                case error.PERMISSION_DENIED:
                    alert('Permiso denegado por el usuario');
                    break;
                case error.POSITION_UNAVAILABLE:
                    alert('Posición no disponible');
                    break;
                case error.TIMEOUT:
                    alert('Tiempo de espera agotado');
                    break;
                default:
                    alert('Error de Geolocalización desconocido :' + error.code);
            }
        }

        var opciones = {
            enableHighAccuracy: true
            //timeout: 10000,
            //maximumAge: 1000
        };

        function detener() {
            navigator.geolocation.clearWatch(watchId);
        }
    }
    return {
        init: function () {
            init();
        }
    };
}();


jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});

