using GestaoSindicatos.Auth;
using GestaoSindicatos.Exceptions;
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
    public class LitigiosService : CrudService<Litigio>
    {
        private readonly Context _db;
        private readonly ArquivosService _arquivosService;
        private readonly ItensLitigiosService _itensLitigiosService;

        public LitigiosService(Context db, ArquivosService arquivosService, ItensLitigiosService itensLitigiosService) : base(db)
        {
            _db = db;
            _arquivosService = arquivosService;
            _itensLitigiosService = itensLitigiosService;
        }

        private Expression<Func<Litigio, bool>> QueryUser(ClaimsPrincipal claims)
        {
            return (Litigio n) => _db.EmpresasUsuarios.Any(u => u.UserName == claims.Identity.Name && u.EmpresaId == n.EmpresaId);
        }

        public IQueryable<Litigio> Query(Expression<Func<Litigio, bool>>  query, ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query(query).Where(QueryUser(claims));

            return base.Query(query);
        }

        public IQueryable<Litigio> Query(ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query().Where(QueryUser(claims));

            return base.Query();
        }


        public override Litigio Find(params object[] key)
        {
            Litigio litigio = _db.Litigios
                .Where(l => l.Id == int.Parse(key[0].ToString()))
                .Include(l => l.Empresa)
                .Include(l => l.Laboral)
                .Include(l => l.Laboral)
                .Include(l => l.Itens)
                    .ThenInclude(l => l.PlanoAcao)
                .FirstOrDefault();
            return litigio;
        }

        public override Litigio Delete(params object[] key)
        {
            _arquivosService.DeleteFiles(DependencyFileType.Litigio, (int)key[0]);
            _itensLitigiosService.Delete(i => i.Litigio.Id == (int)key[0]);
            return base.Delete(key);
        }

        public override void Delete(Expression<Func<Litigio, bool>> query)
        {
            Query(query).ToList().ForEach(l => Delete(l.Id));
        }

        public Litigio UpdateStatus(Litigio litigio)
        {
            if (litigio?.Itens == null || litigio?.Itens.Count == 0)
                return litigio;

            var query = litigio.Itens.Where(i => i.PossuiPlano && i.PlanoAcao != null);
            if (query.Count() == 0)
            {
                litigio.StatusPlanos = null;
            }
            else if (query.Any(i => i.PlanoAcao.Status == StatusPlanoAcao.Vencido))
            {
                litigio.StatusPlanos = StatusPlanoAcao.Vencido;
            } else if (query.Any(i => i.PlanoAcao.Status == StatusPlanoAcao.AVencer))
            {
                litigio.StatusPlanos = StatusPlanoAcao.AVencer;
            } else if (query.Any(i => i.PlanoAcao.Status == StatusPlanoAcao.NoPrazo))
            {
                litigio.StatusPlanos = StatusPlanoAcao.NoPrazo;
            } else if (query.All(i => i.PlanoAcao.Status == StatusPlanoAcao.Solucionado))
            {
                litigio.StatusPlanos = StatusPlanoAcao.Solucionado;
            } 

            return litigio;

        }

    }
}
