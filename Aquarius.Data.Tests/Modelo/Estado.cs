using System.Collections.Generic;

namespace Aquarius.Data.Tests.Modelo
{
    public class Estado
    {
        public int Id { get; set; }
        public string NomeEstado { get; set; }
        public string UF { get; set; }
        public Pais Pais { get; set; }
        public List<Cidade> Cidades { get; set; }

        public Estado()
        {
            this.Cidades = new List<Cidade>();
        }
    }
}
