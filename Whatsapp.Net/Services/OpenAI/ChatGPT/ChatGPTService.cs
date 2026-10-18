using OpenAI_API;
using OpenAI_API.Completions;

namespace WhatsappNet.Api.Services.OpenAI.ChatGPT
{
    public class ChatGPTService: IChatGPTService
    {
        private readonly IConfiguration _configuration;

        public ChatGPTService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> Execute(string textUser)
        {
            try 
            {
                string apiKey = _configuration["OpenAI:ApiKey"] ?? string.Empty;
                var openAiService = new OpenAIAPI(apiKey);

                var completion = new CompletionRequest
                {
                    Prompt = textUser,
                    Model = "text-davinci-003",
                    NumChoicesPerPrompt = 1,
                    MaxTokens = 200,
                };
                var result = await openAiService.Completions.CreateCompletionAsync(completion);

                if (result != null && result.Completions.Count > 0)
                    return result.Completions[0].Text;

                return "Lo siento, sucedio un problema, intentalo mas tarde.";
            }
            catch(Exception)
            {
                return "Lo siento, sucedio un problema, intentalo mas tarde.";
            }
            
        }
        
    }
}
