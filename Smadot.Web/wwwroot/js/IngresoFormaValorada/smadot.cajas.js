"use strict"

$(document).on('click', '#btnSave', function (e) {
    GuardarFormulario.guardar();
});

var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(function (status) {
            if (status === 'Valid') {
                var form = $('#form')[0];
                var formData = new FormData(form);
                formData.set('[0].IdIngresoFV', formData.get('IdIngresoFV'));
                formData.set('[0].NumeroCaja', formData.get('DobleCeroCaja'));
                formData.set('[0].FolioInicial', formData.get('DobleCeroFolioInicial'));
                formData.set('[0].FolioFinal', formData.get('DobleCeroFolioFinal'));
                formData.set('[1].NumeroCaja', formData.get('CeroCaja'));
                formData.set('[1].FolioInicial', formData.get('CeroFolioInicial'));
                formData.set('[1].FolioFinal', formData.get('CeroFolioFinal'));
                formData.set('[2].NumeroCaja', formData.get('UnoCaja'));
                formData.set('[2].FolioInicial', formData.get('UnoFolioInicial'));
                formData.set('[2].FolioFinal', formData.get('UnoFolioFinal'));
                formData.set('[3].NumeroCaja', formData.get('DosCaja'));
                formData.set('[3].FolioInicial', formData.get('DosFolioInicial'));
                formData.set('[3].FolioFinal', formData.get('DosFolioFinal'));
                formData.set('[4].NumeroCaja', formData.get('ConstanciaNoAprobadoCaja'));
                formData.set('[4].FolioInicial', formData.get('ConstanciaNoAprobadoFolioInicial'));
                formData.set('[4].FolioFinal', formData.get('ConstanciaNoAprobadoFolioFinal'));
                formData.set('[5].NumeroCaja', formData.get('ExentosCaja'));
                formData.set('[5].FolioInicial', formData.get('ExentosFolioInicial'));
                formData.set('[5].FolioFinal', formData.get('ExentosFolioFinal'));
                formData.set('[6].NumeroCaja', formData.get('TestificacionCaja'));
                formData.set('[6].FolioInicial', formData.get('TestificacionFolioInicial'));
                formData.set('[6].FolioFinal', formData.get('TestificacionFolioFinal'));
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/IngresoFormaValorada/Cajas',
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
                    DobleCeroCaja: {
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
                    CeroCaja: {
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
                    UnoCaja: {
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
                    DosCaja: {
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
                    ConstanciaNoAprobadoCaja: {
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
                    ExentosCaja: {
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
                    TestificacionCaja: {
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
                    TestificacionFolioInicial: {
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
                    TestificacionFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap({
                        rowSelector: '.fv-row',
                    })
                }
            }
        );
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

jQuery(document).ready(function () {
    moment.locale('es');
    GuardarFormulario.init();
});