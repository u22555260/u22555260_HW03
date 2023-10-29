using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace u22555260_HW03.Models
{
    public class CombindedTableViews
    {
        public IEnumerable<authors> Authors { get; set; }
        public IEnumerable<books> Books { get; set; }
        public IEnumerable<borrows> Borrows { get; set; }
        public IEnumerable<students> Students { get; set; }
        public IEnumerable<types> Types { get; set; }
        public IEnumerable<DownloadedFiles> downloadedFiles { get; set; }   
    }
}