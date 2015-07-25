using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShotclockXaml
{

    class Logger
    {
        private StreamWriter m_Writer;
        
        public Logger(string logFile)
        {
            m_Writer = File.AppendText(logFile);
        }

    
        ~Logger()
        {
            m_Writer.Close();
        }

        public  void Log(string logMessage)
        {
            m_Writer.Write("\r\nLog Entry : ");
            m_Writer.Write("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            m_Writer.WriteLine("  :");
            m_Writer.WriteLine("  :{0}", logMessage);
            m_Writer.WriteLine ("-------------------------------");
        }

    }
}
