using System;
using System.Linq;
using System.Threading;
using ReaderApi.Error;
using ReaderApi.Model;
using ReaderApi.Reader;
using log4net;

namespace ReaderApi
{
    class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        static int badReadCnt = 0;
        static int numReadCnt = 0;
        
        static void Main(string[] args)
        {
            RFIDReader.ConfigureLog4Net();
            RFIDReader reader = new RFIDReader("LineA");
            reader.RFIDTagsReported += new RFIDReader.RFIDTagsReportedHandler(reader_RFIDTagsReported);
            
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    reader.ReportStrongestTag(1);
                }
                catch (ConnectionException)
                { }
            }

            Thread.Sleep(1000);

            Logger.Debug("BadReadCnt = " + badReadCnt + " numReadCnt = " + numReadCnt);

            //reader = new RFIDReader("LineB");
            //reader.ReportStrongestTagPerAntenna();

            Console.ReadLine();
        }

        static void reader_RFIDTagsReported(RFIDReader reader, System.Collections.Generic.List<RFIDTag> tags)
        {
            foreach (RFIDTag tag in tags)
            {
                if (!(tag.Epc == "201302040000000000000042" || tag.Epc == "201302040000000000000047"))
                {
                    badReadCnt++;
                }
                numReadCnt++;
            }
        }
    }
}
