using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class EmpresaUsuario
    {
        public int Id { get; set; }
        [Required]
        public int EmpresaId { get; set; }
        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        public virtual Empresa Empresa { get; set; }
    }
}
