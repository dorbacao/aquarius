using System.Data.Entity.ModelConfiguration;
using Northwind.Tests.Domain.Entity;

namespace Northwind.Tests.UnitOfWork.Mapping
{
    public class PessoaTypeConfiguration : EntityTypeConfiguration<Pessoa>
    {
        public PessoaTypeConfiguration()
        {
            // Primary Key
            this.HasKey(r => r.Id);

        }
    }
}
