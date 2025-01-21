using GradeCalculator.Models;

namespace GradeCalculator.Service
{
    public interface ILogService
    {
         void AddLog(string opis);
        List<Log> GetLogsAsXML();
    }
}
