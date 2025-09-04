"use strict"
var datatable;
var flagPS;
var busquedas;

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
                url: 'ConsultaPruebaVerificacion/Consulta',
                type: 'POST',
                data: function (d) {
                    d.placa = $('select[id=PlacaSerie]').val();
                    d.placaSerie = $('#select2-autocomplete-container').text();
                }
            },
            columnDefs: [{
                "defaultContent": "-",
                "targets": "_all"
            }],
            columns: [

                { data: 'placa', name: 'Placa', title: 'Placa'.bold() },
                { data: 'serie', name: 'Serie', title: 'Serie'.bold() },
                {
                    data: 'fecha', name: 'Fecha', title: 'Fecha'.bold(),
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fecha).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'folioCertificado', name: 'FolioCertificado', title: 'Folio Certificado'.bold() },
                {
                    data: 'vigencia', name: 'Vigencia', title: 'Vigencia'.bold(),
                    render: function (data, type, row) {
                        if (type === 'display') {
                            let date = moment(row.fechaSolicitudFV).format('DD/MM/YYYY');
                            return `<p>${date}</p>`
                        }
                    }
                },
                //{ data: 'resultadosPrueba', name: 'ResultadosPrueba', title: 'ResultadosPrueba'.bold() },
                { data: 'tipoCertificado', name: 'TipoCertificado', title: 'Tipo Certificado'.bold() },
                { data: 'semestre', name: 'Semestre', title: 'Semestre'.bold() },
                { data: 'marca', name: 'Marca', title: 'Marca'.bold() },
                { data: 'modelo', name: 'Modelo', title: 'Modelo'.bold() },
                { data: 'combustible', name: 'Combustible', title: 'Combustible'.bold() },
                { data: 'tarjetaCirculacion', name: 'TarjetaCirculacion', title: 'Tarjeta Circulacion'.bold() },
            ],
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

/*------Autocomplete---------*/
$("#autocomplete").select2({
    width: '100%',
    ajax: {
        url: 'ConsultaPruebaVerificacion/ProveedorAutocomplete',
        dataType: "json",
        delay: 2000,
        data: function (params) {
            return {
                q: params.term,
                f: flagPS,
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
    placeholder: 'Ingresar...',
    minimumInputLength: 3,
    language: {
        inputTooShort: function () {
            return 'Por favor ingrese 3 o más caracteres';
        }
    }
});

$(document).on('change', '#autocomplete', function (data) {
    KTDatatableRemoteAjax.recargar();
    /*let id = this.value*/
    //$("#_idProveedor").text(id);
});

$("#PlacaSerie").change(function () {
    flagPS = $('select[id=PlacaSerie]').val();
    if (flagPS === "true") {
        $('#label-ps').html("Placa:&nbsp;")
    } else {
        $('#label-ps').html("Serie:&nbsp;");
    }
    $('#select2-autocomplete-container').text("");
    KTDatatableRemoteAjax.recargar();
});

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
    $("#kt_datatable_filter").hide();
    flagPS = $('select[id=PlacaSerie]').val();
    if (flagPS === "true") {
        $('#label-ps').html("Placa:&nbsp;")
    } else {
        $('#label-ps').html("Serie:&nbsp;");
    }
});