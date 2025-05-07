using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SmsLib
{
    [Guid("75C2A623-6169-4B95-96F0-73642C514C50")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [ComVisible(true)]
    public interface IBase
    {
        [DispId(1)]
        string Login(string username, string password);

        [DispId(2)]
        string GetAllSms();

        [DispId(3)]
        string GetLatestSms();

        [DispId(4)]
        string SendSms(string number, string message);

        [DispId(5)]
        string DeleteSms(int id);

        [DispId(6)]
        string Greeting();

        [DispId(7)]
        void SetIp(string url);
    }
}