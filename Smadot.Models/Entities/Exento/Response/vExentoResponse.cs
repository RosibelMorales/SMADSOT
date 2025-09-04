using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Exento.Response
{
    public class vExentoResponse
    {
        public long Id { get; set; }

        public string? Placa { get; set; }

        public string? Serie { get; set; }

        public string? Marca { get; set; }

        public string? Submarca { get; set; }
        public bool? Permanente { get; set; }

        public int Modelo { get; set; }

        public int IdCatTipoCertificado { get; set; }

        public string? Nombre { get; set; }
        public long Folio { get; set; }
        public string FolioStr
        {
            get
            {
                return Folio.ToString("000000000");
            }
        }

        public DateTime Vigencia { get; set; }
        public DateTime Fecha { get; set; }
        public int Total { get; set; }
        public long? IdRefrendo { get; set; }
        public long? IdFoliosFormaValoradaVerificentro { get; set; }
        public long? RefrendoIdFoliosFormaValoradaVerificentro { get; set; }

        public string? ClaveTramite { get; set; }
    }

    public class vExentoResponseGrid : vExentoResponse
    {
        public string Acciones { get; set; }
    }
}
