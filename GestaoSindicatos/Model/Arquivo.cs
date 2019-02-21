using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Model
{
    public enum DependencyFileType
    {
        Empresa,
        SindicatoLaboral,
        SindicatoPatronal,
        Negociacao,
        RodadaNegociacao,
        Litigio,
        PlanoAcao,
        ItemLitigio
    }

    public class Arquivo
    {
        public long Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Nome { get; set; }
        public DateTime? DataUpload { get; set; }
        public long Tamanho { get; set; }
        // public byte[] Content { get; set; }
        [StringLength(255)]
        public string ContentType { get; set; }
        [StringLength(255)]
        public string Path { get; set; }        

        // Reference
        [JsonIgnore]
        public DependencyFileType DependencyType { get; set; }
        [JsonIgnore]
        public int DependencyId { get; set; }


    }
}
