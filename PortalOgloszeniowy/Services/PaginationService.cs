using Microsoft.EntityFrameworkCore;

namespace PortalOgloszeniowy.Models
{
    public class PaginationService<T> : List<T>
    {
        public int PageIndex { get; private set; }

        public int TotalPages{ get; set;}

        public PaginationService(IEnumerable<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count/ (double)pageSize);
            AddRange(items);
        }
        public bool PreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool NextPage
        {
            get
            {
                return(PageIndex < TotalPages);
            }
        }
        
        public static PaginationService<T> CreateAsync(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            
            var count = source.Count();
            var items = source.Skip((pageIndex-1)* pageSize).Take(pageSize).ToList();
            return new PaginationService<T>(items,count,pageIndex,pageSize);
        }
    }
}
