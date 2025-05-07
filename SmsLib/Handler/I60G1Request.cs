using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SmsLib.Handler
{
    internal class I60G1Request : IRequset
    {
        const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        string BaseUrl;

        public I60G1Request(string baseUrl) 
        {
            this.BaseUrl = baseUrl;
        }

        public HttpRequestMessage DeleteInboxRequest(int id)
        {
            throw new NotImplementedException();
        }

        public HttpRequestMessage DeleteOutboxRequest(int id)
        {
            throw new NotImplementedException();
        }

        public HttpRequestMessage DeleteSmsRequest(int id)
        {
            string action = $"action=refresh&next_page=private/GP/sms_req_live.asp&cmd=Command%3DDelSMS&sms_receive_storage=2&sms_delete_flag=0&sms_delete_idx={id}";

            throw new NotImplementedException();
        }

        public HttpRequestMessage GetAllSmsRequest()
        {
            throw new NotImplementedException();
        }

        public HttpRequestMessage GetInboxRequest()
        {
            throw new NotImplementedException();
        }

        public HttpRequestMessage GetOutboxRequest()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public string GetCookie(HttpResponseMessage response)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetDefaultHeaders()
        {
            return new Dictionary<string, string>
            {
                { "Accept", "*/*" },
                { "Accept-Language", "en-US,en;q=0.9" },
                { "Origin", $"http://{BaseUrl}" },
            };
        }
    }
}
