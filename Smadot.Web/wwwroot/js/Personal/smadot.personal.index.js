"use strict"

var datatablePersonal;

var KTDatatableRemoteAjaxPersonal = function () {
    var init = function () {
        datatablePersonal = $('#kt_datatablePersonal').DataTable({
            pageLength: 15,
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
                url: siteLocation + 'Personal/Consulta?id=' + $("#IdVerificentro").val(),
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                {
                    data: 'urlFoto', name: 'Nombre', title: '',
                    render: function (data, type, row) {

                        if (type === 'display') {
                            var htmlString = "";
                            if ((row.urlFoto != "" || row.urlFoto != null)) {
                                if (row.urlFoto.includes("http")) {
                                    //htmlString = '<a href="javascript:;" class="imageUser" data-id="' + row.urlFotoBase64 + '" data-nombre="' + row.nombre + '"><div class="symbol symbol-circle symbol-40px"><img src="' + row.urlFotoBase64 + '" alt=""/></div></a>'
                                    htmlString = '<a href="javascript:;" class="imageUser" data-id="' + row.urlFoto + '" data-nombre="' + row.nombre + '"><div class="symbol symbol-circle symbol-40px"><img src="' + row.urlFoto + '" alt=""/></div></a>'
                                } else {
                                    htmlString = '<div class="symbol symbol-circle symbol-40px"><div class="symbol-label fs-2 fw-semibold bg-success text-inverse-success">' + row.iniciales + '</div></div>'
                                }
                            } else {
                                htmlString = '<div class="symbol symbol-circle symbol-40px mb-10"><div class="symbol-label fs-2 fw-semibold bg-success text-inverse-success">' + row.iniciales + '</div></div>'
                            }

                            return htmlString
                        }
                    }
                },
                { data: 'numeroTrabajador', name: 'NumeroTrabajador', title: '# Trabajador' },
                { data: 'nombre', name: 'Nombre', title: 'Nombre' },
                { data: 'genero', name: 'Genero', title: 'Género' },
                { data: 'rfc', name: 'RFC', title: 'RFC' },
                { data: 'curp', name: 'CURP', title: 'CURP' },
                { data: 'nombrePuesto', name: 'NombrePuesto', title: 'Nombre de Puesto' },
                { data: 'estatusPuesto', name: 'EstatusPuesto', title: 'Estatus' },
                //{ data: 'tipoPuesto', name: 'TipoPuesto', title: 'Tipo de Puesto' },
                //{ data: 'fechaIncorporacionPuestoString', name: 'FechaIncorporacionPuesto', title: 'Fecha de Incorporación Puesto' },

                //{ data: 'fechaSeparacionPuestoString', name: 'FechaSeparacionPuesto', title: 'Fecha de Separación Puesto' },
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
        //$(document).off().on('click', '.btnDetalle', function (e) {
        //    var id = $(this).data('id');
        //    window.location.href = siteLocation + 'Personal/Detalle/' + id;
        //});

        $(document).on('click', '.imageUser', function (e) {
            var ref = $(this).data('id')
            var nombre = $(this).data('nombre')
            ModalImagen.init(ref, nombre)
        })
        $('.imageUser').off()

        $(document).on('click', '.btnBaja', function (e) {
            var fechabaja = $(this).data('fechabaja');
            if (fechabaja != '') {
                toastr.warning('Este usuario ya ha sido de baja');
                return;
            }
            var id = $(this).data('id');
            var nombre = $(this).data('nombre');
            var nombrepuesto = $(this).data('nombre-puesto');
            var idpuestoverificentro = $(this).data('idpuestoverificentro');
            blockUI.block();
            ModalBaja.init(id, nombre, nombrepuesto, idpuestoverificentro);
        });
        $('.btnBaja').off()

        $(document).on('click', '.btnSolicitarM', function (e) {
            /*var idUserPuestoVerificentro = $('#idUserPuestoVerificentro').val();*/
            var idUserPuestoVerificentro = $(this).data('idupv');
            var idCatEstatusPuesto = $(this).data('estatus');
            modificarEstatus(idUserPuestoVerificentro, idCatEstatusPuesto);
        });
        $('.btnSolicitarM').off()
    };
    var recargar = function () {
        datatablePersonal.ajax.reload();
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

//$(document).off().on('click', '.btnDetalle', function (e) {
//    var id = $(this).data('id');
//    window.location.href = siteLocation + 'Personal/Detalle/'+id;
//});

//$(document).off().on('click', '.imageUser', function (e) {
//    var ref = $(this).data('id')
//    var nombre = $(this).data('nombre')
//    ModalImagen.init(ref,nombre)
//})

//$(document).on('click', '.btnBaja', function (e) {
//    var fechabaja = $(this).data('fechabaja');
//    if (fechabaja != '') {
//        toastr.warning('Este usuario ya ha sido de baja');
//        return;
//    }
//    var id = $(this).data('id');
//    var nombre = $(this).data('nombre');
//    var nombrepuesto = $(this).data('nombre-puesto');
//    var idpuestoverificentro = $(this).data('idpuestoverificentro');
//    blockUI.block();
//    ModalBaja.init(id, nombre, nombrepuesto, idpuestoverificentro);
//});

var ModalBaja = function () {
    var init = function (id, nombre, nombrepuesto, idpuestoverificentro) {
        abrirModal(id, nombre, nombrepuesto, idpuestoverificentro);
    }
    var abrirModal = function (id, nombre, nombrepuesto, idpuestoverificentro) {

        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'Personal/Baja?nombre=' + nombre + '&nombrePuesto=' + nombrepuesto,
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro .modal-title').html('Baja');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro .modal-dialog').removeClass('modal-lg');
                $('#modal_registro .modal-dialog').addClass('modal-md');
                $('#modal_registro .modal-body').html(result.result);
                $('#modal_registro').modal('show');
                submitModal(id, idpuestoverificentro);
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var submitModal = function (id, idpuestoverificentro) {
        $(document).off().on('click', '#btnGuardarRegistro', function (e) {
            var Data = {
                IdUser: id,
                IdUserPuestoVerificentro: idpuestoverificentro,
                FechaSeparacion: moment($('#FechaSeparacion').val(), "DD-MM-YYYY")
            }
            $.ajax({
                cache: false,
                type: 'POST',
                contentType: 'application/json;charset=UTF-8',
                url: siteLocation + 'Personal/Baja',
                dataType: 'json',
                data: JSON.stringify(Data),
                async: true,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, "");
                        blockUI.release();
                    } else {
                        toastr.success("Usuario dado de baja exitosamente", "");
                        KTDatatableRemoteAjaxPersonal.recargar();
                        $('#modal_registro').modal('hide');
                        blockUI.release();
                    }
                    return;
                },
                error: function (res) {
                    toastr.error("Ocurrió un error al registrar la venta", "");
                    blockUI.release();
                }
            });

        });
    }
    var cerrarModal = function () {
        KTDatatableRemoteAjaxPersonal.recargar();
        $('#btnCerrarRegistro').click();
    }
    return {
        init: function (id, nombre, nombrepuesto, idpuestoverificentro) {
            init(id, nombre, nombrepuesto, idpuestoverificentro);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        },
        submitModal: function () {
            submitModal(id, idpuestoverificentro);
        }
    }
}();

var ModalImagen = function () {
    var init = function (id, nombre) {
        abrirModal(id, nombre);
    }
    var abrirModal = function (id, nombre) {

        $('#modal_registro .modal-title').html(nombre);
        $('#modal_registro .modal-body').html('');
        $('#modal_registro .modal-dialog').removeClass('modal-md');
        $('#modal_registro .modal-dialog').addClass('modal-lg');
        var stringHtml = '<div class="row mb-3 mt-3"><div class="col-md-6 offset-md-3"><img class="w-100 card-rounded" src="' + id + '" alt=""></div></div>'
        $('#modal_registro .modal-body').html(stringHtml);
        $("#btnGuardarRegistro").hide()
        $('#modal_registro').modal('show');
    }

    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
    }
    return {
        init: function (id, nombre) {
            init(id, nombre);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        },
    }
}();

$('#modal_registro').on('shown.bs.modal', function () {
    $('#FechaSeparacion').daterangepicker({
        singleDatePicker: true,
        showDropdowns: true,
        minYear: 1901,
        maxYear: parseInt(moment().format("YYYY"), 12),
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
});

var modificarEstatus = function (idUserPuestoVerificentro, idCatEstatusPuesto) {

    Swal.fire({
        title: 'Solicitar Modificar',
        text: "¿Está seguro que desea solicitar permiso para editar?",
        icon: "question",
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
                        blockUI.release();
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    blockUI.release();
                    toastr.success(result.message, 'SMADSOT');
                    //window.location.href = siteLocation + 'Personal';
                    KTDatatableRemoteAjaxPersonal.recargar()

                    return;
                },
                error: function (res) {
                    toastr.error(res, 'SMADSOT');
                }
            });
        }
    });
}

jQuery(document).ready(function () {
    KTDatatableRemoteAjaxPersonal.init();
});