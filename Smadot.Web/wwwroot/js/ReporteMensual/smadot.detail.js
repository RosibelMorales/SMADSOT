"use strict"
var btns = false;
var myDropzone, myDropzone2;
var accept = 2; var acceptModified = 5;
var denegar = 3; var denegarModified = 6;

$(document).on('click', '#btnDetail, .btnDetail', function (e) {
    var id = $(this).data('id');

    ModalDetail.init(id);
});


var ModalDetail = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/ReporteMensual/Edit/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_detalle #modalLabelTitleDetalle').html(id === undefined ? 'Detalle de Reporte' : 'Detalle de Reporte');
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
                /*listeners();*/
                return;

            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
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

$(document).on('click', '#btnAceptarSolicitud, .btnAceptarSolicitud', function (e) {
    var id = $(this).data('id');
    
    Swal.fire({
        title: '¿Estas Seguro?',
        text: "¡Se aceptara la Solicitud DVRF!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, estoy seguro',
        cancelButtonText: 'No, regresar'
    }).then((result) => {
        if (result.isConfirmed) {
            var form = $('#form_detalleReporteMensual')[0];
            var formData = new FormData(form);
            formData.set('[0].IdReporte', formData.get('ID'));
            formData.set('[0].IdCatEstatusReporte', accept);
            $.ajax({
                cache: false,
                type: 'POST',
                processData: false,
                contentType: false,
                enctype: 'multipart/form-data',
                url: '/ReporteMensual/Edit',
                data: formData,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    toastr.success('Los datos se actualizaron correctamente.', 'SMADSOT');
                    ModalDetail.cerrarventanamodal();
                    return;
                },
                error: function (res) {
                    toastr.error(res, 'SMADSOT');
                }
            });
        }
    })
});

$(document).on('click', '#btnDenegarSolicitud, .btnDenegarSolicitud', function (e) {
    var id = $(this).data('id');

    Swal.fire({
        title: '¿Estas Seguro?',
        text: "¡Se denegara la Solicitud DVRF!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, estoy seguro',
        cancelButtonText: 'No, regresar'
    }).then((result) => {
        if (result.isConfirmed) {
            var form = $('#form_detalleReporteMensual')[0];
            var formData = new FormData(form);
            var dato = formData.get('ID');
            formData.set('[0].IdReporte', formData.get('ID'));
            formData.set('[0].IdCatEstatusReporte', denegar);
            $.ajax({
                cache: false,
                type: 'POST',
                processData: false,
                contentType: false,
                enctype: 'multipart/form-data',
                url: '/ReporteMensual/Edit',
                data: formData,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    toastr.success('Los datos se actualizaron correctamente.', 'SMADSOT');
                    ModalDetail.cerrarventanamodal();
                    return;
                },
                error: function (res) {
                    toastr.error(res, 'SMADSOT');
                }
            });
        }
    })
});

$(document).on('click', '#btnAceptarModificar, .btnAceptarModificar', function (e) {
    var id = $(this).data('id');

    Swal.fire({
        title: '¿Estas Seguro?',
        text: "¡Se aceptara la Solicitud para modificar!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, estoy seguro',
        cancelButtonText: 'No, regresar'
    }).then((result) => {
        if (result.isConfirmed) {
            var form = $('#form_detalleReporteMensual')[0];
            var formData = new FormData(form);
            formData.set('[0].IdReporte', formData.get('ID'));
            formData.set('[0].IdCatEstatusReporte', acceptModified);
            $.ajax({
                cache: false,
                type: 'POST',
                processData: false,
                contentType: false,
                enctype: 'multipart/form-data',
                url: '/ReporteMensual/Edit',
                data: formData,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    toastr.success('Los datos se actualizaron correctamente.', 'SMADSOT');
                    ModalDetail.cerrarventanamodal();
                    return;
                },
                error: function (res) {
                    toastr.error(res, 'SMADSOT');
                }
            });
        }
    })
});

$(document).on('click', '#btnDenegarModificar, .btnDenegarModificar', function (e) {
    var id = $(this).data('id');

    Swal.fire({
        title: '¿Estas Seguro?',
        text: "¡Se aceptara la Solicitud para modificar!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, estoy seguro',
        cancelButtonText: 'No, regresar'
    }).then((result) => {
        if (result.isConfirmed) {
            var form = $('#form_detalleReporteMensual')[0];
            var formData = new FormData(form);
            formData.set('[0].IdReporte', formData.get('ID'));
            formData.set('[0].IdCatEstatusReporte', denegarModified);
            $.ajax({
                cache: false,
                type: 'POST',
                processData: false,
                contentType: false,
                enctype: 'multipart/form-data',
                url: '/ReporteMensual/Edit',
                data: formData,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    toastr.success('Los datos se actualizaron correctamente.', 'SMADSOT');
                    ModalDetail.cerrarventanamodal();
                    return;
                },
                error: function (res) {
                    toastr.error(res, 'SMADSOT');
                }
            });
        }
    })
});

$(document).on('click', '.descargarDoc', function (e) {
    DescargarDocumentoCierre.generar($(this).data('url'));
});

var DescargarDocumentoCierre = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: '/ReporteMensual/DescargarDocumento?url=' + url,
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