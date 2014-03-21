using ImageGenModels;
using ImageRender;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web.Http;

namespace ImageGenWebApi.Controllers
{
    public class CustomImageController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GenerateLabelPreview(int labelTemplateID)
        {
            ProviewImageGenerator generator = new ProviewImageGenerator();
            byte[] data = generator.GenerateLabelPreview(labelTemplateID);
            HttpResponseMessage response = null;
            if (data != null)
            {
                response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ByteArrayContent(data) };
            }
            return response;
        }

        [HttpGet]
        public HttpResponseMessage GenerateLabelPreview(int labelTemplateID, int assetID)
        {
            ProviewImageGenerator generator = new ProviewImageGenerator();
            byte[] data = generator.GenerateLabelPreview(labelTemplateID, assetID);
            HttpResponseMessage response = null;
            if (data != null)
            {
                response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ByteArrayContent(data) };
            }
            return response;
        }

        [HttpGet]
        public HttpResponseMessage GenerateLabelPreviewTextOnly(int labelTemplateID, int? assetID = null)
        {
            ProviewImageGenerator generator = new ProviewImageGenerator();
            byte[] data = generator.GenerateLabelPreviewTextOnly(labelTemplateID, assetID);
            HttpResponseMessage response = null;
            if (data != null)
            {
                response = new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new ByteArrayContent(data) };
            }
            return response;
        }

        [HttpGet]
        public HttpResponseMessage GetLabelParameters(int labelTemplateID)
        {
            ProviewImageGenerator generator = new ProviewImageGenerator();
            Dictionary<string, string> data = generator.GetLabelParameters(labelTemplateID);

            HttpResponseMessage response = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ObjectContent<Dictionary<string, string>>(data, new JsonMediaTypeFormatter(), "application/json")
            };

            return response;
        }


    }
}
