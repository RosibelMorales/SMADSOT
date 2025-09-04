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
                url: '/ConstanciaUltimaVerificacion/Consulta',
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
                { data: 'placaSerie', name: 'placaSerie', title: 'Placa / Serie' },
                {
                    data: 'fechaRegistro', name: 'FechaRegistro', title: 'Fecha Registro',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaRegistro).format('DD/MM/YYYY');
                        }
                    }
                },
                { data: 'nombre', name: 'Nombre', title: 'Usuario Registró' },
                { data: 'numeroReferencia', name: 'numeroReferencia', title: 'Número de Referencia' },
                { data: 'claveTramite', name: 'ClaveTramite', title: 'Clave de Trámite' },  
                { data: 'tipoCertificado', name: 'TipoCertificado', title: 'Certificado' },
                {
                    data: 'vigencia', name: 'Vigencia', title: 'Vigencia',
                    render: function (data, type, row) {
                        if (type === 'display') {
                            return moment(row.fechaRegistro).format('DD/MM/YYYY');
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

        $(document).on('click', '.btnPDFCertificado', function (e) {
            e.preventDefault();
            GenerarPDF.generar($(this).data('id'));
        });

        $('#qrcode').on('DOMSubtreeModified', function () {
            if (this.children.length > 1) {
                var src = this.children[1].getAttribute('src');
                if (src !== null) {
                    var win = window.open();
                    win.document.write('<html><head><title>QR</title></head><body style="height: 100%;width: 100%;overflow: hidden;margin: 40px;"><iframe src="' + src + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe></body></html>');
                    $(this).html('');
                }
            }
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

var GenerarPDF = function (id) {
    var generar = function (id) {
        var qrc = new QRCode(document.getElementById('qrcode'), location.href.replace('ConstanciaUltimaVerificacion', 'DescargaDocumentos/Index/' + id));
    };

    return {
        generar: function (id) {
            generar(id);
        }
    }

}();

jQuery(document).ready(function () {
    KTDatatableRemoteAjax.init();
});