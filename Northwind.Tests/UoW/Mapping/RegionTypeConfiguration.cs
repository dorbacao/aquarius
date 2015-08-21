using System.Data.Entity.ModelConfiguration;
using Northwind.Tests.Domain.Entity;

namespace Northwind.Tests.UoW.Mapping
{
    public class RegionTypeConfiguration : EntityTypeConfiguration<Region>
    {
        public RegionTypeConfiguration()
        {
            // Primary Key
            this.HasKey(r => r.Id);

            // Properties
            this.Property(r => r.Id).IsRequired();
            this.Property(r => r.RegionDescription).IsRequired();

            // Table & Column Mappings
            this.ToTable("Region");
            this.Property(r => r.Id).HasColumnName("RegionID");
            this.Property(r => r.RegionDescription).HasColumnName("RegionDescription");

            // Relationships
            this.HasMany(t => t.Territories)
                .WithRequired(r => r.Region)
                .WillCascadeOnDelete(true);
        }
    }
}
