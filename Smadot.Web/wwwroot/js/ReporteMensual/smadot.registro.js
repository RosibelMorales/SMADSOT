"use strict"
var btns = false;
var myDropzone, myDropzone2;

$(document).on('click', '#btnRegistro, .btnRegistro', function (e) {
    var id = $(this).data('id');

    ModalEdit.init(id);
});

$(document).on('click', '#btnEditar, .btnEditar', function (e) {
    var id = $(this).data('id');

    ModalUpdate.init(id);
});


var ModalEdit = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/ReporteMensual/Edit/' + id,
            data: function (d) {
                d.id = $('#motivoGrid').select2('data')[0].id;
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Registro de reportes' : 'Detalle de Linea');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#motivoGrid').select2({
                        dropdownParent: $('#modal_registro')
                    });
                });
                //if (!btns) {
                //   btns = true;
                //} 
                $('#modal_registro').modal('show');
                listeners();
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
        myDropzone = new Dropzone("#dropzonejs", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs", // Este es el contenedor donde se mostrarán las vistas previas
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
            acceptedFiles: "text/csv,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
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

        GuardarFormulario.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });


    }

    // Cerramos la ventana modal
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarRegistro').click();
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


var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };

    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_registroReporteMensual')[0];
                var formData = new FormData(form);
                formData.set('[0].IdReporte', formData.get('IDReporte'));
                formData.set('[0].NumeroReporte', formData.get('NumeroReporte'));
                formData.set('[0].PruebasRealizadas', formData.get('PruebasRealizas'));
                formData.set('[0].PruebasAprobadas', formData.get('PruebasAprobadas'));
                formData.set('[0].PruebasReprobadas', formData.get('PruebasReprobadas'));
                formData.set('[0].Entregados', formData.get('Entregados'));
                formData.set('[0].Cancelados', formData.get('Cancelados'));
                formData.set('[0].SinUsar', formData.get('SinUsar'));
                formData.set('[0].ServicioTransportePublico', formData.get('ServicioTransportePublico'));
                formData.set('[0].ServicioTransporteMercantil', formData.get('ServicioTransporteMercantil'));


                formData.set('[0].Cantidad', formData.get('DobleCeroCantidad'));
                formData.set('[0].FolioInicial', formData.get('DobleCeroFolioInicial'));
                formData.set('[0].FolioFinal', formData.get('DobleCeroFolioFinal'));

                formData.set('[1].Cantidad', formData.get('CeroCantidad'));
                formData.set('[1].FolioInicial', formData.get('CeroFolioInicial'));
                formData.set('[1].FolioFinal', formData.get('CeroFolioFinal'));

                formData.set('[2].Cantidad', formData.get('UnoCantidad'));
                formData.set('[2].FolioInicial', formData.get('UnoFolioInicial'));
                formData.set('[2].FolioFinal', formData.get('UnoFolioFinal'));

                formData.set('[3].Cantidad', formData.get('DosCantidad'));
                formData.set('[3].FolioInicial', formData.get('DosFolioInicial'));
                formData.set('[3].FolioFinal', formData.get('DosFolioFinal'));

                formData.set('[4].Cantidad', formData.get('ConstanciaNoAprobadoCantidad'));
                formData.set('[4].FolioInicial', formData.get('ConstanciaNoAprobadoFolioInicial'));
                formData.set('[4].FolioFinal', formData.get('ConstanciaNoAprobadoFolioFinal'));

                formData.set('[5].Cantidad', formData.get('ExentosCantidad'));
                formData.set('[5].FolioInicial', formData.get('ExentosFolioInicial'));
                formData.set('[5].FolioFinal', formData.get('ExentosFolioFinal'));

                var files = [];
                files.push(Dropzone.forElement("#dropzonejs").files[0]);
                files.push(Dropzone.forElement("#dropzonejs2").files[0]);
                files = files.filter(element => {
                    return element !== undefined;
                });

                if (files.length > 0) {
                    if (files.length > 0) {
                        var arrayOfBase64 = await readFile(files);
                        files = [];
                        for (let i = 0; i < arrayOfBase64.length; i++) {
                            files.push((arrayOfBase64[i]));
                        }
                        formData.set('Files', JSON.stringify(files));
                    }
                }
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/ReporteMensual/Edit',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                        ModalEdit.cerrarventanamodal();
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
        var form = document.getElementById('form_registroReporteMensual');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    PruebasRealizas: {
                        validators: {
                            notEmpty: {
                                message: 'La prueba realizada es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    PruebasAprobadas: {
                        validators: {
                            notEmpty: {
                                message: 'La prueba aprobada es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },   
                    PruebasReprobadas: {
                        validators: {
                            notEmpty: {
                                message: 'La prueba reprobada es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },      
                    Entregados: {
                        validators: {
                            notEmpty: {
                                message: 'El entregado es requerido.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },    
                    Cancelados: {
                        validators: {
                            notEmpty: {
                                message: 'El cancelado es requerido.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    SinUsar: {
                        validators: {
                            notEmpty: {
                                message: 'El campo "sin usar" es requerido.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ServicioTransportePublico: {
                        validators: {
                            notEmpty: {
                                message: 'El servicio de transporte público es requerido.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ServicioTransporteMercantil: {
                        validators: {
                            notEmpty: {
                                message: 'El servicio de transporte mercantil es requerido.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DobleCeroCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DobleCeroFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DobleCeroFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }

                    },
                    CeroCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    CeroFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    CeroFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    UnoCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    UnoFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    UnoFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DosCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DosFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    DosFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ConstanciaNoAprobadoCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ConstanciaNoAprobadoFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ConstanciaNoAprobadoFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ExentosCantidad: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ExentosFolioInicial: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
                            },
                        }
                    },
                    ExentosFolioFinal: {
                        validators: {
                            notEmpty: {
                                message: 'La cantidad es requerida.'
                            },
                            greaterThan: {
                                min: 0,
                                message: 'Solo se permiten números igual o mayor a 0',
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

        jQuery(document).ready(function () {
            var cantidad = getNumero($('#DobleCeroCantidad').val()) + getNumero($('#CeroCantidad').val()) + getNumero($('#UnoCantidad').val()) + getNumero($('#DosCantidad').val()) + getNumero($('#ConstanciaNoAprobadoCantidad').val()) + getNumero($('#ExentosCantidad').val());
            $('#Cantidad').val(cantidad);
            $('#CantidadCol').text(cantidad);        
        });

        $(document).on('change', '.cantidadInput', function (e) {

                var cantidad = getNumero($('#DobleCeroCantidad').val()) + getNumero($('#CeroCantidad').val()) + getNumero($('#UnoCantidad').val()) + getNumero($('#DosCantidad').val()) + getNumero($('#ConstanciaNoAprobadoCantidad').val()) + getNumero($('#ExentosCantidad').val());
                $('#Cantidad').val(cantidad);
                $('#CantidadCol').text(cantidad);        
        });
        var getNumero = function (val) {
            var res = parseInt(val);
            return isNaN(res) ? 0 : res;
        };

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

var ModalUpdate = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/ReporteMensual/Update/' + id,
            data: function (d) {
                d.id = $('#motivoGrid').select2('data')[0].id;
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Registro de reportes' : 'Editar reporte');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#motivoGrid').select2({
                        dropdownParent: $('#modal_registro')
                    });
                });
                //if (!btns) {
                //   btns = true;
                //} 
                $('#modal_registro').modal('show');
                listeners();
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
        myDropzone = new Dropzone("#dropzonejs", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs", // Este es el contenedor donde se mostrarán las vistas previas
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
            acceptedFiles: "text/csv,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
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

        GuardarFormulario.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });


    }

    // Cerramos la ventana modal
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarRegistro').click();
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


