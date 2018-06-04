using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ImageWeb.Models
{
    public class Image
    {
        public string name { get; set; }
        public DateTime date { get; set; }
        public string path { get; set; }
        public string thumbPath { get; set; }
    }
}