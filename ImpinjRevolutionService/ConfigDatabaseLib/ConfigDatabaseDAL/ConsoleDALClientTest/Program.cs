using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfigDatabaseDAL;

namespace ConsoleDALClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ConfigDatabaseEntities context = ConfigDatabaseContext.GetContext())
            {
                int configGroupCount = context.ConfigItemGroup.Count();

                //print out name of each config group:
                var configGroups = from cg in context.ConfigItemGroup
                                   select cg;

                List <ConfigDatabaseDAL.ConfigItemGroup> cigs = configGroups.ToList();

                foreach (ConfigDatabaseDAL.ConfigItemGroup cig in cigs)
                {
                    Console.WriteLine(string.Format("Config Item Group : {0} - {1}", cig.Name, cig.Description));

                    foreach (ConfigItem ci in cig.ConfigItem)
                    {
                        Console.WriteLine("\t" + "Item Name : " + ci.Name + " Type : " + ci.ConfigItemType.Name +  " Value : " + ci.Value); 
                    }
                }


                //verify can add item / value of same name to two (or more ) different configuration groups:

                ConfigItemType ciStringType = context.ConfigItemType.Single(t => t.Name == "String");


                ConfigItemGroup cigReader1 = context.ConfigItemGroup.Single(c => c.Name == "Reader1");

                //cigReader1.ConfigItem.Add(new ConfigItem() 
                //{ Name = "Address", Value = "speedwayr-10-A1-D9.local",
                //             ConfigItemType = ciStringType   });

                //ConfigItemGroup cigReader2 = context.ConfigItemGroup.Single(c => c.Name == "Reader2");


                //cigReader2.ConfigItem.Add(new ConfigItem()
                //{
                //    Name = "Address",
                //    Value = "speedwayr-10-A1-44.local",
                //    ConfigItemType = ciStringType
                //});

                //context.SaveChanges();

                //test updating an existing value:


                ConfigItem ciReader1Address = cigReader1.ConfigItem.Single(i => i.Name == "Address");
                ciReader1Address.Value = "speedwayr-10-A1-D9.local";

                context.SaveChanges();
                   

                
            }
        }
    }
}
