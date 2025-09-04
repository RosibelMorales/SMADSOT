var myDropzone, myDropzone2, myDropzone3;

var table;
var devolucionSPFGrid = function () {
    let datatable;
    var init = function () {
        datatable = $('#devolucionSPFGrid').DataTable({
            pageLength: 15,
            //Order
            order: [[0, 'desc']],
            //Numero Devolucion
            /*order: [[1, 'desc']],*/
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
                url: 'DevolucionSPF/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all",
                target: 0, visible: false
            }],
            columns: [
                { data: 'id', name: 'Id', title: 'Id' },
                { data: 'numeroDevolucion', name: 'NumeroDevolucion', title: 'Número de Devolución' },
                { data: 'fechaRegistroStr', name: 'FechaRegistro', title: 'Fecha de Registro' },
                { data: 'fechaEntregaStr', name: 'FechaEntrega', title: 'Fecha de Entrega' },
                { data: 'userAprobo', name: 'userAprobo', title: 'Usuario que aprobó' },
                { data: 'responsableEntrega', name: 'ResponsableEntrega', title: 'Responsabe de la entrega' },
                {
                    data: 'recibioSPF', name: 'recibioSPF', title: 'Recibió en SPF'
                },
                { data: 'numeroSolicitud', name: 'numeroSolicitud', title: 'Número de Solicitud' },
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            var htmlString = row.actions;
                            return htmlString
                        }
                    }
                }
            ],
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');
        listeners();
    }

    var exportButtons = () => {
        var buttons = new $.fn.dataTable.Buttons(table, {
            buttons: [
                {
                    extend: 'excelHtml5',
                    action: function (e, dt, node, config) {
                        var search = dt.ajax.params().search.value;
                        var sortColumnDirection = dt.ajax.params().order[0].dir;
                        var sortcolumn = dt.ajax.params().columns[dt.ajax.params().order[0].column].name;
                        var url = siteLocation + 'DevolucionSPF/CrearReporte?search=' + search + "&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn;

                        $.ajax({
                            cache: false,
                            type: 'GET',
                            url: url,
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
                    }
                },
                {
                    extend: 'pdfHtml5',
                    action: function (e, dt, node, config) {
                        var search = dt.ajax.params().search.value;
                        var sortColumnDirection = dt.ajax.params().order[0].dir;
                        var sortcolumn = dt.ajax.params().columns[dt.ajax.params().order[0].column].name;
                        var url = siteLocation + 'DevolucionSPF/CrearReporte?search=' + search + "&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn + "&esReportePdf=" + true;
                        window.open(url, "_blank");
                    }
                }
            ]
        }).container().appendTo($('#kt_datatable_buttons'));

        const exportButtons = document.querySelectorAll('#kt_datatable_export_menu [data-kt-export]');
        exportButtons.forEach(exportButton => {
            exportButton.addEventListener('click', e => {
                e.preventDefault();

                const exportValue = e.target.getAttribute('data-kt-export');
                const target = document.querySelector('.dt-buttons .buttons-' + exportValue);
                target.click();
            });
        });
    }

    var handleSearchDatatable = () => {
        const filterSearch = document.querySelector('[data-kt-filter="search"]');
        filterSearch.addEventListener('keyup', function (e) {
            datatable.search(e.target.value).draw();
        });
    }

    var recargar = function () {
        datatable.ajax.reload();
    }

    var listeners = function () {

    }

    return {
        init: function () {

            table = document.querySelector('#devolucionSPFGrid');
            if (!table) {
                return;
            }
            init();
            exportButtons();
            handleSearchDatatable();

        },
        recargar: function () {
            recargar();
        }
    };
}();

$(document).on("click", ".btnDevolucionSPFDetalle", function () {
    let id = $(this).attr("data-id");
    //devolucionSPFRegistro.init(id, true);
    modalDetalle.init(id);
});

var devolucionSPFControls = function () {
    var devolucionSPFDropzone = function () {
        //    var myDropzone = new Dropzone("#devolucionSPF_dropzonejs", {
        //        url: "https://keenthemes.com/scripts/void.php", // Set the url for your upload script location
        //        paramName: "file", // The name that will be used to transfer the file
        //        maxFiles: 3,
        //        maxFilesize: 10, // MB
        //        addRemoveLinks: true,
        //        autoQueue: false,
        //        accept: function (file, done) {
        //            if (file.name == "wow.jpg") {
        //                done("Naha, you don't.");
        //            } else {
        //                done();
        //            }
        //        }
        //    });
        myDropzone = new Dropzone("#dropzonejs1", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs1", // Este es el contenedor donde se mostrarán las vistas previas
            init: function () {
                this.on("addedfile", function (file) {
                    if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
                        return file.type.includes(acceptedType);
                    })) {
                        toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
                        toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (this.files.length > this.options.maxFiles) {
                        toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
                        this.removeFile(file);
                    }
                });
                this.on("removedfile", function (file) {
                    // Aquí puedes realizar acciones cuando un archivo se elimina
                });
            },
        });
        myDropzone2 = new Dropzone("#dropzonejs2", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs2", // Este es el contenedor donde se mostrarán las vistas previas
            init: function () {
                this.on("addedfile", function (file) {
                    if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
                        return file.type.includes(acceptedType);
                    })) {
                        toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
                        toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (this.files.length > this.options.maxFiles) {
                        toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
                        this.removeFile(file);
                    }
                });
                this.on("removedfile", function (file) {
                    // Aquí puedes realizar acciones cuando un archivo se elimina
                });
            },
        });
        myDropzone3 = new Dropzone("#dropzonejs3", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs3", // Este es el contenedor donde se mostrarán las vistas previas
            init: function () {
                this.on("addedfile", function (file) {
                    if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
                        return file.type.includes(acceptedType);
                    })) {
                        toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
                        toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
                        this.removeFile(file);
                    } else if (this.files.length > this.options.maxFiles) {
                        toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
                        this.removeFile(file);
                    }
                });
                this.on("removedfile", function (file) {
                    // Aquí puedes realizar acciones cuando un archivo se elimina
                });
            },
        });
    }


    var devolucionSPFNumeroSolicitudAutocomplete = function () {
        $("#NumeroSolicitud").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: siteLocation + "DevolucionSPF/NumeroSolicitudAutocomplete",
                    type: "POST",
                    dataType: "json",
                    data: {
                        prefix: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return { label: item.nombre, value: item.id };
                        }));
                    }
                });
            },
            select: function (event, ui) {
                event.preventDefault();
                $("#NumeroSolicitud").val(ui.item.label);
                //actualizarTablaFija(ui.item.label);
            },
            search: function (event, ui) {
                //event.preventDefault();
                //$("#cp").val("");
                //$("#Estado").val('');
                //$("#Pais").val('');
                //$("#Municipio").val('');
                //$("#IdCatColonia").find('option').remove();
            }
        });
    }

    var devolucionSPFResponsableEntregaAutocomplete = function () {
        $("#ResponsableEntrega").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: siteLocation + "DevolucionSPF/ResponsableEntregaAutocomplete",
                    type: "POST",
                    dataType: "json",
                    data: {
                        prefix: request.term
                    },
                    success: function (data) {
                        //console.log(data);
                        //response(data);
                        response($.map(data, function (item) {
                            return { label: item.nombre, value: item.id };
                        }));
                    }
                });
            },
            select: function (event, ui) {
                event.preventDefault();
                $("#ResponsableEntrega").val(ui.item.label);
            },
            search: function (event, ui) {
                //event.preventDefault();
                //$("#cp").val("");
                //$("#Estado").val('');
                //$("#Pais").val('');
                //$("#Municipio").val('');
                //$("#IdCatColonia").find('option').remove();
            }
        });
    }

    var devolucionSPFFechaEntrega = function () {
        moment.locale('es');
        $('#FechaEntrega').daterangepicker({
            alwaysShowCalendars: true,
            singleDatePicker: true,
            showDropdowns: true,
            showCustomRangeLabel: false,
            autoApply: true,
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        });
    }

    //var actualizarTablaFija = function (numeroSolicitud) {
    //    $.ajax({
    //        url: siteLocation + "DevolucionSPF/ActualizarTablaFija",
    //        type: "POST",
    //        dataType: "json",
    //        data: {
    //            numeroSolicitud: numeroSolicitud
    //        },
    //        success: function (data) {
    //            if (data.isSuccessFully) {
    //                $('#tablaFijaPartial').html('');
    //                $('#tablaFijaPartial').html(data.result);
    //            } else {
    //                toastr.error("Error al cargar la información del número de solicitud proporcionado.", "SMADSOT")
    //            }
    //        }
    //    });
    //}

    var isReadOnly = function () {
        let read = $("#isReadOnly").val();
        if (read === true || read === 'True' || read === 'true') {
            $("#btnGuardarRegistro").hide();
            //$("#NumeroSolicitud").prop('disabled', true);
            $("#ResponsableEntrega").prop('disabled', true);
            $("#FechaEntrega").prop('disabled', true);
            $("#RecibioEnSPF").prop('disabled', true);
        } else {
            $("#btnGuardarRegistro").show();
        }
    }

    return {
        init: function (isDetail) {
            devolucionSPFNumeroSolicitudAutocomplete();
            if ($("#isReadOnly").val() !== 'true') {
                devolucionSPFDropzone();
            }
            devolucionSPFFechaEntrega();
            devolucionSPFResponsableEntregaAutocomplete();
            isReadOnly();
        }
    }
}();

