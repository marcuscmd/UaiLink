using System.ComponentModel.DataAnnotations;

namespace UaiLink.Models
{
    public class ComentarioDto
    {
        [Key]
        public int ComentarioID { get; set; }
        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string Conteudo { get; set; }
        
        //public int UsuarioID { get; set; }
        //public UsuarioDto Usuario { get; set; }
        public int ProjetoID { get; set; }
        public ProjetoDto Projeto { get; set; }
    }
}
