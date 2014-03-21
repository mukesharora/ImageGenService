using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace ReaderApi.Config
{
    public class ReaderListDeserializer
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ReaderListDeserializer));
        
        #region P/Invoke Code Declaration for GetModuleFileName
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity]
        private static extern uint GetModuleFileName(
        [System.Runtime.InteropServices.In]
        IntPtr hModule,
        [System.Runtime.InteropServices.Out]
        System.Text.StringBuilder lpFilename,
        [System.Runtime.InteropServices.In]
        [System.Runtime.InteropServices.MarshalAs(
        System.Runtime.InteropServices.UnmanagedType.U4)]
        int nSize
        );
        #endregion

        public static ReaderConfig LoadReaderConfig(string readerId)
        {
            List<ReaderConfig> readerConfigs = LoadReaderConfigs();
            ReaderConfig ret = readerConfigs.Where(r => r.ReaderID == readerId).FirstOrDefault();
            return ret;
        }

        public static List<ReaderConfig> LoadReaderConfigs()
        {
            string appDirectory = "";

            System.Text.StringBuilder sb = new System.Text.StringBuilder(260);
            if (GetModuleFileName(System.IntPtr.Zero, sb, sb.Capacity) == 0)
            {
                throw new Exception("Reader API Unable to determine application directory path");
            }
            else
            {
                appDirectory = System.IO.Path.GetDirectoryName(sb.ToString());
            }

            ReaderListDeserializer rld = new ReaderListDeserializer();

            List<ReaderConfig> readerConfigs = rld.LoadReaderData(Path.Combine(appDirectory, "Config\\ReaderList.txt"));
            return readerConfigs;
        }
        
        
        private List<ReaderConfig> LoadReaderData(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException(configFilePath);
            }

            List<ReaderConfig> list = new List<ReaderConfig>();

            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(configFilePath);
            while ((line = file.ReadLine()) != null)
            {
                string[] parts = line.Split(',');

                if (parts.Count() < 3)
                {
                    continue;
                } 
              
                // default read on time of 500 ms
                int readTimeInMs = 500;
                try
                {
                    readTimeInMs = int.Parse(parts[2].Trim());
                } 
                catch (FormatException ex)
                {
                    Logger.Error(string.Format("Could not format the string {0} to an integer. Tried to determine the read time in MS. Using default value of {1}", parts[2].Trim(), readTimeInMs));
                }

                // default full power
                List<int> antennaPowers = new List<int>() { 30, 30 };
                if (parts.Count() > 3)
                {
                    antennaPowers = new List<int>();
                    try
                    {
                        for (int i = 3; i < parts.Count(); i++)
                        {
                            antennaPowers.Add(int.Parse(parts[i].Trim()));
                        }
                    }
                    catch (Exception)
                    {
                        antennaPowers = new List<int>() { 30, 30 };
                    }
                }

                list.Add(new ReaderConfig() { HostName = parts[0].Trim(), ReaderID = parts[1].Trim(), ReadTimeInMs = readTimeInMs, AntennaPowers = antennaPowers.ToArray() });
            }

            file.Close();

            return list;

        }
    }
}
