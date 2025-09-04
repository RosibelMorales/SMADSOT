"use strict"
var myDropzone, myDropzone2, myDropzone3;

$(document).on('click', '#btnSave', function (e) {
    GuardarFormulario.guardar();
});

var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form')[0];
                var formData = new FormData(form);
                /*formData.set('[0].FechaSolicitud', formData.get('FechaSolicitud').split('/').reverse().join('/'));*/
                formData.set('[0].FechaEntrega', formData.get('FechaEntregaIFV').split('/').reverse().join('/'));
                formData.set('[0].NombreRecibio', formData.get('NombreRecibio'));
                formData.set('[0].IdSolicitudFV', formData.get('IdSolicitudFV'));

                formData.set('[0].CantidadRecibida', formData.get('DobleCeroCantidadRecibida'));
                formData.set('[0].CantidadSolicitada', formData.get('DobleCeroCantidad'));
                formData.set('[0].FolioInicial', formData.get('DobleCeroFolioInicial'));
                formData.set('[0].FolioFinal', formData.get('DobleCeroFolioFinal'));
                formData.set('[1].CantidadRecibida', formData.get('CeroCantidadRecibida'));
                formData.set('[1].CantidadSolicitada', formData.get('CeroCantidad'));
                formData.set('[1].FolioInicial', formData.get('CeroFolioInicial'));
                formData.set('[1].FolioFinal', formData.get('CeroFolioFinal'));
                formData.set('[2].CantidadRecibida', formData.get('UnoCantidadRecibida'));
                formData.set('[2].CantidadSolicitada', formData.get('UnoCantidad'));
                formData.set('[2].FolioInicial', formData.get('UnoFolioInicial'));
                formData.set('[2].FolioFinal', formData.get('UnoFolioFinal'));
                formData.set('[3].CantidadRecibida', formData.get('DosCantidadRecibida'));
                formData.set('[3].CantidadSolicitada', formData.get('DosCantidad'));
                formData.set('[3].FolioInicial', formData.get('DosFolioInicial'));
                formData.set('[3].FolioFinal', formData.get('DosFolioFinal'));
                formData.set('[4].CantidadRecibida', formData.get('ConstanciaNoAprobadoCantidadRecibida'));
                formData.set('[4].CantidadSolicitada', formData.get('ConstanciaNoAprobadoCantidad'));
                formData.set('[4].FolioInicial', formData.get('ConstanciaNoAprobadoFolioInicial'));
                formData.set('[4].FolioFinal', formData.get('ConstanciaNoAprobadoFolioFinal'));
                formData.set('[5].CantidadRecibida', formData.get('ExentosCantidadRecibida'));
                formData.set('[5].CantidadSolicitada', formData.get('ExentosCantidad'));
                formData.set('[5].FolioInicial', formData.get('ExentosFolioInicial'));
                formData.set('[5].FolioFinal', formData.get('ExentosFolioFinal'));
                formData.set('[6].CantidadRecibida', formData.get('TestificacionCantidadRecibida'));
                formData.set('[6].CantidadSolicitada', formData.get('TestificacionCantidad'));
                formData.set('[6].FolioInicial', formData.get('TestificacionFolioInicial'));
                formData.set('[6].FolioFinal', formData.get('TestificacionFolioFinal'));

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
                    formData.set('[0].FilesString', JSON.stringify(files));
                }
                //formData.set('[0].kt_dropzonejs_example_1', formData.get('kt_dropzonejs_example_1'));
                //formData.set('[0].kt_dropzonejs_example_2', formData.get('kt_dropzonejs_example_2')); 
                //formData.set('[0].kt_dropzonejs_example_3', formData.get('kt_dropzonejs_example_3')); 
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/IngresoFormaValorada/Ingresos',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT', {
                            timeOut: 250,
                            preventDuplicates: true,

                            // Redirect 
                            onHidden: function () {
                                window.location.href = "/IngresoFormaValorada";
                            }
                        });
                    },
                    error: function (res) {
                        toastr.error(res, "SMADSOT");
                    }
                });
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
        $('#FechaEntregaIFV').daterangepicker({
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
        });
        var form = document.getElementById('form');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    NombreRecibio: {
                        validators: {
                            notEmpty: {
                                message: 'El nombre es requerido.'
                            },
                        }
                    },
                    DobleCeroCantidadRecibida: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    CeroCantidadRecibida: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    UnoCantidadRecibida: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DosCantidadRecibida: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ConstanciaNoAprobadoCantidadRecibida: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ExentosCantidadRecibida: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DobleCeroCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DobleCeroFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DobleCeroFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }

                    },
                    CeroCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    CeroFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    CeroFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    UnoCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    UnoFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    UnoFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DosCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DosFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DosFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ConstanciaNoAprobadoCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ConstanciaNoAprobadoFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ConstanciaNoAprobadoFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ExentosCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ExentosFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ExentosFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap({
                        rowSelector: '.fv-row',
                    })
                }
            }
        );

        $(document).on('change', '#DobleCeroFolioInicial, #DobleCeroCantidadRecibida', function (e) {
            var folioFinal = getNumero($('#DobleCeroFolioInicial').val()) + (getNumero($('#DobleCeroCantidadRecibida').val()) - 1);
            folioFinal = folioFinal <= 0 ? 0 : folioFinal;
            $('#DobleCeroFolioFinal').val(folioFinal);
            $('#DobleCeroFolioFinal').text(folioFinal);
        });

        $(document).on('change', '#CeroFolioInicial, #CeroCantidadRecibida', function (e) {
            var folioFinal = getNumero($('#CeroFolioInicial').val()) + (getNumero($('#CeroCantidadRecibida').val()) - 1);
            folioFinal = folioFinal <= 0 ? 0 : folioFinal;
            $('#CeroFolioFinal').val(folioFinal);
            $('#CeroFolioFinal').text(folioFinal);
        });

        $(document).on('change', '#UnoFolioInicial, #UnoCantidadRecibida', function (e) {
            var folioFinal = getNumero($('#UnoFolioInicial').val()) + (getNumero($('#UnoCantidadRecibida').val()) - 1);
            folioFinal = folioFinal <= 0 ? 0 : folioFinal;
            $('#UnoFolioFinal').val(folioFinal);
            $('#UnoFolioFinal').text(folioFinal);
        });

        $(document).on('change', '#DosFolioInicial, #DosCantidadRecibida', function (e) {
            var folioFinal = getNumero($('#DosFolioInicial').val()) + (getNumero($('#DosCantidadRecibida').val()) - 1);
            folioFinal = folioFinal <= 0 ? 0 : folioFinal;
            $('#DosFolioFinal').val(folioFinal);
            $('#DosFolioFinal').text(folioFinal);
        });

        $(document).on('change', '#ConstanciaNoAprobadoFolioInicial, #ConstanciaNoAprobadoCantidadRecibida', function (e) {
            var folioFinal = getNumero($('#ConstanciaNoAprobadoFolioInicial').val()) + (getNumero($('#ConstanciaNoAprobadoCantidadRecibida').val()) - 1);
            folioFinal = folioFinal <= 0 ? 0 : folioFinal;
            $('#ConstanciaNoAprobadoFolioFinal').val(folioFinal);
            $('#ConstanciaNoAprobadoFolioFinal').text(folioFinal);
        });

        $(document).on('change', '#ExentosFolioInicial, #ExentosCantidadRecibida', function (e) {
            var folioFinal = getNumero($('#ExentosFolioInicial').val()) + (getNumero($('#ExentosCantidadRecibida').val()) - 1);
            folioFinal = folioFinal <= 0 ? 0 : folioFinal;
            $('#ExentosFolioFinal').val(folioFinal);
            $('#ExentosFolioFinal').text(folioFinal);
        });

        $(document).on('change', '#TestificacionFolioInicial, #TestificacionCantidadRecibida', function (e) {
            var folioFinal = getNumero($('#TestificacionFolioInicial').val()) + (getNumero($('#TestificacionCantidadRecibida').val()) - 1);
            folioFinal = folioFinal <= 0 ? 0 : folioFinal;
            $('#TestificacionFolioFinal').val(folioFinal);
            $('#TestificacionFolioFinal').text(folioFinal);
        });

        $(document).on('change', '.cantidadInput', function (e) {
            if (this.id.includes('Recibida')) {
                var cantidadRecibida = getNumero($('#DobleCeroCantidadRecibida').val()) + getNumero($('#CeroCantidadRecibida').val()) + getNumero($('#UnoCantidadRecibida').val()) + getNumero($('#DosCantidadRecibida').val()) + getNumero($('#ConstanciaNoAprobadoCantidadRecibida').val()) + getNumero($('#ExentosCantidadRecibida').val()) + getNumero($('#TestificacionCantidadRecibida').val());
                $('#CantidadRecibida').val(cantidadRecibida);
                $('#CantidadRecibidaCol').text(cantidadRecibida);
            } else {
                var cantidad = getNumero($('#DobleCeroCantidad').val()) + getNumero($('#CeroCantidad').val()) + getNumero($('#UnoCantidad').val()) + getNumero($('#DosCantidad').val()) + getNumero($('#ConstanciaNoAprobadoCantidad').val()) + getNumero($('#ExentosCantidad').val()) + getNumero($('#TestificacionCantidad').val());
                $('#CantidadSolicitada').val(cantidad);
                $('#CantidadSolicitadaCol').text(cantidad);
            }
        });
    }
    var getNumero = function (val) {
        var res = parseInt(val);
        return isNaN(res) ? 0 : res;
    };
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
        }
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
    return {
        init: function () {
            init();
        },
        guardar: function () {
            Swal.fire({
                text: "Al guardar la información se reemplazará lo que hay en almacén por el nuevo ingreso ¿Desea continuar?",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Aceptar",
                cancelButtonText: 'Cancelar',
                showCancelButton: true,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: 'btn btn-danger'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    guardar();
                }
            })
        }
    }

}();

jQuery(document).ready(function () {
    moment.locale('es');
    GuardarFormulario.init();
});