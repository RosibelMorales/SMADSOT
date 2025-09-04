"use strict"

var datatable;
var formData = new FormData();
var myDropzone

/*-------Tabla Principal-------*/

var KTDatatableRemoteAjax = function () {
    var init = function () {
        datatable = $('#OrdenServicioTable').DataTable({
            pageLength: 15,
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
                url: 'OrdenServicio/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'verificentro', name: '', title: 'Verificentro' },
                { data: 'equipo', name: 'NombreEquipo', title: 'Equipo' },
                { data: 'linea', name: 'NombreLinea', title: 'Linea' },
                { data: 'fechaRegistro', name: 'FechaRegistro', title: 'Fecha de Registro' },
                { data: 'folioServicio', name: 'FolioServicio', title: 'Folio del Servicio' },
                { data: 'userRegistro', name: 'NombreUsuario', title: 'Usuario Registró' },
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

/*-------Detalle-------*/

$(document).on('click', '.btnDetalle', function (e) {
    var id = $(this).attr('data-id');

    ModalDetalle.init(id);
});

$(document).on('click', '#btnOpenFile', function (e) {
    let url = $(this).attr("data-id");
    window.open(url);
    return false;
});

$(document).on('click', '.descargarDoc', function (e) {
    DescargarDocumento.generar($(this).data('url'));
});

var ModalDetalle = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "OrdenServicio/Detalle?id=" + id,
            success: function (result) {
                if (result.error) {
                    toastr.error(result.errorDescription, "SMADSOT");
                } else {

                    $('#modal_registro').removeClass('modal-md');
                    $('#modal_registro').removeClass('modal-xl');
                    $('#modal_registro').addClass('modal-lg');
                    $('#modal_registro #modalLabelTitle').html("Detalle Orden de Servicio");
                    $('#modal_registro .modal-body').html("");
                    $('#modal_registro .modal-body').html(result.result);
                    $('#modal_registro').modal('show');
                    $('#btnGuardarRegistro').hide();
                }

                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
    }

    return {
        init: function (id) {
            init(id);
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
            url: '/OrdenServicio/DescargarDocumento?url=' + url,
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

/*-------Registrar-------*/

$(document).on('click', '#btnRegistrar', function (e) {
    Modal.init();
});

var Modal = function () {
    var init = function () {
        abrirModal();
    }
    var abrirModal = function () {

        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "OrdenServicio/Registro",
            success: function (result) {
                if (result.error) {
                    toastr.error(result.errorDescription, "SMADSOT");
                } else {
                    $('#modal_registro').removeClass('modal-md');
                    $('#modal_registro').removeClass('modal-lg');
                    $('#modal_registro').addClass('modal-lg');
                    $('#modal_registro #modalLabelTitle').html("Registrar Orden de Servicio");
                    $('#modal_registro .modal-body').html("");
                    $('#modal_registro .modal-body').html(result.result);
                    $('#modal_registro').modal('show');
                    $('#btnGuardarRegistro').show();
                    listeners();
                }

                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
    }

    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {
        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarData.init();
        });
        $('#modal_registro').on('show.bs.modal', function (event) {
        });

        $.ajax({
            type: 'POST',
            url: 'OrdenServicio/CatTipoOrdenServicio',
            data: '{}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                //var datos = $.parseJSON(msg.d);

                $(msg).each(function () {
                    var option = $(document.createElement('option'));

                    option.text(this.text);
                    option.val(this.id);

                    $("#TipoOrdenServicio").append(option);
                });
            },
            error: function (msg) {
                $("#dvAlerta > span").text("Error al obtener Catalogo Tipo Orden Servicio");
            }
        });


        $("#autocomplete").select2({
            dropdownParent: $("#modal_registro"),
            width: '100%',
            ajax: {
                url: 'OrdenServicio/EquipoAutocomplete',
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
            placeholder: 'Ingresar Nombre / Numero de Serie...',
            minimumInputLength: 3,
            language: 'es',
        });

        $(document).on('change', '#autocomplete', function (data) {
            let id = this.value
            $("#_idEquipo").text(id);
            listenersData();
        });

        $(document).on('change', '#NoSerieActual', function (data) {
            let id = this.value
            if (id === "") {
                $(".divMessageInfo").hide()
            } else {
                $(".divMessageInfo").show()
            }
        });

    }

    /*-------Archivo-------*/
    var listenersData = function () {

        formData = new FormData();

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
        })
    }

    var cerrarModal = function () {
        $('#btnCerrarRegistro').click();
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

/*-------Guardar-------*/
var GuardarData = function () {

    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
        guardar();

    };
    var guardar = function () {
        validation.validate().then(async function (status) {

            if (status === 'Valid') {
                blockUI.block();

                formData.append("IdEquipo", $("#_idEquipo").text());
                formData.append("TipoOrdenServicio", $("#TipoOrdenServicio").val());
                formData.append("Folio", $("#FolioOS").val());
                formData.append("Solucion", $("#Solucion").val());
                formData.append("NoSerieActual", $("#NoSerieActual").val());
                //formData.append("NoSerieAnterior", $("#NoSerieAnterior").val());
                formData.append("Motivo", $("#Motivo").val());
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
                    formData.set('FileString', JSON.stringify(files));
                }

                $.ajax({
                    cache: false,
                    type: 'POST',
                    url: siteLocation + "OrdenServicio/CreateOrdenServicio",
                    processData: false,
                    contentType: false,
                    data: formData,
                    success: function (result) {
                        blockUI.release();
                        if (result.error) {
                            Modal.cerrarventanamodal();
                            toastr.error(result.errorDescription, '');
                        } else {

                            Modal.cerrarventanamodal();
                            toastr.success(result.errorDescription, '');
                            KTDatatableRemoteAjax.recargar();
                        }
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error("Error", '');
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
        var form = document.getElementById('form_registro');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    autocomplete: {
                        validators: {
                            notEmpty: {
                                message: 'Todos los datos son requeridos'
                            },
                        }
                    }

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
            init();
        }
    }
}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});