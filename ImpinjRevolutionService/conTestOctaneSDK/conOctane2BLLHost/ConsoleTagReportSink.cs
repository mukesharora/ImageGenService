using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Octane2ReaderBLL;


namespace conOctane2BLLHost
{
    public class ConsoleTagReportSink : ITagReportSink
    {
        private bool _useLightBackground = true;

        public void ReportTagRead(string readerID, string readerAddress, string EPC)
        {
            Console.WriteLine(string.Format("Tag Read: RdrID:{0} RdrAddr:{1}, EPC:{2}", readerID, readerAddress, EPC));

            //if (true == _useLightBackground)
            //{
            //    Console.BackgroundColor = ConsoleColor.Gray;
            //}
            //else
            //{
            //    Console.BackgroundColor = ConsoleColor.Black;
            //}

            //_useLightBackground = !_useLightBackground;

            
        }

        public void ReportTagRead(string readerID, string readerAddress, string EPC, int AntennaPortNumber = -1)
        {
            if (AntennaPortNumber == 0)
            {
                Console.WriteLine(string.Format("Tag Read: RdrID:{0} RdrAddr:{1}, EPC:{2}", readerID, readerAddress, EPC));
            }
            else
            {
                Console.WriteLine(string.Format("Tag Read: RdrID:{0} RdrAddr:{1}, Ant#:{2} EPC:{3}", readerID, readerAddress, AntennaPortNumber,EPC));
            }
        }

        public void ReportTagRead(string readerID, string readerAddress, string EPC, int AntennaPortNumber = -1, string tagUSER = "")
        {
            throw new NotImplementedException();
        }
    }
}
