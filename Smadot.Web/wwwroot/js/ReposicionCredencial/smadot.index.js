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
                url: '/ReposicionCredencial/Consulta',
                type: 'POST',
                data: function (d) {
                }
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }
            ],
            columns: [
                { data: 'numeroTrabajador', name: 'NumeroTrabajador', title: 'Número de empleado' },
                { data: 'nombre', name: 'Nombre', title: 'Nombre de empleado' },
                { data: 'fechaStr', name: 'FechaRegistro', title: 'Fecha de registro' },
                { data: 'motivoReporteCredencial', name: 'MotivoReporteCredencial', title: 'Motivo del reporte' },
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
            order: [[2, 'desc']],
        });
        $('thead tr th').removeClass('fake-link');
        $('thead tr').addClass('fw-bold fs-6 text-gray-800');

        $(document).on('click', '.btnDetalle', function (e) {
            var id = $(this).data('id');
            blockUI.block();
            ModalDetalle.init(id);
        });

        $(document).on('click', '#btnRegistrar', function (e) {
            blockUI.block();
            ModalRegistro.init();
            blockUI.release();
        });
    };

    var recargar = function () {
        datatable.ajax.reload();
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
var ModalDetalle = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'ReposicionCredencial/Detalle/' + id,
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modalNotificacion2 .modal-title').html('Detalle');
                $('#modalNotificacion2 .modal-body').html('');
                $('#modalNotificacion2 .modal-dialog').addClass('modal-xl');
                $('#modalNotificacion2 .modal-body').html(result.result);
                var estatusCredencial = parseInt($('#modalNotificacion2 .IdCatEstatusReporteCredencial').val());
                if (estatusCredencial != 1) {
                    $('#modalNotificacion2 #btnValidar').hide();
                    $('#modalNotificacion2 #btnRechazar').hide();
                } else {
                    if ($('#modalNotificacion2 .modal-footer').length > 0) {
                        $('#modalNotificacion2 #btnValidar').show();
                        $('#modalNotificacion2 #btnRechazar').show();
                    } else {
                        $('#modalNotificacion2 #modalClass .modal-content').append('<div class="modal-footer" style="padding:10px">' +
                            '<button type="button" class="btn btn-secondary btn-sm font-weight-bold" data-bs-dismiss="modal" aria-label="Close" id="btnCerrarRegistro">Cerrar</button>' +
                            '<button type="button" class="btn btn-primary btn-sm font-weight-bold" id="btnValidar">Validar</button>' +
                            '<button type="button" class="btn btn-warning btn-sm font-weight-bold" id="btnRechazar">Rechazar</button>' +
                            '</div>');
                    }
                }
                const id = parseInt($('#modalNotificacion2 .IdReposicionCredencial').val())
                $('#modalNotificacion2').modal('show');
                listeners(id);
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var listeners = function (id) {
        $(function () {
            $('.descargarDoc').click(function () {
                DescargarDocumento.generar($(this).data('url'));

            });
        });

        $('#btnValidar').off().on('click', function (e) {
            $.ajax({
                cache: false,
                type: 'PUT',
                url: siteLocation + 'ReposicionCredencial/Aprobar?id=' + id,
                success: function (result) {
                    blockUI.release();
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, "SMADSOT");
                        return;
                    }
                    toastr.success(result.message, "SMADSOT");
                    $('#modalNotificacion2').modal('hide');
                    return;
                },
                error: function (res) {
                    toastr.error(res, "SMADSOT");
                    return;
                }
            });
        });

        $('#btnRechazar').off().on('click', function (e) {
            $.ajax({
                cache: false,
                type: 'PUT',
                url: siteLocation + 'ReposicionCredencial/Rechazar?id='+id,
                success: function (result) {
                    blockUI.release();
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, "SMADSOT");
                        return;
                    }
                    toastr.success(result.message, "SMADSOT");
                    $('#modalNotificacion2').modal('hide');
                    return;
                },
                error: function (res) {
                    toastr.error(res, "SMADSOT");
                    return;
                }
            });
        });        
    }
    var DescargarDocumento = function (url) {
        var generar = function (url) {
            $.ajax({
                cache: false,
                type: 'GET',
                processData: false,
                contentType: false,
                url: siteLocation + 'ReposicionCredencial/DescargarDocumento?url=' + url,
                success: function (result) {
                    if (!result.isSuccessFully) {
                        toastr.error(result.message, 'SMADSOT');
                        return;
                    }
                    var win = window.open();
                    win.document.write('<html><head><title>' + result.result.fileName + '</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64 + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');

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

    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarModalRegistro').click();
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
var ModalRegistro = function () {
    var fv; // https://formvalidation.io/

    var init = function () {
        abrirModal();
    }
    var abrirModal = function () {

        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'ReposicionCredencial/Registro',
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro .modal-title').html('Registro');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro .modal-dialog').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
                $('#modal_registro').modal('show');
                listeners();
                validate();
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

        $('#IdCatMotivoReporteCredencial').select2({
            minimumResultsForSearch: 2,
        });
        AutoComplete.init();
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

        myDropzone.on("addedfile", function (file) {
            $('#ArchivoCredencial').val(true);
        });

        $('#IdCatMotivoReporteCredencial').select2({
            minimumResultsForSearch: Infinity,
        });

        $("#IdCatMotivoReporteCredencial").select2({
            dropdownParent: $('#form_registro_reposicion_credencial'),
            placeholder: "Selecciona una motivo..."
        });

        $("#IdCatMotivoReporteCredencial").change(function () {
            const RoboExtravio = 1;
            const IdCatMotivoReporteCredencial = parseInt(this.value);
            if (IdCatMotivoReporteCredencial == RoboExtravio) {
                myDropzone.removeAllFiles(true);
                $('#ArchivoCredencial').val(true);
                $('#url-credencial').hide();
            }
            if (IdCatMotivoReporteCredencial == 2) {
                $('#ArchivoCredencial').val('');
                $('#url-credencial').show();
            }
            return;
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

        
    }

    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#btnCerrarRegistro').click();
    }

    var validate = () => {
        var form = document.getElementById('form_registro_reposicion_credencial');
        fv = FormValidation.formValidation(
            form,
            {
                fields: {
                    IdCatMotivoReporteCredencial: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    NombreUserPuestoVerificentro: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    Denuncia: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
                            }
                        }
                    },
                    ArchivoCredencial: {
                        validators: {
                            notEmpty: {
                                message: 'El campo es requerido.'
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

    const GuardarInfo = () => {

        fv.validate().then(async function (status) {
            /*validation.validate().then(async function (status) {*/
            if (status === 'Valid') {
                var form = $('#form_registro_reposicion_credencial')[0];
                var formData = new FormData(form);
                var files = [];
                files.push(Dropzone.forElement("#dropzonejs").files[0]);

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

                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: siteLocation + 'ReposicionCredencial/Registro',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error('Ocurrió un erro al registrar', 'SMADSOT');
                            return;
                        }
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                        location.href = siteLocation + 'ReposicionCredencial';
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, 'SMADSOT');
                    }
                });
            }
        });
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
var AutoComplete = function () {
    var init = function () {
        $("#NombreUserPuestoVerificentro").val('')
        $("#IdUserPuestoVerificentro").val('')
        $("#autocomplete").select2({
            dropdownParent: $("#modal_registro"),
            ajax: {
                url: siteLocation + 'ReposicionCredencial/Autocomplete',
                dataType: "json",
                delay: 2000,
                data: function (params) {
                    return {
                        q: params.term,
                        page: params.page,
                        records: 10,
                    }
                },
                processResults: function (data, params) {
                    params.page = params.page || 1
                    return {
                        results: data.items,
                        pagination: {
                            more: params.page * 10 < data.total_count,
                        },
                    }
                },
                cache: true,
            },
            placeholder: 'Ingresar número de empleado',
            minimumInputLength: 1,
            language: 'es',
        });

        $(document).on('change', '#autocomplete', function (e) {
            $("#NombreUserPuestoVerificentro").val('')
            $("#IdUserPuestoVerificentro").val('')
            $("#NombreUserPuestoVerificentro").val($("#autocomplete option:selected").text())
            $("#IdUserPuestoVerificentro").val(this.value)
        });
    };

    return {
        init: function () {
            init();
        }
    };
}();
jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});