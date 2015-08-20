using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aquarius.Data.SqlClient.Tests.Modelo
{
    [Table("TabelaEntidadesTesteDataAnnotation")]
    public class EntidadeTesteDataAnnotation        
    {
        [Key]
        public int Id { get; set; }

        [Column("NomeEntidade")]
        public string Nome { get; set; }

        [Column(Order = 0)]
        public string SobreNome { get; set; }

        [NotMapped]
        public int? Idade { get; set; }

        [Column("TipoDePessoa")]
        [Required]
        public TipoPessoa? TipoPessoa { get; set; }
    }
}