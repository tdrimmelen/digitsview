using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace DigitsViewLib
{
    [DataContract]
    public class ScoreboardResponse
    {
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "home")]
        public long Home { get; set; }
        [DataMember(Name = "guest")]
        public long Guest { get; set; }
    }

}
