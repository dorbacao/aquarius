using System.Collections.Generic;

namespace Aquarius.Seedwork.UnitOfWork
{
    public interface ISql
    {
        int ExecuteSqlCommand(string sql, params object[] parameters);
        List<T> SqlQuery<T>(string sql, params object[] parameters);
    }
}
