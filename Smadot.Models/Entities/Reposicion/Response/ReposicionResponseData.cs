using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Reposicion.Response
{
    public class ReposicionResponseData
    {
        public class ReposicionResponse
        {
            public long? IdReposicion { get; set; }
            public long? IdVerificacion { get; set; }
            public string? FechaRegistro { get; set; }
            public string? NumeroReferencia { get; set; }
            public string? UsuarioRegistro { get; set; }
            public string? Placa { get; set; }
            public string? Serie { get; set; }
            public long? IdFoliosFormaValoradaVerificentro { get; set; }
            public int Total { get; set; }

            public long? Folio { get; set; }
            public string? ClaveTramite { get; set; }
        }

        public class ReposicionResponseGrid : ReposicionResponse
        {
            public string Acciones { get; set; }
        }

        public class VerificacionDataResponse
        {

        }
    }
}
