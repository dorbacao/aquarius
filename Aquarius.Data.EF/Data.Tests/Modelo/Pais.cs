using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vvs.Data.Tests.Modelo
{
    public class Pais
    {
        public int Id { get; set; }
        public String NomePais { get; set; }
        public List<Estado> Estados { get; set; }

        public Pais()
        {
            this.Estados = new List<Estado>();
        }
    }
}
