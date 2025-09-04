moment.locale('es');
var myDropzone, myDropzone2, myDropzone3;
var accept = 2; var denied = 3;

$(document).on('click', '#botonEnvio, .botonEnvio', function (e) {
    e.preventDefault();
    Registro.init();

});

var Registro = function () {

    var init = function () {
        agregarFila();
    }
    var agregarFila = function (event) {

        const form = document.getElementById("form_registroCapacitacion");
        let transactionFormData = new FormData(form);

        let transactionTableRef = document.getElementById("transactionTable");
        let newTransactionRowRef = transactionTableRef.insertRow(-1);

        var rows = transactionTableRef.rows;
        var cells, t;

        //Id del empleado
        let newTypeCellRef = newTransactionRowRef.insertCell(0);
        newTypeCellRef.setAttribute("hidden", "hidden")
        newTypeCellRef.textContent = transactionFormData.get("empleadosGrid");

        //Nombre del tema
        let namenewTypeCellRef = newTransactionRowRef.insertCell(1);
        var combo = document.getElementById("empleadosGrid");
        namenewTypeCellRef.textContent = combo.options[combo.selectedIndex].text;

        //Subir fotografia
        let newFileCell = newTransactionRowRef.insertCell(2);
        let fileTypeCell = document.createElement("div");
        fileTypeCell.classList.add("dropzone");
        fileTypeCell.setAttribute("hidden", "hidden");
        fileTypeCell.setAttribute("id", "dropzonejs2");

        let div2 = document.createElement("div");
        div2.classList.add("dz-message"); div2.classList.add("needsclick");
        fileTypeCell.appendChild(div2);

        let icon = document.createElement("i");
        icon.classList.add("bi"); icon.classList.add("bi-file-earmark-arrow-up"); icon.classList.add("text-primary"); icon.classList.add("fs-3x");
        div2.appendChild(icon);

        let div3 = document.createElement("div");
        div3.classList.add("ms-2");
        div2.appendChild(div3);

        let letra = document.createElement("h3");
        letra.classList.add("fs-5"); letra.classList.add("fw-bold"); letra.classList.add("text-gray-900"); letra.classList.add("mb-1");
        letra.textContent = "Suelte los archivos aquí o haga clic para cargarlos.";
        div3.appendChild(letra);

        newFileCell.appendChild(fileTypeCell);

        //Boton para eliminar fila
        let deleteCell = newTransactionRowRef.insertCell(3);
        let deleteTypeButton = document.createElement("button");
        deleteTypeButton.classList.add("btn"); deleteTypeButton.classList.add("btn-primary");
        deleteTypeButton.classList.add("bi"); deleteTypeButton.classList.add("bi-trash-fill");
        deleteCell.appendChild(deleteTypeButton);

        deleteTypeButton.addEventListener("click", function (event) {
            event.target.parentNode.parentNode.remove();
            event.preventDefault();
        });


        myDropzone2 = new Dropzone(fileTypeCell, {
            autoProcessQueue: false,
            url: "/",
            maxFiles: 1,
            maxFilesize: 5, // MB
            addRemoveLinks: true,
            acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
            addedfiles: function (files) {
                if (files[0].accepted == false) {
                    if (this.files.length > 1)
                        toastr.error("Solo se permiten archivos de 5MB.", 'SMADSOT'),
                            toastr.error("Solo se permiten archivos con extensión .jpeg, .jpg, .png, pdf", 'SMADSOT')
                    else
                        toastr.error("Elimine el documento antes de agregar uno nuevo.", 'SMADSOT')
                    this.removeFile(files[0])
                }
            }
        });


        if (rows.length > 2) {
            var result = []
            result.Id = [];
            var h = 1;
            var idComparar = transactionFormData.get("empleadosGrid");

            for (var i = 0, iLen = rows.length; i < iLen - h; i++) {
                cells = rows[i].cells;
                t = [];

                result.Id.push(rows[i].cells[0].textContent);
                if (result.Id[i] == idComparar) {
                    toastr.error("El empleado ya ha sido agregado, intenta con otro empleado", 'SMADSOT');
                    $('#transactionTable tr:last').remove();
                    break;
                }
            }
        }

        if (transactionFormData.get("empleadosGrid") == 0) {
            toastr.error("No fue posible agregar al empleado, asegurate de seleccionar uno.", 'SMADSOT');
            $('#transactionTable tr:last').remove();
        }


    }

    $('#btnSave').off().on('click', function (e) {
        e.preventDefault();
        GuardarCapacitacion.guardar();
    });
    return {
        init: function (id) {
            init(id);
        }
    }
}();

