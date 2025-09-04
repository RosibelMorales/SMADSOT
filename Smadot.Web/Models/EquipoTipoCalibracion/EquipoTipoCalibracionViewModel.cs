using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.EquipoTipoCalibracion.Response;

namespace Smadot.Web.Models.EquipoTipoCalibracion
{
    public class EquipoTipoCalibracionViewModel
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

        public List<SelectListItem>? Calibraciones { get; set; }

        public string? FilesString { get; set; }
        public List<EquipoTipoCalibracionDoc> Files { get; set; }

        public int Total { get; set; }

    }
    public class EquipoTipoCalibracionDoc
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }


}
