using GestaoSindicatos.Exceptions;
using GestaoSindicatos.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class ItensLitigiosService : CrudService<ItemLitigio>
    {
        private readonly Context _db;
        private readonly PlanosAcaoService _planosService;
        private readonly ArquivosService _arquivosService;
        public ItensLitigiosService(Context db, PlanosAcaoService planosService, ArquivosService arquivosService) : base(db)
        {
            _db = db;
            _planosService = planosService;
            _arquivosService = arquivosService;
        }

        public override ItemLitigio Add(ItemLitigio entity)
        {
            if (entity.PlanoAcao == null)
            {
                entity.PlanoAcao = new PlanoAcao
                {
                    DataPrevista = DateTime.Today,
                    Data = DateTime.Today,
                    Procedencia = false
                };
            }
            ItemLitigio item = base.Add(entity);
            item.PlanoAcaoId = item.PlanoAcao.Id;
            _db.SaveChanges();
            return item;
        }

        public override ItemLitigio Update(ItemLitigio entity, params object[] key)
        {
            PlanoAcao planoAcao = _db.PlanosAcao.Find(entity.PlanoAcaoId);
            if (planoAcao == null) throw new NotFoundException();
            _db.Entry(planoAcao).CurrentValues.SetValues(entity.PlanoAcao);

            ItemLitigio currentEntity = Find(key);
            if (currentEntity == null) throw new NotFoundException();
            _db.Entry(currentEntity).CurrentValues.SetValues(entity);
            _db.SaveChanges();
            return currentEntity;
        }

        public override ItemLitigio Delete(params object[] key)
        {
            ItemLitigio item = Find(key);
            if (item == null) throw new NotFoundException("Item de Litígio não encontrado!");
            _db.Entry(item).Reference(i => i.PlanoAcao).Load();

            if (item.PlanoAcao != null)
            {
                _planosService.Delete(item.PlanoAcao.Id);
            }
            _arquivosService.DeleteFiles(DependencyFileType.ItemLitigio, item.Id);
            _db.ItensLitigios.Remove(item);
            _db.SaveChanges();
            return item;
        }

        public override void Delete(Expression<Func<ItemLitigio, bool>> query)
        {
            Query(query).ToList().ForEach(p => Delete(p.Id));
        }
    }
}
