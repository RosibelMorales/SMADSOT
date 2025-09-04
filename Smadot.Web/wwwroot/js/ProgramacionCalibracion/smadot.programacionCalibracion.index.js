"use strict"

var datatableProgramacionCalibracion;
var btns = false;
var myDropzone;

var KTDatatableRemoteAjax = function () {
    var init = function () {
        var IdEquipo = $('#IdEquipo').val();
        datatableProgramacionCalibracion = $('#kt_datatable').DataTable({
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
                url: siteLocation + 'ProgramacionCalibracion/Consulta?id=' + IdEquipo,
                type: 'POST'
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'nombreTipoCalibracion', name: 'NombreTipoCalibracion', title: 'Tipo de Calibración' },
                {
                    data: 'primeraFechaCalibracion', name: 'PrimeraFechaCalibracion', title: 'Primera Calibración',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.primeraFechaCalibracion).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'nombreUser', name: 'NombreUser', title: 'Registró' },
                {
                    data: 'fechaRegistro', name: 'FechaRegistro', title: 'Fecha de Registro',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaRegistro).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'nombreValido', name: 'NombreValido', title: 'Validó' },
                {
                    data: 'estatusParaColor', name: 'Estatus', title: 'Estatus',
                    render: function (data, type, row) {
                        let codeColor = '';
                        if (data == 'Gris') {
                            codeColor = '7E8299'
                        }
                        if (data == 'Rojo') {
                            codeColor = 'B8123D'
                        }
                        if (data == 'Verde') {
                            codeColor = '406959'
                        }

                        return `<span class="badge py-3 px-4 fs-7" style="background-color:#${codeColor}" title="${data}" >
                               <i class="bi bi-check-circle" style="color:white"></i>
                               </span>`;
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
    };

    var recargar = function () {
        datatableProgramacionCalibracion.ajax.reload();
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

$(document).off().on('click', '.btnDetalle', function (e) {
    var id = $(this).data('id');
    blockUI.block();
    ModalDetalle.init(id);
});

$(document).on('click', '#bntRegistrar', function (e) {
    ModalRegistro.init();
});

var ModalDetalle = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'ProgramacionCalibracion/Detalle/' + id,
            success: function (result) {
                //blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modalNotificacion2 .modal-title').html('Detalle');
                $('#modalNotificacion2 .modal-body').html('');
                $('#modalNotificacion2 .modal-dialog').addClass('modal-xl');
                $('#modalNotificacion2 .modal-body').html(result.result);
                listeners();
                $('#modalNotificacion2').modal('show');
                DetalleProgramacionCalibracion.init();
                //blockUI.release();
                return;
            },
            error: function (res) {
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var listeners = function () {
        $(document).on('click', '.descargarDoc', function (e) {
            DescargarDocumento.generar($(this).data('url'));
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


var DescargarDocumento = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: 'ProgramacionCalibracion/DescargarDocumento?url=' + url,
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

var ModalRegistro = function () {
    var init = function () {
        abrirModal();
    }
    var abrirModal = function () {
        
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'ProgramacionCalibracion/Registro',
            data: function (d) {
                d.id = $('#TipoCalibracion').select2('data')[0].id;
            },
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
                $("#modal_registro").on('shown.bs.modal', function () {
                    $('#TipoCalibracion').select2({
                        dropdownParent: $('#modal_registro')
                    });
                });
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

        //moment.locale('es');
        //$('#PrimeraCalibracion').daterangepicker({
        //    alwaysShowCalendars: true,
        //    singleDatePicker: true,
        //    showDropdowns: true,
        //    showCustomRangeLabel: false,
        //    autoApply: true,
        //    onSelect: function () {
        //        var dateObject = $(this).datepicker('getDate');
        //    }
        //});
        $('#PrimeraCalibracion').daterangepicker({
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

        //moment.locale('es');
        //$('#FechaRegistro').daterangepicker({
        //    alwaysShowCalendars: true,
        //    singleDatePicker: true,
        //    showDropdowns: true,
        //    showCustomRangeLabel: false,
        //    autoApply: true,
        //    onSelect: function () {
        //        var dateObject = $(this).datepicker('getDate');
        //    }
        //});

        $('#FechaRegistro').daterangepicker({
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

        GuardarFormulario.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });      
    }

    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
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


var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };

    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_registroProgramacionCalibracion')[0];
                var formData = new FormData(form);

                var files = [];
                files.push(Dropzone.forElement("#dropzonejs").files[0]);
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
                        /*formData.set('Files', JSON.stringify(files));*/
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

                var Data = {
                    IdEquipo: $("#IdEquipo").val(),
                    IdCatTipoCalibracion: $("#TipoCalibracion").val(),
                    PrimeraFechaCalibracion: moment($('#PrimeraCalibracion').val(), "DD-MM-YYYY"),
                    FechaRegistro: moment($('#FechaRegistro').val(), "DD-MM-YYYY"),
                    /*Nota: $("#Nota").val(),*/
                    FilesString: JSON.stringify(files)
                }

                $.ajax({
                    cache: false,
                    type: 'POST',
                    contentType: 'application/json;charset=UTF-8',
                    url: siteLocation + "ProgramacionCalibracion/Registro",
                    dataType: 'json',
                    data: JSON.stringify(Data),
                    async: true,
                    success: function (result) {
                        if (result.error) {
                            toastr.error("Error al guardar la información", "Error");
                            blockUI.release();
                        } else {
                            if (!$("#Id").length > 0) {
                                $("#save").hide();
                            }
                            toastr.success("Datos guardados con exito", "");
                            ModalRegistro.cerrarventanamodal();
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
            }
        });
    };



    var init_validacion = function () {
        var form = document.getElementById('form_registroProgramacionCalibracion');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    TipoCalibracion: {
                        validators: {
                            notEmpty: {
                                message: 'El tipo de calibración es requerido.'
                            }
                        }
                    },
                    PrimeraCalibracion: {
                        validators: {
                            notEmpty: {
                                message: 'La fecha es requerido.'
                            }                          
                        }
                    },                    
                    FechaRegistro: {
                        validators: {
                            notEmpty: {
                                message: 'La fecha es requerida.'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap()
                    //bootstrap: new FormValidation.plugins.Bootstrap({
                    //    rowSelector: '.fv-row',
                    //    eleInvalidClass: 'is-invalid',
                    //    eleValidClass: ''
                    //})
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
        guardar: function () {
            guardar();
        }
    }

}();


jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});