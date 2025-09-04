"use strict";

var validation; // https://formvalidation.io/
var myDropzone, myDropzone2, myDropzone3, myDropzone4;

var RegistroPage = function () {
    var imagen;
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var files = [];
                files.push(Dropzone.forElement("#dropzonejs1").files[0]);
                files.push(Dropzone.forElement("#dropzonejs2").files[0]);
                files.push(Dropzone.forElement("#dropzonejs3").files[0]);
                files.push(Dropzone.forElement("#dropzonejs4").files[0]);
                files = files.filter(element => {
                    return element !== undefined;
                });
                if (files.length > 0) {
                    var arrayOfBase64 = await readFile(files);
                    files = [];
                    for (let i = 0; i < arrayOfBase64.length; i++) {
                        files.push((arrayOfBase64[i]));
                    }
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
                var data = {
                    id: $("#Id")?.val(),
                    idCatTipoEquipo: $("#IdCatTipoEquipo").val(),
                    numeroSerie: $("#NumeroSerie").val(),
                    idLinea: $("#linea").val() == "" ? 0 : $("#linea").val(),
                    filesString: JSON.stringify(files)
                }
                data.id = data.id !== "null" ? data.id : 0;
                $.ajax({
                    cache: false,
                    type: 'POST',
                    contentType: 'application/json;charset=UTF-8',
                    url: siteLocation + "Equipo/Registro",
                    dataType: 'json',
                    data: JSON.stringify(data),
                    async: true,
                    success: function (result) {
                        if (result.error) {
                            toastr.error(result.errorDescription, "Error");
                        } else {
                            toastr.success("Datos guardados con exito", "SMADSOT");
                            ModalEditarCrear.cerrarventanamodal();
                            //KTDatatableRemoteAjax.recargar();
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
                // // KTUtil.scrollTop();
            }
        });
    };
    var init_validacion = function () {
        var form = document.getElementById('form_registro');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    IdTipoEquipo: {
                        validators: {
                            notEmpty: {
                                message: 'El nombre es requerido'
                            }
                        }
                    },
                    NumeroSerie: {
                        validators: {
                            notEmpty: {
                                message: 'El número de serie es requerido'
                            }
                        }
                    },
                    FechaRegistro: {
                        validators: {
                            notEmpty: {
                                message: 'La fecha es requerida'
                            }
                        }
                    },
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap()
                }
            }
        );
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
