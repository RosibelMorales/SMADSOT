"use strict"

var datatable;
var formData = new FormData();
var myDropzone, myDropzone2, myDropzone3;

var KTDatatableRemoteAjax = function () {
    var init = function () {
        datatable = $('#kt_datatable').DataTable({
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
                url: 'Reposicion/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                //{ data: 'idVerificacion', name: 'IdVerificacion', title: 'Verificacion' },
                { data: 'fechaRegistro', name: 'FechaRegistro', title: 'Fecha' },
                { data: 'numeroReferencia', name: 'NumeroReferencia', title: 'Número de referencia' },
                { data: 'usuarioRegistro', name: 'UsuarioRegistro', title: 'Usuario registro' },
                { data: 'placa', name: 'Placa', title: 'Placa' },
                { data: 'serie', name: 'Serie', title: 'Serie' },
                { data: 'folio', name: 'Folio', title: 'Folio' },
                { data: 'claveTramite', name: 'ClaveTramite', title: 'Clave Trámite' },
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

$(document).on('click', '#btnRegistrar', function (e) {
    /*var id = $(this).data('id');*/
    Modal.init();
});
$(document).on('click', '.btnDetalle', function (e) {
    var id = $(this).data('id');

    ModalDetalle.init(id);
});

$(document).on('click', '.btnGenerarCertificado', function (e) {
    var id = $(this).data('id');
    GenerarPDF.generar(id);
});

$(document).on('click', '.btnEliminar', function (e) {
    e.preventDefault();
    EliminarRegistro.eliminar($(this).data('id'));
});

var Modal = function () {
    var init = function () {
        abrirModal();
    }
    var abrirModal = function () {

        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Reposicion/Registro",
            success: function (result) {
                if (result.error) {
                    toastr.error(result.errorDescription, "SMADSOT");
                } else {

                    $('#modal_registro').removeClass('modal-md');
                    $('#modal_registro').removeClass('modal-lg');
                    $('#modal_registro').addClass('modal-xl');
                    $('#modal_registro #modalLabelTitle').html("Registrar Reposición");
                    $('#modal_registro .modal-body').html("");
                    /*$('#modal_registro .modal-body').html("<div class='row mb-5 mt-5'><form role='form' class='clearfix'><div class='row fv-row mb-5'><div class='col-md-12'><div class='form-group'><label for='name'>Mensaje: &nbsp;</label><textarea class='form-control' id='Mensaje'></textarea></div></div></div></div></form></div>");*/
                    $('#modal_registro .modal-body').html(result.result);
                    $('#modal_registro').modal('show');
                    $('#btnGuardarRegistro').hide();
                    listeners();
                }

                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });

        /*listeners();*/
    }
    // Inicializa los listeners de los botones relacionados a la ventana modal
    var listeners = function () {
        $('#btnGuardarRegistro').off().on('click', function (e) {
            //toastr.success('Registro guardado exitosamente', "SMADSOT");
            //$('#modal_registro').modal('hide');
            //SendEmailData.init();
            e.preventDefault();
            GuardarData.init();
        });
        $('#modal_registro').on('show.bs.modal', function (event) {
            //$('#Mensaje').summernote();
            /*$('#Mensaje').summernote();*/

        });

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
                            return { label: item.nombre, value: item.id};
                        }));
                    }
                });
            },
            select: function (event, ui) {
                event.preventDefault();
                $("#ConsultaPlacaSerie").val(ui.item.label);

                //$("#FolioId").val('');
                //$("#Tipo").val('');
                //$("#Fecha").val('');
                //$("#Usuario").val('');
                //$("#Vehiculo").val('');
                //$("#Persona").val('');
                //$("#FechaC").val('');
                //$("#Motivo").val('');
                //$("#Otro").val('');
                //$("#IdF").val('');
                ////limpiar busqueda
                $(".DivListData").hide();
                $('#ContentTable').html('');
                $("#ContentTable").hide();
                $('.DivData').html('');
                $(".DivData").hide();
                if (!ui.item.exento && !ui.item.administrativa) {
                    $.ajax({
                        url: siteLocation + "Reposicion/ConsultaDataVerificacion",
                        type: "GET",
                        dataType: "json",
                        data: { id: ui.item.value },
                        success: function (data) {
                            if (data.error === false) {
                                $(".DivListData").show();
                                $('#ContentTable').html(data.result);
                                $("#ContentTable").show();
                                listenersTable();
                                //$("#DetalleFolio").show();
                                ////establecerBusqueda(data.Data);
                                //$("#FolioId").val(data.data.folio);
                                //$("#Tipo").val(data.data.tipoTramite);
                                //$("#Fecha").val(data.data.fecha);
                                //$("#Usuario").val(data.data.datosUsuario);
                                //$("#Vehiculo").val(data.data.datosVehiculo);
                                //$("#Persona").val(data.data.personaRealizoTramite);
                                //$("#FechaC").val(data.data.fechaCancelacion);
                                //$("#Motivo").val(data.data.idCatMotivoCancelacion);
                                //if (data.data.idCatMotivoCancelacion == "3") {
                                //    $("#InputOtro").show();
                                //} else {
                                //    $("#InputOtro").hide();
                                //}
                                //$("#Otro").val(data.data.otroMotivo);
                                //$("#IdF").val(data.data.id);
                                $('#btnGuardarRegistro').show();
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
                        url: siteLocation + "Reposicion/ConsultaDataVerificacion",
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
                                $('#btnGuardarRegistro').show();
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
            //search: function (event, ui) {
            //    event.preventDefault();
            //    //$("#cp").val("");
            //    //$("#Estado").val('');
            //    //$("#Pais").val('');
            //    //$("#Municipio").val('');
            //    //$("#IdCatColonia").find('option').remove();
            //}
        });

    }

    var listenersTable = function () {
        $('input[type=checkbox]').off().on('change', function (e) {
            if ($(this).is(':checked')) {
                $(".r_select").prop("checked", false);
                $(".r_select").removeProp("checked");
                $(this).removeProp("checked");
                $(this).prop("checked", true)
                var id = $(this).data('id');
                $.ajax({
                    url: siteLocation + "Reposicion/ConsultaDataVerificacion",
                    type: "GET",
                    dataType: "json",
                    data: {
                        id: id,
                        tipo: 'Verificacion',
                        placa: 'Verificacion',
                        serie: 'Verificacion'
                    },
                    success: function (data) {
                        if (data.error === false) {
                            $('.DivData').html('');
                            $('.DivData').html(data.result);
                            $("#TipoReposicion").val('Verificacion');
                            $("#Placa").val('Verificacion');
                            $("#Serie").val('Verificacion');
                            $(".DivData").show();
                            $('#btnGuardarRegistro').show();
                            listenersData();
                        }
                    }
                });

            } else {
                $(".r_select").prop("checked", false);
                $(".r_select").removeProp("checked");
                $(this).removeProp("checked");
                $(this).prop("checked", false)
                $('.DivData').html('');
                $(".DivData").hide();
                $('#btnGuardarRegistro').hide();
            }
        });
    }

    var listenersData = function () {

        formData = new FormData();

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
        $('#EntidadProcedencia').select2({
            dropdownParent: $('#modal_registro')
        }).on('select2:close', function () {
            $('#EntidadProcedencia').valid();
            $(this).valid();
        });
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
var ModalDetalle = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + "Reposicion/Detalle?id=" + id,
            success: function (result) {
                if (result.error) {
                    toastr.error(result.errorDescription, "SMADSOT");
                } else {

                    $('#modalNotificacion .modal-title').html('Detalle');
                    $('#modalNotificacion .modal-body').html('');
                    $('#modalNotificacion .modal-dialog').addClass('modal-xl');
                    $('#modalNotificacion .modal-body').html(result.result);

                    $('#modalNotificacion').modal('show');
                }

                return;
            },
            error: function (res) {
                toastr.error("error", "SMADSOT");
            }
        });
    }
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

                //var formData = new FormData();

                formData.append("NumeroReferencia", $("#NumeroReferencia").val());
                //formData.append("Documento1", $("#DocumentoAdicional").prop("files")[0]);
                //formData.append("Documento2", $("#DocumentoAdicional").prop("files")[0]);
                //formData.append("Documento3", $("#DocumentoAdicional").prop("files")[0]);
                //formData.append("IdVerificacion", $("#ValueVerificacion").val());
                //formData.append("Placa", $("#Placa").val());
                formData.append("Id", $("#Id").val());
                formData.append("FechaEmisionRef", moment($('#FechaEmisionRef').val(), "DD-MM-YYYY").format('YYYY-MM-DD'));
                formData.append("FechaPago", moment($('#FechaPago').val(), "DD-MM-YYYY").format('YYYY-MM-DD'));
                formData.append("EntidadProcedencia", $("#EntidadProcedencia").val());
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
                    formData.set('FilesString', JSON.stringify(files));
                }

                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "Reposicion/CreateReposicion",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
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
            } else {
                // // KTUtil.scrollTop();
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
                    NumeroReferencia: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar el número de referencia.'
                            },
                        }
                    },
                    FechaEmisionRef: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la fecha de la referencia.'
                            },
                        }
                    }, FechaPago: {
                        validators: {
                            notEmpty: {
                                message: 'Debe agregar la fecha de la referencia.'
                            },
                        }
                    }, EntidadProcedencia: {
                        validators: {
                            notEmpty: {
                                message: 'Debe seleccionar la entidad.'
                            },
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
            init();
        }
    }
}();

