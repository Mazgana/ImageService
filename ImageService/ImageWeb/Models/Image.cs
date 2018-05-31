using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageWeb.Models
{
    public class Image
    {
        string Nmae { get; set; }
        DateTime date { get; set; }
        string path { get; set; }
        string thumbPath { get; set; }
    }
}