"use strict";


var DetalleProgramacionCalibracion = function () {
    var validationProgramacion; // https://formvalidationProgramacion.io/

    var init = function () {
        init_validacion();

        $(document).on('click', '#btnRechazar', function (e) {
            rechazar();
        });

        $(document).on('click', '.descargarDoc', function (e) {
            descarga(($(this).data('url')));
        });

        $(document).on('click', '#btnValidar', function (e) {
            validar();
        });
    };

    var descarga = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: siteLocation + 'ProgramacionCalibracion/DescargarDocumento?url=' + url,
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
    }
    var validar = function () {
        var Data = {
            Id: $("#IdProgramacionCalibracion").val()
        }

        $.ajax({
            cache: false,
            type: 'POST',
            contentType: 'application/json;charset=UTF-8',
            url: siteLocation + "ProgramacionCalibracion/Detalle",
            dataType: 'json',
            data: JSON.stringify(Data),
            async: true,
            success: function (result) {
                if (result.error) {
                    toastr.error("Error al guardar la información", "Error");
                    blockUI.release();
                } else {
                    if (!$("#Id").length > 0) {
                        $("#save").hide();
                    }
                    toastr.success("Datos actualizados con exito", "");
                    $('#modalNotificacion2').modal('hide');
                    ModalDetalle.cerrarventanamodal();
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
    }
    var rechazar = function () {

        var form = $('#formDetalleProgramacionCalibracion');
        //validationProgramacion.validate().then(async function (status) {
        if (form.valid()) {
            //var formData = new FormData(form);

            var Data = {
                Id: $("#IdProgramacionCalibracion").val(),
                Nota: $("#DetalleNota").val()
            }

            $.ajax({
                cache: false,
                type: 'POST',
                contentType: 'application/json;charset=UTF-8',
                url: siteLocation + "ProgramacionCalibracion/DetalleRechazar",
                dataType: 'json',
                data: JSON.stringify(Data),
                async: true,
                success: function (result) {
                    if (result.error) {
                        toastr.error("Error al guardar la información", "Error");
                        blockUI.release();
                    } else {
                        if (!$("#Id").length > 0) {
                            $("#save").hide();
                        }
                        toastr.success("Datos actualizados con exito", "");
                        $('#modalNotificacion2').modal('hide');
                        ModalDetalle.cerrarventanamodal();                       
                    }
                    return;
                },
                error: function (res) {
                    $(window).scrollTop(0);
                    toastr.error("Error al guardar la información", "Error");
                    blockUI.release();
                }
            });
        }
        //});
    }

    //var rechazar = function () {
    //    var Data = {
    //        Id: $("#IdProgramacionCalibracion").val(),
    //        Nota: $("#DetalleNota").val()
    //    }

    //    console.log(Data);
    //    $.ajax({
    //        cache: false,
    //        type: 'POST',
    //        contentType: 'application/json;charset=UTF-8',
    //        url: siteLocation + "ProgramacionCalibracion/DetalleRechazar",
    //        dataType: 'json',
    //        data: JSON.stringify(Data),
    //        async: true,
    //        success: function (result) {
    //            if (result.error) {
    //                toastr.error("Error al guardar la información", "Error");
    //                blockUI.release();
    //            } else {
    //                if (!$("#Id").length > 0) {
    //                    $("#save").hide();
    //                }
    //                toastr.success("Datos actualizados con exito", "");
    //                KTDatatableRemoteAjax.recargar();
    //            }
    //            return;
    //        },
    //        error: function (res) {
    //            $(window).scrollTop(0);
    //            toastr.error("Error al guardar la información", "Error");
    //            blockUI.release();
    //        }
    //    });
    //}

    const init_validacion = function () {
        //$("#formDetalleProgramacionCalibracion").each(function () {
        //    $(this).rules('add', {
        //        required: true
        //    });
        //});
        $("#formDetalleProgramacionCalibracion").validate({
            errorClass: 'is-invalid text-danger',
            errorElement: 'span',
            // Specify validation rules
            rules: {
                // The key name on the left side is the name attribute
                // of an input field. Validation rules are defined
                // on the right side
                DetalleNota: "required",
            },
            // Specify validation error messages
            messages: {
                DetalleNota: "Debe ingresar una nota.",
               
               
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                form.submit();
            }
        });
    }
    return {
        init: function () {
            init();
        }
    }
}();

