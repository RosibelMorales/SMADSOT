$(document).on('click', '.descargarDoc', function (e) {
    DescargarDocumento.generar($(this).data('url'));
});

$(document).on('click', '#btnValidar', function (e) {
    Actualizar.guardar(false);
});

$(document).on('click', '#btnRechazar', function (e) {
    Actualizar.guardar(true);
});

$(document).on('click', '#btnDesactivar, .btnDesactivar', function (e) {
    var id = $(this).data('id');
    Swal.fire({
        title: '¿Deseas desactivar este registro?',
        text: "Si, para desactivar / No, para regresar",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#235b4e',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                cache: false,
                type: 'GET',
                url: siteLocation + "Equipo/Validar?id=" + id,
                success: function (result) {
                    if (result.error) {
                        toastr.error("", "Error");
                    } else {
                        if (!$("#Id").length > 0) {
                            $("#save").hide();
                        }
                        toastr.success("El registro se desactivo correctamente", "SMADSOT");
                        /*KTDatatableRemoteAjax.recargar();*/
                        location.href = "/Equipo"
                        return;
                    }

                    return;
                },
                error: function (res) {
                    toastr.error(res.errorDescripction);
                }
            });
        }
    })

});

var DescargarDocumento = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: siteLocation + 'Equipo/DescargarDocumento?url=' + url,
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

var Actualizar = function () {

    var guardar = function (rechazar) {
        var Data = {
            Id: $("#IdEquipo").val(),
            Comentarios: rechazar ? null : $("#Comentario").val()
        }

        $.ajax({
            cache: false,
            type: 'POST',
            contentType: 'application/json;charset=UTF-8',
            url: siteLocation + "Equipo/Detalle",
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
                    location.href = "/Equipo"
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
    };

    return {
        guardar: function (rechazar) {
            guardar(rechazar);
        }
    }

}();

//jQuery(document).ready(function () {
//    KTDatatableRemoteAjax.init();
//});
