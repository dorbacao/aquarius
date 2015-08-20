using System.Data.Entity.ModelConfiguration;
using Northwind.Tests.Domain.Entity;

namespace Northwind.Tests.UnitOfWork.Mapping
{
    public class FornecedorTypeConfiguration : EntityTypeConfiguration<Fornecedor>
    {
        public FornecedorTypeConfiguration()
        {
            // Primary Key
            this.HasKey(r => r.Id);

        }
    }
}
