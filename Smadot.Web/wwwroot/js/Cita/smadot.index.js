var datatable;

var KTDatatableRemoteAjax = function () {
    const today = moment();
    var f1 = today.format('DD/MM/YYYY');
    var f2 = today.format('DD/MM/YYYY');


    var init = function () {
        datatable = $('#kt_datatable').DataTable({
            language: {
                "decimal": "",
                "emptyTable": "No hay información",
                "info": "Mostrando _END_ de _TOTAL_ entradas",
                "infoEmpty": "Mostrando 0 de 0 entradas",
                "infoFiltered": "",
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
            order: [8, 'desc'],
            ajax: {
                url: `/Citas/Consulta?fecha1=${f1}&&fecha2=${f2}`,
                type: 'POST'
            },
            columnDefs: [{
                className: "dtr-control",
                "defaultContent": "-",
            }, { responsivePriority: 1, targets: -1 }, { responsivePriority: 2, targets: -2 }],
            columns: [
                //{ data: 'id', name: 'Id', title: '#' },
                { data: 'folio', name: 'Folio', title: 'Folio Cita', width: 120 },
                { data: 'folioAsignado', name: 'Folio', title: 'Folio Forma Valorada' },
                { data: 'placa', name: 'Placa', title: 'Placa' },
                { data: 'marca', name: 'Marca', title: 'Marca' },
                { data: 'modelo', name: 'Modelo', title: 'Modelo' },
                { data: 'nombreCentro', name: 'NombreCentro', title: 'Centro de Verificación' },
                { data: 'progreso', name: 'Progreso', title: 'Progreso Verificación', orderable: false, },
                // { data: 'cancelada', name: 'Cancelada', title: 'Cancelada', orderable: false,},
                { data: 'resultadoStr', name: 'RESULTADO', title: 'Resultado' },
                { data: 'fechaStr', name: 'Fecha', title: 'Fecha' },
                { data: 'serie', name: 'Serie', title: 'VIN' },
                { data: 'ingresoManualStr', name: 'IngresoManual', title: 'Tipo Ingreso' },
                {
                    data: 'cancelada', name: 'Cancelada', title: 'Estatus',
                    render: function (data, type, row) {

                        if (row.cancelada) {
                            return '<span>' + 'Cancelada</span>'
                        }
                        else {
                            return '<span>' + 'Activa</span>'
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
        $("#kt_daterangepicker_1").daterangepicker({
            showDropdowns: true,
            minYear: 2019,
            maxYear: parseInt(moment().format("YYYY"), 12),
            autoApply: true,
            autoUpdateInput: true,
            startDate: moment(),
            endDate: moment(),
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
        $("#limpiarFiltros").click(function () {
            $(`input[data-kt-filter="search"]`).val(null);
            f1 = today.format('DD/MM/YYYY');
            f2 = today.format('DD/MM/YYYY');
            $("#kt_daterangepicker_1").data('daterangepicker').setStartDate(today);
            $("#kt_daterangepicker_1").data('daterangepicker').setEndDate(today);
            recargar();
        });
        $("#kt_daterangepicker_1_open").click(function () {
            $("#kt_daterangepicker_1").data('daterangepicker').toggle();
        });
        $("#kt_daterangepicker_1").on('apply.daterangepicker', function (ev, picker) {
            f1 = picker.startDate.format('DD/MM/YYYY');
            f2 = picker.endDate.format('DD/MM/YYYY');
            $(this).val(f1 + ' - ' + f2);
            recargar();
        });
        $('#Atendida').select2({});
        $("#Atendida").change(function () {
            console.log(this.value)
            recargar()
        });

        $('thead tr').addClass('fw-bold fs-6 text-gray-800');
        $(document).on('click', "#btn-reimprimir", function (e) {
            e.preventDefault();
            if ($(this).data()) {
                folioCancelado.init($(this).data())
            }
        });
        $(document).on('click', '.btnImprimir', function () {
            let id = $(this).data("id");
            enviar(id);
        })
        $(document).on('click', '.btnReiniciarCita', function (e) {
            e.preventDefault();
            ReiniciarCita.reiniciar(this.dataset);
        });
    };
    var exportButtons = () => {
        $(document).on('click', '.btnExport', function (e) {
            e.preventDefault();
            let dtvalue = $("#kt_daterangepicker_1").val() ?? "";
            let fechas = dtvalue.split(' - ');
            let f1 = fechas.length > 1 ? fechas[0] : '';
            let f2 = fechas.length > 1 ? fechas[1] : '';


            var isPDF = this.dataset.ktExport === 'pdf' ? true : false;
            var search = datatable.ajax.params().search.value;
            var sortColumnDirection = datatable.ajax.params().order[0].dir;
            var sortcolumn = datatable.ajax.params().columns[datatable.ajax.params().order[0].column].name;
            var cicloExport = $("#CicloVerificacion option:selected").val();
            $.ajax({
                cache: false,
                type: 'GET',
                url: `/Citas/GetPDF?search=${search}&sortColumnDirection=${sortColumnDirection}&sortColumn=${sortcolumn}&PDF=${isPDF}&fecha1=${f1}&&fecha2=${f2}&Atendida=${$("#Atendida").val() ? Number($("#Atendida").val()) !== 1 : null}`,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    window.open('/ConsultaStockDVRF/' + (isPDF ? 'DescargarReportePDF' : 'DescargarReporte'), '_blank');
                    return;
                },
                error: function (res) {
                    toastr.error("Ocurrió un error al generar el archivo", "SMADSOT");
                }
            });
        });
    }
    var recargar = function () {
        datatable.ajax.url(`/Citas/Consulta?fecha1=${f1}&&fecha2=${f2}&&atendida=${$("#Atendida").val() ? Number($("#Atendida").val()) !== 1 : null}`);
        datatable.ajax.reload();
    }

    //var recargaCita = function () {
    //    datatable.ajax.url(`/Citas/Consulta?Atendida=${$("#Atendida").val()}`);
    //    datatable.ajax.reload();
    //}

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
    reDrawDatatable = (e) => {
        datatable.ajax.url(`/Citas/Consulta`);
        datatable.search(e).draw();
    }

    var enviar = (id) => {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Citas/VolverImprimir?id=" + id,
            success: function (result) {
                if (result.error) {
                    toastr.error("", "Error");
                } else {
                    if (!$("#Id").length > 0) {
                        $("#save").hide();
                    }
                    toastr.success("Acción exitosa. Revise pantalla de Impresión.", "SMADSOT");
                    recargar();
                    return;
                }

                return;
            },
            error: function (res) {
                toastr.error(res.errorDescripction);
            }
        });
    }

    return {
        init: function () {
            init();
            handleSearchDatatable()
            exportButtons()
        },
        recargar: function () {
            recargar();
        }
    };
}();
var ReiniciarCita = function () {
    var reiniciar = function (data) {
        Swal.fire({
            title: '¿Seguro que desea continuar?',
            text: "Se eliminará toda la información de captura y los resultados de la verificación para volver a realizar el proceso de captura.",
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
                    type: 'POST',
                    data: {
                        req: data
                    },
                    url: '/Citas/ReiniciarCita',
                    success: function (result) {
                        if (!result.isSuccessFully) {
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
        reiniciar: function (data) {
            reiniciar(data);
        }
    }

}();
jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});