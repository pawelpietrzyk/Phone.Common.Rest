using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Common.Rest.Model
{
    [DataContract]
    public class BaseResult
    {
        [DataMember]
        public ResultCodes ResultCode { get; set; }
        [DataMember]
        public string ResultMessage { get; set; }
    }
}