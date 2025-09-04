"use strict"
var datatable;

var KTDatatableRemoteAjax = function () {
    var table;
    var initDatatable = function () {
        const tableRows = table.querySelectorAll('tbody tr');
        datatable = $('#kt_datatable').DataTable({
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
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) { //COLORES DE LA TABLA 
                if (aData.cantidadStock > aData.cantidadMedia) {
                    $('td', nRow).css('color', '#00b347'); //VERDE
                }
                else if (aData.cantidadStock >= aData.cantidadMinima) {
                    $('td', nRow).css('color', '#ff8000'); //NARANJA
                }
                else if (aData.cantidadStock < aData.cantidadMinima) {
                    $('td', nRow).css('color', '#cc0605'); //ROJO
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
                url: 'ConsultaStockDVRF/Consulta',
                type: 'POST'
            },
            columnDefs: [{
                defaultContent: "-",
                targets: "_all"
            }],
            columns: [
                { data: 'claveCertificado', name: 'ClaveCertificado', title: 'Clave del Certificado' },
                { data: 'nombreTipoCertificado', name: 'NombreTipoCertificado', title: 'Tipo Certificado' },
                { data: 'almacen', name: 'almacen', title: 'Almacen' },
                { data: 'numeroSolucitud', name: 'NumeroSolucitud', title: 'Número de Solucitud' },
                { data: 'cantidadStock', name: 'CantidadStock', title: 'Cantidad' },
                { data: 'numeroCaja', name: 'NumeroCaja', title: 'Caja' },
                { data: 'folioInicial', name: 'FolioInicial', title: 'Folio Inicial' },
                { data: 'folioFinal', name: 'FolioFinal', title: 'Folio Final' },
                
            ]


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
                        var url = siteLocation + 'ConsultaStockDVRF/CrearReporte?search=' + search + "&almacen=0&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn;

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
                        var url = siteLocation + 'ConsultaStockDVRF/CrearReporte?search=' + search + "&almacen=0&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn + "&esReportePdf=" + true;
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

    //BUSQUEDA SELECT
    var imagen;
    var init = function () {
        $("#divForm").hide();
        $('#IdAlmacen').on('change', function () {
            if (this.value != '') {
                blockUI.block();
                $.ajax({
                    cache: false,
                    type: 'GET',
                    url: siteLocation + 'ConsultaStockDVRF/Consulta/' + this.value,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error('Ocurrió un error al consultar.', "SMADSOT");
                            blockUI.release();
                            return;
                        } else {
                            $("#divForm").show();
                            $('#divForm .card-body').html('');
                            $('#divForm .card-body').html(result.result);
                            blockUI.release();
                        }
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, "SMADSOT");
                        blockUI.release();
                        return;
                    }
                });
            } else {
                $("#divForm").hide();
            }
        });
    };
}();



$(document).on('change', '#almacenGrid', function (e) {
    var selectVal = $("#almacenGrid option:selected").val();
    datatable.ajax.url('ConsultaStockDVRF/Consulta?IdAlmacen=' + selectVal).load();
});


jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});