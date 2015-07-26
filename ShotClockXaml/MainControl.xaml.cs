using System;
using System.Windows;
using System.Windows.Media;
using System.Reflection;
using System.Diagnostics;

using DigitsViewLib;

namespace ShotclockXaml
{
	public partial class MainControl
	{

        private string theUrl;
        private long theAttentionTime = 5;

        private ShotclockRetriever theRetriever;

        public MainControl()
		{

            long time = 50; 
            
            this.InitializeComponent();

            Logger.Source = "Shotclock";

            string configFileName = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Shotclock.config";
            try
            {
                Configuration myConfig = new Configuration(configFileName);

                theUrl = myConfig["URL"];
                time = Int64.Parse(myConfig["refreshTime"]);
                theAttentionTime = Int64.Parse(myConfig["attentionTime"]);
            }
            catch(Exception e)
            {
                Logger.Log("Could not read config file or property not present'" + configFileName + "': " + e.Message, EventLogEntryType.Error, Logger.Type.InitFailure);
            }


			//create a new thread
            theRetriever = new ShotclockRetriever(this.Dispatcher, Update, time, theUrl);

            URL.Text = theUrl;


        }		

        private void Update(ShotclockResponse aResponse)
        {

            if ( aResponse != null && aResponse.Status == "OK" )
            {
                Shotclock.Text = String.Format("{0:00}", aResponse.Time);
                if (aResponse.Time <= theAttentionTime)
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

            theUrl = URL.Text;

		
		}
	}
}


//