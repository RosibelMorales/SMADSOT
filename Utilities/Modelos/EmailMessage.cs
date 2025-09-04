using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            ToAddresses = new List<EmailAddress>();
            CcAddresses = new List<EmailAddress>();
            FromAddresses = new List<EmailAddress>();
        }

        public EmailMessage(string body, string textoBoton, string enlaceBoton, string contenido, string asunto,
            List<EmailAddress> destinatarios, List<EmailAddress> remitente)
        {
            this.Body = body;
            this.Boton = textoBoton;
            this.Enlace = enlaceBoton;
            this.Content = contenido;
            this.Subject = asunto;
            this.ToAddresses = destinatarios;

            this.CcAddresses = new List<EmailAddress>();
            this.FromAddresses = remitente;
        }

        public List<EmailAddress> ToAddresses { get; set; }
        public List<EmailAddress> CcAddresses { get; set; }
        public List<EmailAddress> FromAddresses { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Content { get; set; }

        public string Boton { get; set; }
        public string Enlace { get; set; }
    }
}
