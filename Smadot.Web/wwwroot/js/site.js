var SmadotUtilities = function () {
    var searchObject;
    const searchPanel = () => {
        var processs = function (search) {
            var timeout = setTimeout(function () {
                searchFunction(search);
                searchObject.complete();
            }, 1500);
        }

        var clear = function (search) {
            searchFunction('');
            // Show recently viewed
            //suggestionsElement.classList.remove("d-none");
            //// Hide results
            //resultsElement.classList.add("d-none");
            //// Hide empty message
            //emptyElement.classList.add("d-none");
        }

        // Input handler
        const handleInput = () => {
            // Select input field
            const inputField = element.querySelector('[data-kt-search-element="input"]');

            // Handle keyboard press event
            inputField.addEventListener("keydown", e => {
                // Only apply action to Enter key press
                if (e.key === "Enter") {
                    e.preventDefault(); // Stop form from submitting
                }
            });
        }

        // Elements
        element = document.querySelector('#kt_docs_search_handler_basic');

        if (!element) {
            return;
        }

        wrapperElement = element.querySelector('[data-kt-search-element="wrapper"]');
        suggestionsElement = element.querySelector('[data-kt-search-element="suggestions"]');
        resultsElement = element.querySelector('[data-kt-search-element="results"]');
        emptyElement = element.querySelector('[data-kt-search-element="empty"]');

        // Initialize search handler
        searchObject = new KTSearch(element);

        // Search handler
        searchObject.on("kt.search.process", processs);

        // Clear handler
        searchObject.on("kt.search.clear", clear);

        // Handle select
        KTUtil.on(element, '[data-kt-search-element="customer"]', "click", function () {
            //modal.hide();
        });

        // Handle input enter keypress
        handleInput();
    }

    const templateItem = (item) => {
        return `<div class="p-4 bg-hover-light-dark align-items-center flex-wrap mb-5 itemholder d-flex" data-search="${item.nombre}" style="">
                                    <div class="d-flex flex-column flex-grow-1 mr-2">
                                        <a href="#" class="text-secondary text-uppercase font-weight-bolder pb-0 btnCambioCvv" data-id="${item.id}">${item.nombre}</a>
                                    </div>
                                </div>`
    }
    var searchFunction = (search) => {
        // Hide recently viewed
        suggestionsElement.classList.add("d-none");
        resultsElement.innerHTML = '';
        $.ajax({
            url: "/Autenticacion/GetVerificentroChange",
            type: "POST",
            dataType: "json",
            data: {
                searchParam: searchObject.getQuery()
            },
            success: function (data) {
                if (data.isSuccessFully) {
                    if (data.result) {
                        // Show results
                        resultsElement.classList.remove("d-none");
                        // Hide empty message
                        emptyElement.classList.add("d-none");
                        data.result.forEach((item, i) => {
                            var div = document.createElement('div');
                            div.innerHTML = templateItem(item)
                            resultsElement.append(div)
                        });
                        listeners();
                        blockUI.release();

                        return;
                    }
                }
                // Hide results
                resultsElement.classList.add("d-none");
                // Show empty message
                emptyElement.classList.remove("d-none");
                // Complete search

            }
        });


    }
    const MarkActiveOption = () => {
        var URLactual = window.location;
        var options = document.getElementsByClassName('menu-link');
        var arrUrl = URLactual.toString().split('/');
        if (arrUrl.includes("ProgramacionCalibracion")) {
            let o = document.querySelectorAll('[href="/Equipo"]')[0];
            ProcessOption(o, arrUrl, true);
            return;
        }
        options.forEach((o, i) => {
            ProcessOption(o, arrUrl);

        });
    }
    const ProcessOption = (o, arrUrl, direct = false) => {
        if (o.getAttribute("href")) {
            if (o.getAttribute("href") === "#")
                return;
            let urllink = o.getAttribute("href").substring(1);

            if (arrUrl.some(x => x === urllink) || direct) {

                let divParent = o.parentElement;
                if (!divParent.classList.contains("menu-item")) {
                    o.classList.add('here');
                    let menuItemPrincipal = o.parentElement.parentElement;
                    let firstMenuItemPrincipal = menuItemPrincipal.getElementsByClassName("menu-link")[0];
                    if (firstMenuItemPrincipal)
                        firstMenuItemPrincipal.classList.add('here');
                    return;
                }

                divParent.classList.add('here');
                let menuParent = divParent.parentElement.parentElement;
                let firstMenuItem = menuParent.getElementsByClassName("menu-link")[0];
                if (firstMenuItem) {
                    firstMenuItem.classList.add('here');
                    let menuItemPrincipal = firstMenuItem.parentElement.parentElement.parentElement;
                    let firstMenuItemPrincipal = menuItemPrincipal.getElementsByClassName("menu-link")[0];
                    if (firstMenuItemPrincipal)
                        firstMenuItemPrincipal.classList.add('here');


                }
            }
        }
    }
    const listeners = () => {
        $('.btnCambioCvv').on('click', function (e) {
            e.preventDefault();
            let id = $(this).data('id');
            $.ajax({
                url: "/Autenticacion/ChangeVerificentro",
                type: "POST",
                dataType: "json",
                data: {
                    id: id
                },
                success: function (data) {
                    if (data.isSuccessFully) {
                        location.reload();
                    } else {
                        toastr.error(data.message ?? "Ocurrió un error al realizar la operación.", 'SMADSOT');
                    }
                }
            });
        })
    }
    return {
        initSearchPanel: () => {
            searchPanel();
            MarkActiveOption();
        }, InitalSearch: () => {
            searchFunction('');
        }
    }
}()

