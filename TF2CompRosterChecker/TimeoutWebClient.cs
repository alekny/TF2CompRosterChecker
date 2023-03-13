
using System;
using System.Net;

namespace TF2CompRosterChecker
{
    //The default timeout for webrequests appears to be 100 seconds, 
    //which is far too much for our case, so we just make a new class
    internal class TimeoutWebClient : WebClient
    {
        private readonly int timeout = 20 * 1000;

        public TimeoutWebClient(int timeout) : base()
        {
            if (timeout > 0 && timeout < 20 * 1000)
            {
                this.timeout = timeout;
            }
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            HttpWebRequest w = (HttpWebRequest)base.GetWebRequest(uri);
            //Set a custom timeout
            w.Timeout = timeout; 
            //Set a user agent here, for privacy and interoperability.
            w.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36";
            return w;
        }
    }
}