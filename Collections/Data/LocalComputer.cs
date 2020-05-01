using System.Runtime.Serialization;

namespace XDIPAPI
{
    [DataContract]
    public class LocalComputer
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