let formValInitModalLayoutRegistroConfiguracion = '';
$('#btnModalLayoutRegistroConfiguracion').click(function (e) {
    e.preventDefault();
    ModalLayoutRegistroConfiguracion.init();
});

var ModalLayoutRegistroConfiguracion = function () {
    var init = function () {
        abrirModal();
    }
    var abrirModal = function () {
        $.ajax({
            cache: false,
            type: 'GET',
            url: '/DirectorioCentrosVerificacion/ConfiguracionLayout',
            success: function (result) {
                if (!result.isSuccessFully) {
                    toastr.error(result.message, "SMADSOT");
                    return;
                }
                $('#modal_LayoutRegistroConfiguracion #modalLabelTitleLayoutRegistroConfiguracion').html("Configuración");
                $('#modal_LayoutRegistroConfiguracion .modal-body').html(result.result);
                $('#modalClassLayoutRegistroConfiguracion').removeClass('modal-md');
                $('#modalClassLayoutRegistroConfiguracion').addClass('modal-xl');
                $('#modal_LayoutRegistroConfiguracion').modal('show');
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

        $('.btnGuardarLayoutRegistroConfiguracion').off().on('click', function (e) {
            e.preventDefault();
            if (formValInitModalLayoutRegistroConfiguracion !== this.dataset.selected)
                GuardarFormularioModalLayoutRegistroConfiguracion.init(this.dataset.selected);
            GuardarFormularioModalLayoutRegistroConfiguracion.guardar();
        });

    }
    // Cerramos la ventana modal
    var cerrarModal = function () {
        //$('#btnCancelarLayoutRegistroConfiguracion').click();
        $('#btnCerrarLayoutRegistroConfiguracion').click();
        $('#modal_LayoutRegistroConfiguracion .modal-body').html();
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

var GuardarFormularioModalLayoutRegistroConfiguracion = function () {
    var validation; // https://formvalidation.io/

    var init = function (id) {
        init_validacion(id);
    };
    var guardar = function () {
        validation.validate().then(function (status) {
            if (status === 'Valid') {
                var form = $('#form_LayoutRegistroConfiguracion')[0];
                var formData = new FormData(form);
                var newFormData = new FormData();
                for (const pair of formData.entries()) {
                    if (formValInitModalLayoutRegistroConfiguracion === undefined && pair[0].includes('.Selected.')) {
                        pair[1] = 'True';
                    } else if (formValInitModalLayoutRegistroConfiguracion !== undefined && '[' + formValInitModalLayoutRegistroConfiguracion.split('.')[0].replace('z', '') + ']' === pair[0].split('.')[0] && pair[0].includes('.Selected')) {
                        pair[1] = 'True';
                    }
                    pair[0] = pair[0].replace('.InputLayoutRegistroConfiguracion', '');
                    newFormData.append(pair[0], pair[1]);
                }
                $.ajax({
                    cache: false,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    enctype: 'multipart/form-data',
                    url: '/DirectorioCentrosVerificacion/ConfiguracionLayout',
                    data: newFormData,
                    success: function (result) {
                        if (!result.isSuccessFully) {
                            //$(window).scrollTop(0);
                            toastr.error(result.message, "SMADSOT");
                        } else {
                            //$(window).scrollTop(0);
                            toastr.success(result.message, "SMADSOT");
                        }
                        return;
                    },
                    error: function (res) {
                        toastr.error(res, "SMADSOT");
                    }
                });
            }
        });
    };

    var init_validacion = function (id) {
        var form = document.getElementById('form_LayoutRegistroConfiguracion');
        if (validation !== undefined)
            validation.destroy();
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

        var total = $('.selectedInputLayoutRegistroConfiguracion').length;
        var i = 1;
        if (id === undefined) {
            validation.addField('[0].Precio.InputLayoutRegistroConfiguracion', {
                validators: {
                    notEmpty: {
                        message: 'El precio es requerido.'
                    },
                    greaterThan: {
                        min: 1,
                        message: 'Solo se permiten números igual o mayor a 0',
                    },
                }
            });
            while (i < total) {
                validation.addField('[' + i + '].Folio.InputLayoutRegistroConfiguracion', {
                    validators: {
                        notEmpty: {
                            message: 'El folio es requerido.'
                        },
                        greaterThan: {
                            min: 1,
                            message: 'Solo se permiten números igual o mayor a 1',
                        },
                    }
                });
                i++;
            }
        } else if (id.includes('.Precio.')) {
            validation.addField('[0].Precio.InputLayoutRegistroConfiguracion', {
                validators: {
                    notEmpty: {
                        message: 'El precio es requerido.'
                    },
                    greaterThan: {
                        min: 1,
                        message: 'Solo se permiten números igual o mayor a 0',
                    },
                }
            });
        }
        else {
            validation.addField('[' + id.split('.')[0].replace('z', '') + '].Folio.InputLayoutRegistroConfiguracion', {
                validators: {
                    notEmpty: {
                        message: 'El folio es requerido.'
                    },
                    greaterThan: {
                        min: 1,
                        message: 'Solo se permiten números igual o mayor a 1',
                    },
                }
            });
        }
        formValInitModalLayoutRegistroConfiguracion = id;
    }
    return {
        init: function (id) {
            init(id);
        },
        guardar: function () {
            guardar();
        }
    }

}();

// $(document).ajaxStart(function () {
//     if (!blockUI.isBlocked())
//         blockUI.block();
// });

$(document).ready(function () {
    $.ajaxSetup({
        beforeSend: function (jqXHR, settings) {
            if (settings.url.toString().includes('MenuNotificaciones') || settings.url.toString().includes('EstadisiticaUsoFormaValorada')) {
                return; // Salir sin activar el bloqueo de interfaz de usuario
            }
    
            // Activar el bloqueo de interfaz de usuario para todas las demás solicitudes Ajax
            if (!blockUI.isBlocked()) {
                blockUI.block();
            }
        }
    });
    
    $(document).ajaxStop(function (event, jqxhr, settings) {
        //if ((typeof settings !== 'undefined') && !settings.url.includes("FinanzasPersonales/CrearCredencial"))
        blockUI.release();
    }).ajaxError(function () {
        blockUI.release();
    }).ajaxSuccess(function () {
        blockUI.release();
    }).ajaxComplete(function (event, jqxhr, settings) {
        blockUI.release();
    });
    SmadotUtilities.initSearchPanel();
    SmadotUtilities.InitalSearch();

})