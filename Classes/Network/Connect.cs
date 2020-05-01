using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace XDIPAPI.Classes.Network
{
	public class Connect
	{
		public enum Method { POST, PUT, GET, DELETE }	
		public bool Debug = false;
		public string IPAddress = string.Empty;
		public int Retries = 1;
		public int Timeout = 1000;

		public string Response { get; private set; } = string.Empty;
		public string ErrorMessage { get; private set; } = string.Empty;
		public int StatusCode { get; private set; } = 0;
		public bool IsError { get; private set; } = false;

		public Connect() { }

		public Connect(string ipaddress)
		{
			IPAddress = ipaddress;
		}

		public API_Response Get(string parameter, Method method)
		{
			return Get(parameter, method, "", "");
		}

		public API_Response Get(string parameter, Method method, string json, string bearer)
		{			
			string url = "https://" + IPAddress + ":8443/api" + parameter;

			API_Response apiResponse = null;

			int loop = 0;
			bool complete = false;
			DateTime datetime = DateTime.MinValue;

			if (parameter == string.Empty || IPAddress == string.Empty)
			{
				throw new Exception("You must provide a paramater and/or IP address");
			}

			while (loop <= Retries && !complete)
			{
				apiResponse = new API_Response();
				IsError = false;
				ErrorMessage = string.Empty;
				Response = string.Empty;
				datetime = DateTime.Now;

				HttpWebRequest request = null;
				HttpWebResponse response = null;

				try
				{
					if (Debug)
					{
						Console.WriteLine("Atempt " + (loop + 1).ToString() + " of " + (Retries + 1).ToString());
						Console.WriteLine("Sending WebRequest: " + method.ToString() + " " + url);
					}

					request = (HttpWebRequest)WebRequest.Create(url);
					
					// SSL Certificate is Self-Signed, ignore validatation.
					ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

					// Set SSL Protocol to TLS 1.2
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

					request.ContentType = "application/json";
					request.Accept = "application/json";
					request.Method = method.ToString();
					request.Timeout = Timeout;
					request.ReadWriteTimeout = Timeout;					
					request.KeepAlive = false;
					request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
					request.Proxy = null;
					request.ContentLength = json.Length;

					if (bearer != string.Empty)
					{
						request.Headers["Authorization"] = "Bearer " + bearer;
					}

					if (json != string.Empty)
					{
						if (Debug) Console.WriteLine("Data TX: " + json + "  Length: " + json.Length.ToString());
						byte[] payload = Encoding.ASCII.GetBytes(json);
						Stream stream = request.GetRequestStream();
						stream.Write(payload, 0, payload.Length);
						stream.Flush();
						stream.Close();
					}

					if (Debug) Console.WriteLine("Waiting for Response");

					using (response = (HttpWebResponse)request.GetResponse())
					{
						Stream dataStream = response.GetResponseStream();
						StreamReader reader = new StreamReader(dataStream);
						apiResponse.Response = reader.ReadToEnd();
						apiResponse.StatusCode = (int)((HttpWebResponse)response).StatusCode;
						reader.Close();
						dataStream.Close();
					}

					if (Debug)
					{
						Console.WriteLine("Data RX: " + apiResponse.Response);
						Console.WriteLine("Status Code: " + apiResponse.StatusCode);
					}

					complete = true;
				}

				catch (SocketException ex)
				{
					apiResponse.Error = true;
					apiResponse.ErrorMessage = ex.Message;
					apiResponse.StatusCode = (int)response.StatusCode;
				}
				catch (IOException ex)
				{
					apiResponse.Error = true;
					apiResponse.ErrorMessage = ex.Message;
					apiResponse.StatusCode = (int)response.StatusCode;
				}
				catch (WebException ex)
				{
					apiResponse.Error = true;
					apiResponse.ErrorMessage = ex.Message;
					if (ex.Response != null)
					{
						apiResponse.StatusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
					}
				}
				catch (Exception ex)
				{
					apiResponse.Error = true;
					apiResponse.ErrorMessage = ex.Message;
					if (response != null) apiResponse.StatusCode = (int)response.StatusCode;
				}

				if (Debug)
				{
					if (apiResponse.Error)
					{
						Console.WriteLine("Error:       " + apiResponse.ErrorMessage);
						Console.WriteLine("Status Code: " + apiResponse.StatusCode);
					}

					TimeSpan ts = DateTime.Now.Subtract(datetime);
					Console.WriteLine("TimeTaken:   " + ts.TotalMilliseconds + "ms");
				}

				loop++;
			}

			IsError = apiResponse.Error;
			Response = apiResponse.Response;
			ErrorMessage = apiResponse.ErrorMessage;
			StatusCode = apiResponse.StatusCode;

			return apiResponse;
		}
	}
}
