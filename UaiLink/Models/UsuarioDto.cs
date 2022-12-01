using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using UaiLink.Models.Enum;

namespace UaiLink.Models
{
    public class UsuarioDto
    {
        [System.ComponentModel.DataAnnotations.Key]
        public int UsuarioID { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Digite seu nome")]
        public string Nome { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Senha é obrigatorio")]
        public string Senha { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Email é obrigatorio")]
        [System.ComponentModel.DataAnnotations.EmailAddress(ErrorMessage = "Email Invalido")]
        public string Email { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "CPF é obrigatorio")]
        public string Cpf { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "CEP é obrigatorio")]
        public string CEP { get; set; }
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Digite seu telefone")]
        public string Telefone { get; set; }
        public string Curso { get; set; }
        public string Descricao { get; set; }
        public DateTime DataInicioCurso { get; set; }
        public DateTime PrevisaoFimCurso { get; set; }
        public EspecificacaoVagaEnum EspecificacaoVaga { get; set; }
        public ICollection<ProjetoDto> Projetos { get; set; }
        public TipoUsuarioEnum TipoUsuario { get; set; }
        public ICollection<ComentarioDto> Comentarios { get; set; }
        public ICollection<TagDto> Tag { get; set; }
        public string NomeTags { get; set; }
        public string UrlFoto { get; set; }

    }
}
