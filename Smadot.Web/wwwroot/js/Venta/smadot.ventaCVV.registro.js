"use strict";

var validation; // https://formvalidation.io/
var myDropzone, myDropzone2, myDropzone3;

//var AutoCompleteNumeroSolicitud = function () {
//    var init = function () {
//        $("#busquedaSolicitud").select2({
//            ajax: {
//                url: '/VentaCVV/AutocompleteSolicitud',
//                dataType: "json",
//                delay: 2000,
//                data: function (params) {
//                    if ($('#IdAlmacen').val() === "") {
//                        toastr.error('Debe seleccionar un almacén.', "SMADSOT");
//                        return;
//                    }
//                    return {
//                        q: params.term,
//                        page: params.page,
//                        records: 10,
//                        idAlmacen: $('#IdAlmacen').val()
//                    }
//                },
//                processResults: function (data, params) {
//                    params.page = params.page || 1

//                    return {
//                        results: data.items,
//                        pagination: {
//                            more: params.page * 10 < data.total_count,
//                        },
//                    }
//                },
//                cache: true
//            },
//            placeholder: 'Ingresar número de solicitud',
//            minimumInputLength: 1,
//            language: 'es',
//        });

//        $(document).on('change', '#busquedaSolicitud', function (e) {
//            Registro.init();
//        }).on('keypress', '.select2-search__field', function () {
//            $(this).val($(this).val().replace(/[^\d].+/, ""));
//            if ((event.which < 48 || event.which > 57)) {
//                event.preventDefault();
//            }
//        });
//    };
//    return {
//        init: function () {
//            init();
//        }
//    };
//}();

//var Registro = function () {
//    var imagen;
//    var init = function () {
//        // $("#divForm").hide();
//        $("#divDocumentacion").hide();

//        obtenerInventario();
//    };
//    const obtenerInventario = () => {
//        $.ajax({
//            cache: false,
//            type: 'GET',
//            url: siteLocation + 'VentaCVV/ObtenerInventario/' + $("#busquedaSolicitud").val(),
//            success: function (result) {
//                if (!result.isSuccessFully) {
//                    toastr.error('Ocurrió un error al consultar.', "SMADSOT");
//                    blockUI.release();
//                    return;
//                } else {
//                    $("#divForm").show();
//                    $('#divForm .card-body').html('');
//                    $('#divForm .card-body').html(result.result);
//                    OperacionesTabla();
//                    blockUI.release();
//                }
//                return;
//            },
//            error: function (res) {
//                toastr.error(res, "SMADSOT");
//                blockUI.release();
//                return;
//            }
//        });
//    }
//    $(document).on('input', '#DobleCeroCantidad', function (e) {
//        var ImporteFV = parseFloat($("#ImporteFV").val());
//        var DobleCeroCantidad = parseInt($("#DobleCeroCantidad").val());
//        var DobleCeroCantidadOriginal = parseInt($("#DobleCeroCantidadOriginal").val());
//        var DobleCeroFolioInicialOriginal = parseInt($("#DobleCeroFolioInicialOriginal").val());

//        if (DobleCeroCantidad > DobleCeroCantidadOriginal) {
//            DobleCeroCantidad = DobleCeroCantidadOriginal;
//            $("#DobleCeroCantidad").val($("#DobleCeroCantidadOriginal").val());
//        }
//        if (isNaN(DobleCeroCantidad) || DobleCeroCantidad < 0) {
//            DobleCeroCantidad = 0;
//        }
//        var DobleCeroImporteTotal = ImporteFV * DobleCeroCantidad;
//        $("#DobleCeroCantidad").val(DobleCeroCantidad);
//        $("#DobleCeroFolioInicial").val(DobleCeroFolioInicialOriginal + DobleCeroCantidad);
//        $("#DobleCeroImporteTotal").val(DobleCeroImporteTotal.toFixed(2));
//    });

//    $(document).on('input', '#ExentosCantidad', function (e) {
//        var ImporteFV = parseFloat($("#ImporteFV").val());
//        var ExentosCantidad = parseInt($("#ExentosCantidad").val());
//        var ExentosCantidadOriginal = parseInt($("#ExentosCantidadOriginal").val());
//        var ExentosFolioInicialOriginal = parseInt($("#ExentosFolioInicialOriginal").val());

