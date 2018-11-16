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
        public PlanosAcaoService(Context db): base(db)
        {
            _db = db;
        }

        private Expression<Func<PlanoAcao, bool>> QueryUser(ClaimsPrincipal claims)
        {
            return (PlanoAcao p) => _db.EmpresasUsuarios
                .Where(u => u.UserName == claims.Identity.Name)
                .Include(u => u.Empresa)
                .Any(u => u.UserName == claims.Identity.Name && (p.LaboralId == u.Empresa.SindicatoLaboralId || p.PatronalId == u.Empresa.SindicatoPatronalId));
        }

        public IQueryable<PlanoAcao> Query(Expression<Func<PlanoAcao, bool>> query, ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query(query).Where(QueryUser(claims));

            return base.Query(query);
        }

        public IQueryable<PlanoAcao> Query(ClaimsPrincipal claims)
        {
            if (!claims.IsInRole(Roles.ADMIN))
                return base.Query().Where(QueryUser(claims));

            return base.Query();
        }

        public override PlanoAcao Find(params object[] key)
        {
            PlanoAcao plano = base.Find(key);
            _db.Entry(plano).Reference(p => p.Laboral).Load();
            _db.Entry(plano).Reference(p => p.Patronal).Load();
            return plano;
        }
    }
}
