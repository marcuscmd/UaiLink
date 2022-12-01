using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UaiLink.Models.Enum;

namespace UaiLink.Models
{
    public class TagDto
    {
        [Key]
        public int TagId { get; set; }
        [Required(ErrorMessage = "Obrigatorio")]
        public string TagName { get; set; }
        public int UsuarioID { get; set; }

    }
}