//        if (ExentosCantidad > ExentosCantidadOriginal) {
//            ExentosCantidad = ExentosCantidadOriginal;
//            $("#ExentosCantidad").val($("#ExentosCantidadOriginal").val());
//        }
//        if (isNaN(ExentosCantidad) || ExentosCantidad < 0) {
//            ExentosCantidad = 0;
//        }
//        var ExentosImporteTotal = ImporteFV * ExentosCantidad;
//        $("#ExentosCantidad").val(ExentosCantidad);
//        $("#ExentosFolioInicial").val(ExentosFolioInicialOriginal + ExentosCantidad);
//        $("#ExentosImporteTotal").val(ExentosImporteTotal.toFixed(2));
//    });

//    $(document).on('input', '#CeroCantidad', function (e) {
//        var CeroImporteIndividual = parseFloat($("#ImporteFV").val());
//        var CeroCantidad = parseInt($("#CeroCantidad").val());
//        var CeroCantidadOriginal = parseInt($("#CeroCantidadOriginal").val());
//        var CeroFolioInicialOriginal = parseInt($("#CeroFolioInicialOriginal").val());

//        if (CeroCantidad > CeroCantidadOriginal) {
//            CeroCantidad = CeroCantidadOriginal;
//            $("#CeroCantidad").val($("#CeroCantidadOriginal").val());
//        }
//        if (isNaN(CeroCantidad) || CeroCantidad < 0) {
//            CeroCantidad = 0;
//        }
//        var CeroImporteTotal = CeroImporteIndividual * CeroCantidad;
//        $("#CeroCantidad").val(CeroCantidad);
//        $("#CeroFolioInicial").val(CeroFolioInicialOriginal + CeroCantidad);
//        $("#CeroImporteTotal").val(CeroImporteTotal.toFixed(2));
//    });

//    $(document).on('input', '#UnoCantidad', function (e) {
//        var UnoImporteIndividual = parseFloat($("#ImporteFV").val());
//        var UnoCantidad = parseInt($("#UnoCantidad").val());
//        var UnoCantidadOriginal = parseInt($("#UnoCantidadOriginal").val());
//        var UnoFolioInicialOriginal = parseInt($("#UnoFolioInicialOriginal").val());

//        if (UnoCantidad > UnoCantidadOriginal) {
//            UnoCantidad = UnoCantidadOriginal;
//            $("#UnoCantidad").val($("#UnoCantidadOriginal").val());
//        }
//        if (isNaN(UnoCantidad) || UnoCantidad < 0) {
//            UnoCantidad = 0;
//        }
//        var UnoImporteTotal = UnoImporteIndividual * UnoCantidad;
//        $("#UnoCantidad").val(UnoCantidad);
//        $("#UnoFolioInicial").val(UnoFolioInicialOriginal + UnoCantidad);
//        $("#UnoImporteTotal").val(UnoImporteTotal.toFixed(2));
//    });

//    $(document).on('input', '#DosCantidad', function (e) {
//        var DosImporteIndividual = parseFloat($("#ImporteFV").val());
//        var DosCantidad = parseInt($("#DosCantidad").val());
//        var DosCantidadOriginal = parseInt($("#DosCantidadOriginal").val());
//        var DosFolioInicialOriginal = parseInt($("#DosFolioInicialOriginal").val());

//        if (DosCantidad > DosCantidadOriginal) {
//            DosCantidad = DosCantidadOriginal;
//            $("#DosCantidad").val($("#DosCantidadOriginal").val());
//        }
//        if (isNaN(DosCantidad) || DosCantidad < 0) {
//            DosCantidad = 0;
//        }
//        var DosImporteTotal = DosImporteIndividual * DosCantidad;
//        $("#DosCantidad").val(DosCantidad);
//        $("#DosFolioInicial").val(DosFolioInicialOriginal + DosCantidad);
//        $("#DosImporteTotal").val(DosImporteTotal.toFixed(2));
//    });

