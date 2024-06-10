using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryObject
    {
        //the fisrt two were only for flitering, done on video number 18
        public string? Symbol { get; set; } = null;
        public string? CompanyName { get; set; } = null;

        //the next two, these were added in video number 19 for sorting.

        public string? SortBy { get; set; } = null;
        public bool IsDecending { get; set; } = false;

        //the nex ---, these were/this was added in video number 20 for Pagination
        public int PageNUmber { get; set; } = 1;
        public int PageSize { get; set;} = 20;

    }
}