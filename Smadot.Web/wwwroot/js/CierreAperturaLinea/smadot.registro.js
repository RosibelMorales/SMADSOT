"use strict"
var btns = false;
var myDropzone;

$(document).on('click', '#btnRegistro, .btnRegistro', function (e) {
    var id = $(this).data('id');

    ModalEdit.init(id);
});


var ModalEdit = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/CierreAperturaLinea/Edit/' + id,
            data: function (d) {
                d.id = $('#motivoGrid').select2('data')[0].id;
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Apertura de linea' : 'Detalle de Linea');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#motivoGrid').select2({
                        dropdownParent: $('#modal_registro'),
                        width: 'resolve',

                    });
                });
                //if (!btns) {
                //   btns = true;
                //} 
                $('#modal_registro').modal('show');
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
        myDropzone = new Dropzone("#dropzonejs", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs", // Este es el contenedor donde se mostrarán las vistas previas
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

        GuardarFormulario.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });


    }

    // Cerramos la ventana modal
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarRegistro').click();
    }

    window.addEventListener("keypress", function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
        }
    }, false);


    return {
        init: function (id) {
            init(id);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();


var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };

    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_registroCierreAperturaLinea')[0];
                var formData = new FormData(form);
                formData.set('[0].NombreLinea', formData.get('NombreLinea'));
                formData.set('[0].Motivo', formData.get('MotivoLinea'));
                formData.set('[0].NotasMotivo', formData.get('NotasMotivo'));
                formData.set('[0].IdMotivo', formData.get('motivoGrid'));
                formData.set('[0].Clave', formData.get('claveLinea'));
                var files = [];
                files.push(Dropzone.forElement("#dropzonejs").files[0]);
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
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                        ModalEdit.cerrarventanamodal();
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
        var form = document.getElementById('form_registroCierreAperturaLinea');

        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    NombreLinea: {
                        validators: {
                            notEmpty: {
                                message: 'El nombre es requerido.'
                            }
                        }
                    },
                    MotivoLinea: {
                        validators: {
                            notEmpty: {
                                message: 'El motivo es requerido.'
                            }
                        }
                    },
                    NotasMotivo: {
                        validators: {
                            notEmpty: {
                                message: 'La nota es requerida.'
                            }
                        }
                    },
                    claveLinea: {
                        validators: {
                            notEmpty: {
                                message: 'La clave es requerida.'
                            },
                            stringLength: {
                                min: 3,
                                max: 3,
                                message: 'La clave deben ser 3 carácteres.',
                            },
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
        guardar: function () {
            guardar();
        }
    }

}();

var init = function () {
    $("#divForm").hide();
    //$('#IdAlmacen').on('change', function () {
    //    if (this.value != '') {
    //        blockUI.block();
    //        $.ajax({
    //            cache: false,
    //            type: 'GET',
    //            url: siteLocation + 'ConsultaStockDVRF/Consulta/' + this.value,
    //            success: function (result) {
    //                if (!result.isSuccessFully) {
    //                    toastr.error('Ocurrió un error al consultar.', "SMADSOT");
    //                    blockUI.release();
    //                    return;
    //                } else {
    //                    $("#divForm").show();
    //                    $('#divForm .card-body').html('');
    //                    $('#divForm .card-body').html(result.result);
    //                    blockUI.release();
    //                }
    //                return;
    //            },
    //            error: function (res) {
    //                toastr.error(res, "SMADSOT");
    //                blockUI.release();
    //                return;
    //            }
    //        });
    //    } else {
    //        $("#divForm").hide();
    //    }
    //});
};

//$(document).on('change', '#motivoGrid', function (e) {

//    var selectVal = $("#motivoGrid option:selected").val();
//    datatable.ajax.url('ConsultaStockDVRF/Consulta?IdAlmacen=' + selectVal).load();


//});




