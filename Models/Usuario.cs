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

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Senha { get; set; }

        public string? Foto_Perfil { get; set; }

        public DateTime Data_Cadastro { get; set; } = DateTime.Now;

        // Atributos do Diagrama de Classes
        public string? LocalizacaoAtual { get; set; }
        public string? Bio { get; set; }

        // Atributo Privado (Apenas acessível dentro desta classe)
        private int? _telefone;

        // Propriedade pública para acessar o telefone com segurança, se necessário
        public int? Telefone
        {
            get { return _telefone; }
            set { _telefone = value; }
        }

        // Relacionamentos (Navegação)
        public virtual ICollection<Ocorrencia> OcorrenciasRegistradas { get; set; } = new List<Ocorrencia>();
        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public virtual ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();

        // Métodos do Diagrama de Classes (Assinaturas)
        public void CadastrarConta() { /* Lógica aqui */ }
        public void EditarPerfil() { /* Lógica aqui */ }
        public void ExcluirConta() { /* Lógica aqui */ }
        public void RedefinirSenha() { /* Lógica aqui */ }
        public void Logout() { /* Lógica aqui */ }
    }
}