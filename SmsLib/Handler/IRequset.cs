using System.Collections.Generic;
using System.Net.Http;

namespace SmsLib.Handler
{
    internal interface IRequset
    {
        //Default Headers
        Dictionary<string, string> GetDefaultHeaders();

        //Login
        HttpRequestMessage LoginRequest(string username, string password);

        //Get All
        HttpRequestMessage GetAllSmsRequest();
        HttpRequestMessage GetInboxRequest();
        HttpRequestMessage GetOutboxRequest();

        //Send
        HttpRequestMessage SendSms(string message, string destination);

        //Delete
        HttpRequestMessage DeleteSmsRequest(int id);
        HttpRequestMessage DeleteInboxRequest(int id);
        HttpRequestMessage DeleteOutboxRequest(int id);
        string GetCookie(HttpResponseMessage response);
    }
}