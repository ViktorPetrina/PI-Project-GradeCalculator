using GradeCalculator.Models;
using GradeCalculator.ViewModels;

namespace GradeCalculator.Adapter
{
    public class LogAdapter : ILogAdapter
    {
        public static readonly LogAdapter Instance = new LogAdapter();
        public LogAdapter()
        {
            
        }
        public ShowLogVM Adapt(Log log)
        {
            return new ShowLogVM
            {
                Opis = log.Opis,
                Vrijeme = log.Vrijeme
            };
        }
    }
}
