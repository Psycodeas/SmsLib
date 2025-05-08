using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace SmsLib
{
    public class I60G1Request : IRequset
    {
        const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        string BaseUrl;

        public I60G1Request(string baseUrl) 
        {
            this.BaseUrl = baseUrl;
        }

        public HttpRequestMessage DeleteSmsRequest(int id)
        {
            string action = $"action=refresh&next_page=private/GP/sms_req_live.asp&cmd=Command%3DDelSMS&sms_receive_storage=2&sms_delete_flag=0&sms_delete_idx={id}";


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"http://{BaseUrl}/apply.cgi")
            {
                Content = new StringContent(action, Encoding.UTF8, CONTENT_TYPE)
            };

            return request;
        }

        public HttpRequestMessage GetAllSmsRequest()
        {
            string action = $"action=refresh&next_page=private%2FGP%2Fsms_req_live.asp" +
                $"&cmd=Command%3DGetSMSInfoAll&sms_receive_format=1" +
                $"&sms_receive_storage=2&sms_receive_filter=6" +
                $"&T={Utils.Time()}";

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"http://{BaseUrl}/apply.cgi")
            {
                Content = new StringContent(action, Encoding.UTF8, CONTENT_TYPE)
            };

            return request;
        }

        public HttpRequestMessage LoginRequest(string username, string password)
        {
            string action = $"submit_button=login&" +
              $"submit_type=do_login&change_action=gozila_cgi&" +
              $"password=&username={username}&passwd={password}";


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"http://{BaseUrl}/../login.cgi")
            {
                Content = new StringContent(action, Encoding.UTF8, CONTENT_TYPE)
            };



            return request;
        }

        public HttpRequestMessage SendSms(string message, string destination)
        {
            string action = $"action=refresh&next_page=private%2FGP%2Fsms_req_live.asp" +
                $"&cmd=Command%3DSendSMS&sms_receive_format=1&sms_receive_storage=2" +
                $"&sms_receive_filter=6&sms_to={destination}&sms_message={message}" +
                $"&sms_send_format=1&sms_dcs_value=0&sms_send_priority=0" +
                $"&sms_send_privacy=0&sms_reply_user=0&sms_reply_delivery=0" +
                $"&sms_reply_read=0&sms_reply_report=0&sms_send_alert=0" +
                $"&T={Utils.Time()}";


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"http://{BaseUrl}/apply.cgi")
            {
                Content = new StringContent(action, Encoding.UTF8, CONTENT_TYPE)
            };

            return request;
        }

        public string SetCookie(HttpResponseMessage response, HttpClient client)
        {
            string setCookieHeader;

            try
            {
                setCookieHeader = response.Headers.GetValues("Set-Cookie").FirstOrDefault();
            }
            catch
            {
                return "Wrong Username or Password";
            }

            string cookieValue = setCookieHeader
                .Split(';')
                .First();

            client.DefaultRequestHeaders.Add("cookie", cookieValue);

            return "Success";
        }

        public void GetDefaultHeaders(HttpClient client)
        {
            var headers =  new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en-US,en;q=0.9" },
                { "Origin", $"http://{BaseUrl}" },
            };

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }

        public bool ValidateToken(string responseContent)
        {
            Regex titleRegex = new Regex(@"<title>Login</title>", RegexOptions.IgnoreCase);
            Match match = titleRegex.Match(responseContent);

            return !match.Success;
        }

        public bool IsSmsSent(string responseContent)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(responseContent);

            XmlNode resultNode = doc.SelectSingleNode("//SMS_SUBMIT_RSP/Result");

            string result = resultNode?.InnerText ?? "-1";
            if (result == "-1")
            {
                return false;
            }
            else if (result == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public HttpRequestMessage ResetDevice()
        {
            string action = "submit_button=SystemReset" +
                "&action=Reboot" +
                "&submit_type=" +
                "&change_action=" +
                "&next_page=SystemReset.asp";


            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"http://{BaseUrl}/apply.cgi")
            {
                Content = new StringContent(action, Encoding.UTF8, CONTENT_TYPE)
            };

            return request;
        }
    }
}
