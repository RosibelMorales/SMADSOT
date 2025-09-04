"use strict"

var datatable;

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
            aaSorting: [[0, 'desc']],
            ajax: {
                url: 'ConsultaCirculares/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'numeroCircular', name: 'NumeroCircular', title: 'Número de Circular' },
                { data: 'fechaString', name: 'Fecha', title: 'Fecha' },
                { data: 'leidos', name: 'Leidos', title: 'Leídos', orderable: false, },
                { data: 'noLeidos', name: 'noLeidos', title: 'No Leídos', orderable: false, },
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

$(document).on('click', '#bntRegistrar', function (e) {
    var id = $(this).data('id');
    Modal.init(id);
});
$(document).on('click', '.btnEditar', function (e) {
    var id = $(this).data('id');
    var leido = $(this).data('leido');
    var noleido = $(this).data('no-leido');
    ModalDetalle.init(id, leido, noleido);
});
var Modal = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {

        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "ConsultaCirculares/Registro",
            success: function (result) {
                if (result.error) {
                    toastr.error(result.errorDescription, "SMADSOT");
                } else {

                    $('#modal_registro').removeClass('modal-md');
                    $('#modal_registro').removeClass('modal-lg');
                    $('#modal_registro').addClass('modal-xl');
                    $('#modal_registro #modalLabelTitle').html("Registrar Envío de Circular");
                    $('#modal_registro .modal-body').html("");
                    /*$('#modal_registro .modal-body').html("<div class='row mb-5 mt-5'><form role='form' class='clearfix'><div class='row fv-row mb-5'><div class='col-md-12'><div class='form-group'><label for='name'>Mensaje: &nbsp;</label><textarea class='form-control' id='Mensaje'></textarea></div></div></div></div></form></div>");*/
                    $('#modal_registro .modal-body').html(result.result);
                    $('#Mensaje').summernote({
                        height: 300
                    });

                    $('#modal_registro').modal('show');
                    $('#btnGuardarRegistro').html('');
                    $('#btnGuardarRegistro').html('Enviar');
                    $('#btnGuardarRegistro').show();
                }

                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });

        listeners();
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {
        $('#btnGuardarRegistro').off().on('click', function (e) {
            //toastr.success('Registro guardado exitosamente', "SMADSOT");
            //$('#modal_registro').modal('hide');
            SendEmailData.init();
        });
        $('#modal_registro').on('show.bs.modal', function (event) {
            //$('#Mensaje').summernote();
            /*$('#Mensaje').summernote();*/

        });
    }

    var cerrarModal = function () {
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
var ModalDetalle = function () {
    var init = function (id, leido, noleido) {
        abrirModal(id, leido, noleido);
    }
    var abrirModal = function (id, leido, noleido) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "ConsultaCirculares/Detalle?id=" + id,
            success: function (result) {
                if (result.error) {
                    toastr.error(result.errorDescription, "SMADSOT");
                } else {

                    $('#modal_registro').removeClass('modal-md');
                    $('#modal_registro').removeClass('modal-xl');
                    $('#modal_registro').addClass('modal-lg');
                    $('#modal_registro #modalLabelTitle').html("Detalle de Circular");
                    $('#modal_registro .modal-body').html("");
                    /*$('#modal_registro .modal-body').html("<div class='row mb-5 mt-5'><form role='form' class='clearfix'><div class='row fv-row mb-5'><div class='col-md-12'><div class='form-group'><label for='name'>Mensaje: &nbsp;</label><textarea class='form-control' id='Mensaje'></textarea></div></div></div></div></form></div>");*/
                    $('#modal_registro .modal-body').html(result.result);
                    $('#DetalleMensaje').summernote({
                        height: 300, dialogsInBody: true,
                        toolbar: [
                        ]
                    });
                    $('#DetalleMensaje').summernote('disable');
                    $('#modal_registro').modal('show');
                    $('#btnGuardarRegistro').hide();
                    KTDatatableRemoteAjax.recargar();
                }

                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
        //$('#modal_registro').modal('show');
        //$('#modal_registro').removeClass('modal-md');
        //$('#modal_registro').removeClass('modal-xl');
        //$('#modal_registro').addClass('modal-lg');
        //$('#modal_registro #modalLabelTitle').html("Detalle de Circular");
        //$('#modal_registro .modal-body').html("");
        //$('#modal_registro .modal-body').html("<div class='row mb-5 mt-5'><form role='form' class='clearfix'><div class='row fv-row mb-5'><div class='col-md-12'><div class='form-group'><label for='name'>Mensaje: &nbsp;</label><textarea readonly class='form-control'></textarea></div></div></div><div class='row fv-row mb-5'><div class='col-md-6'><div class='form-group'><label for='name'>Leído: &nbsp;</label><input readonly type='text' value='" + leido + "' class='form-control'/></div></div><div class='col-md-6'><div class='form-group'><label for='name'>No leído: &nbsp;</label><input readonly type='text' value='" + noleido + "' class='form-control'/></div></div></div></form></div>");
        //$('#btnGuardarRegistro').hide();
        listeners();
    }
    var listeners = function () {
        $('#btnGuardarRegistro').off().on('click', function (e) {
            toastr.success('Registro guardado exitosamente', "SMADSOT");
            $('#modal_registro').modal('hide');
        });
    }

    return {
        init: function (id, leido, noleido) {
            init(id, leido, noleido);
        }
    }
}();

var SendEmailData = function () {

    var validation; // https://formvalidation.io/

    var init = function () {
        guardar();

    };
    var guardar = function () {
        blockUI.block();



        var formData = new FormData();

        formData.append("Mensaje", $("#Mensaje").val());

        $.ajax({
            cache: false,
            type: 'POST',
            //contentType: 'application/json;charset=UTF-8',
            // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
            /*enctype: 'multipart/form-data',*/
            url: siteLocation + "ConsultaCirculares/CreateBodyCircular",
            processData: false,
            contentType: false,
            //dataType: 'json',
            data: formData,
            success: function (result) {
                if (result.error) {
                    //Modal.cerrarventanamodal();
                    toastr.error(result.errorDescription, '');
                } else {

                    Modal.cerrarventanamodal();
                    toastr.success(result.errorDescription, '');
                    KTDatatableRemoteAjax.recargar();
                }
                blockUI.release();

                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error("Error", '');
            }
        });
    };

    return {
        init: function () {
            init();
        }
    }
}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});