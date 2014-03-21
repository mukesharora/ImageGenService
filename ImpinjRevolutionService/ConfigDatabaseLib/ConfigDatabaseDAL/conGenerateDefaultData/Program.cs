using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConfigDatabaseDAL;


namespace conGenerateDefaultData
{
    class Program
    {
        public static void CleanTables(ConfigDatabaseEntities context)
        {
            //Clean out Config Item Table:
            foreach (ConfigItem ci in context.ConfigItem)
            {
                context.ConfigItem.DeleteObject(ci);
            }

            //Clean out Config Group Table:
            foreach (ConfigItemGroup cig in context.ConfigItemGroup)
            {
                context.ConfigItemGroup.DeleteObject(cig);
            }

            //Clean out Config Item Type table:
            foreach (ConfigItemType cit in context.ConfigItemType)
            {
                context.ConfigItemType.DeleteObject(cit);
            }

            context.SaveChanges();
        }

        public static void InsertConfigItemTypeData(ConfigDatabaseEntities context)
        {
            //Insert String Type:

            context.ConfigItemType.AddObject(new ConfigItemType()
            {
                 Name = "String",
                 Description = "A sequence of 1 or more characters"
            });
           

            //Insert Integer Type:

            context.ConfigItemType.AddObject(new ConfigItemType()
            {
                Name = "Integer",
                Description = "A whole number, can be negative"
            });

            context.SaveChanges();


        }

        public static void InsertConfigItemGroupData(ConfigDatabaseEntities context)
        {
            //Insert "Default" Group:

            context.ConfigItemGroup.AddObject(new ConfigItemGroup()
            {
                Name = "Default",
                Description = "Default catch-all group for config items"
            });

           
            //Insert 100 ImpinjReader groups:

            //for each new reader group, insert the items for that group:

            for (int index = 1; index < 101; index++)
            {
                string groupName = "ImpinjReader" + (index).ToString();

                context.ConfigItemGroup.AddObject(new ConfigItemGroup()
                {
                     Name = groupName.ToString(),
                     Description = "Impinj Reader Configuration Data"
                });

            }


            context.SaveChanges();

        }

        

        public static void InsertConfigItemDataForConfigGroups(ConfigDatabaseEntities context)
        {

            ConfigItemType stringType = context.ConfigItemType.Single(t => t.Name == "String");
            ConfigItemType intType = context.ConfigItemType.Single(t => t.Name == "Integer");


            
            foreach (ConfigItemGroup cig in context.ConfigItemGroup)
            {
                if (cig.Name == "Default")
                {
                    continue;
                }

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReaderConfigEnabled", Value = "0", ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReaderAddress",
                    Value = "speedwayr-XX-YY-ZZ.local",
                    ConfigItemType = stringType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "FriendlyName",
                    Value = "ImpinjReader-XXX",
                    ConfigItemType = stringType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ConnectTimeoutMS",
                    Value = "10000",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReadUserBankEnabled",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "UserBankReadWordCount",
                    Value = "2",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "UserBankReadWordPointer",
                    Value = "0",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReaderSearchMode",
                    Value = "SingleTarget",
                    ConfigItemType = stringType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReaderRFIDSessionNumber",
                    Value = "2",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "RFIDReadIncludeAntennaPortNumber",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "GPIPort1Enabled",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "GPIPort1DebounceInMS",
                    Value = "50",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "GPIPort2Enabled",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "GPIPort2DebounceInMS",
                    Value = "50",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "GPIPort3Enabled",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "GPIPort3DebounceInMS",
                    Value = "50",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "GPIPort4Enabled",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "GPIPort4DebounceInMS",
                    Value = "50",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "AutoStartMode",
                    Value = "GPITrigger",
                    ConfigItemType = stringType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "AutoStartGPIPortNumber",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "AutoStartGPILevel",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReaderKeepAlivesEnabled",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReaderKeepAlivePeriodInMS",
                    Value = "3000",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReaderKeepAlives.EnableLinkMonitorMode",
                    Value = "1",
                    ConfigItemType = intType
                });

                cig.ConfigItem.Add(new ConfigItem()
                {
                    Name = "ReaderKeepAlives.LinkDownThreshold",
                    Value = "5",
                    ConfigItemType = intType
                });




            }

            context.SaveChanges();
        }

        static void Main(string[] args)
        {
            using (ConfigDatabaseEntities context = ConfigDatabaseContext.GetContext())
            {
                Console.WriteLine("Cleaning Tables...");
                Program.CleanTables(context);

                Console.WriteLine("Inserting Config Item Type Table Data...");
                Program.InsertConfigItemTypeData(context);

                Console.WriteLine("Inserting Group Data...");
                Program.InsertConfigItemGroupData(context);

                Console.WriteLine("Inserting Item Data for each group with default Values");
                Program.InsertConfigItemDataForConfigGroups(context);


            }

        }
    }
}
