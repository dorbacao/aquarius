using System;
using System.Collections.Generic;

namespace Aquarius.Data.Tests.Modelo
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
