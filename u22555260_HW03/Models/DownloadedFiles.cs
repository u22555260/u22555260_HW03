//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace u22555260_HW03.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DownloadedFiles
    {
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public int UserID { get; set; }
        public System.DateTime DateDownloaded { get; set; }
        public Nullable<int> studentID { get; set; }
        public string FilePath { get; set; }
    
        public virtual students students { get; set; }
    }
}
