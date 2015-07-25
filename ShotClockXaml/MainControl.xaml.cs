using System;
using System.Globalization;
using System.Windows.Threading;
using System.Linq;
using System.Media;
using System.Text;
using System.Xml;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Data.OleDb;
using System.Data;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Configuration;
using System.IO;
using System.Reflection;


using System.Diagnostics;


namespace ShotclockXaml
{

    [DataContract]
    public class ShotclockResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "time")]
        public long Time { get; set; }
     }

	public partial class MainControl
	{

        private System.Timers.Timer m_timer;
        private Logger m_ErrorLog;
        private ShotclockResponse m_response;
        private string m_url;
        private long m_attentionTime = 5;


		// Here I am declaring a new variable that will be in the style of XML
		// If we where going to receive a word we would use, "String data"
		XmlDocument data = new XmlDocument();
		// a constant, this is where we tell the program to get the data
		//This does not have to be a http request, this can be from c: 
		public MainControl()
		{
        
			this.InitializeComponent();

            m_ErrorLog = new Logger("Shotclock");

            long time = 50;

            string configFilename =  System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\shotclock.config";
            try
            {
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
                configMap.ExeConfigFilename = configFilename;
                Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                AppSettingsSection appSettings = configuration.AppSettings;

                URL.Text = appSettings.Settings["URL"].Value;
                time = Int64.Parse(appSettings.Settings["refreshTime"].Value);
                m_attentionTime = Int64.Parse(appSettings.Settings["attentionTime"].Value);
            }
            catch (Exception e)
            {
                m_ErrorLog.Log("Could not read config file '" + configFilename +"': "+ e.Message, EventLogEntryType.Error, Logger.Type.InitFailure);
            }

			//create a new thread

            m_url = URL.Text;
            m_timer = new System.Timers.Timer(time);

            m_timer.Elapsed += ThreadProc;
            m_timer.Enabled = true;

        }

        ~MainControl()
        {
            if (m_timer != null)
            {
                m_timer.Enabled = false;
            }
            m_ErrorLog = null;
        }

        private ShotclockResponse MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Timeout = 1000;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ShotclockResponse));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    ShotclockResponse jsonResponse = objResponse as ShotclockResponse;
                    return jsonResponse;
                }
            }
            catch (Exception e)
            {
                m_ErrorLog.Log("Error retrieving data: " + e.Message, EventLogEntryType.Error, Logger.Type.RetrieveFailure);

                return null;
            }
        }
		
		// a void function for our new thread
		// this just has to be void, it can have any name you choose
        public void ThreadProc(Object source, ElapsedEventArgs e)
        {
            m_timer.Enabled = false;

            m_response  = MakeRequest(m_url);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                    (ThreadStart)delegate()
                    {
                        if ( m_response != null && m_response.Status == "OK" )
                        {
                            Shotclock.Text = String.Format("{0:00}",m_response.Time);
                            if (m_response.Time <= m_attentionTime)
                            {
                                Shotclock.Fill = new SolidColorBrush(Colors.Yellow);
                            }
                            else
                            {
                                Shotclock.Fill = new SolidColorBrush(Colors.White);
                            }
                        }
                        else 
                        {
                            Shotclock.Text = "-";
                        }

                        m_url = URL.Text;
                    }
                    );

                m_timer.Enabled = true;
		
		}
	}
}


//