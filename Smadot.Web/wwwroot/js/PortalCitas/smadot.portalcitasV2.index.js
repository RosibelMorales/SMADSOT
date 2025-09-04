"use strict"
var myDropzone, myDropzone2, myDropzone3, myDropzone4, myDropzone5;
var validator; // https://formvalidation.io/
var stepper;
var stepperObj;
var validations = [];
var form;
var formSubmitButton;
var marcaStr;
var modeloStr;
var horariosDisponibles;
var KTDatatableRemoteAjax = function () {
    var init = function () {

        $(document).on('change', '#CheckAceptoDocumentacion', function (e) {
            if (this.checked) {
                GeneralStep.init()
            } else {
                $("#NavStepDiv").html('')
            }
        })

        $(document).on('click', '#btnBuscarCita', function (e) {
            var search = $("#txt_Search").val();
            if (search === "") {
                toastr.error("Debe ingresar un folio para iniciar la busqueda.", "SMADSOT");
                return;
            }
            ConsultarCita.init(search)
        });
        $('#btnBuscarCita').off()




    }
    return {
        init: function () {
            init();
        }
    };
}();

var GeneralStep = function () {
    var fv;
    var init = function () {
        showView();
    }
    function regenerateRecaptchaToken() {
        grecaptcha.enterprise.ready(function () {
            grecaptcha.enterprise
                .execute(googleKey, { action: 'CitaVerificacion' })
                .then(function (token) {
                    $('#recaptchaToken').val(token);
                });
        });
    }
    var showView = function () {
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'PortalCitas/GeneralStep',
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#NavStepDiv').html('');
                $('#NavStepDiv').html(result.result);
                listeners();
                init_validaciones_jvalidation();
                if (window.reCaptchaInterval) {
                    clearInterval(window.reCaptchaInterval);
                }

                // Ejecutar la función para regenerar el token cada 1:55 minutos
                window.reCaptchaInterval = setInterval(regenerateRecaptchaToken, 115000); // 115000 milisegundos = 1:55 minutos

                // Generar el token de reCaptcha por primera vez
                regenerateRecaptchaToken();

                $('#NavStepDiv').show();

                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error("No es posible abrir la pantalla.", "SMADSOT");
                return;
            }
        });
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {
        stepper = document.querySelector("#kt_create_account_stepper");
        formSubmitButton = stepper.querySelector('[data-kt-stepper-action="submit"]');

        initStepper();
        validations = [];
        initValidation();
        handleForm();
        GetMarcas();
        $("#IdTipoCombustible").select2();
        $("#Estado").select2();
        //$(document).ajaxComplete(function () {
        $("#IdCentroAtencion").off().select2();
        //});

        $('#btnCentrosAtencion').off().on("click", function (e) {
            e.preventDefault()
            CentrosAtencionModal.init()
        })
        // $('#btnCentrosAtencion').off()

        $(document).on('change', '#IdCentroAtencion', function (e) {
            if (this.value !== "") {
                $("#CentroSeleccionado").html('')
                $("#FechaSeleccionada").html('')
                $("#HoraSeleccionada").html('')
                $("#CentroSeleccionado").html($("option:selected", this).text())

                $.ajax({
                    cache: false,
                    type: 'GET',
                    url: siteLocation + 'PortalCitas/GetCalendarDates?id=' + this.value,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        $("#DivStep2").html('');

                        $("#DivStep2").html(result.result.viewPartial);
                        validations = [];
                        initValidation()

                        $("#HoraCentro").select2();
                        if ($('#datetimepicker1').data('DateTimePicker')) {
                            // El datepicker existe en $('#datetimepicker1')
                            $('#datetimepicker1').tempusDominus('destroy');
                        }
                        $('#datetimepicker1').tempusDominus({
                            restrictions: {
                                minDate: min,
                                maxDate: max,
                                daysOfWeekDisabled: result.result.disableDays,
                                disabledDates: result.result.disableDates
                            },
                            display: {
                                components: {
                                    calendar: true,
                                    date: true,
                                    month: true,
                                    year: true,
                                    decades: false,
                                    clock: false,
                                    hours: false,
                                    minutes: false,
                                    seconds: false,
                                    useTwentyfourHour: undefined,
                                },
                                inline: false,
                                theme: 'light',
                            },
                            localization: {
                                //format: 'dd/MM/yyyy'
                                today: 'Hoy',
                                clear: 'Borrar selección',
                                close: 'Cerrar selector',
                                selectMonth: 'Seleccionar mes',
                                previousMonth: 'Mes anterior',
                                nextMonth: 'Próximo mes',
                                selectYear: 'Seleccionar año',
                                previousYear: 'Año anterior',
                                nextYear: 'Próximo año',
                                selectDecade: 'Seleccionar década',
                                previousDecade: 'Década anterior',
                                nextDecade: 'Próxima década',
                                previousCentury: 'Siglo anterior',
                                nextCentury: 'Próximo siglo',
                                pickHour: 'Elegir hora',
                                incrementHour: 'Incrementar hora',
                                decrementHour: 'Decrementar hora',
                                pickMinute: 'Elegir minuto',
                                incrementMinute: 'Incrementar minuto',
                                decrementMinute: 'Decrementar minuto',
                                pickSecond: 'Elegir segundo',
                                incrementSecond: 'Incrementar segundo',
                                decrementSecond: 'Decrementar segundo',
                                toggleMeridiem: 'Cambiar AM/PM',
                                selectTime: 'Seleccionar tiempo',
                                selectDate: 'Seleccionar fecha',
                                dayViewHeaderFormat: { month: 'long', year: '2-digit' },
                                locale: 'es-MX',
                                startOfTheWeek: 1,
                                dateFormats: {
                                    LT: 'H:mm',
                                    LTS: 'H:mm:ss',
                                    L: 'dd/MM/yyyy',
                                    LL: 'd [de] MMMM [de] yyyy',
                                    LLL: 'd [de] MMMM [de] yyyy H:mm',
                                    LLLL: 'dddd, d [de] MMMM [de] yyyy H:mm',
                                },
                                ordinal: (n) => `${n}º`,
                                format: 'L LT',
                            },
                            useCurrent: false,
                            /*                            locale: 'es'*/
                        });

                        $("#datetimepicker1Input").click(function (e) {
                            $("#spanIconCalendar").click()
                        });
                        let horasResponses = result.result.horasResponses;
                        $("#datetimepicker1Input").on('change', function (e) {
                            var dt = this.value;
                            $('#FechaSeleccionada').text(this.value);
                            $('#HoraCentro').html('');
                            var newOption = new Option('Seleccione...', '', false, false);
                            $('#HoraCentro').append(newOption).trigger('change');
                            if (this.value !== "") {
                                const primerRegistro = horasResponses.find((registro) => registro['fecha'] === this.value);

                                // Verificar si se encontró el registro y mostrarlo
                                if (primerRegistro) {
                                    primerRegistro.horasDisponible.forEach((registro) => {
                                        var newOption = new Option(registro, registro, false, false);
                                        $('#HoraCentro').append(newOption).trigger('change');

                                    });
                                }

                            } else {
                                $("#HoraCentro").html('')
                            }

                        })

                        $("#HoraCentro").on('change', function (e) {
                            $("#HoraSeleccionada").html('')
                            $("#HoraSeleccionada").html($("option:selected", this).text())
                        });

                        blockUI.release();
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error("Ocurrio un error.", "SMADSOT");
                        return;
                    }
                });



            } else {
                $("#CentroSeleccionado").html('')
            }
        })
        $(document).on('change', '#EsPersonaMoral', function (e) {
            if (this.checked) {
                $(".labelPropietario").text('Razón Social');

            } else {
                $(".labelPropietario").text('Nombre del Propietario como aparece en la tarjeta de circulación');
            }
        })
        $('#Poblano').off().on('change', function (e) {
            ResetInfo();
            if (this.checked) {
                $(".divEstado").show();
                $(".divModeloRegistro").hide();
            } else {
                $(".divModeloRegistro").show();
                $(".divEstado").hide();
            }
        })

        $('#Placa').off().on('change', function (e) {
            //if (this.value !== "" && $("#NumSerie").val() !== "" && !$("#Poblano")?.is(":checked")) {
            //    var placa = this.value;
            //    var numserie = $("#NumSerie").val();
            //    marcaStr = null;
            //    modeloStr = null;
            //    Consulta(placa, numserie);
            //}
        });

        //$('#NumSerie').off().on('change', function (e) {
        //    if (this.value !== "" && !$("#Poblano")?.is(":checked")) {
        //        var numserie = this.value;
        //        Consulta(numserie);
        //    }
        //});

    }
    //// Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
    }

    return {
        init: function () {
            init();
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

var initStepper = function () {

    stepperObj = new KTStepper(stepper);
    stepperObj.on('kt.stepper.changed', function (stepper) {

    });

    // Validation before going to next page
    stepperObj.on('kt.stepper.next', function (stepper) {

        if (validator) {
            validator.destroy();
        }
        if (validations[2]) {
            validations[2].destroy();
        }
        if (stepper?.getCurrentStepIndex() != 3) {
            validator = validations[stepper.getCurrentStepIndex() - 1]; // get validator for currnt step
        }
        if (validator) {
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    stepper.goNext();

                } else {
                    Swal.fire({
                        text: "Se encontraron algunos errores, revise su información e intente de nuevo.",
                        icon: "error",
                        buttonsStyling: false,
                        confirmButtonText: "Aceptar",
                        customClass: {
                            confirmButton: "btn btn-light"
                        }
                    }).then(function () {
                    });
                }
            });
        } else {
            Swal.fire({
                text: "Se encontraron algunos errores, revise su información e intente de nuevo...",
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Aceptar",
                customClass: {
                    confirmButton: "btn btn-light"
                }
            }).then(function () {
            });
        }
    });

    // Prev event
    stepperObj.on('kt.stepper.previous', function (stepper) {

        stepper.goPrevious();
    });
}

