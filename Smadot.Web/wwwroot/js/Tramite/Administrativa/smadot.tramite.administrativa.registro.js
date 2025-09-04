"use strict";

var validation; // https://formvalidation.io/
var formData = new FormData();
var myDropzone, myDropzone2, myDropzone3, myDropzone4;

var KTDatatableRemoteAjax = function () {
    var imagen;
    var init = function () {
        //ConsultaAutocomplete();
        // $("#IdTipoCombustible").select2();
        // $("#IdCatTipoCertificado").select2();
        // $("#EntidadProcedencia").select2();
        formData = new FormData();
        $('#Placa, #Serie').keyup(function () {
            this.value = this.value.toLocaleUpperCase();
        });
        $("#FechaEmisionReferencia, #FechaPago").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            minYear: 1901,
            maxYear: parseInt(moment().format("YYYY"), 12),
            autoApply: true,
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        });
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
        myDropzone2 = new Dropzone("#dropzonejs2", {
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
        myDropzone3 = new Dropzone("#dropzonejs3", {
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
        myDropzone4 = new Dropzone("#dropzonejs4", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs4", // Este es el contenedor donde se mostrarán las vistas previas
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

        $("#btnGuardarAdministrativa").click(function (e) {
            e.preventDefault();
            //toastr.success('Registro guardado exitosamente', "SMADSOT");
            GuardarAdministrativa.init();
        });

    };
    //var ConsultaAutocomplete = function () {
    //    $("#ConsultaAutoComplete").autocomplete({
    //        source: function (request, response) {
    //            $.ajax({
    //                url: siteLocation + "Administrativa/ConsultaAutocomplete",
    //                type: "POST",
    //                dataType: "json",
    //                data: {
    //                    prefix: request.term
    //                },
    //                success: function (data) {
    //                    //response(data);
    //                    response($.map(data, function (item) {
    //                        return { label: item.placaSerie, placa: item.placa, value: item.id, serie: item.serie };
    //                    }));
    //                }
    //            });
    //        },
    //        select: function (event, ui) {
    //            event.preventDefault();
    //            $("#ConsultaAutoComplete").val(ui.item.placaSerie);

    //            $("#Placa").val('');
    //            $("#Serie").val('');

    //            //limpiar busqueda

    //            $("#Placa").val(ui.item.placa);
    //            $("#Serie").val(ui.item.serie);

    //        },
    //        focus: function (event, ui) {
    //            event.preventDefault();
    //            $("#ConsultaAutoComplete").val(ui.item.placaSerie);
    //        },
    //        //search: function (event, ui) {
    //        //    event.preventDefault();
    //        //    //$("#cp").val("");
    //        //    //$("#Estado").val('');
    //        //    //$("#Pais").val('');
    //        //    //$("#Municipio").val('');
    //        //    //$("#IdCatColonia").find('option').remove();
    //        //}
    //    });
    //}

    return {
        init: function () {
            init();
        }
    }

}();

var GuardarAdministrativa = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
        guardar();

    };
    var guardar = function () {
        validation.validate().then(async function (status) {

            if (status === 'Valid') {
                blockUI.block();

                const formElement = document.querySelector("#form_registro");
                let formSent = new FormData(formElement);
                //var formSent = new formSent();
                // formSent.set("Placa", $("#Placa").val());
                // formSent.set("Serie", $("#Serie").val());
                // formSent.set("IdCatMotivoTramite", $("#MotivoTramite").val());
                // formSent.set("NumeroReferencia", $("#NumReferencia").val());
                // formSent.set("FolioAsignado", $("#FolioAsignado").val());
                var files = [];
                files.push(Dropzone.forElement("#dropzonejs1").files[0]);
                files.push(Dropzone.forElement("#dropzonejs2").files[0]);
                files.push(Dropzone.forElement("#dropzonejs3").files[0]);
                files.push(Dropzone.forElement("#dropzonejs4").files[0]);
                files = files.filter(element => {
                    return element !== undefined;
                });
                if (files.length > 0) {
                    var arrayOfBase64 = await readFile(files);
                    files = [];
                    for (let i = 0; i < arrayOfBase64.length; i++) {
                        files.push((arrayOfBase64[i]));
                    }
                    formSent.set('FilesString', JSON.stringify(files));
                }

                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "Administrativa/CreateAdministrativa",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formSent,
                    success: function (result) {
                        blockUI.release();
                        if (result.error) {
                            toastr.error(result.errorDescription, '');
                        } else {
                            toastr.success(result.errorDescription, '');
                            $("#btnGuardarAdministrativa").prop("disabled", true);
                            setTimeout(function () { location.href = siteLocation + "Administrativa" }, 2000);
                        }
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error("Error", '');
                    }
                });
            } else {
                // // KTUtil.scrollTop();
            }
        });
    };
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
    var init_validacion = function () {
        if (validation) validation.destroy();
        var form = document.getElementById('form_registro');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    Placa: {
                        validators: {
                            notEmpty: {
                                message: 'La placa es requerida'
                            },
                            stringLength: {
                                max: 7,
                                min: 5,
                                message: "Las placas no son válidas.",
                            },
                            regexp: {
                                regexp: /^[a-zA-Z0-9]*$/i,
                                message: "No se permiten caracteres especiales. Omitir los guiones en la placa.",
                            },
                        }
                    },
                    Serie: {
                        validators: {
                            notEmpty: {
                                message: 'El número de serie es requerido'
                            },
                            stringLength: {
                                max: 17,
                                min: 10,
                                message: "El número de serie no es válido.",
                            },
                            regexp: {
                                regexp: /^[a-zA-Z0-9]*$/i,
                                message: "No se permiten caracteres especiales.",
                            },
                        }
                    },
                    Marca: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la marca del vehículo.'
                            },
                        }
                    }
                    , Submarca: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la submarca del vehículo.'
                            },
                        }
                    }
                    , IdTipoCombustible: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la tipo de combustible del vehículo.'
                            },
                        }
                    }
                    , IdCatTipoCertificado: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la tipo de certificado para el vehículo.'
                            },
                        }
                    }
                    , Modelo: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la año modelo del vehículo.'
                            },
                        }
                    }
                    , TarjetaCirculacion: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar el númerod e tarjeta de circulación del vehículo.'
                            },
                        }
                    }
                    , EntidadProcedencia: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la entidad de procedencia del vehículo.'
                            },
                        }
                    }
                    , FechaEmisionReferencia: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la fecha de emisión.'
                            },
                        }
                    }
                    , FechaPago: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la fecha de pago.'
                            },
                        }
                    }
                    , NumeroReferencia: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar el número de referencia.'
                            },
                        }
                    }
                    , NombrePropietario: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar el nombre del propietario.'
                            },
                        }
                    }
                    , IdCatMotivoTramite: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar el motivo del trámite.'
                            },
                        }
                    }


                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap()
                }
            }
        );
    }
    return {
        init: function () {
            init();
        }
    }

}();


jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});