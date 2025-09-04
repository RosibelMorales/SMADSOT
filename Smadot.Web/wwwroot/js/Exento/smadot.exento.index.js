"use strict"

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
                url: 'Exento/Consulta',
                type: 'POST'
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            aaSorting: [[6, 'desc']],
            columns: [
                { data: 'placa', name: 'Placa', title: 'Placa' },
                { data: 'serie', name: 'Serie', title: 'Serie' },
                { data: 'marca', name: 'Marca', title: 'Marca' },
                { data: 'submarca', name: 'Submarca', title: 'Submarca' },
                { data: 'modelo', name: 'Modelo', title: 'Modelo' },
                { data: 'folioStr', name: 'Folio', title: 'Folio' },
                {
                    data: 'fecha', name: 'FechaRegistro', title: 'Fecha Registro',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            console.log(row.fecha);
                            return moment(row.fecha).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'claveTramite', name: 'ClaveTramite', title: 'Clave Trámite' },
                {
                    data: 'vigencia', name: 'Vigencia', title: 'Vigencia',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.vigencia).format('DD/MM/YYYY');
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

    return {
        init: function () {
            init();
            handleSearchDatatable()
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

$(document).on('click', '.btnEliminar', function (e) {
    e.preventDefault();
    EliminarRegistro.eliminar($(this).data('id'));
});

$(document).on('click', '.btnPDFCertificadoRefrendo', function (e) {
    e.preventDefault();
    GenerarPDF.generar($(this).data('id'));
});

var ModalDetalle = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'Exento/Detalle/' + id,
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_cierre .modal-title').html('Detalle');
                $('#modal_cierre .modal-body').html('');
                $('#modal_cierre .modal-dialog').addClass('modal-xl');
                $('#modal_cierre .modal-body').html(result.result);
                $('#modal_cierre').modal('show');
                listeners();
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    const listeners = () => {
        $("#btn-registrar-ref").off().on('click', function (e) {
            e.preventDefault();
            ModalEditRefrendo.init($(this).data("id"));
        })
    }
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarModalRegistro').click();
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

var GenerarPDF = function (id) {
    var generar = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            enctype: 'multipart/form-data',
            url: '/Refrendo/GetPDFFormaValorada?id=' + id + '&exento=true',
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>Certificado</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:application/pdf;base64,' + result.result + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
                toastr.success('PDF generado correctamente.', 'SMADSOT');
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    };

    return {
        generar: function (id) {
            generar(id);
        }
    }

}();

var EliminarRegistro = function (id) {
    var eliminar = function (id) {
        Swal.fire({
            title: '¿Seguro que desea continuar?',
            text: "Se eliminará toda la información ingresada.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Aceptar',
            cancelButtonText: 'Cancelar'
        }).then(async (result) => {
            if (result.isConfirmed) {
                $.ajax({
                    cache: false,
                    type: 'DELETE',
                    url: 'Exento/EliminarRegistro?id=' + id,
                    success: function (result) {
                        if (result.error) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        KTDatatableRemoteAjax.recargar();
                        toastr.success('Información eliminada correctamente.', 'SMADSOT');
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, 'SMADSOT');
                    }
                });
            }
        })
    };

    return {
        eliminar: function (data) {
            eliminar(data);
        }
    }

}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
    const myModalEl = document.getElementById('modal_registro')
    myModalEl.addEventListener('hidden.bs.modal', event => {
        $('.modal-backdrop').css('--bs-backdrop-zindex', '1050');
    })
});