using GradeCalculator.Models;
using System.Linq.Expressions;

namespace GradeCalculator.Repository
{
    public class RepositoryFactory
    {
        public static IRepository<T> CreateRepository<T>() where T : class
        {
            if (typeof(T) == typeof(Godina))
            {
                return new GodinaRepository(new PiGradeCalculatorContext()) as IRepository<T> 
                    ?? throw new NotImplementedException("No repository found for the given type.");
            }

            throw new NotImplementedException("No repository found for the given type.");
        }
    }

}
