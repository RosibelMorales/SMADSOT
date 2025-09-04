"use strict"
var btns = false;
var acceso;
moment.locale('es');

$(document).on('click', '#btnRegistro, .btnEditar', function (e) {
    var id = $(this).data('id');

    ModalEdit.init(id);
});

var ModalEdit = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Roles/Edit/' + id,
            data: function (d) {
                d.id = $('#motivoGrid').select2('data')[0].id;
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Registrar Rol' : 'Editar Rol');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#motivoGrid').select2({
                        dropdownParent: $('#modal_registro')
                    });
                });
                //if (!btns) {
                //   btns = true;
                //} 
                $('#modal_registro').modal('show');
                listeners();
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {
        
        $('.AccesoTotalVerificentro').off().on('click', function (e) {
            e.preventDefault();
        });

        GuardarFormulario.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });


    }

    // Cerramos la ventana modal
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
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


var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(function (status) {
            if (status === 'Valid') {
                var form = $('#form_Roles')[0];
                var formData = new FormData(form);
                if (document.getElementById("AccesoTotalVerificentro").checked == true) {
                    acceso = true;                   
                }
                else {
                    acceso = false;   
                }
                formData.set('[0].Id', formData.get('IdRol'));
                formData.set('[0].AccesoTotalVerificentros', acceso);
                formData.set('[0].Nombre', formData.get('NombreRol'));
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: 'Roles/Edit',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                        ModalEdit.cerrarventanamodal();
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
        var form = document.getElementById('form_Roles');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    NombreRol: {
                        validators: {
                            notEmpty: {
                                message: 'El Alias del Rol es requerido.'
                            },                          
                        }
                    },
                    Alias: {
                        validators: {
                            notEmpty: {
                                message: 'El Alias del Rol es requerido.'
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

        var total = $('.cantidadInput').length;
        var i = 1;
        while (i < total) {
            validation.addField('[' + i + '].CantidadSC', {
                validators: {
                    notEmpty: {
                        message: 'La cantidad es requerida.'
                    },
                    greaterThan: {
                        min: 0,
                        message: 'Solo se permiten números igual o mayor a 0',
                    },
                }
            });
            i++;
        }

        $(document).on('change', '.cantidadInput', function (e) {
            var id = this.id;
            i = 0;
            var value = this.value;
            var folioFinal = parseInt($('#z' + i + '__FolioInicialSC').val());
            validation.validate().then(function (status) {
                if (status === 'Valid') {
                    while (i < total) {
                        $('#z' + i + '__FolioInicialSC').val(folioFinal);
                        if (i > 0)
                            folioFinal = parseInt($('#z' + (i - 1) + '__FolioFinalSC').val());
                        folioFinal = parseInt($('#z' + i + '__CantidadSC').val()) + parseInt(folioFinal);
                        $('#z' + i + '__FolioFinalSC').val(folioFinal);
                        i++;
                    }
                }
            });
        });
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


$(document).on('change', '#AccesoTotalVerificentro, .AccesoTotalVerificentro', function (e) {
    var valor = $('#AccesoTotalVerificentro').val(this.value);
   
    if (document.getElementById("AccesoTotalVerificentro").checked == true) {
    }
    else {
        document.getElementById("AccesoTotalVerificentro").value = "";
    }
});