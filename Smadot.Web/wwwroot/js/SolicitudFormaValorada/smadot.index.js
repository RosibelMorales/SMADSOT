"use strict"

var datatable;
var btns = false;

var KTDatatableRemoteAjax = function () {
    var init = function () {
        datatable = $('#kt_datatable').DataTable({
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
            aaSorting: [[0, "desc"]],
            ajax: {
                url: 'SolicitudFormaValorada/Consulta',
                type: 'POST',
                data: function (d) {
                    if ($('#almacenGrid').select2('data') !== undefined)
                        d.idAlmacen = $('#almacenGrid').select2('data')[0].id;
                }
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'idSolicitudFV', name: 'IdSolicitudFV', title: 'Número de Solicitud' },
                {
                    data: 'fechaSolicitudFV', name: 'FechaSolicitudFV', title: 'Fecha de Solicitud',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaSolicitudFV).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'userSolicitaFV', name: 'UserSolicitaFV', title: 'Usuario que Solicitó' },
                { data: 'estatusFV', name: 'EstatusFV', title: 'Estatus' },
                {
                    data: 'fechaEntregaIFV', name: 'FechaEntregaIFV', title: 'Fecha de Entrega',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaEntregaIFV === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaEntregaIFV).format('DD/MM/YYYY');
                        }
                    }
                },
                {
                    data: 'fechaVentaVFV', name: 'FechaVentaVFV', title: 'Fecha de Venta',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaVentaVFV === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaVentaVFV).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'nombreRecibioIFV', name: 'NombreRecibioIFV', title: 'Usuario que recibió' },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            var htmlString = row.acciones;
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

$(document).on('click', '#bntRegistrar, .btnEditar', function (e) {
    var id = $(this).data('id');
    ModalEdit.init(id);
});

$(document).on('change', '#almacenGrid', function (e) {
    KTDatatableRemoteAjax.recargar();
});

