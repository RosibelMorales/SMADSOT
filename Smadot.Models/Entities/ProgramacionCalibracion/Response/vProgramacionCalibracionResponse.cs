using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ProgramacionCalibracion.Response
{
    public class vProgramacionCalibracionResponse
    {
        public long? Id { get; set; }

        public long? IdEquipo { get; set; }

        public string? NombreEquipo { get; set; }

        public int? IdCatTipoCalibracion { get; set; }

        public string? NombreTipoCalibracion { get; set; }

        public string? UrlDocumento1 { get; set; }

        public DateTime? PrimeraFechaCalibracion { get; set; }

        public string? Nota { get; set; }

        public long? IdUserRegistro { get; set; }

        public string? NombreUser { get; set; }

        public string? UserName { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public long? IdUserValido { get; set; }

        public string? NombreValido { get; set; }

        public int Total { get; set; }

        public string Estatus { get; set; } = null!;
    }

    public class vProgramacionCalibracionResponseGrid : vProgramacionCalibracionResponse
    {
        public string Acciones { get; set; }

        public string EstatusParaColor
        {
            get
            {
                string estatus = string.Empty;
                if (IdUserValido != null)
                {
                    estatus = "Verde";
                }
                if(IdUserValido == null && (Nota == null || Nota == ""))
                {
                    estatus = "Gris";
                }
                if(IdUserValido == null && Nota != null)
                {
                    estatus = "Rojo";
                }
                return estatus;
            }
        }
        
    }
}

