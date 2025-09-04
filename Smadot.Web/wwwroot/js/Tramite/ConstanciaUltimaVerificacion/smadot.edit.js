"use strict"
var btns = false;
var myDropzone, myDropzone2, myDropzone3;
moment.locale('es');

var AutoComplete = function () {
    var init = function () {
        $("#ConsultaPlacaSerie").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: siteLocation + "Reposicion/ConsultaPlacaSerieAutocomplete",
                    type: "POST",
                    dataType: "json",
                    data: {
                        prefix: request.term
                    },
                    success: function (data) {
                        //response(data);
                        response($.map(data, function (item) {
                            return { label: item.nombre, value: item.id };
                        }));
                    }
                });
            },
            select: function (event, ui) {
                event.preventDefault();
                $("#ConsultaPlacaSerie").val(ui.item.label);
                $('#DivListData').hide();
                $('#ContentTable').html('');
                $("#ContentTable").hide();
                $('.DivData').html('');
                $(".DivData").hide();
                if (!ui.item.exento && !ui.item.administrativa) {
                    $.ajax({
                        url: siteLocation + "ConstanciaUltimaVerificacion/EditPartial",
                        type: "GET",
                        dataType: "json",
                        data: { id: ui.item.value },
                        success: function (data) {
                            if (data.error === false) {
                                $(".DivListData").show();
                                $('#ContentTable').html(data.result);
                                $("#ContentTable").show();
                                $('#btnGuardarRegistro').removeClass("d-none");
                                listenersData();
                            }
                        }
                    });
                } else {
                    var tipo = '';
                    if (ui.item.exento)
                        tipo = 'Exento';
                    else
                        tipo = 'Administrativa';
                    $.ajax({
                        url: siteLocation + "ConstanciaUltimaVerificacion/EditPartial",
                        type: "GET",
                        dataType: "json",
                        data: {
                            id: ui.item.value,
                            tipo: tipo,
                            placa: ui.item.value,
                            serie: ui.item.label.replace(ui.item.value + '/', '')
                        },
                        success: function (data) {
                            if (data.error === false) {
                                $('.DivData').html('');
                                $('.DivData').html(data.result);
                                $("#TipoReposicion").val(tipo);
                                $("#Placa").val(ui.item.value);
                                $("#Serie").val(ui.item.label.replace(ui.item.value + '/', ''));
                                $(".DivData").show();
                                $('#btnGuardarRegistro').removeClass("d-none");
                                listenersData();
                            }
                        }
                    });
                }
            },
            focus: function (event, ui) {
                event.preventDefault();
                $("#ConsultaPlacaSerie").val(ui.item.label);
            },         
        });
    };
    var listenersData = function () {

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

        $("#FechaEmisionRef, #FechaPago").daterangepicker({
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
        $('#EntidadProcedencia').select2();

        GuardarFormulario.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });
    }
    return {
        init: function () {
            init();
        }
    };
}();

//var ModalEdit = function () {

//    var init = function (id) {
//        abrirModal(id);
//    }
//    var abrirModal = function (id) {
//        $.ajax({
//            cache: false,
//            type: 'GET',
//            url: '/ConstanciaUltimaVerificacion/EditPartial/',
//            data: {
//                id: id,
//                verificacion: ViewBagId === '0' ? true : false
//            },
//            success: function (result) {
//                if (!result.isSuccessFully) {
//                    toastr.error(result.message, "SMADSOT");
//                    return;
//                }
//                $('#_datos').html(result.result);
//                $('#_datos').show();
//                listeners();
//                blockUI.release();
//                return;
//            },
//            error: function (res) {
//                toastr.error(res, "SMADSOT");
//                return;
//            }
//        });
//    }
//    // Inicializa los listeners de los botones relacionados a la ventana modal
//    var listeners = function () {
//        if (ViewBagId === '0') {
//            $('#btnGuardarRegistro').removeClass('d-none');
//            $(document).on('change', '.checkVerificacion', function (e) {
//                var check = $(this);
//                if (check.is(':checked')) {
//                    $('.checkVerificacion').prop('checked', false);
//                    check.prop('checked', true);
//                }
//            });

            
            
//        }
//        else {
//            var newOption = new Option($('#PlacaVerificacion').val(), $('#IdVerificacion').val(), false, false);
//            $('#busquedaPlacaSerie').append(newOption).trigger('change');
//            $('#busquedaPlacaSerie').prop('disabled', true);
//            $('.checkVerificacion').prop('checked', true).prop('disabled', true);
//            $('#NumeroReferencia').prop('disabled', true);
//            /*$('#tipoAutocomplete').prop('disabled', true);*/
//            $('#FechaEmisionRef').prop('disabled', true);
//            $('#FechaPago').prop('disabled', true);
//            $('#EntidadProcedencia').prop('disabled', true);

//            $(document).on('click', '.descargarDoc', function (e) {
//                DescargarDocumento.generar($(this).data('url'));
//            });
//        }
//    }

    
//}();

var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                //if ($('.checkVerificacion:checked').length !== 1) {
                //    toastr.error('Debe seleccionar una verificación.', "SMADSOT");
                //    return;
                //}
                var form = $('#form_registro')[0];
                var formData = new FormData(form);
                formData.set('IdFv', $("#IdFv").val());
                formData.set('FechaEmisionRef', formData.get('FechaEmisionRef').split('/').reverse().join('/'));
                formData.set('FechaPago', formData.get('FechaPago').split('/').reverse().join('/'));
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
                    formData.set('Files', JSON.stringify(files));
                }
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/ConstanciaUltimaVerificacion/Edit',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                        location.href = "/ConstanciaUltimaVerificacion"
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
                    'NumeroReferencia': {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    FechaEmisionRef: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                        }
                    }, FechaPago: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            },
                        }
                    }, EntidadProcedencia: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
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
        return await Promise.all(promises)
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

var DescargarDocumento = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: '/ConstanciaUltimaVerificacion/DescargarDocumento?url=' + url,
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

jQuery(document).ready(function () {
    AutoComplete.init();
    //if (ViewBagId !== '0')
    //    ModalEdit.init(ViewBagId)
    //else
    //    AutoComplete.init();
});