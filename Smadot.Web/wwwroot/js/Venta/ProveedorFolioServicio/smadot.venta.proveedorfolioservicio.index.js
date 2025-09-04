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
            lengthMenu: [15, 25, 50, 100],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: 'ProveedorFolioServicio/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'folioPF', name: 'FolioPF', title: 'Folio' },
                { data: 'proveedorEmpresa', name: 'proveedorEmpresa', title: 'Empresa' },
                { data: 'fechaRegistro', name: 'FechaRegistro', title: 'Fecha' },
                /*{ data: 'proveedor', name: 'Proveedor', title: 'Proveedor' },*/
                {
                    data: 'proveedor', name: 'Proveedor', title: 'Proveedor',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.esLaboratorio == false) {
                                return data
                            }
                            return '-'
                        }
                    }
                },
                {
                    data: 'proveedor', name: 'EsLaboratorio', title: 'Laboratorio',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.esLaboratorio == true) {
                                return data
                            }
                            return '-'
                        }
                    }
                },
                { data: 'motivo', name: 'Motivo', title: 'Motivo' },
                {
                    data: 'estatusFolio', name: 'EstatusFolio', title: 'EstatusFolio',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.estatusFolio) {
                                return 'En uso'
                            }
                            return 'Disponible'
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

$(document).on("click", ".btnDetalle", function () {
    let id = $(this).attr("data-id");
    modalDetalle.init(id);
});

$(document).on("click", "#btnRegistrarProveedor", function () {
    ProveedorRegistro.init();
})

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
            url: '/ProveedorFolioServicio/Registro',
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Registrar Proveedor - Laboratorio');
                $('#btnGuardarRegistro').show()
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                AutoComplete.init();
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
                //Nombre: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //    },
                //},
                //CorreoElectronico: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //        emailAddress: {
                //            message: 'La dirección de correo electronico no es valida',
                //        },
                //    },
                //},
                //Telefono: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //    },
                //},
                //Direccion: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //    },
                //},
                //Empresa: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //    },
                //},

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
                request.IdProveedor = $("#IdProveedor")?.val();
                request.FolioInicial = $('#FolioInicial')?.val();
                request.FolioFinal = $("#FolioFinal")?.val();

                $.ajax({
                    cache: false,
                    type: 'Post',
                    url: 'ProveedorFolioServicio/Registro',
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
            url: 'ProveedorFolioServicio/Detalle/' + id,
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

var AutoComplete = function () {
    var init = function () {
        $("#ProveedorNombre").val('')
        $("#IdProveedor").val('')
        $("#autocomplete").select2({
            dropdownParent: $("#modal_registro"),
            ajax: {
                url: '/ProveedorFolioServicio/Autocomplete',
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
            placeholder: 'Ingresar nombre de proveedor - laboratorio',
            minimumInputLength: 3,
            language: 'es',
        });

        $(document).on('change', '#autocomplete', function (e) {
            /*ModalEdit.init(this.value)*/
            $("#ProveedorNombre").val('')
            $("#IdProveedor").val('')
            $("#ProveedorNombre").val($("#autocomplete option:selected").text())
            $("#IdProveedor").val(this.value)
        });
    };

    return {
        init: function () {
            init();
        }
    };
}();

jQuery(document).ready(function () {
    proveedorGrid.init();
});