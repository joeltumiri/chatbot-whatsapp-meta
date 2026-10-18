using WhatsappNet.Api.Models.WhatsappCloud;

namespace WhatsappNet.Api.Util
{
    public class Util: IUtil
    {
        public object TextMessage(string message, string number)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = number,
                type = "text",
                text = new
                {
                    body = message
                }
            };
        }
        public object GifMessage(string gifUrl, string recipient)
        {
            var gifMessage = new
            {
                messaging_product = "whatsapp",
                to = recipient,
                type = "gif",
                gif = new
                {
                    link = gifUrl
                }
            };

            return gifMessage;
        }

        public object ImageMessage(string url, string number)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = number,
                type = "image",
                text = new
                {
                  link = url
                }
            };
        }

        public object AudioMessage(string url, string number)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = number,
                type = "audio",
                audio = new
                {
                    link = url
                }
            };
        }
        public object VideoMessage(string url, string number)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = number,
                type = "video",
                video = new
                {
                    link = url
                }
            };
        }

        public object DocumentMessage(string url, string number)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = number,
                type = "document",
                document = new
                {
                    link = url
                }
            };
        }

        public object LocationMessage(string number)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = number,
                type = "location",
                location = new
                {
                    latitude = "-16.508622255393146",
                    longitude = "-68.16637680863494",
                    name = "Universidad Unifranz El Alto",
                    address = "Av. del Aeropuerto Internacional El Alto, N° 1015, El Alto"
                }
            };
        }

        public object ButtonsMessage(string number)
        {
            return new
            {
                messaging_product = "whatsapp",
                to = number,
                type = "interactive",
                interactive = new
                {
                  type = "button",
                  body = new
                  {
                      text = "Selecciona una opcion"
                  },
                  action = new
                  {
                      buttons = new List<object>
                      {
                          new
                          {
                              type = "reply",
                              reply = new
                              {
                                  id = "01",
                                  title = "Comprar"
                              }
                          },
                           new
                          {
                              type = "reply",
                              reply = new
                              {
                                  id = "02",
                                  title = "Vender"
                              }
                          }
                      }
                  }
                }
            };
        }
    }
}
