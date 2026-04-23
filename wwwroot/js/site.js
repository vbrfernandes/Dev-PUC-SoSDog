// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// wwwroot/js/dog-api.js
document.addEventListener("DOMContentLoaded", async () => {
    const dogImages = document.querySelectorAll(".dog-api-img");

    if (dogImages.length === 0) return;

    try {
        // Busca imagens aleatórias baseadas na quantidade de cards na tela
        const response = await fetch(`https://dog.ceo/api/breeds/image/random/${dogImages.length}`);
        const data = await response.json();

        if (data.status === "success") {
            dogImages.forEach((img, index) => {
                // Efeito suave de carregamento
                img.style.opacity = "0";
                img.src = data.message[index];

                img.onload = () => {
                    img.style.transition = "opacity 0.5s ease-in";
                    img.style.opacity = "1";
                };
            });
        }
    } catch (error) {
        console.error("Erro ao conectar com a Dog API:", error);
    }
});