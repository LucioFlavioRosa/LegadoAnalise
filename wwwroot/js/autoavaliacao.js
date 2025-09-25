// AutoAvaliacao JavaScript Functions

// Configuração do auto-save
window.configureAutoSave = (dotNetHelper) => {
    if (dotNetHelper) {
        // Auto-save a cada 15 minutos (900000 ms)
        setInterval(() => {
            try {
                dotNetHelper.invokeMethodAsync('AutoSave');
            } catch (error) {
                console.error('Erro no auto-save:', error);
            }
        }, 900000);
    }
};

// Inicialização dos componentes Bootstrap
window.initializeBootstrapComponents = () => {
    try {
        // Inicializar tooltips
        if (typeof $ !== 'undefined' && $.fn.tooltip) {
            $('[data-toggle="tooltip"]').tooltip();
        }

        // Inicializar popovers
        if (typeof $ !== 'undefined' && $.fn.popover) {
            $('[data-toggle="popover"]').popover();
        }

        // Inicializar select2 se disponível
        if (typeof $ !== 'undefined' && $.fn.select2) {
            $('.select2').select2({
                theme: 'bootstrap4',
                width: '100%'
            });
        }

        // Configurar accordions
        $('.accordion-toggle').on('click', function(e) {
            e.preventDefault();
            const target = $(this).attr('data-target') || $(this).attr('href');
            if (target) {
                $(target).collapse('toggle');
            }
        });

    } catch (error) {
        console.error('Erro ao inicializar componentes Bootstrap:', error);
    }
};

// Função para mostrar mensagens usando SweetAlert
window.showMessage = (type, message, duration = 5000) => {
    if (typeof swal !== 'undefined') {
        swal({
            title: "ATENÇÃO!",
            text: message,
            icon: type,
            timer: duration,
            button: "Ok",
        });
    } else {
        // Fallback para alert nativo
        alert(message);
    }
};

// Função para truncar texto
window.truncateText = (text, maxLength) => {
    if (!text || text.length <= maxLength) {
        return text;
    }
    return text.substring(0, maxLength) + '...';
};

// Função para validar formulário
window.validateForm = (formSelector) => {
    try {
        const form = document.querySelector(formSelector);
        if (!form) return false;

        const requiredFields = form.querySelectorAll('[required]');
        let isValid = true;

        requiredFields.forEach(field => {
            if (!field.value.trim()) {
                field.classList.add('is-invalid');
                isValid = false;
            } else {
                field.classList.remove('is-invalid');
            }
        });

        return isValid;
    } catch (error) {
        console.error('Erro na validação do formulário:', error);
        return false;
    }
};

// Função para destacar campos com erro
window.highlightErrorFields = (fieldIds, errorClass = 'is-invalid') => {
    try {
        fieldIds.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                field.classList.add(errorClass);
            }
        });
    } catch (error) {
        console.error('Erro ao destacar campos com erro:', error);
    }
};

// Função para remover destaque de erro
window.clearErrorHighlight = (fieldIds, errorClass = 'is-invalid') => {
    try {
        fieldIds.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                field.classList.remove(errorClass);
            }
        });
    } catch (error) {
        console.error('Erro ao remover destaque de erro:', error);
    }
};

// Função para scroll suave até elemento
window.scrollToElement = (elementId, offset = 0) => {
    try {
        const element = document.getElementById(elementId);
        if (element) {
            const elementPosition = element.offsetTop - offset;
            window.scrollTo({
                top: elementPosition,
                behavior: 'smooth'
            });
        }
    } catch (error) {
        console.error('Erro no scroll:', error);
    }
};

// Função para confirmar ação
window.confirmAction = (message, callback) => {
    if (typeof swal !== 'undefined') {
        swal({
            title: "Confirmação",
            text: message,
            icon: "warning",
            buttons: ["Cancelar", "Confirmar"],
            dangerMode: true,
        })
        .then((willProceed) => {
            if (willProceed && callback) {
                callback();
            }
        });
    } else {
        // Fallback para confirm nativo
        if (confirm(message) && callback) {
            callback();
        }
    }
};

// Inicialização quando o DOM estiver carregado
document.addEventListener('DOMContentLoaded', function() {
    // Inicializar componentes Bootstrap
    initializeBootstrapComponents();

    // Configurar eventos globais
    document.addEventListener('click', function(e) {
        // Fechar dropdowns ao clicar fora
        if (!e.target.closest('.dropdown')) {
            const dropdowns = document.querySelectorAll('.dropdown-menu.show');
            dropdowns.forEach(dropdown => {
                dropdown.classList.remove('show');
            });
        }
    });

    // Configurar teclas de atalho
    document.addEventListener('keydown', function(e) {
        // Ctrl+S para salvar
        if (e.ctrlKey && e.key === 's') {
            e.preventDefault();
            const saveButton = document.querySelector('[data-action="save"]');
            if (saveButton && !saveButton.disabled) {
                saveButton.click();
            }
        }
    });
});