using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SmsLib.Handler
{
    internal class Handler
    {
        HttpClient client;
        HttpClientHandler httpHandler;
        ConcurrentQueue<Sms> inboxQueue;
        IRequset request;

        private string username, password;

        public Handler(IRequset requset)
        {
            this.request = requset;
            inboxQueue = new ConcurrentQueue<Sms>();
            httpHandler = new HttpClientHandler
            {
                UseCookies = false,
                UseProxy = false,
                AllowAutoRedirect = true
            };
            client = new HttpClient(httpHandler);
            client.DefaultRequestHeaders.ExpectContinue = false;

            var headers = request.GetDefaultHeaders();
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            client.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<string> Login(string username, string password)
        {
            if (this.username == null || this.password == null)
            {
                this.username = username;
                this.password = password;
            }

            try
            {
                var response = await client.SendAsync(request.LoginRequest(username,password));

                if (response.IsSuccessStatusCode) 
                {
                    var cookie = request.GetCookie(response);

                    return "Success";
                }
                else
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<List<Sms>> GetAllSms()
        {
            List<Sms> list = new List<Sms>();

            try
            {
                var response = await client.SendAsync(request.GetAllSmsRequest());

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (request is I60G1Request)
                    {
                        list = Sms.SmsSerializerI60G1(content);
                    } else if (request is D100Request)
                    {
                        list = Sms.SmsSerializerD100(content, "D");
                    }
                    
                    return list;
                }
                else
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void SetAllSms(List<Sms> smsDatas)
        {
            foreach (var sms in smsDatas)
            {
                if (sms.Type.StartsWith("D"))
                {
                    if (!inboxQueue.Any(existingSms => existingSms.Id == sms.Id))
                    {
                        inboxQueue.Enqueue(sms);
                    }
                }

                Task.Run(async () =>
                {
                    await DeleteSms(sms.Id);
                });
            }
        }


        public async Task<int> DeleteSms(int id)
        {
            d
        }
    }
}
