// Variável global para rastrear qual ocorrência está aberta no painel lateral
let ocorrenciaSelecionadaId = null;

document.addEventListener("DOMContentLoaded", function () {

    // ==========================================
    // 1. LÓGICA DO MAPA - MODAL DE OCORRÊNCIAS
    // ==========================================
    const modalOcorrenciaElement = document.getElementById('modalOcorrencia');
    let mapPicker;
    let marker;

    if (modalOcorrenciaElement) {
        modalOcorrenciaElement.addEventListener('shown.bs.modal', function () {
            if (!mapPicker) {
                mapPicker = L.map('mapPicker').setView([-19.9167, -43.9333], 12);
                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    maxZoom: 19,
                    attribution: '© OpenStreetMap'
                }).addTo(mapPicker);

                mapPicker.on('click', function (e) {
                    const lat = e.latlng.lat;
                    const lng = e.latlng.lng;
                    if (marker) {
                        marker.setLatLng(e.latlng);
                    } else {
                        marker = L.marker(e.latlng).addTo(mapPicker);
                    }
                    document.getElementById('lat').value = lat;
                    document.getElementById('lng').value = lng;
                });
            }
            setTimeout(() => { mapPicker.invalidateSize(); }, 100);
        });
    }

    // ==========================================
    // 2. LÓGICA DE DETALHES (SIDEBAR DIREITA)
    // ==========================================
    const cards = document.querySelectorAll('.case-card');
    const painelDetalhes = document.getElementById('painel-detalhes');
    const btnEditar = document.getElementById('btn-editar-ocorrencia');

    cards.forEach(card => {
        card.addEventListener('click', function () {
            painelDetalhes.style.display = 'flex';

            // ATUALIZAÇÃO: Salva o ID da ocorrência clicada
            ocorrenciaSelecionadaId = this.dataset.id;

            const idCodigo = this.dataset.codigo || `ID #${this.dataset.id}`;
            const sexo = this.dataset.sexo || 'Não informado';
            const cor = this.dataset.cor || 'Não informada';
            const porte = this.dataset.porte || 'Não informado';
            const sociabilidade = this.dataset.sociabilidade || 'Não informada';
            const idade = this.dataset.idade || 'Não informada';
            const idUsuario = this.dataset.usuario || 'Desconhecido';
            const dataAlimentacao = this.dataset.alimentacao || 'Sem registro';

            const imgElement = this.querySelector('img');
            const imgSrc = imgElement ? imgElement.src : '/img/placeholder-dog.png';

            // Injeta os dados no HTML
            document.getElementById('sidebar-titulo-id').innerText = idCodigo;
            document.getElementById('sidebar-foto').src = imgSrc;
            document.getElementById('sidebar-sexo').innerText = sexo;
            document.getElementById('sidebar-cor').innerText = cor;
            document.getElementById('sidebar-porte').innerText = porte;
            document.getElementById('sidebar-sociabilidade').innerText = sociabilidade;
            document.getElementById('sidebar-idade').innerText = idade;

            document.getElementById('sidebar-log-usuario').innerHTML =
                `ID usuário: ${idUsuario}<br>Alimentado:<br>${dataAlimentacao}`;

            // Destaque visual do card
            cards.forEach(c => c.classList.remove('border', 'border-success', 'bg-light'));
            this.classList.add('border', 'border-success', 'bg-light');
        });
    });

    // ==========================================
    // 3. LÓGICA DO BOTÃO EDITAR
    // ==========================================
    if (btnEditar) {
        btnEditar.addEventListener('click', function () {
            if (this.classList.contains('auth-required')) return;

            if (ocorrenciaSelecionadaId) {

                // 1. Em vez de sair da página, fazemos uma requisição "escondida" (AJAX)
                fetch(`/Ocorrencias/Edit/${ocorrenciaSelecionadaId}`)
                    .then(response => {
                        if (response.status === 401) {
                            const loginModal = new bootstrap.Modal(document.getElementById('loginModal'));
                            loginModal.show();
                            throw new Error("Login necessário.");
                        }
                        if (!response.ok) {
                            throw new Error("Erro ao carregar os dados. Você tem permissão?");
                        }
                        // Pega o HTML cru que o Controller devolveu (a PartialView)
                        return response.text();
                    })
                    .then(html => {
                        // 2. Injeta esse HTML dentro daquela div vazia no seu Index.cshtml
                        document.getElementById('editModalContainer').innerHTML = html;

                        // 3. Avisa ao Bootstrap que isso é um modal e manda abrir!
                        const modalEditElement = document.getElementById('modalEditarOcorrencia');
                        const modalEdit = new bootstrap.Modal(modalEditElement);
                        modalEdit.show();
                    })
                    .catch(error => {
                        console.error(error);
                        alert("Não foi possível carregar a edição.");
                    });

            } else {
                alert("Por favor, selecione uma ocorrência primeiro.");
            }
        });
    }
});