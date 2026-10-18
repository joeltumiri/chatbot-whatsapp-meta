using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using WhatsappNet.Api.Models.WhatsappCloud;
using WhatsappNet.Api.Services.WhatsappCloud.SendMessage;
using WhatsappNet.Api.Util;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Whatsapp.Net.Controllers
{
    [ApiController]
    [Route("api/whatsapp")]
    public class WhatsappController : Controller
    {
        private readonly IWhatsappCloudSendMessage _whatsappCloudSendMessage;
        private readonly IUtil _util;
        private readonly IConfiguration _configuration;
      
        public WhatsappController(IWhatsappCloudSendMessage whatsappCloudSendMessage, IUtil util, IConfiguration configuration)
        {
            _whatsappCloudSendMessage = whatsappCloudSendMessage;
            _util = util;
            _configuration = configuration;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Sample()
        {
            var data = new 
            {
                messaging_product = "whatsapp",
                to = "59170667968",
                type = "text",
                text = new
                {
                    body = "Hola Mundo"
                }
            };         

            var result = await _whatsappCloudSendMessage.Execute(data);

            return Ok("ok sample");

        }

        [HttpGet]
        public IActionResult VerifyToken()
        {
            string AccessToken = _configuration["WhatsApp:VerifyToken"] ?? string.Empty;

            var token = Request.Query["hub.verify_token"].ToString();
            var challenge = Request.Query["hub.challenge"].ToString();

            if (!string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(challenge) && token == AccessToken)
            {
                return Ok(challenge);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReceivedMessage([FromBody] WhatsAppCloudModel body)
        {
            try
            {
                var Message = body.Entry[0]?.Changes[0]?.Value?.Messages[0];
                if (Message != null)
                {
                    var userNumber = Message.From;
                    var userText = GetUserText(Message);

                    object objectMessage;

                    if (userText.ToUpper().Contains("HOLA"))
                    {
                        objectMessage = _util.TextMessage("¡Ohayou gozaimasu, querido amo! ¡Bienvenido a la tienda AnimeTalllica! Estoy aquí para asistirte en todo lo que necesites. ¿En qué puedo ayudarte hoy? uWu😘💋", userNumber);
                    }
                    else if (userText.ToUpper().Contains("GRACIAS") || userText.ToUpper().Contains("AGRADECIDO"))
                    {
                        objectMessage = _util.TextMessage("¡Arigatou gozaimasu! Siempre es un placer ayudar a nuestros queridos clientes otakus como tú. Si tienes más preguntas, no dudes en preguntar. Estoy aquí para ti. 😊🎉", userNumber);
                    }
                    else if (userText.ToUpper().Contains("ADIOS") || userText.ToUpper().Contains("YA ME VOY"))
                    {
                        objectMessage = _util.TextMessage("Sayonara, querido amo. Espero haberte brindado un servicio excepcional. Si necesitas algo más, no dudes en volver. ¡Ten un día lleno de anime y felicidad! 🎌🤗", userNumber);
                    }
                    else if (userText.ToUpper().Contains("QUISIERA INFORMACION") || userText.ToUpper().Contains("PRODUCTOS"))
                    {
                        objectMessage = _util.TextMessage("¡Por supuesto, mi otaku favorito! En AnimeTalllica tenemos una amplia variedad de pruductos favoritos. ¿Podrías decirme el nombre del personaje o la serie en particular que estás buscando? ¡Con mucho gusto te mostraré nuestras opciones más increíbles! uWu", userNumber);
                    }
                    else if (userText.ToUpper().Contains("NARUTO"))
                    {
                        objectMessage = _util.TextMessage("¡Oh, un amante de Naruto! ¡Qué gran elección! Déjame mostrarte nuestras increíbles figuras de Naruto Uzumaki. 😍🍥", userNumber);
                        await _whatsappCloudSendMessage.Execute(objectMessage);

                        var pdfMessage = _util.DocumentMessage("https://archivo.ucr.ac.cr/docum/tesis2.pdf", userNumber);
                        await _whatsappCloudSendMessage.Execute(pdfMessage);

                        var flipSnackMessage = _util.TextMessage("Además, puedes echar un vistazo a nuestra pagina de facebook: https://www.facebook.com/animetallica.prj ¡Espero que encuentres la figura perfecta para tu colección de Naruto! uWu💩", userNumber);
                        await _whatsappCloudSendMessage.Execute(flipSnackMessage);

                        return Ok("EVENT_RECEIVED");
                    }
                    else if (userText.ToUpper().Contains("ENVÍO"))
                    {
                        objectMessage = _util.ButtonsMessage(userNumber);
                    }
                    else if (userText.ToUpper().Contains("ESTÁNDAR"))
                    {
                        objectMessage = _util.TextMessage("¡Entendido, amo! Optaste por el envío estándar. Ahora necesitaré tu dirección de envío para completar el proceso. ¿Podrías proporcionarme tu dirección completa, por favor? uWu", userNumber);
                    }
                    else if (userText.ToUpper().Contains("EXPRESS"))
                    {
                        objectMessage = _util.TextMessage("¡Entendido, amo! Optaste por el envío express. Ahora necesitaré tu dirección de envío para completar el proceso. ¿Podrías proporcionarme tu dirección completa, por favor? uWu", userNumber);
                    }
                    else if (userText.ToUpper().Contains("DIRECCIÓN"))
                    {
                        objectMessage = _util.TextMessage("¡Gracias por la información, amo! Ya tengo tu dirección. Ahora procederé a ingresar los detalles de envío y procesar tu pedido. ¡Listo! Tu pedido ha sido confirmado y procesado. Lo recibirás en tu hogar en un plazo de 5 a 7 días hábiles. ¿Hay algo más en lo que pueda ayudarte? uWU", userNumber);
                    }
                    else if (userText.ToUpper().Contains("VISITAR"))
                    {
                        objectMessage = _util.TextMessage("¡Konnichiwa, mi apreciado amo! ¡Gracias por considerar visitar nuestra querida tienda AnimeTalllica! Nuestra dirección es la siguiente: Calle Otaku 123, Ciudad Otaku. Estamos ubicados en el corazón de la ciudad, cerca del famoso Café Neko-chan. ¡Esperamos verte pronto en nuestra tienda llena de magia otaku! uWU", userNumber);
                    }
                    else if (userText.ToUpper().Contains("DESCUENTO"))
                    {
                        objectMessage = _util.TextMessage("¡Ohayou gozaimasu, mi apreciado amo! ¡Qué emoción que estés interesado en nuestros productos en AnimeTalllica! ¡Sí, tenemos descuentos especiales disponibles en este momento! ¿En qué tipo de productos estás interesado? ¡Con gusto te informaré sobre nuestras ofertas más geniales! uWU", userNumber);
                        var discountVideoMessage = _util.VideoMessage("https://www.tiktok.com/@animetallica/video/7020538171544980741?is_from_webapp=1&sender_device=pc&web_id=7220365014086321669", userNumber);
                        await _whatsappCloudSendMessage.Execute(discountVideoMessage);
                    }
                    else
                    {
                        objectMessage = _util.TextMessage("¡Hola! No estoy seguro de entender lo que me dices, amo. Pero no te preocupes, siempre estoy aquí para ayudarte en todo lo que necesites relacionado con AnimeTalllica. ¡No dudes en preguntarme cualquier otra cosa! uWU", userNumber);
                    }

                    await _whatsappCloudSendMessage.Execute(objectMessage);


                }

                return Ok("EVENT_RECEIVED");
            }
            catch (Exception ex)
            {
                return Ok("EVENT_RECEIVED");
            }
        }

        private string GetUserText(Message message)
        {
            string TypeMessage = message.Type;

            if (TypeMessage.ToUpper() == "TEXT")
            {
                return message.Text.Body;
            }
            else if (TypeMessage.ToUpper() == "INTERACTIVE")
            {
                string interactiveType = message.Interactive.Type;

                if (interactiveType.ToUpper() == "LIST_REPLY")
                {
                    return message.Interactive.List_Reply.Title;
                }
                else if (interactiveType.ToUpper() == "BUTTON_REPLY")
                {
                    return message.Interactive.Button_Reply.Title;
                }
                else
                {
                    return string.Empty;
                }

            }
            else
            {
                return string.Empty;
            }
        }
    }

}

