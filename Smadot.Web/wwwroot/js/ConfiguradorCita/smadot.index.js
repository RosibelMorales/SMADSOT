"use strict"
var datatable;

var KTDatatableRemoteAjax = function () {
    var init = function () {

        datatable = $('#kt_datatable').DataTable({
            order: [[0, 'desc']],
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
            order: [[1, 'asc']],
            ajax: {
                url: `/DirectorioCentrosVerificacion/ConsultaFechas/${$('#IdCVV').val()}`,
                type: 'POST',
                data: function (d) {
                    //d.idAlmacen = $('#almacenGrid').select2('data')[0].id;
                }
            },
            columnDefs: [{
                className: "dtr-control",
                "defaultContent": "-",

            },
            {
                targets: [0, 1, 2, 3, 4, 5], // Índices de las columnas a alinear (empezando desde 0)
                className: "text-center", // Agregar la clase "text-center"
            }
            ],

            columns: [
                //{ data: 'id', name: 'Id', title: '#' },
                { data: 'habilitadoStr', name: 'Habilitado', title: 'Habilitado', orderable: false, },
                { data: 'fechaStr', name: 'Fecha', title: 'Fecha' },
                {
                    data: null, defaultContent: '', title: 'Día', render: function (data, type, row) {
                        if (type === 'display') {
                            const fechaString = row.fechaStr;

                            const fecha = moment(fechaString, "DD/MM/YYYY", 'es');

                            const nombreDiaCompleto = fecha.format("dddd").charAt(0).toUpperCase() + fecha.format("dddd").slice(1);

                            return nombreDiaCompleto;
                        }
                    }
                },
                { data: 'horaInicioString', name: 'HoraInicio', title: 'Hora Inicio' },
                { data: 'horaFinString', name: 'HoraFin', title: 'Hora Fin' },
                { data: 'intervaloMin', name: 'InvertavaloMin', title: 'Intervalo Citas' },
                { data: 'capacidad', name: 'Capacidad', title: 'Capacidad' },
                {
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
            ]
        });

        $('thead tr').addClass('fw-bold fs-6 text-gray-800');

        $(document).on('click', ".btn-editar", function (e) {
            e.preventDefault();
            let id = $(this).data('id');
            EdicionHorario.abrirModal(id);
        });
    };

    var recargar = function () {
        datatable.ajax.reload();
    }

    var handleSearchDatatable = () => {
        // Variable para almacenar el temporizador
        let typingTimer;

        // Tiempo de espera en milisegundos
        const waitTime = 800; // 0.8 segundos
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        // Evento keyup con función de retardo
        filterSearch.addEventListener('keyup', function (e) {
            clearTimeout(typingTimer); // Reiniciar el temporizador en cada pulsación de tecla
            typingTimer = setTimeout(function () {

                if (e.target.value.length !== 0) {
                    reDrawDatatable(e.target.value); // Ejecutar el evento después del tiempo de espera
                }
            }, waitTime);
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

var EdicionHorario = function () {
    var fecha;
    var init = () => {
        $(document).on('click', '#modal_registro #btnGuardarRegistro', () => {
            if ($("#form-registro").valid()) {
                guardar(false);
            }
        });
    }
    var guardar = (validar) => {
        var formData = $('#form-registro').serializeArray();
        var jsonData = formData.reduce(function (obj, item) {
            if (item.name === "Habilitado") {
                obj[item.name] = $("#Habilitado").is(":checked");
            } else if (!isNaN(item.value)) {
                obj[item.name] = parseFloat(item.value);
            } else {
                obj[item.name] = item.value;
            }
            return obj;
        }, {});
        jsonData.fecha = fecha;
        $.ajax({
            cache: false,
            type: 'POST',
            url: `/DirectorioCentrosVerificacion/EdicionFecha`,
            data: {
                NoValidar: validar,
                Data: jsonData
            },
            success: function (result) {
                console.log(result);
                if (!result.isSuccessFully || result?.result.error) {
                    if (result?.result?.modificar)
                        return reEnviar(result.message);
                    else
                        return toastr.error(result.message, 'SMADSOT');
                }

                toastr.success(result.message, 'SMADSOT');
                $('#modal_registro').modal('hide');
                KTDatatableRemoteAjax.recargar();
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    }
    const reEnviar = (msg) => {
        Swal.fire({
            text: "Advertencia",
            html: msg,
            icon: "warning",
            buttonsStyling: false,
            confirmButtonText: "Aceptar",
            cancelButtonText: 'Cancelar',
            showCancelButton: true,
            customClass: {
                confirmButton: "btn btn-primary",
                cancelButton: 'btn btn-danger'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                guardar(true);
            }
        });
    }
    var abrirModal = (id) => {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/DirectorioCentrosVerificacion/ConfiguradorCita/' + id,
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                fecha = result.result.fechaStr;
                $('#modal_registro .modal-title').html(`Editar Horario ${fecha}`);
                $('#modal_registro .modal-body').html('');
                $('#modal_registro .modal-dialog').addClass('modal-md');
                $('#modal_registro .modal-body').html(result.result.html);
                $('#modal_registro').modal('show');
                listeners();
                validations();

                return;
            },
            error: function (res) {
                toastr.error("Ocurrió un error al conectar con el servicio.", "SMADSOT");
                return;
            }
        });
    }
    const listeners = () => {
        $("#Habilitado").off().on('change', function (e) {
            if ($(this).is(':checked'))
                $(".container-controls").show();
            else
                $(".container-controls").hide();

        });
        var timePicker = flatpickr("#HoraInicio", {
            allowInput: true,
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
        });
        var timePicker1 = flatpickr("#HoraFin", {
            allowInput: true,
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
        });
    }

    const validations = () => {
        $.validator.addMethod("checkHoraFin", function (value, element) {
            // Obtener el valor de horaInicio y horaFin
            var horaInicio = $(element).closest('form').find('input[name="HoraInicio"]').val();
            var horaFin = value;

            // Convertir los valores a objetos de tipo Date
            var fechaInicio = new Date('01/01/2000 ' + horaInicio);
            var fechaFin = new Date('01/01/2000 ' + horaFin);
            console.log(fechaInicio, fechaFin)
            // Comparar las fechas
            return fechaFin > fechaInicio;
        }, "La hora de fin debe ser mayor o igual que la hora de inicio.");
        $("#form-registro").validate({
            errorClass: 'is-invalid text-danger',
            errorElement: 'span',
            // Specify validation rules
            rules: {

                'Capacidad': {
                    required: true,
                    min: 1,
                    max: 6,
                },
                'IntervaloMin': {
                    required: true,
                    min: 5,
                    max: 60,
                },
                'HoraInicio': {
                    required: true,
                },
                'HoraFin': {
                    required: true,
                    checkHoraFin: true
                },

            },
            // Specify validation error messages
            messages: {
                'Capacidad': {
                    required: "El campo es requerido.",
                    min: "El mínimo permitido es 1.",
                    max: "El máximo permitido es 6.",
                },
                'IntervaloMin': {
                    required: "El campo es requerido.",
                    min: "El mínimo permitido es 5 min.",
                    max: "El máximo permitido es 60 min.",
                },
                'HoraInicio': {
                    required: "El campo es requerido.",
                },
                'HoraFin': {
                    required: "El campo es requerido.",
                },


            },
            errorPlacement: function (error, element) {
                if (element.attr('name') == 'IntervaloMin') {
                    error.appendTo(element.parent().parent());
                } else {
                    error.insertAfter(element);
                }
                // }
            },
            // Make sure the form is submitted to the destination defined
            // in the "action" attribute of the form when valid
            submitHandler: function (form) {
                form.submit();
            }
        });
    }
    return {
        init: () => {
            init();
        },
        abrirModal: (id) => {
            abrirModal(id);
        }
    }
}()

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
    EdicionHorario.init();
});