"use strict"
var main = function () {
    let f1, f2, datatable;
    const init = () => {
        listeners();

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
            order: [[3, 'asc']],
            scrollY: 350,
            ajax: {
                url: '/EstadisiticaUsoFormaValorada/Consulta',
                type: 'POST',
                data: function (d) {
                    d.tipoHolograma = $('#hologramaGrid')?.val() ?? null;
                    d.modeloMax = $('#ModeloMin')?.val() ?? null;
                    d.modeloMax = $('#ModeloMax')?.val() ?? null;
                    d.causaRechazo = $('#rechazoGrid')?.val() ?? null;
                    d.idVerificentro = $("#IdVerificentro")?.val() ?? null;
                    d.idMotivoVerificacion = $("#MotivosVerificacion")?.val() ?? null;
                    d.idCatTipoServicio = $("#TiposServicio")?.val() ?? null;
                    d.fechaInicial = $('#grid_date_range').data('daterangepicker').startDate.format('YYYY-MM-DD');
                    d.fechaFinal = $('#grid_date_range').data('daterangepicker').endDate.format('YYYY-MM-DD');
                    d.esGrid = true
                }
            },
            columnDefs: [{
                className: "dtr-control",
                "defaultContent": "-",

            }, { responsivePriority: 13, targets: -1 }],
            columns: [
                { data: 'id', name: 'Id', title: 'Id', searchable: false, },
                { data: 'combustible', name: 'Combustible', title: 'Combustible', searchable: false, },
                {
                    data: null, title: 'Pruebas',
                    render: function (data, type, row) {
                        let codeColor1 = '';
                        let icon1 = '';
                        let codeColor2 = '';
                        let icon2 = '';
                        let codeColor3 = '';
                        let icon3 = '';
                        if (row.pruebaObd == true) {
                            codeColor1 = '50cd89'
                            icon1 = 'bi bi-check-circle'
                        } else {
                            codeColor1 = 'CCC'
                            icon1 = 'bi bi-slash-circle'
                        }
                        if (row.pruebaEmisiones == true) {
                            codeColor2 = '50cd89'
                            icon2 = 'bi bi-check-circle'
                        } else {
                            codeColor2 = 'CCC'
                            icon2 = 'bi bi-slash-circle'
                        }
                        if (row.pruebaOpacidad == true) {
                            codeColor3 = '50cd89'
                            icon3 = 'bi bi-check-circle'
                        } else {
                            codeColor3 = 'CCC'
                            icon3 = 'bi bi-slash-circle'
                        }

                        return `<div style="min-width: 125px;">
                            <span class="badge py-3 px-4 fs-7" style="background-color:#${codeColor1}" title="Prueba Obd" >
                                <i class="${icon1}" style="color:white"></i>
                            </span>
                            <span class="badge py-3 px-4 fs-7" style="background-color:#${codeColor2}" title="Prueba emisiones" >
                                <i class="${icon2}" style="color:white"></i>
                            </span>
                            <span class="badge py-3 px-4 fs-7" style="background-color:#${codeColor3}" title="Prueba Opacidad" >
                                <i class="${icon3}" style="color:white"></i>
                            </span>
                        </div>`;
                    }
                },
                {
                    data: 'sinMulta', name: 'sinMulta', title: 'Sin Multa',
                    render: function (data, type, row) {
                        let codeColor1 = '';
                        let icon1 = '';
                        let title = '';
                        if (!row.sinMulta) {
                            codeColor1 = 'CCC'
                            icon1 = 'bi bi-circle'
                            title = 'Sin Multa';
                        } else {
                            codeColor1 = 'B8123D'
                            icon1 = 'bi bi-x-circle'
                            title = 'Multa';
                        }
                        return `<span class="badge py-3 px-4 fs-7" style="background-color:#${codeColor1}" title="${title}" >
                                    <i class="${icon1}" style="color:white"></i>
                                </span>`;
                    }
                },
                { data: 'placa', name: 'Placa', title: 'Placa', searchable: false, },
                { data: 'marca', name: 'Marca', title: 'Marca', searchable: false, },
                { data: 'subMarca', name: 'SubMarca', title: 'Submarca', searchable: false, },
                { data: 'causaRechazo', name: 'C_RECHAZO', title: 'Causa de Rechazo', searchable: false, },
                { data: 'aprobado', name: 'RESULTADO', title: 'Estatus Verificación', searchable: false, },
                { data: 'tipoServicio', name: 'IdCatTipoServicio', title: 'Tipo Servicio', sort: false, searchable: false, },
                { data: 'tipoCertificadoString', name: 'IdTipoCertificado', title: 'Tipo Certificado', searchable: false, },
                { data: 'claveTramite', name: 'ClaveTramite', title: 'Clave Trámite', orderable: false, searchable: false, },
                // {
                //     data: 'verificacionActiva', title: 'Verificación activa', searchable: false,
                //     render: function (data, type, row) {
                //         var mensaje = "";
                //         if (data == true) {
                //             mensaje = `<span class="" title="Verificación Activa">Si</span>`;
                //         } else {
                //             mensaje = `<span class="" title="Verificación Activa">No</span>`;
                //         }
                //         return mensaje;
                //     }
                // },
                { data: 'intervaloMinutos', name: 'intervaloMinutos', title: 'Tiempo duración prueba', searchable: false, },
                { data: 'noIntentos', name: 'noIntentos', title: 'No. Intentos', searchable: false, },
                //{ data: null, title: 'Canje de placa', searchable: false, },
                { data: 'motivoVerificacion', name: 'IdMotivoVerificacion', title: 'Motivo Verificación', searchable: false, },
                {
                    data: 'cambioPlacas', name: 'cambioPlacas', title: 'Canje de placa',
                    render: function (data, type, row) {
                        var mensaje = "";
                        if (data == true) {
                            mensaje = `<span class="" title="Canje placas">Si</span>`;
                        } else {
                            mensaje = `<span class="" title="Canje placas">No</span>`;
                        }
                        return mensaje;
                    }
                },

                {
                    target: -2,
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

    }
    var exportButtons = () => {
        $(document).on('click', '#btnExport', function (e) {
            e.preventDefault();
            $.ajax({
                cache: false,
                type: 'GET',
                data: GetDataFilters(false),
                url: 'EstadisiticaUsoFormaValorada/CrearReporte',
                success: function (result) {
                    if (result.error) {
                        var text = result.errorDescription.split("|");
                        toastr.error(text[0], "");
                    } else {
                        $("#linkDownloadReport").get(0).click();
                    }

                    return;
                },
                error: function (res) {
                    toastr.error("Error no controlado", "");
                }
            });
        });
    }
    const GetDataFilters = (esGrid) => {
        return {
            tipoHolograma: $('#hologramaGrid')?.val() ?? null,
            modeloMax: $('#ModeloMin')?.val() ?? null,
            modeloMax: $('#ModeloMax')?.val() ?? null,
            causaRechazo: $('#rechazoGrid')?.val() ?? null,
            idVerificentro: $("#IdVerificentro")?.val() ?? null,
            idMotivoVerificacion: $("#MotivosVerificacion")?.val() ?? null,
            idCatTipoServicio: $("#TiposServicio")?.val() ?? null,
            fechaInicial: $('#grid_date_range').data('daterangepicker').startDate.format('YYYY-MM-DD'),
            fechaFinal: $('#grid_date_range').data('daterangepicker').endDate.format('YYYY-MM-DD'),
            esGrid: esGrid
        }
    }
    var recargarDt = function () {
        datatable.ajax.reload();
    }
    const listeners = () => {
        $("#btn-buscar").on("click", function () {
            recargarDt();
        })
        $("#kt_daterangepicker_1").daterangepicker({
            showDropdowns: true,
            minYear: 2019,
            maxYear: parseInt(moment().format("YYYY"), 12),
            autoApply: true,
            autoUpdateInput: true,
            startDate: moment().subtract(7, 'days'),
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
        $("#kt_daterangepicker_1_open").click(function () {
            $("#kt_daterangepicker_1").data('daterangepicker').toggle();
        });

        $("#kt_daterangepicker_1").on('apply.daterangepicker', function (ev, picker) {
            f1 = picker.startDate.format('DD/MM/YYYY');
            f2 = picker.endDate.format('DD/MM/YYYY');
            $(this).val(f1 + ' - ' + f2);
            recargar();
        });
        $("#grid_date_range").daterangepicker({
            showDropdowns: true,
            minYear: 2019,
            maxYear: parseInt(moment().format("YYYY"), 12),
            autoApply: true,
            autoUpdateInput: true,
            startDate: moment().subtract(1, 'days'),
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
        $("#grid_date_range_open").click(function () {
            $("#grid_date_range").data('daterangepicker').toggle();
        });
        $("#IdCentroAtencion").select2({
            placeholder: 'TODOS',
            allowClear: true
        })
        $("#IdVerificentro").select2({
            placeholder: 'TODOS',
            allowClear: true
        })
        $("#hologramaGrid").select2({
            placeholder: 'TODOS',
            allowClear: true
        })
        $("#rechazoGrid").select2({
            placeholder: 'TODOS',
            allowClear: true
        })
        $("#TiposServicio").select2({
            placeholder: 'TODOS',
            allowClear: true
        })
        $("#MotivosVerificacion").select2({
            placeholder: 'TODOS',
            allowClear: true
        })
        $('#hologramaGrid').val(null).trigger('change');
        $('#rechazoGrid').val(null).trigger('change');
        $('#TiposServicio').val(null).trigger('change');
        $('#MotivosVerificacion').val(null).trigger('change');

        $("#IdCentroAtencion").on('change', function () {
            recargar();
        });
        let dtvalue = $("#kt_daterangepicker_1").val() ?? "";
        let fechas = dtvalue.split(' - ');
        f1 = fechas.length > 1 ? fechas[0] : '';
        f2 = fechas.length > 1 ? fechas[1] : '';
        recargar();
    }
    const recargar = () => {
        let data = GetData();
        indicadores.init(data);
        foliosContent.init(data);
    }
    var GetData = () => {
        return {
            fechaInicial: moment(f1, 'DD/MM/YYYY').format('YYYY-MM-DD'),
            fechaFinal: moment(f2, 'DD/MM/YYYY').format('YYYY-MM-DD'),
            idVerificentro: $("#IdCentroAtencion").val() !== undefined ? $("#IdCentroAtencion").val() : null
        }
    }

    return {
        init: () => {
            init();
            exportButtons();
        },
    }
}()
var Charts = function () {
    let f1, f2
    var chart, piechart, gaugechart;
    const init = () => {
        filters();
        reloadBarChart();
        reloadChartPie();
        reloadChartGauge();

    }
    function updateChart(data) {

        var root = am5.Root.new("barchart_cvv");


        // Set themes
        // https://www.amcharts.com/docs/v5/concepts/themes/
        root.setThemes([
            am5themes_Animated.new(root)
        ]);



        // Create chart
        // https://www.amcharts.com/docs/v5/charts/xy-chart/
        chart = root.container.children.push(am5xy.XYChart.new(root, {
            panX: false,
            panY: false,
            wheelX: "panX",
            wheelY: "zoomX",
            layout: root.verticalLayout
        }));


        // Add scrollbar
        // https://www.amcharts.com/docs/v5/charts/xy-chart/scrollbars/
        chart.set("scrollbarX", am5.Scrollbar.new(root, {
            orientation: "horizontal"
        }));
        // Add legend
        // https://www.amcharts.com/docs/v5/charts/xy-chart/legend-xy-series/
        var legend = chart.children.push(am5.Legend.new(root, {
            centerX: am5.p50,
            x: am5.p50
        }));


        // Create axes
        // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
        var xRenderer = am5xy.AxisRendererX.new(root, {
            rotation: 90 // Rotar 45 grados
        });

        // Create axes
        // https://www.amcharts.com/docs/v5/charts/xy-chart/axes/
        // var xRenderer = am5xy.AxisRendererX.new(root, {});
        var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
            categoryField: "clave",
            renderer: xRenderer,
            tooltip: am5.Tooltip.new(root, {})
        }));

        // var xAxis = chart.xAxes.push(am5xy.CategoryAxis.new(root, {
        //     categoryField: "clave",
        //     renderer: xRenderer,
        //     tooltip: am5.Tooltip.new(root, {})
        // }));


        xAxis.data.setAll(data);

        var yAxis = chart.yAxes.push(am5xy.ValueAxis.new(root, {
            min: 0,
            renderer: am5xy.AxisRendererY.new(root, {
                strokeOpacity: 0.1
            })
        }));


        // Add series
        // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
        function makeSeries(name, fieldName) {
            var series = chart.series.push(am5xy.ColumnSeries.new(root, {
                name: name,
                stacked: true,
                xAxis: xAxis,
                yAxis: yAxis,
                valueYField: fieldName,
                categoryXField: "clave"
            }));

            series.columns.template.setAll({
                tooltipText: "{name}, {categoryX}: {valueY}",
                tooltipY: am5.percent(10)
            });
            series.data.setAll(data);

            // Make stuff animate on load
            // https://www.amcharts.com/docs/v5/concepts/animations/
            series.appear();

            series.bullets.push(function () {
                return am5.Bullet.new(root, {
                    sprite: am5.Label.new(root, {
                        text: "{valueY}",
                        fill: root.interfaceColors.get("alternativeText"),
                        centerY: am5.p50,
                        centerX: am5.p50,
                        populateText: true
                    })
                });
            });

            legend.data.push(series);
        }

        makeSeries("Aprobadas", "aprobadas");
        makeSeries("Rechazadas", "rechazadas");
        makeSeries("No Procesadas", "noProcesadas");
        makeSeries("Total", "todas");

        // chart.get("colors").set("colors", [
        //     am5.color(0x095256),
        //     am5.color(0x087f8c),
        //     am5.color(0x5aaa95),
        //     am5.color(0x86a873),
        //     am5.color(0xbb9f06)
        // ]);

        // Make stuff animate on load
        // https://www.amcharts.com/docs/v5/concepts/animations/
        chart.appear(1000, 100);
        let exporting = am5plugins_exporting.Exporting.new(root, {
            menu: am5plugins_exporting.ExportingMenu.new(root, {}),
            dataSource: data,
            filePrefix: "verificaciones_centros",
            title: `Pruebas Realizas por CVV ${f1} - ${f2}`,
            dataFields: {
                clave: "Centro",
                rechazadas: "Rechazadas",
                aprobadas: "Aprobadas",
                noProcesadas: "No Procesadas",
                todas: "Total"
            },
            pdfOptions: {
                align: 'center',
                includeData: true,
                addURL: false,
                //font: am5fonts_notosans_kr
            },
            pngOptions: {
                disabled: true
            },
            jpgOptions: {
                disabled: true
            },
            printOptions: {
                disabled: true
            },
            //xlsxOptions: {
            //    disabled: false
            //}
        });

    }
    const UpdateChartPie = (data) => {
        // Create root element
        // https://www.amcharts.com/docs/v5/getting-started/#Root_element
        var root = am5.Root.new("kt_amcharts_3");

        //var rootP = root;
        // Set themes
        // https://www.amcharts.com/docs/v5/concepts/themes/
        root.setThemes([
            am5themes_Animated.new(root)
        ]);


        // Create chart
        // https://www.amcharts.com/docs/v5/charts/xy-chart/
        piechart = root.container.children.push(am5percent.PieChart.new(root, {
            layout: root.verticalLayout
        }));

        // Create series
        // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Series
        var series = piechart.series.push(am5percent.PieSeries.new(root, {
            alignLabels: true,
            calculateAggregates: true,
            valueField: "value",
            categoryField: "category"
        }));

        series.slices.template.setAll({
            strokeWidth: 3,
            stroke: am5.color(0xffffff)
        });

        series.labels.template.set("forceHidden", true);
        series.ticks.template.set("forceHidden", true);

        // Set data
        // https://www.amcharts.com/docs/v5/charts/percent-charts/pie-chart/#Setting_data
        // var i = 0;
        // for (i; i < data.length; i++) {
        //     var item = {
        //         category: data[i].Category,
        //         value: data[i].Value,
        //     }
        //     data.push(item)
        // }

        series.data.setAll(data);

        // Create legend
        // https://www.amcharts.com/docs/v5/charts/percent-charts/legend-percent-series/
        var legend = piechart.children.push(am5.Legend.new(root, {
            centerX: am5.p50,
            x: am5.p50,
            marginTop: 15,
            marginBottom: 15
        }));

        legend.data.setAll(series.dataItems);

        var exporting = am5plugins_exporting.Exporting.new(root, {
            menu: am5plugins_exporting.ExportingMenu.new(root, {}),
            dataSource: data,
            filePrefix: `solicitudes_trámite_${moment().format('YYYY-MM-DD_hh:mm')}`,
            title: `Total de Solicitudes Por Trámite ${f1} - ${f2}`,
            dataFields: {
                value: "Total",
                category: "Trámite",
            },
            pdfOptions: {
                align: 'center',
                includeData: true,
                addURL: false,
                //font: am5fonts_notosans_kr
            },
            pngOptions: {
                disabled: true
            },
            jpgOptions: {
                disabled: true
            },
            printOptions: {
                disabled: true
            },
            xlsxOptions: {
                disabled: false
            }
        });

        // Play initial series animation
        // https://www.amcharts.com/docs/v5/concepts/animations/#Animation_of_series
        series.appear(1000, 100);

    }
    const UpdateChartGauge = (data, maxvalue) => {

        // Create root element
        // https://www.amcharts.com/docs/v5/getting-started/#Root_element
        var root = am5.Root.new("kt_amcharts_5");

        //var rootG = root;
        // Set themes
        // https://www.amcharts.com/docs/v5/concepts/themes/
        root.setThemes([
            am5themes_Animated.new(root)
        ]);


        // Create chart
        // https://www.amcharts.com/docs/v5/charts/xy-chart/
        gaugechart = root.container.children.push(am5radar.RadarChart.new(root, {
            panX: false,
            panY: false,
            wheelX: "panX",
            wheelY: "zoomX",
            innerRadius: am5.percent(20),
            startAngle: -90,
            endAngle: 180
        }));

        // Data
        var i = 0;

        // Add cursor
        // https://www.amcharts.com/docs/v5/charts/xy-chart/cursor/
        var cursor = gaugechart.set("cursor", am5radar.RadarCursor.new(root, {
            behavior: "zoomX"
        }));
        cursor.lineY.set("visible", false);

        // Create axes and their renderers
        // https://www.amcharts.com/docs/v5/charts/radar-chart/#Adding_axes
        var xRenderer = am5radar.AxisRendererCircular.new(root, {
            //minGridDistance: 50
        });

        xRenderer.labels.template.setAll({
            radius: 10
        });

        xRenderer.grid.template.setAll({
            forceHidden: true
        });

        var xAxis = gaugechart.xAxes.push(am5xy.ValueAxis.new(root, {
            renderer: xRenderer,
            min: 0,
            max: maxvalue,
            maxPrecision: 0,
            strictMinMax: true,
            //numberFormat: "#'%'",
            tooltip: am5.Tooltip.new(root, {})
        }));

        var yRenderer = am5radar.AxisRendererRadial.new(root, {
            minGridDistance: 10
        });

        yRenderer.labels.template.setAll({
            centerX: am5.p100,
            fontWeight: "500",
            fontSize: 12,
            templateField: "columnSettings"
        });

        yRenderer.grid.template.setAll({
            forceHidden: true
        });

        var yAxis = gaugechart.yAxes.push(am5xy.CategoryAxis.new(root, {
            categoryField: "category",
            renderer: yRenderer
        }));

        yAxis.data.setAll(data);

        // Create series
        // https://www.amcharts.com/docs/v5/charts/xy-chart/series/
        var series1 = gaugechart.series.push(am5radar.RadarColumnSeries.new(root, {
            xAxis: xAxis,
            yAxis: yAxis,
            clustered: false,
            valueXField: "full",
            categoryYField: "category",
            fill: root.interfaceColors.get("alternativeBackground")
        }));

        series1.columns.template.setAll({
            width: am5.p100,
            fillOpacity: 0.08,
            strokeOpacity: 0,
            cornerRadius: 20
        });

        series1.data.setAll(data);

        var series2 = gaugechart.series.push(am5radar.RadarColumnSeries.new(root, {
            xAxis: xAxis,
            yAxis: yAxis,
            clustered: false,
            valueXField: "value",
            categoryYField: "category"
        }));

        series2.columns.template.setAll({
            width: am5.p100,
            strokeOpacity: 0,
            tooltipText: "{category}: {valueX}",
            cornerRadius: 20,
            templateField: "columnSettings"
        });

        series2.data.setAll(data);

        // Animate chart and series in
        // https://www.amcharts.com/docs/v5/concepts/animations/#Initial_animation
        series1.appear(1000);
        series2.appear(1000);

        var exporting = am5plugins_exporting.Exporting.new(root, {
            menu: am5plugins_exporting.ExportingMenu.new(root, {}),
            dataSource: data,
            filePrefix: `verificaciones_tipo_certificado_${moment().format('YYYY-MM-DD_hh:mm')}`,
            title: `Verificaciones Por Certificado ${f1} - ${f2}`,
            dataFields: {
                value: "Total",
                category: "Trámite",
            },
            pdfOptions: {
                align: 'center',
                includeData: true,
                addURL: false,
                //font: am5fonts_notosans_kr
            },
            pngOptions: {
                disabled: true
            },
            jpgOptions: {
                disabled: true
            },
            printOptions: {
                disabled: true
            },
            xlsxOptions: {
                disabled: false
            }
        });


    }
    const filters = () => {
        $("#barchart_date_filter").daterangepicker({
            showDropdowns: true,
            minYear: 2019,
            maxYear: parseInt(moment().format("YYYY"), 12),
            autoApply: true,
            autoUpdateInput: true,
            startDate: moment(),
            startDate: moment().subtract(7, 'days'),
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
        $("#barchart_date_filter_open").click(function () {
            $("#barchart_date_filter").data('daterangepicker').toggle();
        });

        $("#barchart_date_filter").on('apply.daterangepicker', function (ev, picker) {
            f1 = picker.startDate.format('DD/MM/YYYY');
            f2 = picker.endDate.format('DD/MM/YYYY');
            $(this).val(f1 + ' - ' + f2);
            reloadBarChart();
            reloadChartPie();
            reloadChartGauge();
        });
        let dtvalue = $("#barchart_date_filter").val() ?? "";
        let fechas = dtvalue.split(' - ');
        f1 = fechas.length > 1 ? fechas[0] : '';
        f2 = fechas.length > 1 ? fechas[1] : '';
    }
    var GetData = () => {
        return {
            fechaInicial: moment(f1, 'DD/MM/YYYY').format('YYYY-MM-DD'),
            fechaFinal: moment(f2, 'DD/MM/YYYY').format('YYYY-MM-DD'),
        }
    }
    function maybeDisposeRoot(divId) {

        let arr = am5.registry.rootElements;
        let newArr = arr.filter(e => e.dom.id == divId);
        if (newArr.length > 0) {
            newArr[0].container.children.clear();
            newArr[0].dispose()
        }
    }
    var reloadChartPie = () => {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/EstadisiticaUsoFormaValorada/GetGraphPie',
            data: GetData(),
            success: function (result) {

                if (piechart)
                    maybeDisposeRoot('kt_amcharts_3');
                UpdateChartPie(result?.result ?? []);

                return;
            },
            error: function (res) {
                toastr.error("No fue posible actualizar la gráfica.", "SMADSOT");
                return;
            }
        });
    }
    var reloadChartGauge = () => {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/EstadisiticaUsoFormaValorada/GetGraphGauge',
            data: GetData(),
            success: function (response) {

                if (gaugechart)
                    maybeDisposeRoot('kt_amcharts_5');
                UpdateChartGauge(response?.result?.result ?? [], response?.result?.maxValue);

                return;
            },
            error: function (res) {
                toastr.error("No fue posible actualizar la gráfica.", "SMADSOT");
                return;
            }
        });
    }
    const reloadBarChart = () => {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/EstadisiticaUsoFormaValorada/GetGraphBar',
            data: GetData(),
            success: function (result) {
                // Transforma los datos para incluir los totales
                var transformedData = result.map(function (item) {
                    return {
                        clave: item.clave,
                        aprobadas: item.aprobadas,
                        rechazadas: item.rechazadas,
                        noProcesadas: item.noProcesadas,
                        todas: item.todas
                    };
                });
                if (chart)
                    maybeDisposeRoot('barchart_cvv');
                updateChart(transformedData);
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error("No fue posible actualizar la gráfica.", "SMADSOT");
                return;
            }
        });
    }
    return {
        init: () => {
            return init();
        }
    }
}()
var indicadores = function () {
    const target = document.querySelector("#indicadores-content");
    const blockUIcustom = new KTBlockUI(target, {
        message: '<div class="blockui-message"><span class="spinner-border text-primary"></span> Cargando...</div>',
    });
    var init = (data) => {
        uiblockunblock();
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/EstadisiticaUsoFormaValorada/GetCounters',
            data: data,
            success: function (result) {
                if (result.isSuccessFully) {
                    $("#indicadores-content").html('');
                    $("#indicadores-content").html(result.result);
                }
                uiblockunblock();
                return;
            },
            error: function (res) {
                uiblockunblock();
                // toastr.error("No fue posible actualizar la gráfica.", "SMADSOT");
                return;
            }
        });
    }
    const uiblockunblock = () => {
        if (blockUIcustom.isBlocked()) {
            blockUIcustom.release();
        } else {
            blockUIcustom.block();
        }
    }
    return {
        init: (data) => {
            init(data);
        }
    }
}();
var foliosContent = function () {
    const target = document.querySelector("#content-folios").parentElement;
    const blockUIcustom = new KTBlockUI(target, {
        message: '<div class="blockui-message"><span class="spinner-border text-primary"></span> Cargando...</div>',
    });
    var init = (data) => {
        uiblockunblock();
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/EstadisiticaUsoFormaValorada/GetConteoCertificados',
            data: data,
            success: function (result) {
                if (result.isSuccessFully) {
                    $("#content-folios").html('');
                    $("#content-folios").html(result.result);
                }
                uiblockunblock();
                return;
            },
            error: function (res) {
                uiblockunblock();
                return;
            }
        });
    }
    const uiblockunblock = () => {
        if (blockUIcustom.isBlocked()) {
            blockUIcustom.release();
        } else {
            blockUIcustom.block();
        }
    }
    return {
        init: (data) => {
            init(data);
        }
    }
}();
$(document).ready(function () {
    main.init();
    Charts.init();
});