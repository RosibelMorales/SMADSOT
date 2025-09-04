"use strict"
var myDropzone, myDropzone2, myDropzone3, myDropzone4, myDropzone5;
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
            acceptedFiles: "image/jpeg,image/jpg,image/png",
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
        //myDropzone5 = new Dropzone("#dropzonejs5", {
        //    autoProcessQueue: false,
        //    url: "/",
        //    maxFiles: 1,
        //    maxFilesize: 5, // MB
        //    addRemoveLinks: true,
        //    acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
        //    addedfiles: function (files) {
        //        if (files[0].accepted == false) {
        //            if (this.files.length >= 1)
        //                toastr.error("Elimine el documento antes de agregar uno nuevo.", 'SMADSOT')
        //            else
        //                toastr.error("Solo se permiten archivos de 5MB.", 'SMADSOT')
        //            this.removeFile(files[0])
        //        }
        //    }
        //});
        HorarioListeners.init();
    }
    return {
        init: function () {
            init();
        }
    };
}();
//function validarEmail(valor) {
//    //var email = $("#CorreoUsuario").val();
//    if (/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/.test(valor)) {
//        toastr.success("La dirección de email  es correcta");
//    } else {
//        toastr.error("La dirección de email es incorrecta");
//    }
//}
//function validarRFC(valor) {
//    //var email = $("#CorreoUsuario").val();
//    if (/^^[A-Z&Ñ]{3,4}[0-9]{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])[A-Z0-9]{2}[0-9A]$/.test(valor)) {
//        toastr.success("El RFC es correcto");
//    } else {
//        toastr.error("El RFC es invalido");
//    }
//}
function SoloNumeros(e, ControlId) {
    var letra = e.onkeypress.arguments[0].key;

    if (!/[^0-9]+$/g.test(letra)) {
        $("#" + ControlId).html('');
        return true;
    } else {
        $("#" + ControlId).html('Solo se aceptan numeros');
        return false;
    }
}
//function validarEmail() {
//    var regex = /[\w-\.]{2,}@@([\w-]{2,}\.)*([\w-]{2,}\.)[\w-]{2,4}/;

