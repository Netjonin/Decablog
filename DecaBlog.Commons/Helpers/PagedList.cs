using DecaBlog.Models;
using DecaBlog.Models.DTO;
using System.Collections.Generic;
using System.Linq;

namespace DecaBlog.Helpers
{
    public static class PagedList<T>
    {
        public static PageMeta CreatePageMetaData(int page, int perPage, int total)
        {
            var total_pages = total % perPage == 0 ? total / perPage : total / perPage + 1;
            return new PageMeta
            {
                Page = page,
                PerPage = perPage,
                Total = total,
                TotalPages = total_pages
            };
        }

        public static PaginatedListDto<T> Paginate(IEnumerable<T> source, int page, int perPage)
        {
            page = page < 1 ? 1 : page;
            var paginatedList = source.Skip((page - 1) * perPage).Take(perPage).ToList();
            var pageMeta = CreatePageMetaData(page, perPage, source.ToList().Count);
            return new PaginatedListDto<T>
            {
                MetaData = pageMeta,
                Data = paginatedList
            };
        }
    }
}