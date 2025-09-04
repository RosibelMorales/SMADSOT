var myDropzone, myDropzone2, myDropzone3;
var proveedorGrid = function () {
    let datatable;
    var init = function () {
        datatable = $('#proveedorGrid').DataTable({
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
            lengthMenu: [10, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: 'Proveedor/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'nombre', name: 'Nombre', title: 'Nombre Técnico' },
                { data: 'correoElectronico', name: 'CorreoElectronico', title: 'Correo' },
                { data: 'telefono', name: 'Telefono', title: 'Teléfono' },
                { data: 'empresa', name: 'Empresa', title: 'Empresa' },
                {
                    data: 'autorizado', name: 'Autorizado', title: 'Autorizado',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.autorizado) {
                                return 'Sí'
                            }
                            return 'No'
                        }
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
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');
        listeners();
    };

    var recargar = function () {
        datatable.ajax.reload();
    }

    var listeners = function () {

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

$(document).on("click", ".btnEstatus", function () {
    let id = $(this).attr("data-id");
    let nombre = $(this).attr("data-nombre");
    let estatus = $(this).attr("data-estatus");
    Confirmar.init(id,nombre,estatus);
});

$(document).on("click", ".btnDetalle", function () {
    let id = $(this).attr("data-id");
    modalDetalle.init(id);
});

$(document).on("click", "#btnRegistrarProveedor", function () {
    ProveedorRegistro.init();
})

function SoloNumeros(e, ControlId) {
    var letra = e.onkeypress.arguments[0].key;

    if (!/[^0-9]+$/g.test(letra)) {
        $("#" + ControlId).html('');
        return true;
    } else {
        $("#" + ControlId).html('Solo se aceptan numeros');
        return false;
    }
}

var ProveedorRegistro = function () {
    var fv;
    var init = function () {
        abrirModal();
    }

    var abrirModal = function () {
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Proveedor/Registro',
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Registrar Proveedor');
                $('#btnGuardarRegistro').show()
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $('#modal_registro').modal('show');
                listeners();
                validate();
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error("No es posible abrir la pantalla de registro.", "SMADSOT");
                return;
            }
        });
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {

        $('#btnGuardarRegistro').off().on('click', function (e) {
            if (fv) {
                fv.validate().then(async function (status) {
                    if (status === 'Valid') {
                        GuardarInfo();
                    }
                });
            }
        });
    }
    //// Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
    }

    var validate = () => {
        const form = document.getElementById('form_registroProveedor');
        fv = FormValidation.formValidation(form, {
            fields: {
                Nombre: {
                    validators: {
                        notEmpty: {
                            message: 'El campo es obligatorio.',
                        },
                    },
                },
                CorreoElectronico: {
                    validators: {
                        notEmpty: {
                            message: 'El campo es obligatorio.',
                        },
                        emailAddress: {
                            message: 'La dirección de correo electronico no es valida',
                        },
                    },
                },
                Telefono: {
                    validators: {
                        notEmpty: {
                            message: 'El campo es obligatorio.',
                        },
                    },
                },
                Direccion: {
                    validators: {
                        notEmpty: {
                            message: 'El campo es obligatorio.',
                        },
                    },
                },
                Empresa: {
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
    
    const GuardarInfo = () => {

        fv.validate().then(async function (status) {
            /*validation.validate().then(async function (status) {*/
            if (status === 'Valid') {
                var request = {}
                request.Nombre = $("#Nombre")?.val();
                request.CorreoElectronico = $('#CorreoElectronico')?.val();
                request.Telefono = $("#Telefono")?.val();
                request.Direccion = $("#Direccion")?.val();
                request.Empresa = $('#Empresa')?.val();

                $.ajax({
                    cache: false,
                    type: 'Post',
                    url: 'Proveedor/Registro',
                    data: request,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        toastr.success('Registro guardado exitosamente', "SMADSOT");
                        $('#modal_registro').modal('hide');
                        proveedorGrid.recargar();
                        //KTDatatableRemoteAjax.recargar();
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, "SMADSOT");
                        return;
                    }
                });
            }
        });
    }

    return {
        init: function (id, isReadOnly) {
            init(id, isReadOnly);
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
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'Proveedor/Detalle/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Detalle de proveedor');

                $('#btnGuardarRegistro').hide()
                $('#modal_registro input').prop('readonly', true);
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                listeners();
                $('#modal_registro').modal('show');
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error(res, "SMADSOT");
                return;
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

var Confirmar = function () {
    const init = (id, nombre, estatus) => {
        abrirModal(id, nombre, estatus);
    }
    var abrirModal = function (id, nombre, estatus) {
        //blockUI.block();
        
        var est = estatus === "True" ? "desautorizar" : "autorizar";

        Swal.fire({
            html: '¿Está seguro que desea ' + est + ' al proveedor <strong>' + nombre + '</strong>?',
            icon: "info",
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
                var formData = new FormData();
                formData.append('IdEstatus', id);

                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "Proveedor/EstatusProveedor",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formData,
                    success: function (res) {
                        if (!res.isSuccessFully) {
                            toastr.error(res.message, "SMADSOT");
                            return;
                        }
                        toastr.success('La información se actualizo exitosamente', "SMADSOT");
                        //$('#modal_registro').modal('hide');
                        proveedorGrid.recargar();
                        //KTDatatableRemoteAjax.recargar();
                        return;
                    },
                    error: function (res) {
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
        init: (id, nombre, estatus) => {
            init(id, nombre, estatus);
        }

    }
}()

jQuery(document).ready(function () {
    proveedorGrid.init();
});