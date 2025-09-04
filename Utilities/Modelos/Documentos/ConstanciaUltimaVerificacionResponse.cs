using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos.Documentos
{
    public class ConstanciaUltimaVerificacionResponse
    {
        public long Id { get; set; }

        public long? IdVerificacion { get; set; }

        public string NumeroReferencia { get; set; } = null!;

        public string? UrlDoc1 { get; set; }

        public string? UrlDoc2 { get; set; }

        public string? UrlDoc3 { get; set; }

        public DateTime FechaCartaFactura { get; set; }

        public DateTime VigenciaHoloAnterior { get; set; }

        public string Placa { get; set; } = null!;

        public string Propietario { get; set; } = null!;

        public DateTime FechaRegistro { get; set; }

        public long IdUserRegistro { get; set; }

        public string? Nombre { get; set; }

        public long? IdFormaValorada { get; set; }

        public DateTime Fecha { get; set; }

        public string PlacaVerificacion { get; set; } = null!;

        public string Serie { get; set; } = null!;

        public string FolioCertificado { get; set; } = null!;

        public DateTime Vigencia { get; set; }

        public string Marca { get; set; } = null!;

        public string Modelo { get; set; }
        public string ClaveTramite { get; set; }

        public int Anio { get; set; }

        public string Semestre
        {
            get
            {
                var fechaActual = Fecha;
                var semestre1FechaInicio = new DateTime(fechaActual.Year, 1, 1);
                var semestre1FechaFinal = new DateTime(fechaActual.Year, 6, 30, 23, 59, 59);
                var semestre = $"Segundo Semestre {fechaActual.Year}";
                // Validamos que la sesión tenga el semestre correcti
                if (fechaActual >= semestre1FechaInicio && fechaActual <= semestre1FechaFinal)
                {
                    semestre = $"Primer Semestre {fechaActual.Year}";

                }
                return semestre;

            }
        }

        public string ResultadosPrueba { get; set; } = null!;

        public string TipoCertificado { get; set; } = null!;

        public string Combustible { get; set; } = null!;

        public string TarjetaCirculacion { get; set; } = null!;

        public string NombreC { get; set; }

        public string NombreEncargado { get; set; }

        public string Telefono { get; set; }

        public string Direccion { get; set; }

        public string Folio { get; set; }

        public string? UrlRoot { get; set; }
        public List<vVerificacionCertificado> verificaciones { get; set; }
    }

    public partial class vVerificacionCertificado
    {

        public long Id { get; set; }

        public long? IdFormaValorada { get; set; }

        public DateTime Fecha { get; set; }

        public string Placa { get; set; } = null!;

        public string Serie { get; set; } = null!;

        public string FolioCertificado { get; set; } = null!;

        public DateTime Vigencia { get; set; }

        public string ResultadosPrueba { get; set; } = null!;

        public string Marca { get; set; } = null!;

        public string Modelo { get; set; }

        public int Anio { get; set; }

        public string Semestre { get; set; }

        public string TipoCertificado { get; set; } = null!;

        public string Combustible { get; set; } = null!;

        public string TarjetaCirculacion { get; set; } = null!;
    }
}
