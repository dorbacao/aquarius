using System.Linq;
using Aquarius.Seedwork.Criterias;

namespace Aquarius.Data.Tests.Criterios.Estado
{
    public class DoPais : ICriteria<Modelo.Estado>
    {
        protected string NomePais;

        public DoPais(string nomePais) { this.NomePais = nomePais; }

        public IQueryable<Modelo.Estado> MeetCriteria(IQueryable<Modelo.Estado> estados)
        {
            return from estado in estados
                   where estado.Pais != null && estado.Pais.NomePais == this.NomePais
                   select estado;
        }
    }
}
