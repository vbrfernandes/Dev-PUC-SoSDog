// wwwroot/js/usuario.js

document.addEventListener("DOMContentLoaded", function () {

    // ==========================================
    // LÓGICA DE REABERTURA DO MODAL DE LOGIN
    // ==========================================
    const loginModalElement = document.getElementById('loginModal');

    if (loginModalElement) {
        // Verifica se o Razor injetou "true" no atributo data-has-error
        const hasError = loginModalElement.getAttribute('data-has-error');

        if (hasError === "true") {
            // Instancia e exibe o modal usando o Bootstrap via JS
            const loginModal = new bootstrap.Modal(loginModalElement);
            loginModal.show();
        }
    }
    // ==========================================
    // LÓGICA DE REABERTURA DO MODAL DE CADASTRO
    // ==========================================
    const cadastroModalElement = document.getElementById('cadastroModal');
    if (cadastroModalElement) {
        const hasError = cadastroModalElement.getAttribute('data-has-error');
        if (hasError === "true") {
            const cadastroModal = new bootstrap.Modal(cadastroModalElement);
            cadastroModal.show();
        }
    }
});
// ==========================================
// LÓGICA DE ABERTURA DO MODAL DE TOKEN
// ==========================================
const tokenModalElement = document.getElementById('modalToken');

if (tokenModalElement) {
    const abrirToken = tokenModalElement.getAttribute('data-abrir-token');

    if (abrirToken === "true") {
        const tokenModal = new bootstrap.Modal(tokenModalElement);
        tokenModal.show();
    }
}