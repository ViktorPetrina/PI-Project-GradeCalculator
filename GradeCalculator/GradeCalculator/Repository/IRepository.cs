namespace GradeCalculator.Repository
{
    public interface IRepository<T>
    {
        public IEnumerable<T> GetAll();
        public T? Get(int id);
        public T? Add(T value);
        public T? Modify(int id, T value);
        public T? Remove(int id);
    }
}
