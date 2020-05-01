using System;
using System.Collections.Generic;
using XDIPAPI.Classes;
using XDIPAPI.Classes.Network;
using XDIPAPI.Collections.API;

namespace XDIPAPI
{
    public class API
    {
        private Connect _connect = new Connect();

        /// <summary>
        /// IP Address for the REDPSU
        /// </summary>
        public string IPAddress
        {
            get { return _connect.IPAddress; }
            set { _connect.IPAddress = value; }
        }

        /// <summary>
        /// Get the API Response
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="method"></param>
        /// <returns>API Response</returns>
        private API_Response GetAPIResponse(string parameter, Connect.Method method)
        {
            return GetAPIResponse(parameter, method, "");
        }


        /// <summary>
        /// Get the API Response
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="method"></param>
        /// <param name="json"></param>
        /// <returns>API Response</returns>
        private API_Response GetAPIResponse(string parameter, Connect.Method method, string json)
        {
            return GetAPIResponse(parameter, method, json, "");
        }


        /// <summary>
        /// Get the API Response
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="method"></param>
        /// <param name="json"></param>
        /// <param name="bearer"></param>
        /// <returns>API Response</returns>
        private API_Response GetAPIResponse(string parameter, Connect.Method method, string json, string bearer)
        {
            API_Response apiresponse = _connect.Get(parameter, method, json, bearer);
            if (apiresponse.Error)
            {
               throw new Exception(apiresponse.ErrorMessage);
            }

            return apiresponse;
        }


