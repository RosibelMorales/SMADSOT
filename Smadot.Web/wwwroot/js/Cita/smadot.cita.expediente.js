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

        $('.breadcrumb').remove();
        $(document).on('click', '.imageUser', function (e) {
            e.preventDefault();
            var nombre = $(this).data('nombre')
            let element = this.querySelector('img');
            if (element) {
                var ref = element.src;
                ModalImagen.init(ref, nombre)
            }
        })
        $('.imageUser > div > img').each((i, item) => {

            $.ajax({
                cache: false,
                type: 'GET',
                processData: false,
                contentType: false,
                url: '/Citas/DescargarDocumentoExpediente?url=' + item.dataset.url,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        // toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    item.src = 'data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64;
                    // toastr.success('Documento descargado correctamente.', 'SMADSOT');
                    return;
                },
                error: function (res) {
                    // toastr.error(res, 'SMADSOT');
                }
            });
        })
        $('.imageUser').off()

        $(document).on('click', '.descargarDoc', function (e) {
            DescargarDocumento.generar($(this).data('url'));
        });
    }
    return {
        init: function () {
            init();
        }
    };
}();

var ModalImagen = function () {
    var init = function (id, nombre) {
        abrirModal(id, nombre);
    }
    var abrirModal = function (id, nombre) {

        $('#modal_registro .modal-title').html(nombre);
        $('#modal_registro .modal-body').html('');
        $('#modal_registro .modal-dialog').removeClass('modal-md');
        $('#modal_registro .modal-dialog').addClass('modal-xl');
        var stringHtml = '<div class="row mb-3 mt-3"><img class="w-100 card-rounded" src="' + id + '" alt=""></div>'
        $('#modal_registro .modal-body').html(stringHtml);
        $('#modal_registro').modal('show');
        $('#modal_registro #btnGuardarRegistro').hide();
    }

    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
    }
    return {
        init: function (id, nombre) {
            init(id, nombre);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        },
    }
}();

var DescargarDocumento = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: '/Citas/DescargarDocumentoExpediente?url=' + url,
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

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});