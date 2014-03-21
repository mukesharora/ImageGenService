using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImageGenModels;

using System.IO;
using System.Net;
using ImageRender;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteFileToIIS();
            DownloadFile(); 
        }

        private static void WriteFileToIIS()
        {
            List<ImageField> imageInfo = new List<ImageField>();
            imageInfo.Add(new ImageField() { Field = "123" });
            imageInfo.Add(new ImageField() { Field = "456" });
            imageInfo.Add(new ImageField() { Field = "789" });
            Bitmap image = CoralImage.CreateImageForEP55(imageInfo);
            object val = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\InetStp", "PathWWWRoot", @"C:\inetpub\wwwroot");
            string path = val.ToString() + @"\CoralImages\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            image.Save(path + "temp.bmp");   
        }

        private static void DownloadFile()
        {
            string remoteUri = "http://localhost/CoralImages/";
            string fileName = "temp.bmp", myStringWebResource = null;
            // Create a new WebClient instance.
            WebClient myWebClient = new WebClient();
            // Concatenate the domain with the Web resource filename.
            myStringWebResource = remoteUri + fileName;
            Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
            // Download the Web resource and save it into the current filesystem folder.
            myWebClient.DownloadFile(myStringWebResource, fileName);
            Console.WriteLine("Successfully Downloaded File \"{0}\" from \"{1}\"", fileName, myStringWebResource);
            //Console.WriteLine("\nDownloaded file saved in the following file system folder:\n\t" + Application.StartupPath);
        }
    }
}
