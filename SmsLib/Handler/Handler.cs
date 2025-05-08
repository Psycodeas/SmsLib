using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmsLib
{
    public class Handler
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

            request.GetDefaultHeaders(client);


            client.Timeout = TimeSpan.FromSeconds(10);
        }

        ~Handler()
        {
            client = null;
            httpHandler = null;
            inboxQueue = null;
            request = null;
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
                    return request.SetCookie(response, client);
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
                    
                    SetAllSms(list);

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
        
        public Sms Dequeue()
        {
            if (inboxQueue.TryDequeue(out Sms result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<string> SendSms(string message, string phone)
        {
            try
            {
                HttpResponseMessage response = await client.SendAsync(request.SendSms(message,phone));

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (request.ValidateToken(responseContent))
                    {
                        if(request.IsSmsSent(responseContent))
                        {
                            return "1";
                        }
                        else
                        {
                            return "0";
                        }
                    }
                    else
                    {
                        await Login(username, password);
                        return await SendSms(message, phone);
                    }
                }
                else
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }
        public async Task<string> DeleteSms(int id)
        {
            try
            {
                HttpResponseMessage response = await client.SendAsync(request.DeleteSmsRequest(id));

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (request.ValidateToken(responseContent))
                    {
                        return "1";
                    }
                    else
                    {
                        await Login(username, password);
                        return await DeleteSms(id);
                    }
                }
                else
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public async Task<string> ResetDevice()
        {
            try
            {
                HttpResponseMessage response = await client.SendAsync(request.ResetDevice());

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (request.ValidateToken(responseContent))
                    {
                        return "1";
                    }
                    else
                    {
                        await Login(username, password);
                        return await ResetDevice();
                    }
                }
                else
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
