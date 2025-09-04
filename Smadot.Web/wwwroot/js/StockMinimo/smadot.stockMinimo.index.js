
var KTDatatableRemoteAjax = function () {
    var imagen;
    var init = function () {
        $("#divForm").hide();

        $('#IdAlmacen').on('change', function () {
            if (this.value != '') {
                blockUI.block();
                $.ajax({
                    cache: false,
                    type: 'GET',
                    url: siteLocation + 'StockMinimo/Consulta/' + this.value,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error('Ocurrió un error al consultar.', "SMADSOT");
                            blockUI.release();
                            return;
                        } else {
                            $("#divForm").show();
                            $('#divForm .card-body').html('');
                            $('#divForm .card-body').html(result.result);
                            blockUI.release();
                        }
                        listeners();
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, "SMADSOT");
                        blockUI.release();
                        return;
                    }
                });
            } else {
                $("#divForm").hide();
            }
        });
    };
    var listeners = function () {
        $('#btnSave').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });
        $(".cantidadInput").on("change", function () {
            if (GuardarFormulario.intentoEnvio())
                GuardarFormulario.validar();
        })
    }
    return {
        init: function () {
            init();
        }
    };
}();



var GuardarFormulario = function () {
    let intentoEnvio = false;
    const ValidarFormulario = () => {
        let isValid = true;

        document.querySelectorAll('#tableRegistro > tbody tr').forEach((item, i) => {
            let element = item;

            item.querySelectorAll('input.cantidadInput').forEach((item, i) => {
                let valor = Number(item?.value);
                let elementParent = item.parentNode;
                let cantidadMax = element.querySelectorAll('input.cantidadInput')[1];
                DeleteMessage(elementParent);
                if (valor < 0) {
                    elementParent.append(Createmessage('No se permiten números negativos.'));
                    isValid = false;
                }

                if (valor === undefined || valor === null || valor === NaN || valor === 0) {

                    elementParent.append(Createmessage('Debe ingresar una cantidad.'));
                    isValid = false;
                }
                if (i === 0 && valor >= Number(cantidadMax?.value)) {
                    elementParent.append(Createmessage('La cantidad mínima no puede ser mayor a la media.'));
                    isValid = false;
                }

            })
        })
        return isValid;
    }

    var guardar = function () {
        var form = $('#stockMinimoRegistro')[0];
        var formData = new FormData(form);
        intentoEnvio = true;
        if (false) { // TODO: Reemplazar por validació de formulario
            toastr.error("Hay errores en el formulario", "SMADSOT");
            return;
        }


        $.ajax({
            cache: false,
            type: 'POST',
            processData: false,
            contentType: false,
            enctype: 'multipart/form-data',
            url: siteLocation + "StockMinimo/Registro",
            data: formData,
            success: function (result) {
                if (result.isSuccessFully) {
                    toastr.success(result.message, "SMADSOT");
                } else {
                    toastr.error(result.message, "SMADSOT");
                }
                return;
            },
            error: function (res) {
                toastr.error("Ocurrió un error al registrar la venta", "");
                blockUI.release();
            }
        });
    };

    const Createmessage = (message, isTableMessage, dataField) => {
        var elementMessage = document.createElement('div');
        elementMessage.classList.add('fv-plugins-message-container');
        if (isTableMessage)
            elementMessage.classList.add('tableMessage');
        elementMessage.innerHTML = `<div data-field="${dataField}" data-validator="notEmpty" class="fv-help-block">${message}</div>`;
        return elementMessage;
    }
    const DeleteMessage = function (element) {
        element.querySelectorAll(".fv-plugins-message-container").forEach(e => e.remove());
    }

    return {

        guardar: function () {
            guardar();
        },
        validar: function () {
            ValidarFormulario();
        },
        intentoEnvio: () => { return intentoEnvio }
    }

}();


jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});





