using System;
using System.Collections.Generic;
using System.Linq;
using CALCManager.Models;
using CALCManager.Models.OtherClientModels;
using ImageGenModels;
using RESTClients.General;

namespace CoralDemo.demo
{
    public class CoralDemo
    {
        public void Start()
        {
            while (true)
            {
                Console.WriteLine("Enter 1 for page flip, 2 for send image, or 3 for quit");
                string pageFlipOrSendImage = Console.ReadLine();
                if (pageFlipOrSendImage != "1" && pageFlipOrSendImage != "2" && pageFlipOrSendImage != "3")
                {
                    Console.WriteLine("You failed to enter 1 for page flip, 2 for send image, or 3 for quit");
                    continue;
                }

                if (pageFlipOrSendImage == "3")
                {
                    break;
                }

                Console.WriteLine("Enter CORAL UID: ");
                string uid = Console.ReadLine();
                Console.WriteLine("Enter Page number: ");
                string pageStr = Console.ReadLine();
                uint pageNum = 0;
                try
                {
                    pageNum = uint.Parse(pageStr);
                }
                catch (Exception)
                {
                    Console.WriteLine("Please enter numbers only");
                    continue;
                }

                if (pageFlipOrSendImage == "2")
                {
                    Console.WriteLine("Enter 1 for image URL or 2 for image template");
                    string imageType = Console.ReadLine();
                    if (imageType != "1" && imageType != "2")
                    {
                        Console.WriteLine("You failed to enter 1 for image URL or 2 for image template");
                        continue;
                    }
                    if (imageType == "1")
                    {
                        Console.WriteLine("Enter URL");
                        string url = Console.ReadLine();
                        updateCoral(url, uid, pageNum);
                    }
                    else
                    {
                        Console.WriteLine("Enter 1 for P3 or 2 for E2 image template");
                        string templateNum = Console.ReadLine();
                        if (templateNum != "1" && templateNum != "2")
                        {
                            Console.WriteLine("You failed to enter 1 for P3 or 2 for E2 image template");
                            continue;
                        }
                        CoralTypes coralType = CoralTypes.E2;
                        if (templateNum == "1")
                        {
                            coralType = CoralTypes.P3;
                        }
                        
                        Console.WriteLine("Enter field 1");
                        string field1 = Console.ReadLine();
                        Console.WriteLine("Enter field 2");
                        string field2 = Console.ReadLine();
                        Console.WriteLine("Enter field 3");
                        string field3 = Console.ReadLine();
                        Console.WriteLine("Field 3: 1 for text, 2 for 1D barcode, 3 for 2D barcode");
                        string barcodeType = Console.ReadLine();
                        if (barcodeType != "1" && barcodeType != "2" && barcodeType != "3")
                        {
                            Console.WriteLine("You failed field 3: 1 for text, 2 for 1D barcode, 3 for 2D barcode");
                            continue;
                        }
                        BarcodeType bct = BarcodeType.TWO_D;
                        if (barcodeType == "1")
                        {
                            bct = BarcodeType.NONE;
                        }
                        else if (barcodeType == "2")
                        {
                            bct = BarcodeType.ONE_D;
                        }
                        string imageUrl = generateImageFromTemplate(field1, field2, field3, coralType, bct);
                        updateCoral(imageUrl, uid, pageNum);
                    }
                }
                else
                {
                    flipToPage(uid, pageNum);
                }
            }
        }

        private static void flipToPage(string uid, uint pageNum)
        {
            Coral coral = new Coral();
            coral.Uid = uid;
            coral.CalcId = "DEADBEEFBEBECAFE";
            coral.CurrentPage = pageNum;
            PostRestClient<Coral, CmdStatus> postClient = new PostRestClient<Coral, CmdStatus>("api/corals", "CALCManWebAPIHostName");
            WebResponseDataEventArgs<CmdStatus> status = postClient.Post(coral);
        }

        private static void updateCoral(string url, string uid, uint pageNum)
        {
            Coral coral = new Coral();
            coral.Uid = uid;
            coral.ImageUrl = url;
            coral.CalcId = "DEADBEEFBEBECAFE";
            coral.CurrentPage = pageNum;
            PostRestClient<Coral, CmdStatus> postClient = new PostRestClient<Coral, CmdStatus>("api/corals", "CALCManWebAPIHostName");
            WebResponseDataEventArgs<CmdStatus> status = postClient.Post(coral);
        }

        private static string generateImageFromTemplate(string field1, string field2, string field3, CoralTypes coralType, BarcodeType barcodeType)
        {
            List<ImageField> imageInfo = new List<ImageField>();
            imageInfo.Add(new ImageField() { Field = field1 });
            imageInfo.Add(new ImageField() { Field = field2 });
            imageInfo.Add(new ImageField() { Field = field3, BarcodeType = barcodeType });
            ImageData id = new ImageData();
            id.CoralType = coralType;
            id.Data = imageInfo;
            id.TemplateNum = 1;
            PostRestClient<ImageData, string> postClient = new PostRestClient<ImageData, string>("api/image", "ImageGenWebAPIHostName");
            WebResponseDataEventArgs<string> resp = postClient.Post(id);
            return resp.responseData;
        }
    }
}
