using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WhatsappNet.Api.Services.WhatsappCloud.SendMessage
{
    public class WhatsappCloudSendMessage : IWhatsappCloudSendMessage
    {
        private readonly IConfiguration _configuration;

        public WhatsappCloudSendMessage(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> Execute(object model)
        {
            var client = new HttpClient();
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));
            //contiene los datos y asegura que los recursos utilizados por este objeto se liberen automáticamente cuando ya no sean necesarios
            using (var content = new ByteArrayContent(byteData))
            {
                string endpoint = "https://graph.facebook.com";
                string phoneNumberId = _configuration["WhatsApp:PhoneNumberId"] ?? string.Empty;
                string accessToken = _configuration["WhatsApp:AccessToken"] ?? string.Empty;
                string uri = $"{endpoint}/v17.0/{phoneNumberId}/messages";


                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                var response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;

            }
        }
    }
}