var devolucionSPFRegistro = function () {
    var fv;
    var init = function (id, isReadOnly) {
        abrirModal(id, isReadOnly);
    }

    var abrirModal = function (id, isReadOnly) {
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/DevolucionSPF/Registro',
            data: {
                id: id,
                isReadOnly: isReadOnly
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Registrar Devolución del stock de ventanilla' : 'Detalle Devolución del stock de ventanilla');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#IdAlmacen').select2({
                        dropdownParent: $('#modal_registro'),
                        placeholder: "Selecciona un almacen..."
                    });
                });
                $('#modal_registro').modal('show');
                listeners();
                validate();
                devolucionSPFControls.init();
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error("No es posible abrir la pantalla de registro.", "SMADSOT");
                return;
            }
        });
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {

        // ActualizarTablaFija.actualizar($("#IdAlmacen").val())

        // $(document).on('change', '#IdAlmacen', function (e) {

        //     //actualizarTablaFija(this.value);
        //     ActualizarTablaFija.actualizar(this.value)
        // });

        $('#btnGuardarRegistro').off().on('click', function (e) {
            if (fv) {
                fv.validate().then(async function (status) {
                    if (status === 'Valid' && validateTable()) {
                        GuardarInfo();
                    }
                });
            }
        });

        moment.locale('es');
        $('#FechaEntrega').daterangepicker({
            alwaysShowCalendars: true,
            singleDatePicker: true,
            showDropdowns: true,
            showCustomRangeLabel: false,
            autoApply: true,
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        });

        const folioInicialInputs = document.querySelectorAll('.folioInicialInput');
        const cantidadInputs = document.querySelectorAll('.cantidadInput');

        folioInicialInputs.forEach(box => {
            box.addEventListener('change', function handleClick(event) {
                let valor = Number(this.value);
                if (valor < 1) {
                    this.value = 0;
                    return;
                }
                let cantidad = Number(this.parentNode.parentNode.querySelectorAll('input')[0]?.value);
                this.parentNode.parentNode.querySelectorAll('input')[2].value = valor + (cantidad - 1);
            });
        });
        cantidadInputs.forEach(box => {
            box.addEventListener('change', function handleClick(event) {
                let valor = Number(this.parentNode.parentNode.querySelectorAll('input')[1]?.value);
                if (valor < 1) {
                    // this.value = 0;
                    return;
                }
                let cantidad = Number(this?.value);
                this.parentNode.parentNode.querySelectorAll('input')[2].value = valor + (cantidad - 1);
                validateTable();
            });
        });
        //var actualizarTablaFija = function (numeroAlmacen) {
        //    $.ajax({
        //        url: siteLocation + "DevolucionSPF/ActualizarTablaFija",
        //        type: "POST",
        //        dataType: "json",
        //        data: {
        //            almacen: numeroAlmacen
        //        },
        //        success: function (data) {
        //            console.log(data)
        //            if (data.isSuccessFully) {
        //                $('#tablaFijaPartial').html('');
        //                $('#tablaFijaPartial').html(data.result);
        //                const folioInicialInputs = document.querySelectorAll('.folioInicialInput');
        //                const cantidadInputs = document.querySelectorAll('.cantidadInput');

        //                folioInicialInputs.forEach(box => {
        //                    box.addEventListener('change', function handleClick(event) {
        //                        let valor = Number(this.value);
        //                        if (valor < 1) {
        //                            this.value = 0;
        //                            return;
        //                        }
        //                        let cantidad = Number(this.parentNode.parentNode.querySelectorAll('input')[0]?.value);
        //                        this.parentNode.parentNode.querySelectorAll('input')[2].value = valor + cantidad;
        //                    });
        //                });
        //                cantidadInputs.forEach(box => {
        //                    box.addEventListener('change', function handleClick(event) {
        //                        let valor = Number(this.value);
        //                        if (valor < 1) {
        //                            this.value = 0;
        //                            return;
        //                        }
        //                        let cantidad = Number(this.parentNode.parentNode.querySelectorAll('input')[1]?.value);
        //                        this.parentNode.parentNode.querySelectorAll('input')[2].value = valor + cantidad;
        //                    });
        //                });
        //            } else {
        //                toastr.error("Error al cargar la información del almacen proporcionado.", "SMADSOT")
        //            }
        //        }
        //    });
        //}
    }
    //const initSelectUsuarios = () => {
    //    let records = 15
    //    $("#usuarioasignado").select2({
    //        ajax: {
    //            url: "",
    //            dataType: "json",
    //            delay: 2000,
    //            data: function (params) {
    //                return {
    //                    q: params.term, // search term
    //                    page: params.page,
    //                    record: records,
    //                }
    //            },
    //            processResults: function (data, params) {
    //                // parse the results into the format expected by Select2
    //                // since we are using custom formatting functions we do not need to
    //                // alter the remote JSON data, except to indicate that infinite
    //                // scrolling can be used
    //                params.page = params.page || 1

    //                return {
    //                    results: data.items,
    //                    pagination: {
    //                        more: params.page * 15 < data.total_count,
    //                    },
    //                }
    //            },
    //            cache: true,
    //        },
    //        placeholder: "Seleccionar usuario...",
    //        minimumInputLength: 2,
    //        language: "es",
    //    })
    //}
    //// Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
    }

    var validate = () => {
        const form = document.getElementById('form_registroDevolucionSPF');
        fv = FormValidation.formValidation(form, {
            fields: {
                FechaEntrega: {
                    date: {
                        format: 'DD/MM/YYYY',
                        message: 'El valor no es una fecha válida',
                    },
                },
                // IdAlmacen: {
                //     validators: {
                //         greaterThan: {
                //             message: 'Debe seleccionar un almacen.',
                //             min: 1,
                //         },
                //     },
                // },
                IdAlmacen: {
                    validators: {
                        greaterThan: {
                            message: 'Debe seleccionar un almacen.',
                            min: 1,
                        },
                    },
                },
                ResponsableEntrega: {
                    validators: {
                        notEmpty: {
                            message: 'El campo es obligatorio.',
                        },
                    },
                },
                RecibioEnSPF: {
                    validators: {
                        notEmpty: {
                            message: 'El campo es obligatorio.',
                        },
                    },
                },

            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap(),
                submitButton: new FormValidation.plugins.SubmitButton(),
                icon: new FormValidation.plugins.Icon({
                    validating: 'fa fa-refresh',
                }),
            },
        });

    }
    var validateTable = function () {
        let CantidadTotal = 0;
        let isValid = true;
        document.querySelectorAll('#TableAsignacion tbody tr').forEach((item, i) => {
            let element = item;
            item.querySelectorAll('input').forEach((item, i) => {
                let valor = Number(item?.value);
                let elementParent = item.parentNode;
                if (valor < 0) {
                    DeleteMessage(elementParent);
                    elementParent.append(Createmessage('No se permiten números negativos.'));
                    isValid = false;
                }
            })
            item.querySelectorAll('input.cantidadInput').forEach((item, i) => {
                let cantidad = Number(item?.value)
                CantidadTotal += cantidad;
                let folioInicial = element.querySelectorAll('input')[1];
                let valuefolioInicial = Number(folioInicial?.value);
                let folioFinal = element.querySelectorAll('input')[2];
                let valuefolioFinal = Number(folioFinal?.value);
                let elementParent = folioInicial.parentNode;
                let elementParentFinal = folioFinal.parentNode;
                if (cantidad !== 0) {
                    if (valuefolioInicial < 1) {
                        DeleteMessage(elementParent);
                        elementParent.append(Createmessage('Debe ingresar un folio inicial.'));
                        isValid = false;
                    }
                    if ((valuefolioFinal - valuefolioInicial) !== (cantidad - 1)) {
                        DeleteMessage(elementParentFinal);
                        elementParentFinal.append(Createmessage('El folio final debe corresponder a la cantidad ingresada.'));
                        isValid = false;
                    }
                }
            })
        })
        if (CantidadTotal < 1) {
            let elementParentTable = document.getElementById('TableAsignacion').parentNode;
            document.querySelectorAll(".fv-plugins-message-container.tableMessage").forEach(e => e.remove());
            elementParentTable.append(Createmessage("Debe ingresar al menos una cantidad.", true));
            isValid = false;
        }
        return isValid;
    }
    const DeleteMessage = function (element) {
        element.querySelectorAll(".fv-plugins-message-container").forEach(e => e.remove());
    }
    const Createmessage = (message, isTableMessage, dataField) => {
        var elementMessage = document.createElement('div');
        elementMessage.classList.add('fv-plugins-message-container');
        if (isTableMessage)
            elementMessage.classList.add('tableMessage');
        elementMessage.innerHTML = `<div data-field="${dataField}" data-validator="notEmpty" class="fv-help-block">${message}</div>`;
        return elementMessage;
    }
    const GuardarInfo = () => {

        fv.validate().then(async function (status) {
            /*validation.validate().then(async function (status) {*/
            if (status === 'Valid') {
                var request = {}
                request.NumeroDevolucion = $("#NumeroDevolucion")?.val();
                request.IdAlmacen = $('#IdAlmacen')?.val();
                request.NumeroSolicitud = $("#NumeroSolicitud")?.val();
                request.ResponsableEntrega = $("#ResponsableEntrega")?.val();
                request.FechaEntrega = $('#FechaEntrega')?.val();
                request.RecibioEnSPF = $("#RecibioEnSPF")?.val();

                let asignaciones = [];
                document.querySelectorAll('#TableAsignacion tbody tr').forEach((element, i) => {
                    let asignacion = {};
                    let cantidad = Number(element.querySelectorAll('input')[0]?.value);
                    if (cantidad > 0) {
                        asignacion.Cantidad = cantidad
                        asignacion.FolioInicial = Number(element.querySelectorAll('input')[1]?.value);
                        asignacion.FolioFinal = Number(element.querySelectorAll('input')[2]?.value);
                        asignacion.IdTipoCertificado = element.querySelectorAll('td[data-tipocertificado]')[0].dataset.tipocertificado;
                        asignaciones.push(asignacion);
                    }
                })
                request.TablaFijaViewModel = asignaciones;

                var files = [];
                files.push(Dropzone.forElement("#dropzonejs1").files[0]);
                files.push(Dropzone.forElement("#dropzonejs2").files[0]);
                files.push(Dropzone.forElement("#dropzonejs3").files[0]);
                files = files.filter(element => {
                    return element !== undefined;
                });
                if (files.length > 0) {
                    var arrayOfBase64 = await readFile(files);
                    files = [];
                    for (let i = 0; i < arrayOfBase64.length; i++) {
                        files.push((arrayOfBase64[i]));
                    }
                    /*formData.set('Files', JSON.stringify(files));*/
                    request.Files = JSON.stringify(files);
                }

                $.ajax({
                    cache: false,
                    type: 'Post',
                    url: 'DevolucionSPF/Registro',
                    data: request,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        toastr.success('Registro guardado exitosamente', "SMADSOT");
                        $('#modal_registro').modal('hide');
                        devolucionSPFGrid.recargar();
                        //KTDatatableRemoteAjax.recargar();
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, "SMADSOT");
                        return;
                    }
                });
            }
        });



        async function readFile(fileList) {
            function getBase64(file) {
                let fileData = {};
                fileData.Nombre = file.name;
                fileData.Tipo = file.type.split("/")[1];

                const reader = new FileReader()
                return new Promise((resolve) => {
                    reader.onload = (ev) => {
                        var base64Data = ev.target.result.split('base64,')[1];
                        fileData.Base64 = base64Data;
                        resolve(fileData)
                    }
                    reader.readAsDataURL(file)
                })
            }

            const promises = []

            for (let i = 0; i < fileList.length; i++) {
                promises.push(getBase64(fileList[i]))
            }
            return Promise.all(promises)
        }
    }

    return {
        init: function (id, isReadOnly) {
            init(id, isReadOnly);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

$(document).on("click", "#btnRegistrarDevolucionSPF", function () {
    devolucionSPFRegistro.init();
})

//$(document).on("click", "#btnGuardarRegistro", function () {
//    let numeroDevolucion = $("#NumeroDevolucion").val();
//    let numeroSolicitud = $("#NumeroSolicitud").val();
//    let responsableEntrega = $("#ResponsableEntrega").val();
//    let fechaEntrega = $("#FechaEntrega").val();
//    let recibioEnSPF = $("#RecibioEnSPF").val();

//    if (numeroSolicitud === undefined || numeroSolicitud === '') {
//        toastr.error("El Número de Solicitud es requerido", "SMADSOT")
//        return;
//    }

//    if (responsableEntrega === undefined || responsableEntrega === '') {
//        toastr.error("El Responsable de Entrega es requerido", "SMADSOT")
//        return;
//    }

//    if (recibioEnSPF === undefined || recibioEnSPF === '') {
//        toastr.error("Quién recibió en es requerido", "SMADSOT")
//        return;
//    }

//    devolucionSPFRegistro.cerrarventanamodal();
//    toastr.success("La información ha sido guardada.", "SMADSOT")
//});

var modalDetalle = function () {
    const init = (id) => {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'DevolucionSPF/Detalle/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Detalle de solicitud');

                $('#btnGuardarRegistro').hide()
                $('#modal_registro input').prop('readonly', true);
                //$("#modal_registro").on('shown.bs.modal', function () {
                //    $('#IdAlmacen').select2({
                //        dropdownParent: $('#modal_registro'),
                //        placeholder: "Selecciona un almacen..."
                //    });
                //});
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                listeners();
                $('#modal_registro').modal('show');
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var listeners = function () {
        $(document).on('click', '.descargarDoc', function (e) {
            DescargarDocumento.generar($(this).data('url'));
        });
        moment.locale('es');
        $('#FechaEntrega').daterangepicker({
            alwaysShowCalendars: true,
            singleDatePicker: true,
            showDropdowns: true,
            showCustomRangeLabel: false,
            autoApply: true,
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        });

    }
    return {
        init: (id) => {
            init(id);
        }

    }
}()

var DescargarDocumento = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: '/DevolucionSPF/DescargarDocumento?url=' + url,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>' + result.result.fileName + '</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64 + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
                //var w = window.open('data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64, '_blank');
                //w.document.title = result.result.fileName + '';
                toastr.success('Documento descargado correctamente.', 'SMADSOT');
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    };

    return {
        generar: function (url) {
            generar(url);
        }
    }

}();

