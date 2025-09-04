

var ModalDetalleAlertaGeneral = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Alertas/Detalle?id=" + id,
            success: function (result) {
                if (result.error) {
                    toastr.error(result.errorDescription, "SMADSOT");
                } else {
                    //let titleAlert = $('#title-alert').text()
                    $('#modal_alerta #modalLabelTitle').html("Alerta - " + result.result.tipo);
                    $('#modal_alerta .modal-body').html("");
                    $('#modal_alerta .modal-body').html(result.result.html);
                    $('#modal_alerta .modal-footer #btnRechazarIngresoManual').hide();
                    $('#modal_alerta .modal-footer #btnAutorizarIngresoManual').hide();
                    $('#modal_alerta .modal-footer #btnAtenderAlerta').hide();
                    if (result.result.tableName != 'Verificacion') {
                        if (
                            result.result.tableName == 'Capacitacion' ||
                            result.result.tableName == 'Acreditacion' ||
                            result.result.tableName == 'Calibracion' ||
                            result.result.tableName == 'Equipo' ||
                            result.result.tableName == 'UserPuestoVerificentro' ||
                            result.result.tableName == 'EquipoTipoCalibracion'
                        ) {

                            $('#modal_alerta .modal-footer #btnAtenderAlerta').show();
                        } else {
                            $('#modal_alerta .modal-footer #btnAtenderAlerta').hide();

                        }

                        $('#modal_alerta .modal-footer #btnAtenderAlerta').data('tablename', result.result.tableName);
                        if (result.result.atendida) {
                            $('#modal_alerta .modal-footer #btnAtenderAlerta').hide();
                        }
                        $('#modal_alerta .modal-footer #btnAtenderAlerta').data('id', result.result.id);
                        $('#modal_alerta .modal-footer #btnRechazarIngresoManual').hide();
                        $('#modal_alerta .modal-footer #btnAutorizarIngresoManual').hide();
                        listeners();
                    } else {
                        $('#modal_alerta .modal-footer #btnAtenderAlerta').hide();
                        if (result?.result?.atendida == false) {
                            $('#modal_alerta .modal-footer #btnRechazarIngresoManual').show();
                            $('#modal_alerta .modal-footer #btnAutorizarIngresoManual').show();
                            $('#modal_alerta .modal-footer #btnRechazarIngresoManual').data('id', result.result.id);
                            $('#modal_alerta .modal-footer #btnRechazarIngresoManual').data('tablename', result.result.tableName);
                            $('#modal_alerta .modal-footer #btnAutorizarIngresoManual').data('id', result.result.id);
                            $('#modal_alerta .modal-footer #btnAutorizarIngresoManual').data('tablename', result.result.tableName);
                            listenersVerificacion();
                        }
                    }
                    $('#modal_alerta').modal('show');
                    Alertas.init();
                    if (location.toString().includes('Alertas')) {
                        KTDatatableRemoteAjax.recargar();
                    }
                }
                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
    }
    const listeners = () => {
        $('#btnAtenderAlerta').off().on('click', function (e) {
            e.preventDefault();
            let tableName = $(this).data('tablename');
            let id = $(this).data('id');
            var url = siteLocation + `Alertas/Redireccionar?tableName=${tableName}&id=${id}`;
            location.href = url;
        });
    };
    const listenersVerificacion = () => {
        $('#btnRechazarIngresoManual').off().on('click', function (e) {
            e.preventDefault();
            let id = $(this).data('id');
            AutorizacionVerificacion.autorizar(id, false)

        });
        $('#btnAutorizarIngresoManual').off().on('click', function (e) {
            e.preventDefault();
            let id = $(this).data('id');
            AutorizacionVerificacion.autorizar(id, true)
        });
    };
    return {
        init: function (id) {
            init(id);
        }
    }
}();

