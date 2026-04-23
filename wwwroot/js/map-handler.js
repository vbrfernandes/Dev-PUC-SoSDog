class SosDogMap {
    constructor() {
        this.map = null;
        this.userLocation = [-15.7801, -47.9292]; // Default: Brasília
        this.init();
    }

    init() {
        // 1. Tentar obter localização do usuário
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                (pos) => {
                    this.userLocation = [pos.coords.latitude, pos.coords.longitude];
                    this.renderMap(13);
                },
                () => this.renderMap(4) // Fallback se negar
            );
        } else {
            this.renderMap(4);
        }
    }

    renderMap(zoom) {
        this.map = L.map('map').setView(this.userLocation, zoom);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap'
        }).addTo(this.map);

        this.loadMarkersFromList();

        // Correção de renderização
        setTimeout(() => this.map.invalidateSize(), 300);
    }

    loadMarkersFromList() {
        // Pega todos os cards da lista lateral que possuem coordenadas
        const cards = document.querySelectorAll('.case-card');

        cards.forEach(card => {
            const lat = parseFloat(card.dataset.lat.replace(',', '.'));
            const lng = parseFloat(card.dataset.lng.replace(',', '.'));
            const id = card.dataset.id;

            if (!isNaN(lat) && !isNaN(lng)) {
                const marker = L.marker([lat, lng]).addTo(this.map);
                marker.bindPopup(`<b>Ocorrência #${id}</b><br><button onclick="focusCard(${id})">Ver detalhes</button>`);
            }
        });
    }
}

// Iniciar quando o DOM estiver pronto
document.addEventListener('DOMContentLoaded', () => {
    window.sosDogMap = new SosDogMap();
});

// Função para destacar o card ao clicar no mapa
function focusCard(id) {
    const card = document.querySelector(`.case-card[data-id="${id}"]`);
    if (card) {
        card.scrollIntoView({ behavior: 'smooth', block: 'center' });
        card.style.borderColor = 'var(--primary-orange)';
    }
}