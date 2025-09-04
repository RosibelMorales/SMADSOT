using Smadot.Models.Dicts;

namespace Smadot.Models.Entities.Calibracion.Response
{
    public class vCalibracionResponse
    {
        public long Id { get; set; }

        public long IdEquipoTipoCalibracion { get; set; }

        public long IdEquipo { get; set; }

        public string? NombreEquipo { get; set; } 

        public int Consecutivo { get; set; }

        public DateTime FechaCalibracion { get; set; }

        public DateTime FechaProgramada { get; set; }

        public DateTime FechaProximaCalibracion { get; set; }

        public long IdUserRegistro { get; set; }

        public string? NombreUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long? IdUserValido { get; set; }

        public string? NombreUserValido { get; set; }

        public string? Nota { get; set; }

        public int IdCatEstatusCalibracion { get; set; }

        public string? NombreEstatusCalibracion { get; set; } 

        public long? IdTecnicoLaboratorio { get; set; }

        public string? NombreTecnicoLaboratorio { get; set; }

        public int? NumeroCertificado { get; set; }

        public DateTime? FechaEmisionCertificad { get; set; }

        public string? UrlResultado { get; set; }

        public string? Laboratorio { get; set; }

        public string? NombreLinea { get; set; }
        
        public int? IdCatEstatusEquipo { get; set; }
        public long? IdLinea { get; set; }
        public short IdRealiza 
        {
            get
            {                
                if (Id == 0)                
                    return 0;

                if (IdTecnicoLaboratorio > 0 && !string.IsNullOrEmpty(NombreTecnicoLaboratorio))
                    return 2;

                return 1;
            }
        }

        public int Total { get; set; }
    }

    public class vCalibracionResponseGrid : vCalibracionResponse
    {
        public string Acciones { get; set; }
        public string EstatusColor
        {
            get
            {
                string estatus = EstatusCalibracion.Colores[IdCatEstatusCalibracion];

                return estatus;
            } 
        }
    }
}
