using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml;

namespace SmsLib
{
    [ComVisible(false)]
    public class Sms
    {
        public Sms() { }
        public Sms(int id, string type, string message, string phone, string time)
        {
            Id = id;
            Type = type;
            Message = message;
            Phone = phone;
            Time = time;
        }

        public int Id { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string Phone { get; set; }
        public string Time { get; set; }

        public static List<Sms> SmsSerializerI60G1(string xmlContent)
        {
            List<Sms> list = new List<Sms>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                XmlNodeList smsNodes = xmlDoc.SelectNodes("//SMS_READ");

                foreach (XmlNode smsNode in smsNodes)
                {
                    list.Add(new Sms(
                        id: int.Parse(smsNode["IDX"]?.InnerText),
                        type: smsNode["Type"]?.InnerText,
                        message: smsNode["Message"]?.InnerText,
                        phone: smsNode["Direction"]?.InnerText,
                        time: smsNode["Time"]?.InnerText
                    ));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }


            return list;
        }
        public static List<Sms> SmsSerializerD100(string response, string type)
        {
            List<Sms> list = new List<Sms>();
            var messages = response.Split('\t');
            messages = messages[1].Split(',');
            int count = (messages.Length - 1) / 5;

            for (int i = 0; i < count; i++)
            {
                list.Add(new Sms(
                    id: int.Parse(messages[i * 5]),
                    time: messages[i * 5 + 2],
                    phone: messages[i * 5 + 3],
                    message: messages[i * 5 + 4],
                    type: type
                ));
            }

            return list;
        }

    }
}
