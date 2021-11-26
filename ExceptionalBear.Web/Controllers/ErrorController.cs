using System.Threading.Tasks;
using Icebear.Exceptions.Core.LogReaders;
using Icebear.Exceptions.Core.LogWriters;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ExceptionalBear.Web.Controllers
{
    [Route("Error")]
    public class ErrorController: Controller
    {
        private readonly ILogReader reader;
        private readonly ILogWriter writer ;
        
        public ErrorController(ILogReader reader, ILogWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }
        
        public async Task<ViewResult> Index()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context.Error; // Your exception

            var entry = await writer.LogErrorAsync(exception);

            ViewBag.Error = entry;
            return View("ErrorDetail");
        }
    }
}