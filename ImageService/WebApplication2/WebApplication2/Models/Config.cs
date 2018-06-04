using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Models
{
    public class Config
    {
        string outputDirectory { get; set; }
        string sourceName { get; set; }
        string logName { get; set; }
        string thumbSize { get; set; }
        List<string> handlers { get; set; }
    }
}