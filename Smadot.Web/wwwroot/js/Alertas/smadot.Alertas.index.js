const KTDatatableRemoteAjax = function () {
    let datatable;
    const init = function () {
        datatable = $('#KT_DataTable').DataTable({
            language: {
                "decimal": "",
                "emptyTable": "No hay información",
                "info": "Mostrando _END_ de _TOTAL_ entradas",
                "infoEmpty": "Mostrando 0 de 0 entradas",
                "infoFiltered": "(Filtrado _TOTAL_ de _MAX_ total entradas)",
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
            lengthMenu: [10, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            order: [[1, 'desc']],
            ajax: {
                url: siteLocation + 'Alertas/Consulta',
                type: 'POST',
                data: function (d) {

                }
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'tableName', name: 'TableName', title: 'Tipo' },
                { data: 'fechaStr', name: 'Fecha', title: 'Fecha' },
                { data: 'fechaModificacionStr', name: 'FechaModificacion', title: 'Fecha Atención' },
                { data: 'movimientoInicial', name: 'MovimientoInicial', title: 'Mensaje' },
                // { data: 'atendidaStr', name: 'FechaModificacion', title: 'Atendida' },
                { data: 'leidoStatus', name: 'Leido', title: 'Leída' },
                { data: 'movimientoFinal', name: 'MovimientoFinal', title: 'Acción Realizada' },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            const htmlString = row.acciones;
                            return htmlString
                        }
                    }
                }
            ],
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');
        listeners();
    };

    const recargar = function () {
        datatable.ajax.reload();
    }

    const listeners = function () {
        //$(document).on('click', '.btnDetalle', function (e) {
        //    const id = $(this).data('id');
        //    var url = siteLocation + 'Alertas/Detalle?id=' + id ;
        //    location.href = url;
        //});

        $(document).on('click', '.btnAtender', function (e) {
            let tableName = $(this).data('tablename');
            let id = $(this).data('id');
            var url = siteLocation + `Alertas/Redireccionar?tableName=${tableName}&&id=${id}`;
            location.href = url;
        });
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