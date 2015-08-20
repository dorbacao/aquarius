using System;
using System.Linq.Expressions;
using Vvs.Domain.Seedwork.Aggregates;
using Vvs.Domain.Seedwork.Repositorios;
using Vvs.Domain.Seedwork.UnitOfWork;

namespace Vvs.Infraestrutura.Data.EF.Testes.Aggregates.Models
{
    public class CompanyRepository : Repository<Company>
    {
        public CompanyRepository(IUnitOfWork uow) : base(uow)
        {
        }

        public new void AlterarAgregacao(Company item, Expression<Func<IAggregateConfiguration<Company>, object>> aggregateConfiguration)
        {
            base.AlterarAgregacao(item, aggregateConfiguration);
        }
    

    }
}
