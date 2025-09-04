"use strict"

var datatable;

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
                url: 'Equipo/Consulta',
                type: 'POST'
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [
                { data: 'nombreLinea', name: 'NombreLinea', title: 'Linea' },
                { data: 'nombreEquipo', name: 'NombreEquipo', title: 'Equipo' },
                { data: 'numeroSerie', name: 'NumeroSerie', title: 'Número de Serie' },
                { data: 'nombre', name: 'Nombre', title: 'Capturó' },
                {
                    data: 'fechaRegistro', name: 'FechaRegistro', title: 'Fecha de Registro',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaRegistro).format('DD/MM/YYYY');
                        }
                    }
                },
                {
                    data: 'estatusColor', name: 'Estatus', title: 'Estatus',
                    render: function (data, type, row) {
                        let codeColor = '';
                        let title = '';
                        let icon = '';
                        let style = '';
                        if (data == 'Gris') {
                            title = 'Inactivo';
                            codeColor = 'badge-secondary';
                            icon = 'bi bi-circle text-dark';
                            style = 'background-color: var(--bs-secondary)!important;';
                        }
                        else if (data == 'Rojo') {
                            title = 'Sin Calibrar';
                            codeColor = 'badge-danger';
                            icon = 'bi bi-exclamation-circle text-white';
                        }
                        else if (data == 'Naranja') {
                            title = 'Documentación Inválida';
                            codeColor = 'badge-warning';
                            icon = 'bi bi-slash-circle text-white';
                        }
                        else if (data == 'Verde') {
                            title = 'Activo';
                            codeColor = 'badge-success';
                            icon = 'bi bi-check-circle text-white';
                        }
                        return `<span class="badge ${codeColor} py-3 px-4 fs-7" style="${style}" title="${title}" >
                                <i class="${icon}" style="color:white"></i>
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
        $('#save').on("click", function (e) {
            e.preventDefault();
            ModalEditarCrear.init(null);
        })
        $(document).on("click", ".editar", function (e) {
            e.preventDefault();
            ModalEditarCrear.init($(this).data("id") || 0);
        })
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
var ModalEditarCrear = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Equipo/Registro/' + id,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro .modal-title').html(id === "null" ? 'Registrar Equipo' : 'Editar Equipo');
                $('#modal_registro .modal-body').html('');
                $('#modal_registro').addClass('modal-xl');
                $('#modal_registro .modal-body').html(result.result);

                //if (!btns) {
                //    btns = true;
                //}
                $('#modal_registro').modal('show');
                RegistroPage.initValidacion();

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
        $("#IdTipoEquipo").select2({
            
            dropdownParent: $('#modal_registro'),
            placeholder: "Selecciona un equipo..."
        });
        $("#linea").select2({
            dropdownParent: $('#modal_registro'),
            placeholder: "Selecciona una línea..."
        });
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
        myDropzone4 = new Dropzone("#dropzonejs4", {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            previewsContainer: "#dropzonejs4", // Este es el contenedor donde se mostrarán las vistas previas
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
            e.preventDefault();
            RegistroPage.init();
            return;
        });
    }

    // Cerramos la ventana modal
    var cerrarModal = function () {
        KTDatatableRemoteAjax.recargar();
        $('#modal_registro').modal('hide');

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




jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});