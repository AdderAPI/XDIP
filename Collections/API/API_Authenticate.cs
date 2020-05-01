using System.Runtime.Serialization;

namespace XDIPAPI.Collections.API
{
    [DataContract]
    public class API_Authenticate
    {
        [DataMember(Name = "accessToken")]
        public string AccessToken { get; set; }
    }
}
