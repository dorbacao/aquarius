using System.Data.Entity;
using System.Linq;

namespace Aquarius.Data.EF.Queryable
{
    public class EntityQueryBuilder<TEntidade> : Aquarius.Seedwork.Repositorios.Queryable.DefaultQueryBuilder<TEntidade> where TEntidade : class
    {

        /// <summary>
        /// Método com a especificação de como o Entity Framework realiza o Join em queries (Include Properties)
        /// </summary>
        public override IQueryable<TEntidade> JoinQueryWith(IQueryable<TEntidade> query, string[] joinWith)
        {
            ValidaArrayDeNavigationProperties(joinWith);
            return joinWith.Aggregate(query, (x, prop) => x.Include(prop));
        }

    }
}
