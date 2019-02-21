using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public class Usuario
    {
        [Key]
        [StringLength(450)]
        [JsonIgnore]
        public string Id { get; set; }
        [StringLength(256)]
        public string Login { get; set; }
        [StringLength(256)]
        public string Nome { get; set; }
        [NotMapped]
        public string Senha { get; set; }
        [StringLength(50)]
        public string Perfil { get; set; }

        [NotMapped]
        public string CodigoRecuperacao { get; set; }
        

    }
}
