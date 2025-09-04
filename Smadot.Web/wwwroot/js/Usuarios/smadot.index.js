"use strict"

moment.locale('es');
var datatable;

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
            ajax: {
                url: '/Usuarios/Consulta',
                type: 'POST'
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'correoElectronico', name: 'CorreoElectronico', title: 'Correo electrónico' },

                { data: 'nombreUsuario', name: 'NombreUsuario', title: 'Nombre de Usuario' },
                { data: 'nombreRol', name: 'NombreRol', title: 'Rol' },
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

        $(document).on('click', '.btnLock', function (e) {
            e.preventDefault();
            ActivarDesactivar.init($(this).data('id'), $(this).data('lock'));
        });
        $(document).on('click', '.btn-cambiar-contrasenia', function (e) {
            e.preventDefault();
            ActualizarContrasenia.init($(this).data('id'), $(this).data('nombre'));
        });
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

var ActivarDesactivar = function () {
    var init = function (id, lock) {
        $.ajax({
            cache: false,
            type: 'POST',
            url: '/Usuarios/ActivarDesactivar',
            data: {
                Id: id
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                toastr.success('Usuario actualizado correctamente.', 'SMADSOT');
                KTDatatableRemoteAjax.recargar();
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    };

    return {
        init: function (id, lock) {
            init(id, lock);
        }
    }

}();
var ActualizarContrasenia = function () {
    var init = function (id, nombre) {
        Swal.fire({
            html: `¿Seguro que desea reestablecer la contraseña del usuario ${nombre}? Esta acción es irreversible.`,
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
                $.ajax({
                    cache: false,
                    type: 'POST',
                    url: '/Usuarios/CambiarContrasenia',
                    data: {
                        Id: id
                    },
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success(result.message ?? "Datos actualizados", 'SMADSOT');
                        KTDatatableRemoteAjax.recargar();
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, 'SMADSOT');
                    }
                });
            }

        });

    };

    return {
        init: function (id, lock) {
            init(id, lock);
        }
    }

}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});