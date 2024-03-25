using MechAppBackend.Conns;

namespace MechAppBackend.Helpers
{
    public class smssender
    {
        public static async void SendSms(string _message, string _to)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smsApiToken) || string.IsNullOrEmpty(connections.smsApiSender)) return;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.smsapi.pl/sms.do");
                request.Headers.Add("Authorization", connections.smsApiToken);
                var content = new StringContent("{\r\n    \"from\": \"" + connections.smsApiSender + "\",\r\n    \"to\": \"" + _to + "\",\r\n    \"message\": \"" + _message + "\"\r\n}", null, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("MECHAPP", "SMSSENDER", "SENDSMS", ex);
            }
        }
    }
}
