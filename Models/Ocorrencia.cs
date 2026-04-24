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

        [Required]
        public string Tipo { get; set; } // Ex: "Perdido", "Rua"

        [Required]
        public string Status { get; set; } // Ex: "Aberto", "Resolvido"

        public string? Foto_Animal { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Required]
        public float Latitude { get; set; }

        [Required]
        public float Longitude { get; set; }

        [Required]
        public DateTime Data_Registro { get; set; } = DateTime.UtcNow;

        // Chave Estrangeira - Registra (1:N)
        [Required]
        public int ID_Usuario { get; set; }

        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; }

        // Relacionamentos (Navegação)
        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public virtual ICollection<Favorito> FavoritadosPor { get; set; } = new List<Favorito>();

    }
}