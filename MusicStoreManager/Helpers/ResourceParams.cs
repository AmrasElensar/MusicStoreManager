using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Helpers
{
    public class ResourceParams
    {
        const int maxPageSize = 96;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 12;

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }

        public string Genre { get; set; }

        public string SearchQuery { get; set; }

        public string OrderBy { get; set; } = "Artist";

        public string Fields { get; set; }
    }
}