var GenerarPDF = function () {
    var init = function () {
    };
    var generar = function (id) {
        $.ajax({
            cache: false,
            //type: 'POST',
            type: 'GET',
            processData: false,
            contentType: false,
            enctype: 'multipart/form-data',
            url: '/Refrendo/GetPDFFormaValorada?id=' + id + '&exento=false',
            //url: 'Reposicion/GetPDF?id=' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>Certificado Reposicion</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:application/pdf;base64,' + result.result + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
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
        generar: function (id) {
            generar(id);
        }
    }

}();

var EliminarRegistro = function (id) {
    var eliminar = function (id) {
        Swal.fire({
            title: '¿Seguro que desea continuar?',
            text: "Se eliminará toda la información ingresada.",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Aceptar',
            cancelButtonText: 'Cancelar'
        }).then(async (result) => {
            if (result.isConfirmed) {
                $.ajax({
                    cache: false,
                    type: 'DELETE',
                    url: 'Reposicion/EliminarRegistro?id=' + id,
                    success: function (result) {
                        if (result.error) {
                            toastr.error(result.message, 'SMADSOT');
                            return;
                        }
                        KTDatatableRemoteAjax.recargar();
                        toastr.success('Información eliminada correctamente.', 'SMADSOT');
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, 'SMADSOT');
                    }
                });
            }
        })
    };

    return {
        eliminar: function (data) {
            eliminar(data);
        }
    }

}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});