//    $(document).on('input', '#ConstanciaNoAprobadoCantidad', function (e) {
//        var ConstanciaNoAprobadoImporteIndividual = parseFloat($("#ImporteFV").val());
//        var ConstanciaNoAprobadoCantidad = parseInt($("#ConstanciaNoAprobadoCantidad").val());
//        var ConstanciaNoAprobadoCantidadOriginal = parseInt($("#ConstanciaNoAprobadoCantidadOriginal").val());
//        var ConstanciaNoAprobadoFolioInicialOriginal = parseInt($("#ConstanciaNoAprobadoFolioInicialOriginal").val());

//        if (ConstanciaNoAprobadoCantidad > ConstanciaNoAprobadoCantidadOriginal) {
//            ConstanciaNoAprobadoCantidad = ConstanciaNoAprobadoCantidadOriginal;
//            $("#ConstanciaNoAprobadoCantidad").val($("#ConstanciaNoAprobadoCantidadOriginal").val());
//        }
//        if (isNaN(ConstanciaNoAprobadoCantidad) || ConstanciaNoAprobadoCantidad < 0) {
//            ConstanciaNoAprobadoCantidad = 0;
//        }
//        var ConstanciaNoAprobadoImporteTotal = ConstanciaNoAprobadoImporteIndividual * ConstanciaNoAprobadoCantidad;
//        $("#ConstanciaNoAprobadoCantidad").val(ConstanciaNoAprobadoCantidad);
//        $("#ConstanciaNoAprobadoFolioInicial").val(ConstanciaNoAprobadoFolioInicialOriginal + ConstanciaNoAprobadoCantidad);
//        $("#ConstanciaNoAprobadoImporteTotal").val(ConstanciaNoAprobadoImporteTotal.toFixed(2));
//    });

//    $(document).on('click', '#btnSave', function (e) {
//        if (!validator) {
//            validation();
//        }

//        if (validator) {
//            validator.validate().then(async function (status) {
//                if (status == 'Valid') {
//                    //RegistrarVenta();
//                    blockUI.block();
//                    if (parseInt($("#DobleCeroCantidad").val()) == 0 &&
//                        parseInt($("#CeroCantidad").val()) == 0 &&
//                        parseInt($("#UnoCantidad").val()) == 0 &&
//                        parseInt($("#DosCantidad").val()) == 0 &&
//                        parseInt($("#ConstanciaNoAprobadoCantidad").val()) == 0 ) {
//                        toastr.warning('No hay certificados por registrar', 'SMADSOT');
//                        blockUI.release();
//                        return;
//                    }
//                    var files = [];
//                    files.push(Dropzone.forElement("#dropzonejs1").files[0]);
//                    files.push(Dropzone.forElement("#dropzonejs2").files[0]);
//                    files.push(Dropzone.forElement("#dropzonejs3").files[0]);
//                    files = files.filter(element => {
//                        return element !== undefined;
//                    });
//                    if (files.length > 0) {
//                        var arrayOfBase64 = await readFile(files);
//                        files = [];
//                        for (let i = 0; i < arrayOfBase64.length; i++) {
//                            files.push((arrayOfBase64[i]));
//                        }
//                        //formData.set('Files', JSON.stringify(files));
//                    }

//                    async function readFile(fileList) {
//                        function getBase64(file) {
//                            let fileData = {};
//                            fileData.Nombre = file.name;
//                            fileData.Tipo = file.type.split("/")[1];

//                            const reader = new FileReader()
//                            return new Promise((resolve) => {
//                                reader.onload = (ev) => {
//                                    var base64Data = ev.target.result.split('base64,')[1];
//                                    fileData.Base64 = base64Data;
//                                    resolve(fileData)
//                                }
//                                reader.readAsDataURL(file)
//                            })
//                        }

//                        const promises = []

//                        for (let i = 0; i < fileList.length; i++) {
//                            promises.push(getBase64(fileList[i]))
//                        }
//                        return await Promise.all(promises)
//                    }

