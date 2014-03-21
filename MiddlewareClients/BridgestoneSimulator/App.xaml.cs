using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace BridgestoneSimulator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void LogException(Exception Exception, string method, string message = null)
        {
            // TODO, log to real place
            if (message == null)
            {
                Debug.WriteLine(string.Format("Exception in {0}\n{1}", method, Exception.ToString()));
            }
            else
            {
                Debug.WriteLine(string.Format("Exception in {0}\n{1}\n{2}", method, message, Exception.ToString()));
            }
        }

        public static void LogError(string method, string message)
        {
            Debug.WriteLine(string.Format("Error: {0}\n{1}", method, message));
        }

        public static void LogTrace(string message)
        {
            Debug.WriteLine(string.Format("Trace: {0}", message));
        }
    }


}
