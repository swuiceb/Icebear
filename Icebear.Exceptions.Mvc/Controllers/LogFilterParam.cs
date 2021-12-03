using yourLogs.Exceptions.Core.Models;

namespace yourLogs.Exceptions.Mvc.Controllers
{
    public class LogFilterParam
    {
        public FilterParam FilterParam { get; set; } 
        public SortByParam SortByParam { get; set; } 
    }
}