const AutorizacionVerificacion = function () {

    return {
        autorizar: (id, autorizacion) => {
            $.ajax({
                cache: false,
                type: 'POST',
                url: siteLocation + 'Alertas/AutorizarIngresoManual',
                data: {
                    id: id,
                    estatus: autorizacion
                },
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, "SMADSOT");
                        return;
                    }
                    $('#modal_alerta').modal('hide');
                    if (location.toString().includes('Alertas')) {
                        KTDatatableRemoteAjax.recargar();
                    }
                    //toastr.success(result.message, "SMADSOT");
                    return;
                },
                error: function (res) {
                    toastr.error(res, "SMADSOT");
                    return;
                }
            });
        }
    }
}();
const Alertas = function () {
    const init = function () {
        MenuNotificaciones();
    }
    const pushNotification = function (data) {
        var buttonLength = $('#kt_topbar_notifications_1 .divMenuNotificaciones button')?.length;
        if (buttonLength > 0)
            $('#kt_topbar_notifications_1 .divMenuNotificaciones button').remove();
        movimientoLen = data.movimiento?.length;
        let minlength = 0;
        if (movimientoLen > 0)
            minlength = Math.min(movimientoLen, 250);
        const container = $('#kt_topbar_notifications_1 .divMenuNotificaciones');

        // Verificar la cantidad actual de rows
        const rowLength = container.find('.row').length;

        // Eliminar el último row si ya hay más de 9
        if (rowLength >= 10) {
            container.find('.row:last').remove();
        }
        let notificationEl = ` <div class="row mb-5">
                                    <div class="d-flex flex-stack py-4">
                                        <div class="d-flex align-items-center">
                                            <div class="symbol symbol-35px me-4">
                                                <span class="symbol-label bg-light-danger">
                                                    <span class="svg-icon svg-icon-2 svg-icon-danger">
                                                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                                            <rect opacity="0.3" x="2" y="2" width="20" height="20" rx="10" fill="currentColor"></rect>
                                                            <rect x="11" y="14" width="7" height="2" rx="1" transform="rotate(-90 11 14)" fill="currentColor"></rect>
                                                            <rect x="11" y="17" width="2" height="2" rx="1" transform="rotate(-90 11 17)" fill="currentColor"></rect>
                                                        </svg>
                                                    </span>
                                                </span>
                                            </div>
                                            <div class="mb-0 me-2">
                                                <div class="text-gray-500 fs-7"><a href="javascript:void(0)" class="alertDetalle" data="${data.id}">${data.movimiento.substr(0, minlength)}</a></div>
                                                <div class="text-gray-400 fs-8">${data.fecha}</div>
                                            </div>
                                        </div>
                                    </div>
                                </div>`;
        container.prepend(notificationEl);
    }

    const MenuNotificaciones = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'Alertas/MenuNotificaciones',
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#kt_topbar_notifications_1 .divMenuNotificaciones').html('');
                $('#kt_topbar_notifications_1 .divMenuNotificaciones').html(result.result);
                listeners();
                var notificaciones = $('#kt_topbar_notifications_1 .divMenuNotificaciones > .row > .d-flex')?.length ?? 0;
                if (notificaciones > 0) {
                    addRingAlert();
                } else {
                    clearRingAlert();
                }
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    const listeners = () => {
        $(document).on('click', '.btnDetalleAlerta', function (e) {
            const id = $(this).data('id');
            ModalDetalleAlertaGeneral.init(id);
        });

        $(document).on('click', '.alertDetalle', function (e) {
            const id = $(this).attr("data")
            $('.menu-notif').addClass('show');
            ModalDetalleAlertaGeneral.init(id);
        });
        $(document).on('click', '#btnCerrarAlerta', function (e) {
            CerrarAlerta();
        });
    }
    const CerrarAlerta = function () {
        $('#modal_alerta').removeClass('show');
        init();
    }
    const addRingAlert = () => {
        $('#alert-pulse-ring').removeClass('pulse-ring')
        $('#alert-pulse-ring').addClass('pulse-ring')
    };

    const clearRingAlert = () => {
        $('#alert-pulse-ring').removeClass('pulse-ring')
    };
    return {
        init: function () {
            init();
        },
        push: function (data) {
            pushNotification(data);
        }
    }
}();
const UpdateDashboardLineasByNotification = function () {
    const init = function () {
        reload();
    }

    const reload = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/DashboardLineas/UpdateLineas',
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#rowDashboardLineasLayoutSMADSOT').html('');
                $('#rowDashboardLineasLayoutSMADSOT').html(result.result);
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    const listeners = () => {
    }
    return {
        init: function () {
            init();
        }
    }
}();
$(document).ready(function () {
    Alertas.init();
})