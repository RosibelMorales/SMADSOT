using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using Smadot.Models.Entities.Exento.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Equipo.Response
{
    public class EquipoResponse
    {
        public long Id { get; set; }

        public int? IdTipoEquipo { get; set; } 
        public string? Nombre { get; set; } 

        public string? NumeroSerie { get; set; } 

        public int IdCatEstatusEquipo { get; set; }

        public long IdUserRegistro { get; set; }

        public DateTime FechaRegistro { get; set; }

        public long? IdUserReviso { get; set; }

        public string? Comentarios { get; set; }

        public long? IdLinea { get; set; }

        public long IdVerificentro { get; set; }

        public int IdCatTipoEquipo { get; set; }

        public string? FilesString { get; set; }

        public List<EquipoFiles> Files { get; set; }
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

    public class AuxEstatusResponse
    {
        public long Id { get; set; }
        public int IdCatEstatusEquipo { get; set; }
        public string? Comentarios { get; set; }
        public long? IdUserReviso { get; set; }
    }
}
