namespace Smadot.Models.Entities.OrdenServicio.Response
{
    public class OrdenServicioResponseData
    {
        public class OrdenServicioResponse
        {
            public long? Id { get; set; }
            public string? Verificentro { get; set; }
            public string? Equipo { get; set; }
            public string? Linea { get; set; }
            public string? FechaRegistro { get; set; }
            public long? FolioServicio { get; set; }
            public string? UserRegistro { get; set; }
            public string? Solución { get; set; }
            public string? UrlDocumento { get; set; }
            public string? CatTipoOrden { get; set; }
            public string? NoSerieActual { get; set; }
            public string? NoSerieAnterior { get; set; }
            public string? Motivo { get; set; }
            public int Total { get; set; }
        }
        public class OrdenServicioResponseGrid : OrdenServicioResponse
        {
            public string Acciones { get; set; }
        }
    }
}
