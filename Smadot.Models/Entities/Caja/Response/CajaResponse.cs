using Smadot.Models.DataBase;
using Smadot.Models.Entities.Catalogos.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.Caja.Response
{
    public class CajaResponse
    {
        public int Id { get; set; }

        public long IdIngresoFV { get; set; }

        public int IdCatTipoCertificado {get;set;}

        public int NumeroCaja { get; set; }

        public int FolioInicial { get; set; }

        public int FolioFinal { get; set; }

        public int FechaEntrega { get; set; }

        public string? NombreCatTipoCertificado { get; set; }

        public string? ClaveCertificadoCatTipoCertificado { get; set; }

        

        public List<AlmacenResponse>? Almacenes { get; set; }


        public long? IdVerificentro { get; set; }



    }
}
