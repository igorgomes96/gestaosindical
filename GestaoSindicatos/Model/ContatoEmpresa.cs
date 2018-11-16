using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class ContatoEmpresa
    {
        public int Id { get; set; }
        [Required]
        public int ContatoId { get; set; }
        [Required]
        public int EmpresaId { get; set; }

        public virtual Contato Contato { get; set; }
        public virtual Empresa Empresa { get; set; }
    }
}
