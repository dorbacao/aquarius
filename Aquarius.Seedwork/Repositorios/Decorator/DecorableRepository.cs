using System;
using System.Linq;
using System.Linq.Expressions;
using Aquarius.Seedwork.Criterias;
using Aquarius.Seedwork.UnitOfWork;

namespace Aquarius.Seedwork.Repositorios.Decorator
{

    /// <summary>
    /// Repositorio readonly com suporte a decoração.
    /// </summary>
    public class DecorableRepository<TEntidade> : IRepository<TEntidade> where TEntidade : class
    {
        private readonly IRepository<TEntidade> _decoratedRepo;

        #region contrutor

        public DecorableRepository(IUnitOfWork unitOfWork)
        {
            this._decoratedRepo = new Repository<TEntidade>(unitOfWork);
        }
        public DecorableRepository(IRepository<TEntidade> decoratedRepo)
        {
            this._decoratedRepo = decoratedRepo;
        }

        #endregion

        #region ' IRepository<> '

        public virtual IUnitOfWork UnitOfWork { get { return _decoratedRepo.UnitOfWork; } }

        public virtual IQueryable<TEntidade> BaseQuery { get { return _decoratedRepo.BaseQuery; } }

        public virtual IQueryable<TEntidade> Listar(Expression<Func<TEntidade, bool>> @where = null, Func<IQueryable<TEntidade>, IOrderedQueryable<TEntidade>> orderBy = null, string[] joinWith = null)
        {
            return _decoratedRepo.Listar(where, orderBy, joinWith);
        }

        public virtual IQueryable<TOutput> Listar<TOutput>(Expression<Func<TEntidade, TOutput>> output, Expression<Func<TEntidade, bool>> @where = null, Func<IQueryable<TEntidade>, IOrderedQueryable<TEntidade>> orderBy = null)
        {
            return _decoratedRepo.Listar<TOutput>(output, where, orderBy);
        }

        public virtual TEntidade Selecionar(Expression<Func<TEntidade, bool>> @where, string[] joinWith = null)
        {
            return _decoratedRepo.Selecionar(where, joinWith);
        }

        public virtual TOutput Selecionar<TOutput>(Expression<Func<TEntidade, bool>> @where, Expression<Func<TEntidade, TOutput>> output)
        {
            return _decoratedRepo.Selecionar(where, output);
        }

        public virtual IQueryable<TOutput> Listar<TOutput>(ICriteria<TEntidade, TOutput> criterio)
        {
            return _decoratedRepo.Listar(criterio);
        }

        public virtual IQueryable<TReturn> Listar<TOutput, TReturn>(ICriteria<TEntidade, TOutput> criterio, Expression<Func<TOutput, TReturn>> output)
        {
            return _decoratedRepo.Listar(criterio, output);
        }

        public IQueryable<TEntidade> Listar()
        {
            return _decoratedRepo.Listar();
        }

        public virtual TOutput Selecionar<TOutput>(ICriteria<TEntidade, TOutput> criterio)
        {
            return _decoratedRepo.Selecionar(criterio);
        }

        public virtual TReturn Selecionar<TOutput, TReturn>(ICriteria<TEntidade, TOutput> criterio, Expression<Func<TOutput, TReturn>> output)
        {
            return _decoratedRepo.Selecionar(criterio, output);
        }

        public virtual void Incluir(TEntidade item)
        {
            _decoratedRepo.Incluir(item);
        }

        public virtual void Alterar(TEntidade item)
        {
            _decoratedRepo.Alterar(item);
        }

        public virtual void Excluir(TEntidade item)
        {
            _decoratedRepo.Excluir(item);
        }

        #endregion

    }

}
