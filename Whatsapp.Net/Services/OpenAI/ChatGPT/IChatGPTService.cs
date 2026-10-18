namespace WhatsappNet.Api.Services.OpenAI.ChatGPT
{
    public interface IChatGPTService
    {
        Task<string> Execute(string textUser);
    }
}
