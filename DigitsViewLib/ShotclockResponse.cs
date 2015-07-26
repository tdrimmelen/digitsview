using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DigitsViewLib
{
    [DataContract]
    public class ShotclockResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "time")]
        public long Time { get; set; }
    }

}
