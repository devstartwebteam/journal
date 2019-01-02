using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using journalentry.Models;

namespace journalentry.Models
{
    public class JournalEntryViewModel
    {
        public HttpPostedFileBase excelFile { get; set; }
    }
}