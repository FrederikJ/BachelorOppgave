using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/**
 * Model klasse for filer
*/

namespace HovedOppgave.Models
{
    public class Files
    {
        public int FileID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public DateTime Date { get; set; }
        public bool Kassert { get; set; }
    }
}