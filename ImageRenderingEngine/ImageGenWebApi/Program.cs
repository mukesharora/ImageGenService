using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageGenWebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageLog.CreateSource();
            ImageGenWebApiService service = new ImageGenWebApiService("http://localhost:30526");
            service.Start();

            Console.ReadLine();

            service.Stop();
            MessageLog.DeleteSource();
        }
    }
}
