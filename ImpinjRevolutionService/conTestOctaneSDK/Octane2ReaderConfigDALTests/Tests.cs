using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Octane2ReaderConfigDAL;
using System.Data.EntityClient;

namespace Octane2ReaderConfigDALTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestCreateEntities()
        {
            try
            {
                EntityConnectionStringBuilder conn = new EntityConnectionStringBuilder();
                conn.ProviderConnectionString = @"data source=C:\Workshop\February2013\ImpinjOctane2SDKPrototypes\conTestOctaneSDK\ImpinjReadersConfiguration.s3db";
                conn.Metadata = "res://*/Octane2ReaderConfig.csdl|res://*/Octane2ReaderConfig.ssdl|res://*/Octane2ReaderConfig.msl";
                conn.Provider = "System.Data.SQLite";

                ImpinjReadersConfigurationEntities context = new ImpinjReadersConfigurationEntities(conn.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
