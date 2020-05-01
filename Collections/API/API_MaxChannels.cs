using System.Runtime.Serialization;


namespace XDIPAPI
{
    [DataContract]
    class API_MaxChannels
    {
        [DataMember]
        public int count { get; set; }
    }
}
