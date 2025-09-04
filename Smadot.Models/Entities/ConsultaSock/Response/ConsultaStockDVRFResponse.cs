using Smadot.Models.Entities.Catalogos.Response;
using Newtonsoft.Json;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Smadot.Models.Entities.ConsultaSock.Response
{
    public class ConsultaStockDVRFResponse
    {
		public string NombreTipoCertificado { get; set; } 

		public int? CantidadStock { get; set; }
        public long? IdAlmacen { get; set; }
        public string? Almacen { get; set; }

        public int? NumeroCaja { get; set; }

		public int? FolioInicial { get; set; }

		public int? FolioFinal { get; set; }

		public string? ClaveCertificado { get; set; }

        public int? CantidadMinima { get; set; }

        public int? CantidadMedia { get; set; }

        public long? NumeroSolucitud { get; set; }

        public int Total { get; set; }

        public List<AlmacenResponse>? Almacenes { get; set; }
    }

}
