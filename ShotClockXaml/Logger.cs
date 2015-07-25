using System;
using System.Diagnostics;
using System.Security;
using System.Collections.Generic;

namespace ShotclockXaml
{

    class Logger
    {
        // Ensure event source is created:
        // On elevated prompt: New-EventLog -logname Application -Source <sourcename>
        private string m_Source;
        
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

        private Dictionary<LogEntry, DateTime> m_LogEntries = new Dictionary<LogEntry, DateTime>();

    

        public Logger(string Source)
        {
            m_Source = Source;
        }


        ~Logger()
        {
            // Do nothing
        }

        public void Log(string logMessage, EventLogEntryType type, int eventID )
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
                EventLog.WriteEntry(m_Source, logMessage, type, eventID);
                m_LogEntries[entry] = DateTime.Now;
            }
            catch(SecurityException)
            {
                // Cannot log, just return
            }
        }

    }
}
