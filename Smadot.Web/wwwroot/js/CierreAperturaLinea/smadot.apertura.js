"use strict"
var btns = false;
var myDrop;

$(document).on('click', '#btnTryApertura, .btnTryApertura', function (e) {
    var id = $(this).data('id');

    ModalApertura.init(id);
});

var ModalApertura = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/CierreAperturaLinea/Update/' + id,
            data: function (d) {
                d.id = $('#motivoGrid').select2('data')[0].id;
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_cierre #modalLabelTitleCierre').html(id === undefined ? 'Apertura de linea' : 'Apertura de linea');
                $('#modal_cierre .modal-body').html('');
                $('#modalClassCierre').addClass('modal-xl');
                $('#modal_cierre .modal-body').html(result.result);
                $("#modal_cierre").on('shown.bs.modal', function () {
                    $('#motivoSelect').select2({
                        dropdownParent: $('#modal_cierre')
                    });
                });
                //if (!btns) {
                //   btns = true;
                //} 
                $('#modal_cierre').modal('show');
                listeners();
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
        myDropzone = new Dropzone("#dropzonejs1", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs1", // Este es el contenedor donde se mostrarán las vistas previas
            init: function () {
                this.on("addedfile", function (file) {
                    if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
                        return file.type.includes(acceptedType);
                    })) {
                        toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
                        toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (this.files.length > this.options.maxFiles) {
                        toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
                        this.removeFile(file);
                    }
                });
                this.on("removedfile", function (file) {
                    // Aquí puedes realizar acciones cuando un archivo se elimina
                });
            },
        });

        GuardarCierre.init();

        $('#btnGuardarCierre').off().on('click', function (e) {
            e.preventDefault();
            GuardarCierre.cerrar();
        });


    }

    // Cerramos la ventana modal
    var cerrarModalCierre = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarRegistro').click();
    }

    return {
        init: function (id) {
            init(id);
        },
        cerrarventanamodalCierre: function () {
            cerrarModalCierre();
        }
    }
}();

