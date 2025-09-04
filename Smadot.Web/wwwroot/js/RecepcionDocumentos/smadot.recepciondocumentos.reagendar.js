"use strict"
var validator; // https://formvalidation.io/
var validations = [];
var form;
var horariosDisponibles;

var ReagendarCitaAjax = function () {
    var init = function () {
        initValidation()

        $("#IdCentroAtencion").select2();

        $(".step2").hide()

        $(document).on('change', '#IdCentroAtencion', function (e) {
            if (this.value !== "") {
                $(".step2").show()
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

                        //$("#HoraCentro").on('change', function (e) {
                        //    $("#HoraSeleccionada").html('')
                        //    $("#HoraSeleccionada").html($("option:selected", this).text())
                        //});

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
                //$("#CentroSeleccionado").html('')
                $(".step2").hide()
            }
        })

        $(document).on('click', "#btnGuardarRegistro", function (e) {
            //alert("SE guardara")
            //if (!validator) {
            //    validation();
            //}

            var validator = validations[0];

            //if (validator) {
                validator.validate().then(async function (status) {
                    if (status == 'Valid') {
                        blockUI.block();
                        var Data = {
                            FechaString: $("#datetimepicker1Input").val(),
                            HoraString: $("#HoraCentro").val(),
                            IdCVV: $("#IdCentroAtencion").val(),
                            IdCita: $("#IdCita").val(),
                            Folio: $("#Folio").val()
                        }
                        $.ajax({
                            cache: false,
                            type: 'POST',
                            contentType: 'application/json;charset=UTF-8',
                            url: siteLocation + 'RecepcionDocumentos/ReagendarCita',
                            dataType: 'json',
                            data: JSON.stringify(Data),
                            success: function (result) {
                                blockUI.release();
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
                                    });
                                } else {
                                    Swal.fire({
                                        title: 'Registro Exitoso',
                                        text: "Su cita fue registrada de manera correcta",
                                        icon: 'success',

                                        confirmButtonColor: '#3085d6',

                                        confirmButtonText: 'Aceptar',
                                        allowOutsideClick: false
                                    }).then((resultSelect) => {
                                        if (resultSelect.isConfirmed) {
                                            /*window.location.href = siteLocation + 'PortalCitas/ConsultaCita?folio=' + response;*/
                                            window.location.href = siteLocation + 'RecepcionDocumentos';
                                        }
                                    })
                                }
                                return;
                            },
                            error: function (res) {
                                blockUI.release();
                                toastr.error("Ocurrió un error al registrar la información", "");

                            }
                        });
                    }
                });
            //} else {
            //    toastr.error("Se encontraron algunos errores, revise su información e intente de nuevo...", "");
            //}

        })
        $("#btnGuardarRegistro").off()
        
    }
    return {
        init: function () {
            init();
        }
    };
}();

//var validation = function () {
//    const form = document.getElementById('form_reagendar');
//    validator = FormValidation.formValidation(
//        form,
//        {
//            fields: {
//                IdCentroAtencion: {
//                    validators: {
//                        notEmpty: {
//                            message: 'Debe seleccionar un centro de atención'
//                        }
//                    }
//                },
//                datetimepicker1Input: {
//                    validators: {
//                        notEmpty: {
//                            message: 'Debe seleccionar una fecha'
//                        }
//                    }
//                },
//                HoraCentro: {
//                    validators: {
//                        notEmpty: {
//                            message: 'Debe seleccionar una hora'
//                        }
//                    }
//                }

//            },
//            plugins: {
//                trigger: new FormValidation.plugins.Trigger(),
//                bootstrap: new FormValidation.plugins.Bootstrap({
//                    rowSelector: '.fv-row',
//                })
//            }
//        }
//    );
//}

var initValidation = function () {
    validations = [];
    // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
    // Step 1
    form = document.querySelector('#form_reagendar');

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
                },
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
}


//jQuery(document).ready(function () {
//    ReagendarCitaAjax.init();
//});