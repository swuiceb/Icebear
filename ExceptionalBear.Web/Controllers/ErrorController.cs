using System.Threading.Tasks;
using yourLogs.Exceptions.Core.LogReaders;
using yourLogs.Exceptions.Core.LogWriters;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionalBear.Web.Controllers
{
    [Route("Error")]
    public class ErrorController: Controller
    {
        private readonly ILogWriter writer ;
        
        public ErrorController(ILogWriter writer)
        {
            this.writer = writer;
        }
        
        public async Task<ViewResult> Index()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error; // Your exception

            var entry = await writer.LogErrorAsync(exception, "global");

            ViewBag.Error = entry;
            return View("ErrorDetail");
        }
    }
}