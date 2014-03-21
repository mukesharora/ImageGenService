using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageGenModels;
using ImageGenWebApi.Models;

namespace ImageGenWebApi.Controllers
{
    public class ImageController : ApiController
    {
        /// <summary>
        /// Returns a collection of all images on the server
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<string> Get()
        //{
        //    return ImageDataModel.LoadImagePaths();
        //}

        public ImageData Get()
        {
            ImageData id = new ImageData();
            List<ImageField> imageInfo = new List<ImageField>();
            imageInfo.Add(new ImageField() { Field = "123" });
            imageInfo.Add(new ImageField() { Field = "456" });
            imageInfo.Add(new ImageField() { Field = "789" });
            id.Data = imageInfo;
            id.CoralType = 0;
            id.TemplateNum = 0;
            return id;
        }

        public HttpResponseMessage Post([FromBody]ImageData imageData)
        {
            string filePath = ImageDataModel.SaveImage(imageData);
            return Request.CreateResponse(HttpStatusCode.Created, filePath, "text/plain");                        
        }

        public HttpResponseMessage Delete(string imageName)
        {
            HttpStatusCode statusCode = HttpStatusCode.NoContent;
            if (!ImageDataModel.DeleteImage(imageName))
            {
                statusCode = HttpStatusCode.NotFound;
            }
            return Request.CreateResponse(statusCode);
        }

    }
}