//                    var Data = {
//                        FechaVenta: moment($('#FechaVenta').val(), "DD-MM-YYYY"),
//                        NumeroReferencia: $("#NumeroReferencia").val(),
//                        NumeroCompra: $("#NumeroCompra").val(),
//                        FilesString: JSON.stringify(files)
//                    }
//                    $.ajax({
//                        cache: false,
//                        type: 'POST',
//                        contentType: 'application/json;charset=UTF-8',
//                        url: siteLocation + 'VentaCVV/RegistrarVenta',
//                        dataType: 'json',
//                        data: JSON.stringify(Data),
//                        async: true,
//                        success: function (result) {
//                            if (!result.isSuccessFully) {
//                                var mensaje = (result.message != "" && result.message != null) ? result.message : "Ocurrió un error al registrar la venta";
//                                toastr.error(mensaje, "");
//                                blockUI.release();
//                                return
//                            } else {
//                                toastr.success("Venta registrada exitosamente", "");
//                                window.location.href = siteLocation + 'VentaCVV';
//                                blockUI.release();
//                                return
//                            }
//                            return;
//                        },
//                        error: function (res) {
//                            toastr.error("Ocurrió un error al registrar la venta", "");
//                            blockUI.release();
//                        }
//                    });
//                }
//                else {
//                    blockUI.release();
//                }
//            })
//        }
//    });

//    return {
//        init: function () {
//            init();

//        }
//    }
//}();
//var OperacionesTabla = function () {
//    myDropzone = new Dropzone("#dropzonejs1", {
//        autoProcessQueue: false,
//        url: "/",
//        maxFiles: 1,
//        maxFilesize: 5, // MB
//        addRemoveLinks: true,
//        acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
//        previewsContainer: "#dropzonejs1", // Este es el contenedor donde se mostrarán las vistas previas
//            init: function () {
//                this.on("addedfile", function (file) {
//                    if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
//                        return file.type.includes(acceptedType);
//                    })) {
//                        toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
//                        this.removeFile(file);
//                    } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
//                        toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
//                        this.removeFile(file);
//                    } else if (this.files.length > this.options.maxFiles) {
//                        toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
//                        this.removeFile(file);
//                    }
//                });
//                this.on("removedfile", function (file) {
//                    // Aquí puedes realizar acciones cuando un archivo se elimina
//                });
//            },
//    });
//    myDropzone2 = new Dropzone("#dropzonejs2", {
//        autoProcessQueue: false,
//        url: "/",
//        maxFiles: 1,
//        maxFilesize: 5, // MB
//        addRemoveLinks: true,
//        acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
//        previewsContainer: "#dropzonejs2", // Este es el contenedor donde se mostrarán las vistas previas
//        init: function () {
//            this.on("addedfile", function (file) {
//                if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
//                    return file.type.includes(acceptedType);
//                })) {
//                    toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
//                    this.removeFile(file);
//                } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
//                    toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
//                    this.removeFile(file);
//                } else if (this.files.length > this.options.maxFiles) {
//                    toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
//                    this.removeFile(file);
//                }
//            });
//            this.on("removedfile", function (file) {
//                // Aquí puedes realizar acciones cuando un archivo se elimina
//            });
//        },
//    });
//    myDropzone3 = new Dropzone("#dropzonejs3", {
//        autoProcessQueue: false,
//        url: "/",
//        maxFiles: 1,
//        maxFilesize: 5, // MB
//        addRemoveLinks: true,
//        acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
//        previewsContainer: "#dropzonejs3", // Este es el contenedor donde se mostrarán las vistas previas
//        init: function () {
//            this.on("addedfile", function (file) {
//                if (!this.options.acceptedFiles.split(',').some(function (acceptedType) {
//                    return file.type.includes(acceptedType);
//                })) {
//                    toastr.error("Tipo de archivo no aceptado.", 'SMADSOT');
//                    this.removeFile(file);
//                } else if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
//                    toastr.error("Tamaño de archivo excede el límite permitido.", 'SMADSOT');
//                    this.removeFile(file);
//                } else if (this.files.length > this.options.maxFiles) {
//                    toastr.error("Excedió el número máximo de archivos permitidos.", 'SMADSOT');
//                    this.removeFile(file);
//                }
//            });
//            this.on("removedfile", function (file) {
//                // Aquí puedes realizar acciones cuando un archivo se elimina
//            });
//        },
//    });

