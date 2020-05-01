using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XDIPAPI
{
    [DataContract]
    public partial class API_Channels
    {
        [DataMember]
        public int id { get; set; }
        [DataMember]
        public List<Node> nodes { get; set; }
    }

    [DataContract]
    public partial class Node
    {
        [DataMember]
        public string nodeUuid { get; set; }
    }

}
