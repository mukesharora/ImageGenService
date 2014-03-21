using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.EntityClient;
using System.IO;

namespace ConfigDatabaseDAL
{
    public static class ConfigDatabaseContext
    {


        public static ConfigDatabaseEntities GetContext()
        {
            //get the full location of the assembly with ConfigDatabaseEntities in it
            string fullPath = System.Reflection.Assembly.GetAssembly(typeof(ConfigDatabaseEntities)).Location;

            //get the folder that's in
            string theDirectory = Path.GetDirectoryName(fullPath);

            string DB_NAME = "ConfigDatabase.s3db";

            string dbPath = Path.Combine(theDirectory, DB_NAME);

           
        

            EntityConnectionStringBuilder sb = new EntityConnectionStringBuilder();
            sb.Metadata = "res://*/ConfigDatabaseModel.csdl|res://*/ConfigDatabaseModel.ssdl|res://*/ConfigDatabaseModel.msl";
            sb.Provider = "System.Data.SQLite";
            sb.ProviderConnectionString = "data source=" + dbPath;

            return new ConfigDatabaseEntities(sb.ToString());
        }

        public static bool ToBool(int value)
        {
            if (value == 0)
            {
                return false;
            }

            return true;
        }


    }
}
