using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SmsLib;

namespace SmsApi
{
    internal class D100 
    {
        private HttpClient client;
        private HttpClientHandler handler;
        private string sid;
        private string userLevel;

        public D100()
        {
            handler = new HttpClientHandler
            {
                UseCookies = false,
                UseProxy = false,
                AllowAutoRedirect = true
            };
            client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://192.168.1.1")
            };

            client.DefaultRequestHeaders.ExpectContinue = false;
        }

        public async Task<string> Login(string username, string password)
        {
            try
            {
                var formData = new Dictionary<string, string> {
                    { "user_name", $"{username}" },
                    { "user_passwd", $"{password}" },
                    { "ui_CurrentLanguage", "en" }
                };

                var encodedContent = new FormUrlEncodedContent(formData);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Host", "192.168.1.1");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
                client.DefaultRequestHeaders.Add("Origin", "http://192.168.1.1");
                client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36 Edg/135.0.0.0");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                client.DefaultRequestHeaders.Add("Referer", $"http://192.168.1.1/login.asp?{Utils.Time()}");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,fa;q=0.8");

                var response = await client.PostAsync("/cgi-bin/sysconf.cgi?page=login.asp&action=login", encodedContent);

                if (response.IsSuccessStatusCode)
                {
                    if (response.Headers.TryGetValues("Set-Cookie", out var values))
                    {
                        GetSid(values.FirstOrDefault());
                        GetUserLevel(values.LastOrDefault());

                        return "Success";
                    }
                    else
                    {
                        return "Login Failed";
                    }
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
        public async Task<string> GetSmsInbox()
        {
            try
            {
                string requestUri = $"/cgi-bin/sysconf.cgi?page=ajax.asp&action=getlist_sms_inbox&time={Utils.Time()}";

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Host", "192.168.1.1");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("sid", sid);
                client.DefaultRequestHeaders.Add("Referer", "http://192.168.1.1/index2.asp");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

                var response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
        public async Task<string> DeleteSms(int id)
        {
            try
            {
                string requestUri = $"/cgi-bin/sysconf.cgi?page=ajax.asp&action=delete_sms_message&delete_index={id}&sms_read_num=7&sms_unread_num=0&sms_check_page=mobil_sms_inbox&sms_send_num=7&time=${Utils.Time()}&_={Utils.Time()}";

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Host", "192.168.1.1");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("sid", sid);
                client.DefaultRequestHeaders.Add("Referer", "http://192.168.1.1/index2.asp");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9");

                var response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                    return "SMS deletion successful.";
                }
                else
                {
                    return $"Error: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }
        private string GetUserLevel(string cookie)
        {
            string pattern = @"userlevel=([^;]+)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(cookie);

            if (match.Success)
            {
                string userLevelValue = match.Groups[1].Value;
                this.userLevel = userLevelValue;

                return userLevelValue;
            }

            return null;
        }
        private string GetSid(string cookie)
        {
            string pattern = @"sid=([^;]+)";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(cookie);

            if (match.Success)
            {
                string sidValue = match.Groups[1].Value;
                this.sid = sidValue;

                return sidValue;
            }
            
            return null;
        }
    }
}
