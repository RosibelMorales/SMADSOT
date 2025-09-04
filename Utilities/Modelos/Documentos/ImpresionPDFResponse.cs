using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{
    public class ImpresionPDFResponse
    {
        public long Folio { get; set; }
        public string Marca { get; set; }
        public string Anio { get; set; }
        public string Placa { get; set; }
        public string Holograma { get; set; }
        public string Submarca { get; set; }
        public string Vigencia { get; set; }
        public DateTime VigenciaFecha { get; set; }
        public string Centro { get; set; }
        public string Linea { get; set; }
        public string Equipo { get; set; }
        public string FolioAnterior { get; set; }
        public string Fecha { get; set; }
        public string HoraInicio { get; set; }
        public string HoraFin { get; set; }
        public string Nombre { get; set; }
        public string Semestre { get; set; }
        public string Combustible { get; set; }
        public string Version { get; set; }
        public string NombreUsuario { get; set; }
        public string TecnicoCapturaNumero { get; set; }
        public string TecnicoCapturaNombre { get; set; }
        public string TecnicoPruebaNumero { get; set; }
        public string TecnicoPruebaNombre { get; set; }
        public string AprobadoPor { get; set; }
        public string? UrlExpediente { get; set; }
        public string? LeyendaCNA { get; set; }
        public List<ImpresionPDFEmisionResponse> Emisiones { get; set; }
        public string? Entidad { get; set; }
        public string NumSerie { get; set; }
        public int IdCatTipoTramite { get; set; }
    }
    public class ImpresionPDFEmisionResponse
    {
        public string Nombre { get; set; }
        public string PrimeraColumna { get; set; }
        public string SegundaColumna { get; set; }
        public string TerceraColumna { get; set; }
    }
}
