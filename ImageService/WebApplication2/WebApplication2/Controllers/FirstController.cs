using WebApplication2.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class FirstController : Controller
    {
        static Communication comm = new Communication();
        static List<Image> images = new List<Image>();
        static List<string> relativeThumbs = new List<string>();

        // GET: First
        public ActionResult Index()
        {
            ViewBag.numOfImages = (Directory.GetFiles(@HostingEnvironment.MapPath("~/outputCheck"), "*.*", SearchOption.AllDirectories).Length)/2;
            // Read the file and display it line by line. 
            System.IO.StreamReader file = new System.IO.StreamReader(@HostingEnvironment.MapPath("~/students.txt"));
            ViewBag.student1 = file.ReadLine().Split(' ');
            ViewBag.student2 = file.ReadLine().Split(' ');
            file.Close();
            ViewBag.photoPath = @HostingEnvironment.MapPath("~/outputCheck/funny.jpg");

            if (comm.IsConnected)
                ViewBag.status = "Connected";
            else
                ViewBag.status = "Disconnected";

            return View();
        }

        //[HttpGet]
        public ActionResult Logs()
        {
            return View(comm);
        }
        
        // GET: First/Photos
        public ActionResult Photos()
        {
            //get paths of all thumnail pathes of output photos
            string root = @HostingEnvironment.MapPath("~/outputCheck");
            var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Where(d => d.Contains("Thumbnail"));
            foreach (string fullThumb in files)
            {
                var relativeThumb = fullThumb.Replace(root, "");
                if (!relativeThumbs.Contains(relativeThumb))
                {
                    relativeThumbs.Add(relativeThumb);
                    //getting regular photo full path
                    var fullPath = fullThumb.Replace("Thumbnail\\", "").Replace("thumb-", "");
                    //getting regular photo relative path
                    var relativePath = relativeThumb.Replace("Thumbnail\\", "").Replace("thumb-", "");
                    //get name and date
                    var name = Path.GetFileNameWithoutExtension(fullPath);
                    var date = Path.GetDirectoryName(relativePath).TrimStart('\\');
                    images.Add(new Image { Path = relativePath, ThumbPath=relativeThumb, Date = date, Name = name, FullPath=fullPath, FullThumbPath = fullThumb });
                }
            }
            return View(images);
        }

        // GET: First/Config
        public ActionResult Config()
        {
            //update the view mwmbers to contain the config details
            ViewBag.outputDir = comm.ServiceConfig.OutputDirectory;
            ViewBag.sourceName = comm.ServiceConfig.SourceName;
            ViewBag.logName = comm.ServiceConfig.LogName;
            ViewBag.size = comm.ServiceConfig.ThumbSize;

            return View(comm);
        }

        // GET: First/Edit/5
        //Display the given image
        public ActionResult showImage(string thumbpath)
        {
            foreach (Image img in images)
            {
                if(img.ThumbPath.Equals(thumbpath))
                {
                    return View(img);
                }
            }
            return View("Error");
        }

        // GET: First/Delete/5
        //Delete the given image
        public ActionResult DeletePhoto(string thumbpath)
        {
            int i = 0;
            foreach (Image img in images)
            {
                if (img.ThumbPath.Equals(thumbpath))
                {
                    if (!relativeThumbs.Contains(thumbpath))
                    {
                        return RedirectToAction("Error");
                    }
                    relativeThumbs.Remove(thumbpath);
                    images.RemoveAt(i);

                    System.IO.File.Delete(img.FullThumbPath);
                    System.IO.File.Delete(img.FullPath);

                    return RedirectToAction("Photos");
                }
                i++;
            }
            return RedirectToAction("Error");
        }

        // GET: First/Delete/5
        //Delete the given image
        public ActionResult Delete(string thumbpath)
        {
            foreach (Image img in images)
            {
                if (img.ThumbPath.Equals(thumbpath))
                {
                    return View(img);
                }
            }
            return View("Error");
        }

        //Remove handler command send to through the communication model
        [HttpGet]
        public ActionResult DeleteHandler(string handler)
        {
            comm.CloseHandler = handler;
            return View(comm);
        }

        public ActionResult CloseHandler()
        {
            comm.RemoveHandler(comm.CloseHandler);

            return RedirectToAction("Config");
        }

        //When cancel button in 'delete image' is clicked - redirect to photos.
        public ActionResult Cancel()
        {
            return RedirectToAction("photos");
        }
    }
}
