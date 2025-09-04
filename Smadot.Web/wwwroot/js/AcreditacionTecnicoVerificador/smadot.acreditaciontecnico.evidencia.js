"use strict"
var myDropzone;
var validator; // https://formvalidation.io/

var KTDatatableRemoteAjax = function () {
    var init = function () {
        $('.datepicker-js').daterangepicker({
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

        $('.datepicker-js2').daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            minYear: 1901,
            maxYear: parseInt(moment().format("YYYY"), 12),
            autoApply: true,
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

        $('input[name="FechaAmpliacion"]').on('apply.daterangepicker', function (ev, picker) {
            $(this).val(picker.startDate.format('DD/MM/YYYY'));
        });

        $('input[name="FechaAmpliacion"]').on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
        });

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

        $('.btnDelete').off().on('click',  function (e) {
            var id = $(this).data('id')
            var nombre = $(this).data('nombre')
            var us = $(this).data('us')
            var row = $(this)
            DeleteRegistro.init(id, nombre, row,us)
        })

        $(document).off().on('click', '.descargarDoc', function (e) {
            DescargarDocumento.generar($(this).data('url'));
        });
        $(document).off().on('click', '#btnSave', function (e) {

            if (!validator) {
                validation();
            }

            if (validator) {
                validator.validate().then(async function (status) {
                    if (status == 'Valid') {
                        //RegistrarVenta();
                        var files = [];
                        var empleados = [];
                        var erroresMensajes = [];
                        files.push(Dropzone.forElement("#dropzonejs1").files[0]);
                        files = files.filter(element => {
                            return element !== undefined;
                        });
                        if (files.length > 0) {
                            var arrayOfBase64 = await readFile(files);
                            files = [];
                            for (let i = 0; i < arrayOfBase64.length; i++) {
                                files.push((arrayOfBase64[i]));
                            }
                            //formData.set('Files', JSON.stringify(files));
                        }

                        async function readFile(fileList) {
                            function getBase64(file) {
                                let fileData = {};
                                fileData.Nombre = file == undefined ? "" : file.name;
                                fileData.Tipo = file == undefined ? "" : file.type.split("/")[1];

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
                        var Data = {
                            Id: $('#Id').val(),
                            NumeroSolicitud: $('#NumeroSolicitud').val(),
                            TipoTramite: $('#TipoTramite').val(),
                            NumeroAcreditacion: $('#NumeroAcreditacion').val(),
                            NumeroReferencia: $('#NumeroReferencia').val(),
                            FechaAcreditacion: moment($('#FechaAcreditacion').val(), "DD-MM-YYYY"),
                            FechaAmpliacion: moment($('#FechaAmpliacion').val(), "DD-MM-YYYY"),
                            FechaEmision: moment($('#FechaEmision').val(), "DD-MM-YYYY"),
                            UrlAcreditacionString: JSON.stringify(files),
                            EvidenciaEmpleados: []
                        }
                        var tb = $("#tb_Empleados").find('tbody tr').length;
                        var i;
                        for (i = 0; i < tb; i++) {

                            if ($(".tipo_" + i).val() == "" || $(".norma_" + i).val() == "") {
                                if ($(".tipo_" + i).val() == "" && $(".norma_" + i).val() == "") {
                                    var mensajeError = "Debe seleccionar el tipo de acreditación y la norma para el empleado " + $(".nombre_" + i).val() + ".";
                                    erroresMensajes.push(mensajeError);
                                    break;
                                } else if ($(".tipo_" + i).val() == "") {
                                    var mensajeError = "Debe seleccionar el tipo de acreditación para el empleado " + $(".nombre_" + i).val() + ".";
                                    erroresMensajes.push(mensajeError);
                                    break;
                                } else if ($(".norma_" + i).val() == "") {
                                    var mensajeError = "Debe seleccionar una norma para el empleado " + $(".nombre_" + i).val() + ".";
                                    erroresMensajes.push(mensajeError);
                                    break;
                                }

                            }

                            

                            var dataempl = {
                                IdUserPuesto: $(".user_" + i).val(),
                                Nombre: $(".nombre_" + i).val(),
                                Eliminado: $(".eliminado_" + i).val() == "True" ? true : false,
                                TipoAcreditacion: $(".eliminado_" + i).val() == "True" ? 1 : $(".tipo_" + i).val(),
                                NormaAcreditacion: $(".eliminado_" + i).val() == "True" ? 0 : $(".norma_" + i).val()
                            }
                            empleados.push(dataempl)
                        }

                        if (erroresMensajes.length > 0) {
                            toastr.error(erroresMensajes[0], "SMADSOT")
                            blockUI.release();
                            return;
                        }

                        Data.EvidenciaEmpleados = empleados;

                        $.ajax({
                            cache: false,
                            type: 'POST',
                            contentType: 'application/json;charset=UTF-8',
                            url: siteLocation + 'AcreditacionTecnicoVerificador/Evidencia',
                            dataType: 'json',
                            data: JSON.stringify(Data),
                            async: true,
                            success: function (result) {
                                if (!result.isSuccessFully) {
                                    toastr.error(result.message, "");
                                    blockUI.release();
                                } else {
                                    toastr.success("Usuario registrado exitosamente", "");
                                    window.location.href = siteLocation + 'AcreditacionTecnicoVerificador/Index';
                                    blockUI.release();
                                }
                                return;
                            },
                            error: function (res) {
                                toastr.error("Ocurrió un error al registrar la venta", "");
                                blockUI.release();
                            }
                        });
                    }
                    else {
                        blockUI.release();
                    }
                })
            }
        });

    }
    return {
        init: function () {
            init();
        }
    };
}();

var DeleteRegistro = function () {
    var init = function (id, nombre,row,us) {
        abrirModal(id, nombre, row,us);
    }
    var abrirModal = function (id, nombre, row,us) {

        $(row).closest('tr').remove();
        var html =
            '<tr>' +
            '<td>' + nombre + '</td>' +
            '<td colspan="3" style="text-align: -webkit-center;">Este usuario fue eliminado</td>' +
            '<input type="hidden" class="nombre_' + id + '" value="' + nombre + '" />' +
            '<input type="hidden" class="user_' + id + '" value="' + us + '" asp-for="' + us + '" />' +
            '<input type="hidden" class="eliminado_' + id + '" value="True" id="t_' + us + '">'

        $("#bodyEmpleados").append(html);
        $(".eliminado_" + id).val("True");
    }

    
    return {
        init: function (id, nombre, row,us) {
            init(id, nombre, row, us);
        },
    }
}();

var DescargarDocumento = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: siteLocation + 'AcreditacionTecnicoVerificador/DescargarDocumentoEvidencia?url=' + url,
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
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
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

var validation = function () {
    const form = document.getElementById('form_registro');
    validator = FormValidation.formValidation(
        form,
        {
            fields: {
                NumeroSolicitud: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                TipoTramite: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                NumeroAcreditacion: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                NumeroReferencia: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                FechaAcreditacion: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                FechaEmision: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
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




jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
    
});