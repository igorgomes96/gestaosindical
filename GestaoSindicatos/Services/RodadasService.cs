using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class RodadasService: CrudService<RodadaNegociacao>
    {
        private readonly Context _db;
        private readonly ArquivosService _arquivosService;

        public RodadasService(Context db, ArquivosService arquivosService) : base(db)
        {
            _db = db;
            _arquivosService = arquivosService;
        }

        public override RodadaNegociacao Add(RodadaNegociacao entity)
        {
            int qtdaRodadas = Count(r => r.NegociacaoId == entity.NegociacaoId);
            entity.Numero = qtdaRodadas + 1;
            entity = base.Add(entity);
            _db.Entry(entity).Reference(r => r.Negociacao).Load();
            entity.Negociacao.QtdaRodadas = _db.RodadasNegociacoes.Count(r => r.NegociacaoId == entity.NegociacaoId);
            _db.SaveChanges();
            return entity;
        }

        public override RodadaNegociacao Delete(params object[] key)
        {
            _arquivosService.DeleteFiles(DependencyFileType.RodadaNegociacao, (int)key[0]);
            RodadaNegociacao rodada = Find(key);
            _db.Entry(rodada).Reference(r => r.Negociacao).Load();
            rodada.Negociacao.QtdaRodadas--;

            _db.RodadasNegociacoes.Where(x => x.Numero > rodada.Numero)
                .ToList().ForEach(r => {
                    r.Numero--;
                    Update(r, r.Id);
                 });
            _db.SaveChanges();
            return base.Delete(key);
        }

        public override void Delete(Expression<Func<RodadaNegociacao, bool>> query)
        {
            Query(query).ToList().ForEach(r => Delete(r.Id));
        }
    }
}
