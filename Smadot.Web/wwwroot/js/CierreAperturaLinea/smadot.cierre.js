"use strict"
var btns = false;
var myDrop;

$(document).on('click', '#btnCierre, .btnCierre', function (e) {
    var id = $(this).data('id');

    ModalCierre.init(id);
});

var ModalCierre = function () {

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
                $('#modal_cierre #modalLabelTitleCierre').html(id === undefined ? 'Cierre de linea' : 'Cierre de Linea');
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
        $('#btnCerrarLinea').click();
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


var GuardarCierre = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };

    var cerrar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_registroCierreLinea')[0];
                var formData = new FormData(form);
                formData.set('[0].Id', formData.get('Id'));
                formData.set('[0].NotasMotivo', formData.get('NotasMotivo'));
                formData.set('[0].IdMotivo', formData.get('motivoSelect'));
                formData.set('[0].Estatus', formData.get('IdEstatus'));

                var files = [];
                files.push(Dropzone.forElement("#dropzonejs1").files[0]);
                files = files.filter(element => {
                    return element !== undefined;
                });

                if (files.length > 0) {
                    if (files.length > 0) {
                        var arrayOfBase64 = await readFile(files);
                        files = [];
                        for (let i = 0; i < arrayOfBase64.length; i++) {
                            files.push((arrayOfBase64[i]));
                        }
                        formData.set('Files', JSON.stringify(files));
                    }
                }
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/CierreAperturaLinea/Edit',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success('La linea se ha cerrado correctamente.', 'SMADSOT');
                        ModalCierre.cerrarventanamodalCierre();
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, 'SMADSOT');
                    }
                });
            }
        });
    };

    var init_validacion = function () {
        var form = document.getElementById('form_registroCierreLinea');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    NotasMotivo: {
                        validators: {
                            notEmpty: {
                                message: 'La nota es requerida.'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap({
                        rowSelector: '.fv-row',
                        eleInvalidClass: 'is-invalid',
                        eleValidClass: ''
                    })
                }
            }
        );

    }

    async function readFile(fileList) {
        function getBase64(file) {
            let fileData = {};
            fileData.Nombre = file.name;
            fileData.Tipo = file.type.split("/")[1];

            const reader = new FileReader()
            return new Promise((resolve) => {
                reader.onload = (ev) => {
                    var base64Data = ev.target.result.split('base64,')[1];
                    fileData.Base64 = base64Data;
                    resolve(fileData)
                }
                reader.readAsDataURL(file)
            })
        }

        const promises = []

        for (let i = 0; i < fileList.length; i++) {
            promises.push(getBase64(fileList[i]))
        }
        return await Promise.all(promises)
    }

    return {
        init: function () {
            init();
        },
        cerrar: function () {
            cerrar();
        }
    }

}();