var ActualizarTablaFija = function () {
    var actualizar = function (numeroAlmacen) {
        $.ajax({
            url: siteLocation + "DevolucionSPF/ActualizarTablaFija",
            type: "POST",
            dataType: "json",
            data: {
                almacen: numeroAlmacen
            },
            success: function (data) {
                if (data.isSuccessFully) {
                    $('#tablaFijaPartial').html('');
                    $('#tablaFijaPartial').html(data.result);
                    const folioInicialInputs = document.querySelectorAll('.folioInicialInput');
                    const cantidadInputs = document.querySelectorAll('.cantidadInput');

                    folioInicialInputs.forEach(box => {
                        box.addEventListener('change', function handleClick(event) {
                            let valor = Number(this.value);
                            if (valor < 1) {
                                this.value = 0;
                                return;
                            }
                            let cantidad = Number(this.parentNode.parentNode.querySelectorAll('input')[0]?.value);
                            this.parentNode.parentNode.querySelectorAll('input')[2].value = valor + cantidad;
                        });
                    });
                    cantidadInputs.forEach(box => {
                        box.addEventListener('change', function handleClick(event) {
                            let valor = Number(this.value);
                            if (valor < 1) {
                                this.value = 0;
                                return;
                            }
                            let cantidad = Number(this.parentNode.parentNode.querySelectorAll('input')[1]?.value);
                            this.parentNode.parentNode.querySelectorAll('input')[2].value = valor + cantidad;
                        });
                    });
                } else {
                    toastr.error("Error al cargar la información del almacen proporcionado.", "SMADSOT")
                }
            }
        });
    };

    return {
        actualizar: function (numeroAlmacen) {
            actualizar(numeroAlmacen);
        }
    }

}();

jQuery(document).ready(function () {
    devolucionSPFGrid.init();
});