using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SosDog.Models
{
    public class Favorito
    {
        [Key]
        public int ID_Favorito { get; set; }

        // Chave Estrangeira - Quem favoritou
        [Required]
        public int ID_Usuario { get; set; }

        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; }

        // Chave Estrangeira - O que foi favoritado
        [Required]
        public int ID_Ocorrencia { get; set; }

        [ForeignKey("ID_Ocorrencia")]
        public virtual Ocorrencia Ocorrencia { get; set; }
    }
}