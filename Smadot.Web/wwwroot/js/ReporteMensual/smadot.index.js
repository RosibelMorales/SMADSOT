"use strict"

moment.locale('es');
var datatable;
var solicitar = 4;

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
                url: '/ReporteMensual/Consulta',
                type: 'POST'
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [               
                { data: 'numeroReporte', name: 'NumeroReporte', title: 'Numero de Reporte' },
                {
                    data: 'fechaModificacionReporte', name: 'FechaModificacionReporte', title: 'Última Modificación',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaModificacionReporte === null)
                                return '--/--/--';
                            else
                                return moment(row.fechaModificacionReporte).format('DD/MM/YYYY');
                        }
                    }
                },           
                {
                    data: 'fechaRegistroReporte', name: 'FechaRegistroReporte', title: 'Mes de Registró',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            if (row.fechaRegistroLinea === null)
                                return '--/--/--';
                            else

                                return moment(row.fechaRegistroReporte).format('MMMM');
                                //return moment(row.fechaRegistroLinea).format('MM');
                        }
                    }
                },
                { data: 'nombreCatEstatusReporte', name: 'NombreCatEstatusReporte', title: 'Estatus' },
                { data: 'nombreUsuario', name: 'NombreUsuario', title: 'Personal Registro' },
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

$(document).on('click', '#btnSolitarModificar, .btnSolitarModificar', function (e) {
    var id = $(this).data('id');

    Swal.fire({
        title: '¿Estas seguro?',
        text: "Se enviara una solicitud para modificar",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Si, estoy seguro',
        cancelButtonText: 'No, regresar'
    }).then((result) => {
        if (result.isConfirmed) {
            var form = $('#form_registroReporteMensual')[0];
            var formData = new FormData(form);
            formData.set('[0].IdReporte', id);
            formData.set('[0].IdCatEstatusReporte', solicitar);
            $.ajax({
                cache: false,
                type: 'POST',
                processData: false,
                contentType: false,
                enctype: 'multipart/form-data',
                url: '/ReporteMensual/Edit',
                data: formData,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    toastr.success('Los datos se actualizaron correctamente.', 'SMADSOT');
                    ModalEdit.cerrarventanamodal();
                    return;
                },
                error: function (res) {
                    toastr.error(res, 'SMADSOT');
                }
            });
        }
    })
});

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});