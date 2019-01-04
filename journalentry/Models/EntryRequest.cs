using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace journalentry.Models
{
    public class EntryRequest
    {
        public string requestCode { get; set; }
        public int company { get; set; }
        public DateTime accountingDate { get; set; }
        public string accountingMonth { get; set; }
        public string userId { get; set; }
        public string ipAddress { get; set; }
        public string reversalIndicator { get; set; }
        public List<EntryRow> records { get; set; }
    }
}