"use strict"

var datatable;
var KTDatatableRemoteAjax = function () {
    var table;
    var datatable;
    var initDatatable = function () {
        const tableRows = table.querySelectorAll('tbody tr');
        datatable = $(table).DataTable({
            language: {
                decimal: "",
                emptyTable: "No hay información",
                info: "Mostrando _END_ de _TOTAL_ entradas",
                infoEmpty: "Mostrando 0 de 0 entradas",
                infoFiltered: "(Filtrado de _MAX_ total entradas)",
                infoPostFix: "",
                thousands: ",",
                lengthMenu: "Mostrar _MENU_ Entradas",
                loadingRecords: "Cargando...",
                processing: "Procesando...",
                search: "Buscar:",
                zeroRecords: "No se encontraron resultados para mostrar",
                paginate: {
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
            ajax: {
                url: siteLocation + 'FoliosVendidosCentrosVerificacion/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                defaultContent: "-",
                targets: "_all"
            }],
            columns: [

                { data: 'folioVenta', name: 'FolioVenta', title: 'Folio de venta' },
                { data: 'folioFV', name: 'FoliosFV', title: 'Folios de FV' },
                { data: 'claveVenta', name: 'ClaveVenta', title: 'Clave de venta' },
                { data: 'foliosStock', name: 'FoliosStock', title: 'Folios en stock' },
                { data: 'referenciaBancaria', name: 'ReferenciaBancaria', title: 'Referencia bancaria' },
                { data: 'montoString', name: 'Monto', title: 'Monto de cada venta' },
                {
                    data: 'fecha', name: 'Fecha', title: 'Fecha',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fecha).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'acciones', name: '', title: '', orderable: false, },

            ],
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');
    }

    var exportButtons = () => {
        const documentTitle = 'Folios vendidos en centros de verificación';
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'excelHtml5',
                    action: function (e, dt, node, config) {
                        var search = dt.ajax.params().search.value;
                        var sortColumnDirection = dt.ajax.params().order[0].dir;
                        var sortcolumn = dt.ajax.params().columns[dt.ajax.params().order[0].column].name;
                        var url = siteLocation + 'FoliosVendidosCentrosVerificacion/CrearReporte?search=' + search + "&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn;

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
                        var url = siteLocation + 'FoliosVendidosCentrosVerificacion/CrearReporte?search=' + search + "&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn + "&esReportePdf=" + true;
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

    return {
        init: function () {
            table = document.querySelector('#kt_datatable');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            handleSearchDatatable();
        }
    };
}();
$(document).off().on('click', '.btnDetalle', function (e) {
    var id = $(this).data('id');
    blockUI.block();
    ModalDetalle.init(id);
});
var ModalDetalle = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'FoliosVendidosCentrosVerificacion/DetalleFolioVendido/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modalNotificacion .modal-title').html('Detalle');
                $('#modalNotificacion .modal-body').html('');
                $('#modalNotificacion .modal-dialog').addClass('modal-xl');
                $('#modalNotificacion .modal-body').html(result.result);
                $('#modalNotificacion').modal('show');
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
        blockUI.release();
    }
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarModalRegistro').click();
    }
    return {
        init: function (id) {
            init(id);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});