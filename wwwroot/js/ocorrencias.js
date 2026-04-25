// wwwroot/js/ocorrencias.js

document.addEventListener("DOMContentLoaded", function () {

    // ==========================================
    // LÓGICA DO MAPA - MODAL DE OCORRÊNCIAS
    // ==========================================
    const modalOcorrenciaElement = document.getElementById('modalOcorrencia');
    let mapPicker;
    let marker;

    if (modalOcorrenciaElement) {
        // Evento disparado assim que o modal termina de abrir
        modalOcorrenciaElement.addEventListener('shown.bs.modal', function () {

            // Só inicializa o mapa na primeira vez que o modal abre
            if (!mapPicker) {
                // Centralizado em Belo Horizonte (ajuste se necessário)
                mapPicker = L.map('mapPicker').setView([-19.9167, -43.9333], 12);

                L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                    maxZoom: 19,
                    attribution: '© OpenStreetMap'
                }).addTo(mapPicker);

                // Evento de clique no mapa
                mapPicker.on('click', function (e) {
                    const lat = e.latlng.lat;
                    const lng = e.latlng.lng;

                    // Cria o marcador ou move ele se já existir
                    if (marker) {
                        marker.setLatLng(e.latlng);
                    } else {
                        marker = L.marker(e.latlng).addTo(mapPicker);
                    }

                    // Preenche os inputs ocultos do formulário
                    document.getElementById('lat').value = lat;
                    document.getElementById('lng').value = lng;
                });
            }

            // Corrige o bug de renderização do Leaflet dentro de modais
            setTimeout(() => {
                mapPicker.invalidateSize();
            }, 100);
        });
    }
});


document.addEventListener('DOMContentLoaded', function () {
    // Pega todos os cards da esquerda
    const cards = document.querySelectorAll('.case-card');

    // Pega o painel da direita que criamos
    const painelDetalhes = document.getElementById('painel-detalhes');

    cards.forEach(card => {
        card.addEventListener('click', function () {
            // 1. Torna o painel da direita visível
            painelDetalhes.style.display = 'flex'; // ou block, dependendo do seu CSS

            // 2. Coleta os dados do card clicado usando o dataset
            const idCodigo = this.dataset.codigo || `ID #${this.dataset.id}`;
            const sexo = this.dataset.sexo || 'Não informado';
            const cor = this.dataset.cor || 'Não informada';
            const porte = this.dataset.porte || 'Não informado';
            const sociabilidade = this.dataset.sociabilidade || 'Não informada';
            const idade = this.dataset.idade || 'Não informada';
            const idUsuario = this.dataset.usuario || 'Desconhecido';
            const dataAlimentacao = this.dataset.alimentacao || 'Sem registro';

            // Pega a imagem exata que está aparecendo no card (pode ser da API de cães ou do banco)
            const imgElement = this.querySelector('img');
            const imgSrc = imgElement ? imgElement.src : '/img/placeholder-dog.png';

            // 3. Injeta os dados na barra lateral direita
            document.getElementById('sidebar-titulo-id').innerText = idCodigo;
            document.getElementById('sidebar-foto').src = imgSrc;
            document.getElementById('sidebar-sexo').innerText = sexo;
            document.getElementById('sidebar-cor').innerText = cor;
            document.getElementById('sidebar-porte').innerText = porte;
            document.getElementById('sidebar-sociabilidade').innerText = sociabilidade;
            document.getElementById('sidebar-idade').innerText = idade;

            // Formata o texto da caixa do usuário
            document.getElementById('sidebar-log-usuario').innerHTML =
                `ID usuário: ${idUsuario}<br>Alimentado:<br>${dataAlimentacao}`;

            // 4. (Opcional) Destacar o card que está selecionado
            cards.forEach(c => c.classList.remove('border', 'border-success', 'bg-light')); // Remove destaque dos outros
            this.classList.add('border', 'border-success', 'bg-light'); // Adiciona destaque no clicado
        });
    });
});