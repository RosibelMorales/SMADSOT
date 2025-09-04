using Newtonsoft.Json;
using Smadot.Utilities.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Usuarios.Request
{
    public class UsuariosListRequest : RequestList
    {
        [JsonProperty("siguienteFolio")]
        public bool? SiguienteFolio { get; set; }
    }

    public class UsuarioActivarDesRequest
    {
        public long Id { get; set; }
    }
    public class UpdatePasswordRequest
    {
        public long Id { get; set; }
    }
}
