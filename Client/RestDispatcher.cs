using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Common.Rest.Client
{
    public abstract class RestDispatcher
    {        

        public void DispatchRequest(HttpWebRequest request, HttpWebResponse response)
        {
            switch (request.Method)
            {
                case "GET":
                    this.GET(request, response);
                    break;
                case "POST":
                    this.POST(request, response);
                    break;
                case "DELETE":
                    this.DELETE(request, response);
                    break;
                default: break;
            }
        }
        public void ResultOK(HttpWebResponse response)
        {
            if (response != null)
            {
                //response.StatusCode = HttpStatusCode.OK;
                //response.Response.StatusCode = 200;
            }
        }
        //public static RestMethod DecodeRestMethod(HttpWebRequest request, HttpWebResponse response)
        //{
        //    RestMethod method = Serializer.Deserialize(request., typeof(RestMethod)) as RestMethod;
            
        //    return method;
        //}
        public abstract void DispatchRestMethod(RestMethod method);
        public abstract void GET(HttpWebRequest request, HttpWebResponse response);
        public abstract void POST(HttpWebRequest request, HttpWebResponse response);
        public abstract void DELETE(HttpWebRequest request, HttpWebResponse response);
    }
}
