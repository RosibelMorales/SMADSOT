"use strict"
var btns = false;
var acceso, idrolselect;
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
        var rolSelect = $('#rolGrid');
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Usuarios/Edit/' + id,
            data: function (d) {
                d.id = $('#rolGrid').select2('data')[0].id;
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Registrar Usuario' : 'Editar Usuario');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#rolGrid').select2({
                        dropdownParent: $('#modal_registro'),

                    });
                });
                //if (!btns) {
                //   btns = true;
                //} 

                //$('#rolGrid').val('3'); // Select the option with a value of '1'
                //$('#rolGrid').trigger('change'); // Notify any JS components that the value changed

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
                var form = $('#form_Usuarios')[0];
                var formData = new FormData(form);              
                formData.set('[0].IdUsuario', formData.get('IdUsuario'));
                formData.set('[0].CorreoElectronico', formData.get('Email'));
                formData.set('[0].Contrasenia', formData.get('Email'));
                formData.set('[0].NombreUsuario', formData.get('Nombre'));
                formData.set('[0].IdRol', formData.get('rolGrid'));
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: 'Usuarios/Edit',
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
        var form = document.getElementById('form_Usuarios');      
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    Nombre: {
                        validators: {
                            notEmpty: {
                                message: 'El Nombre de Usuario es requerido.'
                            },
                        }
                    },
                    Email: {
                        validators: {
                            notEmpty: {
                                message: 'El Correo Electrónico es requerido.'
                            },                          
                        }
                    },
                    
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


$(document).on('change', '#Email, .Email', function (e) {
    var emailField = document.getElementById('Email');

    // Define our regular expression.
    var validEmail = /^\w+([.-_+]?\w+)*@\w+([.-]?\w+)*(\.\w{2,10})+$/;

    // Using test we can check if the text match the pattern
    if (validEmail.test(emailField.value)) {
        emailField.checkValidity("Validado");
        //alert('Email is valid, continue with form submission');
        return true;
    } else {
        emailField.checkValidity("mensaje de prueba");
        return false;
    }
    emailField.reportValidity();
});

