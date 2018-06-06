using ImageService.Communication;
using ImageService.Communication.Model;
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
        TcpClientChannel client { get; set; }

        static Config serviceConfig { get; set; }
        static List<Image> images = new List<Image>();
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
            return View();
        }

        [HttpGet]
        public ActionResult Logs()
        {
            return View();
        }

        [HttpGet]
        public JObject GetEmployee()
        {
            JObject data = new JObject();
            data["FirstName"] = "Kuky";
            data["LastName"] = "Mopy";
            return data;
        }

        [HttpPost]
        public JObject GetEmployee(string name, int salary)
        {
            foreach (var empl in employees)
            {
                if (empl.Salary > salary || name.Equals(name))
                {
                    JObject data = new JObject();
                    data["FirstName"] = empl.FirstName;
                    data["LastName"] = empl.LastName;
                    data["Salary"] = empl.Salary;
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
            var files = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Select(path => path.Replace(root, ""));
            foreach (string filename in files)
            {
                var name = Path.GetFileNameWithoutExtension(filename);
                var date = Path.GetDirectoryName(filename);
                date = date.TrimStart('\\');
                images.Add(new Image { Path = filename, Date = date, Name = name});
            }
            return View(images);
        }

        // GET: First/Config
        public ActionResult Config()
        {
            serviceConfig = new Config();

            this.client = TcpClientChannel.getInstance();
            this.client.UpdateModel += ViewUpdate;
            this.client.SendCommand(new ImageService.Communication.Model.CommandMessage(2, null));

            Console.WriteLine("hereeeeeeeeeeeee: " + ViewBag.outputDir + "\n");

            ViewBag.outputDir = serviceConfig.OutputDirectory;
            ViewBag.sourceName = serviceConfig.SourceName;
            ViewBag.logName = serviceConfig.LogName;
            ViewBag.size = serviceConfig.ThumbSize;

            return View();
        }

        private void ViewUpdate(object sender, CommandRecievedEventArgs e)
        {
            //Checks if the server respose is the application config.
            if (e.CommandID == 2)
            {

                //initialize the settings window's members to hold the configuration values.
                string config = e.Args[0];
                string[] configSrtings = config.Split('|');

                serviceConfig.OutputDirectory = configSrtings[0];
                serviceConfig.SourceName = configSrtings[1];
                serviceConfig.LogName = configSrtings[2];
                serviceConfig.ThumbSize = configSrtings[3];

                string[] handlersDirectories = configSrtings[4].Split(';');
                //for (int i = 0; i < handlersDirectories.Length; i++)
                //{
                //    if (handlersDirectories[i].Length != 0)
                //        serviceConfig.Handlers.Add(handlersDirectories[i]);
                //}
            }
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
        public ActionResult Delete(int id)
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
    }
}
