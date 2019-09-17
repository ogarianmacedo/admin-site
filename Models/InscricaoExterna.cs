using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdminSite.Models
{
    public class InscricaoExterna
    {
        public int InscricaoExternaId { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Mensagem { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DtInscricao { get; set; } 
    }
}
