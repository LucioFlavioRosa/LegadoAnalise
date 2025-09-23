// Função para download de arquivos
window.downloadFile = (filename, base64Data) => {
    const link = document.createElement('a');
    link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,' + base64Data;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

// Função para confirmar ações
window.confirm = (message) => {
    return confirm(message);
};