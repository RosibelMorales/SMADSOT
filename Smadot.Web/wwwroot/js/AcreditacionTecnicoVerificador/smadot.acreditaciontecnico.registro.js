"use strict"
//var myDropzone, myDropzone2, myDropzone3, myDropzone4, myDropzone5;
var validator; // https://formvalidation.io/
var empleados = [];
let data2 = {};
var KTDatatableRemoteAjax = function () {
    var init = function () {
        
        $("#autocomplete").select2({
            ajax: {
                url: siteLocation + 'AcreditacionTecnicoVerificador/Autocomplete',
                dataType: "json",
                delay: 2000,
                data: function (params) {
                    return {
                        q: params.term,
                        emp: JSON.stringify($('.ListEmp').map((_, el) => el.value).get()),
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
            placeholder: 'Ingresar nombre del empleado...',
            minimumInputLength: 3,
            language: 'es',
        });

        $(document).on('change', '#autocomplete', function (e) {
            //ModalEdit.init(this.value)
            
            var item = $('#autocomplete').find(':selected')[0];
            
            var lst = $('.ListEmp').map((_, el) => el.value).get()
            
            var html = '<tr><td><input type="hidden" class="ListEmp" value="'+item.value+'" />'+item.label+'</td></tr>';
            $("#bodyEmpleados").append(html)
            data2.Id = parseInt(item.value);
            empleados.push(parseInt(item.value))
        });
        //$('.datepicker-js').daterangepicker({
        //    singleDatePicker: true,
        //    showDropdowns: true,
        //    minYear: 1901,
        //    maxYear: parseInt(moment().format("YYYY"), 12),
        //    autoApply: true,
        //    locale: {
        //        format: 'DD/MM/YYYY',
        //        applyLabel: 'Aplicar',
        //        cancelLabel: 'Cancelar',
        //        daysOfWeek: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sa"],
        //        monthNames: ["Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"],
        //        weekLabel: "S"
        //    }
        //});

        //myDropzone = new Dropzone("#dropzonejs1", {
        //    autoProcessQueue: false,
        //    url: "/",
        //    maxFiles: 1,
        //    maxFilesize: 5, // MB
        //    addRemoveLinks: true,
        //    acceptedFiles: "image/jpeg,image/jpg,image/png,application/pdf",
        //    addedfiles: function (files) {
        //        if (files[0].accepted == false) {
        //            if (this.files.length >= 1)
        //                toastr.error("Elimine el documento antes de agregar uno nuevo.", "SMADSOT")
        //            else
        //                toastr.error("Solo se permiten archivos de 5MB.", "SMADSOT")
        //            this.removeFile(files[0])
        //        }
        //    }
        //});
        
    }
    return {
        init: function () {
            init();
        }
    };
}();
//var HorarioListeners = function () {
//    var init = function () {
//        $(".chkDay").change(function () {
//            var dia = $(this).data('dia');
//            if (this.checked) {
//                $('#HoraInicio-' + dia).prop('disabled', false);
//                $('#HoraFin-' + dia).prop('disabled', false);
//                $('#HoraInicio-' + dia).val("08:00");
//                $('#HoraFin-' + dia).val("18:00");

//            } else {
//                $('#HoraInicio-' + dia).prop('disabled', true);
//                $('#HoraFin-' + dia).prop('disabled', true);
//                $('#HoraInicio-' + dia).val('');
//                $('#HoraFin-' + dia).val('');
//            }
//        });
//    }
//    return {
//        init: function () {
//            init();
//        }
//    };
//}();
var validation = function () {
    const form = document.getElementById('form_registro');
    validator = FormValidation.formValidation(
        form,
        {
            fields: {
                NumeroSolicitud: {
                    validators: {
                        notEmpty: {
                            message: 'Este campo es obligatorio.'
                        }
                    }
                },
            },
            plugins: {
                trigger: new FormValidation.plugins.Trigger(),
                bootstrap: new FormValidation.plugins.Bootstrap({
                    rowSelector: '.fv-row',
                })
            }
        }
    );
}

$(document).on('click', '#btnSave', function (e) {

    if (!validator) {
        validation();
    }

    if (validator) {
        validator.validate().then(async function (status) {
            if (status == 'Valid') {
                //RegistrarVenta();
                blockUI.block();
                var emplelist = $('.ListEmp').map((_, el) => el.value).get();
                if (emplelist.length === 0) {
                    toastr.error('Debe seleccionar al menos un empleado..', "SMADSOT");
                    blockUI.release();
                    return;
                }
                var Data = {
                    NumeroSolicitud: $('#NumeroSolicitud').val(),
                    EmpleadosString: JSON.stringify(emplelist),
                }

                $.ajax({
                    cache: false,
                    type: 'POST',
                    contentType: 'application/json;charset=UTF-8',
                    url: siteLocation + 'AcreditacionTecnicoVerificador/Registro',
                    dataType: 'json',
                    data: JSON.stringify(Data),
                    async: true,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            toastr.error(result.message, "");
                            blockUI.release();
                        } else {
                            toastr.success("Información registrada exitosamente", "");
                            window.location.href = siteLocation + 'AcreditacionTecnicoVerificador/Index';
                            blockUI.release();
                        }
                        return;
                    },
                    error: function (res) {
                        toastr.error("Ocurrió un error al registrar la información", "");
                        blockUI.release();
                    }
                });
            }
            else {
                blockUI.release();
            }
        })
    }
});


var GenerarPDF = function () {
    var init = function () {
    };
    var generar = function () {
        var Data = {
            IdUsuario: $('#IdUsuario').val(),
            Nombre: $('#Nombre').val(),
            CorreoUsuario: $('#CorreoUsuario').val(),
            FechaNacimiento: moment($('#FechaNacimiento').val(), "DD-MM-YYYY"),
            Genero: $('#Genero').val(),
            Rfc: $('#Rfc').val(),
            Curp: $('#Curp').val(),
            IdPuesto: $('#IdPuesto').val(),
            NumeroTrabajador: $('#NumeroTrabajador').val(),
            FechaCapacitacionInicio: moment($('#FechaCapacitacionInicio').val(), "DD-MM-YYYY"),
            FechaCapacitacionFinal: moment($('#FechaCapacitacionFinal').val(), "DD-MM-YYYY"),
            FechaIncorporacion: moment($('#FechaIncorporacion').val(), "DD-MM-YYYY"),
            FechaAcreditacionNorma: $('#FechaAcreditacionNorma').val() == "" ? null : moment($('#FechaAcreditacionNorma').val(), "DD-MM-YYYY"),
            //HorarioRequest: [
            //    { dia: $('#Dia-0').val(), horaInicio: $('#HoraInicio-0').val(), horaFin: $('#HoraFin-0').val() },
            //    { dia: $('#Dia-1').val(), horaInicio: $('#HoraInicio-1').val(), horaFin: $('#HoraFin-1').val() },
            //    { dia: $('#Dia-2').val(), horaInicio: $('#HoraInicio-2').val(), horaFin: $('#HoraFin-2').val() },
            //    { dia: $('#Dia-3').val(), horaInicio: $('#HoraInicio-3').val(), horaFin: $('#HoraFin-3').val() },
            //    { dia: $('#Dia-4').val(), horaInicio: $('#HoraInicio-4').val(), horaFin: $('#HoraFin-4').val() },
            //    { dia: $('#Dia-5').val(), horaInicio: $('#HoraInicio-5').val(), horaFin: $('#HoraFin-5').val() },
            //    { dia: $('#Dia-6').val(), horaInicio: $('#HoraInicio-6').val(), horaFin: $('#HoraFin-6').val() },
            //],
            Puesto: $('select[name="IdPuesto"] option:selected').text(),
            GeneroText: $('select[name="Genero"] option:selected').text()
        }
        $.ajax({
            cache: false,
            type: 'POST',
            contentType: 'application/json;charset=UTF-8',
            url: siteLocation + 'Personal/GetPDF',
            dataType: 'json',
            data: JSON.stringify(Data),
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, 'SMADSOT');
                    return;
                }
                var win = window.open();
                win.document.write('<html><head><title>Vista Previa</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 0px;background-color: rgb(51, 51, 51);"><iframe src="data:application/pdf;base64,' + result.result + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
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
        generar: function () {
            generar();
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
            url: '/Personal/DescargarDocumento?url=' + url,
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

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});