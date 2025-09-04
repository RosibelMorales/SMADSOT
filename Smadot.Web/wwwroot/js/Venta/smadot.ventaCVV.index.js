"use strict"
var datatable;

var KTDatatableRemoteAjax = function () {
    var table;
    var initDatatable = function () {
        const tableRows = table.querySelectorAll('tbody tr');
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
            aaSorting: [[0, "desc"]],
            ajax: {
                url: 'VentaCVV/Consulta',
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
                { data: 'idSolicitudFV', name: 'IdSolicitudFV', title: 'Número de Solicitud' },
                {
                    data: 'fechaSolicitudFV', name: 'FechaSolicitudFV', title: 'Fecha de Solicitud',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaSolicitudFV).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'userSolicitaFV', name: 'UserSolicitaFV', title: 'Usuario que Solicitó' },
                { data: 'estatusFV', name: 'EstatusFV', title: 'Estatus' },
                {
                    data: 'fechaEntregaIFV', name: 'FechaEntregaIFV', title: 'Fecha de Entrega',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaEntregaIFV === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaEntregaIFV).format('DD/MM/YYYY');
                        }
                    }
                },
                {
                    data: 'fechaVentaVFV', name: 'FechaVentaVFV', title: 'Fecha de Conclusión',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaVentaVFV === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaVentaVFV).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'nombreRecibioIFV', name: 'NombreRecibioIFV', title: 'Usuario que recibió' },
                {
                    title: 'Importe Total',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return Number(row.importeTotal).toLocaleString('en-US', {
                                style: 'currency',
                                currency: 'USD'
                            });
                        }
                    }
                },
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
    }

    var exportButtons = () => {
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'excelHtml5',
                    action: function (e, dt, node, config) {
                        var search = dt.ajax.params().search.value;
                        var sortColumnDirection = dt.ajax.params().order[0].dir;
                        var sortcolumn = dt.ajax.params().columns[dt.ajax.params().order[0].column].name;
                        var url = siteLocation + 'VentaCVV/CrearReporte?search=' + search + "&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn;

                        $.ajax({
                            cache: false,
                            type: 'GET',
                            url: url,
                            success: function (result) {
                                if (result.error) {
                                    var text = result.errorDescription.split("|");
                                    toastr.error(text[0], "");
                                } else {
                                    $("#linkDownloadReport").get(0).click();
                                }

                                return;
                            },
                            error: function (res) {
                                toastr.error("Error no controlado", "");
                            }
                        });
                    }
                },
                {
                    extend: 'pdfHtml5',
                    action: function (e, dt, node, config) {
                        var search = dt.ajax.params().search.value;
                        var sortColumnDirection = dt.ajax.params().order[0].dir;
                        var sortcolumn = dt.ajax.params().columns[dt.ajax.params().order[0].column].name;
                        var url = siteLocation + 'VentaCVV/CrearReporte?search=' + search + "&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn + "&esReportePdf=" + true;
                        window.open(url, "_blank");
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_buttons'));

        const exportButtons = document.querySelectorAll('#kt_datatable_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);
                target.click();
            });
        });
    }

    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    var recargar = function () {
        datatable.ajax.reload();
    }

    return {
        init: function () {
            table = document.querySelector('#kt_datatable');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        },
        recargar: function () {
            recargar();
        }
    };
}();

$(document).on('change', '#almacenGrid', function (e) {
    KTDatatableRemoteAjax.recargar();
});

$(document).on('click', '.btnRecibo', function (e) {
    blockUI.block();
    var idVenta = $(this).data('idventa');
    var idAlmacen = $(this).data('idalmacen');
    $.ajax({
        cache: false,
        type: 'GET',
        processData: false,
        contentType: false,
        url: siteLocation + 'VentaCVV/GetPDF?idAlmacen=' + idAlmacen + "&idVenta=" + idVenta,
        success: function (result) {
            if (!result.isSuccessFully) {
                toastr.error("Ocurrió un error al generar el recibo", 'SMADSOT');
                blockUI.release();
                return;
            }
            var win = window.open();
            win.document.write('<html><head><title>Venta de formas valoradas a verificentros</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:application/pdf;base64,' + result.result + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
            toastr.success('PDF generado correctamente.', 'SMADSOT');
            blockUI.release();
            return;
        },
        error: function (res) {
            toastr.error(res, 'SMADSOT');
            blockUI.release();
        }
    });
});

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});