var initValidation = function () {
    if (validator) {
        validator.destroy();
    }
    if (validations[2]) {
        validations[2].destroy();
    }
    validations = [];
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    // Step 1
    form = document.querySelector('#kt_create_account_form');

    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                IdCentroAtencion: {
                    validators: {
                        notEmpty: {
                            message: 'Debe seleccionar un centro de atención'
                        }
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
    ));
    // Step 2
    validations.push(FormValidation.formValidation(
        form,
        {
            fields: {
                datetimepicker1Input: {
                    validators: {
                        notEmpty: {
                            message: 'Debe seleccionar una fecha'
                        }
                    }
                },
                HoraCentro: {
                    validators: {
                        notEmpty: {
                            message: 'Debe seleccionar una hora'
                        }
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
    ));
    // Step 3
    // validations.push(FormValidation.formValidation(
    //     form,
    //     {
    //         fields: {
    //             Placa: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     }
    //                 }
    //             },
    //             NumSerie: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     }
    //                 }
    //             },
    //             Nombre: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     }
    //                 }
    //             },
    //             Correo: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     },
    //                     emailAddress: {
    //                         message: 'El correo ingresado no es válido',
    //                     },
    //                 }
    //             }, IdTipoCombustible: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Debe seleccionar un tipo de combustible.'
    //                     }
    //                 }
    //             }, ColorVehiculo: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     }
    //                 }
    //             }, Modelo: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     }
    //                 }
    //             }, Estado: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     }
    //                 }
    //             }, Marca: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     }
    //                 }
    //             }, Year: {
    //                 validators: {
    //                     notEmpty: {
    //                         message: 'Este campo es requerido'
    //                     }
    //                 }
    //             }

    //         },
    //         plugins: {
    //             trigger: new FormValidation.plugins.Trigger(),
    //             bootstrap: new FormValidation.plugins.Bootstrap({
    //                 rowSelector: '.fv-row',
    //             })
    //         }
    //     }
    // ));
}
// var GetSubmarcas = function () {
//     if ($('#Modelo').hasClass("select2-hidden-accessible")) {
//         $('#Modelo').select2('destroy');
//     }
//     $("#Modelo").select2({
//         tags: true,
//         ajax: {
//             url: `/PortalCitas/GetSubMarcas?marca=${$("#Marca").val() ?? ""}`,
//             dataType: "json",
//             delay: 2500,
//             data: function (params) {
//                 return {
//                     q: params.term,
//                     page: params.page,
//                     records: 10,
//                 }
//             },
//             processResults: function (data, params) {
//                 params.page = params.page || 1
//                 if (!data.isSuccessFully) {
//                     Swal.fire({
//                         title: "Ocurrió un problema",
//                         text: result.message,
//                         icon: "error",
//                         buttonsStyling: false,
//                         confirmButtonText: "Aceptar",
//                         customClass: {
//                             confirmButton: "btn btn-light"
//                         }
//                     }).then(function () {
//                     });

//                 }
//                 if (data.result?.data?.length < 1) {
//                     Swal.fire({
//                         title: 'Falta Información del Vehículo',
//                         text: "No se encontró la submarca de su vehículo. ¿Desea continuar con el registro para notificar la falta de información a Secretaría de Medio Ambiente, Desarrollo Sustentable y Ordenamiento Territorial?",
//                         icon: "warning",
//                         buttonsStyling: false,
//                         showCancelButton: true,
//                         reverseButtons: true,
//                         cancelButtonText: 'Cancelar',
//                         confirmButtonText: "Confirmar",
//                         focusConfirm: true,
//                         customClass: {
//                             confirmButton: "btn btn-primary",
//                             cancelButton: 'btn btn-secondary'
//                         }
//                     }).then((result) => {
//                         if (result.isConfirmed) {
//                             if (params.term) {
//                                 var newOption = new Option(params.term, -1, false, false);
//                                 $('#Modelo').append(newOption).trigger('change');
//                                 $('#Modelo').val(-1).trigger('change');
//                             }
//                         }
//                     });
//                 }
//                 return {
//                     results: data.result.data.map(function (item) {
//                         return {
//                             id: item.value,
//                             text: item.text
//                         };
//                     }),
//                     pagination: {
//                         more: params.page * 10 < data.result.recordsTotal,
//                     },
//                 }
//             },
//             cache: true,
//         },
//         placeholder: 'Ingresa el modelo del Vehículo',
//         minimumInputLength: 2,
//         language: 'es',
//     });
//     $('#Marca').on('select2:select', function (e) {
//         $(this).valid();
//     });
// }
var GetMarcas = function () {
    $("#Marca").off().select2(
        {

            ajax: {
                url: siteLocation + 'PortalCitas/GetMarca',
                dataType: "json",
                delay: 1000,
                data: function (params) {
                    return {
                        q: params.term,
                        page: params.page,
                        records: 10,
                    }
                },
                processResults: function (data, params) {
                    params.page = params.page || 1
                    if (!data.isSuccessFully) {
                        toastr.error(data.result.message, "SMADSOT");
                    }
                    return {
                        results: data.result.data.map(function (item, i) {
                            if (i === 0) {
                                return {
                                    id: item.value,
                                    text: item.text,
                                    selected: true
                                };
                            } else {

                                return {
                                    id: item.value,
                                    text: item.text
                                };
                            }

                        }),
                        pagination: {
                            more: params.page * 10 < data.result.recordsTotal,
                        },
                    }
                },
                cache: true,
                transport: function (params, success, failure) {
                    var $request = $.ajax(params);
                    $request.then(success);
                    $request.fail(failure => {
                        toastr.error('Ocurrió un error al obtener la información.', "SMADSOT");
                    });

                    // $('#Marca').val(101).trigger('change')
                    return $request;
                }
            },

            placeholder: 'Ingresa la Marca del Vehículo',
            minimumInputLength: 2,
            language: 'es',
        }
    );

    $('#Marca').on('select2:select', function (e) {
        $(this).valid();
        // GetSubmarcas();
    });

    // $('#auto').val(1).trigger('change.select2');
    // $('#Marca').val($('#Marca option:eq(1)').val()).trigger('select2.change');
    // $('#Marca').select2('close');
}
const init_validaciones_jvalidation = () => {
    $.validator.addMethod('regexp', function (value, element, param) {
        return this.optional(element) || value.match(param);
    },
        'This value doesn\'t match the acceptable pattern.');
    $("#kt_create_account_form").validate({
        errorClass: 'is-invalid text-danger',
        errorElement: 'span',
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            Placa: {
                required: true,
                minlength: 5,
                maxlength: 7,
                regexp: /^[a-zA-Z0-9]*$/i
            },
            NumSerie: {
                required: true,
                minlength: 7,
                maxlength: 17,
                regexp: /^[a-zA-Z0-9]*$/i,
                remote: {
                    url: '/PortalCitas/Consulta',
                    // URL de la API para la validación
                    type: 'GET',
                    // Método HTTP utilizado para la petición
                    data: {
                        // Datos enviados en la petición AJAX
                        numeroSerie: function () { return $('#NumSerie').val() },
                        placa: function () { return $('#Placa').val() },
                        poblano: function () { return !$("#Poblano")?.is(":checked") }
                    },
                    dataType: 'json',
                    contentType: 'application/json; charset=UTF-8',
                    dataFilter: function (response) {
                        let data = JSON.parse(response);
                        console.log(data);
                        if (!data?.isSuccessFully) {
                            Swal.fire({
                                text: data.message,
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: {
                                    confirmButton: "btn btn-light"
                                }
                            });
                        }
                        return data?.isSuccessFully === true ? 'true' : 'false'; //Email not existreturn 'true'; //Email already exist
                        
                    }
                }
            },
            Nombre: "required",
            Correo: {
                required: true,
                regexp: /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/
            },
            IdTipoCombustible: "required",
            ColorVehiculo: "required",
            //Modelo: "required",
            Estado: "required",
            Marca: "required",
            Year: {
                required: true,
                minlength: 4,
                maxlength: 17,
                max: Number(moment().year()) + 1
            },
            NombreGeneraCita: "required",
            Token: "required",
        },
        // Specify validation error messages
        messages: {
            DetalleNota: "El campo es requerido.",
            Placa: {
                required: "El campo es requerido.",
                maxlength: "Ingrese todos los caracteres de la placa.",
                minlength: "Ingrese todos los caracteres de la placa.",
                regexp: "Las placas no son válidas, contienen caracteres no válidos. Omitir los guiones en la placa."
            },
            NumSerie: {
                required: "El campo es requerido.",
                minlength: "Ingrese un número de serie válido.",
                maxlength: "Ingrese un número de serie válido.",
                regexp: "Ingrese un número de serie válido.",
                remote: "No fue posible validar el número de serie "
            },
            Nombre: "El campo es requerido.",
            Correo: {
                required: "El campo es requerido.",
                regexp: "Por favor ingresa un correo electrónico válido"
            },
            IdTipoCombustible: "El campo es requerido.",
            ColorVehiculo: "El campo es requerido.",
            //Modelo: "El campo es requerido.",
            Estado: "El campo es requerido.",
            Token: "El captcha es requerido.",
            Marca: "El campo es requerido.",
            Year: {
                required: "El campo es requerido.",
                minlength: "Ingrese el año del vehículo en formato YYYY.",
                maxlength: "Ingrese el año del vehículo en formato YYYY.",
                max: `El año no puede ser mayor a ${Number(moment().year()) + 1}`
            },
            NombreGeneraCita: "El campo es requerido.",

        },
        onkeyup: false,
        errorPlacement: function (error, element) {
            if (element.prop('type') == 'select-one') {
                error.appendTo(element.parent().parent());
            } else {
                error.insertAfter(element);
            }
            // }
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
        }
    });
}
var handleForm = function () {
    formSubmitButton.addEventListener('click', function (e) {

        var form = $('#kt_create_account_form');
        if (form.valid()) {
            e.preventDefault();
            formSubmitButton.disabled = true;

            formSubmitButton.setAttribute('data-kt-indicator', 'on');

            var Data = {
                Nombre: $("#Nombre").val(),
                Correo: $("#Correo").val(),
                FechaString: $("#datetimepicker1Input").val(),
                HoraString: $("#HoraCentro").val(),
                IdCVV: $("#IdCentroAtencion").val(),
                Placa: $("#Placa").val(),
                IdSubMarca: $("#Modelo").val() ?? "0",
                Anio: $("#Year").val(),
                Acepto: $("#CheckAceptoDocumentacion").is(":checked"),
                EsPersonaMoral: $("#EsPersonaMoral").is(":checked"),
                Poblano: $("#Poblano").is(":checked"),
                SubMarcaNueva: "",
                Marca: $('#Marca').val(),
                IdTipoCombustible: $("#IdTipoCombustible").val(),
                NombreGeneraCita: $("#NombreGeneraCita").val(),
                ClaveEstado: $("#Estado").val(),
                Serie: $("#NumSerie").val(),
                ColorVehiculo: $("#ColorVehiculo").val(),
                Token: $("#recaptchaToken").val(),
            }
            $.ajax({
                cache: false,
                type: 'POST',
                //contentType: 'application/json;charset=UTF-8',
                url: siteLocation + 'PortalCitas/Registro',
                //dataType: 'json',
                data: (Data),
                success: function (result) {
                    formSubmitButton.removeAttribute('data-kt-indicator');
                    formSubmitButton.disabled = false;
                    var response = result.result;
                    if (!result.isSuccessFully) {
                        Swal.fire({
                            text: result.message,
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn btn-light"
                            }
                        }).then(function () {
                            location.reload();
                        });
                    } else {
                        window.location.href = siteLocation + 'PortalCitas/AvisoCitaCreadaPorConfirmar?folio=' + response;
                    }
                    return;
                },
                error: function (res) {
                    formSubmitButton.removeAttribute('data-kt-indicator');
                    toastr.error("Ocurrió un error al registrar la información", "");

                }
            });

        } else {
            Swal.fire({
                text: "Se encontraron algunos errores, revise su información e intente de nuevo...",
                icon: "error",
                buttonsStyling: false,
                confirmButtonText: "Aceptar",
                customClass: {
                    confirmButton: "btn btn-light"
                }
            });
        }
    });

}

var Consulta = function (numserie) {
    $.ajax({
        cache: false,
        type: 'GET',
        url: siteLocation + 'PortalCitas/Consulta?numserie=' + numserie,
        success: function (result) {
            ResetInfo();
            if (!result.isSuccessFully) {
                // Swal.fire({
                //     text: result.message,
                //     icon: "error",
                //     buttonsStyling: false,
                //     confirmButtonText: "Aceptar",
                //     customClass: {
                //         confirmButton: "btn btn-light"
                //     }
                // }).then(function () {

                // });
                return;
            }
            if (result?.result?.marca?.text != null) {
                let marca = result?.result?.marca;
                var newOption = new Option(marca.text, marca.value, false, false);
                $('#Marca').prepend(newOption).trigger('change');
                $('#Marca').val($('#Marca option:eq(0)').val()).trigger('change');
                $('#Marca').trigger({
                    type: 'select2:select',
                    params: {
                        data: { id: marca.value, text: marca.text }
                    }
                });
            }
            if (result?.result?.submarca?.text != null) {
                let submarca = result?.result?.submarca;
                var newOption = new Option(submarca.text, submarca.value, false, false);
                $('#Modelo').prepend(newOption).trigger('change');
                $('#Modelo').val($('#Modelo option:eq(0)').val()).trigger('change');
            }
            $("#ModeloFinanzas").val(result.result.linea);
            $("#Year").val(result.result.anio);

            blockUI.release();
            return;
        },
        error: function (res) {
            blockUI.release();
            toastr.error("Ocurrio un error.", "SMADSOT");
            return;
        }
    });
}
function select2_search($el, term) {
    // $el.select2('open');

    // Get the search box within the dropdown or the selection
    // Dropdown = single, Selection = multiple
    var $search = $el.data('select2').dropdown.$search || $el.data('select2').selection.$search;
    // This is undocumented and may change in the future

    $search.val(term);
    $search.trigger('keyup');
}
var ResetInfo = function () {
    $("#Marca").val(null).trigger('change');
    $("#Modelo").val(null).trigger('change');
    $("#Year").val('');
};

var CentrosAtencionModal = function () {
    var fv;
    var init = function () {
        abrirModal();
    }

    var abrirModal = function () {
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'PortalCitas/CentrosAtencion',
            success: function (result) {
                if (!result.isSuccessFully) {
                    blockUI.release();
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Ubicación de centros de Atención');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $('#btnGuardarRegistro').hide()
                $('#modal_registro').modal('show');
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error("No es posible abrir la pantalla.", "SMADSOT");
                return;
            }
        });
        blockUI.release();
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {

    }
    //// Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
    }

    return {
        init: function () {
            init();
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

var ConsultarCita = function () {
    const init = (folio) => {
        abrirModal(folio);
    }
    var abrirModal = function (folio) {

        window.location.href = siteLocation + 'PortalCitas/ConsultaCita?folio=' + folio;
        // blockUI.block();

        // $.ajax({
        //     cache: false,
        //     type: 'GET',
        //     url: siteLocation + "PortalCitas/ConsultaCitaGet?folio=" + folio,
        //     success: function (result) {

        //         blockUI.release();
        //         if (!result.isSuccessFully) {
        //             blockUI.release();
        //             toastr.error(result.message, "SMADSOT");
        //             return;
        //         }

        //         if (result.result === "") {
        //             blockUI.release();
        //             toastr.error(result.message, "SMADSOT");
        //             return;
        //         }

        //         return;
        //     },
        //     error: function (res) {
        //         blockUI.release();
        //         toastr.error(res, "SMADSOT");
        //         return;
        //     }
        // });
    }

    var listeners = function () {

    }
    return {
        init: (folio) => {
            init(folio);
        }

    }
}();



jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});