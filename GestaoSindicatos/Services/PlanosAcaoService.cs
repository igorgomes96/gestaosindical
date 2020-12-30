using GestaoSindicatos.Auth;
using GestaoSindicatos.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestaoSindicatos.Services
{
    public class PlanosAcaoService : CrudService<PlanoAcao>
    {
        private readonly Context _db;
        private readonly ArquivosService _arquivosService;
        public PlanosAcaoService(Context db, ArquivosService arquivosService): base(db)
        {
            _db = db;
            _arquivosService = arquivosService;
        }


        public override PlanoAcao Delete(params object[] key)
        {
            _arquivosService.DeleteFiles(DependencyFileType.PlanoAcao, (int)key[0]);
            return base.Delete(key);
        }

        public override void Delete(Expression<Func<PlanoAcao, bool>> query)
        {
            Query(query).ToList().ForEach(p => Delete(p.Id));
        }
    }
}
