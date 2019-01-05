using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DataTables;

namespace journalentry.Models
{
    public class EntryRow
    {
        [Display(Name = "ROWID")]
        [EditorTypeError("DT_RowId must be an integer")]
        public int DT_RowId { get; set; }

        [Display(Name = "GL")]
        [Required]
        [EditorTypeError("Gl Number must be a string")]
        public string glNumber { get; set; }

        [Display(Name = "JOURNAL")]
        [Required]
        [EditorTypeError("Journal Number must be a double")]
        public double journalNumber { get; set; }

        [Display(Name = "SOURCE")]
        [Required]
        [EditorTypeError("Source must be a string")]
        public string Source { get; set; }

        [Display(Name = "REFERENCE")]
        [Required]
        [EditorTypeError("Reference must be a string")]
        public string Reference { get; set; }

        [Display(Name = "MODEL")]
        [Required]
        [EditorTypeError("Model must be a string")]
        public string Model { get; set; }

        [Display(Name = "DATE")]
        [EditorTypeError("Date must be in date format YYYY/MM/DD")]
        public string Date { get; set; }

        [Display(Name = "CONTROL")]
        [EditorTypeError("Control must be a string")]
        public string Control { get; set; }

        [Display(Name = "ACCOUNT")]
        [EditorTypeError("Account must be a double")]
        public double Account { get; set; }

        [Display(Name = "AMOUNT")]
        [Required]
        [EditorTypeError("Amout must be a double")]
        public double Amount { get; set; }

        [Display(Name = "TYPE")]
        [EditorTypeError("Type must be a double")]
        public double Type { get; set; }
    }
}