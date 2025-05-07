using System;

namespace SmsLib
{
    internal class I60G1Props : IModemProps
    {
        public string LoginUrl { get => "../login.cgi"; }
        public string GetAllSmsUrl { get => "apply.cgi"; }
        public string SendSmsUrl { get => "apply.cgi"; }
        public string DeleteSmsUrl { get => "apply.cgi"; }

        public string DeleteSmsCommand(int id)
        {
            return $"action=refresh&next_page=private/GP/sms_req_live.asp&cmd=Command%3DDelSMS&sms_receive_storage=2&sms_delete_flag=0&sms_delete_idx={id}";
        }

        public string GetAllSmsCommand()
        {
            return $"action=refresh&next_page=private%2FGP%2Fsms_req_live.asp" +
                $"&cmd=Command%3DGetSMSInfoAll&sms_receive_format=1" +
                $"&sms_receive_storage=2&sms_receive_filter=6" +
                $"&T={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        }

        public string LoginCommand(string username,string password)
        {
            return $"submit_button=login&" +
                          $"submit_type=do_login&change_action=gozila_cgi&" +
                          $"password=&username={username}&passwd={password}";
        }

        public string SendSmsCommand(string number, string message)
        {
            return $"action=refresh&next_page=private%2FGP%2Fsms_req_live.asp" +
                $"&cmd=Command%3DSendSMS&sms_receive_format=1&sms_receive_storage=2" +
                $"&sms_receive_filter=6&sms_to={number}&sms_message={message}" +
                $"&sms_send_format=1&sms_dcs_value=0&sms_send_priority=0" +
                $"&sms_send_privacy=0&sms_reply_user=0&sms_reply_delivery=0" +
                $"&sms_reply_read=0&sms_reply_report=0&sms_send_alert=0" +
                $"&T={DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        }
    }
}
