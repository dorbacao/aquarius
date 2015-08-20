using Vvs.Domain.Seedwork;

namespace Northwind.Tests.Domain.Entity
{
    public class Territories : EntityBase
    {
        public string TerritoryDescription { get; set; }
        public virtual Region Region { get; set; }
    }
}
