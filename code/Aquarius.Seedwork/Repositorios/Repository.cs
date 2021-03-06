﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Aquarius.Seedwork.Aggregates;
using Aquarius.Seedwork.Criterias;
using Aquarius.Seedwork.Repositorios.Queryable;
using Aquarius.Seedwork.UnitOfWork;

namespace Aquarius.Seedwork.Repositorios
{
    public class Repository<TEntidade> 
        : QueryableRepository<TEntidade>
        , IRepository<TEntidade> where TEntidade : class
    {

        #region ' Construtor '

        public Repository(IUnitOfWork uow) : base(uow) { }

        #endregion

        #region ' IRepository<TEntidade> '

        public virtual void Incluir(TEntidade item)
        {            
            this.UnitOfWork.RegisterNew(item);
        }

        public virtual void Alterar(TEntidade item)
        {
            AlterarAgregacao(item, null);
        }

        public virtual void Excluir(TEntidade item)
        {
            this.UnitOfWork.RegisterDeleted(item);
        }

        #endregion

        #region ' IReadonlyRepository '

        public IQueryable<TOutput> Listar<TOutput>(ICriteria<TEntidade, TOutput> criterio)
        {
            return criterio.MeetCriteria(BaseQuery);
        }
               
        public IQueryable<TEntidade> Listar()
        {
            return this.BaseQuery;
        }

        public IQueryable<TReturn> Listar<TOutput, TReturn>(ICriteria<TEntidade, TOutput> criterio, Expression<Func<TOutput, TReturn>> output)
        {
            return criterio.MeetCriteria(BaseQuery).Select(output);
        }

        public TOutput Selecionar<TOutput>(ICriteria<TEntidade, TOutput> criterio)
        {
            return criterio.MeetCriteria(BaseQuery).SingleOrDefault();
        }

        public TReturn Selecionar<TOutput, TReturn>(ICriteria<TEntidade, TOutput> criterio, Expression<Func<TOutput, TReturn>> output)
        {
            return criterio.MeetCriteria(BaseQuery).Select(output).SingleOrDefault();
        }

        #endregion

        protected void AlterarAgregacao(TEntidade item, Expression<Func<IAggregateConfiguration<TEntidade>, object>> aggregateConfiguration)
        {
            UnitOfWork.AggregateUpdateStrategy.AlterarAgregacao(UnitOfWork, item, aggregateConfiguration);
        }

    }
}
