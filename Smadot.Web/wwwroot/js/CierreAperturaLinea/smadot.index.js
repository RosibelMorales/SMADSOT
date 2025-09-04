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
            aaSorting: [[2, 'desc']],
            ajax: {
                url: '/CierreAperturaLinea/Consulta',
                type: 'POST'
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'nombreVerificentro', name: 'NombreVerificentro', title: 'Verificentro' },
                { data: 'nombreLinea', name: 'NombreLinea', title: 'Linea' },
                { data: 'clave', name: 'Clave', title: 'Clave Linea' },
                { data: 'nombreUsuario', name: 'NombreUsuario', title: 'Usuario Registro' },
                {
                    data: 'fechaRegistroLinea', name: 'FechaRegistroLinea', title: 'Cierre / Apertura',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaRegistroLinea === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaRegistroLinea).format('DD/MM/YYYY HH:mm') + ' hrs';
                        }
                    }
                },
                { data: 'estatus', name: 'Estatus', title: 'Estatus' },
                { data: 'motivo', name: 'Motivo', title: 'Motivo' },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            var htmlString = row.acciones;
                            return htmlString
                        }
                    }
                }

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

