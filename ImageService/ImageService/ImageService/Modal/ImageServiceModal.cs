using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Globalization;

namespace ImageService.Modal
{
    class ImageServiceModal : IImageServiceModal
    {
        private String outputDir = ConfigurationManager.AppSettings["OutputDir"];

        public string AddFile(string path, out bool result) {
            String response = "Image copied successfully.";
            try
            {
                DateTime date = GetDateTakenFromImage(path);
                string year = date.Year.ToString();
                string month = date.Month.ToString();

                string newFile = Path.GetFileName(path);
                String dest = outputDir + @"\" + year + @"\" + month;
                response = CreateFolder(dest, out result);
                if (result == false)
                    return response;

                File.Copy(path, dest + @"\" + newFile);

                dest = outputDir + @"\Thumbnail\" + year + @"\" + month;
                response = CreateFolder(dest, out result);
                if (result == false)
                    return response;

                File.Copy(path, dest + @"\" + "thumb-" + newFile);

                string thumbPath = dest + @"\" + "thumb-" + newFile;

                FileInfo imageInfo = new FileInfo(thumbPath);
                Image image = Image.FromFile(thumbPath);
                int size;
                Int32.TryParse(ConfigurationManager.AppSettings["thumbnailSize"], out size);
                Image thumb = image.GetThumbnailImage(size, size, () => false, IntPtr.Zero);
                thumb.Save(Path.ChangeExtension(imageInfo.FullName, "thumb"));

                /**              Console.WriteLine(year);

                              string filename = Path.GetFileName(path);
                              String dest = outputDir + @"\" + year + @"\" + month;
                              response = CreateFolder(dest, out result);
                              if (result == false)
                                  return response;

                              File.Copy(path, dest + @"\" + filename);

                              dest = outputDir + @"\Thumbnail\" + year + @"\" + month;
                              response = CreateFolder(dest, out result);
                              if (result == false)
                                  return response;

                              File.Copy(path, dest + @"\" + "thumb" + filename);

                              Image image = Image.FromFile(dest);
                              int size;
                              Int32.TryParse(ConfigurationManager.AppSettings["thumbnailSize"], out size);
                              Image thumb = image.GetThumbnailImage(size, size, () => false, IntPtr.Zero);
                              FileInfo imageInfo = new FileInfo(dest);
                              thumb.Save(Path.ChangeExtension(imageInfo.FullName, "thumb"));
                              //Path.ChangeExtension(imageInfo.FullName, null); **/

            } catch (Exception e)
            {
                result = false;
                return e.ToString();
            }

            result = true;
            return response;
        }

        //retrieves the datetime WITHOUT loading the whole image
        public static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                // Get the Date Created property 
                PropertyItem propertyItem = myImage.PropertyItems.FirstOrDefault(i => i.Id == 306);
                if (propertyItem != null)
                {
                    // Extract the property value as a String. 
                    ASCIIEncoding encoding = new ASCIIEncoding();
                    string text = encoding.GetString(propertyItem.Value, 0, propertyItem.Len - 1);

                    // Parse the date and time. 
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    return DateTime.ParseExact(text, "yyyy:MM:d H:m:s", provider);
                } else
                {
                    return DateTime.Now;
                }
            }
        }

        public string CreateFolder(string dest, out bool result)
        {
            try
            {
                System.IO.Directory.CreateDirectory(dest);
            } catch (Exception e)
            {
                result = false;
                return "faild to create directory. error: "+ e.ToString();
            }

            result = true;
            return "directory created successfully.";
        }
    }
}
