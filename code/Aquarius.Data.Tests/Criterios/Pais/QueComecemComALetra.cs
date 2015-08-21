using Aquarius.Seedwork.Criterias;

namespace Aquarius.Data.Tests.Criterios.Pais
{
    public class QueComecemComALetra : Criteria<Modelo.Pais>
    {
        public QueComecemComALetra(char letra) : base(pais => pais.NomePais[0] == letra) { }
    }
}