        /// <summary>
        /// Get Channel List that contains the Channel ID and Node UuID
        /// </summary>
        /// <returns>Channel</returns>
        public List<Channel> GetChannels()
        {
            API_Response apiresponse = GetAPIResponse("/channels", Connect.Method.GET);
            if (apiresponse.StatusCode == 200)
            {
                List<API_Channels> apichannels = JSON.Deserialize<List<API_Channels>>(apiresponse.Response);
                if (apichannels != null)
                {
                    if (apichannels.Count > 0)
                    {
                        List<Channel> channels = new List<Channel>();
                        foreach (API_Channels apichannel in apichannels)
                        {
                            if (apichannel.nodes.Count > 0)
                            {
                                Channel channel = new Channel();
                                channel.Id = apichannel.id;
                                channel.NodeUuID = apichannel.nodes[0].nodeUuid;
                                channels.Add(channel);
                            }
                        }
                        return channels;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get the Local Computers Name and Description
        /// </summary>
        /// <returns>LocalComputer</returns>
        public LocalComputer GetLocalComputer()
        {
            API_Response apiresponse = GetAPIResponse("/localComputer", Connect.Method.GET);
            if (apiresponse.StatusCode == 200)
            {
                LocalComputer localcomputer = JSON.Deserialize<LocalComputer>(apiresponse.Response);
                if (localcomputer != null)
                {
                    return localcomputer;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the Node Details by Node Uuid
        /// </summary>
        /// <param name="nodeuuid"></param>
        /// <returns>NodeInfo</returns>
        public NodeInfo GetNode(string nodeuuid)
        {
            API_Response apiresponse = GetAPIResponse("/nodes/" + nodeuuid, Connect.Method.GET);
            if (apiresponse.StatusCode == 200)
            {
                NodeInfo apinodeinfo = JSON.Deserialize<NodeInfo>(apiresponse.Response);
                if (apinodeinfo != null)
                {
                    return apinodeinfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a list of the configured Nodes
        /// </summary>
        /// <returns>List<NodeInfo></returns>
        public List<NodeInfo> GetSelectedNode()
        {
            API_Response apiresponse = GetAPIResponse("/nodes/selected", Connect.Method.GET);
            if (apiresponse.StatusCode == 200)
            {
                List<NodeInfo> apinodeinfo = JSON.Deserialize<List<NodeInfo>>(apiresponse.Response);
                if (apinodeinfo != null)
                {
                    return apinodeinfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the maximum number of channels supported by this node.
        /// /// </summary>
        /// <returns>int</returns>
        public int GetMaxChannels()
        {
            API_Response apiresponse = GetAPIResponse("/channels/maximumAllowed", Connect.Method.GET);
            if (apiresponse.StatusCode == 200)
            {
                API_MaxChannels apimaxchannels = JSON.Deserialize<API_MaxChannels>(apiresponse.Response);
                if (apimaxchannels != null)
                {
                    return apimaxchannels.count;
                }
            }

            return 0;
        }

        /// <summary>
        /// Get the ID of the Connected Channel
        /// </summary>
        /// <returns>Channel Id</returns>
        public int GetConnectedChannel()
        {
            API_Response apiresponse = GetAPIResponse("/channels/connected", Connect.Method.GET);
            if (apiresponse.StatusCode == 200)
            {
                API_ConnectedChannel channelid = JSON.Deserialize<API_ConnectedChannel>(apiresponse.Response);
                if (channelid != null)
                {
                    return channelid.id;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get the Channels Status
        /// </summary>
        /// <returns>ChannelStatus</returns>
        public ChannelStatus GetChannelStatus()
        {
            API_Response apiresponse = GetAPIResponse("/channels/status", Connect.Method.GET);
            if (apiresponse.StatusCode == 200)
            {
                ChannelStatus channelstatus = JSON.Deserialize<ChannelStatus>(apiresponse.Response);
                if (channelstatus != null)
                {
                    return channelstatus;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a Full Channel list including their details.
        /// </summary>
        /// <returns>A list with all Channel ID and Detials combind</returns>
        public List<ChannelDetail> GetChannelDetails()
        {
            List<Channel> channels = GetChannels();
            if (channels != null)
            {
                List<ChannelDetail> channeldetails = new List<ChannelDetail>();

                LocalComputer localcomputer = GetLocalComputer();
                if (localcomputer != null)
                {
                    NodeInfo localnode = new NodeInfo();
                    localnode.Name = localcomputer.Name;
                    localnode.Description = localcomputer.Description;

                    if (channels.Count > 0)
                    {
                        NodeInfo thisnode = GetNode(channels[0].NodeUuID);
                        localnode.Active = thisnode.Active;
                        localnode.MacAddress = thisnode.MacAddress;
                        localnode.SerialNumber = thisnode.SerialNumber;
                        localnode.Type = thisnode.Type;
                        localnode.Uuid = thisnode.Uuid;
                    }

                    ChannelDetail localchannel = new ChannelDetail();
                    localchannel.Id = 0;
                    localchannel.Node = localnode;
                    channeldetails.Add(localchannel);
                }

                for (int i = 1; i < channels.Count; i++)
                {
                    NodeInfo node = GetNode(channels[i].NodeUuID);
                    if (node != null)
                    {
                        ChannelDetail channeldetail = new ChannelDetail();
                        channeldetail.Id = channels[i].Id;
                        channeldetail.Node = node;
                        channeldetails.Add(channeldetail);
                    }
                }

                return channeldetails;
            }

            return null;
        }       

        /// <summary>
        /// Retrieve Authenication Token.  This is required to change channel on the receiver.
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Authentication Token</returns>
        public string Authenticate(string password)
        {
            string json = "{\"accessPassword\":\""+ password + "\"}";
            string parameter = "/nodes/self/access";

            API_Response apiresponse = GetAPIResponse(parameter, Connect.Method.POST, json);

            if (apiresponse.StatusCode == 200)
            {
                API_Authenticate authenticate = JSON.Deserialize<API_Authenticate>(apiresponse.Response);
                if (authenticate.AccessToken != string.Empty)
                {
                    return authenticate.AccessToken;
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// Change to the Channel Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bearer"></param>
        /// <returns>Success</returns>
        public bool ChangeChannel(int id, string bearer)
        {
            API_Response apiresponse = GetAPIResponse("/channels/" + id.ToString() + "/switch", Connect.Method.POST, "", bearer);
            if (apiresponse.StatusCode == 204)
            {
                return true;
            }
            return false;
        }
    }
}
