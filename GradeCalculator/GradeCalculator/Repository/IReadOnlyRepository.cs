namespace GradeCalculator.Repository
{
    public interface IReadAllRepository<T>
    {
        public IEnumerable<T> GetAll();
    }
}
