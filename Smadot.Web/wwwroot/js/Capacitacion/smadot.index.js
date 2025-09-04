var myDropzone, myDropzone2, myDropzone3;
moment.locale('es');


var ReporteGrid = function () {
    let datatable;
    var init = function () {
        datatable = $('#KT_Datatable').DataTable({
            language: {
                "sProcessing": "Procesando...",
                "sLengthMenu": "Mostrar _MENU_ registros",
                "sZeroRecords": "No se encontraron resultados",
                "sEmptyTable": "Ningún dato disponible en esta tabla",
                "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
                "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
                "sInfoPostFix": "",
                "sSearch": "Buscar:",
                "sUrl": "",
                "sInfoThousands": ",",
                "sLoadingRecords": "Cargando...",
                "oPaginate": {
                    "sFirst": "Primero",
                    "sLast": "Último",
                    "sNext": "Siguiente",
                    "sPrevious": "Anterior"
                },
                "oAria": {
                    "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                    "sSortDescending": ": Activar para ordenar la columna de manera descendente"
                }
            },
            responsive: true,
            pagingType: 'simple_numbers',
            searching: true,
            lengthMenu: [10, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: 'Capacitacion/Consulta',
                type: 'POST',
                data: function (d) {
                   
                }
            },
            columnDefs: [
                {
                    "defaultContent": "-",
                    targets: ['_all'],
                    className: 'mdc-data-table__cell',
                },
            ],
            
            
            columns: [
                { data: 'idCapacitacion', name: 'IdCapacitacion', title: 'Id' },
                {
                    data: 'fechaCapacitacion', name: 'FechaCapacitacion', title: 'Fecha',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaCapacitacion === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaCapacitacion).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'temaCapacitacion', name: 'TemaCapacitacion', title: 'Tema' },
                { data: 'totalAsistentes', name: 'TotalAsistentes', title: 'Asistentes' },
                { data: 'nombreCatEstatusCapacitacion', name: 'NombreCatEstatusCapacitacion', title: 'Estatus' },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display' && permisoDetalle) {
                            return '<div class="btn-group dropup drop-up dropdown">' +
                                '<a href="Capacitacion/Edit/' + row.idCapacitacion + '" class="btn btn-sm btn-secondary btn-text-primary btn-hover-primary btn-icon mr-2" title="Detalles">' +
                                '<i class="bi bi-list-check"></i>' +
                                '</a>' +
                                '</div>';                          
                        }
                    }
                }
            ],
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800'); 
        listeners();
    };

    var recargar = function () {
        datatable.ajax.reload();
    }

    var listeners = function () {

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
    ReporteGrid.init();
});