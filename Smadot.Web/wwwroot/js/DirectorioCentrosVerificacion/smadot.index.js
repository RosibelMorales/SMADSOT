"use strict"

var datatable;
var datatable2;
var myDropzone;

var KTDatatableRemoteAjax = function () {
    var init = function () {
        datatable = $('#kt_datatable').DataTable({
            language: {
                "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Spanish.json",
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
            lengthMenu: [[15, 25, 50, -1], [15, 25, 50, "Mostrar todo"]],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: 'DirectorioCentrosVerificacion/Consulta',
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all",
                "className": "mdc-data-table__cell"
            }],
            columns: [
                { data: 'clave', name: 'Clave', title: 'Clave'.bold() },
                { data: 'nombre', name: 'Nombre', title: 'Nombre'.bold() },

                { data: 'rfc', name: 'Rfc', title: 'RFC'.bold() },
                { data: 'telefono', name: 'Telefono', title: 'Telefono'.bold() },
                { data: 'correo', name: 'Correo', title: 'Correo'.bold() },
                { data: 'representanteLegal', name: 'RepresentanteLegal', title: 'Representante Legal'.bold() },
                //{

                //    title: ''.bold(),
                //    orderable: false,
                //    data: null,
                //    defaultContent: '',
                //    render: function (data, type, row) {
                //        if (type === 'display') {
                //                return '<div class="btn-group dropup drop-up dropdown">' +
                //                    '<a href="javascript:;" class="btn btn-sm btn-secondary btn-text-primary btn-hover-primary btn-icon mr-2" data-bs-toggle="dropdown">' +
                //                    '<i class="bi bi-three-dots"></i>' +
                //                    '</a>' +
                //                    '<ul class="dropdown-menu">' +
                //                    '<li><h6 class="dropdown-header"></h6></li>' +

                //                    '</a></li>' +
                //                    '<li><a href="/DirectorioCentrosVerificacion/DetallesVerificentro/' + row.id + '">' +
                //                    '<label class="dropdown-item">' +
                //                    '<span class="navi-icon"><i class="bi bi-list"></i></span>' +
                //                    '<span class="navi-text">Detalles</span>' +
                //                    '</label>' +
                //                    '</a></li>' +
                //                    '</ul>' +
                //                    '</div>';

                //        }
                //    }
                //}
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
        GuardarFormularioCarga.init();
    };

    var recargar = function () {
        datatable.ajax.reload();
    }

    var listeners = function () {
        $(document).on('click', '.btnConfigurar', function (e) {
            //$('.btnConfigurar').off().on('click', function (e) {
            e.preventDefault()
            let id = $(this).attr("data-id");
            modalConfigurar.init(id);
        });
        $('.btnConfigurar').off()

        $(document).on('click', '.btnDiaNoLaboral', function (e) {
            //$('.btnDiaNoLaboral').off().on('click', function (e) {
            e.preventDefault()
            let id = $(this).attr("data-id");
            modalDiaNoLaboralGrid.init(id);
        });

        $('.btnDiaNoLaboral').off()
        myDropzone = new Dropzone("#dropzonejs", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 10, // MB
            addRemoveLinks: true,
            acceptedFiles: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet, application/vnd.ms-excel',
            previewsContainer: "#dropzonejs", // Este es el contenedor donde se mostrarán las vistas previas
            init: function () {
                this.on("addedfile", function (file) {

                    if (file.size > (this.options.maxFilesize * 1024 * 1024)) {
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
        $("#kt_modal_1").on('hide.bs.modal', function () {
            myDropzone.removeAllFiles();
        });
        $('#btnCargaMasiva').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormularioCarga.guardar();
        });
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

var KTDatatableRemoteAjax2 = function () {
    var init = function (id) {
        datatable2 = $('#kt_datatableDiaNLaboral').DataTable({
            language: {
                "url": "//cdn.datatables.net/plug-ins/9dcbecd42ad/i18n/Spanish.json",
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
            searching: false,
            lengthMenu: [[15, 25, 50, -1], [15, 25, 50, "Mostrar todo"]],
            processing: true,
            serverSide: true,
            filter: true,
            ordering: true,
            ajax: {
                url: 'DirectorioCentrosVerificacion/ConsultaDiaNoLaborales?id=' + id,
                type: 'POST',
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all",
                "className": "mdc-data-table__cell"
            }],
            columns: [
                { data: 'fechaString', name: 'Fecha', title: 'Fecha'.bold() },
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
    };

    var recargar = function () {
        datatable2.ajax.reload();
    }

    var listeners = function () {
        $(document).on('click', '.btnDiaNoLaboralDelete', function (e) {
            let id = $(this).attr("data-id");
            ConfirmarFechaDNL.init(id);
        });
        $('.btnDiaNoLaboralDelete').off()
    }

    return {
        init: function (id) {
            init(id);
        },
        recargar: function () {
            recargar();
        }
    };
}();

$(document).on('click', '#bntRegistrar', function (e) {
    ModalRegistro.init(0);
});

$(document).on('click', '.btnEditar', function (e) {
    var id = $(this).data('id');
    ModalRegistro.init(id);
});

$(document).on('click', '#bntCarga', function (e) {
    $('#kt_modal_1').modal('show');

});

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

var ModalRegistro = function () {
    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        const idVerificentro = parseInt(id);
        $.ajax({
            cache: false,
            type: 'GET',
            url: siteLocation + 'DirectorioCentrosVerificacion/RegistroCVV?id=' + idVerificentro,
            success: function (result) {
                blockUI.release();
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro .modal-title').html(idVerificentro === 0 ? 'Registro CVV' : 'Editar CVV');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro .modal-dialog').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);
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
                var form = $('#form_registroCVV');


                var Data = {
                    Id: $("#Id")?.val(),
                    Nombre: $("#Nombre").val(),
                    Activo: $("#Activo").val(),
                    Clave: $("#Clave").val(),
                    Direccion: $("#Direccion").val(),
                    Rfc: $("#Rfc").val(),
                    Telefono: $("#Telefono").val(),
                    Correo: $("#Correo").val(),
                    GerenteTecnico: $("#GerenteTecnico").val(),
                    RepresentanteLegal: $("#RepresentanteLegal").val(),
                    DirectorGestionCalidadAire: $("#DirectorGestionCalidadAire").val(),
                    Municipio: $("#Municipio").val(),
                    Latitud: $("#Latitud").val(),
                    Longitud: $("#Longitud").val(),
                    ApiKey: $("#ApiKey").val(),
                    ApiEndPoint: $("#ApiEndPoint").val()
                }
                Data.Id = Data.Id !== "" ? Data.Id : 0;
                $.ajax({
                    cache: false,
                    type: 'POST',
                    url: siteLocation + "DirectorioCentrosVerificacion/RegistroCVV",
                    data: Data,
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
        var form = document.getElementById('form_registroCVV');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    Nombre: {
                        validators: {
                            notEmpty: {
                                message: 'El Nombre es requerido.'
                            }
                        }
                    },
                    DirectorGestionCalidadAire: {
                        validators: {
                            notEmpty: {
                                message: 'El Nombre es requerido.'
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
    return {
        init: function () {
            init();
        },
        guardar: function () {
            guardar();
        }
    }

}();


var modalConfigurar = function () {

    var fv;
    const init = (id) => {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'DirectorioCentrosVerificacion/ConfiguradorCita/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html('Configurador de Cita');
                $('#modal_registro .modal-body').html('');
                $('#modalClass').addClass('modal-lg');
                $('#modal_registro .modal-body').html(result.result);

                $('#modal_registro').on('shown.bs.modal', function () {

                    $(".flatpickr-input").flatpickr({
                        enableTime: true,
                        noCalendar: true,
                        dateFormat: "H:i",
                        time_24hr: true,
                        minuteIncrement: 1,
                        static: true,
                    });
                    $('.flatpickr-wrapper').on('click', function () {
                        $(this)[0].children[0].click();
                    });
                    $('.dtInputGroup > .fv-plugins-icon').on('click', function () {
                        $(this)[0].parentElement.children[0].click();
                    });
                });

                //console.log(pick)
                //pickerHI.dates.setFromInput(document.getElementById('ModelHoraInicial').value)
                //pickerHF.dates.setFromInput(document.getElementById('ModelHoraFinal').value)

                //var DateTimeVal = moment('02/02/2022 00:00', 'MM/DD/YYYY HH:mm').toDate();

                //picker.setValue(tempusDominus.DateTime.convert(DateTimeVal));

                listeners();
                validate();
                $('#modal_registro').modal('show');
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var listeners = function () {




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
        $('#btnCerrarRegistro').click();
    }
    var validate = () => {
        const form = document.getElementById('form_registro');
        fv = FormValidation.formValidation(form, {
            fields: {
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap({
                    rowSelector: '.fv-row',
                    eleInvalidClass: 'is-invalid',
                    eleValidClass: ''
                }),
                submitButton: new FormValidation.plugins.SubmitButton(),
                icon: new FormValidation.plugins.Icon({
                    validating: 'fa fa-refresh',
                }),
            },
        });
        var i = 0;
        while (i < 7) {
            fv.addField('Dias[' + i + '].Capacidad', {
                validators: {
                    notEmpty: {
                        message: 'El campo es requerido.'
                    }
                }
            });
            fv.addField('Dias[' + i + '].IntervaloCitas', {
                validators: {
                    notEmpty: {
                        message: 'El campo es requerido.'
                    }
                }
            });
            fv.addField('Dias[' + i + '].HoraInicioString', {
                validators: {
                    notEmpty: {
                        message: 'El campo es requerido.'
                    }
                }
            });
            fv.addField('Dias[' + i + '].HoraFinString', {
                validators: {
                    notEmpty: {
                        message: 'El campo es requerido.'
                    }
                }
            });
            i++;
        }
    }

    const GuardarInfo = () => {

        fv.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_registro')[0];
                var formData = new FormData(form);
                //if ($('.checkVerificacion:checked').length === 0) {
                //    toastr.error('Debe seleccionar al menos un día.', "SMADSOT");
                //    return;
                //}
                $.ajax({
                    cache: false,
                    type: 'Post',
                    processData: false,
                    contentType: false,
                    url: siteLocation + 'DirectorioCentrosVerificacion/ConfiguradorCita',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        toastr.success('Registro guardado exitosamente', "SMADSOT");
                        modalConfigurar.cerrarventanamodal();
                        KTDatatableRemoteAjax.recargar();
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
        init: (id) => {
            init(id);
        },
        cerrarventanamodal: function () {
            cerrarModal();
        }

    }
}()


var modalDiaNoLaboralGrid = function () {
    var fv;
    const init = (id) => {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        blockUI.block();
        $.ajax({
            cache: false,
            type: 'GET',
            url: 'DirectorioCentrosVerificacion/DiaNoLaborable/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_detalle #modalLabelTitleDetalle').html('Configurar día no laboral');
                $('#modal_detalle .modal-body').html('');
                $('#modalClassDetalle').addClass('modal-xl');
                $('#modal_detalle .modal-body').html(result.result);
                listeners(id);
                validate();
                $('#modal_detalle').modal('show');
                blockUI.release();
                return;
            },
            error: function (res) {
                blockUI.release();
                toastr.error(res, "SMADSOT");
                return;
            }
        });
    }
    var listeners = function (id) {
        $("#Fecha").daterangepicker({
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

        $('#btnSaveNewFecha').off().on('click', function (e) {

            e.preventDefault();
            if (fv) {
                fv.validate().then(async function (status) {
                    if (status === 'Valid') {
                        GuardarInfoDia();
                    }
                });
            }
        });

        KTDatatableRemoteAjax2.init(id);
    }

    var validate = () => {
        const form = document.getElementById('form_registroDNL');
        fv = FormValidation.formValidation(form, {
            fields: {
                //'Capacidad': {
                //    validators: {
                //        notEmpty: {
                //            message: 'El campo es requerido.'
                //        }
                //    }
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

    const GuardarInfoDia = () => {
        blockUI.block();
        fv.validate().then(async function (status) {
            if (status === 'Valid') {

                var request = {}
                request.IdCVV = $("#IdCvv")?.val();
                request.Fecha = $('#Fecha')?.val();
                $.ajax({
                    cache: false,
                    type: 'Post',
                    url: siteLocation + 'DirectorioCentrosVerificacion/RegistroDiaNoLaboral',
                    data: request,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "SMADSOT");
                            blockUI.release();
                            return;
                        }
                        $("#Fecha").val('')
                        toastr.success('Registro guardado exitosamente', "SMADSOT");
                        blockUI.release();
                        KTDatatableRemoteAjax2.recargar();
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error(res, "SMADSOT");
                        return;
                    }
                });
            }
        });
    }
    return {
        init: (id) => {
            init(id);
        }

    }
}()

var ConfirmarFechaDNL = function () {
    const init = (id) => {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        Swal.fire({
            html: '¿Está seguro que desea eliminar esta fecha?',
            icon: "info",
            buttonsStyling: false,
            showCancelButton: true,
            reverseButtons: true,
            cancelButtonText: 'Cancelar',
            confirmButtonText: "Confirmar",
            focusConfirm: true,
            customClass: {
                confirmButton: "btn btn-primary",
                cancelButton: 'btn btn-secondary'
            }
        }).then((result) => {
            if (result.isConfirmed) {
                blockUI.block();

                var formData = new FormData();
                formData.append('Id', id);

                $.ajax({
                    cache: false,
                    type: 'POST',
                    //contentType: 'application/json;charset=UTF-8',
                    // headers: { 'X-CSRF-TOKEN': $('meta[name="_token"]').attr('content') },
                    /*enctype: 'multipart/form-data',*/
                    url: siteLocation + "DirectorioCentrosVerificacion/DeleteDiaNoLaboral",
                    processData: false,
                    contentType: false,
                    //dataType: 'json',
                    data: formData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            blockUI.release();
                            toastr.error(result.message, "SMADSOT");
                            return;
                        }
                        toastr.success('La información se actualizo exitosamente', "SMADSOT");
                        blockUI.release();
                        KTDatatableRemoteAjax2.recargar();
                        return;
                    },
                    error: function (res) {
                        blockUI.release();
                        toastr.error(res, "SMADSOT");
                        return;
                    }
                });
            }


        });
    }
    var listeners = function () {

    }
    return {
        init: (id, nombre, estatus) => {
            init(id, nombre, estatus);
        }

    }
}()





var GuardarFormularioCarga = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };
    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                blockUI.block();

                var formData = new FormData();
                var fileInput = Dropzone.forElement("#dropzonejs"); // Reemplaza 'fileInput' con el ID real de tu input de archivo

                formData.append('file', fileInput.files[0]); // Agrega el archivo al FormData
                //formData.append('tipo', id);
                let filenamedownload = "";
                fetch('/DirectorioCentrosVerificacion/CargaMasiva', {
                    method: 'POST',
                    body: formData
                })
                    .then(response => {
                        const contentType = response.headers.get('content-type');
                        const contentDisposition = response.headers.get('content-disposition');
                        let filename = 'downloaded-file.xlsx';

                        if (contentDisposition && contentDisposition.includes('filename=')) {
                            filename = contentDisposition.split('filename=')[1].split(';')[0].replace(/"/g, '');
                        }

                        if (contentType.includes('application/json')) {
                            return response.json();
                        } else if (contentType.includes('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet')) {
                            const modifiedFilename = filename.replace('.xlsx', '_errores.xlsx');
                            filenamedownload = modifiedFilename;
                            return response.blob().then(blob => ({ blob, filename: modifiedFilename }));
                        } else {
                            throw new Error('Unexpected content type');
                        }
                    })
                    .then(data => {
                        blockUI.release();

                        if (data.blob) {
                            Swal.fire({
                                text: "Por favor, revise el archivo de resultados. El archivo contiene errores",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Aceptar",
                                customClass: {
                                    confirmButton: "btn btn-light"
                                }
                            }).then(function () {
                            });

                            const url = window.URL.createObjectURL(data.blob);
                            const a = document.createElement('a');
                            a.href = url;
                            a.download = data.filename;
                            a.click();
                            window.URL.revokeObjectURL(url);
                        } else {
                            if (data.isSuccessFully) {
                                Swal.fire({
                                    text: "Verificaciones cargadas de manera exitosa.",
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: {
                                        confirmButton: "btn btn-light"
                                    }
                                }).then(function () {
                                });
                            } else {
                                Swal.fire({
                                    text: data.message,
                                    icon: "error",
                                    buttonsStyling: false,
                                    confirmButtonText: "Aceptar",
                                    customClass: {
                                        confirmButton: "btn btn-light"
                                    }
                                }).then(function () {
                                });
                            }
                        }
                        fileInput.removeAllFiles();
                    })
                    .catch(error => {
                        blockUI.release();
                        Swal.fire({
                            text: "Error al importar, revise el documento",
                            icon: "error",
                            buttonsStyling: false,
                            confirmButtonText: "Aceptar",
                            customClass: {
                                confirmButton: "btn btn-light"
                            }
                        }).then(function () {
                        });
                    });
            }
        });
    };

    var init_validacion = function () {
        var form = document.getElementById('form_registro');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {
                    'dropzoneFiles': {
                        validators: {
                            callback: {
                                message: 'Debe agregar un archivo en el Dropzone.',
                                callback: function (input) {
                                    let dp = Dropzone.forElement("#dropzonejs").files;
                                    return dp.length > 0;
                                }
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



