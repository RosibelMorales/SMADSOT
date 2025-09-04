"use strict";

var validation; // https://formvalidation.io/
var myDropzone, myDropzone2, myDropzone3, myDropzone4;

var KTDatatableRemoteAjax = function () {
    var imagen;
    var init = function () {
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
        $("#FechaCarta").daterangepicker({
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
        $("#FechaEmisionRef, #FechaPago").daterangepicker({
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
        const td = new tempusDominus.TempusDominus(document.getElementById("HoloAnterior"), {
            localization: {
                locale: "es-MX",
                startOfTheWeek: 1,
                format: "dd/MM/yyyy"
            },
            display: {
                viewMode: "calendar",
                components: {
                    decades: true,
                    year: true,
                    month: true,
                    date: true,
                    hours: false,
                    minutes: false,
                    seconds: false
                }, buttons: {
                    clear: true,
                },
            }
        });
        // td.dates.formatInput = function(date) { {return moment(date).format('MM/DD/YYYY') } }
        if (!$("Id").val()) {
            $("#HoloAnterior").val("");
        }
        if (!$("Modelo").val()) {
            $("#Modelo").val(moment().year());
        }
        $("#save").click(function (e) {
            e.preventDefault();
            guardar();
        });
    };
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_registro')[0];
                var formData = new FormData(form);

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
                    //formData.set('Files', JSON.stringify(files));
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
                var Data = {
                    Marca: $("#Marca").val(),
                    Submarca: $("#Submarca").val(),
                    Modelo: $("#Modelo").val(),
                    Placa: $("#Placa").val(),
                    Serie: $("#Serie").val(),
                    ResultadoPrueba: $("#Resultado").val(),
                    IdCatTipoCertificado: $("#TipoCertificado").val(),
                    Vigencia: $('#Vigencia').val(),
                    FechaCartaFactura: $('#FechaCarta').val(),
                    Propietario: $("#Propietario").val(),
                    Combustible: $("#Combustible").val(),
                    NumTarjetaCirculacion: $("#TarjetaCirculacion").val(),
                    UltimoFolio: $("#UltimoFolio").val(),
                    VigenciaHoloAnterior: $('#HoloAnterior').val() ?? "",
                    NumeroReferencia: $("#Referencia").val(),
                    FechaEmisionRef: $('#FechaEmisionRef').val(),
                    FechaPago: $('#FechaPago').val(),
                    EntidadProcedencia: $("#EntidadProcedencia").val(),
                    ServidorPublico: '',
                    FilesString: JSON.stringify(files)
                }


                $.ajax({
                    cache: false,
                    type: 'POST',
                    url: siteLocation + "Exento/Registro",
                    data: Data,
                    success: function (result) {
                        if (result.error) {
                            toastr.error(result.errorDescription, "Error");
                            blockUI.release();
                        } else {
                            toastr.success("Datos guardados con exito", "SMADSOT");
                            location.href = "/Exento"
                            return;
                        }
                        return;
                    },
                    error: function (res) {
                        $(window).scrollTop(0);
                        toastr.error("Error al guardar la información", "Error");
                        blockUI.release();
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
        var form = document.getElementById('form_registro');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    Marca: {
                        validators: {
                            notEmpty: {
                                message: 'La marca es requerida'
                            }
                        }
                    },
                    Submarca: {
                        validators: {
                            notEmpty: {
                                message: 'La submarca es requerida'
                            }
                        }
                    },
                    Placa: {
                        validators: {
                            notEmpty: {
                                message: 'La placa es requerida'
                            },
                            stringLength: {
                                max: 7,
                                min: 4,
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
                    Modelo: {
                        validators: {
                            notEmpty: {
                                message: 'El modelo es requerido'
                            },
                            integer: {
                                message: 'El campo no es entero',
                            },
                            between: {
                                max: moment().year() + 1,
                                min: 1920,
                                message: "El año no es válido.",
                            },
                        }
                    },
                    //Resultado: {
                    //  validators: {
                    //    notEmpty: {
                    //      message: 'El campo es requerido'
                    //}
                    //}
                    //},
                    TipoCertificado: {
                        validators: {
                            notEmpty: {
                                message: 'El tipo de certificado es requerido'
                            }
                        }
                    },
                    // Vigencia: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La vigencia es requerida'
                    //         }
                    //     }
                    // },
                    FechaCarta: {
                        validators: {
                            notEmpty: {
                                message: 'La vigencia del Holo es requerida'
                            }
                        }
                    },
                    Propietario: {
                        validators: {
                            notEmpty: {
                                message: 'El propietario es requerido'
                            }
                        }
                    },
                    Combustible: {
                        validators: {
                            notEmpty: {
                                message: 'El tipo de combustible es requerido'
                            }
                        }
                    },
                    TarjetaCirculacion: {
                        validators: {
                            notEmpty: {
                                message: 'La tarjeta de circulación es requerida'
                            }
                        }
                    },
                    Referencia: {
                        validators: {
                            notEmpty: {
                                message: 'El núm de referencia es requerido'
                            }, stringLength: {
                                max: 20,
                                min: 10,
                                message: "El número de refeferencia no es válido.",
                            },
                        }
                    },
                    EntidadProcedencia: {
                        validators: {
                            notEmpty: {
                                message: 'La entidad es requerida'
                            }
                        }
                    },
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
            init_validacion();
        }
    }
}();


jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});