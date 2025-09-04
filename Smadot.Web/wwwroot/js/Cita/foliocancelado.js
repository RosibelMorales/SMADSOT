var folioCancelado = function () {
    var validator;
    var folio;
    const init = (folio) => {
        folio = folio;
        if (folio.generado) {
            $.ajax({
                url: "/FoliosCancelados/DetalleFolioImpresion",
                type: "POST",
                dataType: "json",
                data: { id: folio.id },
                success: function (data) {
                    if (!data.error) {
                        $('#modal_registro #modalLabelTitle').html('Cancelar Folio');
                        $('#modal_registro .modal-body').html('');
                        $('#modal_registro .modal-body').html(data.html);
                        $('#modal_registro .modal-dialog').addClass('modal-xl');
                        listeners();
                        $('#modal_registro').modal('show');
                        init_validacion();

                    } else {
                        toastr.error("Ocurrió un error al obtener la información del folio.", "SMADSOT")
                    }
                }
            });
        } else {
            Swal.fire({
                html: '¿Está seguro que desea generar un folio para la verificación del auto con Placas: <b>' + folio.placa + '</b> y No. de Serie: <b>' + folio.serie + '</b>?',
                icon: "warning",
                buttonsStyling: false,
                showCancelButton: true,
                reverseButtons: true,
                cancelButtonText: 'Cancelar',
                confirmButtonText: "Confirmar",
                focusConfirm: true,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: 'btn btn-secondary'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: "/FoliosCancelados/ReimpresionFolio",
                        type: "POST",
                        dataType: "json",
                        data: { IdVerificacion: folio.idverificacion },
                        success: function (data) {

                            if (data.error === false) {
                                toastr.success(data.errorDescription, "SMADSOT")

                            } else {
                                toastr.error(data.errorDescription, "SMADSOT")
                            }
                            KTDatatableRemoteAjax.recargar();
                        }
                    });
                }
            })
        }


    }
    const listeners = () => {
        $("#btnGuardarRegistro").off().click(function (e) {
            e.preventDefault();
            validator.validate().then(function (status) {
                if (status === 'Valid') {
                    ConfirmarCancelarFolio();
                }
            });
        });

        $('#Fecha').daterangepicker({
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

        $('#FechaCancelacion').daterangepicker({
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
        $("#IdCatMotivoCancelacion").select2({
            dropdownParent: $('#modal_registro')
        });
        $("#IdCatMotivoCancelacion").on('change', function () {
            if (this.value == "3") {
                $("#InputOtro").show();
            } else {
                $("#InputOtro").hide();
            }
        });

    }
    const ConfirmarCancelarFolio = function () {
        Swal.fire({
            html: '¿Está seguro que desea cancelar el folio <strong>' + $('#Folio').val() + '</strong> y generar uno nuevo?',
            icon: "warning",
            buttonsStyling: false,
            showCancelButton: true,
            reverseButtons: true,
            cancelButtonText: 'Cancelar',
            confirmButtonText: "Confirmar",
            focusConfirm: true,
            customClass: {
                confirmButton: "btn btn-primary",
                cancelButton: 'btn btn-secondary'
            }
        }).then((result) => {

            if (result.isConfirmed) {
                var form = $('#form_registro')[0];
                var formData = new FormData(form);
                formData.set('IdFolio', $('#Id').val());
                // formData.set('FechaCancelacion',$('#Id').val());
                formData.set('MotivoCancelacion', $('#IdCatMotivoCancelacion').val());
                formData.set('OtroMotivo', $('#OtroMotivo').val());
                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: "/FoliosCancelados/ReimpresionFolio",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formData,
                    success: function (result) {
                        if (result.error) {
                            toastr.error(result.errorDescription, '');
                        } else {
                            toastr.success(result.errorDescription, '');
                            $('#modal_registro').modal('hide');

                            // setTimeout(function () { location.href = siteLocation + "FoliosCancelados" }, 2000);
                            //location.href = siteLocation + "FoliosCancelados";
                        }
                        KTDatatableRemoteAjax.recargar();
                        return;
                    },
                    error: function (res) {
                        toastr.error('Error', '');
                    }
                });
            }

        });
    }
    var init_validacion = function () {
        if (validator) validator.destroy()
        var form = document.getElementById('form_registro');
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    Id: {
                        validators: {
                            notEmpty: {
                                message: 'Este campo es obligatorio.'
                            }
                        }
                    },
                    FechaCancelacion: {
                        validators: {
                            notEmpty: {
                                message: 'Este campo es obligatorio.'
                            }
                        }
                    },
                    IdCatMotivoCancelacion: {
                        validators: {
                            notEmpty: {
                                message: 'Este campo es obligatorio.'
                            }
                        }
                    },
                    OtroMotivo: {
                        validators: {
                            notEmpty: {
                                message: 'Este campo es obligatorio.'
                            }
                        }
                    },
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    excluded: new FormValidation.plugins.Excluded(),
                    bootstrap: new FormValidation.plugins.Bootstrap()
                }
            }
        );
    }


    return {
        init: (folio) => {
            init(folio);
        }
    }
}()