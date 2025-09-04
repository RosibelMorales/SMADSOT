"use strict"
var btns = false;
var myDrop;

//Subir fotografía
$(document).on('click', '#btnUploadPhoto, .btnUploadPhoto', function (e) {
    var id = $(this).data('id');

    ModalUpload.init(id);
});

var ModalUpload = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Capacitacion/UploadPhoto/' + id,
            data: function (d) {
                d.id = $('#motivoGrid').select2('data')[0].id;
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_cierre #modalLabelTitleCierre').html(id === undefined ? 'Subir fotografía' : 'Subir fotografía');
                $('#modal_cierre .modal-body').html('');
                $('#modalClassCierre').addClass('modal-xl');
                $('#modal_cierre .modal-body').html(result.result);
                $("#modal_cierre").on('shown.bs.modal', function () {
                    $('#motivoSelect').select2({
                        dropdownParent: $('#modal_cierre')
                    });
                });
                //if (!btns) {
                //   btns = true;
                //} 
                $('#modal_cierre').modal('show');
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
        myDrop = new Dropzone("#dropzonejs3", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png",
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

        GuardarCierre.init();

        $('#btnGuardarCierre').off().on('click', function (e) {
            e.preventDefault();
            GuardarCierre.cerrar();
        });


    }

    // Cerramos la ventana modal
    var cerrarventanaCierre = function () {
        /*TablaCapacitacionEmpleado.recargar();*/
        $('#btnCerrarLinea').click();
    }

    return {
        init: function (id) {
            init(id);
        },
        cerrarventanamodalCierre: function () {
            cerrarventanaCierre();
        }
    }
}();

var GuardarCierre = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };

    var cerrar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_CapacitacionUpload')[0];
                var formData = new FormData(form);
                var id = formData.get('IdCapacitacionEmpleado')
                formData.set('[0].Id', formData.get('IdCapacitacionEmpleado'));
                formData.set('[0].IdEmpleado', formData.get('IdUserPuestoVerificentro'));
                formData.set('[0].Total', 2);
                var loc = window.location.pathname;
                var URLactual = jQuery(location).attr('pathname');
                //var pathName = loc.pathname.substring(0, loc.pathname.lastIndexOf('/') + 1);
                var files = []; 
                files.push(Dropzone.forElement("#dropzonejs3").files[0]);
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
                    url: '/Capacitacion/Edit',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        toastr.success('La linea se ha cerrado correctamente.', 'SMADSOT');
                        ModalUpload.cerrarventanamodalCierre();
                        //KTDataTableCapacitacion.recargar();
                        //var URLactual = jQuery(location).attr('pathname');
                        //var sentencia = URLactual + "," + "#tblCapacitacionEmpleado";
                        //$("#tblCapacitacionEmpleado").load(URLactual + "," + "#tblCapacitacionEmpleado");
                        //console.log(location);
                        //document.getElementById("tblCapacitacionEmpleado").reload();
                        setTimeout: 400,
                        window.location.reload();
                        //InitOverviewDataTable.ajax.reload();
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
        var form = document.getElementById('form_CapacitacionUpload');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    NotasMotivo: {
                        validators: {
                            notEmpty: {
                                message: 'La nota es requerida.'
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
        cerrar: function () {
            cerrar();
        }
    }

}();
