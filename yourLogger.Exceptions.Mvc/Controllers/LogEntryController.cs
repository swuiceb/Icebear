using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yourLogs.Exceptions.Core.LogReaders;
using yourLogs.Exceptions.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace yourLogs.Exceptions.Mvc.Controllers
{
    [Controller, Route("iblogs")]
    public class LogEntryController : Controller
    {
        private readonly ILogReader logReader;

        public LogEntryController(
            ILogReader logReader)
        {
            this.logReader = logReader;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Get(
            PageInfo pageParam,
            FilterParam filterParam,
            SortByParam sortByParam)
        {
            var logs = await logReader.GetAll(pageParam, filterParam, sortByParam);
            return new OkObjectResult(logs); 
        }
        
        [HttpGet("")]
        public async Task<IActionResult> GetAll(
            String searchInput,
            String[] logTypes,
            String[] tags,
            DateTimeOffset? from,
            DateTimeOffset? to)
        {
            if (Guid.TryParse(searchInput, out Guid id))
            {
                var logEntry = await logReader.GetByIdAsync(id);
                if (logEntry != null)
                {
                    var relevant = await logReader.GetAll(
                        new PageInfo
                        {
                            Page = 0,
                            PageSize = 20
                        },
                        new FilterParam()
                        {
                            Until = logEntry.OccurredDate.AddSeconds(3)
                        });

                    ViewBag.Relevant = relevant.Items;
                    ViewBag.Specific = logEntry;
                    return View("IcebearDetail");
                }
                else if (!String.IsNullOrEmpty(searchInput))
                {
                    ViewBag.Error = "Entry Code does not exist";
                }
            }
            else if (!String.IsNullOrEmpty(searchInput))
            {
                ViewBag.Error = "Entry Code does not exist";
            }

            IEnumerable<LogType> typeFilters = new[] {LogType.Warning, LogType.Error, LogType.Custom};
            IEnumerable<String> allTags = await logReader.GetTagsAsync();
            IEnumerable<String> selectedTags =
                tags is {Length: > 0} ? tags : allTags.ToList();

            if (logTypes is {Length: > 0})
            {
                typeFilters = logTypes.Select(f => Enum.TryParse<LogType>(
                    f,
                    out LogType logType)
                    ? logType
                    : LogType.Error);
            }

            var results = await logReader.GetAll(new PageInfo()
            {
                Page = 0,
                PageSize = 10
            }, new FilterParam()
            {
                Since = from,
                Until = to,
                Tags = selectedTags,
                LogTypes = typeFilters.ToArray()
            });

            var viewData = results.Items;

            ViewBag.SelectedFilterTypes = typeFilters;

            ViewBag.Tags = allTags;
            ViewBag.SelectedTags = selectedTags;
            ViewBag.LastN = viewData;
            return View("Icebear");
        }
    }
}