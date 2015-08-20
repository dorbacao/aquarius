using System.Data.Entity.ModelConfiguration;
using Northwind.Tests.Domain.Entity;

namespace Northwind.Tests.UnitOfWork.Mapping
{
    public class TerritoriesTypeConfiguration : EntityTypeConfiguration<Territories>
    {
        public TerritoriesTypeConfiguration()
        {            
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.TerritoryDescription).IsRequired();

            // Table & Column Mappings
            this.ToTable("Territories");
            this.Property(r => r.Id).HasColumnName("TerritoriesID");
            this.Property(t => t.TerritoryDescription).HasColumnName("TerritoryDescription");
        }
    }
}
