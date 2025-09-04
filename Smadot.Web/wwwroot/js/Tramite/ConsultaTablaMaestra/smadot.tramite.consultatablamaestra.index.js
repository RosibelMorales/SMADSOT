"use strict"
var datatable;
var myDropzone;
var selectValInicial = $("#CicloVerificacion option:selected").val();
var id = 0;
var KTDatatableRemoteAjax = function () {
    var table;
    var initDatatable = function () {
        const tableRows = table.querySelectorAll('tbody tr');
        datatable = $(table).DataTable({
            language: {
                decimal: "",
                emptyTable: "No hay información",
                info: "Mostrando _END_ de _TOTAL_ entradas",
                infoEmpty: "Mostrando 0 de 0 entradas",
                infoFiltered: "(Filtrado de _MAX_ total entradas)",
                infoPostFix: "",
                thousands: ",",
                lengthMenu: "Mostrar _MENU_ Entradas",
                loadingRecords: "Cargando...",
                processing: "Procesando...",
                search: "Buscar:",
                zeroRecords: "No se encontraron resultados para mostrar",
                paginate: {
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
                url: 'ConsultaTablaMaestra/Consulta?IdCicloVerificacion=' + selectValInicial,
                type: 'POST'
            },
            columnDefs: [{
                className: "dtr-control",
                "defaultContent": "-",

            }, { responsivePriority: 13, targets: -1 }],
            columns: [
                {
                    data: 'id', name: 'Id', title: 'ID', width: 80
                },
                { data: 'marca', name: 'Marca', title: 'Marca' },
                { data: 'subMarca', name: 'Submarca', title: 'Submarca' },
                { data: 'protocoloStr', name: 'Protocolo', title: 'Protocolo' },
                { data: 'pbV_ASM', name: 'PBV_ASM', title: 'PBV ASM' },
                { data: 'anO_DESDE', name: 'AnioDesde', title: 'Desde' },
                { data: 'anO_HASTA', name: 'AnioHasta', title: 'Hasta' },
                { data: 'cilindros', name: 'Cilindros', title: 'Cilindros' },
                { data: 'cilindrada', name: 'Cilindrada', title: 'Cilindrada' },
                { data: 'poT_2540', name: 'POT_2540', title: 'POT_2540' },
                { data: 'poT_5024', name: 'POT_5024', title: 'POT_5024' },
                { data: 'aplicaDobleCero', name: 'DOBLECERO', title: 'Doble Cero' },
                {
                    title: 'Refrendo 00',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {

                        if (row?.reF_00 > 0)
                            return 'SÍ';
                        else
                            return 'NO';
                    }
                },
                {
                    title: 'Combustible',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {

                        let Combustible = row.cerO_GASOL == 0 ? '' : 'Gasolina, ';
                        Combustible += row.cerO_GASLP == 0 ? '' : 'Gas LP, ';
                        Combustible += row.cerO_GASNC == 0 ? '' : 'Gas Natural, ';
                        Combustible += row.cerO_DSL == 0 ? '' : ' Diesel, ';
                        if (Combustible.length > 2)
                            return Combustible.substring(0, Combustible.length - 2);
                        else
                            return '';
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
            ]
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');
        myDropzone = new Dropzone("#dropzonejs", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 10, // MB
            addRemoveLinks: true,
            acceptedFiles: ".dbf",
            previewsContainer: "#dropzonejs", // Este es el contenedor donde se mostrarán las vistas previas
            init: function () {
                this.on("addedfile", function (file) {
                    var fileNameParts = file.name.split('.');
                    var fileExtension = fileNameParts[fileNameParts.length - 1].toLowerCase(); // Obtener la extensión en minúsculas

                    if (fileExtension !== "dbf") {
                        toastr.error("Extensión de archivo no permitida.", 'SMADSOT');
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
        $("#kt_modal_1").on('hide.bs.modal', function () {
            myDropzone.removeAllFiles();
        });
        $('#btnActualizarTMaestra').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });
        $(document).on('click', '.btn-editar', function (e) {
            ModalEditarCrear.init($(this).data('id'));
        });
        $("#MarcaAutoComplete").on('change', function (e) {
            recargar();
        });
        $("#MarcaAutoComplete").select2({
            ajax: {
                url: `/ConsultaTablaMaestra/GetSubMarcasBusqueda`,
                dataType: "json",
                delay: 2500,
                data: function (params) {
                    return {
                        q: params.term,
                        page: params.page,
                        records: 10,
                    }
                },
                processResults: function (data, params) {
                    params.page = params.page || 1
                    if (!data.isSuccessFully) {
                        Swal.fire({
                            title: "Ocurrió un problema",
                            text: result.message,
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn btn-light"
                            }
                        }).then(function () {
                        });

                    }
                    // if (data.result?.data?.length < 1) {
                    //     Swal.fire({
                    //         title: 'Falta Información del Vehículo',
                    //         text: "No se encontró la submarca del vehículo. ¿Desea continuar con el registro para notificar la falta de información a Secretaría de Medio Ambiente, Desarrollo Sustentable y Ordenamiento Territorial?",
                    //         icon: "warning",
                    //         buttonsStyling: false,
                    //         showCancelButton: true,
                    //         reverseButtons: true,
                    //         cancelButtonText: 'Cancelar',
                    //         confirmButtonText: "Confirmar",
                    //         focusConfirm: true,
                    //         customClass: {
                    //             confirmButton: "btn btn-primary",
                    //             cancelButton: 'btn btn-secondary'
                    //         }
                    //     }).then((result) => {
                    //         if (result.isConfirmed) {
                    //             if (params.term) {
                    //                 var newOption = new Option(params.term, -1, false, false);
                    //                 $('#Modelo').append(newOption).trigger('change');
                    //                 $('#Modelo').val(-1).trigger('change');
                    //             }
                    //         }
                    //     });
                    // }
                    return {
                        results: data.result.data.map(function (item) {
                            return {
                                id: item.value,
                                text: item.text
                            };
                        }),
                        pagination: {
                            more: params.page * 10 < data.result.recordsTotal,
                        },
                    }
                },
                cache: true,
            },
            placeholder: 'Ingresa el modelo del Vehículo',
            minimumInputLength: 2,
            language: 'es',
        });
        $('.actualizarTMaestra').off().on('click', function (e) {
            e.preventDefault();
            id = this.dataset.ktActualizartmaestra;
            switch (id) {
                case '1':
                    $('#modalTitle').text('Carga de Archivo Tabla Maestra');
                    break;
                case '2':
                    $('#modalTitle').text('Carga de Archivo Diesel');
                    break;
                case '3':
                    $('#modalTitle').text('Carga de Archivo SubDiesel');
                    break;
                case '4':
                    $('#modalTitle').text('Carga de Archivo Marca');
                    break;
                case '5':
                    $('#modalTitle').text('Carga de Archivo SubMarca');
                    break;
            }
            $('#kt_modal_1').modal('show');
        });
    }

    var exportButtons = () => {
        $(document).on('click', '.btnExport', function (e) {
            var isPDF = this.dataset.ktExport === 'pdf' ? true : false;
            var search = datatable.ajax.params().search.value;
            var sortColumnDirection = datatable.ajax.params().order[0].dir;
            var sortcolumn = datatable.ajax.params().columns[datatable.ajax.params().order[0].column].name;
            var cicloExport = $("#CicloVerificacion option:selected").val();
            $.ajax({
                cache: false,
                type: 'GET',
                url: '/ConsultaTablaMaestra/GetPDF?search=' + search + "&sortColumnDirection=" + sortColumnDirection + "&sortColumn=" + sortcolumn + "&IdAlmacen=" + $("#almacenGrid option:selected").val() + "&PDF=" + isPDF +
                    '&IdCicloVerificacion=' + (cicloExport === undefined ? selectValInicial : cicloExport) + '&Marca=' + $("#MarcaAutoComplete").val() + '&Submarca=' + $("#SubmarcaAutoComplete").val() + '&Modelo=' + $("#ModeloAutoComplete").val(),
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
    const listeners = () => {
        $('#save').click(function (e) {
            ModalEditarCrear.init();
        });

    }
    //var handleSearchDatatable = () => {
    //    const filterSearch = document.querySelector('[data-kt-filter="search"]');
    //    filterSearch.addEventListener('keyup', function (e) {
    //        datatable.search(e.target.value).draw();
    //    });
    //}

    var recargar = function () {
        datatable.ajax.url(`ConsultaTablaMaestra/Consulta?idSubMarca=${Number($("#MarcaAutoComplete").val() ?? 0)}&&validarCombustible=false`);
        datatable.ajax.reload();
    }

    return {
        init: function () {
            table = document.querySelector('#kt_datatable');

            if (!table) {
                return;
            }

            initDatatable();
            exportButtons();
            listeners();
            //handleSearchDatatable();
        },
        recargar: function () {
            recargar();
        }
    };
}();

var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {

                var formData = new FormData();
                var fileInput = Dropzone.forElement("#dropzonejs"); // Reemplaza 'fileInput' con el ID real de tu input de archivo

                formData.append('file', fileInput.files[0]); // Agrega el archivo al FormData
                formData.append('tipo', id);

                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/ConsultaTablaMaestra/UploadFile',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        $('#kt_modal_1').modal('hide');
                        toastr.success(result.message, 'SMADSOT');
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
        var form = document.getElementById('form_registro');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    'dropzoneFiles': {
                        validators: {
                            callback: {
                                message: 'Debe agregar un archivo en el Dropzone.',
                                callback: function (input) {
                                    let dp = Dropzone.forElement("#dropzonejs").files;
                                    return dp.length > 0;
                                }
                            }
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
    }

    // async function readFile(fileList) {
    //     function getBase64(file) {
    //         let fileData = {};
    //         fileData.Nombre = file.name;
    //         fileData.Tipo = file.type.split("/")[1];

    //         const reader = new FileReader()
    //         return new Promise((resolve) => {
    //             reader.onload = (ev) => {
    //                 var base64Data = ev.target.result.split('base64,')[1];
    //                 fileData.Base64 = base64Data;
    //                 resolve(fileData)
    //             }
    //             reader.readAsDataURL(file)
    //         })
    //     }

    //     const promises = []

    //     for (let i = 0; i < fileList.length; i++) {
    //         promises.push(getBase64(fileList[i]))
    //     }
    //     return await Promise.all(promises)
    // }

    return {
        init: function () {
            init();
        },
        guardar: function () {
            guardar();
        }
    }

}();


// $(document).on('change', '#CicloVerificacion', function (e) {

//     var selectVal = $("#CicloVerificacion option:selected").val();
//     //datatable.ajax.url('ConsultaTablaMaestra/Consulta?IdCicloVerificacion=' + selectVal).load();
//     KTDatatableRemoteAjax.recargar();

// });

// $("#MarcaAutoComplete").autocomplete({
//     minLength: 3,
//     source: function (request, response) {
//         $.ajax({
//             url: siteLocation + "ConsultaTablaMaestra/MarcaAutocomplete",
//             type: "POST",
//             dataType: "json",
//             data: {
//                 prefix: request.term
//             },
//             success: function (data) {
//                 //response(data);
//                 response($.map(data, function (item) {
//                     return { label: item.nombre, value: item.id };
//                 }));
//             }
//         });
//     },
//     select: function (event, ui) {
//         event.preventDefault();
//         $("#MarcaAutoComplete").val(ui.item.label);
//         KTDatatableRemoteAjax.recargar();
//     },
//     focus: function (event, ui) {
//         event.preventDefault();
//         $("#MarcaAutoComplete").val(ui.item.label);
//     },
//     //search: function (event, ui) {
//     //    event.preventDefault();
//     //}
// });

// $("#SubmarcaAutoComplete").autocomplete({
//     minLength: 3,
//     source: function (request, response) {
//         $.ajax({
//             url: siteLocation + "ConsultaTablaMaestra/SubMarcaAutocomplete",
//             type: "POST",
//             dataType: "json",
//             data: {
//                 prefix: request.term
//             },
//             success: function (data) {
//                 //response(data);
//                 response($.map(data, function (item) {
//                     return { label: item.nombre, value: item.id };
//                 }));
//             }
//         });
//     },
//     select: function (event, ui) {
//         event.preventDefault();
//         $("#SubmarcaAutoComplete").val(ui.item.label);
//         KTDatatableRemoteAjax.recargar();
//     },
//     focus: function (event, ui) {
//         event.preventDefault();
//         $("#SubmarcaAutoComplete").val(ui.item.label);
//     },
//     //search: function (event, ui) {
//     //    event.preventDefault();
//     //}
// });

// $("#LineaAutoComplete").autocomplete({
//     minLength: 3,
//     source: function (request, response) {
//         $.ajax({
//             url: siteLocation + "ConsultaTablaMaestra/LineaAutocomplete",
//             type: "POST",
//             dataType: "json",
//             data: {
//                 prefix: request.term
//             },
//             success: function (data) {
//                 //response(data);
//                 response($.map(data, function (item) {
//                     return { label: item.nombre, value: item.id };
//                 }));
//             }
//         });
//     },
//     select: function (event, ui) {
//         event.preventDefault();
//         $("#LineaAutoComplete").val(ui.item.label);
//         KTDatatableRemoteAjax.recargar();
//     },
//     focus: function (event, ui) {
//         event.preventDefault();
//         $("#LineaAutoComplete").val(ui.item.label);
//     },
//     //search: function (event, ui) {
//     //    event.preventDefault();
//     //}
// });

// $("#ModeloAutoComplete").autocomplete({
//     minLength: 3,
//     source: function (request, response) {
//         $.ajax({
//             url: siteLocation + "ConsultaTablaMaestra/ModeloAutocomplete",
//             type: "POST",
//             dataType: "json",
//             data: {
//                 prefix: request.term
//             },
//             success: function (data) {
//                 //response(data);
//                 response($.map(data, function (item) {
//                     return { label: item.nombre, value: item.id };
//                 }));
//             }
//         });
//     },
//     select: function (event, ui) {
//         event.preventDefault();
//         $("#ModeloAutoComplete").val(ui.item.label);
//         KTDatatableRemoteAjax.recargar();
//     },
//     focus: function (event, ui) {
//         event.preventDefault();
//         $("#ModeloAutoComplete").val(ui.item.label);
//     },
//     //search: function (event, ui) {
//     //    event.preventDefault();
//     //}
// });
var ModalEditarCrear = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/ConsultaTablaMaestra/Registro/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro .modal-title').html(id === undefined ? 'Crear Registro' : 'Editar Registro');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $('#modal_registro').modal('show');
                listeners();
                RegistroPage.initValidacion();
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

        Inputmask({
            "mask": "9{0,4}.9{0,2}", // El 0,4 permite que de 0 a 4 dígitos enteros sean opcionales
            "numericInput": true
        }).mask(".form-input-mask-lamda");
        Inputmask({
            "mask": "9{0,4}.9{0,1}",
            "numericInput": true
        }).mask(".form-input-mask");
        $("#Marca").select2({
            dropdownParent: $('#modal_registro'),
        });
        $("#PROTOCOLO").select2({
            dropdownParent: $('#modal_registro'),
        });
        $("#IdRegistroSubMarca").select2({
            dropdownParent: $('#modal_registro'),

            ajax: {
                url: function (params) {
                    // Actualiza el valor del campo a medida que se realiza la búsqueda
                    let campoActualizado = $('#Marca').val();

                    // Retorna la URL de búsqueda con el valor actualizado
                    return `/ConsultaTablaMaestra/GetSubMarcas?marca=${campoActualizado}&&submarca=true`;
                },
                dataType: "json",
                delay: 2500,
                data: function (params) {
                    return {
                        q: params.term,
                        page: params.page,
                        records: 10,
                    }
                },
                processResults: function (data, params) {
                    params.page = params.page || 1
                    if (!data.isSuccessFully) {
                        Swal.fire({
                            title: "Ocurrió un problema",
                            text: result.message,
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn btn-light"
                            }
                        }).then(function () {
                        });

                    }
                    // if (data.result?.data?.length < 1) {
                    //     Swal.fire({
                    //         title: 'Falta Información del Vehículo',
                    //         text: "No se encontró la submarca del vehículo. ¿Desea continuar con el registro para notificar la falta de información a Secretaría de Medio Ambiente, Desarrollo Sustentable y Ordenamiento Territorial?",
                    //         icon: "warning",
                    //         buttonsStyling: false,
                    //         showCancelButton: true,
                    //         reverseButtons: true,
                    //         cancelButtonText: 'Cancelar',
                    //         confirmButtonText: "Confirmar",
                    //         focusConfirm: true,
                    //         customClass: {
                    //             confirmButton: "btn btn-primary",
                    //             cancelButton: 'btn btn-secondary'
                    //         }
                    //     }).then((result) => {
                    //         if (result.isConfirmed) {
                    //             if (params.term) {
                    //                 var newOption = new Option(params.term, -1, false, false);
                    //                 $('#Modelo').append(newOption).trigger('change');
                    //                 $('#Modelo').val(-1).trigger('change');
                    //             }
                    //         }
                    //     });
                    // }
                    return {
                        results: data.result.map(function (item) {
                            return {
                                id: item.value,
                                text: item.text
                            };
                        }),
                        pagination: {
                            more: params.page * 10 < data.recordsTotal,
                        },
                    }
                },
                cache: true,
            },
            placeholder: 'Ingresa el modelo del Vehículo',
            minimumInputLength: 2,
            language: 'es',
        });
        $('#IdRegistroSubMarca, #PROTOCOLO, #Marca').on('change', function () {
            // Activa la validación del campo select
            $(this).valid();
        });
        let select2 = $("#IdRegistroSubMarca");
        // Verifica si el valor no está vacío
        if (select2.val()) {
            // Selecciona la primera opción
            let value = select2.find('option:first').val()
            console.log('valor', value)
            select2.val(value).trigger('change.select2');
        }
        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            RegistroPage.init();
            return;
        });
    }
    $()
    // Cerramos la ventana modal
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#modal_registro').modal('hide');

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

var validation; // https://formvalidation.io/
var RegistroPage = function () {
    var guardar = function () {

        if ($("#form_tabla_maestra").valid()) {
            var form = $('#form_tabla_maestra')[0];
            var formData = new FormData(form);

            // Escucha los cambios en los campos de tipo checkbox
            $('#form_tabla_maestra input[type="checkbox"]').each(function () {
                var name = $(this).attr('name');
                var value = this.checked ? '1' : '0';

                // Actualiza el valor en el objeto FormData
                formData.set(name, value);
            });
            formData.set("IdCatSubmarcaVehiculo", "");
            formData.set("IdCatMarcaVehiculo", "");
            let jsonData = {}
            for (var pair of formData.entries()) {
                jsonData[pair[0]] = pair[1];
            }

            $.ajax({
                type: 'POST',
                url: siteLocation + "ConsultaTablaMaestra/Registro",
                data: jsonData,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, "Error");
                    } else {
                        toastr.success(result.message, "SMADSOT");
                        ModalEditarCrear.cerrarventanamodal();
                        KTDatatableRemoteAjax.recargar();
                        return;
                    }
                    return;
                },
                error: function (res) {
                    $(window).scrollTop(0);
                    toastr.error("Error al guardar la información", "Error");
                    blockUI.release();
                }
            });
        } else {
            toastr.error("Hay Errores en la información", "Error");

        }
    };
    var init_validacion = function () {
        $.validator.addMethod("lessThanHasta", function (value, element) {
            var hasta = parseInt(value);
            var desde = parseInt($("#ANO_DESDE").val());
            return desde <= hasta;
        }, "El valor debe ser mayor que el ANIO_DESDE");
        $('#form_tabla_maestra').validate({
            errorClass: 'is-invalid text-danger',
            errorElement: 'span',
            // Specify validation rules
            rules: {
                IdRegistroSubMarca: {
                    required: true
                },
                Marca: {
                    required: true
                },
                Motor_DSL: {
                    required: true,
                    digits: true
                },
                COMB_ORIG: {
                    required: true,
                    digits: true
                },
                CARROCERIA: {
                    required: true,
                    digits: true
                },
                ALIM_COMB: {
                    required: true,
                    digits: true
                },
                CILINDROS: {
                    required: true,
                    digits: true
                },
                CILINDRADA: {
                    required: true,
                    digits: true
                },
                PBV: {
                    required: true,
                    digits: true
                },
                PBV_EQUIV: {
                    required: true,
                    digits: true
                },
                PBV_ASM: {
                    required: true,
                    digits: true
                },
                CONV_CATAL: {
                    required: true,
                    digits: true
                },
                C_ABS: {
                    required: true,
                    digits: true
                },
                T_TRACC: {
                    required: true,
                    digits: true
                },
                C_TRACC: {
                    required: true,
                    digits: true
                },
                T_PRUEBA: {
                    required: true,
                    digits: true
                },
                PROTOCOLO: {
                    required: true,
                    digits: true
                },
                POTMAX_RPM: {
                    required: true,
                    digits: true
                },
                ANO_DESDE: {
                    required: true,
                    digits: true,
                    min: 1920,
                    max: new Date().getFullYear() + 1
                },
                ANO_HASTA: {
                    lessThanHasta: true,
                    required: true,
                    digits: true,
                    min: 1920,
                    max: new Date().getFullYear() + 1
                },
                O2_MAX: {
                    required: true,
                    number: true
                },
                LAMDA_MAX: {
                    required: true,
                    number: true
                },
                POT_5024: {
                    required: true,
                    number: true
                },
                POT_2540: {
                    required: true,
                    number: true
                },
            },
            messages: {
                IdRegistroSubMarca: {
                    required: 'El campo SUBMARCA es obligatorio',
                },
                Marca: {
                    required: 'El campo SUBMARCA es obligatorio',
                },
                Motor_DSL: {
                    required: 'El campo Motor DSL es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                COMB_ORIG: {
                    required: 'El campo COMB ORIG es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                CARROCERIA: {
                    required: 'El campo CARROCERIA es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                ALIM_COMB: {
                    required: 'El campo ALIM COMB es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                CILINDROS: {
                    required: 'El campo CILINDROS es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                CILINDRADA: {
                    required: 'El campo CILINDRADA es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                PBV: {
                    required: 'El campo PBV es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                PBV_EQUIV: {
                    required: 'El campo PBV EQUIV es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                PBV_ASM: {
                    required: 'El campo PBV ASM es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                CONV_CATAL: {
                    required: 'El campo CONV CATAL es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                C_ABS: {
                    required: 'El campo C ABS es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                T_TRACC: {
                    required: 'El campo T TRACC es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                C_TRACC: {
                    required: 'El campo C TRACC es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                T_PRUEBA: {
                    required: 'El campo T PRUEBA es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                PROTOCOLO: {
                    required: 'El campo PROTOCOLO es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                POTMAX_RPM: {
                    required: 'El campo POTMAX RPM es obligatorio',
                    digits: 'Ingrese un número entero válido'
                },
                ANO_DESDE: {
                    required: 'El campo Año Desde es obligatorio',
                    digits: 'Ingrese un número entero válido',
                    min: 'El valor debe ser igual o mayor que 1920',
                    max: 'El valor no puede ser mayor que el año actual más uno'
                },
                ANO_HASTA: {
                    required: 'El campo Año Hasta es obligatorio',
                    digits: 'Ingrese un número entero válido',
                    min: 'El valor debe ser igual o mayor que 1920',
                    max: 'El valor no puede ser mayor que el año actual más uno'
                },
                O2_MAX: {
                    required: 'El campo O2 MAX es obligatorio',
                    number: 'Ingrese un número decimal válido'
                },
                LAMDA_MAX: {
                    required: 'El campo LAMDA MAX es obligatorio',
                    number: 'Ingrese un número decimal válido'
                },
                POT_5024: {
                    required: 'El campo POT 5024 es obligatorio',
                    number: 'Ingrese un número decimal válido'
                },
                POT_2540: {
                    required: 'El campo POT 2540 es obligatorio',
                    number: 'Ingrese un número decimal válido'
                },
            }, errorPlacement: function (error, element) {
                if (element.prop('type') == 'select-one') {
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
        init: function () {
            guardar();
        },
        initValidacion: function () {
            init_validacion();
        }
    }
}();



jQuery(document).ready(function () {
    GuardarFormulario.init();
    KTDatatableRemoteAjax.init();
});