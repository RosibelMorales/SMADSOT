(function ($) {

    var eventosControl = function () {

        $("#btnSesion").click(function (e) {
            e.preventDefault();

            eventos.botonCargando();

            var form = $(this).closest('form');

            if (!$("#formAutenticacion").valid()) {
                eventos.botonNoCargando();
                return;
            }

            eventos.ocultarMsjErrores();

            var formData = new FormData(form[0]);

            funciones.Post($(form).prop("action"), formData, function (response) {
                if (response.isSuccessFully === true) {
                    eventos.botonCargando();
                    window.localStorage.clear();
                    window.location.href = '/EstadisiticaUsoFormaValorada';
                } else {
                    eventos.botonNoCargando();
                    eventos.mostrarMsjErrores(undefined);
                }
            });
        });

        $("#Usuario").focus(function () {
            eventos.ocultarMsjErrores();
        });

        $("#Contrasenia").focus(function () {
            eventos.ocultarMsjErrores();
        });

        $('#IdVerificentro').select2({
            dropdownParent: $('#formAutenticacion'),
            placeholder:'Verificentro'
        });

        $('#Usuario').focusout(function (e) {
            e.preventDefault();

            eventos.botonCargando();

            $('#IdVerificentro').empty();
            $.ajax({
                type: 'GET',
                url: '/Autenticacion/GetVerificentros?usuario=' + $("#Usuario").val(),
                dateType: 'json',
                success: function (res) {
                    if (res.isSuccessFully === true) {
                        $.each(res.result, function (i, e) {
                            $('#IdVerificentro').append('<option value="' + e.id + '">' + e.nombre + '</option>');
                        });
                    } else {
                        eventos.mostrarMsjErrores(res.message);
                    }
                    $('.select2').click(function () {
                        $("#IdVerificentro").select2('open');
                    });
                    eventos.botonNoCargando();
                },
                error: function (ex) {
                    eventos.mostrarMsjErrores(ex);
                    eventos.botonNoCargando();
                }
            });
            return false;
        });

        eventos.ocultarMsjErrores();
    }

    var eventos = {

        ocultarMsjErrores: function () {
            $("#divAlerta").addClass("item-hide-Elemento");
        },
        mostrarMsjErrores: function (result) {
            if (result !== undefined)
                $('#divAlertaMsg').html(result);
            else
                $('#divAlertaMsg').html('Usuario o contraseña incorrectos.');
            $("#divAlerta").removeClass("item-hide-Elemento");
        },
        botonCargando: function () {

            document.querySelector('#btnSesion').setAttribute('data-kt-indicator', 'on');
            $("#btnSesion").prop('disabled', true);

        },
        botonNoCargando: function () {

            document.querySelector('#btnSesion').removeAttribute('data-kt-indicator', 'on');
            $("#btnSesion").prop('disabled', false);
        }
    }

    var funciones = {

        Post: function (url, data, callback) {
            $.ajax({
                url: url,
                type: 'POST',
                processData: false,
                contentType: false,
                data: data,
                success: callback
            });

        }

    }

    eventosControl();

})(jQuery);



