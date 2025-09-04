"use strict"

var datatable;

var KTDatatableRemoteAjax = function () {
    var init = function () {
        datatable = $('#kt_datatable').DataTable({
            language: {
                "decimal": "",
                "emptyTable": "No hay información",
                "info": "Mostrando _END_ de _TOTAL_ entradas",
                "infoEmpty": "Mostrando 0 de 0 entradas",
                "infoFiltered": "(Filtrado de _MAX_ total entradas)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Mostrar _MENU_ Entradas",
                "loadingRecords": "Cargando...",
                "processing": "Procesando...",
                "search": "Buscar:",
                "zeroRecords": "No se encontraron resultados para mostrar",
                "paginate": {
                    "first": "Primero",
                    "last": "Último",
                    "next": "&raquo;",
                    "previous": "&laquo;"
                }
            },
            responsive: true,
            pagingType: 'simple_numbers',
            searching: true,
            lengthMenu: [15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            aaSorting: [[1, 'desc']],
            ajax: {
                url: 'FoliosCancelados/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                //{ data: 'id', name: 'Id', title: 'Número de Solicitud' },
                { data: 'folio', name: 'Folio', title: 'Folio' },
                { data: 'fechaCancelacion', name: 'FechaCancelacion', title: 'Fecha de cancelación' },
                { data: 'motivo', name: 'motivo', title: 'Motivo' },
                { data: 'usuarioAprobo', name: 'usuarioAprobo', title: 'Usuario que aprobó' },
                { data: 'tipoTramite', name: 'TipoTramite', title: 'Tipo de trámite' },
                { data: 'datosVehiculo', name: 'DatosVehiculo', title: 'Datos del vehículo' },
                { data: 'personaRealizoTramite', name: 'PersonaRealizoTramite', title: 'Persona que realizó el trámite' },

                //{
                //    title: '',
                //    orderable: false,
                //    data: null,
                //    defaultContent: '',
                //    render: function (data, type, row) {
                //        if (type === 'display') {
                //            var htmlString = row.acciones;
                //            return htmlString
                //        }
                //    }
                //}
            ],
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');
    };

    var recargar = function () {
        datatable.ajax.reload();
    }

    return {
        init: function () {
            init();
        },
        recargar: function () {
            recargar();
        }
    };
}();


jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});