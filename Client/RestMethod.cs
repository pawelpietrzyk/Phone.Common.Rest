using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Common.Rest.Client
{
    [DataContract]
    public class RestMethod
    {
        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public object Param { get; set; }
    }
}
