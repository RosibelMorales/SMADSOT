jQuery(document).ready(function () {
    
        $('.btnModificarEstatus').off().on('click', function (e) {
            var idUserPuestoVerificentro = $('#idUserPuestoVerificentro').val();
            var idCatEstatusPuesto = $(this).data('estatus');
            modificarEstatus(idUserPuestoVerificentro, idCatEstatusPuesto);
        });

    var modificarEstatus = function (idUserPuestoVerificentro, idCatEstatusPuesto) {
        var url = siteLocation + `Personal/ModificarEstatusPuesto?idUserPuestoVerificentro=${idUserPuestoVerificentro}&idCatEstatusPuesto=${idCatEstatusPuesto}`
        $.ajax({
            cache: false,
            type: 'PUT',
            processData: false,
            contentType: false,
            enctype: 'multipart/form-data',
            url: url,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                toastr.success(result.message, 'SMADSOT');
                window.location.href = siteLocation + 'Personal';

                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    }
});