namespace Smadot.Utilities.Modelos.Documentos
{
    public class DevolucionSPFDocument
    {
        public string? UrlRoot { get; set; }

        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? NombreC { get; set; }
        public string? NombreEncargado { get; set; }
        public string? Folio { get; set; }
        public int? Cantidad { get; set; }


        public long NumeroDevolucion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string UsuarioAprobo { get; set; }
        public string ResponsableEntrega { get; set; }
        public string RecibioSPF { get; set; }
        public long NumeroSolicitud { get; set; }

        public string FolioCertificado { get; set; }

        public List<vDevolucion> Devoluciones { get; set; }
    }

    public class vDevolucion
    {
        public long NumeroDevolucion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string UsuarioAprobo { get; set; }
        public string ResponsableEntrega { get; set; }
        public string RecibioSPF { get; set; }
        public long NumeroSolicitud { get; set; }

        public string FolioCertificado { get; set; }

        
    }
    public class PersonaCita
    {
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public DateTime Fecha { get; set; }

        
    }
}
