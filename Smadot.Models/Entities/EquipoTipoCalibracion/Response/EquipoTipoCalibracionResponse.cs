using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Smadot.Models.Entities.EquipoTipoCalibracion.Response
{
    public class EquipoTipoCalibracionResponse
    {
        public long Id { get; set; }

        public long IdEquipo { get; set; }

        public int IdCatTipoCalibracion { get; set; }

        public string? UrlDocumento1 { get; set; }

        public DateTime PrimeraFechaCalibracion { get; set; }

        public string? Nota { get; set; }

        public long IdUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long? IdUserValido { get; set; }

        public List<TipoCalibracionResponse>? Calibraciones { get; set; }
        public string? FilesString { get; set; }

        public List<EquipoTipoCalibracionFile>? Files { get; set; }
    }

    public class EquipoTipoCalibracionFile
    {
        public string? Base64 { get; set; }

        public string? Nombre { get; set; }

        public string? Tipo { get; set; }

    }
}

