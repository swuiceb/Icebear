namespace yourLogs.Exceptions.Core.Models
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

        public static PageInfo Create(int page, int size)
        {
            return new PageInfo()
            {
                Page = page,
                PageSize = size
            };
        }
        
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}