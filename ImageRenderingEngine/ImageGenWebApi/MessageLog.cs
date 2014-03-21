namespace ImageGenWebApi
{
    using System;
    using System.Diagnostics;

    public class MessageLog
    {
        private static string LOG_NAME = "OMNI";
        private static string SOURCE_NAME = "ImageGenWebApiService";
        public static MessageLog Instance { get; private set; }
        private EventLog eventLog;

        public static void DeleteSource()
        {
            try
            {
                EventLog.DeleteEventSource(SOURCE_NAME);
            }
            catch (ArgumentException)
            {
                // the source might not exist or the user does not have permission to delete it
            }
                
        }

        public static void CreateSource()
        {
            string currentLogName = EventLog.LogNameFromSourceName(SOURCE_NAME, ".");
            if (currentLogName != null && currentLogName != LOG_NAME)
            {
                DeleteSource();
            }

            if (!EventLog.SourceExists(SOURCE_NAME))
            {
                EventLog.CreateEventSource(SOURCE_NAME, LOG_NAME);
            }
        }

        private MessageLog()
        {
            eventLog = new EventLog(LOG_NAME);
            eventLog.Source = SOURCE_NAME;
        }

        static MessageLog()
        {            
            Instance = new MessageLog();            
        }
                
        public void Error(string message)
        {
            eventLog.WriteEntry(message, EventLogEntryType.Error);
        }

        public void Info(string message)
        {
            eventLog.WriteEntry(message, EventLogEntryType.Information);
        }

        public void Warn(string message)
        {
            eventLog.WriteEntry(message, EventLogEntryType.Warning);
        }
    }
}