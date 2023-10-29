using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace u22555260_HW03.Models
{
    public class BookCategoryViewModel
    {
        public string Categories { get; set; }
        public int NumberOfBooks { get; set; }
        public IEnumerable<DownloadedFiles> downloadedFiles { get; set; }
    }
}