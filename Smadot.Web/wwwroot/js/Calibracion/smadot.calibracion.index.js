"use strict"

var datatable;
var myDropzone, myDropzone1;

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
            order: [0, 'desc'],
            ajax: {
                url: 'Calibracion/Consulta?id=' + $("#IdVerificentro").val(),
                type: 'POST'
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }, { responsivePriority: 1, targets: -1 }, { responsivePriority: 2, targets: -2 }],
            columns: [
                {
                    data: 'fechaCalibracion', name: 'FechaCalibracion', title: 'Fecha de Remisión',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaCalibracion).format('DD/MM/YYYY');
                        }
                    }
                },
                {
                    data: 'consecutivo', name: 'Consecutivo', title: 'Calibración',
                    render: function (data, type, row) {
                        return 'Calibración' + '<br>' + data
                    }
                },
                {
                    data: 'fechaProgramada', name: 'FechaProgramada', title: 'Fecha Programada Calibración',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaProgramada).format('DD/MM/YYYY');
                        }
                    }
                },
                {
                    data: 'fechaProximaCalibracion', name: 'FechaProximaCalibracion', title: 'Fecha Siguiente Calibración',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaProximaCalibracion).format('DD/MM/YYYY');
                        }
                    }
                },
                // { data: 'laboratorio', name: 'Laboratorio', title: 'Laboratorio' },
                { data: 'nombreUserRegistro', name: 'NombreUserRegistro', title: 'Registró' },
                { data: 'nombreUserValido', name: 'NombreUserValido', title: 'Validó' },
                { data: 'nombreEquipo', name: 'NombreEquipo', title: 'Equipo' },
                { data: 'nombreLinea', name: 'NombreLinea', title: 'Línea' },
                {
                    data: 'estatusColor', name: 'IdCatEstatusCalibracion', title: 'Estatus',
                    render: function (data, type, row) {
                        let codeColor = '';
                        let title = '';
                        let icon = '';
                        let style = '';
                        if (data == 'Gris') {
                            title = 'Documentación sin revisar';
                            codeColor = 'badge-light';
                            style = 'background-color: var(--bs-secondary)!important;';
                            icon = 'bi bi-circle text-white';
                        }
                        else if (data == 'Rojo') {
                            title = 'Sin Calibrar';
                            codeColor = 'badge-danger';
                            icon = 'bi bi-exclamation-circle text-white';
                        }
                        else if (data == 'Naranja') {
                            title = 'Solicita Modificar';
                            codeColor = 'badge-warning';
                            icon = 'bi bi-slash-circle text-white';
                        }
                        else if (data == 'Verde') {
                            title = 'Validado DVRF';
                            codeColor = 'badge-success';
                            icon = 'bi bi-check-circle text-white';
                        }
                        return `<span class="badge ${codeColor} py-3 px-4 fs-7" style="${style}" title="${title}" >
                               ${row.nombreEstatusCalibracion}
                               </span>`;
                    }
                },
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
            order: [[1, 'desc']],

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

$(document).on('click', '.btnDetalle', function (e) {
    var id = $(this).data('id');
    blockUI.block();
    ModalDetalle.init(id);
});

$(document).on('click', '#bntRegistrar', function (e) {
    ModalRegistro.init(0);
});

$(document).on('click', '.btnEditar', function (e) {
    var id = $(this).data('id');
    ModalRegistro.init(id);
});


$(document).on('click', '.btnSolicitarModificacion', function (e) {
    var id = parseInt($(this).data('id'));
    Swal.fire({
        text: "¿Estás seguro de solicitar una modificación para esta calibración?",
        icon: "success",
        buttonsStyling: false,
        confirmButtonText: "Aceptar",
        cancelButtonText: 'Cancelar',
        showCancelButton: true,
        customClass: {
            confirmButton: "btn btn-primary",
            cancelButton: 'btn btn-danger'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                cache: false,
                type: 'PUT',
                url: siteLocation + 'Calibracion/SolicitarModificacion?id=' + id,
                success: function (result) {
                    blockUI.release();
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, "SMADSOT");
                        return;
                    }
                    toastr.success(result.message, "SMADSOT");
                    $('#modalNotificacionCalibracion').modal('hide');
                    KTDatatableRemoteAjax.recargar();

                    return;
                },
                error: function (res) {
                    toastr.error(res, "SMADSOT");
                    return;
                }
            });
        }
    });
});

