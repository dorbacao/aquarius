
namespace Aquarius.Seedwork.Repositorios.Queryable
{
    public interface IQueryableUnitOfWork
    {
        IQueryBuilder<TEntidade> CreateQueryBuilder<TEntidade>() where TEntidade : class;
    }
}
