using GestaoSindicatos.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class ArquivosRepository
    {
        private readonly IConfiguration _configuration;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly string _collectionName;
        private readonly string _databaseName;

        public ArquivosRepository(IConfiguration config)
        {
            _configuration = config;
            _client = new MongoClient(
                _configuration.GetConnectionString("MongoConnection"));
            _collectionName = "arquivos";
            _databaseName = "gestaosindicatos";
            _db = _client.GetDatabase(_databaseName);

        }

        public List<Arquivo> Query()
        {
            var filter = Builders<Arquivo>.Filter.Empty;
            var projection = Builders<Arquivo>.Projection.Exclude("content");
            return _db.GetCollection<Arquivo>(_collectionName).Find(filter)
                .Project<Arquivo>(projection).ToList();
        }

        public List<Arquivo> Query(Expression<Func<Arquivo, bool>> expression)
        {
            var filter = Builders<Arquivo>.Filter.Empty;
            var projection = Builders<Arquivo>.Projection.Exclude("content");
            return _db.GetCollection<Arquivo>(_collectionName).Find(expression)
                .Project<Arquivo>(projection).ToList();
        }


        public Arquivo Find(ObjectId id)
        {
            var filter = Builders<Arquivo>.Filter.Eq("_id", id);
            return _db.GetCollection<Arquivo>(_collectionName).Find(filter).FirstOrDefault();
        }

        public void Delete(ObjectId id)
        {
            _db.GetCollection<Arquivo>(_collectionName)
                .DeleteOne(x => x.Id == id);
        }

        public void DeleteMany(Expression<Func<Arquivo, bool>> expression)
        {
            _db.GetCollection<Arquivo>(_collectionName)
                .DeleteMany(expression);
        }


        public void InsertOne(Arquivo arquivo)
        {
            _db.GetCollection<Arquivo>(_collectionName).InsertOne(arquivo);
        }

        public void InsertMany(List<Arquivo> arquivos)
        {
            _db.GetCollection<Arquivo>(_collectionName).InsertMany(arquivos);
        }
    }
}
