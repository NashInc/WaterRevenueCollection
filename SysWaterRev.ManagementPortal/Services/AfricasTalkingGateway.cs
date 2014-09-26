using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace SimpleRevCollection.Management.Services
{
    public class AfricasTalkingGateway
    {
        private readonly string username;
        private readonly string apiKey;

        private const string SmsUrlString = "https://api.africastalking.com/version1/messaging";
        private const string VoiceUrlString = "https://voice.africastalking.com/call";

        public AfricasTalkingGateway(string username_, string apiKey_)
        {
            if (username_ == null) throw new ArgumentNullException("username_");
            if (apiKey_ == null) throw new ArgumentNullException("apiKey_");
            username = username_;
            apiKey = apiKey_;
        }

        public JArray SendMessage(string to, string message, string @from = "", int bulkSmsMode = 1, Hashtable options = null)
        {
            /*
         * The optional from_ parameter should be populated with the value of a shortcode or alphanumeric that is
         * registered with us
         * The optional  will be used by the Mobile Service Provider to determine who gets billed for a
         * message sent using a Mobile-Terminated ShortCode. The default value is 1 (which means that
         * you, the sender, gets charged). This parameter will be ignored for messages sent using
         * alphanumerics or Mobile-Originated shortcodes.
         */

            var data = new Hashtable();
            data["username"] = username;
            data["to"] = to;
            data["message"] = message;

            if (@from.Length > 0)
            {
                data["from"] = @from;
                data["bulkSMSMode"] = Convert.ToString(bulkSmsMode);

                if (options != null)
                {
                    if (options.Contains("keyword"))
                    {
                        data["keyword"] = options["keyword"];
                    }

                    if (options.Contains("linkId"))
                    {
                        data["linkId"] = options["linkId"];
                    }

                    if (options.Contains("enqueue"))
                    {
                        data["enqueue"] = options["enqueue"];
                    }
                }
            }

            var json = SendPostRequest(data, SmsUrlString);
            var obj = JObject.Parse(json);

            var smsMessageData = (JObject)obj["SMSMessageData"];
            var recipients = (JArray)smsMessageData["Recipients"];
            if (recipients.Count == 0)
            {
                throw new AfricasTalkingGatewayException((string)smsMessageData["Message"]);
            }
            return recipients;
        }

        public void Call(string @from, string to)
        {
            var data = new Hashtable();
            data["username"] = username;
            data["from"] = @from;
            data["to"] = to;

            var json = SendPostRequest(data, VoiceUrlString);
            var obj = JObject.Parse(json);
            if ((string)obj["Status"] != "Success")
            {
                throw new AfricasTalkingGatewayException((string)obj["ErrorMessage"]);
            }
        }

        public JArray FetchMessages(int lastReceivedId)
        {
            var sb = new StringBuilder();
            sb.Append(lastReceivedId);

            var data = new Hashtable();
            data["username"] = username;
            data["lastReceivedId"] = Convert.ToString(lastReceivedId);

            var json = SendGetRequest(data);
            var obj = JObject.Parse(json);

            var smsMessageData = (JObject)obj["SMSMessageData"];
            return (JArray)smsMessageData["Messages"];
        }

        private string SendPostRequest(IDictionary dataMap, string urlString)
        {
            try
            {
                var dataStr = "";
                foreach (string key in dataMap.Keys)
                {
                    if (dataStr.Length > 0) dataStr += "&";
                    var value = (string)dataMap[key];
                    dataStr += HttpUtility.UrlEncode(key, Encoding.UTF8);
                    dataStr += "=" + HttpUtility.UrlEncode(value, Encoding.UTF8);
                }

                var byteArray = Encoding.UTF8.GetBytes(dataStr);

                var webRequest = (HttpWebRequest)WebRequest.Create(urlString);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = byteArray.Length;
                webRequest.Accept = "application/json";

                webRequest.Headers.Add("apiKey", apiKey);

                var webpageStream = webRequest.GetRequestStream();
                webpageStream.Write(byteArray, 0, byteArray.Length);
                webpageStream.Close();

                var webpageReader = new StreamReader(webRequest.GetResponse().GetResponseStream());

                return webpageReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private string SendGetRequest(IDictionary dataMap)
        {
            try
            {
                var dataStr = "";
                foreach (string key in dataMap.Keys)
                {
                    if (dataStr.Length > 0) dataStr += "&";
                    var value = (string)dataMap[key];
                    dataStr += HttpUtility.UrlEncode(key, Encoding.UTF8);
                    dataStr += "=" + HttpUtility.UrlEncode(value, Encoding.UTF8);
                }

                var url = SmsUrlString + '?' + dataStr;

                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                webRequest.Accept = "application/json";
                webRequest.Headers.Add("apiKey", apiKey);

                var webpageReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                return webpageReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var obj = JObject.Parse(json);
                    var smsMessageData = (JObject)obj["SMSMessageData"];
                    throw new AfricasTalkingGatewayException((string)smsMessageData["Message"]);
                }
            }
        }
    }
}