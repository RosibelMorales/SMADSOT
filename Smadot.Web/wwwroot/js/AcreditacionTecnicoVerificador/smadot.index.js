"use strict"

var datatable;
var myDropzone;

var KTDatatableRemoteAjax = function () {
    var init = function () {
        datatable = $('#kt_datatable').DataTable({
            language: {
                "decimal": "",
                "emptyTable": "No hay información",
                "info": "Mostrando _END_ de _TOTAL_ entradas",
                "infoEmpty": "Mostrando 0 de 0 entradas",
                "infoFiltered": "(Filtrado de _MAX_ total entradas)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Mostrar _MENU_ Entradas",
                "loadingRecords": "Cargando...",
                "processing": "Procesando...",
                "search": "Buscar:",
                "zeroRecords": "No se encontraron resultados para mostrar",
                "paginate": {
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
                url: siteLocation + 'AcreditacionTecnicoVerificador/Consulta',
                type: 'POST',
                data: function (d) {
                    //d.idAlmacen = $('#almacenGrid').select2('data')[0].id;
                }
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'numeroSolicitud', name: 'NumeroSolicitud', title: '# Solicitud' },
                { data: 'estatus', name: 'IdCatEstatusAcreditacion', title: 'Estatus' },
                {
                    data: 'urlAprobacion', name: 'UrlAprobacion', title: 'Aprobacion',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            var htmlString = "";
                            if ((row.urlAprobacion != "" && row.urlAprobacion != null)) {
                                htmlString = '<a class="btn btn-link btn-color-muted btn-active-color-primary" download="' + row.urlAprobacionString + '" href="' + row.urlAprobacionBase64 + '" title="Descargar" role="button">' + row.urlAprobacionString + '</a>';
                            } else {
                                htmlString = "";
                            }
                            return htmlString
                        }
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
            ],
        });
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');

        listeners();


        //$(document).on('click', '.btnPDFCertificado', function (e) {
        //    e.preventDefault();
        //    GenerarPDF.generar($(this).data('id'));
        //});
    };

    var recargar = function () {
        datatable.ajax.reload();
    }

    var listeners = function () {
        $(document).on('click', '.descargarDoc', function (e) {
            e.preventDefault()
            DescargarDocumento.generar($(this).data('url'), $(this).data('id'));
        });
        $('.descargarDoc').off()

        $(document).on("click", '.btnAutorizar', function (e) {
            e.preventDefault()
            let id = $(this).attr("data-id");
            AutorizarModal.init(id);
        })
        $('.btnAutorizar').off()
    }

    return {
        init: function () {
            init();
        },
        recargar: function () {
            recargar();
        }
    };
}();



//$(document).on('click', '.descargarDoc', function (e) {
//    DescargarDocumento.generar($(this).data('url'));
//});
//$('.descargarDoc').off()

//$(document).off().on("click", '.btnAutorizar', function () {
//    let id = $(this).attr("data-id");
//    console.log(id)
//    AutorizarModal.init(id);
//}) 
//$('.btnAutorizar').off()

