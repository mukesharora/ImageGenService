using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ImageGenModels;
using System.Net;

namespace ImageGenWebApi.Models
{
    public class ImageDataModel
    {
        private static string _path;
        private static readonly string IMAGE_DIR = @"/CoralImages/";

        public static string SaveImage(ImageData imageData)
        {            
            Bitmap bitmap = CoralTemplate.GenerateImage(imageData.Data, imageData.CoralType, imageData.TemplateNum);

            string path = GetPath();

            string fileName = Guid.NewGuid() + ".bmp";
            string file = string.Format("{0}{1}", path, fileName);
            bitmap.Save(file);
            string localComputerName = Dns.GetHostName();
            return string.Format("http://{0}{1}{2}", localComputerName, IMAGE_DIR, fileName);
        }

        public static IEnumerable<string> LoadImagePaths()
        {
            string[] filePaths = Directory.GetFiles(GetPath(), "*.bmp");
            List<string> imagePaths = new List<string>();
            for(int i = 0;i<filePaths.Length;i++)
            {
                string fileName = Path.GetFileName(filePaths[i]);
                string localComputerName = Dns.GetHostName();
                imagePaths.Add(string.Format("http://{0}{1}{2}", localComputerName, IMAGE_DIR, fileName));
            }            
            return imagePaths;
        }

        public static bool DeleteImage(string imageName)
        {            
            string[] filePaths = Directory.GetFiles(GetPath(), "*.bmp");
            for (int i = 0; i < filePaths.Length; i++)
            {
                string fileName = Path.GetFileName(filePaths[i]);
                if (fileName == imageName)
                {
                    try
                    {
                        File.Delete(filePaths[i]);
                        return true;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        private static string GetPath()
        {
            if (_path == null)
            {
                object val = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\InetStp", "PathWWWRoot", @"C:\inetpub\wwwroot");
				if (val == null)
				{
					val = @"C:\inetpub\wwwroot";
				}
                _path = val.ToString() + IMAGE_DIR;
                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                }
            }            
            return _path;
        }        
    }
}