//    if (regex.test($('#CorreoUsuario').val().trim())) {
//        $("#CorreoUsuario").css("border-color", "green")
//        alert('Correo validado');
//        return true;
//    } else {
//        $("#CorreoUsuario").css("border-color", "red")
//        alert('La direccón de correo no es válida');
//        return false;
//    }
//}
var HorarioListeners = function () {
    var init = function () {
        $(".chkDay").change(function () {
            var dia = $(this).data('dia');
            if (this.checked) {
                $('#HoraInicio-' + dia).prop('disabled', false);
                $('#HoraFin-' + dia).prop('disabled', false);
                $('#HoraInicio-' + dia).val("08:00");
                $('#HoraFin-' + dia).val("20:00");

            } else {
                $('#HoraInicio-' + dia).prop('disabled', true);
                $('#HoraFin-' + dia).prop('disabled', true);
                $('#HoraInicio-' + dia).val('');
                $('#HoraFin-' + dia).val('');
            }
        });
    }
    return {
        init: function () {
            init();
        }
    };
}();
var validation = function () {
    const form = document.getElementById('form_registro');
    validator = FormValidation.formValidation(
        form,
        {
            fields: {
                Nombre: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                CorreoUsuario: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        },
                        regexp: {
                            regexp: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/i,
                            message: 'Por favor ingrese una dirección de correo electrónico válida'
                        },
                    }
                },
                TelefonoUsuario: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                FechaNacimiento: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                Genero: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                Rfc: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                Curp: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                FechaCapacitacionInicio: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                FechaCapacitacionFinal: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                FechaIncorporacion: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                IdPuesto: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                NumeroTrabajador: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
                'CheckHorarios[]': {
                    validators: {
                        choice: {
                            min: 1,
                            message: 'Seleccione al menos 1 horario'
                        }
                    }
                }
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

$(document).off().on('click', '.btnValidate', function (e) {
    Swal.fire({
        text: "Here's a basic example of SweetAlert!",
        icon: "success",
        buttonsStyling: false,
        confirmButtonText: "Ok, got it!",
        customClass: {
            confirmButton: "btn btn-primary"
        }
    });
});

$('#btnPreview').off().on('click', function (e) {
    e.preventDefault();
    GenerarPDF.generar();
});

$(document).off().on('click', '.descargarDoc', function (e) {
    DescargarDocumento.generar($(this).data('url'));
});

$(document).on('click', '#btnSave', function (e) {

    if (!validator) {
        validation();
    }

    if (validator) {
        validator.validate().then(async function (status) {
            if (status == 'Valid') {
                //RegistrarVenta();
                blockUI.block();
                var files = [];
                var filesFoto = [];
                var filesSeguroSocial = [];
                var filesFirma = [];
                /*var filesContrato = [];*/
                files.push(Dropzone.forElement("#dropzonejs1").files[0]);
                filesFoto.push(Dropzone.forElement("#dropzonejs2").files[0]);

                //files.push(Dropzone.forElement("#dropzonejs2").files[0]);
                filesSeguroSocial.push(Dropzone.forElement("#dropzonejs3").files[0]);
                filesFirma.push(Dropzone.forElement("#dropzonejs4").files[0]);
                /*filesContrato.push(Dropzone.forElement("#dropzonejs5").files[0]);*/
                files = files.filter(element => {
                    return element !== undefined;
                });
                filesFoto = filesFoto.filter(element => {
                    return element !== undefined;
                });
                filesSeguroSocial = filesSeguroSocial.filter(element => {
                    return element !== undefined;
                });
                filesFirma = filesFirma.filter(element => {
                    return element !== undefined;
                });
                //filesContrato = filesContrato.filter(element => {
                //    return element !== undefined;
                //});
                if (files.length > 0) {
                    var arrayOfBase64 = await readFile(files);
                    files = [];
                    for (let i = 0; i < arrayOfBase64.length; i++) {
                        files.push((arrayOfBase64[i]));
                    }
                    //formData.set('Files', JSON.stringify(files));
                }

                if (filesFoto.length > 0) {
                    var arrayOfBase64 = await readFile(filesFoto);
                    filesFoto = [];
                    for (let i = 0; i < arrayOfBase64.length; i++) {
                        filesFoto.push((arrayOfBase64[i]));
                    }
                    //formData.set('Files', JSON.stringify(files));
                }

                if (filesSeguroSocial.length > 0) {
                    var arrayOfBase64 = await readFile(filesSeguroSocial);
                    filesSeguroSocial = [];
                    for (let i = 0; i < arrayOfBase64.length; i++) {
                        filesSeguroSocial.push((arrayOfBase64[i]));
                    }
                    //formData.set('Files', JSON.stringify(files));
                }

                if (filesFirma.length > 0) {
                    var arrayOfBase64 = await readFile(filesFirma);
                    filesFirma = [];
                    for (let i = 0; i < arrayOfBase64.length; i++) {
                        filesFirma.push((arrayOfBase64[i]));
                    }
                    //formData.set('Files', JSON.stringify(files));
                }

                //if (filesContrato.length > 0) {
                //    var arrayOfBase64 = await readFile(filesContrato);
                //    filesContrato = [];
                //    for (let i = 0; i < arrayOfBase64.length; i++) {
                //        filesContrato.push((arrayOfBase64[i]));
                //    }
                //    //formData.set('Files', JSON.stringify(files));
                //}


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
                    IdUsuario: $('#IdUsuario').val(),
                    Nombre: $('#Nombre').val(),
                    CorreoUsuario: $('#CorreoUsuario').val(),
                    TelefonoUsuario: $('#TelefonoUsuario').val(),
                    FechaNacimiento: moment($('#FechaNacimiento').val(), "DD-MM-YYYY"),
                    Genero: $('#Genero').val(),
                    Rfc: $('#Rfc').val(),
                    Curp: $('#Curp').val(),
                    IdPuesto: $('#IdPuesto').val(),
                    NumeroTrabajador: $('#NumeroTrabajador').val(),
                    FechaCapacitacionInicio: moment($('#FechaCapacitacionInicio').val(), "DD-MM-YYYY"),
                    FechaCapacitacionFinal: moment($('#FechaCapacitacionFinal').val(), "DD-MM-YYYY"),
                    FechaIncorporacion: moment($('#FechaIncorporacion').val(), "DD-MM-YYYY"),
                    FechaAcreditacionNorma: $('#FechaAcreditacionNorma').val() == "" ? null : moment($('#FechaAcreditacionNorma').val(), "DD-MM-YYYY"),
                    HorarioRequest: [
                        { dia: $('#Dia-0').val(), horaInicio: $('#HoraInicio-0').val(), horaFin: $('#HoraFin-0').val() },
                        { dia: $('#Dia-1').val(), horaInicio: $('#HoraInicio-1').val(), horaFin: $('#HoraFin-1').val() },
                        { dia: $('#Dia-2').val(), horaInicio: $('#HoraInicio-2').val(), horaFin: $('#HoraFin-2').val() },
                        { dia: $('#Dia-3').val(), horaInicio: $('#HoraInicio-3').val(), horaFin: $('#HoraFin-3').val() },
                        { dia: $('#Dia-4').val(), horaInicio: $('#HoraInicio-4').val(), horaFin: $('#HoraFin-4').val() },
                        { dia: $('#Dia-5').val(), horaInicio: $('#HoraInicio-5').val(), horaFin: $('#HoraFin-5').val() },
                        { dia: $('#Dia-6').val(), horaInicio: $('#HoraInicio-6').val(), horaFin: $('#HoraFin-6').val() },
                    ],
                    FilesString: JSON.stringify(files),
                    FilesFotoString: JSON.stringify(filesFoto),
                    FilesSeguroSocialString: JSON.stringify(filesSeguroSocial),
                    FilesFirmaString: JSON.stringify(filesFirma),
                    /*FilesContratoString: JSON.stringify(filesContrato),*/
                }
                /*const archivos = [Data.FilesString, Data.FilesFotoString, Data.FilesSeguroSocialString, Data.FilesFirmaString, Data.FilesContratoString];*/
                const archivos = [Data.FilesString, Data.FilesFotoString, Data.FilesSeguroSocialString, Data.FilesFirmaString];

                if (archivos.includes('[]')) {
                    blockUI.release();
                    toastr.warning('Los documentos son obligatorios');
                    return;
                }

                $.ajax({
                    cache: false,
                    type: 'POST',
                    contentType: 'application/json;charset=UTF-8',
                    url: siteLocation + 'Personal/Registro',
                    dataType: 'json',
                    data: JSON.stringify(Data),
                    async: true,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "");
                            blockUI.release();
                        } else {
                            toastr.success("Usuario registrado exitosamente", "");
                            window.location.href = siteLocation + 'Personal/Index';
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


var GenerarPDF = function () {
    var init = function () {
    };
    var generar = function () {
        var Data = {
            IdUsuario: $('#IdUsuario').val(),
            Nombre: $('#Nombre').val(),
            CorreoUsuario: $('#CorreoUsuario').val(),
            FechaNacimiento: moment($('#FechaNacimiento').val(), "DD-MM-YYYY"),
            Genero: $('#Genero').val(),
            Rfc: $('#Rfc').val(),
            Curp: $('#Curp').val(),
            IdPuesto: $('#IdPuesto').val(),
            NumeroTrabajador: $('#NumeroTrabajador').val(),
            FechaCapacitacionInicio: moment($('#FechaCapacitacionInicio').val(), "DD-MM-YYYY"),
            FechaCapacitacionFinal: moment($('#FechaCapacitacionFinal').val(), "DD-MM-YYYY"),
            FechaIncorporacion: moment($('#FechaIncorporacion').val(), "DD-MM-YYYY"),
            FechaAcreditacionNorma: $('#FechaAcreditacionNorma').val() == "" ? null : moment($('#FechaAcreditacionNorma').val(), "DD-MM-YYYY"),
            //HorarioRequest: [
            //    { dia: $('#Dia-0').val(), horaInicio: $('#HoraInicio-0').val(), horaFin: $('#HoraFin-0').val() },
            //    { dia: $('#Dia-1').val(), horaInicio: $('#HoraInicio-1').val(), horaFin: $('#HoraFin-1').val() },
            //    { dia: $('#Dia-2').val(), horaInicio: $('#HoraInicio-2').val(), horaFin: $('#HoraFin-2').val() },
            //    { dia: $('#Dia-3').val(), horaInicio: $('#HoraInicio-3').val(), horaFin: $('#HoraFin-3').val() },
            //    { dia: $('#Dia-4').val(), horaInicio: $('#HoraInicio-4').val(), horaFin: $('#HoraFin-4').val() },
            //    { dia: $('#Dia-5').val(), horaInicio: $('#HoraInicio-5').val(), horaFin: $('#HoraFin-5').val() },
            //    { dia: $('#Dia-6').val(), horaInicio: $('#HoraInicio-6').val(), horaFin: $('#HoraFin-6').val() },
            //],
            Puesto: $('select[name="IdPuesto"] option:selected').text(),
            GeneroText: $('select[name="Genero"] option:selected').text()
        }
        $.ajax({
            cache: false,
            type: 'POST',
            contentType: 'application/json;charset=UTF-8',
            url: siteLocation + 'Personal/GetPDF',
            dataType: 'json',
            data: JSON.stringify(Data),
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>Vista Previa</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:application/pdf;base64,' + result.result + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
                toastr.success('PDF generado correctamente.', 'SMADSOT');
                return;
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
            }
        });
    };

    return {
        init: function () {
            init();
        },
        generar: function () {
            generar();
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
            url: '/Personal/DescargarDocumento?url=' + url,
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

