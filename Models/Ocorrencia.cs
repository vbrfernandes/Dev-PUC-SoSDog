using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SosDog.Models
{
    public class Ocorrencia
    {
        [Key]
        public int ID_Ocorrencia { get; set; }

        // ==========================================
        // NOVOS CAMPOS: PERFIL DO ANIMAL (OPCIONAIS)
        // ==========================================
        public string? Codigo_Cachorro { get; set; }

        public string? Sexo { get; set; } // O '?' permite que seja nulo no banco

        public string? Cor_Pelagem { get; set; }

        public string? Porte { get; set; }

        public string? Sociabilidade { get; set; }

        public string? Faixa_Etaria { get; set; }

        [Required]
        public string? Endereco { get; set; }

        // ==========================================
        // REGISTRO DE AÇÕES (CUIDADOS BÁSICOS)
        // ==========================================
        public bool Recebeu_Agua { get; set; } = false;

        public bool Recebeu_Comida { get; set; } = false;

        public DateTime? Data_Ultima_Alimentacao { get; set; }

        // ==========================================
        // CAMPOS ORIGINAIS
        // ==========================================
        [Required(ErrorMessage = "O tipo é obrigatório")]
        public string? Tipo { get; set; }

        public string? Foto_Animal { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string? Descricao { get; set; }

        // Se a localização também for opcional (ex: a pessoa prefere digitar só o endereço), usamos float?
        // Mas se o mapa for o coração do sistema, você pode deixar como 'public float Latitude' e tratar no JS.
        public float? Latitude { get; set; }

        public float? Longitude { get; set; }

        [Required]
        public DateTime Data_Registro { get; set; } = DateTime.UtcNow;

        [Required]
        public int ID_Usuario { get; set; }

        [ForeignKey("ID_Usuario")]
        public virtual Usuario? Usuario { get; set; }

        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public virtual ICollection<Favorito> FavoritadosPor { get; set; } = new List<Favorito>();
    }
}