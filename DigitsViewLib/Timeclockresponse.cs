using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DigitsViewLib
{
    [DataContract]
    public class TimeclockResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "second")]
        public long Second { get; set; }
        [DataMember(Name = "minute")]
        public long Minute { get; set; }
    }

}
