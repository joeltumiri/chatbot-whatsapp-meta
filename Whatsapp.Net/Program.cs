using WhatsappNet.Api.Services.OpenAI.ChatGPT;
using WhatsappNet.Api.Services.WhatsappCloud.SendMessage;
using WhatsappNet.Api.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<IWhatsappCloudSendMessage, WhatsappCloudSendMessage>();
builder.Services.AddSingleton<IUtil, Util>();
builder.Services.AddSingleton<IChatGPTService, ChatGPTService>();

var app = builder.Build();

// Avisa al arrancar si falta alguna clave (se configuran con secretos de usuario o variables de entorno).
string[] requiredKeys = { "WhatsApp:AccessToken", "WhatsApp:PhoneNumberId", "WhatsApp:VerifyToken", "OpenAI:ApiKey" };
foreach (var key in requiredKeys)
{
    if (string.IsNullOrWhiteSpace(app.Configuration[key]))
        app.Logger.LogWarning("Falta la clave de configuración \"{Key}\". Defínela en los secretos de usuario (secrets.json) o como variable de entorno.", key);
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
