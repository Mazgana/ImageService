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

        /// <summary>
        /// Move the file from the given path to the aoutput folder that appears in the app config file.
        /// </summary>
        /// <param name="path"> The image's path. </param>
        /// <param name="result"> The result of the action. </param>
        /// <returns> A sting message thet add information on the action result. </returns>
        public string AddFile(string path, out bool result) {
            //Creating the output folder and set it to be hidden.
            if (!Directory.Exists(outputDir))
            {
                DirectoryInfo di = Directory.CreateDirectory(outputDir);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            String response = "Image copy had failed.";

            try
            {
                DateTime date = DateTime.Now;
                try
                {
                  //Getting the image's creation date.
                  date = GetDateTakenFromImage(path);

                }
                catch (System.IO.FileNotFoundException)
                {
                    //Sometimes the application's processes tries to reach the image more then once.
                    //In that case - if in this function we get the exception that the path doesn't exist
                    //it's probably because other process already move it to the output folder.
                    result = true;
                    return "File isn't there - probably already copied.";
                }

                string year = date.Year.ToString();
                string month = date.Month.ToString();

                //copying the new image to output directory
                string newFile = Path.GetFileName(path);
                String dest = outputDir + @"\" + year + @"\" + month;
                response = CreateFolder(dest, out result);
                if (result == false)
                    return response;

                ////checks if a picture with the same name is already exists in folder. If it does, add the name
                //// of the picture the number of appearence in this folder.
                //int i = 2;
                //while (File.Exists(dest + @"\" + newFile))
                //{
                //  newFile = Path.GetFileNameWithoutExtension(path) + " (" + i + ")" + Path.GetExtension(path);
                //    i++;
                //}

                //The imgae name - if this name already exists it will delete the old one and save the new one
                newFile = Path.GetFileNameWithoutExtension(path) + Path.GetExtension(path);

                File.Move(path, dest + @"\" + newFile);

                //creating thumbnail directory
                string thumbDest = outputDir + @"\Thumbnail\" + year + @"\" + month;
                response = addingThumbCopyToThumbnailFolder(dest + @"\" + newFile, thumbDest, out result);
                if (result == false)
                    return response;

            } catch (Exception e)
            {
                result = false;
                return e.ToString();
            }

            result = true;
            return "Image copied successfully.";
        }

        /// <summary>
        /// retrieves the datetime WITHOUT loading the whole image
        /// </summary>
        /// <param name="path"> The image's path. </param>
        /// <returns> The image's creation date. </returns>
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
                    //If the creation time dosen't exists, the method will return the copy date.
                    return DateTime.Now;
                }
            }
        }

        /// <summary>
        /// Create new folder in the destination path if it's not already exists. 
        /// </summary>
        /// <param name="dest"> The folder's path. </param>
        /// <param name="result"> The creation's result. </param>
        /// <returns> String message with more information about the action. </returns>
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

        /// <summary>
        /// Create a thumb size copy of the picture and save it in the 'Thumbnail' folder.
        /// </summary>
        /// <param name="path"> The original image's path. </param>
        /// <param name="dest"> The thumbnail size image destination path. </param>
        /// <param name="result"> The action's result. </param>
        /// <returns> A message with information about the action. </returns>
        public string addingThumbCopyToThumbnailFolder(string path, string dest, out bool result)
        {
            try
            {
                // create the destination folder.
                string response = CreateFolder(dest, out result);
                if (result == false)
                    return response;

                string thumbPath = dest + @"\" + "thumb-" + Path.GetFileName(path);

                //Create the thumbnail size copy
                FileInfo imageInfo = new FileInfo(path);
                Image image = Image.FromFile(path);
                int size;
                Int32.TryParse(ConfigurationManager.AppSettings["thumbnailSize"], out size);
                Image thumb = image.GetThumbnailImage(size, size, () => false, IntPtr.Zero);
                thumb.Save(thumbPath);
                image.Dispose();
            } catch (Exception e)
            {
                result = false;
                return "faild to create thumb picture in thumbnail folder. error: " + e.ToString();
            }

            result = true;
            return "Thumb copy is saved in the 'Thumbnail' folder.";
        }
    }
}
