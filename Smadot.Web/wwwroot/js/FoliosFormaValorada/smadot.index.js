var datatable;

var KTDatatableRemoteAjax = function () {
    var init = function () {
        foliosImprimir();

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
            ordering: false,
            ajax: {
                url: '/FoliosFormaValorada/Consulta',
                type: 'POST',
                data: function (d) {
                    // if ($('#certificadoGrid').select2('data') !== undefined)
                    //     d.tipoCertificado = $('#certificadoGrid').select2('data')[0].id;
                }
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all",

            }, { responsivePriority: 1, targets: -1 }, { responsivePriority: 2, targets: -2 }],
            columns: [
                //{ data: 'id', name: 'Id', title: '#' },
                { data: 'folioStr', name: 'Folio', title: 'Folio' },
                { data: 'propietario', name: 'Propietario', title: 'Propietario' },
                { data: 'tarjetaCirculacion', name: 'TarjetaCirculacion', title: 'Folio Tarjeta Circulación' },
                { data: 'placa', name: 'Placa', title: 'Placa' },
                { data: 'serie', name: 'Serie', title: 'Serie' },
                { data: 'marca', name: 'Marca', title: 'Marca' },
                { data: 'submarca', name: 'Submarca', title: 'Submarca' },
                { data: 'fechaStr', name: 'Fecha', title: 'Fecha' },
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
        $(document).on('click', '.btnImprimir', function () {
            var data = {
                id: $(this).data('id'),
                idVerificacion: $(this).data('idverificacion'),
                imprimir: false
            }
            ModalDetalleCertificado.init(data);
        })
        $(document).on('click', '.btnRecalcularFolio', function () {
            var idFolioFormaValorada = $(this).data('id');
            var idVerificentro = $(this).data('idverificentro')
            recalcularFolio(idFolioFormaValorada, idVerificentro);
        });

        // $("#certificadoGrid").on('change', function () { datatable.ajax.reload(); });
    };
    var recargar = function () {
        datatable.ajax.reload();
        foliosImprimir();
    }
    var enviar = (data) => {
        $.ajax({
            cache: false,
            type: 'POST',
            url: siteLocation + 'FoliosFormaValorada/EnviarImpresion',
            data: data,
            success: function (result) {
                //blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                toastr.success(result.message, "SMADSOT");
                recargar();
                $('#modal_registro').modal('hide');
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var foliosImprimir = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'FoliosFormaValorada/GetFoliosProximos',
            success: function (res) {
                $("#FolioDobleCero").text(res?.result?.folioDoblecero);
                $("#FolioCero").text(res?.result?.folioCero);
                $("#FolioUno").text(res?.result?.folioUno);
                $("#FolioDos").text(res?.result?.folioDos);
                $("#FolioCNA").text(res?.result?.folioCNA);
                return;
            },
            error: function (res) {
                toastr.error("Ocurrió un error al consultar el servicio.", "SMADSOT");
                return;
            }
        });
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
        },
        enviar: function (data) {
            enviar(data);
        }
    };
}();



var ModalDetalleCertificado = function () {
    var request;
    var init = function (data) {
        request = data;
        abrirModal();
    }
    var abrirModal = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'FoliosFormaValorada/DetalleCertificado',
            data: request,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro .modal-title').html('Vista Previa de Impresión');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro .modal-dialog').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $('#modal_registro #btnGuardarRegistro').html("Imprimir");
                listeners();
                KTDatatableRemoteAjax.recargar();
                $('#modal_registro').modal('show');
                return;
            },
            error: function (res) {
                toastr.error("Ocurrió un error al realizare la petición", "SMADSOT");
                return;
            }
        });
    }
    const listeners = () => {
        $('#modal_registro #btnGuardarRegistro').off().on('click', function () {
            request.imprimir = true;
            KTDatatableRemoteAjax.enviar(request);
        })
    }
    const cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarModalRegistro').click();
    }
    return {
        init: function (data) {
            init(data);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

var recalcularFolio = function (idFolioFormaValorada, idVerificentro) {

    Swal.fire({
        title: 'Calcular Folio',
        text: "¿Está seguro que desea recalcular el folio?",
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

            var url = siteLocation + `FoliosFormaValorada/RecalcularFolio?idFolioFormaValorada=${idFolioFormaValorada}&idVerificentro=${idVerificentro}`
            $.ajax({
                cache: false,
                type: 'PUT',
                processData: false,
                contentType: false,
                enctype: 'multipart/form-data',
                url: url,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    toastr.success(result.message, 'SMADSOT');
                    KTDatatableRemoteAjax.recargar();
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
    KTDatatableRemoteAjax.init();
});