using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class Endereco
    {
        public int Id { get; set; }
        [Required]
        public string Cidade { get; set; }
        [Required]
        [MaxLength(2), MinLength(2)]
        public string UF { get; set; }
        [Required]
        [StringLength(400)]
        public string Logradouro { get; set; }
        [MaxLength(6)]
        public string Numero { get; set; }
    }
}
