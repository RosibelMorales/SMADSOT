"use strict"
var myDropzone, myDropzone2, myDropzone3, myDropzone4, myDropzone5, myDropzone6, datatable;
var validator; // https://formvalidation.io/
var validators;
var validators2;
var isBusy = false;
var doblecero = false;
var ref00 = false;
var anioMax00 = Number(moment().year()) + 1;
var anioMin00 = Number(moment().year()) - 3;
var initPage = function () {
    var initDatatable = function () {
        // const tableRows = table.querySelectorAll('tbody tr');

        datatable = $('#kt_datatable').DataTable({
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
            aaSorting: [[1, 'asc']],
            ajax: {
                url: `/ConsultaTablaMaestra/Consulta`,
                type: 'POST',
                data: function (d) {
                    d.IdModelo = $("#IdCatMarcaVehiculo").val() ?? 0;
                    d.IdSubmarca = $("#Modelo").val() ?? 0;
                    d.ValidarCombustible = false;
                }
            },
            columnDefs: [{
                defaultContent: "-",
                targets: "_all"
            }],
            columns: [
                {
                    title: '',
                    orderable: false,
                    data: null,
                    defaultContent: '',
                    render: function (data, type, row) {
                        return `  <div class="form-check form-check-custom form-check-secondary form-check-solid">
                        <input class="form-check-input check-param form-check-secondary" type="checkbox" value="${row.id}" id="flexCheckChecked" />
                        </div>
                    `
                    }
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
            ]
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');

    }

    //var handleSearchDatatable = () => {
    //    const filterSearch = document.querySelector('[data-kt-filter="search"]');
    //    filterSearch.addEventListener('keyup', function (e) {
    //        datatable.search(e.target.value).draw();
    //    });
    //}

    var recargar = function () {
        // datatable.ajax.url(`/ConsultaTablaMaestra/Consulta?IdModelo=${$("#IdCatMarcaVehiculo").val() ?? 0}&&IdSubmarca=${$("#Modelo").val() ?? 0}&&IdCombustible=${$("#IdTipoCombustible").val() ?? 0}&&ValidarCombustible=false`);//+ '&Modelo=' + $("#ModeloAutoComplete").val().replaceAll(' ', ''));
        datatable.ajax.reload();
    }
    var showFactura = () => {

        let isValidYear = Number($("#Anio")?.val() ?? "0") >= anioMin00 && Number($("#Anio")?.val() ?? "0") <= anioMax00;
        let validMotivo = $("#IdMotivoVerificacion")?.val() === '6' || $("#IdMotivoVerificacion")?.val() === '15' || $("#IdMotivoVerificacion")?.val() === '18';
        if ((validMotivo && isValidYear && doblecero) || ($("#IdMotivoVerificacion")?.val() === '4' && isValidYear)) {
            $("#conteinerFechaFacturacion").removeClass('d-none');
            $("#divFactura").show();
            return true;
        } else {
            $("#conteinerFechaFacturacion").addClass('d-none')
            $("#divFactura").hide();
            return false;
        }
    }
    var valid00 = (motivoVerificacion) => {
        let isValidYear = Number($("#Anio")?.val() ?? "0") >= anioMin00 && Number($("#Anio")?.val() ?? "0") <= anioMax00;
        if (!isValidYear) {
            toastr.warning(`Para obtener el holograma 00 o refrendar el año modelo del vehículo debe estar entre ${anioMin00} - ${anioMax00}`)
            return false;
        } else if (!doblecero && Number(motivoVerificacion) == 6) {
            toastr.warning(`El registro seleccionado de tabla maestra no aplica para 00`)
            return false;
        } else if (!ref00 && Number(motivoVerificacion) == 15) {
            toastr.warning(`El registro seleccionado de tabla maestra no aplica para refrendar el holograma 00`)
            return false;
        }
        return true;
    }
    var init = function () {
        initDatatable();

        $('#Modelo').val($('#Modelo option:eq(0)').val()).trigger('change');
        GetSubmarcas();
        $(document).on('change', '#IdMotivoVerificacion', function (e) {
            if (this.value === '3') {
                $(".divPlacasUrl").show()
            } else {
                $(".divPlacasUrl").hide()
            }
            //if (this.value === '7') {
            //    $("#containerEstado").removeClass('d-none')
            //} else {
            //    $("#containerEstado").addClass('d-none')
            //}
        });
        $(document).on('change', '#IdMotivoVerificacion,#Anio', function (e) {
            showFactura();
        });
        $(document).on('change', '#PersonaMoral', function (e) {
            if (this.checked) {
                $(".titulo-check").html('Razón Social: &nbsp;')
            } else {
                $(".titulo-check").html('Nombre del Propietario: &nbsp;')
            }
        });
        $(document).on('change', '.check-param', function (e) {
            if (!this.checked) {
                $("#error-tabla").show()
                doblecero = false;
                ref00 = false;
            } else {
                $(".check-param").prop('checked', false);
                $(this).prop('checked', true);
                $("#error-tabla").hide()
                let row = $(this).closest('tr');
                let rowData = datatable.row(row).data();
                doblecero = Number(rowData.doblecero) === 1;
                ref00 = Number(rowData.reF_00) > 1;
            }
            showFactura();
        })

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
        if ($("#dropzonejs2")?.length > 0) {
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
        }
        if ($("#dropzonejs3")?.length > 0) {
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
        myDropzone4 = new Dropzone("#dropzonejs4", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs4", // Este es el contenedor donde se mostrarán las vistas previas
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
        myDropzone5 = new Dropzone("#dropzonejs5", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs5", // Este es el contenedor donde se mostrarán las vistas previas
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
        myDropzone6 = new Dropzone("#dropzonejs6", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs6", // Este es el contenedor donde se mostrarán las vistas previas
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
        $("#FechaFacturacion").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            minYear: 1901,
            maxYear: parseInt(moment().format("YYYY"), 12),
            autoApply: true,
            useCurrent: false,
            defaultDate: true,
            autoUpdateInput: false,
            locale: {
                format: 'DD/MM/YYYY',
                applyLabel: 'Aplicar',
                cancelLabel: 'Cancelar',
                daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
                monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
                weekLabel: "S"
            }
        });
        $("#FechaFacturacion").on('apply.daterangepicker', function (ev, picker) {

            let f1 = picker.startDate.format('DD/MM/YYYY');

            $(this).val(f1);

        });
        // $("#IdCilindros").select2();
        $(".divCilindros").hide()
        $(".divCilindrada").hide()
        // $("#IdCilindrada").select2();
        $("#IdTipoServicio").select2();
        $("#IdCatMarcaVehiculo").select2();
        $("#IdTipoCombustible").select2();
        // GetCilindros();
        $("#IdCatMarcaVehiculo").on("select2:select", function (e) {
            var selectedOption = e.params.data;
            var selectedText = selectedOption.text;
            // $("#Marca").val(selectedText);
            GetSubmarcas();
        });
        $("#IdTipoCombustible").on("select2:select", function (e) {
            recargar();

        });


        // $(document).on('change', "#IdCilindros", function (e) {
        //     $("#IdCilindrada").html('')
        //     $(".divCilindrada").hide()
        //     if (this.value !== "") {
        //         GetCilindrada(this.value)
        //     }
        // })
        // $("#IdCilindros").off()



    }
    return {
        init: function () {
            init();
        },
        reload: () => {
            recargar();
        },
        showFactura: () => {
            return showFactura();
        },
        valid00: (motivo) => {
            return valid00(motivo);
        }
    };
}();
var GetSubmarcas = function () {
    if ($('#Modelo').hasClass("select2-hidden-accessible")) {
        $('#Modelo').select2('destroy');
    }

    $("#Modelo").select2({
        ajax: {
            url: `/PortalCitas/GetSubMarcas?marca=${$("#IdCatMarcaVehiculo").val() ?? ""}&&submarca=true`,
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
    $('#Modelo').on('select2:select', function (e) {
        initPage.reload();
    });
    // $('#Marca').on('select2:select', function (e) {
    //     $(this).valid();
    // });
}
const ValidNumSerie = function () {
    return {
        validate: function (input) {
            return new Promise(function (resolve, reject) {
                $.ajax({
                    type: 'GET',
                    url: '/PortalCitas/Consulta',
                    data: {
                        numserie: $('#NumSerie').val(),
                        poblano: function () { return $('#IdfMotivoVerificacion').val() != 7; }
                    },
                    processData: false,
                    contentType: 'application / json; charset=UTF - 8',
                    success: function (result) {
                        resolve({
                            valid: isSuccessFully, // retornar el isSuccessfully
                            message: Swal.fire({
                                text: data.message,
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: {
                                    confirmButton: "btn btn-light"
                                }
                            })
                        });
                    },
                    error: function (res) {
                        resolve({
                            valid: false,
                            message: "La información no pudo ser procesada o el documento es inválido." // cambiar el mensaje
                        });
                    }
                });

            });
        }
    };
};
var validation = function () {
    const form = document.getElementById('form_registro');
    let anioMax = Number(moment().year()) + 1;
    let anioMin = 1910;
    FormValidation.validators.ValidNumSerie = ValidNumSerie;
    validator = FormValidation.formValidation(
        form,
        {
            fields: {
                FolioTarjeta: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                Color: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                Placa: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                ValidNumSerie: {
                    message: 'Número de serie inválido' // Colocar cualquier error custom
                },

                Serie: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                IdCatMarcaVehiculo: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                NombreAux: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                IdTipoServicio: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                IdMotivoVerificacion: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                FolioCertificadoAnterior: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                // IdCilindros: {
                //     validators: {
                //         notEmpty: {
                //             message: 'Este campo es obligatorio.'
                //         }
                //     }
                // },
                // IdCilindrada: {
                //     validators: {
                //         notEmpty: {
                //             message: 'Este campo es obligatorio.'
                //         }
                //     }
                // },
                Modelo: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                FechaFacturacion: {
                    validators: {
                        callback: {
                            callback: function (input) {
                                const inputValue = input.value.trim();
                                const container = $('#conteinerFechaFacturacion');
                                let isValid = true;
                                let errorMessage = '';



                                // Si el campo está vacío y el contenedor está oculto, es válido
                                if (inputValue === '' && container.hasClass('d-none')) {
                                    return {
                                        valid: true,
                                        message: 'El campo es obligatorio'
                                    };
                                }



                                // Intentar analizar la fecha con Moment.js
                                const parsedDate = moment(inputValue, 'DD/MM/YYYY', true);



                                // Verificar si la fecha es válida
                                if (!parsedDate.isValid()) {
                                    isValid = false;
                                    errorMessage = 'Por favor, ingrese una fecha válida en formato DD/MM/YYYY.';
                                }



                                return {
                                    valid: isValid,
                                    message: errorMessage
                                };
                            },
                        },
                    },
                },
                Estado: {
                    validators: {
                        callback: {
                            callback: function (input) {
                                const inputValue = input.value.trim();
                                const container = $('#containerEstado');
                                let isValid = true;
                                let errorMessage = '';



                                // Si el campo está vacío y el contenedor está oculto, es válido
                                if (inputValue === '' && !container.hasClass('d-none')) {
                                    return {
                                        valid: false,
                                        message: 'El campo es obligatorio'
                                    };
                                }



                                return {
                                    valid: isValid,
                                    message: errorMessage
                                };
                            },
                        },
                    },
                },
                NumeroReferencia: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                Anio: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        },
                        integer: {
                            message: 'El dato ingresado no es un año válido.',
                        },
                        between: {
                            min: anioMin,
                            max: anioMax,
                            message: `El año debe ser entre ${anioMin} y ${anioMax}`,
                        },
                    }
                },

            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap({
                    rowSelector: '.fv-row',
                })
            }
        }
    );
}

$(document).on('click', '#btnCancelarCita', function (e) {
    let id = $(this).attr("data-id");
    let folio = $(this).attr("data-folio");
    CancelarCita.init(id, folio);
})

$('#btnCancelarCita').off();

$(document).on('click', '#btnGuardarDoc', function (e) {

    let checkboxesMarcados = $('.check-param:checked');
    if (checkboxesMarcados?.length == 0)
        $("#error-tabla").show()
    if (!validator) {
        validation();
    }

    if (validator) {
        validator.validate().then(async function (status) {
            if (status == 'Valid') {
                if (checkboxesMarcados == 0) {
                    $("#error-tabla").show()
                    return toastr.error('Hay errores en la información, revise.', 'SMADSOT');
                }

                //RegistrarVenta();
                var fileIdentificacion = [];
                var fileFactura = [];
                var fileCartaFactura = [];
                var fileValidacionCertificado = [];
                var fileBajaPlaca = [];
                var fileAltaPlaca = [];

                if ($("#dropzonejs1")?.length > 0 && Dropzone.forElement("#dropzonejs1")?.files?.length > 0) {
                    fileIdentificacion.push(Dropzone.forElement("#dropzonejs1")?.files[0]);
                }
                if ($("#dropzonejs2")?.length > 0 && Dropzone.forElement("#dropzonejs2")?.files?.length > 0) {
                    fileFactura.push(Dropzone.forElement("#dropzonejs2")?.files[0]);
                }
                if ($("#dropzonejs3")?.length > 0 && Dropzone.forElement("#dropzonejs3")?.files?.length > 0) {
                    fileCartaFactura.push(Dropzone.forElement("#dropzonejs3")?.files[0]);
                }
                if ($("#dropzonejs4")?.length > 0 && Dropzone.forElement("#dropzonejs4")?.files?.length > 0) {
                    fileValidacionCertificado.push(Dropzone.forElement("#dropzonejs4")?.files[0]);
                }
                if ($("#dropzonejs5")?.length > 0 && Dropzone.forElement("#dropzonejs5")?.files?.length > 0) {
                    fileBajaPlaca.push(Dropzone.forElement("#dropzonejs5")?.files[0]);
                }
                if ($("#dropzonejs6")?.length > 0 && Dropzone.forElement("#dropzonejs6")?.files?.length > 0) {
                    fileAltaPlaca.push(Dropzone.forElement("#dropzonejs6")?.files[0]);
                }

                // fileIdentificacion = fileIdentificacion.filter(element => { return element !== undefined; });
                // fileFactura = fileFactura.filter(element => { return element !== undefined; });
                // fileCartaFactura = fileCartaFactura.filter(element => { return element !== undefined; });
                // fileValidacionCertificado = fileValidacionCertificado.filter(element => { return element !== undefined; });
                // fileBajaPlaca = fileBajaPlaca.filter(element => { return element !== undefined; });
                // fileAltaPlaca = fileAltaPlaca.filter(element => { return element !== undefined; });
                let cambioPlaca = ($("#IdfMotivoVerificacion").val() === '3' ? true : false);
                const formData = new FormData();
                formData.append('IdCitaVerificacion', $('#IdCita').val());
                formData.append('IdCatTipoServicio', $("#IdTipoServicio").val());
                formData.append('IdTipoCombustible', $("#IdTipoCombustible").val());
                formData.append('ColorVehiculo', $("#Color").val());
                formData.append('FolioTarjetaCirculacion', $('#FolioTarjeta').val());
                formData.append('CambioPlacas', cambioPlaca);
                formData.append('FechaFacturacion', $("#FechaFacturacion").val() ?? null);
                formData.append('FileIdentificacion', fileIdentificacion[0]);
                formData.append('FileFactura', fileFactura[0]);
                formData.append('FileValidacionCertificado', fileValidacionCertificado[0]);
                formData.append('FileBajaPlacas', fileBajaPlaca[0]);
                formData.append('FileAltaPlacas', fileAltaPlaca[0]);
                formData.append('NumeroReferencia', $("#NumeroReferencia").val());
                formData.append('Anio', $("#Anio").val());
                formData.append('Modelo', $("#Modelo").val());
                formData.append('Cilindros', 0);
                formData.append('Cilindrada', 0);
                formData.append('Diesel', null);
                formData.append('SubDiesel', null);
                formData.append('IdTablaMaestra', $('.check-param:checked:first').val());
                formData.append('NombrePersona', $("#NombrePersona").val());
                formData.append('NombreGeneraCita', $("#NombreGeneraCita").val());
                formData.append('PersonaMoral', $("#PersonaMoral").is(":checked"));
                formData.append('Estado', $("#Estado").val());
                formData.append('Placa', $("#Placa").val());
                formData.append('Serie', $("#Serie").val());
                formData.append('FileMulta', fileCartaFactura[0]);
                formData.append('IdMotivoVerificacion', $("#IdMotivoVerificacion").val());
                formData.append('FolioCertificadoAnterior', $("#FolioCertificadoAnterior").val());
                let tieneArchivos = false;
                let hasCf = initPage.showFactura();
                if (cambioPlaca) {
                    if (hasCf) {
                        tieneArchivos =
                            (fileIdentificacion?.length > 0) &&
                            (fileValidacionCertificado?.length > 0) &&
                            (
                                ($("#IdMotivoVerificacion").val() === '6' || $("#IdMotivoVerificacion").val() === '15') ?
                                    (fileFactura?.length > 0) :
                                    true
                            ) &&
                            (fileAltaPlaca?.length > 0) &&
                            (fileBajaPlaca?.length > 0);
                    } else {
                        tieneArchivos =
                            (fileIdentificacion?.length > 0) &&
                            (fileAltaPlaca?.length > 0) &&
                            (fileBajaPlaca?.length > 0) &&
                            (fileValidacionCertificado?.length > 0);
                    }
                } else {
                    if (hasCf) {
                        tieneArchivos =
                            (fileIdentificacion?.length > 0) &&
                            (fileValidacionCertificado?.length > 0) &&
                            (
                                ($("#IdMotivoVerificacion").val() === '6' || $("#IdMotivoVerificacion").val() === '15') ?
                                    (fileFactura?.length > 0) :
                                    true
                            );
                    } else {
                        tieneArchivos =
                            (fileIdentificacion?.length > 0) &&
                            (fileValidacionCertificado?.length > 0);
                    }
                }
                if (($("#IdMotivoVerificacion").val() === '6' || $("#IdMotivoVerificacion").val() === '15')) {
                    if (!initPage.valid00($("#IdMotivoVerificacion").val())) {
                        return;
                    }
                }
                if (!tieneArchivos && !$("#IdRecepcionDocumento").val()) {
                    toastr.warning('Los documentos son obligatorios');
                    return;
                }

                Swal.fire({
                    title: '¿Seguro que desea continuar?',
                    html: `
                    <p class="text-justify">
                    Una vez  guardada la información no podrá ser rectificada y podría afectar las pruebas de verificación. Por favor, Rectifique 2 veces la siguiente información: <br/><br/>
                    Nombre del Propietario: <b>${$("#NombrePersona")?.val()?.toUpperCase()}</b> <br/>
                    Tarjeta de Circulación:<b> ${$("#FolioTarjeta")?.val()}</b> <br/>
                    Placa: <b>${$("#Placa")?.val()}</b> <br/>
                    Número de Serie:<b> ${$("#Serie")?.val()}</b> <br/>
                    Folio Anterior: <b>${$("#FolioCertificadoAnterior")?.val()}</b> <br/>
                    </p>`,
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Aceptar',
                    cancelButtonText: 'Cancelar'
                }).then(async (result) => {
                    if (result.isConfirmed) {
                        const result = await $.ajax({
                            cache: false,
                            type: 'POST',
                            url: siteLocation + 'RecepcionDocumentos/Documentos',
                            data: formData, // Usar formData en lugar de Data
                            contentType: false, // Importante: no configurar el tipo de contenido
                            processData: false, // Importante: no procesar los datos
                            async: true,
                            success: function (result) {
                                if (!result.isSuccessFully) {
                                    toastr.error(result.message ?? "Ocurrió un error al obtener la información", "SMADSOT");
                                } else {
                                    Swal.fire({
                                        title: 'Recepción de Documentos',
                                        html: result.message,
                                        icon: "success",
                                        buttonsStyling: false,
                                        confirmButtonText: "Aceptar",
                                        customClass: {
                                            confirmButton: "btn btn-primary",
                                            cancelButton: 'btn btn-secondary'
                                        },
                                        allowOutsideClick: false
                                    }).then((resultSwal) => {
                                        window.location.href = siteLocation + 'RecepcionDocumentos';
                                    });
                                }
                                return;
                            },
                            error: function (res) {
                                toastr.error("Ocurrió un error al registrar la información", "SMADSOT");
                            }
                        });
                    }
                })
            }
            else {
                if (checkboxesMarcados == 0) {
                    $("#error-tabla").show()
                }
                toastr.error('Hay errores en la información, revise.', 'SMADSOT');
            }
        })
    }


});

$('#btnGuardarDoc').off();

var CancelarCita = function () {
    const init = (id, folio) => {
        abrirModal(id, folio);
    }
    var abrirModal = function (id, folio) {
        Swal.fire({
            title: '¿Está seguro que desea cancelar la cita?',
            text: "¡Está acción no se podrá revertir!",
            icon: "warning",
            buttonsStyling: false,
            showCancelButton: true,
            reverseButtons: true,
            cancelButtonText: 'Cancelar',
            confirmButtonText: "Confirmar",
            focusConfirm: true,
            customClass: {
                confirmButton: "btn btn-primary",
                cancelButton: 'btn btn-secondary'
            }
        }).then((result) => {
            if (result.isConfirmed) {

                var formData = new FormData();
                formData.append('Id', id);
                formData.append('Folio', folio);

                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "PortalCitas/CancelarCita",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formData,
                    success: function (result) {
                        var response = result.result;
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }

                        Swal.fire({
                            title: 'Cancelación exitosa',
                            text: "Su cita fue cancelada de manera correcta",
                            icon: "success",
                            buttonsStyling: false,
                            confirmButtonText: "Confirmar",
                            customClass: {
                                confirmButton: "btn btn-primary",
                                cancelButton: 'btn btn-secondary'
                            },
                            allowOutsideClick: false
                        }).then((resultSwal) => {
                            if (resultSwal.isConfirmed) {
                                /*window.location.href = siteLocation + 'PortalCitas/Index'*/
                                Swal.fire({
                                    title: 'Reagendar Cita',
                                    text: "¿Desea reagendar la cita?",
                                    icon: "question",
                                    buttonsStyling: false,
                                    reverseButtons: true,
                                    showCancelButton: true,
                                    confirmButtonText: "Confirmar",
                                    customClass: {
                                        confirmButton: "btn btn-primary",
                                        cancelButton: 'btn btn-secondary'
                                    },
                                    allowOutsideClick: false
                                }).then((resultSwal) => {
                                    if (resultSwal.isConfirmed) {
                                        /*window.location.href = siteLocation + 'PortalCitas/Index'*/

                                        ReagendarCitaModal.init()
                                    } else if (resultSwal.dismiss === Swal.DismissReason.cancel) {
                                        window.location.href = siteLocation + 'RecepcionDocumentos'
                                    }


                                });
                            }


                        });


                        return;
                    },
                    error: function (res) {
                        toastr.error(res, "SMADSOT");
                        return;
                    }
                });
            }


        });
    }
    var listeners = function () {

    }
    return {
        init: (id, folio) => {
            init(id, folio);
        }

    }
}()


var ReagendarCitaModal = function () {
    var fv;
    var init = function () {
        abrirModal();
    }

    var abrirModal = function () {

        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'RecepcionDocumentos/Reagendar',
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Reagendar Cita');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').removeClass('modalminH');
                $('#modal_registro .modal-body').addClass('modalminH');

                $('#modal_registro .modal-body').html(result.result);
                //$('#btnGuardarRegistro').hide()
                $('#modal_registro').modal('show');
                listeners()
                return;
            },
            error: function (res) {
                toastr.error("No es posible abrir la pantalla.", "SMADSOT");
                return;
            }
        });
        //blockUI.release();
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {
        ReagendarCitaAjax.init()
    }
    //// Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
    }

    return {
        init: function () {
            init();
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

// var GetCilindros = function () {
//     if ($('#IdCilindros').hasClass("select2-hidden-accessible")) {
//         $('#IdCilindros').select2('destroy');
//     }
//     $.ajax({
//         cache: false,
//         type: 'GET',
//         url: siteLocation + 'RecepcionDocumentos/GetCilindros?cita=' + $("#IdCita").val() + '&&IdMarca=' + $("#IdCatMarcaVehiculo").val() + '&&IdSubmarca=' + $("#Modelo").val(),
//         success: function (result) {
//             if (!result.isSuccessFully) {
//                 toastr.error(result.message, "SMADSOT");
//                 return;
//             }
//             //console.log(result.result)
//             //console.log(result.result.length)
//             $("#IdCilindros").select2('')
//             $("#IdCilindros").html('')
//             $("#IdCilindros").append('<option value="">Seleccione</option>')
//             if (result.result.length > 0) {
//                 var i = 0;
//                 for (i; i < result.result.length; i++) {
//                     //console.log(result.result[i])
//                     $("#IdCilindros").append('<option value="' + result.result[i].value + '">' + result.result[i].text + '</option>');
//                 }
//             }
//             if (result.result.length < 1) {
//                 Swal.fire({
//                     title: '¿Desea cancelar la cita?',
//                     text: "No se encontró la información del vehículo en la tabla maestra.",
//                     icon: "warning",
//                     buttonsStyling: false,
//                     showCancelButton: true,
//                     reverseButtons: true,
//                     cancelButtonText: 'Cancelar',
//                     confirmButtonText: "Confirmar",
//                     focusConfirm: true,
//                     customClass: {
//                         confirmButton: "btn btn-primary",
//                         cancelButton: 'btn btn-secondary'
//                     }
//                 }).then((result) => {
//                     if (result.isConfirmed) {


//                         var formData = new FormData();
//                         formData.append('Id', $("#IdCita").val());
//                         formData.append('Folio', $("#Folio").val());
//                         formData.append('ErrorTablaMaestra', true);

//                         $.ajax({
//                             cache: false,
//                             type: 'POST',
//                             //contentType: 'application/json;charset=UTF-8',
//                             // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
//                             /*enctype: 'multipart/form-data',*/
//                             url: siteLocation + "PortalCitas/CancelarCita",
//                             processData: false,
//                             contentType: false,
//                             //dataType: 'json',
//                             data: formData,
//                             success: function (result) {
//                                 if (!result.isSuccessFully) {
//                                     toastr.error(result.message, "SMADSOT");
//                                     return;
//                                 }

//                                 Swal.fire({
//                                     title: 'Cancelación exitosa',
//                                     text: "Su cita fue cancelada de manera correcta",
//                                     icon: "success",
//                                     buttonsStyling: false,
//                                     confirmButtonText: "Confirmar",
//                                     customClass: {
//                                         confirmButton: "btn btn-primary",
//                                         cancelButton: 'btn btn-secondary'
//                                     },
//                                     allowOutsideClick: false
//                                 }).then((resultSwal) => {
//                                     if (resultSwal.isConfirmed) {
//                                         window.location.href = siteLocation + 'RecepcionDocumentos'
//                                     }
//                                 });
//                                 return;
//                             },
//                             error: function (res) {
//                                 toastr.error(res, "SMADSOT");
//                                 return;
//                             }
//                         });
//                     }


//                 });
//             }
//             if (Cilindros) {
//                 $("#IdCilindros").val(Cilindros).trigger('change');
//             }
//             $(".divCilindros").show()
//             $(".divCilindrada").hide()
//             return;
//         },
//         error: function (res) {
//             toastr.error("Ocurrio un error.", "SMADSOT");
//             return;
//         }
//     });
// }
// var GetCilindrada = function (val) {
//     if ($('#IdCilindrada').hasClass("select2-hidden-accessible")) {
//         $('#IdCilindrada').select2('destroy');
//     } $.ajax({
//         cache: false,
//         type: 'GET',
//         url: siteLocation + 'RecepcionDocumentos/GetCilindrada?cita=' + $("#IdCita").val() + '&&IdMarca=' + $("#IdCatMarcaVehiculo").val() + '&&IdSubmarca=' + $("#Modelo").val() + '&&anio=' + $("#Anio").val() + '&&cilindros=' + val,
//         success: function (result) {
//             if (!result.isSuccessFully) {
//                 toastr.error(result.message, "SMADSOT");
//                 return;
//             }
//             //console.log(result.result)
//             //console.log(result.result.length)
//             $("#IdCilindrada").select2()
//             $("#IdCilindrada").html('')
//             $("#IdCilindrada").append('<option value="">Seleccione</option>')
//             if (result.result.length > 0) {
//                 var i = 0;
//                 for (i; i < result.result.length; i++) {
//                     //console.log(result.result[i])
//                     $("#IdCilindrada").append('<option value="' + result.result[i].value + '">' + result.result[i].text + '</option>');
//                 }
//             }
//             if (Cilindrada) {
//                 $("#IdCilindrada").val(Cilindrada).trigger('change');
//             }
//             $(".divCilindrada").show()
//             return;
//         },
//         error: function (res) {
//             toastr.error("Ocurrio un error.", "SMADSOT");
//             return;
//         }
//     });
// }



jQuery(document).ready(function () {
    initPage.init();
});