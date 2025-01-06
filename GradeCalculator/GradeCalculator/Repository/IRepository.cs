namespace GradeCalculator.Repository
{
    // Interface segregation principle
    // Open / Close principle
    public interface IRepository<T> : IReadAllRepository<T>
    {
        public T? Get(int id);
        public T? Add(T value);
        public T? Modify(int id, T value);
        public T? Remove(int id);
    }
}
