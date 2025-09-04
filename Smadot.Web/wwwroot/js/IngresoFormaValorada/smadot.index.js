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
            order: [[0, 'desc']],
            lengthMenu: [15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: 'IngresoFormaValorada/Consulta',
                type: 'POST',
                data: function (d) {
                    if ($('#almacenGrid').select2('data') !== undefined)
                        d.idAlmacen = $('#almacenGrid').select2('data')[0].id;
                }
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [

                { data: 'idSolicitudFV', name: 'IdSolicitudFV', title: 'Número de Solicitud'.bold() },
                {
                    data: 'fechaSolicitudFV', name: 'FechaSolicitudFV', title: 'Fecha de Solicitud'.bold(),
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaSolicitudFV).format('DD/MM/YYYY');
                        }
                    }
                },
                {
                    data: 'userSolicitaFV', name: 'UserSolicitaFV', title: 'Usuario que Solicitó'.bold()
                },
                { data: 'almacenFV', name: 'IdAlmacenFV', title: 'Almacen'.bold() },
                { data: 'estatusFV', name: 'EstatusFV', title: 'Estatus'.bold() },
                {
                    data: 'fechaEntregaIFV', name: 'FechaEntregaIFV', title: 'Fecha de Entrega'.bold(),
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaEntregaIFV === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaEntregaIFV).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'nombreRecibioIFV', name: 'NombreRecibioIFV', title: 'Usuario que recibió'.bold() },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return row.acciones;
                        }
                    }
                }
            ],
        });
        $(document).on('change', '#almacenGrid', function (e) {
            KTDatatableRemoteAjax.recargar();
        });

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



