"use strict"
var myDropzone, myDropzone2, myDropzone3;

$(document).on('click', '#btnSave', function (e) {
    GuardarFormulario.guardar();
});

var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/
    let certificadosForm = [];
    let counts = {};
    var init = function () {
        let nameEl = "kt_repeater_basic"
        for (let i = 1; i <= 7; i++) {
            const nameIndexElement = nameEl + i;
            const ellement = $(`#${nameIndexElement}`)
            if (ellement)
                ellement.repeater({
                    initEmpty: false,
                    defaultValues: {
                        '[CantidadRecibida]':"0",
                    },
                    show: function () {
                        $(this).slideDown();
                        listeners();
                    },
                    hide: function (deleteElement) {
                        $(this).slideUp(deleteElement);
                    },
                    ready: function () {
                        listeners();
                    }
                });
        }
        init_validacion();
    };
    const listeners = () => {
        $(".cantidad-control,.folio-inicial-control").off().on('change', function (e) {
            let parent = $(this).closest('.form-group');
            let parentRow = $(this).closest('.certificado-row-form');
            let cantidad = parent.find("#certificado_CantidadRecibida");
            let folioInicial = parent.find("#certificado_FolioInicial");
            let folioFinal = parent.find("#certificado_FolioFinal");
            if (Number(cantidad.val()) > 0 && Number(folioInicial.val()) > 0) {
                folioFinal.val(Number(folioInicial.val()) + (Number(cantidad.val()) - 1));
            }
            UpdateRecibidas(parentRow.data('idcertificado'));
            validFormCartificados();
        });
    }
    const UpdateRecibidas = (id) => {
        const rowsCertificados = document.querySelector(`.certificado-row-form[data-idcertificado="${id}"]`);
        const inputsCantidad = rowsCertificados.querySelectorAll("#certificado_CantidadRecibida");
        let cantidad = 0;
        inputsCantidad.forEach((el, i) => {
            cantidad += Number(el.value ?? 0);
        })
        counts[id].update(cantidad);
    }
    const validFormCartificados = () => {
        let isValid = true;
        certificadosForm = [];
        let rowsCertificados = document.querySelectorAll('.repeat-item');
        let contenedor = document.getElementById('container-datacertificados');
        DeleteMessage(contenedor);
        rowsCertificados.forEach((el, i) => {
            let rowParent = el.closest('.certificado-row-form');
            let idTipoCertificado = rowParent.dataset.idcertificado;
            let cantidadEl = el.querySelector("#certificado_CantidadRecibida");
            let cantidad = cantidadEl?.value ?? 0;
            let folioInicialEl = el.querySelector("#certificado_FolioInicial");
            let folioInicial = folioInicialEl?.value ?? 0;
            let folioFinalEl = el.querySelector("#certificado_FolioFinal");
            let folioFinal = folioFinalEl?.value ?? 0;
            let idIngresoCertificado = el.querySelector("#certificado_IdIngresoCertificado")?.value ?? 0;
            if (cantidad < 0) {
                let element = Createmessage(["No se aceptan valores negativos."], true, cantidad.name);
                cantidadEl.parentNode.append(element);
            }
            if (folioInicial == 0 && cantidad > 0) {
                let element = Createmessage(["El folio inicial debe ser mayor a 0."], true, folioInicial.name);
                folioInicialEl.parentNode.append(element);
            }
            if (folioFinal == 0 && cantidad > 0) {
                let element = Createmessage(["El folio final no puede ser 0."], true, folioFinal.name);
                folioFinalEl.parentNode.append(element);
            }
            certificadosForm.push({
                idIngresoCertificado : idIngresoCertificado,
                idCatTipoCertificado: idTipoCertificado,
                folioInicial: folioInicial,
                folioFinal: folioFinal,
                cantidadRecibida: cantidad
            })
        })
        let sumaPorTipoCertificado = certificadosForm.reduce((accumulator, certificado) => {
            const { idCatTipoCertificado, cantidadRecibida } = certificado;
            accumulator[idCatTipoCertificado] = Number((accumulator[idCatTipoCertificado] || 0)) + Number(cantidadRecibida ?? 0);
            return accumulator;
        }, {});
        let sumatotal = 0;
        for (let key in sumaPorTipoCertificado) {
            const valorActual = sumaPorTipoCertificado[key];
            sumatotal += valorActual;
            const rowsCertificados = document.querySelector(`.certificado-row-form[data-idcertificado="${key}"]`);
            const contadorRecibidos = document.getElementById(`kt_countup_solicitadas_${key}`);
            const cantidadSolicitada = Number(contadorRecibidos?.dataset?.ktCountupValue ?? 0);
            if(valorActual>cantidadSolicitada){
                let element = Createmessage(["La cantidad ingresada supera la cantidad solicitada, rectifique las cantidades."], true, "table-total");
                rowsCertificados.append(element);
            }
        }
        if (sumatotal <= 0) {
            let element = Createmessage(["Debe establecer alguna serie de ingreso para guardar la información."], true, "table-total");
            contenedor.prepend(element);
        }
        let totalErrores = document.querySelectorAll('.tableMessage.fv-plugins-message-container')?.length;

        return totalErrores === 0;
    }
    const DeleteMessage = function (element) {
        element.querySelectorAll(".tableMessage.fv-plugins-message-container").forEach(e => e.remove());
    }
    const Createmessage = (messages, isTableMessage, dataField) => {
        var elementMessage = document.createElement('div');
        elementMessage.classList.add('fv-plugins-message-container');
        if (isTableMessage)
            elementMessage.classList.add('tableMessage');

        messages.forEach(message => {

            elementMessage.innerHTML += `<div data-field="${dataField}" data-validator="notEmpty" class="fv-help-block">${message}</div>`;
        });
        return elementMessage;
    }
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form')[0];
                var formData = new FormData(form);
                if (!validFormCartificados()) {
                    return toastr.error("Hay errores en la información", 'SMADSOT');
                }
                certificadosForm.forEach((certificado, index) => {
                    for (let key in certificado) {
                        formData.append(`certificados[${index}].${key}`, certificado[key]);
                        console.log(index, key, certificado[key])
                    }
                });
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
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/IngresoFormaValorada/Ingresos',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT', {
                            timeOut: 250,
                            preventDuplicates: true,

                            // Redirect 
                            onHidden: function () {
                                window.location.href = "/IngresoFormaValorada";
                            }
                        });
                    },
                    error: function (res) {
                        toastr.error(res, "SMADSOT");
                    }
                });
            }
        });
    };

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

    var init_validacion = function () {
        const rowCounts = document.querySelectorAll('.count-up-rc');
        rowCounts.forEach((el) => {
            counts[el.dataset.idcertificado] = new countUp.CountUp(`kt_countup_recibidas_${el.dataset.idcertificado}`);
        });
        $('#FechaEntrega, #FechaVenta').daterangepicker({
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
        $('#row-certificado-data-Tipo1').repeater({
            initEmpty: true,
            defaultValues: {
            },
            show: function () {
                $(this).slideDown();
            },
            hide: function (deleteElement) {
                $(this).slideUp(deleteElement);
            },
            ready: function () {
            }
        });
        var form = document.getElementById('form');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    NombreRecibio: {
                        validators: {
                            notEmpty: {
                                message: 'El nombre es requerido.'
                            },
                        }
                    },
                    // DobleCeroCantidadRecibida: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // CeroCantidadRecibida: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // UnoCantidadRecibida: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // DosCantidadRecibida: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // ConstanciaNoAprobadoCantidadRecibida: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // ExentosCantidadRecibida: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // DobleCeroCantidad: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // DobleCeroFolioInicial: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // DobleCeroFolioFinal: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }

                    // },
                    // CeroCantidad: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // CeroFolioInicial: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // CeroFolioFinal: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // UnoCantidad: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // UnoFolioInicial: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // UnoFolioFinal: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // DosCantidad: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // DosFolioInicial: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // DosFolioFinal: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // ConstanciaNoAprobadoCantidad: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // ConstanciaNoAprobadoFolioInicial: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // ConstanciaNoAprobadoFolioFinal: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // ExentosCantidad: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // ExentosFolioInicial: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                    // ExentosFolioFinal: {
                    //     validators: {
                    //         notEmpty: {
                    //             message: 'La cantidad es requerida.'
                    //         },
                    //         greaterThan: {
                    //             min: 0,
                    //             message: 'Solo se permiten números igual o mayor a 0',
                    //         },
                    //     }
                    // },
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap({
                        rowSelector: '.fv-row',
                    })
                }
            }
        );

        // $(document).on('change', '#DobleCeroFolioInicial, #DobleCeroCantidadRecibida', function (e) {
        //     var folioFinal = getNumero($('#DobleCeroFolioInicial').val()) + (getNumero($('#DobleCeroCantidadRecibida').val()) - 1);
        //     folioFinal = folioFinal <= 0 ? 0 : folioFinal;
        //     $('#DobleCeroFolioFinal').val(folioFinal);
        //     $('#DobleCeroFolioFinal').text(folioFinal);
        // });

        // $(document).on('change', '#CeroFolioInicial, #CeroCantidadRecibida', function (e) {
        //     var folioFinal = getNumero($('#CeroFolioInicial').val()) + (getNumero($('#CeroCantidadRecibida').val()) - 1);
        //     folioFinal = folioFinal <= 0 ? 0 : folioFinal;
        //     $('#CeroFolioFinal').val(folioFinal);
        //     $('#CeroFolioFinal').text(folioFinal);
        // });

        // $(document).on('change', '#UnoFolioInicial, #UnoCantidadRecibida', function (e) {
        //     var folioFinal = getNumero($('#UnoFolioInicial').val()) + (getNumero($('#UnoCantidadRecibida').val()) - 1);
        //     folioFinal = folioFinal <= 0 ? 0 : folioFinal;
        //     $('#UnoFolioFinal').val(folioFinal);
        //     $('#UnoFolioFinal').text(folioFinal);
        // });

        // $(document).on('change', '#DosFolioInicial, #DosCantidadRecibida', function (e) {
        //     var folioFinal = getNumero($('#DosFolioInicial').val()) + (getNumero($('#DosCantidadRecibida').val()) - 1);
        //     folioFinal = folioFinal <= 0 ? 0 : folioFinal;
        //     $('#DosFolioFinal').val(folioFinal);
        //     $('#DosFolioFinal').text(folioFinal);
        // });

        // $(document).on('change', '#ConstanciaNoAprobadoFolioInicial, #ConstanciaNoAprobadoCantidadRecibida', function (e) {
        //     var folioFinal = getNumero($('#ConstanciaNoAprobadoFolioInicial').val()) + (getNumero($('#ConstanciaNoAprobadoCantidadRecibida').val()) - 1);
        //     folioFinal = folioFinal <= 0 ? 0 : folioFinal;
        //     $('#ConstanciaNoAprobadoFolioFinal').val(folioFinal);
        //     $('#ConstanciaNoAprobadoFolioFinal').text(folioFinal);
        // });

        // $(document).on('change', '#ExentosFolioInicial, #ExentosCantidadRecibida', function (e) {
        //     var folioFinal = getNumero($('#ExentosFolioInicial').val()) + (getNumero($('#ExentosCantidadRecibida').val()) - 1);
        //     folioFinal = folioFinal <= 0 ? 0 : folioFinal;
        //     $('#ExentosFolioFinal').val(folioFinal);
        //     $('#ExentosFolioFinal').text(folioFinal);
        // });

        // $(document).on('change', '#TestificacionFolioInicial, #TestificacionCantidadRecibida', function (e) {
        //     var folioFinal = getNumero($('#TestificacionFolioInicial').val()) + (getNumero($('#TestificacionCantidadRecibida').val()) - 1);
        //     folioFinal = folioFinal <= 0 ? 0 : folioFinal;
        //     $('#TestificacionFolioFinal').val(folioFinal);
        //     $('#TestificacionFolioFinal').text(folioFinal);
        // });

        // $(document).on('change', '.cantidadInput', function (e) {
        //     if (this.id.includes('Recibida')) {
        //         var cantidadRecibida = getNumero($('#DobleCeroCantidadRecibida').val()) + getNumero($('#CeroCantidadRecibida').val()) + getNumero($('#UnoCantidadRecibida').val()) + getNumero($('#DosCantidadRecibida').val()) + getNumero($('#ConstanciaNoAprobadoCantidadRecibida').val()) + getNumero($('#ExentosCantidadRecibida').val()) + getNumero($('#TestificacionCantidadRecibida').val());
        //         $('#CantidadRecibida').val(cantidadRecibida);
        //         $('#CantidadRecibidaCol').text(cantidadRecibida);
        //     } else {
        //         var cantidad = getNumero($('#DobleCeroCantidad').val()) + getNumero($('#CeroCantidad').val()) + getNumero($('#UnoCantidad').val()) + getNumero($('#DosCantidad').val()) + getNumero($('#ConstanciaNoAprobadoCantidad').val()) + getNumero($('#ExentosCantidad').val()) + getNumero($('#TestificacionCantidad').val());
        //         $('#CantidadSolicitada').val(cantidad);
        //         $('#CantidadSolicitadaCol').text(cantidad);
        //     }
        // });
    }
    // var getNumero = function (val) {
    //     var res = parseInt(val);
    //     return isNaN(res) ? 0 : res;
    // };
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
        }
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
    return {
        init: function () {
            init();
        },
        guardar: function () {
            // if (result.isConfirmed) {
            guardar();
            // }
        }
    }

}();

jQuery(document).ready(function () {
    moment.locale('es');
    GuardarFormulario.init();
});