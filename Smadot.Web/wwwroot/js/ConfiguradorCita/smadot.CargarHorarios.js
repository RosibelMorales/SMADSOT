"use strict"
var datatable, data = [];

var KTDatatableRemoteAjax = function () {
    var timePicker, timePicker1, data = [];
    var currentDate = moment().add('days', 1);
    var maxDate = moment().add('days', 15);
    var init = function () {
        datatable = $('#kt_datatable').DataTable({
            language: {
                "decimal": "",
                "emptyTable": "No se han cargado horarios.",
                "info": "Mostrando _END_ de _TOTAL_ entradas",
                "infoEmpty": "Mostrando 0 de 0 entradas",
                "infoFiltered": "(Filtrado de _MAX_ total entradas)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Mostrar _MENU_ Entradas",
                "loadingRecords": "Cargando...",
                "processing": "Procesando...",
                "search": "Buscar:",
                "zeroRecords": "No se han cargado horarios",
                "paginate": {
                    "first": "Primero",
                    "last": "Último",
                    "next": "&raquo;",
                    "previous": "&laquo;"
                }
            },
            dom: 'ilrtp',
            scrollY: "600px",
            responsive: true,
            paging: false,
            searching: true,
            lengthMenu: [10, 20, 50, 100],
            filter: true,
            ordering: true,
            order: [1, 'asc'],
            data: data,
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            },
            { responsivePriority: 1, targets: -1 }, { responsivePriority: 2, targets: -2 }
            ],
            columns: [
                //{ data: 'id', name: 'Id', title: '#' },
                {
                    data: 'habilitado', name: 'habilitado', title: 'Habilitado', render: function (data, type, row) {
                        if (type === 'display') {
                            return `<div class="form-check py-2 m-0">
                                        <input class="form-check-input form-check-input-sm" ${row.habilitado ? "checked" : ""} type="checkbox" name="habilitado[]" />
                                    </div>`;
                        }
                    }
                },
                { data: 'fecha', name: 'fecha', title: 'Fecha' },
                {
                    title: 'Día', orderable: false, render: function (data, type, row) {
                        if (type === 'display') {
                            const fechaString = row.fecha;

                            const fecha = moment(fechaString, "DD/MM/YYYY", 'es');

                            const nombreDiaCompleto = fecha.format("dddd").charAt(0).toUpperCase() + fecha.format("dddd").slice(1);

                            return nombreDiaCompleto;
                        }
                    }
                },
                {
                    data: 'horaInicio', name: 'horaInicio', title: 'Hora de Inicio', render: function (data, type, row) {
                        if (type === 'display') {
                            return `<input class="form-control form-control-sm table-timepicker" name="horaInicio[]" placeholder="00:00" value="${row.horaInicio}" />`;
                        }
                    }
                },
                {
                    data: 'horaFin', name: 'horaFin', title: 'Hora de Fin', render: function (data, type, row) {
                        if (type === 'display') {
                            return `<input class="form-control form-control-sm table-timepicker" name="horaFin[]" placeholder="00:00" value="${row.horaFin}" />`;
                        }
                    }
                },
                {
                    data: 'capacidad', name: 'capacidad', title: 'Capacidad', render: function (data, type, row) {
                        if (type === 'display') {
                            return `<input type="number" class="form-control form-control-sm" name="capacidad[]" placeholder="0" value="${row.capacidad}" />`;
                        }
                    }
                },
                {
                    data: 'intervaloMin', name: 'intervaloMin', title: 'Duración Citas', render: function (data, type, row) {
                        if (type === 'display') {
                            return `<div class="input-group input-group-sm">
                                        <input type="number" class="form-control form-control-sm" name="intervaloMin[]" placeholder="0" aria-describedby="inputGroup-sizing-sm" value="${row.intervaloMin}" />
                                        <span class="input-group-text">min.</span>
                                    </div>`;
                        }
                    }
                },
            ], createdRow: function (row, data, dataIndex) {
                // Obtener el checkbox en esta fila
                const isChecked = row.querySelector('input[type="checkbox"][name="habilitado[]"]').checked;

                // Agregar evento change al checkbox

                if (!isChecked) {
                    row.style.opacity = '0.5';

                } else {
                    row.style.opacity = '1';
                }
                let inputs = row.querySelectorAll('input:not([type="checkbox"])');
                inputs.forEach((inputElement) => {
                    inputElement.disabled = !isChecked;
                });
            },
        });
        timePicker = flatpickr("#HoraInicio", {
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
        });
        timePicker1 = flatpickr("#HoraFin", {
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
        });
        $("#Fecha").daterangepicker({
            showDropdowns: true,
            minYear: 2019,
            maxYear: parseInt(moment().format("YYYY"), 12),
            minDate: currentDate,
            autoApply: true,
            autoUpdateInput: true,
            startDate: currentDate,
            endDate: maxDate,
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S",
                customRangeLabel: "Personalizado",
            }, ranges: {
                'Hoy': [moment(), moment()],
                'Ayer': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Últimos 7 días': [moment().subtract(6, 'days'), moment()],
                'Últimos 30 días': [moment().subtract(29, 'days'), moment()],
                'Este mes': [moment().startOf('month'), moment().endOf('month')],
                'Último mes': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            }
        });
        $('input[name="option2"]').change(function () {
            reset();
            // Obtener el valor del radio button seleccionado
            var selectedValue = $(this).val();

            // Realizar acciones diferentes dependiendo del valor seleccionado
            if (selectedValue === "1") {
                $('#container-meses').show();
                $('#container-fechas').hide();
            } else if (selectedValue === "2") {
                $('#container-meses').hide();
                $('#container-fechas').show();
            }
        });
        UpdateHours();
        $(document).on('change', ' input[name="capacidad[]"], input[name="intervaloMin[]"], input[name="habilitado[]"]', function () {

            dtProcessData($(this))
        });
        $(document).on('draw.dt', '#kt_datatable', function () {
            listenersTable();
        });
        listenersTable();
    };

    const dtProcessData = ($input) => { // Utilizar $input en lugar de $(this)
        var rowIndex = datatable.cell($input.closest('td')).index().row; // Obtener el índice de la fila actual

        // Obtener los nuevos valores de los inputs
        var habilitado = $input.closest('tr').find('input[name="habilitado[]"]').is(":checked");
        var horaInicioVal = $input.closest('tr').find('input[name="horaInicio[]"]').val();
        var horaFinVal = $input.closest('tr').find('input[name="horaFin[]"]').val();
        var capacidadVal = $input.closest('tr').find('input[name="capacidad[]"]').val();
        var intervaloMinVal = $input.closest('tr').find('input[name="intervaloMin[]"]').val();

        // Actualizar los valores en las celdas de la tabla
        datatable.cell(rowIndex, 0).data(habilitado);
        datatable.cell(rowIndex, 3).data(horaInicioVal);
        datatable.cell(rowIndex, 4).data(horaFinVal);
        datatable.cell(rowIndex, 5).data(capacidadVal);
        datatable.cell(rowIndex, 6).data(intervaloMinVal);
        let trData = document.querySelectorAll('#form-dt-horarios #kt_datatable tbody tr');
        if (trData)
            validator_dt(trData);

        listenersTable();

    }


    const listenersTable = () => {
        $(".table-timepicker").flatpickr({
            enableTime: true,
            noCalendar: true,
            dateFormat: "H:i",
            onClose: function (selectedDates, dateStr, instance) {
                dtProcessData($(instance.element));

            },
        });
    }
    const reset = () => {
        datatable.clear().draw();
    }
    const getData = () => {
        return datatable.rows().data();
    }

    const UpdateHours = () => {
        // Actualizar el valor del TimePicker
        timePicker.setDate("08:00", true, "H:i");
        timePicker1.setDate("20:00", true, "H:i");
    }


    return {
        init: function () {
            init();
        },
        reset: () => {
            reset();
        },
        getData: () => {
            return getData();
        }
    };
}();
const validator_dt = (trElements) => {
    const rows = trElements;

    const required = (el) => {
        return el.value.trim() !== '';
    };

    const checkHoraFin = (nameCompare) => {
        return (el) => {
            const horaInicio = el.value;
            const horaFin = el.parentNode.parentNode.querySelector(`[name="${nameCompare}"]`).value;

            // Realizar la comparación con Moment.js
            var fechaInicio = new Date('01/01/2000 ' + horaInicio);
            var fechaFin = new Date('01/01/2000 ' + horaFin);

            // Comparar las fechas
            return fechaFin > fechaInicio;
        };
    };
    const spanMessage = (name, error) => {
        return `<span id="${name}-error" class="is-invalid text-danger">
                   ${error}</span >
`
    }
    const integer = function (el) {
        const value = parseInt(el.value, 10);
        const min = parseInt(this.min, 10);
        const max = parseInt(this.max, 10);
        return !isNaN(value) && Number.isInteger(value) && value >= min && value <= max;
    };
    const validator = [
        {
            name: "horaInicio[]",
            validations: [
                {
                    name: "required", message: "El campo es requerido.", process: required
                },
                { name: "timeCompare", message: "La hora de inicio no puede ser mayor que la hora de fin.", process: checkHoraFin("horaFin[]") },
            ]
        },
        {
            name: "horaFin[]",
            validations: [
                {
                    name: "required", message: "El campo es requerido.", process: required
                }
            ]
        },
        {
            name: "intervaloMin[]",
            validations: [
                {
                    name: "required", message: "El campo es requerido.", process: required
                },
                {
                    name: "integer", min: 5, max: 60, message: "El mínimo permitido es 5 min, y el máximo permitido es 60 min.", process: integer
                }
            ]
        },
        {
            name: "capacidad[]",
            validations: [
                {
                    name: "required", message: "El campo es requerido.", process: required
                },
                {
                    name: "integer", min: 1, max: 6, message: "El mínimo permitido es 1 cita, y el máximo permitido es de 6.", process: integer
                }
            ]
        }
    ];

    // Recorrer el arreglo de validaciones y aplicar las reglas
    let formValid = true;
    rows.forEach((row) => {
        // Obtener el checkbox en esta fila
        const isChecked = row.querySelector('input[type="checkbox"][name="habilitado[]"]').checked;

        // Agregar evento change al checkbox

        if (!isChecked) {
            row.style.opacity = '0.5';

        } else {
            row.style.opacity = '1';
        }
        let inputs = row.querySelectorAll('input:not([type="checkbox"])');
        inputs.forEach((inputElement) => {
            inputElement.disabled = !isChecked;
        });
        if (isChecked) {
            validator.forEach((field) => {
                // Utilizar querySelectorAll para obtener los elementos por el atributo name en cada fila
                let input = row.querySelector(`[name="${field.name}"]`);
                if (input) {

                    field.validations.forEach((validation) => {
                        const isValid = validation.process(input);
                        if (!isValid) {
                            formValid = false;
                            if (!input.classList.contains("is-invalid")) {
                                input.classList.add("is-invalid", "text-danger");
                            }
                            let td = input.closest('td');
                            let spanError = td?.querySelector('span.is-invalid')
                            if (td && !spanError) {
                                let content = spanMessage(input.name, validation.message);
                                td.innerHTML += content;
                            } else if (spanError) {
                                spanError.innerHTML = validation.message;
                            }
                        } else {
                            if (input.classList.contains("is-invalid")) {
                                input.classList.remove("is-invalid", "text-danger");
                            }
                            let td = input.closest('td');
                            let spanError = td.querySelector('span.is-invalid')
                            if (spanError) {
                                td.removeChild(spanError);
                            }
                        }
                    });
                    if (input.type === 'checkbox') {
                        console.log('lo haré')
                        row.style.opacity = input.checked ? "1" : "0.5"
                    }
                }

            });
        }
    });
    return formValid;
}
var formHorarios = function () {

    const init = () => {
        validations();

        $("#btn-borrar").click(function (e) {
            e.preventDefault();
            $("#btn-cargar").removeAttr("disabled");
            $("#btn-borrar").attr("disabled", "disabled");
            KTDatatableRemoteAjax.reset();
        })
        $("#btn-guardar").click(function (e) {
            e.preventDefault();
            let trData = document.querySelectorAll('#form-dt-horarios #kt_datatable tbody tr');
            let data = KTDatatableRemoteAjax.getData()?.toArray();
            if (data.length > 0 && validator_dt(trData)) {
                guardar(data);
            } else if (data?.length === 0) {
                toastr.error('Aún no se han cargado horarios', 'SMADSOT')
            }
        });
        $("#btn-cargar").click(function (e) {
            e.preventDefault();
            var form = $('#formHorarios');
            if (form.valid()) {
                $("#btn-borrar").removeAttr("disabled");
                $("#btn-cargar").attr("disabled", "disabled");
                cargarHorarios();
            }
        });
    }
    const cargarHorarios = () => {
        let fechas = $('#Fecha').val()?.split(' - ');
        let fechaInicio;
        let fechaFin;
        let selectedVal = $('input[name="option2"]:checked').val();
        if (selectedVal === "1") {
            fechaInicio = moment(fechas[0], "DD/MM/YYYY");
            fechaFin = moment(fechas[1], "DD/MM/YYYY");
        } else if (selectedVal === "2") {
            const primerDia = moment().month(Number($('#mesSelect').val()) - 1).startOf('month');
            fechaInicio = moment.max(primerDia, currentDate);
            fechaFin = moment().month(Number($('#mesSelect').val()) - 1).endOf('month');
        }

        generateDates(fechaInicio.format("YYYY-MM-DD"), fechaFin.format("YYYY-MM-DD"));
    }
    const generateDates = (startDate, endDate) => {
        let date = moment(startDate);
        data = [];
        while (date.isSameOrBefore(endDate, 'day')) {
            let registro = {
                habilitado: date.day() !== 0,
                fecha: date.format('DD/MM/YYYY'),
                horaInicio: $('#HoraInicio').val(),
                horaFin: $('#HoraFin').val(),
                capacidad: $('#Capacidad').val(),
                intervaloMin: $('#Intervalo').val()
            };
            data.push(registro);
            date.add(1, 'day');
        }
        // Limpiar los datos actuales
        datatable.clear().draw();

        // Agregar los nuevos datos
        datatable.rows.add(data);

        // Dibujar el DataTable con los nuevos datos
        datatable.draw();

    }
    const validations = () => {
        $.validator.addMethod('regexp', function (value, element, param) {
            return this.optional(element) || value.match(param);
        },
            'This value doesn\'t match the acceptable pattern.');
        $("#formHorarios").validate({
            errorClass: 'is-invalid text-danger',
            errorElement: 'span',
            // Specify validation rules
            rules: {

                Capacidad: {
                    required: true,
                    min: 1,
                    max: 6,
                },
                intervaloMin: {
                    required: true,
                    min: 5,
                    max: 60,
                },

            },
            // Specify validation error messages
            messages: {
                Capacidad: {
                    required: "El campo es requerido.",
                    min: "El mínimo permitido es 1.",
                    max: "El máximo permitido es 6.",
                },
                intervaloMin: {
                    required: "El campo es requerido.",
                    min: "El mínimo permitido es 5 min.",
                    max: "El máximo permitido es 60 min.",
                },


            },
            errorPlacement: function (error, element) {
                if (element.attr('name') == 'intervaloMin') {
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
    const guardar = (dataHorarios) => {
        dataHorarios = dataHorarios.map((obj) => {
            return { ...obj, idCvv: $('#IdCVV').val() };
        });

        $.ajax({
            cache: false,
            type: 'POST',
            url: "/DirectorioCentrosVerificacion/ConfiguradorCita",
            data: {
                request: dataHorarios
            },
            success: function (result) {
                console.log(result);
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "Error");
                    return;
                }
                location.replace(location.origin + `/DirectorioCentrosVerificacion/ConfiguradorCitas/${$('#IdCVV').val()}`);
                toastr.success("La información se guardo exitosamente", "Error");

            },
            error: function (res) {
                toastr.error("Servicio no disponible", "Error");
            }
        });
    }
    return {
        init: () => {
            init();
        }
    }
}()
jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
    formHorarios.init();
});