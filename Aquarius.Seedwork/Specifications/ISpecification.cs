
namespace Aquarius.Seedwork.Specifications
{
    public interface ISpecification<T> where T : class
    {
        bool SatisfiedBy(T cadidate);
    }
}
