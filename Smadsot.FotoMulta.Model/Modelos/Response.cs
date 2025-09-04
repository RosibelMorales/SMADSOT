using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Smadsot.FotoMulta.Model.Modelos
{
    [DataContract]
    public class Response
    {
        public Response()
        {
            this.Status = ResponseStatus.Success;
        }
        public Response(ResponseStatus status, string mensaje, string respuesta)
        {
            this.Status = status;
            this.mensaje = mensaje;
            this.respuesta = respuesta;

        }

        public Response(Exception currentException)
        {
            this.CurrentException = currentException.ToString();
            this.Status = ResponseStatus.Failed;
        }

        public Response(string currentException)
        {
            this.CurrentException = currentException;
            this.Status = ResponseStatus.Failed;
        }

        public Response(string format, params object[] args)
        {
            this.CurrentException = string.Format(format, args);
            this.Status = ResponseStatus.Failed;
        }

        [DataMember]
        public ResponseStatus Status { get; set; }
        [DataMember]
        public string CurrentException { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public string respuesta { get; set; }
        [DataMember]
        public bool AccesoTotalVerificentros { get; set; }
    }
}
