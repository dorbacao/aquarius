using System.Data.Entity.ModelConfiguration;
using Vvs.Infraestrutura.Data.SqlClient.Tests.Modelo;

namespace Vvs.Infraestrutura.Data.SqlClient.Tests.UnitOfWork.Mapping
{
    public class EntidadeTesteConfiguration : EntityTypeConfiguration<EntidadeTeste>
    {
        public EntidadeTesteConfiguration()
        {
            // Primary key
            HasKey(p => p.Id);

            // Properties
            Property(p => p.Nome).HasColumnName("NomeEntidade");
            Property(p => p.TipoPessoa).HasColumnName("TipoDePessoa").IsRequired();
            Ignore(p => p.Idade);

            // Table
            this.ToTable("TabelaEntidadesTeste");
        }
    }
}
