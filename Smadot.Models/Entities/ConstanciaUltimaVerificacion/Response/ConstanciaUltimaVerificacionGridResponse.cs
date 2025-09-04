using Smadot.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Models.Entities.ConstanciaUltimaVerificacion.Response
{
	public class ConstanciaUltimaVerificacionGridResponse : vConstanciaUltimaVerificacion
	{
		public string Acciones { get; set; }

		public List<vDataTramitesVentanilla> verificaciones { get; set; }

		public bool VerificacionBool { get; set; }
        public string? NombreC { get; set; }
        public string? NombreEncargado { get; set; }
        public string? UrlRoot { get; set; }
        public string? Semestre { get; set; }

    }
}
