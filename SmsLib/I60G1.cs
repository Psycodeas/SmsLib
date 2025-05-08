using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SmsLib
{
    [ComVisible(true)]
    [Guid("61A26512-9737-4C94-A01E-982220CA0AD1")]
    [ProgId("SmsLib.I60")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComDefaultInterface(typeof(IBase))]
    public class I60G1 : IBase
    {
        private Handler smsHandler;

        public I60G1(){}

        ~I60G1()
        {
            smsHandler = null;
        }

        public void SetIp(string url)
        {
            smsHandler = new Handler(new I60G1Request(url));
        }

        public string DeleteSms(int id)
        {
            try
            {
                return smsHandler.DeleteSms(id).GetAwaiter().GetResult().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetAllSms()
        {
            try
            {
                smsHandler.GetAllSms().GetAwaiter().GetResult();
                return "1";
            }
            catch (HttpRequestException ex)
            {
                return "Network Error : " + ex.Message;
            }
            catch (TaskCanceledException ex)
            {
                return "Request Timeout: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Unexpected Error: " + ex.Message;
            }
        }

        public string GetLatestSms()
        {
            try
            {
                string res;

                var sms = smsHandler.Dequeue();

                if (sms != null)
                {
                    res = sms.Phone + "~" + sms.Message;
                }
                else
                {
                    res = "-1";
                }

                return res;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public string Login(string username, string password)
        {
            try
            {
                return smsHandler.Login(username, password).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                return "Network Error : " + ex.Message;
            }
            catch (TaskCanceledException ex)
            {
                return "Request Timeout: " + ex.Message;
            }
            catch (Exception ex)
            {
                return "Unexpected Error: " + ex.Message;
            }
        }

        public string SendSms(string number, string message)
        {
            try
            {
                return smsHandler.SendSms(message , Utils.NormalaizeNumber(number)).GetAwaiter().GetResult().ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Reset()
        {
            try
            {
                return smsHandler.ResetDevice().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
