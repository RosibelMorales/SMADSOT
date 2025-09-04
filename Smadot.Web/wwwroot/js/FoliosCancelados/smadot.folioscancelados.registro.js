"use strict";

var validation; // https://formvalidation.io/

var KTDatatableRemoteAjax = function () {
    var imagen;
    var init = function () {
        ConsultaFolioAutocomplete();

        $("#btnGuardarFolio").click(function (e) {
            e.preventDefault();
            //toastr.success('Registro guardado exitosamente', "SMADSOT");
            ConfirmarCancelarFolio();
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

        $('#FechaC').daterangepicker({
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

        $("#Motivo").on('change', function () {
            if (this.value == "3") {
                $("#InputOtro").show();
            } else {
                $("#InputOtro").hide();
            }
        });

        $('#IdCatTipoCertificado, #Motivo').select2();

        $('#FolioExists').on('change', function () {
            $('#ConsultaFolio').val('');
            $('#FolioNuevo').val('');
            $('#IdCatTipoCertificado').val('');
            if (this.checked) {
                $('#autocompleteDiv').show();
                $('#nuevoFolioDiv').hide();
            } else {
                $('#autocompleteDiv').hide();
                $('#nuevoFolioDiv').show();
            }
            $('#DetalleFolio').hide();
        });

        $('#FolioNuevo, #IdCatTipoCertificado').on('change', function () {
            $('#ConsultaFolio').val('');
            var folioNuevo = $('#FolioNuevo').val();
            var idCat = $('#IdCatTipoCertificado').val();
            if (folioNuevo !== undefined && folioNuevo !== '' && idCat !== undefined && idCat !== '' && idCat > 0) {
                $('#DetalleFolio').show();
                $('.detalleFolioRow').hide();
            } else {
                $('#DetalleFolio').hide();
                $('.detalleFolioRow').hide();
            }
        });
    };
    var ConsultaFolioAutocomplete = function () {
        $("#ConsultaFolio").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: siteLocation + "FoliosCancelados/ConsultaFolioAutocomplete",
                    type: "POST",
                    dataType: "json",
                    data: {
                        prefix: request.term
                    },
                    success: function (data) {
                        //response(data);
                        response($.map(data, function (item) {
                            return { label: item.nombre, value: item.id };
                        }));
                    }
                });
            },
            select: function (event, ui) {
                event.preventDefault();
                $("#ConsultaFolio").val(ui.item.label);

                $("#FolioId").val('');
                $("#Tipo").val('');
                $("#Fecha").val('');
                $("#Usuario").val('');
                $("#Vehiculo").val('');
                $("#Persona").val('');
                $("#FechaC").val('');
                $("#Motivo").val('');
                $("#Otro").val('');
                $("#IdF").val('');
                //limpiar busqueda

                $.ajax({
                    url: siteLocation + "FoliosCancelados/DetalleFolio",
                    type: "POST",
                    dataType: "json",
                    data: { id: ui.item.value },
                    success: function (data) {
                        if (data.error === false) {
                            $("#DetalleFolio").show();
                            $(".detalleFolioRow").show();
                            //establecerBusqueda(data.Data);
                            $("#FolioId").val(data.data.folio);
                            $("#Tipo").val(data.data.tipoTramite);
                            $("#Fecha").val(data.data.fecha);
                            $("#Usuario").val(data.data.datosUsuario);
                            $("#Vehiculo").val(data.data.datosVehiculo);
                            $("#Persona").val(data.data.personaRealizoTramite);
                            $("#FechaC").val(data.data.fechaCancelacion);
                            $("#Motivo").val(data.data.idCatMotivoCancelacion);
                            if (data.data.idCatMotivoCancelacion == "3") {
                                $("#InputOtro").show();
                            } else {
                                $("#InputOtro").hide();
                            }
                            //$("#Otro").val(data.data.otroMotivo);
                            $("#Otro").val('');
                            $("#IdF").val(data.data.id);
                        }
                    }
                });

            },
            focus: function (event, ui) {
                event.preventDefault();
                $("#ConsultaFolio").val(ui.item.label);
            },
            //search: function (event, ui) {
            //    event.preventDefault();
            //    //$("#cp").val("");
            //    //$("#Estado").val('');
            //    //$("#Pais").val('');
            //    //$("#Municipio").val('');
            //    //$("#IdCatColonia").find('option').remove();
            //}
        });
    }

    var ConfirmarCancelarFolio = function () {
        Swal.fire({
            html: '¿Está seguro que desea cancelar el folio <strong>' + ($('#FolioExists').is(':checked') ? $("#FolioId").val() : $('#FolioNuevo').val()) + '</strong>?',
            icon: "info",
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
                if ($("#Motivo").val() > 0 === false) {
                    toastr.error('Seleccione un motivo', 'SMADSOT');
                    return;
                }
                var formData = new FormData();
                formData.append('Folio', $("#FolioId").val());
                formData.append('Folio', $("#FolioId").val());
                formData.append('IdFolio', $("#IdF").val());
                formData.append('FechaCancelacion', $("#FechaC").val());
                formData.append('MotivoCancelacion', $("#Motivo").val());
                formData.append('OtroMotivo', $("#Otro").val());
                formData.append('FolioNuevo', $("#FolioNuevo").val());
                formData.append('IdCatTipoCertificado', $('#IdCatTipoCertificado').select2().val());
                formData.append('FolioExists', $('#FolioExists').is(":checked"));
                
                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "FoliosCancelados/CancelarFolio",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formData,
                    success: function (result) {
                        blockUI.release();
                        if (result.error) {
                            toastr.error(result.errorDescription, '');
                        } else {
                            toastr.success(result.errorDescription, '');
                            setTimeout(function () { location.href = siteLocation + "FoliosCancelados" }, 2000);
                            //location.href = siteLocation + "FoliosCancelados";
                        }
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error('Error', '');
                    }
                });
            }

        });
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