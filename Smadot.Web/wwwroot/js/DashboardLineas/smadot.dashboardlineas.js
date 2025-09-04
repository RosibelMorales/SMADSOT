/*-------Ver Camara-------*/
$(document).on('click', '.btnDetalle', function (e) {
    ModalDetalle.init();
});

var ModalDetalle = function () {
    var init = function () {
        abrirModal();
    }
    var abrirModal = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "DashboardLineas/CameraModal",
            success: function (result) {
                if (result.error) {
                    toastr.error(result.errorDescription, "SMADSOT");
                } else {
                    $('#modal_registro').removeClass('modal-md');
                    $('#modal_registro').removeClass('modal-xl');
                    $('#modal_registro').addClass('modal-lg');
                    $('#modal_registro #modalLabelTitle').html("Camara");
                    $('#modal_registro .modal-body').html("");
                    $('#modal_registro .modal-body').html(result.result);
                    $('#modal_registro').modal('show');
                    $('#btnGuardarRegistro').hide();
                }
                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
    }

    return {
        init: function () {
            init();
        }
    }
}();

/*-------ScreenShot-------*/
$(document).on('click', '#ScreenShot', function (e) {
    AccionScreenshot.init();
});

var AccionScreenshot = function () {
    var init = function () {
        screenShot();
    }
    var screenShot = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Camara/ScreenShot",
            success: function (result) {
                $('#sr').css('display', 'block')
                //if (result.error) {
                //    toastr.error(result.errorDescription, "SMADSOT");
                //} else {
                //    //Return some acción after ScreenShot
                //}
                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
    }

    return {
        init: function () {
            init();
        }
    }
}();

/*-------Zoom Up-------*/
$(document).on('click', '#ZoomUp', function (e) {
    AccionZoomUp.init();
});

var AccionZoomUp = function () {
    var init = function () {
        zoomUp();
    }
    var zoomUp = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Camara/ZoomUp",
            success: function (result) {
                $('#zu').css('display', 'block')
                //if (result.error) {
                //    toastr.error(result.errorDescription, "SMADSOT");
                //} else {
                //    //Return some acción after ZoomUp
                //}
                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
    }

    return {
        init: function () {
            init();
        }
    }
}();

/*-------Zoom Down-------*/
$(document).on('click', '#ZoomDown', function (e) {
    AccionZoomDown.init();
});

var AccionZoomDown = function () {
    var init = function () {
        zoomDown();
    }
    var zoomDown = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Camara/ZoomDown",
            success: function (result) {
                $('#zd').css('display', 'block')
                //if (result.error) {
                //    toastr.error(result.errorDescription, "SMADSOT");
                //} else {
                //    //Return some acción after ZoomDown
                //}
                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
    }

    return {
        init: function () {
            init();
        }
    }
}();

/*------- Cerrar/Abrir Linea -------*/
$(document).on('click', '.btnOpen', function (e) {
    //alerta('Abrir');
    let id = $(this).attr("data-id");
    AbrirLinea.init(id)
});

$(document).on('click', '.btnClose', function (e) {
    //alerta('Cerrar');
    let id = $(this).attr("data-id");
    CerrarLinea.init(id)
});

var AbrirLinea = function () {
    const init = (id) => {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        Swal.fire({
            title: '¿Está seguro que desea Abrir esta Linea?',
            text: "Esta a punto de Abrir la Linea",
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
                blockUI.block();

                var formData = new FormData();
                formData.append('Id', id);

                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "DashboardLineas/AbrirLinea",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formData,
                    success: function (result) {

                        blockUI.release();
                        if (!result.isSuccessFully) {
                            blockUI.release();
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        toastr.success("La linea ha sido abierta", "SMADSOT");
                        location.reload()
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error(res, "SMADSOT");
                        return;
                    }
                });
            }


        });
    }
    var listeners = function () {

    }
    return {
        init: (id) => {
            init(id);
        }

    }
}()

var CerrarLinea = function () {
    const init = (id) => {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        Swal.fire({
            title: '¿Está seguro que desea Cerrar esta Linea?',
            text: "Esta a punto de Cerrar la Linea",
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
                blockUI.block();

                var formData = new FormData();
                formData.append('Id', id);

                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "DashboardLineas/CerrarLinea",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formData,
                    success: function (result) {

                        blockUI.release();
                        if (!result.isSuccessFully) {
                            blockUI.release();
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        toastr.success("La linea ha sido cerrada", "SMADSOT");
                        location.reload()
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error(res, "SMADSOT");
                        return;
                    }
                });
            }


        });
    }
    var listeners = function () {

    }
    return {
        init: (id) => {
            init(id);
        }

    }
}()



//function alerta(e) {
//    confirm('Esta a punto de ' + e + ' la Linea');
//    // ------->> Logica para esta funcionalidad
//    // <--------
//}