//    $('#FechaVenta').daterangepicker({
//        singleDatePicker: true,
//        showDropdowns: true,
//        minYear: 1901,
//        maxYear: parseInt(moment().format("YYYY"), 12),
//        autoApply: true,
//        locale: {
//            format: 'DD/MM/YYYY',
//            applyLabel: 'Aplicar',
//            cancelLabel: 'Cancelar',
//            daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
//            monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
//            weekLabel: "S"
//        }
//    });   
//};

//var validation = function () {
//    const form = document.getElementById('form');
//    validator = FormValidation.formValidation(
//        form,
//        {
//            fields: {
//                Verificentro: {
//                    validators: {
//                        notEmpty: {
//                            message: 'Este campo es obligatorio.'
//                        }
//                    }
//                },
//                IdAlmacen: {
//                    validators: {
//                        notEmpty: {
//                            message: 'Este campo es obligatorio.'
//                        }
//                    }
//                },
//                FechaVenta: {
//                    validators: {
//                        notEmpty: {
//                            message: 'Este campo es obligatorio.'
//                        }
//                    }
//                },
//                NumeroReferencia: {
//                    validators: {
//                        notEmpty: {
//                            message: 'Este campo es obligatorio.'
//                        }
//                    }
//                },
//            },
//            plugins: {
//                trigger: new FormValidation.plugins.Trigger(),
//                bootstrap: new FormValidation.plugins.Bootstrap({
//                    rowSelector: '.fv-row',
//                })
//            }
//        }
//    );
//}
//jQuery(document).ready(function () {
//    $('#IdVerificentro').val(null).trigger('change');
//    $('#IdVerificentro').on('change', function () {
//        blockUI.block();
//        $.ajax({
//            cache: false,
//            type: 'GET',
//            url: siteLocation + 'VentaCVV/ConsultarAlmacenes/' + $('#IdVerificentro').val(),
//            success: function (result) {
//                if (result) {
//                    $('#IdAlmacen').html('');
//                    var newOption = new Option("Seleccione...", null, false, false);
//                    $('#IdAlmacen').append(newOption).trigger('change');
//                    $('#IdAlmacen').val(null).trigger('change');
//                    if (result.length > 0) {
//                        result.forEach((v, i) => {
//                            var newOption = new Option(v.text, v.value, false, false);
//                            $('#IdAlmacen').append(newOption).trigger('change');
//                        })
//                    }
//                }
//                return;
//            },
//            error: function (res) {
//                toastr.error(res, "SMADSOT");
//                blockUI.release();
//                return;
//            }
//        });
//    });
//    AutoCompleteNumeroSolicitud.init();
//});

