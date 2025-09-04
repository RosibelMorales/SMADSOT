"use strict"

var datatable;
var myDropzone;

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
            order: [[3, 'asc']],
            ajax: {
                url: siteLocation + 'RecepcionDocumentos/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                className: "dtr-control",
                "defaultContent": "-",

            }, { responsivePriority: 4, targets: 9 }, { responsivePriority: 12, targets: -1 }, { responsivePriority: 13, targets: -2 }],
            columns: [
                { data: 'folio', name: 'Folio', title: 'Folio', with: 120 },
                { data: 'nombrePersona', name: 'NombrePersona', title: 'Nombre Propietrario / Representante Legal' },
                { data: 'nombreGeneraCita', name: 'NombreGeneraCita', title: 'Nombre de Quien Verifica' },
                { data: 'fechaStr', name: 'Fecha', title: 'Fecha' },
                { data: 'placa', name: 'Placa', title: 'Placa' },
                { data: 'marca', name: 'Marca', title: 'Marca' },
                { data: 'modelo', name: 'Modelo', title: 'Modelo' },
                { data: 'serie', name: 'Serie', title: 'Serie' },
                { data: 'progreso', name: 'EstatusPrueba', title: 'Estatus', target: -2 },
                { data: 'lineaNombre', name: 'LineaNombre', title: 'Linea' },
                { data: 'verificentroAbrv', name: 'Verificentro', title: 'Verificentro', target: -2 },
                { data: 'orden', name: 'Turno', title: 'Turno' },
                {
                    target: 1,
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
        handleSearchDatatable();
    };

    var recargar = function () {
        datatable.ajax.reload();
    }
    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    var listeners = function () {
        //$(document).on('click', '.descargarDoc', function (e) {
        //    e.preventDefault()
        //    DescargarDocumento.generar($(this).data('url'), $(this).data('id'));
        //});
        //$('.descargarDoc').off()

        //$(document).on("click", '.btnAutorizar', function (e) {
        //    e.preventDefault()
        //    let id = $(this).attr("data-id");
        //    AutorizarModal.init(id);
        //})
        //$('.btnAutorizar').off()
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
var folioCancelado = function () {
    const init = (folio) => {
        Swal.fire({
            html: '¿Está seguro que desea realizar el cálculo de resultados para la verificación del auto con Placas: <b>' + folio.placa + '</b> y No. de Serie: <b>' + folio.serie + '</b>?',
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
                GenerarFolio(folio.id);
            }
        })

    }
    const GenerarFolio = (id) => {
        $.ajax({
            url: "/RecepcionDocumentos/CalcularResultados",
            type: "POST",
            dataType: "json",
            data: { id: id },
            success: function (data) {
                console.log(data)
                if (!data.error) {
                    KTDatatableRemoteAjax.recargar();
                    toastr.success(data.errorDescription, "SMADSOT")

                } else {
                    toastr.error(data.errorDescription, "SMADSOT")
                }
            },
            error: function (res) {
                toastr.error("El servicio no está disponible", 'SMADSOT');
            }
        });
    }
    return {
        init: (folio) => {
            init(folio);
        }
    }
}()

$(document).on('click', '.btnCambiarLinea', function (e) {
    /*var idVerificacion = $(this).data('idVerificacion');*/
    var idVerificacion = $(this).attr('data-id');
    ModalCambioLinea.init(idVerificacion);
});
$(document).on('click', '.btnCalcular', function (e) {
    /*var idVerificacion = $(this).data('idVerificacion');*/
    var folio = $(this).data();
    folioCancelado.init(folio);
});

var ModalCambioLinea = function () {
    var init = function (idVerificacion) {
        abrirModal(idVerificacion);
    }
    var abrirModal = function (idVerificacion) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'RecepcionDocumentos/CambioLinea/' + idVerificacion,
            success: function (result) {
                //blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro .modal-title').html('Cambio de Linea');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro .modal-body').html(result.result);
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#IdLinea').select2({
                        dropdownParent: $('#modal_registro')
                    });
                });
                $('#modal_registro').modal('show');
                listeners();

                //blockUI.release();
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var listeners = function () {
        GuardarFormularioCambiarLinea.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormularioCambiarLinea.guardar();
        });
    }
    var cerrarModal = function () {
        //KTDatatableRemoteAjax.recargar();
        $('#btnCerrarRegistro').click();
    }
    return {
        init: function (idVerificacion) {
            init(idVerificacion);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

var GuardarFormularioCambiarLinea = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_cambiarLinea')[0];
                var formData = new FormData(form);
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/RecepcionDocumentos/CambiarLinea',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        KTDatatableRemoteAjax.recargar();
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                        ModalCambioLinea.cerrarventanamodal();
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
        var form = document.getElementById('form_cambiarLinea');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    IdVerificacion: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                        }
                    },
                    IdLinea: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
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

    return {
        init: function () {
            init();
        },
        guardar: function () {
            guardar();
        }
    }

}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});