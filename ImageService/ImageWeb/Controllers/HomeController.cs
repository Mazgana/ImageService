using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImageWeb.Models;
using System.IO;
using System.Web.Hosting;

namespace ImageWeb.Controllers
{
    public class HomeController : Controller
    {
        static List<Image> images = new List<Image>();

        public ActionResult Index()
        {
            ViewBag.numOfImages = Directory.GetFiles(@HostingEnvironment.MapPath("~/outputCheck"), "*.*", SearchOption.AllDirectories).Length;
            // Read the file and display it line by line. 
            System.IO.StreamReader file = new System.IO.StreamReader(@HostingEnvironment.MapPath("~/students.txt"));
            ViewBag.Student1 = file.ReadLine();;
            ViewBag.Student2 = file.ReadLine();
            file.Close();
            return View();
        }

        public ActionResult Config()
        {
            ViewBag.Message = "Your config page.";

            return View();
        }

        public ActionResult Photos()
        {
            ViewBag.Message = "Your photos page.";

            return View();
        }

        public ActionResult Logs()
        {
            ViewBag.Message = "Your logs page.";

            return View();
        }
    }
}