using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class ArquivosService : CrudService<Arquivo>
    {
        private readonly Context _db;
        private readonly ILogger<ArquivosService> _logger;
        public ArquivosService(Context db, ILogger<ArquivosService> logger) : base(db)
        {
            _db = db;
            _logger = logger;
        }

        public List<Arquivo> GetFiles(DependencyFileType dependency, int id)
        {
            return Query(a => a.DependencyId == id && a.DependencyType == dependency).ToList();
        }

        public override Arquivo Delete(params object[] key) {
            Arquivo arquivo = base.Delete(key);
            string file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), arquivo.Path);
            try {
                _logger.LogInformation($"Excluindo arquivo {arquivo.Nome}");
                File.Delete(file);
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
            }
            return arquivo;
        }

        public override void Delete(Expression<Func<Arquivo, bool>> query) {
            Query(query).ToList().ForEach(a => Delete(a.Id));
        }

        public void DeleteFiles(DependencyFileType dependency, int id)
        {
            Delete(a => a.DependencyId == id && a.DependencyType == dependency);
        }


        private string GetFileName(string path, string filename)
        {
            int count = 1;
            string file = Path.Combine(path, filename);
            string fileName = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file);
            while (File.Exists(file))
            {
                string tempFileName = string.Format("{0} ({1}){2}", fileName, count++, extension.ToString());
                file = Path.Combine(path, tempFileName);
            }
            return file;
        }

        public (string, string) GetPath(DependencyFileType dependencyType, int dependencyId) {
            var relativePath = $@"arquivos\{dependencyType.ToString().ToLower()}\{dependencyId.ToString()}";
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), relativePath);
            return (path, relativePath);
        }

        public void SaveFiles(DependencyFileType dependencyType, int dependencyId, IFormFileCollection files)
        {
            (string path, string relativePath) = GetPath(dependencyType, dependencyId);
            Directory.CreateDirectory(path);

            List<Arquivo> arquivos = new List<Arquivo>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string file = GetFileName(path, formFile.FileName);
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }

                    arquivos.Add(new Arquivo
                    {
                        DataUpload = DateTime.Now,
                        DependencyId = dependencyId,
                        DependencyType = dependencyType,
                        Nome = formFile.FileName,
                        Tamanho = formFile.Length,
                        Path = Path.Combine(relativePath, Path.GetFileName(file)),
                        ContentType = formFile.ContentType
                    });
                }
            }
            _db.Arquivos.AddRange(arquivos);
            _db.SaveChanges();
        }
    }
}
