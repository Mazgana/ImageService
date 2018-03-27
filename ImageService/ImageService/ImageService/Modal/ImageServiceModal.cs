using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            String response = "Image copied successfully.";

            try
            {
                FileInfo imageInfo = new FileInfo(path);
                DateTime date = imageInfo.CreationTime;
                String year = date.Year.ToString();
                String month = date.Month.ToString();

                String dest = outputDir + "/" + year + "/" + month;
                response = CreateFolder(dest, out result);
                if (result == false)
                    return response;

                File.Copy(path, dest);

                dest = outputDir + "/Thumbnail/" + year + "/" + month;
                response = CreateFolder(dest, out result);
                if (result == false)
                    return response;

                File.Copy(path, dest);
                Image image = Image.FromFile(path);
                int size;
                Int32.TryParse(ConfigurationManager.AppSettings["thumbnailSize"], out size);
                Image thumb = image.GetThumbnailImage(size, size, () => false, IntPtr.Zero);
                thumb.Save(Path.ChangeExtension(imageInfo.FullName, "thumb"));

            } catch (Exception e)
            {
                result = false;
                return e.ToString();
            }

            result = true;
            return response;
        }

        public string CreateFolder(string dest, out bool result)
        {
            try
            {
                System.IO.Directory.CreateDirectory(dest);
            } catch (Exception e)
            {
                result = false;
                return "faild to create directory.";
            }

            result = true;
            return "directory created successfully.";
        }
    }
}
