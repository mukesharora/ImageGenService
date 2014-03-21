using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ImageGenModels;
using ImageGenWebApi.Models;
using ImageRender;

namespace ImageGenWebApi.Controllers
{
    public class TemplateController : ApiController
    {
		public BarcodeList Get(CoralTypes coralType)
		{
			return CoralTemplate.GetTemplateFields(coralType);
		}
    }
}