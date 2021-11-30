using Microsoft.AspNetCore.Mvc;

namespace Icebear.Exceptions.Middleware.Endpoints
{
    [Controller]
    public class LogEndpointProcessor : ControllerBase
    {
       public async Task<IActionResult> GetAll(
               String searchInput, 
               String[]logTypes,
               String[]tags,
               DateTimeOffset? from,
               DateTimeOffset? to)
           {
               if (Guid.TryParse(searchInput, out Guid id))
               {
                   var logEntry = await logReader.GetByIdAsync(id);
                   if (logEntry != null)
                   {
                       ViewBag.Specific = logEntry;
                   }
                   else
                   {
                       ViewBag.Error = "Entry Code does not exist";
                   }
               }
               else if (searchInput != null)
               {
                   ViewBag.Error = "Entry Code does not exist";
               }
          
               IEnumerable<LogType> typeFilters = new[] {LogType.Warning, LogType.Error, LogType.Custom};
               IEnumerable<String> allTags =  await logReader.GetTagsAsync();
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
               return View("Index");
           } 
    }
}