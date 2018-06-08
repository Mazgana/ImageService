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
        static List<string> fileNames = new List<string>();
        static List<Employee> employees = new List<Employee>()
        {
          new Employee  { FirstName = "Moshe", LastName = "Aron", Email = "Stam@stam", Salary = 10000, Phone = "08-8888888" },
          new Employee  { FirstName = "Dor", LastName = "Nisim", Email = "Stam@stam", Salary = 2000, Phone = "08-8888888" },
          new Employee   { FirstName = "Mor", LastName = "Sinai", Email = "Stam@stam", Salary = 500, Phone = "08-8888888" },
          new Employee   { FirstName = "Dor", LastName = "Nisim", Email = "Stam@stam", Salary = 20, Phone = "08-8888888" },
          new Employee   { FirstName = "Dor", LastName = "Nisim", Email = "Stam@stam", Salary = 700, Phone = "08-8888888" }
        };

        //public FirstController()
        //{
        //    this.client = TcpClientChannel.getInstance();
        //}

        // GET: First
        public ActionResult Index()
        {
            ViewBag.numOfImages = Directory.GetFiles(@HostingEnvironment.MapPath("~/outputCheck"), "*.*", SearchOption.AllDirectories).Length;
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

        [HttpGet]
        public ActionResult Logs()
        {
            return View(comm.LogList);
        }

        //[HttpGet]
        //public JObject GetLog()
        //{
        //    JObject data = new JObject();
        //    data["FirstName"] = "Kuky";
        //    data["LastName"] = "Mopy";
        //    return data;
        //}

        [HttpPost]
        public JObject GetLog(string type)
        {
            foreach (var log in comm.LogList)
            {
                if (log.Type.Equals(type))
                {
                    JObject data = new JObject();
                    data["Type"] = log.Type;
                    data["Message"] = log.Message;
                    return data;
                }
            }
            return null;
        }

        // GET: First/Photos
        public ActionResult Photos()
        {
            //var files = Directory.GetFiles(@HostingEnvironment.MapPath("~/outputCheck"), "*.*", SearchOption.AllDirectories);
            string root = @HostingEnvironment.MapPath("~/outputCheck");
            var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories);
          /*  foreach(string full in fulls)
            {
                full.Replace(root, "");
            }
            var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Select(path => path.Replace(root, ""));
            */
            foreach (string filename in files)
            {
                var path = filename.Replace(root, "");
                if (!fileNames.Contains(path))
                {
                    fileNames.Add(path);
                    var name = Path.GetFileNameWithoutExtension(filename);
                    var date = Path.GetDirectoryName(path);
                    date = date.TrimStart('\\');
                    images.Add(new Image { Path = path, Date = date, Name = name, fullPath=filename });
                }
            }
            return View(images);
        }

        // GET: First/Config
        public ActionResult Config()
        {
            ViewBag.outputDir = comm.ServiceConfig.OutputDirectory;
            ViewBag.sourceName = comm.ServiceConfig.SourceName;
            ViewBag.logName = comm.ServiceConfig.LogName;
            ViewBag.size = comm.ServiceConfig.ThumbSize;

            return View();
        }

        // POST: First/Config
        [HttpPost]
        public ActionResult Config(Employee emp)
        {
            try
            {
                employees.Add(emp);

                return RedirectToAction("Photos");
            }
            catch
            {
                return View();
            }
        }
        // GET: First/Edit/5
        public ActionResult showImage(string path)
        {
            foreach (Image img in images)
            {
                if(img.Path.Equals(path))
                {
                    return View(img);
                }
            }
            return View("Error");
        }

        // GET: First/Edit/5
        public ActionResult Edit(int id)
        {
            foreach (Employee emp in employees) {
                if (emp.ID.Equals(id)) { 
                    return View(emp);
                }
            }
            return View("Error");
        }

        // POST: First/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Employee empT)
        {
            try
            {
                foreach (Employee emp in employees)
                {
                    if (emp.ID.Equals(id))
                    {
                        emp.copy(empT);
                        return RedirectToAction("Index");
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Error");
            }
        }

        // GET: First/Delete/5
        /*   public ActionResult Delete(int id)
           {
               int i = 0;
               foreach (Employee emp in employees)
               {
                   if (emp.ID.Equals(id))
                   {
                       employees.RemoveAt(i);
                       return RedirectToAction("Photos");
                   }
                   i++;
               }
               return RedirectToAction("Error");
           }
           */
        // GET: First/Delete/5
        public ActionResult DeletePhoto(string path)
        {
            int i = 0;
            foreach (Image img in images)
            {
                if (img.Path.Equals(path))
                {
                    if (!fileNames.Contains(path))
                    {
                        return RedirectToAction("Error");
                    }
                    fileNames.Remove(path);
                    images.RemoveAt(i);
                  //    if (System.IO.File.Exists("~\\outputCheck"+img.Path))
                  //    {
                        System.IO.File.Delete(img.fullPath);
                  //  }
                    return RedirectToAction("Photos");
                }
                i++;
            }
            return RedirectToAction("Error");
        }

        // GET: First/Delete/5
        public ActionResult Delete(string path)
        {
            foreach (Image img in images)
            {
                if (img.Path.Equals(path))
                {
                    return View(img);
                }
            }
            return View("Error");
        }

        public ActionResult Cancel()
        {
            return RedirectToAction("photos");
        }
    }
}

