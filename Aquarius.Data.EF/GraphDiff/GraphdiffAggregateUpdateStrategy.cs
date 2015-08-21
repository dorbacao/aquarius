using System;
using System.Data.Entity;
using System.Linq.Expressions;


namespace Aquarius.Data.EF.GraphDiff
{
    
    //public class GraphdiffAggregateUpdateStrategy : OnlyAggregateRootUpdateStrategy
    //{
    //    #region ' IAggregateUpdateStrategy '

    //    public override void AlterarAgregacao<TAggRoot>(Aquarius.Seedwork.UnitOfWork.IUnitOfWork unitOfWork, TAggRoot aggRoot, Expression<Func<Aquarius.Seedwork.Aggregates.IAggregateConfiguration<TAggRoot>, object>> aggregateConfiguration)
    //    {
    //        // Recupera DbContext
    //        var dbContext = unitOfWork as DbContext;
    //        if (dbContext == null) throw new ArgumentException("Unit of work precisa ser um EntityFramework DbContext.");


    //        // se não foi informada a configuração da agregação, atualiza apenas o aggregate root. 
    //        if (aggregateConfiguration == null)
    //        {
    //            base.AlterarAgregacao(unitOfWork, aggRoot, null);
    //            return;
    //        }

    //        // Converte expressão
    //        var configExp = new UpdateGraphConfigurationBuilder<TAggRoot>().ConvertFrom(aggregateConfiguration);
    //        dbContext.UpdateGraph(aggRoot, configExp);
    //    }

    //    #endregion
        
    //}
}
