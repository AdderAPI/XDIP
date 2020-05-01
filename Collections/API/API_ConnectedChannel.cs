using System.Runtime.Serialization;


namespace XDIPAPI
{
    [DataContract]
    public class API_ConnectedChannel
    {
        [DataMember]
        public int id { get; set; }
    }
}
