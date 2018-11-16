using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class ArquivosService
    {
        private readonly ArquivosRepository _arquivosRepository;
        public ArquivosService(ArquivosRepository arquivosRepository)
        {
            _arquivosRepository = arquivosRepository;
        }

        public List<Arquivo> GetFiles(DependencyFileType dependency, int id)
        {
            return _arquivosRepository.Query(a => a.DependencyId == id && a.DependencyType == dependency);
        }

        public void DeleteFiles(DependencyFileType dependency, int id)
        {
            _arquivosRepository.DeleteMany(a => a.DependencyId == id && a.DependencyType == dependency);
        }


        public void SaveFiles(DependencyFileType dependencyType, int dependencyId, IFormFileCollection files)
        {

            var filePath = Path.GetTempFileName();

            List<Arquivo> arquivos = new List<Arquivo>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }

                    byte[] fileContent;
                    FileStream fileStream = File.Open(filePath, FileMode.Open);
                    using (BinaryReader streamReader = new BinaryReader(fileStream))
                    {
                        fileContent = streamReader.ReadBytes((int)fileStream.Length);
                    }

                    arquivos.Add(new Arquivo
                    {
                        DataUpload = DateTime.Now,
                        DependencyId = dependencyId,
                        DependencyType = dependencyType,
                        Nome = formFile.FileName,
                        Tamanho = formFile.Length,
                        Content = fileContent,
                        ContentType = formFile.ContentType
                    });
                }
                File.Delete(filePath);
            }
            _arquivosRepository.InsertMany(arquivos);
        }
    }
}
