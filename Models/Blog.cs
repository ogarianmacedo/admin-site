using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoAdminSite.Models
{
    public class Blog
    {
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string Texto { get; set; }

        public string Imagem { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyy}")]
        public DateTime DtPublicacao { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public bool Ativo { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }
    }
}
