using System.Collections.Generic;
using Vvs.Domain.Seedwork;

namespace Northwind.Tests.Domain.Entity
{
    public class Region : EntityBase
    {
        public string RegionDescription { get; set; }
        public virtual ICollection<Territories> Territories { get; set; }
    }
}
