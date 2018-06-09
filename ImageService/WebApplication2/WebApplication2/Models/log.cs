using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{

    public class Log
    {
        static int count = 0;
        public Log() { }

        public void copy(Log log)
        {
            Type = log.Type;
            Message = log.Message;
        }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }
}