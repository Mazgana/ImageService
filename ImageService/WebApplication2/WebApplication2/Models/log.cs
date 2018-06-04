using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Log
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Type")]
        string Type { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Message")]
        string Message { get; set; }
    }
}