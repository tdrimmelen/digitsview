using System;
using System.Windows;
using System.Windows.Media;
using System.Reflection;
using System.Diagnostics;

using DigitsViewLib;

namespace ScoreboardXaml
{
	public partial class MainControl
	{

        private string theScoreboardUrl;
        private string theTimeclockUrl;

        private ScoreboardRetriever theScoreboardRetriever;
        private TimeclockRetriever theTimeclockRetriever;

        public MainControl()
		{

            long time = 50; 
            
            this.InitializeComponent();

            Logger.Source = "Scoreboard";

            string configFileName = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Scoreboard.config";
            try
            {
                Configuration myConfig = new Configuration(configFileName);

                theScoreboardUrl = myConfig["ScoreboardURL"];
                theTimeclockUrl = myConfig["TimeclockURL"];
                time = Int64.Parse(myConfig["refreshTime"]);
            }
            catch(Exception e)
            {
                Logger.Log("Could not read config file or property not present'" + configFileName + "': " + e.Message, EventLogEntryType.Error, Logger.Type.InitFailure);
            }

            theScoreboardRetriever = new ScoreboardRetriever(this.Dispatcher, ScoreboardUpdate, time, theScoreboardUrl);
            theTimeclockRetriever = new TimeclockRetriever(this.Dispatcher, TimeclockUpdate, time, theTimeclockUrl);

        }		

        private void ScoreboardUpdate(ScoreboardResponse aResponse)
        {

            if ( aResponse != null && aResponse.Status == "OK" )
            {
                Home.Text = String.Format("{0}", aResponse.Home);
                Guest.Text = String.Format("{0}", aResponse.Guest);
            }
            else 
            {
                Home.Text = "-";
                Guest.Text = "-";
            }		
		}

        private void TimeclockUpdate(TimeclockResponse aResponse)
        {

            if (aResponse != null && aResponse.Status == "OK")
            {
                Timeclock.Text = String.Format("{0}", aResponse.Minute) + ":" + String.Format("{0:00}", aResponse.Second);
            }
            else
            {
                Timeclock.Text = "-";
            }
        }
    }
}


//