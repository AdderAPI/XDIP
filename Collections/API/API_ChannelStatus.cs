using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XDIPAPI
{
    [DataContract]
    public class ChannelStatus
    {
        [DataContract]
        public class Connection
        {
            [DataMember(Name = "id")]
            public string Id { get; set; }
            [DataMember(Name = "state")]
            public string State { get; set; }
            [DataMember(Name = "status")]
            public string Status { get; set; }
        }
        [DataMember(Name = "summary")]
        public string Summary { get; set; }
        [DataMember(Name = "connections")]
        public List<Connection> Connections { get; set; }
    }
}
