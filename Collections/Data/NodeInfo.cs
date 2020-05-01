using System.Runtime.Serialization;

namespace XDIPAPI
{
    [DataContract]
    public class NodeInfo
    {
        [DataMember(Name = "uuid")]
        public string Uuid { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "active")]
        public bool Active { get; set; }
        [DataMember(Name = "serialNumber")]
        public string SerialNumber { get; set; }
        [DataMember(Name = "macAddress")]
        public string MacAddress { get; set; }
    }
}
