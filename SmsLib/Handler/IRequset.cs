using System.Collections.Generic;
using System.Net.Http;

namespace SmsLib
{
    public interface IRequset
    {
        //Default Headers
        void GetDefaultHeaders(HttpClient client);

        //Login
        HttpRequestMessage LoginRequest(string username, string password);

        //Get All
        HttpRequestMessage GetAllSmsRequest();
        //HttpRequestMessage GetInboxRequest();
        //HttpRequestMessage GetOutboxRequest();

        //Send
        HttpRequestMessage SendSms(string message, string destination);

        //Delete
        HttpRequestMessage DeleteSmsRequest(int id);
        //HttpRequestMessage DeleteInboxRequest(int id);
        //HttpRequestMessage DeleteOutboxRequest(int id);

        string SetCookie(HttpResponseMessage response, HttpClient client);
        bool ValidateToken(string responseContent);
        bool IsSmsSent(string responseContent);

        HttpRequestMessage ResetDevice();


    }
}