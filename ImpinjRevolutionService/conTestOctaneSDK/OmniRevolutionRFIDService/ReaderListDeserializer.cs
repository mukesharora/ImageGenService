using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OmniRevolutionRFIDService
{
    public class ReaderListDeserializer
    {
        public List<Reader> LoadReaderData(string configFilePath)
        {
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException(configFilePath);
            }

            List<Reader> list = new List<Reader>();

            string line;

            System.IO.StreamReader file = new System.IO.StreamReader(configFilePath);
            while ((line = file.ReadLine()) != null)
            {
                string[] parts = line.Split(',');

                if (parts.Count() < 2)
                {
                    continue;
                }
                
                // default switch mode
                string readerMode = Reader.SWITCH_READERMODE;
                if (parts.Count() > 2)
                {
                    readerMode = parts[2].Trim();
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
                
                list.Add(new Reader() { HostName = parts[0].Trim(), ReaderID = parts[1].Trim(), ReaderMode = readerMode, AntennaPowers = antennaPowers.ToArray()});
            }

            file.Close();

            return list;

        }
    }
}
