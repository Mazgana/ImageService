using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Image
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Date")]
        public string Date { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Path")]
        public string Path { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "FullPath")]
        public string FullPath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ThumbPath")]
        public string ThumbPath { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "FullThumbPath")]
        public string FullThumbPath { get; set; }
    }
}