var ModalDetalle = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'Calibracion/Detalle/' + id,
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modalNotificacionCalibracion .modal-title').html('Detalle');
                $('#modalNotificacionCalibracion .modal-body').html('');
                $('#modalNotificacionCalibracion .modal-dialog').addClass('modal-xl');
                $('#modalNotificacionCalibracion .modal-body').html(result.result);

                var estatusCalibracion = parseInt($('#modalNotificacionCalibracion .estatusCalibracion').val());
                //if (estatusCalibracion != 4) {
                //    $('#modalNotificacionCalibracion #btnValidar').hide();
                //    $('#modalNotificacionCalibracion #btnAutorizar').hide();
                //} else {
                //    $('#modalNotificacionCalibracion #btnValidar').show();
                //    $('#modalNotificacionCalibracion #btnAutorizar').show();
                //}
                $('#modalNotificacionCalibracion #btnValidar').hide();
                $('#modalNotificacionCalibracion #btnRechazarDoc').hide();
                $('#modalNotificacionCalibracion #btnAutorizar').hide();
                $('#modalNotificacionCalibracion #btnRechazarSolic').hide();
                if ($("#IsDoc").val() == 1) {
                    $('#modalNotificacionCalibracion #btnValidar').show();
                    $('#modalNotificacionCalibracion #btnRechazarDoc').show();
                    $('#modalNotificacionCalibracion #btnAutorizar').hide();
                    $('#modalNotificacionCalibracion #btnRechazarSolic').hide();
                }
                else if ($("#IsSolic").val() == 1) {
                    $('#modalNotificacionCalibracion #btnValidar').hide();
                    $('#modalNotificacionCalibracion #btnRechazarDoc').hide();
                    $('#modalNotificacionCalibracion #btnAutorizar').show();
                    $('#modalNotificacionCalibracion #btnRechazarSolic').show();
                }
                const idCalibracion = parseInt($('#modalNotificacionCalibracion .idCalibracion').val())
                $('#modalNotificacionCalibracion').modal('show');
                listeners(idCalibracion);
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var listeners = function (id) {
        $('#btnValidar').off().on('click', function (e) {
            Swal.fire({
                text: "¿Estás seguro de validar la documentación de esta calibración?",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Aceptar",
                cancelButtonText: 'Cancelar',
                showCancelButton: true,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: 'btn btn-danger'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        cache: false,
                        type: 'PUT',
                        url: siteLocation + 'Calibracion/Validar?id=' + id,
                        success: function (result) {
                            blockUI.release();
                            if (!result.isSuccessFully) {
                                toastr.error(result.message, "SMADSOT");
                                return;
                            }
                            toastr.success(result.message, "SMADSOT");
                            $('#modalNotificacionCalibracion').modal('hide');
                            KTDatatableRemoteAjax.recargar();
                            return;
                        },
                        error: function (res) {
                            toastr.error(res, "SMADSOT");
                            return;
                        }
                    });
                }
            });

        });

        $('#btnRechazarDoc').off().on('click', function (e) {
            Swal.fire({
                text: "¿Estás seguro de rechazar la documentación de esta calibración?",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Aceptar",
                cancelButtonText: 'Cancelar',
                showCancelButton: true,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: 'btn btn-danger'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        cache: false,
                        type: 'PUT',
                        url: siteLocation + 'Calibracion/RechazarDocumentacion?id=' + id,
                        success: function (result) {
                            blockUI.release();
                            if (!result.isSuccessFully) {
                                toastr.error(result.message, "SMADSOT");
                                return;
                            }
                            toastr.success(result.message, "SMADSOT");
                            $('#modalNotificacionCalibracion').modal('hide');
                            KTDatatableRemoteAjax.recargar();
                            return;
                        },
                        error: function (res) {
                            toastr.error(res, "SMADSOT");
                            return;
                        }
                    });
                }
            });

        });


        $('#btnAutorizar').off().on('click', function (e) {
            Swal.fire({
                text: "¿Estás seguro de autorizar la modificación de esta calibración?",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Aceptar",
                cancelButtonText: 'Cancelar',
                showCancelButton: true,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: 'btn btn-danger'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        cache: false,
                        type: 'PUT',
                        url: siteLocation + 'Calibracion/Autorizar?id=' + id,
                        success: function (result) {
                            blockUI.release();
                            if (!result.isSuccessFully) {
                                toastr.error(result.message, "SMADSOT");
                                return;
                            }
                            toastr.success(result.message, "SMADSOT");
                            $('#modalNotificacionCalibracion').modal('hide');
                            KTDatatableRemoteAjax.recargar();
                            return;
                        },
                        error: function (res) {
                            toastr.error(res, "SMADSOT");
                            return;
                        }
                    });
                }
            });

        });

        $('#btnRechazarSolic').off().on('click', function (e) {
            Swal.fire({
                text: "¿Estás seguro de rechazar la modificación de esta calibración?",
                icon: "success",
                buttonsStyling: false,
                confirmButtonText: "Aceptar",
                cancelButtonText: 'Cancelar',
                showCancelButton: true,
                customClass: {
                    confirmButton: "btn btn-primary",
                    cancelButton: 'btn btn-danger'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        cache: false,
                        type: 'PUT',
                        url: siteLocation + 'Calibracion/RechazarSolicitar?id=' + id,
                        success: function (result) {
                            blockUI.release();
                            if (!result.isSuccessFully) {
                                toastr.error(result.message, "SMADSOT");
                                return;
                            }
                            toastr.success(result.message, "SMADSOT");
                            $('#modalNotificacionCalibracion').modal('hide');
                            KTDatatableRemoteAjax.recargar();
                            return;
                        },
                        error: function (res) {
                            toastr.error(res, "SMADSOT");
                            return;
                        }
                    });
                }
            });

        });
    }

    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrar').click();
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

