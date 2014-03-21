using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane2ReaderConfigDAL;
using System.Data.EntityClient;

namespace conTestOctane2ReaderConfigDAL
{
    class Program
    {
        static void Main(string[] args)
        {
            TestCreateEntities();
        }

        private static void TestCreateEntities()
        {
            try
            {
                EntityConnectionStringBuilder conn = new EntityConnectionStringBuilder();
                conn.ProviderConnectionString = @"data source=C:\Workshop\February2013\ImpinjOctane2SDKPrototypes\conTestOctaneSDK\ImpinjReadersConfiguration.s3db";
                conn.Metadata = "res://*/Octane2ReaderConfig.csdl|res://*/Octane2ReaderConfig.ssdl|res://*/Octane2ReaderConfig.msl";
                conn.Provider = "System.Data.SQLite";

                string connStr = conn.ToString();
                
                ImpinjReadersConfigurationEntities context = new ImpinjReadersConfigurationEntities(conn.ToString());
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                //throw ex;
            }

        }
    }
}
