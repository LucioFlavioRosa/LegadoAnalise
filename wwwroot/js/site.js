// Funções JavaScript para integração com Blazor (Interop)
window.siteInterop = {
    showMessageBox: function (type, message) {
        // Exemplo simples: pode ser substituído por um modal/toast de biblioteca JS
        alert((type ? type.toUpperCase() + ': ' : '') + message);
    },
    initializeDataTable: function (tableId) {
        if (window.$ && window.$.fn.dataTable) {
            $('#' + tableId).DataTable();
        }
    },
    getContentEditableValue: function (elementId) {
        var el = document.getElementById(elementId);
        return el ? el.innerText : '';
    },
    setContentEditableValue: function (elementId, value) {
        var el = document.getElementById(elementId);
        if (el) el.innerText = value;
    }
};