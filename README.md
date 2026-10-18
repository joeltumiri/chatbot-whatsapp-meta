# Whatsapp.Net

Chatbot de WhatsApp construido con ASP.NET Core. Recibe los mensajes de los usuarios a través del webhook de la **WhatsApp Cloud API** de Meta y responde automáticamente con texto, botones, videos y documentos, según las palabras clave del mensaje (saludos, productos, envíos, descuentos, etc.).

El proyecto incluye también un servicio de **ChatGPT** (`ChatGPTService`) que envía el texto del usuario a la API de OpenAI y devuelve la respuesta generada. El servicio está registrado en `Program.cs`, pero **todavía no se llama desde el controlador**: hoy las respuestas salen solo de las reglas por palabra clave.

## Tecnologías

- C# / .NET 7
- ASP.NET Core (Web API)
- WhatsApp Cloud API (Meta Graph API v17.0)
- OpenAI API (paquete `OpenAI` 1.7.2)
- Newtonsoft.Json

## Cómo funciona

1. Meta verifica el webhook con `GET /api/whatsapp`. `WhatsappController` compara `hub.verify_token` con `WhatsApp:VerifyToken` y, si coinciden, devuelve `hub.challenge`.
2. Cada mensaje entrante llega por `POST /api/whatsapp` y se deserializa en `WhatsAppCloudModel`.
3. El controlador obtiene el número del remitente y el texto (mensaje de texto, o título de un botón o lista interactiva).
4. Según las palabras clave, `Util` construye el mensaje de respuesta (texto, botones, video, documento…).
5. `WhatsappCloudSendMessage` envía ese mensaje con un `POST` a `https://graph.facebook.com/v17.0/{PhoneNumberId}/messages`, usando `WhatsApp:AccessToken` como token Bearer.
6. El controlador siempre responde `200 OK` (`EVENT_RECEIVED`) para que Meta no reintente el envío.

## Configuración

La aplicación necesita estas claves. No las guardes en `appsettings.json` ni las subas al repositorio:

| Clave                    | Descripción                                    |
| ------------------------ | ---------------------------------------------- |
| `WhatsApp:AccessToken`   | Token de acceso de la app de Meta              |
| `WhatsApp:PhoneNumberId` | ID del número de teléfono de WhatsApp Business |
| `WhatsApp:VerifyToken`   | Texto que tú eliges para verificar el webhook  |
| `OpenAI:ApiKey`          | Clave de la API de OpenAI                      |

**Opción 1: dotnet user-secrets** (desarrollo local)

```bash
cd Whatsapp.Net
dotnet user-secrets set "WhatsApp:AccessToken" "<tu-access-token>"
dotnet user-secrets set "WhatsApp:PhoneNumberId" "<tu-phone-number-id>"
dotnet user-secrets set "WhatsApp:VerifyToken" "<tu-verify-token>"
dotnet user-secrets set "OpenAI:ApiKey" "<tu-api-key>"
```

**Opción 2: variables de entorno** (usa `__` en lugar de `:`)

```bash
WhatsApp__AccessToken=<tu-access-token>
WhatsApp__PhoneNumberId=<tu-phone-number-id>
WhatsApp__VerifyToken=<tu-verify-token>
OpenAI__ApiKey=<tu-api-key>
```

Al arrancar, la aplicación muestra una advertencia en el log por cada clave que falte.

## Cómo probarlo

1. **Ejecuta la API** (perfil HTTP, puerto 5098):

   ```bash
   dotnet run --project Whatsapp.Net --launch-profile http
   ```

2. **Expón el puerto con ngrok**:

   ```bash
   ngrok http 5098
   ```

3. **Registra el webhook en Meta**: en tu app de [Meta for Developers](https://developers.facebook.com/), ve a _WhatsApp → Configuración_ y define:
   - URL de devolución de llamada: `https://<tu-subdominio>.ngrok-free.app/api/whatsapp`
   - Token de verificación: el mismo valor de `WhatsApp:VerifyToken`
   - Suscríbete al campo `messages`.

4. **Usa el número de prueba de Meta**: en _WhatsApp → Configuración de la API_, agrega tu teléfono como destinatario de prueba y envía un mensaje (por ejemplo, "Hola") al número de prueba. El bot debería responder.
