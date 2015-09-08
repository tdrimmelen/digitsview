using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;
using System.Windows.Threading;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Diagnostics;

namespace DigitsViewLib
{
    public class ScoreboardRetriever
    {
        private Action<ScoreboardResponse> theDelegate;
        private System.Timers.Timer theTimer;
        private string theURL;
        private const int theRequestTimeout = 1000;
        private Dispatcher theDispatcher;

        public ScoreboardRetriever(Dispatcher aDispatcher, Action<ScoreboardResponse> aDelegate, long aTime, string aURL)
        {
            theDispatcher = aDispatcher;
            theDelegate = aDelegate;
            theURL = aURL;

            theTimer = new System.Timers.Timer(aTime);

            theTimer.Elapsed += TimerProc;
            theTimer.Enabled = true;

        }

        ~ScoreboardRetriever()
        {
            theTimer.Enabled = false;
        }

        private void TimerProc(Object source, ElapsedEventArgs ea)
        {
            theTimer.Enabled = false;

            try
            {
                HttpWebRequest request = WebRequest.Create(theURL) as HttpWebRequest;
                request.Timeout = theRequestTimeout;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ScoreboardResponse));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    ScoreboardResponse jsonResponse = objResponse as ScoreboardResponse;

                    theDispatcher.BeginInvoke(DispatcherPriority.Send, theDelegate, jsonResponse);
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error retrieving data: " + e.Message, EventLogEntryType.Error, Logger.Type.RetrieveFailure);
                theDispatcher.BeginInvoke(DispatcherPriority.Send, theDelegate, null);
            }

            theTimer.Enabled = true;

        }
    }
}
