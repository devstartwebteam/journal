using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace journalentry.Models
{
    public class EntryRow
    {
        [Display(Name = "ROWID")]
        public int DT_RowId { get; set; }

        [Display(Name = "GL")]
        [Required]
        public string glNumber { get; set; }

        [Display(Name = "JOURNAL")]
        [Required]
        public double journalNumber { get; set; }

        [Display(Name = "SOURCE")]
        [Required]
        public string Source { get; set; }

        [Display(Name = "REFERENCE")]
        [Required]
        public string Reference { get; set; }

        [Display(Name = "MODEL")]
        [Required]
        public string Model { get; set; }

        [Display(Name = "DATE")]
        public string Date { get; set; }

        [Display(Name = "CONTROL")]
        public string Control { get; set; }

        [Display(Name = "ACCOUNT")]
        public double Account { get; set; }

        [Display(Name = "AMOUNT")]
        [Required]
        public double Amount { get; set; }

        [Display(Name = "TYPE")]
        public double Type { get; set; }
    }
}