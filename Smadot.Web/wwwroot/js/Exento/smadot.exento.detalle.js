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
            url: siteLocation + 'Exento/DescargarDocumento?url=' + url,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>' + result.result.fileName + '</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64 + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');

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

var ModalEditRefrendo = function () {
    let RefDropzone, RefDropzone2, RefDropzone3, idExento;

    var init = function (id) {
        idExento = id;
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Refrendo/EditPartial/',
            data: {
                idExento: id
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro').attr('data-bs-backdrop', 'static');
                $('#modal_registro .modal-title').html('Registro Refrendo');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro .modal-dialog').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $('#modal_registro').modal('show');

                $('.modal-backdrop').css('--bs-backdrop-zindex', '1056');
                // if ($('#IdExento').val() === '0')
                //     $('#IdExento').val(ViewBagIdEx);
                // $('#_datos').show();
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
        //Se modifica los refrendos para pertenecer a un Exento (reunion 13/01/2023)
        //if (ViewBagId === '0') {
        // if ($('#Id').val() === '0') {
        // $('#btnGuardarRegistro').removeClass('d-none');
        // $(document).on('change', '.checkVerificacion', function (e) {
        //     var check = $(this);
        //     if (check.is(':checked')) {
        //         $('.checkVerificacion').prop('checked', false);
        //         check.prop('checked', true);
        //     }
        // });

        RefDropzone = new Dropzone("#dropzonejs1", {
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
        RefDropzone2 = new Dropzone("#dropzonejs2", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs2", // Este es el contenedor donde se mostrarán las vistas previas
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
        RefDropzone3 = new Dropzone("#dropzonejs3", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs3", // Este es el contenedor donde se mostrarán las vistas previas
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
        GuardarFormularioRef.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormularioRef.guardar();
        });

        $('#FechaCartaFactura, #VigenciaHoloAnterior, #FechaEmisionRef, #FechaPago').daterangepicker({
            alwaysShowCalendars: true,
            singleDatePicker: true,
            showDropdowns: true,
            showCustomRangeLabel: false,
            autoApply: true,
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        }).on('show.daterangepicker', function (ev, picker) {
            // Ajusta el z-index del calendario para que sea mayor que el z-index del modal
            picker.container.css("z-index", 1061);
        });
        $('#EntidadProcedencia').select2();
        // } else {
        //     $('#btnGuardarRegistro').remove();
        //     var newOption = new Option($('#PlacaVerificacion').val(), $('#IdVerificacion').val(), false, false);
        //     $('#autocomplete').append(newOption).trigger('change');
        //     $('#autocomplete').prop('disabled', true);
        //     $('.checkVerificacion').prop('checked', true).prop('disabled', true);
        //     $('#NumeroReferencia').prop('disabled', true);
        //     $('#Placa').prop('disabled', true);
        //     $('#Propietario').prop('disabled', true);
        //     $('#EntidadProcedencia').prop('disabled', true);

        //     $(document).on('click', '.descargarDoc', function (e) {
        //         DescargarDocumento.generar($(this).data('url'));
        //     });
        // }
    }
    const cerrar = () => {
        $('#modal_registro').modal('hide');
    }
    var recargarRefrendos = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Exento/ListRefrendos/' + idExento,
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $("#container-refrendos").html(result.result);
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }

    return {
        init: function (id) {
            init(id);
        },
        recargarRefrendos: function () {
            recargarRefrendos();
        },
        cerrar: function () {
            cerrar();
        }
    }
}();
var GuardarFormularioRef = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                //Se modifica los refrendos para pertenecer a un Exento (reunion 13/01/2023)
                //if ($('.checkVerificacion:checked').length !== 1) {
                //    toastr.error('Debe seleccionar una verificación.', "SMADSOT");
                //    return;
                //}
                var form = $('#form_registro')[0];
                var formData = new FormData(form);
                formData.set('FechaCartaFactura', formData.get('FechaCartaFactura').split('/').reverse().join('/'));
                formData.set('FechaPago', formData.get('FechaPago').split('/').reverse().join('/'));
                var files = [];
                files.push(Dropzone.forElement("#dropzonejs1").files[0]);
                files.push(Dropzone.forElement("#dropzonejs2").files[0]);
                files.push(Dropzone.forElement("#dropzonejs3").files[0]);
                files = files.filter(element => {
                    return element !== undefined;
                });
                if (files.length > 0) {
                    var arrayOfBase64 = await readFile(files);
                    files = [];
                    for (let i = 0; i < arrayOfBase64.length; i++) {
                        files.push((arrayOfBase64[i]));
                    }
                    formData.set('Files', JSON.stringify(files));
                }
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/Refrendo/Edit',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        ModalEditRefrendo.cerrar();
                        ModalEditRefrendo.recargarRefrendos();
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
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
        var form = document.getElementById('form_registro');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    NumeroReferencia: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                            regexp: {
                                regexp: /^[a-zA-Z0-9]*$/i,
                                message: "No se permiten caracteres especiales.",
                            },
                        }
                    },
                    Placa: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                            regexp: {
                                regexp: /^[a-zA-Z0-9]*$/i,
                                message: "No se permiten caracteres especiales.",
                            },
                        }
                    },
                    FechaEmisionRef: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                        }
                    }, FechaPago: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                        }
                    }, EntidadProcedencia: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                        }
                    },
                    
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
