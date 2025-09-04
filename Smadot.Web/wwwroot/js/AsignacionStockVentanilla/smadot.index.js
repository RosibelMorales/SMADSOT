"use strict"

var datatable;

var KTDatatableRemoteAjax = function () {
    var init = function () {
        datatable = $('#kt_datatable').DataTable({
            pageLength: 15,
            order: [[0, 'desc']],
            language: {
                "decimal": "",
                "emptyTable": "No hay información",
                "info": "Mostrando _END_ de _TOTAL_ entradas",
                "infoEmpty": "Mostrando 0 de 0 entradas",
                "infoFiltered": "(Filtrado de _MAX_ total entradas)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Mostrar _MENU_ Entradas",
                "loadingRecords": "Cargando...",
                "processing": "Procesando...",
                "search": "Buscar:",
                "zeroRecords": "No se encontraron resultados para mostrar",
                "paginate": {
                    "first": "Primero",
                    "last": "Último",
                    "next": "&raquo;",
                    "previous": "&laquo;"
                }
            },
            responsive: true,
            pagingType: 'simple_numbers',
            searching: true,
            lengthMenu: [15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: 'AsignacionStockVentanilla/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'id', name: 'Id', title: 'Número Salida' },
                { data: 'fechaRegistroStr', name: 'FechaRegistro', title: 'Fecha de Registro' },
                { data: 'fechaEntregaStr', name: 'FechaEntrega', title: 'Fecha de Entrega' },
                { data: 'userAprobo', name: 'userAprobo', title: 'Usuario Aprobó' },
                { data: 'userRecibe', name: 'userRecibe', title: 'Usuario Recibió' },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            var htmlString = row.actions;
                            return htmlString
                        }
                    }
                }
            ],
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');

    };
    var recargar = function () {
        datatable.ajax.reload();
    }
    return {
        init: function () {
            init();
        },
        recargar: function () {
            recargar();
        }
    };
}();

$(document).on('click', '#bntRegistrar', function (e) {
    var id = $(this).data('id');
    ModalEdit.init(id);
});
$(document).on('click', '.btnEditar', function (e) {
    var id = $(this).data('id');
    modalDetalle.init(id);
});

