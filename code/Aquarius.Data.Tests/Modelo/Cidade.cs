using System;
using Aquarius.Seedwork;

namespace Aquarius.Data.Tests.Modelo
{
    public class Cidade : EntityBase
    {
        public String NomeCidade { get; set; }
        public virtual Estado Estado { get; set; }

    }
}
