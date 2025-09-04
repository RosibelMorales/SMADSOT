"use strict"

var datatable;
var formData = new FormData();
var KTDatatableRemoteAjax = function () {
    var init = function () {
        datatable = $('#kt_datatable').DataTable({
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
            aaSorting: [[3, 'desc']],
            ajax: {
                url: 'Testificacion/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",                
                "targets": "_all"
            }],
            columns: [
                { data: 'placa', name: 'Placa', title: 'Placa' },
                { data: 'entidadProcedencia', name: 'EntidadProcedencia', title: 'Entidad de Procedencia' },
                { data: 'propietario', name: 'Propietario', title: 'Propietario' },
                { data: 'marca', name: 'Marca', title: 'Marca' },
                { data: 'modelo', name: 'Modelo', title: 'Submarca' },
                {
                    data: 'fechaRegistro', name: 'FechaRegistro', title: 'Fecha de Registro',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaRegistro).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'nombreTecnico', name: 'NombreTecnico', title: 'Usuario Registró' },
                { data: 'folioFoliosFormaValoradaVerificentro', name: 'FolioFoliosFormaValoradaVerificentro', title: 'Folio asignado' },
                { data: 'claveTramite', name: 'ClaveTramite', title: 'Clave trámite' },
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

//$(document).on('click', '.btnDetalle', function (e) {
//    var id = $(this).data('id');

//    location.href = siteLocation + "Testificacion/Detalle/" + id;
//});

$(document).on('click', '.btnGenerarCertificado', function (e) {
    var id = $(this).data('id');
    GenerarPDF.generar(id);
});

$(document).on('click', '.btnEliminar', function (e) {
    e.preventDefault();
    EliminarRegistro.eliminar($(this).data('id'));
});

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
        init: function () {
            init();
        },
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
                    url: 'Testificacion/EliminarRegistro?id=' + id,
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
});