var ModalEdit = function () {
    var fv;
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'AsignacionStockVentanilla/Edit/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Registrar Asignación del stock de ventanilla' : 'Detalle de solicitud');
                if (id === undefined) {
                    $('#btnGuardarRegistro').show()
                    $('#modal_registro input').prop('readonly', false);
                } else {
                    $('#btnGuardarRegistro').hide()
                    $('#modal_registro input').prop('readonly', true);
                }
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#IdAlmacen').select2({
                        dropdownParent: $('#modal_registro'),
                        placeholder: "Selecciona un almacen..."
                    });
                });
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $('#modal_registro').modal('show');
                listeners();
                validate();
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {

        $('#btnGuardarRegistro').off().on('click', function (e) {
            if (fv) {
                fv.validate().then(async function (status) {
                    if (status === 'Valid' && validateTable()) {
                        GuardarInfo();
                    }
                });
            }
        });
        const folioInicialInputs = document.querySelectorAll('.folioInicialInput');
        const cantidadInputs = document.querySelectorAll('.cantidadInput');

        folioInicialInputs.forEach(box => {
            box.addEventListener('change', function handleClick(event) {
                let valor = Number(this.value);
                if (valor < 1) {
                    this.value = 0;
                    return;
                }
                let cantidad = Number(this.parentNode.parentNode.querySelectorAll('input')[0]?.value);
                this.parentNode.parentNode.querySelectorAll('input')[2].value = valor + (cantidad - 1);
                validateTable();
            });
        });
        cantidadInputs.forEach(box => {
            box.addEventListener('change', function handleClick(event) {
                let valor = Number(this.parentNode.parentNode.querySelectorAll('input')[1]?.value);
                if (valor < 1) {
                    // this.value = 0;
                    return;
                }
                let cantidad = Number(this?.value);
                this.parentNode.parentNode.querySelectorAll('input')[2].value = valor + (cantidad - 1);
                validateTable();
            });
        });
        moment.locale('es');
        $('#FechaEntrega').daterangepicker({
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

    }
    // Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCancelar').click();
    }
    var validate = () => {
        const form = document.getElementById('form_registroSolicitudFormaValorada');
        fv = FormValidation.formValidation(form, {
            fields: {
                FechaEntrega: {
                    date: {
                        format: 'DD/MM/YYYY',
                        message: 'El valor no es unca fecha válida',
                    },
                },
                IdAlmacen: {
                    validators: {
                        greaterThan: {
                            message: 'Debe seleccionar un almacen.',
                            min: 1,
                        },
                    },
                },
                UserRecibe: {
                    validators: {
                        notEmpty: {
                            message: 'El campo es obligatorio.',
                        },
                    },
                },

            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap(),
                submitButton: new FormValidation.plugins.SubmitButton(),
                icon: new FormValidation.plugins.Icon({
                    validating: 'fa fa-refresh',
                }),
            },
        });

    }
    var validateTable = function () {
        let CantidadTotal = 0;
        let isValid = true;
        document.querySelectorAll('#TableAsignacion tbody tr').forEach((item, i) => {
            let element = item;
            item.querySelectorAll('input').forEach((item, i) => {
                let valor = Number(item?.value);
                let elementParent = item.parentNode;
                if (valor < 0) {
                    DeleteMessage(elementParent);
                    elementParent.append(Createmessage('No se permiten números negativos.'));
                    isValid = false;
                }
            })
            item.querySelectorAll('input.cantidadInput').forEach((item, i) => {
                let cantidad = Number(item?.value)
                CantidadTotal += cantidad;
                let folioInicial = element.querySelectorAll('input')[1];
                let valuefolioInicial = Number(folioInicial?.value);
                let folioFinal = element.querySelectorAll('input')[2];
                let valuefolioFinal = Number(folioFinal?.value);
                let elementParent = folioInicial.parentNode;
                let elementParentFinal = folioFinal.parentNode;
                if (cantidad !== 0) {
                    if (valuefolioInicial < 1) {
                        DeleteMessage(elementParent);
                        elementParent.append(Createmessage('Debe ingresar un folio inicial.'));
                        isValid = false;
                    }
                    if ((valuefolioFinal - valuefolioInicial) !== (cantidad - 1)) {
                        DeleteMessage(elementParentFinal);
                        elementParentFinal.append(Createmessage('El folio final debe corresponder a la cantidad ingresada.'));
                        isValid = false;
                    }
                }
            })
        })
        if (CantidadTotal < 1) {
            let elementParentTable = document.getElementById('TableAsignacion').parentNode;
            document.querySelectorAll(".fv-plugins-message-container.tableMessage").forEach(e => e.remove());
            elementParentTable.append(Createmessage("Debe ingresar al menos una cantidad.", true));
            isValid = false;
        }
        return isValid;
    }
    const DeleteMessage = function (element) {
        element.querySelectorAll(".fv-plugins-message-container").forEach(e => e.remove());
    }
    const Createmessage = (message, isTableMessage, dataField) => {
        var elementMessage = document.createElement('div');
        elementMessage.classList.add('fv-plugins-message-container');
        if (isTableMessage)
            elementMessage.classList.add('tableMessage');
        elementMessage.innerHTML = `<div data-field="${dataField}" data-validator="notEmpty" class="fv-help-block">${message}</div>`;
        return elementMessage;
    }
    const GuardarInfo = () => {
        var request = {}
        request.FechaEntrega = $('#FechaEntrega')?.val();
        request.IdAlmacen = $('#IdAlmacen')?.val();
        request.UserRecibe = $('#UserRecibe')?.val();
        let asignaciones = [];
        document.querySelectorAll('#TableAsignacion tbody tr').forEach((element, i) => {
            let asignacion = {};
            let cantidad = Number(element.querySelectorAll('input')[0]?.value);
            if (cantidad > 0) {
                asignacion.Cantidad = cantidad
                asignacion.FolioInicial = Number(element.querySelectorAll('input')[1]?.value);
                asignacion.FolioFinal = Number(element.querySelectorAll('input')[2]?.value);
                asignacion.IdTipoCertificado = element.querySelectorAll('td[data-tipocertificado]')[0].dataset.tipocertificado;
                asignaciones.push(asignacion);
            }
        })
        request.Asignaciones = asignaciones;
        $.ajax({
            cache: false,
            type: 'Post',
            url: 'AsignacionStockVentanilla/Registro',
            data: request,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                toastr.success('Registro guardado exitosamente', "SMADSOT");
                $('#modal_registro').modal('hide');
                KTDatatableRemoteAjax.recargar();
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    return {
        init: function (id) {
            init(id);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();
var modalDetalle = function () {
    const init = (id) => {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'AsignacionStockVentanilla/Detalle/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Detalle de solicitud');

                $('#btnGuardarRegistro').hide()
                $('#modal_registro input').prop('readonly', true);
                //$("#modal_registro").on('shown.bs.modal', function () {
                //    $('#IdAlmacen').select2({
                //        dropdownParent: $('#modal_registro'),
                //        placeholder: "Selecciona un almacen..."
                //    });
                //});
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                listeners();
                $('#modal_registro').modal('show');
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var listeners = function () {

        moment.locale('es');
        $('#FechaEntrega').daterangepicker({
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

    }
    return {
        init: (id) => {
            init(id);
        }

    }
}()
jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});