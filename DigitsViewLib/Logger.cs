using System;
using System.Diagnostics;
using System.Security;
using System.Collections.Generic;

namespace DigitsViewLib
{

    public static class Logger
    {
        // Ensure event source is created:
        // On elevated prompt: New-EventLog -logname Application -Source <sourcename>

        [System.ComponentModel.DefaultValue("Unknown")]
        public static string Source  { set; get; }
        
        public class Type
        {
            public  const int InitFailure = 1;
            public  const int RetrieveFailure = 2;
        }

        public struct LogEntry
        {
            public string Message;
            public int EventId;

            public LogEntry(string aMessage, int aEventId)
            {
                Message = aMessage;
                EventId = aEventId;
            }
        }

        private static Dictionary<LogEntry, DateTime> m_LogEntries = new Dictionary<LogEntry, DateTime>();

        public static void Log(string logMessage, EventLogEntryType type, int eventID )
        {
            LogEntry entry = new LogEntry(logMessage, eventID);

            if (m_LogEntries.ContainsKey(entry))
            {
                if (DateTime.Now < (m_LogEntries[entry]).AddMinutes(1))
                {
                    // Already logged in the last minute
                    return;
                }
            }

            try
            {
                EventLog.WriteEntry(Source, logMessage, type, eventID);
                m_LogEntries[entry] = DateTime.Now;
            }
            catch(SecurityException)
            {
                // Cannot log, just return
            }
        }

    }
}
