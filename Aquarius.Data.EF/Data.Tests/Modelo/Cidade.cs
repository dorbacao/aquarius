using System;

namespace Vvs.Data.Tests.Modelo
{
    public class Cidade : EntityBase
    {
        public String NomeCidade { get; set; }
        public virtual Estado Estado { get; set; }

    }
}
