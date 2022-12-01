using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace UaiLink.Models.Enum
{
    public enum CategoriaProjetoEnum
    {
        [Display(Name = "Ensino")]
        Ensino = 0,
        [Display(Name = "Pesquisa")]
        Pesquisa = 1,
        [Display(Name = "Extenção")]
        Extencao = 2,
        [Display(Name = "Atividades de estágio")]
        AtividadeEstagio = 3,
        [Display(Name = "Monitorias")]
        Monitorias = 4,
        [Display(Name = "Curso")]
        Cursos = 5
    }
}
