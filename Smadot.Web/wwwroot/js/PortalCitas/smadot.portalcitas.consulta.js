"use strict"
var myDropzone, myDropzone2, myDropzone3, myDropzone4, myDropzone5;
var validator; // https://formvalidation.io/
var stepper;
var stepperObj;
var validations = [];
var form;
var formSubmitButton;
var KTDatatableRemoteAjax = function () {
    var init = function () {

        $(document).on('click', '#btnImprimirCita', function (e) {
            DescargarComprobanteCita.generar($(this).data('url'));
        });
        $('#btnImprimirCita').off()

        $(document).on('click', '#btnCancelarCita', function (e) {
            let id = $(this).attr("data-id");
            let folio = $(this).attr("data-folio");
            CancelarCita.init(id,folio);
        });
        $('#btnCancelarCita').off()

    }
    return {
        init: function () {
            init();
        }
    };
}();

var DescargarComprobanteCita = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: siteLocation + 'PortalCitas/DescargarDocumento?url=' + url,
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


var CancelarCita = function () {
    const init = (id, folio) => {
        abrirModal(id, folio);
    }
    var abrirModal = function (id, folio) {
        Swal.fire({
            title: '¿Está seguro que desea cancelar su cita?',
            text: "¡Está acción no se podrá revertir!",
            icon: "warning",
            buttonsStyling: false,
            showCancelButton: true,
            reverseButtons: true,
            cancelButtonText: 'Cancelar',
            confirmButtonText: "Confirmar",
            focusConfirm: true,
            customClass: {
                confirmButton: "btn btn-primary",
                cancelButton: 'btn btn-secondary'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                blockUI.block();

                var formData = new FormData();
                formData.append('Id', id);
                formData.append('Folio', folio);

                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "PortalCitas/CancelarCita",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formData,
                    success: function (result) {

                        blockUI.release();
                        if (!result.isSuccessFully) {
                            blockUI.release();
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        Swal.fire({
                            title: 'Cancelación exitosa',
                            text: "Su cita fue cancelada de manera correcta",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Confirmar",
                            customClass: {
                                confirmButton: "btn btn-primary",
                                cancelButton: 'btn btn-secondary'
                            },
                            allowOutsideClick: false
                        }).then((result) => {
                            if (result.isConfirmed) {
                                window.location.href = '/'
                            }


                        });
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error(res, "SMADSOT");
                        return;
                    }
                });
            }


        });
    }
    var listeners = function () {

    }
    return {
        init: (id, folio) => {
            init(id, folio);
        }

    }
}()

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});