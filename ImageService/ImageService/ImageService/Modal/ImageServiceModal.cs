using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace ImageService.Modal
{
    class ImageServiceModal : IImageServiceModal
    {
        private String outputDir = ConfigurationManager.AppSettings["OutputDir"];

        public string AddFile(string path, out bool result) {

            try
            {
                FileInfo imageInfo = new FileInfo(path);
                DateTime date = imageInfo.CreationTime;
                String year = date.Year.ToString();
                String month = date.Month.ToString();
                String dest = outputDir + "/" + year + "/" + month;
                System.IO.Directory.CreateDirectory(dest);
                File.Copy(path, dest);
            } catch (Exception e)
            {
                result = false;
                return e.ToString();
            }

            result = true;
            return "Image copied successfully.";
        }
    }
}
