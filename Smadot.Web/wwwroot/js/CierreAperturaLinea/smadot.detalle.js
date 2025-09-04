"use strict"

//var btns = false;
moment.locale('es');

$(document).on('click', '#btnDetalle, .btnDetalle', function (e) {
    var id = $(this).data('id');
    ModalDetail.init(id);
});

var ModalDetail = function () {
    let dt_detalleTable;
    var init = function (id) {
        abrirModal(id);
    }
    const initDatatable = () => {
        datatable = $('#kt_detalleTable').DataTable({
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
            searching: false,
            lengthMenu: [15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            aaSorting: [[2, 'desc']],
            ajax: {
                url: `/CierreAperturaLinea/ConsultaDetalle/${$('#idLinea').val()??0}`,
                type: 'POST'
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'userRegistroMotivo', name: 'UserRegistroMotivo', title: 'Usuario Registró' },
                { data: 'motivo', name: 'Motivo', title: 'Motivo' },
                {
                    data: 'fechaRegistroLineaMotivo', name: 'FechaRegistroLineaMotivo', title: 'Cierre / Apertura',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaRegistroLinea === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaRegistroLineaMotivo).format('DD/MM/YYYY HH:mm') + ' hrs';
                        }
                    }
                },
                { data: 'estatusString', name: 'Estatus', title: 'Estatus' },
                { data: 'notas', name: 'Notas', title: 'Nota' },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            var htmlString = row.documento;
                            return htmlString
                        }
                    }
                }

            ],
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/CierreAperturaLinea/Edit/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_detalle #modalLabelTitleDetalle').html(id === undefined ? 'Apertura de Línea' : 'Detalle de Línea');
                $('#modal_detalle .modal-body').html('');
                $('#modalClassDetalle').addClass('modal-xl');
                $('#modal_detalle .modal-body').html(result.result);
                $("#modal_detalle").on('shown.bs.modal', function () {
                    $('#z0__IdAlmacenFV').select2({
                        dropdownParent: $('#modal_detalle')
                    });
                });
                //if (!btns) {
                //    btns = true;
                //}
                $('#modal_detalle').modal('show');
                listeners();
                initDatatable();
                return;

            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {

    }

    // Cerramos la ventana modal
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarDetalle').click();
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


$(document).on('click', '.descargarDoc', function (e) {
    DescargarDocumento.generar($(this).data('url'));
});

var DescargarDocumento = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: '/CierreAperturaLinea/DescargarDocumento?url=' + url,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>' + result.result.fileName + '</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64 + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
                //var w = window.open('data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64, '_blank');
                //w.document.title = result.result.fileName + '';
                toastr.success('Documento descargado correctamente.', 'SMADSOT');
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    };

    return {
        generar: function (url) {
            generar(url);
        }
    }

}();


$(document).on('click', '.descargarDocCierre', function (e) {
    DescargarDocumentoCierre.generar($(this).data('url'));
});

var DescargarDocumentoCierre = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: '/CierreAperturaLinea/DescargarDocumentoCierre?url=' + url,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>' + result.result.fileName + '</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64 + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
                //var w = window.open('data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64, '_blank');
                //w.document.title = result.result.fileName + '';
                toastr.success('Documento descargado correctamente.', 'SMADSOT');
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    };

    return {
        generar: function (url) {
            generar(url);
        }
    }

}();

$(document).on('click', '#docApertura, .docApertura', function (e) {
    var id = $(this).data('id');
    myFunction(id);
});

