using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SosDog.Models
{
    public class Comentario
    {
        [Key]
        public int ID_Comentario { get; set; }

        [Required]
        public string Texto { get; set; }

        public DateTime Data_hora { get; set; } = DateTime.Now;

        // Chave Estrangeira - Autor do comentário
        [Required]
        public int ID_Usuario { get; set; }

        [ForeignKey("ID_Usuario")]
        public virtual Usuario Usuario { get; set; }

        // Chave Estrangeira - Onde o comentário foi feito
        [Required]
        public int ID_Ocorrencia { get; set; }

        [ForeignKey("ID_Ocorrencia")]
        public virtual Ocorrencia Ocorrencia { get; set; }
    }
}