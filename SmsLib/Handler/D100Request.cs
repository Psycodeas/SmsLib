using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SmsLib.Handler
{
    internal class D100Request : IRequset
    {
        private string BaseAddress;
        D100Request(string baseAddress) 
        {
            this.BaseAddress = baseAddress;
        }
        public HttpRequestMessage DeleteInboxRequest(int id)
        {
            throw new System.NotImplementedException();
        }

        public HttpRequestMessage DeleteOutboxRequest(int id)
        {
            throw new System.NotImplementedException();
        }

        public HttpRequestMessage DeleteSmsRequest(int id)
        {
            throw new System.NotImplementedException();
        }

        public HttpRequestMessage GetAllSmsRequest()
        {
            throw new System.NotImplementedException();
        }

        public HttpRequestMessage GetInboxRequest()
        {
            throw new System.NotImplementedException();
        }

        public HttpRequestMessage GetOutboxRequest()
        {
            throw new System.NotImplementedException();
        }

        public HttpRequestMessage LoginRequest(string username, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"http://{BaseAddress}/cgi-bin/sysconf.cgi?page=login.asp&action=login")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "user_name", username },
                    { "user_passwd", password },
                    { "ui_CurrentLanguage", "en" }
                })
            };

            // Adding headers
            request.Headers.Add("Host", BaseAddress);
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Cache-Control", "max-age=0");
            request.Headers.Add("Origin", $"http://{BaseAddress}");
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Referer", $"http://{BaseAddress}/login.asp");
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.9");

            return request;
        }

        public HttpRequestMessage SendSms(string message, string destination)
        {
            throw new System.NotImplementedException();
        }

        public string GetCookie(HttpResponseMessage response)
        {
            if (response.Headers.TryGetValues("Set-Cookie", out var values))
            {
                string pattern = @"sid=([^;]+)";
                Regex regex = new Regex(pattern);
                Match match = regex.Match(values.FirstOrDefault());

                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