var ModalRegistro = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        const idCalibracion = parseInt(id);
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'Calibracion/Registro?id=' + idCalibracion,
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro .modal-title').html(idCalibracion === 0 ? 'Registro' : 'Editar');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro .modal-dialog').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
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

    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {
        moment.locale('es');
        $('#FechaHora').daterangepicker({
            alwaysShowCalendars: true,
            singleDatePicker: true,
            showDropdowns: true,
            showCustomRangeLabel: false,
            autoApply: true,
            timePicker: true,
            onSelect: function () {
                var dateObject = $(this).datepicker('getDate');
            },
            locale: {
                format: 'DD/MM/YYYY hh:mm A',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        });
        myDropzone = new Dropzone("#dropzonejs-reporte-sistema", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs-reporte-sistema", // Este es el contenedor donde se mostrarán las vistas previas
            init: function () {
                this.on("addedfile", function (file) {
                    if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
                        return file.type.includes(acceptedType);
                    })) {
                        toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
                        toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (this.files.length > this.options.maxFiles) {
                        toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
                        this.removeFile(file);
                    }
                });
                this.on("removedfile", function (file) {
                    // Aquí puedes realizar acciones cuando un archivo se elimina
                });
            },
        });
        myDropzone1 = new Dropzone("#dropzonejs-resultado-calibracion", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs-resultado-calibracion", // Este es el contenedor donde se mostrarán las vistas previas
            init: function () {
                this.on("addedfile", function (file) {
                    if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
                        return file.type.includes(acceptedType);
                    })) {
                        toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
                        toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (this.files.length > this.options.maxFiles) {
                        toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
                        this.removeFile(file);
                    }
                });
                this.on("removedfile", function (file) {
                    // Aquí puedes realizar acciones cuando un archivo se elimina
                });
            },
        });
        $('#FechaEmisionCertificado').daterangepicker({
            alwaysShowCalendars: true,
            singleDatePicker: true,
            showDropdowns: true,
            showCustomRangeLabel: false,
            autoApply: true,
            onSelect: function () {
                var dateObject = $(this).datepicker('getDate');
            },
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        });
        $('#IdLinea').select2({
            minimumResultsForSearch: 2,
            dropdownParent: $('#modal_registro'),

        });
        $('#IdLinea').on('select2:select', function (e) {
            var id = $(this).val();
            $("#IdEquipo").val('');
            cargarEquipos(id);
        });
        $('#IdEquipo').select2({
            minimumResultsForSearch: 2,
            dropdownParent: $('#modal_registro'),
        });
        var cargarEquipos = function (id) {
            $("#IdEquipo").empty();
            var valIdE = $("#IdEquipoPreSelected").val();
            $.ajax({
                cache: false,
                type: 'GET',
                url: siteLocation + 'Calibracion/GetEquipos?idLinea=' + id,
                success: function (result) {
                    $.each(result.result, function (i, equipo) {
                        $("#IdEquipo").append('<option value="' + equipo.id + '">' + equipo.nombre + '</option>');
                    });
                    if (valIdE > 0)
                        $('#IdEquipo').val(valIdE).trigger('change');
                    return;
                },
                error: function (res) {
                    toastr.error(res, "SMADSOT");
                    return;
                }
            });
        };
        $('#IdRealiza').select2({
            dropdownParent: $('#modal_registro'),
        });
        $("#IdRealiza").change(function () {
            if (this.value == '1') {
                MostrarDivCVV();
            }
            if (this.value == '2') {
                MostrarDivLaboratorio();
            }
            if (this.value == '') {
                MostrarDivInicial();
            }
        });
        const MostrarDivCVV = function () {
            EliminarValoresCamposDivLaboratorio();
            $('#div-inicial').show();
            $('#div-realizado-cvv').show();
            $('#div-realizado-laboratorio').hide();
        };
        const MostrarDivLaboratorio = function () {
            EliminarValoresCamposDivCVV();
            $('#div-inicial').show();
            $('#div-realizado-laboratorio').show();
            $('#div-realizado-cvv').hide();
        };
        const MostrarDivInicial = function () {
            EliminarValoresCamposDivCVV();
            EliminarValoresCamposDivLaboratorio();
            $('#div-inicial').hide();
            $('#div-realizado-laboratorio').hide();
            $('#div-realizado-cvv').hide();
        };
        const EliminarValoresCamposDivCVV = function () {
            myDropzone.removeAllFiles(true);
        };
        const EliminarValoresCamposDivLaboratorio = function () {
            $("#IdTecnico").val(null);
            $("#NombreTecnico").val('');
            $("#NumeroFolio").val(null);
            $("#FechaEmisionCertificado").val(null);
            myDropzone1.removeAllFiles(true);
        };

        AutoComplete.init();
        var listenersEditar = function () {

            var idRealiza = parseInt($("#IdRealiza").val());

            if (idRealiza === 1) {
                MostrarDivCVV();
            }
            if (idRealiza === 2) {
                MostrarDivLaboratorio();
                const NombreTecnico = $("#NombreTecnicoLaboratorio").val();
                const IdTecnico = $("#IdTecnicoLaboratorio").val();
                $("#NombreTecnico").val(NombreTecnico);
                $("#IdTecnico").val(IdTecnico);
                $("#autocomplete").append('<option value="' + IdTecnico + '">' + NombreTecnico + '</option>');
            }
            if (idRealiza === 0) {
                MostrarDivInicial();
            }
            cargarEquipos($("#IdLinea").val());
        }
        listenersEditar();
        GuardarFormulario.init();
        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            const realiza = parseInt($("#IdRealiza").val());

            if (realiza == 1) {
                GuardarFormulario.guardarCVV();
            }
            if (realiza == 2) {
                GuardarFormulario.guardarLaboratorio();
            }

            return;
        });
    }

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

