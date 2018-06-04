using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageWeb.Models
{
    public class Image
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nmae")]
        string Nmae { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Date")]
        DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Path")]
        string Path { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ThumbPath")]
        string ThumbPath { get; set; }
    }
}