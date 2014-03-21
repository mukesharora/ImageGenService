using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReaderConfigDALProto
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Impinj Reader Data Access Layer Prototype");

//            TestCreateEntities();

            TestAddReader();

            Console.WriteLine("Press Enter (twice) to Exit");
            Console.ReadLine();
        }


        static void TestCreateEntities()
        {
            try
            {
                ImpinjReadersConfigurationEntities context = new ImpinjReadersConfigurationEntities();

                List<Reader> readers = context.Readers.ToList();

                int readerCount = readers.Count;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Also tests unique constraint if you run this method twice against the same database (without first deleting the previous add)
        /// </summary>
        static void TestAddReader()
        {
            try
            {
                using (ImpinjReadersConfigurationEntities context = new ImpinjReadersConfigurationEntities())
                {
                    int beforeCount = context.Readers.Count();


                    Reader rdr = new Reader() { HostName = "ersatz.reader.hostname.local" };

                    context.Readers.AddObject(rdr);

                    context.SaveChanges();

                    int afterCount = context.Readers.Count();
                }
            }
            catch (System.Data.UpdateException ue)
            {
                Console.WriteLine(ue.Message);
                if (ue.InnerException != null)
                {
                    Console.WriteLine(ue.InnerException.Message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
