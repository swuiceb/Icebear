namespace Icebear.Exceptions.Core.Models
{
    public class PageInfo
    {
        public static PageInfo Default()
        {
            return new PageInfo()
            {
                Page = 0,
                PageSize = 100
            };
        }
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}