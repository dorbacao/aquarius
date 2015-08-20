using System.Collections.Generic;

namespace Vvs.Domain.Seedwork.UnitOfWork
{
    public interface ISql
    {
        int ExecuteSqlCommand(string sql, params object[] parameters);
        List<T> SqlQuery<T>(string sql, params object[] parameters);
    }
}