var RegistroPage = function () {
    var imagen;
    var init = function () {
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

        $("#FechaVenta").daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            minYear: 1901,
            maxYear: parseInt(moment().format("YYYY"), 12),
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

        $("#btnSave").click(function (e) {
            e.preventDefault();
            guardar();
        });
    };
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_registro')[0];
                var formData = new FormData(form);
                // certificadosForm.forEach((certificado, index) => {
                //     for (let key in certificado) {
                //         formData.append(`certificados[${index}].${key}`, certificado[key]);
                //         console.log(index, key, certificado[key])
                //     }
                // });
                let file1 = [];
                let file2 = [];
                let file3 = [];
                if ($("#dropzonejs1")?.length > 0 && Dropzone.forElement("#dropzonejs1")?.files?.length > 0) {
                    file1.push(Dropzone.forElement("#dropzonejs1")?.files[0]);
                }
                formData.append('documento1', file1[0]);
                if ($("#dropzonejs2")?.length > 0 && Dropzone.forElement("#dropzonejs2")?.files?.length > 0) {
                    file2.push(Dropzone.forElement("#dropzonejs2")?.files[0]);
                }
                formData.append('documento2', file2[0]);
                if ($("#dropzonejs3")?.length > 0 && Dropzone.forElement("#dropzonejs3")?.files?.length > 0) {
                    file3.push(Dropzone.forElement("#dropzonejs3")?.files[0]);
                }
                formData.append('documento3', file3[0]);
                var contadores = document.querySelectorAll('.contador-folios');
                let cantidadesDistintas = false;
                contadores.forEach((el, i,) => {
                    const solicitadasElement = el.querySelector('[id^="kt_countup_solicitadas_"]');

                    // Obtener elementos que contienen "kt_countup_recibidas_" en su ID
                    const recibidasElement = el.querySelector('[id^="kt_countup_recibidas_"]');

                    // Obtener los valores de los atributos data-kt-countup-value
                    const solicitadasValue = solicitadasElement.getAttribute('data-kt-countup-value');
                    const recibidasValue = recibidasElement.getAttribute('data-kt-countup-value');

                    // Convertir los valores a números y comparar
                    if (Number(solicitadasValue) !== Number(recibidasValue)) {
                        // Ambos valores son iguales
                        cantidadesDistintas = true;
                    }

                });
                if (cantidadesDistintas) {

                    Swal.fire({
                        title: '¿Seguro que desea finalizar la venta?',
                        text: "Hay certificados que no se han entregado en su totalidad, si finaliza la venta no podrá realizar más ingresos de formas valoradas al almacen del centro y el inventario estará incompleto.",
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#3085d6',
                        cancelButtonColor: '#d33',
                        confirmButtonText: 'Aceptar',
                        cancelButtonText: 'Cancelar'
                    }).then(async (result) => {
                        if (result.isConfirmed) {
                            sendForm(formData);
                        }
                    })
                } else {
                    sendForm(formData);
                }


            } else {
                // // KTUtil.scrollTop();
            }
        });
    };
    const sendForm = (formData) => {
        $.ajax({
            cache: false,
            type: 'POST',
            processData: false,
            contentType: false,
            enctype: 'multipart/form-data',
            url: '/VentaCVV/RegistrarVenta',
            data: formData,
            success: function (result) {
                if (!result.isSuccessFully) {
                    $(window).scrollTop(0);
                    toastr.error(result.message, "Error");
                } else {
                    toastr.success("Datos guardados con exito", "SMADSOT");
                    window.location.href = '/VentaCVV';
                    return;
                }
                return;
            },
            error: function (res) {
                $(window).scrollTop(0);
                toastr.error("Error al guardar la información", "Error");
            }
        });
    }
    var init_validacion = function () {
        var form = document.getElementById('form_registro');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    FechaVenta: {
                        validators: {
                            notEmpty: {
                                message: 'Este campo es obligatorio.'
                            }
                        }
                    },
                    NumeroCompra: {
                        validators: {
                            notEmpty: {
                                message: 'El número de compra es requerido'
                            }
                        }
                    },
                    NumeroReferencia: {
                        validators: {
                            notEmpty: {
                                message: 'El número de referencia es requerido'
                            }
                        }
                    },
                    // 'PrecioUnitario\\[\\]': {  // Corregir el selector del nombre del campo
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'El número de referencia es requerido'
                    //         }
                    //     }
                    // }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap()
                }
            }
        );
        let inputPrecio = document.querySelectorAll('input[name*="PrecioUnitario"]');
        let i = 0;
        while (i < inputPrecio?.length) {
            validation.addField(`certificadoVentas[${i}].PrecioUnitario`, {
                validators: {
                    notEmpty: {
                        message: 'La cantidad es requerida.'
                    },
                    greaterThan: {
                        min: 0,
                        message: 'Solo se permiten números igual o mayor a 0',
                    },
                    callback: {
                        message: 'El formato del número es incorrecto',
                        callback: function (input) {
                            var value = input.value;

                            // Expresión regular actualizada
                            var regex = /^\d+(\.\d{1,2})?$/;
                            // Devuelve true si el formato es válido y no termina con un punto, de lo contrario, false
                            return regex.test(value) && !value.endsWith('.');

                        }
                    }
                }
            });
            i++;
        }
    }
    return {
        init: function () {
            init();
            init_validacion();
        }
    }
}();
$(document).ready(function () {
    RegistroPage.init();
});
