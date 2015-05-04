using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Common.Rest
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    
    public class Http
    {
        public const string DefaultContentType = "text/xml";
        public static Http get(string url, HttpParams parameters, string contentType = Http.DefaultContentType, int timeout = 5000)
        {
            return Http.method(HttpMethod.GET, url, String.Empty, parameters, contentType, timeout);            
        }
        public static Http post(string url, string data, HttpParams parameters, string contentType = Http.DefaultContentType)
        {
            return Http.method(HttpMethod.POST, url, data, parameters, contentType);            
        }
        public static Http put(string url, string data, HttpParams parameters, string contentType = Http.DefaultContentType)
        {
            return Http.method(HttpMethod.PUT, url, data, parameters, contentType);            
        }
        public static Http delete(string url, HttpParams parameters, string contentType = Http.DefaultContentType)
        {
            return Http.method(HttpMethod.DELETE, url, String.Empty, parameters, contentType);            
        }
        public static Http method(HttpMethod method, string url, string data, HttpParams parameters, string contentType = Http.DefaultContentType, int timeout = 5000)
        {
            Http http = new Http();
            http.Method = method;
            http.Url = url;
            http.Data = data;
            http.Params = parameters;
            http.ContentType = contentType;
            http.Timeout = timeout;
            return http;
        }
        public Http()
        {
            this.Params = new HttpParams();
        }
        public Http(HttpResponseEventHandler responseCallback) : this()
        {
            this.responseCallback = responseCallback;
        }
        public HttpMethod Method { get; set; }
        public string ContentType { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }
        public int Timeout { get; set; }
        public HttpParams Params { get; set; }

        public Http then(HttpResponseEventHandler callback)
        {
            this.responseCallback = callback;            
            return this;
        }
        public void Async()
        {
            this.BeginReguest();
        }
        protected void BeginReguest()
        {
            HttpWebRequest request = HttpWebRequest.Create(this.RequestUri) as HttpWebRequest;
            if (request != null)
            {
                //request.ContentType = this.ContentType;
                request.Method = this.Method.ToString();                             
                //request.AllowWriteStreamBuffering = true;
                if (!String.IsNullOrEmpty(this.Data))
                {
                    //request.AllowReadStreamBuffering = true;
                    request.ContentType = this.ContentType;
                    request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
                }
                else
                {
                    request.BeginGetResponse(new AsyncCallback(GetRequestCallback), request);
                }
                
            }
        }
        private void GetRequestStreamCallback(IAsyncResult result)
        {            
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                Stream stream = request.EndGetRequestStream(result);
                if (!String.IsNullOrEmpty(this.Data))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(this.Data);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Close();
                    //request.ContentLength = bytes.Length;
                }                
                request.BeginGetResponse(new AsyncCallback(GetRequestCallback), request);
            }
        }
        private void GetRequestCallback(IAsyncResult result)
        {            
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request != null)
            {
                HttpWebResponse response = request.EndGetResponse(result) as HttpWebResponse;
                if (response != null)
                {
                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();
                    reader.Close();
                    
                    //this.callResponseCallback(response.StatusCode, content);
                    this.callResponseCallbackMain(response.StatusCode, content);
                }
            }
        }                        
        
        public string RequestUri
        {
            get
            {
                if (this.Params.Count > 0)
                {
                    return String.Format("{0}?{1}", this.Url, this.Params.ToString());
                }
                else
                {
                    return this.Url;
                }                
            }            
        }
        public void AddParam(HttpParam param)
        {
            if (!this.Params.Contains(param))
            {
                this.Params.Add(param);
            }            
        }
        public void AddParam(string name, string value)
        {
            this.AddParam(new HttpParam(name, value));
        }
        private void callResponseCallbackMain(HttpStatusCode code, string content)
        {
            Dispatcher disp = Deployment.Current.Dispatcher;
            disp.BeginInvoke(() =>
            {
                this.callResponseCallback(code, content);
            });
        }
        private void callResponseCallback(HttpStatusCode code, string content)
        {
            if (responseCallback != null)
            {
                HttpResponseEventArgs args = new HttpResponseEventArgs();
                args.StatusCode = code;
                args.Content = content;
                responseCallback(this, args);
            }
        }
        protected HttpResponseEventHandler responseCallback;
    }
    public class HttpResponseEventArgs
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; }
    }
    public delegate void HttpResponseEventHandler(object sender, HttpResponseEventArgs e);
    public class HttpParam
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public HttpParam()
        {
            this.Name = String.Empty;
            this.Value = String.Empty;
        }
        public HttpParam(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
        public override string ToString()
        {
            return String.Format("{0}={1}", new object[] { this.Name, this.Value} );
        }
        public bool Equals(HttpParam obj)
        {
            bool ret = false;
            if (obj != null)
            {
                if (this.Name.Equals(obj.Name) && this.Value.Equals(obj.Value))
                {
                    ret = true;
                }
            }
            return ret;
        }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as HttpParam);
        }
        public override int GetHashCode()
        {
            int hashCodeName = (this.Name != null ? this.Name.GetHashCode() : 0);
            int hashCodeValue = (this.Value != null ? this.Value.GetHashCode() : 0);
            return hashCodeName ^ hashCodeValue;
        }
    }
    public class HttpParams : List<HttpParam>
    {
        public HttpParams() : base()
        {
            this.Separator = "&";
        }
        public HttpParams(string separator) : base()
        {
            this.Separator = separator;
        }
        public string Separator { get; set; }
        public override string ToString()
        {
            List<string> list = new List<string>();
            foreach (HttpParam param in this)
            {
                list.Add(param.ToString());
            }
            string[] tmp = list.ToArray();
            return String.Join(this.Separator, tmp);
        }
        public void Add(string name, string value)
        {
            this.Add(new HttpParam() { Name = name, Value = value });
        }

    }
    

    
}
