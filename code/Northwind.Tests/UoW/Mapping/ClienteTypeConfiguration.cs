using System.Data.Entity.ModelConfiguration;
using Northwind.Tests.Domain.Entity;

namespace Northwind.Tests.UoW.Mapping
{
    public class ClienteTypeConfiguration : EntityTypeConfiguration<Cliente>
    {
        public ClienteTypeConfiguration()
        {
            // Primary Key
            this.HasKey(r => r.Id);

        }
    }
}
