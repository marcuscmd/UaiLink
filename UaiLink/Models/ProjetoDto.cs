using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UaiLink.Models.Enum;

namespace UaiLink.Models
{
    public class ProjetoDto
    {
        [Key]
        public int ProjetoID { get; set; }
        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string Titulo { get; set; }
        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string Descricao { get; set; }
        [Required(ErrorMessage = "Campo Obrigatorio")]
        public CategoriaProjetoEnum Categoria { get; set; }
        public int UsuarioID { get; set; }
        public UsuarioDto Usuario { get; set; }
        public ICollection<ComentarioDto> Comentarios { get; set; }
        public AnexoDto Anexo { get; set; }
    }
}
