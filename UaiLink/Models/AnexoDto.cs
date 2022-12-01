using System.ComponentModel.DataAnnotations;

namespace UaiLink.Models
{
    public class AnexoDto
    {
        [Key]
        public int AnexoID { get; set; }
        [Required(ErrorMessage = "Campo Obrigatorio")]
        public string CampoAnexo { get; set; }
        public string NomeAnexo { get; set; }

        public int ProjetoID { get; set; }
        public ProjetoDto Projeto { get; set; }
    }
}
