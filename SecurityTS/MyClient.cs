using System;
using System.Collections.Generic;
using System.Text;
using System.Net;


namespace SecurityTS
{
    class MyClient : WebClient          //class to force only a header return to a uri query
    {
        public bool HeadOnly { get; set; }
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest req = base.GetWebRequest(address);
            if (HeadOnly && req.Method=="GET")
            {
                req.Method = "HEAD";
            }
            return req;
        }
    }
}
