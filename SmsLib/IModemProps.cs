using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsLib
{
    internal interface IModemProps
    {
        string LoginUrl { get; }
        string GetAllSmsUrl {  get; }
        string SendSmsUrl {  get; }
        string DeleteSmsUrl {  get; }

        string LoginCommand(string username, string password);
        string GetAllSmsCommand();
        string SendSmsCommand(string number, string message); 
        string DeleteSmsCommand(int id);
    }
}
