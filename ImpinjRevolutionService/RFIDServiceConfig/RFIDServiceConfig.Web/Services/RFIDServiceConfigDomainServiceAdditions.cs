using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.ServiceModel.DomainServices.EntityFramework;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using RFIDServiceConfig.Web.Models;

namespace RFIDServiceConfig.Web.Services
{
    public partial class RFIDServiceConfigDomainService
    {
        /// <summary>
        /// Load all readers that are not the default one. The default reader is used when adding a new one to fill in default data.
        /// Include the antennas.
        /// </summary>
        /// <returns></returns>
        public IQueryable<Reader> GetReadersAndAtennas()
        {            
            var ret = from r in ObjectContext.Readers.Include("Antennae")
                      //where r.IsDefault == false
                      select r;
            return ret;
        }

        public Reader GetReaderConfig(int id)
        {
            var ret = (from r in ObjectContext.Readers.Include("ConfigItems").Include("ConfigItemGroup").Include("ConfigItemType")
                      where r.ID == id
                      select r).FirstOrDefault();
            return ret;
        }
    }
}