using System;
using System.Linq;
using Vvs.Domain.Seedwork.Aggregates;
using Vvs.Domain.Seedwork.Repositorios.Queryable;

namespace Vvs.Domain.Seedwork.UnitOfWork
{
    public interface IUnitOfWork : IQueryableUnitOfWork, ISql, IDisposable
    {
        void RegisterNew<TEntidade>(TEntidade obj) where TEntidade : class;
        void RegisterDirty<TEntidade>(TEntidade obj) where TEntidade : class;
        void RegisterClean<TEntidade>(TEntidade obj) where TEntidade : class;
        void RegisterDeleted<TEntidade>(TEntidade obj) where TEntidade : class;

        void Commit();
        void Rollback();

        IQueryable<TEntidade> Set<TEntidade>() where TEntidade : class;

        /// <summary>
        ///     Retorna a estratégia que será usada por um repositório para atualizar uma agregação em um Unit Of Work.
        /// </summary>
        IAggregateUpdateStrategy AggregateUpdateStrategy { get; }

    }
}
