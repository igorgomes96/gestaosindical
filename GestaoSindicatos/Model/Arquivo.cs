using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
        PlanoAcao
    }

    public class Arquivo
    {
        public ObjectId Id { get; set; }
        [BsonElement("nome")]
        public string Nome { get; set; }
        [BsonElement("dataUpdload")]
        public DateTime? DataUpload { get; set; }
        [BsonElement("tamanho")]
        public long Tamanho { get; set; }
        [BsonElement("content")]
        public byte[] Content { get; set; }
        [BsonElement("contentType")]
        public string ContentType { get; set; }

        // Reference
        [JsonIgnore]
        [BsonElement("dependencyType")]
        public DependencyFileType DependencyType { get; set; }
        [JsonIgnore]
        [BsonElement("dependencyId")]
        public int DependencyId { get; set; }


    }
}
