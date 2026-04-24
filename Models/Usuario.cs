using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SosDog.Models
{
    public class Usuario
    {
        [Key]
        public int ID_Usuario { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required(ErrorMessage = "A foto de perfil é obrigatória.")]
        public string Foto_Perfil { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "Formato de telefone inválido.")]
        public string Telefone { get; set; }

        // NOVOS CAMPOS PARA O RESET DE SENHA
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiracao { get; set; }

        // Relacionamentos (Navegação)
        public virtual ICollection<Ocorrencia> OcorrenciasRegistradas { get; set; } = new List<Ocorrencia>();
        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
    }
}