using Smadot.Models.Dicts;

namespace Smadot.Models.Entities.Equipo.Response
{
    public class vEquipoResponse
    {
        public long? Id { get; set; }

        public string? NombreEquipo { get; set; }

        public string? NumeroSerie { get; set; }

        public string? UrlFactura { get; set; }

        public string? UrlManualUsuario { get; set; }

        public string? UrlEspecificacionTecnica { get; set; }

        public string? UrlRecomendacionServicio { get; set; }

        public int? IdCatEstatusEquipo { get; set; }

        public string? NombreEstatusEquipo { get; set; }

        public long? IdUserRegistro { get; set; }

        public string? Nombre { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public long? IdUserReviso { get; set; }

        public string? Comentarios { get; set; }

        public long? IdLinea { get; set; }

        public string? NombreLinea { get; set; }

        public long? IdVerificentro { get; set; }

        public string? NombreVerificentro { get; set; }

        public string Estatus { get; set; } = null!;

        public DateTime? FechaProximaCalibracion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public int IdCatTipoEquipo { get; set; }

        public int Total { get; set; }
    }

    public class vEquipoResponseGrid : vEquipoResponse
    {
        public string Acciones { get; set; }

        public string NombreLineaString
            => NombreLinea is not null ? NombreLinea : "";

         public string EstatusColor
        {
            get
            {
                string estatus = CatColoresDic.Nombres[CatColoresDic.Verde];

                if (FechaProximaCalibracion.HasValue && TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)")) > FechaProximaCalibracion || IdCatEstatusEquipo == EstatusEquipo.SinCalibrar)
                {
                    estatus = CatColoresDic.Nombres[CatColoresDic.Rojo];
                }
                else if (IdCatEstatusEquipo == EstatusEquipo.Inactivo)
                {
                    estatus = CatColoresDic.Nombres[CatColoresDic.Gris];
                }
                else if (IdCatEstatusEquipo == EstatusEquipo.DocumentacionInvalida)
                {
                    estatus = CatColoresDic.Nombres[CatColoresDic.Naranja];
                }
                return estatus;
            }
        }
    }

}