var AutorizarModal = function () {
    var fv;
    var init = function (id) {
        abrirModal(id);
    }

    var abrirModal = function (id) {
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'AcreditacionTecnicoVerificador/Autorizar?id=' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Autorizar Acreditación');
                $('#btnGuardarRegistro').show()
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $('#btnGuardarRegistro').html('Autorizar')
                $('#modal_registro .modal-footer').html(
                    '<button type="button" class="btn btn-secondary btn-sm font-weight-bold" data-bs-dismiss="modal" aria-label="Close" id="btnCerrarRegistro">Cerrar</button>' +
                    (permisoAutorizar ? '<button type="button" class="btn btn-primary btn-sm font-weight-bold" id="btnGuardarRegistro">Autorizar</button>' : '') +
                    (permisoRechazar ? '<button type="button" class="btn btn-danger btn-sm font-weight-bold" id="btnRechazar">Rechazar</button>' : ''));
                $('#modal_registro').modal('show');
                listeners();
                validate();
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error("No es posible abrir la pantalla de autorización.", "SMADSOT");
                return;
            }
        });
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {
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

        $('#btnGuardarRegistro').off().on('click', function (e) {
            if (fv) {
                fv.validate().then(async function (status) {
                    if (status === 'Valid') {
                        GuardarInfo();
                    }
                });
            }
        });

        $('#btnRechazar').off().on('click', function (e) {
            if (fv) {
                fv.validate().then(async function (status) {
                    if (status === 'Valid') {
                        RechazarInfo();
                    }
                });
            }
        });
    }
    //// Cerramos la ventana modal
    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
    }

    var validate = () => {
        const form = document.getElementById('form_Autorizar');
        fv = FormValidation.formValidation(form, {
            fields: {
                //Nombre: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //    },
                //},
                //CorreoElectronico: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //        emailAddress: {
                //            message: 'La dirección de correo electronico no es valida',
                //        },
                //    },
                //},
                //Telefono: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //    },
                //},
                //Direccion: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //    },
                //},
                //Empresa: {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es obligatorio.',
                //        },
                //    },
                //},

            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap(),
                submitButton: new FormValidation.plugins.SubmitButton(),
                icon: new FormValidation.plugins.Icon({
                    validating: 'fa fa-refresh',
                }),
            },
        });

    }

    const GuardarInfo = () => {

        fv.validate().then(async function (status) {
            /*validation.validate().then(async function (status) {*/
            if (status === 'Valid') {
                var request = {}
                var files = [];

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

                request.Id = $("#Id")?.val();
                request.UrlAprobacionString = JSON.stringify(files)

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

                $.ajax({
                    cache: false,
                    type: 'Post',
                    contentType: 'application/json;charset=UTF-8',
                    url: siteLocation + 'AcreditacionTecnicoVerificador/Autorizar',
                    dataType: 'json',
                    data: JSON.stringify(request),
                    async: true,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        toastr.success('Registro guardado exitosamente', "SMADSOT");
                        $('#modal_registro').modal('hide');
                        KTDatatableRemoteAjax.recargar();

                        //KTDatatableRemoteAjax.recargar();
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

    const RechazarInfo = () => {

        fv.validate().then(async function (status) {
            /*validation.validate().then(async function (status) {*/
            if (status === 'Valid') {
                var request = {}
                var files = [];

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

                request.Id = $("#Id")?.val();
                request.UrlAprobacionString = JSON.stringify(files)

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

                $.ajax({
                    cache: false,
                    type: 'Post',
                    contentType: 'application/json;charset=UTF-8',
                    url: siteLocation + 'AcreditacionTecnicoVerificador/Rechazar',
                    dataType: 'json',
                    data: JSON.stringify(request),
                    async: true,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        toastr.success('Registro guardado exitosamente', "SMADSOT");
                        $('#modal_registro').modal('hide');
                        KTDatatableRemoteAjax.recargar();

                        //KTDatatableRemoteAjax.recargar();
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

    return {
        init: function (id) {
            init(id);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }
    }
}();

var DescargarDocumento = function (url, id) {
    var generar = function (url, id) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: siteLocation + 'AcreditacionTecnicoVerificador/DescargarDocumento?url=' + url + "&id=" + id,
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
        generar: function (url, id) {
            generar(url, id);
        }
    }

}();

//var GenerarPDF = function (id) {
//    var generar = function (id) {
//        $.ajax({
//            cache: false,
//            type: 'GET',
//            processData: false,
//            contentType: false,
//            enctype: 'multipart/form-data',
//            url: '/Refrendo/GetPDFCertificado/' + id,
//            success: function (result) {
//                if (!result.isSuccessFully) {
//                    toastr.error(result.message, 'SMADSOT');
//                    return;
//                }
//                var win = window.open();
//                win.document.write('<html><head><title>Certificado</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:application/pdf;base64,' + result.result + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
//                toastr.success('PDF generado correctamente.', 'SMADSOT');
//                return;
//            },
//            error: function (res) {
//                toastr.error(res, 'SMADSOT');
//            }
//        });
//    };

//    return {
//        generar: function (id) {
//            generar(id);
//        }
//    }

//}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});