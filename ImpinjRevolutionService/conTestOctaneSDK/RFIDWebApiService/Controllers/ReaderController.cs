using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using RFIDWebApiService.Models;
using Octane2ReaderBLL;
using Impinj.OctaneSdk;

namespace RFIDWebApiService.Controllers
{
    public class ReaderController : ApiController
    {
        // GET api/request
        public IEnumerable<Reader> Get()
        {
			List<Reader> readers = new List<Reader>();

			foreach(ImpinjReader impinj in Globals.ReaderInterface.GetReaders())
			{
				readers.Add(ImpinjToReader(impinj));
			}

			if(readers.Count == 0)
			{
				throw new HttpResponseException(new HttpResponseMessage()
				{
					Content = new StringContent("No Readers found."),
					StatusCode = HttpStatusCode.NotFound
				});
			}

			return readers;
        }

        public Reader Get(string id)
        {
			ImpinjReader impinj = Globals.ReaderInterface.GetReader(id);

			//Return 404 if the request was not found
			if(impinj == null)
			{
				throw new HttpResponseException(new HttpResponseMessage()
				{
					Content = new StringContent(String.Format("No Reader found with device id: {0}.", id)),
					StatusCode = HttpStatusCode.NotFound
				});
			}

			return ImpinjToReader(impinj);
        }

		public GPIOState Get(string id, int gpiPort)
		{
			Reader reader = Get(id);

			if(reader.IsConnected.Value)
			{
				return reader.GPIPorts[gpiPort - 1];
			}

			throw new HttpResponseException(new HttpResponseMessage()
			{
				Content = new StringContent(String.Format("Reader {0} disconnected, cannot query GPI state.", id)),
				StatusCode = HttpStatusCode.InternalServerError
			});
		}

        public HttpResponseMessage Post(ReaderRequest request)
        {
			string successString = "";
			string errorString = "";

			ImpinjReader reader = Globals.ReaderInterface.GetReader(request.ReaderName);

			if(reader == null)
			{
				return new HttpResponseMessage()
				{
					Content = new StringContent(String.Format("No Reader found with device id: {0}.", request.ReaderName)),
					StatusCode = HttpStatusCode.NotFound
				};
			}

			try
			{
				if(request.ReaderStateChange != null)
				{
					string onOrOff = request.ReaderStateChange.Value ? "on" : "off";
					successString = String.Format("Successfully turned reader {0} {1}",
						request.ReaderName, onOrOff);
					errorString = String.Format("An error occured when trying to turn reader {0} {1}:",
						request.ReaderName, onOrOff);

					Globals.ReaderInterface.StartStopReader(reader, request.ReaderStateChange.Value);
				}
				else if(request.GpoStateChange != null)
				{
					string highOrLow = request.GpoStateChange.State ? "high" : "low";
					successString = String.Format("Successfully set GPO port {0} on reader {1} to {2}",
						request.GpoStateChange.PortNum, request.ReaderName, highOrLow);
					errorString = String.Format("An error occured when trying to set GPO port {0} on reader {1} to {2}:",
						request.GpoStateChange.PortNum, request.ReaderName, highOrLow);

					Globals.ReaderInterface.SetGPO(reader, request.GpoStateChange.PortNum,
						request.GpoStateChange.State);
				}
			}
			catch(Exception e)
			{
				return new HttpResponseMessage()
				{
					Content = new StringContent(String.Format(errorString + "\n{0}", e.Message)),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}

            HttpResponseMessage response = new HttpResponseMessage()
            {
                Content = new StringContent(successString),
                StatusCode = HttpStatusCode.Created
            };

            string uri = Url.Route(WebApiConfig.READER_ROUTE_NAME, new { id = request.ReaderName });
            response.Headers.Location = new Uri(Request.RequestUri, uri);
            
			return response;
        }

		private Reader ImpinjToReader(ImpinjReader impinj)
		{
			Reader reader = new Reader()
			{
				Name = impinj.Name,
				IsConnected = impinj.IsConnected
			};

			if(impinj.IsConnected)
			{
				Status readerStatus = impinj.QueryStatus();
				List<GPIOState> gpiPorts = new List<GPIOState>();

				foreach(GpiStatus gpi in readerStatus.Gpis)
				{
					gpiPorts.Add(new GPIOState()
					{
						PortNum = gpi.PortNumber,
						State = gpi.State
					});
				}

				reader.IsReading = readerStatus.IsSingulating;
				reader.GPIPorts = gpiPorts;
			}

			return reader;
		}
    }
}