var GuardarCapacitacion = function () {
    var validation2; // https://formvalidation.io/

    var init2 = function () {
        init_validacion2();
    };
    var guardar = async function () {
        var table = document.getElementById("transactionTable");
        var form = $('#form_registroCapacitacion')[0];
        var formData = new FormData(form);
        var files = [];
        var result = []
        result.Id = [];
        result.Dropzone = [];
        var rows = table.rows;
        var cells, t;

        for (var i = 0, iLen = rows.length; i < iLen; i++) {
            cells = rows[i].cells;
            t = [];
            for (var j = 0, jLen = cells.length; j < jLen; j++) {
                t.push(cells[j].textContent);
            }
            result.Id.push(rows[i].cells[0].textContent);
            result.Dropzone.push(rows[i].cells[2].textContent);
        }
        formData.set('[0].FechaCapacitacion', formData.get('FechaRegistroCapacitacion').split('/').reverse().join('/'));
        formData.set('[0].TemaCapacitacion', formData.get('TemaCapacitacion'));
        var t = result.Id[1];
        var p = result.Id[2];
        var pt = 0;

        for (var k = 1, kLen = result.Id.length; k < kLen; k++) {
            formData.set('[' + pt + '].IdCapEmp', result.Id[k]);
            files.push(Dropzone.forElement("#dropzonejs2").files[0]);
            pt++;
        }
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
                toastr.success('Los datos se guardaron correctamente.', 'SMADSOT', {
                    timeOut: 250,
                    preventDuplicates: true,

                    // Redirect 
                    onHidden: function () {
                        window.location.href = "/Capacitacion";
                    }
                });
            },
            error: function (res) {
                toastr.error(res, 'SMADSOT');
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

    var init_validacion2 = function () {
        var form = document.getElementById('form_registroCapacitacion');

        validation2 = FormValidation.formValidation(
            form,
            {
                fields: {
                    TemaCapacitacion: {
                        validators: {
                            notEmpty: {
                                message: 'El tema es requerido.'
                            }
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
    }

    return {
        init2: function () {
            init2();
        },
        guardar: function () {
            guardar();
        }
    }

}();

var dataTable;

var KTDataTableCapacitacion = function () {
    var init = function () {
        dataTable = $('#tblCapacitacionEmpleado').DataTable({
            language: {
                "sProcessing": "Procesando...",
                "sLengthMenu": "Mostrar _MENU_ registros",
                "sZeroRecords": "No se encontraron resultados",
                "sEmptyTable": "Ningún dato disponible en esta tabla",
                "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
                "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
                "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
                "sInfoPostFix": "",
                "sSearch": "Buscar:",
                "sUrl": "",
                "sInfoThousands": ",",
                "sLoadingRecords": "Cargando...",
                "oPaginate": {
                    "sFirst": "Primero",
                    "sLast": "Último",
                    "sNext": "Siguiente",
                    "sPrevious": "Anterior"
                },
                "oAria": {
                    "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                    "sSortDescending": ": Activar para ordenar la columna de manera descendente"
                }
            },
            searching: true,
            "lengthMenu": [[5, 10, 15, 20, -1], [5, 10, 15, 20, "All"]],
            autoWidth: true,
            columnDefs: [
                {
                    targets: ['_all'],
                    className: 'mdc-data-table__cell',
                },
            ],


        });
    }

    var recargar = function () {
        dataTable.ajax.reload();
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

jQuery(document).ready(function () {
    KTDataTableCapacitacion.init();
});

$(document).on('click', '#btnListaEmpleado, .btnListaEmpleado', function (e) {
    var id = $(this).data('id');

    ModalEdit.init(id);
});

var ModalEdit = function () {

    var init = function (id) {
        abrirModal(id);
    }
    var abrirModal = function (id) {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/Capacitacion/Update/' + id,
            data: function (d) {
                d.id = $('#empleadosGrid').select2('data')[0].id;
            },
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_registro #modalLabelTitle').html(id === undefined ? 'Actualización de Evaluación' : 'Actualización de Evaluación');
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

        GuardarFormulario.init();

        $('#btnGuardarRegistro').off().on('click', function (e) {
            e.preventDefault();
            GuardarFormulario.guardar();
        });


    }

    // Cerramos la ventana modal
    var cerrarventanaCierre = function () {
        //TablaCapacitacionEmpleado.recargar();
        $('#btnCerrarRegistro').click();
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

var GuardarFormulario = function () {
    var validation; // https://formvalidation.io/

    var init = function () {
        init_validacion();
    };

    var guardar = function () {
        validation.validate().then(async function (status) {
            if (status === 'Valid') {
                var form = $('#form_CapacitacionUpdate')[0];
                var formData = new FormData(form);
                formData.set('[0].Id', formData.get('IdCapacitacionEmpleado'));
                formData.set('[0].IdEmpleado', formData.get('IdUserPuestoVerificentro'));
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
                        toastr.success('Los datos se guardaron correctamente.', 'SMADSOT');
                        //$("#tblCapacitacionEmpleado").load(location.href + " #tblCapacitacionEmpleado");
                        //KTDataTableCapacitacion.init();

                        ModalEdit.cerrarventanamodalCierre();
                        setTimeout: 400,
                            window.location.reload();
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
        var form = document.getElementById('form_CapacitacionUpdate');
        validation = FormValidation.formValidation(
            form,
            {
                fields: {

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
        guardar: function () {
            guardar();
        }
    }

}();

$(document).on('change', '#Asistencia, .Asistencia', function (e) {
    var id = $(this).data('id');
    $.ajax({
        cache: false,
        type: 'POST',
        processData: false,
        contentType: false,
        enctype: 'multipart/form-data',
        url: '/Capacitacion/UpdateAsistencia/' + id,
        success: function (result) {
            if (!result.isSuccessFully) {
                toastr.error(result.message, 'SMADSOT');
                return;
            }
            toastr.success('Se actualizo la asistencia correctamente.', 'SMADSOT');
            return;
        },
        error: function (res) {
            toastr.error(res, 'SMADSOT');
        }
    });
});

//Evaluacion
$(document).on('click', '.descargarDoc', function (e) {
    DescargarDocumento.generar($(this).data('url'));
});

var DescargarDocumento = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: '/Capacitacion/DescargarDocumento?url=' + url,
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

$('#FechaRegistroCapacitacion').daterangepicker({
    singleDatePicker: true,
    showDropdowns: true,
    showCustomRangeLabel: false,
    autoApply: true,
    locale: {
        format: "YYYY-MM-DD",
        applyLabel: 'Aplicar',
        cancelLabel: 'Cancelar',
        daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        weekLabel: "S"
    }
});
//Autorizar

$(document).on('click', '#btnAutorizado, .btnAutorizado', function (e) {
    var id = $(this).data('id');
    var form = $('#form_registroCapacitacion')[0];
    var formData = new FormData(form);
    formData.set('[0].IdCapacitacion', id);
    formData.set('[0].AcceptOrDenied', accept)
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
            toastr.success('Los datos se guardaron correctamente.', 'SMADSOT', {
                timeOut: 250,
                preventDuplicates: true,

                // Redirect 
                onHidden: function () {
                    window.location.href = "/Capacitacion";
                }
            });
        },
        error: function (res) {
            toastr.error(res, 'SMADSOT');
        }
    });
});

//Denegar
$(document).on('click', '#btnDenegado, .btnDenegado', function (e) {
    var id = $(this).data('id');
    var id = $(this).data('id');
    var form = $('#form_registroCapacitacion')[0];
    var formData = new FormData(form);
    formData.set('[0].IdCapacitacion', id);
    formData.set('[0].AcceptOrDenied', denied)
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
            toastr.success('Los datos se guardaron correctamente.', 'SMADSOT', {
                timeOut: 250,
                preventDuplicates: true,

                // Redirect 
                onHidden: function () {
                    window.location.href = "/Capacitacion";
                }
            });
        },
        error: function (res) {
            toastr.error(res, 'SMADSOT');
        }
    });
});

//Fotografía
$(document).on('click', '.descargarFoto', function (e) {
    DescargarFotografia.generar($(this).data('url'));
});

var DescargarFotografia = function (url) {
    var generar = function (url) {
        $.ajax({
            cache: false,
            type: 'GET',
            processData: false,
            contentType: false,
            url: '/Capacitacion/DescargarFotografia?url=' + url,
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>' + result.result.fileName + '</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64 + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
                //var w = window.open('data:' + (result.result.ext === 'pdf' ? 'application/pdf' : 'image/' + result.result.ext) + ';base64,' + result.result.base64, '_blank');
                //w.document.title = result.result.fileName + '';
                toastr.success('Fotografía descargada correctamente.', 'SMADSOT');
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


//function RefreshTable(tableId, urlData) {
//    $.getJSON(urlData, null, function (json) {
//        table = $(tableId).dataTable();
//        oSettings = table.fnSettings();

//        table.fnClearTable(this);

//        for (var i = 0; i < json.aaData.length; i++) {
//            table.oApi._fnAddData(oSettings, json.aaData[i]);
//        }

//        oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
//        table.fnDraw();
//    });
//}
//// Edited by Prasad
//function AutoReload() {
//    RefreshTable('#tblCapacitacionEmpleado', '/Capacitacion/Edit/');

//    setTimeout(function () {
//        AutoReload();
//    }, 2000);
//}

//$(document).ready(function () {
//    InitOverviewDataTable();
//    setTimeout(function () {
//        AutoReload();
//    }, 2000);
//});
