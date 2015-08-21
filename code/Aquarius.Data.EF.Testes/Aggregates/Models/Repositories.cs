using System;
using System.Linq.Expressions;

namespace Aquarius.Data.EF.Testes.Aggregates.Models
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
