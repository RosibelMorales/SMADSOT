using Smadot.Models.Entities.Equipo.Response;
using Smadot.Models.DataBase;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Smadot.Models.Entities.Linea.Response;

namespace Smadot.Web.Models.Equipo
{
    public class EquipoViewModel
    {
        public long Id { get; set; }

        public string? Nombre { get; set; } 

        public string? NumeroSerie { get; set; } 

        public int IdCatEstatusEquipo { get; set; }

        public long IdUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long? IdUserReviso { get; set; }

        public string? Comentarios { get; set; }

        public long? IdLinea { get; set; }

        public long IdVerificentro { get; set; }

        public string? FilesString { get; set; }

        public List<EquipoFiles> Files { get; set; }

        public List<SelectListItem> Lineas { get; set; }
    }

    public class EquipoFiles
    {
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string Base64 { get; set; }
    }

    public class AuxValidarResponse
    {
        public long Id { get; set; }
    }
}