var AutoComplete = function () {
    var init = function () {
        $("#NombreTecnico").val('')
        $("#IdTecnico").val('')
        $("#autocomplete").select2({
            dropdownParent: $("#modal_registro"),
            ajax: {
                url: siteLocation + 'Calibracion/Autocomplete',
                dataType: "json",
                delay: 2000,
                data: function (params) {
                    return {
                        q: params.term,
                        page: params.page,
                        records: 10,
                    }
                },
                processResults: function (data, params) {
                    params.page = params.page || 1
                    return {
                        results: data.items,
                        pagination: {
                            more: params.page * 10 < data.total_count,
                        },
                    }
                },
                cache: true,
            },
            placeholder: 'Ingresar Nombre de proveedor',
            minimumInputLength: 3,
            language: 'es',
        });

        $(document).on('change', '#autocomplete', function (e) {
            $("#NombreTecnico").val('')
            $("#IdTecnico").val('')
            $("#NombreTecnico").val($("#autocomplete option:selected").text())
            $("#IdTecnico").val(this.value)
        });
    };

    return {
        init: function () {
            init();
        }
    };
}();
var GuardarFormulario = function () {
    var validacionCVV, validacionLaboratorio;
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacionCVV();
        init_validacionLaboratorio();
    };
    var guardarCVV = function () {
        validacionCVV.validate().then(async function (status) {
            if (status === 'Valid') {
                guardar();
            }
        });
    };

    var guardarLaboratorio = function () {
        validacionLaboratorio.validate().then(async function (status) {
            if (status === 'Valid') {
                guardar();
            }
        });
    };

    var init_validacionCVV = function () {
        var form = document.getElementById('form_registroCalibracion');
        validacionCVV = FormValidation.formValidation(
            form,
            {
                fields: {
                    IdRealiza: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    FechaHora: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    IdLinea: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    IdEquipo: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    Nota: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
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
    }

    var init_validacionLaboratorio = function () {
        var form = document.getElementById('form_registroCalibracion');
        validacionLaboratorio = FormValidation.formValidation(
            form,
            {
                fields: {
                    IdRealiza: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    FechaHora: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    IdLinea: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    IdEquipo: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    NombreTecnico: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    FechaEmisionCertificado: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    NumeroFolio: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                            integer: {
                                message: 'El campo no es entero',
                            },
                            greaterThan: {
                                message: 'El campo debe ser mayor a 1',
                                min: 1,
                            }
                        }
                    },
                    Nota: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
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
                },
                excluded: new FormValidation.plugins.Excluded({
                    excluded: function (field, element, elements) {
                        if (field == "FechaHora" && field == "IdLinea" && field == "IdEquipo" && field == "Nota") {
                            var selectRealiza = $("#IdRealiza").val();
                            if (selectRealiza == 1) {
                                return false;
                            }
                            return true;
                        }
                        if (field == "FechaHora" && field == "IdLinea" && field == "IdEquipo" && field == "NombreTecnico" && field == "FechaEmisionCertificado" && field == "NumeroFolio" && field == "Nota") {
                            var selectRealiza = $("#IdRealiza").val();
                            if (selectRealiza == 2) {
                                return false;
                            }
                            return true;
                        }
                        return false;
                    }
                })
            }
        );
    }

    var guardar = async function () {
        var form = $('#form_registroCalibracion')[0];
        var formData = new FormData(form);
        var files = [];
        if (Dropzone.forElement("#dropzonejs-resultado-calibracion").files[0] != undefined) {
            files.push(Dropzone.forElement("#dropzonejs-resultado-calibracion").files[0]);
        }
        if (Dropzone.forElement("#dropzonejs-reporte-sistema").files[0] != undefined) {
            files.push(Dropzone.forElement("#dropzonejs-reporte-sistema").files[0]);
        }
        files = files.filter(element => {
            return element !== undefined;
        });
        if (files.length > 0) {
            var arrayOfBase64 = await readFile(files);
            files = [];
            for (let i = 0; i < arrayOfBase64.length; i++) {
                files.push((arrayOfBase64[i]));
            }
            formData.set('Files', JSON.stringify(files));
        }

        async function readFile(fileList) {
            function getBase64(file) {
                let fileData = {};
                fileData.Nombre = file.name;
                fileData.Tipo = file.type.split("/")[1];

                const reader = new FileReader()
                return new Promise((resolve) => {
                    reader.onload = (ev) => {
                        var base64Data = ev.target.result.split('base64,')[1];
                        fileData.Base64 = base64Data;
                        resolve(fileData)
                    }
                    reader.readAsDataURL(file)
                })
            }

            const promises = []

            for (let i = 0; i < fileList.length; i++) {
                promises.push(getBase64(fileList[i]))
            }
            return await Promise.all(promises)
        }

        $.ajax({
            cache: false,
            type: 'POST',
            processData: false,
            contentType: false,
            enctype: 'multipart/form-data',
            url: siteLocation + 'Calibracion/Registro',
            data: formData,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                $('#modal_registro').modal('hide');
                KTDatatableRemoteAjax.recargar();
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });

    }

    return {
        init: function () {
            init();
        },
        guardarCVV: function () {
            guardarCVV();
        },
        guardarLaboratorio: function () {
            guardarLaboratorio();
        }
    }

}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});