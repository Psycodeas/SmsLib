using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SmsLib
{
    [ComVisible(false)]
    internal class SmsHandler
    {
        HttpClient client;
        HttpClientHandler handler;
        ConcurrentQueue<Sms> inboxQueue;
        const string CONTENT_TYPE = "application/x-www-form-urlencoded";
        IModemProps props;
        public string BaseUrl { get; set; } = "192.168.1.1";
        private string username = "admin", password = "admin";

        public SmsHandler(IModemProps props)
        {
            this.props = props;
            inboxQueue = new ConcurrentQueue<Sms>();

            handler = new HttpClientHandler
            {
                UseCookies = false,
            };

            client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromSeconds(10);

            var headers = new Dictionary<string, string>
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
        ~SmsHandler()
        {
            client = null;
            handler = null;
            inboxQueue = null;
        }
        public async Task<string> Login(string username, string password)
        {
            if (this.username == null || this.password == null)
            {
                this.username = username;
                this.password = password;
            }
            string data = props.LoginCommand(username, password);
            try
            {
                HttpContent content = new StringContent(data, Encoding.UTF8, CONTENT_TYPE);
                HttpResponseMessage response = await client.PostAsync($"http://{BaseUrl}/{props.LoginUrl}", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
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
                else
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
                throw ex;
            }
        }
        public async Task<List<Sms>> GetAllSms()
        {
            List<Sms> result = new List<Sms>();
            string command = props.GetAllSmsCommand();
            HttpContent content = new StringContent(command, Encoding.UTF8, CONTENT_TYPE);

            try
            {
                HttpResponseMessage response = await client.PostAsync($"http://{BaseUrl}/{props.GetAllSmsUrl}", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (Utils.ValidateToken(responseContent))
                    {
                        try
                        {
                            List<Sms> list = Sms.SmsSerializerI60G1(responseContent);

                            foreach (var sms in list)
                            {
                                result.Add(sms);
                            }

                            SetAllSms(result);

                            return result;
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine("XML Deserialization Error: " + ex.Message);
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Parsing Error: " + ex.Message);
                            throw ex;
                        }

                    }
                    else
                    {
                        await Login(username, password);
                        return await GetAllSms();
                    }

                }
                else
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network Error: " + ex.Message);
                throw ex;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Request Timeout: " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
                throw ex;
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
        public async Task<int> DeleteSms(int id)
        {
            string command = props.DeleteSmsCommand(id);
            HttpContent content = new StringContent(command, Encoding.UTF8, CONTENT_TYPE);

            try
            {
                HttpResponseMessage response = await client.PostAsync($"http://{BaseUrl}/{props.DeleteSmsUrl}", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (Utils.ValidateToken(responseContent))
                    {
                        return 1;
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
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network Error: " + ex.Message);
                throw ex;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Request Timeout: " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
                throw ex;
            }
        }
        public async Task<int> SendSms(string number, string message)
        {
            string command = props.SendSmsCommand(number, message);
            HttpContent content = new StringContent(command, Encoding.UTF8, CONTENT_TYPE);

            try
            {

                HttpResponseMessage response = await client.PostAsync($"http://{BaseUrl}/{props.SendSmsUrl}", content);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    if (Utils.ValidateToken(responseContent))
                    {
                        try
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(responseContent);

                            XmlNode resultNode = doc.SelectSingleNode("//SMS_SUBMIT_RSP/Result");

                            string result = resultNode?.InnerText ?? "-1";
                            if (result == "-1")
                            {
                                return 0;
                            }
                            else if (result == "0")
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                        catch (InvalidOperationException ex)
                        {
                            Console.WriteLine("XML Deserialization Error: " + ex.Message);
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Parsing Error: " + ex.Message);
                            throw ex;
                        }
                    }
                    else
                    {
                        await Login(username, password);
                        return await SendSms(number, message);
                    }

                }
                else
                {
                    throw new HttpRequestException(response.StatusCode.ToString());
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("Network Error: " + ex.Message);
                throw ex;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Request Timeout: " + ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
                throw ex;
            }
        }
        public void SetBaseUrl(string url)
        {
            BaseUrl = url;
        }
    }
}
