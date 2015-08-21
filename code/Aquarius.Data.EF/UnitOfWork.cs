
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System;
using System.Data.Common;
using System.Data.Entity;
using Aquarius.Seedwork.UnitOfWork;

namespace Aquarius.Data.EF {
    
    public class UnitOfWork : DbContext, IUnitOfWork {

        public UnitOfWork() : base() { }

        public UnitOfWork(string nameOrConnectionString)
            : this(nameOrConnectionString, new Aquarius.Seedwork.Aggregates.OnlyAggregateRootUpdateStrategy())
        {
        }

        public UnitOfWork(string nameOrConnectionString, Aquarius.Seedwork.Aggregates.IAggregateUpdateStrategy aggregateUpdateStrategy)
            : base(nameOrConnectionString)
        {
            if (aggregateUpdateStrategy == null) throw new ArgumentNullException("aggregateUpdateStrategy");
            this.AggregateUpdateStrategy = aggregateUpdateStrategy;
        }

        public UnitOfWork(DbConnection existingConnection, bool contextOwnsConnection)
            : this(existingConnection, contextOwnsConnection, new Aquarius.Seedwork.Aggregates.OnlyAggregateRootUpdateStrategy())
        {
        }

        public UnitOfWork(DbConnection existingConnection, bool contextOwnsConnection,
            Aquarius.Seedwork.Aggregates.IAggregateUpdateStrategy aggregateUpdateStrategy) : base(existingConnection, contextOwnsConnection)
        {
            if (aggregateUpdateStrategy == null) throw new ArgumentNullException("aggregateUpdateStrategy");
            this.AggregateUpdateStrategy = aggregateUpdateStrategy;
        }

        protected ObjectContext ObjectContext
        {
            get { return (this as IObjectContextAdapter).ObjectContext; }
        }

        #region ' IUnitOfWork '

        public new IQueryable<TEntidade> Set<TEntidade>() where TEntidade : class
        {
            return base.Set<TEntidade>();
        }

        public void RegisterClean<TEntidade>(TEntidade obj) where TEntidade : class
        {
            this.Entry(obj).State = EntityState.Unchanged;
        }

        public void RegisterNew<TEntidade>(TEntidade obj) where TEntidade : class
        {
            base.Set<TEntidade>().Add(obj);
        }

        public void RegisterDirty<TEntidade>(TEntidade obj) where TEntidade : class
        {
            // Faz o select do item pela sua chave primária.
            var objNoContexto = ObjectContext.GetObjectByKey(ObjectContext.CreateEntityKey(ObjectContext.CreateObjectSet<TEntidade>().EntitySet.Name, obj));

            if (ReferenceEquals(obj, objNoContexto)) { }
                // ... Não faz nada, pois o objeto no contexto já está alterado.
            else 
                // ...atualiza os valores escalares da instância no item informado.
                Entry((TEntidade)objNoContexto).CurrentValues.SetValues(obj);
        }

        public void RegisterDeleted<TEntidade>(TEntidade obj) where TEntidade : class
        {
            base.Set<TEntidade>().Remove(obj);
        }

        public virtual void Commit() {
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var erros = ex.EntityValidationErrors.SelectMany(entity => entity.ValidationErrors).Select(erro => erro.ErrorMessage).ToArray();
                throw new DbEntityValidationException(String.Join("\n", erros), ex.EntityValidationErrors, ex.InnerException);
            }
            catch (Exception ex) {
                throw;
            }
        }

        public void Rollback()
        {
            // code from http://code.msdn.microsoft.com/How-to-undo-the-changes-in-00aed3c4

            // Undo the changes of the all entries. 
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    // Under the covers, changing the state of an entity from  
                    // Modified to Unchanged first sets the values of all  
                    // properties to the original values that were read from  
                    // the database when it was queried, and then marks the  
                    // entity as Unchanged. This will also reject changes to  
                    // FK relationships since the original value of the FK  
                    // will be restored. 
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    // If the EntityState is the Deleted, reload the date from the database.   
                    case EntityState.Deleted:
                        entry.Reload();
                        break;

                }
            } 
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return base.Database.ExecuteSqlCommand(sql, parameters);
        }

        public List<T> SqlQuery<T>(string sql, params object[] parameters)
        {
            return base.Database.SqlQuery<T>(sql, parameters). ToList();
        }

        public virtual Aquarius.Seedwork.Aggregates.IAggregateUpdateStrategy AggregateUpdateStrategy { get; private set; }

        #endregion

        #region ' IQueryableUnitOfWork '

        public Aquarius.Seedwork.Repositorios.Queryable.IQueryBuilder<TEntidade> CreateQueryBuilder<TEntidade>() where TEntidade : class
        {
            return new Aquarius.Data.EF.Queryable.EntityQueryBuilder<TEntidade>();
        }

        #endregion

    }

}