var ModalEdit = function () {
    let idSolicitud;
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        idSolicitud = id;
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'SolicitudFormaValorada/Edit/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Registrar solicitud' : 'Editar solicitud');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#z0__IdAlmacenFV').select2({
                        dropdownParent: $('#modal_registro')
                    });
                });
                if (!btns) {
                    $('#btnCerrarRegistro').after('<button type="button" class="btn btn-success btn-sm font-weight-bold btnPDF">Vista Previa</button>');
                    $('#btnCerrarRegistro').after('<button type="button" class="btn btn-success btn-sm font-weight-bold btnPDF">Imprimir</button>');
                    btns = true;
                }
                $('#modal_registro').modal('show');
                listeners();
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    const reloadTableCertificados = function (idAlmacen) {
        console.log("almacen",idAlmacen)
        $.ajax({
            cache: false,
            type: 'GET',
            url: `SolicitudFormaValorada/TableCertificado/${idSolicitud}?idAlmacen=${idAlmacen}`,
            success: function (result) {
                $('#table-certificados').html('');
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#table-certificados').html(result.result);

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
        GuardarFormulario.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });

        $('.btnPDF').off().on('click', function (e) {
            e.preventDefault();
            GenerarPDF.generar();
        });
        $('#z0__IdAlmacenFV').on('change', function (e) {
            reloadTableCertificados($(this).val());
        })
        moment.locale('es');
        $('#FechaSolicitud').daterangepicker({
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
        }).on('show.daterangepicker', function (ev, picker) {
            // Ajusta el z-index del calendario para que sea mayor que el z-index del modal
            picker.container.css("z-index", 1061);
        });
        // $('#FechaSolicitud').daterangepicker({
        //     alwaysShowCalendars: true,
        //     singleDatePicker: true,
        //     showDropdowns: true,
        //     showCustomRangeLabel: false,
        //     autoApply: true,
        // });
    }
    // Cerramos la ventana modal
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarRegistro').click();
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

var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(function (status) {
            if (status === 'Valid') {
                // var form = $('#form_registroSolicitudFormaValorada')[0];
                // var formData = new FormData(form);
                // formData.set('[0].FechaSolicitudFV', formData.get('FechaSolicitud').split('/').reverse().join('/'));
                let data = {
                    idSolicitudFV: $("#IdSolicitudFV").val() ?? 0,
                    fechaSolicitudFV: $("#FechaSolicitud").val(),
                    idAlmacenFV: $("#z0__IdAlmacenFV").val() ?? 0,
                    solicitudesCertificado: []
                };
                var filas = document.querySelectorAll("#tableSolicitud tbody > tr");
                filas.forEach(function (row) {
                    // Buscar todos los elementos input dentro del tr actual
                    var cantidad = row.querySelectorAll('input[name*="CantidadSC"]')[0]?.value;
                    var idCatTipoCertificado = row.dataset.idcattipocertificado;
                    if (Number(cantidad) > 0) {

                        data.solicitudesCertificado.push({
                            idCatTipoCertificado: idCatTipoCertificado,
                            cantidad: cantidad
                        })
                    }
                });
                if (data.solicitudesCertificado.length === 0) {
                    toastr.error("No se ha ingresado ninguna cantidad.", 'SMADSOT');
                    return;
                }
                $.ajax({
                    cache: false,
                    type: 'POST',
                    url: 'SolicitudFormaValorada/Edit',
                    data: data,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                        ModalEdit.cerrarventanamodal();
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, 'SMADSOT');
                    }
                });
            }
        });
    };

    var init_validacion = function () {
        var form = document.getElementById('form_registroSolicitudFormaValorada');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    '[0].CantidadSC': {
                        validators: {
                            // notEmpty: {
                            //     message: 'La cantidad es requerida.'
                            // },
                            greaterThan: {
                                min: 1,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    '[0].IdAlmacenFV': {
                        validators: {
                            notEmpty: {
                                message: 'El almacén es requerido.'
                            }
                        }
                    },
                    'FechaSolicitud': {
                        validators: {
                            notEmpty: {
                                message: 'La fecha es requerida.'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap({
                        rowSelector: '.fv-row',
                        eleInvalidClass: 'is-invalid',
                        eleValidClass: ''
                    })
                }
            }
        );

        var total = $('.cantidadInput').length;
        var i = 1;
        while (i < total) {
            validation.addField('[' + i + '].CantidadSC', {
                validators: {
                    notEmpty: {
                        message: 'La cantidad es requerida.'
                    },
                    greaterThan: {
                        min: 0,
                        message: 'Solo se permiten números igual o mayor a 0',
                    },
                }
            });
            i++;
        }

        $(document).on('change', '.cantidadInput', function (e) {
            var id = this.id;
            var idCon = (this.id.split('__')[0].replace('z', ''));
            i = 0;
            var value = this.value;
            //var folioFinal = parseInt($('#z' + i + '__FolioInicialSC').val());
            var folioFinal = parseInt($('#z' + idCon + '__FolioInicialSC').val()) + parseInt(value) - 1;
            validation.validate().then(function (status) {
                if (status === 'Valid') {
                    //while (i < total) {
                    //    $('#z' + i + '__FolioInicialSC').val(folioFinal);
                    //    if (i > 0)
                    //        folioFinal = parseInt($('#z' + (i - 1) + '__FolioFinalSC').val());
                    //    $('#z' + i + '__FolioFinalSC').val(folioFinal);
                    //    i++;
                    //}
                    $('#z' + idCon + '__FolioFinalSC').val(folioFinal);
                }
            });
        });
    }
    return {
        init: function () {
            init();
        },
        guardar: function () {
            guardar();
        }
    }

}();

var GenerarPDF = function () {
    var init = function () {
    };
    var generar = function () {
        var form = $('#form_registroSolicitudFormaValorada')[0];
        var formData = new FormData(form);
        formData.set('[0].FechaSolicitudFV', formData.get('FechaSolicitud').split('/').reverse().join('/'));
        formData.set('[0].Almacen', $('#z0__IdAlmacenFV').select2('data')[0].text);
        $.ajax({
            cache: false,
            type: 'POST',
            processData: false,
            contentType: false,
            enctype: 'multipart/form-data',
            url: 'SolicitudFormaValorada/GetPDF',
            data: formData,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>Solicitud Forma Valorada</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:application/pdf;base64,' + result.result + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
                toastr.success('PDF generado correctamente.', 'SMADSOT');
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    };

    return {
        init: function () {
            init();
        },
        generar: function () {
            generar();
        }
    }

}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});