using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace journalentry.Models
{
    public class EntryError
    {
        public string errorMessage { get; set; }
        public int errorRow { get; set; }
        public int errorCol { get; set; }
    }
}