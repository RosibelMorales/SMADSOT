"use strict";

var Ajax = function () {

    var init = function () {
        $('#rol').change(function () {
            PermisosView.init();
        });

        $('#btnSave').click(function () {
            var checked_ids = [];
            var selectedNodes = $('#kt_tree').jstree("get_selected", true);
            $.each(selectedNodes, function () {
                checked_ids.push(this.id.split('jtree_')[1]);
            });
            var selectedNodes = $('#kt_tree').jstree("get_undetermined", true);
            $.each(selectedNodes, function () {
                checked_ids.push(this.id.split('jtree_')[1]);
            });
            if (checked_ids.length <= 0) {
                toastr['error'](PermResources.SeleccioneAlMenos1Permiso, 'SMADSOT');
                return false;
            }
            $.ajax({
                cache: false,
                type: 'POST',
                data: { rol: $('#rol').val(), permisos: checked_ids },
                url: '/Permiso/SavePermisos',
                success: function (content) {
                    if (content.isSuccessFully) {
                        toastr['success'](PermResources.PermisosActualizadosConExito, 'SMADSOT');
                    } else {
                        toastr['error'](content.message, 'SMADSOT');
                    }
                    //$.unblockUI();
                },
                error: function () {
                    toastr['error'](PermResources.HuboErrorGuardarPermisos, 'SMADSOT');
                    //$.unblockUI();
                }
            });
        });
    };

    return {
        init: function () {
            init();
        }
    };
}();

var PermisosView = function () {

    var init = function () {
        Cargar();
    }

    var Cargar = function () {
        // Se obtiene una partialview con los permisos
        $("#kt_tree").jstree('destroy');
        $('#divTree').html('');
        $.ajax({
            cache: false,
            type: 'POST',
            data: { id: $('#rol').val() },
            url: '/Permiso/PermisosTree',
            success: function (content) {
                if (!content.error) {
                    $('#divTree').html(content.result);
                    $('#kt_tree').jstree({
                        "plugins": ["checkbox", "types"],
                        "core": {
                            "themes": {
                                "responsive": false
                            }
                        },
                        "types": {
                            "default": {
                                "icon": "fa fa-folder text-warning"
                            },
                            "file": {
                                "icon": "fa fa-file  text-warning"
                            }
                        }
                    });
                } else {
                    toastr['error'](content.errorDescription, 'SMADSOT');
                }
                //$.unblockUI();
            },
            error: function () {
                toastr['error'](PermResources.HuboErrorCargarPermisos, 'SMADSOT');
                //$.unblockUI();
            }
        });
    }

    return {
        init: function () {
            init();
        }
    }
}();

jQuery(document).ready(function () {
    Ajax.init();
    PermisosView.init();
});