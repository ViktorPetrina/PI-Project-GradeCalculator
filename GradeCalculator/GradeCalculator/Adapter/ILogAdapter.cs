using GradeCalculator.Models;
using GradeCalculator.ViewModels;

namespace GradeCalculator.Adapter
{
    public interface ILogAdapter
    {
        ShowLogVM Adapt(Log log);
    }
}
